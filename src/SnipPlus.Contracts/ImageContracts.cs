namespace SnipPlus.Contracts;

public enum ImagePixelFormat
{
    Bgra8
}

public enum ImageAlphaMode
{
    Premultiplied
}

public enum ImageColorSpace
{
    SrgbSdr
}

public sealed record ImageResultMetadata
{
    public required Guid ResultId { get; init; }
    public required Guid SessionId { get; init; }
    public required int PixelWidth { get; init; }
    public required int PixelHeight { get; init; }
    public required ImagePixelFormat PixelFormat { get; init; }
    public required ImageAlphaMode AlphaMode { get; init; }
    public required ImageColorSpace ColorSpace { get; init; }
    public required double DpiX { get; init; }
    public required double DpiY { get; init; }
    public required int RowStride { get; init; }
    public required SourceKind SourceKind { get; init; }
    public required PhysicalRect SourcePhysicalBounds { get; init; }
    public required PhysicalRect CropPhysicalBounds { get; init; }
    public required DateTimeOffset CapturedAt { get; init; }
    public bool CursorIncluded { get; init; }
    public int ContentVersion { get; init; } = 1;
}

public interface IImageResult : IDisposable
{
    ImageResultMetadata Metadata { get; }

    bool IsDisposed { get; }

    IImageResultLease AcquireLease();
}

public interface IImageResultLease : IDisposable
{
    IImageResult ImageResult { get; }
}
