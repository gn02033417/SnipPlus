using SnipPlus.Contracts;
using Windows.Graphics.Imaging;

namespace SnipPlus.Windows;

public sealed class SoftwareBitmapImageResult : IImageResult
{
    private readonly object _gate = new();
    private SoftwareBitmap? _bitmap;
    private int _leaseCount;

    public SoftwareBitmapImageResult(SoftwareBitmap bitmap, ImageResultMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        ArgumentNullException.ThrowIfNull(metadata);

        if (bitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8
            || bitmap.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
        {
            bitmap.Dispose();
            throw new ArgumentException("Canonical image must be BGRA8 premultiplied.", nameof(bitmap));
        }

        if (bitmap.PixelWidth != metadata.PixelWidth
            || bitmap.PixelHeight != metadata.PixelHeight
            || metadata.PixelFormat != ImagePixelFormat.Bgra8
            || metadata.AlphaMode != ImageAlphaMode.Premultiplied
            || metadata.ColorSpace != ImageColorSpace.SrgbSdr
            || metadata.RowStride < metadata.PixelWidth * 4)
        {
            bitmap.Dispose();
            throw new ArgumentException("Image metadata does not match the canonical bitmap.", nameof(metadata));
        }

        _bitmap = bitmap;
        Metadata = metadata;
    }

    public ImageResultMetadata Metadata { get; }

    public bool IsDisposed
    {
        get
        {
            lock (_gate)
            {
                return _bitmap is null;
            }
        }
    }

    public SoftwareBitmapLease AcquireBitmapLease()
    {
        lock (_gate)
        {
            var bitmap = _bitmap;
            ObjectDisposedException.ThrowIf(bitmap is null, nameof(SoftwareBitmapImageResult));

            _leaseCount++;
            return new SoftwareBitmapLease(this, bitmap);
        }
    }

    public IImageResultLease AcquireLease() => AcquireBitmapLease();

    public void Dispose()
    {
        SoftwareBitmap? bitmapToDispose = null;
        lock (_gate)
        {
            if (_bitmap is null)
            {
                return;
            }

            if (_leaseCount == 0)
            {
                bitmapToDispose = _bitmap;
                _bitmap = null;
            }
            else
            {
                _bitmap = null;
            }
        }

        bitmapToDispose?.Dispose();
    }

    internal void ReleaseLease(SoftwareBitmap bitmap)
    {
        SoftwareBitmap? bitmapToDispose = null;
        lock (_gate)
        {
            if (_leaseCount > 0)
            {
                _leaseCount--;
            }

            if (_leaseCount == 0 && _bitmap is null)
            {
                bitmapToDispose = bitmap;
            }
        }

        bitmapToDispose?.Dispose();
    }
}

public sealed class SoftwareBitmapLease : IImageResultLease
{
    private SoftwareBitmapImageResult? _owner;
    private SoftwareBitmap? _bitmap;

    internal SoftwareBitmapLease(SoftwareBitmapImageResult owner, SoftwareBitmap bitmap)
    {
        _owner = owner;
        _bitmap = bitmap;
    }

    public IImageResult ImageResult => _owner ?? throw new ObjectDisposedException(nameof(SoftwareBitmapLease));

    public SoftwareBitmap Bitmap => _bitmap ?? throw new ObjectDisposedException(nameof(SoftwareBitmapLease));

    public void Dispose()
    {
        var owner = Interlocked.Exchange(ref _owner, null);
        var bitmap = Interlocked.Exchange(ref _bitmap, null);
        if (owner is not null && bitmap is not null)
        {
            owner.ReleaseLease(bitmap);
        }
    }
}
