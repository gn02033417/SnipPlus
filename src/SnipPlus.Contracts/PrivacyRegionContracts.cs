namespace SnipPlus.Contracts;

public enum PrivacyRegionMode
{
    Mosaic,
    Blur
}

public sealed record PrivacyRegionEffectParameters
{
    public const int MinMosaicBlockSize = 2;
    public const int MaxMosaicBlockSize = 64;
    public const double MinBlurRadius = 1;
    public const double MaxBlurRadius = 32;

    public PrivacyRegionEffectParameters(int mosaicBlockSize, double blurRadius)
    {
        if (mosaicBlockSize is < MinMosaicBlockSize or > MaxMosaicBlockSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(mosaicBlockSize),
                $"Mosaic block size must be between {MinMosaicBlockSize} and {MaxMosaicBlockSize} pixels.");
        }

        if (!double.IsFinite(blurRadius)
            || blurRadius < MinBlurRadius
            || blurRadius > MaxBlurRadius)
        {
            throw new ArgumentOutOfRangeException(
                nameof(blurRadius),
                $"Blur radius must be finite and between {MinBlurRadius} and {MaxBlurRadius} pixels.");
        }

        MosaicBlockSize = mosaicBlockSize;
        BlurRadius = blurRadius;
    }

    public int MosaicBlockSize { get; }

    public double BlurRadius { get; }
}

public sealed record PrivacyRegionAnnotationContent : IAnnotationContent
{
    public PrivacyRegionAnnotationContent(
        PrivacyRegionMode mode,
        PrivacyRegionEffectParameters effectParameters)
    {
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        Mode = mode;
        EffectParameters = effectParameters
            ?? throw new ArgumentNullException(nameof(effectParameters));
    }

    public PrivacyRegionMode Mode { get; }

    public PrivacyRegionEffectParameters EffectParameters { get; }
}

public sealed record PrivacyRegionModeSelectionRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    PrivacyRegionMode Mode);

public enum PrivacyRegionModeSelectionResultKind
{
    Selected,
    InvalidMode,
    StaleSession,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    InvalidWorkflowState,
    InvalidEffectParameters,
    Failed
}

public sealed record PrivacyRegionModeSelectionResult(
    PrivacyRegionModeSelectionResultKind Kind,
    EditingToolKind ActiveTool,
    PrivacyRegionMode ActiveMode,
    PrivacyRegionEffectParameters ActiveEffectParameters,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    Failure? Failure,
    string Message);

public sealed record PrivacyRegionPointerEvent(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    int PointerId,
    PhysicalPoint GlobalPhysicalPoint);

public enum PrivacyRegionPointerResultKind
{
    DraftStarted,
    DraftUpdated,
    Committed,
    Cancelled,
    IgnoredOutsideSelection,
    InvalidGeometry,
    InvalidMode,
    InvalidEffectParameters,
    StaleSession,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    PointerMismatch,
    DraftMismatch,
    NoActiveDraft,
    RevisionOverflow,
    Failed
}

public sealed record PrivacyRegionPointerResult(
    PrivacyRegionPointerResultKind Kind,
    EditingToolKind ActiveTool,
    PrivacyRegionMode ActiveMode,
    PrivacyRegionEffectParameters ActiveEffectParameters,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision AnnotationRevision,
    Guid? DraftId,
    PhysicalRect? DraftPhysicalBounds,
    AnnotationObject? CommittedObject,
    AnnotationDocument? Document,
    Failure? Failure,
    string Message);
