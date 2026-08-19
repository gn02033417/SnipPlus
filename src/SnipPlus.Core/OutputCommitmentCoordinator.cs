using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class OutputCommitmentCoordinator : IOutputCommitmentCoordinator
{
    private readonly IClipboardDeliveryService _clipboardDelivery;

    public OutputCommitmentCoordinator(IClipboardDeliveryService clipboardDelivery)
    {
        _clipboardDelivery = clipboardDelivery
            ?? throw new ArgumentNullException(nameof(clipboardDelivery));
    }

    public async ValueTask<ClipboardDeliveryResult> PublishAsync(
        OutputCommitmentRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var deliveryId = Guid.NewGuid();
        var validationFailure = Validate(request, deliveryId);
        if (validationFailure is not null)
        {
            return new ClipboardDeliveryResult.TerminalFailure(
                deliveryId,
                request.Authorization?.SessionId ?? Guid.Empty,
                request.Authorization?.ResultId ?? Guid.Empty,
                validationFailure);
        }

        var deliveryRequest = new ClipboardDeliveryRequest
        {
            DeliveryId = deliveryId,
            SessionId = request.Authorization.SessionId,
            ResultId = request.Authorization.ResultId,
            ImageResult = request.ImageResult,
            SelectionWidth = request.SelectionWidth,
            SelectionHeight = request.SelectionHeight,
            DisplayCount = request.DisplayCount,
            HistoryAllowed = false,
            RoamingAllowed = false,
            MaximumAttempts = 5,
            RetryBudget = TimeSpan.FromSeconds(1),
            Cancellation = request.Cancellation
        };

        try
        {
            var result = await _clipboardDelivery
                .DeliverAsync(deliveryRequest, cancellationToken)
                .ConfigureAwait(false);

            return result is null || !HasMatchingIdentity(result, deliveryRequest)
                ? new ClipboardDeliveryResult.TerminalFailure(
                    deliveryId,
                    deliveryRequest.SessionId,
                    deliveryRequest.ResultId,
                    CreateFailure(
                        FailureCode.ClipboardPublicationRejected,
                        deliveryId,
                        "The Clipboard delivery returned a mismatched result identity."))
                : result;
        }
        catch (OperationCanceledException)
        {
            return new ClipboardDeliveryResult.Cancelled(
                deliveryId,
                deliveryRequest.SessionId,
                deliveryRequest.ResultId,
                "CancellationToken");
        }
        catch (Exception exception)
        {
            return new ClipboardDeliveryResult.TerminalFailure(
                deliveryId,
                deliveryRequest.SessionId,
                deliveryRequest.ResultId,
                CreateFailure(
                    FailureCode.UnexpectedFailure,
                    deliveryId,
                    exception.GetType().Name,
                    exception.HResult));
        }
    }

    private static Failure? Validate(
        OutputCommitmentRequest request,
        Guid deliveryId)
    {
        if (request.Authorization is null || request.ImageResult is null)
        {
            return CreateFailure(
                FailureCode.ClipboardPublicationRejected,
                deliveryId,
                "Clipboard publication requires an explicit output commitment and image result.");
        }

        if (request.WorkflowState != WorkflowState.Delivering)
        {
            return CreateFailure(
                FailureCode.InvalidStateTransition,
                deliveryId,
                "Clipboard publication requires the Delivering workflow state.");
        }

        if (request.ImageResult.IsDisposed)
        {
            return CreateFailure(
                FailureCode.InvalidResultLifetime,
                deliveryId,
                "Clipboard publication cannot use a disposed image result.");
        }

        if (request.SelectionWidth <= 0
            || request.SelectionHeight <= 0
            || request.DisplayCount <= 0)
        {
            return CreateFailure(
                FailureCode.ClipboardPublicationRejected,
                deliveryId,
                "Clipboard publication requires positive Selection dimensions and display count.");
        }

        var authorization = request.Authorization;
        var metadata = request.ImageResult.Metadata;
        if (authorization.SessionId == Guid.Empty
            || authorization.ResultId == Guid.Empty
            || string.IsNullOrWhiteSpace(authorization.CoordinateVersion)
            || authorization.SelectionRevision < 0
            || !authorization.AnnotationRevision.IsValid
            || metadata.SessionId != authorization.SessionId
            || metadata.ResultId != authorization.ResultId
            || metadata.SelectionRevision != authorization.SelectionRevision
            || metadata.AnnotationRevision != authorization.AnnotationRevision)
        {
            return CreateFailure(
                FailureCode.ClipboardPublicationRejected,
                deliveryId,
                "Clipboard publication authorization does not match the canonical image result identity.");
        }

        if (authorization is OutputCommitmentAuthorization.SuccessfulSave save
            && (save.PngWrite is null
                || save.PngWrite.SessionId != authorization.SessionId
                || save.PngWrite.ResultId != authorization.ResultId))
        {
            return CreateFailure(
                FailureCode.ClipboardPublicationRejected,
                deliveryId,
                "Successful Save Clipboard publication requires matching PNG-write evidence.");
        }

        return null;
    }

    private static bool HasMatchingIdentity(
        ClipboardDeliveryResult result,
        ClipboardDeliveryRequest request) =>
        result.DeliveryId == request.DeliveryId
        && result.SessionId == request.SessionId
        && result.ResultId == request.ResultId;

    private static Failure CreateFailure(
        FailureCode code,
        Guid correlationId,
        string message,
        int? nativeCode = null) => Failure.Create(
            code,
            code == FailureCode.ClipboardBusy
                ? FailureCategory.Contention
                : FailureCategory.Validation,
            FailureRecoverability.RetrySameIntent,
            nameof(OutputCommitmentCoordinator),
            correlationId,
            message,
            nativeCode: nativeCode);
}
