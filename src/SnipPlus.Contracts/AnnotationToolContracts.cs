namespace SnipPlus.Contracts;

public enum EditingToolKind
{
    Selection,
    Rectangle,
    ArrowLine,
    Highlighter,
    Text,
    PrivacyRegion
}

public readonly record struct ArgbColor(byte A, byte R, byte G, byte B)
{
    public bool IsVisible => A > 0;

    public static ArgbColor Red => new(255, 220, 60, 60);
}

public interface IAnnotationContent
{
}

public enum ArrowLineEndStyle
{
    Arrow,
    None
}

public readonly record struct PhysicalLineSegment(PhysicalPoint Start, PhysicalPoint End)
{
    public bool IsPositive => Start != End;

    public PhysicalRect Bounds
    {
        get
        {
            var left = Math.Min(Start.X, End.X);
            var top = Math.Min(Start.Y, End.Y);
            var right = Math.Max(Start.X, End.X);
            var bottom = Math.Max(Start.Y, End.Y);
            return new PhysicalRect(
                left,
                top,
                right > left ? right : right + 1,
                bottom > top ? bottom : bottom + 1);
        }
    }
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

public sealed record RectangleAnnotationContent : IAnnotationContent
{
    public RectangleAnnotationContent(RectangleAnnotationStyle style)
    {
        Style = style ?? throw new ArgumentNullException(nameof(style));
    }

    public RectangleAnnotationStyle Style { get; }
}

public sealed record ArrowLineAnnotationStyle
{
    public ArrowLineAnnotationStyle(
        ArgbColor strokeColor,
        int strokeThickness,
        ArrowLineEndStyle endStyle)
    {
        if (!strokeColor.IsVisible)
        {
            throw new ArgumentException(
                "Arrow or line stroke color must have a visible alpha channel.",
                nameof(strokeColor));
        }

        if (strokeThickness is < 1 or > 64)
        {
            throw new ArgumentOutOfRangeException(
                nameof(strokeThickness),
                "Arrow or line stroke thickness must be between 1 and 64 physical pixels.");
        }

        if (!Enum.IsDefined(endStyle))
        {
            throw new ArgumentOutOfRangeException(nameof(endStyle));
        }

        StrokeColor = strokeColor;
        StrokeThickness = strokeThickness;
        EndStyle = endStyle;
    }

    public ArgbColor StrokeColor { get; }

    public int StrokeThickness { get; }

    public ArrowLineEndStyle EndStyle { get; init; }

    public static ArrowLineAnnotationStyle Default => new(
        ArgbColor.Red,
        2,
        ArrowLineEndStyle.Arrow);
}

public sealed record ArrowLineAnnotationContent : IAnnotationContent
{
    public ArrowLineAnnotationContent(
        PhysicalLineSegment segment,
        ArrowLineAnnotationStyle style)
    {
        if (!segment.IsPositive)
        {
            throw new ArgumentException(
                "Arrow or line geometry must have distinct endpoints.",
                nameof(segment));
        }

        Segment = segment;
        Style = style ?? throw new ArgumentNullException(nameof(style));
    }

    public PhysicalLineSegment Segment { get; }

    public ArrowLineAnnotationStyle Style { get; }
}

public sealed record PhysicalPolyline
{
    public PhysicalPolyline(IEnumerable<PhysicalPoint> points)
    {
        ArgumentNullException.ThrowIfNull(points);
        var materialized = points.ToArray();
        if (materialized.Length == 0)
        {
            throw new ArgumentException(
                "A physical polyline must contain at least one point.",
                nameof(points));
        }

        Points = Array.AsReadOnly(materialized);
    }

    public IReadOnlyList<PhysicalPoint> Points { get; }

    public bool HasLength => Points.Count > 1
        && Points.Zip(Points.Skip(1), static (first, second) => first != second).Any(value => value);

    public PhysicalRect Bounds
    {
        get
        {
            var left = Points.Min(point => point.X);
            var top = Points.Min(point => point.Y);
            var right = Points.Max(point => point.X);
            var bottom = Points.Max(point => point.Y);
            return new PhysicalRect(
                left,
                top,
                right > left ? right : right + 1,
                bottom > top ? bottom : bottom + 1);
        }
    }
}

public sealed record HighlighterAnnotationStyle
{
    public HighlighterAnnotationStyle(ArgbColor strokeColor, int strokeThickness)
    {
        if (strokeColor.A is 0 or 255)
        {
            throw new ArgumentException(
                "Highlighter stroke color must be visible and semi-transparent.",
                nameof(strokeColor));
        }

        if (strokeThickness is < 1 or > 64)
        {
            throw new ArgumentOutOfRangeException(
                nameof(strokeThickness),
                "Highlighter stroke thickness must be between 1 and 64 physical pixels.");
        }

        StrokeColor = strokeColor;
        StrokeThickness = strokeThickness;
    }

    public ArgbColor StrokeColor { get; }

    public int StrokeThickness { get; }

    public static HighlighterAnnotationStyle Default => new(
        new ArgbColor(128, 255, 235, 59),
        8);
}

public sealed record HighlighterStrokeContent : IAnnotationContent
{
    public HighlighterStrokeContent(
        PhysicalPolyline path,
        HighlighterAnnotationStyle style)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
        if (!path.HasLength)
        {
            throw new ArgumentException(
                "A Highlighter stroke must contain at least two distinct points.",
                nameof(path));
        }

        Style = style ?? throw new ArgumentNullException(nameof(style));
    }

    public PhysicalPolyline Path { get; }

    public HighlighterAnnotationStyle Style { get; }
}

