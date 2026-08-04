using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class AnnotationObjectEditCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void PointerMoveUsesPreviewAndCommitsOneReplaceRevision()
    {
        var fixture = CreateFixture();
        var original = Add(fixture.Documents, fixture.SessionId, new AnnotationObject(
            AnnotationObjectId.New(), fixture.SessionId, AnnotationToolKind.Rectangle,
            new PhysicalRect(100, 100, 200, 180), 0));
        fixture.Editing.BeginSession(fixture.Selection);

        var pressed = fixture.Editing.PointerPressed(Event(fixture, new PhysicalPoint(150, 140)));
        var moved = fixture.Editing.PointerMoved(Event(fixture, new PhysicalPoint(180, 170)));

        Assert.AreEqual(AnnotationObjectEditResultKind.EditStarted, pressed.Kind);
        Assert.AreEqual(AnnotationObjectEditResultKind.EditUpdated, moved.Kind);
        Assert.AreEqual(1, fixture.Documents.Current!.Revision.Value);

        var released = fixture.Editing.PointerReleased(Event(fixture, new PhysicalPoint(180, 170)));

        Assert.AreEqual(AnnotationObjectEditResultKind.EditCommitted, released.Kind);
        Assert.AreEqual(2, fixture.Documents.Current!.Revision.Value);
        Assert.AreEqual(original.ObjectId, fixture.Documents.Current.Objects.Single().ObjectId);
        Assert.AreEqual(new PhysicalRect(130, 130, 230, 210), fixture.Documents.Current.Objects.Single().Geometry);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void TopmostHitTestSelectsHighestZOrderDeterministically()
    {
        var fixture = CreateFixture();
        var low = Add(fixture.Documents, fixture.SessionId, new AnnotationObject(
            AnnotationObjectId.New(), fixture.SessionId, AnnotationToolKind.Rectangle,
            new PhysicalRect(100, 100, 240, 240), 1));
        var high = Add(fixture.Documents, fixture.SessionId, new AnnotationObject(
            AnnotationObjectId.New(), fixture.SessionId, AnnotationToolKind.Rectangle,
            new PhysicalRect(120, 120, 260, 260), 5));
        fixture.Editing.BeginSession(fixture.Selection);

        var result = fixture.Editing.PointerPressed(Event(fixture, new PhysicalPoint(150, 150)));

        Assert.AreEqual(AnnotationObjectEditResultKind.EditStarted, result.Kind);
        Assert.AreEqual(high.ObjectId, fixture.Editing.State.SelectedObjectId);
        Assert.AreNotEqual(low.ObjectId, fixture.Editing.State.SelectedObjectId);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RectangleResizeSupportsFlippingAndStaysInsideSelection()
    {
        var fixture = CreateFixture();
        var original = Add(fixture.Documents, fixture.SessionId, new AnnotationObject(
            AnnotationObjectId.New(), fixture.SessionId, AnnotationToolKind.Rectangle,
            new PhysicalRect(100, 100, 200, 180), 0));
        fixture.Editing.BeginSession(fixture.Selection);

        fixture.Editing.PointerPressed(Event(fixture, new PhysicalPoint(100, 140)));
        fixture.Editing.PointerMoved(Event(fixture, new PhysicalPoint(230, 140)));
        var released = fixture.Editing.PointerReleased(Event(fixture, new PhysicalPoint(230, 140)));

        Assert.AreEqual(AnnotationObjectEditResultKind.EditCommitted, released.Kind);
        Assert.AreEqual(new PhysicalRect(200, 100, 230, 180), fixture.Documents.Current!.Objects.Single().Geometry);
        Assert.AreEqual(2, fixture.Documents.Current.Revision.Value);
        Assert.AreEqual(original.ObjectId, fixture.Documents.Current.Objects.Single().ObjectId);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void StaleReleaseDoesNotMutateDocument()
    {
        var fixture = CreateFixture();
        var original = Add(fixture.Documents, fixture.SessionId, new AnnotationObject(
            AnnotationObjectId.New(), fixture.SessionId, AnnotationToolKind.Rectangle,
            new PhysicalRect(100, 100, 200, 180), 0));
        fixture.Editing.BeginSession(fixture.Selection);
        fixture.Editing.PointerPressed(Event(fixture, new PhysicalPoint(150, 140)));

        var stale = fixture.Editing.PointerReleased(Event(fixture, new PhysicalPoint(180, 170)) with
        {
            ExpectedAnnotationRevision = new AnnotationRevision(99)
        });

        Assert.AreEqual(AnnotationObjectEditResultKind.StaleAnnotationRevision, stale.Kind);
        Assert.AreEqual(1, fixture.Documents.Current!.Revision.Value);
        Assert.AreEqual(original.Geometry, fixture.Documents.Current.Objects.Single().Geometry);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void StyleDeleteAndTextEditMutateOnlyTheSelectedObject()
    {
        var fixture = CreateFixture();
        var rectangle = Add(fixture.Documents, fixture.SessionId, new AnnotationObject(
            AnnotationObjectId.New(), fixture.SessionId, AnnotationToolKind.Rectangle,
            new PhysicalRect(100, 100, 200, 180), 0));
        var text = Add(fixture.Documents, fixture.SessionId, new AnnotationObject(
            AnnotationObjectId.New(), fixture.SessionId, AnnotationToolKind.Text,
            new PhysicalRect(300, 100, 500, 180), 1,
            new TextAnnotationContent(
                "before",
                new PhysicalPoint(300, 100),
                new PhysicalRect(300, 100, 500, 180),
                TextAnnotationStyle.Default)));
        fixture.Editing.BeginSession(fixture.Selection);

        var selected = fixture.Editing.SelectObject(new AnnotationObjectSelectionRequest(
            fixture.SessionId, fixture.CoordinateVersion, 1, new AnnotationRevision(2), rectangle.ObjectId));
        var restyled = fixture.Editing.ChangeStyle(new AnnotationObjectStyleChangeRequest(
            fixture.SessionId, fixture.CoordinateVersion, 1, new AnnotationRevision(2), rectangle.ObjectId,
            new AnnotationObjectStyleChange(Color: new ArgbColor(255, 0, 255, 0), Thickness: 6)));

        Assert.AreEqual(AnnotationObjectEditResultKind.Selected, selected.Kind);
        Assert.AreEqual(AnnotationObjectEditResultKind.Restyled, restyled.Kind);
        Assert.AreEqual(3, fixture.Documents.Current!.Revision.Value);
        Assert.AreEqual(new ArgbColor(255, 0, 255, 0),
            ((RectangleAnnotationContent)fixture.Documents.Current.Objects[0].Content!).Style.StrokeColor);

        var beginText = fixture.Editing.BeginTextEdit(new AnnotationObjectSelectionRequest(
            fixture.SessionId, fixture.CoordinateVersion, 1, new AnnotationRevision(3), text.ObjectId));
        var draftId = beginText.State.TextEditDraftId!.Value;
        var textResult = fixture.Editing.CommitTextEdit(new AnnotationObjectTextEditRequest(
            fixture.SessionId, fixture.CoordinateVersion, 1, new AnnotationRevision(3), text.ObjectId,
            draftId, "after"));

        Assert.AreEqual(AnnotationObjectEditResultKind.TextEditCommitted, textResult.Kind);
        Assert.AreEqual("after", ((TextAnnotationContent)fixture.Documents.Current.Objects[1].Content!).Text);
        var deleted = fixture.Editing.Delete(new AnnotationObjectDeleteRequest(
            fixture.SessionId, fixture.CoordinateVersion, 1, new AnnotationRevision(4), rectangle.ObjectId));
        Assert.AreEqual(AnnotationObjectEditResultKind.Deleted, deleted.Kind);
        Assert.IsFalse(fixture.Documents.Current.Objects.Any(value => value.ObjectId == rectangle.ObjectId));
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void SelectionRevisionCancelsPreviewWithoutChangingAnnotationRevision()
    {
        var fixture = CreateFixture();
        var original = Add(fixture.Documents, fixture.SessionId, new AnnotationObject(
            AnnotationObjectId.New(), fixture.SessionId, AnnotationToolKind.Rectangle,
            new PhysicalRect(100, 100, 200, 180), 0));
        fixture.Editing.BeginSession(fixture.Selection);
        fixture.Editing.PointerPressed(Event(fixture, new PhysicalPoint(150, 140)));

        fixture.Editing.UpdateSelection(fixture.Selection with { SelectionRevision = 2 });

        Assert.IsFalse(fixture.Editing.State.HasActiveEdit);
        Assert.AreEqual(new AnnotationRevision(1), fixture.Documents.Current!.Revision);
        Assert.AreEqual(original.ObjectId, fixture.Documents.Current.Objects.Single().ObjectId);
    }

    private static Fixture CreateFixture()
    {
        var sessionId = Guid.NewGuid();
        var documents = new AnnotationDocumentCoordinator();
        documents.BeginSession(sessionId);
        var selection = new SelectionVisualState
        {
            SessionId = sessionId,
            CoordinateVersion = "test-coordinate",
            SelectionRevision = 1,
            Status = SelectionStatus.Locked,
            InteractionMode = SelectionInteractionMode.Locked,
            IsGeometryValid = true,
            NormalizedPhysicalBounds = new PhysicalRect(0, 0, 1000, 800)
        };
        return new Fixture(sessionId, selection, documents, new AnnotationObjectEditCoordinator(documents));
    }

    private static AnnotationObject Add(
        AnnotationDocumentCoordinator documents,
        Guid sessionId,
        AnnotationObject annotationObject)
    {
        var result = documents.Add(new AddAnnotationObjectRequest(
            sessionId,
            documents.Current!.Revision,
            annotationObject));
        return ((AnnotationMutationResult.Succeeded)result).Document.Objects
            .Single(value => value.ObjectId == annotationObject.ObjectId);
    }

    private static AnnotationObjectPointerEvent Event(Fixture fixture, PhysicalPoint point) =>
        new(fixture.SessionId, fixture.CoordinateVersion, 1,
            fixture.Documents.Current!.Revision, 1, point);

    private sealed record Fixture(
        Guid SessionId,
        SelectionVisualState Selection,
        AnnotationDocumentCoordinator Documents,
        AnnotationObjectEditCoordinator Editing)
    {
        public string CoordinateVersion => Selection.CoordinateVersion;
    }
}
