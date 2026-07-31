using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Core;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class AnnotationEditingCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void EditingStartsInSelectionMode()
    {
        var editing = CreateEditing(out var sessionId, out var selection);

        Assert.AreEqual(EditingToolKind.Selection, editing.ActiveTool);
        Assert.AreEqual(selection.SelectionRevision, editing.CurrentSelectionRevision);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CreatePresentationSnapshot(selection).AnnotationRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void ToolSelectionRequiresCurrentSessionAndRevisions()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        var request = new EditingToolSelectionRequest(
            sessionId,
            "annotation-v1",
            selection.SelectionRevision,
            AnnotationRevision.Initial,
            EditingToolKind.Rectangle);

        var selected = editing.SelectTool(request, WorkflowState.Editing, selection);
        var staleSelection = editing.SelectTool(
            request with { SelectionRevision = selection.SelectionRevision + 1 },
            WorkflowState.Editing,
            selection);
        var staleSession = editing.SelectTool(
            request with { SessionId = Guid.NewGuid() },
            WorkflowState.Editing,
            selection);
        var staleAnnotation = editing.SelectTool(
            request with { ExpectedAnnotationRevision = new AnnotationRevision(1) },
            WorkflowState.Editing,
            selection);

        Assert.AreEqual(EditingToolSelectionResultKind.Selected, selected.Kind);
        Assert.AreEqual(EditingToolKind.Rectangle, editing.ActiveTool);
        Assert.AreEqual(EditingToolSelectionResultKind.StaleSelectionRevision, staleSelection.Kind);
        Assert.AreEqual(EditingToolSelectionResultKind.StaleSession, staleSession.Kind);
        Assert.AreEqual(EditingToolSelectionResultKind.StaleAnnotationRevision, staleAnnotation.Kind);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void TextClickInsideSelectionStartsDraftWithoutDocumentRevision()
    {
        var draftId = Guid.Parse("00000000-0000-0000-0000-000000000010");
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectText(editing, sessionId, selection);

        var result = editing.BeginTextDraft(
            TextInput(sessionId, selection, new PhysicalPoint(10, 12), draftId),
            selection);

        Assert.AreEqual(TextDraftResultKind.DraftStarted, result.Kind);
        Assert.AreEqual(draftId, result.Request!.DraftId);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
        Assert.IsEmpty(result.Document!.Objects);
        Assert.IsNotNull(editing.CreatePresentationSnapshot(selection).DraftText);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void TextOutsideSelectionAndStaleRequestDoNotCreateDraft()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectText(editing, sessionId, selection);

        var outside = editing.BeginTextDraft(
            TextInput(sessionId, selection, new PhysicalPoint(99, 99)),
            selection);
        var stale = editing.BeginTextDraft(
            TextInput(
                sessionId,
                selection with { SelectionRevision = selection.SelectionRevision + 1 },
                new PhysicalPoint(10, 10)),
            selection);

        Assert.AreEqual(TextDraftResultKind.IgnoredOutsideSelection, outside.Kind);
        Assert.AreEqual(TextDraftResultKind.StaleSelectionRevision, stale.Kind);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
        Assert.IsNull(editing.CreatePresentationSnapshot(selection).DraftText);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void TextDraftPreservesUnicodeAndNormalizesLineEndings()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectText(editing, sessionId, selection);
        var started = editing.BeginTextDraft(
            TextInput(sessionId, selection, new PhysicalPoint(10, 12)),
            selection);

        var updated = editing.UpdateTextDraftContent(
            started.Request!,
            "第一行\r\n第二行 😀",
            selection);

        Assert.AreEqual(TextDraftResultKind.DraftUpdated, updated.Kind);
        Assert.AreEqual("第一行\n第二行 😀", updated.Text);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void EmptyTextCommitKeepsDraftAndValidCommitAddsOneObject()
    {
        var objectId = new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000020"));
        var editing = CreateEditing(out var sessionId, out var selection, () => objectId);
        SelectText(editing, sessionId, selection);
        var started = editing.BeginTextDraft(
            TextInput(sessionId, selection, new PhysicalPoint(10, 12)),
            selection);

        var empty = editing.CommitTextDraft(started.Request!, selection);
        var updated = editing.UpdateTextDraftContent(started.Request!, "完成 😀", selection);
        var committed = editing.CommitTextDraft(updated.Request!, selection);
        var duplicate = editing.CommitTextDraft(updated.Request!, selection);

        Assert.AreEqual(TextDraftResultKind.EmptyText, empty.Kind);
        Assert.AreEqual(TextDraftResultKind.DraftUpdated, updated.Kind);
        Assert.AreEqual(TextDraftResultKind.Committed, committed.Kind);
        Assert.AreEqual(objectId, committed.CommittedObject!.ObjectId);
        Assert.AreEqual(AnnotationToolKind.Text, committed.CommittedObject.ToolKind);
        Assert.AreEqual("完成 😀", ((TextAnnotationContent)committed.CommittedObject.Content!).Text);
        Assert.AreEqual(1, committed.Document!.Objects.Count);
        Assert.AreEqual(new AnnotationRevision(1), editing.CurrentAnnotationRevision);
        Assert.AreEqual(TextDraftResultKind.NoActiveDraft, duplicate.Kind);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void TextDraftCancelAndToolSwitchClearDraftWithoutMutation()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectText(editing, sessionId, selection);
        var started = editing.BeginTextDraft(
            TextInput(sessionId, selection, new PhysicalPoint(10, 12)),
            selection);
        var cancelled = editing.CancelTextDraft(started.Request!, selection);

        Assert.AreEqual(TextDraftResultKind.Cancelled, cancelled.Kind);
        Assert.IsNull(editing.CreatePresentationSnapshot(selection).DraftText);

        var restarted = editing.BeginTextDraft(
            TextInput(sessionId, selection, new PhysicalPoint(10, 12)),
            selection);
        var switched = editing.SelectTool(
            new EditingToolSelectionRequest(
                sessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                AnnotationRevision.Initial,
                EditingToolKind.Rectangle),
            WorkflowState.Editing,
            selection);

        Assert.AreEqual(TextDraftResultKind.DraftStarted, restarted.Kind);
        Assert.AreEqual(EditingToolSelectionResultKind.Selected, switched.Kind);
        Assert.IsNull(editing.CreatePresentationSnapshot(selection).DraftText);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void PressOutsideSelectionIsIgnoredAndDoesNotCreateDraftOrRevision()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectRectangle(editing, sessionId, selection);
        var input = Input(sessionId, selection, new PhysicalPoint(99, 99));

        var result = editing.PointerPressed(input, selection);

        Assert.AreEqual(RectanglePointerResultKind.IgnoredOutsideSelection, result.Kind);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
        Assert.IsEmpty(result.Document!.Objects);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void ReverseDragCreatesOneNormalizedDraftWithoutDocumentMutation()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectRectangle(editing, sessionId, selection);
        var start = Input(sessionId, selection, new PhysicalPoint(40, 40), pointerId: 7);
        var end = Input(sessionId, selection, new PhysicalPoint(10, 20), pointerId: 7);

        var started = editing.PointerPressed(start, selection);
        var moved = editing.PointerMoved(end, selection);

        Assert.AreEqual(RectanglePointerResultKind.DraftStarted, started.Kind);
        Assert.AreEqual(RectanglePointerResultKind.DraftUpdated, moved.Kind);
        Assert.AreEqual(new PhysicalRect(10, 20, 40, 40), moved.DraftPhysicalBounds);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
        Assert.IsEmpty(moved.Document!.Objects);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void PointerMismatchAndStaleRequestsDoNotMutateDraft()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectRectangle(editing, sessionId, selection);
        var first = Input(sessionId, selection, new PhysicalPoint(10, 10), pointerId: 1);
        editing.PointerPressed(first, selection);

        var mismatch = editing.PointerMoved(
            Input(sessionId, selection, new PhysicalPoint(20, 20), pointerId: 2),
            selection);
        var stale = editing.PointerMoved(
            first with
            {
                SelectionRevision = selection.SelectionRevision + 1,
                GlobalPhysicalPoint = new PhysicalPoint(20, 20)
            },
            selection);

        Assert.AreEqual(RectanglePointerResultKind.PointerMismatch, mismatch.Kind);
        Assert.AreEqual(RectanglePointerResultKind.StaleSelectionRevision, stale.Kind);
        Assert.AreEqual(new PhysicalRect(10, 10, 10, 10), mismatch.DraftPhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void ValidReleaseCommitsExactlyOnceWithDeterministicIdAndTopmostZOrder()
    {
        var firstId = new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000001"));
        var secondId = new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000002"));
        var ids = new Queue<AnnotationObjectId>(new[] { firstId, secondId });
        var editing = CreateEditing(out var sessionId, out var selection, () => ids.Dequeue());
        SelectRectangle(editing, sessionId, selection);

        var first = Commit(editing, sessionId, selection, new PhysicalPoint(10, 10), new PhysicalPoint(20, 30), 3);
        var secondSelection = selection with { SelectionRevision = selection.SelectionRevision + 2 };
        editing.UpdateSelection(secondSelection);
        var second = Commit(editing, sessionId, secondSelection, new PhysicalPoint(15, 15), new PhysicalPoint(25, 35), 4);

        Assert.AreEqual(RectanglePointerResultKind.Committed, first.Kind);
        Assert.AreEqual(RectanglePointerResultKind.Committed, second.Kind);
        Assert.AreEqual(2, editing.CreatePresentationSnapshot(secondSelection).Document.Objects.Count);
        Assert.AreEqual(firstId, first.CommittedObject!.ObjectId);
        Assert.AreEqual(secondId, second.CommittedObject!.ObjectId);
        Assert.AreEqual(0, first.CommittedObject.ZOrder);
        Assert.AreEqual(1, second.CommittedObject.ZOrder);
        Assert.AreEqual(2, editing.CurrentAnnotationRevision.Value);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void ZeroSizeReleaseClearsDraftWithoutRevisionOrObject()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectRectangle(editing, sessionId, selection);
        var point = new PhysicalPoint(10, 10);
        editing.PointerPressed(Input(sessionId, selection, point), selection);
        var result = editing.PointerReleased(Input(sessionId, selection, point), selection);
        var noDraft = editing.PointerMoved(
            Input(sessionId, selection, new PhysicalPoint(11, 11)),
            selection);

        Assert.AreEqual(RectanglePointerResultKind.InvalidGeometry, result.Kind);
        Assert.AreEqual(RectanglePointerResultKind.NoActiveDraft, noDraft.Kind);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
        Assert.IsEmpty(result.Document!.Objects);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void NonPositivePointerIdIsRejectedWithoutMutation()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectRectangle(editing, sessionId, selection);
        var result = editing.PointerPressed(
            Input(sessionId, selection, new PhysicalPoint(10, 10), pointerId: 0),
            selection);

        Assert.AreEqual(RectanglePointerResultKind.PointerMismatch, result.Kind);
        Assert.IsEmpty(result.Document!.Objects);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void SwitchingToolDiscardsDraftButDoesNotChangeDocument()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectRectangle(editing, sessionId, selection);
        editing.PointerPressed(
            Input(sessionId, selection, new PhysicalPoint(10, 10)),
            selection);

        var switched = editing.SelectTool(
            new EditingToolSelectionRequest(
                sessionId,
                "annotation-v1",
                selection.SelectionRevision,
                AnnotationRevision.Initial,
                EditingToolKind.Selection),
            WorkflowState.Editing,
            selection);
        var snapshot = editing.CreatePresentationSnapshot(selection);

        Assert.AreEqual(EditingToolSelectionResultKind.Selected, switched.Kind);
        Assert.AreEqual(EditingToolKind.Selection, snapshot.ActiveTool);
        Assert.IsNull(snapshot.DraftPhysicalBounds);
        Assert.IsEmpty(snapshot.Document.Objects);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void SelectionRevisionChangesOnlyClipPresentationAndRetainCommittedGeometry()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectRectangle(editing, sessionId, selection);
        var committed = Commit(
            editing,
            sessionId,
            selection,
            new PhysicalPoint(10, 10),
            new PhysicalPoint(20, 30),
            1);
        var adjusted = selection with
        {
            SelectionRevision = selection.SelectionRevision + 2,
            NormalizedPhysicalBounds = new PhysicalRect(0, 0, 15, 15)
        };
        editing.UpdateSelection(adjusted);
        var snapshot = editing.CreatePresentationSnapshot(adjusted);

        Assert.AreEqual(RectanglePointerResultKind.Committed, committed.Kind);
        Assert.AreEqual(committed.CommittedObject!.Geometry, snapshot.Document.Objects.Single().Geometry);
        Assert.AreEqual(committed.CommittedObject.ZOrder, snapshot.Document.Objects.Single().ZOrder);
        Assert.AreEqual(adjusted.NormalizedPhysicalBounds, snapshot.SelectionPhysicalBounds);
        Assert.AreEqual(1, snapshot.AnnotationRevision.Value);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void ArrowLineReleaseCommitsExactSegmentAndSelectedEndStyle()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectArrowLine(editing, sessionId, selection, ArrowLineEndStyle.None);
        var start = ArrowInput(
            sessionId,
            selection,
            new PhysicalPoint(60, 60),
            pointerId: 9);
        var end = ArrowInput(
            sessionId,
            selection,
            new PhysicalPoint(20, 30),
            pointerId: 9);

        var started = editing.PointerPressed(start, selection);
        var moved = editing.PointerMoved(end, selection);
        var committed = editing.PointerReleased(end, selection);
        var content = (ArrowLineAnnotationContent)committed.CommittedObject!.Content!;

        Assert.AreEqual(ArrowLinePointerResultKind.DraftStarted, started.Kind);
        Assert.AreEqual(ArrowLinePointerResultKind.DraftUpdated, moved.Kind);
        Assert.AreEqual(ArrowLinePointerResultKind.Committed, committed.Kind);
        Assert.AreEqual(
            new PhysicalLineSegment(new PhysicalPoint(60, 60), new PhysicalPoint(20, 30)),
            content.Segment);
        Assert.AreEqual(ArrowLineEndStyle.None, content.Style.EndStyle);
        Assert.AreEqual(new PhysicalRect(20, 30, 60, 60), committed.CommittedObject.Geometry);
        Assert.AreEqual(1, editing.CurrentAnnotationRevision.Value);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void ArrowLineDraftDoesNotMutateDocumentAndStaleReleaseIsRejected()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectArrowLine(editing, sessionId, selection, ArrowLineEndStyle.Arrow);
        editing.PointerPressed(
            ArrowInput(sessionId, selection, new PhysicalPoint(10, 10)),
            selection);

        var stale = editing.PointerReleased(
            ArrowInput(
                sessionId,
                selection with { SelectionRevision = selection.SelectionRevision + 1 },
                new PhysicalPoint(20, 20)),
            selection);

        Assert.AreEqual(ArrowLinePointerResultKind.StaleSelectionRevision, stale.Kind);
        Assert.IsEmpty(editing.CreatePresentationSnapshot(selection).Document.Objects);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void HighlighterReleaseCommitsFreehandPointsWithSemiTransparentStyle()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectHighlighter(editing, sessionId, selection);
        var start = HighlighterInput(
            sessionId,
            selection,
            new PhysicalPoint(10, 12),
            pointerId: 5);
        var middle = HighlighterInput(
            sessionId,
            selection,
            new PhysicalPoint(20, 18),
            pointerId: 5);
        var end = HighlighterInput(
            sessionId,
            selection,
            new PhysicalPoint(32, 24),
            pointerId: 5);

        var started = editing.PointerPressed(start, selection);
        var moved = editing.PointerMoved(middle, selection);
        var committed = editing.PointerReleased(end, selection);
        var content = (HighlighterStrokeContent)committed.CommittedObject!.Content!;

        Assert.AreEqual(HighlighterPointerResultKind.DraftStarted, started.Kind);
        Assert.AreEqual(HighlighterPointerResultKind.DraftUpdated, moved.Kind);
        Assert.AreEqual(HighlighterPointerResultKind.Committed, committed.Kind);
        CollectionAssert.AreEqual(
            new[]
            {
                new PhysicalPoint(10, 12),
                new PhysicalPoint(20, 18),
                new PhysicalPoint(32, 24)
            },
            content.Path.Points.ToArray());
        Assert.AreEqual(new PhysicalRect(10, 12, 32, 24), committed.CommittedObject.Geometry);
        Assert.IsTrue(content.Style.StrokeColor.A > 0);
        Assert.IsTrue(content.Style.StrokeColor.A < 255);
        Assert.AreEqual(1, editing.CurrentAnnotationRevision.Value);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void HighlighterStaleReleasePreservesDraftAndDocument()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectHighlighter(editing, sessionId, selection);
        editing.PointerPressed(
            HighlighterInput(sessionId, selection, new PhysicalPoint(10, 10)),
            selection);

        var stale = editing.PointerReleased(
            HighlighterInput(
                sessionId,
                selection with { SelectionRevision = selection.SelectionRevision + 1 },
                new PhysicalPoint(20, 20)),
            selection);
        var snapshot = editing.CreatePresentationSnapshot(selection);

        Assert.AreEqual(HighlighterPointerResultKind.StaleSelectionRevision, stale.Kind);
        Assert.IsNotNull(snapshot.DraftHighlighterPoints);
        Assert.AreEqual(1, snapshot.DraftHighlighterPoints!.Count);
        Assert.IsEmpty(snapshot.Document.Objects);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void CancelDraftLeavesDocumentEmptyAndNewSessionStartsClean()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectRectangle(editing, sessionId, selection);
        editing.PointerPressed(
            Input(sessionId, selection, new PhysicalPoint(10, 10)),
            selection);

        var cancelled = editing.CancelDraft(sessionId, "annotation-v1");
        editing.ClearSession(sessionId);
        var nextSession = Guid.NewGuid();
        var nextSelection = LockedSelection(nextSession, 2);
        editing.BeginSession(nextSelection);
        var snapshot = editing.CreatePresentationSnapshot(nextSelection);

        Assert.AreEqual(RectanglePointerResultKind.Cancelled, cancelled.Kind);
        Assert.IsEmpty(snapshot.Document.Objects);
        Assert.AreEqual(AnnotationRevision.Initial, snapshot.AnnotationRevision);
        Assert.AreEqual(EditingToolKind.Selection, snapshot.ActiveTool);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void PrivacyRegionUsesCentralPolicyAndModeSwitchDoesNotMutateDocument()
    {
        var policy = new TestPrivacyPolicy(
            PrivacyRegionMode.Blur,
            new PrivacyRegionEffectParameters(6, 3));
        var editing = CreateEditing(out var sessionId, out var selection, privacyRegionEffectPolicy: policy);

        var selected = SelectPrivacy(editing, sessionId, selection);
        Assert.AreEqual(EditingToolSelectionResultKind.Selected, selected.Kind);
        Assert.AreEqual(PrivacyRegionMode.Blur, selected.ActivePrivacyRegionMode);
        Assert.AreEqual(6, selected.ActivePrivacyRegionEffectParameters!.MosaicBlockSize);
        Assert.AreEqual(3, selected.ActivePrivacyRegionEffectParameters.BlurRadius);

        var switched = editing.SelectPrivacyRegionMode(
            new PrivacyRegionModeSelectionRequest(
                sessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                AnnotationRevision.Initial,
                PrivacyRegionMode.Mosaic),
            WorkflowState.Editing,
            selection);

        Assert.AreEqual(PrivacyRegionModeSelectionResultKind.Selected, switched.Kind);
        Assert.AreEqual(PrivacyRegionMode.Mosaic, switched.ActiveMode);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
        Assert.IsEmpty(editing.CreatePresentationSnapshot(selection).Document.Objects);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void PrivacyRegionDragCommitsOneObjectPerReleaseWithSelectedEffect()
    {
        var firstId = new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000031"));
        var secondId = new AnnotationObjectId(Guid.Parse("00000000-0000-0000-0000-000000000032"));
        var ids = new Queue<AnnotationObjectId>(new[] { firstId, secondId });
        var editing = CreateEditing(
            out var sessionId,
            out var selection,
            () => ids.Dequeue());
        SelectPrivacy(editing, sessionId, selection, PrivacyRegionMode.Mosaic);

        var mosaic = CommitPrivacy(
            editing,
            sessionId,
            selection,
            new PhysicalPoint(10, 12),
            new PhysicalPoint(30, 32),
            4);
        Assert.AreEqual(PrivacyRegionPointerResultKind.Committed, mosaic.Kind);
        Assert.AreEqual(new PhysicalRect(10, 12, 30, 32), mosaic.CommittedObject!.Geometry);
        Assert.AreEqual(
            PrivacyRegionMode.Mosaic,
            ((PrivacyRegionAnnotationContent)mosaic.CommittedObject.Content!).Mode);
        Assert.AreEqual(new AnnotationRevision(1), editing.CurrentAnnotationRevision);

        var switched = editing.SelectPrivacyRegionMode(
            new PrivacyRegionModeSelectionRequest(
                sessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                editing.CurrentAnnotationRevision,
                PrivacyRegionMode.Blur),
            WorkflowState.Editing,
            selection);
        Assert.AreEqual(PrivacyRegionModeSelectionResultKind.Selected, switched.Kind);

        var blur = CommitPrivacy(
            editing,
            sessionId,
            selection,
            new PhysicalPoint(40, 42),
            new PhysicalPoint(60, 62),
            5);
        Assert.AreEqual(PrivacyRegionPointerResultKind.Committed, blur.Kind);
        Assert.AreEqual(
            PrivacyRegionMode.Blur,
            ((PrivacyRegionAnnotationContent)blur.CommittedObject!.Content!).Mode);
        Assert.AreEqual(2, blur.Document!.Objects.Count);
        Assert.AreEqual(new AnnotationRevision(2), editing.CurrentAnnotationRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public void PrivacyRegionStaleAndZeroSizeInputsDoNotLeaveDraftOrMutateDocument()
    {
        var editing = CreateEditing(out var sessionId, out var selection);
        SelectPrivacy(editing, sessionId, selection);

        var outside = editing.PointerPressed(
            PrivacyInput(sessionId, selection, new PhysicalPoint(99, 99)),
            selection);
        Assert.AreEqual(PrivacyRegionPointerResultKind.IgnoredOutsideSelection, outside.Kind);

        var point = new PhysicalPoint(20, 20);
        var started = editing.PointerPressed(PrivacyInput(sessionId, selection, point), selection);
        Assert.AreEqual(PrivacyRegionPointerResultKind.DraftStarted, started.Kind);
        var stale = editing.PointerMoved(
            PrivacyInput(
                sessionId,
                selection with { SelectionRevision = selection.SelectionRevision + 1 },
                new PhysicalPoint(30, 30)),
            selection);
        Assert.AreEqual(PrivacyRegionPointerResultKind.StaleSelectionRevision, stale.Kind);

        var zeroSize = editing.PointerReleased(
            PrivacyInput(sessionId, selection, point),
            selection);
        Assert.AreEqual(PrivacyRegionPointerResultKind.InvalidGeometry, zeroSize.Kind);
        Assert.IsNull(editing.CreatePresentationSnapshot(selection).DraftPrivacyRegionBounds);
        Assert.AreEqual(AnnotationRevision.Initial, editing.CurrentAnnotationRevision);
        Assert.IsEmpty(editing.CreatePresentationSnapshot(selection).Document.Objects);
    }

    private static AnnotationEditingCoordinator CreateEditing(
        out Guid sessionId,
        out SelectionVisualState selection,
        Func<AnnotationObjectId>? objectIdFactory = null,
        IPrivacyRegionEffectPolicy? privacyRegionEffectPolicy = null)
    {
        sessionId = Guid.NewGuid();
        selection = LockedSelection(sessionId, 4);
        var documents = new AnnotationDocumentCoordinator();
        var editing = new AnnotationEditingCoordinator(
            documents,
            objectIdFactory,
            privacyRegionEffectPolicy: privacyRegionEffectPolicy);
        editing.BeginSession(selection);
        return editing;
    }

    private static EditingToolSelectionResult SelectPrivacy(
        AnnotationEditingCoordinator editing,
        Guid sessionId,
        SelectionVisualState selection,
        PrivacyRegionMode? mode = null)
    {
        var result = editing.SelectTool(
            new EditingToolSelectionRequest(
                sessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                editing.CurrentAnnotationRevision,
                EditingToolKind.PrivacyRegion)
            {
                RequestedPrivacyRegionMode = mode
            },
            WorkflowState.Editing,
            selection);
        Assert.AreEqual(EditingToolSelectionResultKind.Selected, result.Kind);
        return result;
    }

    private static PrivacyRegionPointerResult CommitPrivacy(
        AnnotationEditingCoordinator editing,
        Guid sessionId,
        SelectionVisualState selection,
        PhysicalPoint start,
        PhysicalPoint end,
        int pointerId)
    {
        editing.PointerPressed(
            PrivacyInput(sessionId, selection, start, pointerId, editing.CurrentAnnotationRevision),
            selection);
        editing.PointerMoved(
            PrivacyInput(sessionId, selection, end, pointerId, editing.CurrentAnnotationRevision),
            selection);
        return editing.PointerReleased(
            PrivacyInput(sessionId, selection, end, pointerId, editing.CurrentAnnotationRevision),
            selection);
    }

    private static PrivacyRegionPointerEvent PrivacyInput(
        Guid sessionId,
        SelectionVisualState selection,
        PhysicalPoint point,
        int pointerId = 1,
        AnnotationRevision? annotationRevision = null) => new(
        sessionId,
        selection.CoordinateVersion,
        selection.SelectionRevision,
        annotationRevision ?? AnnotationRevision.Initial,
        pointerId,
        point);

    private static SelectionVisualState LockedSelection(Guid sessionId, int revision) => new()
    {
        SessionId = sessionId,
        CoordinateVersion = "annotation-v1",
        SelectionRevision = revision,
        Status = SelectionStatus.Locked,
        InteractionMode = SelectionInteractionMode.Locked,
        IsGeometryValid = true,
        NormalizedPhysicalBounds = new PhysicalRect(0, 0, 80, 80),
        CurrentPhysicalPoint = new PhysicalPoint(1, 1)
    };

    private static void SelectRectangle(
        AnnotationEditingCoordinator editing,
        Guid sessionId,
        SelectionVisualState selection)
    {
        var result = editing.SelectTool(
            new EditingToolSelectionRequest(
                sessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                AnnotationRevision.Initial,
                EditingToolKind.Rectangle),
            WorkflowState.Editing,
            selection);
        Assert.AreEqual(EditingToolSelectionResultKind.Selected, result.Kind);
    }

    private static void SelectArrowLine(
        AnnotationEditingCoordinator editing,
        Guid sessionId,
        SelectionVisualState selection,
        ArrowLineEndStyle endStyle)
    {
        var result = editing.SelectTool(
            new EditingToolSelectionRequest(
                sessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                AnnotationRevision.Initial,
                EditingToolKind.ArrowLine)
            {
                RequestedArrowLineEndStyle = endStyle
            },
            WorkflowState.Editing,
            selection);
        Assert.AreEqual(EditingToolSelectionResultKind.Selected, result.Kind);
        Assert.AreEqual(endStyle, result.ActiveArrowLineEndStyle);
    }

    private static void SelectHighlighter(
        AnnotationEditingCoordinator editing,
        Guid sessionId,
        SelectionVisualState selection)
    {
        var result = editing.SelectTool(
            new EditingToolSelectionRequest(
                sessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                AnnotationRevision.Initial,
                EditingToolKind.Highlighter),
            WorkflowState.Editing,
            selection);
        Assert.AreEqual(EditingToolSelectionResultKind.Selected, result.Kind);
        Assert.AreEqual(EditingToolKind.Highlighter, result.ActiveTool);
    }

    private static void SelectText(
        AnnotationEditingCoordinator editing,
        Guid sessionId,
        SelectionVisualState selection)
    {
        var result = editing.SelectTool(
            new EditingToolSelectionRequest(
                sessionId,
                selection.CoordinateVersion,
                selection.SelectionRevision,
                AnnotationRevision.Initial,
                EditingToolKind.Text),
            WorkflowState.Editing,
            selection);
        Assert.AreEqual(EditingToolSelectionResultKind.Selected, result.Kind);
        Assert.AreEqual(EditingToolKind.Text, result.ActiveTool);
    }

    private static RectanglePointerResult Commit(
        AnnotationEditingCoordinator editing,
        Guid sessionId,
        SelectionVisualState selection,
        PhysicalPoint start,
        PhysicalPoint end,
        int pointerId)
    {
        editing.PointerPressed(Input(
            sessionId,
            selection,
            start,
            pointerId,
            editing.CurrentAnnotationRevision), selection);
        editing.PointerMoved(Input(
            sessionId,
            selection,
            end,
            pointerId,
            editing.CurrentAnnotationRevision),
            selection);
        return editing.PointerReleased(Input(
            sessionId,
            selection,
            end,
            pointerId,
            editing.CurrentAnnotationRevision),
            selection);
    }

    private static RectanglePointerEvent Input(
        Guid sessionId,
        SelectionVisualState selection,
        PhysicalPoint point,
        int pointerId = 1,
        AnnotationRevision? annotationRevision = null) => new(
        sessionId,
        selection.CoordinateVersion,
        selection.SelectionRevision,
        annotationRevision ?? AnnotationRevision.Initial,
        pointerId,
        point);

    private static ArrowLinePointerEvent ArrowInput(
        Guid sessionId,
        SelectionVisualState selection,
        PhysicalPoint point,
        int pointerId = 1,
        AnnotationRevision? annotationRevision = null) => new(
        sessionId,
        selection.CoordinateVersion,
        selection.SelectionRevision,
        annotationRevision ?? AnnotationRevision.Initial,
        pointerId,
        point);

    private static HighlighterPointerEvent HighlighterInput(
        Guid sessionId,
        SelectionVisualState selection,
        PhysicalPoint point,
        int pointerId = 1,
        AnnotationRevision? annotationRevision = null) => new(
        sessionId,
        selection.CoordinateVersion,
        selection.SelectionRevision,
        annotationRevision ?? AnnotationRevision.Initial,
        pointerId,
        point);

    private static TextDraftPointerEvent TextInput(
        Guid sessionId,
        SelectionVisualState selection,
        PhysicalPoint point,
        Guid? draftId = null,
        AnnotationRevision? annotationRevision = null) => new(
        sessionId,
        selection.CoordinateVersion,
        selection.SelectionRevision,
        annotationRevision ?? AnnotationRevision.Initial,
        1,
        point)
        {
            DraftId = draftId ?? Guid.Empty
        };

    private sealed class TestPrivacyPolicy : IPrivacyRegionEffectPolicy
    {
        private readonly PrivacyRegionMode _defaultMode;
        private readonly PrivacyRegionEffectParameters _parameters;

        public TestPrivacyPolicy(
            PrivacyRegionMode defaultMode,
            PrivacyRegionEffectParameters parameters)
        {
            _defaultMode = defaultMode;
            _parameters = parameters;
        }

        public PrivacyRegionMode GetDefaultMode() => _defaultMode;

        public PrivacyRegionEffectParameters GetParameters(PrivacyRegionMode mode) =>
            mode is PrivacyRegionMode.Mosaic or PrivacyRegionMode.Blur
                ? _parameters
                : throw new ArgumentOutOfRangeException(nameof(mode));
    }
}
