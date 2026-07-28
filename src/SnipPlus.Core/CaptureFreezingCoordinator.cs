using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class CaptureFreezingCoordinator : IFreezingBoundary, IDisposable
{
    private readonly object _gate = new();
    private readonly CaptureRequestCoordinator _requestCoordinator;
    private readonly WorkflowStateAuthority _stateAuthority;
    private readonly IDisplayTopologyProvider _topologyProvider;
    private readonly IFrozenDisplayFrameProvider _frameProvider;
    private readonly IForegroundContextProvider? _foregroundContextProvider;
    private readonly SupportedCapacityPolicy _capacityPolicy;
    private CaptureSessionContext? _activeSession;
    private bool _beginInProgress;
    private bool _disposed;

    public CaptureFreezingCoordinator(
        CaptureRequestCoordinator requestCoordinator,
        IDisplayTopologyProvider topologyProvider,
        IFrozenDisplayFrameProvider frameProvider,
        IForegroundContextProvider? foregroundContextProvider = null,
        SupportedCapacityPolicy? capacityPolicy = null)
    {
        _requestCoordinator = requestCoordinator ?? throw new ArgumentNullException(nameof(requestCoordinator));
        _stateAuthority = requestCoordinator.StateAuthority;
        _topologyProvider = topologyProvider ?? throw new ArgumentNullException(nameof(topologyProvider));
        _frameProvider = frameProvider ?? throw new ArgumentNullException(nameof(frameProvider));
        _foregroundContextProvider = foregroundContextProvider;
        _capacityPolicy = capacityPolicy ?? new SupportedCapacityPolicy();
    }

    public CaptureSessionContext? ActiveSession
    {
        get
        {
            lock (_gate)
            {
                return _activeSession;
            }
        }
    }

    public WorkflowStateAuthority StateAuthority => _stateAuthority;

    public bool ReleaseSession(CaptureSessionContext session)
    {
        ArgumentNullException.ThrowIfNull(session);
        lock (_gate)
        {
            if (!ReferenceEquals(_activeSession, session))
            {
                return false;
            }

            _activeSession = null;
            return true;
        }
    }

    public async ValueTask<CaptureFreezingOutcome> BeginFreezingAsync(
        CaptureRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        lock (_gate)
        {
            if (_disposed || _requestCoordinator.IsDisposed)
            {
                return new CaptureFreezingOutcome.Cancelled(
                    request.RequestId,
                    null,
                    "ApplicationExiting");
            }

            if (_activeSession is not null)
            {
                return new CaptureFreezingOutcome.AlreadyStarted(
                    request.RequestId,
                    _activeSession.SessionId,
                    "Freezing has already started for the active capture request.");
            }

            if (_beginInProgress)
            {
                return new CaptureFreezingOutcome.Busy(
                    request.RequestId,
                    "Another request is already establishing the frozen capture session.");
            }

            if (!_requestCoordinator.IsActive(request))
            {
                return new CaptureFreezingOutcome.StaleRequest(
                    request.RequestId,
                    "The capture request is not the active accepted request.");
            }

            if (_stateAuthority.CurrentState != WorkflowState.CaptureRequested)
            {
                return new CaptureFreezingOutcome.AlreadyStarted(
                    request.RequestId,
                    null,
                    $"Freezing cannot start from workflow state {_stateAuthority.CurrentState}.");
            }

            _beginInProgress = true;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var topologyOutcome = await _topologyProvider
                .GetSnapshotAsync(request, cancellationToken)
                .ConfigureAwait(false);

            switch (topologyOutcome)
            {
                case DisplayTopologyOutcome.Cancelled cancelled:
                    return new CaptureFreezingOutcome.Cancelled(
                        request.RequestId,
                        null,
                        cancelled.CancellationOrigin);
                case DisplayTopologyOutcome.Invalid invalid:
                    return new CaptureFreezingOutcome.TopologyInvalid(invalid.Failure);
                case DisplayTopologyOutcome.Succeeded succeeded:
                    {
                        var capacity = _capacityPolicy.ValidateTopology(succeeded.Snapshot);
                        if (!capacity.IsSupported)
                        {
                            return new CaptureFreezingOutcome.UnsupportedCapacity(capacity);
                        }

                        ForegroundContextReference? foregroundContext = null;
                        if (_foregroundContextProvider is not null)
                        {
                            foregroundContext = await _foregroundContextProvider
                                .CaptureAsync(request, cancellationToken)
                                .ConfigureAwait(false);
                        }

                        var session = new CaptureSessionContext(
                            request,
                            succeeded.Snapshot,
                            capacity,
                            foregroundContext,
                            cancellationToken);

                        lock (_gate)
                        {
                            if (_disposed || _requestCoordinator.IsDisposed)
                            {
                                session.Dispose();
                                return new CaptureFreezingOutcome.Cancelled(
                                    request.RequestId,
                                    null,
                                    "ApplicationExiting");
                            }

                            if (!_requestCoordinator.IsActive(request))
                            {
                                session.Dispose();
                                return new CaptureFreezingOutcome.StaleRequest(
                                    request.RequestId,
                                    "The accepted capture request is no longer active.");
                            }

                            var transition = _stateAuthority.RequestTransition(new(
                                WorkflowState.CaptureRequested,
                                WorkflowState.Freezing,
                                $"CaptureFreezing:{request.RequestSource}"));
                            if (!transition.IsSuccess)
                            {
                                session.Dispose();
                                return new CaptureFreezingOutcome.AlreadyStarted(
                                    request.RequestId,
                                    null,
                                    transition.Failure?.UserMessageKey
                                        ?? "Freezing could not start because the workflow state changed.");
                            }

                            _activeSession = session;
                            return new CaptureFreezingOutcome.FreezingStarted(session);
                        }
                    }
                default:
                    return new CaptureFreezingOutcome.TopologyInvalid(CreateFailure(
                        FailureCode.UnexpectedFailure,
                        FailureCategory.Unexpected,
                        FailureRecoverability.RetryNewIntent,
                        request.RequestId,
                        "Display topology provider returned an unknown outcome."));
            }
        }
        catch (OperationCanceledException)
        {
            return new CaptureFreezingOutcome.Cancelled(
                request.RequestId,
                null,
                "CancellationToken");
        }
        catch (Exception exception)
        {
            return new CaptureFreezingOutcome.TopologyInvalid(CreateFailure(
                FailureCode.UnexpectedFailure,
                FailureCategory.Unexpected,
                FailureRecoverability.RetryNewIntent,
                request.RequestId,
                exception.GetType().Name,
                exception.HResult));
        }
        finally
        {
            lock (_gate)
            {
                _beginInProgress = false;
            }
        }
    }

    public async ValueTask<CaptureFreezingOutcome> AcquireFrozenFramesAsync(
        CaptureSessionContext session,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);

        lock (_gate)
        {
            if (_disposed || _requestCoordinator.IsDisposed)
            {
                return new CaptureFreezingOutcome.Cancelled(
                    session.RequestId,
                    session.SessionId,
                    "ApplicationExiting");
            }

            if (!ReferenceEquals(_activeSession, session)
                || session.IsDisposed
                || session.Status is CaptureSessionStatus.Cancelled
                    or CaptureSessionStatus.Failed
                    or CaptureSessionStatus.Disposed)
            {
                return new CaptureFreezingOutcome.StaleRequest(
                    session.RequestId,
                    "The capture session is no longer the active freezing session.");
            }

            if (session.FrozenDisplayFrames is not null
                || session.Status == CaptureSessionStatus.FrozenFrameSetReady)
            {
                return new CaptureFreezingOutcome.AlreadyStarted(
                    session.RequestId,
                    session.SessionId,
                    "Frozen display frames have already been acquired for this session.");
            }
        }

        if (_frameProvider is IAllDisplayFrameProvider allDisplayFrameProvider)
        {
            return await AcquireAllFramesAsync(
                session,
                allDisplayFrameProvider,
                cancellationToken).ConfigureAwait(false);
        }

        var acquiredFrames = new List<FrozenDisplayFrame>(session.VirtualDesktopSnapshot.Displays.Count);
        try
        {
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                session.Cancellation,
                cancellationToken);

            foreach (var display in session.VirtualDesktopSnapshot.Displays)
            {
                var outcome = await _frameProvider
                    .AcquireAsync(session, display, linkedCancellation.Token)
                    .ConfigureAwait(false);
                switch (outcome)
                {
                    case FrozenDisplayFrameAcquisitionOutcome.Succeeded succeeded:
                        acquiredFrames.Add(succeeded.Frame);
                        break;
                    case FrozenDisplayFrameAcquisitionOutcome.Cancelled cancelled:
                        DisposeFrames(acquiredFrames);
                        session.Cancel();
                        ClearActiveSession(session);
                        return new CaptureFreezingOutcome.Cancelled(
                            session.RequestId,
                            session.SessionId,
                            cancelled.CancellationOrigin);
                    case FrozenDisplayFrameAcquisitionOutcome.Failed failed:
                        DisposeFrames(acquiredFrames);
                        session.MarkFailedAndDispose();
                        ClearActiveSession(session);
                        return new CaptureFreezingOutcome.FrameFailed(
                            session.SessionId,
                            failed.Failure,
                            true);
                    default:
                        DisposeFrames(acquiredFrames);
                        session.MarkFailedAndDispose();
                        ClearActiveSession(session);
                        return new CaptureFreezingOutcome.FrameFailed(
                            session.SessionId,
                            CreateFailure(
                                FailureCode.UnexpectedFailure,
                                FailureCategory.Unexpected,
                                FailureRecoverability.RetryNewIntent,
                                session.RequestId,
                                "Frozen display frame provider returned an unknown outcome."),
                            true);
                }
            }

            if (!FrozenDisplayFrameSet.TryCreate(
                    session,
                    session.VirtualDesktopSnapshot.Displays,
                    acquiredFrames,
                    out var frameSet,
                    out var validation)
                || frameSet is null)
            {
                session.MarkFailedAndDispose();
                ClearActiveSession(session);
                return new CaptureFreezingOutcome.FrameFailed(
                    session.SessionId,
                    CreateFailure(
                        FailureCode.InvalidCaptureIntent,
                        FailureCategory.Validation,
                        FailureRecoverability.RetryNewIntent,
                        session.RequestId,
                        validation.Message),
                    true);
            }

            if (!session.TryAttachFrozenDisplayFrames(frameSet))
            {
                frameSet.Dispose();
                session.MarkFailedAndDispose();
                ClearActiveSession(session);
                return new CaptureFreezingOutcome.FrameFailed(
                    session.SessionId,
                    CreateFailure(
                        FailureCode.InvalidCaptureIntent,
                        FailureCategory.Session,
                        FailureRecoverability.RetryNewIntent,
                        session.RequestId,
                        "Frozen display frame set could not be attached to its session."),
                    true);
            }

            return new CaptureFreezingOutcome.FrozenFrameSetReady(session);
        }
        catch (OperationCanceledException)
        {
            DisposeFrames(acquiredFrames);
            session.Cancel();
            ClearActiveSession(session);
            return new CaptureFreezingOutcome.Cancelled(
                session.RequestId,
                session.SessionId,
                "CancellationToken");
        }
        catch (Exception exception)
        {
            DisposeFrames(acquiredFrames);
            session.MarkFailedAndDispose();
            ClearActiveSession(session);
            return new CaptureFreezingOutcome.FrameFailed(
                session.SessionId,
                CreateFailure(
                    FailureCode.UnexpectedFailure,
                    FailureCategory.Unexpected,
                    FailureRecoverability.RetryNewIntent,
                    session.RequestId,
                    exception.GetType().Name,
                    exception.HResult),
                true);
        }
    }

    public void Dispose()
    {
        CaptureSessionContext? session;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            session = _activeSession;
            _activeSession = null;
        }

        session?.Dispose();
        if (_frameProvider is IDisposable disposableFrameProvider)
        {
            disposableFrameProvider.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    private async ValueTask<CaptureFreezingOutcome> AcquireAllFramesAsync(
        CaptureSessionContext session,
        IAllDisplayFrameProvider frameProvider,
        CancellationToken cancellationToken)
    {
        try
        {
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                session.Cancellation,
                cancellationToken);
            var outcome = await frameProvider
                .AcquireAllAsync(session, linkedCancellation.Token)
                .ConfigureAwait(false);

            switch (outcome)
            {
                case FrozenDisplayFrameSetAcquisitionOutcome.Succeeded succeeded:
                    if (!session.TryAttachFrozenDisplayFrames(succeeded.FrameSet))
                    {
                        succeeded.FrameSet.Dispose();
                        session.MarkFailedAndDispose();
                        ClearActiveSession(session);
                        return new CaptureFreezingOutcome.FrameFailed(
                            session.SessionId,
                            CreateFailure(
                                FailureCode.InvalidCaptureIntent,
                                FailureCategory.Session,
                                FailureRecoverability.RetryNewIntent,
                                session.RequestId,
                                "Frozen display frame set could not be attached to its session."),
                            true);
                    }

                    return new CaptureFreezingOutcome.FrozenFrameSetReady(session);
                case FrozenDisplayFrameSetAcquisitionOutcome.Cancelled cancelled:
                    session.Cancel();
                    ClearActiveSession(session);
                    return new CaptureFreezingOutcome.Cancelled(
                        session.RequestId,
                        session.SessionId,
                        cancelled.CancellationOrigin);
                case FrozenDisplayFrameSetAcquisitionOutcome.Failed failed:
                    session.MarkFailedAndDispose();
                    ClearActiveSession(session);
                    return new CaptureFreezingOutcome.FrameFailed(
                        session.SessionId,
                        failed.Failure,
                        failed.CleanupCompleted);
                default:
                    session.MarkFailedAndDispose();
                    ClearActiveSession(session);
                    return new CaptureFreezingOutcome.FrameFailed(
                        session.SessionId,
                        CreateFailure(
                            FailureCode.PartialAcquisitionFailed,
                            FailureCategory.Resource,
                            FailureRecoverability.RetryNewIntent,
                            session.RequestId,
                            "All-display frame provider returned an unknown outcome."),
                        true);
            }
        }
        catch (OperationCanceledException)
        {
            session.Cancel();
            ClearActiveSession(session);
            return new CaptureFreezingOutcome.Cancelled(
                session.RequestId,
                session.SessionId,
                "CancellationToken");
        }
        catch (Exception exception)
        {
            session.MarkFailedAndDispose();
            ClearActiveSession(session);
            return new CaptureFreezingOutcome.FrameFailed(
                session.SessionId,
                CreateFailure(
                    FailureCode.PartialAcquisitionFailed,
                    FailureCategory.Resource,
                    FailureRecoverability.RetryNewIntent,
                    session.RequestId,
                    exception.GetType().Name,
                    exception.HResult),
                true);
        }
    }

    private void ClearActiveSession(CaptureSessionContext session)
    {
        lock (_gate)
        {
            if (ReferenceEquals(_activeSession, session))
            {
                _activeSession = null;
            }
        }
    }

    private static void DisposeFrames(IEnumerable<FrozenDisplayFrame> frames)
    {
        foreach (var frame in frames)
        {
            frame.Dispose();
        }
    }

    private static Failure CreateFailure(
        FailureCode code,
        FailureCategory category,
        FailureRecoverability recoverability,
        Guid correlationId,
        string diagnosticMessage,
        int? nativeCode = null) => Failure.Create(
            code,
            category,
            recoverability,
            "CaptureFreezingCoordinator",
            correlationId,
            diagnosticMessage,
            nativeCode: nativeCode);
}
