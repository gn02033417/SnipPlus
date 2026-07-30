using SnipPlus.Contracts;

namespace SnipPlus.Core;

public interface IRectangleAnnotationStylePolicy
{
    RectangleAnnotationStyle GetDefaultStyle();
}

public sealed class DefaultRectangleAnnotationStylePolicy : IRectangleAnnotationStylePolicy
{
    public RectangleAnnotationStyle GetDefaultStyle() => RectangleAnnotationStyle.Default;
}

public sealed class AnnotationEditingCoordinator
{
    private readonly object _gate = new();
    private readonly AnnotationDocumentCoordinator _documents;
    private readonly Func<AnnotationObjectId> _objectIdFactory;
    private readonly IRectangleAnnotationStylePolicy _stylePolicy;
    private Guid? _sessionId;
    private string _coordinateVersion = string.Empty;
    private EditingToolKind _activeTool = EditingToolKind.Selection;
    private int _selectionRevision;
    private RectangleDraft? _draft;

    public AnnotationEditingCoordinator(
        AnnotationDocumentCoordinator documents,
        Func<AnnotationObjectId>? objectIdFactory = null,
        IRectangleAnnotationStylePolicy? stylePolicy = null)
    {
        _documents = documents ?? throw new ArgumentNullException(nameof(documents));
        _objectIdFactory = objectIdFactory ?? AnnotationObjectId.New;
        _stylePolicy = stylePolicy ?? new DefaultRectangleAnnotationStylePolicy();
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
            _draft = null;
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
            _draft = null;
            return new EditingToolSelectionResult(
                EditingToolSelectionResultKind.Selected,
                _activeTool,
                request.SessionId,
                request.CoordinateVersion,
                selection.SelectionRevision,
                currentRevision,
                null,
                $"The {_activeTool} editing tool is active.");
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
                document);
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
            }
        }
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
        message);

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
}
