using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsDisplayTopologyTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void MapsNegativeMixedDpiTopologyToPhysicalVirtualDesktopSnapshot()
    {
        var result = WindowsDisplayTopologyMapper.Map(new[]
        {
            Descriptor(20, new(0, 0, 2560, 1440), 1, 1, "Landscape", "primary"),
            Descriptor(10, new(-2560, 0, 0, 1440), 1, 1, "Landscape", "left"),
            Descriptor(30, new(0, 1440, 1920, 2520), 1.5, 1.5, "Portrait", "lower")
        });

        var succeeded = result as WindowsDisplayTopologyMappingOutcome.Succeeded;

        Assert.IsNotNull(succeeded);
        Assert.AreEqual(new PhysicalRect(-2560, 0, 2560, 2520), succeeded.Snapshot.VirtualPhysicalBounds);
        Assert.AreEqual(new PhysicalPoint(-2560, 0), succeeded.Snapshot.VirtualOrigin);
        Assert.AreEqual(1920, succeeded.Snapshot.Displays.Single(display => display.DisplayId == "display:30").ExpectedFrozenFramePixelSize.Width);
        Assert.AreEqual(1.5, succeeded.Snapshot.Displays.Single(display => display.DisplayId == "display:30").DpiScaleX);
        Assert.AreEqual(3, succeeded.Snapshot.Displays.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void DeduplicatesMirroredLogicalSurfaceWithoutInventingGapDisplay()
    {
        var result = WindowsDisplayTopologyMapper.Map(new[]
        {
            Descriptor(1, new(0, 0, 1920, 1080), 1, 1, "Landscape", "mirrored-surface"),
            Descriptor(2, new(0, 0, 1920, 1080), 1, 1, "Landscape", "mirrored-surface"),
            Descriptor(3, new(2560, 0, 4480, 1080), 1, 1, "Landscape", "second-surface")
        });

        var succeeded = result as WindowsDisplayTopologyMappingOutcome.Succeeded;

        Assert.IsNotNull(succeeded);
        Assert.AreEqual(2, succeeded.Snapshot.Displays.Count);
        Assert.IsFalse(succeeded.Snapshot.Displays.Any(
            display => display.PhysicalBoundsInVirtualDesktop == new PhysicalRect(1920, 0, 2560, 1080)));
        Assert.AreEqual(new PhysicalRect(0, 0, 4480, 1080), succeeded.Snapshot.VirtualPhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void SameTopologyProducesDeterministicCoordinateVersion()
    {
        var first = WindowsDisplayTopologyMapper.Map(new[]
        {
            Descriptor(2, new(100, 0, 200, 100), 1.25, 1.25, "Landscape", "b"),
            Descriptor(1, new(-100, 0, 0, 100), 1, 1, "Landscape", "a")
        });
        var second = WindowsDisplayTopologyMapper.Map(new[]
        {
            Descriptor(1, new(-100, 0, 0, 100), 1, 1, "Landscape", "a"),
            Descriptor(2, new(100, 0, 200, 100), 1.25, 1.25, "Landscape", "b")
        });

        Assert.AreEqual(
            ((WindowsDisplayTopologyMappingOutcome.Succeeded)first).Snapshot.CoordinateVersion,
            ((WindowsDisplayTopologyMappingOutcome.Succeeded)second).Snapshot.CoordinateVersion);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void RejectsInvalidDpiBoundsAndConflictingMirroredMetadata()
    {
        var invalidDpi = WindowsDisplayTopologyMapper.Map(new[]
        {
            Descriptor(1, new(0, 0, 100, 100), double.NaN, 1, "Landscape", "one")
        });
        var invalidBounds = WindowsDisplayTopologyMapper.Map(new[]
        {
            Descriptor(1, new(0, 0, 0, 100), 1, 1, "Landscape", "one")
        });
        var conflictingMirror = WindowsDisplayTopologyMapper.Map(new[]
        {
            Descriptor(1, new(0, 0, 100, 100), 1, 1, "Landscape", "one"),
            Descriptor(2, new(0, 0, 200, 100), 1, 1, "Landscape", "one")
        });

        Assert.IsInstanceOfType<WindowsDisplayTopologyMappingOutcome.Invalid>(invalidDpi);
        Assert.IsInstanceOfType<WindowsDisplayTopologyMappingOutcome.Invalid>(invalidBounds);
        Assert.IsInstanceOfType<WindowsDisplayTopologyMappingOutcome.Invalid>(conflictingMirror);
    }

    private static WindowsDisplayDescriptor Descriptor(
        ulong id,
        PhysicalRect bounds,
        double dpiX,
        double dpiY,
        string orientation,
        string logicalSurface) => new(
        id,
        bounds,
        dpiX,
        dpiY,
        orientation,
        logicalSurface);
}
