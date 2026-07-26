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
