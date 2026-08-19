using SnipPlus.Contracts;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace SnipPlus.Windows;

public static class PngEncoder
{
    public static ValueTask<InMemoryRandomAccessStream> EncodeAsync(
        IImageResult imageResult,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(imageResult);
        if (imageResult is not SoftwareBitmapImageResult softwareBitmapImageResult)
        {
            throw new ArgumentException(
                "PNG encoding requires the canonical SoftwareBitmap image result.",
                nameof(imageResult));
        }

        return EncodeAsync(softwareBitmapImageResult, cancellationToken);
    }

    public static async ValueTask<InMemoryRandomAccessStream> EncodeAsync(
        SoftwareBitmapImageResult imageResult,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(imageResult);
        cancellationToken.ThrowIfCancellationRequested();
        using var lease = imageResult.AcquireBitmapLease();
        var stream = new InMemoryRandomAccessStream();
        try
        {
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetSoftwareBitmap(lease.Bitmap);
            await encoder.FlushAsync().AsTask(cancellationToken);
            stream.Seek(0);
            return stream;
        }
        catch
        {
            stream.Dispose();
            throw;
        }
    }
}
