using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class Win2DRenderingTests
{
    [TestMethod]
    [TestCategory("Rendering")]
    public async Task SyntheticRectangleRenderIsDeterministicAndCanonical()
    {
        var intent = new RenderIntent
        {
            SceneId = Guid.NewGuid(),
            PixelWidth = 4,
            PixelHeight = 4,
            Target = RenderTargetKind.CanonicalRaster,
            Background = new RgbaColor(0, 0, 0, 0),
            Nodes = new RenderNode[]
            {
                new RenderNode.Rectangle(new PhysicalRect(1, 1, 3, 3), new RgbaColor(10, 20, 30, 255), true)
            }
        };

        var adapter = new Win2DRenderingAdapter();
        var first = await adapter.RenderAsync(intent, CancellationToken.None);
        var second = await adapter.RenderAsync(intent, CancellationToken.None);

        var firstSuccess = first as RenderOutcome.Succeeded;
        var secondSuccess = second as RenderOutcome.Succeeded;
        Assert.IsNotNull(firstSuccess?.CanonicalRaster);
        Assert.IsNotNull(secondSuccess?.CanonicalRaster);
        using (firstSuccess.CanonicalRaster)
        using (secondSuccess.CanonicalRaster)
        using (var firstLease = ((SoftwareBitmapImageResult)firstSuccess.CanonicalRaster).AcquireBitmapLease())
        using (var secondLease = ((SoftwareBitmapImageResult)secondSuccess.CanonicalRaster).AcquireBitmapLease())
        {
            var firstPixels = ReadPixels(firstLease.Bitmap);
            var secondPixels = ReadPixels(secondLease.Bitmap);
            CollectionAssert.AreEqual(firstPixels, secondPixels);
            CollectionAssert.AreEqual(new byte[] { 30, 20, 10, 255 }, firstPixels.Skip(20).Take(4).ToArray());
        }
    }

    [TestMethod]
    [TestCategory("Rendering")]
    public async Task CancelledRenderReturnsCancelledOutcome()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var adapter = new Win2DRenderingAdapter();

        var outcome = await adapter.RenderAsync(new RenderIntent
        {
            SceneId = Guid.NewGuid(),
            PixelWidth = 2,
            PixelHeight = 2
        }, cancellation.Token);

        Assert.IsInstanceOfType<RenderOutcome.Cancelled>(outcome);
    }

    private static byte[] ReadPixels(global::Windows.Graphics.Imaging.SoftwareBitmap bitmap)
    {
        var byteCount = checked(bitmap.PixelWidth * bitmap.PixelHeight * 4);
        var buffer = new global::Windows.Storage.Streams.Buffer((uint)byteCount);
        bitmap.CopyToBuffer(buffer);
        var pixels = new byte[byteCount];
        using var reader = global::Windows.Storage.Streams.DataReader.FromBuffer(buffer);
        reader.ReadBytes(pixels);
        return pixels;
    }
}
