using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;
using Windows.Storage.Streams;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class SoftwareBitmapTests
{
    [TestMethod]
    [TestCategory("Rendering")]
    public void StraightBgraIsConvertedToDeterministicPremultipliedBgra8()
    {
        var metadata = CreateMetadata(1, 1, 4);
        using var result = SoftwareBitmapFactory.CreateFromStraightBgra(new byte[] { 100, 50, 200, 128 }, metadata);
        using var lease = result.AcquireBitmapLease();

        var pixels = ReadPixels(lease.Bitmap);

        CollectionAssert.AreEqual(new byte[] { 50, 25, 100, 128 }, pixels);
        Assert.AreEqual(global::Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, lease.Bitmap.BitmapPixelFormat);
        Assert.AreEqual(global::Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied, lease.Bitmap.BitmapAlphaMode);
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public void CropCopiesOnlyTheRequestedExclusiveBounds()
    {
        var metadata = CreateMetadata(4, 3, 16);
        var sourcePixels = new byte[]
        {
            0, 0, 0, 255, 1, 0, 0, 255, 2, 0, 0, 255, 3, 0, 0, 255,
            0, 1, 0, 255, 1, 1, 0, 255, 2, 1, 0, 255, 3, 1, 0, 255,
            0, 2, 0, 255, 1, 2, 0, 255, 2, 2, 0, 255, 3, 2, 0, 255
        };
        using var source = SoftwareBitmapFactory.CreateFromPremultipliedBgra(sourcePixels, metadata);

        using var cropped = SoftwareBitmapCropper.Crop(source, new PhysicalRect(1, 1, 3, 3), Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        using var lease = cropped.AcquireBitmapLease();

        var pixels = ReadPixels(lease.Bitmap);

        CollectionAssert.AreEqual(
            new byte[]
            {
                1, 1, 0, 255, 2, 1, 0, 255,
                1, 2, 0, 255, 2, 2, 0, 255
            },
            pixels);
        Assert.AreEqual(2, cropped.Metadata.PixelWidth);
        Assert.AreEqual(2, cropped.Metadata.PixelHeight);
        Assert.AreEqual(new PhysicalRect(1, 1, 3, 3), cropped.Metadata.CropPhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public void CropRejectsZeroSizeAndOutOfBoundsRegions()
    {
        using var source = SoftwareBitmapFactory.CreateFromPremultipliedBgra(
            new byte[4 * 4 * 4],
            CreateMetadata(4, 4, 16));

        AssertOutOfRange(() =>
            SoftwareBitmapCropper.Crop(source, new PhysicalRect(1, 1, 1, 3), Guid.NewGuid(), DateTimeOffset.UnixEpoch));
        AssertOutOfRange(() =>
            SoftwareBitmapCropper.Crop(source, new PhysicalRect(0, 0, 5, 4), Guid.NewGuid(), DateTimeOffset.UnixEpoch));
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public void FrozenMosaicPreviewUsesTheRequestedNegativeCoordinateBounds()
    {
        var sourcePixels = new byte[]
        {
            1, 0, 0, 255, 2, 0, 0, 255, 3, 0, 0, 255, 4, 0, 0, 255,
            5, 0, 0, 255, 6, 0, 0, 255, 7, 0, 0, 255, 8, 0, 0, 255
        };
        var sourceBounds = new PhysicalRect(-2, 10, 2, 12);
        using var source = SoftwareBitmapFactory.CreateFromPremultipliedBgra(
            sourcePixels,
            CreateMetadata(4, 2, 16) with
            {
                SourcePhysicalBounds = sourceBounds,
                CropPhysicalBounds = sourceBounds
            });
        using var preview = FrozenPrivacyEffectRenderer.Render(
            source,
            sourceBounds,
            sourceBounds,
            new PrivacyRegionAnnotationContent(
                PrivacyRegionMode.Mosaic,
                new PrivacyRegionEffectParameters(2, 2)));

        using var lease = preview.AcquireBitmapLease();
        CollectionAssert.AreEqual(
            new byte[]
            {
                3, 0, 0, 255, 3, 0, 0, 255, 5, 0, 0, 255, 5, 0, 0, 255,
                3, 0, 0, 255, 3, 0, 0, 255, 5, 0, 0, 255, 5, 0, 0, 255
            },
            ReadPixels(lease.Bitmap));
        Assert.AreEqual(sourceBounds, preview.Metadata.CropPhysicalBounds);
        Assert.AreEqual(sourceBounds, preview.Metadata.SourcePhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public void FrozenBlurPreviewIsDeterministicAndKeepsCanonicalMetadata()
    {
        var sourceBounds = new PhysicalRect(20, -4, 23, -3);
        using var source = SoftwareBitmapFactory.CreateFromPremultipliedBgra(
            new byte[]
            {
                0, 0, 0, 255,
                100, 0, 0, 255,
                200, 0, 0, 255
            },
            CreateMetadata(3, 1, 12) with
            {
                SourcePhysicalBounds = sourceBounds,
                CropPhysicalBounds = sourceBounds
            });
        using var preview = FrozenPrivacyEffectRenderer.Render(
            source,
            sourceBounds,
            sourceBounds,
            new PrivacyRegionAnnotationContent(
                PrivacyRegionMode.Blur,
                new PrivacyRegionEffectParameters(2, 1)));

        using var lease = preview.AcquireBitmapLease();
        CollectionAssert.AreEqual(
            new byte[]
            {
                33, 0, 0, 255,
                100, 0, 0, 255,
                166, 0, 0, 255
            },
            ReadPixels(lease.Bitmap));
        Assert.AreEqual(ImagePixelFormat.Bgra8, preview.Metadata.PixelFormat);
        Assert.AreEqual(ImageAlphaMode.Premultiplied, preview.Metadata.AlphaMode);
        Assert.AreEqual(ImageColorSpace.SrgbSdr, preview.Metadata.ColorSpace);
        Assert.AreEqual(source.Metadata.CapturedAt, preview.Metadata.CapturedAt);
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public async Task PngEncodingProducesAReadableInMemoryStream()
    {
        using var result = SoftwareBitmapFactory.CreateFromPremultipliedBgra(
            new byte[] { 1, 2, 3, 255 },
            CreateMetadata(1, 1, 4));

        using var stream = await PngEncoder.EncodeAsync(result, CancellationToken.None);

        Assert.IsTrue(stream.Size > 0);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void DisposingWithAnActiveLeaseDefersBitmapReleaseUntilTheLeaseEnds()
    {
        var result = SoftwareBitmapFactory.CreateFromPremultipliedBgra(
            new byte[] { 1, 2, 3, 255 },
            CreateMetadata(1, 1, 4));
        var lease = result.AcquireBitmapLease();

        result.Dispose();

        Assert.IsTrue(result.IsDisposed);
        Assert.AreEqual(1, lease.Bitmap.PixelWidth);
        lease.Dispose();
        var disposedFailureObserved = false;
        try
        {
            result.AcquireBitmapLease();
        }
        catch (ObjectDisposedException)
        {
            disposedFailureObserved = true;
        }

        Assert.IsTrue(disposedFailureObserved);
    }

    private static ImageResultMetadata CreateMetadata(int width, int height, int rowStride) => new()
    {
        ResultId = Guid.NewGuid(),
        SessionId = Guid.NewGuid(),
        PixelWidth = width,
        PixelHeight = height,
        PixelFormat = ImagePixelFormat.Bgra8,
        AlphaMode = ImageAlphaMode.Premultiplied,
        ColorSpace = ImageColorSpace.SrgbSdr,
        DpiX = 96,
        DpiY = 96,
        RowStride = rowStride,
        SourceKind = SourceKind.Monitor,
        SourcePhysicalBounds = new PhysicalRect(0, 0, width, height),
        CropPhysicalBounds = new PhysicalRect(0, 0, width, height),
        CapturedAt = DateTimeOffset.UnixEpoch
    };

    private static byte[] ReadPixels(global::Windows.Graphics.Imaging.SoftwareBitmap bitmap)
    {
        var byteCount = checked(bitmap.PixelWidth * bitmap.PixelHeight * 4);
        var buffer = new global::Windows.Storage.Streams.Buffer((uint)byteCount);
        bitmap.CopyToBuffer(buffer);
        var pixels = new byte[byteCount];
        using var reader = DataReader.FromBuffer(buffer);
        reader.ReadBytes(pixels);
        return pixels;
    }

    private static void AssertOutOfRange(Action action)
    {
        try
        {
            action();
        }
        catch (ArgumentOutOfRangeException)
        {
            return;
        }

        Assert.Fail("Expected ArgumentOutOfRangeException.");
    }
}
