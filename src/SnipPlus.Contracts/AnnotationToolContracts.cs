namespace SnipPlus.Contracts;

public enum EditingToolKind
{
    Selection,
    Rectangle
}

public readonly record struct ArgbColor(byte A, byte R, byte G, byte B)
{
    public bool IsVisible => A > 0;

    public static ArgbColor Red => new(255, 220, 60, 60);
}

public sealed record RectangleAnnotationStyle
{
    public RectangleAnnotationStyle(ArgbColor strokeColor, int strokeThickness)
    {
        if (!strokeColor.IsVisible)
        {
            throw new ArgumentException(
                "Rectangle stroke color must have a visible alpha channel.",
                nameof(strokeColor));
        }

        if (strokeThickness is < 1 or > 64)
        {
            throw new ArgumentOutOfRangeException(
                nameof(strokeThickness),
                "Rectangle stroke thickness must be between 1 and 64 physical pixels.");
        }

        StrokeColor = strokeColor;
        StrokeThickness = strokeThickness;
    }

    public ArgbColor StrokeColor { get; }

    public int StrokeThickness { get; }

    public static RectangleAnnotationStyle Default => new(ArgbColor.Red, 2);
}

public sealed record RectangleAnnotationContent
{
    public RectangleAnnotationContent(RectangleAnnotationStyle style)
    {
        Style = style ?? throw new ArgumentNullException(nameof(style));
    }

    public RectangleAnnotationStyle Style { get; }
}

public sealed record EditingToolSelectionRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    EditingToolKind Tool);

public enum EditingToolSelectionResultKind
{
    Selected,
    StaleSession,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    InvalidWorkflowState,
    Failed
}

public sealed record EditingToolSelectionResult(
    EditingToolSelectionResultKind Kind,
    EditingToolKind ActiveTool,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    Failure? Failure,
    string Message);

public sealed record RectanglePointerEvent(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    int PointerId,
    PhysicalPoint GlobalPhysicalPoint);

public enum RectanglePointerResultKind
{
    DraftStarted,
    DraftUpdated,
    Committed,
    IgnoredOutsideSelection,
    InvalidGeometry,
    StaleSession,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    PointerMismatch,
    NoActiveDraft,
    Cancelled,
    Failed
}

public sealed record RectanglePointerResult(
    RectanglePointerResultKind Kind,
    EditingToolKind ActiveTool,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    PhysicalRect? DraftPhysicalBounds,
    AnnotationObject? CommittedObject,
    AnnotationDocument? Document,
    Failure? Failure,
    string Message);

public sealed record AnnotationPresentationSnapshot(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    PhysicalRect? SelectionPhysicalBounds,
    EditingToolKind ActiveTool,
    PhysicalRect? DraftPhysicalBounds,
    AnnotationDocument Document);

public interface IEditingToolSelectionSink
{
    EditingToolSelectionResult SelectTool(EditingToolSelectionRequest request);
}

public interface IAnnotationPointerInputSink
{
    RectanglePointerResult PointerPressed(RectanglePointerEvent input);

    RectanglePointerResult PointerMoved(RectanglePointerEvent input);

    RectanglePointerResult PointerReleased(RectanglePointerEvent input);
}

public interface IEditingInputRouter :
    ISelectionInputSink,
    IAnnotationPointerInputSink,
    IEditingToolSelectionSink
{
    EditingToolKind ActiveTool { get; }

    int CurrentSelectionRevision { get; }

    AnnotationRevision CurrentAnnotationRevision { get; }
}
