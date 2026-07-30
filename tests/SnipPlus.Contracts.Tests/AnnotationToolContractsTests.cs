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
            new[] { EditingToolKind.Selection, EditingToolKind.Rectangle },
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
