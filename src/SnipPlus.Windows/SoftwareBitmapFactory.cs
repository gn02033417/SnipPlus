using SnipPlus.Contracts;
using Windows.Graphics.Imaging;

namespace SnipPlus.Windows;

public static class SoftwareBitmapFactory
{
    public static SoftwareBitmapImageResult CreateFromPremultipliedBgra(
        ReadOnlySpan<byte> bgra8Premultiplied,
        ImageResultMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        var bitmap = SoftwareBitmapBuffer.Create(
            bgra8Premultiplied,
            metadata.PixelWidth,
            metadata.PixelHeight);

        try
        {
            return new SoftwareBitmapImageResult(bitmap, metadata);
        }
        catch
        {
            bitmap.Dispose();
            throw;
        }
    }

    public static SoftwareBitmapImageResult CreateFromStraightBgra(
        ReadOnlySpan<byte> straightBgra,
        ImageResultMetadata metadata)
    {
        var premultiplied = PremultipliedBgra8Converter.FromStraightBgra(straightBgra);
        return CreateFromPremultipliedBgra(premultiplied, metadata);
    }
}
