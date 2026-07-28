using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.UI.Windowing;
using SnipPlus.Contracts;

namespace SnipPlus.Windows;

public sealed record WindowsDisplayDescriptor
{
    public WindowsDisplayDescriptor(
        ulong displayId,
        PhysicalRect physicalBoundsInVirtualDesktop,
        double dpiScaleX,
        double dpiScaleY,
        string rotationOrOrientation,
        string logicalSurfaceIdentity)
    {
        DisplayId = displayId;
        PhysicalBoundsInVirtualDesktop = physicalBoundsInVirtualDesktop;
        DpiScaleX = dpiScaleX;
        DpiScaleY = dpiScaleY;
        RotationOrOrientation = rotationOrOrientation ?? throw new ArgumentNullException(nameof(rotationOrOrientation));
        LogicalSurfaceIdentity = logicalSurfaceIdentity ?? throw new ArgumentNullException(nameof(logicalSurfaceIdentity));
    }

    public ulong DisplayId { get; }

    public PhysicalRect PhysicalBoundsInVirtualDesktop { get; }

    public double DpiScaleX { get; }

    public double DpiScaleY { get; }

    public string RotationOrOrientation { get; }

    public string LogicalSurfaceIdentity { get; }
}

public abstract record WindowsDisplayTopologyMappingOutcome
{
    private WindowsDisplayTopologyMappingOutcome()
    {
    }

    public sealed record Succeeded(VirtualDesktopSnapshot Snapshot) : WindowsDisplayTopologyMappingOutcome;

    public sealed record Invalid(string Message) : WindowsDisplayTopologyMappingOutcome;
}

public static class WindowsDisplayTopologyMapper
{
    public static WindowsDisplayTopologyMappingOutcome Map(
        IEnumerable<WindowsDisplayDescriptor> descriptors)
    {
        ArgumentNullException.ThrowIfNull(descriptors);
        var input = descriptors.ToArray();
        if (input.Length == 0)
        {
            return new WindowsDisplayTopologyMappingOutcome.Invalid(
                "The Windows display topology contains no active display surface.");
        }

        if (input.Any(descriptor => descriptor is null))
        {
            return new WindowsDisplayTopologyMappingOutcome.Invalid(
                "The Windows display topology contains an empty display descriptor.");
        }

        if (input.Select(descriptor => descriptor.DisplayId).Distinct().Count() != input.Length)
        {
            return new WindowsDisplayTopologyMappingOutcome.Invalid(
                "The Windows display topology contains duplicate display identities.");
        }

        var logicalDisplays = new List<WindowsDisplayDescriptor>(input.Length);
        foreach (var group in input.GroupBy(
                     descriptor => descriptor.LogicalSurfaceIdentity,
                     StringComparer.Ordinal))
        {
            var first = group.First();
            if (string.IsNullOrWhiteSpace(first.LogicalSurfaceIdentity)
                || group.Any(descriptor => !IsValidDescriptor(descriptor)))
            {
                return new WindowsDisplayTopologyMappingOutcome.Invalid(
                    "The Windows display topology contains invalid display metadata.");
            }

            if (group.Skip(1).Any(descriptor =>
                    descriptor.PhysicalBoundsInVirtualDesktop != first.PhysicalBoundsInVirtualDesktop
                    || descriptor.DpiScaleX != first.DpiScaleX
                    || descriptor.DpiScaleY != first.DpiScaleY
                    || !string.Equals(
                        descriptor.RotationOrOrientation,
                        first.RotationOrOrientation,
                        StringComparison.Ordinal)))
            {
                return new WindowsDisplayTopologyMappingOutcome.Invalid(
                    "Mirrored logical display surfaces have inconsistent topology metadata.");
            }

            logicalDisplays.Add(first);
        }

        logicalDisplays.Sort((left, right) => left.DisplayId.CompareTo(right.DisplayId));

        try
        {
            var left = logicalDisplays.Min(descriptor => descriptor.PhysicalBoundsInVirtualDesktop.Left);
            var top = logicalDisplays.Min(descriptor => descriptor.PhysicalBoundsInVirtualDesktop.Top);
            var right = logicalDisplays.Max(descriptor => descriptor.PhysicalBoundsInVirtualDesktop.Right);
            var bottom = logicalDisplays.Max(descriptor => descriptor.PhysicalBoundsInVirtualDesktop.Bottom);
            var virtualBounds = new PhysicalRect(left, top, right, bottom);
            if (!virtualBounds.IsPositive)
            {
                return new WindowsDisplayTopologyMappingOutcome.Invalid(
                    "The Windows Virtual Desktop bounds are not positive.");
            }

            var coordinateVersion = CreateCoordinateVersion(logicalDisplays, virtualBounds);
            var snapshots = logicalDisplays.Select(descriptor => new DisplaySnapshot(
                $"display:{descriptor.DisplayId.ToString(CultureInfo.InvariantCulture)}",
                descriptor.PhysicalBoundsInVirtualDesktop,
                descriptor.DpiScaleX,
                descriptor.DpiScaleY,
                descriptor.RotationOrOrientation,
                new PhysicalPixelSize(
                    checked((int)descriptor.PhysicalBoundsInVirtualDesktop.Width64),
                    checked((int)descriptor.PhysicalBoundsInVirtualDesktop.Height64)),
                descriptor.LogicalSurfaceIdentity));

            return new WindowsDisplayTopologyMappingOutcome.Succeeded(new VirtualDesktopSnapshot(
                coordinateVersion,
                virtualBounds,
                new PhysicalPoint(left, top),
                snapshots));
        }
        catch (OverflowException)
        {
            return new WindowsDisplayTopologyMappingOutcome.Invalid(
                "The Windows display topology exceeds the supported coordinate range.");
        }
        catch (ArgumentException exception)
        {
            return new WindowsDisplayTopologyMappingOutcome.Invalid(exception.Message);
        }
    }

