using SnipPlus.Contracts;

namespace SnipPlus.Windows;

/// <summary>
/// Renders a privacy annotation from the already-frozen display bitmap.
/// This renderer never captures a live frame and returns only the affected rectangle.
/// </summary>
public static class FrozenPrivacyEffectRenderer
{
    public static SoftwareBitmapImageResult Render(
        SoftwareBitmapImageResult source,
        PhysicalRect sourcePhysicalBounds,
        PhysicalRect effectPhysicalBounds,
        PrivacyRegionAnnotationContent content)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(content);

        if (!sourcePhysicalBounds.IsPositive
            || !effectPhysicalBounds.IsPositive
            || !sourcePhysicalBounds.Contains(effectPhysicalBounds)
            || source.Metadata.PixelWidth != sourcePhysicalBounds.Width
            || source.Metadata.PixelHeight != sourcePhysicalBounds.Height)
        {
            throw new ArgumentOutOfRangeException(nameof(effectPhysicalBounds));
        }

        var width = effectPhysicalBounds.Width;
        var height = effectPhysicalBounds.Height;
        var sourceLeft = checked(effectPhysicalBounds.Left - sourcePhysicalBounds.Left);
        var sourceTop = checked(effectPhysicalBounds.Top - sourcePhysicalBounds.Top);
        var sourceStride = checked(source.Metadata.PixelWidth * 4);
        var resultStride = checked(width * 4);
        var effectBytes = new byte[checked(resultStride * height)];

        using var lease = source.AcquireBitmapLease();
        var sourceBytes = SoftwareBitmapBuffer.Read(lease.Bitmap);
        for (var row = 0; row < height; row++)
        {
            var sourceOffset = checked((sourceTop + row) * sourceStride + sourceLeft * 4);
            var resultOffset = row * resultStride;
            sourceBytes.AsSpan(sourceOffset, resultStride)
                .CopyTo(effectBytes.AsSpan(resultOffset, resultStride));
        }

        switch (content.Mode)
        {
            case PrivacyRegionMode.Mosaic:
                ApplyMosaic(effectBytes, width, height, content.EffectParameters.MosaicBlockSize);
                break;
            case PrivacyRegionMode.Blur:
                ApplyBlur(effectBytes, width, height, content.EffectParameters.BlurRadius);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(content));
        }

        var metadata = new ImageResultMetadata
        {
            ResultId = Guid.NewGuid(),
            SessionId = source.Metadata.SessionId,
            PixelWidth = width,
            PixelHeight = height,
            PixelFormat = ImagePixelFormat.Bgra8,
            AlphaMode = ImageAlphaMode.Premultiplied,
            ColorSpace = ImageColorSpace.SrgbSdr,
            DpiX = source.Metadata.DpiX,
            DpiY = source.Metadata.DpiY,
            RowStride = resultStride,
            SourceKind = source.Metadata.SourceKind,
            SourcePhysicalBounds = sourcePhysicalBounds,
            CropPhysicalBounds = effectPhysicalBounds,
            CapturedAt = source.Metadata.CapturedAt,
            CursorIncluded = false,
            ContentVersion = source.Metadata.ContentVersion
        };

        return SoftwareBitmapFactory.CreateFromPremultipliedBgra(effectBytes, metadata);
    }

    private static void ApplyMosaic(byte[] pixels, int width, int height, int blockSize)
    {
        var stride = checked(width * 4);
        for (var blockTop = 0; blockTop < height; blockTop += blockSize)
        {
            var blockBottom = Math.Min(blockTop + blockSize, height);
            for (var blockLeft = 0; blockLeft < width; blockLeft += blockSize)
            {
                var blockRight = Math.Min(blockLeft + blockSize, width);
                var count = checked((blockRight - blockLeft) * (blockBottom - blockTop));
                var sums = new long[4];
                for (var y = blockTop; y < blockBottom; y++)
                {
                    var rowOffset = y * stride;
                    for (var x = blockLeft; x < blockRight; x++)
                    {
                        var offset = rowOffset + x * 4;
                        for (var channel = 0; channel < 4; channel++)
                        {
                            sums[channel] += pixels[offset + channel];
                        }
                    }
                }

                for (var y = blockTop; y < blockBottom; y++)
                {
                    var rowOffset = y * stride;
                    for (var x = blockLeft; x < blockRight; x++)
                    {
                        var offset = rowOffset + x * 4;
                        for (var channel = 0; channel < 4; channel++)
                        {
                            pixels[offset + channel] = (byte)(sums[channel] / count);
                        }
                    }
                }
            }
        }
    }

    private static void ApplyBlur(byte[] pixels, int width, int height, double radius)
    {
        var radiusPixels = Math.Max(1, (int)Math.Ceiling(radius));
        var stride = checked(width * 4);
        var horizontal = new byte[pixels.Length];
        var diameter = checked(radiusPixels * 2 + 1);

        for (var y = 0; y < height; y++)
        {
            var rowOffset = y * stride;
            for (var x = 0; x < width; x++)
            {
                var destinationOffset = rowOffset + x * 4;
                for (var channel = 0; channel < 4; channel++)
                {
                    var sum = 0;
                    for (var sample = x - radiusPixels; sample <= x + radiusPixels; sample++)
                    {
                        var clampedX = Math.Clamp(sample, 0, width - 1);
                        sum += pixels[rowOffset + clampedX * 4 + channel];
                    }

                    horizontal[destinationOffset + channel] = (byte)(sum / diameter);
                }
            }
        }

        for (var y = 0; y < height; y++)
        {
            var rowOffset = y * stride;
            for (var x = 0; x < width; x++)
            {
                var destinationOffset = rowOffset + x * 4;
                for (var channel = 0; channel < 4; channel++)
                {
                    var sum = 0;
                    for (var sample = y - radiusPixels; sample <= y + radiusPixels; sample++)
                    {
                        var clampedY = Math.Clamp(sample, 0, height - 1);
                        sum += horizontal[clampedY * stride + x * 4 + channel];
                    }

                    pixels[destinationOffset + channel] = (byte)(sum / diameter);
                }
            }
        }
    }
}
