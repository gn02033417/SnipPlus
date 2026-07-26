using System.Globalization;
using Microsoft.Graphics.Canvas;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsGraphicsCapturePlatformTests
{
    private static bool _windowsAppRuntimeInitialized;

    [AssemblyInitialize]
    public static void InitializeWindowsAppRuntime(TestContext context)
    {
        _windowsAppRuntimeInitialized = Bootstrap.TryInitialize(0, out _);
    }

    [AssemblyCleanup]
    public static void ShutdownWindowsAppRuntime()
    {
        if (_windowsAppRuntimeInitialized)
        {
            Bootstrap.Shutdown();
        }
    }

    [TestMethod]
    [TestCategory("Platform")]
    [TestCategory("Capture")]
    [TestCategory("Interactive")]
    public void WindowsGraphicsCaptureSupportIsObservable()
    {
        Assert.IsTrue(
            WindowsGraphicsCaptureAdapter.IsSupported,
            "Windows.Graphics.Capture is unavailable on this Windows baseline.");
    }

    [TestMethod]
    [TestCategory("Platform")]
    [TestCategory("Capture")]
    [TestCategory("Interactive")]
    public async Task WindowsGraphicsCaptureProducesOneInMemoryFrame()
    {
        if (!_windowsAppRuntimeInitialized)
        {
            Assert.Inconclusive(
                "The un-packaged test runner could not resolve a Windows App Runtime package graph; run this verification from the packaged app runtime.");
        }

        Assert.IsTrue(
            WindowsGraphicsCaptureAdapter.IsSupported,
            "Windows.Graphics.Capture is unavailable on this Windows baseline.");

        global::Windows.Graphics.DisplayId displayId = default;
        global::Windows.Graphics.Capture.GraphicsCaptureItem? captureItem = null;
        for (ulong value = 1; value <= 16 && captureItem is null; value++)
        {
            displayId = new global::Windows.Graphics.DisplayId { Value = value };
            captureItem = global::Windows.Graphics.Capture.GraphicsCaptureItem.TryCreateFromDisplayId(displayId);
        }

        if (captureItem is null)
        {
            Assert.Inconclusive("The test runner could not resolve a logical display id.");
        }

        using var device = CanvasDevice.GetSharedDevice();
        var adapter = new WindowsGraphicsCaptureAdapter(device, captureItem);
        var sourceBounds = new PhysicalRect(0, 0, 1, 1);
        var requestId = Guid.NewGuid();
        var intent = new CaptureIntent
        {
            RequestId = requestId,
            SessionId = Guid.NewGuid(),
            SourceKind = SourceKind.Monitor,
            SourceId = displayId.Value.ToString(CultureInfo.InvariantCulture),
            SourcePhysicalBounds = sourceBounds,
            SelectionDipBounds = new DipRect(0, 0, 1, 1),
            SelectionPhysicalBounds = new PhysicalRect(sourceBounds.Left, sourceBounds.Top, sourceBounds.Left + 1, sourceBounds.Top + 1),
            CropBoundsInSource = new PhysicalRect(0, 0, 1, 1),
            DpiScaleX = 1,
            DpiScaleY = 1,
            CoordinateVersion = "platform-test",
            Cancellation = CancellationToken.None
        };

        var outcome = await adapter.CaptureAsync(intent, CancellationToken.None);
        if (outcome is CaptureOutcome.Succeeded succeeded)
        {
            using (succeeded.ImageResult)
            {
                Assert.AreEqual(1, succeeded.ImageResult.Metadata.PixelWidth);
                Assert.AreEqual(1, succeeded.ImageResult.Metadata.PixelHeight);
                Assert.AreEqual(ImagePixelFormat.Bgra8, succeeded.ImageResult.Metadata.PixelFormat);
                Assert.AreEqual(ImageAlphaMode.Premultiplied, succeeded.ImageResult.Metadata.AlphaMode);
                Assert.AreEqual(ImageColorSpace.SrgbSdr, succeeded.ImageResult.Metadata.ColorSpace);
            }

            return;
        }

        if (outcome is CaptureOutcome.Cancelled cancelled)
        {
            Assert.Fail($"Capture was cancelled unexpectedly: {cancelled.CancellationOrigin}");
        }

        var failure = ((CaptureOutcome.Failed)outcome).Failure;
        Assert.Fail($"Monitor frame capture failed: {failure.Code} ({failure.DiagnosticMessage})");
    }

}
