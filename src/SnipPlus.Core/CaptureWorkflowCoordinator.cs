using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class CaptureWorkflowCoordinator
{
    public CaptureWorkflowCoordinator(WorkflowStateAuthority stateAuthority)
    {
        StateAuthority = stateAuthority;
    }

    public WorkflowStateAuthority StateAuthority { get; }

    public async ValueTask<CaptureFrameOutcome> BeginSelectionAsync(
        CaptureIntent fullFrameIntent,
        ICaptureService captureService,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(captureService);

        if (!TryTransition(WorkflowState.Idle, WorkflowState.Starting, fullFrameIntent.RequestId, out var transitionFailure)
            || !TryTransition(WorkflowState.Starting, WorkflowState.Selecting, fullFrameIntent.RequestId, out transitionFailure))
        {
            return new CaptureFrameOutcome.Failed(
                fullFrameIntent.RequestId,
                fullFrameIntent.SessionId,
                transitionFailure!,
                false,
                true);
        }

        CaptureFrameOutcome frameOutcome;
        try
        {
            frameOutcome = await captureService.CaptureFrameAsync(fullFrameIntent, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            var cleanup = Cancelled(fullFrameIntent.RequestId, true);
            return new CaptureFrameOutcome.Cancelled(
                fullFrameIntent.RequestId,
                fullFrameIntent.SessionId,
                "CancellationToken",
                true,
                cleanup.CleanupCompleted);
        }
        catch (Exception exception)
        {
            var failure = Failure.Create(
                FailureCode.UnexpectedFailure,
                FailureCategory.Unexpected,
                FailureRecoverability.TerminalForSession,
                "CaptureWorkflowCoordinator.CaptureFrame",
                fullFrameIntent.RequestId,
                exception.GetType().Name,
                nativeCode: exception.HResult);
            var cleanup = FailAndCleanup(failure, false);
            return new CaptureFrameOutcome.Failed(
                fullFrameIntent.RequestId,
                fullFrameIntent.SessionId,
                failure,
                cleanup.CleanupCompleted,
                true);
        }

        switch (frameOutcome)
        {
            case CaptureFrameOutcome.Succeeded:
                return frameOutcome;
            case CaptureFrameOutcome.Cancelled cancelled:
            {
                var cleanup = Cancelled(fullFrameIntent.RequestId, cancelled.CleanupCompleted);
                return new CaptureFrameOutcome.Cancelled(
                    cancelled.RequestId,
                    cancelled.SessionId,
                    cancelled.CancellationOrigin,
                    cancelled.SourceSessionStarted,
                    cleanup.CleanupCompleted);
            }
            case CaptureFrameOutcome.Failed failed:
            {
                var cleanup = FailAndCleanup(failed.Failure, failed.CleanupCompleted);
                return new CaptureFrameOutcome.Failed(
                    failed.RequestId,
                    failed.SessionId,
                    failed.Failure,
                    cleanup.CleanupCompleted,
                    failed.RequiresNewIntent);
            }
            default:
            {
                var failure = Failure.Create(
                    FailureCode.UnexpectedFailure,
                    FailureCategory.Unexpected,
                    FailureRecoverability.TerminalForSession,
                    "CaptureWorkflowCoordinator.CaptureFrameOutcome",
                    fullFrameIntent.RequestId,
                    "Unknown capture frame outcome.");
                var cleanup = FailAndCleanup(failure, false);
                return new CaptureFrameOutcome.Failed(
                    fullFrameIntent.RequestId,
                    fullFrameIntent.SessionId,
                    failure,
                    cleanup.CleanupCompleted,
                    true);
            }
        }
    }

    public WorkflowRunResult CancelSelection(
        Guid correlationId,
        FrozenCaptureFrame? frozenFrame)
    {
        frozenFrame?.Dispose();
        return Cancelled(correlationId, true);
    }

    public async ValueTask<WorkflowRunResult> CompleteSelectionAsync(
        CaptureIntent intent,
        FrozenCaptureFrame frozenFrame,
        ICaptureService captureService,
        IClipboardDeliveryService? clipboardService,
        CancellationToken cancellationToken,
        Func<IImageResult, CancellationToken, ValueTask>? onResultReady = null)
    {
        ArgumentNullException.ThrowIfNull(frozenFrame);
        ArgumentNullException.ThrowIfNull(captureService);

        if (!TryTransition(WorkflowState.Selecting, WorkflowState.Capturing, intent.RequestId, out var transitionFailure))
        {
            frozenFrame.Dispose();
            return TerminalFailure(transitionFailure!);
        }

        CaptureOutcome captureOutcome;
        try
        {
            captureOutcome = await captureService.CropFrameAsync(intent, frozenFrame, cancellationToken);
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
                    "CaptureWorkflowCoordinator.CropFrame",
                    intent.RequestId,
                    exception.GetType().Name,
                    nativeCode: exception.HResult),
                false,
                true);
        }
        finally
        {
            frozenFrame.Dispose();
        }

        return captureOutcome switch
        {
            CaptureOutcome.Cancelled cancelled => Cancelled(intent.RequestId, cancelled.CleanupCompleted),
            CaptureOutcome.Failed failed => FailAndCleanup(failed.Failure, failed.CleanupCompleted),
            CaptureOutcome.Succeeded succeeded => await HandleSuccessAsync(
                intent,
                succeeded.ImageResult,
                clipboardService,
                onResultReady,
                cancellationToken),
            _ => FailAndCleanup(Failure.Create(
                FailureCode.UnexpectedFailure,
                FailureCategory.Unexpected,
                FailureRecoverability.TerminalForSession,
                "CaptureWorkflowCoordinator.CaptureOutcome",
                intent.RequestId,
                "Unknown crop outcome."), false)
        };
    }

    public async ValueTask<WorkflowRunResult> RunAsync(
        CaptureIntent intent,
        ICaptureService captureService,
        IClipboardDeliveryService? clipboardService,
        CancellationToken cancellationToken,
        Func<IImageResult, CancellationToken, ValueTask>? onResultReady = null)
    {
        var frameOutcome = await BeginSelectionAsync(intent, captureService, cancellationToken);
        return frameOutcome switch
        {
            CaptureFrameOutcome.Succeeded succeeded => await CompleteSelectionAsync(
                intent,
                succeeded.FrozenFrame,
                captureService,
                clipboardService,
                cancellationToken,
                onResultReady),
            CaptureFrameOutcome.Cancelled cancelled => new WorkflowRunResult(
                WorkflowOutcomeKind.Cancelled,
                StateAuthority.CurrentState,
                null,
                null,
                cancelled.CleanupCompleted),
            CaptureFrameOutcome.Failed failed => new WorkflowRunResult(
                WorkflowOutcomeKind.TerminalFailure,
                StateAuthority.CurrentState,
                null,
                failed.Failure,
                failed.CleanupCompleted),
            _ => throw new InvalidOperationException("Unknown capture frame outcome.")
        };
    }

    private async ValueTask<WorkflowRunResult> HandleSuccessAsync(
        CaptureIntent intent,
        IImageResult imageResult,
        IClipboardDeliveryService? clipboardService,
        Func<IImageResult, CancellationToken, ValueTask>? onResultReady,
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

        if (onResultReady is not null)
        {
            try
            {
                await onResultReady(imageResult, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                imageResult.Dispose();
                return Cancelled(intent.RequestId, true);
            }
            catch (Exception exception)
            {
                imageResult.Dispose();
                return FailAndCleanup(Failure.Create(
                    FailureCode.RenderingFailed,
                    FailureCategory.Resource,
                    FailureRecoverability.RetryNewIntent,
                    "CaptureWorkflowCoordinator.ResultReady",
                    intent.RequestId,
                    exception.GetType().Name,
                    nativeCode: exception.HResult), true);
            }
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
