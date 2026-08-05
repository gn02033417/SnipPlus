using SnipPlus.Contracts;

namespace SnipPlus.Core;

/// <summary>
/// Owns the selection and preview draft for editing an existing annotation.
/// AnnotationDocumentCoordinator remains the only document and revision authority.
/// </summary>
public sealed class AnnotationObjectEditCoordinator : IAnnotationObjectEditingSink
{
    private readonly object _gate = new();
    private readonly AnnotationDocumentCoordinator _documents;
    private readonly Func<AnnotationObjectStyleChangeRequest, AnnotationObjectEditResult>? _defaultStyleChange;
    private readonly AnnotationHistoryCoordinator? _history;
    private Guid? _sessionId;
    private string _coordinateVersion = string.Empty;
    private int _selectionRevision;
    private PhysicalRect? _selectionBounds;
    private AnnotationObjectSelectionState _state = AnnotationObjectSelectionState.Empty(
        Guid.Empty,
        string.Empty,
        0,
        AnnotationRevision.Initial);
    private PhysicalPoint _draftStart;

    public AnnotationObjectEditCoordinator(
        AnnotationDocumentCoordinator documents,
        Func<AnnotationObjectStyleChangeRequest, AnnotationObjectEditResult>? defaultStyleChange = null,
        AnnotationHistoryCoordinator? history = null)
    {
        _documents = documents ?? throw new ArgumentNullException(nameof(documents));
        _defaultStyleChange = defaultStyleChange;
        _history = history;
    }

    public bool HasActiveEdit
    {
        get
        {
            lock (_gate)
            {
                return _state.HasActiveEdit;
            }
        }
    }

    public AnnotationObjectSelectionState State
    {
        get
        {
            lock (_gate)
            {
                return _state;
            }
        }
    }

    public bool CanHandlePointer(PhysicalPoint point)
    {
        lock (_gate)
        {
            return _state.HasActiveEdit
                || GetSelectedObject() is AnnotationObject selected && IsHit(selected, point)
                || HitTest(point) is not null;
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
            _selectionBounds = selection.NormalizedPhysicalBounds;
            _state = AnnotationObjectSelectionState.Empty(
                selection.SessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                CurrentRevision());
        }
    }

    public void UpdateSelection(SelectionVisualState selection)
    {
        ArgumentNullException.ThrowIfNull(selection);
        lock (_gate)
        {
            if (_sessionId != selection.SessionId
                || !string.Equals(_coordinateVersion, selection.CoordinateVersion, StringComparison.Ordinal))
            {
                return;
            }

            if (_selectionRevision != selection.SelectionRevision)
            {
                ClearDraft();
            }

            _selectionRevision = selection.SelectionRevision;
            _selectionBounds = selection.NormalizedPhysicalBounds;
            var selected = GetSelectedObject();
            if (selected is null
                || _selectionBounds is not PhysicalRect bounds
                || !selected.Geometry.Intersects(bounds))
            {
                _state = AnnotationObjectSelectionState.Empty(
                    selection.SessionId,
                    selection.CoordinateVersion,
                    selection.SelectionRevision,
                    CurrentRevision());
            }
            else
            {
                _state = _state with
                {
                    SelectionRevision = selection.SelectionRevision,
                    AnnotationRevision = CurrentRevision(),
                    Operation = AnnotationObjectEditOperationKind.None,
                    ActiveHandle = null,
                    ActivePointerId = null,
                    OriginalObject = selected,
                    PreviewObject = null,
                    TextEditDraftId = null
                };
            }
        }
    }

