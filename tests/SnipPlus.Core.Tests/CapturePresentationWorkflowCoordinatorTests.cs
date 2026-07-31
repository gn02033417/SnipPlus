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
    public async Task LockedSelectionEntersEditingAndCompleteAndCancelAreEnabled()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        var renderer = new FakeFinalRenderer();
        var clipboard = new FakeClipboardDelivery();
        using var workflow = CreateWorkflow(
            requests,
            provider,
            overlay,
            functionBar,
            renderer,
            clipboard);

        var result = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        var input = overlay.InputSink!;
        input.PointerPressed(Input(result.Session, -3, 0));
        input.PointerMoved(Input(result.Session, 3, 2));
        var locked = input.PointerReleased(Input(result.Session, 3, 2));

        Assert.AreEqual(SelectionInputResultKind.Locked, locked.Kind);
        Assert.AreEqual(WorkflowState.Editing, authority.CurrentState);
        Assert.AreEqual(SelectionStatus.Locked, workflow.CurrentSelection!.Status);
        Assert.IsNotNull(workflow.CurrentAnnotationDocument);
        Assert.AreEqual(AnnotationRevision.Initial, workflow.CurrentAnnotationDocument!.Revision);
        Assert.IsEmpty(workflow.CurrentAnnotationDocument.Objects);
        Assert.AreEqual(1, functionBar.PrepareCalls);
        Assert.AreEqual(1, functionBar.ShowCalls);
        Assert.AreEqual(0, overlay.CloseCalls);

        Assert.IsTrue(functionBar.LastRequest!.Availability.IsEnabled(FunctionBarCommand.Complete));
        Assert.IsTrue(functionBar.LastRequest.Availability.IsEnabled(FunctionBarCommand.Cancel));

        foreach (var command in new[]
        {
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

        var complete = workflow.Execute(new FunctionBarCommandRequest(
            result.Session.SessionId,
            result.Session.VirtualDesktopSnapshot.CoordinateVersion,
            locked.State.SelectionRevision,
            FunctionBarCommand.Complete));
        Assert.AreEqual(FunctionBarCommandResultKind.Accepted, complete.Kind);
        await WaitForStateAsync(authority, WorkflowState.ResidentReady);
        Assert.AreEqual(1, renderer.Calls);
        Assert.AreEqual(1, provider.AcquireAllCalls);
        Assert.AreEqual(1, clipboard.Calls);
        Assert.AreSame(renderer.LastImageResult, clipboard.LastRequest!.ImageResult);
        Assert.AreEqual(1, overlay.CloseCalls);
        Assert.IsTrue(renderer.LastImageResult?.IsDisposed ?? false);
        Assert.IsTrue(result.Session.IsDisposed);
        Assert.IsNull(workflow.CurrentAnnotationDocument);
        Assert.AreEqual(WorkflowState.ResidentReady, authority.CurrentState);
        Assert.IsTrue(functionBar.CloseCalls >= 1);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Rendering")]
    public async Task NonEmptyAnnotationDocumentBlocksBaseCompleteAndRetainsEditing()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        var renderer = new FakeFinalRenderer();
        var clipboard = new FakeClipboardDelivery();
        var trace = new FakeCompleteExecutionTraceSink();
        using var workflow = CreateWorkflow(
            requests,
            provider,
            overlay,
            functionBar,
            renderer,
            clipboard,
            trace);

        var ready = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        LockSelection(overlay.InputSink!, ready.Session);
        var retainedObject = new AnnotationObject(
            AnnotationObjectId.New(),
            ready.Session.SessionId,
            AnnotationToolKind.Rectangle,
            new PhysicalRect(-2, 1, 2, 4),
            1);
        var retainedDocument = (workflow.AddAnnotationObject(new AddAnnotationObjectRequest(
            ready.Session.SessionId,
            workflow.CurrentAnnotationDocument!.Revision,
            retainedObject)) as AnnotationMutationResult.Succeeded)!.Document;
        var accepted = workflow.Execute(new FunctionBarCommandRequest(
            ready.Session.SessionId,
            ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
            workflow.CurrentSelection!.SelectionRevision,
            FunctionBarCommand.Complete));

        Assert.AreEqual(FunctionBarCommandResultKind.AnnotationOutputNotSupported, accepted.Kind);
        await Task.Yield();
        Assert.AreEqual(WorkflowState.Editing, authority.CurrentState);
        Assert.AreEqual(0, renderer.Calls);
        Assert.AreEqual(0, clipboard.Calls);
        Assert.AreEqual(0, overlay.CloseCalls);
        Assert.IsFalse(ready.Session.IsDisposed);
        Assert.IsNotNull(workflow.CurrentAnnotationDocument);
        Assert.AreEqual(retainedDocument.Revision, workflow.CurrentAnnotationDocument!.Revision);
        Assert.AreEqual(retainedObject.ObjectId, workflow.CurrentAnnotationDocument.Objects.Single().ObjectId);
        Assert.AreEqual(1, functionBar.FeedbackCalls);
        Assert.AreEqual(
            "Rectangle annotations are retained; Complete output is not available in this slice.",
            functionBar.LastFeedback);
        Assert.AreEqual(FunctionBarCommandAvailability.Stage6C, functionBar.LastRequest!.Availability);
        Assert.IsFalse(trace.Entries.Any(entry => entry.CompleteStage == CompleteExecutionStage.Rendering));
        await workflow.CancelCurrentAsync("test");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task TraceSinkFailureDoesNotChangeCompleteFailureRecovery()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        var renderer = new FakeFinalRenderer
        {
            Outcome = new FrozenDisplayFrameSetRenderOutcome.Failed(Failure.Create(
                FailureCode.RenderingFailed,
                FailureCategory.Resource,
                FailureRecoverability.RetrySameIntent,
                "test-renderer",
                request.RequestId,
                "synthetic renderer failure"))
        };
        var clipboard = new FakeClipboardDelivery();
        using var workflow = CreateWorkflow(
            requests,
            provider,
            overlay,
            functionBar,
            renderer,
            clipboard,
            new ThrowingCompleteExecutionTraceSink());

        var ready = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        LockSelection(overlay.InputSink!, ready.Session);

        var accepted = workflow.Execute(new FunctionBarCommandRequest(
            ready.Session.SessionId,
            ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
            workflow.CurrentSelection!.SelectionRevision,
            FunctionBarCommand.Complete));

        Assert.AreEqual(FunctionBarCommandResultKind.Accepted, accepted.Kind);
        await WaitForAsync(() => renderer.Calls == 1);
        Assert.AreEqual(WorkflowState.Editing, authority.CurrentState);
        Assert.AreEqual(1, functionBar.FeedbackCalls);
        Assert.AreEqual("無法產生截圖影像，請再試一次。", functionBar.LastFeedback);
        await workflow.CancelCurrentAsync("test");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public async Task CompleteCommandGateRejectsASecondCommandWhileTheFirstIsRunning()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        var renderer = new BlockingFinalRenderer();
        var clipboard = new FakeClipboardDelivery();
        using var workflow = CreateWorkflow(
            requests,
            provider,
            overlay,
            functionBar,
            renderer,
            clipboard);

        var ready = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        LockSelection(overlay.InputSink!, ready.Session);
        var command = new FunctionBarCommandRequest(
            ready.Session.SessionId,
            ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
            workflow.CurrentSelection!.SelectionRevision,
            FunctionBarCommand.Complete);

        var first = workflow.Execute(command);
        await renderer.Started.Task;
        var second = workflow.Execute(command);

        Assert.AreEqual(FunctionBarCommandResultKind.Accepted, first.Kind);
        Assert.AreEqual(FunctionBarCommandResultKind.Busy, second.Kind);
        renderer.Release.TrySetResult(true);
        await WaitForStateAsync(authority, WorkflowState.ResidentReady);
        Assert.AreEqual(1, provider.AcquireAllCalls);
        Assert.AreEqual(1, clipboard.Calls);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Clipboard")]
    public async Task ClipboardFailureReturnsToEditingAndDoesNotCloseFrozenSession()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        var renderer = new FakeFinalRenderer();
        var clipboard = new FakeClipboardDelivery
        {
            Failure = Failure.Create(
                FailureCode.ClipboardBusy,
                FailureCategory.Contention,
                FailureRecoverability.RetrySameIntent,
                "test-clipboard",
                request.RequestId,
                "synthetic clipboard contention")
        };
        var trace = new FakeCompleteExecutionTraceSink();
        using var workflow = CreateWorkflow(
            requests,
            provider,
            overlay,
            functionBar,
            renderer,
            clipboard,
            trace);

        var ready = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        LockSelection(overlay.InputSink!, ready.Session);

        var accepted = workflow.Execute(new FunctionBarCommandRequest(
            ready.Session.SessionId,
            ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
            workflow.CurrentSelection!.SelectionRevision,
            FunctionBarCommand.Complete));

        Assert.AreEqual(FunctionBarCommandResultKind.Accepted, accepted.Kind);
        await WaitForAsync(() => clipboard.Calls == 1);
        Assert.AreEqual(WorkflowState.Editing, authority.CurrentState);
        Assert.AreEqual(0, overlay.CloseCalls);
        Assert.IsFalse(ready.Session.IsDisposed);
        Assert.IsTrue(renderer.LastImageResult?.IsDisposed ?? false);
        Assert.IsTrue(functionBar.ShowCalls >= 2);
        Assert.AreEqual(1, functionBar.FeedbackCalls);
        Assert.AreEqual("無法複製到剪貼簿，請再試一次。", functionBar.LastFeedback);
        Assert.IsTrue(trace.Entries.Any(entry => entry.CompleteStage == CompleteExecutionStage.TransitioningToDelivering));
        var clipboardFailure = trace.Entries.Last(entry => entry.CompleteStage == CompleteExecutionStage.ClipboardFailed);
        Assert.AreEqual(FailureCode.ClipboardBusy, clipboardFailure.FailureCode);
        Assert.AreEqual(nameof(IClipboardDeliveryService), clipboardFailure.Component);
        await workflow.CancelCurrentAsync("test");
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

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public async Task SelectionAdjustmentsDoNotChangeAnnotationGeometryOrRevision()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        using var workflow = CreateWorkflow(requests, provider, overlay, functionBar);

        var ready = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        LockSelection(overlay.InputSink!, ready.Session);
        var annotationObject = new AnnotationObject(
            AnnotationObjectId.New(),
            ready.Session.SessionId,
            AnnotationToolKind.Rectangle,
            new PhysicalRect(-2, 1, 2, 4),
            7);
        var added = workflow.AddAnnotationObject(new AddAnnotationObjectRequest(
            ready.Session.SessionId,
            workflow.CurrentAnnotationDocument!.Revision,
            annotationObject));
        var baseline = (added as AnnotationMutationResult.Succeeded)!.Document;

        var input = overlay.InputSink!;
        input.PointerPressed(Input(ready.Session, 0, 1));
        input.PointerMoved(Input(ready.Session, 2, 3));
        input.PointerReleased(Input(ready.Session, 2, 3));
        input.PointerPressed(Input(ready.Session, 5, 3));
        input.PointerMoved(Input(ready.Session, 6, 4));
        input.PointerReleased(Input(ready.Session, 6, 4));
        input.PointerPressed(Input(ready.Session, 20, 10));
        input.PointerMoved(Input(ready.Session, 22, 12));
        input.PointerReleased(Input(ready.Session, 22, 12));

        var current = workflow.CurrentAnnotationDocument!;
        Assert.AreEqual(baseline.Revision, current.Revision);
        Assert.AreEqual(baseline.SessionId, current.SessionId);
        Assert.AreEqual(baseline.Objects.Single().ObjectId, current.Objects.Single().ObjectId);
        Assert.AreEqual(baseline.Objects.Single().Geometry, current.Objects.Single().Geometry);
        Assert.AreEqual(baseline.Objects.Single().ZOrder, current.Objects.Single().ZOrder);

        await workflow.CancelCurrentAsync("test");
        Assert.IsNull(workflow.CurrentAnnotationDocument);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public async Task RectangleToolRoutesPointerInputToCoreAndCommitsOneObject()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        using var workflow = CreateWorkflow(requests, provider, overlay, functionBar);

        var ready = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        LockSelection(overlay.InputSink!, ready.Session);
        var selection = workflow.CurrentSelection!;
        var selected = workflow.SelectTool(new EditingToolSelectionRequest(
            ready.Session.SessionId,
            ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
            selection.SelectionRevision,
            workflow.CurrentAnnotationDocument!.Revision,
            EditingToolKind.Rectangle));

        var start = new RectanglePointerEvent(
            ready.Session.SessionId,
            ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
            selection.SelectionRevision,
            workflow.CurrentAnnotationDocument!.Revision,
            9,
            new PhysicalPoint(-2, 1));
        var moved = start with { GlobalPhysicalPoint = new PhysicalPoint(2, 4) };
        var draftStarted = workflow.PointerPressed(start);
        var draftUpdated = workflow.PointerMoved(moved);
        var committed = workflow.PointerReleased(moved);

        Assert.AreEqual(EditingToolSelectionResultKind.Selected, selected.Kind);
        Assert.AreEqual(RectanglePointerResultKind.DraftStarted, draftStarted.Kind);
        Assert.AreEqual(RectanglePointerResultKind.DraftUpdated, draftUpdated.Kind);
        Assert.AreEqual(RectanglePointerResultKind.Committed, committed.Kind);
        Assert.AreEqual(EditingToolKind.Rectangle, workflow.ActiveTool);
        Assert.AreEqual(selection.SelectionRevision, workflow.CurrentSelection!.SelectionRevision);
        Assert.AreEqual(1, workflow.CurrentAnnotationDocument!.Objects.Count);
        Assert.AreEqual(new PhysicalRect(-2, 1, 2, 4), committed.CommittedObject!.Geometry);
        Assert.AreEqual(EditingToolKind.Rectangle, overlay.LastAnnotationSnapshot!.ActiveTool);
        Assert.AreEqual(1, overlay.LastAnnotationSnapshot.Document.Objects.Count);
        Assert.AreEqual(EditingToolKind.Rectangle, functionBar.LastRequest!.ActiveTool);

        await workflow.CancelCurrentAsync("test");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public async Task ArrowLineToolRoutesPointerInputAndPreservesLineMode()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        using var workflow = CreateWorkflow(requests, provider, overlay, functionBar);

        var ready = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        LockSelection(overlay.InputSink!, ready.Session);
        var selection = workflow.CurrentSelection!;
        var selected = workflow.SelectTool(
            new EditingToolSelectionRequest(
                ready.Session.SessionId,
                ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
                selection.SelectionRevision,
                workflow.CurrentAnnotationDocument!.Revision,
                EditingToolKind.ArrowLine)
            {
                RequestedArrowLineEndStyle = ArrowLineEndStyle.None
            });

        var start = new ArrowLinePointerEvent(
            ready.Session.SessionId,
            ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
            selection.SelectionRevision,
            workflow.CurrentAnnotationDocument!.Revision,
            11,
            new PhysicalPoint(-2, 1));
        var moved = start with { GlobalPhysicalPoint = new PhysicalPoint(2, 4) };
        var draftStarted = workflow.PointerPressed(start);
        var draftUpdated = workflow.PointerMoved(moved);
        var committed = workflow.PointerReleased(moved);
        var content = (ArrowLineAnnotationContent)committed.CommittedObject!.Content!;

        Assert.AreEqual(EditingToolSelectionResultKind.Selected, selected.Kind);
        Assert.AreEqual(ArrowLineEndStyle.None, selected.ActiveArrowLineEndStyle);
        Assert.AreEqual(ArrowLinePointerResultKind.DraftStarted, draftStarted.Kind);
        Assert.AreEqual(ArrowLinePointerResultKind.DraftUpdated, draftUpdated.Kind);
        Assert.AreEqual(ArrowLinePointerResultKind.Committed, committed.Kind);
        Assert.AreEqual(new PhysicalLineSegment(
            new PhysicalPoint(-2, 1),
            new PhysicalPoint(2, 4)), content.Segment);
        Assert.AreEqual(EditingToolKind.ArrowLine, workflow.ActiveTool);
        Assert.AreEqual(ArrowLineEndStyle.None, workflow.ActiveArrowLineEndStyle);
        Assert.AreEqual(EditingToolKind.ArrowLine, overlay.LastAnnotationSnapshot!.ActiveTool);
        Assert.AreEqual(ArrowLineEndStyle.None, overlay.LastAnnotationSnapshot.ActiveArrowLineEndStyle);
        Assert.AreEqual(EditingToolKind.ArrowLine, functionBar.LastRequest!.ActiveTool);

        await workflow.CancelCurrentAsync("test");
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Annotation")]
    public async Task HighlighterToolRoutesFreehandPointerInputAndPreservesStyle()
    {
        var authority = new WorkflowStateAuthority();
        using var requests = new CaptureRequestCoordinator(authority);
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        Assert.IsTrue(requests.Submit(request).IsAccepted);
        var provider = new FakeAllDisplayProvider();
        var overlay = new FakeOverlayCoordinator();
        var functionBar = new FakeFunctionBarPresentationCoordinator();
        using var workflow = CreateWorkflow(requests, provider, overlay, functionBar);

        var ready = (CapturePresentationOutcome.SelectingReady)
            await workflow.StartAsync(request, CancellationToken.None);
        LockSelection(overlay.InputSink!, ready.Session);
        var selection = workflow.CurrentSelection!;
        var selected = workflow.SelectTool(
            new EditingToolSelectionRequest(
                ready.Session.SessionId,
                ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
                selection.SelectionRevision,
                workflow.CurrentAnnotationDocument!.Revision,
                EditingToolKind.Highlighter));

        var start = new HighlighterPointerEvent(
            ready.Session.SessionId,
            ready.Session.VirtualDesktopSnapshot.CoordinateVersion,
            selection.SelectionRevision,
            workflow.CurrentAnnotationDocument!.Revision,
            12,
            new PhysicalPoint(-2, 1));
        var middle = start with { GlobalPhysicalPoint = new PhysicalPoint(0, 2) };
        var end = start with { GlobalPhysicalPoint = new PhysicalPoint(2, 4) };
        var draftStarted = workflow.PointerPressed(start);
        var draftUpdated = workflow.PointerMoved(middle);
        var committed = workflow.PointerReleased(end);
        var content = (HighlighterStrokeContent)committed.CommittedObject!.Content!;

        Assert.AreEqual(EditingToolSelectionResultKind.Selected, selected.Kind);
        Assert.AreEqual(HighlighterPointerResultKind.DraftStarted, draftStarted.Kind);
        Assert.AreEqual(HighlighterPointerResultKind.DraftUpdated, draftUpdated.Kind);
        Assert.AreEqual(HighlighterPointerResultKind.Committed, committed.Kind);
        CollectionAssert.AreEqual(
            new[]
            {
                new PhysicalPoint(-2, 1),
                new PhysicalPoint(0, 2),
                new PhysicalPoint(2, 4)
            },
            content.Path.Points.ToArray());
        Assert.IsTrue(content.Style.StrokeColor.A > 0);
        Assert.IsTrue(content.Style.StrokeColor.A < 255);
        Assert.AreEqual(EditingToolKind.Highlighter, workflow.ActiveTool);
        Assert.AreEqual(EditingToolKind.Highlighter, overlay.LastAnnotationSnapshot!.ActiveTool);
        Assert.AreEqual(EditingToolKind.Highlighter, functionBar.LastRequest!.ActiveTool);

        await workflow.CancelCurrentAsync("test");
    }

    private static CapturePresentationWorkflowCoordinator CreateWorkflow(
        CaptureRequestCoordinator requests,
        FakeAllDisplayProvider provider,
        FakeOverlayCoordinator overlay,
        IFunctionBarPresentationCoordinator? functionBar = null,
        IFrozenDisplayFrameSetRenderer? finalRenderer = null,
        IClipboardDeliveryService? clipboardDelivery = null,
        ICompleteExecutionTraceSink? traceSink = null)
    {
        var freezing = new CaptureFreezingCoordinator(
            requests,
            new FixedTopologyProvider(CreateSnapshot()),
            provider);
        return new CapturePresentationWorkflowCoordinator(
            freezing,
            overlay,
            new HiddenSourceExclusion(),
            functionBarPresentation: functionBar,
            finalRenderer: finalRenderer,
            clipboardDelivery: clipboardDelivery,
            traceSink: traceSink);
    }

    private static async Task WaitForStateAsync(
        WorkflowStateAuthority authority,
        WorkflowState expected)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            if (authority.CurrentState == expected)
            {
                return;
            }

            await Task.Delay(1);
        }

        Assert.AreEqual(expected, authority.CurrentState);
    }

    private static SelectionPointerEvent Input(
        CaptureSessionContext session,
        int x,
        int y) => new(
        session.SessionId,
        session.VirtualDesktopSnapshot.CoordinateVersion,
        1,
        new PhysicalPoint(x, y));

    private static SelectionInputResult LockSelection(
        ISelectionInputSink input,
        CaptureSessionContext session)
    {
        input.PointerPressed(Input(session, -3, 0));
        input.PointerMoved(Input(session, 3, 2));
        return input.PointerReleased(Input(session, 3, 2));
    }

    private static async Task WaitForAsync(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            if (condition())
            {
                return;
            }

            await Task.Yield();
        }

        Assert.IsTrue(condition());
    }

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

    private sealed class FakeFinalRenderer : IFrozenDisplayFrameSetRenderer
    {
        public int Calls { get; private set; }

        public TestImageResult? LastImageResult { get; private set; }

        public FrozenDisplayFrameSetRenderOutcome? Outcome { get; set; }

        public ValueTask<FrozenDisplayFrameSetRenderOutcome> RenderAsync(
            FrozenDisplayFrameSet frameSet,
            PhysicalRect selectionPhysicalBounds,
            CancellationToken cancellationToken)
        {
            Calls++;
            if (Outcome is not null)
            {
                return ValueTask.FromResult(Outcome);
            }

            LastImageResult = new TestImageResult(
                sessionId: frameSet.SessionId,
                pixelWidth: selectionPhysicalBounds.Width,
                pixelHeight: selectionPhysicalBounds.Height,
                sourceBounds: selectionPhysicalBounds,
                cropBounds: selectionPhysicalBounds);
            return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(
                new FrozenDisplayFrameSetRenderOutcome.Succeeded(LastImageResult));
        }
    }

    private sealed class FakeClipboardDelivery : IClipboardDeliveryService
    {
        public int Calls { get; private set; }

        public Failure? Failure { get; init; }

        public ClipboardDeliveryRequest? LastRequest { get; private set; }

        public ValueTask<ClipboardDeliveryResult> DeliverAsync(
            ClipboardDeliveryRequest request,
            CancellationToken cancellationToken)
        {
            Calls++;
            LastRequest = request;
            if (Failure is not null)
            {
                return ValueTask.FromResult<ClipboardDeliveryResult>(
                    new ClipboardDeliveryResult.RetryableFailure(
                        request.DeliveryId,
                        request.SessionId,
                        request.ResultId,
                        Failure,
                        1));
            }

            return ValueTask.FromResult<ClipboardDeliveryResult>(
                new ClipboardDeliveryResult.Delivered(
                    request.DeliveryId,
                    request.SessionId,
                    request.ResultId,
                    1));
        }
    }

    private sealed class BlockingFinalRenderer : IFrozenDisplayFrameSetRenderer
    {
        public TaskCompletionSource<bool> Started { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> Release { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async ValueTask<FrozenDisplayFrameSetRenderOutcome> RenderAsync(
            FrozenDisplayFrameSet frameSet,
            PhysicalRect selectionPhysicalBounds,
            CancellationToken cancellationToken)
        {
            Started.TrySetResult(true);
            await Release.Task.WaitAsync(cancellationToken);
            return new FrozenDisplayFrameSetRenderOutcome.Succeeded(
                new TestImageResult(
                    sessionId: frameSet.SessionId,
                    pixelWidth: selectionPhysicalBounds.Width,
                    pixelHeight: selectionPhysicalBounds.Height,
                    sourceBounds: selectionPhysicalBounds,
                    cropBounds: selectionPhysicalBounds));
        }
    }

    private sealed class FakeOverlayCoordinator : IAllDisplayOverlayPresentationCoordinator
    {
        public Failure? Failure { get; set; }

        public int PresentCalls { get; private set; }

        public int CloseCalls { get; private set; }

        public FrozenDisplayOverlayPlan? LastPlan { get; private set; }

        public ISelectionInputSink? InputSink { get; private set; }

        public AnnotationPresentationSnapshot? LastAnnotationSnapshot { get; private set; }

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

        public void ApplyAnnotation(AnnotationPresentationSnapshot snapshot)
        {
            LastAnnotationSnapshot = snapshot;
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

        public int FeedbackCalls { get; private set; }

        public string? LastFeedback { get; private set; }

        public int CloseCalls { get; private set; }

        public FunctionBarPresentationRequest? LastRequest { get; private set; }

        public FunctionBarPresentationResult Prepare(FunctionBarPresentationRequest request)
        {
            PrepareCalls++;
            LastRequest = request;
            return PreparationFailure is null
                ? Ready(request, FunctionBarPresentationResultKind.Ready)
                : Failed(request, PreparationFailure);
        }

        public FunctionBarPresentationResult Reposition(FunctionBarPresentationRequest request)
        {
            RepositionCalls++;
            LastRequest = request;
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

        public FunctionBarPresentationResult ShowFeedback(
            Guid sessionId,
            string coordinateVersion,
            int selectionRevision,
            string message)
        {
            FeedbackCalls++;
            LastFeedback = message;
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
                message);
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

    private sealed class FakeCompleteExecutionTraceSink : ICompleteExecutionTraceSink
    {
        public List<CompleteExecutionTraceEntry> Entries { get; } = new();

        public void Record(CompleteExecutionTraceEntry entry) => Entries.Add(entry);
    }

    private sealed class ThrowingCompleteExecutionTraceSink : ICompleteExecutionTraceSink
    {
        public void Record(CompleteExecutionTraceEntry entry) => throw new InvalidOperationException("synthetic trace failure");
    }
}
