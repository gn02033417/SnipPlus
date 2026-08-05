namespace SnipPlus.Contracts;

public sealed record AnnotationAwareRenderRequest
{
    public required Guid SessionId { get; init; }

    public required string CoordinateVersion { get; init; }

    public required int SelectionRevision { get; init; }

    public required AnnotationRevision AnnotationRevision { get; init; }

    public required PhysicalRect SelectionPhysicalBounds { get; init; }

    public required VirtualDesktopSnapshot VirtualDesktopSnapshot { get; init; }

    public required CapacityValidationOutcome CapacityValidation { get; init; }

    public required FrozenDisplayFrameSet FrozenDisplayFrames { get; init; }

    public required AnnotationDocument AnnotationDocument { get; init; }

    public CancellationToken Cancellation { get; init; }
}

public enum AnnotationAwareRenderOutcomeKind
{
    Succeeded,
    Cancelled,
    StaleSession,
    StaleCoordinateVersion,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    InvalidSelection,
    InvalidFrameSet,
    InvalidAnnotationDocument,
    UnsupportedAnnotation,
    RenderCapacityExceeded,
    Failed
}

public sealed class AnnotationAwareRenderResult : IDisposable
{
    private IImageResult? _imageResult;

    public AnnotationAwareRenderResult(
        Guid resultId,
        Guid sessionId,
        int selectionRevision,
        AnnotationRevision annotationRevision,
        IImageResult imageResult,
        int renderedObjectCount,
        int transparentGapPixelCount)
    {
        if (resultId == Guid.Empty)
        {
            throw new ArgumentException("Render result identifier is required.", nameof(resultId));
        }

        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Render result session identifier is required.", nameof(sessionId));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(selectionRevision);

        if (!annotationRevision.IsValid)
        {
            throw new ArgumentException("Render result annotation revision is invalid.", nameof(annotationRevision));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(renderedObjectCount);
        ArgumentOutOfRangeException.ThrowIfNegative(transparentGapPixelCount);

        _imageResult = imageResult ?? throw new ArgumentNullException(nameof(imageResult));
        ResultId = resultId;
        SessionId = sessionId;
        SelectionRevision = selectionRevision;
        AnnotationRevision = annotationRevision;
        PixelWidth = imageResult.Metadata.PixelWidth;
        PixelHeight = imageResult.Metadata.PixelHeight;
        RenderedObjectCount = renderedObjectCount;
        TransparentGapPixelCount = transparentGapPixelCount;
    }

    public Guid ResultId { get; }

    public Guid SessionId { get; }

    public int SelectionRevision { get; }

    public AnnotationRevision AnnotationRevision { get; }

    public int PixelWidth { get; }

    public int PixelHeight { get; }

    public int RenderedObjectCount { get; }

    public int TransparentGapPixelCount { get; }

    public bool HasTransparentGap => TransparentGapPixelCount > 0;

    public IImageResult ImageResult =>
        _imageResult ?? throw new ObjectDisposedException(nameof(AnnotationAwareRenderResult));

    public void Dispose() => Interlocked.Exchange(ref _imageResult, null)?.Dispose();
}

public abstract record AnnotationAwareRenderOutcome(
    Guid SessionId,
    AnnotationAwareRenderOutcomeKind Kind)
{
    public sealed record Succeeded(
        AnnotationAwareRenderResult Result)
        : AnnotationAwareRenderOutcome(
            Result.SessionId,
            AnnotationAwareRenderOutcomeKind.Succeeded);

    public sealed record Cancelled(
        Guid SessionId,
        string CancellationOrigin)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.Cancelled);

    public sealed record StaleSession(
        Guid SessionId,
        Guid ActiveSessionId,
        string Message)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.StaleSession);

    public sealed record StaleCoordinateVersion(
        Guid SessionId,
        string RequestedCoordinateVersion,
        string CurrentCoordinateVersion,
        string Message)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.StaleCoordinateVersion);

    public sealed record StaleSelectionRevision(
        Guid SessionId,
        int RequestedSelectionRevision,
        int CurrentSelectionRevision,
        string Message)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.StaleSelectionRevision);

    public sealed record StaleAnnotationRevision(
        Guid SessionId,
        AnnotationRevision RequestedAnnotationRevision,
        AnnotationRevision CurrentAnnotationRevision,
        string Message)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.StaleAnnotationRevision);

    public sealed record InvalidSelection(
        Guid SessionId,
        string Message)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.InvalidSelection);

    public sealed record InvalidFrameSet(
        Guid SessionId,
        string Message)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.InvalidFrameSet);

    public sealed record InvalidAnnotationDocument(
        Guid SessionId,
        string Message)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.InvalidAnnotationDocument);

    public sealed record UnsupportedAnnotation(
        Guid SessionId,
        AnnotationToolKind ToolKind,
        string Message)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.UnsupportedAnnotation);

    public sealed record RenderCapacityExceeded(
        Guid SessionId,
        string Message)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.RenderCapacityExceeded);

    public sealed record Failed(
        Guid SessionId,
        Failure Failure)
        : AnnotationAwareRenderOutcome(SessionId, AnnotationAwareRenderOutcomeKind.Failed);
}

public interface IAnnotationAwareRenderAdapter
{
    ValueTask<AnnotationAwareRenderOutcome> RenderAsync(
        AnnotationAwareRenderRequest request,
        CancellationToken cancellationToken);
}