    public AnnotationObjectEditResult SelectObject(AnnotationObjectSelectionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            var rejection = Validate(request.SessionId, request.CoordinateVersion,
                request.SelectionRevision, request.ExpectedAnnotationRevision);
            if (rejection is not null)
            {
                return rejection;
            }

            var annotationObject = Find(request.ObjectId);
            if (annotationObject is null)
            {
                return Result(AnnotationObjectEditResultKind.ObjectNotFound,
                    "The requested annotation object was not found.");
            }

            if (!IsVisibleInSelection(annotationObject))
            {
                return Result(AnnotationObjectEditResultKind.ObjectNotFound,
                    "The annotation object is outside the current Selection.");
            }

            ClearDraft();
            Select(annotationObject);
            return Result(AnnotationObjectEditResultKind.Selected,
                "The annotation object is selected.", annotationObject);
        }
    }

    public AnnotationObjectEditResult PointerPressed(AnnotationObjectPointerEvent input)
    {
        ArgumentNullException.ThrowIfNull(input);
        lock (_gate)
        {
            var rejection = Validate(input.SessionId, input.CoordinateVersion,
                input.SelectionRevision, input.ExpectedAnnotationRevision);
            if (rejection is not null)
            {
                return rejection;
            }

            if (input.PointerId <= 0)
            {
                return Result(AnnotationObjectEditResultKind.PointerMismatch,
                    "The annotation pointer identifier must be positive.");
            }

            if (_state.HasActiveEdit)
            {
                return Result(AnnotationObjectEditResultKind.PointerMismatch,
                    "Another annotation edit pointer is already active.");
            }

            var selected = GetSelectedObject();
            var selectedHandle = selected is not null
                && IsHit(selected, input.GlobalPhysicalPoint)
                ? GetHandle(selected, input.GlobalPhysicalPoint)
                : AnnotationObjectEditHandleKind.Body;
            var hit = selected is not null
                && selectedHandle != AnnotationObjectEditHandleKind.Body
                ? selected
                : HitTest(input.GlobalPhysicalPoint);
            if (hit is null)
            {
                ClearDraft();
                return Result(AnnotationObjectEditResultKind.SelectionCleared,
                    "No annotation object was hit.");
            }

            Select(hit);
            _draftStart = input.GlobalPhysicalPoint;
            var handle = GetHandle(hit, input.GlobalPhysicalPoint);
            _state = _state with
            {
                Operation = handle is not AnnotationObjectEditHandleKind.Body
                    ? AnnotationObjectEditOperationKind.Resize
                    : AnnotationObjectEditOperationKind.Move,
                ActiveHandle = handle,
                ActivePointerId = input.PointerId,
                OriginalObject = hit,
                PreviewObject = hit,
                AnnotationRevision = CurrentRevision()
            };
            return Result(AnnotationObjectEditResultKind.EditStarted,
                "Annotation object editing started.", hit);
        }
    }

    public AnnotationObjectEditResult PointerMoved(AnnotationObjectPointerEvent input)
    {
        ArgumentNullException.ThrowIfNull(input);
        lock (_gate)
        {
            var rejection = ValidateActivePointer(input);
            if (rejection is not null)
            {
                return rejection;
            }

            var preview = BuildPreview(_state.OriginalObject!, input.GlobalPhysicalPoint,
                _state.Operation, _state.ActiveHandle!.Value);
            if (preview is null)
            {
                return Result(AnnotationObjectEditResultKind.InvalidGeometry,
                    "The annotation edit would create invalid geometry.");
            }

            _state = _state with { PreviewObject = preview };
            return Result(AnnotationObjectEditResultKind.EditUpdated,
                "Annotation object edit preview updated.", preview);
        }
    }

    public AnnotationObjectEditResult PointerReleased(AnnotationObjectPointerEvent input)
    {
        ArgumentNullException.ThrowIfNull(input);
        lock (_gate)
        {
            var rejection = ValidateActivePointer(input);
            if (rejection is not null)
            {
                return rejection;
            }

            var original = _state.OriginalObject!;
            var preview = BuildPreview(original, input.GlobalPhysicalPoint,
                _state.Operation, _state.ActiveHandle!.Value);
            var operation = _state.Operation;
            ClearDraft(operation == AnnotationObjectEditOperationKind.TextEdit);
            if (preview is null)
            {
                return Result(AnnotationObjectEditResultKind.InvalidGeometry,
                    "The annotation edit was rejected because its geometry is invalid.");
            }

            if (preview.Equals(original))
            {
                Select(original);
                return Result(AnnotationObjectEditResultKind.Selected,
                    "The annotation object remains selected.", original);
            }

            var beforeDocument = _documents.Current!;
            var mutation = _documents.Replace(new ReplaceAnnotationObjectRequest(
                input.SessionId,
                input.ExpectedAnnotationRevision,
                preview));
            if (mutation is not AnnotationMutationResult.Succeeded succeeded)
            {
                return MutationFailure(mutation, "The annotation edit could not be committed.");
            }

            Select(succeeded.Document.Objects.Single(value => value.ObjectId == preview.ObjectId));
            _state = _state with { AnnotationRevision = succeeded.Document.Revision };
            _history?.RecordReplace(
                input.SessionId,
                input.CoordinateVersion,
                input.SelectionRevision,
                beforeDocument,
                succeeded.Document,
                original,
                preview);
            return Result(AnnotationObjectEditResultKind.EditCommitted,
                "The annotation edit was committed.", preview, succeeded.Document);
        }
    }

    public AnnotationObjectEditResult ChangeStyle(AnnotationObjectStyleChangeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            var rejection = Validate(request.SessionId, request.CoordinateVersion,
                request.SelectionRevision, request.ExpectedAnnotationRevision);
            if (rejection is not null)
            {
                return rejection;
            }

            if (request.ObjectId is not AnnotationObjectId objectId)
            {
                return _defaultStyleChange?.Invoke(request)
                    ?? Result(AnnotationObjectEditResultKind.UnsupportedOperation,
                        "Future-object default style changes are unavailable.");
            }

            var current = Find(objectId);
            if (current is null)
            {
                return Result(AnnotationObjectEditResultKind.ObjectNotFound,
                    "The selected annotation object was not found.");
            }

            AnnotationObject styled;
            try
            {
                styled = ApplyStyle(current, request.Change);
            }
            catch (ArgumentException exception)
            {
                return Result(AnnotationObjectEditResultKind.InvalidStyle,
                    exception.Message);
            }
            catch (OverflowException exception)
            {
                return Result(AnnotationObjectEditResultKind.InvalidGeometry,
                    exception.Message);
            }

            if (!IsVisibleInSelection(styled))
            {
                return Result(AnnotationObjectEditResultKind.InvalidGeometry,
                    "The style change would move the annotation outside the current Selection.");
            }

            var beforeDocument = _documents.Current!;
            var mutation = _documents.Replace(new ReplaceAnnotationObjectRequest(
                request.SessionId,
                request.ExpectedAnnotationRevision,
                styled));
            if (mutation is not AnnotationMutationResult.Succeeded succeeded)
            {
                return MutationFailure(mutation, "The annotation style could not be changed.");
            }

            var resultObject = succeeded.Document.Objects.Single(value => value.ObjectId == objectId);
            Select(resultObject);
            _state = _state with { AnnotationRevision = succeeded.Document.Revision };
            _history?.RecordReplace(
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                beforeDocument,
                succeeded.Document,
                current,
                styled);
            return Result(AnnotationObjectEditResultKind.Restyled,
                "The annotation style was changed.", resultObject, succeeded.Document);
        }
    }

    public AnnotationObjectEditResult Delete(AnnotationObjectDeleteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            var rejection = Validate(request.SessionId, request.CoordinateVersion,
                request.SelectionRevision, request.ExpectedAnnotationRevision);
            if (rejection is not null)
            {
                return rejection;
            }

            var current = Find(request.ObjectId);
            if (current is null)
            {
                return Result(AnnotationObjectEditResultKind.ObjectNotFound,
                    "The selected annotation object was not found.");
            }

            var beforeDocument = _documents.Current!;
            var mutation = _documents.Remove(new RemoveAnnotationObjectRequest(
                request.SessionId,
                request.ExpectedAnnotationRevision,
                request.ObjectId));
            if (mutation is not AnnotationMutationResult.Succeeded succeeded)
            {
                return MutationFailure(mutation, "The annotation object could not be deleted.");
            }

            _state = AnnotationObjectSelectionState.Empty(
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                succeeded.Document.Revision);
            _history?.RecordRemove(
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                beforeDocument,
                succeeded.Document,
                current);
            return Result(AnnotationObjectEditResultKind.Deleted,
                "The annotation object was deleted.", current, succeeded.Document);
        }
    }

    public AnnotationObjectEditResult BeginTextEdit(AnnotationObjectSelectionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            var rejection = Validate(request.SessionId, request.CoordinateVersion,
                request.SelectionRevision, request.ExpectedAnnotationRevision);
            if (rejection is not null)
            {
                return rejection;
            }

            var current = Find(request.ObjectId);
            if (current?.Content is not TextAnnotationContent)
            {
                return Result(AnnotationObjectEditResultKind.UnsupportedOperation,
                    "Only a Text annotation can enter text editing.");
            }

            Select(current);
            _state = _state with
            {
                Operation = AnnotationObjectEditOperationKind.TextEdit,
                ActiveHandle = AnnotationObjectEditHandleKind.Body,
                TextEditDraftId = Guid.NewGuid(),
                OriginalObject = current,
                PreviewObject = current
            };
            return Result(AnnotationObjectEditResultKind.TextEditStarted,
                "Text editing started.", current);
        }
    }

    public AnnotationObjectEditResult UpdateTextEdit(AnnotationObjectTextEditRequest request) =>
        UpdateText(request, commit: false);

    public AnnotationObjectEditResult CommitTextEdit(AnnotationObjectTextEditRequest request) =>
        UpdateText(request, commit: true);

    public AnnotationObjectEditResult CancelEdit(Guid sessionId, string coordinateVersion)
    {
        lock (_gate)
        {
            if (_sessionId != sessionId
                || !string.Equals(_coordinateVersion, coordinateVersion, StringComparison.Ordinal))
            {
                return Result(AnnotationObjectEditResultKind.StaleSession,
                    "The annotation edit belongs to a stale capture session.");
            }

            var selected = GetSelectedObject();
            var wasActive = _state.HasActiveEdit;
            ClearDraft();
            if (selected is not null)
            {
                Select(selected);
            }

            return Result(wasActive
                ? AnnotationObjectEditResultKind.EditCancelled
                : AnnotationObjectEditResultKind.Selected,
                wasActive ? "The annotation edit was cancelled." : "No annotation edit was active.",
                selected);
        }
    }

    public void ClearSession(Guid sessionId)
    {
        lock (_gate)
        {
            if (_sessionId != sessionId)
            {
                return;
            }

            ClearDraft();
            _sessionId = null;
            _coordinateVersion = string.Empty;
            _selectionRevision = 0;
            _selectionBounds = null;
            _state = AnnotationObjectSelectionState.Empty(
                Guid.Empty,
                string.Empty,
                0,
                AnnotationRevision.Initial);
        }
    }

    public void ReconcileAfterHistory(AnnotationHistoryResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        lock (_gate)
        {
            if (result.Kind != AnnotationHistoryResultKind.Succeeded
                || result.Document is null
                || result.AffectedObjectId is not AnnotationObjectId objectId)
            {
                return;
            }

            var affected = result.Document.Objects
                .FirstOrDefault(value => value.ObjectId == objectId);
            var select = result.EntryKind switch
            {
                AnnotationHistoryEntryKind.Add or AnnotationHistoryEntryKind.NumberedMarkerCreation =>
                    result.Command == AnnotationHistoryCommand.Redo,
                AnnotationHistoryEntryKind.Remove =>
                    result.Command == AnnotationHistoryCommand.Undo,
                AnnotationHistoryEntryKind.Replace => true,
                _ => false
            };

            if (select && affected is AnnotationObject annotationObject)
            {
                Select(annotationObject);
            }
            else if (!select)
            {
                _state = AnnotationObjectSelectionState.Empty(
                    result.SessionId,
                    result.CoordinateVersion,
                    result.SelectionRevision,
                    result.CurrentAnnotationRevision);
            }
        }
    }

    private AnnotationObjectEditResult UpdateText(
        AnnotationObjectTextEditRequest request,
        bool commit)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            var rejection = ValidateActiveText(request);
            if (rejection is not null)
            {
                return rejection;
            }

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return Result(AnnotationObjectEditResultKind.EmptyText,
                    "Text annotation content cannot be empty.");
            }

            var original = _state.OriginalObject!;
            var content = (TextAnnotationContent)original.Content!;
            AnnotationObject preview;
            try
            {
                preview = new AnnotationObject(
                    original.ObjectId,
                    original.SessionId,
                    original.ToolKind,
                    original.Geometry,
                    original.ZOrder,
                    new TextAnnotationContent(
                        request.Text,
                        content.AnchorInVirtualDesktop,
                        content.BoundsInVirtualDesktop,
                        content.Style));
            }
            catch (ArgumentException exception)
            {
                return Result(AnnotationObjectEditResultKind.InvalidGeometry,
                    exception.Message);
            }

            _state = _state with { PreviewObject = preview };
            if (!commit)
            {
                return Result(AnnotationObjectEditResultKind.EditUpdated,
                    "Text edit preview updated.", preview);
            }

            var beforeDocument = _documents.Current!;
            var mutation = _documents.Replace(new ReplaceAnnotationObjectRequest(
                request.SessionId,
                request.ExpectedAnnotationRevision,
                preview));
            if (mutation is not AnnotationMutationResult.Succeeded succeeded)
            {
                return MutationFailure(mutation, "The text annotation could not be committed.");
            }

            ClearDraft();
            var committed = succeeded.Document.Objects.Single(value => value.ObjectId == preview.ObjectId);
            Select(committed);
            _state = _state with { AnnotationRevision = succeeded.Document.Revision };
            _history?.RecordReplace(
                request.SessionId,
                request.CoordinateVersion,
                request.SelectionRevision,
                beforeDocument,
                succeeded.Document,
                original,
                preview);
            return Result(AnnotationObjectEditResultKind.TextEditCommitted,
                "The text annotation was committed.", committed, succeeded.Document);
        }
    }

    private AnnotationObjectEditResult? ValidateActiveText(AnnotationObjectTextEditRequest request)
    {
        var rejection = Validate(request.SessionId, request.CoordinateVersion,
            request.SelectionRevision, request.ExpectedAnnotationRevision);
        if (rejection is not null)
        {
            return rejection;
        }

        if (_state.Operation != AnnotationObjectEditOperationKind.TextEdit
            || _state.SelectedObjectId != request.ObjectId
            || _state.TextEditDraftId != request.DraftId)
        {
            return Result(AnnotationObjectEditResultKind.DraftMismatch,
                "The text edit draft does not match the selected object.");
        }

        return null;
    }

    private AnnotationObjectEditResult? ValidateActivePointer(AnnotationObjectPointerEvent input)
    {
        var rejection = Validate(input.SessionId, input.CoordinateVersion,
            input.SelectionRevision, input.ExpectedAnnotationRevision);
        if (rejection is not null)
        {
            return rejection;
        }

        if (!_state.HasActiveEdit || _state.ActivePointerId != input.PointerId)
        {
            return Result(AnnotationObjectEditResultKind.PointerMismatch,
                "The annotation pointer does not match the active edit.");
        }

        if (_state.OriginalObject is null
            || Find(_state.OriginalObject.ObjectId) is not AnnotationObject current
            || !current.Equals(_state.OriginalObject))
        {
            return Result(AnnotationObjectEditResultKind.StaleObject,
                "The selected annotation object changed while it was being edited.");
        }

        return null;
    }

    private AnnotationObjectEditResult? Validate(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationRevision expectedAnnotationRevision)
    {
        if (_sessionId != sessionId
            || !string.Equals(_coordinateVersion, coordinateVersion, StringComparison.Ordinal))
        {
            return Result(AnnotationObjectEditResultKind.StaleSession,
                "The annotation request belongs to a stale capture session.");
        }

        if (_selectionRevision != selectionRevision)
        {
            return Result(AnnotationObjectEditResultKind.StaleSelectionRevision,
                "The annotation request belongs to a stale Selection revision.");
        }

        if (CurrentRevision() != expectedAnnotationRevision)
        {
            return Result(AnnotationObjectEditResultKind.StaleAnnotationRevision,
                "The annotation request belongs to a stale Annotation revision.");
        }

        return null;
    }

    private AnnotationObject? HitTest(PhysicalPoint point) =>
        _documents.Current?.Objects
            .Where(IsVisibleInSelection)
            .OrderByDescending(value => value.ZOrder)
            .ThenByDescending(value => value.ObjectId.Value)
            .FirstOrDefault(value => IsHit(value, point));

    private bool IsVisibleInSelection(AnnotationObject annotationObject) =>
        _selectionBounds is PhysicalRect bounds
        && bounds.IsPositive
        && annotationObject.Geometry.Intersects(bounds);

    private static bool IsHit(AnnotationObject annotationObject, PhysicalPoint point)
    {
        if (!annotationObject.Geometry.IsPositive)
        {
            return false;
        }

        return annotationObject.Content switch
        {
            ArrowLineAnnotationContent arrow => DistanceToSegment(point, arrow.Segment) <= 12,
            HighlighterStrokeContent highlighter => highlighter.Path.Points
                .Zip(highlighter.Path.Points.Skip(1))
                .Any(pair => DistanceToSegment(point, new PhysicalLineSegment(pair.First, pair.Second)) <=
                    Math.Max(12, highlighter.Style.StrokeThickness / 2d)),
            _ => Contains(annotationObject.Geometry, point)
        };
    }

    private static AnnotationObjectEditHandleKind GetHandle(
        AnnotationObject annotationObject,
        PhysicalPoint point)
    {
        if (annotationObject.ToolKind == AnnotationToolKind.ArrowLine
            && annotationObject.Content is ArrowLineAnnotationContent arrow)
        {
            if (Distance(point, arrow.Segment.Start) <= 14)
            {
                return AnnotationObjectEditHandleKind.StartEndpoint;
            }

            if (Distance(point, arrow.Segment.End) <= 14)
            {
                return AnnotationObjectEditHandleKind.EndEndpoint;
            }

            return AnnotationObjectEditHandleKind.Body;
        }

        if (annotationObject.ToolKind == AnnotationToolKind.NumberedMarker)
        {
            return AnnotationObjectEditHandleKind.Body;
        }

        var bounds = annotationObject.Geometry;
        const double threshold = 14;
        var left = Math.Abs(point.X - bounds.Left) <= threshold;
        var right = Math.Abs(point.X - bounds.Right) <= threshold;
        var top = Math.Abs(point.Y - bounds.Top) <= threshold;
        var bottom = Math.Abs(point.Y - bounds.Bottom) <= threshold;
        return (left, top, right, bottom) switch
        {
            (true, true, _, _) => AnnotationObjectEditHandleKind.TopLeftCorner,
            (_, true, true, _) => AnnotationObjectEditHandleKind.TopRightCorner,
            (true, _, _, true) => AnnotationObjectEditHandleKind.BottomLeftCorner,
            (_, _, true, true) => AnnotationObjectEditHandleKind.BottomRightCorner,
            (true, _, _, _) => AnnotationObjectEditHandleKind.LeftEdge,
            (_, true, _, _) => AnnotationObjectEditHandleKind.TopEdge,
            (_, _, true, _) => AnnotationObjectEditHandleKind.RightEdge,
            (_, _, _, true) => AnnotationObjectEditHandleKind.BottomEdge,
            _ => AnnotationObjectEditHandleKind.Body
        };
    }

    private AnnotationObject? BuildPreview(
        AnnotationObject original,
        PhysicalPoint point,
        AnnotationObjectEditOperationKind operation,
        AnnotationObjectEditHandleKind handle)
    {
        try
        {
            var preview = operation switch
            {
                AnnotationObjectEditOperationKind.Move => Translate(original,
                    checked((long)point.X - _draftStart.X),
                    checked((long)point.Y - _draftStart.Y)),
                AnnotationObjectEditOperationKind.Resize => Resize(original, point, handle),
                _ => original
            };

            return preview is not null
                && preview.Geometry.IsPositive
                && IsVisibleInSelection(preview)
                ? preview
                : null;
        }
        catch (OverflowException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static AnnotationObject Translate(AnnotationObject original, long dx, long dy)
    {
        var geometry = TranslateRect(original.Geometry, dx, dy);
        return original.Content switch
        {
            ArrowLineAnnotationContent arrow => NewObject(original, geometry,
                new ArrowLineAnnotationContent(
                    new PhysicalLineSegment(
                        TranslatePoint(arrow.Segment.Start, dx, dy),
                        TranslatePoint(arrow.Segment.End, dx, dy)),
                    arrow.Style)),
            HighlighterStrokeContent highlighter => NewObject(original, geometry,
                new HighlighterStrokeContent(
                    new PhysicalPolyline(highlighter.Path.Points.Select(value => TranslatePoint(value, dx, dy))),
                    highlighter.Style)),
            TextAnnotationContent text => NewObject(original, geometry,
                new TextAnnotationContent(
                    text.Text,
                    TranslatePoint(text.AnchorInVirtualDesktop, dx, dy),
                    geometry,
                    text.Style)),
            _ => NewObject(original, geometry, original.Content)
        };
    }

    private static AnnotationObject? Resize(
        AnnotationObject original,
        PhysicalPoint point,
        AnnotationObjectEditHandleKind handle)
    {
        if (original.ToolKind == AnnotationToolKind.ArrowLine
            && original.Content is ArrowLineAnnotationContent arrow)
        {
            var segment = handle switch
            {
                AnnotationObjectEditHandleKind.StartEndpoint => arrow.Segment with { Start = point },
                AnnotationObjectEditHandleKind.EndEndpoint => arrow.Segment with { End = point },
                _ => arrow.Segment
            };
            return segment.IsPositive
                ? NewObject(original, segment.Bounds, new ArrowLineAnnotationContent(segment, arrow.Style))
                : null;
        }

        var bounds = original.Geometry;
        if (original.ToolKind == AnnotationToolKind.HighlighterStroke
            && original.Content is HighlighterStrokeContent highlighter)
        {
            var resized = ResizeRect(bounds, point, handle);
            if (!resized.IsPositive)
            {
                return null;
            }

            var points = highlighter.Path.Points.Select(value => ScalePoint(value, bounds, resized));
            var path = new PhysicalPolyline(points);
            return path.HasLength
                ? NewObject(original, path.Bounds, new HighlighterStrokeContent(path, highlighter.Style))
                : null;
        }

        if (original.ToolKind is not AnnotationToolKind.Rectangle
            and not AnnotationToolKind.PrivacyRegion)
        {
            return null;
        }

        var nextBounds = ResizeRect(bounds, point, handle);
        if (!nextBounds.IsPositive)
        {
            return null;
        }

        return NewObject(original, nextBounds, original.Content);
    }

    private static PhysicalRect ResizeRect(
        PhysicalRect original,
        PhysicalPoint point,
        AnnotationObjectEditHandleKind handle)
    {
        long left = original.Left;
        long top = original.Top;
        long right = original.Right;
        long bottom = original.Bottom;
        switch (handle)
        {
            case AnnotationObjectEditHandleKind.LeftEdge:
            case AnnotationObjectEditHandleKind.TopLeftCorner:
            case AnnotationObjectEditHandleKind.BottomLeftCorner:
                left = point.X;
                break;
            case AnnotationObjectEditHandleKind.RightEdge:
            case AnnotationObjectEditHandleKind.TopRightCorner:
            case AnnotationObjectEditHandleKind.BottomRightCorner:
                right = point.X;
                break;
        }

        switch (handle)
        {
            case AnnotationObjectEditHandleKind.TopEdge:
            case AnnotationObjectEditHandleKind.TopLeftCorner:
            case AnnotationObjectEditHandleKind.TopRightCorner:
                top = point.Y;
                break;
            case AnnotationObjectEditHandleKind.BottomEdge:
            case AnnotationObjectEditHandleKind.BottomLeftCorner:
            case AnnotationObjectEditHandleKind.BottomRightCorner:
                bottom = point.Y;
                break;
        }

        var normalizedLeft = Math.Min(left, right);
        var normalizedRight = Math.Max(left, right);
        var normalizedTop = Math.Min(top, bottom);
        var normalizedBottom = Math.Max(top, bottom);
        return new PhysicalRect(
            checked((int)normalizedLeft),
            checked((int)normalizedTop),
            checked((int)normalizedRight),
            checked((int)normalizedBottom));
    }

    private static PhysicalPoint ScalePoint(
        PhysicalPoint point,
        PhysicalRect original,
        PhysicalRect resized)
    {
        var x = resized.Left + Math.Round(
            (point.X - original.Left) * (double)resized.Width64 / original.Width64,
            MidpointRounding.AwayFromZero);
        var y = resized.Top + Math.Round(
            (point.Y - original.Top) * (double)resized.Height64 / original.Height64,
            MidpointRounding.AwayFromZero);
        return new PhysicalPoint(checked((int)x), checked((int)y));
    }

    private static AnnotationObject ApplyStyle(
        AnnotationObject current,
        AnnotationObjectStyleChange change)
    {
        ArgumentNullException.ThrowIfNull(change);
        return current.Content switch
        {
            RectangleAnnotationContent rectangle => NewObject(current, current.Geometry,
                new RectangleAnnotationContent(new RectangleAnnotationStyle(
                    change.Color ?? rectangle.Style.StrokeColor,
                    change.Thickness ?? rectangle.Style.StrokeThickness))),
            ArrowLineAnnotationContent arrow => NewObject(current, current.Geometry,
                new ArrowLineAnnotationContent(arrow.Segment, new ArrowLineAnnotationStyle(
                    change.Color ?? arrow.Style.StrokeColor,
                    change.Thickness ?? arrow.Style.StrokeThickness,
                    change.ArrowLineEndStyle ?? arrow.Style.EndStyle))),
            HighlighterStrokeContent highlighter => NewObject(current, current.Geometry,
                new HighlighterStrokeContent(highlighter.Path, new HighlighterAnnotationStyle(
                    change.Color ?? highlighter.Style.StrokeColor,
                    change.Thickness ?? highlighter.Style.StrokeThickness))),
            TextAnnotationContent text => NewObject(current, current.Geometry,
                new TextAnnotationContent(text.Text, text.AnchorInVirtualDesktop, text.BoundsInVirtualDesktop,
                    new TextAnnotationStyle(
                        text.Style.FontFamily,
                        change.FontSize ?? text.Style.FontSize,
                        change.Color ?? text.Style.Color,
                        change.Bold ?? text.Style.Bold))),
            PrivacyRegionAnnotationContent privacy => NewObject(current, current.Geometry,
                new PrivacyRegionAnnotationContent(
                    change.PrivacyMode ?? privacy.Mode,
                    change.PrivacyEffectParameters ?? privacy.EffectParameters)),
            NumberedMarkerAnnotationContent marker => NewMarker(current, marker,
                change.Color ?? marker.Style.Color,
                change.MarkerSize ?? marker.Style.Size),
            _ => throw new ArgumentException("The annotation object has unsupported content.", nameof(current))
        };
    }

    private static AnnotationObject NewMarker(
        AnnotationObject current,
        NumberedMarkerAnnotationContent marker,
        ArgbColor color,
        int size)
    {
        var style = new NumberedMarkerAnnotationStyle(color, size);
        var center = new PhysicalPoint(
            checked((int)(((long)current.Geometry.Left + current.Geometry.Right) / 2)),
            checked((int)(((long)current.Geometry.Top + current.Geometry.Bottom) / 2)));
        return NewObject(current, NumberedMarkerAnnotationContent.GetBounds(center, style),
            new NumberedMarkerAnnotationContent(marker.Number, style));
    }

    private static AnnotationObject NewObject(
        AnnotationObject original,
        PhysicalRect geometry,
        IAnnotationContent? content) => new(
        original.ObjectId,
        original.SessionId,
        original.ToolKind,
        geometry,
        original.ZOrder,
        content);

    private AnnotationObject? Find(AnnotationObjectId objectId) =>
        _documents.Current?.Objects.FirstOrDefault(value => value.ObjectId == objectId);

    private AnnotationObject? GetSelectedObject() =>
        _state.SelectedObjectId is AnnotationObjectId objectId ? Find(objectId) : null;

    private void Select(AnnotationObject annotationObject) =>
        _state = _state with
        {
            SelectedObjectId = annotationObject.ObjectId,
            Operation = AnnotationObjectEditOperationKind.None,
            ActiveHandle = null,
            ActivePointerId = null,
            OriginalObject = annotationObject,
            PreviewObject = null,
            TextEditDraftId = null,
            AnnotationRevision = CurrentRevision()
        };

    private void ClearDraft(bool keepTextEdit = false)
    {
        _state = _state with
        {
            Operation = AnnotationObjectEditOperationKind.None,
            ActiveHandle = null,
            ActivePointerId = null,
            PreviewObject = null,
            TextEditDraftId = keepTextEdit ? _state.TextEditDraftId : null
        };
    }

    private AnnotationRevision CurrentRevision() =>
        _documents.Current?.Revision ?? AnnotationRevision.Initial;

    private AnnotationObjectEditResult Result(
        AnnotationObjectEditResultKind kind,
        string message,
        AnnotationObject? annotationObject = null,
        AnnotationDocument? document = null,
        Failure? failure = null) => new(
        kind,
        _state with { AnnotationRevision = CurrentRevision() },
        document ?? _documents.Current,
        annotationObject,
        failure,
        message);

    private AnnotationObjectEditResult MutationFailure(
        AnnotationMutationResult mutation,
        string message) => mutation switch
        {
            AnnotationMutationResult.ObjectNotFound => Result(
                AnnotationObjectEditResultKind.ObjectNotFound, message),
            AnnotationMutationResult.StaleSession => Result(
                AnnotationObjectEditResultKind.StaleSession, message),
            AnnotationMutationResult.StaleAnnotationRevision => Result(
                AnnotationObjectEditResultKind.StaleAnnotationRevision, message),
            AnnotationMutationResult.RevisionOverflow => Result(
                AnnotationObjectEditResultKind.RevisionOverflow, message),
            _ => Result(AnnotationObjectEditResultKind.Failed, message)
        };

    private static bool Contains(PhysicalRect bounds, PhysicalPoint point) =>
        point.X >= bounds.Left && point.X < bounds.Right
        && point.Y >= bounds.Top && point.Y < bounds.Bottom;

    private static double Distance(PhysicalPoint first, PhysicalPoint second) =>
        Math.Sqrt(Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2));

    private static double DistanceToSegment(PhysicalPoint point, PhysicalLineSegment segment)
    {
        var dx = segment.End.X - (double)segment.Start.X;
        var dy = segment.End.Y - (double)segment.Start.Y;
        if (dx == 0 && dy == 0)
        {
            return Distance(point, segment.Start);
        }

        var t = ((point.X - segment.Start.X) * dx + (point.Y - segment.Start.Y) * dy)
            / (dx * dx + dy * dy);
        t = Math.Clamp(t, 0, 1);
        var x = segment.Start.X + t * dx;
        var y = segment.Start.Y + t * dy;
        return Math.Sqrt(Math.Pow(point.X - x, 2) + Math.Pow(point.Y - y, 2));
    }

    private static PhysicalRect TranslateRect(PhysicalRect rect, long dx, long dy) => new(
        checked((int)(rect.Left + dx)),
        checked((int)(rect.Top + dy)),
        checked((int)(rect.Right + dx)),
        checked((int)(rect.Bottom + dy)));

    private static PhysicalPoint TranslatePoint(PhysicalPoint point, long dx, long dy) => new(
        checked((int)(point.X + dx)),
        checked((int)(point.Y + dy)));
}
