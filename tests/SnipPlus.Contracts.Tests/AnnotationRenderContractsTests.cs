using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Contracts.Tests;

[TestClass]
public sealed class AnnotationRenderContractsTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void ResultOwnsCanonicalImageAndExposesImmutableMetadata()
    {
        var sessionId = Guid.NewGuid();
        var image = new FakeImageResult(sessionId, 3, 2);
        var result = new AnnotationAwareRenderResult(
            Guid.NewGuid(),
            sessionId,
            4,
            new AnnotationRevision(7),
            image,
            2,
            3);

        Assert.AreEqual(sessionId, result.SessionId);
        Assert.AreEqual(3, result.PixelWidth);
        Assert.AreEqual(2, result.PixelHeight);
        Assert.AreEqual(2, result.RenderedObjectCount);
        Assert.AreEqual(3, result.TransparentGapPixelCount);
        Assert.IsTrue(result.HasTransparentGap);
        result.Dispose();
        Assert.IsTrue(image.IsDisposed);
        var threw = false;
        try
        {
            _ = result.ImageResult;
        }
        catch (ObjectDisposedException)
        {
            threw = true;
        }

        Assert.IsTrue(threw);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void BoundaryAssemblyDoesNotReferenceWindowsRenderingTypes()
    {
        var references = typeof(AnnotationAwareRenderRequest).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToArray();

        CollectionAssert.DoesNotContain(references, "Microsoft.Graphics.Win2D");
        CollectionAssert.DoesNotContain(references, "Microsoft.UI.Xaml");
        CollectionAssert.DoesNotContain(references, "Microsoft.WindowsAppSDK");
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void TypedOutcomeKindsRemainDistinct()
    {
        var sessionId = Guid.NewGuid();
        var outcomes = new AnnotationAwareRenderOutcome[]
        {
            new AnnotationAwareRenderOutcome.Cancelled(sessionId, "test"),
            new AnnotationAwareRenderOutcome.StaleCoordinateVersion(sessionId, "old", "new", "test"),
            new AnnotationAwareRenderOutcome.StaleSelectionRevision(sessionId, 1, 2, "test"),
            new AnnotationAwareRenderOutcome.StaleAnnotationRevision(sessionId, new(1), new(2), "test"),
            new AnnotationAwareRenderOutcome.InvalidSelection(sessionId, "test"),
            new AnnotationAwareRenderOutcome.InvalidFrameSet(sessionId, "test"),
            new AnnotationAwareRenderOutcome.InvalidAnnotationDocument(sessionId, "test"),
            new AnnotationAwareRenderOutcome.UnsupportedAnnotation(sessionId, AnnotationToolKind.Text, "test"),
            new AnnotationAwareRenderOutcome.RenderCapacityExceeded(sessionId, "test")
        };

        CollectionAssert.AreEqual(
            new[]
            {
                AnnotationAwareRenderOutcomeKind.Cancelled,
                AnnotationAwareRenderOutcomeKind.StaleCoordinateVersion,
                AnnotationAwareRenderOutcomeKind.StaleSelectionRevision,
                AnnotationAwareRenderOutcomeKind.StaleAnnotationRevision,
                AnnotationAwareRenderOutcomeKind.InvalidSelection,
                AnnotationAwareRenderOutcomeKind.InvalidFrameSet,
                AnnotationAwareRenderOutcomeKind.InvalidAnnotationDocument,
                AnnotationAwareRenderOutcomeKind.UnsupportedAnnotation,
                AnnotationAwareRenderOutcomeKind.RenderCapacityExceeded
            },
            outcomes.Select(outcome => outcome.Kind).ToArray());
    }

    private sealed class FakeImageResult : IImageResult
    {
        public FakeImageResult(Guid sessionId, int width, int height)
        {
            Metadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = sessionId,
                PixelWidth = width,
                PixelHeight = height,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = 96,
                DpiY = 96,
                RowStride = width * 4,
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = new(0, 0, width, height),
                CropPhysicalBounds = new(0, 0, width, height),
                CapturedAt = DateTimeOffset.UnixEpoch
            };
        }

        public ImageResultMetadata Metadata { get; }

        public bool IsDisposed { get; private set; }

        public IImageResultLease AcquireLease() => throw new NotSupportedException();

        public void Dispose() => IsDisposed = true;
    }
}
