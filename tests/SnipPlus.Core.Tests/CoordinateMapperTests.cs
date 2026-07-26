using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class CoordinateMapperTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void DipsAreConvertedToVirtualPhysicalBoundsAndSourceCrop()
    {
        var context = new DisplayContextSnapshot("display-v1", "synthetic-monitor", new(100, 200, 1100, 1000), 1.5, 1.5);

        var result = CoordinateMapper.CreateMonitorIntent(
            context,
            new DipRect(10, 20, 100, 120),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UnixEpoch);

        var success = result as CoordinateMappingResult.Success;
        Assert.IsNotNull(success);
        Assert.AreEqual(new PhysicalRect(15, 30, 150, 180), success.Intent.CropBoundsInSource);
        Assert.AreEqual(new PhysicalRect(115, 230, 250, 380), success.Intent.SelectionPhysicalBounds);
        Assert.AreEqual("display-v1", success.Intent.CoordinateVersion);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void RoundingExpandsEdgesOutwardAndPreservesExclusiveBottomRight()
    {
        var context = new DisplayContextSnapshot("display-v1", "synthetic-monitor", new(-100, -50, 900, 950), 1.5, 1.5);

        var result = CoordinateMapper.CreateMonitorIntent(
            context,
            new DipRect(0.1, 0.1, 1.1, 1.1),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UnixEpoch);

        var success = result as CoordinateMappingResult.Success;
        Assert.IsNotNull(success);
        Assert.AreEqual(new PhysicalRect(0, 0, 2, 2), success.Intent.CropBoundsInSource);
        Assert.AreEqual(new PhysicalRect(-100, -50, -98, -48), success.Intent.SelectionPhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void OutOfBoundsSelectionIsRejectedWithNewIntentRecovery()
    {
        var context = new DisplayContextSnapshot("display-v1", "synthetic-monitor", new(0, 0, 100, 100), 1, 1);

        var result = CoordinateMapper.CreateMonitorIntent(
            context,
            new DipRect(90, 90, 110, 110),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UnixEpoch);

        var failure = result as CoordinateMappingResult.FailureResult;
        Assert.IsNotNull(failure);
        Assert.AreEqual(FailureCode.InvalidCoordinateMapping, failure.Failure.Code);
        Assert.AreEqual(FailureRecoverability.RetryNewIntent, failure.Failure.Recoverability);
    }
}
