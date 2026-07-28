using System.Reflection;
using Microsoft.UI.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
}
