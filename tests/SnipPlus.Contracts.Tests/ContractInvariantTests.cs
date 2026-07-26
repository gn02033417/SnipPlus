using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Contracts.Tests;

[TestClass]
public sealed class ContractInvariantTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void ClipboardDeliveryDefaultsKeepHistoryAndRoamingDisabled()
    {
        var request = new ClipboardDeliveryRequest
        {
            DeliveryId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            ResultId = Guid.NewGuid(),
            ImageResult = new NoopImageResult()
        };

        Assert.IsFalse(request.HistoryAllowed);
        Assert.IsFalse(request.RoamingAllowed);
        Assert.AreEqual(5, request.MaximumAttempts);
        Assert.AreEqual(TimeSpan.FromSeconds(1), request.RetryBudget);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void FailureContractPreservesStableCodeCategoryAndRecovery()
    {
        var correlationId = Guid.NewGuid();

        var failure = Failure.Create(
            FailureCode.ClipboardBusy,
            FailureCategory.Contention,
            FailureRecoverability.RetrySameIntent,
            "synthetic-operation",
            correlationId,
            "redacted synthetic diagnostic",
            occurredAt: DateTimeOffset.UnixEpoch);

        Assert.AreEqual(FailureCode.ClipboardBusy, failure.Code);
        Assert.AreEqual(FailureCategory.Contention, failure.Category);
        Assert.AreEqual(FailureRecoverability.RetrySameIntent, failure.Recoverability);
        Assert.AreEqual(correlationId, failure.CorrelationId);
        Assert.AreEqual(DateTimeOffset.UnixEpoch, failure.OccurredAt);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void CanonicalImageMetadataIsExplicit()
    {
        var metadata = new ImageResultMetadata
        {
            ResultId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            PixelWidth = 4,
            PixelHeight = 3,
            PixelFormat = ImagePixelFormat.Bgra8,
            AlphaMode = ImageAlphaMode.Premultiplied,
            ColorSpace = ImageColorSpace.SrgbSdr,
            DpiX = 96,
            DpiY = 96,
            RowStride = 16,
            SourceKind = SourceKind.Monitor,
            SourcePhysicalBounds = new PhysicalRect(-4, -3, 4, 3),
            CropPhysicalBounds = new PhysicalRect(0, 0, 4, 3),
            CapturedAt = DateTimeOffset.UnixEpoch
        };

        Assert.AreEqual(ImagePixelFormat.Bgra8, metadata.PixelFormat);
        Assert.AreEqual(ImageAlphaMode.Premultiplied, metadata.AlphaMode);
        Assert.AreEqual(ImageColorSpace.SrgbSdr, metadata.ColorSpace);
        Assert.AreEqual(16, metadata.RowStride);
    }

    private sealed class NoopImageResult : IImageResult
    {
        public ImageResultMetadata Metadata => throw new NotSupportedException();

        public bool IsDisposed => false;

        public IImageResultLease AcquireLease() => throw new NotSupportedException();

        public void Dispose()
        {
        }
    }
}
