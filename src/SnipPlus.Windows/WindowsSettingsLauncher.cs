using SnipPlus.Contracts;

namespace SnipPlus.Windows;

public sealed class WindowsSettingsLauncher : ISettingsLauncher
{
    public const string KeyboardSettingsUri = "ms-settings:easeofaccess-keyboard";

    private readonly Func<Uri, Task<bool>> _launchUriAsync;

    public WindowsSettingsLauncher()
        : this(uri => global::Windows.System.Launcher.LaunchUriAsync(uri).AsTask())
    {
    }

    public WindowsSettingsLauncher(Func<Uri, Task<bool>> launchUriAsync)
    {
        ArgumentNullException.ThrowIfNull(launchUriAsync);
        _launchUriAsync = launchUriAsync;
    }

    public async Task<SettingsLaunchResult> OpenKeyboardSettingsAsync()
    {
        try
        {
            var launched = await _launchUriAsync(new Uri(KeyboardSettingsUri)).ConfigureAwait(false);
            return launched
                ? SettingsLaunchResult.Success()
                : SettingsLaunchResult.Failed();
        }
        catch (Exception exception)
        {
            return SettingsLaunchResult.Failed(
                $"Windows keyboard settings could not be opened: {exception.GetType().Name}.");
        }
    }
}
