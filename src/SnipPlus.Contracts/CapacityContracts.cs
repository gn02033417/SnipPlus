using System.Collections.ObjectModel;

namespace SnipPlus.Contracts;

public readonly record struct PhysicalPoint(int X, int Y);

public readonly record struct PhysicalPixelSize(int Width, int Height)
{
    public bool IsPositive => Width > 0 && Height > 0;
}

public enum GapPolicy
{
    Transparent
}

public enum CapacityValidationKind
{
    Supported,
    UnsupportedDisplayCount,
    UnsupportedDisplayDimensions,
    UnsupportedTotalSourcePixels,
    UnsupportedVirtualDesktopBounds,
    UnsupportedSelectionDimensions,
    UnsupportedSelectionArea,
    InvalidTopology,
    Overflow
}

public sealed record CapacityValidationOutcome
{
    private CapacityValidationOutcome(
        CapacityValidationKind kind,
        string userMessageKey,
        string userMessage,
        long? actualValue,
        long? limitValue,
        long? actualWidth,
        long? actualHeight)
    {
        Kind = kind;
        UserMessageKey = userMessageKey;
        UserMessage = userMessage;
        ActualValue = actualValue;
        LimitValue = limitValue;
        ActualWidth = actualWidth;
        ActualHeight = actualHeight;
    }

    public CapacityValidationKind Kind { get; }

    public bool IsSupported => Kind == CapacityValidationKind.Supported;

    public string UserMessageKey { get; }

    public string UserMessage { get; }

    public long? ActualValue { get; }

    public long? LimitValue { get; }

    public long? ActualWidth { get; }

    public long? ActualHeight { get; }

    public static CapacityValidationOutcome Supported() => new(
        CapacityValidationKind.Supported,
        "Capacity.Supported",
        "The display topology is within the supported capacity.",
        null,
        null,
        null,
        null);

    public static CapacityValidationOutcome Failure(
        CapacityValidationKind kind,
        string userMessage,
        long? actualValue = null,
        long? limitValue = null,
        long? actualWidth = null,
        long? actualHeight = null) => new(
        kind,
        $"Capacity.{kind}",
        userMessage,
        actualValue,
        limitValue,
        actualWidth,
        actualHeight);
}

public sealed record SupportedCapacityPolicy
{
    public const int MinDisplayCount = 1;
    public const int MaxDisplayCount = 4;
    public const int MaxDisplayWidth = 3840;
    public const int MaxDisplayHeight = 2160;
    public const long MaxTotalSourcePixels = 33_177_600;
    public const int MaxVirtualDesktopWidth = 16_384;
    public const int MaxVirtualDesktopHeight = 16_384;
    public const int MaxSelectionWidth = 16_384;
    public const int MaxSelectionHeight = 16_384;
    public const long MaxSelectionArea = 67_108_864;

    public int MinimumDisplayCount { get; init; } = MinDisplayCount;

    public int MaximumDisplayCount { get; init; } = MaxDisplayCount;

    public int MaximumDisplayWidth { get; init; } = MaxDisplayWidth;

    public int MaximumDisplayHeight { get; init; } = MaxDisplayHeight;

    public long MaximumTotalSourcePixels { get; init; } = MaxTotalSourcePixels;

    public int MaximumVirtualDesktopWidth { get; init; } = MaxVirtualDesktopWidth;

    public int MaximumVirtualDesktopHeight { get; init; } = MaxVirtualDesktopHeight;

    public int MaximumSelectionWidth { get; init; } = MaxSelectionWidth;

    public int MaximumSelectionHeight { get; init; } = MaxSelectionHeight;

    public long MaximumSelectionArea { get; init; } = MaxSelectionArea;

