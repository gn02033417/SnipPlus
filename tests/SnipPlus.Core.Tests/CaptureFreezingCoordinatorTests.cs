using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class CaptureFreezingCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task AcceptedRequestCreatesOneSessionAndOnlyCaptureRequestedToFreezingTransition()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = SubmitRequest(requests);
        using var coordinator = new CaptureFreezingCoordinator(
            requests,
            new FakeTopologyProvider(CreateTwoDisplaySnapshot()),
            new FakeFrameProvider());

        var result = await coordinator.BeginFreezingAsync(request, CancellationToken.None);

        var started = result as CaptureFreezingOutcome.FreezingStarted;
        Assert.IsNotNull(started);
        Assert.AreEqual(WorkflowState.Freezing, authority.CurrentState);
        Assert.AreEqual(2, authority.SuccessfulTransitionCount);
        Assert.AreEqual(request.RequestId, started.Session.RequestId);
        Assert.AreEqual(request.RequestedAt, started.Session.RequestedAt);
        Assert.AreNotEqual(Guid.Empty, started.Session.SessionId);
        Assert.AreSame(started.Session, coordinator.ActiveSession);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task SecondFreezingStartIsRejectedWithoutCreatingAnotherSession()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = SubmitRequest(requests);
        using var coordinator = new CaptureFreezingCoordinator(
            requests,
            new FakeTopologyProvider(CreateTwoDisplaySnapshot()),
            new FakeFrameProvider());

        var first = await coordinator.BeginFreezingAsync(request, CancellationToken.None);
        var second = await coordinator.BeginFreezingAsync(request, CancellationToken.None);

        Assert.IsInstanceOfType<CaptureFreezingOutcome.FreezingStarted>(first);
        var alreadyStarted = second as CaptureFreezingOutcome.AlreadyStarted;
        Assert.IsNotNull(alreadyStarted);
        Assert.AreEqual(((CaptureFreezingOutcome.FreezingStarted)first).Session.SessionId, alreadyStarted.SessionId);
        Assert.AreEqual(2, authority.SuccessfulTransitionCount);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task StaleRequestCannotCreateSessionOrAdvanceState()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var active = SubmitRequest(requests);
        var stale = CaptureRequest.CreateSecondary(Guid.NewGuid(), active.RequestedAt.AddSeconds(1));
        var topology = new FakeTopologyProvider(CreateTwoDisplaySnapshot());
        using var coordinator = new CaptureFreezingCoordinator(requests, topology, new FakeFrameProvider());

        var result = await coordinator.BeginFreezingAsync(stale, CancellationToken.None);

        Assert.IsInstanceOfType<CaptureFreezingOutcome.StaleRequest>(result);
        Assert.IsNull(coordinator.ActiveSession);
        Assert.AreEqual(WorkflowState.CaptureRequested, authority.CurrentState);
        Assert.AreEqual(0, topology.Calls);
    }

    [TestMethod]
    [TestCategory("Cancellation")]
    public async Task DisposedCoordinatorCannotStartFreezing()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = SubmitRequest(requests);
        var topology = new FakeTopologyProvider(CreateTwoDisplaySnapshot());
        var coordinator = new CaptureFreezingCoordinator(requests, topology, new FakeFrameProvider());
        coordinator.Dispose();

        var result = await coordinator.BeginFreezingAsync(request, CancellationToken.None);

        var cancelled = result as CaptureFreezingOutcome.Cancelled;
        Assert.IsNotNull(cancelled);
        Assert.AreEqual(WorkflowState.CaptureRequested, authority.CurrentState);
        Assert.AreEqual(0, topology.Calls);
        coordinator.Dispose();
    }

    [TestMethod]
    [TestCategory("Cancellation")]
    public async Task ExitingRequestBoundaryCannotStartFreezing()
    {
        var authority = new WorkflowStateAuthority();
        var requests = new CaptureRequestCoordinator(authority);
        var request = SubmitRequest(requests);
        var topology = new FakeTopologyProvider(CreateTwoDisplaySnapshot());
        using var coordinator = new CaptureFreezingCoordinator(requests, topology, new FakeFrameProvider());
        requests.Dispose();

        var result = await coordinator.BeginFreezingAsync(request, CancellationToken.None);

        Assert.IsInstanceOfType<CaptureFreezingOutcome.Cancelled>(result);
        Assert.AreEqual(WorkflowState.CaptureRequested, authority.CurrentState);
        Assert.AreEqual(0, topology.Calls);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task EveryDisplayIsAcquiredOnceAndFrameSetOwnsCleanup()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = SubmitRequest(requests);
        var provider = new FakeFrameProvider();
        using var coordinator = new CaptureFreezingCoordinator(
            requests,
            new FakeTopologyProvider(CreateTwoDisplaySnapshot()),
            provider);
        var started = (CaptureFreezingOutcome.FreezingStarted)await coordinator.BeginFreezingAsync(request, CancellationToken.None);

        var result = await coordinator.AcquireFrozenFramesAsync(started.Session, CancellationToken.None);

        var ready = result as CaptureFreezingOutcome.FrozenFrameSetReady;
        Assert.IsNotNull(ready);
        Assert.AreSame(started.Session, ready.Session);
        Assert.AreEqual(2, provider.Calls);
        Assert.AreEqual(2, ready.Session.FrozenDisplayFrames!.Frames.Count);
        Assert.AreEqual(CaptureSessionStatus.FrozenFrameSetReady, ready.Session.Status);
        Assert.AreEqual(2, provider.CreatedImages.Count);

        started.Session.Dispose();
        started.Session.Dispose();

        Assert.IsTrue(provider.CreatedImages.All(image => image.IsDisposed));
    }

    [TestMethod]
    [TestCategory("Cancellation")]
    public async Task FrameFailureDisposesPartialFramesAndSession()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = SubmitRequest(requests);
        var provider = new FakeFrameProvider(failingDisplayId: "second");
        using var coordinator = new CaptureFreezingCoordinator(
            requests,
            new FakeTopologyProvider(CreateTwoDisplaySnapshot()),
            provider);
        var started = (CaptureFreezingOutcome.FreezingStarted)await coordinator.BeginFreezingAsync(request, CancellationToken.None);

        var result = await coordinator.AcquireFrozenFramesAsync(started.Session, CancellationToken.None);

        var failed = result as CaptureFreezingOutcome.FrameFailed;
        Assert.IsNotNull(failed);
        Assert.IsTrue(failed.CleanupCompleted);
        Assert.IsTrue(started.Session.IsDisposed);
        Assert.IsNull(coordinator.ActiveSession);
        Assert.IsTrue(provider.CreatedImages.Single().IsDisposed);
        Assert.AreEqual(WorkflowState.Freezing, authority.CurrentState);
    }

    [TestMethod]
    [TestCategory("Cancellation")]
    public async Task CancellationDuringFrameAcquisitionDisposesPartialFrames()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = SubmitRequest(requests);
        var provider = new FakeFrameProvider(throwCancellationOnDisplayId: "second");
        using var coordinator = new CaptureFreezingCoordinator(
            requests,
            new FakeTopologyProvider(CreateTwoDisplaySnapshot()),
            provider);
        var started = (CaptureFreezingOutcome.FreezingStarted)await coordinator.BeginFreezingAsync(request, CancellationToken.None);

        var result = await coordinator.AcquireFrozenFramesAsync(started.Session, CancellationToken.None);

        var cancelled = result as CaptureFreezingOutcome.Cancelled;
        Assert.IsNotNull(cancelled);
        Assert.AreEqual("CancellationToken", cancelled.CancellationOrigin);
        Assert.IsTrue(started.Session.IsDisposed);
        Assert.IsTrue(provider.CreatedImages.Single().IsDisposed);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task StaleSessionCannotAcquireFramesForTheActiveSession()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = SubmitRequest(requests);
        using var coordinator = new CaptureFreezingCoordinator(
            requests,
            new FakeTopologyProvider(CreateTwoDisplaySnapshot()),
            new FakeFrameProvider());
        var started = (CaptureFreezingOutcome.FreezingStarted)await coordinator.BeginFreezingAsync(request, CancellationToken.None);
        var staleSession = new CaptureSessionContext(
            request,
            CreateTwoDisplaySnapshot(),
            CapacityValidationOutcome.Supported(),
            null,
            CancellationToken.None);

        var result = await coordinator.AcquireFrozenFramesAsync(staleSession, CancellationToken.None);

        Assert.IsInstanceOfType<CaptureFreezingOutcome.StaleRequest>(result);
        Assert.AreSame(started.Session, coordinator.ActiveSession);
        staleSession.Dispose();
    }

    [TestMethod]
    [TestCategory("Contract")]
    public async Task UnsupportedTopologyReturnsCapacityOutcomeBeforeFreezing()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = SubmitRequest(requests);
        var topology = new FakeTopologyProvider(new VirtualDesktopSnapshot(
            "oversized-v1",
            new(0, 0, 20_000, 100),
            new(0, 0),
            new[] { Display("one", new(0, 0, 100, 100)) }));
        using var coordinator = new CaptureFreezingCoordinator(requests, topology, new FakeFrameProvider());

        var result = await coordinator.BeginFreezingAsync(request, CancellationToken.None);

        var unsupported = result as CaptureFreezingOutcome.UnsupportedCapacity;
        Assert.IsNotNull(unsupported);
        Assert.AreEqual(CapacityValidationKind.UnsupportedVirtualDesktopBounds, unsupported.Validation.Kind);
        Assert.AreEqual(WorkflowState.CaptureRequested, authority.CurrentState);
        Assert.IsNull(coordinator.ActiveSession);
    }

    private static CaptureRequest SubmitRequest(CaptureRequestCoordinator requests)
    {
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch.AddMinutes(2));
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        return request;
    }

    private static VirtualDesktopSnapshot CreateTwoDisplaySnapshot() => new(
        "synthetic-v1",
        new(0, 0, 4, 2),
        new(0, 0),
        new[]
        {
            Display("first", new(0, 0, 2, 2)),
            Display("second", new(2, 0, 4, 2))
        });

    private static DisplaySnapshot Display(string id, PhysicalRect bounds) => new(
        id,
        bounds,
        1,
        1,
        "Landscape",
        new(bounds.Width, bounds.Height),
        $"surface-{id}");

    private sealed class FakeTopologyProvider : IDisplayTopologyProvider
    {
        private readonly VirtualDesktopSnapshot _snapshot;

        public FakeTopologyProvider(VirtualDesktopSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public int Calls { get; private set; }

        public ValueTask<DisplayTopologyOutcome> GetSnapshotAsync(
            CaptureRequest request,
            CancellationToken cancellationToken)
        {
            Calls++;
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult<DisplayTopologyOutcome>(new DisplayTopologyOutcome.Succeeded(_snapshot));
        }
    }

    private sealed class FakeFrameProvider : IFrozenDisplayFrameProvider
    {
        private readonly string? _failingDisplayId;
        private readonly string? _throwCancellationOnDisplayId;

        public FakeFrameProvider(
            string? failingDisplayId = null,
            string? throwCancellationOnDisplayId = null)
        {
            _failingDisplayId = failingDisplayId;
            _throwCancellationOnDisplayId = throwCancellationOnDisplayId;
        }

        public int Calls { get; private set; }

        public List<TestImageResult> CreatedImages { get; } = new();

        public ValueTask<FrozenDisplayFrameAcquisitionOutcome> AcquireAsync(
            CaptureSessionContext session,
            DisplaySnapshot display,
            CancellationToken cancellationToken)
        {
            Calls++;
            if (_throwCancellationOnDisplayId == display.DisplayId)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            if (_failingDisplayId == display.DisplayId)
            {
                return ValueTask.FromResult<FrozenDisplayFrameAcquisitionOutcome>(
                    new FrozenDisplayFrameAcquisitionOutcome.Failed(Failure.Create(
                        FailureCode.CaptureSourceUnavailable,
                        FailureCategory.Device,
                        FailureRecoverability.RetryNewIntent,
                        "synthetic-frame-provider",
                        session.RequestId,
                        "synthetic failure")));
            }

            var bounds = display.PhysicalBoundsInVirtualDesktop;
            var image = new TestImageResult(
                sessionId: session.SessionId,
                pixelWidth: display.ExpectedFrozenFramePixelSize.Width,
                pixelHeight: display.ExpectedFrozenFramePixelSize.Height,
                sourceBounds: bounds);
            CreatedImages.Add(image);
            var frame = new FrozenDisplayFrame(
                session.SessionId,
                display.DisplayId,
                Guid.NewGuid(),
                session.VirtualDesktopSnapshot.CoordinateVersion,
                bounds,
                display.ExpectedFrozenFramePixelSize,
                new FrozenCaptureFrame(image));
            return ValueTask.FromResult<FrozenDisplayFrameAcquisitionOutcome>(
                new FrozenDisplayFrameAcquisitionOutcome.Succeeded(frame));
        }
    }
}
