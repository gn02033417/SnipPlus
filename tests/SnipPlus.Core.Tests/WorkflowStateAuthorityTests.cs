using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class WorkflowStateAuthorityTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void InitialStateIsResidentReady()
    {
        var authority = new WorkflowStateAuthority();

        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(0, authority.SuccessfulTransitionCount);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void ResidentReadyCanTransitionToCaptureRequested()
    {
        var authority = new WorkflowStateAuthority();

        var result = authority.RequestTransition(new(
            WorkflowState.ResidentReady,
            WorkflowState.CaptureRequested,
            "test"));

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(WorkflowState.CaptureRequested, authority.CurrentState);
        Assert.AreEqual(1, authority.SuccessfulTransitionCount);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void FreezingCanEnterSelectingAndSelectionLockedThroughTheSingleAuthority()
    {
        var authority = new WorkflowStateAuthority();
        Assert.IsTrue(authority.RequestTransition(new(
            WorkflowState.ResidentReady,
            WorkflowState.CaptureRequested,
            "test")).IsSuccess);

        var freezing = authority.RequestTransition(new(
            WorkflowState.CaptureRequested,
            WorkflowState.Freezing,
            "third-slice"));

        Assert.IsTrue(freezing.IsSuccess);
        Assert.AreEqual(WorkflowState.Freezing, authority.CurrentState);

        var selecting = authority.RequestTransition(new(
            WorkflowState.Freezing,
            WorkflowState.Selecting,
            "frozen-frame-set-ready"));
        Assert.IsTrue(selecting.IsSuccess);

        var locked = authority.RequestTransition(new(
            WorkflowState.Selecting,
            WorkflowState.SelectionLocked,
            "valid-pointer-release"));
        Assert.IsTrue(locked.IsSuccess);
        Assert.AreEqual(WorkflowState.SelectionLocked, authority.CurrentState);

        var outputState = authority.RequestTransition(new(
            WorkflowState.SelectionLocked,
            WorkflowState.Capturing,
            "output-is-out-of-scope"));
        Assert.IsFalse(outputState.IsSuccess);
        Assert.AreEqual(WorkflowState.SelectionLocked, authority.CurrentState);
        Assert.AreEqual(4, authority.SuccessfulTransitionCount);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void CancellationReturnsToResidentReadyWithoutAllowingOutputStates()
    {
        var authority = new WorkflowStateAuthority();

        Assert.IsTrue(authority.RequestTransition(new(
            WorkflowState.ResidentReady,
            WorkflowState.CaptureRequested,
            "test")).IsSuccess);
        Assert.IsTrue(authority.RequestTransition(new(
            WorkflowState.CaptureRequested,
            WorkflowState.Freezing,
            "test")).IsSuccess);
        Assert.IsTrue(authority.RequestTransition(new(
            WorkflowState.Freezing,
            WorkflowState.Selecting,
            "test")).IsSuccess);
        Assert.IsTrue(authority.RequestTransition(new(
            WorkflowState.Selecting,
            WorkflowState.Cancelled,
            "escape")).IsSuccess);
        Assert.IsTrue(authority.RequestTransition(new(
            WorkflowState.Cancelled,
            WorkflowState.ResidentReady,
            "cleanup")).IsSuccess);

        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void IllegalTransitionIsRejectedWithoutMutatingState()
    {
        var authority = new WorkflowStateAuthority();

        var result = authority.RequestTransition(new(
            WorkflowState.ResidentReady,
            WorkflowState.Selecting,
            "illegal"));

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(FailureCode.InvalidStateTransition, result.Failure?.Code);
        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(0, authority.SuccessfulTransitionCount);
    }
}
