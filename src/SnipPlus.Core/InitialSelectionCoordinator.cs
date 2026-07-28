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

            if (_state.Status is not SelectionStatus.None)
            {
                return Ignored();
            }

            _state = _state with
            {
                SelectionRevision = _state.SelectionRevision + 1,
                Status = SelectionStatus.Dragging,
                DragStartPhysicalPoint = input.GlobalPhysicalPoint,
                CurrentPhysicalPoint = input.GlobalPhysicalPoint,
                NormalizedPhysicalBounds = new PhysicalRect(
                    input.GlobalPhysicalPoint.X,
                    input.GlobalPhysicalPoint.Y,
                    input.GlobalPhysicalPoint.X,
                    input.GlobalPhysicalPoint.Y)
            };
            return Publish(new SelectionInputResult(
                SelectionInputResultKind.Dragging,
                _state,
                "Selection drag started."));
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

            if (_state.Status != SelectionStatus.Dragging)
            {
                return Ignored();
            }

            var start = _state.DragStartPhysicalPoint!.Value;
            _state = _state with
            {
                SelectionRevision = _state.SelectionRevision + 1,
                CurrentPhysicalPoint = input.GlobalPhysicalPoint,
                NormalizedPhysicalBounds = Normalize(start, input.GlobalPhysicalPoint)
            };
            return Publish(new SelectionInputResult(
                SelectionInputResultKind.Dragging,
                _state,
                "Selection drag updated."));
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

            if (_state.Status != SelectionStatus.Dragging)
            {
                return Ignored();
            }

            var start = _state.DragStartPhysicalPoint!.Value;
            var bounds = Normalize(start, input.GlobalPhysicalPoint);
            _state = _state with
            {
                SelectionRevision = _state.SelectionRevision + 1,
                CurrentPhysicalPoint = input.GlobalPhysicalPoint,
                NormalizedPhysicalBounds = bounds
            };

            if (!IsValidSelection(bounds))
            {
                return Publish(new SelectionInputResult(
                    SelectionInputResultKind.InvalidSelection,
                    _state,
                    "Selection bounds are invalid."));
            }

            _state = _state with
            {
                SelectionRevision = _state.SelectionRevision + 1,
                Status = SelectionStatus.Locked
            };
            return Publish(new SelectionInputResult(
                SelectionInputResultKind.Locked,
                _state,
                "Selection locked."));
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
                Status = SelectionStatus.Cancelled
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

    private bool IsCurrentInput(SelectionPointerEvent input) => !_disposed
        && !_session.IsDisposed
        && input.SessionId == _session.SessionId
        && string.Equals(
            input.CoordinateVersion,
            _session.VirtualDesktopSnapshot.CoordinateVersion,
            StringComparison.Ordinal);

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

    private SelectionInputResult Publish(SelectionInputResult result)
    {
        StateChanged?.Invoke(result.State);
        return result;
    }

    private SelectionInputResult Ignored()
    {
        var state = _state;
        var kind = !_disposed
            && state.SessionId == _session.SessionId
            ? SelectionInputResultKind.Ignored
            : SelectionInputResultKind.StaleSession;
        return new SelectionInputResult(kind, state, "Selection input was ignored.");
    }

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
