using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Contracts.Tests;

[TestClass]
public sealed class NumberedMarkerContractsTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void NumberedMarkerContentRequiresPositiveNumberAndKeepsStyleImmutable()
    {
        var style = new NumberedMarkerAnnotationStyle(ArgbColor.Red, 32);
        var content = new NumberedMarkerAnnotationContent(7, style);

        Assert.AreEqual(7, content.Number);
        Assert.AreSame(style, content.Style);
        AssertThrows<ArgumentOutOfRangeException>(
            () => _ = new NumberedMarkerAnnotationContent(0, style));
        AssertThrows<ArgumentOutOfRangeException>(
            () => _ = new NumberedMarkerAnnotationStyle(ArgbColor.Red, NumberedMarkerAnnotationStyle.MaxSize + 1));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void NumberedMarkerBoundsAreDeterministicPhysicalPixels()
    {
        var style = new NumberedMarkerAnnotationStyle(ArgbColor.Red, 24);

        var bounds = NumberedMarkerAnnotationContent.GetBounds(
            new PhysicalPoint(-100, 50),
            style);

        Assert.AreEqual(new PhysicalRect(-112, 38, -88, 62), bounds);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void NumberedMarkerObjectRequiresTypedContent()
    {
        var sessionId = Guid.NewGuid();
        var bounds = NumberedMarkerAnnotationContent.GetBounds(
            new PhysicalPoint(20, 20),
            NumberedMarkerAnnotationStyle.Default);

        AssertThrows<ArgumentException>(() => _ = new AnnotationObject(
            AnnotationObjectId.New(),
            sessionId,
            AnnotationToolKind.NumberedMarker,
            bounds,
            0));

        var marker = new AnnotationObject(
            AnnotationObjectId.New(),
            sessionId,
            AnnotationToolKind.NumberedMarker,
            bounds,
            0,
            new NumberedMarkerAnnotationContent(
                1,
                NumberedMarkerAnnotationStyle.Default));
        Assert.AreEqual(AnnotationToolKind.NumberedMarker, marker.ToolKind);
    }

    private static void AssertThrows<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        Assert.Fail($"Expected {typeof(TException).Name}.");
    }
}
