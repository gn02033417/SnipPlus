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

public sealed class AnnotationEditingCoordinator
{
    private readonly object _gate = new();
    private readonly AnnotationDocumentCoordinator _documents;
    private readonly Func<AnnotationObjectId> _objectIdFactory;
    private readonly IRectangleAnnotationStylePolicy _stylePolicy;
    private readonly IArrowLineAnnotationStylePolicy _arrowLineStylePolicy;
    private readonly IHighlighterAnnotationStylePolicy _highlighterStylePolicy;
    private Guid? _sessionId;
    private string _coordinateVersion = string.Empty;
    private EditingToolKind _activeTool = EditingToolKind.Selection;
    private ArrowLineEndStyle _arrowLineEndStyle = ArrowLineEndStyle.Arrow;
    private int _selectionRevision;
    private RectangleDraft? _draft;
    private ArrowLineDraft? _arrowLineDraft;
    private HighlighterDraft? _highlighterDraft;

    public AnnotationEditingCoordinator(
        AnnotationDocumentCoordinator documents,
        Func<AnnotationObjectId>? objectIdFactory = null,
        IRectangleAnnotationStylePolicy? stylePolicy = null,
        IArrowLineAnnotationStylePolicy? arrowLineStylePolicy = null,
        IHighlighterAnnotationStylePolicy? highlighterStylePolicy = null)
    {
        _documents = documents ?? throw new ArgumentNullException(nameof(documents));
        _objectIdFactory = objectIdFactory ?? AnnotationObjectId.New;
        _stylePolicy = stylePolicy ?? new DefaultRectangleAnnotationStylePolicy();
        _arrowLineStylePolicy = arrowLineStylePolicy ?? new DefaultArrowLineAnnotationStylePolicy();
        _highlighterStylePolicy = highlighterStylePolicy ?? new DefaultHighlighterAnnotationStylePolicy();
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
        _highlighterStylePolicy.GetDefaultStyle();

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
            _draft = null;
            _arrowLineDraft = null;
            _highlighterDraft = null;
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

            _activeTool = request.Tool;
            if (request.Tool == EditingToolKind.ArrowLine)
            {
                _arrowLineEndStyle = request.RequestedArrowLineEndStyle;
            }

            _draft = null;
            _arrowLineDraft = null;
            _highlighterDraft = null;
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
                ActiveArrowLineEndStyle = _arrowLineEndStyle
            };
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
                    new RectangleAnnotationContent(_stylePolicy.GetDefaultStyle()));
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
                var defaultStyle = _arrowLineStylePolicy.GetDefaultStyle();
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
                        _highlighterStylePolicy.GetDefaultStyle()));
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
                ActiveHighlighterStyle = _highlighterStylePolicy.GetDefaultStyle(),
                DraftHighlighterPoints = _highlighterDraft?.Points
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
                _arrowLineDraft = null;
                _highlighterDraft = null;
            }
        }
    }

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
        _highlighterStylePolicy.GetDefaultStyle(),
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
            ActiveArrowLineEndStyle = _arrowLineEndStyle
        };

    private static bool Contains(PhysicalRect bounds, PhysicalPoint point) =>
        point.X >= bounds.Left
        && point.X < bounds.Right
        && point.Y >= bounds.Top
        && point.Y < bounds.Bottom;

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
}
