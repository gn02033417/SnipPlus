using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class InitialSelectionCoordinator : ISelectionInputSink, IDisposable
{
    private readonly object _gate = new();
    private readonly CaptureSessionContext _session;
    private readonly SupportedCapacityPolicy _capacityPolicy;
    private SelectionVisualState _state;
    private bool _disposed;

    public InitialSelectionCoordinator(
        CaptureSessionContext session,
        SupportedCapacityPolicy? capacityPolicy = null)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _capacityPolicy = capacityPolicy ?? new SupportedCapacityPolicy();
        _state = SelectionVisualState.Initial(
            session.SessionId,
            session.VirtualDesktopSnapshot.CoordinateVersion);
    }

    public event Action<SelectionVisualState>? StateChanged;

    public SelectionVisualState State
    {
        get
        {
            lock (_gate)
            {
                return _state;
            }
        }
    }

    public SelectionInputResult PointerPressed(SelectionPointerEvent input)
    {
        ArgumentNullException.ThrowIfNull(input);
        lock (_gate)
        {
            if (!IsCurrentInput(input))
            {
                return Stale();
            }

            if (_state.InteractionMode == SelectionInteractionMode.None
                && _state.Status == SelectionStatus.None)
            {
                var point = input.GlobalPhysicalPoint;
                var initialBounds = new PhysicalRect(point.X, point.Y, point.X, point.Y);
                _state = _state with
                {
                    SelectionRevision = _state.SelectionRevision + 1,
                    Status = SelectionStatus.Dragging,
                    InteractionMode = SelectionInteractionMode.InitialDragging,
                    ActivePointerId = input.PointerId,
                    DragStartPhysicalPoint = point,
                    InteractionStartPhysicalPoint = point,
                    CurrentPhysicalPoint = point,
                    NormalizedPhysicalBounds = initialBounds,
                    IsGeometryValid = false,
                    HoverHitTest = SelectionHitTestKind.Outside,
                    ActiveHitTest = SelectionHitTestKind.Outside
                };
                return Publish(new SelectionInputResult(
                    SelectionInputResultKind.Dragging,
                    _state,
                    "Selection drag started."));
            }

            if (_state.Status != SelectionStatus.Locked
                || _state.InteractionMode != SelectionInteractionMode.Locked
                || _state.NormalizedPhysicalBounds is not PhysicalRect bounds
                || !bounds.IsPositive)
            {
                return Ignored();
            }

            var hitTest = SelectionHitTesting.HitTest(bounds, input.GlobalPhysicalPoint);
            var mode = hitTest.Kind switch
            {
                SelectionHitTestKind.Interior => SelectionInteractionMode.Moving,
                SelectionHitTestKind.LeftEdge => SelectionInteractionMode.ResizingLeft,
                SelectionHitTestKind.TopEdge => SelectionInteractionMode.ResizingTop,
                SelectionHitTestKind.RightEdge => SelectionInteractionMode.ResizingRight,
                SelectionHitTestKind.BottomEdge => SelectionInteractionMode.ResizingBottom,
                SelectionHitTestKind.TopLeftCorner => SelectionInteractionMode.ResizingTopLeft,
                SelectionHitTestKind.TopRightCorner => SelectionInteractionMode.ResizingTopRight,
                SelectionHitTestKind.BottomLeftCorner => SelectionInteractionMode.ResizingBottomLeft,
                SelectionHitTestKind.BottomRightCorner => SelectionInteractionMode.ResizingBottomRight,
                _ => SelectionInteractionMode.Reselecting
            };

            _state = _state with
            {
                SelectionRevision = _state.SelectionRevision + 1,
                InteractionMode = mode,
                ActivePointerId = input.PointerId,
                InteractionStartPhysicalPoint = input.GlobalPhysicalPoint,
                InteractionStartBounds = bounds,
                InteractionStartHitTest = hitTest.Kind,
                CurrentPhysicalPoint = input.GlobalPhysicalPoint,
                HoverHitTest = hitTest.Kind,
                ActiveHitTest = hitTest.Kind
            };

            var resultKind = mode switch
            {
                SelectionInteractionMode.Moving => SelectionInputResultKind.Moving,
                SelectionInteractionMode.Reselecting => SelectionInputResultKind.Reselecting,
                _ => SelectionInputResultKind.Resizing
            };
            return Publish(new SelectionInputResult(
                resultKind,
                _state,
                mode == SelectionInteractionMode.Reselecting
                    ? "Replacement Selection started."
                    : "Selection adjustment started."));
        }
    }

    public SelectionInputResult PointerMoved(SelectionPointerEvent input)
    {
        ArgumentNullException.ThrowIfNull(input);
        lock (_gate)
        {
            if (!IsCurrentInput(input))
            {
                return Stale();
            }

            switch (_state.InteractionMode)
            {
                case SelectionInteractionMode.None:
                case SelectionInteractionMode.Locked:
                    return UpdateHover(input);
                case SelectionInteractionMode.InitialDragging:
                    if (!HasActivePointer(input))
                    {
                        return Ignored();
                    }

                    return UpdateInitialPreview(input);
                case SelectionInteractionMode.Moving:
                    if (!HasActivePointer(input))
                    {
                        return Ignored();
                    }

                    return UpdateAdjustmentPreview(
                        input,
                        ClampMove(
                            _state.InteractionStartBounds!.Value,
                            _state.InteractionStartPhysicalPoint!.Value,
                            input.GlobalPhysicalPoint),
                        SelectionInteractionMode.Moving,
                        SelectionHitTestKind.Interior);
                case SelectionInteractionMode.ResizingLeft:
                case SelectionInteractionMode.ResizingTop:
                case SelectionInteractionMode.ResizingRight:
                case SelectionInteractionMode.ResizingBottom:
                case SelectionInteractionMode.ResizingTopLeft:
                case SelectionInteractionMode.ResizingTopRight:
                case SelectionInteractionMode.ResizingBottomLeft:
                case SelectionInteractionMode.ResizingBottomRight:
                    if (!HasActivePointer(input))
                    {
                        return Ignored();
                    }

                    var resized = BuildResizePreview(
                        _state.InteractionStartBounds!.Value,
                        _state.InteractionStartHitTest!.Value,
                        input.GlobalPhysicalPoint);
                    var effectiveHandle = EffectiveResizeHandle(
                        _state.InteractionStartHitTest.Value,
                        input.GlobalPhysicalPoint,
                        _state.InteractionStartBounds.Value);
                    return UpdateAdjustmentPreview(
                        input,
                        resized,
                        ModeForHandle(effectiveHandle),
                        effectiveHandle);
                case SelectionInteractionMode.Reselecting:
                    if (!HasActivePointer(input))
                    {
                        return Ignored();
                    }

                    return UpdateAdjustmentPreview(
                        input,
                        Normalize(
                            _state.InteractionStartPhysicalPoint!.Value,
                            input.GlobalPhysicalPoint),
                        SelectionInteractionMode.Reselecting,
                        SelectionHitTestKind.Outside);
                default:
                    return Ignored();
            }
        }
    }

    public SelectionInputResult PointerReleased(SelectionPointerEvent input)
    {
        ArgumentNullException.ThrowIfNull(input);
        lock (_gate)
        {
            if (!IsCurrentInput(input))
            {
                return Stale();
            }

            if (_state.InteractionMode == SelectionInteractionMode.InitialDragging)
            {
                if (!HasActivePointer(input))
                {
                    return Ignored();
                }

                var start = _state.DragStartPhysicalPoint!.Value;
                var bounds = Normalize(start, input.GlobalPhysicalPoint);
                var initialPreviewRevision = _state.SelectionRevision + 1;
                _state = _state with
                {
                    SelectionRevision = initialPreviewRevision,
                    CurrentPhysicalPoint = input.GlobalPhysicalPoint,
                    NormalizedPhysicalBounds = bounds,
                    IsGeometryValid = IsValidSelection(bounds)
                };

                if (!_state.IsGeometryValid)
                {
                    return Publish(new SelectionInputResult(
                        SelectionInputResultKind.InvalidSelection,
                        _state,
                        "Selection bounds are invalid."));
                }

                _state = _state with
                {
                    SelectionRevision = initialPreviewRevision + 1,
                    Status = SelectionStatus.Locked,
                    InteractionMode = SelectionInteractionMode.Locked,
                    ActivePointerId = null,
                    InteractionStartPhysicalPoint = null,
                    InteractionStartBounds = null,
                    InteractionStartHitTest = null,
                    ActiveHitTest = SelectionHitTestKind.Outside,
                    HoverHitTest = SelectionHitTesting.HitTest(
                        bounds,
                        input.GlobalPhysicalPoint).Kind
                };
                return Publish(new SelectionInputResult(
                    SelectionInputResultKind.Locked,
                    _state,
                    "Selection locked."));
            }

            if (!IsAdjustmentMode(_state.InteractionMode)
                || !HasActivePointer(input))
            {
                return Ignored();
            }

            var originalBounds = _state.InteractionStartBounds!.Value;
            var candidate = _state.InteractionMode == SelectionInteractionMode.Moving
                ? ClampMove(
                    originalBounds,
                    _state.InteractionStartPhysicalPoint!.Value,
                    input.GlobalPhysicalPoint)
                : _state.InteractionMode == SelectionInteractionMode.Reselecting
                    ? Normalize(
                        _state.InteractionStartPhysicalPoint!.Value,
                        input.GlobalPhysicalPoint)
                    : BuildResizePreview(
                        originalBounds,
                        _state.InteractionStartHitTest!.Value,
                        input.GlobalPhysicalPoint);
            var adjustmentPreviewRevision = _state.SelectionRevision + 1;
            if (!IsValidSelection(candidate))
            {
                _state = _state with
                {
                    SelectionRevision = adjustmentPreviewRevision + 1,
                    InteractionMode = SelectionInteractionMode.Locked,
                    ActivePointerId = null,
                    InteractionStartPhysicalPoint = null,
                    InteractionStartBounds = null,
                    InteractionStartHitTest = null,
                    CurrentPhysicalPoint = input.GlobalPhysicalPoint,
                    NormalizedPhysicalBounds = originalBounds,
                    IsGeometryValid = true,
                    ActiveHitTest = SelectionHitTestKind.Outside,
                    HoverHitTest = SelectionHitTesting.HitTest(
                        originalBounds,
                        input.GlobalPhysicalPoint).Kind
                };
                return Publish(new SelectionInputResult(
                    SelectionInputResultKind.AdjustmentRolledBack,
                    _state,
                    "The Selection adjustment was invalid; the previous Selection was restored."));
            }

            _state = _state with
            {
                SelectionRevision = adjustmentPreviewRevision + 1,
                InteractionMode = SelectionInteractionMode.Locked,
                ActivePointerId = null,
                InteractionStartPhysicalPoint = null,
                InteractionStartBounds = null,
                InteractionStartHitTest = null,
                CurrentPhysicalPoint = input.GlobalPhysicalPoint,
                NormalizedPhysicalBounds = candidate,
                IsGeometryValid = true,
                ActiveHitTest = SelectionHitTestKind.Outside,
                HoverHitTest = SelectionHitTesting.HitTest(
                    candidate,
                    input.GlobalPhysicalPoint).Kind
            };
            return Publish(new SelectionInputResult(
                SelectionInputResultKind.AdjustmentCommitted,
                _state,
                "Selection adjustment committed."));
        }
    }

    public SelectionInputResult Escape(Guid sessionId, string coordinateVersion)
    {
        lock (_gate)
        {
            if (_disposed
                || sessionId != _session.SessionId
                || !string.Equals(
                    coordinateVersion,
                    _session.VirtualDesktopSnapshot.CoordinateVersion,
                    StringComparison.Ordinal)
                || _state.Status == SelectionStatus.Cancelled)
            {
                return Ignored();
            }

            _state = _state with
            {
                SelectionRevision = _state.SelectionRevision + 1,
                Status = SelectionStatus.Cancelled,
                InteractionMode = SelectionInteractionMode.Cancelled,
                ActivePointerId = null,
                InteractionStartPhysicalPoint = null,
                InteractionStartBounds = null,
                InteractionStartHitTest = null,
                IsGeometryValid = false
            };
            return Publish(new SelectionInputResult(
                SelectionInputResultKind.Cancelled,
                _state,
                "Selection cancelled."));
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    private SelectionInputResult UpdateHover(SelectionPointerEvent input)
    {
        if (_state.Status != SelectionStatus.Locked
            || _state.NormalizedPhysicalBounds is not PhysicalRect bounds)
        {
            return Ignored();
        }

        var hitTest = SelectionHitTesting.HitTest(bounds, input.GlobalPhysicalPoint).Kind;
        if (_state.HoverHitTest == hitTest
            && _state.CurrentPhysicalPoint == input.GlobalPhysicalPoint)
        {
            return new SelectionInputResult(
                SelectionInputResultKind.HitTested,
                _state,
                "Selection hit-test unchanged.");
        }

        _state = _state with
        {
            CurrentPhysicalPoint = input.GlobalPhysicalPoint,
            HoverHitTest = hitTest
        };
        return Publish(new SelectionInputResult(
            SelectionInputResultKind.HitTested,
            _state,
            "Selection hit-test updated."));
    }

    private SelectionInputResult UpdateInitialPreview(SelectionPointerEvent input)
    {
        var bounds = Normalize(
            _state.DragStartPhysicalPoint!.Value,
            input.GlobalPhysicalPoint);
        _state = _state with
        {
            SelectionRevision = _state.SelectionRevision + 1,
            CurrentPhysicalPoint = input.GlobalPhysicalPoint,
            NormalizedPhysicalBounds = bounds,
            IsGeometryValid = IsValidSelection(bounds)
        };
        return Publish(new SelectionInputResult(
            SelectionInputResultKind.Dragging,
            _state,
            "Selection drag updated."));
    }

    private SelectionInputResult UpdateAdjustmentPreview(
        SelectionPointerEvent input,
        PhysicalRect bounds,
        SelectionInteractionMode mode,
        SelectionHitTestKind activeHitTest)
    {
        _state = _state with
        {
            SelectionRevision = _state.SelectionRevision + 1,
            InteractionMode = mode,
            CurrentPhysicalPoint = input.GlobalPhysicalPoint,
            NormalizedPhysicalBounds = bounds,
            ActiveHitTest = activeHitTest,
            IsGeometryValid = IsValidSelection(bounds)
        };
        var kind = mode == SelectionInteractionMode.Moving
            ? SelectionInputResultKind.Moving
            : mode == SelectionInteractionMode.Reselecting
                ? SelectionInputResultKind.Reselecting
                : SelectionInputResultKind.Resizing;
        return Publish(new SelectionInputResult(
            kind,
            _state,
            "Selection adjustment updated."));
    }

    private bool IsCurrentInput(SelectionPointerEvent input) => !_disposed
        && !_session.IsDisposed
        && input.SessionId == _session.SessionId
        && string.Equals(
            input.CoordinateVersion,
            _session.VirtualDesktopSnapshot.CoordinateVersion,
            StringComparison.Ordinal);

    private bool HasActivePointer(SelectionPointerEvent input) =>
        _state.ActivePointerId == input.PointerId;

    private bool IsValidSelection(PhysicalRect bounds)
    {
        if (!bounds.IsPositive
            || !_session.VirtualDesktopSnapshot.VirtualPhysicalBounds.Contains(bounds))
        {
            return false;
        }

        if (!_session.VirtualDesktopSnapshot.Displays.Any(
                display => display.PhysicalBoundsInVirtualDesktop.Intersects(bounds)))
        {
            return false;
        }

        return _capacityPolicy.ValidateSelection(bounds).IsSupported;
    }

    private PhysicalRect ClampMove(
        PhysicalRect original,
        PhysicalPoint start,
        PhysicalPoint current)
    {
        var virtualBounds = _session.VirtualDesktopSnapshot.VirtualPhysicalBounds;
        var deltaX = (long)current.X - start.X;
        var deltaY = (long)current.Y - start.Y;
        var minLeft = (long)virtualBounds.Left;
        var maxLeft = (long)virtualBounds.Right - original.Width64;
        var minTop = (long)virtualBounds.Top;
        var maxTop = (long)virtualBounds.Bottom - original.Height64;
        var left = Math.Clamp((long)original.Left + deltaX, minLeft, maxLeft);
        var top = Math.Clamp((long)original.Top + deltaY, minTop, maxTop);
        return new(
            checked((int)left),
            checked((int)top),
            checked((int)(left + original.Width64)),
            checked((int)(top + original.Height64)));
    }

    private PhysicalRect BuildResizePreview(
        PhysicalRect original,
        SelectionHitTestKind handle,
        PhysicalPoint pointer)
    {
        var clamped = ClampToVirtualBounds(pointer);
        return handle switch
        {
            SelectionHitTestKind.LeftEdge => Normalize(
                new PhysicalPoint(clamped.X, original.Top),
                new PhysicalPoint(original.Right, original.Bottom)),
            SelectionHitTestKind.TopEdge => Normalize(
                new PhysicalPoint(original.Left, clamped.Y),
                new PhysicalPoint(original.Right, original.Bottom)),
            SelectionHitTestKind.RightEdge => Normalize(
                new PhysicalPoint(original.Left, original.Top),
                new PhysicalPoint(clamped.X, original.Bottom)),
            SelectionHitTestKind.BottomEdge => Normalize(
                new PhysicalPoint(original.Left, original.Top),
                new PhysicalPoint(original.Right, clamped.Y)),
            SelectionHitTestKind.TopLeftCorner => Normalize(
                clamped,
                new PhysicalPoint(original.Right, original.Bottom)),
            SelectionHitTestKind.TopRightCorner => Normalize(
                new PhysicalPoint(original.Left, clamped.Y),
                new PhysicalPoint(clamped.X, original.Bottom)),
            SelectionHitTestKind.BottomLeftCorner => Normalize(
                new PhysicalPoint(clamped.X, original.Top),
                new PhysicalPoint(original.Right, clamped.Y)),
            SelectionHitTestKind.BottomRightCorner => Normalize(
                new PhysicalPoint(original.Left, original.Top),
                clamped),
            _ => original
        };
    }

    private PhysicalPoint ClampToVirtualBounds(PhysicalPoint point)
    {
        var bounds = _session.VirtualDesktopSnapshot.VirtualPhysicalBounds;
        return new(
            Math.Clamp(point.X, bounds.Left, bounds.Right),
            Math.Clamp(point.Y, bounds.Top, bounds.Bottom));
    }

    private static SelectionHitTestKind EffectiveResizeHandle(
        SelectionHitTestKind initialHandle,
        PhysicalPoint point,
        PhysicalRect original)
    {
        var left = point.X <= original.Right;
        var right = point.X >= original.Left;
        var top = point.Y <= original.Bottom;
        var bottom = point.Y >= original.Top;
        return initialHandle switch
        {
            SelectionHitTestKind.LeftEdge => left
                ? SelectionHitTestKind.LeftEdge
                : SelectionHitTestKind.RightEdge,
            SelectionHitTestKind.RightEdge => right
                ? SelectionHitTestKind.RightEdge
                : SelectionHitTestKind.LeftEdge,
            SelectionHitTestKind.TopEdge => top
                ? SelectionHitTestKind.TopEdge
                : SelectionHitTestKind.BottomEdge,
            SelectionHitTestKind.BottomEdge => bottom
                ? SelectionHitTestKind.BottomEdge
                : SelectionHitTestKind.TopEdge,
            SelectionHitTestKind.TopLeftCorner => Corner(left, top),
            SelectionHitTestKind.TopRightCorner => Corner(right, top),
            SelectionHitTestKind.BottomLeftCorner => Corner(left, bottom),
            SelectionHitTestKind.BottomRightCorner => Corner(right, bottom),
            _ => initialHandle
        };
    }

    private static SelectionHitTestKind Corner(bool left, bool top) => (left, top) switch
    {
        (true, true) => SelectionHitTestKind.TopLeftCorner,
        (false, true) => SelectionHitTestKind.TopRightCorner,
        (true, false) => SelectionHitTestKind.BottomLeftCorner,
        _ => SelectionHitTestKind.BottomRightCorner
    };

    private static SelectionInteractionMode ModeForHandle(SelectionHitTestKind handle) => handle switch
    {
        SelectionHitTestKind.LeftEdge => SelectionInteractionMode.ResizingLeft,
        SelectionHitTestKind.TopEdge => SelectionInteractionMode.ResizingTop,
        SelectionHitTestKind.RightEdge => SelectionInteractionMode.ResizingRight,
        SelectionHitTestKind.BottomEdge => SelectionInteractionMode.ResizingBottom,
        SelectionHitTestKind.TopLeftCorner => SelectionInteractionMode.ResizingTopLeft,
        SelectionHitTestKind.TopRightCorner => SelectionInteractionMode.ResizingTopRight,
        SelectionHitTestKind.BottomLeftCorner => SelectionInteractionMode.ResizingBottomLeft,
        SelectionHitTestKind.BottomRightCorner => SelectionInteractionMode.ResizingBottomRight,
        _ => SelectionInteractionMode.Locked
    };

    private static bool IsAdjustmentMode(SelectionInteractionMode mode) => mode is
        SelectionInteractionMode.Moving
        or SelectionInteractionMode.ResizingLeft
        or SelectionInteractionMode.ResizingTop
        or SelectionInteractionMode.ResizingRight
        or SelectionInteractionMode.ResizingBottom
        or SelectionInteractionMode.ResizingTopLeft
        or SelectionInteractionMode.ResizingTopRight
        or SelectionInteractionMode.ResizingBottomLeft
        or SelectionInteractionMode.ResizingBottomRight
        or SelectionInteractionMode.Reselecting;

    private SelectionInputResult Publish(SelectionInputResult result)
    {
        StateChanged?.Invoke(result.State);
        return result;
    }

    private SelectionInputResult Ignored() => new(
        SelectionInputResultKind.Ignored,
        _state,
        "Selection input was ignored.");

    private SelectionInputResult Stale() => new(
        SelectionInputResultKind.StaleSession,
        _state,
        "Selection input belongs to a stale session.");

    private static PhysicalRect Normalize(PhysicalPoint first, PhysicalPoint second) => new(
        Math.Min(first.X, second.X),
        Math.Min(first.Y, second.Y),
        Math.Max(first.X, second.X),
        Math.Max(first.Y, second.Y));
}
