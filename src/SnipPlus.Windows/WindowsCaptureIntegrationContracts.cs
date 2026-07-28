using SnipPlus.Contracts;

namespace SnipPlus.Windows;

public enum WindowsCaptureIntegrationOutcomeKind
{
    SupportedAndReady,
    TopologyUnavailable,
    TopologyInvalid,
    UnsupportedCapacity,
    CaptureNotSupported,
    CapturePermissionDenied,
    DisplaySourceUnavailable,
    FrameTimeout,
    FrameSizeMismatch,
    DisplayContextChanged,
    Cancelled,
    StaleSession,
    PartialAcquisitionFailed,
    UnexpectedFailure
}

public sealed record WindowsCaptureIntegrationOutcome(
    WindowsCaptureIntegrationOutcomeKind Kind,
    Failure? Failure,
    bool CleanupCompleted)
{
    public bool IsSuccess => Kind == WindowsCaptureIntegrationOutcomeKind.SupportedAndReady;

    public static WindowsCaptureIntegrationOutcome Success() => new(
        WindowsCaptureIntegrationOutcomeKind.SupportedAndReady,
        null,
        true);

    public static WindowsCaptureIntegrationOutcome FailureResult(
        WindowsCaptureIntegrationOutcomeKind kind,
        Failure failure,
        bool cleanupCompleted) => new(kind, failure, cleanupCompleted);
}

public abstract record WindowsDisplayCaptureAdapterCreationOutcome
{
    private WindowsDisplayCaptureAdapterCreationOutcome()
    {
    }

    public sealed record Succeeded(IWindowsDisplayCaptureAdapter Adapter) : WindowsDisplayCaptureAdapterCreationOutcome;

    public sealed record Cancelled(string CancellationOrigin) : WindowsDisplayCaptureAdapterCreationOutcome;

    public sealed record Failed(WindowsCaptureIntegrationOutcome Outcome) : WindowsDisplayCaptureAdapterCreationOutcome;
}

public abstract record WindowsDisplayCapturePreparationOutcome
{
    private WindowsDisplayCapturePreparationOutcome()
    {
    }

    public sealed record Prepared : WindowsDisplayCapturePreparationOutcome;

    public sealed record Cancelled(string CancellationOrigin) : WindowsDisplayCapturePreparationOutcome;

    public sealed record Failed(WindowsCaptureIntegrationOutcome Outcome) : WindowsDisplayCapturePreparationOutcome;
}

public abstract record WindowsDisplayCaptureStartOutcome
{
    private WindowsDisplayCaptureStartOutcome()
    {
    }

    public sealed record Started : WindowsDisplayCaptureStartOutcome;

    public sealed record Cancelled(string CancellationOrigin) : WindowsDisplayCaptureStartOutcome;

    public sealed record Failed(WindowsCaptureIntegrationOutcome Outcome) : WindowsDisplayCaptureStartOutcome;
}

public abstract record WindowsDisplayCaptureFrameOutcome
{
    private WindowsDisplayCaptureFrameOutcome()
    {
    }

    public sealed record Succeeded(FrozenDisplayFrame Frame) : WindowsDisplayCaptureFrameOutcome;

    public sealed record Cancelled(string CancellationOrigin) : WindowsDisplayCaptureFrameOutcome;

    public sealed record Failed(WindowsCaptureIntegrationOutcome Outcome) : WindowsDisplayCaptureFrameOutcome;
}

public interface IWindowsDisplayCaptureAdapter : IDisposable
{
    string DisplayId { get; }

    ValueTask<WindowsDisplayCapturePreparationOutcome> PrepareAsync(
        CaptureSessionContext session,
        DisplaySnapshot display,
        CancellationToken cancellationToken);

    ValueTask<WindowsDisplayCaptureStartOutcome> StartAsync(
        CancellationToken cancellationToken);

    ValueTask<WindowsDisplayCaptureFrameOutcome> CaptureFirstFrameAsync(
        CancellationToken cancellationToken);
}

public interface IWindowsDisplayCaptureAdapterFactory
{
    ValueTask<WindowsDisplayCaptureAdapterCreationOutcome> CreateAsync(
        CaptureSessionContext session,
        DisplaySnapshot display,
        CancellationToken cancellationToken);
}
