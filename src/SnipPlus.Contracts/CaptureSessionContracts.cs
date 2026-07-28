using System.Collections.ObjectModel;

namespace SnipPlus.Contracts;

public sealed record ForegroundContextReference
{
    public ForegroundContextReference(string contextId, DateTimeOffset capturedAt)
    {
        if (string.IsNullOrWhiteSpace(contextId))
        {
            throw new ArgumentException("A foreground context identifier is required.", nameof(contextId));
        }

        ContextId = contextId;
        CapturedAt = capturedAt;
    }

    public string ContextId { get; }

    public DateTimeOffset CapturedAt { get; }
}

public enum CaptureSessionStatus
{
    Freezing,
    FrozenFrameSetReady,
    Cancelled,
    Failed,
    Disposed
}

public sealed class CaptureSessionContext : IDisposable
{
    private readonly object _gate = new();
    private readonly CancellationTokenSource _cancellation;
    private FrozenDisplayFrameSet? _frozenDisplayFrames;
    private CaptureSessionStatus _status;
    private bool _disposed;

    public CaptureSessionContext(
        CaptureRequest request,
        VirtualDesktopSnapshot virtualDesktopSnapshot,
        CapacityValidationOutcome capacityValidation,
        ForegroundContextReference? preCaptureForegroundContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(virtualDesktopSnapshot);
        ArgumentNullException.ThrowIfNull(capacityValidation);
        if (!capacityValidation.IsSupported)
        {
            throw new ArgumentException("A capture session requires supported capacity.", nameof(capacityValidation));
        }

        Request = request;
        RequestId = request.RequestId;
        SessionId = Guid.NewGuid();
        RequestedAt = request.RequestedAt;
        VirtualDesktopSnapshot = virtualDesktopSnapshot;
        CapacityValidation = capacityValidation;
        PreCaptureForegroundContext = preCaptureForegroundContext;
        _cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _status = CaptureSessionStatus.Freezing;
    }

    public CaptureRequest Request { get; }

    public Guid RequestId { get; }

    public Guid SessionId { get; }

    public DateTimeOffset RequestedAt { get; }

    public CancellationToken Cancellation => _cancellation.Token;

    public ForegroundContextReference? PreCaptureForegroundContext { get; }

    public VirtualDesktopSnapshot VirtualDesktopSnapshot { get; }

    public CapacityValidationOutcome CapacityValidation { get; }

    public CaptureSessionStatus Status
    {
        get
        {
            lock (_gate)
            {
                return _status;
            }
        }
    }

    public bool IsCancelled => Cancellation.IsCancellationRequested;

    public bool IsDisposed
    {
        get
        {
            lock (_gate)
            {
                return _disposed;
            }
        }
    }

    public FrozenDisplayFrameSet? FrozenDisplayFrames
    {
        get
        {
            lock (_gate)
            {
                return _frozenDisplayFrames;
            }
        }
    }

    public bool TryAttachFrozenDisplayFrames(FrozenDisplayFrameSet frozenDisplayFrames)
    {
        ArgumentNullException.ThrowIfNull(frozenDisplayFrames);
        lock (_gate)
        {
            if (_disposed
                || _status != CaptureSessionStatus.Freezing
                || _frozenDisplayFrames is not null
                || frozenDisplayFrames.SessionId != SessionId
                || !string.Equals(
                    frozenDisplayFrames.CoordinateVersion,
                    VirtualDesktopSnapshot.CoordinateVersion,
                    StringComparison.Ordinal))
            {
                return false;
            }

            _frozenDisplayFrames = frozenDisplayFrames;
            _status = CaptureSessionStatus.FrozenFrameSetReady;
            return true;
        }
    }

    public void Cancel()
    {
        FrozenDisplayFrameSet? frames;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _status = CaptureSessionStatus.Cancelled;
            frames = _frozenDisplayFrames;
            _frozenDisplayFrames = null;
        }

        _cancellation.Cancel();
        frames?.Dispose();
        _cancellation.Dispose();
    }

    public void MarkFailedAndDispose()
    {
        FrozenDisplayFrameSet? frames;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _status = CaptureSessionStatus.Failed;
            frames = _frozenDisplayFrames;
            _frozenDisplayFrames = null;
        }

        _cancellation.Cancel();
        frames?.Dispose();
        _cancellation.Dispose();
    }

    public void Dispose()
    {
        FrozenDisplayFrameSet? frames;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _status = CaptureSessionStatus.Disposed;
            frames = _frozenDisplayFrames;
            _frozenDisplayFrames = null;
        }

        _cancellation.Cancel();
        frames?.Dispose();
        _cancellation.Dispose();
        GC.SuppressFinalize(this);
    }
}

