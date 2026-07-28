using SnipPlus.Contracts;

namespace SnipPlus.Core;

public abstract record CapturePresentationOutcome
{
    private CapturePresentationOutcome()
    {
    }

    public sealed record SelectingReady(
        CaptureSessionContext Session,
        SelectionVisualState Selection) : CapturePresentationOutcome;

    public sealed record Busy : CapturePresentationOutcome;

    public sealed record Cancelled(string CancellationOrigin) : CapturePresentationOutcome;

    public sealed record Failed(Failure Failure) : CapturePresentationOutcome;
}

public sealed class CapturePresentationWorkflowCoordinator : ISelectionInputSink, IDisposable
{
    private readonly object _gate = new();
    private readonly WorkflowStateAuthority _stateAuthority;
    private readonly CaptureFreezingCoordinator _freezingCoordinator;
    private readonly IAllDisplayOverlayPresentationCoordinator _overlayCoordinator;
    private readonly ICaptureSourceExclusion? _captureSourceExclusion;
    private readonly ICaptureAccessPreflight? _captureAccessPreflight;
    private CaptureSessionContext? _activeSession;
    private InitialSelectionCoordinator? _selectionCoordinator;
    private CancellationTokenSource? _sessionCancellation;
    private bool _startInProgress;
    private bool _inputEnabled;
    private bool _disposed;

    public CapturePresentationWorkflowCoordinator(
        CaptureFreezingCoordinator freezingCoordinator,
        IAllDisplayOverlayPresentationCoordinator overlayCoordinator,
        ICaptureSourceExclusion? captureSourceExclusion = null,
        ICaptureAccessPreflight? captureAccessPreflight = null)
    {
        _freezingCoordinator = freezingCoordinator
            ?? throw new ArgumentNullException(nameof(freezingCoordinator));
        _stateAuthority = freezingCoordinator.StateAuthority;
        _overlayCoordinator = overlayCoordinator
            ?? throw new ArgumentNullException(nameof(overlayCoordinator));
        _captureSourceExclusion = captureSourceExclusion;
        _captureAccessPreflight = captureAccessPreflight;
    }

    public WorkflowState CurrentState => _stateAuthority.CurrentState;

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

    public SelectionVisualState? CurrentSelection
    {
        get
        {
            lock (_gate)
            {
                return _selectionCoordinator?.State;
            }
        }
    }

