using SnipPlus.Contracts;

namespace SnipPlus.Core;

public interface IRectangleAnnotationStylePolicy
{
    RectangleAnnotationStyle GetDefaultStyle();
}

public interface IArrowLineAnnotationStylePolicy
{
    ArrowLineAnnotationStyle GetDefaultStyle();
}

public interface IHighlighterAnnotationStylePolicy
{
    HighlighterAnnotationStyle GetDefaultStyle();
}

public interface ITextAnnotationStylePolicy
{
    TextAnnotationStyle GetDefaultStyle();
}

public interface IPrivacyRegionEffectPolicy
{
    PrivacyRegionMode GetDefaultMode();

    PrivacyRegionEffectParameters GetParameters(PrivacyRegionMode mode);
}

public interface INumberedMarkerAnnotationStylePolicy
{
    NumberedMarkerAnnotationStyle GetDefaultStyle();
}

public sealed class DefaultRectangleAnnotationStylePolicy : IRectangleAnnotationStylePolicy
{
    public RectangleAnnotationStyle GetDefaultStyle() => RectangleAnnotationStyle.Default;
}

public sealed class DefaultArrowLineAnnotationStylePolicy : IArrowLineAnnotationStylePolicy
{
    public ArrowLineAnnotationStyle GetDefaultStyle() => ArrowLineAnnotationStyle.Default;
}

public sealed class DefaultHighlighterAnnotationStylePolicy : IHighlighterAnnotationStylePolicy
{
    public HighlighterAnnotationStyle GetDefaultStyle() => HighlighterAnnotationStyle.Default;
}

public sealed class DefaultTextAnnotationStylePolicy : ITextAnnotationStylePolicy
{
    public TextAnnotationStyle GetDefaultStyle() => TextAnnotationStyle.Default;
}

public sealed class DefaultPrivacyRegionEffectPolicy : IPrivacyRegionEffectPolicy
{
    public PrivacyRegionMode GetDefaultMode() => PrivacyRegionMode.Mosaic;

    public PrivacyRegionEffectParameters GetParameters(PrivacyRegionMode mode) => mode switch
    {
        PrivacyRegionMode.Mosaic => new PrivacyRegionEffectParameters(12, 8),
        PrivacyRegionMode.Blur => new PrivacyRegionEffectParameters(12, 8),
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };
}

public sealed class DefaultNumberedMarkerAnnotationStylePolicy : INumberedMarkerAnnotationStylePolicy
{
    public NumberedMarkerAnnotationStyle GetDefaultStyle() => NumberedMarkerAnnotationStyle.Default;
}

public sealed class AnnotationEditingCoordinator
{
    private readonly object _gate = new();
    private readonly AnnotationDocumentCoordinator _documents;
    private readonly Func<AnnotationObjectId> _objectIdFactory;
    private readonly IRectangleAnnotationStylePolicy _stylePolicy;
    private readonly IArrowLineAnnotationStylePolicy _arrowLineStylePolicy;
    private readonly IHighlighterAnnotationStylePolicy _highlighterStylePolicy;
    private readonly ITextAnnotationStylePolicy _textStylePolicy;
    private readonly IPrivacyRegionEffectPolicy _privacyRegionEffectPolicy;
    private readonly INumberedMarkerAnnotationStylePolicy _numberedMarkerStylePolicy;
    private readonly Func<Guid> _textDraftIdFactory;
    private readonly Func<Guid> _privacyDraftIdFactory;
    private Guid? _sessionId;
    private string _coordinateVersion = string.Empty;
    private EditingToolKind _activeTool = EditingToolKind.Selection;
    private ArrowLineEndStyle _arrowLineEndStyle = ArrowLineEndStyle.Arrow;
    private PrivacyRegionMode _privacyRegionMode;
    private int _selectionRevision;
    private RectangleDraft? _draft;
    private ArrowLineDraft? _arrowLineDraft;
    private HighlighterDraft? _highlighterDraft;
    private TextDraft? _textDraft;
    private PrivacyRegionDraft? _privacyRegionDraft;
    private NumberedMarkerDraft? _numberedMarkerDraft;
    private int _nextNumber = 1;
    private RectangleAnnotationStyle _rectangleStyle = RectangleAnnotationStyle.Default;
    private ArrowLineAnnotationStyle _arrowLineStyle = ArrowLineAnnotationStyle.Default;
    private HighlighterAnnotationStyle _highlighterStyle = HighlighterAnnotationStyle.Default;
    private TextAnnotationStyle _textStyle = TextAnnotationStyle.Default;
    private NumberedMarkerAnnotationStyle _numberedMarkerStyle = NumberedMarkerAnnotationStyle.Default;

    public AnnotationEditingCoordinator(
        AnnotationDocumentCoordinator documents,
        Func<AnnotationObjectId>? objectIdFactory = null,
        IRectangleAnnotationStylePolicy? stylePolicy = null,
        IArrowLineAnnotationStylePolicy? arrowLineStylePolicy = null,
        IHighlighterAnnotationStylePolicy? highlighterStylePolicy = null,
        ITextAnnotationStylePolicy? textStylePolicy = null,
        Func<Guid>? textDraftIdFactory = null,
        IPrivacyRegionEffectPolicy? privacyRegionEffectPolicy = null,
        Func<Guid>? privacyDraftIdFactory = null,
        INumberedMarkerAnnotationStylePolicy? numberedMarkerStylePolicy = null)
    {
        _documents = documents ?? throw new ArgumentNullException(nameof(documents));
        _objectIdFactory = objectIdFactory ?? AnnotationObjectId.New;
        _stylePolicy = stylePolicy ?? new DefaultRectangleAnnotationStylePolicy();
        _arrowLineStylePolicy = arrowLineStylePolicy ?? new DefaultArrowLineAnnotationStylePolicy();
        _highlighterStylePolicy = highlighterStylePolicy ?? new DefaultHighlighterAnnotationStylePolicy();
        _textStylePolicy = textStylePolicy ?? new DefaultTextAnnotationStylePolicy();
        _privacyRegionEffectPolicy = privacyRegionEffectPolicy ?? new DefaultPrivacyRegionEffectPolicy();
        _numberedMarkerStylePolicy = numberedMarkerStylePolicy
            ?? new DefaultNumberedMarkerAnnotationStylePolicy();
        _textDraftIdFactory = textDraftIdFactory ?? Guid.NewGuid;
        _privacyDraftIdFactory = privacyDraftIdFactory ?? Guid.NewGuid;
        _privacyRegionMode = _privacyRegionEffectPolicy.GetDefaultMode();
    }

    public EditingToolKind ActiveTool
    {
        get
        {
            lock (_gate)
            {
                return _activeTool;
            }
        }
    }

    public int CurrentSelectionRevision
    {
        get
        {
            lock (_gate)
            {
                return _selectionRevision;
            }
        }
    }

    public AnnotationRevision CurrentAnnotationRevision =>
        _documents.Current?.Revision ?? AnnotationRevision.Initial;

    public ArrowLineEndStyle ActiveArrowLineEndStyle
    {
        get
        {
            lock (_gate)
            {
                return _arrowLineEndStyle;
            }
        }
    }

    public HighlighterAnnotationStyle ActiveHighlighterStyle =>
        _highlighterStyle;

    public TextAnnotationStyle ActiveTextStyle =>
        _textDraft?.Style ?? _textStyle;

    public PrivacyRegionMode ActivePrivacyRegionMode
    {
        get
        {
            lock (_gate)
            {
                return _privacyRegionMode;
            }
        }
    }

    public PrivacyRegionEffectParameters ActivePrivacyRegionEffectParameters =>
        _privacyRegionEffectPolicy.GetParameters(ActivePrivacyRegionMode);

    public NumberedMarkerAnnotationStyle ActiveNumberedMarkerStyle =>
        GetActiveNumberedMarkerStyle();

    public int ActiveNumberedMarkerNextNumber
    {
        get
        {
            lock (_gate)
            {
                return _nextNumber;
            }
        }
    }

