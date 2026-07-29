using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace SnipPlus.App;

public partial class App : Application
{
    private static DispatcherQueue? _dispatcherQueue;
    private static int _activationPending;

    public static MainWindow? MainWindow { get; private set; }

    internal static void HandleActivated(object? sender, AppActivationArguments args)
    {
        _ = sender;
        _ = args;
        var dispatcherQueue = _dispatcherQueue;
        if (dispatcherQueue is null
            || !dispatcherQueue.TryEnqueue(DispatchActivation))
        {
            Volatile.Write(ref _activationPending, 1);
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _ = args;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        MainWindow = new MainWindow();
        MainWindow.Activate();

        if (Interlocked.Exchange(ref _activationPending, 0) != 0)
        {
            MainWindow.HandleExternalActivation();
        }
    }

    private static void DispatchActivation()
    {
        if (MainWindow is null)
        {
            Volatile.Write(ref _activationPending, 1);
            return;
        }

        MainWindow.HandleExternalActivation();
    }
}
