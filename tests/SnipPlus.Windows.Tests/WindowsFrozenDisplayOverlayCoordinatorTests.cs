using System.Reflection;
using Microsoft.UI.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsFrozenDisplayOverlayCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void OverlayUsesSystemCursorSurfaceWithoutDrawnCrosshairLines()
    {
        var overlaySurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("OverlaySurface", BindingFlags.NonPublic);

        Assert.IsNotNull(overlaySurface);
        Assert.IsNull(overlaySurface.GetField(
            "_crosshairHorizontal",
            BindingFlags.Instance | BindingFlags.NonPublic));
        Assert.IsNull(overlaySurface.GetField(
            "_crosshairVertical",
            BindingFlags.Instance | BindingFlags.NonPublic));

        var canvasField = overlaySurface.GetField(
            "_canvas",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(canvasField);
        Assert.AreEqual("CrosshairCanvas", canvasField.FieldType.Name);

        var cursorProperty = canvasField.FieldType.GetProperty(
            "Cursor",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsNotNull(cursorProperty);
        Assert.AreEqual(typeof(InputCursor), cursorProperty.PropertyType);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void OverlayDeclaresEightLogicalHandlesAndMapsCursors()
    {
        var overlaySurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("OverlaySurface", BindingFlags.NonPublic);

        Assert.IsNotNull(overlaySurface);
        var handlesField = overlaySurface.GetField(
            "_handles",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(handlesField);
        Assert.IsTrue(handlesField.FieldType.IsGenericType);
        Assert.AreEqual(
            typeof(IReadOnlyDictionary<SelectionHitTestKind, Microsoft.UI.Xaml.Shapes.Rectangle>),
            handlesField.FieldType);

        var cursorMapper = overlaySurface.GetMethod(
            "CursorShapeFor",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(cursorMapper);
        Assert.AreEqual(
            InputSystemCursorShape.Cross,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.Outside }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeAll,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.Interior }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeWestEast,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.LeftEdge }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeNorthSouth,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.TopEdge }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeNorthwestSoutheast,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.TopLeftCorner }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeNortheastSouthwest,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.TopRightCorner }));
    }
}
