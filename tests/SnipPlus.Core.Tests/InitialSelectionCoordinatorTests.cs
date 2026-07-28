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

    private static SelectionPointerEvent Input(
        CaptureSessionContext session,
        int x,
        int y) => new(
        session.SessionId,
        session.VirtualDesktopSnapshot.CoordinateVersion,
        1,
        new PhysicalPoint(x, y));

    private static CaptureSessionContext CreateSession()
    {
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        var snapshot = new VirtualDesktopSnapshot(
            "selection-v1",
            new(-4, 0, 4, 2),
            new(-4, 0),
            new[]
            {
                Display("left", new(-4, 0, -2, 2)),
                Display("right", new(0, 0, 4, 2))
            });
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
