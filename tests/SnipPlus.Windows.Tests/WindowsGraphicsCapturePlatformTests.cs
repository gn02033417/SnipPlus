using System.Globalization;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Windowing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
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

    [TestMethod]
    [TestCategory("Platform")]
    [TestCategory("Capture")]
    [TestCategory("Interactive")]
    public async Task WindowsMultiDisplayFrozenDisplayFrameSetRuntimeVerification()
    {
        EnsureWindowsAppRuntimeAndCaptureSupport();

        using var canvasDevice = CanvasDevice.GetSharedDevice();
        var topologyProvider = new WindowsDisplayTopologyProvider();
        var policy = new SupportedCapacityPolicy();
        var firstSnapshot = await GetRuntimeSnapshotAsync(topologyProvider);

        Assert.AreEqual(
            3,
            firstSnapshot.Displays.Count,
            "The authorized runtime baseline requires exactly three active displays.");
        Assert.AreEqual(GapPolicy.Transparent, firstSnapshot.GapPolicy);
        Assert.IsTrue(
            firstSnapshot.Displays.Any(display =>
                display.PhysicalBoundsInVirtualDesktop.Left < 0
                || display.PhysicalBoundsInVirtualDesktop.Top < 0),
            "The authorized runtime baseline requires at least one negative display coordinate.");

        var fhdDisplay = firstSnapshot.Displays.SingleOrDefault(display =>
            display.ExpectedFrozenFramePixelSize.Width == 1920
            && display.ExpectedFrozenFramePixelSize.Height == 1080);

        var capacity = policy.ValidateTopology(firstSnapshot);
        Assert.IsTrue(capacity.IsSupported, capacity.UserMessage);
        Assert.AreEqual(firstSnapshot.Displays.Count, firstSnapshot.Displays.Select(display => display.DisplayId).Distinct(StringComparer.Ordinal).Count());
        Assert.AreEqual(firstSnapshot.Displays.Count, firstSnapshot.Displays.Select(display => display.LogicalSurfaceIdentity).Distinct(StringComparer.Ordinal).Count());
        Assert.IsTrue(firstSnapshot.Displays.All(display => display.PhysicalBoundsInVirtualDesktop.IsPositive));
        Assert.IsTrue(firstSnapshot.Displays.All(display => firstSnapshot.VirtualPhysicalBounds.Contains(display.PhysicalBoundsInVirtualDesktop)));
        Assert.IsFalse(string.IsNullOrWhiteSpace(firstSnapshot.CoordinateVersion));

        var secondSnapshot = await GetRuntimeSnapshotAsync(topologyProvider);
        Assert.AreEqual(firstSnapshot.CoordinateVersion, secondSnapshot.CoordinateVersion);

        var totalSourcePixels = firstSnapshot.Displays.Sum(display =>
            (long)display.ExpectedFrozenFramePixelSize.Width * display.ExpectedFrozenFramePixelSize.Height);
        Assert.IsTrue(totalSourcePixels <= SupportedCapacityPolicy.MaxTotalSourcePixels);
        Assert.IsTrue(firstSnapshot.VirtualPhysicalBounds.Width64 <= SupportedCapacityPolicy.MaxVirtualDesktopWidth);
        Assert.IsTrue(firstSnapshot.VirtualPhysicalBounds.Height64 <= SupportedCapacityPolicy.MaxVirtualDesktopHeight);

        WriteTopologyMetadata(firstSnapshot, capacity, totalSourcePixels);
        if (fhdDisplay is null)
        {
            TestContext.WriteLine(
                "Owner reference difference: no 1920x1080 display was present in the current Windows topology; "
                + "the runtime result uses the current topology as required.");
        }
        else
        {
            Assert.AreEqual(1.5, fhdDisplay.DpiScaleX, 0.01);
            Assert.AreEqual(1.5, fhdDisplay.DpiScaleY, 0.01);
        }

        for (var sessionNumber = 1; sessionNumber <= 3; sessionNumber++)
        {
            var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UtcNow);
            var snapshot = sessionNumber == 1
                ? firstSnapshot
                : await GetRuntimeSnapshotAsync(topologyProvider);
            var sessionCapacity = policy.ValidateTopology(snapshot);
            Assert.IsTrue(sessionCapacity.IsSupported, sessionCapacity.UserMessage);

            using var session = new CaptureSessionContext(
                request,
                snapshot,
                sessionCapacity,
                null,
                CancellationToken.None);
            using var adapterFactory = new WindowsDisplayCaptureAdapterFactory(canvasDevice);
            using var frameProvider = new WindowsFrozenDisplayFrameSetProvider(
                adapterFactory,
                topologyProvider,
                TimeSpan.FromSeconds(5));

            var outcome = await frameProvider.AcquireAllAsync(session, CancellationToken.None);
            var succeeded = outcome as FrozenDisplayFrameSetAcquisitionOutcome.Succeeded;
            Assert.IsNotNull(succeeded, FormatFrameSetFailure(outcome));

            using var frameSet = succeeded.FrameSet;
            Assert.IsTrue(frameSet.IsComplete);
            Assert.AreEqual(snapshot.Displays.Count, frameSet.Frames.Count);
            Assert.AreEqual(session.SessionId, frameSet.SessionId);
            Assert.AreEqual(snapshot.CoordinateVersion, frameSet.CoordinateVersion);
            Assert.AreEqual(CaptureSessionStatus.Freezing, session.Status);
            Assert.IsNull(session.FrozenDisplayFrames);

            var displayIds = new HashSet<string>(StringComparer.Ordinal);
            var frameIds = new HashSet<Guid>();
            var capturedAt = new List<DateTimeOffset>(frameSet.Frames.Count);
            foreach (var display in snapshot.Displays)
            {
                Assert.IsTrue(frameSet.Frames.TryGetValue(display.DisplayId, out var frame));
                Assert.IsTrue(displayIds.Add(frame.DisplayId));
                Assert.IsTrue(frameIds.Add(frame.FrameId));
                Assert.AreEqual(session.SessionId, frame.SessionId);
                Assert.AreEqual(snapshot.CoordinateVersion, frame.CoordinateVersion);
                Assert.AreEqual(display.PhysicalBoundsInVirtualDesktop, frame.PhysicalBoundsInVirtualDesktop);
                Assert.AreEqual(display.ExpectedFrozenFramePixelSize, frame.PixelSize);

                var metadata = frame.FrozenFrame.ImageResult.Metadata;
                Assert.AreNotEqual(Guid.Empty, metadata.ResultId);
                Assert.AreEqual(session.SessionId, metadata.SessionId);
                Assert.AreEqual(ImagePixelFormat.Bgra8, metadata.PixelFormat);
                Assert.AreEqual(ImageAlphaMode.Premultiplied, metadata.AlphaMode);
                Assert.AreEqual(ImageColorSpace.SrgbSdr, metadata.ColorSpace);
                Assert.IsFalse(metadata.CursorIncluded);
                Assert.AreNotEqual(default, metadata.CapturedAt);
                Assert.AreEqual(display.ExpectedFrozenFramePixelSize.Width, metadata.PixelWidth);
                Assert.AreEqual(display.ExpectedFrozenFramePixelSize.Height, metadata.PixelHeight);
                capturedAt.Add(metadata.CapturedAt);

                TestContext.WriteLine(
                    $"Session {sessionNumber}; {SafeDisplayLabel(frame.DisplayId)}; "
                    + $"Bounds={FormatBounds(frame.PhysicalBoundsInVirtualDesktop)}; "
                    + $"Pixels={frame.PixelSize.Width}x{frame.PixelSize.Height}; "
                    + $"Dpi={display.DpiScaleX:F2}x{display.DpiScaleY:F2}; "
                    + $"Orientation={display.RotationOrOrientation}; "
                    + $"SessionId={session.SessionId}; CoordinateVersion={snapshot.CoordinateVersion}; "
                    + $"CapturedAt={metadata.CapturedAt:O}");
            }

            Assert.AreEqual(snapshot.Displays.Count, displayIds.Count);
            Assert.AreEqual(snapshot.Displays.Count, frameIds.Count);
            var minimumCapturedAt = capturedAt.Min();
            var maximumCapturedAt = capturedAt.Max();
            TestContext.WriteLine(
                $"Session {sessionNumber}; CapturedAtRange={minimumCapturedAt:O}..{maximumCapturedAt:O}; "
                + $"MaxDelta={(maximumCapturedAt - minimumCapturedAt).TotalMilliseconds:F2}ms");

            var currentCoordinateVersion = await topologyProvider
                .GetCurrentCoordinateVersionAsync(CancellationToken.None);
            Assert.AreEqual(snapshot.CoordinateVersion, currentCoordinateVersion);

            frameProvider.Dispose();
            frameProvider.Dispose();
            adapterFactory.Dispose();
            adapterFactory.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Platform")]
    [TestCategory("Capture")]
    [TestCategory("Interactive")]
    public async Task WindowsMultiDisplayFrozenDisplayFrameSetCancellationRuntimeVerification()
    {
        EnsureWindowsAppRuntimeAndCaptureSupport();

        using var canvasDevice = CanvasDevice.GetSharedDevice();
        var topologyProvider = new WindowsDisplayTopologyProvider();
        var snapshot = await GetRuntimeSnapshotAsync(topologyProvider);
        Assert.AreEqual(
            3,
            snapshot.Displays.Count,
            "The authorized runtime baseline requires exactly three active displays.");

        var capacity = new SupportedCapacityPolicy().ValidateTopology(snapshot);
        Assert.IsTrue(capacity.IsSupported, capacity.UserMessage);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UtcNow);
        using var session = new CaptureSessionContext(
            request,
            snapshot,
            capacity,
            null,
            CancellationToken.None);
        using var adapterFactory = new WindowsDisplayCaptureAdapterFactory(canvasDevice);
        using var frameProvider = new WindowsFrozenDisplayFrameSetProvider(
            adapterFactory,
            topologyProvider,
            TimeSpan.FromSeconds(5));
        using var cancellation = new CancellationTokenSource();

        var acquisition = frameProvider.AcquireAllAsync(session, cancellation.Token).AsTask();
        await Task.Delay(TimeSpan.FromMilliseconds(250));
        cancellation.Cancel();
        var outcome = await acquisition;

        switch (outcome)
        {
            case FrozenDisplayFrameSetAcquisitionOutcome.Cancelled cancelled:
                Assert.IsTrue(cancelled.CleanupCompleted);
                Assert.AreEqual(CaptureSessionStatus.Freezing, session.Status);
                TestContext.WriteLine("Cancellation outcome=Cancelled; cleanup=complete.");
                break;
            case FrozenDisplayFrameSetAcquisitionOutcome.Succeeded succeeded:
                using (succeeded.FrameSet)
                {
                    Assert.IsTrue(succeeded.FrameSet.IsComplete);
                    Assert.AreEqual(CaptureSessionStatus.Freezing, session.Status);
                }

                TestContext.WriteLine(
                    "Cancellation outcome=Succeeded because frame acquisition completed before cancellation; "
                    + "the runtime cancellation case is inconclusive for this hardware speed.");
                Assert.Inconclusive(
                    "Frame acquisition completed before the authorized cancellation could take effect.");
                break;
            case FrozenDisplayFrameSetAcquisitionOutcome.Failed failed:
                Assert.Fail(FormatFailure(failed.Failure));
                break;
            default:
                Assert.Fail("The Windows frame provider returned an unknown cancellation outcome.");
                break;
        }

        frameProvider.Dispose();
        frameProvider.Dispose();
        adapterFactory.Dispose();
        adapterFactory.Dispose();
    }

    public TestContext TestContext { get; set; } = null!;

    private static void EnsureWindowsAppRuntimeAndCaptureSupport()
    {
        if (!_windowsAppRuntimeInitialized)
        {
            Assert.Inconclusive(
                $"The test runner could not resolve the Windows App Runtime 2.3 package graph (HRESULT 0x{_windowsAppRuntimeHResult:X8}).");
        }

        Assert.IsTrue(
            WindowsGraphicsCaptureAdapter.IsSupported,
            "Windows.Graphics.Capture is unavailable on this Windows baseline.");
    }

    private static async Task<VirtualDesktopSnapshot> GetRuntimeSnapshotAsync(
        WindowsDisplayTopologyProvider topologyProvider)
    {
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UtcNow);
        var outcome = await topologyProvider.GetSnapshotAsync(request, CancellationToken.None);
        if (outcome is DisplayTopologyOutcome.Succeeded succeeded)
        {
            return succeeded.Snapshot;
        }

        if (outcome is DisplayTopologyOutcome.Cancelled cancelled)
        {
            Assert.Fail($"Display topology acquisition was cancelled: {cancelled.CancellationOrigin}");
        }

        var failed = (DisplayTopologyOutcome.Invalid)outcome;
        Assert.Fail(FormatFailure(failed.Failure));
        return null!;
    }

    private void WriteTopologyMetadata(
        VirtualDesktopSnapshot snapshot,
        CapacityValidationOutcome capacity,
        long totalSourcePixels)
    {
        TestContext.WriteLine(
            $"Topology Displays={snapshot.Displays.Count}; "
            + $"VirtualBounds={FormatBounds(snapshot.VirtualPhysicalBounds)}; "
            + $"CoordinateVersion={snapshot.CoordinateVersion}; GapPolicy={snapshot.GapPolicy}; "
            + $"Capacity={capacity.Kind}; TotalSourcePixels={totalSourcePixels}");

        foreach (var display in snapshot.Displays)
        {
            TestContext.WriteLine(
                $"{SafeDisplayLabel(display.DisplayId)}; "
                + $"Bounds={FormatBounds(display.PhysicalBoundsInVirtualDesktop)}; "
                + $"Dpi={display.DpiScaleX:F2}x{display.DpiScaleY:F2}; "
                + $"Orientation={display.RotationOrOrientation}");
        }
    }

    private static string FormatFrameSetFailure(
        FrozenDisplayFrameSetAcquisitionOutcome outcome) => outcome switch
        {
            FrozenDisplayFrameSetAcquisitionOutcome.Cancelled cancelled =>
                $"Frame acquisition was cancelled: {cancelled.CancellationOrigin}; cleanup={cancelled.CleanupCompleted}.",
            FrozenDisplayFrameSetAcquisitionOutcome.Failed failed => FormatFailure(failed.Failure),
            _ => "The Windows frame provider returned an unknown outcome."
        };

    private static string FormatFailure(Failure failure) =>
        $"Failure Code={failure.Code}; Category={failure.Category}; "
        + $"Recoverability={failure.Recoverability}; NativeCode={failure.NativeCode?.ToString(CultureInfo.InvariantCulture) ?? "none"}; "
        + $"Operation={failure.Operation}; Diagnostic={failure.DiagnosticMessage}";

    private static string SafeDisplayLabel(string displayId) =>
        displayId.StartsWith("display:", StringComparison.Ordinal)
            ? $"display-{displayId["display:".Length..]}"
            : "display-unknown";

    private static string FormatBounds(PhysicalRect bounds) =>
        $"{bounds.Left},{bounds.Top}..{bounds.Right},{bounds.Bottom}";

}
