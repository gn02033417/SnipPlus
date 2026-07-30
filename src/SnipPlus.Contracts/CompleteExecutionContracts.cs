namespace SnipPlus.Contracts;

public enum CompleteExecutionStage
{
    CommandAccepted,
    SessionValidated,
    FrozenFrameSetValidated,
    TransitioningToResultReady,
    Rendering,
    RenderSucceeded,
    RenderFailed,
    ResultValidation,
    ResultValidationFailed,
    TransitioningToDelivering,
    ClipboardEncoding,
    ClipboardPublishing,
    ClipboardFlushing,
    ClipboardDelivered,
    ClipboardFailed,
    ReturningToEditing,
    CleaningUp,
    Completed
}

public sealed record CompleteExecutionTraceEntry
{
    public required DateTimeOffset TimestampUtc { get; init; }
    public required Guid SessionId { get; init; }
    public required int SelectionRevision { get; init; }
    public required WorkflowState WorkflowState { get; init; }
    public required CompleteExecutionStage CompleteStage { get; init; }
    public FailureCode? FailureCode { get; init; }
    public FailureCategory? FailureCategory { get; init; }
    public int? NativeCode { get; init; }
    public required string Component { get; init; }
    public int SelectionWidth { get; init; }
    public int SelectionHeight { get; init; }
    public int ResultWidth { get; init; }
    public int ResultHeight { get; init; }
    public int DisplayCount { get; init; }
    public int ClipboardAttempt { get; init; }
    public int? ManagedThreadId { get; init; }
    public bool? DispatcherAvailable { get; init; }
    public bool? DispatcherHasThreadAccess { get; init; }
    public bool? DispatcherEnqueueSucceeded { get; init; }
    public string? DiagnosticEvent { get; init; }
    public string? ExceptionType { get; init; }
}

public interface ICompleteExecutionTraceSink
{
    void Record(CompleteExecutionTraceEntry entry);
}
