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
    public void CaptureRequestedCannotEnterLaterWorkflowStatesInThisSlice()
    {
        var authority = new WorkflowStateAuthority();
        Assert.IsTrue(authority.RequestTransition(new(
            WorkflowState.ResidentReady,
            WorkflowState.CaptureRequested,
            "test")).IsSuccess);

        foreach (var laterState in new[]
                 {
                     WorkflowState.Freezing,
                     WorkflowState.Selecting,
                     WorkflowState.Capturing,
                     WorkflowState.ResultReady,
                     WorkflowState.Delivering,
                     WorkflowState.Completed
                 })
        {
            var result = authority.RequestTransition(new(
                WorkflowState.CaptureRequested,
                laterState,
                "not implemented in this slice"));

            Assert.IsFalse(result.IsSuccess, $"Unexpected transition to {laterState}.");
            Assert.AreEqual(WorkflowState.CaptureRequested, authority.CurrentState);
        }

        Assert.AreEqual(1, authority.SuccessfulTransitionCount);
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
