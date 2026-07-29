using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class FunctionBarPlacementServiceTests
{
    private static readonly Guid SessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void Stage6BEnablesCancelAndDisablesFutureOutputAndHistoryCommands()
    {
        var availability = FunctionBarCommandAvailability.Stage6B;

        Assert.IsFalse(availability.IsEnabled(FunctionBarCommand.Complete));
        Assert.IsFalse(availability.IsEnabled(FunctionBarCommand.Save));
        Assert.IsTrue(availability.IsEnabled(FunctionBarCommand.Cancel));
        Assert.IsFalse(availability.IsEnabled(FunctionBarCommand.Undo));
        Assert.IsFalse(availability.IsEnabled(FunctionBarCommand.Redo));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void PlacesBarBelowSelectionWhenBelowWorkAreaHasRoom()
    {
        var result = Place(
            new PhysicalRect(400, 400, 600, 600),
            new PhysicalRect(0, 0, 1000, 1000),
            new PhysicalRect(0, 0, 1000, 1000),
            new PhysicalPixelSize(100, 40));

        var ready = AssertReady(result);
        Assert.AreEqual(FunctionBarPlacementSide.Below, ready.PlacementSide);
        Assert.AreEqual(new PhysicalRect(450, 608, 550, 648), ready.FunctionBarPhysicalBounds);
        Assert.IsTrue(ready.IsFullyInsideWorkArea);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void UsesAboveWhenBelowWorkAreaCannotFit()
    {
        var result = Place(
            new PhysicalRect(400, 900, 600, 980),
            new PhysicalRect(0, 0, 1000, 1000),
            new PhysicalRect(0, 0, 1000, 1000),
            new PhysicalPixelSize(100, 40));

        var ready = AssertReady(result);
        Assert.AreEqual(FunctionBarPlacementSide.Above, ready.PlacementSide);
        Assert.AreEqual(new PhysicalRect(450, 852, 550, 892), ready.FunctionBarPhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void ClampsBarHorizontallyAndSupportsNegativeCoordinatesAndMixedDpi()
    {
        var result = Place(
            new PhysicalRect(-2550, 100, -2450, 400),
            new PhysicalRect(-2560, 0, 0, 1440),
            new PhysicalRect(-2560, 0, 0, 1400),
            new PhysicalPixelSize(225, 60),
            dpiScale: 1.5);

        var ready = AssertReady(result);
        Assert.AreEqual("left", ready.DisplayId);
        Assert.AreEqual(FunctionBarPlacementSide.ClampedBelow, ready.PlacementSide);
        Assert.AreEqual(new PhysicalRect(-2560, 408, -2335, 468), ready.FunctionBarPhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void ChoosesAnchorByIntersectionThenCurrentPointThenStableDisplayId()
    {
        var displays = new[]
        {
            new FunctionBarDisplayWorkArea(
                "z-display",
                new PhysicalRect(0, 0, 500, 500),
                new PhysicalRect(0, 0, 500, 500),
                1,
                1),
            new FunctionBarDisplayWorkArea(
                "a-display",
                new PhysicalRect(500, 0, 1000, 500),
                new PhysicalRect(500, 0, 1000, 500),
                1,
                1)
        };

        var tieById = PlaceWithDisplays(
            new PhysicalRect(400, 100, 600, 300),
            displays,
            new PhysicalPoint(450, 200));
        Assert.AreEqual("z-display", AssertReady(tieById).DisplayId);

        var tieByPoint = PlaceWithDisplays(
            new PhysicalRect(400, 100, 600, 300),
            displays,
            new PhysicalPoint(550, 200));
        Assert.AreEqual("a-display", AssertReady(tieByPoint).DisplayId);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void RejectsWorkAreaOutsideDisplayBounds()
    {
        var result = Place(
            new PhysicalRect(100, 100, 200, 200),
            new PhysicalRect(0, 0, 500, 500),
            new PhysicalRect(-1, 0, 500, 500),
            new PhysicalPixelSize(100, 40));

        var failure = result as FunctionBarPlacementOutcome.Failed;
        Assert.IsNotNull(failure);
        Assert.AreEqual(FailureCode.InvalidWorkArea, failure.Failure.Code);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void GapDoesNotBecomeAnAnchorDisplay()
    {
        var result = PlaceWithDisplays(
            new PhysicalRect(450, 100, 550, 300),
            new[]
            {
                new FunctionBarDisplayWorkArea(
                    "left",
                    new PhysicalRect(0, 0, 400, 500),
                    new PhysicalRect(0, 0, 400, 500),
                    1,
                    1),
                new FunctionBarDisplayWorkArea(
                    "right",
                    new PhysicalRect(600, 0, 1000, 500),
                    new PhysicalRect(600, 0, 1000, 500),
                    1,
                    1)
            },
            new PhysicalPoint(500, 200));

        Assert.IsInstanceOfType<FunctionBarPlacementOutcome.Failed>(result);
        Assert.AreEqual(
            FailureCode.InvalidSelection,
            ((FunctionBarPlacementOutcome.Failed)result).Failure.Code);
    }

    private static FunctionBarPlacementOutcome Place(
        PhysicalRect selection,
        PhysicalRect displayBounds,
        PhysicalRect workArea,
        PhysicalPixelSize size,
        double dpiScale = 1) => PlaceWithDisplays(
        selection,
        new[]
        {
            new FunctionBarDisplayWorkArea(
                "left",
                displayBounds,
                workArea,
                dpiScale,
                dpiScale)
        },
        null,
        size);

    private static FunctionBarPlacementOutcome PlaceWithDisplays(
        PhysicalRect selection,
        IReadOnlyList<FunctionBarDisplayWorkArea> displays,
        PhysicalPoint? currentPoint,
        PhysicalPixelSize size = default)
    {
        if (!size.IsPositive)
        {
            size = new PhysicalPixelSize(100, 40);
        }

        return new FunctionBarPlacementService().Place(new FunctionBarPlacementRequest(
            SessionId,
            "stage6b-v1",
            3,
            selection,
            displays,
            size,
            MarginPixels: 8,
            currentPoint));
    }

    private static FunctionBarPlacementResult AssertReady(FunctionBarPlacementOutcome outcome)
    {
        Assert.IsInstanceOfType<FunctionBarPlacementOutcome.Ready>(outcome);
        return ((FunctionBarPlacementOutcome.Ready)outcome).Placement;
    }
}
