namespace SnipPlus.Contracts;

public sealed record ClipboardDeliveryRequest
{
    public required Guid DeliveryId { get; init; }
    public required Guid SessionId { get; init; }
    public required Guid ResultId { get; init; }
    public required IImageResult ImageResult { get; init; }
    public bool HistoryAllowed { get; init; }
    public bool RoamingAllowed { get; init; }
    public int MaximumAttempts { get; init; } = 5;
    public TimeSpan RetryBudget { get; init; } = TimeSpan.FromSeconds(1);
    public CancellationToken Cancellation { get; init; }
}

public abstract record ClipboardDeliveryResult(Guid DeliveryId, Guid SessionId, Guid ResultId)
{
    public sealed record Delivered(Guid DeliveryId, Guid SessionId, Guid ResultId, int Attempts)
        : ClipboardDeliveryResult(DeliveryId, SessionId, ResultId);

    public sealed record RetryableFailure(
        Guid DeliveryId,
        Guid SessionId,
        Guid ResultId,
        Failure Failure,
        int AttemptsUsed) : ClipboardDeliveryResult(DeliveryId, SessionId, ResultId);

    public sealed record TerminalFailure(
        Guid DeliveryId,
        Guid SessionId,
        Guid ResultId,
        Failure Failure) : ClipboardDeliveryResult(DeliveryId, SessionId, ResultId);

    public sealed record Cancelled(Guid DeliveryId, Guid SessionId, Guid ResultId, string CancellationOrigin)
        : ClipboardDeliveryResult(DeliveryId, SessionId, ResultId);
}

public interface IClipboardDeliveryService
{
    ValueTask<ClipboardDeliveryResult> DeliverAsync(
        ClipboardDeliveryRequest request,
        CancellationToken cancellationToken);
}

public sealed record OutputDeliveryRequest
{
    public required Guid DeliveryId { get; init; }
    public required Guid SessionId { get; init; }
    public required Guid ResultId { get; init; }
    public required IImageResult ImageResult { get; init; }
    public string Format { get; init; } = "Png";
    public string? DestinationIdentifier { get; init; }
    public CancellationToken Cancellation { get; init; }
}

public abstract record OutputDeliveryResult(Guid DeliveryId, Guid SessionId, Guid ResultId)
{
    public sealed record Delivered(Guid DeliveryId, Guid SessionId, Guid ResultId, string DestinationIdentifier)
        : OutputDeliveryResult(DeliveryId, SessionId, ResultId);

    public sealed record RetryableFailure(Guid DeliveryId, Guid SessionId, Guid ResultId, Failure Failure)
        : OutputDeliveryResult(DeliveryId, SessionId, ResultId);

    public sealed record TerminalFailure(Guid DeliveryId, Guid SessionId, Guid ResultId, Failure Failure)
        : OutputDeliveryResult(DeliveryId, SessionId, ResultId);

    public sealed record Cancelled(Guid DeliveryId, Guid SessionId, Guid ResultId, string CancellationOrigin)
        : OutputDeliveryResult(DeliveryId, SessionId, ResultId);
}

public interface IOutputDeliveryService
{
    ValueTask<OutputDeliveryResult> DeliverAsync(
        OutputDeliveryRequest request,
        CancellationToken cancellationToken);
}