    private static bool IsValidDescriptor(WindowsDisplayDescriptor descriptor) =>
        descriptor.PhysicalBoundsInVirtualDesktop.IsPositive
        && double.IsFinite(descriptor.DpiScaleX)
        && double.IsFinite(descriptor.DpiScaleY)
        && descriptor.DpiScaleX > 0
        && descriptor.DpiScaleY > 0
        && !string.IsNullOrWhiteSpace(descriptor.RotationOrOrientation)
        && !string.IsNullOrWhiteSpace(descriptor.LogicalSurfaceIdentity);

    private static string CreateCoordinateVersion(
        IReadOnlyList<WindowsDisplayDescriptor> displays,
        PhysicalRect virtualBounds)
    {
        var material = new StringBuilder()
            .Append(virtualBounds.Left.ToString(CultureInfo.InvariantCulture)).Append('|')
            .Append(virtualBounds.Top.ToString(CultureInfo.InvariantCulture)).Append('|')
            .Append(virtualBounds.Right.ToString(CultureInfo.InvariantCulture)).Append('|')
            .Append(virtualBounds.Bottom.ToString(CultureInfo.InvariantCulture));

        foreach (var display in displays)
        {
            material.Append('|')
                .Append(display.DisplayId.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(display.PhysicalBoundsInVirtualDesktop.Left.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(display.PhysicalBoundsInVirtualDesktop.Top.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(display.PhysicalBoundsInVirtualDesktop.Right.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(display.PhysicalBoundsInVirtualDesktop.Bottom.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(display.DpiScaleX.ToString("R", CultureInfo.InvariantCulture)).Append('|')
                .Append(display.DpiScaleY.ToString("R", CultureInfo.InvariantCulture)).Append('|')
                .Append(display.RotationOrOrientation).Append('|')
                .Append(display.LogicalSurfaceIdentity);
        }

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(material.ToString())));
        return $"windows-topology:{hash}";
    }
}

public interface IWindowsDisplayTopologySource
{
    ValueTask<IReadOnlyList<WindowsDisplayDescriptor>> GetDisplaysAsync(
        CancellationToken cancellationToken);
}

public interface IWindowsDisplayTopologyRevisionSource
{
    ValueTask<string?> GetCurrentCoordinateVersionAsync(CancellationToken cancellationToken);
}

public sealed class WindowsDisplayTopologyProvider :
    IDisplayTopologyProvider,
    IWindowsDisplayTopologyRevisionSource
{
    private readonly IWindowsDisplayTopologySource _source;

    public WindowsDisplayTopologyProvider(IWindowsDisplayTopologySource? source = null)
    {
        _source = source ?? new WindowsDisplayTopologySource();
    }

    public async ValueTask<DisplayTopologyOutcome> GetSnapshotAsync(
        CaptureRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            var descriptors = await _source
                .GetDisplaysAsync(cancellationToken)
                .ConfigureAwait(false);
            var mapped = WindowsDisplayTopologyMapper.Map(descriptors);
            return mapped switch
            {
                WindowsDisplayTopologyMappingOutcome.Succeeded succeeded =>
                    new DisplayTopologyOutcome.Succeeded(succeeded.Snapshot),
                WindowsDisplayTopologyMappingOutcome.Invalid invalid =>
                    new DisplayTopologyOutcome.Invalid(CreateFailure(
                        FailureCode.DisplayTopologyInvalid,
                        FailureCategory.Validation,
                        request.RequestId,
                        invalid.Message)),
                _ => new DisplayTopologyOutcome.Invalid(CreateFailure(
                    FailureCode.DisplayTopologyInvalid,
                    FailureCategory.Validation,
                    request.RequestId,
                    "The Windows display topology mapper returned an unknown outcome."))
            };
        }
        catch (OperationCanceledException)
        {
            return new DisplayTopologyOutcome.Cancelled("CancellationToken");
        }
        catch (Exception exception)
        {
            return new DisplayTopologyOutcome.Invalid(CreateFailure(
                FailureCode.DisplayTopologyUnavailable,
                FailureCategory.Device,
                request.RequestId,
                exception.GetType().Name,
                exception.HResult));
        }
    }

    public async ValueTask<string?> GetCurrentCoordinateVersionAsync(
        CancellationToken cancellationToken)
    {
        var descriptors = await _source
            .GetDisplaysAsync(cancellationToken)
            .ConfigureAwait(false);
        return WindowsDisplayTopologyMapper.Map(descriptors) is
            WindowsDisplayTopologyMappingOutcome.Succeeded succeeded
            ? succeeded.Snapshot.CoordinateVersion
            : null;
    }

    private static Failure CreateFailure(
        FailureCode code,
        FailureCategory category,
        Guid requestId,
        string diagnosticMessage,
        int? nativeCode = null) => Failure.Create(
            code,
            category,
            FailureRecoverability.RetryNewIntent,
            "WindowsDisplayTopologyProvider",
            requestId,
            diagnosticMessage,
            nativeCode: nativeCode);
}

public sealed class WindowsDisplayTopologySource : IWindowsDisplayTopologySource
{
    public ValueTask<IReadOnlyList<WindowsDisplayDescriptor>> GetDisplaysAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var previousDpiAwarenessContext = SetThreadDpiAwarenessContext(PerMonitorAwareV2Context);
        if (previousDpiAwarenessContext == 0)
        {
            throw new InvalidOperationException("Windows could not enter per-monitor DPI-aware topology context.");
        }

        try
        {
            var displays = DisplayArea.FindAll();
            var descriptors = new List<WindowsDisplayDescriptor>(displays.Count);
            for (var index = 0; index < displays.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var display = displays[index];
                var outerBounds = display.OuterBounds;
                var physicalBounds = new PhysicalRect(
                    outerBounds.X,
                    outerBounds.Y,
                    checked(outerBounds.X + outerBounds.Width),
                    checked(outerBounds.Y + outerBounds.Height));
                var monitor = MonitorFromPoint(new NativePoint(
                    checked(outerBounds.X + Math.Max(0, outerBounds.Width / 2)),
                    checked(outerBounds.Y + Math.Max(0, outerBounds.Height / 2))),
                    MonitorDefaultNearest);
                var (dpiScaleX, dpiScaleY) = GetDpiScale(monitor);
                var orientation = GetOrientation(monitor);
                var displayId = display.DisplayId.Value;
                descriptors.Add(new WindowsDisplayDescriptor(
                    displayId,
                    physicalBounds,
                    dpiScaleX,
                    dpiScaleY,
                    orientation,
                    $"display:{displayId.ToString(CultureInfo.InvariantCulture)}"));
            }

            return ValueTask.FromResult<IReadOnlyList<WindowsDisplayDescriptor>>(descriptors);
        }
        finally
        {
            _ = SetThreadDpiAwarenessContext(previousDpiAwarenessContext);
        }
    }

    private static (double DpiScaleX, double DpiScaleY) GetDpiScale(nint monitor)
    {
        if (monitor == 0
            || GetDpiForMonitor(monitor, MonitorDpiType.Effective, out var dpiX, out var dpiY) != 0
            || dpiX == 0
            || dpiY == 0)
        {
            throw new InvalidOperationException("Windows did not provide monitor DPI metadata.");
        }

        return (dpiX / 96d, dpiY / 96d);
    }

    private static string GetOrientation(nint monitor)
    {
        if (monitor == 0)
        {
            throw new InvalidOperationException("Windows did not provide a monitor handle.");
        }

        var monitorInfo = new MonitorInfoEx { Size = Marshal.SizeOf<MonitorInfoEx>() };
        if (!GetMonitorInfo(monitor, ref monitorInfo)
            || string.IsNullOrWhiteSpace(monitorInfo.DeviceName))
        {
            throw new InvalidOperationException("Windows did not provide monitor identity metadata.");
        }

        var mode = new DevMode { Size = (ushort)Marshal.SizeOf<DevMode>() };
        if (!EnumDisplaySettings(monitorInfo.DeviceName, CurrentSettings, ref mode))
        {
            throw new InvalidOperationException("Windows did not provide monitor orientation metadata.");
        }

        return mode.DisplayOrientation switch
        {
            0 => "Landscape",
            1 => "Portrait",
            2 => "LandscapeFlipped",
            3 => "PortraitFlipped",
            _ => $"Orientation:{mode.DisplayOrientation.ToString(CultureInfo.InvariantCulture)}"
        };
    }

    private const uint MonitorDefaultNearest = 2;
    private const uint CurrentSettings = uint.MaxValue;
    private static readonly nint PerMonitorAwareV2Context = new(-4);

    private enum MonitorDpiType
    {
        Effective = 0
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;

        public NativePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MonitorInfoEx
    {
        public int Size;
        public NativeRect Monitor;
        public NativeRect Work;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct DevMode
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        public ushort SpecVersion;
        public ushort DriverVersion;
        public ushort Size;
        public ushort DriverExtra;
        public uint Fields;
        public int PositionX;
        public int PositionY;
        public uint DisplayOrientation;
        public uint DisplayFixedOutput;
        public short Color;
        public short Duplex;
        public short YResolution;
        public short TTOption;
        public short Collate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FormName;
        public ushort LogPixels;
        public uint BitsPerPel;
        public uint PelsWidth;
        public uint PelsHeight;
        public uint DisplayFlags;
        public uint DisplayFrequency;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern nint MonitorFromPoint(NativePoint point, uint flags);

    [DllImport("user32.dll")]
    private static extern nint SetThreadDpiAwarenessContext(nint dpiContext);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetMonitorInfo(nint monitor, ref MonitorInfoEx monitorInfo);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool EnumDisplaySettings(
        string deviceName,
        uint modeNum,
        ref DevMode devMode);

    [DllImport("Shcore.dll")]
    private static extern int GetDpiForMonitor(
        nint monitor,
        MonitorDpiType dpiType,
        out uint dpiX,
        out uint dpiY);
}
