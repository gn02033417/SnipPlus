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
    private bool _updatingTakeoverSetting;
    private int _shutdownStarted;
    private int _disposed;

    public MainWindow(
        ICaptureService? unusedCaptureService = null,
        IPrintScreenTakeover? printScreenTakeover = null,
        IPrintScreenTakeoverSettingsStore? settingsStore = null)
    {
        _ = unusedCaptureService;
        InitializeComponent();

        _captureRequestCoordinator = new CaptureRequestCoordinator(_stateAuthority);
        _captureRequestApplicationBoundary =
            new CaptureRequestApplicationBoundary(_captureRequestCoordinator);
        _platformResources = new WindowsCapturePlatformResources();
        var freezingCoordinator = new CaptureFreezingCoordinator(
            _captureRequestCoordinator,
            _platformResources.TopologyProvider,
            _platformResources.FrameProvider);
        _capturePresentation = new CapturePresentationWorkflowCoordinator(
            freezingCoordinator,
            _platformResources.OverlayCoordinator,
            new WindowsMainWindowCaptureSourceExclusion(this));
        _residentLifecycle = new ResidentLifecycleCoordinator(
            printScreenTakeover ?? new WindowsPrintScreenTakeover(WindowNative.GetWindowHandle(this)),
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
                SetStatus(failed.Failure.UserMessageKey);
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
}
