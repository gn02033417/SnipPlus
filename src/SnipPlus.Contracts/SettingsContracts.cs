namespace SnipPlus.Contracts;

public enum SettingsLaunchFailureCode
{
    None,
    LaunchFailed
}

public sealed record SettingsLaunchResult(
    bool IsSuccess,
    SettingsLaunchFailureCode FailureCode,
    string UserMessage)
{
    public static SettingsLaunchResult Success(string message = "Windows keyboard settings opened.") =>
        new(true, SettingsLaunchFailureCode.None, message);

    public static SettingsLaunchResult Failed(string message = "Windows keyboard settings could not be opened.") =>
        new(false, SettingsLaunchFailureCode.LaunchFailed, message);
}

public interface ISettingsLauncher
{
    Task<SettingsLaunchResult> OpenKeyboardSettingsAsync();
}

public static class PrintScreenTakeoverCompatibility
{
    public const string Notice =
        "若背景程式中按 PrintScreen 仍開啟 Windows 截圖工具，請關閉 Windows 的「使用 Print Screen 鍵開啟螢幕擷取」設定。";

    public const string OpenKeyboardSettingsLabel = "開啟 Windows 鍵盤設定";

    public const string RegisteredStatus =
        "SnipPlus PrintScreen takeover is registered. Windows native PrintScreen setting may conflict.";
}
