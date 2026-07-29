using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class FunctionBarPlacementService : IFunctionBarPlacementService
{
    public FunctionBarPlacementOutcome Place(FunctionBarPlacementRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.SessionId == Guid.Empty
            || string.IsNullOrWhiteSpace(request.CoordinateVersion)
            || request.SelectionRevision < 0
            || !request.SelectionPhysicalBounds.IsPositive)
        {
            return Failed(
                FailureCode.InvalidSelection,
                FailureCategory.Validation,
                "The Function Bar placement request has invalid Selection identity or geometry.");
        }

        if (request.MarginPixels < 0
            || !request.MeasuredBarPhysicalSize.IsPositive
            || request.DisplayPhysicalWorkAreas is null
            || request.DisplayPhysicalWorkAreas.Count == 0)
        {
            return Failed(
                FailureCode.FunctionBarPlacementFailed,
                FailureCategory.Validation,
                "The Function Bar placement request has invalid size or margin data.");
        }

        foreach (var display in request.DisplayPhysicalWorkAreas)
        {
            if (display is null
                || string.IsNullOrWhiteSpace(display.DisplayId)
                || !display.DisplayPhysicalBounds.IsPositive
                || !display.PhysicalWorkArea.IsPositive
                || !display.DisplayPhysicalBounds.Contains(display.PhysicalWorkArea)
                || !double.IsFinite(display.DpiScaleX)
                || !double.IsFinite(display.DpiScaleY)
                || display.DpiScaleX <= 0
                || display.DpiScaleY <= 0)
            {
                return Failed(
                    FailureCode.InvalidWorkArea,
                    FailureCategory.Validation,
                    "A display has an invalid physical Work Area.");
            }

            if (request.MeasuredBarPhysicalSize.Width > display.PhysicalWorkArea.Width64
                || request.MeasuredBarPhysicalSize.Height > display.PhysicalWorkArea.Height64)
            {
                return Failed(
                    FailureCode.InvalidWorkArea,
                    FailureCategory.Unsupported,
                    "The Function Bar is larger than a display Work Area.");
            }
        }

        var anchor = request.DisplayPhysicalWorkAreas
            .Select(display => new
            {
                Display = display,
                Intersection = display.DisplayPhysicalBounds.Intersection(
                    request.SelectionPhysicalBounds)
            })
            .Where(candidate => candidate.Intersection.IsPositive)
            .Select(candidate => new
            {
                candidate.Display,
                Area = candidate.Intersection.Width64 * candidate.Intersection.Height64,
                ContainsCurrentPoint = request.CurrentPhysicalPoint is { } point
                    && Contains(candidate.Display.DisplayPhysicalBounds, point),
                CenterX = ((long)candidate.Intersection.Left + candidate.Intersection.Right) / 2
            })
            .OrderByDescending(candidate => candidate.Area)
            .ThenByDescending(candidate => candidate.ContainsCurrentPoint)
            .ThenBy(candidate => candidate.Display.DisplayId, StringComparer.Ordinal)
            .FirstOrDefault();

        if (anchor is null)
        {
            return Failed(
                FailureCode.InvalidSelection,
                FailureCategory.Validation,
                "The Selection does not intersect a display.");
        }

        var workArea = anchor.Display.PhysicalWorkArea;
        var barSize = request.MeasuredBarPhysicalSize;
        var centeredLeft = anchor.CenterX - barSize.Width / 2;
        var belowTop = (long)request.SelectionPhysicalBounds.Bottom + request.MarginPixels;
        var aboveTop = (long)request.SelectionPhysicalBounds.Top
            - request.MarginPixels
            - barSize.Height;

        if (Fits(workArea, centeredLeft, belowTop, barSize))
        {
            return Ready(
                anchor.Display.DisplayId,
                ToRect(centeredLeft, belowTop, barSize),
                FunctionBarPlacementSide.Below,
                request.SelectionRevision);
        }

        if (Fits(workArea, centeredLeft, aboveTop, barSize))
        {
            return Ready(
                anchor.Display.DisplayId,
                ToRect(centeredLeft, aboveTop, barSize),
                FunctionBarPlacementSide.Above,
                request.SelectionRevision);
        }

        var belowSpace = Math.Max(
            0,
            (long)workArea.Bottom
                - ((long)request.SelectionPhysicalBounds.Bottom + request.MarginPixels));
        var aboveSpace = Math.Max(
            0,
            ((long)request.SelectionPhysicalBounds.Top - request.MarginPixels)
                - workArea.Top);
        var useBelow = belowSpace >= aboveSpace;
        var clampedLeft = Math.Clamp(
            centeredLeft,
            (long)workArea.Left,
            (long)workArea.Right - barSize.Width);
        var clampedTop = Math.Clamp(
            useBelow ? belowTop : aboveTop,
            (long)workArea.Top,
            (long)workArea.Bottom - barSize.Height);

        return Ready(
            anchor.Display.DisplayId,
            ToRect(clampedLeft, clampedTop, barSize),
            useBelow
                ? FunctionBarPlacementSide.ClampedBelow
                : FunctionBarPlacementSide.ClampedAbove,
            request.SelectionRevision);
    }

    private static bool Fits(
        PhysicalRect workArea,
        long left,
        long top,
        PhysicalPixelSize size) => left >= workArea.Left
            && top >= workArea.Top
            && left + size.Width <= workArea.Right
            && top + size.Height <= workArea.Bottom;

    private static PhysicalRect ToRect(
        long left,
        long top,
        PhysicalPixelSize size) => new(
        checked((int)left),
        checked((int)top),
        checked((int)(left + size.Width)),
        checked((int)(top + size.Height)));

    private static bool Contains(PhysicalRect bounds, PhysicalPoint point) =>
        point.X >= bounds.Left
        && point.X < bounds.Right
        && point.Y >= bounds.Top
        && point.Y < bounds.Bottom;

    private static FunctionBarPlacementOutcome.Ready Ready(
        string displayId,
        PhysicalRect bounds,
        FunctionBarPlacementSide side,
        int selectionRevision) => new FunctionBarPlacementOutcome.Ready(
        new FunctionBarPlacementResult(
            displayId,
            bounds,
            side,
            selectionRevision,
            IsFullyInsideWorkArea: true));

    private static FunctionBarPlacementOutcome.Failed Failed(
        FailureCode code,
        FailureCategory category,
        string message) => new FunctionBarPlacementOutcome.Failed(Failure.Create(
        code,
        category,
        FailureRecoverability.RetryNewIntent,
        nameof(FunctionBarPlacementService),
        Guid.Empty,
        message));
}
