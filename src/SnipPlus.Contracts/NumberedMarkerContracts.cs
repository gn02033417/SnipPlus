namespace SnipPlus.Contracts;

public sealed record NumberedMarkerDraftPresentation(
    int Number,
    PhysicalPoint Center,
    PhysicalRect Bounds,
    NumberedMarkerAnnotationStyle Style,
    bool IsDraft = true);

public sealed record NumberedMarkerPointerEvent(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    int PointerId,
    PhysicalPoint GlobalPhysicalPoint);

public enum NumberedMarkerPointerResultKind
{
    DraftStarted,
    DraftUpdated,
    Committed,
    IgnoredOutsideSelection,
    InvalidNumber,
    InvalidStyle,
    StaleSession,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    PointerMismatch,
    DraftMismatch,
    NoActiveDraft,
    NumberOverflow,
    ZOrderOverflow,
    Cancelled,
    Failed
}

public sealed record NumberedMarkerPointerResult(
    NumberedMarkerPointerResultKind Kind,
    EditingToolKind ActiveTool,
    int ActiveNextNumber,
    NumberedMarkerAnnotationStyle ActiveStyle,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    NumberedMarkerDraftPresentation? Draft,
    AnnotationObject? CommittedObject,
    AnnotationDocument? Document,
    Failure? Failure,
    string Message);

public sealed record SetNextNumberRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    int Number);

public enum SetNextNumberResultKind
{
    Succeeded,
    NoChange,
    InvalidNumber,
    StaleSession,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    InvalidWorkflowState,
    Failed
}

public sealed record SetNextNumberResult(
    SetNextNumberResultKind Kind,
    EditingToolKind ActiveTool,
    int ActiveNextNumber,
    NumberedMarkerAnnotationStyle ActiveStyle,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    Failure? Failure,
    string Message);
