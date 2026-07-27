namespace SnipPlus.Contracts;

public enum SourceKind
{
    Monitor,
    Window
}

public readonly record struct DipRect(double Left, double Top, double Right, double Bottom)
{
    public double Width => Right - Left;

    public double Height => Bottom - Top;

    public bool IsPositive => double.IsFinite(Left)
        && double.IsFinite(Top)
        && double.IsFinite(Right)
        && double.IsFinite(Bottom)
        && Width > 0
        && Height > 0;
}

public readonly record struct PhysicalRect(int Left, int Top, int Right, int Bottom)
{
    public int Width => Right - Left;

    public int Height => Bottom - Top;

    public bool IsPositive => Width > 0 && Height > 0;

    public bool Contains(PhysicalRect other) => other.Left >= Left
        && other.Top >= Top
        && other.Right <= Right
        && other.Bottom <= Bottom;
}

public sealed record DisplayContextSnapshot(
    string CoordinateVersion,
    string SourceId,
    PhysicalRect SourcePhysicalBounds,
    double DpiScaleX,
    double DpiScaleY);

public sealed record CaptureIntent
{
    public required Guid RequestId { get; init; }
    public required Guid SessionId { get; init; }
    public required SourceKind SourceKind { get; init; }
    public required string SourceId { get; init; }
    public required PhysicalRect SourcePhysicalBounds { get; init; }
    public required DipRect SelectionDipBounds { get; init; }
    public required PhysicalRect SelectionPhysicalBounds { get; init; }
    public required PhysicalRect CropBoundsInSource { get; init; }
    public required double DpiScaleX { get; init; }
    public required double DpiScaleY { get; init; }
    public required string CoordinateVersion { get; init; }
    public bool IncludeCursor { get; init; }
    public DateTimeOffset RequestedAt { get; init; } = DateTimeOffset.UtcNow;
    public CancellationToken Cancellation { get; init; }
}

public sealed class FrozenCaptureFrame : IDisposable
{
    private IImageResult? _imageResult;

    public FrozenCaptureFrame(IImageResult imageResult)
    {
        _imageResult = imageResult ?? throw new ArgumentNullException(nameof(imageResult));
    }

    public IImageResult ImageResult =>
        _imageResult ?? throw new ObjectDisposedException(nameof(FrozenCaptureFrame));

    public bool IsDisposed => _imageResult is null;

    public void Dispose()
    {
        Interlocked.Exchange(ref _imageResult, null)?.Dispose();
    }
}

public abstract record CaptureFrameOutcome(Guid RequestId, Guid SessionId)
{
    public sealed record Succeeded(
        Guid RequestId,
        Guid SessionId,
        FrozenCaptureFrame FrozenFrame) : CaptureFrameOutcome(RequestId, SessionId);

    public sealed record Cancelled(
        Guid RequestId,
        Guid SessionId,
        string CancellationOrigin,
        bool SourceSessionStarted,
        bool CleanupCompleted) : CaptureFrameOutcome(RequestId, SessionId);

    public sealed record Failed(
        Guid RequestId,
        Guid SessionId,
        Failure Failure,
        bool CleanupCompleted,
        bool RequiresNewIntent) : CaptureFrameOutcome(RequestId, SessionId);
}

public abstract record CaptureOutcome(Guid RequestId, Guid SessionId)
{
    public sealed record Succeeded(
        Guid RequestId,
        Guid SessionId,
        int SourceWidth,
        int SourceHeight,
        PhysicalRect SourcePhysicalBounds,
        PhysicalRect CropBoundsUsed,
        DateTimeOffset CapturedAt,
        IImageResult ImageResult,
        IReadOnlyList<string> Warnings) : CaptureOutcome(RequestId, SessionId);

    public sealed record Cancelled(
        Guid RequestId,
        Guid SessionId,
        string CancellationOrigin,
        bool SourceSessionStarted,
        bool CleanupCompleted) : CaptureOutcome(RequestId, SessionId);

    public sealed record Failed(
        Guid RequestId,
        Guid SessionId,
        Failure Failure,
        bool CleanupCompleted,
        bool RequiresNewIntent) : CaptureOutcome(RequestId, SessionId);
}

public interface ICaptureService
{
    ValueTask<CaptureFrameOutcome> CaptureFrameAsync(
        CaptureIntent fullFrameIntent,
        CancellationToken cancellationToken);

    ValueTask<CaptureOutcome> CropFrameAsync(
        CaptureIntent intent,
        FrozenCaptureFrame frozenFrame,
        CancellationToken cancellationToken);
}
