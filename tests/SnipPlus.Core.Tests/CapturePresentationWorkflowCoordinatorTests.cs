using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class CapturePresentationWorkflowCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task CompleteFrameSetPresentsAllDisplaysAndMouseReleaseOnlyLocksSelection()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        using var workflow = CreateWorkflow(requests, provider, overlay);

        var started = await workflow.StartAsync(request, CancellationToken.None);

        var ready = started as CapturePresentationOutcome.SelectingReady;
        Assert.IsNotNull(ready);
        Assert.AreEqual(WorkflowState.Selecting, authority.CurrentState);
        Assert.AreEqual(1, provider.AcquireAllCalls);
        Assert.AreEqual(1, overlay.PresentCalls);
        Assert.AreEqual(2, overlay.LastPlan!.Displays.Count);
        Assert.IsNotNull(overlay.InputSink);

        var input = overlay.InputSink!;
        input.PointerPressed(Input(ready.Session, -3, 0));
        input.PointerMoved(Input(ready.Session, 3, 1));
        var locked = input.PointerReleased(Input(ready.Session, 3, 1));

        Assert.AreEqual(SelectionInputResultKind.Locked, locked.Kind);
        Assert.AreEqual(WorkflowState.SelectionLocked, authority.CurrentState);
        Assert.AreEqual(0, overlay.CloseCalls);

        var replacementStart = input.PointerPressed(Input(ready.Session, 20, 10));
        var replacementPreview = input.PointerMoved(Input(ready.Session, 22, 12));
        var replacement = input.PointerReleased(Input(ready.Session, 22, 12));

        Assert.AreEqual(SelectionInputResultKind.Reselecting, replacementStart.Kind);
        Assert.AreEqual(SelectionInputResultKind.Reselecting, replacementPreview.Kind);
        Assert.AreEqual(SelectionInputResultKind.AdjustmentCommitted, replacement.Kind);
        Assert.AreEqual(WorkflowState.SelectionLocked, authority.CurrentState);
        Assert.AreEqual(new PhysicalRect(20, 10, 22, 12), replacement.State.NormalizedPhysicalBounds);

        await workflow.CancelCurrentAsync("test");

        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(1, overlay.CloseCalls);
        Assert.IsTrue(ready.Session.IsDisposed);
    }

    [TestMethod]
    [TestCategory("Cancellation")]
    public async Task OverlayFailureCleansFramesAndNeverEntersSelecting()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator
        {
            Failure = Failure.Create(
                FailureCode.OverlayPresentationFailed,
                FailureCategory.Resource,
                FailureRecoverability.RetryNewIntent,
                "test-overlay",
                request.RequestId,
                "synthetic overlay failure")
        };
        using var workflow = CreateWorkflow(requests, provider, overlay);

        var result = await workflow.StartAsync(request, CancellationToken.None);

        Assert.IsInstanceOfType<CapturePresentationOutcome.Failed>(result);
        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.IsFalse(authority.CurrentState == WorkflowState.Selecting);
        Assert.IsTrue(provider.LastSession?.IsDisposed ?? false);
    }

    [TestMethod]
    [TestCategory("Cancellation")]
    public async Task EscapeCancelsSessionAndReturnsToResidentReadyWithoutOutput()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        using var workflow = CreateWorkflow(requests, provider, overlay);

        var result = await workflow.StartAsync(request, CancellationToken.None);
        var ready = (CapturePresentationOutcome.SelectingReady)result;
        var cancelled = overlay.InputSink!.Escape(
            ready.Session.SessionId,
            ready.Session.VirtualDesktopSnapshot.CoordinateVersion);
        await workflow.CancelCurrentAsync("test");

        Assert.AreEqual(SelectionInputResultKind.Cancelled, cancelled.Kind);
        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(1, overlay.CloseCalls);
        Assert.IsTrue(ready.Session.IsDisposed);
    }

    [TestMethod]
    [TestCategory("Cancellation")]
    public async Task EscapeCleanupLeavesWorkflowReadyForTheNextPrintScreenRequest()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var firstRequest = CaptureRequest.FromPrintScreen(
            new PrintScreenReceivedEventArgs(Guid.NewGuid(), DateTimeOffset.UnixEpoch));
        Assert.IsTrue(requests.Submit(firstRequest).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        using var workflow = CreateWorkflow(requests, provider, overlay);

        var firstResult = await workflow.StartAsync(firstRequest, CancellationToken.None);
        var firstReady = (CapturePresentationOutcome.SelectingReady)firstResult;

        await workflow.CancelCurrentAsync("Escape");
        await workflow.CancelCurrentAsync("LateEscape");

        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(1, overlay.CloseCalls);
        Assert.IsTrue(firstReady.Session.IsDisposed);

        var secondRequest = CaptureRequest.FromPrintScreen(
            new PrintScreenReceivedEventArgs(Guid.NewGuid(), DateTimeOffset.UnixEpoch.AddSeconds(1)));
        var accepted = requests.Submit(secondRequest);

        Assert.IsTrue(accepted.IsAccepted);
        var secondResult = await workflow.StartAsync(secondRequest, CancellationToken.None);
        Assert.IsInstanceOfType<CapturePresentationOutcome.SelectingReady>(secondResult);

        await workflow.CancelCurrentAsync("test");
        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(2, overlay.CloseCalls);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task LockedSelectionEntersEditingAndOnlyCancelIsEnabled()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        using var workflow = CreateWorkflow(requests, provider, overlay, functionBar);

        var result = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        var input = overlay.InputSink!;
        input.PointerPressed(Input(result.Session, -3, 0));
        input.PointerMoved(Input(result.Session, 3, 2));
        var locked = input.PointerReleased(Input(result.Session, 3, 2));

        Assert.AreEqual(SelectionInputResultKind.Locked, locked.Kind);
        Assert.AreEqual(WorkflowState.Editing, authority.CurrentState);
        Assert.AreEqual(SelectionStatus.Locked, workflow.CurrentSelection!.Status);
        Assert.AreEqual(1, functionBar.PrepareCalls);
        Assert.AreEqual(1, functionBar.ShowCalls);
        Assert.AreEqual(0, overlay.CloseCalls);

        foreach (var command in new[]
        {
            FunctionBarCommand.Complete,
            FunctionBarCommand.Save,
            FunctionBarCommand.Undo,
            FunctionBarCommand.Redo
        })
        {
            var disabled = workflow.Execute(new FunctionBarCommandRequest(
                result.Session.SessionId,
                result.Session.VirtualDesktopSnapshot.CoordinateVersion,
                locked.State.SelectionRevision,
                command));
            Assert.AreEqual(FunctionBarCommandResultKind.Disabled, disabled.Kind);
        }

        var cancel = workflow.Execute(new FunctionBarCommandRequest(
            result.Session.SessionId,
            result.Session.VirtualDesktopSnapshot.CoordinateVersion,
            locked.State.SelectionRevision,
            FunctionBarCommand.Cancel));
        Assert.AreEqual(FunctionBarCommandResultKind.Accepted, cancel.Kind);
        await workflow.CancelCurrentAsync("test");
        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.IsTrue(functionBar.CloseCalls >= 1);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Cancellation")]
    public async Task FunctionBarPreparationFailureDoesNotEnterEditing()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator
        {
            PreparationFailure = Failure.Create(
                FailureCode.BarMeasurementFailed,
                FailureCategory.Resource,
                FailureRecoverability.RetryNewIntent,
                "test-function-bar",
                request.RequestId,
                "synthetic measurement failure")
        };
        using var workflow = CreateWorkflow(requests, provider, overlay, functionBar);

        var result = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        var input = overlay.InputSink!;
        input.PointerPressed(Input(result.Session, -3, 0));
        input.PointerMoved(Input(result.Session, 3, 2));
        input.PointerReleased(Input(result.Session, 3, 2));
        await Task.Yield();

        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.AreEqual(1, functionBar.PrepareCalls);
        Assert.AreEqual(FailureCode.BarMeasurementFailed, functionBar.PreparationFailure!.Code);
        Assert.AreEqual(1, overlay.CloseCalls);
        Assert.IsTrue(provider.LastSession?.IsDisposed ?? false);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task EditingAdjustmentHidesAndRepositionsFunctionBar()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        using var workflow = CreateWorkflow(requests, provider, overlay, functionBar);

        var result = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        var input = overlay.InputSink!;
        input.PointerPressed(Input(result.Session, -3, 0));
        input.PointerMoved(Input(result.Session, 3, 2));
        var locked = input.PointerReleased(Input(result.Session, 3, 2));
        Assert.AreEqual(WorkflowState.Editing, authority.CurrentState);

        input.PointerPressed(Input(result.Session, 0, 1));
        input.PointerMoved(Input(result.Session, 5, 4));
        var adjusted = input.PointerReleased(Input(result.Session, 5, 4));

        Assert.AreEqual(SelectionInputResultKind.AdjustmentCommitted, adjusted.Kind);
        Assert.AreEqual(WorkflowState.Editing, authority.CurrentState);
        Assert.IsTrue(functionBar.HideCalls >= 1);
        Assert.IsTrue(functionBar.RepositionCalls >= 1);
        Assert.IsTrue(functionBar.ShowCalls >= 2);

        await workflow.CancelCurrentAsync("test");
    }

    private static CapturePresentationWorkflowCoordinator CreateWorkflow(
        CaptureRequestCoordinator requests,
        FakeAllDisplayProvider provider,
        FakeOverlayCoordinator overlay,
        IFunctionBarPresentationCoordinator? functionBar = null)
    {
        var freezing = new CaptureFreezingCoordinator(
            requests,
            new FixedTopologyProvider(CreateSnapshot()),
            provider);
        return new CapturePresentationWorkflowCoordinator(
            freezing,
            overlay,
            new HiddenSourceExclusion(),
            functionBarPresentation: functionBar);
    }

    private static SelectionPointerEvent Input(
        CaptureSessionContext session,
        int x,
        int y) => new(
        session.SessionId,
        session.VirtualDesktopSnapshot.CoordinateVersion,
        1,
        new PhysicalPoint(x, y));

    private static VirtualDesktopSnapshot CreateSnapshot() => new(
        "presentation-v1",
        new(-40, 0, 40, 20),
        new(-40, 0),
        new[]
        {
            Display("left", new(-40, 0, -20, 20)),
            Display("right", new(0, 0, 40, 20))
        });

    private static DisplaySnapshot Display(string id, PhysicalRect bounds) => new(
        id,
        bounds,
        1,
        1,
        "Landscape",
        new(bounds.Width, bounds.Height),
        $"surface:{id}");

    private sealed class FixedTopologyProvider : IDisplayTopologyProvider
    {
        private readonly VirtualDesktopSnapshot _snapshot;

        public FixedTopologyProvider(VirtualDesktopSnapshot snapshot) => _snapshot = snapshot;

        public ValueTask<DisplayTopologyOutcome> GetSnapshotAsync(
            CaptureRequest request,
            CancellationToken cancellationToken) =>
            ValueTask.FromResult<DisplayTopologyOutcome>(new DisplayTopologyOutcome.Succeeded(_snapshot));
    }

    private sealed class FakeAllDisplayProvider : IAllDisplayFrameProvider
    {
        public int AcquireAllCalls { get; private set; }

        public CaptureSessionContext? LastSession { get; private set; }

        public ValueTask<FrozenDisplayFrameAcquisitionOutcome> AcquireAsync(
            CaptureSessionContext session,
            DisplaySnapshot display,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException("The all-display path is required.");

        public ValueTask<FrozenDisplayFrameSetAcquisitionOutcome> AcquireAllAsync(
            CaptureSessionContext session,
            CancellationToken cancellationToken)
        {
            AcquireAllCalls++;
            LastSession = session;
            var snapshot = session.VirtualDesktopSnapshot;
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
            return ValueTask.FromResult<FrozenDisplayFrameSetAcquisitionOutcome>(
                new FrozenDisplayFrameSetAcquisitionOutcome.Succeeded(frameSet!));
        }
    }

    private sealed class HiddenSourceExclusion : ICaptureSourceExclusion
    {
        public ValueTask<CaptureSourceExclusionOutcome> ExcludeAsync(
            CaptureRequest request,
            CancellationToken cancellationToken) =>
            ValueTask.FromResult(CaptureSourceExclusionOutcome.Hidden());
    }

    private sealed class FakeOverlayCoordinator : IAllDisplayOverlayPresentationCoordinator
    {
        public Failure? Failure { get; set; }

        public int PresentCalls { get; private set; }

        public int CloseCalls { get; private set; }

        public FrozenDisplayOverlayPlan? LastPlan { get; private set; }

        public ISelectionInputSink? InputSink { get; private set; }

        public ValueTask<FrozenDisplayOverlayPresentationOutcome> PresentAsync(
            FrozenDisplayOverlayPresentationRequest request,
            CancellationToken cancellationToken)
        {
            PresentCalls++;
            LastPlan = request.Plan;
            InputSink = request.InputSink;
            return ValueTask.FromResult<FrozenDisplayOverlayPresentationOutcome>(
                Failure is null
                    ? new FrozenDisplayOverlayPresentationOutcome.Ready()
                    : new FrozenDisplayOverlayPresentationOutcome.Failed(Failure));
        }

        public void ApplySelection(SelectionVisualState state)
        {
        }

        public ValueTask CloseAsync(Guid sessionId, CancellationToken cancellationToken)
        {
            CloseCalls++;
            return ValueTask.CompletedTask;
        }

        public void Dispose()
        {
        }
    }

    private sealed class FakeFunctionBarPresentationCoordinator : IFunctionBarPresentationCoordinator
    {
        public Failure? PreparationFailure { get; init; }

        public int PrepareCalls { get; private set; }

        public int RepositionCalls { get; private set; }

        public int ShowCalls { get; private set; }

        public int HideCalls { get; private set; }

        public int CloseCalls { get; private set; }

        public FunctionBarPresentationResult Prepare(FunctionBarPresentationRequest request)
        {
            PrepareCalls++;
            return PreparationFailure is null
                ? Ready(request, FunctionBarPresentationResultKind.Ready)
                : Failed(request, PreparationFailure);
        }

        public FunctionBarPresentationResult Reposition(FunctionBarPresentationRequest request)
        {
            RepositionCalls++;
            return Ready(request, FunctionBarPresentationResultKind.Ready);
        }

        public FunctionBarPresentationResult Show(
            Guid sessionId,
            string coordinateVersion,
            int selectionRevision)
        {
            ShowCalls++;
            return new FunctionBarPresentationResult(
                FunctionBarPresentationResultKind.Shown,
                sessionId,
                coordinateVersion,
                selectionRevision,
                new FunctionBarPlacementResult(
                    "left",
                    new PhysicalRect(0, 8, 100, 48),
                    FunctionBarPlacementSide.Below,
                    selectionRevision,
                    true),
                null,
                "shown");
        }

        public FunctionBarPresentationResult Hide(Guid sessionId)
        {
            HideCalls++;
            return new FunctionBarPresentationResult(
                FunctionBarPresentationResultKind.Hidden,
                sessionId,
                string.Empty,
                0,
                null,
                null,
                "hidden");
        }

        public FunctionBarPresentationResult Close(Guid sessionId)
        {
            CloseCalls++;
            return new FunctionBarPresentationResult(
                FunctionBarPresentationResultKind.Closed,
                sessionId,
                string.Empty,
                0,
                null,
                null,
                "closed");
        }

        public void Dispose()
        {
        }

        private static FunctionBarPresentationResult Ready(
            FunctionBarPresentationRequest request,
            FunctionBarPresentationResultKind kind) => new(
            kind,
            request.SessionId,
            request.CoordinateVersion,
            request.Selection.SelectionRevision,
            new FunctionBarPlacementResult(
                "left",
                new PhysicalRect(0, 8, 100, 48),
                FunctionBarPlacementSide.Below,
                request.Selection.SelectionRevision,
                true),
            null,
            "ready");

        private static FunctionBarPresentationResult Failed(
            FunctionBarPresentationRequest request,
            Failure failure) => new(
            FunctionBarPresentationResultKind.Failed,
            request.SessionId,
            request.CoordinateVersion,
            request.Selection.SelectionRevision,
            null,
            failure,
            "failed");
    }
}
