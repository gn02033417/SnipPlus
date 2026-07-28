using Microsoft.Graphics.Canvas;
using SnipPlus.Contracts;
using Windows.Graphics.Capture;
using Windows.Security.Authorization.AppCapabilityAccess;

namespace SnipPlus.Windows;

public sealed class WindowsDisplayCaptureAdapterFactory : IWindowsDisplayCaptureAdapterFactory, IDisposable
{
    private readonly CanvasDevice _canvasDevice;
    private readonly object _gate = new();
    private Task<AppCapabilityAccessStatus>? _accessTask;
    private bool _disposed;

    public WindowsDisplayCaptureAdapterFactory(CanvasDevice canvasDevice)
    {
        _canvasDevice = canvasDevice ?? throw new ArgumentNullException(nameof(canvasDevice));
    }

    public async ValueTask<WindowsDisplayCaptureAdapterCreationOutcome> CreateAsync(
        CaptureSessionContext session,
        DisplaySnapshot display,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(display);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (IsDisposed())
            {
                return new WindowsDisplayCaptureAdapterCreationOutcome.Cancelled("ApplicationExiting");
            }

            if (!WindowsGraphicsCaptureAdapter.IsSupported)
            {
                return Failed(
                    WindowsCaptureIntegrationOutcomeKind.CaptureNotSupported,
                    FailureCode.UnsupportedCapture,
                    FailureCategory.Unsupported,
                    FailureRecoverability.UserActionRequired,
                    session.RequestId,
                    "Windows.Graphics.Capture is not supported.");
            }

            var accessStatus = await GetAccessStatusAsync(cancellationToken).ConfigureAwait(false);
            if (accessStatus != AppCapabilityAccessStatus.Allowed)
            {
                return Failed(
                    WindowsCaptureIntegrationOutcomeKind.CapturePermissionDenied,
                    FailureCode.CapturePermissionDenied,
                    FailureCategory.Permission,
                    FailureRecoverability.UserActionRequired,
                    session.RequestId,
                    "Programmatic Windows.Graphics.Capture access was not allowed.");
            }

            if (!TryParseDisplayId(display.DisplayId, out var displayId))
            {
                return Failed(
                    WindowsCaptureIntegrationOutcomeKind.TopologyInvalid,
                    FailureCode.DisplayTopologyInvalid,
                    FailureCategory.Validation,
                    FailureRecoverability.RetryNewIntent,
                    session.RequestId,
                    "The display snapshot does not contain a valid Windows display identity.");
            }

            var captureItem = GraphicsCaptureItem.TryCreateFromDisplayId(displayId);
            if (captureItem is null)
            {
                return Failed(
                    WindowsCaptureIntegrationOutcomeKind.DisplaySourceUnavailable,
                    FailureCode.CaptureSourceUnavailable,
                    FailureCategory.Device,
                    FailureRecoverability.RetryNewIntent,
                    session.RequestId,
                    "Windows could not create a capture source for the display snapshot.");
            }

            return new WindowsDisplayCaptureAdapterCreationOutcome.Succeeded(
                new WindowsGraphicsCaptureAdapter(
                    _canvasDevice,
                    captureItem,
                    display.DisplayId));
        }
        catch (OperationCanceledException)
        {
            return new WindowsDisplayCaptureAdapterCreationOutcome.Cancelled("CancellationToken");
        }
        catch (UnauthorizedAccessException exception)
        {
            return Failed(
                WindowsCaptureIntegrationOutcomeKind.CapturePermissionDenied,
                FailureCode.CapturePermissionDenied,
                FailureCategory.Permission,
                FailureRecoverability.UserActionRequired,
                session.RequestId,
                exception.GetType().Name,
                exception.HResult);
        }
        catch (Exception exception)
        {
            return Failed(
                WindowsCaptureIntegrationOutcomeKind.UnexpectedFailure,
                FailureCode.UnexpectedFailure,
                FailureCategory.Unexpected,
                FailureRecoverability.RetryNewIntent,
                session.RequestId,
                exception.GetType().Name,
                exception.HResult);
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    private async Task<AppCapabilityAccessStatus> GetAccessStatusAsync(
        CancellationToken cancellationToken)
    {
        Task<AppCapabilityAccessStatus> accessTask;
        lock (_gate)
        {
            if (_disposed)
            {
                throw new OperationCanceledException("ApplicationExiting", cancellationToken);
            }

            _accessTask ??= RequestAccessAsync(CancellationToken.None);
            accessTask = _accessTask;
        }

        return await accessTask.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task<AppCapabilityAccessStatus> RequestAccessAsync(
        CancellationToken cancellationToken)
    {
        return await GraphicsCaptureAccess
            .RequestAccessAsync(GraphicsCaptureAccessKind.Programmatic)
            .AsTask(cancellationToken)
            .WaitAsync(TimeSpan.FromSeconds(3), cancellationToken)
            .ConfigureAwait(false);
    }

    private bool IsDisposed()
    {
        lock (_gate)
        {
            return _disposed;
        }
    }

    private static bool TryParseDisplayId(string displayId, out global::Windows.Graphics.DisplayId parsed)
    {
        parsed = default;
        const string prefix = "display:";
        if (!displayId.StartsWith(prefix, StringComparison.Ordinal)
            || !ulong.TryParse(
                displayId[prefix.Length..],
                System.Globalization.NumberStyles.None,
                System.Globalization.CultureInfo.InvariantCulture,
                out var value))
        {
            return false;
        }

        parsed = new global::Windows.Graphics.DisplayId { Value = value };
        return true;
    }

    private static WindowsDisplayCaptureAdapterCreationOutcome.Failed Failed(
        WindowsCaptureIntegrationOutcomeKind kind,
        FailureCode code,
        FailureCategory category,
        FailureRecoverability recoverability,
        Guid requestId,
        string message,
        int? nativeCode = null) => new WindowsDisplayCaptureAdapterCreationOutcome.Failed(
            WindowsCaptureIntegrationOutcome.FailureResult(
                kind,
                Failure.Create(
                    code,
                    category,
                    recoverability,
                    "WindowsDisplayCaptureAdapterFactory",
                    requestId,
                    message,
                    nativeCode: nativeCode),
                true));
}
