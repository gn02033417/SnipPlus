using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Core.Tests;

[TestClass]
public sealed class FrozenDisplayOverlayPlanBuilderTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void ThreeDisplayPlanCreatesExactlyOneDescriptorPerDisplayAndPreservesNegativeBounds()
    {
        using var session = CreateSession(CreateThreeDisplaySnapshot());

        var success = FrozenDisplayOverlayPlanBuilder.TryCreate(
            session,
            out var plan,
            out var failure);

        Assert.IsTrue(success);
        Assert.IsNull(failure);
        Assert.IsNotNull(plan);
        Assert.AreEqual(3, plan.Displays.Count);
        Assert.AreEqual(
            new PhysicalRect(-4, 0, -2, 2),
            plan.Displays.Single(display => display.DisplayId == "left").PhysicalBoundsInVirtualDesktop);
        Assert.AreEqual(
            session.VirtualDesktopSnapshot.CoordinateVersion,
            plan.CoordinateVersion);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void FourDisplayPlanDoesNotCreateACombinedVirtualDesktopBitmap()
    {
        using var session = CreateSession(CreateFourDisplaySnapshot());

        Assert.IsTrue(FrozenDisplayOverlayPlanBuilder.TryCreate(
            session,
            out var plan,
            out var failure));

        Assert.IsNull(failure);
        Assert.AreEqual(4, plan!.Displays.Count);
        Assert.AreEqual(4, plan.Displays.Select(display => display.Frame).Distinct().Count());
        Assert.AreEqual(4, plan.Displays.Select(display => display.DisplayId).Distinct().Count());
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void SessionWithoutCompleteFrameSetCannotCreateAnOverlayPlan()
    {
        var session = new CaptureSessionContext(
            CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch),
            CreateThreeDisplaySnapshot(),
            CapacityValidationOutcome.Supported(),
            null,
            CancellationToken.None);
        using (session)
        {
            Assert.IsFalse(FrozenDisplayOverlayPlanBuilder.TryCreate(
                session,
                out var plan,
                out var failure));

            Assert.IsNull(plan);
            Assert.IsNotNull(failure);
        }
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void OverlayPlanRejectsAFrameSetWithAStaleCoordinateVersion()
    {
        using var session = CreateSession(CreateThreeDisplaySnapshot());
        var frameSet = session.FrozenDisplayFrames!;
        Assert.IsTrue(frameSet.IsComplete);

        var staleSession = new CaptureSessionContext(
            CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch),
            new VirtualDesktopSnapshot(
                "stale-v2",
                session.VirtualDesktopSnapshot.VirtualPhysicalBounds,
                session.VirtualDesktopSnapshot.VirtualOrigin,
                session.VirtualDesktopSnapshot.Displays),
            CapacityValidationOutcome.Supported(),
            null,
            CancellationToken.None);
        using (staleSession)
        {
            Assert.IsFalse(FrozenDisplayOverlayPlanBuilder.TryCreate(
                staleSession,
                out var plan,
                out var failure));
            Assert.IsNull(plan);
            Assert.IsNotNull(failure);
        }
    }

    private static CaptureSessionContext CreateSession(VirtualDesktopSnapshot snapshot)
    {
        var session = new CaptureSessionContext(
            CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch),
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

    private static VirtualDesktopSnapshot CreateThreeDisplaySnapshot() => new(
        "three-display-v1",
        new(-4, 0, 4, 4),
        new(-4, 0),
        new[]
        {
            Display("left", new(-4, 0, -2, 2)),
            Display("primary", new(0, 0, 4, 4)),
            Display("lower", new(-1, 2, 2, 4))
        });

    private static VirtualDesktopSnapshot CreateFourDisplaySnapshot() => new(
        "four-display-v1",
        new(-4, 0, 4, 4),
        new(-4, 0),
        new[]
        {
            Display("left", new(-4, 0, -2, 2)),
            Display("primary", new(0, 0, 2, 2)),
            Display("right", new(2, 0, 4, 2)),
            Display("lower", new(0, 2, 4, 4))
        });

    private static DisplaySnapshot Display(string id, PhysicalRect bounds) => new(
        id,
        bounds,
        id == "lower" ? 1.5 : 1,
        id == "lower" ? 1.5 : 1,
        "Landscape",
        new(bounds.Width, bounds.Height),
        $"surface:{id}");
}
