using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class WorkflowStateAuthority
{
    private readonly object _gate = new();
    private WorkflowState _currentState = WorkflowState.Idle;

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
            return WorkflowTransitionResult.Success(_currentState);
        }
    }

    private static bool IsLegal(WorkflowState from, WorkflowState to) => from switch
    {
        WorkflowState.Idle => to == WorkflowState.Starting,
        WorkflowState.Starting => to is WorkflowState.Selecting or WorkflowState.Cancelled or WorkflowState.Failed,
        WorkflowState.Selecting => to is WorkflowState.Capturing or WorkflowState.Cancelled or WorkflowState.Failed,
        WorkflowState.Capturing => to is WorkflowState.ResultReady or WorkflowState.Cancelled or WorkflowState.Failed,
        WorkflowState.ResultReady => to is WorkflowState.Delivering or WorkflowState.Cancelled or WorkflowState.Completed,
        WorkflowState.Delivering => to is WorkflowState.Completed or WorkflowState.Cancelled or WorkflowState.Failed or WorkflowState.ResultReady,
        WorkflowState.Completed or WorkflowState.Cancelled or WorkflowState.Failed => to == WorkflowState.Idle,
        _ => false
    };
}
