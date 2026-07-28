using SnipPlus.Contracts;

namespace SnipPlus.Core;

public static class FrozenDisplayOverlayPlanBuilder
{
    public static bool TryCreate(
        CaptureSessionContext session,
        out FrozenDisplayOverlayPlan? plan,
        out Failure? failure)
    {
        ArgumentNullException.ThrowIfNull(session);
        plan = null;
        failure = null;

        var frameSet = session.FrozenDisplayFrames;
        if (session.IsDisposed
            || session.Status != CaptureSessionStatus.FrozenFrameSetReady
            || frameSet is null
            || frameSet.IsDisposed
            || !frameSet.IsComplete
            || frameSet.Frames.Count != session.VirtualDesktopSnapshot.Displays.Count
            || !string.Equals(
                frameSet.CoordinateVersion,
                session.VirtualDesktopSnapshot.CoordinateVersion,
                StringComparison.Ordinal))
        {
            failure = CreateFailure(
                session,
                "A complete, current frozen display frame set is required before presentation.");
            return false;
        }

        var descriptors = new List<FrozenDisplayOverlayDescriptor>(
            session.VirtualDesktopSnapshot.Displays.Count);
        foreach (var display in session.VirtualDesktopSnapshot.Displays)
        {
            if (!frameSet.Frames.TryGetValue(display.DisplayId, out var frame)
                || frame.IsDisposed
                || frame.SessionId != session.SessionId
                || !string.Equals(
                    frame.CoordinateVersion,
                    session.VirtualDesktopSnapshot.CoordinateVersion,
                    StringComparison.Ordinal)
                || frame.PhysicalBoundsInVirtualDesktop != display.PhysicalBoundsInVirtualDesktop
                || frame.PixelSize != display.ExpectedFrozenFramePixelSize)
            {
                failure = CreateFailure(
                    session,
                    $"Frozen frame metadata does not match display '{display.DisplayId}'.");
                return false;
            }

            descriptors.Add(new FrozenDisplayOverlayDescriptor(
                session.SessionId,
                session.VirtualDesktopSnapshot.CoordinateVersion,
                display.DisplayId,
                display.PhysicalBoundsInVirtualDesktop,
                display.ExpectedFrozenFramePixelSize,
                frame));
        }

        plan = new FrozenDisplayOverlayPlan(
            session.SessionId,
            session.VirtualDesktopSnapshot.CoordinateVersion,
            descriptors);
        return true;
    }

    private static Failure CreateFailure(CaptureSessionContext session, string message) => Failure.Create(
        FailureCode.InvalidCaptureIntent,
        FailureCategory.Validation,
        FailureRecoverability.RetryNewIntent,
        "FrozenDisplayOverlayPlanBuilder",
        session.RequestId,
        message);
}
