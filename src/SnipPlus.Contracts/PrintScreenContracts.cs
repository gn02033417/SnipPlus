namespace SnipPlus.Contracts;

public enum PrintScreenTakeoverFailureCode
{
    None,
    RegistrationFailed,
    UnregistrationFailed,
    SettingsLoadFailed,
    SettingsSaveFailed,
    InvalidWindowHandle,
    Disposed
}

public sealed record PrintScreenTakeoverResult(
    bool IsSuccess,
    bool IsRegistered,
    bool StateChanged,
    PrintScreenTakeoverFailureCode FailureCode,
    string UserMessage)
{
    public static PrintScreenTakeoverResult Enabled(bool stateChanged, string message = "PrintScreen takeover enabled.") =>
        new(true, true, stateChanged, PrintScreenTakeoverFailureCode.None, message);

    public static PrintScreenTakeoverResult Disabled(bool stateChanged, string message = "PrintScreen takeover disabled.") =>
        new(true, false, stateChanged, PrintScreenTakeoverFailureCode.None, message);

    public static PrintScreenTakeoverResult Failed(
        PrintScreenTakeoverFailureCode failureCode,
        bool isRegistered,
        string message) =>
        new(false, isRegistered, false, failureCode, message);
}

public sealed class PrintScreenReceivedEventArgs(Guid requestId, DateTimeOffset receivedAt) : EventArgs
{
    public Guid RequestId { get; } = requestId;

    public DateTimeOffset ReceivedAt { get; } = receivedAt;
}

public interface IPrintScreenTakeover : IDisposable
{
    bool IsRegistered { get; }

    event EventHandler<PrintScreenReceivedEventArgs>? PrintScreenReceived;

    PrintScreenTakeoverResult Register();

    PrintScreenTakeoverResult Unregister();
}

public interface IPrintScreenTakeoverSettingsStore
{
    bool LoadEnabled();

    void SaveEnabled(bool enabled);
}
