using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;

namespace SnipPlus.Windows;

public sealed class WinUiImagePresentationAdapter
{
    public static async ValueTask PresentAsync(
        Image target,
        SoftwareBitmapImageResult imageResult,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(imageResult);
        cancellationToken.ThrowIfCancellationRequested();

        using var lease = imageResult.AcquireBitmapLease();
        var source = new SoftwareBitmapSource();
        await source.SetBitmapAsync(lease.Bitmap).AsTask(cancellationToken);
        target.Source = source;
    }
}
