using System.Runtime.InteropServices;

namespace SnipPlus.Windows;

public sealed class WindowsClipboardRuntimeInitializer : IClipboardRuntimeInitializer
{
    private const uint RoInitMultithreaded = 1;
    private static readonly WindowsClipboardRuntimeInitializer _instance = new();

    public static WindowsClipboardRuntimeInitializer Instance => _instance;

    public IDisposable Enter()
    {
        var hresult = RoInitialize(RoInitMultithreaded);
        if (hresult < 0)
        {
            Marshal.ThrowExceptionForHR(hresult);
        }

        return new Scope();
    }

    [DllImport("combase.dll")]
    private static extern int RoInitialize(uint initType);

    [DllImport("combase.dll")]
    private static extern void RoUninitialize();

    private sealed class Scope : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            RoUninitialize();
        }
    }
}
