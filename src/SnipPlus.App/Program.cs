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
    private static async Task Main(string[] args)
    {
        _ = args;
        WinRT.ComWrappersSupport.InitializeComWrappers();

        var activatedArguments = AppInstance.GetCurrent().GetActivatedEventArgs();
        var instance = AppInstance.FindOrRegisterForKey(MainInstanceKey);
        if (!instance.IsCurrent)
        {
            await instance.RedirectActivationToAsync(activatedArguments);
            return;
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
    }

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
