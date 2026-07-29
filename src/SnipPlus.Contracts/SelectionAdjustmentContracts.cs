namespace SnipPlus.Contracts;

public enum SelectionInteractionMode
{
    None,
    InitialDragging,
    Locked,
    Moving,
    ResizingLeft,
    ResizingTop,
    ResizingRight,
    ResizingBottom,
    ResizingTopLeft,
    ResizingTopRight,
    ResizingBottomLeft,
    ResizingBottomRight,
    Reselecting,
    Cancelled
}

public enum SelectionHitTestKind
{
    Outside,
    Interior,
    LeftEdge,
    TopEdge,
    RightEdge,
    BottomEdge,
    TopLeftCorner,
    TopRightCorner,
    BottomLeftCorner,
    BottomRightCorner
}

public readonly record struct SelectionHitTestResult(SelectionHitTestKind Kind)
{
    public bool IsResizeHandle => Kind is
        SelectionHitTestKind.LeftEdge
        or SelectionHitTestKind.TopEdge
        or SelectionHitTestKind.RightEdge
        or SelectionHitTestKind.BottomEdge
        or SelectionHitTestKind.TopLeftCorner
        or SelectionHitTestKind.TopRightCorner
        or SelectionHitTestKind.BottomLeftCorner
        or SelectionHitTestKind.BottomRightCorner;
}

public static class SelectionHitTesting
{
    public const int DefaultHandleHitZonePixels = 8;

    public static SelectionHitTestResult HitTest(
        PhysicalRect bounds,
        PhysicalPoint point,
        int handleHitZonePixels = DefaultHandleHitZonePixels)
    {
        if (!bounds.IsPositive)
        {
            return new(SelectionHitTestKind.Outside);
        }

        var handleZone = Math.Max(1, handleHitZonePixels);
        var pointInside = point.X >= bounds.Left
            && point.X < bounds.Right
            && point.Y >= bounds.Top
            && point.Y < bounds.Bottom;
        var horizontalHandleBand = point.Y >= (long)bounds.Top - handleZone
            && point.Y <= (long)bounds.Bottom + handleZone;
        var verticalHandleBand = point.X >= (long)bounds.Left - handleZone
            && point.X <= (long)bounds.Right + handleZone;
        var leftDistance = Math.Abs((long)point.X - bounds.Left);
        var rightDistance = Math.Abs((long)point.X - bounds.Right);
        var topDistance = Math.Abs((long)point.Y - bounds.Top);
        var bottomDistance = Math.Abs((long)point.Y - bounds.Bottom);
        var nearLeft = horizontalHandleBand && leftDistance < handleZone;
        var nearRight = horizontalHandleBand && rightDistance < handleZone;
        var nearTop = verticalHandleBand && topDistance < handleZone;
        var nearBottom = verticalHandleBand && bottomDistance < handleZone;

        if (nearLeft && nearRight)
        {
            nearLeft = leftDistance <= rightDistance;
            nearRight = !nearLeft;
        }

        if (nearTop && nearBottom)
        {
            nearTop = topDistance <= bottomDistance;
            nearBottom = !nearTop;
        }

        var kind = (nearLeft, nearTop, nearRight, nearBottom) switch
        {
            (true, true, false, false) => SelectionHitTestKind.TopLeftCorner,
            (false, true, true, false) => SelectionHitTestKind.TopRightCorner,
            (true, false, false, true) => SelectionHitTestKind.BottomLeftCorner,
            (false, false, true, true) => SelectionHitTestKind.BottomRightCorner,
            (true, false, false, false) => SelectionHitTestKind.LeftEdge,
            (false, true, false, false) => SelectionHitTestKind.TopEdge,
            (false, false, true, false) => SelectionHitTestKind.RightEdge,
            (false, false, false, true) => SelectionHitTestKind.BottomEdge,
            _ when pointInside => SelectionHitTestKind.Interior,
            _ => SelectionHitTestKind.Outside
        };

        return new(kind);
    }
}
