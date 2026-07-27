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
    private readonly CaptureWorkflowCoordinator _workflowCoordinator;
    private readonly ResidentLifecycleCoordinator _residentLifecycle;
    private readonly ICaptureService? _injectedCaptureService;
    private CancellationTokenSource? _captureCancellation;
    private AppWindow? _appWindow;
    private CanvasDevice? _canvasDevice;
    private ICaptureService? _captureService;
    private FrozenCaptureFrame? _frozenFrame;
    private DisplayContextSnapshot? _selectionDisplayContext;
    private DisplayArea? _selectionDisplayArea;
    private Point? _selectionStart;
    private double _selectionDpiScale = 1;
    private bool _isSelecting;
    private bool _updatingTakeoverSetting;
    private int _shutdownStarted;

    public MainWindow(
        ICaptureService? captureService = null,
        IPrintScreenTakeover? printScreenTakeover = null,
        IPrintScreenTakeoverSettingsStore? settingsStore = null)
    {
        InitializeComponent();
        _workflowCoordinator = new CaptureWorkflowCoordinator(_stateAuthority);
        _injectedCaptureService = captureService;
        _residentLifecycle = new ResidentLifecycleCoordinator(
            printScreenTakeover ?? new WindowsPrintScreenTakeover(WindowNative.GetWindowHandle(this)),
            settingsStore ?? new WindowsPrintScreenTakeoverSettingsStore());
        _residentLifecycle.PrintScreenReceived += OnPrintScreenReceived;
        Activated += OnActivated;
        Closed += OnClosed;
        ApplyTakeoverResult(_residentLifecycle.Initialize());
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
            await BeginCaptureAsync(_captureCancellation.Token);
        }
        catch (OperationCanceledException)
        {
            LeaveSelectionMode();
            if (_stateAuthority.CurrentState == WorkflowState.Selecting)
            {
                _workflowCoordinator.CancelSelection(Guid.NewGuid(), _frozenFrame);
            }
            DisposeCaptureSession();
            SetStatus("Capture cancelled.");
        }
        catch (Exception exception)
        {
            LeaveSelectionMode();
            if (_stateAuthority.CurrentState == WorkflowState.Selecting)
            {
                _workflowCoordinator.CancelSelection(Guid.NewGuid(), _frozenFrame);
            }
            DisposeCaptureSession();
            SetStatus($"Unable to start capture: {exception.GetType().Name}");
        }
    }

    private void PrintScreenTakeoverCheckBox_Click(object sender, RoutedEventArgs args)
    {
        if (_updatingTakeoverSetting)
        {
            return;
        }

        var requestedState = PrintScreenTakeoverCheckBox.IsChecked == true;
        ApplyTakeoverResult(_residentLifecycle.SetTakeoverEnabled(requestedState));
    }

    private void OnPrintScreenReceived(object? sender, PrintScreenReceivedEventArgs args)
    {
        SetStatus("PrintScreen received. Capture workflow is not started in this slice.");
    }

    private void ApplyTakeoverResult(PrintScreenTakeoverResult result)
    {
        _updatingTakeoverSetting = true;
        PrintScreenTakeoverCheckBox.IsChecked = _residentLifecycle.IsTakeoverEnabled;
        _updatingTakeoverSetting = false;
        SetStatus(result.UserMessage);
    }

    private async Task BeginCaptureAsync(CancellationToken cancellationToken)
    {
        var displayArea = DisplayArea.GetFromWindowId(GetWindowId(), DisplayAreaFallback.Primary)
            ?? throw new InvalidOperationException("No display area is available.");
        _selectionDisplayArea = displayArea;
        _appWindow ??= AppWindow.GetFromWindowId(GetWindowId());
        _selectionDpiScale = SelectionCanvas.XamlRoot?.RasterizationScale ?? 1;
        _selectionDisplayContext = CreateDisplayContext(displayArea, _selectionDpiScale);

        var fullFrameIntentResult = CoordinateMapper.CreateMonitorIntent(
            _selectionDisplayContext,
            new DipRect(
                0,
                0,
                _selectionDisplayContext.SourcePhysicalBounds.Width / _selectionDpiScale,
                _selectionDisplayContext.SourcePhysicalBounds.Height / _selectionDpiScale),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            cancellationToken);
        if (fullFrameIntentResult is CoordinateMappingResult.FailureResult fullFrameFailure)
        {
            SetStatus(fullFrameFailure.Failure.UserMessageKey);
            return;
        }

        _captureService = _injectedCaptureService;
        if (_captureService is null)
        {
            _canvasDevice = CanvasDevice.GetSharedDevice();
            _captureService = await WindowsGraphicsCaptureAdapter.CreateForDisplayAsync(
                _canvasDevice,
                new global::Windows.Graphics.DisplayId { Value = displayArea.DisplayId.Value },
                cancellationToken);
        }

        if (_captureService is null)
        {
            DisposeCaptureSession();
            SetStatus("Capture permission or support is unavailable.");
            return;
        }

        var windowHandle = WindowNative.GetWindowHandle(this);
        SetWindowDisplayAffinity(windowHandle, WindowDisplayAffinityExcludeFromCapture);
        _appWindow.Hide();
        try
        {
            await Task.Delay(100, cancellationToken);
            var frameOutcome = await _workflowCoordinator.BeginSelectionAsync(
            ((CoordinateMappingResult.Success)fullFrameIntentResult).Intent,
            _captureService,
            cancellationToken);
            switch (frameOutcome)
            {
                case CaptureFrameOutcome.Succeeded succeeded:
                    if (succeeded.FrozenFrame.ImageResult.Metadata.PixelWidth
                            != _selectionDisplayContext.SourcePhysicalBounds.Width
                        || succeeded.FrozenFrame.ImageResult.Metadata.PixelHeight
                            != _selectionDisplayContext.SourcePhysicalBounds.Height)
                    {
                        _workflowCoordinator.CancelSelection(Guid.NewGuid(), succeeded.FrozenFrame);
                        DisposeCaptureSession();
                        SetStatus("Capture frame size does not match the display context.");
                        return;
                    }

                    _frozenFrame = succeeded.FrozenFrame;
                    break;
                case CaptureFrameOutcome.Cancelled:
                    DisposeCaptureSession();
                    SetStatus("Capture cancelled.");
                    return;
                case CaptureFrameOutcome.Failed failed:
                    DisposeCaptureSession();
                    SetStatus(failed.Failure.UserMessageKey);
                    return;
                default:
                    DisposeCaptureSession();
                    SetStatus("Capture failed.");
                    return;
            }
        }
        finally
        {
            SetWindowDisplayAffinity(windowHandle, WindowDisplayAffinityNone);
            _appWindow.Show();
        }

        _isSelecting = true;
        _selectionStart = null;
        SelectionRectangle.Visibility = Visibility.Collapsed;
        SelectionSurface.Visibility = Visibility.Visible;
        CommandBar.Visibility = Visibility.Collapsed;
        _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        await Task.Delay(50, cancellationToken);
        await WinUiImagePresentationAdapter.PresentAsync(
            SelectionFrameImage,
            (SoftwareBitmapImageResult)_frozenFrame.ImageResult,
            cancellationToken);
        UpdateSelectionMasks(null);
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

        var displayContext = _selectionDisplayContext;
        var frozenFrame = _frozenFrame;
        var captureService = _captureService;
        var cancellationToken = _captureCancellation?.Token ?? CancellationToken.None;
        if (displayContext is null || frozenFrame is null || captureService is null)
        {
            _workflowCoordinator.CancelSelection(Guid.NewGuid(), frozenFrame);
            _frozenFrame = null;
            DisposeCaptureSession();
            SetStatus("No frozen capture frame is available.");
            return;
        }

        var mapping = CoordinateMapper.CreateMonitorIntent(
            displayContext,
            selection,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            cancellationToken);
        if (mapping is CoordinateMappingResult.FailureResult mappingFailure)
        {
            _workflowCoordinator.CancelSelection(Guid.NewGuid(), frozenFrame);
            _frozenFrame = null;
            DisposeCaptureSession();
            SetStatus(mappingFailure.Failure.UserMessageKey);
            return;
        }

        var intent = ((CoordinateMappingResult.Success)mapping).Intent;
        try
        {
            SetStatus("Preparing result…");
            var result = await _workflowCoordinator.CompleteSelectionAsync(
                intent,
                frozenFrame,
                captureService,
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
            _frozenFrame = null;
            DisposeCaptureSession();
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
        _workflowCoordinator.CancelSelection(Guid.NewGuid(), _frozenFrame);
        _frozenFrame = null;
        DisposeCaptureSession();
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
        SelectionFrameImage.Source = null;
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
        UpdateSelectionMasks(new DipRect(left, top, left + width, top + height));
    }

    private void UpdateSelectionMasks(DipRect? selection)
    {
        var canvasWidth = SelectionCanvas.ActualWidth;
        var canvasHeight = SelectionCanvas.ActualHeight;
        if (canvasWidth <= 0 || canvasHeight <= 0)
        {
            return;
        }

        var bounds = selection ?? new DipRect(0, 0, 0, 0);
        SetCanvasRectangle(SelectionMaskTop, 0, 0, canvasWidth, bounds.IsPositive ? bounds.Top : canvasHeight);
        SetCanvasRectangle(
            SelectionMaskLeft,
            0,
            bounds.Top,
            bounds.IsPositive ? bounds.Left : 0,
            bounds.IsPositive ? bounds.Height : 0);
        SetCanvasRectangle(
            SelectionMaskRight,
            bounds.IsPositive ? bounds.Right : 0,
            bounds.Top,
            bounds.IsPositive ? Math.Max(0, canvasWidth - bounds.Right) : 0,
            bounds.IsPositive ? bounds.Height : 0);
        SetCanvasRectangle(
            SelectionMaskBottom,
            0,
            bounds.IsPositive ? bounds.Bottom : 0,
            canvasWidth,
            bounds.IsPositive ? Math.Max(0, canvasHeight - bounds.Bottom) : 0);
    }

    private static void SetCanvasRectangle(
        Rectangle rectangle,
        double left,
        double top,
        double width,
        double height)
    {
        Canvas.SetLeft(rectangle, left);
        Canvas.SetTop(rectangle, top);
        rectangle.Width = Math.Max(0, width);
        rectangle.Height = Math.Max(0, height);
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

    private static DisplayContextSnapshot CreateDisplayContext(
        DisplayArea displayArea,
        double dpiScale)
    {
        var displayId = displayArea.DisplayId;
        var sourceBounds = displayArea.OuterBounds;
        return new DisplayContextSnapshot(
            $"display:{displayId.Value}:scale:{dpiScale.ToString("0.####", CultureInfo.InvariantCulture)}",
            displayId.Value.ToString(CultureInfo.InvariantCulture),
            new PhysicalRect(
                sourceBounds.X,
                sourceBounds.Y,
                checked(sourceBounds.X + sourceBounds.Width),
                checked(sourceBounds.Y + sourceBounds.Height)),
            dpiScale,
            dpiScale);
    }

    private void DisposeCaptureSession()
    {
        _frozenFrame?.Dispose();
        _frozenFrame = null;
        if (_captureService is IDisposable disposableCaptureService)
        {
            disposableCaptureService.Dispose();
        }

        _captureService = null;
        _canvasDevice?.Dispose();
        _canvasDevice = null;
        _selectionDisplayContext = null;
        _selectionDisplayArea = null;
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        if (Interlocked.Exchange(ref _shutdownStarted, 1) != 0)
        {
            return;
        }

        try
        {
            _residentLifecycle.ExitApplication();
        }
        finally
        {
            Dispose();
            Environment.Exit(0);
        }
    }

    public void Dispose()
    {
        _captureCancellation?.Cancel();
        if (_stateAuthority.CurrentState == WorkflowState.Selecting)
        {
            _workflowCoordinator.CancelSelection(Guid.NewGuid(), _frozenFrame);
        }
        _captureCancellation?.Dispose();
        _captureCancellation = null;
        DisposeCaptureSession();
        _residentLifecycle.Dispose();
        GC.SuppressFinalize(this);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowDisplayAffinity(nint hWnd, uint dwAffinity);
}
