using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class NumberedMarkerPresentationTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void FunctionBarDeclaresNumberedMarkerToolAndAccessibleNextNumberControl()
    {
        var functionBar = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("FunctionBarSurface", BindingFlags.NonPublic)!;
        var tools = functionBar.GetField("_toolButtons", BindingFlags.Instance | BindingFlags.NonPublic);
        var numberBox = functionBar.GetField("_nextNumberBox", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(tools);
        Assert.IsNotNull(numberBox);
        Assert.AreEqual(typeof(Microsoft.UI.Xaml.Controls.NumberBox), numberBox.FieldType);
        Assert.IsTrue(Enum.IsDefined(EditingToolKind.NumberedMarker));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void OverlayDeclaresSeparateNumberedMarkerPreviewAndPresentationMethod()
    {
        var overlay = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("OverlaySurface", BindingFlags.NonPublic)!;
        var previews = overlay.GetField(
            "_numberedMarkerPreviews",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var add = overlay.GetMethod(
            "AddNumberedMarkerPreview",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.AreEqual(
            typeof(List<Microsoft.UI.Xaml.FrameworkElement>),
            previews!.FieldType);
        Assert.IsNotNull(add);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void MarkerPreviewGeometryClipsToSelectionByContract()
    {
        var style = NumberedMarkerAnnotationStyle.Default;
        var markerBounds = NumberedMarkerAnnotationContent.GetBounds(
            new PhysicalPoint(10, 10),
            style);
        var selection = new PhysicalRect(0, 0, 10, 10);
        var visible = markerBounds
            .Intersection(new PhysicalRect(0, 0, 20, 20))
            .Intersection(selection);

        Assert.IsTrue(markerBounds.IsPositive);
        Assert.AreEqual(new PhysicalRect(0, 0, 10, 10), visible);
    }
}
