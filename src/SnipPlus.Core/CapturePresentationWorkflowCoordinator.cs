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
    private readonly IAnnotationAwareRenderAdapter? _annotationAwareRenderer;
    private readonly IOutputCommitmentCoordinator? _outputCommitment;
    private readonly PngSaveCoordinator? _pngSaveCoordinator;
    private readonly Action<string>? _feedback;
    private readonly ICompleteExecutionTraceSink _trace;
    private readonly AnnotationDocumentCoordinator _annotationDocuments;
    private readonly AnnotationHistoryCoordinator _annotationHistory;
    private readonly AnnotationEditingCoordinator _annotationEditing;
    private readonly AnnotationObjectEditCoordinator _annotationObjectEditing;
    private readonly SupportedCapacityPolicy _capacityPolicy;
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
        IOutputCommitmentCoordinator? outputCommitment = null,
        Action<string>? feedback = null,
        ICompleteExecutionTraceSink? traceSink = null,
        AnnotationDocumentCoordinator? annotationDocuments = null,
        IAnnotationAwareRenderAdapter? annotationAwareRenderer = null,
        SupportedCapacityPolicy? capacityPolicy = null,
        PngSaveCoordinator? pngSaveCoordinator = null)
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
        _annotationAwareRenderer = annotationAwareRenderer;
        _outputCommitment = outputCommitment;
        _pngSaveCoordinator = pngSaveCoordinator;
        _feedback = feedback;
        _trace = traceSink ?? NoOpCompleteExecutionTraceSink.Instance;
        _annotationDocuments = annotationDocuments ?? new AnnotationDocumentCoordinator();
        _capacityPolicy = capacityPolicy ?? new SupportedCapacityPolicy();
        _annotationHistory = new AnnotationHistoryCoordinator(_annotationDocuments);
        _annotationEditing = new AnnotationEditingCoordinator(
            _annotationDocuments,
            history: _annotationHistory);
        _annotationObjectEditing = new AnnotationObjectEditCoordinator(
            _annotationDocuments,
            _annotationEditing.ChangeDefaultStyle,
            _annotationHistory);
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

    public AnnotationHistoryState CurrentAnnotationHistory =>
        _annotationHistory.CurrentState;

    public EditingToolKind ActiveTool => _annotationEditing.ActiveTool;

    public int CurrentSelectionRevision =>
        _selectionCoordinator?.State.SelectionRevision ?? -1;

    public AnnotationRevision CurrentAnnotationRevision =>
        _annotationEditing.CurrentAnnotationRevision;

    public ArrowLineEndStyle ActiveArrowLineEndStyle =>
        _annotationEditing.ActiveArrowLineEndStyle;

    public RectangleAnnotationStyle ActiveRectangleStyle =>
        _annotationEditing.ActiveRectangleStyle;

    public ArrowLineAnnotationStyle ActiveArrowLineStyle =>
        _annotationEditing.ActiveArrowLineStyle;

    public HighlighterAnnotationStyle ActiveHighlighterStyle =>
        _annotationEditing.ActiveHighlighterStyle;

    public TextAnnotationStyle ActiveTextStyle =>
        _annotationEditing.ActiveTextStyle;

    public PrivacyRegionMode ActivePrivacyRegionMode =>
        _annotationEditing.ActivePrivacyRegionMode;

    public PrivacyRegionEffectParameters ActivePrivacyRegionEffectParameters =>
        _annotationEditing.ActivePrivacyRegionEffectParameters;

    public NumberedMarkerAnnotationStyle ActiveNumberedMarkerStyle =>
        _annotationEditing.ActiveNumberedMarkerStyle;

    public int ActiveNumberedMarkerNextNumber =>
        _annotationEditing.ActiveNumberedMarkerNextNumber;

    public AnnotationObjectSelectionState SelectedObject =>
        _annotationObjectEditing.State;

    public bool IsObjectEditingEnabled
    {
        get
        {
            var selection = CurrentSelection;
            return CurrentState == WorkflowState.Editing
                && selection?.Status == SelectionStatus.Locked
                && selection.InteractionMode == SelectionInteractionMode.Locked;
        }
    }

    public AnnotationMutationResult AddAnnotationObject(AddAnnotationObjectRequest request)
    {
        var before = _annotationDocuments.Current;
        var result = _annotationDocuments.Add(request);
        RecordDirectMutation(request, before, result);
        return result;
    }

    public AnnotationMutationResult ReplaceAnnotationObject(ReplaceAnnotationObjectRequest request)
    {
        var before = _annotationDocuments.Current;
        var result = _annotationDocuments.Replace(request);
        RecordDirectMutation(request, before, result);
        return result;
    }

    public AnnotationMutationResult RemoveAnnotationObject(RemoveAnnotationObjectRequest request)
    {
        var before = _annotationDocuments.Current;
        var result = _annotationDocuments.Remove(request);
        RecordDirectMutation(request, before, result);
        return result;
    }

    public AnnotationObjectEditResult PointerPressed(AnnotationObjectPointerEvent input)
    {
        var result = _annotationObjectEditing.PointerPressed(input);
        ApplyAnnotationPresentation();
        return result;
    }

    public AnnotationObjectEditResult PointerMoved(AnnotationObjectPointerEvent input)
    {
        var result = _annotationObjectEditing.PointerMoved(input);
        ApplyAnnotationPresentation();
        return result;
    }

    public AnnotationObjectEditResult PointerReleased(AnnotationObjectPointerEvent input)
    {
        var result = _annotationObjectEditing.PointerReleased(input);
        ApplyAnnotationPresentation();
        return result;
    }

    public AnnotationObjectEditResult SelectObject(AnnotationObjectSelectionRequest request)
    {
        var result = _annotationObjectEditing.SelectObject(request);
        ApplyAnnotationPresentation();
        return result;
    }

    public AnnotationObjectEditResult ChangeStyle(AnnotationObjectStyleChangeRequest request)
    {
        var result = _annotationObjectEditing.ChangeStyle(request);
        ApplyAnnotationPresentation();
        return result;
    }

    public AnnotationObjectEditResult Delete(AnnotationObjectDeleteRequest request)
    {
        var result = _annotationObjectEditing.Delete(request);
        ApplyAnnotationPresentation();
        return result;
    }

    public AnnotationObjectEditResult BeginTextEdit(AnnotationObjectSelectionRequest request)
    {
        var result = _annotationObjectEditing.BeginTextEdit(request);
        ApplyAnnotationPresentation();
        return result;
    }

    public AnnotationObjectEditResult UpdateTextEdit(AnnotationObjectTextEditRequest request)
    {
        var result = _annotationObjectEditing.UpdateTextEdit(request);
        ApplyAnnotationPresentation();
        return result;
    }

    public AnnotationObjectEditResult CommitTextEdit(AnnotationObjectTextEditRequest request)
    {
        var result = _annotationObjectEditing.CommitTextEdit(request);
        ApplyAnnotationPresentation();
        return result;
    }

    public AnnotationObjectEditResult CancelEdit(Guid sessionId, string coordinateVersion)
    {
        var result = _annotationObjectEditing.CancelEdit(sessionId, coordinateVersion);
        ApplyAnnotationPresentation();
        return result;
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
                TraceDiagnostic(request.RequestId, "Capture.Start.Busy");
                return new CapturePresentationOutcome.Busy();
            }

            _startInProgress = true;
            _sessionCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            sessionCancellation = _sessionCancellation;
        }

        TraceDiagnostic(request.RequestId, "Capture.Start.Accepted");

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

    public SelectionInputResult PointerPressed(SelectionPointerEvent input)
    {
        if (IsObjectEditingInput(input))
        {
            return ForwardObjectPointer(input, _annotationObjectEditing.PointerPressed);
        }

        return ForwardSelectionInput(input, static (selection, value) => selection.PointerPressed(value));
    }

    public SelectionInputResult PointerMoved(SelectionPointerEvent input)
    {
        if (IsObjectEditingInput(input))
        {
            return ForwardObjectPointer(input, _annotationObjectEditing.PointerMoved);
        }

        return ForwardSelectionInput(input, static (selection, value) => selection.PointerMoved(value));
    }

    public SelectionInputResult PointerReleased(SelectionPointerEvent input)
    {
        ArgumentNullException.ThrowIfNull(input);
        TraceDiagnostic(input.SessionId, "Selection.PointerReleased.Begin");

        try
        {
            if (IsObjectEditingInput(input))
            {
                var objectResult = ForwardObjectPointer(
                    input,
                    _annotationObjectEditing.PointerReleased);
                TraceDiagnostic(
                    input.SessionId,
                    $"Selection.PointerReleased.{objectResult.Kind}");
                return objectResult;
            }

            var result = ForwardSelectionInput(
                input,
                static (selection, value) => selection.PointerReleased(value));
            TraceDiagnostic(
                input.SessionId,
                $"Selection.PointerReleased.{result.Kind}",
                result.State);
            if (result.Kind == SelectionInputResultKind.Locked
                && ActiveTool == EditingToolKind.Selection)
            {
                var transition = _stateAuthority.RequestTransition(new(
                    WorkflowState.Selecting,
                    WorkflowState.SelectionLocked,
                    "InitialSelectionPointerReleased"));
                if (!transition.IsSuccess)
                {
                    var failure = transition.Failure ?? CreateFailure(
                        input.SessionId,
                        FailureCode.InvalidStateTransition,
                        "The valid Selection could not be locked.");
                    TraceDiagnostic(
                        input.SessionId,
                        "Selection.PointerReleased.TransitionFailed",
                        result.State,
                        failure);
                    Observe(FailCurrentAsync(failure));
                }
                else if (_functionBarPresentation is not null)
                {
                    PrepareEditing(result.State);
                }
            }

            return result;
        }
        catch (Exception exception)
        {
            var failure = CreateFailure(
                input.SessionId,
                FailureCode.UnexpectedFailure,
                $"Pointer release threw {exception.GetType().Name}: {exception.Message}",
                exception.HResult);
            TraceDiagnostic(
                input.SessionId,
                "Selection.PointerReleased.Exception",
                CurrentSelection,
                failure,
                exception);
            Observe(FailCurrentAsync(failure));
            return new SelectionInputResult(
                SelectionInputResultKind.InvalidSelection,
                CurrentSelection ?? SelectionVisualState.Initial(
                    input.SessionId,
                    input.CoordinateVersion),
                "Selection release failed and cleanup was requested.");
        }
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

    public NumberedMarkerPointerResult PointerPressed(NumberedMarkerPointerEvent input)
    {
        var result = ForwardNumberedMarkerInput(
            input,
            static (editing, value, selection) => editing.PointerPressed(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public NumberedMarkerPointerResult PointerMoved(NumberedMarkerPointerEvent input)
    {
        var result = ForwardNumberedMarkerInput(
            input,
            static (editing, value, selection) => editing.PointerMoved(value, selection));
        ApplyAnnotationPresentation();
        return result;
    }

    public NumberedMarkerPointerResult PointerReleased(NumberedMarkerPointerEvent input)
    {
        var result = ForwardNumberedMarkerInput(
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

    public SetNextNumberResult SetNextNumber(SetNextNumberRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _selectionCoordinator;
        }

        if (selection is null)
        {
            return new SetNextNumberResult(
                SetNextNumberResultKind.StaleSession,
                ActiveTool,
                ActiveNumberedMarkerNextNumber,
                ActiveNumberedMarkerStyle,
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                CurrentAnnotationRevision,
                null,
                "The next marker number request belongs to a stale capture session.");
        }

        var result = _annotationEditing.SetNextNumber(
            request,
            _stateAuthority.CurrentState,
            selection.State);
        if (result.Kind is SetNextNumberResultKind.Succeeded or SetNextNumberResultKind.NoChange)
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

        if (CurrentAnnotationRevision != request.AnnotationRevision)
        {
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.StaleAnnotationRevision,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                null,
                "The Function Bar command belongs to a stale Annotation revision.")
            {
                CurrentAnnotationRevision = CurrentAnnotationRevision,
                CanUndo = _annotationHistory.CurrentState.CanUndo,
                CanRedo = _annotationHistory.CurrentState.CanRedo
            };
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
                    "Another output command is already in progress.");
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

        var availability = CreateFunctionBarAvailability();
        if (!availability.IsEnabled(request.Command))
        {
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.Disabled,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                null,
                "The Function Bar command is disabled in this slice.")
            {
                CurrentAnnotationRevision = CurrentAnnotationRevision,
                CanUndo = availability.CanUndo,
                CanRedo = availability.CanRedo
            };
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
                        "Another output command is already in progress.");
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

        if (request.Command is FunctionBarCommand.Undo or FunctionBarCommand.Redo)
        {
            var historyResult = _annotationHistory.Execute(
                new AnnotationHistoryRequest(
                    request.SessionId,
                    request.CoordinateVersion,
                    request.SelectionRevision,
                    request.AnnotationRevision,
                    request.Command == FunctionBarCommand.Undo
                        ? AnnotationHistoryCommand.Undo
                        : AnnotationHistoryCommand.Redo),
                _stateAuthority.CurrentState,
                _annotationEditing.HasActiveDraft || _annotationObjectEditing.HasActiveEdit);
            if (historyResult.Kind == AnnotationHistoryResultKind.Succeeded)
            {
                _annotationEditing.ApplyHistoryNextNumber(historyResult.CurrentNextNumber);
                _annotationObjectEditing.ReconcileAfterHistory(historyResult);
                ApplyAnnotationPresentation();
            }

            return ToFunctionBarCommandResult(historyResult);
        }

        var annotationDocument = _annotationDocuments.Current
            ?? AnnotationDocument.CreateEmpty(session.SessionId);
        var hasAnnotations = annotationDocument.Objects.Count > 0;

        if ((!hasAnnotations && _finalRenderer is null)
            || (hasAnnotations && _annotationAwareRenderer is null)
            || _outputCommitment is null
            || (request.Command == FunctionBarCommand.Save && _pngSaveCoordinator is null)
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
                    request.Command == FunctionBarCommand.Save
                        ? "Save requires a valid locked Selection and an available PNG and Clipboard output pipeline."
                        : "Complete requires a valid locked Selection and an available output pipeline."),
                request.Command == FunctionBarCommand.Save
                    ? "Save requires a valid locked Selection and an available PNG and Clipboard output pipeline."
                    : "Complete requires a valid locked Selection and an available output pipeline.");
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
                    "Another output command is already in progress.");
            }

            _completeInProgress = true;
        }

        var executing = _functionBarPresentation?.Reposition(
            CreateFunctionBarRequest(
                currentSelection,
                FunctionBarCommandAvailability.Stage7NExecuting));
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
                    "The Function Bar could not enter the output state."),
                "The Function Bar could not enter the output state.");
        }

        if (request.Command == FunctionBarCommand.Save)
        {
            Observe(SaveAsync(session, currentSelection, annotationDocument));
            return new FunctionBarCommandResult(
                request.Command,
                FunctionBarCommandResultKind.Accepted,
                _stateAuthority.CurrentState,
                currentSelection.SelectionRevision,
                null,
                "The capture is being rendered, saved as PNG, and delivered to Clipboard.");
        }

        Observe(CompleteAsync(session, currentSelection, annotationDocument));
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
            _annotationObjectEditing.ClearSession(session.SessionId);
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
            _annotationObjectEditing.ClearSession(session.SessionId);
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

    private NumberedMarkerPointerResult ForwardNumberedMarkerInput(
        NumberedMarkerPointerEvent input,
        Func<AnnotationEditingCoordinator, NumberedMarkerPointerEvent, SelectionVisualState, NumberedMarkerPointerResult> handler)
    {
        ArgumentNullException.ThrowIfNull(input);
        InitialSelectionCoordinator? selection;
        lock (_gate)
        {
            selection = _inputEnabled ? _selectionCoordinator : null;
        }

        return selection is null
            ? new NumberedMarkerPointerResult(
                NumberedMarkerPointerResultKind.StaleSession,
                _annotationEditing.ActiveTool,
                _annotationEditing.ActiveNumberedMarkerNextNumber,
                _annotationEditing.ActiveNumberedMarkerStyle,
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision,
                CurrentAnnotationRevision,
                null,
                null,
                CurrentAnnotationDocument,
                null,
                "Numbered Marker input was ignored until the capture session was ready.")
            : handler(_annotationEditing, input, selection.State);
    }

    private void ApplyAnnotationPresentation()
    {
        var selection = CurrentSelection;
        if (selection is not null)
        {
            _overlayCoordinator.ApplyAnnotation(CreateAnnotationPresentation(selection));
            if (_functionBarPresentation is not null
                && _stateAuthority.CurrentState == WorkflowState.Editing
                && selection.Status == SelectionStatus.Locked)
            {
                _functionBarPresentation.Reposition(CreateFunctionBarRequest(selection));
            }
        }
    }

    private AnnotationPresentationSnapshot CreateAnnotationPresentation(
        SelectionVisualState selection)
    {
        var objectState = _annotationObjectEditing.State;
        var snapshot = _annotationEditing.CreatePresentationSnapshot(selection) with
        {
            SelectedObject = objectState
        };
        var textObject = objectState.PreviewObject ?? objectState.OriginalObject;
        if (objectState.Operation == AnnotationObjectEditOperationKind.TextEdit
            && objectState.TextEditDraftId is Guid draftId
            && textObject?.Content is TextAnnotationContent text)
        {
            snapshot = snapshot with
            {
                SelectedTextEdit = new TextDraftPresentation(
                    draftId,
                    text.Text,
                    text.AnchorInVirtualDesktop,
                    text.BoundsInVirtualDesktop,
                    text.Style)
            };
        }

        return snapshot;
    }

    private bool IsObjectEditingInput(SelectionPointerEvent input)
    {
        lock (_gate)
        {
            var selection = _selectionCoordinator?.State;
            return _inputEnabled
                && _stateAuthority.CurrentState == WorkflowState.Editing
                && _annotationEditing.ActiveTool == EditingToolKind.Selection
                && selection?.Status == SelectionStatus.Locked
                && selection.InteractionMode == SelectionInteractionMode.Locked
                && _annotationObjectEditing.CanHandlePointer(input.GlobalPhysicalPoint);
        }
    }

    private SelectionInputResult ForwardObjectPointer(
        SelectionPointerEvent input,
        Func<AnnotationObjectPointerEvent, AnnotationObjectEditResult> handler)
    {
        ArgumentNullException.ThrowIfNull(input);
        var selection = CurrentSelection;
        if (selection is null)
        {
            return new SelectionInputResult(
                SelectionInputResultKind.StaleSession,
                SelectionVisualState.Initial(input.SessionId, input.CoordinateVersion),
                "Annotation object input was ignored because the capture session is not active.");
        }

        var result = handler(new AnnotationObjectPointerEvent(
            input.SessionId,
            input.CoordinateVersion,
            selection.SelectionRevision,
            CurrentAnnotationRevision,
            input.PointerId,
            input.GlobalPhysicalPoint));
        ApplyAnnotationPresentation();
        return new SelectionInputResult(
            SelectionInputResultKind.AnnotationObjectEditing,
            selection,
            result.Message);
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
        try
        {
            _annotationEditing.UpdateSelection(state);
            _annotationObjectEditing.UpdateSelection(state);
            _overlayCoordinator.ApplySelection(state);
            _overlayCoordinator.ApplyAnnotation(CreateAnnotationPresentation(state));
        }
        catch (Exception exception)
        {
            var failure = CreateFailure(
                state.SessionId,
                FailureCode.OverlayPresentationFailed,
                $"Selection overlay presentation threw {exception.GetType().Name}: {exception.Message}",
                exception.HResult);
            TraceDiagnostic(
                state.SessionId,
                "Selection.StateChanged.OverlayException",
                state,
                failure,
                exception);
            Observe(FailCurrentAsync(failure));
            return;
        }

        if (_functionBarPresentation is null
            || _stateAuthority.CurrentState != WorkflowState.Editing)
        {
            return;
        }

        try
        {
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
                    TraceDiagnostic(
                        state.SessionId,
                        "FunctionBar.Hide.Failed",
                        state,
                        hidden.Failure);
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
                TraceDiagnostic(
                    state.SessionId,
                    "FunctionBar.Reposition.Failed",
                    state,
                    repositioned.Failure);
                Observe(CancelCurrentAsync("FunctionBarRepositionFailed"));
                return;
            }

            var shown = _functionBarPresentation.Show(
                state.SessionId,
                state.CoordinateVersion,
                state.SelectionRevision);
            if (shown.Kind != FunctionBarPresentationResultKind.Shown)
            {
                TraceDiagnostic(
                    state.SessionId,
                    "FunctionBar.Show.Failed",
                    state,
                    shown.Failure);
                Observe(CancelCurrentAsync("FunctionBarShowFailed"));
            }
        }
        catch (Exception exception)
        {
            var failure = CreateFailure(
                state.SessionId,
                FailureCode.FunctionBarPresentationFailed,
                $"Function Bar presentation threw {exception.GetType().Name}: {exception.Message}",
                exception.HResult);
            TraceDiagnostic(
                state.SessionId,
                "FunctionBar.StateChanged.Exception",
                state,
                failure,
                exception);
            Observe(FailCurrentAsync(failure));
        }
    }

    private void PrepareEditing(SelectionVisualState selection)
    {
        TraceDiagnostic(selection.SessionId, "FunctionBar.Prepare.Begin", selection);
        try
        {
            PrepareEditingCore(selection);
        }
        catch (Exception exception)
        {
            var failure = CreateFailure(
                selection.SessionId,
                FailureCode.FunctionBarPresentationFailed,
                $"Function Bar preparation flow threw {exception.GetType().Name}: {exception.Message}",
                exception.HResult);
            TraceDiagnostic(
                selection.SessionId,
                "FunctionBar.Prepare.Exception",
                selection,
                failure,
                exception);
            Observe(FailCurrentAsync(failure));
        }
    }

    private void PrepareEditingCore(SelectionVisualState selection)
    {
        if (_functionBarPresentation is null
            || selection.Status != SelectionStatus.Locked
            || !selection.IsGeometryValid
            || selection.NormalizedPhysicalBounds is null)
        {
            TraceDiagnostic(
                selection.SessionId,
                "FunctionBar.Prepare.InvalidSelection",
                selection);
            Observe(FailCurrentAsync(CreateFailure(
                selection.SessionId,
                FailureCode.InvalidSelection,
                "A valid locked Selection is required before Editing.")));
            return;
        }

        FunctionBarPresentationResult prepared;
        try
        {
            prepared = _functionBarPresentation.Prepare(
                CreateFunctionBarRequest(selection));
        }
        catch (Exception exception)
        {
            TraceDiagnostic(
                selection.SessionId,
                "FunctionBar.Prepare.Exception",
                selection,
                exception: exception);
            throw;
        }

        if (prepared.Kind != FunctionBarPresentationResultKind.Ready)
        {
            var failure = prepared.Failure ?? CreateFailure(
                selection.SessionId,
                FailureCode.FunctionBarPresentationFailed,
                "The Function Bar could not be prepared.");
            TraceDiagnostic(
                selection.SessionId,
                "FunctionBar.Prepare.Failed",
                selection,
                failure);
            Observe(FailCurrentAsync(failure));
            return;
        }

        TraceDiagnostic(selection.SessionId, "FunctionBar.Prepare.Ready", selection);

        var transition = _stateAuthority.RequestTransition(new(
            WorkflowState.SelectionLocked,
            WorkflowState.Editing,
            "FunctionBarReady"));
        if (!transition.IsSuccess)
        {
            _functionBarPresentation.Close(selection.SessionId);
            var failure = transition.Failure ?? CreateFailure(
                selection.SessionId,
                FailureCode.InvalidStateTransition,
                "The workflow could not enter Editing.");
            TraceDiagnostic(
                selection.SessionId,
                "FunctionBar.EditingTransition.Failed",
                selection,
                failure);
            Observe(FailCurrentAsync(failure));
            return;
        }

        _annotationDocuments.BeginSession(selection.SessionId);
        _annotationEditing.BeginSession(selection);
        _annotationObjectEditing.BeginSession(selection);
        _overlayCoordinator.ApplyAnnotation(CreateAnnotationPresentation(selection));

        FunctionBarPresentationResult refreshed;
        try
        {
            refreshed = _functionBarPresentation.Reposition(
                CreateFunctionBarRequest(selection));
        }
        catch (Exception exception)
        {
            TraceDiagnostic(
                selection.SessionId,
                "FunctionBar.Reposition.Exception",
                selection,
                exception: exception);
            throw;
        }
        if (refreshed.Kind != FunctionBarPresentationResultKind.Ready)
        {
            TraceDiagnostic(
                selection.SessionId,
                "FunctionBar.Reposition.Failed",
                selection,
                refreshed.Failure);
            Observe(CancelCurrentAsync("FunctionBarAvailabilityRefreshFailed"));
            return;
        }

        FunctionBarPresentationResult shown;
        try
        {
            shown = _functionBarPresentation.Show(
                selection.SessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision);
        }
        catch (Exception exception)
        {
            TraceDiagnostic(
                selection.SessionId,
                "FunctionBar.Show.Exception",
                selection,
                exception: exception);
            throw;
        }
        if (shown.Kind != FunctionBarPresentationResultKind.Shown)
        {
            TraceDiagnostic(
                selection.SessionId,
                "FunctionBar.Show.Failed",
                selection,
                shown.Failure);
            Observe(CancelCurrentAsync("FunctionBarShowFailed"));
            return;
        }

        TraceDiagnostic(selection.SessionId, "FunctionBar.Show.Succeeded", selection);
    }

    private FunctionBarPresentationRequest CreateFunctionBarRequest(
        SelectionVisualState selection,
        FunctionBarCommandAvailability? availability = null) => new(
        selection.SessionId,
        selection.CoordinateVersion,
        selection,
        availability ?? CreateFunctionBarAvailability(),
        this)
        {
            ActiveTool = _annotationEditing.ActiveTool,
            AnnotationRevision = _annotationEditing.CurrentAnnotationRevision,
            ActiveRectangleStyle = _annotationEditing.ActiveRectangleStyle,
            ActiveArrowLineStyle = _annotationEditing.ActiveArrowLineStyle,
            ActiveHighlighterStyle = _annotationEditing.ActiveHighlighterStyle,
            ActiveTextStyle = _annotationEditing.ActiveTextStyle,
            ActiveNumberedMarkerStyle = _annotationEditing.ActiveNumberedMarkerStyle,
            ActiveArrowLineEndStyle = _annotationEditing.ActiveArrowLineEndStyle,
            ToolSelectionSink = this,
            ActivePrivacyRegionMode = _annotationEditing.ActivePrivacyRegionMode,
            PrivacyRegionModeSelectionSink = this,
            ActiveNumberedMarkerNextNumber = _annotationEditing.ActiveNumberedMarkerNextNumber,
            NextNumberSelectionSink = this,
            SelectedObject = _annotationObjectEditing.State,
            AnnotationObjectEditingSink = this
        };

    private FunctionBarCommandAvailability CreateFunctionBarAvailability()
    {
        if (_stateAuthority.CurrentState != WorkflowState.Editing)
        {
            return FunctionBarCommandAvailability.Stage6C;
        }

        var history = _annotationHistory.CurrentState;
        var historyEnabled = !_annotationEditing.HasActiveDraft
            && !_annotationObjectEditing.HasActiveEdit;
        var selection = CurrentSelection;
        var document = _annotationDocuments.Current;
        var hasAnnotations = document?.Objects.Count > 0;
        var hasValidSelection = selection is not null
            && selection.Status == SelectionStatus.Locked
            && selection.InteractionMode == SelectionInteractionMode.Locked
            && selection.IsGeometryValid
            && selection.NormalizedPhysicalBounds is not null;
        var hasRenderer = hasAnnotations == true
            ? _annotationAwareRenderer is not null
            : _finalRenderer is not null;
        var canSave = historyEnabled
            && hasValidSelection
            && hasRenderer
            && _outputCommitment is not null
            && _pngSaveCoordinator is not null;

        return FunctionBarCommandAvailability.Stage7N with
        {
            CanSave = canSave,
            CanUndo = historyEnabled && history.CanUndo,
            CanRedo = historyEnabled && history.CanRedo
        };
    }

    private FunctionBarCommandResult ToFunctionBarCommandResult(
        AnnotationHistoryResult result) => new(
        result.Command == AnnotationHistoryCommand.Undo
            ? FunctionBarCommand.Undo
            : FunctionBarCommand.Redo,
        result.Kind switch
        {
            AnnotationHistoryResultKind.Succeeded => FunctionBarCommandResultKind.Accepted,
            AnnotationHistoryResultKind.StaleSession => FunctionBarCommandResultKind.StaleSession,
            AnnotationHistoryResultKind.StaleSelectionRevision => FunctionBarCommandResultKind.StaleSelectionRevision,
            AnnotationHistoryResultKind.StaleAnnotationRevision => FunctionBarCommandResultKind.StaleAnnotationRevision,
            AnnotationHistoryResultKind.InvalidWorkflowState => FunctionBarCommandResultKind.InvalidWorkflowState,
            AnnotationHistoryResultKind.ActiveDraft => FunctionBarCommandResultKind.ActiveDraft,
            AnnotationHistoryResultKind.ObjectConflict => FunctionBarCommandResultKind.ObjectConflict,
            AnnotationHistoryResultKind.RevisionOverflow => FunctionBarCommandResultKind.RevisionOverflow,
            AnnotationHistoryResultKind.NothingToUndo => FunctionBarCommandResultKind.NothingToUndo,
            AnnotationHistoryResultKind.NothingToRedo => FunctionBarCommandResultKind.NothingToRedo,
            AnnotationHistoryResultKind.Disabled => FunctionBarCommandResultKind.Disabled,
            _ => FunctionBarCommandResultKind.Failed
        },
        _stateAuthority.CurrentState,
        result.SelectionRevision,
        result.Failure,
        result.Message)
        {
            CurrentAnnotationRevision = result.CurrentAnnotationRevision,
            CanUndo = result.CanUndo,
            CanRedo = result.CanRedo
        };

    private void RecordDirectMutation(
        AnnotationMutationRequest request,
        AnnotationDocument? before,
        AnnotationMutationResult result)
    {
        if (before is null
            || result is not AnnotationMutationResult.Succeeded succeeded
            || CurrentSelection is not SelectionVisualState selection)
        {
            return;
        }

        switch (request)
        {
            case AddAnnotationObjectRequest add when add.AnnotationObject is AnnotationObject added:
                _annotationHistory.RecordAdd(
                    request.SessionId,
                    selection.CoordinateVersion,
                    selection.SelectionRevision,
                    before,
                    succeeded.Document,
                    added);
                break;
            case ReplaceAnnotationObjectRequest replace when replace.AnnotationObject is AnnotationObject replaced:
                var original = before.Objects.FirstOrDefault(value => value.ObjectId == replaced.ObjectId);
                if (original is not null)
                {
                    _annotationHistory.RecordReplace(
                        request.SessionId,
                        selection.CoordinateVersion,
                        selection.SelectionRevision,
                        before,
                        succeeded.Document,
                        original,
                        replaced);
                }

                break;
            case RemoveAnnotationObjectRequest remove:
                var removed = before.Objects.FirstOrDefault(value => value.ObjectId == remove.ObjectId);
                if (removed is not null)
                {
                    _annotationHistory.RecordRemove(
                        request.SessionId,
                        selection.CoordinateVersion,
                        selection.SelectionRevision,
                        before,
                        succeeded.Document,
                        removed);
                }

                break;
        }
    }

    private abstract record CanonicalRenderOutcome
    {
        public sealed record Succeeded(
            IImageResult Result,
            FrozenDisplayFrameSet FrameSet,
            PhysicalRect Bounds) : CanonicalRenderOutcome;

        public sealed record Failed(Failure Failure) : CanonicalRenderOutcome;
    }

    private async ValueTask<CanonicalRenderOutcome> RenderCanonicalResultAsync(
        CaptureSessionContext session,
        SelectionVisualState selection,
        AnnotationDocument annotationDocument,
        string operation)
    {
        IImageResult? result = null;
        try
        {
            if (!IsCurrentEditingSession(session, selection))
            {
                return new CanonicalRenderOutcome.Failed(CreateFailure(
                    session.SessionId,
                    FailureCode.StaleSession,
                    $"The capture session is no longer current for {operation}."));
            }

            var bounds = selection.NormalizedPhysicalBounds!.Value;
            var frameSet = session.FrozenDisplayFrames;
            var topologyCapacity = _capacityPolicy.ValidateTopology(
                session.VirtualDesktopSnapshot);
            var selectionCapacity = _capacityPolicy.ValidateSelection(bounds);
            if (!topologyCapacity.IsSupported || !selectionCapacity.IsSupported)
            {
                return new CanonicalRenderOutcome.Failed(CreateFailure(
                    session.SessionId,
                    FailureCode.UnsupportedCapacity,
                    !topologyCapacity.IsSupported
                        ? topologyCapacity.UserMessage
                        : selectionCapacity.UserMessage));
            }

            if (annotationDocument.SessionId != session.SessionId
                || annotationDocument.Revision != CurrentAnnotationRevision)
            {
                return new CanonicalRenderOutcome.Failed(CreateFailure(
                    session.SessionId,
                    FailureCode.StaleAnnotationRevision,
                    $"The Annotation Document changed before {operation} rendering started."));
            }

            if (frameSet is null
                || frameSet.IsDisposed
                || frameSet.SessionId != session.SessionId
                || !string.Equals(
                    frameSet.CoordinateVersion,
                    session.VirtualDesktopSnapshot.CoordinateVersion,
                    StringComparison.Ordinal))
            {
                return new CanonicalRenderOutcome.Failed(CreateFailure(
                    session.SessionId,
                    FailureCode.InvalidResultLifetime,
                    $"The frozen display frame set is unavailable for {operation}."));
            }

            TraceStage(
                CompleteExecutionStage.FrozenFrameSetValidated,
                session,
                selection,
                component: $"{nameof(CapturePresentationWorkflowCoordinator)}.{operation}");

            var readyTransition = _stateAuthority.RequestTransition(new(
                WorkflowState.Editing,
                WorkflowState.ResultReady,
                $"{operation}RenderStarted"));
            if (!readyTransition.IsSuccess)
            {
                return new CanonicalRenderOutcome.Failed(readyTransition.Failure ?? CreateFailure(
                    session.SessionId,
                    FailureCode.InvalidStateTransition,
                    $"The workflow could not start the {operation} render."));
            }

            var renderComponent = annotationDocument.Objects.Count > 0
                ? nameof(IAnnotationAwareRenderAdapter)
                : nameof(IFrozenDisplayFrameSetRenderer);
            TraceStage(
                CompleteExecutionStage.Rendering,
                session,
                selection,
                component: renderComponent);

            if (annotationDocument.Objects.Count > 0)
            {
                var annotated = await _annotationAwareRenderer!
                    .RenderAsync(
                        new AnnotationAwareRenderRequest
                        {
                            SessionId = session.SessionId,
                            CoordinateVersion = session.VirtualDesktopSnapshot.CoordinateVersion,
                            SelectionRevision = selection.SelectionRevision,
                            AnnotationRevision = annotationDocument.Revision,
                            SelectionPhysicalBounds = bounds,
                            VirtualDesktopSnapshot = session.VirtualDesktopSnapshot,
                            CapacityValidation = topologyCapacity,
                            FrozenDisplayFrames = frameSet,
                            AnnotationDocument = annotationDocument,
                            Cancellation = session.Cancellation
                        },
                        session.Cancellation)
                    .ConfigureAwait(true);

                switch (annotated)
                {
                    case AnnotationAwareRenderOutcome.Cancelled cancelled:
                        return new CanonicalRenderOutcome.Failed(CreateFailure(
                            session.SessionId,
                            FailureCode.Cancelled,
                            cancelled.CancellationOrigin));
                    case AnnotationAwareRenderOutcome.Failed failed:
                        return new CanonicalRenderOutcome.Failed(failed.Failure);
                    case AnnotationAwareRenderOutcome.Succeeded succeeded:
                        using (succeeded.Result)
                        {
                            var annotatedImage = succeeded.Result.ImageResult;
                            if (succeeded.Result.SessionId != session.SessionId
                                || succeeded.Result.SelectionRevision != selection.SelectionRevision
                                || succeeded.Result.AnnotationRevision != annotationDocument.Revision
                                || !IsCanonicalResult(
                                    annotatedImage,
                                    session.SessionId,
                                    bounds,
                                    selection.SelectionRevision,
                                    annotationDocument.Revision)
                                || annotatedImage.Metadata.ResultId != succeeded.Result.ResultId)
                            {
                                return new CanonicalRenderOutcome.Failed(CreateFailure(
                                    session.SessionId,
                                    FailureCode.InvalidResultLifetime,
                                    "The annotation-aware renderer returned mismatched or non-canonical output."));
                            }

                            result = succeeded.Result.TakeImageResult();
                        }

                        break;
                    default:
                        return new CanonicalRenderOutcome.Failed(
                            MapAnnotationRenderFailure(session.SessionId, annotated));
                }
            }
            else
            {
                var rendered = await _finalRenderer!
                    .RenderAsync(
                        frameSet,
                        bounds,
                        session.Cancellation,
                        selection.SelectionRevision,
                        annotationDocument.Revision)
                    .ConfigureAwait(true);
                switch (rendered)
                {
                    case FrozenDisplayFrameSetRenderOutcome.Cancelled cancelled:
                        return new CanonicalRenderOutcome.Failed(CreateFailure(
                            session.SessionId,
                            FailureCode.Cancelled,
                            cancelled.CancellationOrigin));
                    case FrozenDisplayFrameSetRenderOutcome.Failed failed:
                        return new CanonicalRenderOutcome.Failed(failed.Failure);
                    case FrozenDisplayFrameSetRenderOutcome.Succeeded succeeded:
                        result = succeeded.ImageResult;
                        break;
                    default:
                        return new CanonicalRenderOutcome.Failed(CreateFailure(
                            session.SessionId,
                            FailureCode.RenderingFailed,
                            "The final renderer returned an unknown outcome."));
                }
            }

            if (!IsCurrentRenderContext(session, selection, annotationDocument))
            {
                return new CanonicalRenderOutcome.Failed(CreateFailure(
                    session.SessionId,
                    CurrentSelectionRevision != selection.SelectionRevision
                        ? FailureCode.StaleSelectionRevision
                        : FailureCode.StaleAnnotationRevision,
                    $"The Selection or Annotation revision changed during {operation} rendering."));
            }

            TraceStage(
                CompleteExecutionStage.RenderSucceeded,
                session,
                selection,
                result: result,
                component: renderComponent);
            TraceStage(
                CompleteExecutionStage.ResultValidation,
                session,
                selection,
                result: result,
                component: nameof(CapturePresentationWorkflowCoordinator));
            if (!IsCanonicalResult(
                    result,
                    session.SessionId,
                    bounds,
                    selection.SelectionRevision,
                    annotationDocument.Revision))
            {
                return new CanonicalRenderOutcome.Failed(CreateFailure(
                    session.SessionId,
                    FailureCode.InvalidResultLifetime,
                    "The final render did not produce a valid canonical Selection result."));
            }

            TraceStage(
                CompleteExecutionStage.TransitioningToResultReady,
                session,
                selection,
                result: result,
                component: nameof(WorkflowStateAuthority));
            var completedResult = result;
            result = null;
            return new CanonicalRenderOutcome.Succeeded(
                completedResult,
                frameSet,
                bounds);
        }
        catch (OperationCanceledException)
        {
            return new CanonicalRenderOutcome.Failed(CreateFailure(
                session.SessionId,
                FailureCode.Cancelled,
                "CancellationToken"));
        }
        catch (Exception exception)
        {
            return new CanonicalRenderOutcome.Failed(CreateFailure(
                session.SessionId,
                FailureCode.UnexpectedFailure,
                exception.GetType().Name,
                exception.HResult));
        }
        finally
        {
            result?.Dispose();
        }
    }

    private async ValueTask SaveAsync(
        CaptureSessionContext session,
        SelectionVisualState selection,
        AnnotationDocument annotationDocument)
    {
        IImageResult? result = null;
        try
        {
            var rendered = await RenderCanonicalResultAsync(
                    session,
                    selection,
                    annotationDocument,
                    "Save")
                .ConfigureAwait(true);
            if (rendered is CanonicalRenderOutcome.Failed failedRender)
            {
                ReturnToEditing(session, failedRender.Failure);
                return;
            }

            var succeededRender = (CanonicalRenderOutcome.Succeeded)rendered;
            result = succeededRender.Result;
            var savingTransition = _stateAuthority.RequestTransition(new(
                WorkflowState.ResultReady,
                WorkflowState.Saving,
                "SaveRenderSucceeded"));
            if (!savingTransition.IsSuccess)
            {
                ReturnToEditing(session, savingTransition.Failure ?? CreateFailure(
                    session.SessionId,
                    FailureCode.InvalidStateTransition,
                    "The workflow could not enter Saving."));
                return;
            }

            var saveResult = await _pngSaveCoordinator!
                .SaveAsync(result, session.Cancellation)
                .ConfigureAwait(true);
            if (!IsCurrentRenderContext(session, selection, annotationDocument))
            {
                ReturnToEditing(session, CreateFailure(
                    session.SessionId,
                    FailureCode.StaleSession,
                    "The capture context changed before PNG output could be committed."));
                return;
            }

            switch (saveResult)
            {
                case PngSaveResult.Saved saved
                    when saved.SessionId == session.SessionId
                        && saved.ResultId == result.Metadata.ResultId
                        && saved.PngWrite.SessionId == session.SessionId
                        && saved.PngWrite.ResultId == result.Metadata.ResultId:
                    await DeliverResultAsync(
                            session,
                            selection,
                            annotationDocument,
                            succeededRender.FrameSet,
                            result,
                            OutputCommitmentAuthorization.CreateSuccessfulSave(
                                session.SessionId,
                                session.VirtualDesktopSnapshot.CoordinateVersion,
                                selection.SelectionRevision,
                                annotationDocument.Revision,
                                result.Metadata.ResultId,
                                saved.PngWrite))
                        .ConfigureAwait(true);
                    return;
                case PngSaveResult.SaveDialogCancelled cancelled:
                    ReturnToEditing(
                        session,
                        CreateFailure(
                            session.SessionId,
                            FailureCode.Cancelled,
                            cancelled.CancellationOrigin),
                        showFeedback: false);
                    return;
                case PngSaveResult.Cancelled cancelled:
                    ReturnToEditing(
                        session,
                        CreateFailure(
                            session.SessionId,
                            FailureCode.Cancelled,
                            cancelled.CancellationOrigin),
                        showFeedback: false);
                    return;
                case PngSaveResult.SaveDialogFailed failed:
                    ReturnToEditing(session, failed.Failure);
                    return;
                case PngSaveResult.PngEncodingFailed failed:
                    ReturnToEditing(session, failed.Failure);
                    return;
                case PngSaveResult.PngWriteFailed failed:
                    ReturnToEditing(session, failed.Failure);
                    return;
                case PngSaveResult.Rejected rejected:
                    ReturnToEditing(session, rejected.Failure);
                    return;
                case PngSaveResult.Saved:
                default:
                    ReturnToEditing(session, CreateFailure(
                        session.SessionId,
                        FailureCode.InvalidResultLifetime,
                        "PNG Save As returned mismatched output evidence."));
                    return;
            }
        }
        catch (OperationCanceledException)
        {
            ReturnToEditing(
                session,
                CreateFailure(session.SessionId, FailureCode.Cancelled, "CancellationToken"),
                showFeedback: false);
        }
        catch (Exception exception)
        {
            ReturnToEditing(
                session,
                CreateFailure(
                    session.SessionId,
                    FailureCode.UnexpectedFailure,
                    exception.GetType().Name,
                    exception.HResult));
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

    private async ValueTask DeliverResultAsync(
        CaptureSessionContext session,
        SelectionVisualState selection,
        AnnotationDocument annotationDocument,
        FrozenDisplayFrameSet frameSet,
        IImageResult result,
        OutputCommitmentAuthorization authorization)
    {
        TraceStage(
            CompleteExecutionStage.TransitioningToDelivering,
            session,
            selection,
            result: result,
            component: nameof(WorkflowStateAuthority));
        var fromState = _stateAuthority.CurrentState;
        var deliveryTransition = _stateAuthority.RequestTransition(new(
            fromState,
            WorkflowState.Delivering,
            authorization is OutputCommitmentAuthorization.SuccessfulSave
                ? "PngWriteSucceeded"
                : "CompleteRenderSucceeded"));
        if (!deliveryTransition.IsSuccess)
        {
            ReturnToEditing(session, deliveryTransition.Failure ?? CreateFailure(
                session.SessionId,
                FailureCode.InvalidStateTransition,
                "The workflow could not start Clipboard delivery."));
            return;
        }

        var delivery = await _outputCommitment!
            .PublishAsync(
                new OutputCommitmentRequest
                {
                    Authorization = authorization,
                    ImageResult = result,
                    WorkflowState = WorkflowState.Delivering,
                    SelectionWidth = selection.NormalizedPhysicalBounds!.Value.Width,
                    SelectionHeight = selection.NormalizedPhysicalBounds!.Value.Height,
                    DisplayCount = frameSet.Frames.Count,
                    Cancellation = session.Cancellation
                },
                session.Cancellation)
            .ConfigureAwait(true);

        switch (delivery)
        {
            case ClipboardDeliveryResult.Delivered delivered
                when delivered.SessionId == session.SessionId
                    && delivered.ResultId == result.Metadata.ResultId
                    && IsCurrentRenderContext(session, selection, annotationDocument):
                var completedTransition = _stateAuthority.RequestTransition(new(
                    WorkflowState.Delivering,
                    WorkflowState.Completed,
                    "ClipboardDelivered"));
                if (!completedTransition.IsSuccess)
                {
                    ReturnToEditing(session, completedTransition.Failure ?? CreateFailure(
                        session.SessionId,
                        FailureCode.InvalidStateTransition,
                        "The workflow could not complete after Clipboard delivery."));
                    return;
                }

                TraceStage(
                    CompleteExecutionStage.ClipboardDelivered,
                    session,
                    selection,
                    result: result,
                    clipboardAttempt: delivered.Attempts,
                    component: nameof(IOutputCommitmentCoordinator));
                TraceStage(
                    CompleteExecutionStage.Completed,
                    session,
                    selection,
                    result: result,
                    clipboardAttempt: delivered.Attempts,
                    component: nameof(CapturePresentationWorkflowCoordinator));
                await CompleteSessionAsync(session).ConfigureAwait(true);
                return;
            case ClipboardDeliveryResult.Cancelled cancelled:
                ReturnToEditing(
                    session,
                    CreateFailure(
                        session.SessionId,
                        FailureCode.Cancelled,
                        cancelled.CancellationOrigin),
                    showFeedback: false);
                return;
            case ClipboardDeliveryResult.RetryableFailure retryable:
                ReturnToEditing(session, retryable.Failure);
                return;
            case ClipboardDeliveryResult.TerminalFailure terminal:
                ReturnToEditing(session, terminal.Failure);
                return;
            default:
                ReturnToEditing(session, CreateFailure(
                    session.SessionId,
                    FailureCode.ClipboardPublicationRejected,
                    "Clipboard delivery returned an unknown outcome."));
                return;
        }
    }

    private async ValueTask CompleteAsync(
        CaptureSessionContext session,
        SelectionVisualState selection,
        AnnotationDocument annotationDocument)
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
            var topologyCapacity = _capacityPolicy.ValidateTopology(
                session.VirtualDesktopSnapshot);
            var selectionCapacity = _capacityPolicy.ValidateSelection(bounds);
            if (!topologyCapacity.IsSupported || !selectionCapacity.IsSupported)
            {
                var capacityFailure = CreateFailure(
                    session.SessionId,
                    FailureCode.UnsupportedCapacity,
                    !topologyCapacity.IsSupported
                        ? topologyCapacity.UserMessage
                        : selectionCapacity.UserMessage);
                TraceStage(
                    CompleteExecutionStage.RenderFailed,
                    session,
                    selection,
                    capacityFailure,
                    component: nameof(SupportedCapacityPolicy));
                ReturnToEditing(session, capacityFailure);
                return;
            }

            if (annotationDocument.SessionId != session.SessionId
                || annotationDocument.Revision != CurrentAnnotationRevision)
            {
                var annotationFailure = CreateFailure(
                    session.SessionId,
                    FailureCode.StaleAnnotationRevision,
                    "The Annotation Document changed before Complete rendering started.");
                TraceStage(
                    CompleteExecutionStage.RenderFailed,
                    session,
                    selection,
                    annotationFailure,
                    component: nameof(AnnotationDocumentCoordinator));
                ReturnToEditing(session, annotationFailure);
                return;
            }

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

            var renderComponent = annotationDocument.Objects.Count > 0
                ? nameof(IAnnotationAwareRenderAdapter)
                : nameof(IFrozenDisplayFrameSetRenderer);
            TraceStage(
                CompleteExecutionStage.Rendering,
                session,
                selection,
                component: renderComponent);

            if (annotationDocument.Objects.Count > 0)
            {
                var annotated = await _annotationAwareRenderer!
                    .RenderAsync(
                        new AnnotationAwareRenderRequest
                        {
                            SessionId = session.SessionId,
                            CoordinateVersion = session.VirtualDesktopSnapshot.CoordinateVersion,
                            SelectionRevision = selection.SelectionRevision,
                            AnnotationRevision = annotationDocument.Revision,
                            SelectionPhysicalBounds = bounds,
                            VirtualDesktopSnapshot = session.VirtualDesktopSnapshot,
                            CapacityValidation = topologyCapacity,
                            FrozenDisplayFrames = frameSet,
                            AnnotationDocument = annotationDocument,
                            Cancellation = session.Cancellation
                        },
                        session.Cancellation)
                    .ConfigureAwait(true);

                switch (annotated)
                {
                    case AnnotationAwareRenderOutcome.Cancelled cancelled:
                        ReturnRenderFailure(
                            session,
                            selection,
                            CreateFailure(
                                session.SessionId,
                                FailureCode.Cancelled,
                                cancelled.CancellationOrigin),
                            renderComponent);
                        return;
                    case AnnotationAwareRenderOutcome.Failed failed:
                        ReturnRenderFailure(
                            session,
                            selection,
                            failed.Failure,
                            renderComponent);
                        return;
                    case AnnotationAwareRenderOutcome.Succeeded succeeded:
                        using (succeeded.Result)
                        {
                            var annotatedImage = succeeded.Result.ImageResult;
                            if (succeeded.Result.SessionId != session.SessionId
                                || succeeded.Result.SelectionRevision != selection.SelectionRevision
                                || succeeded.Result.AnnotationRevision != annotationDocument.Revision
                                || !IsCanonicalResult(
                                    annotatedImage,
                                    session.SessionId,
                                    bounds,
                                    selection.SelectionRevision,
                                    annotationDocument.Revision)
                                || annotatedImage.Metadata.ResultId != succeeded.Result.ResultId)
                            {
                                ReturnRenderFailure(
                                    session,
                                    selection,
                                    CreateFailure(
                                        session.SessionId,
                                        FailureCode.InvalidResultLifetime,
                                        "The annotation-aware renderer returned mismatched or non-canonical output."),
                                    renderComponent);
                                return;
                            }

                            result = succeeded.Result.TakeImageResult();
                        }

                        break;
                    default:
                        ReturnRenderFailure(
                            session,
                            selection,
                            MapAnnotationRenderFailure(session.SessionId, annotated),
                            renderComponent);
                        return;
                }
            }
            else
            {
                var rendered = await _finalRenderer!
                    .RenderAsync(
                        frameSet,
                        bounds,
                        session.Cancellation,
                        selection.SelectionRevision,
                        annotationDocument.Revision)
                    .ConfigureAwait(true);
                switch (rendered)
                {
                    case FrozenDisplayFrameSetRenderOutcome.Cancelled cancelled:
                        ReturnRenderFailure(
                            session,
                            selection,
                            CreateFailure(
                                session.SessionId,
                                FailureCode.Cancelled,
                                cancelled.CancellationOrigin),
                            renderComponent);
                        return;
                    case FrozenDisplayFrameSetRenderOutcome.Failed failed:
                        ReturnRenderFailure(session, selection, failed.Failure, renderComponent);
                        return;
                    case FrozenDisplayFrameSetRenderOutcome.Succeeded succeeded:
                        result = succeeded.ImageResult;
                        break;
                    default:
                        ReturnRenderFailure(
                            session,
                            selection,
                            CreateFailure(
                                session.SessionId,
                                FailureCode.RenderingFailed,
                                "The final renderer returned an unknown outcome."),
                            renderComponent);
                        return;
                }
            }

            if (!IsCurrentRenderContext(session, selection, annotationDocument))
            {
                var staleFailure = CreateFailure(
                    session.SessionId,
                    CurrentSelectionRevision != selection.SelectionRevision
                        ? FailureCode.StaleSelectionRevision
                        : FailureCode.StaleAnnotationRevision,
                    "The Selection or Annotation revision changed during Complete rendering.");
                TraceStage(
                    CompleteExecutionStage.RenderFailed,
                    session,
                    selection,
                    staleFailure,
                    result,
                    renderComponent);
                ReturnToEditing(session, staleFailure);
                return;
            }

            TraceStage(
                CompleteExecutionStage.RenderSucceeded,
                session,
                selection,
                result: result,
                component: renderComponent);

            TraceStage(
                CompleteExecutionStage.ResultValidation,
                session,
                selection,
                result: result,
                component: nameof(CapturePresentationWorkflowCoordinator));
            if (!IsCanonicalResult(
                    result,
                    session.SessionId,
                    bounds,
                    selection.SelectionRevision,
                    annotationDocument.Revision))
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

            var delivery = await _outputCommitment!
                .PublishAsync(
                    new OutputCommitmentRequest
                    {
                        Authorization = OutputCommitmentAuthorization.CreateComplete(
                            session.SessionId,
                            session.VirtualDesktopSnapshot.CoordinateVersion,
                            selection.SelectionRevision,
                            annotationDocument.Revision,
                            result.Metadata.ResultId),
                        ImageResult = result,
                        WorkflowState = WorkflowState.Delivering,
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
                        component: nameof(IOutputCommitmentCoordinator));
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
                        nameof(IOutputCommitmentCoordinator));
                    ReturnToEditing(session, cancelledFailure);
                    return;
                case ClipboardDeliveryResult.RetryableFailure retryable:
                    TraceStage(
                        CompleteExecutionStage.ClipboardFailed,
                        session,
                        selection,
                        retryable.Failure,
                        result,
                        nameof(IOutputCommitmentCoordinator),
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
                        nameof(IOutputCommitmentCoordinator));
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
                        nameof(IOutputCommitmentCoordinator));
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

    private void ReturnRenderFailure(
        CaptureSessionContext session,
        SelectionVisualState selection,
        Failure failure,
        string component)
    {
        TraceStage(
            CompleteExecutionStage.RenderFailed,
            session,
            selection,
            failure,
            component: component);
        ReturnToEditing(session, failure);
    }

    private static Failure MapAnnotationRenderFailure(
        Guid sessionId,
        AnnotationAwareRenderOutcome outcome) => outcome switch
        {
            AnnotationAwareRenderOutcome.StaleSession stale => CreateFailure(
                sessionId,
                FailureCode.StaleSession,
                stale.Message),
            AnnotationAwareRenderOutcome.StaleCoordinateVersion stale => CreateFailure(
                sessionId,
                FailureCode.DisplayContextChanged,
                stale.Message),
            AnnotationAwareRenderOutcome.StaleSelectionRevision stale => CreateFailure(
                sessionId,
                FailureCode.StaleSelectionRevision,
                stale.Message),
            AnnotationAwareRenderOutcome.StaleAnnotationRevision stale => CreateFailure(
                sessionId,
                FailureCode.StaleAnnotationRevision,
                stale.Message),
            AnnotationAwareRenderOutcome.InvalidSelection invalid => CreateFailure(
                sessionId,
                FailureCode.InvalidSelection,
                invalid.Message),
            AnnotationAwareRenderOutcome.InvalidFrameSet invalid => CreateFailure(
                sessionId,
                FailureCode.InvalidResultLifetime,
                invalid.Message),
            AnnotationAwareRenderOutcome.InvalidAnnotationDocument invalid => CreateFailure(
                sessionId,
                FailureCode.StaleAnnotationRevision,
                invalid.Message),
            AnnotationAwareRenderOutcome.UnsupportedAnnotation unsupported => CreateFailure(
                sessionId,
                FailureCode.AnnotationOutputNotSupported,
                unsupported.Message),
            AnnotationAwareRenderOutcome.RenderCapacityExceeded capacity => CreateFailure(
                sessionId,
                FailureCode.UnsupportedCapacity,
                capacity.Message),
            AnnotationAwareRenderOutcome.Failed failed => failed.Failure,
            AnnotationAwareRenderOutcome.Cancelled cancelled => CreateFailure(
                sessionId,
                FailureCode.Cancelled,
                cancelled.CancellationOrigin),
            _ => CreateFailure(
                sessionId,
                FailureCode.RenderingFailed,
                "The annotation-aware renderer returned an unknown outcome.")
        };

    private bool IsCurrentRenderContext(
        CaptureSessionContext session,
        SelectionVisualState selection,
        AnnotationDocument annotationDocument) =>
        IsCurrentRenderSession(session, selection)
        && ReferenceEquals(_annotationDocuments.Current, annotationDocument)
        && CurrentAnnotationRevision == annotationDocument.Revision;

    private bool IsCurrentRenderSession(
        CaptureSessionContext session,
        SelectionVisualState selection)
    {
        lock (_gate)
        {
            return !_disposed
                && ReferenceEquals(_activeSession, session)
                && _selectionCoordinator?.State.SelectionRevision == selection.SelectionRevision
                && (_stateAuthority.CurrentState is WorkflowState.Editing
                    or WorkflowState.ResultReady
                    or WorkflowState.Saving
                    or WorkflowState.Delivering);
        }
    }

    private static bool IsCanonicalResult(
        IImageResult? result,
        Guid sessionId,
        PhysicalRect bounds,
        int selectionRevision,
        AnnotationRevision annotationRevision) =>
        result is not null
        && !result.IsDisposed
        && result.Metadata.ResultId != Guid.Empty
        && result.Metadata.SessionId == sessionId
        && result.Metadata.SelectionRevision == selectionRevision
        && result.Metadata.AnnotationRevision == annotationRevision
        && result.Metadata.CropPhysicalBounds == bounds
        && result.Metadata.SourcePhysicalBounds == bounds
        && result.Metadata.PixelWidth == bounds.Width
        && result.Metadata.PixelHeight == bounds.Height
        && result.Metadata.PixelFormat == ImagePixelFormat.Bgra8
        && result.Metadata.AlphaMode == ImageAlphaMode.Premultiplied
        && result.Metadata.ColorSpace == ImageColorSpace.SrgbSdr
        && result.Metadata.DpiX == 96
        && result.Metadata.DpiY == 96
        && result.Metadata.RowStride >= checked(bounds.Width * 4);

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

    private void ReturnToEditing(
        CaptureSessionContext session,
        Failure failure,
        bool showFeedback = true)
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
        if (currentState is WorkflowState.ResultReady
            or WorkflowState.Saving
            or WorkflowState.Delivering)
        {
            _stateAuthority.RequestTransition(new(
                currentState,
                WorkflowState.Editing,
                $"CompleteFailed:{failure.Code}"));
        }

        if (showFeedback)
        {
            _feedback?.Invoke(failure.UserMessageKey);
        }
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
                CreateFunctionBarRequest(selection));
            if (repositioned.Kind == FunctionBarPresentationResultKind.Ready)
            {
                var shown = _functionBarPresentation.Show(
                    selection.SessionId,
                    selection.CoordinateVersion,
                    selection.SelectionRevision);
                if (shown.Kind == FunctionBarPresentationResultKind.Shown
                    && showFeedback)
                {
                    _functionBarPresentation.ShowFeedback(
                        selection.SessionId,
                        selection.CoordinateVersion,
                        selection.SelectionRevision,
                        GetCaptureFailureMessage(failure));
                }
            }

            _overlayCoordinator.ApplyAnnotation(CreateAnnotationPresentation(selection));
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
            _annotationObjectEditing.ClearSession(session.SessionId);
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
        TraceDiagnostic(
            expectedSession?.SessionId ?? failure.CorrelationId,
            "Capture.Cleanup.Begin",
            failure: failure);
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
            _annotationObjectEditing.ClearSession(session.SessionId);
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
        TraceDiagnostic(
            expectedSession?.SessionId ?? failure.CorrelationId,
            "Capture.Cleanup.Completed",
            failure: failure);
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
        _annotationObjectEditing.ClearSession(session.SessionId);
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

    private void TraceDiagnostic(
        Guid sessionId,
        string diagnosticEvent,
        SelectionVisualState? selection = null,
        Failure? failure = null,
        Exception? exception = null,
        string component = nameof(CapturePresentationWorkflowCoordinator))
    {
        try
        {
            var bounds = selection?.NormalizedPhysicalBounds;
            var displayCount = 0;
            lock (_gate)
            {
                displayCount = _activeSession?.FrozenDisplayFrames?.Frames.Count ?? 0;
            }

            _trace.Record(new CompleteExecutionTraceEntry
            {
                TimestampUtc = DateTimeOffset.UtcNow,
                SessionId = sessionId,
                SelectionRevision = selection?.SelectionRevision ?? -1,
                WorkflowState = _stateAuthority.CurrentState,
                CompleteStage = CompleteExecutionStage.Diagnostic,
                FailureCode = failure?.Code,
                FailureCategory = failure?.Category,
                NativeCode = failure?.NativeCode ?? exception?.HResult,
                Component = component,
                SelectionWidth = bounds?.Width ?? 0,
                SelectionHeight = bounds?.Height ?? 0,
                DisplayCount = displayCount,
                ManagedThreadId = Environment.CurrentManagedThreadId,
                DiagnosticEvent = diagnosticEvent,
                DiagnosticMessage = failure?.DiagnosticMessage,
                ExceptionType = exception?.GetType().FullName
            });
        }
        catch
        {
            // Diagnostics must never change the capture outcome.
        }
    }

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
