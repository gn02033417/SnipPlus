using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using SnipPlus.Contracts;
using SnipPlus.Core;
using SnipPlus.Windows;
using Windows.Foundation;
using WinRT.Interop;

namespace SnipPlus.App;

public partial class MainWindow : Window, IDisposable
{
    private const uint WindowDisplayAffinityNone = 0;
    private const uint WindowDisplayAffinityExcludeFromCapture = 0x11;

    private readonly WorkflowStateAuthority _stateAuthority = new();
    private CancellationTokenSource? _captureCancellation;
    private AppWindow? _appWindow;
    private DisplayArea? _selectionDisplayArea;
    private Point? _selectionStart;
    private double _selectionDpiScale = 1;
    private bool _isSelecting;

    public MainWindow()
    {
        InitializeComponent();
        Activated += OnActivated;
        Closed += OnClosed;
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_appWindow is not null)
        {
            return;
        }

        var windowId = GetWindowId();
        _appWindow = AppWindow.GetFromWindowId(windowId);
    }

    private async void StartCaptureButton_Click(object sender, RoutedEventArgs args)
    {
        if (_isSelecting)
        {
            return;
        }

        try
        {
            _captureCancellation?.Dispose();
            _captureCancellation = new CancellationTokenSource();
            await EnterSelectionModeAsync(_captureCancellation.Token);
        }
        catch (OperationCanceledException)
        {
            LeaveSelectionMode();
            SetStatus("Capture cancelled.");
        }
        catch (Exception exception)
        {
            LeaveSelectionMode();
            SetStatus($"Unable to start capture: {exception.GetType().Name}");
        }
    }

    private async Task EnterSelectionModeAsync(CancellationToken cancellationToken)
    {
        var displayArea = DisplayArea.GetFromWindowId(GetWindowId(), DisplayAreaFallback.Primary)
            ?? throw new InvalidOperationException("No display area is available.");
        _selectionDisplayArea = displayArea;
        _isSelecting = true;
        _selectionStart = null;
        SelectionRectangle.Visibility = Visibility.Collapsed;
        SelectionSurface.Visibility = Visibility.Visible;
        CommandBar.Visibility = Visibility.Collapsed;

        _appWindow ??= AppWindow.GetFromWindowId(GetWindowId());
        _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        await Task.Delay(50, cancellationToken);
        _selectionDpiScale = SelectionCanvas.XamlRoot?.RasterizationScale ?? 1;
        SelectionSurface.Focus(FocusState.Programmatic);
        SetStatus("Select a region.");
    }

    private void SelectionCanvas_PointerPressed(object sender, PointerRoutedEventArgs args)
    {
        if (!_isSelecting)
        {
            return;
        }

        _selectionStart = args.GetCurrentPoint(SelectionCanvas).Position;
        SelectionCanvas.CapturePointer(args.Pointer);
        UpdateSelectionRectangle(_selectionStart.Value, _selectionStart.Value);
        args.Handled = true;
    }

    private void SelectionCanvas_PointerMoved(object sender, PointerRoutedEventArgs args)
    {
        if (_isSelecting && _selectionStart is Point start)
        {
            UpdateSelectionRectangle(start, args.GetCurrentPoint(SelectionCanvas).Position);
            args.Handled = true;
        }
    }

    private void SelectionCanvas_PointerReleased(object sender, PointerRoutedEventArgs args)
    {
        if (!_isSelecting || _selectionStart is not Point start)
        {
            return;
        }

        var end = args.GetCurrentPoint(SelectionCanvas).Position;
        SelectionCanvas.ReleasePointerCapture(args.Pointer);
        _selectionStart = null;
        args.Handled = true;
        _ = CompleteSelectionAsync(CreateDipRect(start, end));
    }

    private async Task CompleteSelectionAsync(DipRect selection)
    {
        _isSelecting = false;
        LeaveSelectionMode();

        var displayArea = _selectionDisplayArea;
        var cancellationToken = _captureCancellation?.Token ?? CancellationToken.None;
        if (displayArea is null)
        {
            SetStatus("No display context is available.");
            return;
        }

        var displayId = displayArea.DisplayId;
        var sourceBounds = displayArea.OuterBounds;
        var displayContext = new DisplayContextSnapshot(
            $"display:{displayId.Value}:scale:{_selectionDpiScale.ToString("0.####", CultureInfo.InvariantCulture)}",
            displayId.Value.ToString(CultureInfo.InvariantCulture),
            new PhysicalRect(
                sourceBounds.X,
                sourceBounds.Y,
                checked(sourceBounds.X + sourceBounds.Width),
                checked(sourceBounds.Y + sourceBounds.Height)),
            _selectionDpiScale,
            _selectionDpiScale);
        var mapping = CoordinateMapper.CreateMonitorIntent(
            displayContext,
            selection,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            cancellationToken);
        if (mapping is CoordinateMappingResult.FailureResult mappingFailure)
        {
            SetStatus(mappingFailure.Failure.UserMessageKey);
            return;
        }

        var intent = ((CoordinateMappingResult.Success)mapping).Intent;
        SetStatus("Capturing…");
        var windowHandle = WindowNative.GetWindowHandle(this);
        SetWindowDisplayAffinity(windowHandle, WindowDisplayAffinityExcludeFromCapture);
        _appWindow?.Hide();

        try
        {
            await Task.Delay(100, cancellationToken);
            using var device = CanvasDevice.GetSharedDevice();
            var captureDisplayId = new global::Windows.Graphics.DisplayId
            {
                Value = displayId.Value
            };
            var captureAdapter = await WindowsGraphicsCaptureAdapter.CreateForDisplayAsync(
                device,
                captureDisplayId,
                cancellationToken);
            if (captureAdapter is null)
            {
                SetStatus("Capture permission or support is unavailable.");
                return;
            }

            var coordinator = new CaptureWorkflowCoordinator(_stateAuthority);
            var result = await coordinator.RunAsync(
                intent,
                captureAdapter,
                new WinRtClipboardDeliveryAdapter(),
                cancellationToken,
                PresentResultAsync);
            if (result.Outcome == WorkflowOutcomeKind.Completed)
            {
                SetStatus("Capture copied to Clipboard.");
            }
            else if (result.Outcome == WorkflowOutcomeKind.RetryableFailure)
            {
                SetStatus("Capture ready; Clipboard is busy. Try again.");
            }
            else if (result.Outcome == WorkflowOutcomeKind.Cancelled)
            {
                SetStatus("Capture cancelled.");
            }
            else
            {
                SetStatus(result.Failure?.UserMessageKey ?? "Capture failed.");
            }
        }
        catch (OperationCanceledException)
        {
            SetStatus("Capture cancelled.");
        }
        catch (Exception exception)
        {
            SetStatus($"Capture failed: {exception.GetType().Name}");
        }
        finally
        {
            SetWindowDisplayAffinity(windowHandle, WindowDisplayAffinityNone);
            _appWindow?.Show();
        }
    }

    private async ValueTask PresentResultAsync(IImageResult imageResult, CancellationToken cancellationToken)
    {
        if (imageResult is not SoftwareBitmapImageResult softwareBitmapResult)
        {
            throw new InvalidOperationException("The capture result is not a canonical SoftwareBitmap.");
        }

        await WinUiImagePresentationAdapter.PresentAsync(ResultImage, softwareBitmapResult, cancellationToken);
    }

    private void CancelSelectionButton_Click(object sender, RoutedEventArgs args)
    {
        _captureCancellation?.Cancel();
        _isSelecting = false;
        LeaveSelectionMode();
        SetStatus("Capture cancelled.");
    }

    private void SelectionSurface_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs args)
    {
        if (args.Key == global::Windows.System.VirtualKey.Escape)
        {
            CancelSelectionButton_Click(sender, args);
            args.Handled = true;
        }
    }

    private void LeaveSelectionMode()
    {
        SelectionSurface.Visibility = Visibility.Collapsed;
        CommandBar.Visibility = Visibility.Visible;
        SelectionRectangle.Visibility = Visibility.Collapsed;
        _appWindow?.SetPresenter(AppWindowPresenterKind.Overlapped);
    }

    private void UpdateSelectionRectangle(Point first, Point second)
    {
        var left = Math.Min(first.X, second.X);
        var top = Math.Min(first.Y, second.Y);
        var width = Math.Abs(first.X - second.X);
        var height = Math.Abs(first.Y - second.Y);
        Canvas.SetLeft(SelectionRectangle, left);
        Canvas.SetTop(SelectionRectangle, top);
        SelectionRectangle.Width = width;
        SelectionRectangle.Height = height;
        SelectionRectangle.Visibility = Visibility.Visible;
    }

    private static DipRect CreateDipRect(Point first, Point second) => new(
        Math.Min(first.X, second.X),
        Math.Min(first.Y, second.Y),
        Math.Max(first.X, second.X),
        Math.Max(first.Y, second.Y));

    private WindowId GetWindowId()
    {
        var windowHandle = WindowNative.GetWindowHandle(this);
        return Win32Interop.GetWindowIdFromWindow(windowHandle);
    }

    private void SetStatus(string text) => StatusText.Text = text;

    private void OnClosed(object sender, WindowEventArgs args)
    {
        Dispose();
    }

    public void Dispose()
    {
        _captureCancellation?.Cancel();
        _captureCancellation?.Dispose();
        _captureCancellation = null;
        GC.SuppressFinalize(this);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowDisplayAffinity(nint hWnd, uint dwAffinity);
}
