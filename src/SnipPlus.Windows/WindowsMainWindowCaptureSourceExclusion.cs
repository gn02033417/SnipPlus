using System.Runtime.InteropServices;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SnipPlus.Contracts;
using WinRT.Interop;

namespace SnipPlus.Windows;

public sealed class WindowsMainWindowCaptureSourceExclusion : ICaptureSourceExclusion
{
    private readonly Window _window;

    public WindowsMainWindowCaptureSourceExclusion(Window window)
    {
        _window = window ?? throw new ArgumentNullException(nameof(window));
    }

    public ValueTask<CaptureSourceExclusionOutcome> ExcludeAsync(
        CaptureRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var handle = WindowNative.GetWindowHandle(_window);
            if (handle == 0)
            {
                return ValueTask.FromResult(CaptureSourceExclusionOutcome.Failed(Failure.Create(
                    FailureCode.CaptureSourceUnavailable,
                    FailureCategory.Resource,
                    FailureRecoverability.RetryNewIntent,
                    nameof(WindowsMainWindowCaptureSourceExclusion),
                    request.RequestId,
                    "The MainWindow handle is unavailable.")));
            }

            _ = SetWindowDisplayAffinity(handle, WindowDisplayAffinityExcludeFromCapture);
            _ = ShowWindow(handle, ShowWindowHide);
            if (IsWindowVisible(handle))
            {
                return ValueTask.FromResult(CaptureSourceExclusionOutcome.Failed(Failure.Create(
                    FailureCode.CaptureSourceUnavailable,
                    FailureCategory.Resource,
                    FailureRecoverability.RetryNewIntent,
                    nameof(WindowsMainWindowCaptureSourceExclusion),
                    request.RequestId,
                    "The MainWindow remained visible after the capture exclusion request.")));
            }

            return ValueTask.FromResult(CaptureSourceExclusionOutcome.Hidden());
        }
        catch (OperationCanceledException)
        {
            return ValueTask.FromResult(CaptureSourceExclusionOutcome.Cancelled());
        }
        catch (Exception exception)
        {
            return ValueTask.FromResult(CaptureSourceExclusionOutcome.Failed(Failure.Create(
                FailureCode.CaptureSourceUnavailable,
                FailureCategory.Resource,
                FailureRecoverability.RetryNewIntent,
                nameof(WindowsMainWindowCaptureSourceExclusion),
                request.RequestId,
                exception.GetType().Name,
                nativeCode: exception.HResult)));
        }
    }

    private const uint WindowDisplayAffinityExcludeFromCapture = 0x11;
    private const int ShowWindowHide = 0;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowDisplayAffinity(nint hWnd, uint dwAffinity);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(nint hWnd);
}
