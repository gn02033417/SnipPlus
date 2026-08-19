using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Contracts.Tests;

[TestClass]
public sealed class PngSaveContractsTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void SavedResultCarriesOnlyMatchingPngWriteEvidence()
    {
        var sessionId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var evidence = new PngWriteSucceededEvidence
        {
            SessionId = sessionId,
            ResultId = resultId
        };

        var result = new PngSaveResult.Saved(
            sessionId,
            resultId,
            "synthetic-target.png",
            evidence);

        Assert.AreEqual(sessionId, result.PngWrite.SessionId);
        Assert.AreEqual(resultId, result.PngWrite.ResultId);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void CancelAndFailureResultsAreDistinctTypedOutcomes()
    {
        var sessionId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var failure = Failure.Create(
            FailureCode.OutputWriteFailed,
            FailureCategory.IO,
            FailureRecoverability.RetryNewIntent,
            "synthetic-save",
            resultId,
            "synthetic failure",
            occurredAt: DateTimeOffset.UnixEpoch);

        PngSaveResult cancelled = new PngSaveResult.SaveDialogCancelled(
            sessionId,
            resultId,
            "UserCancelledSaveDialog");
        PngSaveResult encodingFailed = new PngSaveResult.PngEncodingFailed(
            sessionId,
            resultId,
            failure);
        PngSaveResult writeFailed = new PngSaveResult.PngWriteFailed(
            sessionId,
            resultId,
            failure);

        Assert.IsInstanceOfType<PngSaveResult.SaveDialogCancelled>(cancelled);
        Assert.IsInstanceOfType<PngSaveResult.PngEncodingFailed>(encodingFailed);
        Assert.IsInstanceOfType<PngSaveResult.PngWriteFailed>(writeFailed);
    }
}
