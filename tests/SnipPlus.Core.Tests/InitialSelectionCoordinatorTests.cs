using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class InitialSelectionCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void CrossDisplayDragNormalizesNegativeCoordinatesAndLocksOnlyOnValidRelease()
    {
        using var session = CreateSession();
        using var selection = new InitialSelectionCoordinator(session);
        var coordinateVersion = session.VirtualDesktopSnapshot.CoordinateVersion;

        var pressed = selection.PointerPressed(Input(session, -3, 0));
        var moved = selection.PointerMoved(Input(session, 3, 1));
        var released = selection.PointerReleased(Input(session, 3, 1));

        Assert.AreEqual(SelectionInputResultKind.Dragging, pressed.Kind);
        Assert.AreEqual(SelectionInputResultKind.Dragging, moved.Kind);
        Assert.AreEqual(SelectionInputResultKind.Locked, released.Kind);
        Assert.AreEqual(SelectionStatus.Locked, selection.State.Status);
        Assert.AreEqual(new PhysicalRect(-3, 0, 3, 1), selection.State.NormalizedPhysicalBounds);
        Assert.AreEqual(coordinateVersion, selection.State.CoordinateVersion);
        Assert.IsTrue(selection.State.SelectionRevision >= 3);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void ZeroSizeAndGapOnlySelectionsNeverLock()
    {
        using var session = CreateSession();
        using var selection = new InitialSelectionCoordinator(session);

        selection.PointerPressed(Input(session, -1, 1));
        var zeroSize = selection.PointerReleased(Input(session, -1, 1));
        Assert.AreEqual(SelectionInputResultKind.InvalidSelection, zeroSize.Kind);
        Assert.AreEqual(SelectionStatus.Dragging, selection.State.Status);

        using var gapSession = CreateSession();
        using var gapSelection = new InitialSelectionCoordinator(gapSession);
        gapSelection.PointerPressed(Input(gapSession, -2, 0));
        var gapOnly = gapSelection.PointerReleased(Input(gapSession, 0, 2));

        Assert.AreEqual(SelectionInputResultKind.InvalidSelection, gapOnly.Kind);
        Assert.AreNotEqual(SelectionStatus.Locked, gapSelection.State.Status);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void DuplicateReleaseAndStaleInputDoNotCreateAnotherRevision()
    {
        using var session = CreateSession();
        using var selection = new InitialSelectionCoordinator(session);
        selection.PointerPressed(Input(session, -3, 0));
        selection.PointerMoved(Input(session, 3, 1));
        var locked = selection.PointerReleased(Input(session, 3, 1));
        var revision = locked.State.SelectionRevision;

        var duplicate = selection.PointerReleased(Input(session, 3, 1));
        var stale = selection.PointerMoved(new SelectionPointerEvent(
            Guid.NewGuid(),
            session.VirtualDesktopSnapshot.CoordinateVersion,
            1,
            new PhysicalPoint(1, 1)));

        Assert.AreEqual(SelectionInputResultKind.Ignored, duplicate.Kind);
        Assert.AreEqual(SelectionInputResultKind.StaleSession, stale.Kind);
        Assert.AreEqual(revision, selection.State.SelectionRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void IdlePointerMovementDoesNotMutateSelectionState()
    {
        using var session = CreateSession();
        using var selection = new InitialSelectionCoordinator(session);
        var initial = selection.State;

        var moved = selection.PointerMoved(Input(session, 2, 1));

        Assert.AreEqual(SelectionInputResultKind.Ignored, moved.Kind);
        Assert.AreEqual(SelectionStatus.None, moved.State.Status);
        Assert.AreEqual(initial.SelectionRevision, moved.State.SelectionRevision);
        Assert.AreEqual(initial, selection.State);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void InteriorMoveClampsToVirtualBoundsAndPreservesSize()
    {
        using var session = CreateLargeSession();
        using var selection = new InitialSelectionCoordinator(session);
        var locked = Lock(selection, session, new(10, 10), new(40, 40));

        var pressed = selection.PointerPressed(Input(session, 25, 25));
        var moved = selection.PointerMoved(Input(session, 180, 180));
        var released = selection.PointerReleased(Input(session, 180, 180));

        Assert.AreEqual(SelectionInputResultKind.AdjustmentCommitted, released.Kind);
        Assert.AreEqual(SelectionInteractionMode.Locked, released.State.InteractionMode);
        Assert.AreEqual(new PhysicalRect(165, 165, 195, 195), released.State.NormalizedPhysicalBounds);
        Assert.AreEqual(30, released.State.NormalizedPhysicalBounds!.Value.Width);
        Assert.AreEqual(30, released.State.NormalizedPhysicalBounds!.Value.Height);
        Assert.IsTrue(pressed.Kind == SelectionInputResultKind.Moving);
        Assert.AreEqual(SelectionInputResultKind.Moving, moved.Kind);
        Assert.IsTrue(released.State.SelectionRevision > locked.State.SelectionRevision);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void ResizeEdgesAndCornersCommitNormalizedGeometry()
    {
        using var session = CreateLargeSession();
        using var edgeSelection = new InitialSelectionCoordinator(session);
        Lock(edgeSelection, session, new(10, 10), new(40, 40));

        var leftPressed = edgeSelection.PointerPressed(Input(session, 10, 25));
        var leftMoved = edgeSelection.PointerMoved(Input(session, 0, 25));
        var leftReleased = edgeSelection.PointerReleased(Input(session, 0, 25));

        Assert.AreEqual(SelectionInputResultKind.Resizing, leftPressed.Kind);
        Assert.AreEqual(SelectionInteractionMode.ResizingLeft, leftMoved.State.InteractionMode);
        Assert.AreEqual(new PhysicalRect(0, 10, 40, 40), leftReleased.State.NormalizedPhysicalBounds);

        using var cornerSelection = new InitialSelectionCoordinator(session);
        Lock(cornerSelection, session, new(10, 10), new(40, 40));
        var cornerMoved = cornerSelection.PointerMoved(Input(session, 5, 5));
        Assert.AreEqual(SelectionInputResultKind.HitTested, cornerMoved.Kind);
        var cornerPressed = cornerSelection.PointerPressed(Input(session, 10, 10));
        var cornerPreview = cornerSelection.PointerMoved(Input(session, 5, 5));
        var cornerReleased = cornerSelection.PointerReleased(Input(session, 5, 5));

        Assert.AreEqual(SelectionInputResultKind.Resizing, cornerPressed.Kind);
        Assert.AreEqual(SelectionInteractionMode.ResizingTopLeft, cornerPreview.State.InteractionMode);
        Assert.AreEqual(new PhysicalRect(5, 5, 40, 40), cornerReleased.State.NormalizedPhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void ResizeCrossingOppositeEdgeFlipsEffectiveHandleWithoutNegativeSize()
    {
        using var session = CreateLargeSession();
        using var selection = new InitialSelectionCoordinator(session);
        Lock(selection, session, new(10, 10), new(40, 40));

        selection.PointerPressed(Input(session, 10, 25));
        var preview = selection.PointerMoved(Input(session, 50, 25));
        var released = selection.PointerReleased(Input(session, 50, 25));

        Assert.AreEqual(SelectionInteractionMode.ResizingRight, preview.State.InteractionMode);
        Assert.AreEqual(SelectionHitTestKind.RightEdge, preview.State.ActiveHitTest);
        Assert.AreEqual(new PhysicalRect(40, 10, 50, 40), released.State.NormalizedPhysicalBounds);
        Assert.IsTrue(released.State.NormalizedPhysicalBounds!.Value.IsPositive);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void OutsideDragCommitsReplacementAndInvalidReleaseRestoresLockedBounds()
    {
        using var session = CreateLargeSession();
        using var selection = new InitialSelectionCoordinator(session);
        Lock(selection, session, new(10, 10), new(40, 40));

        var reselection = selection.PointerPressed(Input(session, 80, 80));
        var replacement = selection.PointerMoved(Input(session, 120, 120));
        var committed = selection.PointerReleased(Input(session, 120, 120));

        Assert.AreEqual(SelectionInputResultKind.Reselecting, reselection.Kind);
        Assert.AreEqual(SelectionInputResultKind.Reselecting, replacement.Kind);
        Assert.AreEqual(SelectionInputResultKind.AdjustmentCommitted, committed.Kind);
        Assert.AreEqual(new PhysicalRect(80, 80, 120, 120), committed.State.NormalizedPhysicalBounds);

        var oldBounds = committed.State.NormalizedPhysicalBounds;
        selection.PointerPressed(Input(session, 150, 150));
        selection.PointerMoved(Input(session, 150, 150));
        var rolledBack = selection.PointerReleased(Input(session, 150, 150));

        Assert.AreEqual(SelectionInputResultKind.AdjustmentRolledBack, rolledBack.Kind);
        Assert.AreEqual(oldBounds, rolledBack.State.NormalizedPhysicalBounds);
        Assert.AreEqual(SelectionInteractionMode.Locked, rolledBack.State.InteractionMode);
        Assert.IsTrue(rolledBack.State.IsGeometryValid);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Cancellation")]
    public void AdjustmentEscCancelsSessionAndDoesNotChangeWorkflowStateAuthority()
    {
        using var session = CreateLargeSession();
        using var selection = new InitialSelectionCoordinator(session);
        Lock(selection, session, new(10, 10), new(40, 40));
        selection.PointerPressed(Input(session, 25, 25));
        selection.PointerMoved(Input(session, 50, 50));

        var cancelled = selection.Escape(
            session.SessionId,
            session.VirtualDesktopSnapshot.CoordinateVersion);

        Assert.AreEqual(SelectionInputResultKind.Cancelled, cancelled.Kind);
        Assert.AreEqual(SelectionInteractionMode.Cancelled, cancelled.State.InteractionMode);
        Assert.AreEqual(SelectionStatus.Cancelled, cancelled.State.Status);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void SecondPointerAndLateReleaseCannotCommitAdjustment()
    {
        using var session = CreateLargeSession();
        using var selection = new InitialSelectionCoordinator(session);
        var locked = Lock(selection, session, new(10, 10), new(40, 40));
        selection.PointerPressed(Input(session, 25, 25, pointerId: 1));

        var secondPointer = selection.PointerMoved(Input(session, 60, 60, pointerId: 2));
        var lateRelease = selection.PointerReleased(Input(session, 60, 60, pointerId: 2));
        var committed = selection.PointerReleased(Input(session, 60, 60, pointerId: 1));

        Assert.AreEqual(SelectionInputResultKind.Ignored, secondPointer.Kind);
        Assert.AreEqual(SelectionInputResultKind.Ignored, lateRelease.Kind);
        Assert.AreEqual(SelectionInputResultKind.AdjustmentCommitted, committed.Kind);
        Assert.IsTrue(committed.State.SelectionRevision > locked.State.SelectionRevision);
    }

    private static SelectionPointerEvent Input(
        CaptureSessionContext session,
        int x,
        int y,
        int pointerId = 1) => new(
        session.SessionId,
        session.VirtualDesktopSnapshot.CoordinateVersion,
        pointerId,
        new PhysicalPoint(x, y));

    private static SelectionInputResult Lock(
        InitialSelectionCoordinator selection,
        CaptureSessionContext session,
        PhysicalPoint start,
        PhysicalPoint end)
    {
        selection.PointerPressed(Input(session, start.X, start.Y));
        selection.PointerMoved(Input(session, end.X, end.Y));
        return selection.PointerReleased(Input(session, end.X, end.Y));
    }

    private static CaptureSessionContext CreateSession()
    {
        return CreateSession(new VirtualDesktopSnapshot(
            "selection-v1",
            new(-4, 0, 4, 2),
            new(-4, 0),
            new[]
            {
                Display("left", new(-4, 0, -2, 2)),
                Display("right", new(0, 0, 4, 2))
            }));
    }

    private static CaptureSessionContext CreateLargeSession()
    {
        return CreateSession(new VirtualDesktopSnapshot(
            "selection-large-v1",
            new(-100, -50, 200, 200),
            new(-100, -50),
            new[]
            {
                Display("left", new(-100, -50, 0, 200)),
                Display("right", new(0, -50, 200, 200))
            }));
    }

    private static CaptureSessionContext CreateSession(VirtualDesktopSnapshot snapshot)
    {
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        var session = new CaptureSessionContext(
            request,
            snapshot,
            CapacityValidationOutcome.Supported(),
            null,
            CancellationToken.None);
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
        Assert.IsTrue(session.TryAttachFrozenDisplayFrames(frameSet!));
        return session;
    }

    private static DisplaySnapshot Display(string id, PhysicalRect bounds) => new(
        id,
        bounds,
        1,
        1,
        "Landscape",
        new(bounds.Width, bounds.Height),
        $"surface:{id}");
}
