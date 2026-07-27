using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class CaptureWorkflowCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public async Task SuccessfulCaptureAndClipboardDeliveryCleanupToIdle()
    {
        var frozenImage = new TestImageResult();
        var croppedImage = new TestImageResult();
        var capture = new FakeCaptureService(
            _ => FrameSuccess(frozenImage),
            (intent, frame) =>
            {
                Assert.AreSame(frozenImage, frame.ImageResult);
                return Success(intent, croppedImage);
            });
        var clipboard = new FakeClipboardService(request =>
        {
            Assert.AreSame(croppedImage, request.ImageResult);
            return new ClipboardDeliveryResult.Delivered(
                request.DeliveryId,
                request.SessionId,
                request.ResultId,
                1);
        });

        var result = await new CaptureWorkflowCoordinator(new WorkflowStateAuthority()).RunAsync(
            CreateIntent(),
            capture,
            clipboard,
            CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.Completed, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.AreEqual(1, capture.CaptureFrameCalls);
        Assert.AreEqual(1, capture.CropFrameCalls);
        Assert.IsNull(result.RetainedResult);
        Assert.IsTrue(frozenImage.IsDisposed);
        Assert.IsTrue(croppedImage.IsDisposed);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Cancellation")]
    public async Task CancellingSelectionDisposesFrozenFrameAndSkipsCrop()
    {
        var frozenImage = new TestImageResult();
        var capture = new FakeCaptureService(_ => FrameSuccess(frozenImage));
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());

        var frameOutcome = await coordinator.BeginSelectionAsync(CreateIntent(), capture, CancellationToken.None);
        var succeeded = frameOutcome as CaptureFrameOutcome.Succeeded;
        Assert.IsNotNull(succeeded);
        Assert.AreEqual(WorkflowState.Selecting, coordinator.StateAuthority.CurrentState);

        var result = coordinator.CancelSelection(Guid.NewGuid(), succeeded.FrozenFrame);

        Assert.AreEqual(WorkflowOutcomeKind.Cancelled, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.IsTrue(result.CleanupCompleted);
        Assert.IsTrue(frozenImage.IsDisposed);
        Assert.AreEqual(0, capture.CropFrameCalls);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CaptureFailureDoesNotEnterSelectionOrCrop()
    {
        var failure = Failure.Create(
            FailureCode.CaptureSourceUnavailable,
            FailureCategory.Device,
            FailureRecoverability.RetryNewIntent,
            "fake-capture",
            Guid.NewGuid(),
            "synthetic source unavailable");
        var capture = new FakeCaptureService(_ => new CaptureFrameOutcome.Failed(
            Guid.Empty,
            Guid.Empty,
            failure,
            true,
            true));
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());

        var frameOutcome = await coordinator.BeginSelectionAsync(CreateIntent(), capture, CancellationToken.None);

        var failed = frameOutcome as CaptureFrameOutcome.Failed;
        Assert.IsNotNull(failed);
        Assert.AreEqual(FailureCode.CaptureSourceUnavailable, failed.Failure.Code);
        Assert.AreEqual(WorkflowState.Idle, coordinator.StateAuthority.CurrentState);
        Assert.AreEqual(0, capture.CropFrameCalls);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Cancellation")]
    public async Task CaptureOperationCanceledExceptionReturnsCancelledAndCleansUp()
    {
        using var cancellation = new CancellationTokenSource();
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());

        var result = await coordinator.RunAsync(
            CreateIntent(),
            new ThrowingCaptureService(cancellation.Token),
            null,
            cancellation.Token);

        Assert.AreEqual(WorkflowOutcomeKind.Cancelled, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.IsTrue(result.CleanupCompleted);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Cancellation")]
    public async Task ResultReadyCancellationDisposesImageAndSkipsClipboard()
    {
        var frozenImage = new TestImageResult();
        var croppedImage = new TestImageResult();
        var clipboard = new FakeClipboardService(_ => throw new AssertFailedException("Clipboard must not be called."));
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());
        var capture = new FakeCaptureService(
            _ => FrameSuccess(frozenImage),
            (intent, _) => Success(intent, croppedImage));

        var result = await coordinator.RunAsync(
            CreateIntent(),
            capture,
            clipboard,
            CancellationToken.None,
            (_, _) => ThrowCancellationAsync());

        Assert.AreEqual(WorkflowOutcomeKind.Cancelled, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.IsTrue(result.CleanupCompleted);
        Assert.IsTrue(frozenImage.IsDisposed);
        Assert.IsTrue(croppedImage.IsDisposed);
        Assert.IsFalse(clipboard.WasCalled);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task RetryableClipboardFailureRetainsOnlyTheCroppedResult()
    {
        var frozenImage = new TestImageResult();
        var croppedImage = new TestImageResult();
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());
        var clipboardFailure = Failure.Create(
            FailureCode.ClipboardBusy,
            FailureCategory.Contention,
            FailureRecoverability.RetrySameIntent,
            "fake-clipboard",
            Guid.NewGuid(),
            "synthetic clipboard busy");
        var clipboard = new FakeClipboardService(request => new ClipboardDeliveryResult.RetryableFailure(
            request.DeliveryId,
            request.SessionId,
            request.ResultId,
            clipboardFailure,
            2));

        var result = await coordinator.RunAsync(
            CreateIntent(),
            new FakeCaptureService(
                _ => FrameSuccess(frozenImage),
                (intent, _) => Success(intent, croppedImage)),
            clipboard,
            CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.RetryableFailure, result.Outcome);
        Assert.AreEqual(WorkflowState.ResultReady, result.FinalState);
        Assert.AreSame(croppedImage, result.RetainedResult);
        Assert.IsFalse(croppedImage.IsDisposed);
        Assert.IsTrue(frozenImage.IsDisposed);
        Assert.AreEqual(FailureCode.ClipboardBusy, result.Failure?.Code);

        croppedImage.Dispose();
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task TerminalCropFailureReturnsTypedFailureAndCleanup()
    {
        var frozenImage = new TestImageResult();
        var failure = Failure.Create(
            FailureCode.InvalidCaptureIntent,
            FailureCategory.Validation,
            FailureRecoverability.RetryNewIntent,
            "fake-crop",
            Guid.NewGuid(),
            "synthetic crop failure");
        var capture = new FakeCaptureService(
            _ => FrameSuccess(frozenImage),
            (_, _) => new CaptureOutcome.Failed(Guid.Empty, Guid.Empty, failure, true, true));

        var result = await new CaptureWorkflowCoordinator(new WorkflowStateAuthority()).RunAsync(
            CreateIntent(),
            capture,
            null,
            CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.TerminalFailure, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.AreEqual(FailureCode.InvalidCaptureIntent, result.Failure?.Code);
        Assert.IsTrue(result.CleanupCompleted);
        Assert.IsTrue(frozenImage.IsDisposed);
    }

    private static CaptureIntent CreateIntent() => new()
    {
        RequestId = Guid.NewGuid(),
        SessionId = Guid.NewGuid(),
        SourceKind = SourceKind.Monitor,
        SourceId = "synthetic-monitor",
        SourcePhysicalBounds = new PhysicalRect(0, 0, 2, 2),
        SelectionDipBounds = new DipRect(0, 0, 2, 2),
        SelectionPhysicalBounds = new PhysicalRect(0, 0, 2, 2),
        CropBoundsInSource = new PhysicalRect(0, 0, 2, 2),
        DpiScaleX = 1,
        DpiScaleY = 1,
        CoordinateVersion = "display-v1",
        RequestedAt = DateTimeOffset.UnixEpoch
    };

    private static CaptureFrameOutcome.Succeeded FrameSuccess(TestImageResult image) => new(
        Guid.Empty,
        Guid.Empty,
        new FrozenCaptureFrame(image));

    private static CaptureOutcome.Succeeded Success(CaptureIntent intent, TestImageResult image) => new(
        intent.RequestId,
        intent.SessionId,
        2,
        2,
        new PhysicalRect(0, 0, 2, 2),
        new PhysicalRect(0, 0, 2, 2),
        DateTimeOffset.UnixEpoch,
        image,
        Array.Empty<string>());

    private static async ValueTask ThrowCancellationAsync()
    {
        await Task.Yield();
        throw new OperationCanceledException();
    }

    private sealed class ThrowingCaptureService(CancellationToken cancellationToken) : ICaptureService
    {
        public ValueTask<CaptureFrameOutcome> CaptureFrameAsync(CaptureIntent intent, CancellationToken ignored)
            => ValueTask.FromException<CaptureFrameOutcome>(new OperationCanceledException(cancellationToken));

        public ValueTask<CaptureOutcome> CropFrameAsync(
            CaptureIntent intent,
            FrozenCaptureFrame frozenFrame,
            CancellationToken cancellationToken)
            => ValueTask.FromException<CaptureOutcome>(new AssertFailedException("Crop must not be called."));
    }

    private sealed class FakeCaptureService(
        Func<CaptureIntent, CaptureFrameOutcome> frameHandler,
        Func<CaptureIntent, FrozenCaptureFrame, CaptureOutcome>? cropHandler = null) : ICaptureService
    {
        public int CaptureFrameCalls { get; private set; }

        public int CropFrameCalls { get; private set; }

        public ValueTask<CaptureFrameOutcome> CaptureFrameAsync(
            CaptureIntent intent,
            CancellationToken cancellationToken)
        {
            CaptureFrameCalls++;
            return ValueTask.FromResult(frameHandler(intent));
        }

        public ValueTask<CaptureOutcome> CropFrameAsync(
            CaptureIntent intent,
            FrozenCaptureFrame frozenFrame,
            CancellationToken cancellationToken)
        {
            CropFrameCalls++;
            if (cropHandler is not null)
            {
                return ValueTask.FromResult(cropHandler(intent, frozenFrame));
            }

            return ValueTask.FromResult<CaptureOutcome>(Success(intent, new TestImageResult()));
        }
    }

    private sealed class FakeClipboardService(Func<ClipboardDeliveryRequest, ClipboardDeliveryResult> handler) : IClipboardDeliveryService
    {
        public bool WasCalled { get; private set; }

        public ValueTask<ClipboardDeliveryResult> DeliverAsync(
            ClipboardDeliveryRequest request,
            CancellationToken cancellationToken)
        {
            WasCalled = true;
            return ValueTask.FromResult(handler(request));
        }
    }
}
