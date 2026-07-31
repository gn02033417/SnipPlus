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

public sealed class AnnotationEditingCoordinator
{
    private readonly object _gate = new();
    private readonly AnnotationDocumentCoordinator _documents;
    private readonly Func<AnnotationObjectId> _objectIdFactory;
    private readonly IRectangleAnnotationStylePolicy _stylePolicy;
    private readonly IArrowLineAnnotationStylePolicy _arrowLineStylePolicy;
    private readonly IHighlighterAnnotationStylePolicy _highlighterStylePolicy;
    private readonly ITextAnnotationStylePolicy _textStylePolicy;
    private readonly Func<Guid> _textDraftIdFactory;
    private Guid? _sessionId;
    private string _coordinateVersion = string.Empty;
    private EditingToolKind _activeTool = EditingToolKind.Selection;
    private ArrowLineEndStyle _arrowLineEndStyle = ArrowLineEndStyle.Arrow;
    private int _selectionRevision;
    private RectangleDraft? _draft;
    private ArrowLineDraft? _arrowLineDraft;
    private HighlighterDraft? _highlighterDraft;
    private TextDraft? _textDraft;

    public AnnotationEditingCoordinator(
        AnnotationDocumentCoordinator documents,
        Func<AnnotationObjectId>? objectIdFactory = null,
        IRectangleAnnotationStylePolicy? stylePolicy = null,
        IArrowLineAnnotationStylePolicy? arrowLineStylePolicy = null,
        IHighlighterAnnotationStylePolicy? highlighterStylePolicy = null,
        ITextAnnotationStylePolicy? textStylePolicy = null,
        Func<Guid>? textDraftIdFactory = null)
    {
        _documents = documents ?? throw new ArgumentNullException(nameof(documents));
        _objectIdFactory = objectIdFactory ?? AnnotationObjectId.New;
        _stylePolicy = stylePolicy ?? new DefaultRectangleAnnotationStylePolicy();
        _arrowLineStylePolicy = arrowLineStylePolicy ?? new DefaultArrowLineAnnotationStylePolicy();
        _highlighterStylePolicy = highlighterStylePolicy ?? new DefaultHighlighterAnnotationStylePolicy();
        _textStylePolicy = textStylePolicy ?? new DefaultTextAnnotationStylePolicy();
        _textDraftIdFactory = textDraftIdFactory ?? Guid.NewGuid;
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

    public TextAnnotationStyle ActiveTextStyle =>
        _textDraft?.Style ?? _textStylePolicy.GetDefaultStyle();

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
            _textDraft = null;
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

            _activeTool = request.Tool;
            if (request.Tool == EditingToolKind.ArrowLine)
            {
                _arrowLineEndStyle = request.RequestedArrowLineEndStyle;
            }

            _draft = null;
            _arrowLineDraft = null;
            _highlighterDraft = null;
            _textDraft = null;
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
                ActiveTextStyle = _textStylePolicy.GetDefaultStyle()
            };
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
                _textStylePolicy.GetDefaultStyle());
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
                DraftHighlighterPoints = _highlighterDraft?.Points,
                ActiveTextStyle = _textStylePolicy.GetDefaultStyle(),
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
                _arrowLineDraft = null;
                _highlighterDraft = null;
                _textDraft = null;
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
        _textDraft?.Style ?? _textStylePolicy.GetDefaultStyle(),
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

    private sealed record TextDraft(
        TextDraftRequest Request,
        string Text,
        TextAnnotationStyle Style);
}