    public void BeginSession(SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            _documents.BeginSession(selection.SessionId);
            _sessionId = selection.SessionId;
            _coordinateVersion = selection.CoordinateVersion;
            _selectionRevision = selection.SelectionRevision;
            _activeTool = EditingToolKind.Selection;
            _arrowLineEndStyle = ArrowLineEndStyle.Arrow;
            _privacyRegionMode = _privacyRegionEffectPolicy.GetDefaultMode();
            _draft = null;
            _arrowLineDraft = null;
            _highlighterDraft = null;
            _textDraft = null;
            _privacyRegionDraft = null;
            _numberedMarkerDraft = null;
            _nextNumber = 1;
            _rectangleStyle = _stylePolicy.GetDefaultStyle();
            _arrowLineStyle = _arrowLineStylePolicy.GetDefaultStyle();
            _highlighterStyle = _highlighterStylePolicy.GetDefaultStyle();
            _textStyle = _textStylePolicy.GetDefaultStyle();
            _numberedMarkerStyle = _numberedMarkerStylePolicy.GetDefaultStyle();
        }
    }

    public RectangleAnnotationStyle ActiveRectangleStyle => _rectangleStyle;

    public ArrowLineAnnotationStyle ActiveArrowLineStyle => _arrowLineStyle;

    public AnnotationObjectEditResult ChangeDefaultStyle(
        AnnotationObjectStyleChangeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
            if (_sessionId != request.SessionId
                || !string.Equals(_coordinateVersion, request.CoordinateVersion, StringComparison.Ordinal))
            {
                return DefaultStyleResult(request, AnnotationObjectEditResultKind.StaleSession,
                    "The default style request belongs to a stale capture session.");
            }

            if (_selectionRevision != request.SelectionRevision)
            {
                return DefaultStyleResult(request, AnnotationObjectEditResultKind.StaleSelectionRevision,
                    "The default style request belongs to a stale Selection revision.");
            }

            if (currentRevision != request.ExpectedAnnotationRevision)
            {
                return DefaultStyleResult(request, AnnotationObjectEditResultKind.StaleAnnotationRevision,
                    "The default style request belongs to a stale Annotation revision.");
            }

            try
            {
                var change = request.Change;
                switch (_activeTool)
                {
                    case EditingToolKind.Rectangle:
                        _rectangleStyle = new RectangleAnnotationStyle(
                            change.Color ?? _rectangleStyle.StrokeColor,
                            change.Thickness ?? _rectangleStyle.StrokeThickness);
                        break;
                    case EditingToolKind.ArrowLine:
                        _arrowLineStyle = new ArrowLineAnnotationStyle(
                            change.Color ?? _arrowLineStyle.StrokeColor,
                            change.Thickness ?? _arrowLineStyle.StrokeThickness,
                            change.ArrowLineEndStyle ?? _arrowLineStyle.EndStyle);
                        _arrowLineEndStyle = _arrowLineStyle.EndStyle;
                        break;
                    case EditingToolKind.Highlighter:
                        _highlighterStyle = new HighlighterAnnotationStyle(
                            change.Color ?? _highlighterStyle.StrokeColor,
                            change.Thickness ?? _highlighterStyle.StrokeThickness);
                        break;
                    case EditingToolKind.Text:
                        _textStyle = new TextAnnotationStyle(
                            _textStyle.FontFamily,
                            change.FontSize ?? _textStyle.FontSize,
                            change.Color ?? _textStyle.Color,
                            change.Bold ?? _textStyle.Bold);
                        break;
                    case EditingToolKind.NumberedMarker:
                        _numberedMarkerStyle = new NumberedMarkerAnnotationStyle(
                            change.Color ?? _numberedMarkerStyle.Color,
                            change.MarkerSize ?? _numberedMarkerStyle.Size);
                        break;
                    case EditingToolKind.PrivacyRegion:
                        if (change.PrivacyMode is PrivacyRegionMode mode)
                        {
                            _privacyRegionMode = mode;
                        }
                        break;
                    default:
                        return DefaultStyleResult(request, AnnotationObjectEditResultKind.UnsupportedOperation,
                            "The Selection tool has no creation style defaults.");
                }
            }
            catch (ArgumentException exception)
            {
                return DefaultStyleResult(request, AnnotationObjectEditResultKind.InvalidStyle,
                    exception.Message);
            }

            return DefaultStyleResult(request, AnnotationObjectEditResultKind.Restyled,
                "The active creation style default was updated.");
        }
    }

    public void UpdateSelection(SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            if (_sessionId == selection.SessionId
                && string.Equals(
                    _coordinateVersion,
                    selection.CoordinateVersion,
                    StringComparison.Ordinal))
            {
                if (_selectionRevision != selection.SelectionRevision)
                {
                    _textDraft = null;
                    _privacyRegionDraft = null;
                    _numberedMarkerDraft = null;
                }

                _selectionRevision = selection.SelectionRevision;
            }
        }
    }

    public EditingToolSelectionResult SelectTool(
        EditingToolSelectionRequest request,
        WorkflowState currentState,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            if (currentState != WorkflowState.Editing)
            {
                return ToolResult(
                    EditingToolSelectionResultKind.InvalidWorkflowState,
                    request,
                    CreateFailure(
                        request.SessionId,
                        FailureCode.InvalidStateTransition,
                        "Editing tools can only be selected while the workflow is Editing."),
                    "Editing tools can only be selected while the workflow is Editing.");
            }

            if (!IsCurrentSession(request.SessionId, request.CoordinateVersion))
            {
                return ToolResult(
                    EditingToolSelectionResultKind.StaleSession,
                    request,
                    null,
                    "The editing tool request belongs to a stale capture session.");
            }

            if (request.SelectionRevision != selection.SelectionRevision)
            {
                return ToolResult(
                    EditingToolSelectionResultKind.StaleSelectionRevision,
                    request,
                    null,
                    "The editing tool request belongs to a stale Selection revision.");
            }

            var document = _documents.Current;
            var currentRevision = document?.Revision ?? AnnotationRevision.Initial;
            if (request.ExpectedAnnotationRevision != currentRevision)
            {
                return new EditingToolSelectionResult(
                    EditingToolSelectionResultKind.StaleAnnotationRevision,
                    _activeTool,
                    request.SessionId,
                    request.CoordinateVersion,
                    selection.SelectionRevision,
                    currentRevision,
                    null,
                    "The editing tool request belongs to a stale Annotation revision.");
            }

            if (!Enum.IsDefined(request.Tool))
            {
                return ToolResult(
                    EditingToolSelectionResultKind.Failed,
                    request,
                    CreateFailure(
                        request.SessionId,
                        FailureCode.InvalidStateTransition,
                        "The requested editing tool is not supported."),
                    "The requested editing tool is not supported.");
            }

            var nextPrivacyMode = _privacyRegionMode;
            if (request.Tool == EditingToolKind.PrivacyRegion)
            {
                nextPrivacyMode = request.RequestedPrivacyRegionMode
                    ?? (_activeTool == EditingToolKind.PrivacyRegion
                        ? _privacyRegionMode
                        : _privacyRegionEffectPolicy.GetDefaultMode());
                if (!TryGetPrivacyParameters(nextPrivacyMode, out _, out var privacyFailure))
                {
                    return ToolResult(
                        EditingToolSelectionResultKind.Failed,
                        request,
                        privacyFailure,
                        "The Privacy Region effect parameters are invalid.");
                }
            }

            _activeTool = request.Tool;
            if (request.Tool == EditingToolKind.ArrowLine)
            {
                _arrowLineEndStyle = request.RequestedArrowLineEndStyle;
            }

            if (request.Tool == EditingToolKind.PrivacyRegion)
            {
                _privacyRegionMode = nextPrivacyMode;
            }

            _draft = null;
            _arrowLineDraft = null;
            _highlighterDraft = null;
            _textDraft = null;
            _privacyRegionDraft = null;
            _numberedMarkerDraft = null;
            return new EditingToolSelectionResult(
                EditingToolSelectionResultKind.Selected,
                _activeTool,
                request.SessionId,
                request.CoordinateVersion,
                selection.SelectionRevision,
                currentRevision,
                null,
                $"The {_activeTool} editing tool is active.")
            {
                ActiveArrowLineEndStyle = _arrowLineEndStyle,
                ActiveTextStyle = _textStyle,
                ActivePrivacyRegionMode = _privacyRegionMode,
                ActivePrivacyRegionEffectParameters = _privacyRegionEffectPolicy.GetParameters(_privacyRegionMode),
                ActiveNumberedMarkerStyle = GetActiveNumberedMarkerStyle(),
                ActiveNumberedMarkerNextNumber = _nextNumber
            };
        }
    }

    public PrivacyRegionModeSelectionResult SelectPrivacyRegionMode(
        PrivacyRegionModeSelectionRequest request,
        WorkflowState currentState,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            if (currentState != WorkflowState.Editing || _activeTool != EditingToolKind.PrivacyRegion)
            {
                return PrivacyModeResult(
                    PrivacyRegionModeSelectionResultKind.InvalidWorkflowState,
                    request,
                    CreateFailure(
                        request.SessionId,
                        FailureCode.InvalidStateTransition,
                        "Privacy Region mode can only change while the Privacy Region tool is active."),
                    "Privacy Region mode can only change while the Privacy Region tool is active.");
            }

            if (!IsCurrentSession(request.SessionId, request.CoordinateVersion))
            {
                return PrivacyModeResult(
                    PrivacyRegionModeSelectionResultKind.StaleSession,
                    request,
                    null,
                    "The Privacy Region mode request belongs to a stale capture session.");
            }

            if (request.SelectionRevision != selection.SelectionRevision)
            {
                return PrivacyModeResult(
                    PrivacyRegionModeSelectionResultKind.StaleSelectionRevision,
                    request,
                    null,
                    "The Privacy Region mode request belongs to a stale Selection revision.");
            }

            var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
            if (request.ExpectedAnnotationRevision != currentRevision)
            {
                return PrivacyModeResult(
                    PrivacyRegionModeSelectionResultKind.StaleAnnotationRevision,
                    request,
                    null,
                    "The Privacy Region mode request belongs to a stale Annotation revision.");
            }

            if (!TryGetPrivacyParameters(request.Mode, out _, out var failure))
            {
                return PrivacyModeResult(
                    PrivacyRegionModeSelectionResultKind.InvalidEffectParameters,
                    request,
                    failure,
                    "The Privacy Region effect parameters are invalid.");
            }

            _privacyRegionMode = request.Mode;
            _privacyRegionDraft = null;
            return PrivacyModeResult(
                PrivacyRegionModeSelectionResultKind.Selected,
                request,
                null,
                $"Privacy Region mode {request.Mode} is active.");
        }
    }

    public SetNextNumberResult SetNextNumber(
        SetNextNumberRequest request,
        WorkflowState currentState,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            if (currentState != WorkflowState.Editing)
            {
                return NextNumberResult(
                    SetNextNumberResultKind.InvalidWorkflowState,
                    request,
                    CreateFailure(
                        request.SessionId,
                        FailureCode.InvalidStateTransition,
                        "The next marker number can only change while the workflow is Editing."),
                    "The next marker number can only change while the workflow is Editing.");
            }

            if (!IsCurrentSession(request.SessionId, request.CoordinateVersion))
            {
                return NextNumberResult(
                    SetNextNumberResultKind.StaleSession,
                    request,
                    null,
                    "The next marker number request belongs to a stale capture session.");
            }

            if (request.SelectionRevision != selection.SelectionRevision)
            {
                return NextNumberResult(
                    SetNextNumberResultKind.StaleSelectionRevision,
                    request,
                    null,
                    "The next marker number request belongs to a stale Selection revision.");
            }

            var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
            if (request.ExpectedAnnotationRevision != currentRevision)
            {
                return NextNumberResult(
                    SetNextNumberResultKind.StaleAnnotationRevision,
                    request,
                    null,
                    "The next marker number request belongs to a stale Annotation revision.");
            }

            if (request.Number <= 0)
            {
                return NextNumberResult(
                    SetNextNumberResultKind.InvalidNumber,
                    request,
                    null,
                    "The next marker number must be positive.");
            }

            if (request.Number == _nextNumber)
            {
                return NextNumberResult(
                    SetNextNumberResultKind.NoChange,
                    request,
                    null,
                    "The next marker number is unchanged.");
            }

            _nextNumber = request.Number;
            return NextNumberResult(
                SetNextNumberResultKind.Succeeded,
                request,
                null,
                "The next marker number was updated.");
        }
    }

    public NumberedMarkerPointerResult PointerPressed(
        NumberedMarkerPointerEvent input,
        SelectionVisualState selection) => BeginNumberedMarkerDraft(input, selection);

    public NumberedMarkerPointerResult BeginNumberedMarkerDraft(
        NumberedMarkerPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateNumberedMarker(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_numberedMarkerDraft is not null)
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.DraftMismatch,
                    input,
                    _numberedMarkerDraft,
                    null,
                    _documents.Current,
                    null,
                    "A Numbered Marker draft is already active.");
            }

            if (!TryGetNumberedMarkerStyle(out var style, out var styleFailure))
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.InvalidStyle,
                    input,
                    null,
                    null,
                    _documents.Current,
                    styleFailure,
                    "The Numbered Marker style is invalid.");
            }

            if (!Contains(selection.NormalizedPhysicalBounds!.Value, input.GlobalPhysicalPoint))
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.IgnoredOutsideSelection,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Numbered Marker creation starts only inside the current Selection.");
            }

            _numberedMarkerDraft = new NumberedMarkerDraft(
                input.PointerId,
                _nextNumber,
                input.GlobalPhysicalPoint,
                input.GlobalPhysicalPoint,
                style);
            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.DraftStarted,
                input,
                _numberedMarkerDraft,
                null,
                _documents.Current,
                null,
                "Numbered Marker draft started.");
        }
    }

    public NumberedMarkerPointerResult PointerMoved(
        NumberedMarkerPointerEvent input,
        SelectionVisualState selection) => UpdateNumberedMarkerDraft(input, selection);

    public NumberedMarkerPointerResult UpdateNumberedMarkerDraft(
        NumberedMarkerPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateNumberedMarker(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_numberedMarkerDraft is null)
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Numbered Marker draft is active.");
            }

            if (_numberedMarkerDraft.PointerId != input.PointerId)
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.PointerMismatch,
                    input,
                    _numberedMarkerDraft,
                    null,
                    _documents.Current,
                    null,
                    "Numbered Marker pointer input belongs to another pointer.");
            }

            if (!Contains(selection.NormalizedPhysicalBounds!.Value, input.GlobalPhysicalPoint))
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.IgnoredOutsideSelection,
                    input,
                    _numberedMarkerDraft,
                    null,
                    _documents.Current,
                    null,
                    "Numbered Marker preview remains inside the current Selection.");
            }

            _numberedMarkerDraft = _numberedMarkerDraft with
            {
                Center = input.GlobalPhysicalPoint,
                Bounds = NumberedMarkerAnnotationContent.GetBounds(
                    input.GlobalPhysicalPoint,
                    _numberedMarkerDraft.Style)
            };
            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.DraftUpdated,
                input,
                _numberedMarkerDraft,
                null,
                _documents.Current,
                null,
                "Numbered Marker draft updated.");
        }
    }

    public NumberedMarkerPointerResult PointerReleased(
        NumberedMarkerPointerEvent input,
        SelectionVisualState selection) => CommitNumberedMarkerDraft(input, selection);

    public NumberedMarkerPointerResult CommitNumberedMarkerDraft(
        NumberedMarkerPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateNumberedMarker(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_numberedMarkerDraft is null)
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Numbered Marker draft is active.");
            }

            if (_numberedMarkerDraft.PointerId != input.PointerId)
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.PointerMismatch,
                    input,
                    _numberedMarkerDraft,
                    null,
                    _documents.Current,
                    null,
                    "Numbered Marker pointer input belongs to another pointer.");
            }

            if (!Contains(selection.NormalizedPhysicalBounds!.Value, input.GlobalPhysicalPoint))
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.IgnoredOutsideSelection,
                    input,
                    _numberedMarkerDraft,
                    null,
                    _documents.Current,
                    null,
                    "Numbered Marker commit must finish inside the current Selection.");
            }

            var draft = _numberedMarkerDraft with
            {
                Center = input.GlobalPhysicalPoint,
                Bounds = NumberedMarkerAnnotationContent.GetBounds(
                    input.GlobalPhysicalPoint,
                    _numberedMarkerDraft.Style)
            };
            var document = _documents.Current;
            if (document is null)
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.Failed,
                    input,
                    draft,
                    null,
                    null,
                    CreateFailure(
                        input.SessionId,
                        FailureCode.InvalidStateTransition,
                        "The Annotation Document is unavailable for Numbered Marker commit."),
                    "The Numbered Marker annotation could not be committed.");
            }

            if (draft.Number == _nextNumber && draft.Number == int.MaxValue)
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.NumberOverflow,
                    input,
                    draft,
                    null,
                    document,
                    CreateFailure(
                        input.SessionId,
                        FailureCode.InvalidStateTransition,
                        "The next Numbered Marker number would overflow."),
                    "The Numbered Marker could not be committed because the next number would overflow.");
            }

            var zOrder = document.Objects.Count == 0
                ? 0
                : document.Objects.Max(annotationObject => annotationObject.ZOrder);
            if (zOrder == int.MaxValue)
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.ZOrderOverflow,
                    input,
                    draft,
                    null,
                    document,
                    CreateFailure(
                        input.SessionId,
                        FailureCode.AnnotationZOrderOverflow,
                        "The next Numbered Marker annotation Z-order would overflow."),
                    "The Numbered Marker could not be committed because its Z-order would overflow.");
            }

            AnnotationObject annotationObject;
            try
            {
                annotationObject = new AnnotationObject(
                    _objectIdFactory(),
                    input.SessionId,
                    AnnotationToolKind.NumberedMarker,
                    draft.Bounds,
                    document.Objects.Count == 0 ? 0 : zOrder + 1,
                    new NumberedMarkerAnnotationContent(draft.Number, draft.Style));
            }
            catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException)
            {
                return NumberedMarkerResult(
                    NumberedMarkerPointerResultKind.Failed,
                    input,
                    draft,
                    null,
                    document,
                    CreateFailure(input.SessionId, FailureCode.InvalidStateTransition, exception.Message),
                    "The Numbered Marker annotation could not be created.");
            }

            var mutation = _documents.Add(new AddAnnotationObjectRequest(
                input.SessionId,
                input.ExpectedAnnotationRevision,
                annotationObject));
            if (mutation is not AnnotationMutationResult.Succeeded succeeded)
            {
                return NumberedMarkerResult(
                    mutation is AnnotationMutationResult.StaleAnnotationRevision
                        ? NumberedMarkerPointerResultKind.StaleAnnotationRevision
                        : NumberedMarkerPointerResultKind.Failed,
                    input,
                    draft,
                    null,
                    mutation.CurrentDocument,
                    CreateFailure(
                        input.SessionId,
                        mutation is AnnotationMutationResult.StaleAnnotationRevision
                            ? FailureCode.StaleAnnotationRevision
                            : FailureCode.InvalidStateTransition,
                        "The Annotation Document changed before the Numbered Marker commit."),
                    "The Numbered Marker annotation could not be committed.");
            }

            _numberedMarkerDraft = null;
            if (_nextNumber == draft.Number)
            {
                _nextNumber = checked(draft.Number + 1);
            }

            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.Committed,
                input,
                null,
                annotationObject,
                succeeded.Document,
                null,
                "Numbered Marker annotation committed.");
        }
    }

    public NumberedMarkerPointerResult CancelNumberedMarkerDraft(
        Guid sessionId,
        string coordinateVersion)
    {
        lock (_gate)
        {
            var document = _documents.Current;
            var input = new NumberedMarkerPointerEvent(
                sessionId,
                coordinateVersion,
                _selectionRevision,
                document?.Revision ?? AnnotationRevision.Initial,
                _numberedMarkerDraft?.PointerId ?? 0,
                _numberedMarkerDraft?.Center ?? default);
            var draft = _numberedMarkerDraft;
            _numberedMarkerDraft = null;
            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.Cancelled,
                input,
                draft,
                null,
                document,
                null,
                "Numbered Marker draft cancelled.");
        }
    }

    public TextDraftResult BeginTextDraft(
        TextDraftPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateTextPointer(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_activeTool != EditingToolKind.Text)
            {
                return FailedText(input.SessionId, input.CoordinateVersion, input.SelectionRevision,
                    "Text input was received while another editing tool is active.");
            }

            if (_textDraft is not null)
            {
                return TextResult(
                    TextDraftResultKind.DraftMismatch,
                    _textDraft.Request,
                    _textDraft.Text,
                    null,
                    _documents.Current,
                    null,
                    "A Text draft is already active.");
            }

            var selectionBounds = selection.NormalizedPhysicalBounds!.Value;
            if (!Contains(selectionBounds, input.GlobalPhysicalPoint))
            {
                return TextResult(
                    TextDraftResultKind.IgnoredOutsideSelection,
                    null,
                    string.Empty,
                    null,
                    _documents.Current,
                    null,
                    "Text creation starts only inside the current Selection.",
                    input.SessionId,
                    input.CoordinateVersion,
                    input.SelectionRevision);
            }

            var draftId = input.DraftId == Guid.Empty
                ? _textDraftIdFactory()
                : input.DraftId;
            if (draftId == Guid.Empty)
            {
                return FailedText(input.SessionId, input.CoordinateVersion, input.SelectionRevision,
                    "The Text draft identifier factory returned an empty identifier.");
            }

            var request = new TextDraftRequest(
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision,
                input.ExpectedAnnotationRevision,
                draftId,
                input.GlobalPhysicalPoint,
                CreateTextBounds(input.GlobalPhysicalPoint, selectionBounds));
            if (!request.BoundsInVirtualDesktop.IsPositive)
            {
                return TextResult(
                    TextDraftResultKind.InvalidGeometry,
                    request,
                    string.Empty,
                    null,
                    _documents.Current,
                    null,
                    "Text creation could not create a positive editor boundary.");
            }

            _textDraft = new TextDraft(
                request,
                string.Empty,
                _textStyle);
            return TextResult(
                TextDraftResultKind.DraftStarted,
                request,
                string.Empty,
                null,
                _documents.Current,
                null,
                "Text draft started.");
        }
    }

    public TextDraftResult UpdateTextDraftContent(
        TextDraftRequest request,
        string text,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateTextRequest(request, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            _textDraft = _textDraft! with
            {
                Text = TextAnnotationContent.Normalize(text ?? string.Empty)
            };
            return TextResult(
                TextDraftResultKind.DraftUpdated,
                request,
                _textDraft.Text,
                null,
                _documents.Current,
                null,
                "Text draft content updated.");
        }
    }

    public TextDraftResult UpdateTextDraftStyle(
        TextDraftRequest request,
        TextAnnotationStyle? style,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateTextRequest(request, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (style is null)
            {
                return TextResult(
                    TextDraftResultKind.InvalidStyle,
                    request,
                    _textDraft!.Text,
                    null,
                    _documents.Current,
                    null,
                    "Text style is required.");
            }

            _textDraft = _textDraft! with { Style = style };
            return TextResult(
                TextDraftResultKind.DraftUpdated,
                request,
                _textDraft.Text,
                null,
                _documents.Current,
                null,
                "Text draft style updated.");
        }
    }

    public TextDraftResult CommitTextDraft(
        TextDraftRequest request,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateTextRequest(request, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (string.IsNullOrWhiteSpace(_textDraft!.Text))
            {
                return TextResult(
                    TextDraftResultKind.EmptyText,
                    request,
                    _textDraft.Text,
                    null,
                    _documents.Current,
                    null,
                    "Text draft is empty and remains editable.");
            }

            var document = _documents.Current;
            if (document is null)
            {
                return TextResult(
                    TextDraftResultKind.Failed,
                    request,
                    _textDraft.Text,
                    null,
                    null,
                    CreateFailure(request.SessionId, FailureCode.InvalidStateTransition,
                        "The Annotation Document is unavailable for Text commit."),
                    "The Text annotation could not be committed.");
            }

            var nextZOrder = document.Objects.Count == 0
                ? 0
                : document.Objects.Max(annotationObject => annotationObject.ZOrder);
            if (nextZOrder == int.MaxValue)
            {
                return TextResult(
                    TextDraftResultKind.RevisionOverflow,
                    request,
                    _textDraft.Text,
                    null,
                    document,
                    CreateFailure(request.SessionId, FailureCode.AnnotationZOrderOverflow,
                        "The next Text annotation Z-order would overflow."),
                    "The Text annotation could not be committed because its Z-order would overflow.");
            }

            AnnotationObject annotationObject;
            try
            {
                annotationObject = new AnnotationObject(
                    _objectIdFactory(),
                    request.SessionId,
                    AnnotationToolKind.Text,
                    request.BoundsInVirtualDesktop,
                    document.Objects.Count == 0 ? 0 : nextZOrder + 1,
                    new TextAnnotationContent(
                        _textDraft.Text,
                        request.AnchorInVirtualDesktop,
                        request.BoundsInVirtualDesktop,
                        _textDraft.Style));
            }
            catch (ArgumentException exception)
            {
                return TextResult(
                    TextDraftResultKind.Failed,
                    request,
                    _textDraft.Text,
                    null,
                    document,
                    CreateFailure(request.SessionId, FailureCode.InvalidStateTransition, exception.Message),
                    "The Text annotation could not be created.");
            }

            var mutation = _documents.Add(new AddAnnotationObjectRequest(
                request.SessionId,
                request.ExpectedAnnotationRevision,
                annotationObject));
            if (mutation is AnnotationMutationResult.Succeeded succeeded)
            {
                _textDraft = null;
                return TextResult(
                    TextDraftResultKind.Committed,
                    request,
                    string.Empty,
                    annotationObject,
                    succeeded.Document,
                    null,
                    "Text annotation committed.");
            }

            var kind = mutation is AnnotationMutationResult.RevisionOverflow
                ? TextDraftResultKind.RevisionOverflow
                : mutation is AnnotationMutationResult.StaleAnnotationRevision
                    ? TextDraftResultKind.StaleAnnotationRevision
                    : TextDraftResultKind.Failed;
            return TextResult(
                kind,
                request,
                _textDraft.Text,
                null,
                mutation.CurrentDocument,
                CreateFailure(request.SessionId,
                    kind == TextDraftResultKind.StaleAnnotationRevision
                        ? FailureCode.StaleAnnotationRevision
                        : FailureCode.AnnotationZOrderOverflow,
                    "The Text annotation could not be committed because the document changed."),
                "The Text annotation could not be committed.");
        }
    }

    public TextDraftResult CancelTextDraft(
        TextDraftRequest request,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateTextRequest(request, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            var text = _textDraft!.Text;
            _textDraft = null;
            return TextResult(
                TextDraftResultKind.Cancelled,
                request,
                text,
                null,
                _documents.Current,
                null,
                "Text draft cancelled.");
        }
    }

    public RectanglePointerResult PointerPressed(
        RectanglePointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = Validate(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_activeTool != EditingToolKind.Rectangle)
            {
                return Failed(
                    input,
                    "Rectangle input was received while the Selection tool is active.");
            }

            if (_draft is not null)
            {
                return Result(
                    RectanglePointerResultKind.PointerMismatch,
                    input,
                    _draft.Bounds,
                    null,
                    _documents.Current,
                    null,
                    "A Rectangle draft is already active.");
            }

            if (selection.Status != SelectionStatus.Locked
                || selection.InteractionMode != SelectionInteractionMode.Locked
                || selection.NormalizedPhysicalBounds is not PhysicalRect bounds
                || !bounds.IsPositive)
            {
                return Result(
                    RectanglePointerResultKind.InvalidGeometry,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Rectangle creation requires a valid locked Selection.");
            }

            if (!Contains(bounds, input.GlobalPhysicalPoint))
            {
                return Result(
                    RectanglePointerResultKind.IgnoredOutsideSelection,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Rectangle creation starts only inside the current Selection.");
            }

            _draft = new RectangleDraft(input.PointerId, input.GlobalPhysicalPoint);
            return Result(
                RectanglePointerResultKind.DraftStarted,
                input,
                new PhysicalRect(
                    input.GlobalPhysicalPoint.X,
                    input.GlobalPhysicalPoint.Y,
                    input.GlobalPhysicalPoint.X,
                    input.GlobalPhysicalPoint.Y),
                null,
                _documents.Current,
                null,
                "Rectangle draft started.");
        }
    }

    public RectanglePointerResult PointerMoved(
        RectanglePointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = Validate(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_draft is null)
            {
                return Result(
                    RectanglePointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Rectangle draft is active.");
            }

            if (_draft.PointerId != input.PointerId)
            {
                return Result(
                    RectanglePointerResultKind.PointerMismatch,
                    input,
                    _draft.Bounds,
                    null,
                    _documents.Current,
                    null,
                    "Rectangle pointer input belongs to another pointer.");
            }

            var bounds = Normalize(_draft.Start, input.GlobalPhysicalPoint);
            _draft = _draft with { Current = input.GlobalPhysicalPoint, Bounds = bounds };
            return Result(
                bounds.IsPositive
                    ? RectanglePointerResultKind.DraftUpdated
                    : RectanglePointerResultKind.InvalidGeometry,
                input,
                bounds,
                null,
                _documents.Current,
                null,
                bounds.IsPositive
                    ? "Rectangle draft updated."
                    : "Rectangle draft geometry is not positive yet.");
        }
    }

    public RectanglePointerResult PointerReleased(
        RectanglePointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = Validate(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_draft is null)
            {
                return Result(
                    RectanglePointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Rectangle draft is active.");
            }

            if (_draft.PointerId != input.PointerId)
            {
                return Result(
                    RectanglePointerResultKind.PointerMismatch,
                    input,
                    _draft.Bounds,
                    null,
                    _documents.Current,
                    null,
                    "Rectangle pointer input belongs to another pointer.");
            }

            var bounds = Normalize(_draft.Start, input.GlobalPhysicalPoint);
            _draft = null;
            if (!bounds.IsPositive)
            {
                return Result(
                    RectanglePointerResultKind.InvalidGeometry,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Rectangle geometry must have positive width and height.");
            }

            var document = _documents.Current;
            if (document is null)
            {
                return Failed(input, "The Annotation Document is unavailable for Rectangle commit.");
            }

            var zOrder = document.Objects.Count == 0
                ? 0
                : document.Objects.Max(annotationObject => annotationObject.ZOrder);
            if (zOrder == int.MaxValue)
            {
                return Result(
                    RectanglePointerResultKind.Failed,
                    input,
                    null,
                    null,
                    document,
                    Failure.Create(
                        FailureCode.AnnotationZOrderOverflow,
                        FailureCategory.Validation,
                        FailureRecoverability.RetrySameIntent,
                        nameof(AnnotationEditingCoordinator),
                        input.SessionId,
                        "The next Rectangle annotation Z-order would overflow."),
                    "The next Rectangle annotation Z-order would overflow.");
            }

            AnnotationObject annotationObject;
            try
            {
                annotationObject = new AnnotationObject(
                    _objectIdFactory(),
                    input.SessionId,
                    AnnotationToolKind.Rectangle,
                    bounds,
                    document.Objects.Count == 0 ? 0 : zOrder + 1,
                    new RectangleAnnotationContent(_rectangleStyle));
            }
            catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException)
            {
                return Result(
                    RectanglePointerResultKind.Failed,
                    input,
                    null,
                    null,
                    document,
                    Failure.Create(
                        FailureCode.AnnotationZOrderOverflow,
                        FailureCategory.Validation,
                        FailureRecoverability.RetrySameIntent,
                        nameof(AnnotationEditingCoordinator),
                        input.SessionId,
                        exception.Message),
                    "The Rectangle annotation could not be created.");
            }

            var mutation = _documents.Add(new AddAnnotationObjectRequest(
                input.SessionId,
                input.ExpectedAnnotationRevision,
                annotationObject));
            if (mutation is AnnotationMutationResult.Succeeded succeeded)
            {
                return Result(
                    RectanglePointerResultKind.Committed,
                    input,
                    null,
                    annotationObject,
                    succeeded.Document,
                    null,
                    "Rectangle annotation committed.");
            }

            return Result(
                RectanglePointerResultKind.Failed,
                input,
                null,
                null,
                mutation.CurrentDocument,
                Failure.Create(
                    FailureCode.StaleAnnotationRevision,
                    FailureCategory.Session,
                    FailureRecoverability.RetrySameIntent,
                    nameof(AnnotationEditingCoordinator),
                    input.SessionId,
                    "The Annotation Document changed before the Rectangle commit."),
                "The Rectangle annotation could not be committed because the Annotation Document is stale.");
        }
    }

    public RectanglePointerResult CancelDraft(Guid sessionId, string coordinateVersion)
    {
        lock (_gate)
        {
            var document = _documents.Current;
            var input = new RectanglePointerEvent(
                sessionId,
                coordinateVersion,
                _selectionRevision,
                document?.Revision ?? AnnotationRevision.Initial,
                _draft?.PointerId ?? 0,
                _draft?.Current ?? default);
            _draft = null;
            return Result(
                RectanglePointerResultKind.Cancelled,
                input,
                null,
                null,
                document,
                null,
                "Rectangle draft cancelled.");
        }
    }

    public PrivacyRegionPointerResult PointerPressed(
        PrivacyRegionPointerEvent input,
        SelectionVisualState selection) => BeginPrivacyRegionDraft(input, selection);

    public PrivacyRegionPointerResult BeginPrivacyRegionDraft(
        PrivacyRegionPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidatePrivacyRegion(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_activeTool != EditingToolKind.PrivacyRegion)
            {
                return FailedPrivacyRegion(
                    input,
                    "Privacy Region input was received while another editing tool is active.");
            }

            if (_privacyRegionDraft is not null)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.DraftMismatch,
                    input,
                    _privacyRegionDraft.Bounds,
                    null,
                    _privacyRegionDraft.DraftId,
                    _documents.Current,
                    null,
                    "A Privacy Region draft is already active.");
            }

            if (selection.NormalizedPhysicalBounds is not PhysicalRect bounds
                || !bounds.IsPositive)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.InvalidGeometry,
                    input,
                    null,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Privacy Region creation requires a valid locked Selection.");
            }

            if (!Contains(bounds, input.GlobalPhysicalPoint))
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.IgnoredOutsideSelection,
                    input,
                    null,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Privacy Region creation starts only inside the current Selection.");
            }

            if (!TryGetPrivacyParameters(_privacyRegionMode, out var parameters, out var failure))
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.InvalidEffectParameters,
                    input,
                    null,
                    null,
                    null,
                    _documents.Current,
                    failure,
                    "The Privacy Region effect parameters are invalid.");
            }

            var draftId = _privacyDraftIdFactory();
            if (draftId == Guid.Empty)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.Failed,
                    input,
                    null,
                    null,
                    null,
                    _documents.Current,
                    CreateFailure(
                        input.SessionId,
                        FailureCode.InvalidStateTransition,
                        "The Privacy Region draft identifier factory returned an empty identifier."),
                    "The Privacy Region draft could not be started.");
            }

            _privacyRegionDraft = new PrivacyRegionDraft(
                draftId,
                input.PointerId,
                input.GlobalPhysicalPoint,
                input.GlobalPhysicalPoint,
                _privacyRegionMode,
                parameters);
            return PrivacyRegionResult(
                PrivacyRegionPointerResultKind.DraftStarted,
                input,
                _privacyRegionDraft.Bounds,
                null,
                draftId,
                _documents.Current,
                null,
                "Privacy Region draft started.");
        }
    }

    public PrivacyRegionPointerResult PointerMoved(
        PrivacyRegionPointerEvent input,
        SelectionVisualState selection) => UpdatePrivacyRegionDraft(input, selection);

    public PrivacyRegionPointerResult UpdatePrivacyRegionDraft(
        PrivacyRegionPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidatePrivacyRegion(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_privacyRegionDraft is null)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Privacy Region draft is active.");
            }

            if (_privacyRegionDraft.PointerId != input.PointerId)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.PointerMismatch,
                    input,
                    _privacyRegionDraft.Bounds,
                    null,
                    _privacyRegionDraft.DraftId,
                    _documents.Current,
                    null,
                    "Privacy Region pointer input belongs to another pointer.");
            }

            var bounds = Normalize(
                _privacyRegionDraft.Start,
                input.GlobalPhysicalPoint);
            _privacyRegionDraft = _privacyRegionDraft with
            {
                Current = input.GlobalPhysicalPoint,
                Bounds = bounds
            };
            return PrivacyRegionResult(
                bounds.IsPositive
                    ? PrivacyRegionPointerResultKind.DraftUpdated
                    : PrivacyRegionPointerResultKind.InvalidGeometry,
                input,
                bounds,
                null,
                _privacyRegionDraft.DraftId,
                _documents.Current,
                null,
                bounds.IsPositive
                    ? "Privacy Region draft updated."
                    : "Privacy Region draft geometry is not positive yet.");
        }
    }

    public PrivacyRegionPointerResult PointerReleased(
        PrivacyRegionPointerEvent input,
        SelectionVisualState selection) => CommitPrivacyRegionDraft(input, selection);

    public PrivacyRegionPointerResult CommitPrivacyRegionDraft(
        PrivacyRegionPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidatePrivacyRegion(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_privacyRegionDraft is null)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Privacy Region draft is active.");
            }

            if (_privacyRegionDraft.PointerId != input.PointerId)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.PointerMismatch,
                    input,
                    _privacyRegionDraft.Bounds,
                    null,
                    _privacyRegionDraft.DraftId,
                    _documents.Current,
                    null,
                    "Privacy Region pointer input belongs to another pointer.");
            }

            var bounds = Normalize(
                _privacyRegionDraft.Start,
                input.GlobalPhysicalPoint);
            var draftId = _privacyRegionDraft.DraftId;
            var mode = _privacyRegionDraft.Mode;
            var parameters = _privacyRegionDraft.EffectParameters;
            _privacyRegionDraft = null;
            if (!bounds.IsPositive)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.InvalidGeometry,
                    input,
                    null,
                    null,
                    draftId,
                    _documents.Current,
                    null,
                    "Privacy Region geometry must have positive width and height.");
            }

            var document = _documents.Current;
            if (document is null)
            {
                return FailedPrivacyRegion(
                    input,
                    "The Annotation Document is unavailable for Privacy Region commit.");
            }

            var zOrder = document.Objects.Count == 0
                ? 0
                : document.Objects.Max(annotationObject => annotationObject.ZOrder);
            if (zOrder == int.MaxValue)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.RevisionOverflow,
                    input,
                    null,
                    null,
                    draftId,
                    document,
                    Failure.Create(
                        FailureCode.AnnotationZOrderOverflow,
                        FailureCategory.Validation,
                        FailureRecoverability.RetrySameIntent,
                        nameof(AnnotationEditingCoordinator),
                        input.SessionId,
                        "The next Privacy Region annotation Z-order would overflow."),
                    "The Privacy Region annotation could not be committed because its Z-order would overflow.");
            }

            AnnotationObject annotationObject;
            try
            {
                annotationObject = new AnnotationObject(
                    _objectIdFactory(),
                    input.SessionId,
                    AnnotationToolKind.PrivacyRegion,
                    bounds,
                    document.Objects.Count == 0 ? 0 : zOrder + 1,
                    new PrivacyRegionAnnotationContent(mode, parameters));
            }
            catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.Failed,
                    input,
                    null,
                    null,
                    draftId,
                    document,
                    Failure.Create(
                        FailureCode.InvalidStateTransition,
                        FailureCategory.Validation,
                        FailureRecoverability.RetrySameIntent,
                        nameof(AnnotationEditingCoordinator),
                        input.SessionId,
                        exception.Message),
                    "The Privacy Region annotation could not be created.");
            }

            var mutation = _documents.Add(new AddAnnotationObjectRequest(
                input.SessionId,
                input.ExpectedAnnotationRevision,
                annotationObject));
            if (mutation is AnnotationMutationResult.Succeeded succeeded)
            {
                return PrivacyRegionResult(
                    PrivacyRegionPointerResultKind.Committed,
                    input,
                    null,
                    annotationObject,
                    draftId,
                    succeeded.Document,
                    null,
                    "Privacy Region annotation committed.");
            }

            return PrivacyRegionResult(
                PrivacyRegionPointerResultKind.Failed,
                input,
                null,
                null,
                draftId,
                mutation.CurrentDocument,
                Failure.Create(
                    FailureCode.StaleAnnotationRevision,
                    FailureCategory.Session,
                    FailureRecoverability.RetrySameIntent,
                    nameof(AnnotationEditingCoordinator),
                    input.SessionId,
                    "The Annotation Document changed before the Privacy Region commit."),
                "The Privacy Region annotation could not be committed because the Annotation Document is stale.");
        }
    }

    public PrivacyRegionPointerResult CancelPrivacyRegionDraft(
        Guid sessionId,
        string coordinateVersion)
    {
        lock (_gate)
        {
            var document = _documents.Current;
            var input = new PrivacyRegionPointerEvent(
                sessionId,
                coordinateVersion,
                _selectionRevision,
                document?.Revision ?? AnnotationRevision.Initial,
                _privacyRegionDraft?.PointerId ?? 0,
                _privacyRegionDraft?.Current ?? default);
            var draftId = _privacyRegionDraft?.DraftId;
            _privacyRegionDraft = null;
            return PrivacyRegionResult(
                PrivacyRegionPointerResultKind.Cancelled,
                input,
                null,
                null,
                draftId,
                document,
                null,
                "Privacy Region draft cancelled.");
        }
    }

    public ArrowLinePointerResult PointerPressed(
        ArrowLinePointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateArrowLine(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_activeTool != EditingToolKind.ArrowLine)
            {
                return FailedArrowLine(
                    input,
                    "Arrow or line input was received while another editing tool is active.");
            }

            if (_arrowLineDraft is not null)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.PointerMismatch,
                    input,
                    _arrowLineDraft.Segment,
                    null,
                    _documents.Current,
                    null,
                    "Another Arrow or line draft is already active.");
            }

            if (selection.Status != SelectionStatus.Locked
                || selection.InteractionMode != SelectionInteractionMode.Locked
                || selection.NormalizedPhysicalBounds is not PhysicalRect bounds
                || !bounds.IsPositive)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.InvalidGeometry,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Arrow or line creation requires a valid locked Selection.");
            }

            if (!Contains(bounds, input.GlobalPhysicalPoint))
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.IgnoredOutsideSelection,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Arrow or line creation starts only inside the current Selection.");
            }

            var segment = new PhysicalLineSegment(
                input.GlobalPhysicalPoint,
                input.GlobalPhysicalPoint);
            _arrowLineDraft = new ArrowLineDraft(input.PointerId, segment);
            return ArrowLineResult(
                ArrowLinePointerResultKind.DraftStarted,
                input,
                segment,
                null,
                _documents.Current,
                null,
                "Arrow or line draft started.");
        }
    }

    public ArrowLinePointerResult PointerMoved(
        ArrowLinePointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateArrowLine(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_arrowLineDraft is null)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Arrow or line draft is active.");
            }

            if (_arrowLineDraft.PointerId != input.PointerId)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.PointerMismatch,
                    input,
                    _arrowLineDraft.Segment,
                    null,
                    _documents.Current,
                    null,
                    "Arrow or line pointer input belongs to another pointer.");
            }

            var segment = new PhysicalLineSegment(
                _arrowLineDraft.Segment.Start,
                input.GlobalPhysicalPoint);
            _arrowLineDraft = _arrowLineDraft with { Segment = segment };
            return ArrowLineResult(
                segment.IsPositive
                    ? ArrowLinePointerResultKind.DraftUpdated
                    : ArrowLinePointerResultKind.InvalidGeometry,
                input,
                segment,
                null,
                _documents.Current,
                null,
                segment.IsPositive
                    ? "Arrow or line draft updated."
                    : "Arrow or line draft geometry is not positive yet.");
        }
    }

    public ArrowLinePointerResult PointerReleased(
        ArrowLinePointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateArrowLine(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_arrowLineDraft is null)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Arrow or line draft is active.");
            }

            if (_arrowLineDraft.PointerId != input.PointerId)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.PointerMismatch,
                    input,
                    _arrowLineDraft.Segment,
                    null,
                    _documents.Current,
                    null,
                    "Arrow or line pointer input belongs to another pointer.");
            }

            var segment = new PhysicalLineSegment(
                _arrowLineDraft.Segment.Start,
                input.GlobalPhysicalPoint);
            _arrowLineDraft = null;
            if (!segment.IsPositive)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.InvalidGeometry,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Arrow or line geometry must have distinct endpoints.");
            }

            var document = _documents.Current;
            if (document is null)
            {
                return FailedArrowLine(input, "The Annotation Document is unavailable for Arrow or line commit.");
            }

            var zOrder = document.Objects.Count == 0
                ? 0
                : document.Objects.Max(annotationObject => annotationObject.ZOrder);
            if (zOrder == int.MaxValue)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.Failed,
                    input,
                    null,
                    null,
                    document,
                    Failure.Create(
                        FailureCode.AnnotationZOrderOverflow,
                        FailureCategory.Validation,
                        FailureRecoverability.RetrySameIntent,
                        nameof(AnnotationEditingCoordinator),
                        input.SessionId,
                        "The next Arrow or line annotation Z-order would overflow."),
                    "The next Arrow or line annotation Z-order would overflow.");
            }

            AnnotationObject annotationObject;
            try
            {
                var defaultStyle = _arrowLineStyle;
                annotationObject = new AnnotationObject(
                    _objectIdFactory(),
                    input.SessionId,
                    AnnotationToolKind.ArrowLine,
                    segment.Bounds,
                    document.Objects.Count == 0 ? 0 : zOrder + 1,
                    new ArrowLineAnnotationContent(segment, defaultStyle with
                    {
                        EndStyle = _arrowLineEndStyle
                    }));
            }
            catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.Failed,
                    input,
                    null,
                    null,
                    document,
                    Failure.Create(
                        FailureCode.AnnotationZOrderOverflow,
                        FailureCategory.Validation,
                        FailureRecoverability.RetrySameIntent,
                        nameof(AnnotationEditingCoordinator),
                        input.SessionId,
                        exception.Message),
                    "The Arrow or line annotation could not be created.");
            }

            var mutation = _documents.Add(new AddAnnotationObjectRequest(
                input.SessionId,
                input.ExpectedAnnotationRevision,
                annotationObject));
            if (mutation is AnnotationMutationResult.Succeeded succeeded)
            {
                return ArrowLineResult(
                    ArrowLinePointerResultKind.Committed,
                    input,
                    null,
                    annotationObject,
                    succeeded.Document,
                    null,
                    "Arrow or line annotation committed.");
            }

            return ArrowLineResult(
                ArrowLinePointerResultKind.Failed,
                input,
                null,
                null,
                mutation.CurrentDocument,
                Failure.Create(
                    FailureCode.StaleAnnotationRevision,
                    FailureCategory.Session,
                    FailureRecoverability.RetrySameIntent,
                    nameof(AnnotationEditingCoordinator),
                    input.SessionId,
                    "The Annotation Document changed before the Arrow or line commit."),
                "The Arrow or line annotation could not be committed because the Annotation Document is stale.");
        }
    }

    public ArrowLinePointerResult CancelArrowLineDraft(Guid sessionId, string coordinateVersion)
    {
        lock (_gate)
        {
            var document = _documents.Current;
            var input = new ArrowLinePointerEvent(
                sessionId,
                coordinateVersion,
                _selectionRevision,
                document?.Revision ?? AnnotationRevision.Initial,
                _arrowLineDraft?.PointerId ?? 0,
                _arrowLineDraft?.Segment.End ?? default);
            _arrowLineDraft = null;
            return ArrowLineResult(
                ArrowLinePointerResultKind.Cancelled,
                input,
                null,
                null,
                document,
                null,
                "Arrow or line draft cancelled.");
        }
    }

    public HighlighterPointerResult PointerPressed(
        HighlighterPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateHighlighter(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_activeTool != EditingToolKind.Highlighter)
            {
                return FailedHighlighter(
                    input,
                    "Highlighter input was received while another editing tool is active.");
            }

            if (_highlighterDraft is not null)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.PointerMismatch,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Another Highlighter draft is already active.");
            }

            if (selection.NormalizedPhysicalBounds is not PhysicalRect bounds
                || !bounds.IsPositive)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.InvalidGeometry,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Highlighter creation requires a valid locked Selection.");
            }

            if (!Contains(bounds, input.GlobalPhysicalPoint))
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.IgnoredOutsideSelection,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Highlighter creation starts only inside the current Selection.");
            }

            _highlighterDraft = new HighlighterDraft(
                input.PointerId,
                Array.AsReadOnly([input.GlobalPhysicalPoint]));
            return HighlighterResult(
                HighlighterPointerResultKind.DraftStarted,
                input,
                _highlighterDraft.Points,
                null,
                _documents.Current,
                null,
                "Highlighter draft started.");
        }
    }

    public HighlighterPointerResult PointerMoved(
        HighlighterPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateHighlighter(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_highlighterDraft is null)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Highlighter draft is active.");
            }

            if (_highlighterDraft.PointerId != input.PointerId)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.PointerMismatch,
                    input,
                    _highlighterDraft.Points,
                    null,
                    _documents.Current,
                    null,
                    "Highlighter pointer input belongs to another pointer.");
            }

            var points = _highlighterDraft.Points.ToList();
            if (points[^1] != input.GlobalPhysicalPoint)
            {
                points.Add(input.GlobalPhysicalPoint);
            }

            _highlighterDraft = _highlighterDraft with
            {
                Points = Array.AsReadOnly(points.ToArray())
            };
            return HighlighterResult(
                HighlighterPointerResultKind.DraftUpdated,
                input,
                _highlighterDraft.Points,
                null,
                _documents.Current,
                null,
                "Highlighter draft updated.");
        }
    }

    public HighlighterPointerResult PointerReleased(
        HighlighterPointerEvent input,
        SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var rejection = ValidateHighlighter(input, selection);
            if (rejection is not null)
            {
                return rejection;
            }

            if (_highlighterDraft is null)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.NoActiveDraft,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "No Highlighter draft is active.");
            }

            if (_highlighterDraft.PointerId != input.PointerId)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.PointerMismatch,
                    input,
                    _highlighterDraft.Points,
                    null,
                    _documents.Current,
                    null,
                    "Highlighter pointer input belongs to another pointer.");
            }

            var points = _highlighterDraft.Points.ToList();
            if (points[^1] != input.GlobalPhysicalPoint)
            {
                points.Add(input.GlobalPhysicalPoint);
            }

            var path = new PhysicalPolyline(points);
            _highlighterDraft = null;
            if (!path.HasLength)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.InvalidGeometry,
                    input,
                    null,
                    null,
                    _documents.Current,
                    null,
                    "Highlighter geometry must contain distinct points.");
            }

            var document = _documents.Current;
            if (document is null)
            {
                return FailedHighlighter(
                    input,
                    "The Annotation Document is unavailable for Highlighter commit.");
            }

            var zOrder = document.Objects.Count == 0
                ? 0
                : document.Objects.Max(annotationObject => annotationObject.ZOrder);
            if (zOrder == int.MaxValue)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.Failed,
                    input,
                    null,
                    null,
                    document,
                    Failure.Create(
                        FailureCode.AnnotationZOrderOverflow,
                        FailureCategory.Validation,
                        FailureRecoverability.RetrySameIntent,
                        nameof(AnnotationEditingCoordinator),
                        input.SessionId,
                        "The next Highlighter annotation Z-order would overflow."),
                    "The next Highlighter annotation Z-order would overflow.");
            }

            AnnotationObject annotationObject;
            try
            {
                annotationObject = new AnnotationObject(
                    _objectIdFactory(),
                    input.SessionId,
                    AnnotationToolKind.HighlighterStroke,
                    path.Bounds,
                    document.Objects.Count == 0 ? 0 : zOrder + 1,
                    new HighlighterStrokeContent(
                        path,
                        _highlighterStyle));
            }
            catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.Failed,
                    input,
                    null,
                    null,
                    document,
                    Failure.Create(
                        FailureCode.AnnotationZOrderOverflow,
                        FailureCategory.Validation,
                        FailureRecoverability.RetrySameIntent,
                        nameof(AnnotationEditingCoordinator),
                        input.SessionId,
                        exception.Message),
                    "The Highlighter annotation could not be created.");
            }

            var mutation = _documents.Add(new AddAnnotationObjectRequest(
                input.SessionId,
                input.ExpectedAnnotationRevision,
                annotationObject));
            if (mutation is AnnotationMutationResult.Succeeded succeeded)
            {
                return HighlighterResult(
                    HighlighterPointerResultKind.Committed,
                    input,
                    null,
                    annotationObject,
                    succeeded.Document,
                    null,
                    "Highlighter annotation committed.");
            }

            return HighlighterResult(
                HighlighterPointerResultKind.Failed,
                input,
                null,
                null,
                mutation.CurrentDocument,
                Failure.Create(
                    FailureCode.StaleAnnotationRevision,
                    FailureCategory.Session,
                    FailureRecoverability.RetrySameIntent,
                    nameof(AnnotationEditingCoordinator),
                    input.SessionId,
                    "The Annotation Document changed before the Highlighter commit."),
                "The Highlighter annotation could not be committed because the Annotation Document is stale.");
        }
    }

    public HighlighterPointerResult CancelHighlighterDraft(Guid sessionId, string coordinateVersion)
    {
        lock (_gate)
        {
            var document = _documents.Current;
            var input = new HighlighterPointerEvent(
                sessionId,
                coordinateVersion,
                _selectionRevision,
                document?.Revision ?? AnnotationRevision.Initial,
                _highlighterDraft?.PointerId ?? 0,
                _highlighterDraft?.Points is { Count: > 0 } points
                    ? points[^1]
                    : default);
            _highlighterDraft = null;
            return HighlighterResult(
                HighlighterPointerResultKind.Cancelled,
                input,
                null,
                null,
                document,
                null,
                "Highlighter draft cancelled.");
        }
    }

    public AnnotationPresentationSnapshot CreatePresentationSnapshot(SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            var document = _documents.Current
                ?? AnnotationDocument.CreateEmpty(selection.SessionId);
            return new AnnotationPresentationSnapshot(
                selection.SessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                document.Revision,
                selection.IsGeometryValid
                    ? selection.NormalizedPhysicalBounds
                    : null,
                _activeTool,
                _draft?.Bounds,
                document)
            {
                ActiveArrowLineEndStyle = _arrowLineEndStyle,
                DraftArrowLineSegment = _arrowLineDraft?.Segment,
                ActiveHighlighterStyle = _highlighterStyle,
                DraftHighlighterPoints = _highlighterDraft?.Points,
                ActiveTextStyle = _textStyle,
                ActivePrivacyRegionMode = _privacyRegionMode,
                ActivePrivacyRegionEffectParameters = _privacyRegionEffectPolicy.GetParameters(_privacyRegionMode),
                DraftPrivacyRegionMode = _privacyRegionDraft?.Mode,
                DraftPrivacyRegionEffectParameters = _privacyRegionDraft?.EffectParameters,
                DraftPrivacyRegionBounds = _privacyRegionDraft?.Bounds,
                ActiveNumberedMarkerStyle = GetActiveNumberedMarkerStyle(),
                ActiveNumberedMarkerNextNumber = _nextNumber,
                DraftNumberedMarker = _numberedMarkerDraft is null
                    ? null
                    : new NumberedMarkerDraftPresentation(
                        _numberedMarkerDraft.Number,
                        _numberedMarkerDraft.Center,
                        _numberedMarkerDraft.Bounds,
                        _numberedMarkerDraft.Style),
                DraftText = _textDraft is null
                    ? null
                    : new TextDraftPresentation(
                        _textDraft.Request.DraftId,
                        _textDraft.Text,
                        _textDraft.Request.AnchorInVirtualDesktop,
                        _textDraft.Request.BoundsInVirtualDesktop,
                        _textDraft.Style)
            };
        }
    }

    public void ClearSession(Guid sessionId)
    {
        lock (_gate)
        {
            _documents.ClearSession(sessionId);
            if (_sessionId == sessionId)
            {
                _draft = null;
                _sessionId = null;
                _coordinateVersion = string.Empty;
                _selectionRevision = 0;
                _activeTool = EditingToolKind.Selection;
                _arrowLineEndStyle = ArrowLineEndStyle.Arrow;
                _privacyRegionMode = _privacyRegionEffectPolicy.GetDefaultMode();
                _arrowLineDraft = null;
                _highlighterDraft = null;
                _textDraft = null;
                _privacyRegionDraft = null;
                _numberedMarkerDraft = null;
                _nextNumber = 1;
            }
        }
    }

    private TextDraftResult? ValidateTextPointer(
        TextDraftPointerEvent input,
        SelectionVisualState selection)
    {
        if (input.PointerId <= 0)
        {
            return FailedText(input.SessionId, input.CoordinateVersion, input.SelectionRevision,
                "Text input must contain a positive pointer identifier.");
        }

        if (!IsCurrentSession(input.SessionId, input.CoordinateVersion))
        {
            return TextResult(
                TextDraftResultKind.StaleSession,
                null,
                string.Empty,
                null,
                _documents.Current,
                null,
                "Text input belongs to a stale capture session.",
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision);
        }

        if (input.SelectionRevision != selection.SelectionRevision)
        {
            return TextResult(
                TextDraftResultKind.StaleSelectionRevision,
                null,
                string.Empty,
                null,
                _documents.Current,
                null,
                "Text input belongs to a stale Selection revision.",
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision);
        }

        var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
        if (input.ExpectedAnnotationRevision != currentRevision)
        {
            return TextResult(
                TextDraftResultKind.StaleAnnotationRevision,
                null,
                string.Empty,
                null,
                _documents.Current,
                null,
                "Text input belongs to a stale Annotation revision.",
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision);
        }

        if (selection.Status != SelectionStatus.Locked
            || selection.InteractionMode != SelectionInteractionMode.Locked
            || selection.NormalizedPhysicalBounds is not PhysicalRect bounds
            || !bounds.IsPositive)
        {
            return TextResult(
                TextDraftResultKind.InvalidGeometry,
                null,
                string.Empty,
                null,
                _documents.Current,
                null,
                "Text creation requires a valid locked Selection.",
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision);
        }

        return null;
    }

    private TextDraftResult? ValidateTextRequest(
        TextDraftRequest request,
        SelectionVisualState selection)
    {
        if (!IsCurrentSession(request.SessionId, request.CoordinateVersion))
        {
            return TextResult(
                TextDraftResultKind.StaleSession,
                request,
                _textDraft?.Text ?? string.Empty,
                null,
                _documents.Current,
                null,
                "Text draft request belongs to a stale capture session.");
        }

        if (request.SelectionRevision != selection.SelectionRevision)
        {
            return TextResult(
                TextDraftResultKind.StaleSelectionRevision,
                request,
                _textDraft?.Text ?? string.Empty,
                null,
                _documents.Current,
                null,
                "Text draft request belongs to a stale Selection revision.");
        }

        if (_textDraft is null)
        {
            return TextResult(
                TextDraftResultKind.NoActiveDraft,
                request,
                string.Empty,
                null,
                _documents.Current,
                null,
                "No Text draft is active.");
        }

        var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
        if (request.ExpectedAnnotationRevision != currentRevision)
        {
            return TextResult(
                TextDraftResultKind.StaleAnnotationRevision,
                request,
                _textDraft?.Text ?? string.Empty,
                null,
                _documents.Current,
                null,
                "Text draft request belongs to a stale Annotation revision.");
        }

        if (_activeTool != EditingToolKind.Text
            || request.DraftId == Guid.Empty
            || request.DraftId != _textDraft.Request.DraftId
            || request.AnchorInVirtualDesktop != _textDraft.Request.AnchorInVirtualDesktop
            || request.BoundsInVirtualDesktop != _textDraft.Request.BoundsInVirtualDesktop)
        {
            return TextResult(
                TextDraftResultKind.DraftMismatch,
                request,
                _textDraft.Text,
                null,
                _documents.Current,
                null,
                "The Text draft request does not match the active draft.");
        }

        if (selection.Status != SelectionStatus.Locked
            || selection.InteractionMode != SelectionInteractionMode.Locked
            || selection.NormalizedPhysicalBounds is not PhysicalRect bounds
            || !bounds.IsPositive
            || !bounds.Contains(request.BoundsInVirtualDesktop)
            || !Contains(bounds, request.AnchorInVirtualDesktop))
        {
            return TextResult(
                TextDraftResultKind.InvalidGeometry,
                request,
                _textDraft.Text,
                null,
                _documents.Current,
                null,
                "The Text draft boundary is outside the current Selection.");
        }

        return null;
    }

    private TextDraftResult FailedText(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        string message) => TextResult(
        TextDraftResultKind.Failed,
        null,
        _textDraft?.Text ?? string.Empty,
        null,
        _documents.Current,
        CreateFailure(sessionId, FailureCode.InvalidStateTransition, message),
        message,
        sessionId,
        coordinateVersion,
        selectionRevision);

    private TextDraftResult TextResult(
        TextDraftResultKind kind,
        TextDraftRequest? request,
        string text,
        AnnotationObject? committedObject,
        AnnotationDocument? document,
        Failure? failure,
        string message,
        Guid? sessionId = null,
        string? coordinateVersion = null,
        int? selectionRevision = null) => new(
        kind,
        _activeTool,
        request?.SessionId ?? sessionId ?? _sessionId ?? Guid.Empty,
        request?.CoordinateVersion ?? coordinateVersion ?? _coordinateVersion,
        request?.SelectionRevision ?? selectionRevision ?? _selectionRevision,
        document?.Revision ?? _documents.Current?.Revision ?? AnnotationRevision.Initial,
        request,
        text,
        _textDraft?.Style ?? _textStyle,
        committedObject,
        document,
        failure,
        message);

    private static PhysicalRect CreateTextBounds(
        PhysicalPoint anchor,
        PhysicalRect selection)
    {
        var right = Math.Min((long)selection.Right, (long)anchor.X + 320);
        var bottom = Math.Min((long)selection.Bottom, (long)anchor.Y + 96);
        return new PhysicalRect(
            anchor.X,
            anchor.Y,
            checked((int)right),
            checked((int)bottom));
    }

    private PrivacyRegionPointerResult? ValidatePrivacyRegion(
        PrivacyRegionPointerEvent input,
        SelectionVisualState selection)
    {
        if (input.PointerId <= 0)
        {
            return PrivacyRegionResult(
                PrivacyRegionPointerResultKind.PointerMismatch,
                input,
                _privacyRegionDraft?.Bounds,
                null,
                _privacyRegionDraft?.DraftId,
                _documents.Current,
                null,
                "Privacy Region input must contain a positive pointer identifier.");
        }

        if (!IsCurrentSession(input.SessionId, input.CoordinateVersion))
        {
            return PrivacyRegionResult(
                PrivacyRegionPointerResultKind.StaleSession,
                input,
                _privacyRegionDraft?.Bounds,
                null,
                _privacyRegionDraft?.DraftId,
                _documents.Current,
                null,
                "Privacy Region input belongs to a stale capture session.");
        }

        if (input.SelectionRevision != selection.SelectionRevision)
        {
            return PrivacyRegionResult(
                PrivacyRegionPointerResultKind.StaleSelectionRevision,
                input,
                _privacyRegionDraft?.Bounds,
                null,
                _privacyRegionDraft?.DraftId,
                _documents.Current,
                null,
                "Privacy Region input belongs to a stale Selection revision.");
        }

        var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
        if (input.ExpectedAnnotationRevision != currentRevision)
        {
            return PrivacyRegionResult(
                PrivacyRegionPointerResultKind.StaleAnnotationRevision,
                input,
                _privacyRegionDraft?.Bounds,
                null,
                _privacyRegionDraft?.DraftId,
                _documents.Current,
                null,
                "Privacy Region input belongs to a stale Annotation revision.");
        }

        if (!TryGetPrivacyParameters(_privacyRegionMode, out _, out var failure))
        {
            return PrivacyRegionResult(
                PrivacyRegionPointerResultKind.InvalidEffectParameters,
                input,
                _privacyRegionDraft?.Bounds,
                null,
                _privacyRegionDraft?.DraftId,
                _documents.Current,
                failure,
                "The Privacy Region effect parameters are invalid.");
        }

        if (_activeTool != EditingToolKind.PrivacyRegion)
        {
            return FailedPrivacyRegion(
                input,
                "Privacy Region input was received while another editing tool is active.");
        }

        if (selection.Status != SelectionStatus.Locked
            || selection.InteractionMode != SelectionInteractionMode.Locked
            || selection.NormalizedPhysicalBounds is not PhysicalRect selectionBounds
            || !selectionBounds.IsPositive)
        {
            return PrivacyRegionResult(
                PrivacyRegionPointerResultKind.InvalidGeometry,
                input,
                _privacyRegionDraft?.Bounds,
                null,
                _privacyRegionDraft?.DraftId,
                _documents.Current,
                null,
                "Privacy Region input requires a valid locked Selection boundary.");
        }

        return null;
    }

    private bool TryGetPrivacyParameters(
        PrivacyRegionMode mode,
        out PrivacyRegionEffectParameters parameters,
        out Failure? failure)
    {
        parameters = null!;
        failure = null;
        if (!Enum.IsDefined(mode))
        {
            failure = CreateFailure(
                _sessionId ?? Guid.Empty,
                FailureCode.InvalidStateTransition,
                "The Privacy Region mode is not supported.");
            return false;
        }

        try
        {
            parameters = _privacyRegionEffectPolicy.GetParameters(mode);
            return parameters is not null;
        }
        catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException)
        {
            failure = CreateFailure(
                _sessionId ?? Guid.Empty,
                FailureCode.InvalidStateTransition,
                exception.Message);
            return false;
        }
    }

    private PrivacyRegionPointerResult FailedPrivacyRegion(
        PrivacyRegionPointerEvent input,
        string message) => PrivacyRegionResult(
        PrivacyRegionPointerResultKind.Failed,
        input,
        _privacyRegionDraft?.Bounds,
        null,
        _privacyRegionDraft?.DraftId,
        _documents.Current,
        CreateFailure(input.SessionId, FailureCode.InvalidStateTransition, message),
        message);

    private PrivacyRegionPointerResult PrivacyRegionResult(
        PrivacyRegionPointerResultKind kind,
        PrivacyRegionPointerEvent input,
        PhysicalRect? draftBounds,
        AnnotationObject? committedObject,
        Guid? draftId,
        AnnotationDocument? document,
        Failure? failure,
        string message) => new(
        kind,
        _activeTool,
        _privacyRegionMode,
        _privacyRegionEffectPolicy.GetParameters(_privacyRegionMode),
        input.SessionId,
        input.CoordinateVersion,
        input.SelectionRevision,
        document?.Revision ?? _documents.Current?.Revision ?? AnnotationRevision.Initial,
        draftId,
        draftBounds,
        committedObject,
        document,
        failure,
        message);

    private PrivacyRegionModeSelectionResult PrivacyModeResult(
        PrivacyRegionModeSelectionResultKind kind,
        PrivacyRegionModeSelectionRequest request,
        Failure? failure,
        string message) => new(
        kind,
        _activeTool,
        _privacyRegionMode,
        _privacyRegionEffectPolicy.GetParameters(_privacyRegionMode),
        request.SessionId,
        request.CoordinateVersion,
        request.SelectionRevision,
        _documents.Current?.Revision ?? AnnotationRevision.Initial,
        failure,
        message);

    private HighlighterPointerResult? ValidateHighlighter(
        HighlighterPointerEvent input,
        SelectionVisualState selection)
    {
        if (input.PointerId <= 0)
        {
            return HighlighterResult(
                HighlighterPointerResultKind.PointerMismatch,
                input,
                _highlighterDraft?.Points,
                null,
                _documents.Current,
                null,
                "Highlighter input must contain a positive pointer identifier.");
        }

        if (!IsCurrentSession(input.SessionId, input.CoordinateVersion))
        {
            return HighlighterResult(
                HighlighterPointerResultKind.StaleSession,
                input,
                _highlighterDraft?.Points,
                null,
                _documents.Current,
                null,
                "Highlighter input belongs to a stale capture session.");
        }

        if (input.SelectionRevision != selection.SelectionRevision)
        {
            return HighlighterResult(
                HighlighterPointerResultKind.StaleSelectionRevision,
                input,
                _highlighterDraft?.Points,
                null,
                _documents.Current,
                null,
                "Highlighter input belongs to a stale Selection revision.");
        }

        var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
        if (input.ExpectedAnnotationRevision != currentRevision)
        {
            return HighlighterResult(
                HighlighterPointerResultKind.StaleAnnotationRevision,
                input,
                _highlighterDraft?.Points,
                null,
                _documents.Current,
                null,
                "Highlighter input belongs to a stale Annotation revision.");
        }

        if (selection.Status != SelectionStatus.Locked
            || selection.InteractionMode != SelectionInteractionMode.Locked
            || selection.NormalizedPhysicalBounds is not PhysicalRect selectionBounds
            || !selectionBounds.IsPositive)
        {
            return HighlighterResult(
                HighlighterPointerResultKind.InvalidGeometry,
                input,
                _highlighterDraft?.Points,
                null,
                _documents.Current,
                null,
                "Highlighter input requires a valid locked Selection boundary.");
        }

        return null;
    }

    private ArrowLinePointerResult? ValidateArrowLine(
        ArrowLinePointerEvent input,
        SelectionVisualState selection)
    {
        if (input.PointerId <= 0)
        {
            return ArrowLineResult(
                ArrowLinePointerResultKind.PointerMismatch,
                input,
                _arrowLineDraft?.Segment,
                null,
                _documents.Current,
                null,
                "Arrow or line input must contain a positive pointer identifier.");
        }

        if (!IsCurrentSession(input.SessionId, input.CoordinateVersion))
        {
            return ArrowLineResult(
                ArrowLinePointerResultKind.StaleSession,
                input,
                _arrowLineDraft?.Segment,
                null,
                _documents.Current,
                null,
                "Arrow or line input belongs to a stale capture session.");
        }

        if (input.SelectionRevision != selection.SelectionRevision)
        {
            return ArrowLineResult(
                ArrowLinePointerResultKind.StaleSelectionRevision,
                input,
                _arrowLineDraft?.Segment,
                null,
                _documents.Current,
                null,
                "Arrow or line input belongs to a stale Selection revision.");
        }

        var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
        if (input.ExpectedAnnotationRevision != currentRevision)
        {
            return ArrowLineResult(
                ArrowLinePointerResultKind.StaleAnnotationRevision,
                input,
                _arrowLineDraft?.Segment,
                null,
                _documents.Current,
                null,
                "Arrow or line input belongs to a stale Annotation revision.");
        }

        if (selection.Status != SelectionStatus.Locked
            || selection.InteractionMode != SelectionInteractionMode.Locked
            || selection.NormalizedPhysicalBounds is not PhysicalRect selectionBounds
            || !selectionBounds.IsPositive)
        {
            return ArrowLineResult(
                ArrowLinePointerResultKind.InvalidGeometry,
                input,
                _arrowLineDraft?.Segment,
                null,
                _documents.Current,
                null,
                "Arrow or line input requires a valid locked Selection boundary.");
        }

        return null;
    }

    private RectanglePointerResult? Validate(
        RectanglePointerEvent input,
        SelectionVisualState selection)
    {
        if (input.PointerId <= 0)
        {
            return Result(
                RectanglePointerResultKind.PointerMismatch,
                input,
                _draft?.Bounds,
                null,
                _documents.Current,
                null,
                "Rectangle input must contain a positive pointer identifier.");
        }

        if (!IsCurrentSession(input.SessionId, input.CoordinateVersion))
        {
            return Result(
                RectanglePointerResultKind.StaleSession,
                input,
                _draft?.Bounds,
                null,
                _documents.Current,
                null,
                "Rectangle input belongs to a stale capture session.");
        }

        if (input.SelectionRevision != selection.SelectionRevision)
        {
            return Result(
                RectanglePointerResultKind.StaleSelectionRevision,
                input,
                _draft?.Bounds,
                null,
                _documents.Current,
                null,
                "Rectangle input belongs to a stale Selection revision.");
        }

        var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
        if (input.ExpectedAnnotationRevision != currentRevision)
        {
            return Result(
                RectanglePointerResultKind.StaleAnnotationRevision,
                input,
                _draft?.Bounds,
                null,
                _documents.Current,
                null,
                "Rectangle input belongs to a stale Annotation revision.");
        }

        if (selection.Status != SelectionStatus.Locked
            || selection.InteractionMode != SelectionInteractionMode.Locked
            || selection.NormalizedPhysicalBounds is not PhysicalRect selectionBounds
            || !selectionBounds.IsPositive)
        {
            return Result(
                RectanglePointerResultKind.InvalidGeometry,
                input,
                _draft?.Bounds,
                null,
                _documents.Current,
                null,
                "Rectangle input requires a valid locked Selection boundary.");
        }

        return null;
    }

    private NumberedMarkerPointerResult? ValidateNumberedMarker(
        NumberedMarkerPointerEvent input,
        SelectionVisualState selection)
    {
        if (input.PointerId <= 0)
        {
            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.PointerMismatch,
                input,
                _numberedMarkerDraft,
                null,
                _documents.Current,
                null,
                "Numbered Marker input must contain a positive pointer identifier.");
        }

        if (!IsCurrentSession(input.SessionId, input.CoordinateVersion))
        {
            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.StaleSession,
                input,
                _numberedMarkerDraft,
                null,
                _documents.Current,
                null,
                "Numbered Marker input belongs to a stale capture session.");
        }

        if (input.SelectionRevision != selection.SelectionRevision)
        {
            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.StaleSelectionRevision,
                input,
                _numberedMarkerDraft,
                null,
                _documents.Current,
                null,
                "Numbered Marker input belongs to a stale Selection revision.");
        }

        var currentRevision = _documents.Current?.Revision ?? AnnotationRevision.Initial;
        if (input.ExpectedAnnotationRevision != currentRevision)
        {
            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.StaleAnnotationRevision,
                input,
                _numberedMarkerDraft,
                null,
                _documents.Current,
                null,
                "Numbered Marker input belongs to a stale Annotation revision.");
        }

        if (_activeTool != EditingToolKind.NumberedMarker)
        {
            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.Failed,
                input,
                _numberedMarkerDraft,
                null,
                _documents.Current,
                CreateFailure(
                    input.SessionId,
                    FailureCode.InvalidStateTransition,
                    "Numbered Marker input was received while another editing tool is active."),
                "Numbered Marker input was received while another editing tool is active.");
        }

        if (selection.Status != SelectionStatus.Locked
            || selection.InteractionMode != SelectionInteractionMode.Locked
            || selection.NormalizedPhysicalBounds is not PhysicalRect bounds
            || !bounds.IsPositive)
        {
            return NumberedMarkerResult(
                NumberedMarkerPointerResultKind.Failed,
                input,
                _numberedMarkerDraft,
                null,
                _documents.Current,
                CreateFailure(
                    input.SessionId,
                    FailureCode.InvalidSelection,
                    "Numbered Marker input requires a valid locked Selection boundary."),
                "Numbered Marker input requires a valid locked Selection boundary.");
        }

        return null;
    }

    private bool TryGetNumberedMarkerStyle(
        out NumberedMarkerAnnotationStyle style,
        out Failure? failure)
    {
        failure = null;
        try
        {
            style = _numberedMarkerStyle;
            if (style is null)
            {
                throw new InvalidOperationException("The Numbered Marker style policy returned no style.");
            }

            return true;
        }
        catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException or InvalidOperationException)
        {
            style = NumberedMarkerAnnotationStyle.Default;
            failure = CreateFailure(
                _sessionId ?? Guid.Empty,
                FailureCode.InvalidStateTransition,
                exception.Message);
            return false;
        }
    }

    private NumberedMarkerPointerResult NumberedMarkerResult(
        NumberedMarkerPointerResultKind kind,
        NumberedMarkerPointerEvent input,
        NumberedMarkerDraft? draft,
        AnnotationObject? committedObject,
        AnnotationDocument? document,
        Failure? failure,
        string message) => new(
        kind,
        _activeTool,
        _nextNumber,
        _numberedMarkerStyle,
        input.SessionId,
        input.CoordinateVersion,
        input.SelectionRevision,
        document?.Revision ?? _documents.Current?.Revision ?? AnnotationRevision.Initial,
        draft is null
            ? null
            : new NumberedMarkerDraftPresentation(
                draft.Number,
                draft.Center,
                draft.Bounds,
                draft.Style),
        committedObject,
        document,
        failure,
        message);

    private SetNextNumberResult NextNumberResult(
        SetNextNumberResultKind kind,
        SetNextNumberRequest request,
        Failure? failure,
        string message) => new(
        kind,
        _activeTool,
        _nextNumber,
        _numberedMarkerStyle,
        request.SessionId,
        request.CoordinateVersion,
        request.SelectionRevision,
        _documents.Current?.Revision ?? AnnotationRevision.Initial,
        failure,
        message);

    private NumberedMarkerAnnotationStyle GetActiveNumberedMarkerStyle()
    {
        try
        {
            return _numberedMarkerStyle;
        }
        catch (Exception exception) when (exception is ArgumentException or ArgumentOutOfRangeException or InvalidOperationException)
        {
            return NumberedMarkerAnnotationStyle.Default;
        }
    }

    private bool IsCurrentSession(Guid sessionId, string coordinateVersion) =>
        _sessionId == sessionId
        && string.Equals(_coordinateVersion, coordinateVersion, StringComparison.Ordinal);

    private RectanglePointerResult Failed(RectanglePointerEvent input, string message) => Result(
        RectanglePointerResultKind.Failed,
        input,
        _draft?.Bounds,
        null,
        _documents.Current,
        Failure.Create(
            FailureCode.InvalidStateTransition,
            FailureCategory.Validation,
            FailureRecoverability.RetrySameIntent,
            nameof(AnnotationEditingCoordinator),
            input.SessionId,
            message),
        message);

    private RectanglePointerResult Result(
        RectanglePointerResultKind kind,
        RectanglePointerEvent input,
        PhysicalRect? draft,
        AnnotationObject? committedObject,
        AnnotationDocument? document,
        Failure? failure,
        string message) => new(
        kind,
        _activeTool,
        input.SessionId,
        input.CoordinateVersion,
        input.SelectionRevision,
        document?.Revision ?? AnnotationRevision.Initial,
        draft,
        committedObject,
        document,
        failure,
        message);

    private ArrowLinePointerResult FailedArrowLine(
        ArrowLinePointerEvent input,
        string message) => ArrowLineResult(
        ArrowLinePointerResultKind.Failed,
        input,
        _arrowLineDraft?.Segment,
        null,
        _documents.Current,
        Failure.Create(
            FailureCode.InvalidStateTransition,
            FailureCategory.Validation,
            FailureRecoverability.RetrySameIntent,
            nameof(AnnotationEditingCoordinator),
            input.SessionId,
            message),
        message);

    private ArrowLinePointerResult ArrowLineResult(
        ArrowLinePointerResultKind kind,
        ArrowLinePointerEvent input,
        PhysicalLineSegment? draft,
        AnnotationObject? committedObject,
        AnnotationDocument? document,
        Failure? failure,
        string message) => new(
        kind,
        _activeTool,
        _arrowLineEndStyle,
        input.SessionId,
        input.CoordinateVersion,
        input.SelectionRevision,
        document?.Revision ?? AnnotationRevision.Initial,
        draft,
        committedObject,
        document,
        failure,
        message);

    private HighlighterPointerResult FailedHighlighter(
        HighlighterPointerEvent input,
        string message) => HighlighterResult(
        HighlighterPointerResultKind.Failed,
        input,
        _highlighterDraft?.Points,
        null,
        _documents.Current,
        Failure.Create(
            FailureCode.InvalidStateTransition,
            FailureCategory.Validation,
            FailureRecoverability.RetrySameIntent,
            nameof(AnnotationEditingCoordinator),
            input.SessionId,
            message),
        message);

    private HighlighterPointerResult HighlighterResult(
        HighlighterPointerResultKind kind,
        HighlighterPointerEvent input,
        IReadOnlyList<PhysicalPoint>? draftPoints,
        AnnotationObject? committedObject,
        AnnotationDocument? document,
        Failure? failure,
        string message) => new(
        kind,
        _activeTool,
        _highlighterStyle,
        input.SessionId,
        input.CoordinateVersion,
        input.SelectionRevision,
        document?.Revision ?? AnnotationRevision.Initial,
        draftPoints,
        committedObject,
        document,
        failure,
        message);

    private EditingToolSelectionResult ToolResult(
        EditingToolSelectionResultKind kind,
        EditingToolSelectionRequest request,
        Failure? failure,
        string message) => new(
        kind,
        _activeTool,
        request.SessionId,
        request.CoordinateVersion,
        request.SelectionRevision,
        _documents.Current?.Revision ?? AnnotationRevision.Initial,
        failure,
        message)
        {
            ActiveArrowLineEndStyle = _arrowLineEndStyle,
            ActivePrivacyRegionMode = _privacyRegionMode,
            ActivePrivacyRegionEffectParameters = _privacyRegionEffectPolicy.GetParameters(_privacyRegionMode),
            ActiveNumberedMarkerStyle = GetActiveNumberedMarkerStyle(),
            ActiveNumberedMarkerNextNumber = _nextNumber
        };

    private static bool Contains(PhysicalRect bounds, PhysicalPoint point) =>
        point.X >= bounds.Left
        && point.X < bounds.Right
        && point.Y >= bounds.Top
        && point.Y < bounds.Bottom;

    private AnnotationObjectEditResult DefaultStyleResult(
        AnnotationObjectStyleChangeRequest request,
        AnnotationObjectEditResultKind kind,
        string message) => new(
        kind,
        AnnotationObjectSelectionState.Empty(
            request.SessionId,
            request.CoordinateVersion,
            request.SelectionRevision,
            _documents.Current?.Revision ?? AnnotationRevision.Initial),
        _documents.Current,
        null,
        kind is AnnotationObjectEditResultKind.InvalidStyle
            ? CreateFailure(request.SessionId, FailureCode.InvalidStateTransition, message)
            : null,
        message);

    private static PhysicalRect Normalize(PhysicalPoint first, PhysicalPoint second) => new(
        Math.Min(first.X, second.X),
        Math.Min(first.Y, second.Y),
        Math.Max(first.X, second.X),
        Math.Max(first.Y, second.Y));

    private static Failure CreateFailure(Guid correlationId, FailureCode code, string message) =>
        Failure.Create(
            code,
            FailureCategory.Validation,
            FailureRecoverability.RetrySameIntent,
            nameof(AnnotationEditingCoordinator),
            correlationId,
            message);

    private sealed record RectangleDraft(int PointerId, PhysicalPoint Start)
    {
        public PhysicalPoint Current { get; init; } = Start;

        public PhysicalRect Bounds { get; init; } = new(Start.X, Start.Y, Start.X, Start.Y);
    }

    private sealed record ArrowLineDraft(int PointerId, PhysicalLineSegment Segment);

    private sealed record HighlighterDraft(
        int PointerId,
        IReadOnlyList<PhysicalPoint> Points);

    private sealed record TextDraft(
        TextDraftRequest Request,
        string Text,
        TextAnnotationStyle Style);

    private sealed record PrivacyRegionDraft(
        Guid DraftId,
        int PointerId,
        PhysicalPoint Start,
        PhysicalPoint Current,
        PrivacyRegionMode Mode,
        PrivacyRegionEffectParameters EffectParameters)
    {
        public PhysicalRect Bounds { get; init; } = new(Start.X, Start.Y, Start.X, Start.Y);
    }

    private sealed record NumberedMarkerDraft(
        int PointerId,
        int Number,
        PhysicalPoint Start,
        PhysicalPoint Center,
        NumberedMarkerAnnotationStyle Style)
    {
        public PhysicalRect Bounds { get; init; } = NumberedMarkerAnnotationContent.GetBounds(
            Center,
            Style);
    }
}
