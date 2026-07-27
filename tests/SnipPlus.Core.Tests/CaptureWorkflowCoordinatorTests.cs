using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class CaptureWorkflowCoordinatorTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public async Task LegacyCaptureEntryIsBlockedByTheFormalResidentState()
    {
        var capture = new FailIfCalledCaptureService();
        var authority = new WorkflowStateAuthority();
        var coordinator = new CaptureWorkflowCoordinator(authority);

        var result = await coordinator.BeginSelectionAsync(
            CreateIntent(),
            capture,
            CancellationToken.None);

        var failed = result as CaptureFrameOutcome.Failed;
        Assert.IsNotNull(failed);
        Assert.AreEqual(FailureCode.InvalidStateTransition, failed.Failure.Code);
        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(0, capture.CaptureFrameCalls);
        Assert.AreEqual(0, capture.CropFrameCalls);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task LegacyRunCannotBypassTheCaptureRequestBoundary()
    {
        var capture = new FailIfCalledCaptureService();
        var authority = new WorkflowStateAuthority();
        var coordinator = new CaptureWorkflowCoordinator(authority);

        var result = await coordinator.RunAsync(
            CreateIntent(),
            capture,
            null,
            CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.TerminalFailure, result.Outcome);
        Assert.AreEqual(WorkflowState.ResidentReady, result.FinalState);
        Assert.AreEqual(0, capture.CaptureFrameCalls);
        Assert.AreEqual(0, capture.CropFrameCalls);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task LegacyCompletionCannotAdvanceFormalStateOrCallCapture()
    {
        var capture = new FailIfCalledCaptureService();
        var authority = new WorkflowStateAuthority();
        var coordinator = new CaptureWorkflowCoordinator(authority);
        var frozenImage = new TestImageResult();
        var frozenFrame = new FrozenCaptureFrame(frozenImage);

        var result = await coordinator.CompleteSelectionAsync(
            CreateIntent(),
            frozenFrame,
            capture,
            null,
            CancellationToken.None);

        Assert.AreEqual(WorkflowOutcomeKind.TerminalFailure, result.Outcome);
        Assert.AreEqual(WorkflowState.ResidentReady, result.FinalState);
        Assert.AreEqual(0, capture.CaptureFrameCalls);
        Assert.AreEqual(0, capture.CropFrameCalls);
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

    private sealed class FailIfCalledCaptureService : ICaptureService
    {
        public int CaptureFrameCalls { get; private set; }

        public int CropFrameCalls { get; private set; }

        public ValueTask<CaptureFrameOutcome> CaptureFrameAsync(
            CaptureIntent intent,
            CancellationToken cancellationToken)
        {
            CaptureFrameCalls++;
            return ValueTask.FromException<CaptureFrameOutcome>(
                new AssertFailedException("Legacy capture entry must not call the capture service."));
        }

        public ValueTask<CaptureOutcome> CropFrameAsync(
            CaptureIntent intent,
            FrozenCaptureFrame frozenFrame,
            CancellationToken cancellationToken)
        {
            CropFrameCalls++;
            return ValueTask.FromException<CaptureOutcome>(
                new AssertFailedException("Legacy capture entry must not call the crop service."));
        }
    }
}
