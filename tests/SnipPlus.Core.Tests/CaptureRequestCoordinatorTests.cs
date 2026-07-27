using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class CaptureRequestCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void PrintScreenIdentityAndTimestampArePreserved()
    {
        var authority = new WorkflowStateAuthority();
        using var coordinator = new CaptureRequestCoordinator(authority);
        var requestId = Guid.NewGuid();
        var receivedAt = DateTimeOffset.UnixEpoch.AddMinutes(3);
        var eventArgs = new PrintScreenReceivedEventArgs(requestId, receivedAt);

        var result = coordinator.Submit(CaptureRequest.FromPrintScreen(eventArgs));

        Assert.IsTrue(result.IsAccepted);
        Assert.AreEqual(requestId, result.Request.RequestId);
        Assert.AreEqual(receivedAt, result.Request.RequestedAt);
        Assert.AreEqual(CaptureRequestSource.PrintScreen, result.Request.RequestSource);
        Assert.AreEqual(WorkflowState.CaptureRequested, result.AcceptedWorkflowState);
        Assert.AreEqual(requestId, coordinator.ActiveRequest?.RequestId);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void OnePrintScreenCreatesOneSuccessfulTransition()
    {
        var authority = new WorkflowStateAuthority();
        using var coordinator = new CaptureRequestCoordinator(authority);

        var result = coordinator.Submit(CaptureRequest.FromPrintScreen(
            new PrintScreenReceivedEventArgs(Guid.NewGuid(), DateTimeOffset.UnixEpoch)));

        Assert.IsTrue(result.IsAccepted);
        Assert.AreEqual(1, authority.SuccessfulTransitionCount);
        Assert.AreEqual(WorkflowState.CaptureRequested, authority.CurrentState);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void SecondRequestIsBusyAndDoesNotReplaceTheFirstRequest()
    {
        var authority = new WorkflowStateAuthority();
        using var coordinator = new CaptureRequestCoordinator(authority);
        var firstRequest = CaptureRequest.FromPrintScreen(
            new PrintScreenReceivedEventArgs(Guid.NewGuid(), DateTimeOffset.UnixEpoch));
        var secondRequest = CaptureRequest.FromPrintScreen(
            new PrintScreenReceivedEventArgs(Guid.NewGuid(), DateTimeOffset.UnixEpoch.AddSeconds(1)));

        var firstResult = coordinator.Submit(firstRequest);
        var secondResult = coordinator.Submit(secondRequest);

        Assert.IsTrue(firstResult.IsAccepted);
        Assert.IsFalse(secondResult.IsAccepted);
        Assert.AreEqual(CaptureRequestRejectionReason.Busy, secondResult.RejectionReason);
        Assert.AreEqual(firstRequest.RequestId, secondResult.ActiveRequest?.RequestId);
        Assert.AreEqual(firstRequest.RequestId, coordinator.ActiveRequest?.RequestId);
        Assert.AreEqual(1, authority.SuccessfulTransitionCount);
        StringAssert.Contains(secondResult.UserMessage, "already active");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void SecondaryInAppCommandUsesTheSameBoundary()
    {
        var authority = new WorkflowStateAuthority();
        using var coordinator = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(
            Guid.NewGuid(),
            DateTimeOffset.UnixEpoch.AddHours(1));

        var result = coordinator.Submit(request);

        Assert.IsTrue(result.IsAccepted);
        Assert.AreEqual(CaptureRequestSource.SecondaryInAppCommand, result.Request.RequestSource);
        Assert.AreEqual(WorkflowState.CaptureRequested, result.CurrentState);
        Assert.AreEqual(1, authority.SuccessfulTransitionCount);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void PrintScreenAndSecondaryCommandCannotCreateTwoActiveRequests()
    {
        var authority = new WorkflowStateAuthority();
        using var coordinator = new CaptureRequestCoordinator(authority);
        var printScreen = CaptureRequest.FromPrintScreen(
            new PrintScreenReceivedEventArgs(Guid.NewGuid(), DateTimeOffset.UnixEpoch));
        var secondary = CaptureRequest.CreateSecondary(
            Guid.NewGuid(),
            DateTimeOffset.UnixEpoch.AddSeconds(2));

        var printScreenResult = coordinator.Submit(printScreen);
        var secondaryResult = coordinator.Submit(secondary);

        Assert.IsTrue(printScreenResult.IsAccepted);
        Assert.IsFalse(secondaryResult.IsAccepted);
        Assert.AreEqual(CaptureRequestRejectionReason.Busy, secondaryResult.RejectionReason);
        Assert.AreEqual(printScreen.RequestId, coordinator.ActiveRequest?.RequestId);
        Assert.AreEqual(1, authority.SuccessfulTransitionCount);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Cancellation")]
    public void DisposedBoundaryRejectsLateRequestWithoutChangingState()
    {
        var authority = new WorkflowStateAuthority();
        var coordinator = new CaptureRequestCoordinator(authority);
        coordinator.Dispose();

        var result = coordinator.Submit(CaptureRequest.FromPrintScreen(
            new PrintScreenReceivedEventArgs(Guid.NewGuid(), DateTimeOffset.UnixEpoch)));

        Assert.IsFalse(result.IsAccepted);
        Assert.AreEqual(CaptureRequestRejectionReason.ApplicationExiting, result.RejectionReason);
        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(0, authority.SuccessfulTransitionCount);
        coordinator.Dispose();
    }
}
