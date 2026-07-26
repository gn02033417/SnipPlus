using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class CaptureWorkflowCoordinator
{
    public CaptureWorkflowCoordinator(WorkflowStateAuthority stateAuthority)
    {
        StateAuthority = stateAuthority;
    }

    public WorkflowStateAuthority StateAuthority { get; }

    public async ValueTask<WorkflowRunResult> RunAsync(
        CaptureIntent intent,
        ICaptureService captureService,
        IClipboardDeliveryService? clipboardService,
        CancellationToken cancellationToken)
    {
        if (!TryTransition(WorkflowState.Idle, WorkflowState.Starting, intent.RequestId, out var transitionFailure)
            || !TryTransition(WorkflowState.Starting, WorkflowState.Selecting, intent.RequestId, out transitionFailure)
            || !TryTransition(WorkflowState.Selecting, WorkflowState.Capturing, intent.RequestId, out transitionFailure))
        {
            return TerminalFailure(transitionFailure!);
        }

        CaptureOutcome captureOutcome;
        try
        {
            captureOutcome = await captureService.CaptureAsync(intent, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            captureOutcome = new CaptureOutcome.Cancelled(
                intent.RequestId,
                intent.SessionId,
                "CancellationToken",
                true,
                true);
        }
        catch (Exception exception)
        {
            captureOutcome = new CaptureOutcome.Failed(
                intent.RequestId,
                intent.SessionId,
                Failure.Create(
                    FailureCode.UnexpectedFailure,
                    FailureCategory.Unexpected,
                    FailureRecoverability.TerminalForSession,
                    "CaptureWorkflowCoordinator.Capture",
                    intent.RequestId,
                    exception.GetType().Name),
                false,
                true);
        }

        switch (captureOutcome)
        {
            case CaptureOutcome.Cancelled cancelled:
                return Cancelled(intent.RequestId, cancelled.CleanupCompleted);
            case CaptureOutcome.Failed failed:
                return FailAndCleanup(failed.Failure, failed.CleanupCompleted);
            case CaptureOutcome.Succeeded succeeded:
                return await HandleSuccessAsync(intent, succeeded.ImageResult, clipboardService, cancellationToken);
            default:
                return FailAndCleanup(Failure.Create(
                    FailureCode.UnexpectedFailure,
                    FailureCategory.Unexpected,
                    FailureRecoverability.TerminalForSession,
                    "CaptureWorkflowCoordinator.CaptureOutcome",
                    intent.RequestId,
                    "Unknown capture outcome."), false);
        }
    }

    private async ValueTask<WorkflowRunResult> HandleSuccessAsync(
        CaptureIntent intent,
        IImageResult imageResult,
        IClipboardDeliveryService? clipboardService,
        CancellationToken cancellationToken)
    {
        if (!TryTransition(WorkflowState.Capturing, WorkflowState.ResultReady, intent.RequestId, out var transitionFailure))
        {
            imageResult.Dispose();
            return TerminalFailure(transitionFailure!);
        }

        if (clipboardService is null)
        {
            return CompleteAndCleanup(imageResult, intent.RequestId);
        }

        if (!TryTransition(WorkflowState.ResultReady, WorkflowState.Delivering, intent.RequestId, out transitionFailure))
        {
            imageResult.Dispose();
            return TerminalFailure(transitionFailure!);
        }

        ClipboardDeliveryResult deliveryResult;
        var request = new ClipboardDeliveryRequest
        {
            DeliveryId = Guid.NewGuid(),
            SessionId = intent.SessionId,
            ResultId = imageResult.Metadata.ResultId,
            ImageResult = imageResult,
            HistoryAllowed = false,
            RoamingAllowed = false,
            MaximumAttempts = 5,
            RetryBudget = TimeSpan.FromSeconds(1),
            Cancellation = cancellationToken
        };

        try
        {
            deliveryResult = await clipboardService.DeliverAsync(request, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            deliveryResult = new ClipboardDeliveryResult.Cancelled(
                request.DeliveryId,
                request.SessionId,
                request.ResultId,
                "CancellationToken");
        }
        catch (Exception exception)
        {
            deliveryResult = new ClipboardDeliveryResult.TerminalFailure(
                request.DeliveryId,
                request.SessionId,
                request.ResultId,
                Failure.Create(
                    FailureCode.UnexpectedFailure,
                    FailureCategory.Unexpected,
                    FailureRecoverability.TerminalForSession,
                    "CaptureWorkflowCoordinator.Clipboard",
                    request.DeliveryId,
                    exception.GetType().Name));
        }

        switch (deliveryResult)
        {
            case ClipboardDeliveryResult.Delivered delivered:
                return CompleteAndCleanup(imageResult, intent.RequestId);
            case ClipboardDeliveryResult.RetryableFailure retryable:
                if (!TryTransition(WorkflowState.Delivering, WorkflowState.ResultReady, intent.RequestId, out transitionFailure))
                {
                    imageResult.Dispose();
                    return TerminalFailure(transitionFailure!);
                }

                return new WorkflowRunResult(
                    WorkflowOutcomeKind.RetryableFailure,
                    WorkflowState.ResultReady,
                    imageResult,
                    retryable.Failure,
                    true);
            case ClipboardDeliveryResult.Cancelled cancelled:
                imageResult.Dispose();
                return Cancelled(intent.RequestId, true);
            case ClipboardDeliveryResult.TerminalFailure terminal:
                imageResult.Dispose();
                return FailAndCleanup(terminal.Failure, true);
            default:
                imageResult.Dispose();
                return FailAndCleanup(Failure.Create(
                    FailureCode.UnexpectedFailure,
                    FailureCategory.Unexpected,
                    FailureRecoverability.TerminalForSession,
                    "CaptureWorkflowCoordinator.ClipboardResult",
                    intent.RequestId,
                    "Unknown clipboard outcome."), false);
        }
    }

    private WorkflowRunResult CompleteAndCleanup(IImageResult imageResult, Guid correlationId)
    {
        if (!TryTransition(WorkflowState.Delivering, WorkflowState.Completed, correlationId, out var failure)
            && StateAuthority.CurrentState == WorkflowState.ResultReady)
        {
            TryTransition(WorkflowState.ResultReady, WorkflowState.Completed, correlationId, out failure);
        }

        imageResult.Dispose();
        var cleanup = TryTransition(WorkflowState.Completed, WorkflowState.Idle, correlationId, out failure);
        return new WorkflowRunResult(
            WorkflowOutcomeKind.Completed,
            StateAuthority.CurrentState,
            null,
            failure,
            cleanup);
    }

    private WorkflowRunResult Cancelled(Guid correlationId, bool cleanupCompleted)
    {
        var current = StateAuthority.CurrentState;
        if (current is not WorkflowState.Cancelled)
        {
            TryTransition(current, WorkflowState.Cancelled, correlationId, out _);
        }

        var cleanup = TryTransition(WorkflowState.Cancelled, WorkflowState.Idle, correlationId, out _);
        return new WorkflowRunResult(
            WorkflowOutcomeKind.Cancelled,
            StateAuthority.CurrentState,
            null,
            null,
            cleanup && cleanupCompleted);
    }

    private WorkflowRunResult FailAndCleanup(Failure failure, bool cleanupCompleted)
    {
        var current = StateAuthority.CurrentState;
        if (current is not WorkflowState.Failed)
        {
            TryTransition(current, WorkflowState.Failed, failure.CorrelationId, out _);
        }

        var cleanup = TryTransition(WorkflowState.Failed, WorkflowState.Idle, failure.CorrelationId, out _);
        return new WorkflowRunResult(
            WorkflowOutcomeKind.TerminalFailure,
            StateAuthority.CurrentState,
            null,
            failure,
            cleanup && cleanupCompleted);
    }

    private WorkflowRunResult TerminalFailure(Failure failure) => FailAndCleanup(failure, false);

    private bool TryTransition(
        WorkflowState from,
        WorkflowState to,
        Guid correlationId,
        out Failure? failure)
    {
        var result = StateAuthority.RequestTransition(new WorkflowTransitionRequest(from, to, "CaptureWorkflowCoordinator"));
        failure = result.Failure;
        return result.IsSuccess;
    }
}
