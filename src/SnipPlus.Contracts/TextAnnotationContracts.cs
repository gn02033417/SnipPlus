namespace SnipPlus.Contracts;

public sealed record TextAnnotationStyle
{
    public const string DefaultFontFamily = "Microsoft JhengHei";
    public const double MinFontSize = 8;
    public const double MaxFontSize = 144;

    public TextAnnotationStyle(
        string fontFamily,
        double fontSize,
        ArgbColor color,
        bool bold)
    {
        if (string.IsNullOrWhiteSpace(fontFamily))
        {
            throw new ArgumentException("Text font family is required.", nameof(fontFamily));
        }

        if (!double.IsFinite(fontSize)
            || fontSize < MinFontSize
            || fontSize > MaxFontSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fontSize),
                $"Text font size must be finite and between {MinFontSize} and {MaxFontSize} DIPs.");
        }

        if (!color.IsVisible)
        {
            throw new ArgumentException(
                "Text color must have a visible alpha channel.",
                nameof(color));
        }

        FontFamily = fontFamily.Trim();
        FontSize = fontSize;
        Color = color;
        Bold = bold;
    }

    public string FontFamily { get; }

    public double FontSize { get; }

    public ArgbColor Color { get; }

    public bool Bold { get; }

    public static TextAnnotationStyle Default => new(
        DefaultFontFamily,
        16,
        ArgbColor.Red,
        false);
}

public sealed record TextAnnotationContent : IAnnotationContent
{
    public TextAnnotationContent(
        string text,
        PhysicalPoint anchorInVirtualDesktop,
        PhysicalRect boundsInVirtualDesktop,
        TextAnnotationStyle style)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text annotation content cannot be empty.", nameof(text));
        }

        if (!boundsInVirtualDesktop.IsPositive)
        {
            throw new ArgumentException(
                "Text annotation bounds must be positive.",
                nameof(boundsInVirtualDesktop));
        }

        if (!boundsInVirtualDesktop.Contains(new PhysicalRect(
                anchorInVirtualDesktop.X,
                anchorInVirtualDesktop.Y,
                anchorInVirtualDesktop.X + 1,
                anchorInVirtualDesktop.Y + 1)))
        {
            throw new ArgumentException(
                "Text annotation anchor must be inside its bounds.",
                nameof(anchorInVirtualDesktop));
        }

        Text = Normalize(text);
        AnchorInVirtualDesktop = anchorInVirtualDesktop;
        BoundsInVirtualDesktop = boundsInVirtualDesktop;
        Style = style ?? throw new ArgumentNullException(nameof(style));
    }

    public string Text { get; }

    public PhysicalPoint AnchorInVirtualDesktop { get; }

    public PhysicalRect BoundsInVirtualDesktop { get; }

    public TextAnnotationStyle Style { get; }

    public static string Normalize(string text) =>
        (text ?? throw new ArgumentNullException(nameof(text)))
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
}

public sealed record TextDraftPointerEvent(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    int PointerId,
    PhysicalPoint GlobalPhysicalPoint)
{
    public Guid DraftId { get; init; }
}

public sealed record TextDraftRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    Guid DraftId,
    PhysicalPoint AnchorInVirtualDesktop,
    PhysicalRect BoundsInVirtualDesktop);

public enum TextDraftResultKind
{
    DraftStarted,
    DraftUpdated,
    Committed,
    Cancelled,
    IgnoredOutsideSelection,
    EmptyText,
    InvalidStyle,
    InvalidGeometry,
    StaleSession,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    DraftMismatch,
    NoActiveDraft,
    RevisionOverflow,
    Failed
}

public sealed record TextDraftResult(
    TextDraftResultKind Kind,
    EditingToolKind ActiveTool,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    TextDraftRequest? Request,
    string Text,
    TextAnnotationStyle ActiveStyle,
    AnnotationObject? CommittedObject,
    AnnotationDocument? Document,
    Failure? Failure,
    string Message);

public sealed record TextDraftPresentation(
    Guid DraftId,
    string Text,
    PhysicalPoint AnchorInVirtualDesktop,
    PhysicalRect BoundsInVirtualDesktop,
    TextAnnotationStyle Style);

public interface ITextDraftInputSink
{
    TextDraftResult BeginTextDraft(TextDraftPointerEvent input);

    TextDraftResult UpdateTextDraftContent(TextDraftRequest request, string text);

    TextDraftResult UpdateTextDraftStyle(TextDraftRequest request, TextAnnotationStyle? style);

    TextDraftResult CommitTextDraft(TextDraftRequest request);

    TextDraftResult CancelTextDraft(TextDraftRequest request);
}