public sealed class FrozenDisplayFrame : IDisposable
{
    private FrozenCaptureFrame? _frozenFrame;

    public FrozenDisplayFrame(
        Guid sessionId,
        string displayId,
        Guid frameId,
        string coordinateVersion,
        PhysicalRect physicalBoundsInVirtualDesktop,
        PhysicalPixelSize pixelSize,
        FrozenCaptureFrame frozenFrame)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Session identifier is required.", nameof(sessionId));
        }

        if (string.IsNullOrWhiteSpace(displayId))
        {
            throw new ArgumentException("Display identifier is required.", nameof(displayId));
        }

        if (frameId == Guid.Empty)
        {
            throw new ArgumentException("Frame identifier is required.", nameof(frameId));
        }

        if (string.IsNullOrWhiteSpace(coordinateVersion))
        {
            throw new ArgumentException("Coordinate version is required.", nameof(coordinateVersion));
        }

        if (!physicalBoundsInVirtualDesktop.IsPositive || !pixelSize.IsPositive)
        {
            throw new ArgumentException("Frozen frame bounds and pixel size must be positive.");
        }

        ArgumentNullException.ThrowIfNull(frozenFrame);
        if (frozenFrame.ImageResult.Metadata.SessionId != sessionId
            || frozenFrame.ImageResult.Metadata.PixelWidth != pixelSize.Width
            || frozenFrame.ImageResult.Metadata.PixelHeight != pixelSize.Height)
        {
            throw new ArgumentException("Frozen image metadata does not match the frame identity.", nameof(frozenFrame));
        }

        SessionId = sessionId;
        DisplayId = displayId;
        FrameId = frameId;
        CoordinateVersion = coordinateVersion;
        PhysicalBoundsInVirtualDesktop = physicalBoundsInVirtualDesktop;
        PixelSize = pixelSize;
        _frozenFrame = frozenFrame;
    }

    public Guid SessionId { get; }

    public string DisplayId { get; }

    public Guid FrameId { get; }

    public string CoordinateVersion { get; }

    public PhysicalRect PhysicalBoundsInVirtualDesktop { get; }

    public PhysicalPixelSize PixelSize { get; }

    public FrozenCaptureFrame FrozenFrame =>
        _frozenFrame ?? throw new ObjectDisposedException(nameof(FrozenDisplayFrame));

    public bool IsDisposed => _frozenFrame is null;

    public void Dispose()
    {
        Interlocked.Exchange(ref _frozenFrame, null)?.Dispose();
    }
}

public enum FrozenDisplayFrameSetFailureKind
{
    None,
    DuplicateDisplay,
    MissingDisplay,
    UnknownDisplay,
    DuplicateFrame,
    SessionMismatch,
    CoordinateVersionMismatch,
    BoundsMismatch,
    PixelSizeMismatch,
    DisposedFrame,
    Empty
}

public sealed record FrozenDisplayFrameSetValidation
{
    private FrozenDisplayFrameSetValidation(
        bool isValid,
        FrozenDisplayFrameSetFailureKind failureKind,
        string message)
    {
        IsValid = isValid;
        FailureKind = failureKind;
        Message = message;
    }

    public bool IsValid { get; }

    public FrozenDisplayFrameSetFailureKind FailureKind { get; }

    public string Message { get; }

    public static FrozenDisplayFrameSetValidation Valid() => new(
        true,
        FrozenDisplayFrameSetFailureKind.None,
        "Frozen display frame set is valid.");

    public static FrozenDisplayFrameSetValidation Invalid(
        FrozenDisplayFrameSetFailureKind failureKind,
        string message) => new(false, failureKind, message);
}

public sealed class FrozenDisplayFrameSet : IDisposable
{
    private readonly ReadOnlyDictionary<string, FrozenDisplayFrame> _frames;
    private int _disposed;

    private FrozenDisplayFrameSet(
        Guid sessionId,
        string coordinateVersion,
        IReadOnlyDictionary<string, FrozenDisplayFrame> frames)
    {
        SessionId = sessionId;
        CoordinateVersion = coordinateVersion;
        _frames = new ReadOnlyDictionary<string, FrozenDisplayFrame>(
            new Dictionary<string, FrozenDisplayFrame>(frames, StringComparer.Ordinal));
    }

    public Guid SessionId { get; }

    public string CoordinateVersion { get; }

    public IReadOnlyDictionary<string, FrozenDisplayFrame> Frames => _frames;

    public int ExpectedDisplayCount => _frames.Count;

    public bool IsComplete => _frames.Count > 0;

    public bool IsDisposed => Volatile.Read(ref _disposed) != 0;

