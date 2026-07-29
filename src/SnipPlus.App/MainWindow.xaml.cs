using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using SnipPlus.Contracts;
using SnipPlus.Core;
using SnipPlus.Windows;
using WinRT.Interop;

namespace SnipPlus.App;

public partial class MainWindow : Window, IDisposable
{
    private readonly WorkflowStateAuthority _stateAuthority = new();
    private readonly CaptureRequestCoordinator _captureRequestCoordinator;
    private readonly CaptureRequestApplicationBoundary _captureRequestApplicationBoundary;
    private readonly ResidentLifecycleCoordinator _residentLifecycle;
    private readonly WindowsCapturePlatformResources _platformResources;
    private readonly CapturePresentationWorkflowCoordinator _capturePresentation;
    private readonly ResidentActivationBoundary _residentActivation;
    private readonly ISettingsLauncher _settingsLauncher;
    private bool _updatingTakeoverSetting;
    private int _shutdownStarted;
    private int _disposed;

    public MainWindow(
        ICaptureService? unusedCaptureService = null,
        IPrintScreenTakeover? printScreenTakeover = null,
        IPrintScreenTakeoverSettingsStore? settingsStore = null,
        ISettingsLauncher? settingsLauncher = null)
    {
        _ = unusedCaptureService;
        InitializeComponent();
        _settingsLauncher = settingsLauncher ?? new WindowsSettingsLauncher();
        PrintScreenCompatibilityNoticeText.Text = PrintScreenTakeoverCompatibility.Notice;
        OpenWindowsKeyboardSettingsButton.Content = PrintScreenTakeoverCompatibility.OpenKeyboardSettingsLabel;

        _captureRequestCoordinator = new CaptureRequestCoordinator(_stateAuthority);
        _captureRequestApplicationBoundary =
            new CaptureRequestApplicationBoundary(_captureRequestCoordinator);
        _platformResources = new WindowsCapturePlatformResources(new FunctionBarPlacementService());
        var freezingCoordinator = new CaptureFreezingCoordinator(
            _captureRequestCoordinator,
            _platformResources.TopologyProvider,
            _platformResources.FrameProvider);
        _capturePresentation = new CapturePresentationWorkflowCoordinator(
            freezingCoordinator,
            _platformResources.OverlayCoordinator,
            new WindowsMainWindowCaptureSourceExclusion(this),
            _platformResources.AdapterFactory,
            _platformResources.OverlayCoordinator);
        _residentActivation = new ResidentActivationBoundary(
            isApplicationExiting: () => Volatile.Read(ref _shutdownStarted) != 0
                || Program.IsApplicationExitStarted,
            isCaptureActive: () => _capturePresentation.CurrentState != WorkflowState.ResidentReady,
            showMainWindow: ShowMainWindow);
        _residentLifecycle = new ResidentLifecycleCoordinator(
            printScreenTakeover ?? new WindowsPrintScreenTakeover(),
            settingsStore ?? new WindowsPrintScreenTakeoverSettingsStore());
        _residentLifecycle.PrintScreenReceived += OnPrintScreenReceived;
        Closed += OnClosed;
        ApplyTakeoverResult(_residentLifecycle.Initialize());
    }

    private void StartCaptureButton_Click(object sender, RoutedEventArgs args)
    {
        StartCaptureFromBoundary(_captureRequestApplicationBoundary.SubmitSecondaryInAppCommand(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow));
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

    private async void OpenWindowsKeyboardSettingsButton_Click(object sender, RoutedEventArgs args)
    {
        var result = await _settingsLauncher.OpenKeyboardSettingsAsync();
        SetStatus(result.UserMessage);
    }

    private void OnPrintScreenReceived(object? sender, PrintScreenReceivedEventArgs args)
    {
        if (Volatile.Read(ref _shutdownStarted) != 0)
        {
            return;
        }

        StartCaptureFromBoundary(_captureRequestApplicationBoundary.SubmitPrintScreen(args));
    }

    private void StartCaptureFromBoundary(CaptureRequestResult result)
    {
        SetStatus(result.UserMessage);
        if (!result.IsAccepted)
        {
            return;
        }

        _ = StartCaptureAsync(result.Request);
    }

    private async Task StartCaptureAsync(CaptureRequest request)
    {
        var outcome = await _capturePresentation
            .StartAsync(request, CancellationToken.None)
            .ConfigureAwait(true);
        switch (outcome)
        {
            case CapturePresentationOutcome.SelectingReady:
                SetStatus("Select a region across the displays.");
                break;
            case CapturePresentationOutcome.Busy:
                SetStatus("Capture request rejected because another capture request is already active.");
                break;
            case CapturePresentationOutcome.Cancelled cancelled:
                SetStatus($"Capture cancelled: {cancelled.CancellationOrigin}.");
                break;
            case CapturePresentationOutcome.Failed failed:
                SetStatus($"{failed.Failure.UserMessageKey}: {failed.Failure.DiagnosticMessage}");
                break;
        }
    }

    private void ApplyTakeoverResult(PrintScreenTakeoverResult result)
    {
        _updatingTakeoverSetting = true;
        PrintScreenTakeoverCheckBox.IsChecked = _residentLifecycle.IsTakeoverEnabled;
        _updatingTakeoverSetting = false;
        SetStatus(result.UserMessage);
    }

    private void SetStatus(string text) => StatusText.Text = text;

    internal void HandleExternalActivation()
    {
        _ = _residentActivation.HandleActivation();
    }

    private void ShowMainWindow()
    {
        if (Volatile.Read(ref _shutdownStarted) != 0
            || Volatile.Read(ref _disposed) != 0)
        {
            return;
        }

        var handle = WindowNative.GetWindowHandle(this);
        if (handle != 0)
        {
            _ = ShowWindow(handle, ShowWindowRestore);
        }

        Activate();
        if (handle != 0)
        {
            _ = SetForegroundWindow(handle);
        }
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        if (Interlocked.Exchange(ref _shutdownStarted, 1) != 0)
        {
            return;
        }

        try
        {
            Program.BeginApplicationExit();
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
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _capturePresentation.Dispose();
        _platformResources.Dispose();
        _captureRequestCoordinator.Dispose();
        _residentLifecycle.Dispose();
        GC.SuppressFinalize(this);
    }

    private const int ShowWindowRestore = 9;

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(nint hWnd);
}
