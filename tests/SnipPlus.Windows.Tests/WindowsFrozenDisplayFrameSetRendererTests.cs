using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsFrozenDisplayFrameSetRendererTests
{
    [TestMethod]
    [TestCategory("Rendering")]
    [TestCategory("Contract")]
    public async Task ComposesFrozenFramesIntoSelectionAndLeavesTopologyGapTransparent()
    {
        var fixture = CreateFixture();
        var renderer = new WindowsFrozenDisplayFrameSetRenderer();
        using var result = await RenderSucceeded(
            renderer,
            fixture.FrameSet,
            new PhysicalRect(-1, 1, 3, 3));
        using var lease = ((SoftwareBitmapImageResult)result).AcquireBitmapLease();

        CollectionAssert.AreEqual(
            new byte[]
            {
                13, 13, 13, 255, 0, 0, 0, 0, 23, 23, 23, 255, 24, 24, 24, 255,
                0, 0, 0, 0, 0, 0, 0, 0, 26, 26, 26, 255, 27, 27, 27, 255
            },
            ReadPixels(lease.Bitmap));
        Assert.AreEqual(new PhysicalRect(-1, 1, 3, 3), result.Metadata.CropPhysicalBounds);
        Assert.AreEqual(ImagePixelFormat.Bgra8, result.Metadata.PixelFormat);
        Assert.AreEqual(ImageAlphaMode.Premultiplied, result.Metadata.AlphaMode);
        Assert.AreEqual(ImageColorSpace.SrgbSdr, result.Metadata.ColorSpace);

        fixture.Session.Dispose();
    }

    [TestMethod]
    [TestCategory("Rendering")]
    [TestCategory("Contract")]
    public async Task EmptySelectionIsRejectedWithoutCreatingAnImage()
    {
        var fixture = CreateFixture();
        var renderer = new WindowsFrozenDisplayFrameSetRenderer();

        var outcome = await renderer.RenderAsync(
            fixture.FrameSet,
            new PhysicalRect(0, 0, 0, 2),
            CancellationToken.None);

        var failed = outcome as FrozenDisplayFrameSetRenderOutcome.Failed;
        Assert.IsNotNull(failed);
        Assert.AreEqual(FailureCode.InvalidSelection, failed.Failure.Code);
        fixture.Session.Dispose();
    }

    private static async Task<IImageResult> RenderSucceeded(
        WindowsFrozenDisplayFrameSetRenderer renderer,
        FrozenDisplayFrameSet frameSet,
        PhysicalRect selection)
    {
        var outcome = await renderer.RenderAsync(frameSet, selection, CancellationToken.None);
        var succeeded = outcome as FrozenDisplayFrameSetRenderOutcome.Succeeded;
        Assert.IsNotNull(succeeded);
        return succeeded.ImageResult;
    }

    private static Fixture CreateFixture()
    {
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        var snapshot = new VirtualDesktopSnapshot(
            "renderer-v1",
            new(-2, 0, 4, 3),
            new(-2, 0),
            new[]
            {
                Display("left", new(-2, 0, 0, 2)),
                Display("right", new(1, 0, 4, 3))
            });
        var session = new CaptureSessionContext(
            request,
            snapshot,
            CapacityValidationOutcome.Supported(),
            null,
            CancellationToken.None);

        var leftBounds = new PhysicalRect(-2, 0, 0, 2);
        var rightBounds = new PhysicalRect(1, 0, 4, 3);
        var leftPixels = new byte[]
        {
            10, 10, 10, 255, 11, 11, 11, 255,
            12, 12, 12, 255, 13, 13, 13, 255
        };
        var rightPixels = new byte[]
        {
            20, 20, 20, 255, 21, 21, 21, 255, 22, 22, 22, 255,
            23, 23, 23, 255, 24, 24, 24, 255, 25, 25, 25, 255,
            26, 26, 26, 255, 27, 27, 27, 255, 28, 28, 28, 255
        };
        var frames = new[]
        {
            CreateFrame(session, "left", leftBounds, leftPixels),
            CreateFrame(session, "right", rightBounds, rightPixels)
        };
        Assert.IsTrue(FrozenDisplayFrameSet.TryCreate(
            session,
            snapshot.Displays,
            frames,
            out var frameSet,
            out var validation));
        Assert.IsTrue(validation.IsValid);
        Assert.IsTrue(session.TryAttachFrozenDisplayFrames(frameSet!));
        return new Fixture(session, frameSet!);
    }

    private static FrozenDisplayFrame CreateFrame(
        CaptureSessionContext session,
        string displayId,
        PhysicalRect bounds,
        byte[] pixels)
    {
        var metadata = new ImageResultMetadata
        {
            ResultId = Guid.NewGuid(),
            SessionId = session.SessionId,
            PixelWidth = bounds.Width,
            PixelHeight = bounds.Height,
            PixelFormat = ImagePixelFormat.Bgra8,
            AlphaMode = ImageAlphaMode.Premultiplied,
            ColorSpace = ImageColorSpace.SrgbSdr,
            DpiX = 96,
            DpiY = 96,
            RowStride = bounds.Width * 4,
            SourceKind = SourceKind.Monitor,
            SourcePhysicalBounds = bounds,
            CropPhysicalBounds = bounds,
            CapturedAt = DateTimeOffset.UnixEpoch
        };
        var image = SoftwareBitmapFactory.CreateFromPremultipliedBgra(pixels, metadata);
        return new FrozenDisplayFrame(
            session.SessionId,
            displayId,
            Guid.NewGuid(),
            session.VirtualDesktopSnapshot.CoordinateVersion,
            bounds,
            new PhysicalPixelSize(bounds.Width, bounds.Height),
            new FrozenCaptureFrame(image));
    }

    private static DisplaySnapshot Display(string id, PhysicalRect bounds) => new(
        id,
        bounds,
        1,
        1,
        "Landscape",
        new(bounds.Width, bounds.Height),
        $"surface:{id}");

    private static byte[] ReadPixels(SoftwareBitmap bitmap)
    {
        var buffer = new global::Windows.Storage.Streams.Buffer(
            (uint)(bitmap.PixelWidth * bitmap.PixelHeight * 4));
        bitmap.CopyToBuffer(buffer);
        var pixels = new byte[(int)buffer.Length];
        using var reader = DataReader.FromBuffer(buffer);
        reader.ReadBytes(pixels);
        return pixels;
    }

    private sealed record Fixture(CaptureSessionContext Session, FrozenDisplayFrameSet FrameSet);
}
