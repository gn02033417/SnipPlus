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
}
