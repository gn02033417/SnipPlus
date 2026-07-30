using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class AnnotationDocumentCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void BeginSessionCreatesOneEmptyDocumentAtInitialRevision()
    {
        var coordinator = new AnnotationDocumentCoordinator();
        var sessionId = Guid.NewGuid();

        var first = coordinator.BeginSession(sessionId);
        var second = coordinator.BeginSession(sessionId);

        Assert.AreSame(first, second);
        Assert.AreSame(first, coordinator.Current);
        Assert.AreEqual(AnnotationRevision.Initial, first.Revision);
        Assert.IsEmpty(first.Objects);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void AddAssignsRevisionAndKeepsDeterministicZOrder()
    {
        var coordinator = CreateCoordinator(out var sessionId);
        var first = CreateObject(sessionId, 20);
        var second = CreateObject(sessionId, 10);

        var firstResult = coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            AnnotationRevision.Initial,
            first));
        var secondResult = coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            Revision(firstResult),
            second));

        var document = Success(secondResult).Document;
        Assert.AreEqual(2, document.Revision.Value);
        Assert.AreEqual(second.ObjectId, document.Objects[0].ObjectId);
        Assert.AreEqual(first.ObjectId, document.Objects[1].ObjectId);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void DuplicateObjectIdDoesNotChangeDocument()
    {
        var coordinator = CreateCoordinator(out var sessionId);
        var annotationObject = CreateObject(sessionId, 1);
        var added = Success(coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            AnnotationRevision.Initial,
            annotationObject))).Document;

        var duplicate = coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            added.Revision,
            annotationObject));

        Assert.IsInstanceOfType<AnnotationMutationResult.DuplicateObjectId>(duplicate);
        Assert.AreEqual(added.Revision, coordinator.Current!.Revision);
        Assert.AreEqual(1, coordinator.Current.Objects.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void StaleSessionAndRevisionAreTypedRejections()
    {
        var coordinator = CreateCoordinator(out var sessionId);
        var otherSessionId = Guid.NewGuid();
        var annotationObject = CreateObject(sessionId, 1);
        var staleSession = coordinator.Add(new AddAnnotationObjectRequest(
            otherSessionId,
            AnnotationRevision.Initial,
            CreateObject(otherSessionId, 1)));
        var added = Success(coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            AnnotationRevision.Initial,
            annotationObject))).Document;
        var staleRevision = coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            AnnotationRevision.Initial,
            CreateObject(sessionId, 2)));

        Assert.IsInstanceOfType<AnnotationMutationResult.StaleSession>(staleSession);
        Assert.IsInstanceOfType<AnnotationMutationResult.StaleAnnotationRevision>(staleRevision);
        Assert.AreEqual(added.Revision, coordinator.Current!.Revision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void ReplaceKeepsIdentityAndIncrementsOnlyWhenChanged()
    {
        var coordinator = CreateCoordinator(out var sessionId);
        var original = CreateObject(sessionId, 1);
        var added = Success(coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            AnnotationRevision.Initial,
            original))).Document;
        var replacement = new AnnotationObject(
            original.ObjectId,
            sessionId,
            AnnotationToolKind.HighlighterStroke,
            new PhysicalRect(4, 4, 8, 8),
            3);

        var replaced = Success(coordinator.Replace(new ReplaceAnnotationObjectRequest(
            sessionId,
            added.Revision,
            replacement))).Document;
        var noChange = coordinator.Replace(new ReplaceAnnotationObjectRequest(
            sessionId,
            replaced.Revision,
            replacement));
        var missing = coordinator.Replace(new ReplaceAnnotationObjectRequest(
            sessionId,
            replaced.Revision,
            CreateObject(sessionId, 4)));

        Assert.AreEqual(2, replaced.Revision.Value);
        Assert.AreEqual(original.ObjectId, replaced.Objects[0].ObjectId);
        Assert.AreEqual(AnnotationToolKind.HighlighterStroke, replaced.Objects[0].ToolKind);
        Assert.IsInstanceOfType<AnnotationMutationResult.NoChange>(noChange);
        Assert.IsInstanceOfType<AnnotationMutationResult.ObjectNotFound>(missing);
        Assert.AreEqual(replaced.Revision, coordinator.Current!.Revision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RemoveKeepsRemainingOrderAndMissingOrInvalidObjectsDoNotMutate()
    {
        var coordinator = CreateCoordinator(out var sessionId);
        var first = CreateObject(sessionId, 1);
        var second = CreateObject(sessionId, 2);
        var firstDocument = Success(coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            AnnotationRevision.Initial,
            first))).Document;
        var secondDocument = Success(coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            firstDocument.Revision,
            second))).Document;
        var removed = Success(coordinator.Remove(new RemoveAnnotationObjectRequest(
            sessionId,
            secondDocument.Revision,
            first.ObjectId))).Document;
        var missing = coordinator.Remove(new RemoveAnnotationObjectRequest(
            sessionId,
            removed.Revision,
            first.ObjectId));
        var invalid = coordinator.Add(new AddAnnotationObjectRequest(
            sessionId,
            removed.Revision,
            null));

        Assert.AreEqual(3, removed.Revision.Value);
        Assert.AreEqual(second.ObjectId, removed.Objects.Single().ObjectId);
        Assert.IsInstanceOfType<AnnotationMutationResult.ObjectNotFound>(missing);
        Assert.IsInstanceOfType<AnnotationMutationResult.InvalidObject>(invalid);
        Assert.AreEqual(removed.Revision, coordinator.Current!.Revision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void SessionClearAndNewSessionNeverReuseThePreviousDocument()
    {
        var coordinator = CreateCoordinator(out var firstSessionId);
        var first = coordinator.BeginSession(firstSessionId);
        Assert.IsTrue(coordinator.ClearSession(firstSessionId));
        Assert.IsNull(coordinator.Current);

        var second = coordinator.BeginSession(Guid.NewGuid());

        Assert.AreNotEqual(first.SessionId, second.SessionId);
        Assert.AreNotSame(first, second);
        Assert.IsFalse(coordinator.ClearSession(firstSessionId));
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RevisionIncrementExplicitlyRejectsOverflow()
    {
        var revision = new AnnotationRevision(long.MaxValue);

        Assert.IsFalse(revision.TryIncrement(out var next));
        Assert.AreEqual(revision, next);
    }

    private static AnnotationDocumentCoordinator CreateCoordinator(out Guid sessionId)
    {
        sessionId = Guid.NewGuid();
        var coordinator = new AnnotationDocumentCoordinator();
        coordinator.BeginSession(sessionId);
        return coordinator;
    }

    private static AnnotationObject CreateObject(Guid sessionId, int zOrder) =>
        new(
            AnnotationObjectId.New(),
            sessionId,
            AnnotationToolKind.Rectangle,
            new PhysicalRect(zOrder, zOrder, zOrder + 2, zOrder + 2),
            zOrder);

    private static AnnotationMutationResult.Succeeded Success(AnnotationMutationResult result) =>
        result as AnnotationMutationResult.Succeeded
        ?? throw new AssertFailedException($"Expected success, got {result.GetType().Name}.");

    private static AnnotationRevision Revision(AnnotationMutationResult result) => Success(result).Document.Revision;
}
