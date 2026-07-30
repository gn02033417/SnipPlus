using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace SnipPlus.App;

internal static class Program
{
    private const string MainInstanceKey = "SnipPlus.Main";
    private static AppInstance? _mainInstance;
    private static int _applicationExitStarted;

    [STAThread]
    private static int Main(string[] args)
    {
        _ = args;
        WinRT.ComWrappersSupport.InitializeComWrappers();

        var activatedArguments = AppInstance.GetCurrent().GetActivatedEventArgs();
        var instance = AppInstance.FindOrRegisterForKey(MainInstanceKey);
        if (!instance.IsCurrent)
        {
            RedirectActivation(activatedArguments, instance);
            return 0;
        }

        _mainInstance = instance;
        instance.Activated += App.HandleActivated;
        Application.Start(initializationParameters =>
        {
            _ = initializationParameters;
            var context = new DispatcherQueueSynchronizationContext(
                DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            var app = new App();
            GC.KeepAlive(app);
        });

        return 0;
    }

    private static void RedirectActivation(
        AppActivationArguments activatedArguments,
        AppInstance instance)
    {
        using var redirectCompleted = new EventWaitHandle(
            initialState: false,
            mode: EventResetMode.ManualReset);
        var redirectTask = Task.Run(async () =>
        {
            try
            {
                await instance.RedirectActivationToAsync(activatedArguments);
            }
            finally
            {
                redirectCompleted.Set();
            }
        });

        var waitResult = CoWaitForMultipleObjects(
            flags: 0,
            timeout: uint.MaxValue,
            count: 1,
            handles: [redirectCompleted.SafeWaitHandle.DangerousGetHandle()],
            index: out _);
        if (waitResult >= 0x80000000u)
        {
            Marshal.ThrowExceptionForHR(unchecked((int)waitResult));
        }

        redirectTask.GetAwaiter().GetResult();
    }

    [DllImport("ole32.dll")]
    private static extern uint CoWaitForMultipleObjects(
        uint flags,
        uint timeout,
        ulong count,
        IntPtr[] handles,
        out uint index);

    internal static bool IsApplicationExitStarted =>
        Volatile.Read(ref _applicationExitStarted) != 0;

    internal static void BeginApplicationExit()
    {
        if (Interlocked.Exchange(ref _applicationExitStarted, 1) != 0)
        {
            return;
        }

        _mainInstance?.UnregisterKey();
    }
}
