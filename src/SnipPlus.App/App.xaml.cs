using Microsoft.UI.Xaml;

namespace SnipPlus.App;

public partial class App : Application
{
    public static Window? MainWindow { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var useSyntheticCapture = string.Equals(
                Environment.GetEnvironmentVariable("SNIPPLUS_SYNTHETIC_CAPTURE"),
                "1",
                StringComparison.Ordinal)
            || args.Arguments.Contains("--synthetic", StringComparison.OrdinalIgnoreCase)
            || File.Exists(Path.Combine(AppContext.BaseDirectory, "synthetic-runtime.fixture"));
        MainWindow = new MainWindow(useSyntheticCapture ? new SyntheticCaptureService() : null);
        MainWindow.Activate();
    }
}