public sealed record EditingToolSelectionRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    EditingToolKind Tool)
{
    public ArrowLineEndStyle RequestedArrowLineEndStyle { get; init; } = ArrowLineEndStyle.Arrow;

    public PrivacyRegionMode? RequestedPrivacyRegionMode { get; init; }
}

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
    string Message)
{
    public ArrowLineEndStyle ActiveArrowLineEndStyle { get; init; } = ArrowLineEndStyle.Arrow;

    public TextAnnotationStyle ActiveTextStyle { get; init; } = TextAnnotationStyle.Default;

    public PrivacyRegionMode? ActivePrivacyRegionMode { get; init; }

    public PrivacyRegionEffectParameters? ActivePrivacyRegionEffectParameters { get; init; }
}

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

public sealed record ArrowLinePointerEvent(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    int PointerId,
    PhysicalPoint GlobalPhysicalPoint);

public enum ArrowLinePointerResultKind
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

public sealed record ArrowLinePointerResult(
    ArrowLinePointerResultKind Kind,
    EditingToolKind ActiveTool,
    ArrowLineEndStyle ActiveEndStyle,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    PhysicalLineSegment? DraftSegment,
    AnnotationObject? CommittedObject,
    AnnotationDocument? Document,
    Failure? Failure,
    string Message);

public sealed record HighlighterPointerEvent(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    int PointerId,
    PhysicalPoint GlobalPhysicalPoint);

public enum HighlighterPointerResultKind
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

public sealed record HighlighterPointerResult(
    HighlighterPointerResultKind Kind,
    EditingToolKind ActiveTool,
    HighlighterAnnotationStyle ActiveStyle,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    IReadOnlyList<PhysicalPoint>? DraftPoints,
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
    AnnotationDocument Document)
{
    public ArrowLineEndStyle ActiveArrowLineEndStyle { get; init; } = ArrowLineEndStyle.Arrow;

    public PhysicalLineSegment? DraftArrowLineSegment { get; init; }

    public HighlighterAnnotationStyle ActiveHighlighterStyle { get; init; } = HighlighterAnnotationStyle.Default;

    public IReadOnlyList<PhysicalPoint>? DraftHighlighterPoints { get; init; }

    public TextAnnotationStyle ActiveTextStyle { get; init; } = TextAnnotationStyle.Default;

    public PrivacyRegionMode? ActivePrivacyRegionMode { get; init; }

    public PrivacyRegionEffectParameters? ActivePrivacyRegionEffectParameters { get; init; }

    public TextDraftPresentation? DraftText { get; init; }

    public PrivacyRegionMode? DraftPrivacyRegionMode { get; init; }

    public PrivacyRegionEffectParameters? DraftPrivacyRegionEffectParameters { get; init; }

    public PhysicalRect? DraftPrivacyRegionBounds { get; init; }
}

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

public interface IArrowLinePointerInputSink
{
    ArrowLinePointerResult PointerPressed(ArrowLinePointerEvent input);

    ArrowLinePointerResult PointerMoved(ArrowLinePointerEvent input);

    ArrowLinePointerResult PointerReleased(ArrowLinePointerEvent input);
}

public interface IHighlighterPointerInputSink
{
    HighlighterPointerResult PointerPressed(HighlighterPointerEvent input);

    HighlighterPointerResult PointerMoved(HighlighterPointerEvent input);

    HighlighterPointerResult PointerReleased(HighlighterPointerEvent input);
}

public interface IPrivacyRegionPointerInputSink
{
    PrivacyRegionPointerResult PointerPressed(PrivacyRegionPointerEvent input);

    PrivacyRegionPointerResult PointerMoved(PrivacyRegionPointerEvent input);

    PrivacyRegionPointerResult PointerReleased(PrivacyRegionPointerEvent input);
}

public interface IPrivacyRegionModeSelectionSink
{
    PrivacyRegionModeSelectionResult SelectPrivacyRegionMode(
        PrivacyRegionModeSelectionRequest request);
}

public interface IEditingInputRouter :
    ISelectionInputSink,
    IAnnotationPointerInputSink,
    IArrowLinePointerInputSink,
    IHighlighterPointerInputSink,
    IPrivacyRegionPointerInputSink,
    IPrivacyRegionModeSelectionSink,
    ITextDraftInputSink,
    IEditingToolSelectionSink
{
    EditingToolKind ActiveTool { get; }

    int CurrentSelectionRevision { get; }

    AnnotationRevision CurrentAnnotationRevision { get; }

    ArrowLineEndStyle ActiveArrowLineEndStyle { get; }

    HighlighterAnnotationStyle ActiveHighlighterStyle { get; }

    TextAnnotationStyle ActiveTextStyle { get; }

    PrivacyRegionMode ActivePrivacyRegionMode { get; }

    PrivacyRegionEffectParameters ActivePrivacyRegionEffectParameters { get; }
}
