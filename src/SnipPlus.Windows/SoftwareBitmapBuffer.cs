using Windows.Graphics.Imaging;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace SnipPlus.Windows;

internal static class SoftwareBitmapBuffer
{
    public static SoftwareBitmap Create(ReadOnlySpan<byte> bgra8Premultiplied, int width, int height)
    {
        Validate(bgra8Premultiplied.Length, width, height);
        var bitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, width, height, BitmapAlphaMode.Premultiplied);
        try
        {
            bitmap.CopyFromBuffer(CryptographicBuffer.CreateFromByteArray(bgra8Premultiplied.ToArray()));
            return bitmap;
        }
        catch
        {
            bitmap.Dispose();
            throw;
        }
    }

    public static byte[] Read(SoftwareBitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        var byteCount = checked(bitmap.PixelWidth * bitmap.PixelHeight * 4);
        var buffer = new global::Windows.Storage.Streams.Buffer((uint)byteCount);
        bitmap.CopyToBuffer(buffer);
        var bytes = new byte[byteCount];
        using var reader = DataReader.FromBuffer(buffer);
        reader.ReadBytes(bytes);
        return bytes;
    }

    private static void Validate(int length, int width, int height)
    {
        if (width <= 0 || height <= 0 || length != checked(width * height * 4))
        {
            throw new ArgumentException("BGRA8 dimensions and data length do not match.");
        }
    }
}
