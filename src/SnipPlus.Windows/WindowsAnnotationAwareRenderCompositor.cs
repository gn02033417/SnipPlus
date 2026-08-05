using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using SnipPlus.Contracts;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI.Text;

namespace SnipPlus.Windows;

public sealed class WindowsAnnotationAwareRenderCompositor : IAnnotationAwareRenderAdapter
{
    public ValueTask<AnnotationAwareRenderOutcome> RenderAsync(
        AnnotationAwareRenderRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                request.Cancellation,
                cancellationToken);
            var token = linkedCancellation.Token;
            if (token.IsCancellationRequested)
            {
                return Result(new AnnotationAwareRenderOutcome.Cancelled(
                    request.SessionId,
                    "CancellationToken"));
            }

            var validation = ValidateRequest(request);
            if (validation is not null)
            {
                return Result(validation);
            }

            var composition = WindowsFrozenDisplayCompositor.Compose(
                request.FrozenDisplayFrames,
                request.SelectionPhysicalBounds,
                request.VirtualDesktopSnapshot.Displays.Select(display => display.DisplayId),
                token);

            ApplyPrivacyRegions(composition.Pixels, composition, request, token);

            using var device = CanvasDevice.GetSharedDevice();
            foreach (var annotationObject in OrderedObjects(request.AnnotationDocument))
            {
                token.ThrowIfCancellationRequested();
                DrawAnnotation(
                    composition.Pixels,
                    composition,
                    device,
                    annotationObject,
                    request,
                    token);
            }

