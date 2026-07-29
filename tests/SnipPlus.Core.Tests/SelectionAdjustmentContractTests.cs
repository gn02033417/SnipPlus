using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class SelectionAdjustmentContractTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void HitTestReturnsOutsideInteriorEdgesAndCornerPriority()
    {
        var bounds = new PhysicalRect(0, 0, 100, 100);

        Assert.AreEqual(
            SelectionHitTestKind.Outside,
            SelectionHitTesting.HitTest(bounds, new PhysicalPoint(-20, 50)).Kind);
        Assert.AreEqual(
            SelectionHitTestKind.TopLeftCorner,
            SelectionHitTesting.HitTest(bounds, new PhysicalPoint(2, 2)).Kind);
        Assert.AreEqual(
            SelectionHitTestKind.TopEdge,
            SelectionHitTesting.HitTest(bounds, new PhysicalPoint(50, 2)).Kind);
        Assert.AreEqual(
            SelectionHitTestKind.RightEdge,
            SelectionHitTesting.HitTest(bounds, new PhysicalPoint(98, 50)).Kind);
        Assert.AreEqual(
            SelectionHitTestKind.RightEdge,
            SelectionHitTesting.HitTest(bounds, new PhysicalPoint(100, 50)).Kind);
        Assert.AreEqual(
            SelectionHitTestKind.BottomRightCorner,
            SelectionHitTesting.HitTest(bounds, new PhysicalPoint(100, 100)).Kind);
        Assert.AreEqual(
            SelectionHitTestKind.Interior,
            SelectionHitTesting.HitTest(bounds, new PhysicalPoint(50, 50)).Kind);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void HitTestRecognizesAllEightResizeHandles()
    {
        var bounds = new PhysicalRect(0, 0, 100, 100);
        var expected = new Dictionary<PhysicalPoint, SelectionHitTestKind>
        {
            [new(2, 50)] = SelectionHitTestKind.LeftEdge,
            [new(50, 2)] = SelectionHitTestKind.TopEdge,
            [new(98, 50)] = SelectionHitTestKind.RightEdge,
            [new(50, 98)] = SelectionHitTestKind.BottomEdge,
            [new(2, 2)] = SelectionHitTestKind.TopLeftCorner,
            [new(98, 2)] = SelectionHitTestKind.TopRightCorner,
            [new(2, 98)] = SelectionHitTestKind.BottomLeftCorner,
            [new(98, 98)] = SelectionHitTestKind.BottomRightCorner
        };

        foreach (var pair in expected)
        {
            var result = SelectionHitTesting.HitTest(bounds, pair.Key);
            Assert.AreEqual(pair.Value, result.Kind, pair.Key.ToString());
            Assert.IsTrue(result.IsResizeHandle);
        }
    }
}