    public async ValueTask<CapturePresentationOutcome> StartAsync(
        CaptureRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        CancellationTokenSource sessionCancellation;
        lock (_gate)
        {
            if (_disposed || _activeSession is not null || _startInProgress)
            {
                return new CapturePresentationOutcome.Busy();
            }

            _startInProgress = true;
            _sessionCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            sessionCancellation = _sessionCancellation;
        }

        try
        {
            var token = sessionCancellation.Token;
            if (_captureAccessPreflight is not null)
            {
                var access = await _captureAccessPreflight
                    .EnsureAccessAsync(token)
                    .ConfigureAwait(true);
                if (access is not CaptureAccessPreflightOutcome.Allowed)
                {
                    return access switch
                    {
                        CaptureAccessPreflightOutcome.Cancelled cancelled =>
                            await CancelCurrentAsync(cancelled.CancellationOrigin)
                                .ConfigureAwait(true),
                        CaptureAccessPreflightOutcome.Failed failed =>
                            await FailBeforeSessionAsync(
                                    request,
                                    failed.Failure,
                                    cancelled: false)
                                .ConfigureAwait(true),
                        _ => await FailBeforeSessionAsync(
                                request,
                                CreateFailure(
                                    request,
                                    FailureCode.CapturePermissionDenied,
                                    "Capture access preflight returned an unknown outcome."),
                                cancelled: false)
                            .ConfigureAwait(true)
                    };
                }
            }

            if (_captureSourceExclusion is not null)
            {
                var exclusion = await _captureSourceExclusion
                    .ExcludeAsync(request, token)
                    .ConfigureAwait(true);
                if (!exclusion.IsExcluded)
                {
                    return await FailBeforeSessionAsync(
                        request,
                        exclusion.Failure ?? CreateFailure(
                            request,
                            FailureCode.CaptureSourceUnavailable,
                            "The capture source could not exclude the SnipPlus window."),
                        exclusion.Kind == CaptureSourceExclusionKind.Cancelled)
                        .ConfigureAwait(true);
                }
            }

            var started = await _freezingCoordinator
                .BeginFreezingAsync(request, token)
                .ConfigureAwait(true);
            if (started is not CaptureFreezingOutcome.FreezingStarted freezingStarted)
            {
                return await HandleFreezingFailureAsync(request, started)
                    .ConfigureAwait(true);
            }

            var acquired = await _freezingCoordinator
                .AcquireFrozenFramesAsync(freezingStarted.Session, token)
                .ConfigureAwait(true);
            if (acquired is not CaptureFreezingOutcome.FrozenFrameSetReady ready)
            {
                return await HandleFreezingFailureAsync(request, acquired)
                    .ConfigureAwait(true);
            }

            if (!FrozenDisplayOverlayPlanBuilder.TryCreate(
                    ready.Session,
                    out var plan,
                    out var planFailure)
                || plan is null)
            {
                return await FailSessionAsync(ready.Session, planFailure!)
                    .ConfigureAwait(true);
            }

            var selection = new InitialSelectionCoordinator(ready.Session);
            selection.StateChanged += OnSelectionStateChanged;
            var cancelBeforePresentation = false;
            lock (_gate)
            {
                if (_disposed || sessionCancellation.IsCancellationRequested)
                {
                    cancelBeforePresentation = true;
                }
                else
                {
                    _activeSession = ready.Session;
                    _selectionCoordinator = selection;
                }
            }

            if (cancelBeforePresentation)
            {
                selection.Dispose();
                return await CancelSessionAsync(
                    ready.Session,
                    "CancellationToken").ConfigureAwait(true);
            }

            var presentation = await _overlayCoordinator
                .PresentAsync(
                    new FrozenDisplayOverlayPresentationRequest(plan, this),
                    token)
                .ConfigureAwait(true);
            if (presentation is not FrozenDisplayOverlayPresentationOutcome.Ready)
            {
                return await FailPresentationAsync(ready.Session, presentation)
                    .ConfigureAwait(true);
            }

            var transition = _stateAuthority.RequestTransition(new(
                WorkflowState.Freezing,
                WorkflowState.Selecting,
                "AllDisplayFrozenPresentationReady"));
            if (!transition.IsSuccess)
            {
                return await FailSessionAsync(
                        ready.Session,
                        transition.Failure ?? CreateFailure(
                            request,
                            FailureCode.InvalidStateTransition,
                            "The workflow could not enter Selection after all overlays became ready."))
                    .ConfigureAwait(true);
            }

            lock (_gate)
            {
                _inputEnabled = true;
            }

            _overlayCoordinator.ApplySelection(selection.State);
            return new CapturePresentationOutcome.SelectingReady(
                ready.Session,
                selection.State);
        }
        catch (OperationCanceledException)
        {
            return await CancelCurrentAsync("CancellationToken")
                .ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            var failure = CreateFailure(
                request,
                FailureCode.UnexpectedFailure,
                $"{exception.GetType().Name}: {exception.Message}",
                exception.HResult);
            return await FailCurrentAsync(failure).ConfigureAwait(true);
        }
        finally
        {
            lock (_gate)
            {
                _startInProgress = false;
            }
        }
    }

    public SelectionInputResult PointerPressed(SelectionPointerEvent input) =>
        ForwardSelectionInput(input, static (selection, value) => selection.PointerPressed(value));

    public SelectionInputResult PointerMoved(SelectionPointerEvent input) =>
        ForwardSelectionInput(input, static (selection, value) => selection.PointerMoved(value));

    public SelectionInputResult PointerReleased(SelectionPointerEvent input)
    {
        var result = ForwardSelectionInput(
            input,
            static (selection, value) => selection.PointerReleased(value));
        if (result.Kind == SelectionInputResultKind.Locked)
        {
            var transition = _stateAuthority.RequestTransition(new(
                WorkflowState.Selecting,
                WorkflowState.SelectionLocked,
                "InitialSelectionPointerReleased"));
            if (!transition.IsSuccess)
            {
                Observe(FailCurrentAsync(transition.Failure ?? CreateFailure(
                    input.SessionId,
                    FailureCode.InvalidStateTransition,
                    "The valid Selection could not be locked.")));
            }
        }

        return result;
    }

    public SelectionInputResult Escape(Guid sessionId, string coordinateVersion)
    {
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _selectionCoordinator;
            if (_disposed
                || _activeSession?.SessionId != sessionId
                || !string.Equals(
                    _activeSession.VirtualDesktopSnapshot.CoordinateVersion,
                    coordinateVersion,
                    StringComparison.Ordinal))
            {
                return new SelectionInputResult(
                    SelectionInputResultKind.StaleSession,
                    selection?.State ?? SelectionVisualState.Initial(sessionId, coordinateVersion),
                    "Selection input was ignored.");
            }
        }

