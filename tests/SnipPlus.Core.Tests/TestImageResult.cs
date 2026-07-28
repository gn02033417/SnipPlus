using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

internal sealed class TestImageResult : IImageResult
{
    private int _leaseCount;

    public TestImageResult(
        Guid? resultId = null,
        Guid? sessionId = null,
        int pixelWidth = 2,
        int pixelHeight = 2,
        PhysicalRect? sourceBounds = null,
        PhysicalRect? cropBounds = null)
    {
        Metadata = new ImageResultMetadata
        {
            ResultId = resultId ?? Guid.NewGuid(),
            SessionId = sessionId ?? Guid.NewGuid(),
            PixelWidth = pixelWidth,
            PixelHeight = pixelHeight,
            PixelFormat = ImagePixelFormat.Bgra8,
            AlphaMode = ImageAlphaMode.Premultiplied,
            ColorSpace = ImageColorSpace.SrgbSdr,
            DpiX = 96,
            DpiY = 96,
            RowStride = checked(pixelWidth * 4),
            SourceKind = SourceKind.Monitor,
            SourcePhysicalBounds = sourceBounds ?? new PhysicalRect(0, 0, pixelWidth, pixelHeight),
            CropPhysicalBounds = cropBounds ?? sourceBounds ?? new PhysicalRect(0, 0, pixelWidth, pixelHeight),
            CapturedAt = DateTimeOffset.UtcNow
        };
    }

    public ImageResultMetadata Metadata { get; }

    public bool IsDisposed { get; private set; }

    public IImageResultLease AcquireLease()
    {
        if (IsDisposed)
        {
            throw new InvalidOperationException("Result is disposed.");
        }

        _leaseCount++;
        return new Lease(this);
    }

    public void Dispose()
    {
        IsDisposed = true;
    }

    private sealed class Lease : IImageResultLease
    {
        private TestImageResult? _owner;

        public Lease(TestImageResult owner)
        {
            _owner = owner;
        }

        public IImageResult ImageResult => _owner ?? throw new ObjectDisposedException(nameof(Lease));

        public void Dispose()
        {
            if (_owner is not null)
            {
                _owner._leaseCount--;
                _owner = null;
            }
        }
    }
}
