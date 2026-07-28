namespace SnipPlus.Contracts;

public enum WorkflowState
{
    ResidentReady,
    CaptureRequested,
    Freezing,
    Selecting,
    SelectionLocked,
    Cancelled,
    Failed,
    Idle,
    Starting,
    Capturing,
    ResultReady,
    Delivering,
    Completed
}

public enum CaptureRequestSource
{
    PrintScreen,
    SecondaryInAppCommand
}

public enum CaptureRequestRejectionReason
{
    None,
    Busy,
    InvalidState,
    ApplicationExiting
}

public sealed record CaptureRequest(
    Guid RequestId,
    DateTimeOffset RequestedAt,
    CaptureRequestSource RequestSource)
{
    public static CaptureRequest FromPrintScreen(PrintScreenReceivedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);
        return new CaptureRequest(args.RequestId, args.ReceivedAt, CaptureRequestSource.PrintScreen);
    }

    public static CaptureRequest CreateSecondary(Guid requestId, DateTimeOffset requestedAt) =>
        new(requestId, requestedAt, CaptureRequestSource.SecondaryInAppCommand);
}

public sealed record CaptureRequestResult(
    CaptureRequest Request,
    bool IsAccepted,
    WorkflowState CurrentState,
    WorkflowState? AcceptedWorkflowState,
    CaptureRequestRejectionReason RejectionReason,
    CaptureRequest? ActiveRequest,
    string UserMessage)
{
    public static CaptureRequestResult Accepted(CaptureRequest request, WorkflowState state, string message) =>
        new(request, true, state, state, CaptureRequestRejectionReason.None, request, message);

    public static CaptureRequestResult Rejected(
        CaptureRequest request,
        WorkflowState state,
        CaptureRequestRejectionReason reason,
        CaptureRequest? activeRequest,
        string message) =>
        new(request, false, state, null, reason, activeRequest, message);
}

public interface ICaptureRequestBoundary
{
    CaptureRequestResult Submit(CaptureRequest request);
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
