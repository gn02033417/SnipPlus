namespace SnipPlus.Contracts;

public enum SelectionStatus
{
    None,
    Dragging,
    Locked,
    Cancelled
}

public sealed record SelectionVisualState
{
    public required Guid SessionId { get; init; }

    public required string CoordinateVersion { get; init; }

    public int SelectionRevision { get; init; }

    public SelectionStatus Status { get; init; }

    public PhysicalPoint? DragStartPhysicalPoint { get; init; }

    public PhysicalPoint? CurrentPhysicalPoint { get; init; }

    public PhysicalRect? NormalizedPhysicalBounds { get; init; }

    public static SelectionVisualState Initial(
        Guid sessionId,
        string coordinateVersion) => new()
        {
            SessionId = sessionId,
            CoordinateVersion = coordinateVersion,
            SelectionRevision = 0,
            Status = SelectionStatus.None
        };
}

public sealed record SelectionPointerEvent(
    Guid SessionId,
    string CoordinateVersion,
    int PointerId,
    PhysicalPoint GlobalPhysicalPoint);

public enum SelectionInputResultKind
{
    Ignored,
    Dragging,
    Locked,
    Cancelled,
    InvalidSelection,
    StaleSession
}

public sealed record SelectionInputResult(
    SelectionInputResultKind Kind,
    SelectionVisualState State,
    string Message);

public interface ISelectionInputSink
{
    SelectionInputResult PointerPressed(SelectionPointerEvent input);

    SelectionInputResult PointerMoved(SelectionPointerEvent input);

    SelectionInputResult PointerReleased(SelectionPointerEvent input);

    SelectionInputResult Escape(Guid sessionId, string coordinateVersion);
}

public sealed record FrozenDisplayOverlayDescriptor(
    Guid SessionId,
    string CoordinateVersion,
    string DisplayId,
    PhysicalRect PhysicalBoundsInVirtualDesktop,
    PhysicalPixelSize PixelSize,
    FrozenDisplayFrame Frame);

public sealed record FrozenDisplayOverlayPlan(
    Guid SessionId,
    string CoordinateVersion,
    IReadOnlyList<FrozenDisplayOverlayDescriptor> Displays);

public sealed record FrozenDisplayOverlayPresentationRequest(
    FrozenDisplayOverlayPlan Plan,
    ISelectionInputSink InputSink);

public abstract record FrozenDisplayOverlayPresentationOutcome
{
    private FrozenDisplayOverlayPresentationOutcome()
    {
    }

    public sealed record Ready : FrozenDisplayOverlayPresentationOutcome;

    public sealed record Cancelled(string CancellationOrigin) : FrozenDisplayOverlayPresentationOutcome;

    public sealed record Failed(Failure Failure) : FrozenDisplayOverlayPresentationOutcome;
}

public interface IAllDisplayOverlayPresentationCoordinator : IDisposable
{
    ValueTask<FrozenDisplayOverlayPresentationOutcome> PresentAsync(
        FrozenDisplayOverlayPresentationRequest request,
        CancellationToken cancellationToken);

    void ApplySelection(SelectionVisualState state);

    ValueTask CloseAsync(Guid sessionId, CancellationToken cancellationToken);
}

public enum CaptureSourceExclusionKind
{
    Hidden,
    Failed,
    Cancelled
}

public sealed record CaptureSourceExclusionOutcome(
    CaptureSourceExclusionKind Kind,
    Failure? Failure,
    string Message)
{
    public bool IsExcluded => Kind == CaptureSourceExclusionKind.Hidden;

    public static CaptureSourceExclusionOutcome Hidden() => new(
        CaptureSourceExclusionKind.Hidden,
        null,
        "The application window is hidden before display capture.");

    public static CaptureSourceExclusionOutcome Failed(Failure failure) => new(
        CaptureSourceExclusionKind.Failed,
        failure,
        "The application window could not be excluded from the capture source.");

    public static CaptureSourceExclusionOutcome Cancelled() => new(
        CaptureSourceExclusionKind.Cancelled,
        null,
        "Capture source exclusion was cancelled.");
}

public interface ICaptureSourceExclusion
{
    ValueTask<CaptureSourceExclusionOutcome> ExcludeAsync(
        CaptureRequest request,
        CancellationToken cancellationToken);
}
