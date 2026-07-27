using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class ResidentLifecycleCoordinator : IDisposable
{
    private readonly object _gate = new();
    private readonly IPrintScreenTakeover _printScreenTakeover;
    private readonly IPrintScreenTakeoverSettingsStore _settingsStore;
    private bool _initialized;
    private bool _takeoverEnabled;
    private bool _disposed;

    public ResidentLifecycleCoordinator(
        IPrintScreenTakeover printScreenTakeover,
        IPrintScreenTakeoverSettingsStore settingsStore)
    {
        _printScreenTakeover = printScreenTakeover ?? throw new ArgumentNullException(nameof(printScreenTakeover));
        _settingsStore = settingsStore ?? throw new ArgumentNullException(nameof(settingsStore));
        _printScreenTakeover.PrintScreenReceived += OnPrintScreenReceived;
    }

    public bool IsTakeoverEnabled
    {
        get
        {
            lock (_gate)
            {
                return _takeoverEnabled;
            }
        }
    }

    public event EventHandler<PrintScreenReceivedEventArgs>? PrintScreenReceived;

    public PrintScreenTakeoverResult Initialize()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return PrintScreenTakeoverResult.Failed(
                    PrintScreenTakeoverFailureCode.Disposed,
                    false,
                    "PrintScreen takeover is unavailable because the application is exiting.");
            }

            if (_initialized)
            {
                return _takeoverEnabled
                    ? PrintScreenTakeoverResult.Enabled(false)
                    : PrintScreenTakeoverResult.Disabled(false);
            }

            _initialized = true;
            bool enabled;
            try
            {
                enabled = _settingsStore.LoadEnabled();
            }
            catch (Exception exception)
            {
                _takeoverEnabled = false;
                return PrintScreenTakeoverResult.Failed(
                    PrintScreenTakeoverFailureCode.SettingsLoadFailed,
                    false,
                    $"Unable to load the PrintScreen takeover setting: {exception.GetType().Name}.");
            }

            return enabled ? EnableCore() : DisableCore();
        }
    }

    public PrintScreenTakeoverResult SetTakeoverEnabled(bool enabled)
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return PrintScreenTakeoverResult.Failed(
                    PrintScreenTakeoverFailureCode.Disposed,
                    false,
                    "PrintScreen takeover is unavailable because the application is exiting.");
            }

            _initialized = true;
            return enabled ? EnableCore() : DisableCore();
        }
    }

    public PrintScreenTakeoverResult ExitApplication()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return PrintScreenTakeoverResult.Disabled(false, "PrintScreen takeover is already released.");
            }

            var release = _printScreenTakeover.IsRegistered
                ? _printScreenTakeover.Unregister()
                : PrintScreenTakeoverResult.Disabled(false, "PrintScreen takeover was already released.");

            _printScreenTakeover.PrintScreenReceived -= OnPrintScreenReceived;
            _printScreenTakeover.Dispose();
            _takeoverEnabled = false;
            _disposed = true;

            return release.IsSuccess
                ? PrintScreenTakeoverResult.Disabled(release.StateChanged, "PrintScreen takeover released before application exit.")
                : release;
        }
    }

    public void Dispose()
    {
        ExitApplication();
        GC.SuppressFinalize(this);
    }

    private PrintScreenTakeoverResult EnableCore()
    {
        if (_takeoverEnabled && _printScreenTakeover.IsRegistered)
        {
            return PrintScreenTakeoverResult.Enabled(false);
        }

        var registration = _printScreenTakeover.Register();
        if (!registration.IsSuccess)
        {
            _takeoverEnabled = false;
            return PersistDisabledAfterFailure(registration);
        }

        try
        {
            _settingsStore.SaveEnabled(true);
        }
        catch (Exception exception)
        {
            _printScreenTakeover.Unregister();
            _takeoverEnabled = false;
            return PrintScreenTakeoverResult.Failed(
                PrintScreenTakeoverFailureCode.SettingsSaveFailed,
                false,
                $"PrintScreen takeover was not enabled because the setting could not be saved: {exception.GetType().Name}.");
        }

        _takeoverEnabled = true;
        return PrintScreenTakeoverResult.Enabled(true);
    }

    private PrintScreenTakeoverResult DisableCore()
    {
        if (_printScreenTakeover.IsRegistered)
        {
            var release = _printScreenTakeover.Unregister();
            if (!release.IsSuccess)
            {
                return release;
            }
        }

        try
        {
            _settingsStore.SaveEnabled(false);
        }
        catch (Exception exception)
        {
            _takeoverEnabled = false;
            return PrintScreenTakeoverResult.Failed(
                PrintScreenTakeoverFailureCode.SettingsSaveFailed,
                false,
                $"PrintScreen takeover is released, but the disabled setting could not be saved: {exception.GetType().Name}.");
        }

        var changed = _takeoverEnabled;
        _takeoverEnabled = false;
        return PrintScreenTakeoverResult.Disabled(changed);
    }

    private PrintScreenTakeoverResult PersistDisabledAfterFailure(PrintScreenTakeoverResult registrationFailure)
    {
        try
        {
            _settingsStore.SaveEnabled(false);
            return registrationFailure with
            {
                IsRegistered = false,
                UserMessage = $"{registrationFailure.UserMessage} Takeover remains disabled."
            };
        }
        catch (Exception exception)
        {
            return PrintScreenTakeoverResult.Failed(
                PrintScreenTakeoverFailureCode.SettingsSaveFailed,
                false,
                $"PrintScreen registration failed and the disabled setting could not be saved: {exception.GetType().Name}.");
        }
    }

    private void OnPrintScreenReceived(object? sender, PrintScreenReceivedEventArgs args)
    {
        EventHandler<PrintScreenReceivedEventArgs>? handler;
        lock (_gate)
        {
            if (_disposed || !_takeoverEnabled)
            {
                return;
            }

            handler = PrintScreenReceived;
        }

        handler?.Invoke(this, args);
    }
}
