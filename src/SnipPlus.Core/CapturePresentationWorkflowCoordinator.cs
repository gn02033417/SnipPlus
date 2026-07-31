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

public sealed class CapturePresentationWorkflowCoordinator :
    ISelectionInputSink,
    IEditingInputRouter,
    IFunctionBarCommandSink,
    IDisposable
{
    private readonly object _gate = new();
    private readonly WorkflowStateAuthority _stateAuthority;
    private readonly CaptureFreezingCoordinator _freezingCoordinator;
    private readonly IAllDisplayOverlayPresentationCoordinator _overlayCoordinator;
    private readonly ICaptureSourceExclusion? _captureSourceExclusion;
    private readonly ICaptureAccessPreflight? _captureAccessPreflight;
    private readonly IFunctionBarPresentationCoordinator? _functionBarPresentation;
    private readonly IFrozenDisplayFrameSetRenderer? _finalRenderer;
    private readonly IClipboardDeliveryService? _clipboardDelivery;
    private readonly Action<string>? _feedback;
    private readonly ICompleteExecutionTraceSink _trace;
    private readonly AnnotationDocumentCoordinator _annotationDocuments;
    private readonly AnnotationEditingCoordinator _annotationEditing;
    private CaptureSessionContext? _activeSession;
    private InitialSelectionCoordinator? _selectionCoordinator;
    private CancellationTokenSource? _sessionCancellation;
    private bool _startInProgress;
    private bool _inputEnabled;
    private bool _completeInProgress;
    private bool _disposed;

    public CapturePresentationWorkflowCoordinator(
        CaptureFreezingCoordinator freezingCoordinator,
        IAllDisplayOverlayPresentationCoordinator overlayCoordinator,
        ICaptureSourceExclusion? captureSourceExclusion = null,
        ICaptureAccessPreflight? captureAccessPreflight = null,
        IFunctionBarPresentationCoordinator? functionBarPresentation = null,
        IFrozenDisplayFrameSetRenderer? finalRenderer = null,
        IClipboardDeliveryService? clipboardDelivery = null,
        Action<string>? feedback = null,
        ICompleteExecutionTraceSink? traceSink = null,
        AnnotationDocumentCoordinator? annotationDocuments = null)
    {
        _freezingCoordinator = freezingCoordinator
            ?? throw new ArgumentNullException(nameof(freezingCoordinator));
        _stateAuthority = freezingCoordinator.StateAuthority;
        _overlayCoordinator = overlayCoordinator
            ?? throw new ArgumentNullException(nameof(overlayCoordinator));
        _captureSourceExclusion = captureSourceExclusion;
        _captureAccessPreflight = captureAccessPreflight;
        _functionBarPresentation = functionBarPresentation;
        _finalRenderer = finalRenderer;
        _clipboardDelivery = clipboardDelivery;
        _feedback = feedback;
        _trace = traceSink ?? NoOpCompleteExecutionTraceSink.Instance;
        _annotationDocuments = annotationDocuments ?? new AnnotationDocumentCoordinator();
        _annotationEditing = new AnnotationEditingCoordinator(_annotationDocuments);
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

    public AnnotationDocument? CurrentAnnotationDocument => _annotationDocuments.Current;

    public EditingToolKind ActiveTool => _annotationEditing.ActiveTool;

    public int CurrentSelectionRevision =>
        _selectionCoordinator?.State.SelectionRevision ?? -1;

    public AnnotationRevision CurrentAnnotationRevision =>
        _annotationEditing.CurrentAnnotationRevision;

    public ArrowLineEndStyle ActiveArrowLineEndStyle =>
        _annotationEditing.ActiveArrowLineEndStyle;

    public HighlighterAnnotationStyle ActiveHighlighterStyle =>
        _annotationEditing.ActiveHighlighterStyle;

    public TextAnnotationStyle ActiveTextStyle =>
        _annotationEditing.ActiveTextStyle;

    public PrivacyRegionMode ActivePrivacyRegionMode =>
        _annotationEditing.ActivePrivacyRegionMode;

    public PrivacyRegionEffectParameters ActivePrivacyRegionEffectParameters =>
        _annotationEditing.ActivePrivacyRegionEffectParameters;

    public AnnotationMutationResult AddAnnotationObject(AddAnnotationObjectRequest request) =>
        _annotationDocuments.Add(request);

    public AnnotationMutationResult ReplaceAnnotationObject(ReplaceAnnotationObjectRequest request) =>
        _annotationDocuments.Replace(request);

    public AnnotationMutationResult RemoveAnnotationObject(RemoveAnnotationObjectRequest request) =>
        _annotationDocuments.Remove(request);

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
                    new FrozenDisplayOverlayPresentationRequest(plan, this)
                    {
                        EditingInputRouter = this
                    },
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
            _overlayCoordinator.ApplyAnnotation(
                _annotationEditing.CreatePresentationSnapshot(selection.State));
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
        if (result.Kind == SelectionInputResultKind.Locked
            && ActiveTool == EditingToolKind.Selection)
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
            else if (_functionBarPresentation is not null)
            {
                PrepareEditing(result.State);
            }
        }

        return result;
    }

    public RectanglePointerResult PointerPressed(RectanglePointerEvent input)
    {
        var result = ForwardRectangleInput(
            input,
            static (editing, value, selection) => editing.PointerPressed(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public RectanglePointerResult PointerMoved(RectanglePointerEvent input)
    {
        var result = ForwardRectangleInput(
            input,
            static (editing, value, selection) => editing.PointerMoved(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public RectanglePointerResult PointerReleased(RectanglePointerEvent input)
    {
        var result = ForwardRectangleInput(
            input,
            static (editing, value, selection) => editing.PointerReleased(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public ArrowLinePointerResult PointerPressed(ArrowLinePointerEvent input)
    {
        var result = ForwardArrowLineInput(
            input,
            static (editing, value, selection) => editing.PointerPressed(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public ArrowLinePointerResult PointerMoved(ArrowLinePointerEvent input)
    {
        var result = ForwardArrowLineInput(
            input,
            static (editing, value, selection) => editing.PointerMoved(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public ArrowLinePointerResult PointerReleased(ArrowLinePointerEvent input)
    {
        var result = ForwardArrowLineInput(
            input,
            static (editing, value, selection) => editing.PointerReleased(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public HighlighterPointerResult PointerPressed(HighlighterPointerEvent input)
    {
        var result = ForwardHighlighterInput(
            input,
            static (editing, value, selection) => editing.PointerPressed(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public HighlighterPointerResult PointerMoved(HighlighterPointerEvent input)
    {
        var result = ForwardHighlighterInput(
            input,
            static (editing, value, selection) => editing.PointerMoved(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public HighlighterPointerResult PointerReleased(HighlighterPointerEvent input)
    {
        var result = ForwardHighlighterInput(
            input,
            static (editing, value, selection) => editing.PointerReleased(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public PrivacyRegionPointerResult PointerPressed(PrivacyRegionPointerEvent input)
    {
        var result = ForwardPrivacyRegionInput(
            input,
            static (editing, value, selection) => editing.PointerPressed(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public PrivacyRegionPointerResult PointerMoved(PrivacyRegionPointerEvent input)
    {
        var result = ForwardPrivacyRegionInput(
            input,
            static (editing, value, selection) => editing.PointerMoved(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public PrivacyRegionPointerResult PointerReleased(PrivacyRegionPointerEvent input)
    {
        var result = ForwardPrivacyRegionInput(
            input,
            static (editing, value, selection) => editing.PointerReleased(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public TextDraftResult BeginTextDraft(TextDraftPointerEvent input)
    {
        ArgumentNullException.ThrowIfNull(input);
        var selection = CurrentSelection;
        if (selection is null)
        {
            return StaleTextResult(
                TextDraftResultKind.StaleSession,
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision,
                "Text input was ignored until the capture session was ready.");
        }

        var result = _annotationEditing.BeginTextDraft(input, selection);
        ApplyTextPresentation();
        return result;
    }

    public TextDraftResult UpdateTextDraftContent(TextDraftRequest request, string text)
    {
        ArgumentNullException.ThrowIfNull(request);
        var selection = CurrentSelection;
        if (selection is null)
        {
            return StaleTextResult(
                TextDraftResultKind.StaleSession,
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                "Text draft content was ignored because the capture session is no longer active.",
                request);
        }

        var result = _annotationEditing.UpdateTextDraftContent(request, text, selection);
        ApplyTextPresentation();
        return result;
    }

    public TextDraftResult UpdateTextDraftStyle(
        TextDraftRequest request,
        TextAnnotationStyle? style)
    {
        ArgumentNullException.ThrowIfNull(request);
        var selection = CurrentSelection;
        if (selection is null)
        {
            return StaleTextResult(
                TextDraftResultKind.StaleSession,
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                "Text draft style was ignored because the capture session is no longer active.",
                request);
        }

        var result = _annotationEditing.UpdateTextDraftStyle(request, style, selection);
        ApplyTextPresentation();
        return result;
    }

    public TextDraftResult CommitTextDraft(TextDraftRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var selection = CurrentSelection;
        if (selection is null)
        {
            return StaleTextResult(
                TextDraftResultKind.StaleSession,
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                "Text draft commit was ignored because the capture session is no longer active.",
                request);
        }

        var result = _annotationEditing.CommitTextDraft(request, selection);
        ApplyTextPresentation();
        return result;
    }

    public TextDraftResult CancelTextDraft(TextDraftRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var selection = CurrentSelection;
        if (selection is null)
        {
            return StaleTextResult(
                TextDraftResultKind.StaleSession,
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                "Text draft cancellation was ignored because the capture session is no longer active.",
                request);
        }

        var result = _annotationEditing.CancelTextDraft(request, selection);
        ApplyTextPresentation();
        return result;
    }

    public EditingToolSelectionResult SelectTool(EditingToolSelectionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _selectionCoordinator;
        }

        if (selection is null)
        {
            return new EditingToolSelectionResult(
                EditingToolSelectionResultKind.StaleSession,
                ActiveTool,
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                CurrentAnnotationRevision,
                null,
                "The editing tool request belongs to a stale capture session.");
        }

        var result = _annotationEditing.SelectTool(
            request,
            _stateAuthority.CurrentState,
            selection.State);
        if (result.Kind == EditingToolSelectionResultKind.Selected)
        {
            _overlayCoordinator.ApplySelection(selection.State);
            _overlayCoordinator.ApplyAnnotation(
                _annotationEditing.CreatePresentationSnapshot(selection.State));
            var repositioned = _functionBarPresentation?.Reposition(
                CreateFunctionBarRequest(selection.State));
            if (repositioned is not null
                && repositioned.Kind != FunctionBarPresentationResultKind.Ready)
            {
                return result with
                {
                    Kind = EditingToolSelectionResultKind.Failed,
                    Failure = CreateFailure(
                        request.SessionId,
                        FailureCode.FunctionBarPresentationFailed,
                        "The Function Bar could not reflect the selected editing tool."),
                    Message = "The Function Bar could not reflect the selected editing tool."
                };
            }
        }

        return result;
    }

    public PrivacyRegionModeSelectionResult SelectPrivacyRegionMode(
        PrivacyRegionModeSelectionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _selectionCoordinator;
        }

        if (selection is null)
        {
            return new PrivacyRegionModeSelectionResult(
                PrivacyRegionModeSelectionResultKind.StaleSession,
                ActiveTool,
                ActivePrivacyRegionMode,
                ActivePrivacyRegionEffectParameters,
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                CurrentAnnotationRevision,
                null,
                "The Privacy Region mode request belongs to a stale capture session.");
        }

        var result = _annotationEditing.SelectPrivacyRegionMode(
            request,
            _stateAuthority.CurrentState,
            selection.State);
        if (result.Kind == PrivacyRegionModeSelectionResultKind.Selected)
        {
            _overlayCoordinator.ApplyAnnotation(
                _annotationEditing.CreatePresentationSnapshot(selection.State));
            _functionBarPresentation?.Reposition(CreateFunctionBarRequest(selection.State));
        }

        return result;
    }

    public FunctionBarCommandResult Execute(FunctionBarCommandRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        CaptureSessionContext? session;
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            session = _activeSession;
            selection = _selectionCoordinator;
        }

        if (session is null
            || selection is null
            || session.SessionId != request.SessionId
            || !string.Equals(
                session.VirtualDesktopSnapshot.CoordinateVersion,
                request.CoordinateVersion,
                StringComparison.Ordinal))
        {
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.StaleSession,
                _stateAuthority.CurrentState,
                selection?.State.SelectionRevision ?? -1,
                null,
                "The Function Bar command belongs to a stale capture session.");
        }

        var currentSelection = selection.State;
        if (currentSelection.SelectionRevision != request.SelectionRevision)
        {
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.StaleSelectionRevision,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                null,
                "The Function Bar command belongs to a stale Selection revision.");
        }

        lock (_gate)
        {
            if (_completeInProgress)
            {
                return new FunctionBarCommandResult(
                    request.Command,
                    FunctionBarCommandResultKind.Busy,
                    _stateAuthority.CurrentState,
                    currentSelection.SelectionRevision,
                    null,
                    "The Complete command is already in progress.");
            }
        }

        if (_stateAuthority.CurrentState != WorkflowState.Editing)
        {
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.InvalidWorkflowState,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                null,
                "The Function Bar command is not valid in the current workflow state.");
        }

        if (!FunctionBarCommandAvailability.Stage6C.IsEnabled(request.Command))
        {
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.Disabled,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                null,
                "The Function Bar command is disabled in this slice.");
        }

        if (request.Command == FunctionBarCommand.Cancel)
        {
            lock (_gate)
            {
                if (_completeInProgress)
                {
                    return new FunctionBarCommandResult(
                        request.Command,
                        FunctionBarCommandResultKind.Busy,
                        _stateAuthority.CurrentState,
                        currentSelection.SelectionRevision,
                        null,
                        "The Complete command is already in progress.");
                }
            }

            Observe(CancelCurrentAsync("FunctionBarCancel"));
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.Accepted,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                null,
                "The capture session cancellation was accepted.");
        }

        var annotationDocument = _annotationDocuments.Current;
        if (annotationDocument is not null && annotationDocument.Objects.Count > 0)
        {
            var failure = CreateFailure(
                request.SessionId,
                FailureCode.AnnotationOutputNotSupported,
                "Complete does not render non-empty Annotation Documents in this slice.");
            _functionBarPresentation?.ShowFeedback(
                session.SessionId,
                session.VirtualDesktopSnapshot.CoordinateVersion,
                currentSelection.SelectionRevision,
                "Annotations are retained; Complete output is not available in this slice.");
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.AnnotationOutputNotSupported,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                failure,
                "Annotations are retained; Complete output is not available in this slice.");
        }

        if (_finalRenderer is null
            || _clipboardDelivery is null
            || currentSelection.Status != SelectionStatus.Locked
            || currentSelection.InteractionMode != SelectionInteractionMode.Locked
            || !currentSelection.IsGeometryValid
            || currentSelection.NormalizedPhysicalBounds is not PhysicalRect)
        {
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.Failed,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                CreateFailure(
                    request.SessionId,
                    FailureCode.InvalidSelection,
                    "Complete requires a valid locked Selection and an available output pipeline."),
                "Complete requires a valid locked Selection and an available output pipeline.");
        }

        TraceStage(
            CompleteExecutionStage.CommandAccepted,
            session,
            currentSelection,
            component: nameof(CapturePresentationWorkflowCoordinator));
        TraceStage(
            CompleteExecutionStage.SessionValidated,
            session,
            currentSelection,
            component: nameof(CapturePresentationWorkflowCoordinator));

        lock (_gate)
        {
            if (_completeInProgress)
            {
                return new FunctionBarCommandResult(
                    request.Command,
                    FunctionBarCommandResultKind.Busy,
                    _stateAuthority.CurrentState,
                    currentSelection.SelectionRevision,
                    null,
                    "The Complete command is already in progress.");
            }

            _completeInProgress = true;
        }

        var executing = _functionBarPresentation?.Reposition(
            CreateFunctionBarRequest(
                currentSelection,
                FunctionBarCommandAvailability.Stage6CExecuting));
        if (executing is not null
            && executing.Kind != FunctionBarPresentationResultKind.Ready)
        {
            lock (_gate)
            {
                _completeInProgress = false;
            }

            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.Failed,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                executing.Failure ?? CreateFailure(
                    request.SessionId,
                    FailureCode.FunctionBarPresentationFailed,
                    "The Function Bar could not enter the Complete state."),
                "The Function Bar could not enter the Complete state.");
        }

        Observe(CompleteAsync(session, currentSelection));
        return new FunctionBarCommandResult(
            request.Command,
            FunctionBarCommandResultKind.Accepted,
            _stateAuthority.CurrentState,
            currentSelection.SelectionRevision,
            null,
            "The capture is being rendered and delivered to Clipboard.");
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
            _completeInProgress = false;
            session = _activeSession;
            sessionCancellation = _sessionCancellation;
            selection = _selectionCoordinator;
            _activeSession = null;
            _selectionCoordinator = null;
        }

        sessionCancellation?.Cancel();
        if (session is not null)
        {
            _annotationDocuments.ClearSession(session.SessionId);
            _annotationEditing.ClearSession(session.SessionId);
            _functionBarPresentation?.Close(session.SessionId);
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
            _completeInProgress = false;
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
            _annotationDocuments.ClearSession(session.SessionId);
            _annotationEditing.ClearSession(session.SessionId);
            _functionBarPresentation?.Close(session.SessionId);
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
            selection = _inputEnabled
                && _annotationEditing.ActiveTool == EditingToolKind.Selection
                ? _selectionCoordinator
                : null;
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

    private RectanglePointerResult ForwardRectangleInput(
        RectanglePointerEvent input,
        Func<AnnotationEditingCoordinator, RectanglePointerEvent, SelectionVisualState, RectanglePointerResult> handler)
    {
        ArgumentNullException.ThrowIfNull(input);
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _inputEnabled ? _selectionCoordinator : null;
        }

        return selection is null
            ? new RectanglePointerResult(
                RectanglePointerResultKind.StaleSession,
                _annotationEditing.ActiveTool,
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision,
                CurrentAnnotationRevision,
                null,
                null,
                CurrentAnnotationDocument,
                null,
                "Rectangle input was ignored until the capture session was ready.")
            : handler(_annotationEditing, input, selection.State);
    }

    private ArrowLinePointerResult ForwardArrowLineInput(
        ArrowLinePointerEvent input,
        Func<AnnotationEditingCoordinator, ArrowLinePointerEvent, SelectionVisualState, ArrowLinePointerResult> handler)
    {
        ArgumentNullException.ThrowIfNull(input);
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _inputEnabled ? _selectionCoordinator : null;
        }

        return selection is null
            ? new ArrowLinePointerResult(
                ArrowLinePointerResultKind.StaleSession,
                _annotationEditing.ActiveTool,
                _annotationEditing.ActiveArrowLineEndStyle,
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision,
                CurrentAnnotationRevision,
                null,
                null,
                CurrentAnnotationDocument,
                null,
                "Arrow or line input was ignored until the capture session was ready.")
            : handler(_annotationEditing, input, selection.State);
    }

    private HighlighterPointerResult ForwardHighlighterInput(
        HighlighterPointerEvent input,
        Func<AnnotationEditingCoordinator, HighlighterPointerEvent, SelectionVisualState, HighlighterPointerResult> handler)
    {
        ArgumentNullException.ThrowIfNull(input);
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _inputEnabled ? _selectionCoordinator : null;
        }

        return selection is null
            ? new HighlighterPointerResult(
                HighlighterPointerResultKind.StaleSession,
                _annotationEditing.ActiveTool,
                _annotationEditing.ActiveHighlighterStyle,
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision,
                CurrentAnnotationRevision,
                null,
                null,
                CurrentAnnotationDocument,
                null,
                "Highlighter input was ignored until the capture session was ready.")
            : handler(_annotationEditing, input, selection.State);
    }

    private PrivacyRegionPointerResult ForwardPrivacyRegionInput(
        PrivacyRegionPointerEvent input,
        Func<AnnotationEditingCoordinator, PrivacyRegionPointerEvent, SelectionVisualState, PrivacyRegionPointerResult> handler)
    {
        ArgumentNullException.ThrowIfNull(input);
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _inputEnabled ? _selectionCoordinator : null;
        }

        return selection is null
            ? new PrivacyRegionPointerResult(
                PrivacyRegionPointerResultKind.StaleSession,
                _annotationEditing.ActiveTool,
                _annotationEditing.ActivePrivacyRegionMode,
                _annotationEditing.ActivePrivacyRegionEffectParameters,
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision,
                CurrentAnnotationRevision,
                null,
                null,
                null,
                CurrentAnnotationDocument,
                null,
                "Privacy Region input was ignored until the capture session was ready.")
            : handler(_annotationEditing, input, selection.State);
    }

    private void ApplyAnnotationPresentation()
    {
        var selection = CurrentSelection;
        if (selection is not null)
        {
            _overlayCoordinator.ApplyAnnotation(
                _annotationEditing.CreatePresentationSnapshot(selection));
        }
    }

    private void ApplyTextPresentation()
    {
        ApplyAnnotationPresentation();
        var selection = CurrentSelection;
        if (selection is not null && _functionBarPresentation is not null)
        {
            _ = _functionBarPresentation.Reposition(CreateFunctionBarRequest(selection));
        }
    }

    private TextDraftResult StaleTextResult(
        TextDraftResultKind kind,
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        string message,
        TextDraftRequest? request = null) => new(
        kind,
        _annotationEditing.ActiveTool,
        sessionId,
        coordinateVersion,
        selectionRevision,
        CurrentAnnotationRevision,
        request,
        string.Empty,
        _annotationEditing.ActiveTextStyle,
        null,
        CurrentAnnotationDocument,
        null,
        message);

    private void OnSelectionStateChanged(SelectionVisualState state)
    {
        _annotationEditing.UpdateSelection(state);
        _overlayCoordinator.ApplySelection(state);
        _overlayCoordinator.ApplyAnnotation(
            _annotationEditing.CreatePresentationSnapshot(state));

        if (_functionBarPresentation is null
            || _stateAuthority.CurrentState != WorkflowState.Editing)
        {
            return;
        }

        if (state.InteractionMode is
            SelectionInteractionMode.Moving
            or SelectionInteractionMode.ResizingLeft
            or SelectionInteractionMode.ResizingTop
            or SelectionInteractionMode.ResizingRight
            or SelectionInteractionMode.ResizingBottom
            or SelectionInteractionMode.ResizingTopLeft
            or SelectionInteractionMode.ResizingTopRight
            or SelectionInteractionMode.ResizingBottomLeft
            or SelectionInteractionMode.ResizingBottomRight
            or SelectionInteractionMode.Reselecting)
        {
            var hidden = _functionBarPresentation.Hide(state.SessionId);
            if (hidden.Kind == FunctionBarPresentationResultKind.Failed)
            {
                Observe(CancelCurrentAsync("FunctionBarHideFailed"));
            }

            return;
        }

        if (state.Status != SelectionStatus.Locked
            || !state.IsGeometryValid
            || state.NormalizedPhysicalBounds is null)
        {
            return;
        }

        var repositioned = _functionBarPresentation.Reposition(
            CreateFunctionBarRequest(state));
        if (repositioned.Kind != FunctionBarPresentationResultKind.Ready)
        {
            Observe(CancelCurrentAsync("FunctionBarRepositionFailed"));
            return;
        }

        var shown = _functionBarPresentation.Show(
            state.SessionId,
            state.CoordinateVersion,
            state.SelectionRevision);
        if (shown.Kind != FunctionBarPresentationResultKind.Shown)
        {
            Observe(CancelCurrentAsync("FunctionBarShowFailed"));
        }
    }

    private void PrepareEditing(SelectionVisualState selection)
    {
        if (_functionBarPresentation is null
            || selection.Status != SelectionStatus.Locked
            || !selection.IsGeometryValid
            || selection.NormalizedPhysicalBounds is null)
        {
            Observe(FailCurrentAsync(CreateFailure(
                selection.SessionId,
                FailureCode.InvalidSelection,
                "A valid locked Selection is required before Editing.")));
            return;
        }

        var prepared = _functionBarPresentation.Prepare(
            CreateFunctionBarRequest(selection));
        if (prepared.Kind != FunctionBarPresentationResultKind.Ready)
        {
            Observe(FailCurrentAsync(prepared.Failure ?? CreateFailure(
                selection.SessionId,
                FailureCode.FunctionBarPresentationFailed,
                "The Function Bar could not be prepared.")));
            return;
        }

        var transition = _stateAuthority.RequestTransition(new(
            WorkflowState.SelectionLocked,
            WorkflowState.Editing,
            "FunctionBarReady"));
        if (!transition.IsSuccess)
        {
            _functionBarPresentation.Close(selection.SessionId);
            Observe(FailCurrentAsync(transition.Failure ?? CreateFailure(
                selection.SessionId,
                FailureCode.InvalidStateTransition,
                "The workflow could not enter Editing.")));
            return;
        }

        _annotationDocuments.BeginSession(selection.SessionId);
        _annotationEditing.BeginSession(selection);
        _overlayCoordinator.ApplyAnnotation(
            _annotationEditing.CreatePresentationSnapshot(selection));

        var shown = _functionBarPresentation.Show(
            selection.SessionId,
            selection.CoordinateVersion,
            selection.SelectionRevision);
        if (shown.Kind != FunctionBarPresentationResultKind.Shown)
        {
            Observe(CancelCurrentAsync("FunctionBarShowFailed"));
        }
    }

    private FunctionBarPresentationRequest CreateFunctionBarRequest(
        SelectionVisualState selection,
        FunctionBarCommandAvailability? availability = null) => new(
        selection.SessionId,
        selection.CoordinateVersion,
        selection,
        availability ?? FunctionBarCommandAvailability.Stage6C,
        this)
        {
            ActiveTool = _annotationEditing.ActiveTool,
            AnnotationRevision = _annotationEditing.CurrentAnnotationRevision,
            ActiveArrowLineEndStyle = _annotationEditing.ActiveArrowLineEndStyle,
            ToolSelectionSink = this,
            ActivePrivacyRegionMode = _annotationEditing.ActivePrivacyRegionMode,
            PrivacyRegionModeSelectionSink = this
        };

    private async ValueTask CompleteAsync(
        CaptureSessionContext session,
        SelectionVisualState selection)
    {
        IImageResult? result = null;
        try
        {
            if (!IsCurrentEditingSession(session, selection))
            {
                return;
            }

            var bounds = selection.NormalizedPhysicalBounds!.Value;
            var frameSet = session.FrozenDisplayFrames;
            if (frameSet is null
                || frameSet.IsDisposed
                || frameSet.SessionId != session.SessionId
                || !string.Equals(
                    frameSet.CoordinateVersion,
                    session.VirtualDesktopSnapshot.CoordinateVersion,
                    StringComparison.Ordinal))
            {
                var failure = CreateFailure(
                    session.SessionId,
                    FailureCode.InvalidResultLifetime,
                    "The frozen display frame set is unavailable for Complete.");
                TraceStage(
                    CompleteExecutionStage.RenderFailed,
                    session,
                    selection,
                    failure,
                    component: nameof(CapturePresentationWorkflowCoordinator));
                ReturnToEditing(session, failure);
                return;
            }

            TraceStage(
                CompleteExecutionStage.FrozenFrameSetValidated,
                session,
                selection,
                component: nameof(CapturePresentationWorkflowCoordinator));

            TraceStage(
                CompleteExecutionStage.TransitioningToResultReady,
                session,
                selection,
                component: nameof(WorkflowStateAuthority));
            var readyTransition = _stateAuthority.RequestTransition(new(
                WorkflowState.Editing,
                WorkflowState.ResultReady,
                "CompleteRenderStarted"));
            if (!readyTransition.IsSuccess)
            {
                var failure = readyTransition.Failure ?? CreateFailure(
                    session.SessionId,
                    FailureCode.InvalidStateTransition,
                    "The workflow could not start the final render.");
                TraceStage(
                    CompleteExecutionStage.RenderFailed,
                    session,
                    selection,
                    failure,
                    component: nameof(WorkflowStateAuthority));
                ReturnToEditing(session, failure);
                return;
            }

            TraceStage(
                CompleteExecutionStage.Rendering,
                session,
                selection,
                component: nameof(IFrozenDisplayFrameSetRenderer));
            var rendered = await _finalRenderer!
                .RenderAsync(frameSet, bounds, session.Cancellation)
                .ConfigureAwait(true);
            if (rendered is FrozenDisplayFrameSetRenderOutcome.Cancelled cancelled)
            {
                var failure = CreateFailure(
                    session.SessionId,
                    FailureCode.Cancelled,
                    cancelled.CancellationOrigin);
                TraceStage(
                    CompleteExecutionStage.RenderFailed,
                    session,
                    selection,
                    failure,
                    component: nameof(IFrozenDisplayFrameSetRenderer));
                ReturnToEditing(session, failure);
                return;
            }

            if (rendered is FrozenDisplayFrameSetRenderOutcome.Failed failed)
            {
                TraceStage(
                    CompleteExecutionStage.RenderFailed,
                    session,
                    selection,
                    failed.Failure,
                    component: nameof(IFrozenDisplayFrameSetRenderer));
                ReturnToEditing(session, failed.Failure);
                return;
            }

            if (rendered is not FrozenDisplayFrameSetRenderOutcome.Succeeded succeeded)
            {
                var failure = CreateFailure(
                    session.SessionId,
                    FailureCode.RenderingFailed,
                    "The final renderer returned an unknown outcome.");
                TraceStage(
                    CompleteExecutionStage.RenderFailed,
                    session,
                    selection,
                    failure,
                    component: nameof(IFrozenDisplayFrameSetRenderer));
                ReturnToEditing(session, failure);
                return;
            }

            result = succeeded.ImageResult;
            TraceStage(
                CompleteExecutionStage.RenderSucceeded,
                session,
                selection,
                result: result,
                component: nameof(IFrozenDisplayFrameSetRenderer));

            TraceStage(
                CompleteExecutionStage.ResultValidation,
                session,
                selection,
                result: result,
                component: nameof(CapturePresentationWorkflowCoordinator));
            if (result is null
                || result.IsDisposed
                || result.Metadata.SessionId != session.SessionId
                || result.Metadata.CropPhysicalBounds != bounds
                || result.Metadata.PixelWidth != bounds.Width
                || result.Metadata.PixelHeight != bounds.Height)
            {
                var failure = CreateFailure(
                    session.SessionId,
                    FailureCode.InvalidResultLifetime,
                    "The final render did not produce a valid canonical Selection result.");
                TraceStage(
                    CompleteExecutionStage.ResultValidationFailed,
                    session,
                    selection,
                    failure,
                    result,
                    nameof(CapturePresentationWorkflowCoordinator));
                ReturnToEditing(session, failure);
                return;
            }

            TraceStage(
                CompleteExecutionStage.TransitioningToDelivering,
                session,
                selection,
                result: result,
                component: nameof(WorkflowStateAuthority));
            var deliveryTransition = _stateAuthority.RequestTransition(new(
                WorkflowState.ResultReady,
                WorkflowState.Delivering,
                "CompleteRenderSucceeded"));
            if (!deliveryTransition.IsSuccess)
            {
                var failure = deliveryTransition.Failure ?? CreateFailure(
                    session.SessionId,
                    FailureCode.InvalidStateTransition,
                    "The workflow could not start Clipboard delivery.");
                TraceStage(
                    CompleteExecutionStage.ClipboardFailed,
                    session,
                    selection,
                    failure,
                    result,
                    nameof(WorkflowStateAuthority));
                ReturnToEditing(session, failure);
                return;
            }

            var delivery = await _clipboardDelivery!
                .DeliverAsync(
                    new ClipboardDeliveryRequest
                    {
                        DeliveryId = Guid.NewGuid(),
                        SessionId = session.SessionId,
                        ResultId = result.Metadata.ResultId,
                        ImageResult = result,
                        HistoryAllowed = false,
                        RoamingAllowed = false,
                        MaximumAttempts = 5,
                        RetryBudget = TimeSpan.FromSeconds(1),
                        SelectionWidth = bounds.Width,
                        SelectionHeight = bounds.Height,
                        DisplayCount = frameSet.Frames.Count,
                        Cancellation = session.Cancellation
                    },
                    session.Cancellation)
                .ConfigureAwait(true);

            switch (delivery)
            {
                case ClipboardDeliveryResult.Delivered delivered
                    when delivered.SessionId == session.SessionId
                        && delivered.ResultId == result.Metadata.ResultId:
                    var completedTransition = _stateAuthority.RequestTransition(new(
                        WorkflowState.Delivering,
                        WorkflowState.Completed,
                        "ClipboardDelivered"));
                    if (!completedTransition.IsSuccess)
                    {
                        var failure = completedTransition.Failure ?? CreateFailure(
                            session.SessionId,
                            FailureCode.InvalidStateTransition,
                            "The workflow could not complete after Clipboard delivery.");
                        TraceStage(
                            CompleteExecutionStage.ClipboardFailed,
                            session,
                            selection,
                            failure,
                            result,
                            nameof(WorkflowStateAuthority));
                        ReturnToEditing(session, failure);
                        return;
                    }

                    TraceStage(
                        CompleteExecutionStage.ClipboardDelivered,
                        session,
                        selection,
                        result: result,
                        clipboardAttempt: delivered.Attempts,
                        component: nameof(IClipboardDeliveryService));
                    TraceStage(
                        CompleteExecutionStage.Completed,
                        session,
                        selection,
                        result: result,
                        clipboardAttempt: delivered.Attempts,
                        component: nameof(CapturePresentationWorkflowCoordinator));
                    await CompleteSessionAsync(session).ConfigureAwait(true);
                    return;
                case ClipboardDeliveryResult.Cancelled deliveryCancelled:
                    var cancelledFailure = CreateFailure(
                        session.SessionId,
                        FailureCode.Cancelled,
                        deliveryCancelled.CancellationOrigin);
                    TraceStage(
                        CompleteExecutionStage.ClipboardFailed,
                        session,
                        selection,
                        cancelledFailure,
                        result,
                        nameof(IClipboardDeliveryService));
                    ReturnToEditing(session, cancelledFailure);
                    return;
                case ClipboardDeliveryResult.RetryableFailure retryable:
                    TraceStage(
                        CompleteExecutionStage.ClipboardFailed,
                        session,
                        selection,
                        retryable.Failure,
                        result,
                        nameof(IClipboardDeliveryService),
                        retryable.AttemptsUsed);
                    ReturnToEditing(session, retryable.Failure);
                    return;
                case ClipboardDeliveryResult.TerminalFailure terminal:
                    TraceStage(
                        CompleteExecutionStage.ClipboardFailed,
                        session,
                        selection,
                        terminal.Failure,
                        result,
                        nameof(IClipboardDeliveryService));
                    ReturnToEditing(session, terminal.Failure);
                    return;
                default:
                    var unknownDeliveryFailure = CreateFailure(
                        session.SessionId,
                        FailureCode.ClipboardPublicationRejected,
                        "Clipboard delivery returned an unknown outcome.");
                    TraceStage(
                        CompleteExecutionStage.ClipboardFailed,
                        session,
                        selection,
                        unknownDeliveryFailure,
                        result,
                        nameof(IClipboardDeliveryService));
                    ReturnToEditing(session, unknownDeliveryFailure);
                    return;
            }
        }
        catch (OperationCanceledException)
        {
            var failure = CreateFailure(
                session.SessionId,
                FailureCode.Cancelled,
                "CancellationToken");
            TraceStage(
                CompleteExecutionStage.ReturningToEditing,
                session,
                selection,
                failure,
                result,
                nameof(CapturePresentationWorkflowCoordinator));
            ReturnToEditing(session, failure);
        }
        catch (Exception exception)
        {
            var failure = CreateFailure(
                session.SessionId,
                FailureCode.UnexpectedFailure,
                exception.GetType().Name,
                exception.HResult);
            TraceStage(
                CompleteExecutionStage.ReturningToEditing,
                session,
                selection,
                failure,
                result,
                nameof(CapturePresentationWorkflowCoordinator));
            ReturnToEditing(session, failure);
        }
        finally
        {
            TraceStage(
                CompleteExecutionStage.CleaningUp,
                session,
                selection,
                result: result,
                component: nameof(CapturePresentationWorkflowCoordinator));
            result?.Dispose();
            lock (_gate)
            {
                _completeInProgress = false;
            }
        }
    }

    private bool IsCurrentEditingSession(
        CaptureSessionContext session,
        SelectionVisualState selection)
    {
        lock (_gate)
        {
            return !_disposed
                && ReferenceEquals(_activeSession, session)
                && _selectionCoordinator?.State.SelectionRevision == selection.SelectionRevision
                && _stateAuthority.CurrentState == WorkflowState.Editing;
        }
    }

    private void ReturnToEditing(CaptureSessionContext session, Failure failure)
    {
        SelectionVisualState? selection;
        lock (_gate)
        {
            _completeInProgress = false;
            if (_disposed || !ReferenceEquals(_activeSession, session))
            {
                return;
            }

            selection = _selectionCoordinator?.State;
        }

        var currentState = _stateAuthority.CurrentState;
        if (currentState is WorkflowState.ResultReady or WorkflowState.Delivering)
        {
            _stateAuthority.RequestTransition(new(
                currentState,
                WorkflowState.Editing,
                $"CompleteFailed:{failure.Code}"));
        }

        _feedback?.Invoke(failure.UserMessageKey);
        TraceStage(
            CompleteExecutionStage.ReturningToEditing,
            session,
            selection,
            failure,
            component: nameof(CapturePresentationWorkflowCoordinator));
        if (selection is not null
            && _functionBarPresentation is not null
            && selection.Status == SelectionStatus.Locked
            && selection.InteractionMode == SelectionInteractionMode.Locked
            && selection.IsGeometryValid)
        {
            var repositioned = _functionBarPresentation.Reposition(
                CreateFunctionBarRequest(selection, FunctionBarCommandAvailability.Stage6C));
            if (repositioned.Kind == FunctionBarPresentationResultKind.Ready)
            {
                var shown = _functionBarPresentation.Show(
                    selection.SessionId,
                    selection.CoordinateVersion,
                    selection.SelectionRevision);
                if (shown.Kind == FunctionBarPresentationResultKind.Shown)
                {
                    _functionBarPresentation.ShowFeedback(
                        selection.SessionId,
                        selection.CoordinateVersion,
                        selection.SelectionRevision,
                        GetCaptureFailureMessage(failure));
                }
            }

            _overlayCoordinator.ApplyAnnotation(
                _annotationEditing.CreatePresentationSnapshot(selection));
        }
    }

    private async ValueTask CompleteSessionAsync(CaptureSessionContext session)
    {
        InitialSelectionCoordinator? selection;
        CancellationTokenSource? cancellation;
        lock (_gate)
        {
            if (!ReferenceEquals(_activeSession, session))
            {
                return;
            }

            _inputEnabled = false;
            selection = _selectionCoordinator;
            cancellation = _sessionCancellation;
            _activeSession = null;
            _selectionCoordinator = null;
            _sessionCancellation = null;
        }

        _functionBarPresentation?.Close(session.SessionId);
        try
        {
            await _overlayCoordinator
                .CloseAsync(session.SessionId, CancellationToken.None)
                .ConfigureAwait(true);
        }
        finally
        {
            _annotationDocuments.ClearSession(session.SessionId);
            _annotationEditing.ClearSession(session.SessionId);
            _freezingCoordinator.ReleaseSession(session);
            session.Dispose();
            selection?.Dispose();
            MoveToResidentReady(WorkflowState.Completed, "CompleteCleanup");
            cancellation?.Dispose();
        }
    }

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
            _annotationDocuments.ClearSession(session.SessionId);
            _annotationEditing.ClearSession(session.SessionId);
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
        _annotationDocuments.ClearSession(session.SessionId);
        _annotationEditing.ClearSession(session.SessionId);
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

    private void TraceStage(
        CompleteExecutionStage stage,
        CaptureSessionContext session,
        SelectionVisualState? selection,
        Failure? failure = null,
        IImageResult? result = null,
        string component = "CapturePresentationWorkflowCoordinator",
        int clipboardAttempt = 0)
    {
        try
        {
            var selectionBounds = selection?.NormalizedPhysicalBounds;
            var resultMetadata = result?.Metadata;
            var frameSet = session.FrozenDisplayFrames;
            _trace.Record(new CompleteExecutionTraceEntry
            {
                TimestampUtc = DateTimeOffset.UtcNow,
                SessionId = session.SessionId,
                SelectionRevision = selection?.SelectionRevision ?? -1,
                WorkflowState = _stateAuthority.CurrentState,
                CompleteStage = stage,
                FailureCode = failure?.Code,
                FailureCategory = failure?.Category,
                NativeCode = failure?.NativeCode,
                Component = component,
                SelectionWidth = selectionBounds?.Width ?? 0,
                SelectionHeight = selectionBounds?.Height ?? 0,
                ResultWidth = resultMetadata?.PixelWidth ?? 0,
                ResultHeight = resultMetadata?.PixelHeight ?? 0,
                DisplayCount = frameSet?.Frames.Count ?? 0,
                ClipboardAttempt = clipboardAttempt,
                ManagedThreadId = Environment.CurrentManagedThreadId
            });
        }
        catch
        {
            // Diagnostics must never change the capture outcome.
        }
    }

    private static string GetCaptureFailureMessage(Failure failure) => failure.Code switch
    {
        FailureCode.InvalidSelection => "目前框選範圍無法完成。",
        FailureCode.EncodingFailed
            or FailureCode.ClipboardBusy
            or FailureCode.ClipboardPublicationRejected
            or FailureCode.OutputAccessDenied
            or FailureCode.OutputWriteFailed => "無法複製到剪貼簿，請再試一次。",
        FailureCode.RenderingFailed
            or FailureCode.RenderingResourceLost
            or FailureCode.InvalidResultLifetime => "無法產生截圖影像，請再試一次。",
        _ => "無法完成截圖，請再試一次。"
    };

    private sealed class NoOpCompleteExecutionTraceSink : ICompleteExecutionTraceSink
    {
        public static NoOpCompleteExecutionTraceSink Instance { get; } = new();

        public void Record(CompleteExecutionTraceEntry entry)
        {
        }
    }

    private static void Observe(ValueTask<CapturePresentationOutcome> operation) =>
        _ = operation.AsTask();

    private static void Observe(ValueTask operation) =>
        _ = operation.AsTask();
}
