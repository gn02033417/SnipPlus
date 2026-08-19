namespace SnipPlus.Contracts;

public sealed record PngWriteSucceededEvidence
{
    public required Guid SessionId { get; init; }

    public required Guid ResultId { get; init; }
}

public abstract record OutputCommitmentAuthorization
{
    private OutputCommitmentAuthorization()
    {
    }

    public required Guid SessionId { get; init; }

    public required string CoordinateVersion { get; init; }

    public required int SelectionRevision { get; init; }

    public required AnnotationRevision AnnotationRevision { get; init; }

    public required Guid ResultId { get; init; }

    public sealed record Complete : OutputCommitmentAuthorization;

    public sealed record SuccessfulSave : OutputCommitmentAuthorization
    {
        public required PngWriteSucceededEvidence PngWrite { get; init; }
    }

    public static Complete CreateComplete(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationRevision annotationRevision,
        Guid resultId) => new()
        {
            SessionId = sessionId,
            CoordinateVersion = coordinateVersion,
            SelectionRevision = selectionRevision,
            AnnotationRevision = annotationRevision,
            ResultId = resultId
        };

    public static SuccessfulSave CreateSuccessfulSave(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationRevision annotationRevision,
        Guid resultId,
        PngWriteSucceededEvidence pngWrite) => new()
        {
            SessionId = sessionId,
            CoordinateVersion = coordinateVersion,
            SelectionRevision = selectionRevision,
            AnnotationRevision = annotationRevision,
            ResultId = resultId,
            PngWrite = pngWrite
        };
}

public sealed record OutputCommitmentRequest
{
    public required OutputCommitmentAuthorization Authorization { get; init; }

    public required IImageResult ImageResult { get; init; }

    public required WorkflowState WorkflowState { get; init; }

    public required int SelectionWidth { get; init; }

    public required int SelectionHeight { get; init; }

    public required int DisplayCount { get; init; }

    public CancellationToken Cancellation { get; init; }
}

public interface IOutputCommitmentCoordinator
{
    ValueTask<ClipboardDeliveryResult> PublishAsync(
        OutputCommitmentRequest request,
        CancellationToken cancellationToken);
}
