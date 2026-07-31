using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Contracts.Tests;

[TestClass]
public sealed class AnnotationToolContractsTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void EditingToolsKeepSelectionOutsideAnnotationToolKinds()
    {
        CollectionAssert.AreEquivalent(
            new[]
            {
                EditingToolKind.Selection,
                EditingToolKind.Rectangle,
                EditingToolKind.ArrowLine,
                EditingToolKind.Highlighter,
                EditingToolKind.Text
            },
            Enum.GetValues<EditingToolKind>());
        Assert.IsFalse(Enum.IsDefined(AnnotationToolKind.Rectangle.GetType(), "Selection"));
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void RectangleStyleRejectsInvisibleColorAndInvalidThickness()
    {
        AssertThrows<ArgumentException>(() =>
            _ = new RectangleAnnotationStyle(new ArgbColor(0, 255, 0, 0), 2));
        AssertThrows<ArgumentOutOfRangeException>(() =>
            _ = new RectangleAnnotationStyle(ArgbColor.Red, 0));
        AssertThrows<ArgumentOutOfRangeException>(() =>
            _ = new RectangleAnnotationStyle(ArgbColor.Red, 65));
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void RectangleContentAndPresentationSnapshotAreImmutableAndTyped()
    {
        var sessionId = Guid.NewGuid();
        var content = new RectangleAnnotationContent(RectangleAnnotationStyle.Default);
        var annotationObject = new AnnotationObject(
            AnnotationObjectId.New(),
            sessionId,
            AnnotationToolKind.Rectangle,
            new PhysicalRect(1, 2, 10, 20),
            0,
            content);
        var document = new AnnotationDocument(
            sessionId,
            AnnotationRevision.Initial,
            new[] { annotationObject });
        var snapshot = new AnnotationPresentationSnapshot(
            sessionId,
            "contracts-v1",
            2,
            document.Revision,
            new PhysicalRect(0, 0, 100, 100),
            EditingToolKind.Rectangle,
            new PhysicalRect(3, 4, 8, 9),
            document);

        Assert.AreSame(content, annotationObject.Content);
        Assert.AreSame(document, snapshot.Document);
        Assert.AreEqual(EditingToolKind.Rectangle, snapshot.ActiveTool);
        Assert.AreEqual(new PhysicalRect(3, 4, 8, 9), snapshot.DraftPhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void ArrowLineContentKeepsEndpointsAndEndStyle()
    {
        var segment = new PhysicalLineSegment(
            new PhysicalPoint(40, 30),
            new PhysicalPoint(10, 20));
        var style = new ArrowLineAnnotationStyle(
            ArgbColor.Red,
            3,
            ArrowLineEndStyle.None);
        var content = new ArrowLineAnnotationContent(segment, style);
        var annotationObject = new AnnotationObject(
            AnnotationObjectId.New(),
            Guid.NewGuid(),
            AnnotationToolKind.ArrowLine,
            segment.Bounds,
            0,
            content);

        Assert.AreEqual(segment, content.Segment);
        Assert.AreEqual(ArrowLineEndStyle.None, content.Style.EndStyle);
        Assert.AreSame(content, annotationObject.Content);
        Assert.AreEqual(segment.Bounds, annotationObject.Geometry);
        Assert.AreEqual(
            new PhysicalRect(10, 30, 40, 31),
            new PhysicalLineSegment(
                new PhysicalPoint(10, 30),
                new PhysicalPoint(40, 30)).Bounds);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void HighlighterStyleIsSemiTransparentAndPathIsImmutable()
    {
        var points = new[]
        {
            new PhysicalPoint(4, 5),
            new PhysicalPoint(12, 8),
            new PhysicalPoint(20, 15)
        };
        var path = new PhysicalPolyline(points);
        var content = new HighlighterStrokeContent(
            path,
            HighlighterAnnotationStyle.Default);

        Assert.IsTrue(content.Style.StrokeColor.A > 0);
        Assert.IsTrue(content.Style.StrokeColor.A < 255);
        Assert.IsTrue(path.HasLength);
        CollectionAssert.AreEqual(points, path.Points.ToArray());
        Assert.IsFalse(path.Points.GetType().IsArray);
        Assert.AreEqual(new PhysicalRect(4, 5, 20, 15), path.Bounds);
        AssertThrows<ArgumentException>(() =>
            _ = new HighlighterAnnotationStyle(new ArgbColor(0, 255, 235, 59), 8));
        AssertThrows<ArgumentException>(() =>
            _ = new HighlighterAnnotationStyle(new ArgbColor(255, 255, 235, 59), 8));
        AssertThrows<ArgumentOutOfRangeException>(() =>
            _ = new HighlighterAnnotationStyle(new ArgbColor(128, 255, 235, 59), 65));
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void RectangleObjectRejectsNonRectangleContent()
    {
        var sessionId = Guid.NewGuid();
        AssertThrows<ArgumentException>(() => _ = new AnnotationObject(
            AnnotationObjectId.New(),
            sessionId,
            AnnotationToolKind.HighlighterStroke,
            new PhysicalRect(1, 2, 10, 20),
            0,
            new RectangleAnnotationContent(RectangleAnnotationStyle.Default)));
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
