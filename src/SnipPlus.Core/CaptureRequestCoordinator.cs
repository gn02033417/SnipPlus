using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class CaptureRequestCoordinator : ICaptureRequestBoundary, IDisposable
{
    private readonly object _gate = new();
    private readonly WorkflowStateAuthority _stateAuthority;
    private CaptureRequest? _activeRequest;
    private bool _disposed;

    public CaptureRequestCoordinator(WorkflowStateAuthority stateAuthority)
    {
        _stateAuthority = stateAuthority ?? throw new ArgumentNullException(nameof(stateAuthority));
    }

    public WorkflowState CurrentState => _stateAuthority.CurrentState;

    public CaptureRequest? ActiveRequest
    {
        get
        {
            lock (_gate)
            {
                return _activeRequest;
            }
        }
    }

    public CaptureRequestResult Submit(CaptureRequest request)
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return CaptureRequestResult.Rejected(
                    request,
                    _stateAuthority.CurrentState,
                    CaptureRequestRejectionReason.ApplicationExiting,
                    _activeRequest,
                    "Capture request rejected because the application is exiting.");
            }

            var currentState = _stateAuthority.CurrentState;
            if (currentState != WorkflowState.ResidentReady)
            {
                var reason = currentState == WorkflowState.CaptureRequested
                    ? CaptureRequestRejectionReason.Busy
                    : CaptureRequestRejectionReason.InvalidState;
                var message = reason == CaptureRequestRejectionReason.Busy
                    ? "Capture request rejected because another capture request is already active."
                    : $"Capture request rejected because the workflow is in {currentState}.";

                return CaptureRequestResult.Rejected(
                    request,
                    currentState,
                    reason,
                    _activeRequest,
                    message);
            }

            var transition = _stateAuthority.RequestTransition(new(
                WorkflowState.ResidentReady,
                WorkflowState.CaptureRequested,
                $"CaptureRequest:{request.RequestSource}"));
            if (!transition.IsSuccess)
            {
                return CaptureRequestResult.Rejected(
                    request,
                    transition.CurrentState,
                    CaptureRequestRejectionReason.InvalidState,
                    _activeRequest,
                    transition.Failure?.UserMessageKey
                        ?? "Capture request rejected because the workflow state is invalid.");
            }

            _activeRequest = request;
            return CaptureRequestResult.Accepted(
                request,
                transition.CurrentState,
                "Capture request accepted. Freezing is not implemented in this slice.");
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
