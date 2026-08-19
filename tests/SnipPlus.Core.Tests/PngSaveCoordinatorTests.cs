using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Core;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class PngSaveCoordinatorTests
{
    [TestMethod]
    [TestCategory("Core")]
    public void RequestFactoryUsesInvariantTimestampAndCanonicalIdentity()
    {
        var sessionId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var imageResult = new TestImageResult(resultId, sessionId);
        var clock = new PngSaveRequestFactory(() =>
            new DateTimeOffset(2026, 8, 19, 7, 6, 5, TimeSpan.Zero));

        var preparation = clock.Prepare(imageResult);

        var ready = (PngSavePreparationResult.Ready)preparation;
        Assert.AreEqual(sessionId, ready.Request.SessionId);
        Assert.AreEqual(resultId, ready.Request.ResultId);
        Assert.AreEqual("SnipPlus_2026-08-19_070605.png", ready.Request.SuggestedFileName);
        Assert.IsTrue(PngSaveFileNamePolicy.IsPngFileName("capture.PNG"));
        Assert.IsFalse(PngSaveFileNamePolicy.IsPngFileName("capture.jpg"));
    }

    [TestMethod]
    [TestCategory("Core")]
    public async Task CoordinatorDelegatesOnlyAValidatedCanonicalRequest()
    {
        var sessionId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var imageResult = new TestImageResult(resultId, sessionId);
        var platform = new FakePngSavePlatformService();
        var coordinator = new PngSaveCoordinator(
            platform,
            new PngSaveRequestFactory(() => DateTimeOffset.UnixEpoch));

        var result = await coordinator.SaveAsync(imageResult, CancellationToken.None);

        Assert.IsInstanceOfType<PngSaveResult.Saved>(result);
        Assert.AreEqual(1, platform.CallCount);
        Assert.IsNotNull(platform.Request);
        Assert.AreEqual(sessionId, platform.Request!.SessionId);
        Assert.AreEqual(resultId, platform.Request.ResultId);
        Assert.AreSame(imageResult, platform.Request.ImageResult);
        Assert.AreEqual("SnipPlus_1970-01-01_000000.png", platform.Request.SuggestedFileName);
    }

    [TestMethod]
    [TestCategory("Core")]
    public async Task DisposedCanonicalResultIsRejectedWithoutPlatformCall()
    {
        var imageResult = new TestImageResult();
        imageResult.Dispose();
        var platform = new FakePngSavePlatformService();
        var coordinator = new PngSaveCoordinator(platform);

        var result = await coordinator.SaveAsync(imageResult, CancellationToken.None);

        var rejected = (PngSaveResult.Rejected)result;
        Assert.AreEqual(FailureCode.InvalidResultLifetime, rejected.Failure.Code);
        Assert.AreEqual(0, platform.CallCount);
    }

    [TestMethod]
    [TestCategory("Core")]
    public async Task MismatchedPngEvidenceIsRejectedByCoreBoundary()
    {
        var sessionId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        var platform = new FakePngSavePlatformService(mismatchEvidence: true);
        var coordinator = new PngSaveCoordinator(
            platform,
            new PngSaveRequestFactory(() => DateTimeOffset.UnixEpoch));

        var result = await coordinator.SaveAsync(
            new TestImageResult(resultId, sessionId),
            CancellationToken.None);

        var rejected = (PngSaveResult.Rejected)result;
        Assert.AreEqual(FailureCode.InvalidResultLifetime, rejected.Failure.Code);
        Assert.AreEqual(1, platform.CallCount);
    }

    private sealed class FakePngSavePlatformService : IPngSavePlatformService
    {
        private readonly bool _mismatchEvidence;

        public FakePngSavePlatformService(bool mismatchEvidence = false)
        {
            _mismatchEvidence = mismatchEvidence;
        }

        public int CallCount { get; private set; }

        public PngSaveRequest? Request { get; private set; }

        public ValueTask<PngSaveResult> SaveAsync(
            PngSaveRequest request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            Request = request;
            return ValueTask.FromResult<PngSaveResult>(new PngSaveResult.Saved(
                request.SessionId,
                request.ResultId,
                "synthetic-target.png",
                new PngWriteSucceededEvidence
                {
                    SessionId = _mismatchEvidence ? Guid.NewGuid() : request.SessionId,
                    ResultId = _mismatchEvidence ? Guid.NewGuid() : request.ResultId
                }));
        }
    }
}
