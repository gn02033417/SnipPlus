using SnipPlus.Contracts;

namespace SnipPlus.Windows;

public sealed class WindowsFrozenDisplayFrameSetRenderer : IFrozenDisplayFrameSetRenderer
{
    public ValueTask<FrozenDisplayFrameSetRenderOutcome> RenderAsync(
        FrozenDisplayFrameSet frameSet,
        PhysicalRect selectionPhysicalBounds,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(frameSet);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (frameSet.IsDisposed)
            {
                return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                    frameSet.SessionId,
                    FailureCode.InvalidResultLifetime,
                    "The frozen display frame set has already been disposed."));
            }

            if (!selectionPhysicalBounds.IsPositive
                || selectionPhysicalBounds.Width64 > SupportedCapacityPolicy.MaxSelectionWidth
                || selectionPhysicalBounds.Height64 > SupportedCapacityPolicy.MaxSelectionHeight
                || checked(selectionPhysicalBounds.Width64 * selectionPhysicalBounds.Height64)
                    > SupportedCapacityPolicy.MaxSelectionArea)
            {
                return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                    frameSet.SessionId,
                    FailureCode.InvalidSelection,
                    "The Selection bounds are empty or exceed the supported canonical image size."));
            }

            var width = checked((int)selectionPhysicalBounds.Width64);
            var height = checked((int)selectionPhysicalBounds.Height64);
            var destinationStride = checked(width * 4);
            var destination = new byte[checked(destinationStride * height)];
            var copiedAnyPixels = false;
            DateTimeOffset capturedAt = DateTimeOffset.UtcNow;

            foreach (var frame in frameSet.Frames.Values)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (frame.IsDisposed)
                {
                    return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                        frameSet.SessionId,
                        FailureCode.InvalidResultLifetime,
                        "A frozen display frame has already been disposed."));
                }

                var frameBounds = frame.PhysicalBoundsInVirtualDesktop;
                var intersection = frameBounds.Intersection(selectionPhysicalBounds);
                if (!intersection.IsPositive)
                {
                    continue;
                }

                if (frame.PixelSize.Width != frameBounds.Width64
                    || frame.PixelSize.Height != frameBounds.Height64
                    || frame.FrozenFrame.ImageResult is not SoftwareBitmapImageResult imageResult)
                {
                    return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                        frameSet.SessionId,
                        FailureCode.InvalidResultLifetime,
                        "A frozen display frame is not a canonical SoftwareBitmap with matching physical dimensions."));
                }

                using var lease = imageResult.AcquireBitmapLease();
                var source = SoftwareBitmapBuffer.Read(lease.Bitmap);
                var sourceStride = checked(frame.PixelSize.Width * 4);
                var sourceX = checked(intersection.Left - frameBounds.Left);
                var sourceY = checked(intersection.Top - frameBounds.Top);
                var destinationX = checked(intersection.Left - selectionPhysicalBounds.Left);
                var destinationY = checked(intersection.Top - selectionPhysicalBounds.Top);
                var rowBytes = checked(intersection.Width * 4);

                for (var row = 0; row < intersection.Height; row++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var sourceOffset = checked((sourceY + row) * sourceStride + sourceX * 4);
                    var destinationOffset = checked((destinationY + row) * destinationStride + destinationX * 4);
                    Buffer.BlockCopy(source, sourceOffset, destination, destinationOffset, rowBytes);
                }

                copiedAnyPixels = true;
                capturedAt = imageResult.Metadata.CapturedAt;
            }

            if (!copiedAnyPixels)
            {
                return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                    frameSet.SessionId,
                    FailureCode.InvalidSelection,
                    "The Selection does not intersect a frozen display."));
            }

            var metadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = frameSet.SessionId,
                PixelWidth = width,
                PixelHeight = height,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = 96,
                DpiY = 96,
                RowStride = destinationStride,
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = selectionPhysicalBounds,
                CropPhysicalBounds = selectionPhysicalBounds,
                CapturedAt = capturedAt,
                CursorIncluded = false
            };

            return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(
                new FrozenDisplayFrameSetRenderOutcome.Succeeded(
                    SoftwareBitmapFactory.CreateFromPremultipliedBgra(destination, metadata)));
        }
        catch (OperationCanceledException)
        {
            return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(
                new FrozenDisplayFrameSetRenderOutcome.Cancelled("CancellationToken"));
        }
        catch (OverflowException exception)
        {
            return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                frameSet.SessionId,
                FailureCode.RenderingFailed,
                exception.GetType().Name,
                exception.HResult));
        }
        catch (Exception exception)
        {
            return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                frameSet.SessionId,
                FailureCode.RenderingFailed,
                exception.GetType().Name,
                exception.HResult));
        }
    }

    private static FrozenDisplayFrameSetRenderOutcome.Failed Failed(
        Guid sessionId,
        FailureCode code,
        string message,
        int? nativeCode = null) => new(Failure.Create(
        code,
        FailureCategory.Resource,
        FailureRecoverability.RetrySameIntent,
        nameof(WindowsFrozenDisplayFrameSetRenderer),
        sessionId,
        message,
        nativeCode: nativeCode));
}
