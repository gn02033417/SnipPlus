using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using SnipPlus.Contracts;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Imaging;
using WinRT.Interop;

namespace SnipPlus.Windows;

public sealed class WindowsFrozenDisplayOverlayCoordinator :
    IAllDisplayOverlayPresentationCoordinator,
    IFunctionBarPresentationCoordinator
{
    private readonly object _gate = new();
    private readonly Dictionary<Guid, IReadOnlyList<OverlaySurface>> _sessions = new();
    private readonly Dictionary<Guid, FunctionBarSessionPresentation> _functionBars = new();
    private readonly IFunctionBarPlacementService _placementService;
    private bool _disposed;

    public WindowsFrozenDisplayOverlayCoordinator(IFunctionBarPlacementService placementService)
    {
        _placementService = placementService
            ?? throw new ArgumentNullException(nameof(placementService));
    }

    public async ValueTask<FrozenDisplayOverlayPresentationOutcome> PresentAsync(
        FrozenDisplayOverlayPresentationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var surfaces = new List<OverlaySurface>(request.Plan.Displays.Count);
        try
        {
            if (request.Plan.Displays.Count == 0
                || request.Plan.Displays.Select(display => display.DisplayId)
                    .Distinct(StringComparer.Ordinal).Count() != request.Plan.Displays.Count)
            {
                return Failed(request.Plan.SessionId, FailureCode.OverlayCreationFailed,
                    "The overlay plan must contain one unique surface for every display.");
            }

            foreach (var descriptor in request.Plan.Displays)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var surface = new OverlaySurface(descriptor, request.InputSink);
                surfaces.Add(surface);
                await surface.InitializeAsync(cancellationToken).ConfigureAwait(true);
            }

            lock (_gate)
            {
                if (_disposed)
                {
                    DisposeSurfaces(surfaces);
                    return new FrozenDisplayOverlayPresentationOutcome.Cancelled("ApplicationExiting");
                }

                if (_sessions.ContainsKey(request.Plan.SessionId))
                {
                    DisposeSurfaces(surfaces);
                    return Failed(request.Plan.SessionId, FailureCode.OverlayCreationFailed,
                        "An overlay session already exists for this capture session.");
                }

                _sessions.Add(request.Plan.SessionId, surfaces);
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                OverlaySurface.ShowAll(surfaces);
            }
            catch (OperationCanceledException)
            {
                await CloseAsync(request.Plan.SessionId, CancellationToken.None).ConfigureAwait(true);
                return new FrozenDisplayOverlayPresentationOutcome.Cancelled("CancellationToken");
            }
            catch (Exception exception)
            {
                await CloseAsync(request.Plan.SessionId, CancellationToken.None).ConfigureAwait(true);
                return Failed(
                    request.Plan.SessionId,
                    FailureCode.OverlayPresentationFailed,
                    exception.GetType().Name,
                    exception.HResult);
            }

            return new FrozenDisplayOverlayPresentationOutcome.Ready();
        }
        catch (OperationCanceledException)
        {
            DisposeSurfaces(surfaces);
            return new FrozenDisplayOverlayPresentationOutcome.Cancelled("CancellationToken");
        }
        catch (Exception exception)
        {
            DisposeSurfaces(surfaces);
            return Failed(
                request.Plan.SessionId,
                FailureCode.OverlayCreationFailed,
                exception.GetType().Name,
                exception.HResult);
        }
    }

    public FunctionBarPresentationResult Prepare(FunctionBarPresentationRequest request) =>
        PrepareOrReposition(request, allowExisting: false);

    public FunctionBarPresentationResult Reposition(FunctionBarPresentationRequest request) =>
        PrepareOrReposition(request, allowExisting: true);

    public FunctionBarPresentationResult Show(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision)
    {
        lock (_gate)
        {
            if (!_functionBars.TryGetValue(sessionId, out var presentation))
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.StaleSession,
                    sessionId,
                    coordinateVersion,
                    selectionRevision,
                    null,
                    null,
                    "The Function Bar session is no longer active.");
            }

            if (!string.Equals(
                    presentation.Request.CoordinateVersion,
                    coordinateVersion,
                    StringComparison.Ordinal)
                || selectionRevision != presentation.Request.Selection.SelectionRevision)
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.StaleSelectionRevision,
                    sessionId,
                    coordinateVersion,
                    selectionRevision,
                    presentation.Placement,
                    null,
                    "The Function Bar show request is stale.");
            }

            presentation.Surface.SetVisible(true);
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Shown,
                sessionId,
                coordinateVersion,
                selectionRevision,
                presentation.Placement,
                null,
                "The Function Bar is visible.");
        }
    }

    public FunctionBarPresentationResult Hide(Guid sessionId)
    {
        lock (_gate)
        {
            if (!_functionBars.TryGetValue(sessionId, out var presentation))
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.Hidden,
                    sessionId,
                    string.Empty,
                    0,
                    null,
                    null,
                    "The Function Bar was already hidden.");
            }

            presentation.Surface.SetVisible(false);
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Hidden,
                sessionId,
                presentation.Request.CoordinateVersion,
                presentation.Request.Selection.SelectionRevision,
                presentation.Placement,
                null,
                "The Function Bar is hidden while the Selection is adjusted.");
        }
    }

    public FunctionBarPresentationResult Close(Guid sessionId)
    {
        FunctionBarSessionPresentation? presentation;
        lock (_gate)
        {
            _functionBars.Remove(sessionId, out presentation);
        }

        presentation?.Dispose();
        return FunctionBarResult(
            FunctionBarPresentationResultKind.Closed,
            sessionId,
            presentation?.Request.CoordinateVersion ?? string.Empty,
            presentation?.Request.Selection.SelectionRevision ?? 0,
            presentation?.Placement,
            null,
            "The Function Bar is closed.");
    }

    public void ApplySelection(SelectionVisualState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        IReadOnlyList<OverlaySurface>? surfaces;
        lock (_gate)
        {
            _sessions.TryGetValue(state.SessionId, out surfaces);
        }

        if (surfaces is null)
        {
            return;
        }

        foreach (var surface in surfaces)
        {
            surface.ApplySelection(state);
        }
    }

    public async ValueTask CloseAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Close(sessionId);
        IReadOnlyList<OverlaySurface>? surfaces;
        lock (_gate)
        {
            _sessions.Remove(sessionId, out surfaces);
        }

        if (surfaces is null)
        {
            return;
        }

        foreach (var surface in surfaces)
        {
            cancellationToken.ThrowIfCancellationRequested();
            surface.Dispose();
        }

        await ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        IReadOnlyList<OverlaySurface>[] sessions;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            sessions = _sessions.Values.ToArray();
            _sessions.Clear();
            var functionBars = _functionBars.Values.ToArray();
            _functionBars.Clear();
            foreach (var functionBar in functionBars)
            {
                functionBar.Dispose();
            }
        }

        foreach (var surfaces in sessions)
        {
            DisposeSurfaces(surfaces);
        }

        GC.SuppressFinalize(this);
    }

    private static void DisposeSurfaces(IEnumerable<OverlaySurface> surfaces)
    {
        foreach (var surface in surfaces.Reverse())
        {
            surface.Dispose();
        }
    }

    private static FrozenDisplayOverlayPresentationOutcome.Failed Failed(
        Guid correlationId,
        FailureCode code,
        string message,
        int? nativeCode = null) => new(Failure.Create(
        code,
        FailureCategory.Resource,
        FailureRecoverability.RetryNewIntent,
        nameof(WindowsFrozenDisplayOverlayCoordinator),
        correlationId,
        message,
        nativeCode: nativeCode));

    private FunctionBarPresentationResult PrepareOrReposition(
        FunctionBarPresentationRequest request,
        bool allowExisting)
    {
        ArgumentNullException.ThrowIfNull(request);
        IReadOnlyList<OverlaySurface>? surfaces;
        FunctionBarSessionPresentation? existing;
        lock (_gate)
        {
            _sessions.TryGetValue(request.SessionId, out surfaces);
            _functionBars.TryGetValue(request.SessionId, out existing);
        }

        if (surfaces is null)
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.StaleSession,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                null,
                "The Function Bar belongs to a stale capture session.");
        }

        if (surfaces.Any(surface => !string.Equals(
                surface.CoordinateVersion,
                request.CoordinateVersion,
                StringComparison.Ordinal)))
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.StaleSession,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                existing?.Placement,
                null,
                "The Function Bar coordinate version is stale.");
        }

        if (existing is not null
            && (request.Selection.SelectionRevision < existing.Request.Selection.SelectionRevision
                || (!allowExisting
                    && existing.Request.Selection.SelectionRevision != request.Selection.SelectionRevision)))
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.StaleSelectionRevision,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                existing.Placement,
                null,
                "The Function Bar already has a newer Selection revision.");
        }

        var workAreas = new List<FunctionBarDisplayWorkArea>(surfaces.Count);
        foreach (var surface in surfaces)
        {
            if (!surface.TryGetPhysicalWorkArea(out var workArea))
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.Failed,
                    request.SessionId,
                    request.CoordinateVersion,
                    request.Selection.SelectionRevision,
                    null,
                    CreateFailure(
                        request.SessionId,
                        FailureCode.InvalidWorkArea,
                        "A display Work Area could not be read."),
                    "The Function Bar could not determine a display Work Area.");
            }

            workAreas.Add(new FunctionBarDisplayWorkArea(
                surface.DisplayId,
                surface.PhysicalBounds,
                workArea,
                surface.RasterizationScale,
                surface.RasterizationScale));
        }

        var anchor = _placementService.Place(new FunctionBarPlacementRequest(
            request.SessionId,
            request.CoordinateVersion,
            request.Selection.SelectionRevision,
            request.Selection.NormalizedPhysicalBounds!.Value,
            workAreas,
            new PhysicalPixelSize(1, 1),
            MarginPixels: 8,
            request.Selection.CurrentPhysicalPoint));
        if (anchor is not FunctionBarPlacementOutcome.Ready anchorReady)
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Failed,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                anchor is FunctionBarPlacementOutcome.Failed failed
                    ? failed.Failure
                    : CreateFailure(
                        request.SessionId,
                        FailureCode.FunctionBarPlacementFailed,
                        "The Function Bar anchor could not be selected."),
                "The Function Bar anchor could not be selected.");
        }

        var anchorSurface = surfaces.FirstOrDefault(
            surface => string.Equals(
                surface.DisplayId,
                anchorReady.Placement.DisplayId,
                StringComparison.Ordinal));
        if (anchorSurface is null)
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Failed,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                CreateFailure(
                    request.SessionId,
                    FailureCode.InvalidWorkArea,
                    "The Function Bar anchor display is no longer available."),
                "The Function Bar anchor display is no longer available.");
        }

        FunctionBarSurface? surfaceForBar = null;
        var reusesExisting = existing is not null
            && ReferenceEquals(existing.Surface.Owner, anchorSurface);
        if (reusesExisting)
        {
            surfaceForBar = existing!.Surface;
            surfaceForBar.Update(request);
        }
        else
        {
            surfaceForBar = anchorSurface.CreateFunctionBar(request);
        }

        if (!surfaceForBar.TryMeasurePhysicalSize(out var measuredSize))
        {
            if (!reusesExisting)
            {
                surfaceForBar.Dispose();
            }

            return FunctionBarResult(
                FunctionBarPresentationResultKind.Failed,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                CreateFailure(
                    request.SessionId,
                    FailureCode.BarMeasurementFailed,
                    "The Function Bar could not be measured."),
                "The Function Bar could not be measured.");
        }

        var placed = _placementService.Place(new FunctionBarPlacementRequest(
            request.SessionId,
            request.CoordinateVersion,
            request.Selection.SelectionRevision,
            request.Selection.NormalizedPhysicalBounds.Value,
            workAreas,
            measuredSize,
            MarginPixels: 8,
            request.Selection.CurrentPhysicalPoint));
        if (placed is not FunctionBarPlacementOutcome.Ready placedReady)
        {
            if (!reusesExisting)
            {
                surfaceForBar.Dispose();
            }

            return FunctionBarResult(
                FunctionBarPresentationResultKind.Failed,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                placed is FunctionBarPlacementOutcome.Failed failed
                    ? failed.Failure
                    : CreateFailure(
                        request.SessionId,
                        FailureCode.FunctionBarPlacementFailed,
                        "The Function Bar could not be placed."),
                "The Function Bar could not be placed.");
        }

        surfaceForBar.ApplyPlacement(placedReady.Placement);
        surfaceForBar.SetVisible(false);
        var next = new FunctionBarSessionPresentation(request, placedReady.Placement, surfaceForBar);
        lock (_gate)
        {
            if (_disposed || !_sessions.ContainsKey(request.SessionId))
            {
                next.Dispose();
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.StaleSession,
                    request.SessionId,
                    request.CoordinateVersion,
                    request.Selection.SelectionRevision,
                    null,
                    null,
                    "The Function Bar session ended while it was being prepared.");
            }

            _functionBars[request.SessionId] = next;
        }

        if (existing is not null && !ReferenceEquals(existing.Surface, surfaceForBar))
        {
            existing.Dispose();
        }

        return FunctionBarResult(
            FunctionBarPresentationResultKind.Ready,
            request.SessionId,
            request.CoordinateVersion,
            request.Selection.SelectionRevision,
            placedReady.Placement,
            null,
            "The Function Bar is prepared and hidden.");
    }

    private static FunctionBarPresentationResult FunctionBarResult(
        FunctionBarPresentationResultKind kind,
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        FunctionBarPlacementResult? placement,
        Failure? failure,
        string message) => new(
        kind,
        sessionId,
        coordinateVersion,
        selectionRevision,
        placement,
        failure,
        message);

    private static Failure CreateFailure(Guid correlationId, FailureCode code, string message) =>
        Failure.Create(
            code,
            FailureCategory.Resource,
            FailureRecoverability.RetryNewIntent,
            nameof(WindowsFrozenDisplayOverlayCoordinator),
            correlationId,
            message);

    private sealed class FunctionBarSessionPresentation : IDisposable
    {
        public FunctionBarSessionPresentation(
            FunctionBarPresentationRequest request,
            FunctionBarPlacementResult placement,
            FunctionBarSurface surface)
        {
            Request = request;
            Placement = placement;
            Surface = surface;
        }

        public FunctionBarPresentationRequest Request { get; }

        public FunctionBarPlacementResult Placement { get; }

        public FunctionBarSurface Surface { get; }

        public void Dispose() => Surface.Dispose();
    }

    private sealed class FunctionBarSurface : IDisposable
    {
        private readonly OverlaySurface _owner;
        private readonly Border _root = CreateRoot();
        private readonly StackPanel _panel = new()
        {
            Orientation = Orientation.Horizontal
        };
        private readonly IReadOnlyDictionary<FunctionBarCommand, Button> _buttons;
        private FunctionBarPresentationRequest _request;
        private bool _disposed;

        public FunctionBarSurface(
            OverlaySurface owner,
            FunctionBarPresentationRequest request)
        {
            _owner = owner;
            _request = request;
            _buttons = new Dictionary<FunctionBarCommand, Button>
            {
                [FunctionBarCommand.Complete] = CreateButton("Complete"),
                [FunctionBarCommand.Save] = CreateButton("Save"),
                [FunctionBarCommand.Cancel] = CreateButton("Cancel"),
                [FunctionBarCommand.Undo] = CreateButton("Undo"),
                [FunctionBarCommand.Redo] = CreateButton("Redo")
            };
            _buttons[FunctionBarCommand.Cancel].Click += OnCancelClicked;
            _root.PointerPressed += OnPointerPressed;
            foreach (var button in _buttons.Values)
            {
                _panel.Children.Add(button);
            }

            _root.Child = _panel;
            Update(request);
        }

        public OverlaySurface Owner => _owner;

        public FrameworkElement Root => _root;

        public void Update(FunctionBarPresentationRequest request)
        {
            _request = request;
            foreach (var pair in _buttons)
            {
                pair.Value.IsEnabled = request.Availability.IsEnabled(pair.Key);
            }
        }

        public bool TryMeasurePhysicalSize(out PhysicalPixelSize size)
        {
            size = default;
            if (_disposed)
            {
                return false;
            }

            _root.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return TryConvertToPhysicalSize(
                _root.DesiredSize,
                _owner.RasterizationScale,
                out size);
        }

        public void ApplyPlacement(FunctionBarPlacementResult placement)
        {
            var bounds = _owner.PhysicalBounds;
            var scale = _owner.RasterizationScale;
            Canvas.SetLeft(_root, (placement.FunctionBarPhysicalBounds.Left - bounds.Left) / scale);
            Canvas.SetTop(_root, (placement.FunctionBarPhysicalBounds.Top - bounds.Top) / scale);
            _root.Width = placement.FunctionBarPhysicalBounds.Width / scale;
            _root.Height = placement.FunctionBarPhysicalBounds.Height / scale;
        }

        public void SetVisible(bool visible)
        {
            if (!_disposed)
            {
                ApplyVisibility(_root, visible);
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _buttons[FunctionBarCommand.Cancel].Click -= OnCancelClicked;
            _root.PointerPressed -= OnPointerPressed;
            _owner.RemoveFunctionBar(this);
            _root.Opacity = 0;
            _root.IsHitTestVisible = false;
            _root.Visibility = Visibility.Collapsed;
            _panel.Children.Clear();
            _root.Child = null;
        }

        private static Border CreateRoot()
        {
            var state = GetVisibilityState(visible: false);
            return new Border
            {
                Background = new SolidColorBrush(ColorHelper.FromArgb(245, 32, 32, 32)),
                BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 128, 128, 128)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(6),
                Visibility = state.IsLayoutParticipating
                    ? Visibility.Visible
                    : Visibility.Collapsed,
                Opacity = state.Opacity,
                IsHitTestVisible = state.IsHitTestVisible
            };
        }

        private static void ApplyVisibility(Border root, bool visible)
        {
            var state = GetVisibilityState(visible);
            root.Visibility = state.IsLayoutParticipating
                ? Visibility.Visible
                : Visibility.Collapsed;
            root.Opacity = state.Opacity;
            root.IsHitTestVisible = state.IsHitTestVisible;
        }

        private static FunctionBarVisibilityState GetVisibilityState(bool visible) => visible
            ? new FunctionBarVisibilityState(true, 1, true)
            : new FunctionBarVisibilityState(true, 0, false);

        private readonly record struct FunctionBarVisibilityState(
            bool IsLayoutParticipating,
            double Opacity,
            bool IsHitTestVisible);

        private static bool TryConvertToPhysicalSize(
            Size desired,
            double rasterizationScale,
            out PhysicalPixelSize size)
        {
            size = default;
            if (!double.IsFinite(rasterizationScale)
                || rasterizationScale <= 0
                || !double.IsFinite(desired.Width)
                || !double.IsFinite(desired.Height)
                || desired.Width <= 0
                || desired.Height <= 0)
            {
                return false;
            }

            var width = (int)Math.Ceiling(desired.Width * rasterizationScale);
            var height = (int)Math.Ceiling(desired.Height * rasterizationScale);
            if (width <= 0 || height <= 0)
            {
                return false;
            }

            size = new PhysicalPixelSize(width, height);
            return true;
        }

        private static Button CreateButton(string label)
        {
            var button = new Button
            {
                Content = label,
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(2, 0, 2, 0),
                IsTabStop = true
            };
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(button, label);
            return button;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            _ = _request.CommandSink.Execute(new FunctionBarCommandRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                FunctionBarCommand.Cancel));
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args) =>
            args.Handled = true;
    }

    private sealed class OverlaySurface : IDisposable
    {
        private const int PerMonitorAwareV2 = -4;
        private const int GwlExStyle = -20;
        private const int SwHide = 0;
        private const nint WsExAppWindow = 0x00040000;
        private const nint WsExToolWindow = 0x00000080;
        private const uint SwpNoSize = 0x0001;
        private const uint SwpNoMove = 0x0002;
        private const uint SwpNoZOrder = 0x0004;
        private const uint SwpNoActivate = 0x0010;
        private const uint SwpShowWindow = 0x0040;
        private const int OverlayMaskAlpha = 0x99;
        private const double HandleVisualSizePixels = 8;

        private readonly FrozenDisplayOverlayDescriptor _descriptor;
        private readonly ISelectionInputSink _inputSink;
        private readonly Window _window = new();
        private readonly Grid _root = new();
        private readonly Image _image = new();
        private readonly CrosshairCanvas _canvas = new();
        private readonly Rectangle _maskTop = CreateMask();
        private readonly Rectangle _maskLeft = CreateMask();
        private readonly Rectangle _maskRight = CreateMask();
        private readonly Rectangle _maskBottom = CreateMask();
        private readonly Rectangle _selectionBorder = new()
        {
            Fill = new SolidColorBrush(ColorHelper.FromArgb(0, 0, 0, 0)),
            Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 187, 215, 255)),
            StrokeThickness = 2,
            Visibility = Visibility.Collapsed
        };
        private readonly IReadOnlyDictionary<SelectionHitTestKind, Rectangle> _handles =
            new Dictionary<SelectionHitTestKind, Rectangle>
            {
                [SelectionHitTestKind.LeftEdge] = CreateHandle(),
                [SelectionHitTestKind.TopEdge] = CreateHandle(),
                [SelectionHitTestKind.RightEdge] = CreateHandle(),
                [SelectionHitTestKind.BottomEdge] = CreateHandle(),
                [SelectionHitTestKind.TopLeftCorner] = CreateHandle(),
                [SelectionHitTestKind.TopRightCorner] = CreateHandle(),
                [SelectionHitTestKind.BottomLeftCorner] = CreateHandle(),
                [SelectionHitTestKind.BottomRightCorner] = CreateHandle()
            };
        private FunctionBarSurface? _functionBar;
        private AppWindow? _appWindow;
        private nint _handle;
        private double _rasterizationScale = 1;
        private bool _disposed;

        public string DisplayId => _descriptor.DisplayId;

        public PhysicalRect PhysicalBounds => _descriptor.PhysicalBoundsInVirtualDesktop;

        public string CoordinateVersion => _descriptor.CoordinateVersion;

        public double RasterizationScale => _rasterizationScale;

        public OverlaySurface(
            FrozenDisplayOverlayDescriptor descriptor,
            ISelectionInputSink inputSink)
        {
            _descriptor = descriptor;
            _inputSink = inputSink ?? throw new ArgumentNullException(nameof(inputSink));
            _image.Stretch = Stretch.Fill;
            _canvas.IsTabStop = true;
            _canvas.Background = new SolidColorBrush(ColorHelper.FromArgb(0, 0, 0, 0));
            _canvas.Cursor = InputSystemCursor.Create(InputSystemCursorShape.Cross);
            _canvas.PointerPressed += OnPointerPressed;
            _canvas.PointerMoved += OnPointerMoved;
            _canvas.PointerReleased += OnPointerReleased;
            _canvas.KeyDown += OnKeyDown;
            _root.Children.Add(_image);
            _root.Children.Add(_canvas);
            _canvas.Children.Add(_maskTop);
            _canvas.Children.Add(_maskLeft);
            _canvas.Children.Add(_maskRight);
            _canvas.Children.Add(_maskBottom);
            _canvas.Children.Add(_selectionBorder);
            foreach (var handle in _handles.Values)
            {
                _canvas.Children.Add(handle);
            }
        }

        public async ValueTask InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _window.Content = _root;
            _window.Activate();
            var handle = WindowNative.GetWindowHandle(_window);
            if (handle == 0)
            {
                throw new InvalidOperationException("The overlay window handle is unavailable.");
            }

            _handle = handle;
            _ = ShowWindow(handle, SwHide);

            _appWindow = AppWindow.GetFromWindowId(
                Win32Interop.GetWindowIdFromWindow(handle));
            var previousDpiContext = SetThreadDpiAwarenessContext(new nint(PerMonitorAwareV2));
            try
            {
                var presenter = OverlappedPresenter.Create();
                presenter.SetBorderAndTitleBar(false, false);
                presenter.IsAlwaysOnTop = true;
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
                presenter.IsResizable = false;
                _appWindow.SetPresenter(presenter);
                _appWindow.IsShownInSwitchers = false;
                _appWindow.Move(new PointInt32(
                    _descriptor.PhysicalBoundsInVirtualDesktop.Left,
                    _descriptor.PhysicalBoundsInVirtualDesktop.Top));
                _appWindow.Resize(new SizeInt32(
                    _descriptor.PixelSize.Width,
                    _descriptor.PixelSize.Height));
                _appWindow.Hide();
                ApplyToolWindowStyle(handle);
                _rasterizationScale = _root.XamlRoot?.RasterizationScale ?? 1;
                _canvas.Width = _descriptor.PixelSize.Width / _rasterizationScale;
                _canvas.Height = _descriptor.PixelSize.Height / _rasterizationScale;

                if (_descriptor.Frame.FrozenFrame.ImageResult
                    is not SoftwareBitmapImageResult imageResult)
                {
                    throw new InvalidOperationException(
                        "The frozen display frame is not a canonical SoftwareBitmap.");
                }

                await WinUiImagePresentationAdapter.PresentAsync(
                    _image,
                    imageResult,
                    cancellationToken).ConfigureAwait(true);
                ApplySelection(SelectionVisualState.Initial(
                    _descriptor.SessionId,
                    _descriptor.CoordinateVersion));
            }
            finally
            {
                _ = SetThreadDpiAwarenessContext(previousDpiContext);
            }
        }

        public FunctionBarSurface CreateFunctionBar(FunctionBarPresentationRequest request)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(OverlaySurface));
            _functionBar?.Dispose();
            _functionBar = new FunctionBarSurface(this, request);
            _canvas.Children.Add(_functionBar.Root);
            return _functionBar;
        }

        public void RemoveFunctionBar(FunctionBarSurface functionBar)
        {
            if (ReferenceEquals(_functionBar, functionBar))
            {
                _canvas.Children.Remove(functionBar.Root);
                _functionBar = null;
            }
        }

        public bool TryGetPhysicalWorkArea(out PhysicalRect workArea)
        {
            workArea = default;
            var bounds = _descriptor.PhysicalBoundsInVirtualDesktop;
            if (!bounds.IsPositive)
            {
                return false;
            }

            var center = new PointNative(
                bounds.Left + (int)Math.Min(int.MaxValue, bounds.Width64 / 2),
                bounds.Top + (int)Math.Min(int.MaxValue, bounds.Height64 / 2));
            var monitor = MonitorFromPoint(center, MonitorDefaultToNearest);
            if (monitor == 0)
            {
                return false;
            }

            var info = new MonitorInfoNative
            {
                Size = Marshal.SizeOf<MonitorInfoNative>()
            };
            if (!GetMonitorInfo(monitor, ref info))
            {
                return false;
            }

            workArea = new PhysicalRect(
                info.Work.Left,
                info.Work.Top,
                info.Work.Right,
                info.Work.Bottom);
            return workArea.IsPositive;
        }

        public static void ShowAll(IReadOnlyList<OverlaySurface> surfaces)
        {
            ArgumentNullException.ThrowIfNull(surfaces);
            if (surfaces.Count == 0)
            {
                return;
            }

            var deferredPosition = BeginDeferWindowPos(surfaces.Count);
            if (deferredPosition == 0)
            {
                throw new InvalidOperationException(
                    "The overlay windows could not be prepared for an atomic show.");
            }

            try
            {
                foreach (var surface in surfaces)
                {
                    ObjectDisposedException.ThrowIf(surface._disposed, nameof(OverlaySurface));
                    deferredPosition = DeferWindowPos(
                        deferredPosition,
                        surface._handle,
                        0,
                        0,
                        0,
                        0,
                        0,
                        SwpNoSize
                            | SwpNoMove
                            | SwpNoZOrder
                            | SwpNoActivate
                            | SwpShowWindow);
                    if (deferredPosition == 0)
                    {
                        throw new InvalidOperationException(
                            "The overlay windows could not be staged for an atomic show.");
                    }
                }

                if (!EndDeferWindowPos(deferredPosition))
                {
                    throw new InvalidOperationException(
                        "The overlay windows could not be shown as one batch.");
                }

                FocusSessionEscapeOwner(surfaces[0]);
                deferredPosition = 0;
            }
            finally
            {
                if (deferredPosition != 0)
                {
                    _ = EndDeferWindowPos(deferredPosition);
                }
            }
        }

        public void ApplySelection(SelectionVisualState state)
        {
            if (_disposed
                || state.SessionId != _descriptor.SessionId
                || !string.Equals(
                    state.CoordinateVersion,
                    _descriptor.CoordinateVersion,
                    StringComparison.Ordinal))
            {
                return;
            }

            var width = _descriptor.PixelSize.Width / _rasterizationScale;
            var height = _descriptor.PixelSize.Height / _rasterizationScale;
            var bounds = state.NormalizedPhysicalBounds;
            if (bounds is not PhysicalRect selection
                || !selection.Intersects(_descriptor.PhysicalBoundsInVirtualDesktop))
            {
                SetMask(_maskTop, 0, 0, width, height);
                SetMask(_maskLeft, 0, 0, 0, 0);
                SetMask(_maskRight, 0, 0, 0, 0);
                SetMask(_maskBottom, 0, 0, 0, 0);
                _selectionBorder.Visibility = Visibility.Collapsed;
                HideHandles();
            }
            else
            {
                var intersection = selection.Intersection(
                    _descriptor.PhysicalBoundsInVirtualDesktop);
                var left = (intersection.Left - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale;
                var top = (intersection.Top - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale;
                var right = (intersection.Right - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale;
                var bottom = (intersection.Bottom - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale;
                SetMask(_maskTop, 0, 0, width, top);
                SetMask(_maskLeft, 0, top, left, bottom - top);
                SetMask(_maskRight, right, top, width - right, bottom - top);
                SetMask(_maskBottom, 0, bottom, width, height - bottom);
                SetCanvasRectangle(_selectionBorder, left, top, right - left, bottom - top);
                _selectionBorder.Visibility = intersection.IsPositive
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                ApplyHandles(selection, width, height);
            }

            ApplyCursor(state);

        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _functionBar?.Dispose();
            _functionBar = null;
            _canvas.PointerPressed -= OnPointerPressed;
            _canvas.PointerMoved -= OnPointerMoved;
            _canvas.PointerReleased -= OnPointerReleased;
            _canvas.KeyDown -= OnKeyDown;
            _canvas.Cursor = null;
            HideHandles();
            try
            {
                _appWindow?.Hide();
            }
            catch
            {
            }

            try
            {
                _window.Close();
            }
            catch
            {
            }

            try
            {
                _image.Source = null;
                _window.Content = null;
            }
            catch
            {
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            if (_disposed || !TryGetGlobalPointer(out var point))
            {
                return;
            }

            _canvas.Focus(FocusState.Pointer);
            var handle = WindowNative.GetWindowHandle(_window);
            var result = _inputSink.PointerPressed(new SelectionPointerEvent(
                _descriptor.SessionId,
                _descriptor.CoordinateVersion,
                checked((int)args.Pointer.PointerId),
                point));
            if (result.Kind is SelectionInputResultKind.Dragging
                or SelectionInputResultKind.Moving
                or SelectionInputResultKind.Resizing
                or SelectionInputResultKind.Reselecting)
            {
                _ = SetCapture(handle);
            }
            args.Handled = true;
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            if (_disposed || !TryGetGlobalPointer(out var point))
            {
                return;
            }

            _inputSink.PointerMoved(new SelectionPointerEvent(
                _descriptor.SessionId,
                _descriptor.CoordinateVersion,
                checked((int)args.Pointer.PointerId),
                point));
            args.Handled = true;
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            _ = ReleaseCapture();
            if (_disposed || !TryGetGlobalPointer(out var point))
            {
                return;
            }

            _inputSink.PointerReleased(new SelectionPointerEvent(
                _descriptor.SessionId,
                _descriptor.CoordinateVersion,
                checked((int)args.Pointer.PointerId),
                point));
            args.Handled = true;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key == global::Windows.System.VirtualKey.Escape)
            {
                var sessionId = _descriptor.SessionId;
                var coordinateVersion = _descriptor.CoordinateVersion;
                _ = _canvas.DispatcherQueue.TryEnqueue(() =>
                {
                    _ = _inputSink.Escape(sessionId, coordinateVersion);
                });
                args.Handled = true;
            }
        }

        private static bool TryGetGlobalPointer(out PhysicalPoint point)
        {
            if (GetCursorPos(out var cursor))
            {
                point = new PhysicalPoint(cursor.X, cursor.Y);
                return true;
            }

            point = default;
            return false;
        }

        private static Rectangle CreateMask() => new()
        {
            Fill = new SolidColorBrush(ColorHelper.FromArgb(
                OverlayMaskAlpha,
                0,
                0,
                0))
        };

        private static Rectangle CreateHandle() => new()
        {
            Fill = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
            Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 40, 96, 160)),
            StrokeThickness = 1,
            Visibility = Visibility.Collapsed,
            IsHitTestVisible = false
        };

        private void ApplyCursor(SelectionVisualState state)
        {
            var hitTest = state.InteractionMode is
                SelectionInteractionMode.Moving
                or SelectionInteractionMode.ResizingLeft
                or SelectionInteractionMode.ResizingTop
                or SelectionInteractionMode.ResizingRight
                or SelectionInteractionMode.ResizingBottom
                or SelectionInteractionMode.ResizingTopLeft
                or SelectionInteractionMode.ResizingTopRight
                or SelectionInteractionMode.ResizingBottomLeft
                or SelectionInteractionMode.ResizingBottomRight
                ? state.ActiveHitTest
                : state.HoverHitTest;
            _canvas.Cursor = InputSystemCursor.Create(CursorShapeFor(hitTest));
        }

        private static InputSystemCursorShape CursorShapeFor(SelectionHitTestKind hitTest) => hitTest switch
        {
            SelectionHitTestKind.Interior => InputSystemCursorShape.SizeAll,
            SelectionHitTestKind.LeftEdge or SelectionHitTestKind.RightEdge =>
                InputSystemCursorShape.SizeWestEast,
            SelectionHitTestKind.TopEdge or SelectionHitTestKind.BottomEdge =>
                InputSystemCursorShape.SizeNorthSouth,
            SelectionHitTestKind.TopLeftCorner or SelectionHitTestKind.BottomRightCorner =>
                InputSystemCursorShape.SizeNorthwestSoutheast,
            SelectionHitTestKind.TopRightCorner or SelectionHitTestKind.BottomLeftCorner =>
                InputSystemCursorShape.SizeNortheastSouthwest,
            _ => InputSystemCursorShape.Cross
        };

        private void ApplyHandles(
            PhysicalRect selection,
            double canvasWidth,
            double canvasHeight)
        {
            foreach (var pair in _handles)
            {
                if (!TryGetHandleCenter(selection, pair.Key, out var center)
                    || !IsHandleOnDisplay(center, pair.Key))
                {
                    pair.Value.Visibility = Visibility.Collapsed;
                    continue;
                }

                var size = HandleVisualSizePixels / _rasterizationScale;
                var left = (center.X - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale - size / 2;
                var top = (center.Y - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale - size / 2;
                left = Math.Clamp(left, 0, Math.Max(0, canvasWidth - size));
                top = Math.Clamp(top, 0, Math.Max(0, canvasHeight - size));
                SetCanvasRectangle(pair.Value, left, top, size, size);
                pair.Value.Visibility = Visibility.Visible;
            }
        }

        private void HideHandles()
        {
            foreach (var handle in _handles.Values)
            {
                handle.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsHandleOnDisplay(
            PhysicalPoint center,
            SelectionHitTestKind handle)
        {
            var probeX = UsesRightEdge(handle) ? center.X - 1 : center.X;
            var probeY = UsesBottomEdge(handle) ? center.Y - 1 : center.Y;
            var bounds = _descriptor.PhysicalBoundsInVirtualDesktop;
            return probeX >= bounds.Left
                && probeX < bounds.Right
                && probeY >= bounds.Top
                && probeY < bounds.Bottom;
        }

        private static bool TryGetHandleCenter(
            PhysicalRect selection,
            SelectionHitTestKind handle,
            out PhysicalPoint center)
        {
            var middleX = checked((int)(((long)selection.Left + selection.Right) / 2));
            var middleY = checked((int)(((long)selection.Top + selection.Bottom) / 2));
            center = handle switch
            {
                SelectionHitTestKind.LeftEdge => new(selection.Left, middleY),
                SelectionHitTestKind.TopEdge => new(middleX, selection.Top),
                SelectionHitTestKind.RightEdge => new(selection.Right, middleY),
                SelectionHitTestKind.BottomEdge => new(middleX, selection.Bottom),
                SelectionHitTestKind.TopLeftCorner => new(selection.Left, selection.Top),
                SelectionHitTestKind.TopRightCorner => new(selection.Right, selection.Top),
                SelectionHitTestKind.BottomLeftCorner => new(selection.Left, selection.Bottom),
                SelectionHitTestKind.BottomRightCorner => new(selection.Right, selection.Bottom),
                _ => default
            };
            return handle is not SelectionHitTestKind.Outside
                and not SelectionHitTestKind.Interior;
        }

        private static bool UsesRightEdge(SelectionHitTestKind handle) => handle is
            SelectionHitTestKind.RightEdge
            or SelectionHitTestKind.TopRightCorner
            or SelectionHitTestKind.BottomRightCorner;

        private static bool UsesBottomEdge(SelectionHitTestKind handle) => handle is
            SelectionHitTestKind.BottomEdge
            or SelectionHitTestKind.BottomLeftCorner
            or SelectionHitTestKind.BottomRightCorner;

        private sealed class CrosshairCanvas : Canvas
        {
            public InputCursor? Cursor
            {
                get => ProtectedCursor;
                set => ProtectedCursor = value;
            }
        }

        private static void SetMask(
            Rectangle rectangle,
            double left,
            double top,
            double width,
            double height) => SetCanvasRectangle(
            rectangle,
            left,
            top,
            width,
            height);

        private static void SetCanvasRectangle(
            FrameworkElement element,
            double left,
            double top,
            double width,
            double height)
        {
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
            element.Width = Math.Max(0, width);
            element.Height = Math.Max(0, height);
        }

        private static void ApplyToolWindowStyle(nint handle)
        {
            var style = GetWindowLongPtr(handle, GwlExStyle);
            style &= ~WsExAppWindow;
            style |= WsExToolWindow;
            _ = SetWindowLongPtr(handle, GwlExStyle, style);
        }

        private static void FocusSessionEscapeOwner(OverlaySurface surface)
        {
            _ = SetForegroundWindow(surface._handle);
            _ = surface._canvas.Focus(FocusState.Programmatic);
        }

        [DllImport("user32.dll")]
        private static extern nint SetCapture(nint hWnd);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out PointNative lpPoint);

        private const uint MonitorDefaultToNearest = 2;

        [DllImport("user32.dll")]
        private static extern nint MonitorFromPoint(PointNative point, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(
            nint monitor,
            ref MonitorInfoNative monitorInfo);

        [DllImport("user32.dll")]
        private static extern nint SetThreadDpiAwarenessContext(nint dpiContext);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(nint hWnd, int command);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(nint hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint BeginDeferWindowPos(int numberOfWindows);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint DeferWindowPos(
            nint hWinPosInfo,
            nint hWnd,
            nint hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EndDeferWindowPos(nint hWinPosInfo);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        private static extern nint GetWindowLongPtr(nint hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
        private static extern nint SetWindowLongPtr(nint hWnd, int nIndex, nint dwNewLong);

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct PointNative
        {
            public PointNative(int x, int y)
            {
                X = x;
                Y = y;
            }

            public readonly int X;
            public readonly int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonitorInfoNative
        {
            public int Size;
            public RectNative Monitor;
            public RectNative Work;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct RectNative
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }
    }
}
