using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class CaptureRequestApplicationBoundaryTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void PrintScreenEventIsMappedWithoutChangingIdentityOrTimestamp()
    {
        var boundary = new RecordingBoundary();
        var entryPoint = new CaptureRequestApplicationBoundary(boundary);
        var requestId = Guid.NewGuid();
        var receivedAt = DateTimeOffset.UnixEpoch.AddSeconds(7);

        var result = entryPoint.SubmitPrintScreen(
            new PrintScreenReceivedEventArgs(requestId, receivedAt));

        Assert.AreSame(boundary.LastRequest, result.Request);
        Assert.AreEqual(requestId, boundary.LastRequest?.RequestId);
        Assert.AreEqual(receivedAt, boundary.LastRequest?.RequestedAt);
        Assert.AreEqual(CaptureRequestSource.PrintScreen, boundary.LastRequest?.RequestSource);
        Assert.AreEqual(1, boundary.SubmitCalls);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void SecondaryCommandUsesTheSameBoundary()
    {
        var boundary = new RecordingBoundary();
        var entryPoint = new CaptureRequestApplicationBoundary(boundary);
        var requestId = Guid.NewGuid();
        var requestedAt = DateTimeOffset.UnixEpoch.AddSeconds(8);

        entryPoint.SubmitSecondaryInAppCommand(requestId, requestedAt);

        Assert.AreEqual(1, boundary.SubmitCalls);
        Assert.AreEqual(requestId, boundary.LastRequest?.RequestId);
        Assert.AreEqual(requestedAt, boundary.LastRequest?.RequestedAt);
        Assert.AreEqual(CaptureRequestSource.SecondaryInAppCommand, boundary.LastRequest?.RequestSource);
    }

    private sealed class RecordingBoundary : ICaptureRequestBoundary
    {
        public CaptureRequest? LastRequest { get; private set; }

        public int SubmitCalls { get; private set; }

        public CaptureRequestResult Submit(CaptureRequest request)
        {
            LastRequest = request;
            SubmitCalls++;
            return CaptureRequestResult.Accepted(
                request,
                WorkflowState.CaptureRequested,
                "synthetic boundary result");
        }
    }
}
