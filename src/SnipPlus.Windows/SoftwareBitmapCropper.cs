using SnipPlus.Contracts;

namespace SnipPlus.Windows;

public static class SoftwareBitmapCropper
{
    public static SoftwareBitmapImageResult Crop(
        SoftwareBitmapImageResult source,
        PhysicalRect cropBoundsInSource,
        Guid resultId,
        DateTimeOffset capturedAt)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (!cropBoundsInSource.IsPositive
            || cropBoundsInSource.Left < 0
            || cropBoundsInSource.Top < 0
            || cropBoundsInSource.Right > source.Metadata.PixelWidth
            || cropBoundsInSource.Bottom > source.Metadata.PixelHeight)
        {
            throw new ArgumentOutOfRangeException(nameof(cropBoundsInSource));
        }

        using var lease = source.AcquireBitmapLease();
        var sourceBytes = SoftwareBitmapBuffer.Read(lease.Bitmap);
        var cropWidth = cropBoundsInSource.Width;
        var cropHeight = cropBoundsInSource.Height;
        var cropBytes = new byte[checked(cropWidth * cropHeight * 4)];
        var sourceStride = checked(source.Metadata.PixelWidth * 4);
        var cropStride = checked(cropWidth * 4);

        for (var row = 0; row < cropHeight; row++)
        {
            var sourceOffset = checked((cropBoundsInSource.Top + row) * sourceStride + cropBoundsInSource.Left * 4);
            var cropOffset = row * cropStride;
            sourceBytes.AsSpan(sourceOffset, cropStride).CopyTo(cropBytes.AsSpan(cropOffset, cropStride));
        }

        var metadata = new ImageResultMetadata
        {
            ResultId = resultId,
            SessionId = source.Metadata.SessionId,
            PixelWidth = cropWidth,
            PixelHeight = cropHeight,
            PixelFormat = ImagePixelFormat.Bgra8,
            AlphaMode = ImageAlphaMode.Premultiplied,
            ColorSpace = ImageColorSpace.SrgbSdr,
            DpiX = source.Metadata.DpiX,
            DpiY = source.Metadata.DpiY,
            RowStride = cropStride,
            SourceKind = source.Metadata.SourceKind,
            SourcePhysicalBounds = source.Metadata.SourcePhysicalBounds,
            CropPhysicalBounds = new PhysicalRect(
                source.Metadata.CropPhysicalBounds.Left + cropBoundsInSource.Left,
                source.Metadata.CropPhysicalBounds.Top + cropBoundsInSource.Top,
                source.Metadata.CropPhysicalBounds.Left + cropBoundsInSource.Right,
                source.Metadata.CropPhysicalBounds.Top + cropBoundsInSource.Bottom),
            CapturedAt = capturedAt,
            CursorIncluded = source.Metadata.CursorIncluded,
            ContentVersion = source.Metadata.ContentVersion
        };

        return SoftwareBitmapFactory.CreateFromPremultipliedBgra(cropBytes, metadata);
    }
}
