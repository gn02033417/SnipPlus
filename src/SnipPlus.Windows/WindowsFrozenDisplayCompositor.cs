using SnipPlus.Contracts;

namespace SnipPlus.Windows;

internal sealed record FrozenDisplayComposition(
    byte[] Pixels,
    int PixelWidth,
    int PixelHeight,
    int RowStride,
    DateTimeOffset CapturedAt,
    int TransparentGapPixelCount);

internal static class WindowsFrozenDisplayCompositor
{
    public static FrozenDisplayComposition Compose(
        FrozenDisplayFrameSet frameSet,
        PhysicalRect selectionPhysicalBounds,
        IEnumerable<string> displayOrder,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(frameSet);
        ArgumentNullException.ThrowIfNull(displayOrder);

        ObjectDisposedException.ThrowIf(frameSet.IsDisposed, frameSet);
        ObjectDisposedException.ThrowIf(
            frameSet.Frames.Values.Any(frame => frame.IsDisposed),
            frameSet);

        var width = checked((int)selectionPhysicalBounds.Width64);
        var height = checked((int)selectionPhysicalBounds.Height64);
        var destinationStride = checked(width * 4);
        var destination = new byte[checked(destinationStride * height)];
        var copiedAnyPixels = false;
        var capturedAt = DateTimeOffset.UtcNow;

        foreach (var displayId in displayOrder)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!frameSet.Frames.TryGetValue(displayId, out var frame)
                || frame.IsDisposed)
            {
                continue;
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
                throw new InvalidOperationException(
                    "A frozen display frame is not a canonical SoftwareBitmap with matching physical dimensions.");
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
            throw new InvalidOperationException("The Selection does not intersect a frozen display.");
        }

        var transparentGapPixelCount = CountTransparentGapPixels(
            frameSet,
            selectionPhysicalBounds,
            width,
            height,
            cancellationToken);

        return new FrozenDisplayComposition(
            destination,
            width,
            height,
            destinationStride,
            capturedAt,
            transparentGapPixelCount);
    }

    private static int CountTransparentGapPixels(
        FrozenDisplayFrameSet frameSet,
        PhysicalRect selectionPhysicalBounds,
        int width,
        int height,
        CancellationToken cancellationToken)
    {
        var gapPixels = 0;
        for (var y = 0; y < height; y++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var globalY = checked(selectionPhysicalBounds.Top + y);
            for (var x = 0; x < width; x++)
            {
                var globalX = checked(selectionPhysicalBounds.Left + x);
                if (!frameSet.Frames.Values.Any(frame =>
                    !frame.IsDisposed
                    && frame.PhysicalBoundsInVirtualDesktop.Left <= globalX
                    && globalX < frame.PhysicalBoundsInVirtualDesktop.Right
                    && frame.PhysicalBoundsInVirtualDesktop.Top <= globalY
                    && globalY < frame.PhysicalBoundsInVirtualDesktop.Bottom))
                {
                    gapPixels = checked(gapPixels + 1);
                }
            }
        }

        return gapPixels;
    }
}