    public CapacityValidationOutcome ValidateTopology(VirtualDesktopSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if (snapshot.Displays.Count < MinimumDisplayCount
            || snapshot.Displays.Count > MaximumDisplayCount)
        {
            return CapacityValidationOutcome.Failure(
                CapacityValidationKind.UnsupportedDisplayCount,
                $"The active display count ({snapshot.Displays.Count}) is outside the supported range of {MinimumDisplayCount} to {MaximumDisplayCount}.",
                snapshot.Displays.Count,
                MaximumDisplayCount);
        }

        long virtualWidth;
        long virtualHeight;
        try
        {
            virtualWidth = checked((long)snapshot.VirtualPhysicalBounds.Right - snapshot.VirtualPhysicalBounds.Left);
            virtualHeight = checked((long)snapshot.VirtualPhysicalBounds.Bottom - snapshot.VirtualPhysicalBounds.Top);
        }
        catch (OverflowException)
        {
            return CapacityValidationOutcome.Failure(
                CapacityValidationKind.Overflow,
                "The Virtual Desktop bounds exceed the supported numeric range.");
        }

        if (virtualWidth <= 0 || virtualHeight <= 0)
        {
            return CapacityValidationOutcome.Failure(
                CapacityValidationKind.InvalidTopology,
                "The Virtual Desktop bounds must have positive dimensions.",
                actualWidth: virtualWidth,
                actualHeight: virtualHeight);
        }

        if (virtualWidth > MaximumVirtualDesktopWidth || virtualHeight > MaximumVirtualDesktopHeight)
        {
            return CapacityValidationOutcome.Failure(
                CapacityValidationKind.UnsupportedVirtualDesktopBounds,
                $"The Virtual Desktop bounds ({virtualWidth} x {virtualHeight}) exceed the supported limit of {MaximumVirtualDesktopWidth} x {MaximumVirtualDesktopHeight}.",
                actualValue: Math.Max(virtualWidth, virtualHeight),
                limitValue: Math.Max(MaximumVirtualDesktopWidth, MaximumVirtualDesktopHeight),
                actualWidth: virtualWidth,
                actualHeight: virtualHeight);
        }

        long totalSourcePixels = 0;
        foreach (var display in snapshot.Displays)
        {
            if (!display.IsValid)
            {
                return CapacityValidationOutcome.Failure(
                    CapacityValidationKind.InvalidTopology,
                    $"Display '{display.DisplayId}' has invalid geometry or display metadata.");
            }

            var width = (long)display.PhysicalBoundsInVirtualDesktop.Right
                - display.PhysicalBoundsInVirtualDesktop.Left;
            var height = (long)display.PhysicalBoundsInVirtualDesktop.Bottom
                - display.PhysicalBoundsInVirtualDesktop.Top;
            if (width > MaximumDisplayWidth || height > MaximumDisplayHeight)
            {
                return CapacityValidationOutcome.Failure(
                    CapacityValidationKind.UnsupportedDisplayDimensions,
                    $"A display is {width} x {height}; the supported maximum is {MaximumDisplayWidth} x {MaximumDisplayHeight}.",
                    actualValue: Math.Max(width, height),
                    limitValue: Math.Max(MaximumDisplayWidth, MaximumDisplayHeight),
                    actualWidth: width,
                    actualHeight: height);
            }

            try
            {
                totalSourcePixels = checked(totalSourcePixels + checked(width * height));
            }
            catch (OverflowException)
            {
                return CapacityValidationOutcome.Failure(
                    CapacityValidationKind.Overflow,
                    "The total display source pixel count exceeds the supported numeric range.");
            }
        }

        if (totalSourcePixels > MaximumTotalSourcePixels)
        {
            return CapacityValidationOutcome.Failure(
                CapacityValidationKind.UnsupportedTotalSourcePixels,
                $"The total display source pixel count ({totalSourcePixels}) exceeds the supported limit of {MaximumTotalSourcePixels}.",
                totalSourcePixels,
                MaximumTotalSourcePixels);
        }

        return CapacityValidationOutcome.Supported();
    }

    public CapacityValidationOutcome ValidateSelection(PhysicalRect selectionBounds)
    {
        var width = (long)selectionBounds.Right - selectionBounds.Left;
        var height = (long)selectionBounds.Bottom - selectionBounds.Top;
        if (width <= 0 || height <= 0)
        {
            return CapacityValidationOutcome.Failure(
                CapacityValidationKind.UnsupportedSelectionDimensions,
                "The Selection bounds must have positive dimensions.",
                actualWidth: width,
                actualHeight: height);
        }

        if (width > MaximumSelectionWidth || height > MaximumSelectionHeight)
        {
            return CapacityValidationOutcome.Failure(
                CapacityValidationKind.UnsupportedSelectionDimensions,
                $"The Selection bounds ({width} x {height}) exceed the supported limit of {MaximumSelectionWidth} x {MaximumSelectionHeight}.",
                actualValue: Math.Max(width, height),
                limitValue: Math.Max(MaximumSelectionWidth, MaximumSelectionHeight),
                actualWidth: width,
                actualHeight: height);
        }

        long area;
        try
        {
            area = checked(width * height);
        }
        catch (OverflowException)
        {
            return CapacityValidationOutcome.Failure(
                CapacityValidationKind.Overflow,
                "The Selection area exceeds the supported numeric range.");
        }

        return area > MaximumSelectionArea
            ? CapacityValidationOutcome.Failure(
                CapacityValidationKind.UnsupportedSelectionArea,
                $"The Selection area ({area}) exceeds the supported limit of {MaximumSelectionArea} pixels.",
                area,
                MaximumSelectionArea,
                width,
                height)
            : CapacityValidationOutcome.Supported();
    }
}

