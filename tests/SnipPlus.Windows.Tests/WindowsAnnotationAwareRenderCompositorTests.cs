using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsAnnotationAwareRenderCompositorTests
{
    [TestMethod]
    [TestCategory("Rendering")]
    [TestCategory("Contract")]
    public async Task EmptyDocumentIsPixelEquivalentToBaseFrozenFrameRenderer()
    {
        using var fixture = CreateFixture();
        var selection = fixture.Snapshot.VirtualPhysicalBounds;
        var baseRenderer = new WindowsFrozenDisplayFrameSetRenderer();
        var annotationRenderer = new WindowsAnnotationAwareRenderCompositor();

        using var baseImage = await RenderBase(baseRenderer, fixture.FrameSet, selection);
        using var annotationResult = await RenderAnnotation(
            annotationRenderer,
            fixture,
            selection,
            AnnotationDocument.CreateEmpty(fixture.Session.SessionId),
            new AnnotationRevision(0));

        using var baseLease = baseImage.AcquireBitmapLease();
        using var annotationLease = ((SoftwareBitmapImageResult)annotationResult.ImageResult).AcquireBitmapLease();
        CollectionAssert.AreEqual(
            ReadPixels(baseLease.Bitmap),
            ReadPixels(annotationLease.Bitmap));
        Assert.AreEqual(5 * 40, annotationResult.TransparentGapPixelCount);
        Assert.AreEqual(selection.Width, annotationResult.PixelWidth);
        Assert.AreEqual(selection.Height, annotationResult.PixelHeight);
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public async Task AllCommittedAnnotationKindsRenderFromVirtualPhysicalCoordinates()
    {
        using var fixture = CreateFixture();
        var objects = CreateAllAnnotationObjects(fixture.Session.SessionId);
        var document = new AnnotationDocument(
            fixture.Session.SessionId,
            new AnnotationRevision(objects.Length),
            objects);
        var renderer = new WindowsAnnotationAwareRenderCompositor();

        using var result = await RenderAnnotation(
            renderer,
            fixture,
            fixture.Snapshot.VirtualPhysicalBounds,
            document,
            document.Revision);
        using var lease = ((SoftwareBitmapImageResult)result.ImageResult).AcquireBitmapLease();
        var pixels = ReadPixels(lease.Bitmap);

        Assert.AreEqual(objects.Length, result.RenderedObjectCount);
        Assert.IsTrue(pixels.Where((value, index) => index % 4 == 3 && value != 0).Any());
        Assert.IsTrue(RegionDiffersFromBase(pixels, fixture.BasePixels, new(2, 2, 18, 30), 65));
        Assert.IsTrue(RegionDiffersFromBase(pixels, fixture.BasePixels, new(20, 15, 34, 25), 65));
        Assert.IsTrue(RegionDiffersFromBase(pixels, fixture.BasePixels, new(35, 3, 49, 17), 65));

        for (var y = 0; y < fixture.Snapshot.VirtualPhysicalBounds.Height; y++)
        {
            for (var x = 40; x < 45; x++)
            {
                Assert.AreEqual(
                    (byte)0,
                    pixels[(y * fixture.Snapshot.VirtualPhysicalBounds.Width + x) * 4 + 3],
                    $"Gap pixel ({x},{y}) must remain transparent.");
            }
        }
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public async Task SelectionProjectionClipsObjectsWithoutChangingDocumentGeometry()
    {
        using var fixture = CreateFixture();
        var objectId = new AnnotationObjectId(Guid.NewGuid());
        var annotation = new AnnotationObject(
            objectId,
            fixture.Session.SessionId,
            AnnotationToolKind.Rectangle,
            new(8, 8, 28, 28),
            0,
            new RectangleAnnotationContent(new RectangleAnnotationStyle(
                new ArgbColor(255, 255, 0, 0),
                2)));
        var document = new AnnotationDocument(
            fixture.Session.SessionId,
            new AnnotationRevision(1),
            new[] { annotation });
        var selection = new PhysicalRect(10, 10, 30, 30);
        var renderer = new WindowsAnnotationAwareRenderCompositor();

        using var result = await RenderAnnotation(
            renderer,
            fixture,
            selection,
            document,
            document.Revision);

        Assert.AreEqual(new PhysicalRect(8, 8, 28, 28), document.Objects[0].Geometry);
        Assert.AreEqual(20, result.PixelWidth);
        Assert.AreEqual(20, result.PixelHeight);
        Assert.AreEqual(1, result.RenderedObjectCount);
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public async Task InvalidAndStaleRequestsReturnTypedOutcomesWithoutThrowing()
    {
        using var fixture = CreateFixture();
        var renderer = new WindowsAnnotationAwareRenderCompositor();
        var empty = AnnotationDocument.CreateEmpty(fixture.Session.SessionId);

        var staleSession = await renderer.RenderAsync(
            CreateRequest(fixture, fixture.Snapshot.VirtualPhysicalBounds, empty, 0) with
            {
                SessionId = Guid.NewGuid()
            },
            CancellationToken.None);
        Assert.IsInstanceOfType<AnnotationAwareRenderOutcome.StaleSession>(staleSession);

        var staleCoordinate = await renderer.RenderAsync(
            CreateRequest(fixture, fixture.Snapshot.VirtualPhysicalBounds, empty, 0) with
            {
                CoordinateVersion = "old-coordinate"
            },
            CancellationToken.None);
        Assert.IsInstanceOfType<AnnotationAwareRenderOutcome.StaleCoordinateVersion>(staleCoordinate);

        var staleAnnotation = await renderer.RenderAsync(
            CreateRequest(fixture, fixture.Snapshot.VirtualPhysicalBounds, empty, 1),
            CancellationToken.None);
        Assert.IsInstanceOfType<AnnotationAwareRenderOutcome.StaleAnnotationRevision>(staleAnnotation);

        var invalidSelection = await renderer.RenderAsync(
            CreateRequest(fixture, new PhysicalRect(0, 0, 0, 2), empty, 0),
            CancellationToken.None);
        Assert.IsInstanceOfType<AnnotationAwareRenderOutcome.InvalidSelection>(invalidSelection);

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var cancelled = await renderer.RenderAsync(
            CreateRequest(fixture, fixture.Snapshot.VirtualPhysicalBounds, empty, 0),
            cancellation.Token);
        Assert.IsInstanceOfType<AnnotationAwareRenderOutcome.Cancelled>(cancelled);
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public async Task MissingDisplayFrameIsRejectedBeforePixelAllocation()
    {
        using var fixture = CreateFixture();
        var additionalDisplay = Display("missing", new(50, 0, 65, 40));
        var mismatchedSnapshot = new VirtualDesktopSnapshot(
            fixture.Snapshot.CoordinateVersion,
            fixture.Snapshot.VirtualPhysicalBounds,
            fixture.Snapshot.VirtualOrigin,
            fixture.Snapshot.Displays.Append(additionalDisplay));
        var request = CreateRequest(
            fixture,
            fixture.Snapshot.VirtualPhysicalBounds,
            AnnotationDocument.CreateEmpty(fixture.Session.SessionId),
            0) with
        {
            VirtualDesktopSnapshot = mismatchedSnapshot
        };

        var outcome = await new WindowsAnnotationAwareRenderCompositor()
            .RenderAsync(request, CancellationToken.None);

        Assert.IsInstanceOfType<AnnotationAwareRenderOutcome.InvalidFrameSet>(outcome);
    }

    private static async Task<SoftwareBitmapImageResult> RenderBase(
        WindowsFrozenDisplayFrameSetRenderer renderer,
        FrozenDisplayFrameSet frameSet,
        PhysicalRect selection)
    {
        var outcome = await renderer.RenderAsync(frameSet, selection, CancellationToken.None);
        var succeeded = outcome as FrozenDisplayFrameSetRenderOutcome.Succeeded;
        Assert.IsNotNull(succeeded);
        return (SoftwareBitmapImageResult)succeeded.ImageResult;
    }

    private static async Task<AnnotationAwareRenderResult> RenderAnnotation(
        WindowsAnnotationAwareRenderCompositor renderer,
        Fixture fixture,
        PhysicalRect selection,
        AnnotationDocument document,
        AnnotationRevision revision)
    {
        var outcome = await renderer.RenderAsync(
            CreateRequest(fixture, selection, document, checked((int)revision.Value)),
            CancellationToken.None);
        var succeeded = outcome as AnnotationAwareRenderOutcome.Succeeded;
        Assert.IsNotNull(succeeded);
        return succeeded.Result;
    }

    private static AnnotationAwareRenderRequest CreateRequest(
        Fixture fixture,
        PhysicalRect selection,
        AnnotationDocument document,
        int revision) => new()
        {
            SessionId = fixture.Session.SessionId,
            CoordinateVersion = fixture.Snapshot.CoordinateVersion,
            SelectionRevision = 3,
            AnnotationRevision = new AnnotationRevision(revision),
            SelectionPhysicalBounds = selection,
            VirtualDesktopSnapshot = fixture.Snapshot,
            CapacityValidation = CapacityValidationOutcome.Supported(),
            FrozenDisplayFrames = fixture.FrameSet,
            AnnotationDocument = document
        };

    private static AnnotationObject[] CreateAllAnnotationObjects(Guid sessionId)
    {
        var markerStyle = new NumberedMarkerAnnotationStyle(
            new ArgbColor(255, 40, 90, 200),
            12);
        return
        [
            new AnnotationObject(
                new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000001")),
                sessionId,
                AnnotationToolKind.Rectangle,
                new(2, 2, 18, 14),
                0,
                new RectangleAnnotationContent(new RectangleAnnotationStyle(
                    new ArgbColor(255, 255, 0, 0),
                    2))),
            new AnnotationObject(
                new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000002")),
                sessionId,
                AnnotationToolKind.ArrowLine,
                new(14, 4, 28, 14),
                1,
                new ArrowLineAnnotationContent(
                    new PhysicalLineSegment(new(14, 4), new(28, 14)),
                    new ArrowLineAnnotationStyle(
                        new ArgbColor(255, 255, 100, 0),
                        2,
                        ArrowLineEndStyle.Arrow))),
                new AnnotationObject(
                new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000003")),
                sessionId,
                AnnotationToolKind.HighlighterStroke,
                new(26, 2, 38, 8),
                2,
                new HighlighterStrokeContent(
                    new PhysicalPolyline(new[] { new PhysicalPoint(26, 2), new PhysicalPoint(32, 8), new PhysicalPoint(38, 4) }),
                    new HighlighterAnnotationStyle(new ArgbColor(128, 0, 255, 0), 4))),
            new AnnotationObject(
                new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000004")),
                sessionId,
                AnnotationToolKind.Text,
                new(2, 16, 18, 30),
                3,
                new TextAnnotationContent(
                    "中文🙂",
                    new(3, 17),
                    new(2, 16, 18, 30),
                    new TextAnnotationStyle(
                        TextAnnotationStyle.DefaultFontFamily,
                        12,
                        new ArgbColor(255, 255, 255, 255),
                        true))),
            new AnnotationObject(
                new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000005")),
                sessionId,
                AnnotationToolKind.PrivacyRegion,
                new(20, 15, 34, 25),
                4,
                new PrivacyRegionAnnotationContent(
                    PrivacyRegionMode.Mosaic,
                    new PrivacyRegionEffectParameters(2, 2))),
            new AnnotationObject(
                new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000006")),
                sessionId,
                AnnotationToolKind.NumberedMarker,
                NumberedMarkerAnnotationContent.GetBounds(new(42, 10), markerStyle),
                5,
                new NumberedMarkerAnnotationContent(7, markerStyle))
        ];
    }

    private static bool RegionDiffersFromBase(
        byte[] rendered,
        byte[] baseline,
        PhysicalRect region,
        int selectionWidth)
    {
        for (var y = region.Top; y < region.Bottom; y++)
        {
            for (var x = region.Left; x < region.Right; x++)
            {
                var offset = (y * selectionWidth + x) * 4;
                if (!rendered.AsSpan(offset, 4).SequenceEqual(baseline.AsSpan(offset, 4)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static Fixture CreateFixture()
    {
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        var snapshot = new VirtualDesktopSnapshot(
            "annotation-render-v1",
            new(0, 0, 65, 40),
            new(0, 0),
            new[]
            {
                Display("left", new(0, 0, 20, 40)),
                Display("primary", new(20, 0, 40, 40)),
                Display("right", new(45, 0, 65, 40))
            });
        var session = new CaptureSessionContext(
            request,
            snapshot,
            CapacityValidationOutcome.Supported(),
            null,
            CancellationToken.None);
        var frames = snapshot.Displays
            .Select(display => CreateFrame(session, display))
            .ToArray();
        Assert.IsTrue(FrozenDisplayFrameSet.TryCreate(
            session,
            snapshot.Displays,
            frames,
            out var frameSet,
            out var validation));
        Assert.IsTrue(validation.IsValid);
        Assert.IsTrue(session.TryAttachFrozenDisplayFrames(frameSet!));

        var baseRenderer = new WindowsFrozenDisplayFrameSetRenderer();
        var baseOutcome = baseRenderer.RenderAsync(
            frameSet!,
            snapshot.VirtualPhysicalBounds,
            CancellationToken.None).AsTask().GetAwaiter().GetResult();
        var baseSuccess = baseOutcome as FrozenDisplayFrameSetRenderOutcome.Succeeded;
        Assert.IsNotNull(baseSuccess);
        using var baseLease = ((SoftwareBitmapImageResult)baseSuccess.ImageResult).AcquireBitmapLease();
        return new Fixture(session, snapshot, frameSet!, ReadPixels(baseLease.Bitmap));
    }

    private static FrozenDisplayFrame CreateFrame(
        CaptureSessionContext session,
        DisplaySnapshot display)
    {
        var width = display.ExpectedFrozenFramePixelSize.Width;
        var height = display.ExpectedFrozenFramePixelSize.Height;
        var pixels = new byte[width * height * 4];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var offset = (y * width + x) * 4;
                pixels[offset] = (byte)(20 + (x + display.PhysicalBoundsInVirtualDesktop.Left) % 100);
                pixels[offset + 1] = (byte)(30 + y % 100);
                pixels[offset + 2] = (byte)(40 + (x + y) % 100);
                pixels[offset + 3] = 255;
            }
        }

        var metadata = new ImageResultMetadata
        {
            ResultId = Guid.NewGuid(),
            SessionId = session.SessionId,
            PixelWidth = width,
            PixelHeight = height,
            PixelFormat = ImagePixelFormat.Bgra8,
            AlphaMode = ImageAlphaMode.Premultiplied,
            ColorSpace = ImageColorSpace.SrgbSdr,
            DpiX = 96,
            DpiY = 96,
            RowStride = width * 4,
            SourceKind = SourceKind.Monitor,
            SourcePhysicalBounds = display.PhysicalBoundsInVirtualDesktop,
            CropPhysicalBounds = display.PhysicalBoundsInVirtualDesktop,
            CapturedAt = DateTimeOffset.UnixEpoch
        };
        return new FrozenDisplayFrame(
            session.SessionId,
            display.DisplayId,
            Guid.NewGuid(),
            session.VirtualDesktopSnapshot.CoordinateVersion,
            display.PhysicalBoundsInVirtualDesktop,
            display.ExpectedFrozenFramePixelSize,
            new FrozenCaptureFrame(SoftwareBitmapFactory.CreateFromPremultipliedBgra(pixels, metadata)));
    }

    private static DisplaySnapshot Display(string id, PhysicalRect bounds) => new(
        id,
        bounds,
        1,
        1,
        "Landscape",
        new(bounds.Width, bounds.Height),
        $"surface:{id}");

    private static byte[] ReadPixels(SoftwareBitmap bitmap)
    {
        var buffer = new global::Windows.Storage.Streams.Buffer((uint)(bitmap.PixelWidth * bitmap.PixelHeight * 4));
        bitmap.CopyToBuffer(buffer);
        var pixels = new byte[(int)buffer.Length];
        using var reader = DataReader.FromBuffer(buffer);
        reader.ReadBytes(pixels);
        return pixels;
    }

    private sealed record Fixture(
        CaptureSessionContext Session,
        VirtualDesktopSnapshot Snapshot,
        FrozenDisplayFrameSet FrameSet,
        byte[] BasePixels) : IDisposable
    {
        public void Dispose() => Session.Dispose();
    }
}
