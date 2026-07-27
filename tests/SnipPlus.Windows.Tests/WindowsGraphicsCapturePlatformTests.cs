using System.Globalization;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Windowing;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsGraphicsCapturePlatformTests
{
    private const uint WindowsAppSdkMajorMinor = 0x0002_0003;
    private static bool _windowsAppRuntimeInitialized;
    private static int _windowsAppRuntimeHResult;

    [AssemblyInitialize]
    public static void InitializeWindowsAppRuntime(TestContext context)
    {
        _windowsAppRuntimeInitialized = Bootstrap.TryInitialize(
            WindowsAppSdkMajorMinor,
            out _windowsAppRuntimeHResult);
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
                $"The test runner could not resolve the Windows App Runtime 2.3 package graph (HRESULT 0x{_windowsAppRuntimeHResult:X8}).");
        }

        Assert.IsTrue(
            WindowsGraphicsCaptureAdapter.IsSupported,
            "Windows.Graphics.Capture is unavailable on this Windows baseline.");

        var primaryPoint = new global::Windows.Graphics.PointInt32 { X = 1, Y = 1 };
        var displayArea = DisplayArea.GetFromPoint(primaryPoint, DisplayAreaFallback.Primary);
        var displayId = new global::Windows.Graphics.DisplayId { Value = displayArea.DisplayId.Value };
        var captureItem = global::Windows.Graphics.Capture.GraphicsCaptureItem.TryCreateFromDisplayId(displayId);

        if (captureItem is null)
        {
            Assert.Inconclusive($"The Windows platform could not create a GraphicsCaptureItem for display id {displayId.Value.ToString(CultureInfo.InvariantCulture)}.");
        }

        using var device = CanvasDevice.GetSharedDevice();
        using var adapter = new WindowsGraphicsCaptureAdapter(device, captureItem);
        var outerBounds = displayArea.OuterBounds;
        var sourceBounds = new PhysicalRect(
            outerBounds.X,
            outerBounds.Y,
            outerBounds.X + outerBounds.Width,
            outerBounds.Y + outerBounds.Height);
        var requestId = Guid.NewGuid();
        var intent = new CaptureIntent
        {
            RequestId = requestId,
            SessionId = Guid.NewGuid(),
            SourceKind = SourceKind.Monitor,
            SourceId = displayId.Value.ToString(CultureInfo.InvariantCulture),
            SourcePhysicalBounds = sourceBounds,
            SelectionDipBounds = new DipRect(0, 0, sourceBounds.Width, sourceBounds.Height),
            SelectionPhysicalBounds = sourceBounds,
            CropBoundsInSource = new PhysicalRect(0, 0, sourceBounds.Width, sourceBounds.Height),
            DpiScaleX = 1,
            DpiScaleY = 1,
            CoordinateVersion = "platform-test",
            Cancellation = CancellationToken.None
        };

        var outcome = await adapter.CaptureFrameAsync(intent, CancellationToken.None);
        if (outcome is CaptureFrameOutcome.Succeeded succeeded)
        {
            using (succeeded.FrozenFrame)
            {
                Assert.IsTrue(succeeded.FrozenFrame.ImageResult.Metadata.PixelWidth > 0);
                Assert.IsTrue(succeeded.FrozenFrame.ImageResult.Metadata.PixelHeight > 0);
                Assert.AreEqual(sourceBounds.Width, succeeded.FrozenFrame.ImageResult.Metadata.PixelWidth);
                Assert.AreEqual(sourceBounds.Height, succeeded.FrozenFrame.ImageResult.Metadata.PixelHeight);
                Assert.AreEqual(ImagePixelFormat.Bgra8, succeeded.FrozenFrame.ImageResult.Metadata.PixelFormat);
                Assert.AreEqual(ImageAlphaMode.Premultiplied, succeeded.FrozenFrame.ImageResult.Metadata.AlphaMode);
                Assert.AreEqual(ImageColorSpace.SrgbSdr, succeeded.FrozenFrame.ImageResult.Metadata.ColorSpace);
            }

            return;
        }

        if (outcome is CaptureFrameOutcome.Cancelled cancelled)
        {
            Assert.Fail($"Capture was cancelled unexpectedly: {cancelled.CancellationOrigin}");
        }

        var failure = ((CaptureFrameOutcome.Failed)outcome).Failure;
        Assert.Fail($"Monitor frame capture failed: {failure.Code} ({failure.DiagnosticMessage})");
    }

}