    public static bool TryCreate(
        CaptureSessionContext session,
        IEnumerable<DisplaySnapshot> expectedDisplays,
        IEnumerable<FrozenDisplayFrame> frames,
        out FrozenDisplayFrameSet? frameSet,
        out FrozenDisplayFrameSetValidation validation)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(expectedDisplays);
        ArgumentNullException.ThrowIfNull(frames);

        var expected = expectedDisplays.ToArray();
        var supplied = frames.ToArray();
        frameSet = null;

        validation = Validate(session, expected, supplied);
        if (!validation.IsValid)
        {
            DisposeAll(supplied);
            return false;
        }

        var dictionary = supplied.ToDictionary(frame => frame.DisplayId, StringComparer.Ordinal);
        frameSet = new FrozenDisplayFrameSet(session.SessionId, session.VirtualDesktopSnapshot.CoordinateVersion, dictionary);
        return true;
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        foreach (var frame in _frames.Values)
        {
            frame.Dispose();
        }
    }

    private static FrozenDisplayFrameSetValidation Validate(
        CaptureSessionContext session,
        DisplaySnapshot[] expected,
        FrozenDisplayFrame[] supplied)
    {
        if (expected.Length == 0 || supplied.Length == 0)
        {
            return FrozenDisplayFrameSetValidation.Invalid(
                FrozenDisplayFrameSetFailureKind.Empty,
                "A frozen display frame set must contain one frame for every display.");
        }

        var expectedById = expected.ToDictionary(display => display.DisplayId, StringComparer.Ordinal);
        var displayIds = new HashSet<string>(StringComparer.Ordinal);
        var frameIds = new HashSet<Guid>();
        foreach (var frame in supplied)
        {
            if (frame.IsDisposed)
            {
                return FrozenDisplayFrameSetValidation.Invalid(
                    FrozenDisplayFrameSetFailureKind.DisposedFrame,
                    "A frozen frame has already been disposed.");
            }

            if (!expectedById.TryGetValue(frame.DisplayId, out var display))
            {
                return FrozenDisplayFrameSetValidation.Invalid(
                    FrozenDisplayFrameSetFailureKind.UnknownDisplay,
                    "A frozen frame does not belong to the display snapshot.");
            }

            if (!displayIds.Add(frame.DisplayId))
            {
                return FrozenDisplayFrameSetValidation.Invalid(
                    FrozenDisplayFrameSetFailureKind.DuplicateDisplay,
                    "The frozen frame set contains more than one frame for a display.");
            }

            if (!frameIds.Add(frame.FrameId))
            {
                return FrozenDisplayFrameSetValidation.Invalid(
                    FrozenDisplayFrameSetFailureKind.DuplicateFrame,
                    "The frozen frame set contains a duplicate frame identifier.");
            }

            if (frame.SessionId != session.SessionId)
            {
                return FrozenDisplayFrameSetValidation.Invalid(
                    FrozenDisplayFrameSetFailureKind.SessionMismatch,
                    "A frozen frame belongs to a different capture session.");
            }

            if (!string.Equals(frame.CoordinateVersion, session.VirtualDesktopSnapshot.CoordinateVersion, StringComparison.Ordinal))
            {
                return FrozenDisplayFrameSetValidation.Invalid(
                    FrozenDisplayFrameSetFailureKind.CoordinateVersionMismatch,
                    "A frozen frame belongs to a different coordinate snapshot.");
            }

            if (frame.PhysicalBoundsInVirtualDesktop != display.PhysicalBoundsInVirtualDesktop)
            {
                return FrozenDisplayFrameSetValidation.Invalid(
                    FrozenDisplayFrameSetFailureKind.BoundsMismatch,
                    "A frozen frame bounds do not match its display snapshot.");
            }

            if (frame.PixelSize != display.ExpectedFrozenFramePixelSize)
            {
                return FrozenDisplayFrameSetValidation.Invalid(
                    FrozenDisplayFrameSetFailureKind.PixelSizeMismatch,
                    "A frozen frame pixel size does not match its display snapshot.");
            }
        }

        if (displayIds.Count != expectedById.Count || expectedById.Keys.Any(id => !displayIds.Contains(id)))
        {
            return FrozenDisplayFrameSetValidation.Invalid(
                FrozenDisplayFrameSetFailureKind.MissingDisplay,
                "The frozen frame set is missing one or more displays.");
        }

        return FrozenDisplayFrameSetValidation.Valid();
    }

    private static void DisposeAll(IEnumerable<FrozenDisplayFrame> frames)
    {
        foreach (var frame in frames)
        {
            frame.Dispose();
        }
    }
}