            token.ThrowIfCancellationRequested();
            var metadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = request.SessionId,
                PixelWidth = composition.PixelWidth,
                PixelHeight = composition.PixelHeight,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = 96,
                DpiY = 96,
                RowStride = composition.RowStride,
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = request.SelectionPhysicalBounds,
                CropPhysicalBounds = request.SelectionPhysicalBounds,
                CapturedAt = composition.CapturedAt,
                CursorIncluded = false
            };
            var imageResult = SoftwareBitmapFactory.CreateFromPremultipliedBgra(
                composition.Pixels,
                metadata);
            var result = new AnnotationAwareRenderResult(
                metadata.ResultId,
                request.SessionId,
                request.SelectionRevision,
                request.AnnotationRevision,
                imageResult,
                request.AnnotationDocument.Objects.Count,
                composition.TransparentGapPixelCount);

            return Result(new AnnotationAwareRenderOutcome.Succeeded(result));
        }
        catch (OperationCanceledException)
        {
            return Result(new AnnotationAwareRenderOutcome.Cancelled(
                request.SessionId,
                "CancellationToken"));
        }
        catch (UnsupportedAnnotationException exception)
        {
            return Result(new AnnotationAwareRenderOutcome.UnsupportedAnnotation(
                request.SessionId,
                exception.ToolKind,
                exception.Message));
        }
        catch (Exception exception)
        {
            return Result(new AnnotationAwareRenderOutcome.Failed(
                request.SessionId,
                Failure.Create(
                    FailureCode.RenderingFailed,
                    FailureCategory.Resource,
                    FailureRecoverability.RetrySameIntent,
                    nameof(WindowsAnnotationAwareRenderCompositor),
                    request.SessionId,
                    exception.GetType().Name,
                    nativeCode: exception.HResult)));
        }
    }

    private static AnnotationAwareRenderOutcome? ValidateRequest(
        AnnotationAwareRenderRequest request)
    {
        if (request.SessionId == Guid.Empty
            || request.FrozenDisplayFrames is null
            || request.VirtualDesktopSnapshot is null
            || request.CapacityValidation is null
            || request.AnnotationDocument is null
            || string.IsNullOrWhiteSpace(request.CoordinateVersion))
        {
            return new AnnotationAwareRenderOutcome.InvalidFrameSet(
                request.SessionId,
                "The render request is missing required session metadata.");
        }

        if (request.Cancellation.IsCancellationRequested)
        {
            return new AnnotationAwareRenderOutcome.Cancelled(
                request.SessionId,
                "RequestCancellation");
        }

        if (!request.CapacityValidation.IsSupported)
        {
            return new AnnotationAwareRenderOutcome.RenderCapacityExceeded(
                request.SessionId,
                request.CapacityValidation.UserMessage);
        }

        var capacity = new SupportedCapacityPolicy();
        var topology = capacity.ValidateTopology(request.VirtualDesktopSnapshot);
        if (!topology.IsSupported)
        {
            return new AnnotationAwareRenderOutcome.RenderCapacityExceeded(
                request.SessionId,
                topology.UserMessage);
        }

        if (!request.SelectionPhysicalBounds.IsPositive)
        {
            return new AnnotationAwareRenderOutcome.InvalidSelection(
                request.SessionId,
                "The Selection bounds must have positive dimensions.");
        }

        var selectionCapacity = capacity.ValidateSelection(request.SelectionPhysicalBounds);
        if (!selectionCapacity.IsSupported)
        {
            return new AnnotationAwareRenderOutcome.RenderCapacityExceeded(
                request.SessionId,
                selectionCapacity.UserMessage);
        }

        if (!request.VirtualDesktopSnapshot.VirtualPhysicalBounds.Contains(
                request.SelectionPhysicalBounds))
        {
            return new AnnotationAwareRenderOutcome.InvalidSelection(
                request.SessionId,
                "The Selection must be contained by the Virtual Desktop snapshot.");
        }

        if (!string.Equals(
                request.CoordinateVersion,
                request.VirtualDesktopSnapshot.CoordinateVersion,
                StringComparison.Ordinal))
        {
            return new AnnotationAwareRenderOutcome.StaleCoordinateVersion(
                request.SessionId,
                request.CoordinateVersion,
                request.VirtualDesktopSnapshot.CoordinateVersion,
                "The render request uses a stale coordinate snapshot.");
        }

        if (request.FrozenDisplayFrames.IsDisposed
            || request.FrozenDisplayFrames.SessionId != request.SessionId)
        {
            return new AnnotationAwareRenderOutcome.StaleSession(
                request.SessionId,
                request.FrozenDisplayFrames.SessionId,
                "The frozen frame set belongs to another or disposed session.");
        }

        if (!string.Equals(
                request.FrozenDisplayFrames.CoordinateVersion,
                request.CoordinateVersion,
                StringComparison.Ordinal))
        {
            return new AnnotationAwareRenderOutcome.StaleCoordinateVersion(
                request.SessionId,
                request.CoordinateVersion,
                request.FrozenDisplayFrames.CoordinateVersion,
                "The frozen frame set uses a stale coordinate snapshot.");
        }

        var frameValidation = ValidateFrameSet(request);
        if (frameValidation is not null)
        {
            return frameValidation;
        }

        if (request.AnnotationDocument.SessionId != request.SessionId)
        {
            return new AnnotationAwareRenderOutcome.InvalidAnnotationDocument(
                request.SessionId,
                "The Annotation Document belongs to another capture session.");
        }

        if (request.AnnotationDocument.Revision != request.AnnotationRevision)
        {
            return new AnnotationAwareRenderOutcome.StaleAnnotationRevision(
                request.SessionId,
                request.AnnotationRevision,
                request.AnnotationDocument.Revision,
                "The render request uses a stale Annotation revision.");
        }

        if (request.SelectionRevision < 0)
        {
            return new AnnotationAwareRenderOutcome.StaleSelectionRevision(
                request.SessionId,
                request.SelectionRevision,
                0,
                "The Selection revision is invalid.");
        }

        return null;
    }

    private static AnnotationAwareRenderOutcome.InvalidFrameSet? ValidateFrameSet(
        AnnotationAwareRenderRequest request)
    {
        var expected = request.VirtualDesktopSnapshot.Displays;
        var frames = request.FrozenDisplayFrames.Frames;
        if (frames.Count != expected.Count)
        {
            return new AnnotationAwareRenderOutcome.InvalidFrameSet(
                request.SessionId,
                "The frozen frame set does not contain exactly one frame per display.");
        }

        if (frames.Values.Select(frame => frame.FrameId).Distinct().Count() != frames.Count)
        {
            return new AnnotationAwareRenderOutcome.InvalidFrameSet(
                request.SessionId,
                "The frozen frame set contains duplicate frame identities.");
        }

        foreach (var display in expected)
        {
            if (!frames.TryGetValue(display.DisplayId, out var frame)
                || frame.IsDisposed
                || frame.SessionId != request.SessionId
                || !string.Equals(
                    frame.CoordinateVersion,
                    request.CoordinateVersion,
                    StringComparison.Ordinal)
                || frame.PhysicalBoundsInVirtualDesktop != display.PhysicalBoundsInVirtualDesktop
                || frame.PixelSize != display.ExpectedFrozenFramePixelSize
                || frame.FrozenFrame.ImageResult is not SoftwareBitmapImageResult imageResult
                || imageResult.IsDisposed
                || imageResult.Metadata.SessionId != request.SessionId
                || imageResult.Metadata.PixelWidth != frame.PixelSize.Width
                || imageResult.Metadata.PixelHeight != frame.PixelSize.Height
                || imageResult.Metadata.PixelFormat != ImagePixelFormat.Bgra8
                || imageResult.Metadata.AlphaMode != ImageAlphaMode.Premultiplied
                || imageResult.Metadata.ColorSpace != ImageColorSpace.SrgbSdr
                || imageResult.Metadata.SourcePhysicalBounds != frame.PhysicalBoundsInVirtualDesktop)
            {
                return new AnnotationAwareRenderOutcome.InvalidFrameSet(
                    request.SessionId,
                    "A frozen display frame is missing, foreign or not canonical BGRA8 premultiplied data.");
            }
        }

        return frames.Keys.Any(frameId => expected.All(display => display.DisplayId != frameId))
            ? new AnnotationAwareRenderOutcome.InvalidFrameSet(
                request.SessionId,
                "The frozen frame set contains a foreign display frame.")
            : null;
    }

    private static IEnumerable<AnnotationObject> OrderedObjects(AnnotationDocument document)
    {
        // Stage 7F preview establishes privacy as a frozen-source effect layer.
        // Vector objects then use Z-order and ObjectId as their deterministic tie-breaker.
        return document.Objects
            .Where(annotationObject => annotationObject.ToolKind != AnnotationToolKind.PrivacyRegion)
            .OrderBy(annotationObject => annotationObject.ZOrder)
            .ThenBy(annotationObject => annotationObject.ObjectId.Value);
    }

    private static void ApplyPrivacyRegions(
        byte[] destination,
        FrozenDisplayComposition composition,
        AnnotationAwareRenderRequest request,
        CancellationToken cancellationToken)
    {
        foreach (var annotationObject in request.AnnotationDocument.Objects
                     .Where(annotationObject => annotationObject.ToolKind == AnnotationToolKind.PrivacyRegion)
                     .OrderBy(annotationObject => annotationObject.ZOrder)
                     .ThenBy(annotationObject => annotationObject.ObjectId.Value))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (annotationObject.Content is not PrivacyRegionAnnotationContent content)
            {
                throw new UnsupportedAnnotationException(
                    annotationObject.ToolKind,
                    "A Privacy Region object does not contain supported effect content.");
            }

            foreach (var display in request.VirtualDesktopSnapshot.Displays)
            {
                var visible = annotationObject.Geometry
                    .Intersection(request.SelectionPhysicalBounds)
                    .Intersection(display.PhysicalBoundsInVirtualDesktop);
                if (!visible.IsPositive
                    || !request.FrozenDisplayFrames.Frames.TryGetValue(display.DisplayId, out var frame)
                    || frame.FrozenFrame.ImageResult is not SoftwareBitmapImageResult source)
                {
                    continue;
                }

                using var effect = FrozenPrivacyEffectRenderer.Render(
                    source,
                    display.PhysicalBoundsInVirtualDesktop,
                    visible,
                    content);
                using var lease = effect.AcquireBitmapLease();
                var effectPixels = SoftwareBitmapBuffer.Read(lease.Bitmap);
                CopyToSelection(
                    destination,
                    composition,
                    visible,
                    request.SelectionPhysicalBounds,
                    effectPixels,
                    cancellationToken);
            }
        }
    }

    private static void DrawAnnotation(
        byte[] destination,
        FrozenDisplayComposition composition,
        CanvasDevice device,
        AnnotationObject annotationObject,
        AnnotationAwareRenderRequest request,
        CancellationToken cancellationToken)
    {
        switch (annotationObject.ToolKind)
        {
            case AnnotationToolKind.Rectangle when annotationObject.Content is RectangleAnnotationContent rectangle:
                DrawRectangle(destination, composition, annotationObject.Geometry, rectangle.Style, request);
                break;
            case AnnotationToolKind.ArrowLine when annotationObject.Content is ArrowLineAnnotationContent arrow:
                DrawArrowLine(destination, composition, arrow, request);
                break;
            case AnnotationToolKind.HighlighterStroke when annotationObject.Content is HighlighterStrokeContent highlighter:
                DrawHighlighter(destination, composition, highlighter, request);
                break;
            case AnnotationToolKind.Text when annotationObject.Content is TextAnnotationContent text:
                DrawText(destination, composition, device, text, request, cancellationToken);
                break;
            case AnnotationToolKind.NumberedMarker when annotationObject.Content is NumberedMarkerAnnotationContent marker:
                DrawMarker(destination, composition, device, annotationObject.Geometry, marker, request, cancellationToken);
                break;
            case AnnotationToolKind.PrivacyRegion:
                break;
            default:
                throw new UnsupportedAnnotationException(
                    annotationObject.ToolKind,
                    "The Annotation object content is not supported by the compositor.");
        }
    }

    private static void DrawRectangle(
        byte[] destination,
        FrozenDisplayComposition composition,
        PhysicalRect geometry,
        RectangleAnnotationStyle style,
        AnnotationAwareRenderRequest request)
    {
        var thickness = style.StrokeThickness;
        var bounds = geometry.Intersection(request.SelectionPhysicalBounds);
        for (var y = bounds.Top; y < bounds.Bottom; y++)
        {
            for (var x = bounds.Left; x < bounds.Right; x++)
            {
                if (x < geometry.Left + thickness
                    || x >= geometry.Right - thickness
                    || y < geometry.Top + thickness
                    || y >= geometry.Bottom - thickness)
                {
                    BlendPixel(destination, composition, request, x, y, style.StrokeColor);
                }
            }
        }
    }

    private static void DrawArrowLine(
        byte[] destination,
        FrozenDisplayComposition composition,
        ArrowLineAnnotationContent content,
        AnnotationAwareRenderRequest request)
    {
        DrawSegment(
            destination,
            composition,
            request,
            content.Segment.Start,
            content.Segment.End,
            content.Style.StrokeThickness,
            content.Style.StrokeColor);
        if (content.Style.EndStyle != ArrowLineEndStyle.Arrow)
        {
            return;
        }

        var dx = content.Segment.End.X - content.Segment.Start.X;
        var dy = content.Segment.End.Y - content.Segment.Start.Y;
        var length = Math.Sqrt((double)dx * dx + (double)dy * dy);
        if (length <= double.Epsilon)
        {
            return;
        }

        var headLength = Math.Max(6, content.Style.StrokeThickness * 3);
        var angle = Math.Atan2(dy, dx);
        var left = new PhysicalPoint(
            checked((int)Math.Round(content.Segment.End.X - headLength * Math.Cos(angle - Math.PI / 6))),
            checked((int)Math.Round(content.Segment.End.Y - headLength * Math.Sin(angle - Math.PI / 6))));
        var right = new PhysicalPoint(
            checked((int)Math.Round(content.Segment.End.X - headLength * Math.Cos(angle + Math.PI / 6))),
            checked((int)Math.Round(content.Segment.End.Y - headLength * Math.Sin(angle + Math.PI / 6))));
        DrawSegment(destination, composition, request, content.Segment.End, left, content.Style.StrokeThickness, content.Style.StrokeColor);
        DrawSegment(destination, composition, request, content.Segment.End, right, content.Style.StrokeThickness, content.Style.StrokeColor);
    }

    private static void DrawHighlighter(
        byte[] destination,
        FrozenDisplayComposition composition,
        HighlighterStrokeContent content,
        AnnotationAwareRenderRequest request)
    {
        var points = content.Path.Points;
        for (var index = 1; index < points.Count; index++)
        {
            DrawSegment(
                destination,
                composition,
                request,
                points[index - 1],
                points[index],
                content.Style.StrokeThickness,
                content.Style.StrokeColor);
        }
    }

    private static void DrawSegment(
        byte[] destination,
        FrozenDisplayComposition composition,
        AnnotationAwareRenderRequest request,
        PhysicalPoint start,
        PhysicalPoint end,
        int thickness,
        ArgbColor color)
    {
        var radius = thickness / 2d;
        var bounds = new PhysicalRect(
            Math.Min(start.X, end.X) - thickness,
            Math.Min(start.Y, end.Y) - thickness,
            Math.Max(start.X, end.X) + thickness + 1,
            Math.Max(start.Y, end.Y) + thickness + 1)
            .Intersection(request.SelectionPhysicalBounds);
        var radiusSquared = radius * radius;
        var dx = (double)end.X - start.X;
        var dy = (double)end.Y - start.Y;
        var lengthSquared = dx * dx + dy * dy;
        for (var y = bounds.Top; y < bounds.Bottom; y++)
        {
            for (var x = bounds.Left; x < bounds.Right; x++)
            {
                var px = x + 0.5 - start.X;
                var py = y + 0.5 - start.Y;
                var projection = lengthSquared <= double.Epsilon
                    ? 0
                    : Math.Clamp((px * dx + py * dy) / lengthSquared, 0, 1);
                var distanceX = px - projection * dx;
                var distanceY = py - projection * dy;
                if (distanceX * distanceX + distanceY * distanceY <= radiusSquared)
                {
                    BlendPixel(destination, composition, request, x, y, color);
                }
            }
        }
    }

    private static void DrawText(
        byte[] destination,
        FrozenDisplayComposition composition,
        CanvasDevice device,
        TextAnnotationContent content,
        AnnotationAwareRenderRequest request,
        CancellationToken cancellationToken)
    {
        using var format = new CanvasTextFormat
        {
            FontFamily = content.Style.FontFamily,
            FontSize = (float)content.Style.FontSize,
            FontWeight = content.Style.Bold ? new FontWeight { Weight = 700 } : new FontWeight { Weight = 400 }
        };
        using var layout = new CanvasTextLayout(
            device,
            content.Text,
            format,
            content.BoundsInVirtualDesktop.Width,
            content.BoundsInVirtualDesktop.Height);
        DrawTextLayoutOnVisibleDisplays(
            destination,
            composition,
            device,
            layout,
            content.AnchorInVirtualDesktop,
            content.BoundsInVirtualDesktop,
            content.Style.Color,
            request,
            cancellationToken);
    }

    private static void DrawMarker(
        byte[] destination,
        FrozenDisplayComposition composition,
        CanvasDevice device,
        PhysicalRect geometry,
        NumberedMarkerAnnotationContent content,
        AnnotationAwareRenderRequest request,
        CancellationToken cancellationToken)
    {
        var center = new PhysicalPoint(
            geometry.Left + geometry.Width / 2,
            geometry.Top + geometry.Height / 2);
        var bounds = geometry.Intersection(request.SelectionPhysicalBounds);
        for (var y = bounds.Top; y < bounds.Bottom; y++)
        {
            for (var x = bounds.Left; x < bounds.Right; x++)
            {
                var dx = x + 0.5 - center.X;
                var dy = y + 0.5 - center.Y;
                var radius = content.Style.Size / 2d;
                if (dx * dx + dy * dy <= radius * radius)
                {
                    BlendPixel(destination, composition, request, x, y, content.Style.Color);
                }
            }
        }

        using var format = new CanvasTextFormat
        {
            FontFamily = TextAnnotationStyle.DefaultFontFamily,
            FontSize = Math.Max(8, content.Style.Size * 0.45f),
            FontWeight = new FontWeight { Weight = 700 },
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center
        };
        using var layout = new CanvasTextLayout(
            device,
            content.Number.ToString(System.Globalization.CultureInfo.InvariantCulture),
            format,
            content.Style.Size,
            content.Style.Size);
        DrawTextLayoutOnVisibleDisplays(
            destination,
            composition,
            device,
            layout,
            new PhysicalPoint(center.X - content.Style.Size / 2, center.Y - content.Style.Size / 2),
            geometry,
            new ArgbColor(255, 255, 255, 255),
            request,
            cancellationToken);
    }

    private static void DrawTextLayoutOnVisibleDisplays(
        byte[] destination,
        FrozenDisplayComposition composition,
        CanvasDevice device,
        CanvasTextLayout layout,
        PhysicalPoint origin,
        PhysicalRect geometry,
        ArgbColor color,
        AnnotationAwareRenderRequest request,
        CancellationToken cancellationToken)
    {
        var visibleSelection = geometry.Intersection(request.SelectionPhysicalBounds);
        if (!visibleSelection.IsPositive)
        {
            return;
        }

        foreach (var display in request.VirtualDesktopSnapshot.Displays)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var visible = visibleSelection.Intersection(display.PhysicalBoundsInVirtualDesktop);
            if (!visible.IsPositive)
            {
                continue;
            }

            using var textTarget = new CanvasRenderTarget(
                device,
                visible.Width,
                visible.Height,
                96,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                CanvasAlphaMode.Premultiplied);
            using (var drawingSession = textTarget.CreateDrawingSession())
            {
                drawingSession.Clear(global::Windows.UI.Color.FromArgb(0, 0, 0, 0));
                drawingSession.DrawTextLayout(
                    layout,
                    new Vector2(origin.X - visible.Left, origin.Y - visible.Top),
                    ToColor(color));
            }

            CopyAlphaBlended(
                destination,
                composition,
                request.SelectionPhysicalBounds,
                visible,
                textTarget.GetPixelBytes(),
                cancellationToken);
        }
    }

    private static void BlendPixel(
        byte[] destination,
        FrozenDisplayComposition composition,
        AnnotationAwareRenderRequest request,
        int globalX,
        int globalY,
        ArgbColor color)
    {
        if (!IsDisplayPixel(request.VirtualDesktopSnapshot, globalX, globalY))
        {
            return;
        }

        var localX = globalX - request.SelectionPhysicalBounds.Left;
        var localY = globalY - request.SelectionPhysicalBounds.Top;
        var offset = checked(localY * composition.RowStride + localX * 4);
        BlendPremultiplied(destination, offset, color);
    }

    private static void CopyAlphaBlended(
        byte[] destination,
        FrozenDisplayComposition composition,
        PhysicalRect selection,
        PhysicalRect visible,
        byte[] source,
        CancellationToken cancellationToken)
    {
        for (var y = 0; y < visible.Height; y++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            for (var x = 0; x < visible.Width; x++)
            {
                var sourceOffset = checked((y * visible.Width + x) * 4);
                var destinationOffset = checked(
                    (visible.Top - selection.Top + y) * composition.RowStride
                    + (visible.Left - selection.Left + x) * 4);
                var alpha = source[sourceOffset + 3];
                if (alpha == 0)
                {
                    continue;
                }

                BlendPremultipliedChannels(
                    destination,
                    destinationOffset,
                    source[sourceOffset],
                    source[sourceOffset + 1],
                    source[sourceOffset + 2],
                    alpha);
            }
        }
    }

    private static void CopyToSelection(
        byte[] destination,
        FrozenDisplayComposition composition,
        PhysicalRect bounds,
        PhysicalRect selection,
        byte[] source,
        CancellationToken cancellationToken)
    {
        var rowBytes = checked(bounds.Width * 4);
        var destinationX = checked(bounds.Left - selection.Left);
        var destinationY = checked(bounds.Top - selection.Top);
        for (var row = 0; row < bounds.Height; row++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Buffer.BlockCopy(
                source,
                row * rowBytes,
                destination,
                checked((destinationY + row) * composition.RowStride + destinationX * 4),
                rowBytes);
        }
    }

    private static bool IsDisplayPixel(
        VirtualDesktopSnapshot snapshot,
        int x,
        int y) => snapshot.Displays.Any(display =>
        display.PhysicalBoundsInVirtualDesktop.Left <= x
        && x < display.PhysicalBoundsInVirtualDesktop.Right
        && display.PhysicalBoundsInVirtualDesktop.Top <= y
        && y < display.PhysicalBoundsInVirtualDesktop.Bottom);

    private static void BlendPremultiplied(byte[] destination, int offset, ArgbColor source)
    {
        BlendPremultipliedChannels(
            destination,
            offset,
            source.B * source.A / 255,
            source.G * source.A / 255,
            source.R * source.A / 255,
            source.A);
    }

    private static void BlendPremultipliedChannels(
        byte[] destination,
        int offset,
        int sourceB,
        int sourceG,
        int sourceR,
        int sourceA)
    {
        var inverseAlpha = 255 - sourceA;
        destination[offset] = (byte)Math.Min(255, sourceB + destination[offset] * inverseAlpha / 255);
        destination[offset + 1] = (byte)Math.Min(255, sourceG + destination[offset + 1] * inverseAlpha / 255);
        destination[offset + 2] = (byte)Math.Min(255, sourceR + destination[offset + 2] * inverseAlpha / 255);
        destination[offset + 3] = (byte)Math.Min(255, sourceA + destination[offset + 3] * inverseAlpha / 255);
    }

    private static Rect ToLocalRect(PhysicalRect bounds, PhysicalRect selection) => new(
        bounds.Left - selection.Left,
        bounds.Top - selection.Top,
        bounds.Width,
        bounds.Height);

    private static Vector2 ToLocalPoint(PhysicalPoint point, PhysicalRect selection) => new(
        point.X - selection.Left,
        point.Y - selection.Top);

    private static global::Windows.UI.Color ToColor(ArgbColor color) => new()
    {
        A = color.A,
        R = color.R,
        G = color.G,
        B = color.B
    };

    private static ValueTask<AnnotationAwareRenderOutcome> Result(
        AnnotationAwareRenderOutcome outcome) => ValueTask.FromResult(outcome);

    private sealed class UnsupportedAnnotationException : Exception
    {
        public UnsupportedAnnotationException(AnnotationToolKind toolKind, string message)
            : base(message)
        {
            ToolKind = toolKind;
        }

        public AnnotationToolKind ToolKind { get; }
    }
}
