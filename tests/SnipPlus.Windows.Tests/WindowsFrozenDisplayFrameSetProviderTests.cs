using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsFrozenDisplayFrameSetProviderTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task PreparesAllAdaptersStartsAllThenCollectsOneFramePerDisplay()
    {
        var fixture = CreateFixture();
        var events = new List<string>();
        var factory = new FakeFactory(fixture.Snapshot, events);
        using var provider = new WindowsFrozenDisplayFrameSetProvider(factory);

        var result = await provider.AcquireAllAsync(fixture.Session, CancellationToken.None);

        var succeeded = result as FrozenDisplayFrameSetAcquisitionOutcome.Succeeded;
        Assert.IsNotNull(succeeded);
        Assert.AreEqual(2, succeeded.FrameSet.Frames.Count);
        Assert.IsTrue(events.IndexOf("prepare:display:1") < events.IndexOf("start:display:1"));
        Assert.IsTrue(events.IndexOf("prepare:display:2") < events.IndexOf("start:display:1"));
        Assert.IsTrue(events.IndexOf("start:display:1") < events.IndexOf("capture:display:1"));
        Assert.IsTrue(events.IndexOf("start:display:2") < events.IndexOf("capture:display:2"));
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.PrepareCalls == 1));
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.StartCalls == 1));
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.CaptureCalls == 1));
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.IsDisposed));

        succeeded.FrameSet.Dispose();
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.LastImage!.IsDisposed));
        fixture.Session.Dispose();
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task OneDisplayFailureDisposesEveryAdapterAndSuccessfulFrame()
    {
        var fixture = CreateFixture();
        var factory = new FakeFactory(fixture.Snapshot, new List<string>());
        factory.AdaptersToFail.Add("display:2");
        using var provider = new WindowsFrozenDisplayFrameSetProvider(factory);

        var result = await provider.AcquireAllAsync(fixture.Session, CancellationToken.None);

        var failed = result as FrozenDisplayFrameSetAcquisitionOutcome.Failed;
        Assert.IsNotNull(failed);
        Assert.AreEqual(FailureCode.PartialAcquisitionFailed, failed.Failure.Code);
        Assert.IsTrue(failed.CleanupCompleted);
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.IsDisposed));
        Assert.IsTrue(factory.Adapters.Single(adapter => adapter.DisplayId == "display:1").LastImage!.IsDisposed);
        fixture.Session.Dispose();
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task CancellationStopsAcquisitionAndCleansAdapters()
    {
        var fixture = CreateFixture();
        var factory = new FakeFactory(fixture.Snapshot, new List<string>())
        {
            Delay = TimeSpan.FromSeconds(1)
        };
        using var provider = new WindowsFrozenDisplayFrameSetProvider(factory);
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(20));

        var result = await provider.AcquireAllAsync(fixture.Session, cancellation.Token);

        var cancelled = result as FrozenDisplayFrameSetAcquisitionOutcome.Cancelled;
        Assert.IsNotNull(cancelled);
        Assert.AreEqual("CancellationToken", cancelled.CancellationOrigin);
        Assert.IsTrue(cancelled.CleanupCompleted);
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.IsDisposed));
        fixture.Session.Dispose();
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task TimeoutReturnsTypedFailureAndLateFramesAreDisposed()
    {
        var fixture = CreateFixture();
        var factory = new FakeFactory(fixture.Snapshot, new List<string>())
        {
            Delay = TimeSpan.FromMilliseconds(100),
            IgnoreCaptureCancellation = true
        };
        using var provider = new WindowsFrozenDisplayFrameSetProvider(
            factory,
            frameTimeout: TimeSpan.FromMilliseconds(10));

        var result = await provider.AcquireAllAsync(fixture.Session, CancellationToken.None);

        var failed = result as FrozenDisplayFrameSetAcquisitionOutcome.Failed;
        Assert.IsNotNull(failed);
        Assert.AreEqual(FailureCode.CaptureFrameTimeout, failed.Failure.Code);
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.IsDisposed));
        await Task.Delay(TimeSpan.FromMilliseconds(150));
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.LastImage?.IsDisposed == true));
        fixture.Session.Dispose();
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task CoordinateVersionChangeDisposesFramesAndDoesNotReturnASet()
    {
        var fixture = CreateFixture();
        var factory = new FakeFactory(fixture.Snapshot, new List<string>());
        using var provider = new WindowsFrozenDisplayFrameSetProvider(
            factory,
            new FakeRevisionSource("changed"));

        var result = await provider.AcquireAllAsync(fixture.Session, CancellationToken.None);

        var failed = result as FrozenDisplayFrameSetAcquisitionOutcome.Failed;
        Assert.IsNotNull(failed);
        Assert.AreEqual(FailureCode.DisplayContextChanged, failed.Failure.Code);
        Assert.IsTrue(factory.Adapters.All(adapter => adapter.LastImage!.IsDisposed));
        fixture.Session.Dispose();
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task StaleSessionDoesNotCreateAnyWindowsAdapter()
    {
        var fixture = CreateFixture();
        var factory = new FakeFactory(fixture.Snapshot, new List<string>());
        using var provider = new WindowsFrozenDisplayFrameSetProvider(factory);
        fixture.Session.Dispose();

        var result = await provider.AcquireAllAsync(fixture.Session, CancellationToken.None);

        var failed = result as FrozenDisplayFrameSetAcquisitionOutcome.Failed;
        Assert.IsNotNull(failed);
        Assert.AreEqual(FailureCode.StaleSession, failed.Failure.Code);
        Assert.AreEqual(0, factory.CreateCalls);
    }

    private static Fixture CreateFixture()
    {
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        var snapshot = new VirtualDesktopSnapshot(
            "provider-v1",
            new(0, 0, 4, 2),
            new(0, 0),
            new[]
            {
                Display("display:1", new(0, 0, 2, 2)),
                Display("display:2", new(2, 0, 4, 2))
            });
        return new Fixture(
            new CaptureSessionContext(
                request,
                snapshot,
                CapacityValidationOutcome.Supported(),
                null,
                CancellationToken.None),
            snapshot);
    }

    private static DisplaySnapshot Display(string id, PhysicalRect bounds) => new(
        id,
        bounds,
        1,
        1,
        "Landscape",
        new(bounds.Width, bounds.Height),
        $"surface:{id}");

    private sealed record Fixture(CaptureSessionContext Session, VirtualDesktopSnapshot Snapshot);

    private sealed class FakeRevisionSource : IWindowsDisplayTopologyRevisionSource
    {
        private readonly string _coordinateVersion;

        public FakeRevisionSource(string coordinateVersion)
        {
            _coordinateVersion = coordinateVersion;
        }

        public ValueTask<string?> GetCurrentCoordinateVersionAsync(CancellationToken cancellationToken) =>
            ValueTask.FromResult<string?>(_coordinateVersion);
    }

    private sealed class FakeFactory : IWindowsDisplayCaptureAdapterFactory
    {
        private readonly VirtualDesktopSnapshot _snapshot;
        private readonly List<string> _events;

        public FakeFactory(VirtualDesktopSnapshot snapshot, List<string> events)
        {
            _snapshot = snapshot;
            _events = events;
        }

        public List<FakeAdapter> Adapters { get; } = new();

        public HashSet<string> AdaptersToFail { get; } = new(StringComparer.Ordinal);

        public int CreateCalls { get; private set; }

        public TimeSpan Delay { get; init; }

        public bool IgnoreCaptureCancellation { get; init; }

        public ValueTask<WindowsDisplayCaptureAdapterCreationOutcome> CreateAsync(
            CaptureSessionContext session,
            DisplaySnapshot display,
            CancellationToken cancellationToken)
        {
            CreateCalls++;
            var adapter = new FakeAdapter(
                session,
                display,
                _events,
                AdaptersToFail.Contains(display.DisplayId),
                Delay,
                IgnoreCaptureCancellation);
            Adapters.Add(adapter);
            return ValueTask.FromResult<WindowsDisplayCaptureAdapterCreationOutcome>(
                new WindowsDisplayCaptureAdapterCreationOutcome.Succeeded(adapter));
        }
    }

    private sealed class FakeAdapter : IWindowsDisplayCaptureAdapter
    {
        private readonly CaptureSessionContext _session;
        private readonly DisplaySnapshot _display;
        private readonly List<string> _events;
        private readonly bool _fail;
        private readonly TimeSpan _delay;
        private readonly bool _ignoreCaptureCancellation;

        public FakeAdapter(
            CaptureSessionContext session,
            DisplaySnapshot display,
            List<string> events,
            bool fail,
            TimeSpan delay,
            bool ignoreCaptureCancellation)
        {
            _session = session;
            _display = display;
            _events = events;
            _fail = fail;
            _delay = delay;
            _ignoreCaptureCancellation = ignoreCaptureCancellation;
        }

        public string DisplayId => _display.DisplayId;

        public int PrepareCalls { get; private set; }

        public int StartCalls { get; private set; }

        public int CaptureCalls { get; private set; }

        public bool IsDisposed { get; private set; }

        public TrackingImageResult? LastImage { get; private set; }

        public ValueTask<WindowsDisplayCapturePreparationOutcome> PrepareAsync(
            CaptureSessionContext session,
            DisplaySnapshot display,
            CancellationToken cancellationToken)
        {
            PrepareCalls++;
            _events.Add($"prepare:{DisplayId}");
            return ValueTask.FromResult<WindowsDisplayCapturePreparationOutcome>(
                new WindowsDisplayCapturePreparationOutcome.Prepared());
        }

        public ValueTask<WindowsDisplayCaptureStartOutcome> StartAsync(
            CancellationToken cancellationToken)
        {
            StartCalls++;
            _events.Add($"start:{DisplayId}");
            return ValueTask.FromResult<WindowsDisplayCaptureStartOutcome>(
                new WindowsDisplayCaptureStartOutcome.Started());
        }

        public async ValueTask<WindowsDisplayCaptureFrameOutcome> CaptureFirstFrameAsync(
            CancellationToken cancellationToken)
        {
            CaptureCalls++;
            _events.Add($"capture:{DisplayId}");
            if (_delay > TimeSpan.Zero)
            {
                await Task.Delay(
                    _delay,
                    _ignoreCaptureCancellation ? CancellationToken.None : cancellationToken);
            }

            if (_fail)
            {
                return new WindowsDisplayCaptureFrameOutcome.Failed(
                    WindowsCaptureIntegrationOutcome.FailureResult(
                        WindowsCaptureIntegrationOutcomeKind.DisplaySourceUnavailable,
                        Failure.Create(
                            FailureCode.CaptureSourceUnavailable,
                            FailureCategory.Device,
                            FailureRecoverability.RetryNewIntent,
                            "fake",
                            _session.RequestId,
                            "synthetic display failure"),
                        true));
            }

            LastImage = new TrackingImageResult(
                _session.SessionId,
                _display.ExpectedFrozenFramePixelSize,
                _display.PhysicalBoundsInVirtualDesktop);
            return new WindowsDisplayCaptureFrameOutcome.Succeeded(new FrozenDisplayFrame(
                _session.SessionId,
                _display.DisplayId,
                Guid.NewGuid(),
                _session.VirtualDesktopSnapshot.CoordinateVersion,
                _display.PhysicalBoundsInVirtualDesktop,
                _display.ExpectedFrozenFramePixelSize,
                new FrozenCaptureFrame(LastImage)));
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    private sealed class TrackingImageResult : IImageResult
    {
        public TrackingImageResult(
            Guid sessionId,
            PhysicalPixelSize pixelSize,
            PhysicalRect sourceBounds)
        {
            Metadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = sessionId,
                PixelWidth = pixelSize.Width,
                PixelHeight = pixelSize.Height,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = 96,
                DpiY = 96,
                RowStride = pixelSize.Width * 4,
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = sourceBounds,
                CropPhysicalBounds = sourceBounds,
                CapturedAt = DateTimeOffset.UnixEpoch
            };
        }

        public ImageResultMetadata Metadata { get; }

        public bool IsDisposed { get; private set; }

        public IImageResultLease AcquireLease() => throw new NotSupportedException();

        public void Dispose() => IsDisposed = true;
    }
}
