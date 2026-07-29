namespace SnipPlus.Contracts;

public enum FunctionBarCommand
{
    Complete,
    Save,
    Cancel,
    Undo,
    Redo
}

public readonly record struct FunctionBarCommandAvailability(
    bool CanComplete,
    bool CanSave,
    bool CanCancel,
    bool CanUndo,
    bool CanRedo)
{
    public static FunctionBarCommandAvailability Stage6B => new(
        CanComplete: false,
        CanSave: false,
        CanCancel: true,
        CanUndo: false,
        CanRedo: false);

    public bool IsEnabled(FunctionBarCommand command) => command switch
    {
        FunctionBarCommand.Complete => CanComplete,
        FunctionBarCommand.Save => CanSave,
        FunctionBarCommand.Cancel => CanCancel,
        FunctionBarCommand.Undo => CanUndo,
        FunctionBarCommand.Redo => CanRedo,
        _ => false
    };
}

public sealed record FunctionBarCommandRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    FunctionBarCommand Command);

public enum FunctionBarCommandResultKind
{
    Accepted,
    Disabled,
    Cancelled,
    StaleSession,
    StaleSelectionRevision,
    InvalidWorkflowState,
    Failed
}

public sealed record FunctionBarCommandResult(
    FunctionBarCommand Command,
    FunctionBarCommandResultKind Kind,
    WorkflowState CurrentState,
    int CurrentSelectionRevision,
    Failure? Failure,
    string Message);

public enum FunctionBarPlacementSide
{
    Below,
    Above,
    ClampedBelow,
    ClampedAbove
}

public sealed record FunctionBarDisplayWorkArea(
    string DisplayId,
    PhysicalRect DisplayPhysicalBounds,
    PhysicalRect PhysicalWorkArea,
    double DpiScaleX,
    double DpiScaleY);

public sealed record FunctionBarPlacementRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    PhysicalRect SelectionPhysicalBounds,
    IReadOnlyList<FunctionBarDisplayWorkArea> DisplayPhysicalWorkAreas,
    PhysicalPixelSize MeasuredBarPhysicalSize,
    int MarginPixels,
    PhysicalPoint? CurrentPhysicalPoint);

public sealed record FunctionBarPlacementResult(
    string DisplayId,
    PhysicalRect FunctionBarPhysicalBounds,
    FunctionBarPlacementSide PlacementSide,
    int SelectionRevision,
    bool IsFullyInsideWorkArea);

public abstract record FunctionBarPlacementOutcome
{
    private FunctionBarPlacementOutcome()
    {
    }

    public sealed record Ready(FunctionBarPlacementResult Placement)
        : FunctionBarPlacementOutcome;

    public sealed record Failed(Failure Failure)
        : FunctionBarPlacementOutcome;
}

public interface IFunctionBarPlacementService
{
    FunctionBarPlacementOutcome Place(FunctionBarPlacementRequest request);
}

public sealed record FunctionBarPresentationRequest(
    Guid SessionId,
    string CoordinateVersion,
    SelectionVisualState Selection,
    FunctionBarCommandAvailability Availability,
    IFunctionBarCommandSink CommandSink);

public enum FunctionBarPresentationResultKind
{
    Ready,
    Shown,
    Hidden,
    Closed,
    StaleSession,
    StaleSelectionRevision,
    Failed
}

public sealed record FunctionBarPresentationResult(
    FunctionBarPresentationResultKind Kind,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    FunctionBarPlacementResult? Placement,
    Failure? Failure,
    string Message);

public interface IFunctionBarPresentationCoordinator : IDisposable
{
    FunctionBarPresentationResult Prepare(FunctionBarPresentationRequest request);

    FunctionBarPresentationResult Reposition(FunctionBarPresentationRequest request);

    FunctionBarPresentationResult Show(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision);

    FunctionBarPresentationResult Hide(Guid sessionId);

    FunctionBarPresentationResult Close(Guid sessionId);
}

public interface IFunctionBarCommandSink
{
    FunctionBarCommandResult Execute(FunctionBarCommandRequest request);
}
