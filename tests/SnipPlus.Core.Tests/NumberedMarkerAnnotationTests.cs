using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Core;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class NumberedMarkerAnnotationTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void NewSessionStartsAtOneAndCommitAdvancesOnce()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectMarker(editing, sessionId, selection);

        var committed = Commit(editing, sessionId, selection, 20, 20, 1);

        Assert.AreEqual(NumberedMarkerPointerResultKind.Committed, committed.Kind);
        Assert.AreEqual(1, ((NumberedMarkerAnnotationContent)committed.CommittedObject!.Content!).Number);
        Assert.AreEqual(2, editing.ActiveNumberedMarkerNextNumber);
        Assert.AreEqual(new AnnotationRevision(1), editing.CurrentAnnotationRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void SequentialNumbersDoNotRecomputeAfterRemovingAnEarlierMarker()
    {
        var ids = new Queue<AnnotationObjectId>(
        [
            new(Guid.Parse("00000000-0000-0000-0000-000000000101")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000102")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000103"))
        ]);
        var documents = new AnnotationDocumentCoordinator();
        var editing = CreateEditing(documents, out var sessionId, out var selection, () => ids.Dequeue());
        SelectMarker(editing, sessionId, selection);

        var first = Commit(editing, sessionId, selection, 20, 20, 1);
        var second = Commit(editing, sessionId, selection, 40, 20, 1, first.AnnotationRevision);
        var removed = documents.Remove(new RemoveAnnotationObjectRequest(
            sessionId,
            second.AnnotationRevision,
            first.CommittedObject!.ObjectId));
        var third = Commit(editing, sessionId, selection, 60, 20, 1, removed.CurrentDocument!.Revision);

        Assert.AreEqual(NumberedMarkerPointerResultKind.Committed, second.Kind);
        Assert.IsInstanceOfType<AnnotationMutationResult.Succeeded>(removed);
        Assert.AreEqual(2, ((NumberedMarkerAnnotationContent)second.CommittedObject!.Content!).Number);
        Assert.AreEqual(3, ((NumberedMarkerAnnotationContent)third.CommittedObject!.Content!).Number);
        Assert.AreEqual(4, editing.ActiveNumberedMarkerNextNumber);
        Assert.AreEqual(2, third.Document!.Objects.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void SetNextNumberSupportsTenAndDuplicateNumbersWithoutDerivingFromDocument()
    {
        var documents = new AnnotationDocumentCoordinator();
        var editing = CreateEditing(documents, out var sessionId, out var selection);
        SelectMarker(editing, sessionId, selection);

        var setTen = editing.SetNextNumber(
            new SetNextNumberRequest(sessionId, selection.CoordinateVersion, selection.SelectionRevision,
                AnnotationRevision.Initial, 10),
            WorkflowState.Editing,
            selection);
        var ten = Commit(editing, sessionId, selection, 20, 20, 1);
        var setTwo = editing.SetNextNumber(
            new SetNextNumberRequest(sessionId, selection.CoordinateVersion, selection.SelectionRevision,
                ten.AnnotationRevision, 2),
            WorkflowState.Editing,
            selection);
        var duplicate = Commit(editing, sessionId, selection, 40, 20, 1, ten.AnnotationRevision);

        Assert.AreEqual(SetNextNumberResultKind.Succeeded, setTen.Kind);
        Assert.AreEqual(SetNextNumberResultKind.Succeeded, setTwo.Kind);
        Assert.AreEqual(10, ((NumberedMarkerAnnotationContent)ten.CommittedObject!.Content!).Number);
        Assert.AreEqual(2, ((NumberedMarkerAnnotationContent)duplicate.CommittedObject!.Content!).Number);
        Assert.AreEqual(3, editing.ActiveNumberedMarkerNextNumber);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void ChangingNextNumberWhileDraftIsActiveDoesNotRewriteDraftSnapshot()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectMarker(editing, sessionId, selection);
        var started = editing.PointerPressed(Input(sessionId, selection, 20, 20, 1), selection);

        var setNext = editing.SetNextNumber(
            new SetNextNumberRequest(sessionId, selection.CoordinateVersion, selection.SelectionRevision,
                AnnotationRevision.Initial, 10),
            WorkflowState.Editing,
            selection);
        var snapshot = editing.CreatePresentationSnapshot(selection);

        Assert.AreEqual(NumberedMarkerPointerResultKind.DraftStarted, started.Kind);
        Assert.AreEqual(SetNextNumberResultKind.Succeeded, setNext.Kind);
        Assert.AreEqual(1, snapshot.DraftNumberedMarker!.Number);
        Assert.AreEqual(10, snapshot.ActiveNumberedMarkerNextNumber);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void OutsideSelectionStaleRevisionAndCancelDoNotMutateDocument()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectMarker(editing, sessionId, selection);

        var outside = editing.PointerPressed(Input(sessionId, selection, 100, 100, 1), selection);
        var started = editing.PointerPressed(Input(sessionId, selection, 20, 20, 1), selection);
        var stale = editing.PointerMoved(
            Input(sessionId, selection with { SelectionRevision = selection.SelectionRevision + 1 }, 30, 30, 1),
            selection);
        var cancelled = editing.CancelNumberedMarkerDraft(sessionId, selection.CoordinateVersion);

        Assert.AreEqual(NumberedMarkerPointerResultKind.IgnoredOutsideSelection, outside.Kind);
        Assert.AreEqual(NumberedMarkerPointerResultKind.DraftStarted, started.Kind);
        Assert.AreEqual(NumberedMarkerPointerResultKind.StaleSelectionRevision, stale.Kind);
        Assert.AreEqual(NumberedMarkerPointerResultKind.Cancelled, cancelled.Kind);
        Assert.IsEmpty(editing.CreatePresentationSnapshot(selection).Document.Objects);
        Assert.AreEqual(1, editing.ActiveNumberedMarkerNextNumber);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void MaxNumberCommitReturnsOverflowWithoutPartialMutation()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectMarker(editing, sessionId, selection);
        var setMax = editing.SetNextNumber(
            new SetNextNumberRequest(sessionId, selection.CoordinateVersion, selection.SelectionRevision,
                AnnotationRevision.Initial, int.MaxValue),
            WorkflowState.Editing,
            selection);
        var started = editing.PointerPressed(Input(sessionId, selection, 20, 20, 1), selection);
        var overflow = editing.PointerReleased(Input(sessionId, selection, 20, 20, 1), selection);

        Assert.AreEqual(SetNextNumberResultKind.Succeeded, setMax.Kind);
        Assert.AreEqual(NumberedMarkerPointerResultKind.DraftStarted, started.Kind);
        Assert.AreEqual(NumberedMarkerPointerResultKind.NumberOverflow, overflow.Kind);
        Assert.IsEmpty(editing.CreatePresentationSnapshot(selection).Document.Objects);
        Assert.AreEqual(int.MaxValue, editing.ActiveNumberedMarkerNextNumber);
        Assert.IsNotNull(editing.CreatePresentationSnapshot(selection).DraftNumberedMarker);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void SelectionRevisionChangeClearsDraftButRetainsNextNumber()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectMarker(editing, sessionId, selection);
        editing.PointerPressed(Input(sessionId, selection, 20, 20, 1), selection);

        var adjusted = selection with { SelectionRevision = selection.SelectionRevision + 1 };
        editing.UpdateSelection(adjusted);
        var snapshot = editing.CreatePresentationSnapshot(adjusted);

        Assert.IsNull(snapshot.DraftNumberedMarker);
        Assert.AreEqual(1, snapshot.ActiveNumberedMarkerNextNumber);
    }

    private static AnnotationEditingCoordinator CreateEditing(
        out Guid sessionId,
        out SelectionVisualState selection,
        Func<AnnotationObjectId>? objectIdFactory = null) =>
        CreateEditing(new AnnotationDocumentCoordinator(), out sessionId, out selection, objectIdFactory);

    private static AnnotationEditingCoordinator CreateEditing(
        AnnotationDocumentCoordinator documents,
        out Guid sessionId,
        out SelectionVisualState selection,
        Func<AnnotationObjectId>? objectIdFactory = null)
    {
        sessionId = Guid.NewGuid();
        selection = new SelectionVisualState
        {
            SessionId = sessionId,
            CoordinateVersion = "marker-v1",
            SelectionRevision = 1,
            Status = SelectionStatus.Locked,
            InteractionMode = SelectionInteractionMode.Locked,
            IsGeometryValid = true,
            NormalizedPhysicalBounds = new PhysicalRect(0, 0, 80, 80),
            CurrentPhysicalPoint = new PhysicalPoint(1, 1)
        };
        var editing = new AnnotationEditingCoordinator(documents, objectIdFactory: objectIdFactory);
        editing.BeginSession(selection);
        return editing;
    }

    private static void SelectMarker(
        AnnotationEditingCoordinator editing,
        Guid sessionId,
        SelectionVisualState selection)
    {
        var result = editing.SelectTool(
            new EditingToolSelectionRequest(
                sessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                editing.CurrentAnnotationRevision,
                EditingToolKind.NumberedMarker),
            WorkflowState.Editing,
            selection);
        Assert.AreEqual(EditingToolSelectionResultKind.Selected, result.Kind);
    }

    private static NumberedMarkerPointerResult Commit(
        AnnotationEditingCoordinator editing,
        Guid sessionId,
        SelectionVisualState selection,
        int x,
        int y,
        int pointerId,
        AnnotationRevision? revision = null)
    {
        var input = Input(sessionId, selection, x, y, pointerId, revision);
        var started = editing.PointerPressed(input, selection);
        Assert.AreEqual(NumberedMarkerPointerResultKind.DraftStarted, started.Kind);
        return editing.PointerReleased(input, selection);
    }

    private static NumberedMarkerPointerEvent Input(
        Guid sessionId,
        SelectionVisualState selection,
        int x,
        int y,
        int pointerId,
        AnnotationRevision? revision = null) => new(
        sessionId,
        selection.CoordinateVersion,
        selection.SelectionRevision,
        revision ?? new AnnotationRevision(0),
        pointerId,
        new PhysicalPoint(x, y));
}
