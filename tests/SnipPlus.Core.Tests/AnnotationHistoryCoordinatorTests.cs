using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class AnnotationHistoryCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void AddUndoRedoRestoresExactObjectAndIncrementsRevisionOncePerReplay()
    {
        var (documents, history, sessionId) = CreateHistory();
        var objectBefore = CreateObject(sessionId, AnnotationToolKind.Rectangle, 7);
        var before = documents.Current!;
        var after = Success(documents.Add(new AddAnnotationObjectRequest(
            sessionId, before.Revision, objectBefore))).Document;
        history.RecordAdd(sessionId, "display-v1", 0, before, after, objectBefore);

        var undone = history.Execute(Request(sessionId, after.Revision, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, undone.Kind);
        Assert.IsEmpty(documents.Current!.Objects);
        Assert.AreEqual(2, documents.Current.Revision.Value);
        Assert.IsFalse(history.CurrentState.CanUndo);
        Assert.IsTrue(history.CurrentState.CanRedo);

        var redone = history.Execute(Request(sessionId, documents.Current.Revision, AnnotationHistoryCommand.Redo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, redone.Kind);
        Assert.AreEqual(objectBefore, documents.Current!.Objects.Single());
        Assert.AreEqual(3, documents.Current.Revision.Value);
        Assert.IsFalse(history.CurrentState.CanUndo && history.CurrentState.CanRedo);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void ReplaceAndRemoveReplayByObjectIdAndPreserveZOrder()
    {
        var (documents, history, sessionId) = CreateHistory();
        var original = CreateObject(sessionId, AnnotationToolKind.Rectangle, 1);
        var beforeAdd = documents.Current!;
        var afterAdd = Success(documents.Add(new AddAnnotationObjectRequest(
            sessionId, beforeAdd.Revision, original))).Document;
        history.RecordAdd(sessionId, "display-v1", 0, beforeAdd, afterAdd, original);

        var replacement = new AnnotationObject(
            original.ObjectId,
            sessionId,
            AnnotationToolKind.ArrowLine,
            new PhysicalRect(10, 11, 30, 31),
            original.ZOrder);
        var afterReplace = Success(documents.Replace(new ReplaceAnnotationObjectRequest(
            sessionId, afterAdd.Revision, replacement))).Document;
        history.RecordReplace(sessionId, "display-v1", 0, afterAdd, afterReplace, original, replacement);

        var undone = history.Execute(Request(sessionId, afterReplace.Revision, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, undone.Kind);
        Assert.AreEqual(original, documents.Current!.Objects.Single());

        var redone = history.Execute(Request(sessionId, documents.Current.Revision, AnnotationHistoryCommand.Redo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, redone.Kind);
        Assert.AreEqual(replacement, documents.Current!.Objects.Single());

        var beforeRemove = documents.Current;
        var afterRemove = Success(documents.Remove(new RemoveAnnotationObjectRequest(
            sessionId, beforeRemove.Revision, original.ObjectId))).Document;
        history.RecordRemove(sessionId, "display-v1", 0, beforeRemove, afterRemove, replacement);
        var removeUndo = history.Execute(Request(sessionId, afterRemove.Revision, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, removeUndo.Kind);
        Assert.AreEqual(replacement, documents.Current!.Objects.Single());
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void NextNumberOnlyHistoryDoesNotChangeAnnotationRevision()
    {
        var (documents, history, sessionId) = CreateHistory();
        history.RecordNextNumber(
            sessionId,
            "display-v1",
            0,
            documents.Current!,
            1,
            9);

        var undo = history.Execute(Request(sessionId, AnnotationRevision.Initial, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, undo.Kind);
        Assert.AreEqual(AnnotationRevision.Initial, documents.Current!.Revision);
        Assert.AreEqual(1, undo.CurrentNextNumber);

        var redo = history.Execute(Request(sessionId, documents.Current.Revision, AnnotationHistoryCommand.Redo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, redo.Kind);
        Assert.AreEqual(AnnotationRevision.Initial, documents.Current.Revision);
        Assert.AreEqual(9, redo.CurrentNextNumber);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void NumberedMarkerCreationIsOneAtomicHistoryEntry()
    {
        var (documents, history, sessionId) = CreateHistory();
        var marker = CreateObject(sessionId, AnnotationToolKind.NumberedMarker, 2);
        var before = documents.Current!;
        var after = Success(documents.Add(new AddAnnotationObjectRequest(
            sessionId, before.Revision, marker))).Document;
        history.RecordNumberedMarkerCreation(sessionId, "display-v1", 0,
            before, after, marker, 1, 2);

        Assert.AreEqual(1, history.CurrentState.UndoEntries.Count);
        var undo = history.Execute(Request(sessionId, after.Revision, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, undo.Kind);
        Assert.AreEqual(1, undo.CurrentNextNumber);
        Assert.IsEmpty(documents.Current!.Objects);

        var redo = history.Execute(Request(sessionId, documents.Current.Revision, AnnotationHistoryCommand.Redo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, redo.Kind);
        Assert.AreEqual(2, redo.CurrentNextNumber);
        Assert.AreEqual(marker, documents.Current!.Objects.Single());
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void NewForwardMutationInvalidatesRedoButSelectionUpdatesDoNot()
    {
        var (documents, history, sessionId) = CreateHistory();
        var first = CreateObject(sessionId, AnnotationToolKind.Rectangle, 1);
        var before = documents.Current!;
        var after = Success(documents.Add(new AddAnnotationObjectRequest(
            sessionId, before.Revision, first))).Document;
        history.RecordAdd(sessionId, "display-v1", 0, before, after, first);
        var undo = history.Execute(Request(sessionId, after.Revision, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.Succeeded, undo.Kind);

        history.UpdateSelection(sessionId, "display-v1", 1);
        Assert.IsTrue(history.CurrentState.CanRedo);

        var second = CreateObject(sessionId, AnnotationToolKind.HighlighterStroke, 2);
        var secondBefore = documents.Current!;
        var secondAfter = Success(documents.Add(new AddAnnotationObjectRequest(
            sessionId, secondBefore.Revision, second))).Document;
        history.RecordAdd(sessionId, "display-v1", 1, secondBefore, secondAfter, second);
        Assert.IsFalse(history.CurrentState.CanRedo);
        Assert.IsTrue(history.CurrentState.CanUndo);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void StaleActiveDraftAndConflictLeaveStacksAndDocumentUnchanged()
    {
        var (documents, history, sessionId) = CreateHistory();
        var annotationObject = CreateObject(sessionId, AnnotationToolKind.Rectangle, 4);
        var before = documents.Current!;
        var after = Success(documents.Add(new AddAnnotationObjectRequest(
            sessionId, before.Revision, annotationObject))).Document;
        history.RecordAdd(sessionId, "display-v1", 0, before, after, annotationObject);

        var activeDraft = history.Execute(Request(sessionId, after.Revision, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, true);
        Assert.AreEqual(AnnotationHistoryResultKind.ActiveDraft, activeDraft.Kind);
        Assert.AreEqual(after, documents.Current);
        Assert.IsTrue(history.CurrentState.CanUndo);

        var replacement = new AnnotationObject(
            annotationObject.ObjectId,
            sessionId,
            AnnotationToolKind.Rectangle,
            new PhysicalRect(20, 20, 30, 30),
            annotationObject.ZOrder);
        var changed = Success(documents.Replace(new ReplaceAnnotationObjectRequest(
            sessionId, after.Revision, replacement))).Document;
        var conflict = history.Execute(Request(sessionId, changed.Revision, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.ObjectConflict, conflict.Kind);
        Assert.AreEqual(changed, documents.Current);
        Assert.IsTrue(history.CurrentState.CanUndo);

        var stale = history.Execute(Request(sessionId, AnnotationRevision.Initial, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.StaleAnnotationRevision, stale.Kind);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void SessionClearMakesHistoryEmptyAndNewSessionStartsEmpty()
    {
        var (documents, history, sessionId) = CreateHistory();
        var annotationObject = CreateObject(sessionId, AnnotationToolKind.Rectangle, 1);
        var before = documents.Current!;
        var after = Success(documents.Add(new AddAnnotationObjectRequest(
            sessionId, before.Revision, annotationObject))).Document;
        history.RecordAdd(sessionId, "display-v1", 0, before, after, annotationObject);

        history.ClearSession(sessionId);
        documents.ClearSession(sessionId);
        Assert.IsFalse(history.HasActiveSession);
        Assert.IsEmpty(history.CurrentState.UndoEntries);
        Assert.IsEmpty(history.CurrentState.RedoEntries);
        var stale = history.Execute(Request(sessionId, AnnotationRevision.Initial, AnnotationHistoryCommand.Undo),
            WorkflowState.Editing, false);
        Assert.AreEqual(AnnotationHistoryResultKind.StaleSession, stale.Kind);
    }

    private static (AnnotationDocumentCoordinator Documents, AnnotationHistoryCoordinator History, Guid SessionId)
        CreateHistory()
    {
        var sessionId = Guid.NewGuid();
        var documents = new AnnotationDocumentCoordinator();
        documents.BeginSession(sessionId);
        var history = new AnnotationHistoryCoordinator(documents);
        history.BeginSession(sessionId, "display-v1", 0);
        return (documents, history, sessionId);
    }

    private static AnnotationHistoryRequest Request(
        Guid sessionId,
        AnnotationRevision revision,
        AnnotationHistoryCommand command) => new(
        sessionId,
        "display-v1",
        0,
        revision,
        command);

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

    private static AnnotationMutationResult.Succeeded Success(AnnotationMutationResult result) =>
        result as AnnotationMutationResult.Succeeded
        ?? throw new AssertFailedException($"Expected success, got {result.GetType().Name}.");
}
