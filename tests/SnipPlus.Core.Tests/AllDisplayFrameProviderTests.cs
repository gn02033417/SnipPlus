using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class AllDisplayFrameProviderTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task AllDisplayProviderIsCalledOnceAndAttachesOnlyACompleteFrameSet()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = requests.Submit(CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch)).Request;
        var snapshot = CreateSnapshot();
        var provider = new FakeAllDisplayFrameProvider();
        using var coordinator = new CaptureFreezingCoordinator(
            requests,
            new FixedTopologyProvider(snapshot),
            provider);

        var started = (CaptureFreezingOutcome.FreezingStarted)await coordinator
            .BeginFreezingAsync(request, CancellationToken.None);
        provider.FrameSetFactory = () => CreateFrameSet(started.Session, snapshot);

        var result = await coordinator.AcquireFrozenFramesAsync(started.Session, CancellationToken.None);

        Assert.IsInstanceOfType<CaptureFreezingOutcome.FrozenFrameSetReady>(result);
        Assert.AreEqual(1, provider.AcquireAllCalls);
        Assert.AreEqual(0, provider.AcquireSingleCalls);
        Assert.AreEqual(WorkflowState.Freezing, authority.CurrentState);
        Assert.AreEqual(CaptureSessionStatus.FrozenFrameSetReady, started.Session.Status);
        started.Session.Dispose();
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task UnsupportedCapacityStopsBeforeAllDisplayProviderIsCalled()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = requests.Submit(CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch)).Request;
        var displays = Enumerable.Range(0, 5)
            .Select(index => Display($"display:{index}", new(index * 2, 0, index * 2 + 2, 2)))
            .ToArray();
        var snapshot = new VirtualDesktopSnapshot(
            "unsupported-v1",
            new(0, 0, 10, 2),
            new(0, 0),
            displays);
        var provider = new FakeAllDisplayFrameProvider();
        using var coordinator = new CaptureFreezingCoordinator(
            requests,
            new FixedTopologyProvider(snapshot),
            provider);

        var result = await coordinator.BeginFreezingAsync(request, CancellationToken.None);

        Assert.IsInstanceOfType<CaptureFreezingOutcome.UnsupportedCapacity>(result);
        Assert.AreEqual(0, provider.AcquireAllCalls);
        Assert.AreEqual(0, provider.AcquireSingleCalls);
        Assert.AreEqual(WorkflowState.CaptureRequested, authority.CurrentState);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task AllDisplayFailureLeavesNoActiveSession()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = requests.Submit(CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch)).Request;
        var provider = new FakeAllDisplayFrameProvider
        {
            Failure = Failure.Create(
                FailureCode.PartialAcquisitionFailed,
                FailureCategory.Resource,
                FailureRecoverability.RetryNewIntent,
                "test",
                request.RequestId,
                "synthetic failure")
        };
        using var coordinator = new CaptureFreezingCoordinator(
            requests,
            new FixedTopologyProvider(CreateSnapshot()),
            provider);
        var started = (CaptureFreezingOutcome.FreezingStarted)await coordinator
            .BeginFreezingAsync(request, CancellationToken.None);

        var result = await coordinator.AcquireFrozenFramesAsync(started.Session, CancellationToken.None);

        var failed = result as CaptureFreezingOutcome.FrameFailed;
        Assert.IsNotNull(failed);
        Assert.AreEqual(FailureCode.PartialAcquisitionFailed, failed.Failure.Code);
        Assert.IsTrue(failed.CleanupCompleted);
        Assert.IsNull(coordinator.ActiveSession);
        Assert.IsTrue(started.Session.IsDisposed);
    }

    private static VirtualDesktopSnapshot CreateSnapshot() => new(
        "all-display-v1",
        new(-2, 0, 4, 2),
        new(-2, 0),
        new[]
        {
            Display("display:1", new(-2, 0, 0, 2)),
            Display("display:2", new(0, 0, 4, 2))
        });

    private static DisplaySnapshot Display(string id, PhysicalRect bounds) => new(
        id,
        bounds,
        1,
        1,
        "Landscape",
        new(bounds.Width, bounds.Height),
        $"surface:{id}");

    private static FrozenDisplayFrameSet CreateFrameSet(
        CaptureSessionContext session,
        VirtualDesktopSnapshot snapshot)
    {
        var frames = snapshot.Displays.Select(display => new FrozenDisplayFrame(
            session.SessionId,
            display.DisplayId,
            Guid.NewGuid(),
            snapshot.CoordinateVersion,
            display.PhysicalBoundsInVirtualDesktop,
            display.ExpectedFrozenFramePixelSize,
            new FrozenCaptureFrame(new TestImageResult(
                sessionId: session.SessionId,
                pixelWidth: display.ExpectedFrozenFramePixelSize.Width,
                pixelHeight: display.ExpectedFrozenFramePixelSize.Height,
                sourceBounds: display.PhysicalBoundsInVirtualDesktop))));
        Assert.IsTrue(FrozenDisplayFrameSet.TryCreate(
            session,
            snapshot.Displays,
            frames,
            out var frameSet,
            out var validation));
        Assert.IsTrue(validation.IsValid);
        return frameSet!;
    }

    private sealed class FixedTopologyProvider : IDisplayTopologyProvider
    {
        private readonly VirtualDesktopSnapshot _snapshot;

        public FixedTopologyProvider(VirtualDesktopSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public ValueTask<DisplayTopologyOutcome> GetSnapshotAsync(
            CaptureRequest request,
            CancellationToken cancellationToken) =>
            ValueTask.FromResult<DisplayTopologyOutcome>(new DisplayTopologyOutcome.Succeeded(_snapshot));
    }

    private sealed class FakeAllDisplayFrameProvider : IAllDisplayFrameProvider
    {
        public int AcquireAllCalls { get; private set; }

        public int AcquireSingleCalls { get; private set; }

        public Func<FrozenDisplayFrameSet>? FrameSetFactory { get; set; }

        public Failure? Failure { get; set; }

        public ValueTask<FrozenDisplayFrameAcquisitionOutcome> AcquireAsync(
            CaptureSessionContext session,
            DisplaySnapshot display,
            CancellationToken cancellationToken)
        {
            AcquireSingleCalls++;
            return ValueTask.FromResult<FrozenDisplayFrameAcquisitionOutcome>(new FrozenDisplayFrameAcquisitionOutcome.Failed(
                Failure ?? throw new InvalidOperationException("Unexpected single-display acquisition.")));
        }

        public ValueTask<FrozenDisplayFrameSetAcquisitionOutcome> AcquireAllAsync(
            CaptureSessionContext session,
            CancellationToken cancellationToken)
        {
            AcquireAllCalls++;
            if (Failure is not null)
            {
                return ValueTask.FromResult<FrozenDisplayFrameSetAcquisitionOutcome>(
                    new FrozenDisplayFrameSetAcquisitionOutcome.Failed(Failure, true));
            }

            return ValueTask.FromResult<FrozenDisplayFrameSetAcquisitionOutcome>(
                new FrozenDisplayFrameSetAcquisitionOutcome.Succeeded(FrameSetFactory!()));
        }
    }
}
