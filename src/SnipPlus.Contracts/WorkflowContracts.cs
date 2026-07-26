namespace SnipPlus.Contracts;

public enum WorkflowState
{
    Idle,
    Starting,
    Selecting,
    Capturing,
    ResultReady,
    Delivering,
    Completed,
    Cancelled,
    Failed
}

public sealed record WorkflowTransitionRequest(WorkflowState From, WorkflowState To, string Reason);

public sealed record WorkflowTransitionResult(
    bool IsSuccess,
    WorkflowState CurrentState,
    Failure? Failure)
{
    public static WorkflowTransitionResult Success(WorkflowState state) => new(true, state, null);

    public static WorkflowTransitionResult Rejected(WorkflowState state, Failure failure) => new(false, state, failure);
}

public enum WorkflowOutcomeKind
{
    Completed,
    Cancelled,
    RetryableFailure,
    TerminalFailure
}

public sealed record WorkflowRunResult(
    WorkflowOutcomeKind Outcome,
    WorkflowState FinalState,
    IImageResult? RetainedResult,
    Failure? Failure,
    bool CleanupCompleted);
