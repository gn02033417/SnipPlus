using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class WorkflowStateAuthority
{
    private readonly object _gate = new();
    private WorkflowState _currentState = WorkflowState.ResidentReady;
    private int _successfulTransitionCount;

    public WorkflowState CurrentState
    {
        get
        {
            lock (_gate)
            {
                return _currentState;
            }
        }
    }

    public int SuccessfulTransitionCount
    {
        get
        {
            lock (_gate)
            {
                return _successfulTransitionCount;
            }
        }
    }

    public WorkflowTransitionResult RequestTransition(WorkflowTransitionRequest request)
    {
        lock (_gate)
        {
            if (request.From != _currentState || !IsLegal(request.From, request.To))
            {
                var failure = Failure.Create(
                    FailureCode.InvalidStateTransition,
                    FailureCategory.Validation,
                    FailureRecoverability.TerminalForSession,
                    "WorkflowStateAuthority.RequestTransition",
                    Guid.Empty,
                    $"Transition {request.From}->{request.To} is not legal from {_currentState}.");

                return WorkflowTransitionResult.Rejected(_currentState, failure);
            }

            _currentState = request.To;
            _successfulTransitionCount++;
            return WorkflowTransitionResult.Success(_currentState);
        }
    }

    private static bool IsLegal(WorkflowState from, WorkflowState to) => from switch
    {
        WorkflowState.ResidentReady => to == WorkflowState.CaptureRequested,
        WorkflowState.CaptureRequested => to is WorkflowState.Freezing
            or WorkflowState.Cancelled
            or WorkflowState.Failed,
        WorkflowState.Freezing => to is WorkflowState.Selecting
            or WorkflowState.Cancelled
            or WorkflowState.Failed,
        WorkflowState.Selecting => to is WorkflowState.SelectionLocked
            or WorkflowState.Cancelled
            or WorkflowState.Failed,
        WorkflowState.SelectionLocked => to is WorkflowState.Editing
            or WorkflowState.Cancelled
            or WorkflowState.Failed,
        WorkflowState.Editing => to == WorkflowState.Cancelled,
        WorkflowState.Cancelled => to == WorkflowState.ResidentReady,
        WorkflowState.Failed => to == WorkflowState.ResidentReady,
        _ => false
    };
}
