using SnipPlus.Contracts;

namespace SnipPlus.Windows;

public sealed class WindowsFrozenDisplayFrameSetProvider : IAllDisplayFrameProvider, IDisposable
{
    private readonly object _gate = new();
    private readonly IWindowsDisplayCaptureAdapterFactory _adapterFactory;
    private readonly IWindowsDisplayTopologyRevisionSource? _revisionSource;
    private readonly TimeSpan _frameTimeout;
    private List<IWindowsDisplayCaptureAdapter> _activeAdapters = new();
    private bool _disposed;

    public WindowsFrozenDisplayFrameSetProvider(
        IWindowsDisplayCaptureAdapterFactory adapterFactory,
        IWindowsDisplayTopologyRevisionSource? revisionSource = null,
        TimeSpan? frameTimeout = null)
    {
        _adapterFactory = adapterFactory ?? throw new ArgumentNullException(nameof(adapterFactory));
        _revisionSource = revisionSource;
        _frameTimeout = frameTimeout ?? TimeSpan.FromSeconds(2);
        if (_frameTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(frameTimeout));
        }
    }

    public ValueTask<FrozenDisplayFrameAcquisitionOutcome> AcquireAsync(
        CaptureSessionContext session,
        DisplaySnapshot display,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(display);
        return ValueTask.FromResult<FrozenDisplayFrameAcquisitionOutcome>(new FrozenDisplayFrameAcquisitionOutcome.Failed(
            CreateFailure(
                FailureCode.PartialAcquisitionFailed,
                FailureCategory.Session,
                FailureRecoverability.RetryNewIntent,
                session.RequestId,
                "The Windows all-display provider requires AcquireAllAsync.")));
    }

    public async ValueTask<FrozenDisplayFrameSetAcquisitionOutcome> AcquireAllAsync(
        CaptureSessionContext session,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (IsDisposed())
        {
            return Failed(
                WindowsCaptureIntegrationOutcomeKind.Cancelled,
                FailureCode.Cancelled,
                FailureCategory.Cancelled,
                FailureRecoverability.RetryNewIntent,
                session.RequestId,
                "The Windows display frame provider has been disposed.");
        }

        if (session.IsDisposed || session.Status != CaptureSessionStatus.Freezing)
        {
            return Failed(
                WindowsCaptureIntegrationOutcomeKind.StaleSession,
                FailureCode.StaleSession,
                FailureCategory.Session,
                FailureRecoverability.RetryNewIntent,
                session.RequestId,
                "The capture session is no longer in the freezing state.");
        }

        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            session.Cancellation,
            cancellationToken);
        var adapters = new List<IWindowsDisplayCaptureAdapter>(
            session.VirtualDesktopSnapshot.Displays.Count);
        try
        {
            foreach (var display in session.VirtualDesktopSnapshot.Displays)
            {
                linkedCancellation.Token.ThrowIfCancellationRequested();
                var creation = await _adapterFactory
                    .CreateAsync(session, display, linkedCancellation.Token)
                    .ConfigureAwait(false);
                switch (creation)
                {
                    case WindowsDisplayCaptureAdapterCreationOutcome.Succeeded succeeded:
                        adapters.Add(succeeded.Adapter);
                        break;
                    case WindowsDisplayCaptureAdapterCreationOutcome.Cancelled cancelled:
                        return Cancelled(session, cancelled.CancellationOrigin, adapters);
                    case WindowsDisplayCaptureAdapterCreationOutcome.Failed failed:
                        return FailedWithCleanup(session, failed.Outcome, adapters);
                    default:
                        return FailedWithCleanup(
                            session,
                            UnexpectedOutcome(session.RequestId, "The Windows display adapter factory returned an unknown outcome."),
                            adapters);
                }
            }

            SetActiveAdapters(adapters);

            foreach (var pair in adapters.Zip(session.VirtualDesktopSnapshot.Displays))
            {
                var preparation = await pair.First
                    .PrepareAsync(session, pair.Second, linkedCancellation.Token)
                    .ConfigureAwait(false);
                switch (preparation)
                {
                    case WindowsDisplayCapturePreparationOutcome.Prepared:
                        break;
                    case WindowsDisplayCapturePreparationOutcome.Cancelled cancelled:
                        return Cancelled(session, cancelled.CancellationOrigin, adapters);
                    case WindowsDisplayCapturePreparationOutcome.Failed failed:
                        return FailedWithCleanup(session, failed.Outcome, adapters);
                    default:
                        return FailedWithCleanup(
                            session,
                            UnexpectedOutcome(session.RequestId, "The Windows display adapter returned an unknown preparation outcome."),
                            adapters);
                }
            }

            foreach (var adapter in adapters)
            {
                var start = await adapter
                    .StartAsync(linkedCancellation.Token)
                    .ConfigureAwait(false);
                switch (start)
                {
                    case WindowsDisplayCaptureStartOutcome.Started:
                        break;
                    case WindowsDisplayCaptureStartOutcome.Cancelled cancelled:
                        return Cancelled(session, cancelled.CancellationOrigin, adapters);
                    case WindowsDisplayCaptureStartOutcome.Failed failed:
                        return FailedWithCleanup(session, failed.Outcome, adapters);
                    default:
                        return FailedWithCleanup(
                            session,
                            UnexpectedOutcome(session.RequestId, "The Windows display adapter returned an unknown start outcome."),
                            adapters);
                }
            }

            var captureTasks = adapters
                .Select(adapter => adapter.CaptureFirstFrameAsync(linkedCancellation.Token).AsTask())
                .ToArray();
            WindowsDisplayCaptureFrameOutcome[] frameOutcomes;
            try
            {
                frameOutcomes = await Task.WhenAll(captureTasks)
                    .WaitAsync(_frameTimeout, linkedCancellation.Token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                DisposeLateFrameResults(captureTasks);
                return Cancelled(session, "CancellationToken", adapters);
            }
            catch (TimeoutException)
            {
                DisposeLateFrameResults(captureTasks);
                return FailedWithCleanup(
                    session,
                    FailureOutcome(
                        WindowsCaptureIntegrationOutcomeKind.FrameTimeout,
                        FailureCode.CaptureFrameTimeout,
                        FailureCategory.Device,
                        FailureRecoverability.RetryNewIntent,
                        session.RequestId,
                        "Not every display produced a frame before the bounded timeout."),
                    adapters);
            }
            catch (Exception exception)
            {
                DisposeLateFrameResults(captureTasks);
                return FailedWithCleanup(
                    session,
                    FailureOutcome(
                        WindowsCaptureIntegrationOutcomeKind.UnexpectedFailure,
                        FailureCode.UnexpectedFailure,
                        FailureCategory.Unexpected,
                        FailureRecoverability.RetryNewIntent,
                        session.RequestId,
                        exception.GetType().Name,
                        exception.HResult),
                    adapters);
            }

            if (session.IsDisposed || session.Status != CaptureSessionStatus.Freezing)
            {
                DisposeSuccessfulFrames(frameOutcomes);
                return FailedWithCleanup(
                    session,
                    FailureOutcome(
                        WindowsCaptureIntegrationOutcomeKind.StaleSession,
                        FailureCode.StaleSession,
                        FailureCategory.Session,
                        FailureRecoverability.RetryNewIntent,
                        session.RequestId,
                        "The capture session changed while frames were arriving."),
                    adapters);
            }

            var cancelledOutcome = frameOutcomes
                .OfType<WindowsDisplayCaptureFrameOutcome.Cancelled>()
                .FirstOrDefault();
            if (cancelledOutcome is not null)
            {
                DisposeSuccessfulFrames(frameOutcomes);
                return Cancelled(session, cancelledOutcome.CancellationOrigin, adapters);
            }

            var failedOutcome = frameOutcomes
                .OfType<WindowsDisplayCaptureFrameOutcome.Failed>()
                .FirstOrDefault();
            if (failedOutcome is not null)
            {
                DisposeSuccessfulFrames(frameOutcomes);
                return FailedWithCleanup(session, ToPartialFailure(session, failedOutcome.Outcome), adapters);
            }

            var frames = frameOutcomes
                .OfType<WindowsDisplayCaptureFrameOutcome.Succeeded>()
                .Select(outcome => outcome.Frame)
                .ToArray();

            if (_revisionSource is not null)
            {
                string? currentCoordinateVersion;
                try
                {
                    currentCoordinateVersion = await _revisionSource
                        .GetCurrentCoordinateVersionAsync(linkedCancellation.Token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    DisposeFrames(frames);
                    return Cancelled(session, "CancellationToken", adapters);
                }
                catch (Exception exception)
                {
                    DisposeFrames(frames);
                    return FailedWithCleanup(
                        session,
                        FailureOutcome(
                            WindowsCaptureIntegrationOutcomeKind.DisplayContextChanged,
                            FailureCode.DisplayContextChanged,
                            FailureCategory.Session,
                            FailureRecoverability.RetryNewIntent,
                            session.RequestId,
                            exception.GetType().Name,
                            exception.HResult),
                        adapters);
                }

                if (!string.Equals(
                        currentCoordinateVersion,
                        session.VirtualDesktopSnapshot.CoordinateVersion,
                        StringComparison.Ordinal))
                {
                    DisposeFrames(frames);
                    return FailedWithCleanup(
                        session,
                        FailureOutcome(
                            WindowsCaptureIntegrationOutcomeKind.DisplayContextChanged,
                            FailureCode.DisplayContextChanged,
                            FailureCategory.Session,
                            FailureRecoverability.RetryNewIntent,
                            session.RequestId,
                            "The display topology changed while frames were being acquired."),
                        adapters);
                }
            }

            if (!FrozenDisplayFrameSet.TryCreate(
                    session,
                    session.VirtualDesktopSnapshot.Displays,
                    frames,
                    out var frameSet,
                    out var validation)
                || frameSet is null)
            {
                return FailedWithCleanup(
                    session,
                    FailureOutcome(
                        WindowsCaptureIntegrationOutcomeKind.PartialAcquisitionFailed,
                        FailureCode.PartialAcquisitionFailed,
                        FailureCategory.Validation,
                        FailureRecoverability.RetryNewIntent,
                        session.RequestId,
                        validation.Message),
                    adapters);
            }

            DisposeAdapters(adapters);
            ClearActiveAdapters(adapters);
            return new FrozenDisplayFrameSetAcquisitionOutcome.Succeeded(frameSet);
        }
        catch (OperationCanceledException)
        {
            return Cancelled(session, "CancellationToken", adapters);
        }
        catch (Exception exception)
        {
            return FailedWithCleanup(
                session,
                FailureOutcome(
                    WindowsCaptureIntegrationOutcomeKind.UnexpectedFailure,
                    FailureCode.UnexpectedFailure,
                    FailureCategory.Unexpected,
                    FailureRecoverability.RetryNewIntent,
                    session.RequestId,
                    exception.GetType().Name,
                    exception.HResult),
                adapters);
        }
    }

    public void Dispose()
    {
        List<IWindowsDisplayCaptureAdapter> adapters;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            adapters = _activeAdapters;
            _activeAdapters = new List<IWindowsDisplayCaptureAdapter>();
        }

        DisposeAdapters(adapters);
        GC.SuppressFinalize(this);
    }

    private bool IsDisposed()
    {
        lock (_gate)
        {
            return _disposed;
        }
    }

    private void SetActiveAdapters(List<IWindowsDisplayCaptureAdapter> adapters)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(WindowsFrozenDisplayFrameSetProvider));

            _activeAdapters = adapters;
        }
    }

    private void ClearActiveAdapters(IReadOnlyCollection<IWindowsDisplayCaptureAdapter> adapters)
    {
        lock (_gate)
        {
            if (ReferenceEquals(_activeAdapters, adapters))
            {
                _activeAdapters = new List<IWindowsDisplayCaptureAdapter>();
            }
        }
    }

    private FrozenDisplayFrameSetAcquisitionOutcome.Cancelled Cancelled(
        CaptureSessionContext session,
        string cancellationOrigin,
        IReadOnlyCollection<IWindowsDisplayCaptureAdapter> adapters)
    {
        DisposeAdapters(adapters);
        ClearActiveAdapters(adapters);
        return new FrozenDisplayFrameSetAcquisitionOutcome.Cancelled(cancellationOrigin, true);
    }

    private FrozenDisplayFrameSetAcquisitionOutcome.Failed FailedWithCleanup(
        CaptureSessionContext session,
        WindowsCaptureIntegrationOutcome outcome,
        IReadOnlyCollection<IWindowsDisplayCaptureAdapter> adapters)
    {
        DisposeAdapters(adapters);
        ClearActiveAdapters(adapters);
        var failure = outcome.Failure ?? CreateFailure(
            FailureCode.PartialAcquisitionFailed,
            FailureCategory.Resource,
            FailureRecoverability.RetryNewIntent,
            session.RequestId,
            "Windows display capture returned a failure without details.");
        return new FrozenDisplayFrameSetAcquisitionOutcome.Failed(failure, true);
    }

    private static FrozenDisplayFrameSetAcquisitionOutcome.Failed Failed(
        WindowsCaptureIntegrationOutcomeKind kind,
        FailureCode code,
        FailureCategory category,
        FailureRecoverability recoverability,
        Guid requestId,
        string message) => new FrozenDisplayFrameSetAcquisitionOutcome.Failed(
            CreateFailure(code, category, recoverability, requestId, message),
            true);

    private static WindowsCaptureIntegrationOutcome ToPartialFailure(
        CaptureSessionContext session,
        WindowsCaptureIntegrationOutcome outcome)
    {
        var innerFailure = outcome.Failure;
        var failure = CreateFailure(
            FailureCode.PartialAcquisitionFailed,
            FailureCategory.Resource,
            FailureRecoverability.RetryNewIntent,
            session.RequestId,
            "At least one display failed while acquiring the complete frozen frame set.",
            innerFailure: innerFailure);
        return WindowsCaptureIntegrationOutcome.FailureResult(
            WindowsCaptureIntegrationOutcomeKind.PartialAcquisitionFailed,
            failure,
            outcome.CleanupCompleted);
    }

    private static WindowsCaptureIntegrationOutcome UnexpectedOutcome(Guid requestId, string message) =>
        FailureOutcome(
            WindowsCaptureIntegrationOutcomeKind.UnexpectedFailure,
            FailureCode.UnexpectedFailure,
            FailureCategory.Unexpected,
            FailureRecoverability.RetryNewIntent,
            requestId,
            message);

    private static WindowsCaptureIntegrationOutcome FailureOutcome(
        WindowsCaptureIntegrationOutcomeKind kind,
        FailureCode code,
        FailureCategory category,
        FailureRecoverability recoverability,
        Guid requestId,
        string message,
        int? nativeCode = null,
        Failure? innerFailure = null) => WindowsCaptureIntegrationOutcome.FailureResult(
            kind,
            CreateFailure(code, category, recoverability, requestId, message, nativeCode, innerFailure),
            true);

    private static Failure CreateFailure(
        FailureCode code,
        FailureCategory category,
        FailureRecoverability recoverability,
        Guid requestId,
        string message,
        int? nativeCode = null,
        Failure? innerFailure = null) => Failure.Create(
            code,
            category,
            recoverability,
            "WindowsFrozenDisplayFrameSetProvider",
            requestId,
            message,
            nativeCode: nativeCode,
            innerFailure: innerFailure);

    private static void DisposeSuccessfulFrames(IEnumerable<WindowsDisplayCaptureFrameOutcome> outcomes) =>
        DisposeFrames(outcomes
            .OfType<WindowsDisplayCaptureFrameOutcome.Succeeded>()
            .Select(outcome => outcome.Frame));

    private static void DisposeFrames(IEnumerable<FrozenDisplayFrame> frames)
    {
        foreach (var frame in frames)
        {
            frame.Dispose();
        }
    }

    private static void DisposeAdapters(IEnumerable<IWindowsDisplayCaptureAdapter> adapters)
    {
        foreach (var adapter in adapters.Reverse())
        {
            adapter.Dispose();
        }
    }

    private static void DisposeLateFrameResults(
        IEnumerable<Task<WindowsDisplayCaptureFrameOutcome>> captureTasks)
    {
        foreach (var task in captureTasks)
        {
            _ = task.ContinueWith(
                completed =>
                {
                    if (completed.Status == TaskStatus.RanToCompletion)
                    {
                        DisposeSuccessfulFrames(new[] { completed.Result });
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }
    }
}
