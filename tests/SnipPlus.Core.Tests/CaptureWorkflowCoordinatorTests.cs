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
        var image = new TestImageResult();
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());
        var capture = new FakeCaptureService(_ => new CaptureOutcome.Succeeded(
            Guid.Empty,
            Guid.Empty,
            2,
            2,
            new PhysicalRect(0, 0, 2, 2),
            new PhysicalRect(0, 0, 2, 2),
            DateTimeOffset.UnixEpoch,
            image,
            Array.Empty<string>()));
        var clipboard = new FakeClipboardService(_ => new ClipboardDeliveryResult.Delivered(Guid.Empty, Guid.Empty, image.Metadata.ResultId, 1));

        var result = await coordinator.RunAsync(CreateIntent(), capture, clipboard, CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.Completed, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.IsNull(result.RetainedResult);
        Assert.IsTrue(image.IsDisposed);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task RetryableClipboardFailureRetainsValidResultAtResultReady()
    {
        var image = new TestImageResult();
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
            new FakeCaptureService(_ => Success(image)),
            clipboard,
            CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.RetryableFailure, result.Outcome);
        Assert.AreEqual(WorkflowState.ResultReady, result.FinalState);
        Assert.AreSame(image, result.RetainedResult);
        Assert.IsFalse(image.IsDisposed);
        Assert.AreEqual(FailureCode.ClipboardBusy, result.Failure?.Code);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task CaptureCancellationDoesNotPublishAResult()
    {
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());
        var result = await coordinator.RunAsync(
            CreateIntent(),
            new FakeCaptureService(_ => new CaptureOutcome.Cancelled(Guid.Empty, Guid.Empty, "user", true, true)),
            null,
            CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.Cancelled, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.IsNull(result.RetainedResult);
        Assert.IsTrue(result.CleanupCompleted);
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
        var image = new TestImageResult();
        var clipboard = new FakeClipboardService(_ => throw new AssertFailedException("Clipboard must not be called."));
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());

        var result = await coordinator.RunAsync(
            CreateIntent(),
            new FakeCaptureService(_ => Success(image)),
            clipboard,
            CancellationToken.None,
            (_, _) => ThrowCancellationAsync());

        Assert.AreEqual(WorkflowOutcomeKind.Cancelled, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.IsTrue(result.CleanupCompleted);
        Assert.IsTrue(image.IsDisposed);
        Assert.IsFalse(clipboard.WasCalled);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Cancellation")]
    public async Task ClipboardCancellationDisposesImageAndReturnsIdle()
    {
        var image = new TestImageResult();
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());
        var clipboard = new FakeClipboardService(request => new ClipboardDeliveryResult.Cancelled(
            request.DeliveryId,
            request.SessionId,
            request.ResultId,
            "CancellationToken"));

        var result = await coordinator.RunAsync(
            CreateIntent(),
            new FakeCaptureService(_ => Success(image)),
            clipboard,
            CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.Cancelled, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.IsTrue(result.CleanupCompleted);
        Assert.IsTrue(image.IsDisposed);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task TerminalCaptureFailureReturnsTypedFailureAndCleanup()
    {
        var failure = Failure.Create(
            FailureCode.CaptureSourceUnavailable,
            FailureCategory.Device,
            FailureRecoverability.RetryNewIntent,
            "fake-capture",
            Guid.NewGuid(),
            "synthetic source unavailable");
        var coordinator = new CaptureWorkflowCoordinator(new WorkflowStateAuthority());

        var result = await coordinator.RunAsync(
            CreateIntent(),
            new FakeCaptureService(_ => new CaptureOutcome.Failed(Guid.Empty, Guid.Empty, failure, true, true)),
            null,
            CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.TerminalFailure, result.Outcome);
        Assert.AreEqual(WorkflowState.Idle, result.FinalState);
        Assert.AreEqual(FailureCode.CaptureSourceUnavailable, result.Failure?.Code);
        Assert.IsTrue(result.CleanupCompleted);
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

    private static CaptureOutcome.Succeeded Success(TestImageResult image) => new(
        Guid.Empty,
        Guid.Empty,
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
        public ValueTask<CaptureOutcome> CaptureAsync(CaptureIntent intent, CancellationToken ignored)
            => ValueTask.FromException<CaptureOutcome>(new OperationCanceledException(cancellationToken));
    }

    private sealed class FakeCaptureService(Func<CaptureIntent, CaptureOutcome> handler) : ICaptureService
    {
        public ValueTask<CaptureOutcome> CaptureAsync(CaptureIntent intent, CancellationToken cancellationToken)
            => ValueTask.FromResult(handler(intent));
    }

    private sealed class FakeClipboardService(Func<ClipboardDeliveryRequest, ClipboardDeliveryResult> handler) : IClipboardDeliveryService
    {
        public bool WasCalled { get; private set; }

        public ValueTask<ClipboardDeliveryResult> DeliverAsync(ClipboardDeliveryRequest request, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return ValueTask.FromResult(handler(request));
        }
    }
}
