using System.ComponentModel;
using System.Runtime.InteropServices;
using SnipPlus.Contracts;

namespace SnipPlus.Windows;

public sealed class WindowsPrintScreenTakeover : IPrintScreenTakeover
{
    private const int GwlWndProc = -4;
    private const uint VkSnapshot = 0x2C;
    private const uint WmHotKey = 0x0312;
    private const int ErrorHotKeyNotRegistered = 1409;

    private static int _nextHotKeyId;

    private readonly nint _windowHandle;
    private readonly int _hotKeyId = Interlocked.Increment(ref _nextHotKeyId);
    private readonly WindowProcedure _windowProcedure;
    private nint _previousWindowProcedure;
    private bool _windowProcedureInstalled;
    private bool _isRegistered;
    private bool _disposed;

    public WindowsPrintScreenTakeover(nint windowHandle)
    {
        _windowHandle = windowHandle;
        _windowProcedure = HandleWindowMessage;
    }

    public bool IsRegistered => _isRegistered;

    public event EventHandler<PrintScreenReceivedEventArgs>? PrintScreenReceived;

    public PrintScreenTakeoverResult Register()
    {
        if (_disposed)
        {
            return PrintScreenTakeoverResult.Failed(
                PrintScreenTakeoverFailureCode.Disposed,
                false,
                "PrintScreen takeover is unavailable because the application is exiting.");
        }

        if (_isRegistered)
        {
            return PrintScreenTakeoverResult.Enabled(false);
        }

        if (_windowHandle == 0)
        {
            return PrintScreenTakeoverResult.Failed(
                PrintScreenTakeoverFailureCode.InvalidWindowHandle,
                false,
                "PrintScreen takeover could not start because the application window handle is invalid.");
        }

        if (!InstallWindowProcedure(out var installError))
        {
            return PrintScreenTakeoverResult.Failed(
                PrintScreenTakeoverFailureCode.RegistrationFailed,
                false,
                $"PrintScreen takeover could not start: {FormatWin32Error(installError)}.");
        }

        if (!RegisterHotKey(_windowHandle, _hotKeyId, 0, VkSnapshot))
        {
            var error = Marshal.GetLastWin32Error();
            RestoreWindowProcedure();
            return PrintScreenTakeoverResult.Failed(
                PrintScreenTakeoverFailureCode.RegistrationFailed,
                false,
                $"PrintScreen takeover could not start: {FormatWin32Error(error)}.");
        }

        _isRegistered = true;
        return PrintScreenTakeoverResult.Enabled(true);
    }

    public PrintScreenTakeoverResult Unregister()
    {
        if (!_isRegistered)
        {
            RestoreWindowProcedure();
            return PrintScreenTakeoverResult.Disabled(false);
        }

        if (!UnregisterHotKey(_windowHandle, _hotKeyId))
        {
            var error = Marshal.GetLastWin32Error();
            if (error != ErrorHotKeyNotRegistered)
            {
                return PrintScreenTakeoverResult.Failed(
                    PrintScreenTakeoverFailureCode.UnregistrationFailed,
                    true,
                    $"PrintScreen takeover could not be released: {FormatWin32Error(error)}.");
            }
        }

        _isRegistered = false;
        RestoreWindowProcedure();
        return PrintScreenTakeoverResult.Disabled(true);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_isRegistered)
        {
            Unregister();
        }
        else
        {
            RestoreWindowProcedure();
        }

        PrintScreenReceived = null;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private bool InstallWindowProcedure(out int error)
    {
        _previousWindowProcedure = SetWindowLongPtr(
            _windowHandle,
            GwlWndProc,
            Marshal.GetFunctionPointerForDelegate(_windowProcedure));
        if (_previousWindowProcedure != 0)
        {
            _windowProcedureInstalled = true;
            error = 0;
            return true;
        }

        error = Marshal.GetLastWin32Error();
        return false;
    }

    private void RestoreWindowProcedure()
    {
        if (!_windowProcedureInstalled)
        {
            return;
        }

        SetWindowLongPtr(_windowHandle, GwlWndProc, _previousWindowProcedure);
        _previousWindowProcedure = 0;
        _windowProcedureInstalled = false;
    }

    private nint HandleWindowMessage(nint windowHandle, uint message, nint wParam, nint lParam)
    {
        if (message == WmHotKey
            && wParam.ToInt32() == _hotKeyId
            && _isRegistered)
        {
            PrintScreenReceived?.Invoke(
                this,
                new PrintScreenReceivedEventArgs(Guid.NewGuid(), DateTimeOffset.UtcNow));
        }

        return CallWindowProc(_previousWindowProcedure, windowHandle, message, wParam, lParam);
    }

    private static string FormatWin32Error(int error)
    {
        if (error == 0)
        {
            return "an unknown Windows error";
        }

        return new Win32Exception(error).Message;
    }

    private delegate nint WindowProcedure(nint windowHandle, uint message, nint wParam, nint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(nint hWnd, int id);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static extern nint SetWindowLongPtr(nint hWnd, int nIndex, nint newLong);

    [DllImport("user32.dll", EntryPoint = "CallWindowProcW")]
    private static extern nint CallWindowProc(
        nint previousWindowProcedure,
        nint windowHandle,
        uint message,
        nint wParam,
        nint lParam);
}
