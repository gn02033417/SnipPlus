using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Contracts.Tests;

[TestClass]
public sealed class AnnotationContractsTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void AnnotationObjectIdentityAndRevisionHaveExplicitValidity()
    {
        var objectId = Guid.NewGuid();
        var first = new AnnotationObjectId(objectId);
        var second = new AnnotationObjectId(objectId);

        Assert.AreEqual(first, second);
        Assert.IsTrue(first.IsValid);
        Assert.IsFalse(default(AnnotationObjectId).IsValid);
        Assert.AreEqual(0, AnnotationRevision.Initial.Value);
        Assert.IsTrue(AnnotationRevision.Initial.IsValid);
        Assert.IsFalse(new AnnotationRevision(-1).IsValid);
        AssertArgumentException(() => _ = new AnnotationObjectId(Guid.Empty));
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void AcceptedAnnotationToolKindsArePlatformNeutral()
    {
        CollectionAssert.AreEquivalent(
            new[]
            {
                AnnotationToolKind.Rectangle,
                AnnotationToolKind.ArrowLine,
                AnnotationToolKind.HighlighterStroke,
                AnnotationToolKind.Text,
                AnnotationToolKind.PrivacyRegion,
                AnnotationToolKind.NumberedMarker
            },
            Enum.GetValues<AnnotationToolKind>());
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void AnnotationObjectAndDocumentValidateSessionGeometryAndRevision()
    {
        var sessionId = Guid.NewGuid();
        var annotationObject = CreateObject(sessionId, 1, new PhysicalRect(-10, 20, 10, 40));
        var document = new AnnotationDocument(
            sessionId,
            AnnotationRevision.Initial,
            new[] { annotationObject });

        Assert.AreEqual(sessionId, document.SessionId);
        Assert.AreEqual(AnnotationRevision.Initial, document.Revision);
        Assert.AreEqual(annotationObject.Geometry, document.Objects[0].Geometry);
        AssertArgumentException(() => _ = new AnnotationObject(
                AnnotationObjectId.New(),
                sessionId,
                AnnotationToolKind.Rectangle,
                new PhysicalRect(0, 0, 0, 1),
                0));
        AssertArgumentException(() => _ = new AnnotationDocument(
            Guid.NewGuid(),
            AnnotationRevision.Initial,
            new[] { annotationObject }));
        AssertArgumentException(() => _ = new AnnotationDocument(
            sessionId,
            new AnnotationRevision(-1),
            Array.Empty<AnnotationObject>()));
        AssertArgumentException(() => _ = new AddAnnotationObjectRequest(
            Guid.Empty,
            AnnotationRevision.Initial,
            annotationObject));
        AssertArgumentException(() => _ = new AddAnnotationObjectRequest(
            sessionId,
            new AnnotationRevision(-1),
            annotationObject));
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void AnnotationDocumentExposesAnImmutableDeterministicCollection()
    {
        var sessionId = Guid.NewGuid();
        var high = CreateObject(sessionId, 2, new PhysicalRect(0, 0, 2, 2));
        var low = CreateObject(sessionId, 1, new PhysicalRect(3, 3, 5, 5));
        var document = new AnnotationDocument(
            sessionId,
            AnnotationRevision.Initial,
            new[] { high, low });

        Assert.AreEqual(low.ObjectId, document.Objects[0].ObjectId);
        Assert.AreEqual(high.ObjectId, document.Objects[1].ObjectId);
        Assert.IsInstanceOfType<IReadOnlyList<AnnotationObject>>(document.Objects);
        AssertNotSupported(() =>
            ((IList<AnnotationObject>)document.Objects)[0] = high);
        AssertArgumentException(() => _ = new AnnotationDocument(
            sessionId,
            AnnotationRevision.Initial,
            new[] { high, high }));
    }

    private static AnnotationObject CreateObject(Guid sessionId, int zOrder, PhysicalRect geometry) =>
        new(AnnotationObjectId.New(), sessionId, AnnotationToolKind.Rectangle, geometry, zOrder);

    private static void AssertArgumentException(Action action)
    {
        var threw = false;
        try
        {
            action();
        }
        catch (ArgumentException)
        {
            threw = true;
        }

        Assert.IsTrue(threw, "Expected an ArgumentException.");
    }

    private static void AssertNotSupported(Action action)
    {
        var threw = false;
        try
        {
            action();
        }
        catch (NotSupportedException)
        {
            threw = true;
        }

        Assert.IsTrue(threw, "Expected a NotSupportedException.");
    }
}
