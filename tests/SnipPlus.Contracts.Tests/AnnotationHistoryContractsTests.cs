using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Contracts.Tests;

[TestClass]
public sealed class AnnotationHistoryContractsTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void HistoryEntryCapturesImmutableObjectAndNumberTransitions()
    {
        var sessionId = Guid.NewGuid();
        var annotationObject = CreateObject(sessionId, AnnotationToolKind.NumberedMarker, 3);
        var entry = new AnnotationHistoryEntry(
            sessionId,
            "display-v1",
            4,
            AnnotationHistoryEntryKind.NumberedMarkerCreation,
            afterObject: annotationObject,
            beforeNextNumber: 3,
            afterNextNumber: 4);

        Assert.AreEqual(sessionId, entry.SessionId);
        Assert.AreEqual(AnnotationHistoryEntryKind.NumberedMarkerCreation, entry.Kind);
        Assert.AreEqual(annotationObject.ObjectId, entry.AffectedObjectId);
        Assert.AreEqual(3, entry.BeforeNextNumber);
        Assert.AreEqual(4, entry.AfterNextNumber);

        var state = new AnnotationHistoryState(
            sessionId,
            "display-v1",
            4,
            AnnotationRevision.Initial,
            4,
            new[] { entry },
            Array.Empty<AnnotationHistoryEntry>(),
            true,
            false);
        Assert.AreEqual(1, state.UndoEntries.Count);
        Assert.IsTrue(state.CanUndo);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void InvalidHistoryEntryShapeIsRejected()
    {
        var sessionId = Guid.NewGuid();
        var annotationObject = CreateObject(sessionId, AnnotationToolKind.Rectangle, 1);

        AssertArgumentException(() =>
        {
            _ = new AnnotationHistoryEntry(
                sessionId,
                "display-v1",
                0,
                AnnotationHistoryEntryKind.Add,
                beforeObject: annotationObject,
                afterObject: annotationObject);
        });
        AssertArgumentException(() =>
        {
            _ = new AnnotationHistoryEntry(
                sessionId,
                "display-v1",
                0,
                AnnotationHistoryEntryKind.Replace,
                beforeObject: annotationObject,
                afterObject: CreateObject(sessionId, AnnotationToolKind.Rectangle, 2));
        });
    }

    private static AnnotationObject CreateObject(
        Guid sessionId,
        AnnotationToolKind toolKind,
        int zOrder) => new(
        AnnotationObjectId.New(),
        sessionId,
        toolKind,
        new PhysicalRect(zOrder, zOrder, zOrder + 2, zOrder + 2),
        zOrder,
        toolKind == AnnotationToolKind.NumberedMarker
            ? new NumberedMarkerAnnotationContent(zOrder, NumberedMarkerAnnotationStyle.Default)
            : null);

    private static void AssertArgumentException(Action action)
    {
        try
        {
            action();
        }
        catch (ArgumentException)
        {
            return;
        }

        Assert.Fail("Expected an ArgumentException.");
    }
}
