namespace SnipPlus.Contracts;

public enum AnnotationObjectEditOperationKind
{
    None,
    Move,
    Resize,
    TextEdit
}

public enum AnnotationObjectEditHandleKind
{
    Body,
    StartEndpoint,
    EndEndpoint,
    LeftEdge,
    TopEdge,
    RightEdge,
    BottomEdge,
    TopLeftCorner,
    TopRightCorner,
    BottomLeftCorner,
    BottomRightCorner
}

public enum AnnotationObjectEditResultKind
{
    Selected,
    SelectionCleared,
    EditStarted,
    EditUpdated,
    EditCommitted,
    EditCancelled,
    Deleted,
    Restyled,
    TextEditStarted,
    TextEditCommitted,
    ObjectNotFound,
    UnsupportedOperation,
    IncompatibleStyle,
    InvalidGeometry,
    InvalidStyle,
    EmptyText,
    StaleSession,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    StaleObject,
    PointerMismatch,
    DraftMismatch,
    NoSelectedObject,
    NoActiveEdit,
    RevisionOverflow,
    ZOrderOverflow,
    Failed
}

public sealed record AnnotationObjectSelectionState(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    AnnotationObjectId? SelectedObjectId,
    AnnotationObjectEditOperationKind Operation,
    AnnotationObjectEditHandleKind? ActiveHandle,
    int? ActivePointerId,
    AnnotationObject? OriginalObject,
    AnnotationObject? PreviewObject,
    Guid? TextEditDraftId)
{
    public bool HasSelection => SelectedObjectId is not null;

    public bool HasActiveEdit => Operation != AnnotationObjectEditOperationKind.None;

    public static AnnotationObjectSelectionState Empty(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationRevision annotationRevision) => new(
            sessionId,
            coordinateVersion,
            selectionRevision,
            annotationRevision,
            null,
            AnnotationObjectEditOperationKind.None,
            null,
            null,
            null,
            null,
            null);
}

public sealed record AnnotationObjectPointerEvent(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    int PointerId,
    PhysicalPoint GlobalPhysicalPoint);

public sealed record AnnotationObjectSelectionRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    AnnotationObjectId ObjectId);

public sealed record AnnotationObjectStyleChange(
    ArgbColor? Color = null,
    int? Thickness = null,
    double? FontSize = null,
    bool? Bold = null,
    int? MarkerSize = null,
    ArrowLineEndStyle? ArrowLineEndStyle = null,
    PrivacyRegionMode? PrivacyMode = null,
    PrivacyRegionEffectParameters? PrivacyEffectParameters = null);

public sealed record AnnotationObjectStyleChangeRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    AnnotationObjectId? ObjectId,
    AnnotationObjectStyleChange Change);

public sealed record AnnotationObjectDeleteRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    AnnotationObjectId ObjectId);

public sealed record AnnotationObjectTextEditRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    AnnotationObjectId ObjectId,
    Guid DraftId,
    string Text);

#pragma warning disable CA1720
public sealed record AnnotationObjectEditResult(
    AnnotationObjectEditResultKind Kind,
    AnnotationObjectSelectionState State,
    AnnotationDocument? Document,
    AnnotationObject? Object,
    Failure? Failure,
    string Message);
#pragma warning restore CA1720

public interface IAnnotationObjectEditingSink
{
    AnnotationObjectEditResult PointerPressed(AnnotationObjectPointerEvent input);

    AnnotationObjectEditResult PointerMoved(AnnotationObjectPointerEvent input);

    AnnotationObjectEditResult PointerReleased(AnnotationObjectPointerEvent input);

    AnnotationObjectEditResult SelectObject(AnnotationObjectSelectionRequest request);

    AnnotationObjectEditResult ChangeStyle(AnnotationObjectStyleChangeRequest request);

    AnnotationObjectEditResult Delete(AnnotationObjectDeleteRequest request);

    AnnotationObjectEditResult BeginTextEdit(AnnotationObjectSelectionRequest request);

    AnnotationObjectEditResult UpdateTextEdit(AnnotationObjectTextEditRequest request);

    AnnotationObjectEditResult CommitTextEdit(AnnotationObjectTextEditRequest request);

    AnnotationObjectEditResult CancelEdit(Guid sessionId, string coordinateVersion);
}