public sealed record DisplaySnapshot
{
    public DisplaySnapshot(
        string displayId,
        PhysicalRect physicalBoundsInVirtualDesktop,
        double dpiScaleX,
        double dpiScaleY,
        string rotationOrOrientation,
        PhysicalPixelSize expectedFrozenFramePixelSize,
        string logicalSurfaceIdentity)
    {
        DisplayId = displayId ?? throw new ArgumentNullException(nameof(displayId));
        PhysicalBoundsInVirtualDesktop = physicalBoundsInVirtualDesktop;
        DpiScaleX = dpiScaleX;
        DpiScaleY = dpiScaleY;
        RotationOrOrientation = rotationOrOrientation ?? throw new ArgumentNullException(nameof(rotationOrOrientation));
        ExpectedFrozenFramePixelSize = expectedFrozenFramePixelSize;
        LogicalSurfaceIdentity = logicalSurfaceIdentity ?? throw new ArgumentNullException(nameof(logicalSurfaceIdentity));
    }

    public string DisplayId { get; }

    public PhysicalRect PhysicalBoundsInVirtualDesktop { get; }

    public double DpiScaleX { get; }

    public double DpiScaleY { get; }

    public string RotationOrOrientation { get; }

    public PhysicalPixelSize ExpectedFrozenFramePixelSize { get; }

    public string LogicalSurfaceIdentity { get; }

    public bool IsValid => !string.IsNullOrWhiteSpace(DisplayId)
        && PhysicalBoundsInVirtualDesktop.IsPositive
        && double.IsFinite(DpiScaleX)
        && double.IsFinite(DpiScaleY)
        && DpiScaleX > 0
        && DpiScaleY > 0
        && !string.IsNullOrWhiteSpace(RotationOrOrientation)
        && ExpectedFrozenFramePixelSize.IsPositive
        && ExpectedFrozenFramePixelSize.Width == PhysicalBoundsInVirtualDesktop.Width64
        && ExpectedFrozenFramePixelSize.Height == PhysicalBoundsInVirtualDesktop.Height64
        && !string.IsNullOrWhiteSpace(LogicalSurfaceIdentity);
}

public sealed record VirtualDesktopSnapshot
{
    public VirtualDesktopSnapshot(
        string coordinateVersion,
        PhysicalRect virtualPhysicalBounds,
        PhysicalPoint virtualOrigin,
        IEnumerable<DisplaySnapshot> displays,
        GapPolicy gapPolicy = GapPolicy.Transparent)
    {
        if (string.IsNullOrWhiteSpace(coordinateVersion))
        {
            throw new ArgumentException("Coordinate version is required.", nameof(coordinateVersion));
        }

        if (!virtualPhysicalBounds.IsPositive)
        {
            throw new ArgumentException("Virtual Desktop bounds must be positive.", nameof(virtualPhysicalBounds));
        }

        if (virtualOrigin != new PhysicalPoint(virtualPhysicalBounds.Left, virtualPhysicalBounds.Top))
        {
            throw new ArgumentException("Virtual origin must match the Virtual Desktop bounds origin.", nameof(virtualOrigin));
        }

        ArgumentNullException.ThrowIfNull(displays);
        var displayArray = displays.ToArray();
        if (displayArray.Any(display => display is null || !display.IsValid))
        {
            throw new ArgumentException("Every display snapshot must be valid.", nameof(displays));
        }

        if (displayArray.Select(display => display.DisplayId).Distinct(StringComparer.Ordinal).Count() != displayArray.Length)
        {
            throw new ArgumentException("Display identifiers must be unique.", nameof(displays));
        }

        if (displayArray.Select(display => display.LogicalSurfaceIdentity).Distinct(StringComparer.Ordinal).Count() != displayArray.Length)
        {
            throw new ArgumentException("Logical surface identities must be unique.", nameof(displays));
        }

        if (displayArray.Any(display => !virtualPhysicalBounds.Contains(display.PhysicalBoundsInVirtualDesktop)))
        {
            throw new ArgumentException("Every display must be contained by the Virtual Desktop bounds.", nameof(displays));
        }

        CoordinateVersion = coordinateVersion;
        VirtualPhysicalBounds = virtualPhysicalBounds;
        VirtualOrigin = virtualOrigin;
        Displays = new ReadOnlyCollection<DisplaySnapshot>(displayArray);
        GapPolicy = gapPolicy;
    }

    public string CoordinateVersion { get; }

    public PhysicalRect VirtualPhysicalBounds { get; }

    public PhysicalPoint VirtualOrigin { get; }

    public IReadOnlyList<DisplaySnapshot> Displays { get; }

    public GapPolicy GapPolicy { get; }
}
