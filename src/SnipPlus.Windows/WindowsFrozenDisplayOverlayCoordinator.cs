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
using Windows.Graphics;
using Windows.Graphics.Imaging;
using WinRT.Interop;

namespace SnipPlus.Windows;

public sealed class WindowsFrozenDisplayOverlayCoordinator : IAllDisplayOverlayPresentationCoordinator
{
    private readonly object _gate = new();
    private readonly Dictionary<Guid, IReadOnlyList<OverlaySurface>> _sessions = new();
    private bool _disposed;

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
        private AppWindow? _appWindow;
        private nint _handle;
        private double _rasterizationScale = 1;
        private bool _disposed;

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
            }

        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _canvas.PointerPressed -= OnPointerPressed;
            _canvas.PointerMoved -= OnPointerMoved;
            _canvas.PointerReleased -= OnPointerReleased;
            _canvas.KeyDown -= OnKeyDown;
            _canvas.Cursor = null;
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
            _ = SetCapture(handle);
            _inputSink.PointerPressed(new SelectionPointerEvent(
                _descriptor.SessionId,
                _descriptor.CoordinateVersion,
                checked((int)args.Pointer.PointerId),
                point));
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
            public readonly int X;
            public readonly int Y;
        }
    }
}
