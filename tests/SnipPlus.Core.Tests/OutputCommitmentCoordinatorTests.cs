using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class OutputCommitmentCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public async Task CompleteAuthorizationPublishesOnceWithClipboardPolicy()
    {
        var image = new TestImageResult();
        var clipboard = new FakeClipboardDelivery();
        var coordinator = new OutputCommitmentCoordinator(clipboard);

        var result = await coordinator.PublishAsync(
            CreateRequest(image, OutputCommitmentAuthorization.CreateComplete(
                image.Metadata.SessionId,
                "virtual-desktop-v1",
                image.Metadata.SelectionRevision,
                image.Metadata.AnnotationRevision,
                image.Metadata.ResultId)),
            CancellationToken.None);

        Assert.IsInstanceOfType<ClipboardDeliveryResult.Delivered>(result);
        Assert.AreEqual(1, clipboard.Calls);
        Assert.IsNotNull(clipboard.LastRequest);
        Assert.IsFalse(clipboard.LastRequest!.HistoryAllowed);
        Assert.IsFalse(clipboard.LastRequest.RoamingAllowed);
        Assert.AreEqual(5, clipboard.LastRequest.MaximumAttempts);
        Assert.AreEqual(TimeSpan.FromSeconds(1), clipboard.LastRequest.RetryBudget);
        Assert.AreSame(image, clipboard.LastRequest.ImageResult);
        Assert.IsFalse(image.IsDisposed);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task SuccessfulSaveRequiresMatchingPngWriteEvidence()
    {
        var image = new TestImageResult();
        var clipboard = new FakeClipboardDelivery();
        var coordinator = new OutputCommitmentCoordinator(clipboard);
        var authorization = OutputCommitmentAuthorization.CreateSuccessfulSave(
            image.Metadata.SessionId,
            "virtual-desktop-v1",
            image.Metadata.SelectionRevision,
            image.Metadata.AnnotationRevision,
            image.Metadata.ResultId,
            new PngWriteSucceededEvidence
            {
                SessionId = image.Metadata.SessionId,
                ResultId = image.Metadata.ResultId
            });

        var result = await coordinator.PublishAsync(
            CreateRequest(image, authorization),
            CancellationToken.None);

        Assert.IsInstanceOfType<ClipboardDeliveryResult.Delivered>(result);
        Assert.AreEqual(1, clipboard.Calls);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task SuccessfulSaveWithMismatchedPngEvidenceIsRejectedWithoutClipboardCall()
    {
        var image = new TestImageResult();
        var clipboard = new FakeClipboardDelivery();
        var coordinator = new OutputCommitmentCoordinator(clipboard);
        var authorization = OutputCommitmentAuthorization.CreateSuccessfulSave(
            image.Metadata.SessionId,
            "virtual-desktop-v1",
            image.Metadata.SelectionRevision,
            image.Metadata.AnnotationRevision,
            image.Metadata.ResultId,
            new PngWriteSucceededEvidence
            {
                SessionId = Guid.NewGuid(),
                ResultId = image.Metadata.ResultId
            });

        var result = await coordinator.PublishAsync(
            CreateRequest(image, authorization),
            CancellationToken.None);

        var failure = result as ClipboardDeliveryResult.TerminalFailure;
        Assert.IsNotNull(failure);
        Assert.AreEqual(FailureCode.ClipboardPublicationRejected, failure.Failure.Code);
        Assert.AreEqual(0, clipboard.Calls);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task PublicationRequiresDeliveringStateAndMatchingRevisions()
    {
        var image = new TestImageResult(selectionRevision: 2);
        var clipboard = new FakeClipboardDelivery();
        var coordinator = new OutputCommitmentCoordinator(clipboard);
        var authorization = OutputCommitmentAuthorization.CreateComplete(
            image.Metadata.SessionId,
            "virtual-desktop-v1",
            1,
            image.Metadata.AnnotationRevision,
            image.Metadata.ResultId);

        var invalidState = await coordinator.PublishAsync(
            CreateRequest(image, authorization, WorkflowState.Editing),
            CancellationToken.None);
        var stateFailure = invalidState as ClipboardDeliveryResult.TerminalFailure;
        Assert.IsNotNull(stateFailure);
        Assert.AreEqual(FailureCode.InvalidStateTransition, stateFailure.Failure.Code);

        var invalidRevision = await coordinator.PublishAsync(
            CreateRequest(image, authorization),
            CancellationToken.None);
        var revisionFailure = invalidRevision as ClipboardDeliveryResult.TerminalFailure;
        Assert.IsNotNull(revisionFailure);
        Assert.AreEqual(FailureCode.ClipboardPublicationRejected, revisionFailure.Failure.Code);
        Assert.AreEqual(0, clipboard.Calls);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task DisposedImageIsRejectedAndNotPublished()
    {
        var image = new TestImageResult();
        image.Dispose();
        var clipboard = new FakeClipboardDelivery();
        var coordinator = new OutputCommitmentCoordinator(clipboard);

        var result = await coordinator.PublishAsync(
            CreateRequest(image, OutputCommitmentAuthorization.CreateComplete(
                image.Metadata.SessionId,
                "virtual-desktop-v1",
                image.Metadata.SelectionRevision,
                image.Metadata.AnnotationRevision,
                image.Metadata.ResultId)),
            CancellationToken.None);

        var failure = result as ClipboardDeliveryResult.TerminalFailure;
        Assert.IsNotNull(failure);
        Assert.AreEqual(FailureCode.InvalidResultLifetime, failure.Failure.Code);
        Assert.AreEqual(0, clipboard.Calls);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ClipboardFailureSemanticsArePreserved()
    {
        var image = new TestImageResult();
        var expectedFailure = Failure.Create(
            FailureCode.ClipboardBusy,
            FailureCategory.Contention,
            FailureRecoverability.RetrySameIntent,
            "test",
            image.Metadata.ResultId,
            "busy");
        var clipboard = new FakeClipboardDelivery
        {
            ResultFactory = request => new ClipboardDeliveryResult.RetryableFailure(
                request.DeliveryId,
                request.SessionId,
                request.ResultId,
                expectedFailure,
                2)
        };
        var coordinator = new OutputCommitmentCoordinator(clipboard);

        var result = await coordinator.PublishAsync(
            CreateRequest(image, OutputCommitmentAuthorization.CreateComplete(
                image.Metadata.SessionId,
                "virtual-desktop-v1",
                image.Metadata.SelectionRevision,
                image.Metadata.AnnotationRevision,
                image.Metadata.ResultId)),
            CancellationToken.None);

        var retryable = result as ClipboardDeliveryResult.RetryableFailure;
        Assert.IsNotNull(retryable);
        Assert.AreEqual(FailureCode.ClipboardBusy, retryable.Failure.Code);
        Assert.AreEqual(2, retryable.AttemptsUsed);
    }

    private static OutputCommitmentRequest CreateRequest(
        TestImageResult image,
        OutputCommitmentAuthorization authorization,
        WorkflowState state = WorkflowState.Delivering) => new()
        {
            Authorization = authorization,
            ImageResult = image,
            WorkflowState = state,
            SelectionWidth = image.Metadata.PixelWidth,
            SelectionHeight = image.Metadata.PixelHeight,
            DisplayCount = 1,
            Cancellation = CancellationToken.None
        };

    private sealed class FakeClipboardDelivery : IClipboardDeliveryService
    {
        public int Calls { get; private set; }

        public ClipboardDeliveryRequest? LastRequest { get; private set; }

        public Func<ClipboardDeliveryRequest, ClipboardDeliveryResult>? ResultFactory { get; init; }

        public ValueTask<ClipboardDeliveryResult> DeliverAsync(
            ClipboardDeliveryRequest request,
            CancellationToken cancellationToken)
        {
            Calls++;
            LastRequest = request;
            return ValueTask.FromResult(ResultFactory?.Invoke(request)
                ?? new ClipboardDeliveryResult.Delivered(
                    request.DeliveryId,
                    request.SessionId,
                    request.ResultId,
                    1));
        }
    }
}
