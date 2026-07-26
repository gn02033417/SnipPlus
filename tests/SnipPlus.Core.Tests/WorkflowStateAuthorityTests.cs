using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class WorkflowStateAuthorityTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void LegalTransitionsFollowTheSingleAuthority()
    {
        var authority = new WorkflowStateAuthority();

        Assert.IsTrue(authority.RequestTransition(new(WorkflowState.Idle, WorkflowState.Starting, "test")).IsSuccess);
        Assert.IsTrue(authority.RequestTransition(new(WorkflowState.Starting, WorkflowState.Selecting, "test")).IsSuccess);
        Assert.IsTrue(authority.RequestTransition(new(WorkflowState.Selecting, WorkflowState.Capturing, "test")).IsSuccess);
        Assert.IsTrue(authority.RequestTransition(new(WorkflowState.Capturing, WorkflowState.Cancelled, "test")).IsSuccess);
        Assert.IsTrue(authority.RequestTransition(new(WorkflowState.Cancelled, WorkflowState.Idle, "cleanup")).IsSuccess);
        Assert.AreEqual(WorkflowState.Idle, authority.CurrentState);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void IllegalTransitionIsRejectedWithoutMutatingState()
    {
        var authority = new WorkflowStateAuthority();

        var result = authority.RequestTransition(new(WorkflowState.Idle, WorkflowState.Capturing, "illegal"));

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(FailureCode.InvalidStateTransition, result.Failure?.Code);
        Assert.AreEqual(WorkflowState.Idle, authority.CurrentState);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void ClipboardRetryCancellationCanReachCancelledBoundary()
    {
        var authority = new WorkflowStateAuthority();
        authority.RequestTransition(new(WorkflowState.Idle, WorkflowState.Starting, "test"));
        authority.RequestTransition(new(WorkflowState.Starting, WorkflowState.Selecting, "test"));
        authority.RequestTransition(new(WorkflowState.Selecting, WorkflowState.Capturing, "test"));
        authority.RequestTransition(new(WorkflowState.Capturing, WorkflowState.ResultReady, "test"));
        authority.RequestTransition(new(WorkflowState.ResultReady, WorkflowState.Delivering, "test"));

        var result = authority.RequestTransition(new(WorkflowState.Delivering, WorkflowState.Cancelled, "cancel"));

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(WorkflowState.Cancelled, authority.CurrentState);
    }
}
