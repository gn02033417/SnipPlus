namespace SnipPlus.Contracts;

public sealed record PngSaveRequest
{
    public required Guid SessionId { get; init; }

    public required Guid ResultId { get; init; }

    public required IImageResult ImageResult { get; init; }

    public required string SuggestedFileName { get; init; }
}

public abstract record PngSaveResult(Guid SessionId, Guid ResultId)
{
    public sealed record Saved(
        Guid SessionId,
        Guid ResultId,
        string DestinationIdentifier,
        PngWriteSucceededEvidence PngWrite)
        : PngSaveResult(SessionId, ResultId);

    public sealed record SaveDialogCancelled(
        Guid SessionId,
        Guid ResultId,
        string CancellationOrigin)
        : PngSaveResult(SessionId, ResultId);

    public sealed record SaveDialogFailed(
        Guid SessionId,
        Guid ResultId,
        Failure Failure)
        : PngSaveResult(SessionId, ResultId);

    public sealed record PngEncodingFailed(
        Guid SessionId,
        Guid ResultId,
        Failure Failure)
        : PngSaveResult(SessionId, ResultId);

    public sealed record PngWriteFailed(
        Guid SessionId,
        Guid ResultId,
        Failure Failure)
        : PngSaveResult(SessionId, ResultId);

    public sealed record Cancelled(
        Guid SessionId,
        Guid ResultId,
        string CancellationOrigin)
        : PngSaveResult(SessionId, ResultId);

    public sealed record Rejected(
        Guid SessionId,
        Guid ResultId,
        Failure Failure)
        : PngSaveResult(SessionId, ResultId);
}

public interface IPngSavePlatformService
{
    ValueTask<PngSaveResult> SaveAsync(
        PngSaveRequest request,
        CancellationToken cancellationToken);
}