        var result = selection?.Escape(sessionId, coordinateVersion)
            ?? new SelectionInputResult(
                SelectionInputResultKind.StaleSession,
                SelectionVisualState.Initial(sessionId, coordinateVersion),
                "Selection input was ignored.");
        if (result.Kind == SelectionInputResultKind.Cancelled)
        {
            Observe(CancelCurrentAsync("Escape"));
        }

        return result;
    }

    public async ValueTask<CapturePresentationOutcome> CancelCurrentAsync(string cancellationOrigin)
    {
        CaptureSessionContext? session;
        CancellationTokenSource? sessionCancellation;
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            if (_disposed && _activeSession is null)
            {
                return new CapturePresentationOutcome.Cancelled(cancellationOrigin);
            }

            _inputEnabled = false;
            session = _activeSession;
            sessionCancellation = _sessionCancellation;
            selection = _selectionCoordinator;
            _activeSession = null;
            _selectionCoordinator = null;
        }

        sessionCancellation?.Cancel();
        if (session is not null)
        {
            await _overlayCoordinator
                .CloseAsync(session.SessionId, CancellationToken.None)
                .ConfigureAwait(true);
            _freezingCoordinator.ReleaseSession(session);
            session.Cancel();
        }

        selection?.Dispose();
        MoveToResidentReady(WorkflowState.Cancelled, cancellationOrigin);
        DisposeSessionCancellation(sessionCancellation);
        return new CapturePresentationOutcome.Cancelled(cancellationOrigin);
    }

    public void Dispose()
    {
        CancellationTokenSource? cancellation;
        CaptureSessionContext? session;
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _inputEnabled = false;
            cancellation = _sessionCancellation;
            session = _activeSession;
            selection = _selectionCoordinator;
            _sessionCancellation = null;
            _activeSession = null;
            _selectionCoordinator = null;
        }

        cancellation?.Cancel();
        if (session is not null)
        {
            Observe(_overlayCoordinator.CloseAsync(session.SessionId, CancellationToken.None));
            _freezingCoordinator.ReleaseSession(session);
            session.Dispose();
        }

        selection?.Dispose();
        cancellation?.Dispose();
        _overlayCoordinator.Dispose();
        _freezingCoordinator.Dispose();
        GC.SuppressFinalize(this);
    }

    private SelectionInputResult ForwardSelectionInput(
        SelectionPointerEvent input,
        Func<InitialSelectionCoordinator, SelectionPointerEvent, SelectionInputResult> handler)
    {
        ArgumentNullException.ThrowIfNull(input);
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _inputEnabled ? _selectionCoordinator : null;
        }

        return selection is null
            ? new SelectionInputResult(
                SelectionInputResultKind.Ignored,
                CurrentSelection ?? SelectionVisualState.Initial(
                    input.SessionId,
                    input.CoordinateVersion),
                "Selection input was ignored until all overlays were ready.")
            : handler(selection, input);
    }

    private void OnSelectionStateChanged(SelectionVisualState state) =>
        _overlayCoordinator.ApplySelection(state);

    private async ValueTask<CapturePresentationOutcome> HandleFreezingFailureAsync(
        CaptureRequest request,
        CaptureFreezingOutcome outcome)
    {
        switch (outcome)
        {
            case CaptureFreezingOutcome.Cancelled cancelled:
                return await CancelCurrentAsync(cancelled.CancellationOrigin)
                    .ConfigureAwait(true);
            case CaptureFreezingOutcome.FrameFailed failed:
                return await FailCurrentAsync(failed.Failure)
                    .ConfigureAwait(true);
            case CaptureFreezingOutcome.TopologyInvalid invalid:
                return await FailCurrentAsync(invalid.Failure)
                    .ConfigureAwait(true);
            case CaptureFreezingOutcome.UnsupportedCapacity unsupported:
                {
                    var failure = CreateFailure(
                        request,
                        FailureCode.UnsupportedCapacity,
                        unsupported.Validation.UserMessage);
                    return await FailCurrentAsync(failure).ConfigureAwait(true);
                }
            default:
                {
                    var failure = CreateFailure(
                        request,
                        FailureCode.UnexpectedFailure,
                        "Freezing returned an unexpected outcome.");
                    return await FailCurrentAsync(failure).ConfigureAwait(true);
                }
        }
    }

    private async ValueTask<CapturePresentationOutcome> FailBeforeSessionAsync(
        CaptureRequest request,
        Failure failure,
        bool cancelled)
    {
        if (cancelled)
        {
            return await CancelCurrentAsync("CancellationToken").ConfigureAwait(true);
        }

        await FailCurrentAsync(failure).ConfigureAwait(true);
        return new CapturePresentationOutcome.Failed(failure);
    }

    private async ValueTask<CapturePresentationOutcome> FailSessionAsync(
        CaptureSessionContext session,
        Failure failure)
    {
        await FailCurrentAsync(failure, session).ConfigureAwait(true);
        return new CapturePresentationOutcome.Failed(failure);
    }

    private async ValueTask<CapturePresentationOutcome> FailPresentationAsync(
        CaptureSessionContext session,
        FrozenDisplayOverlayPresentationOutcome outcome)
    {
        if (outcome is FrozenDisplayOverlayPresentationOutcome.Cancelled cancelled)
        {
            return await CancelCurrentAsync(cancelled.CancellationOrigin).ConfigureAwait(true);
        }

        var failure = outcome is FrozenDisplayOverlayPresentationOutcome.Failed failed
            ? failed.Failure
            : CreateFailure(
                session.RequestId,
                FailureCode.OverlayPresentationFailed,
                "The display overlays could not be presented.");
        await FailCurrentAsync(failure, session).ConfigureAwait(true);
        return new CapturePresentationOutcome.Failed(failure);
    }

    private async ValueTask<CapturePresentationOutcome> FailCurrentAsync(
        Failure failure,
        CaptureSessionContext? expectedSession = null)
    {
        CaptureSessionContext? session;
        InitialSelectionCoordinator? selection;
        CancellationTokenSource? cancellation;
        lock (_gate)
        {
            session = expectedSession ?? _activeSession;
            if (expectedSession is not null && !ReferenceEquals(expectedSession, _activeSession))
            {
                session = expectedSession;
            }

            _inputEnabled = false;
            selection = _selectionCoordinator;
            cancellation = _sessionCancellation;
            _activeSession = null;
            _selectionCoordinator = null;
        }

        cancellation?.Cancel();
        if (session is not null)
        {
            try
            {
                await _overlayCoordinator
                    .CloseAsync(session.SessionId, CancellationToken.None)
                    .ConfigureAwait(true);
            }
            catch (Exception cleanupException)
            {
                failure = failure with
                {
                    DiagnosticMessage = $"{failure.DiagnosticMessage}; cleanup {cleanupException.GetType().Name}: {cleanupException.Message}",
                    NativeCode = failure.NativeCode ?? cleanupException.HResult
                };
            }

            _freezingCoordinator.ReleaseSession(session);
            session.MarkFailedAndDispose();
        }

        selection?.Dispose();
        MoveToResidentReady(WorkflowState.Failed, failure.UserMessageKey);
        DisposeSessionCancellation(cancellation);
        return new CapturePresentationOutcome.Failed(failure);
    }

    private async ValueTask<CapturePresentationOutcome> CancelSessionAsync(
        CaptureSessionContext session,
        string origin)
    {
        await _overlayCoordinator
            .CloseAsync(session.SessionId, CancellationToken.None)
            .ConfigureAwait(true);
        _freezingCoordinator.ReleaseSession(session);
        session.Cancel();
        MoveToResidentReady(WorkflowState.Cancelled, origin);
        return new CapturePresentationOutcome.Cancelled(origin);
    }

    private void MoveToResidentReady(WorkflowState terminalState, string reason)
    {
        var current = _stateAuthority.CurrentState;
        if (current != terminalState
            && current != WorkflowState.ResidentReady)
        {
            _stateAuthority.RequestTransition(new(current, terminalState, reason));
        }

        if (_stateAuthority.CurrentState == terminalState)
        {
            _stateAuthority.RequestTransition(new(
                terminalState,
                WorkflowState.ResidentReady,
                $"CaptureCleanup:{reason}"));
        }
    }

    private void DisposeSessionCancellation(CancellationTokenSource? cancellation)
    {
        lock (_gate)
        {
            if (ReferenceEquals(_sessionCancellation, cancellation))
            {
                _sessionCancellation = null;
            }
        }

        cancellation?.Dispose();
    }

    private static Failure CreateFailure(
        CaptureRequest request,
        FailureCode code,
        string message,
        int? nativeCode = null) => CreateFailure(
        request.RequestId,
        code,
        message,
        nativeCode);

    private static Failure CreateFailure(
        Guid correlationId,
        FailureCode code,
        string message,
        int? nativeCode = null) => Failure.Create(
        code,
        FailureCategory.Session,
        FailureRecoverability.RetryNewIntent,
        "CapturePresentationWorkflowCoordinator",
        correlationId,
        message,
        nativeCode: nativeCode);

    private static void Observe(ValueTask<CapturePresentationOutcome> operation) =>
        _ = operation.AsTask();

    private static void Observe(ValueTask operation) =>
        _ = operation.AsTask();
}
