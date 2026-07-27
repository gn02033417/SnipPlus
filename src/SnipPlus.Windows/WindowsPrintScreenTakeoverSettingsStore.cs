using SnipPlus.Contracts;
using Windows.Storage;

namespace SnipPlus.Windows;

public sealed class WindowsPrintScreenTakeoverSettingsStore : IPrintScreenTakeoverSettingsStore
{
    private const string TakeoverEnabledKey = "PrintScreenTakeoverEnabled";

    public bool LoadEnabled()
    {
        var values = ApplicationData.Current.LocalSettings.Values;
        return values.TryGetValue(TakeoverEnabledKey, out var value)
            && value is bool enabled
            && enabled;
    }

    public void SaveEnabled(bool enabled)
    {
        ApplicationData.Current.LocalSettings.Values[TakeoverEnabledKey] = enabled;
    }
}
