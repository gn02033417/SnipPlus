using System.Diagnostics;
using System.Runtime.InteropServices;
using SnipPlus.Contracts;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace SnipPlus.Windows;

public sealed class WinRtClipboardDeliveryAdapter : IClipboardDeliveryService
{
    private const int ClipboardCannotOpen = unchecked((int)0x800401D0);
    private const int RpcCallRejected = unchecked((int)0x80010001);
    private const int RpcServerCallRetryLater = unchecked((int)0x8001010A);
    private readonly Func<DataPackage, ClipboardContentOptions, bool> _setContent;
    private readonly Action _flush;
    private readonly ICompleteExecutionTraceSink _trace;
    private readonly IClipboardDeliveryDispatcher? _dispatcher;
    private readonly IClipboardRuntimeInitializer _runtimeInitializer;

    public WinRtClipboardDeliveryAdapter(
        Func<DataPackage, ClipboardContentOptions, bool>? setContent = null,
        Action? flush = null,
        ICompleteExecutionTraceSink? traceSink = null,
        IClipboardDeliveryDispatcher? dispatcher = null,
        IClipboardRuntimeInitializer? runtimeInitializer = null)
    {
        _setContent = setContent ?? Clipboard.SetContentWithOptions;
        _flush = flush ?? Clipboard.Flush;
        _trace = traceSink ?? NoOpCompleteExecutionTraceSink.Instance;
        _dispatcher = dispatcher;
        _runtimeInitializer = runtimeInitializer ?? NoOpClipboardRuntimeInitializer.Instance;
    }

    public ValueTask<ClipboardDeliveryResult> DeliverAsync(
        ClipboardDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        return DeliverCoreAsync(request, cancellationToken);
    }

    private async ValueTask<ClipboardDeliveryResult> DeliverCoreAsync(
        ClipboardDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.ImageResult is not SoftwareBitmapImageResult imageResult)
        {
            var failure = Failure.Create(
                FailureCode.InvalidResultLifetime,
                FailureCategory.Validation,
                FailureRecoverability.TerminalForSession,
                "WinRtClipboardDeliveryAdapter.ImageResult",
                request.DeliveryId,
                "Clipboard delivery requires a canonical SoftwareBitmap image result.");
            Trace(request, CompleteExecutionStage.ClipboardFailed, failure, component: nameof(WinRtClipboardDeliveryAdapter));
            return TerminalFailure(request, failure);
        }

        InMemoryRandomAccessStream pngStream;
        try
        {
            Trace(request, CompleteExecutionStage.ClipboardEncoding, component: nameof(PngEncoder));
            pngStream = await PngEncoder.EncodeAsync(imageResult, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Cancelled(request);
        }
        catch (Exception exception)
        {
            var failure = Failure.Create(
                FailureCode.EncodingFailed,
                FailureCategory.IO,
                FailureRecoverability.TerminalForSession,
                "WinRtClipboardDeliveryAdapter.Encode",
                request.DeliveryId,
                exception.GetType().Name,
                nativeCode: exception.HResult);
            Trace(request, CompleteExecutionStage.ClipboardFailed, failure, component: nameof(PngEncoder));
            return TerminalFailure(request, failure);
        }

        using (pngStream)
        {
            var stopwatch = Stopwatch.StartNew();
            Exception? lastContention = null;
            var maximumAttempts = Math.Max(1, request.MaximumAttempts);
            var retryBudget = request.RetryBudget < TimeSpan.Zero
                ? TimeSpan.Zero
                : request.RetryBudget;

            for (var attempt = 1; attempt <= maximumAttempts; attempt++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    pngStream.Seek(0);

                    var package = new DataPackage
                    {
                        RequestedOperation = DataPackageOperation.Copy
                    };
                    package.SetBitmap(RandomAccessStreamReference.CreateFromStream(pngStream));

                    var options = new ClipboardContentOptions
                    {
                        IsAllowedInHistory = request.HistoryAllowed,
                        IsRoamable = request.RoamingAllowed
                    };

                    if (!await PublishOnRequiredThreadAsync(
                        package,
                        options,
                        request,
                        attempt,
                        cancellationToken))
                    {
                        Trace(
                            request,
                            CompleteExecutionStage.ClipboardPublishing,
                            Failure.Create(
                                FailureCode.ClipboardBusy,
                                FailureCategory.Contention,
                                FailureRecoverability.RetrySameIntent,
                                "WinRtClipboardDeliveryAdapter.SetContent",
                                request.DeliveryId,
                                "Clipboard.SetContentWithOptions returned false.",
                                severity: FailureSeverity.Warning),
                            attempt,
                            nameof(Clipboard));
                        lastContention = null;
                    }
                    else
                    {
                        return new ClipboardDeliveryResult.Delivered(
                            request.DeliveryId,
                            request.SessionId,
                            request.ResultId,
                            attempt);
                    }
                }
                catch (OperationCanceledException)
                {
                    return Cancelled(request);
                }
                catch (Exception exception) when (IsContention(exception))
                {
                    lastContention = exception;
                    Trace(
                        request,
                        CompleteExecutionStage.ClipboardFailed,
                        Failure.Create(
                            FailureCode.ClipboardBusy,
                            FailureCategory.Contention,
                            FailureRecoverability.RetrySameIntent,
                            "WinRtClipboardDeliveryAdapter.Contention",
                            request.DeliveryId,
                            exception.GetType().Name,
                            severity: FailureSeverity.Warning,
                            nativeCode: exception.HResult),
                        attempt,
                        nameof(Clipboard));
                }
                catch (Exception exception)
                {
                    var failure = Failure.Create(
                        FailureCode.ClipboardPublicationRejected,
                        FailureCategory.IO,
                        FailureRecoverability.TerminalForSession,
                        "WinRtClipboardDeliveryAdapter.Publish",
                        request.DeliveryId,
                        exception.GetType().Name,
                        nativeCode: exception.HResult);
                    Trace(request, CompleteExecutionStage.ClipboardFailed, failure, attempt, nameof(Clipboard));
                    return TerminalFailure(request, failure);
                }

                var decision = ClipboardRetryPolicy.Decide(
                    attempt,
                    maximumAttempts,
                    stopwatch.Elapsed,
                    retryBudget);
                if (!decision.ShouldRetry)
                {
                    var failure = Failure.Create(
                        FailureCode.ClipboardBusy,
                        FailureCategory.Contention,
                        FailureRecoverability.RetrySameIntent,
                        "WinRtClipboardDeliveryAdapter.Retry",
                        request.DeliveryId,
                        lastContention?.GetType().Name ?? "Clipboard.SetContentWithOptions returned false",
                        severity: FailureSeverity.Warning,
                        nativeCode: lastContention?.HResult);
                    Trace(request, CompleteExecutionStage.ClipboardFailed, failure, attempt, nameof(Clipboard));
                    return new ClipboardDeliveryResult.RetryableFailure(
                        request.DeliveryId,
                        request.SessionId,
                        request.ResultId,
                        failure,
                        attempt);
                }

                try
                {
                    await Task.Delay(decision.Delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return Cancelled(request);
                }
            }

            throw new InvalidOperationException("Clipboard retry loop terminated unexpectedly.");
        }
    }

    private async ValueTask<bool> PublishOnRequiredThreadAsync(
        DataPackage package,
        ClipboardContentOptions options,
        ClipboardDeliveryRequest request,
        int attempt,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_dispatcher is null || _dispatcher.HasThreadAccess)
        {
            Trace(
                request,
                CompleteExecutionStage.ClipboardPublishing,
                attempt: attempt,
                component: "ClipboardDispatcher",
                diagnosticEvent: "DirectPath",
                dispatcherAvailable: _dispatcher is not null,
                dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess);
            return PublishDirect(package, options, request, attempt, cancellationToken);
        }

        Trace(
            request,
            CompleteExecutionStage.ClipboardPublishing,
            attempt: attempt,
            component: "ClipboardDispatcher",
            diagnosticEvent: "BeforeEnqueue",
            dispatcherAvailable: true,
            dispatcherHasThreadAccess: _dispatcher.HasThreadAccess);
        var completion = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        bool enqueued;
        try
        {
            enqueued = _dispatcher.TryEnqueue(() =>
            {
                Trace(
                    request,
                    CompleteExecutionStage.ClipboardPublishing,
                    attempt: attempt,
                    component: "ClipboardDispatcher",
                    diagnosticEvent: "CallbackEntered",
                    dispatcherAvailable: true,
                    dispatcherHasThreadAccess: _dispatcher.HasThreadAccess);
                try
                {
                    completion.TrySetResult(
                        PublishDirect(package, options, request, attempt, cancellationToken));
                }
                catch (Exception exception)
                {
                    Trace(
                        request,
                        CompleteExecutionStage.ClipboardFailed,
                        attempt: attempt,
                        component: "ClipboardDispatcher",
                        diagnosticEvent: "CallbackException",
                        dispatcherAvailable: true,
                        dispatcherHasThreadAccess: _dispatcher.HasThreadAccess,
                        nativeCode: exception.HResult,
                        exceptionType: exception.GetType().FullName);
                    completion.TrySetException(exception);
                }
            });
        }
        catch (Exception exception)
        {
            Trace(
                request,
                CompleteExecutionStage.ClipboardFailed,
                attempt: attempt,
                component: "ClipboardDispatcher",
                diagnosticEvent: "EnqueueException",
                dispatcherAvailable: true,
                dispatcherHasThreadAccess: _dispatcher.HasThreadAccess,
                nativeCode: exception.HResult,
                exceptionType: exception.GetType().FullName);
            throw;
        }

        Trace(
            request,
            CompleteExecutionStage.ClipboardPublishing,
            attempt: attempt,
            component: "ClipboardDispatcher",
            diagnosticEvent: "AfterEnqueue",
            dispatcherAvailable: true,
            dispatcherHasThreadAccess: _dispatcher.HasThreadAccess,
            dispatcherEnqueueSucceeded: enqueued);
        if (!enqueued)
        {
            throw new InvalidOperationException("Clipboard dispatcher is unavailable.");
        }

        return await completion.Task;
    }

    private bool PublishDirect(
        DataPackage package,
        ClipboardContentOptions options,
        ClipboardDeliveryRequest request,
        int attempt,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Trace(
            request,
            CompleteExecutionStage.ClipboardPublishing,
            attempt: attempt,
            component: "ClipboardRuntime",
            diagnosticEvent: "RuntimeInitializationBefore",
            dispatcherAvailable: _dispatcher is not null,
            dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess);
        IDisposable runtimeScope;
        try
        {
            runtimeScope = _runtimeInitializer.Enter();
        }
        catch (Exception exception)
        {
            Trace(
                request,
                CompleteExecutionStage.ClipboardFailed,
                attempt: attempt,
                component: "ClipboardRuntime",
                diagnosticEvent: "RuntimeInitializationException",
                dispatcherAvailable: _dispatcher is not null,
                dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess,
                nativeCode: exception.HResult,
                exceptionType: exception.GetType().FullName);
            throw;
        }

        using (runtimeScope)
        {
            Trace(
                request,
                CompleteExecutionStage.ClipboardPublishing,
                attempt: attempt,
                component: "ClipboardRuntime",
                diagnosticEvent: "RuntimeInitializationAfter",
                dispatcherAvailable: _dispatcher is not null,
                dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess);
            return PublishWithInitializedRuntime(package, options, request, attempt, cancellationToken);
        }
    }

    private bool PublishWithInitializedRuntime(
        DataPackage package,
        ClipboardContentOptions options,
        ClipboardDeliveryRequest request,
        int attempt,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Trace(
            request,
            CompleteExecutionStage.ClipboardPublishing,
            attempt: attempt,
            component: nameof(Clipboard),
            diagnosticEvent: "SetContentBefore",
            dispatcherAvailable: _dispatcher is not null,
            dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess);
        bool published;
        try
        {
            published = _setContent(package, options);
        }
        catch (Exception exception)
        {
            Trace(
                request,
                CompleteExecutionStage.ClipboardFailed,
                attempt: attempt,
                component: nameof(Clipboard),
                diagnosticEvent: "SetContentException",
                dispatcherAvailable: _dispatcher is not null,
                dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess,
                nativeCode: exception.HResult,
                exceptionType: exception.GetType().FullName);
            throw;
        }

        Trace(
            request,
            CompleteExecutionStage.ClipboardPublishing,
            attempt: attempt,
            component: nameof(Clipboard),
            diagnosticEvent: "SetContentAfter",
            dispatcherAvailable: _dispatcher is not null,
            dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess);
        if (!published)
        {
            return false;
        }

        cancellationToken.ThrowIfCancellationRequested();
        Trace(
            request,
            CompleteExecutionStage.ClipboardFlushing,
            attempt: attempt,
            component: nameof(Clipboard),
            diagnosticEvent: "FlushBefore",
            dispatcherAvailable: _dispatcher is not null,
            dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess);
        try
        {
            _flush();
        }
        catch (Exception exception)
        {
            Trace(
                request,
                CompleteExecutionStage.ClipboardFailed,
                attempt: attempt,
                component: nameof(Clipboard),
                diagnosticEvent: "FlushException",
                dispatcherAvailable: _dispatcher is not null,
                dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess,
                nativeCode: exception.HResult,
                exceptionType: exception.GetType().FullName);
            throw;
        }

        Trace(
            request,
            CompleteExecutionStage.ClipboardFlushing,
            attempt: attempt,
            component: nameof(Clipboard),
            diagnosticEvent: "FlushAfter",
            dispatcherAvailable: _dispatcher is not null,
            dispatcherHasThreadAccess: _dispatcher?.HasThreadAccess);
        Trace(request, CompleteExecutionStage.ClipboardDelivered, attempt: attempt, component: nameof(Clipboard));
        return true;
    }

    private static bool IsContention(Exception exception) => exception switch
    {
        COMException comException => comException.HResult is ClipboardCannotOpen
            or RpcCallRejected
            or RpcServerCallRetryLater,
        UnauthorizedAccessException => false,
        _ => false
    };

    private static ClipboardDeliveryResult.Cancelled Cancelled(ClipboardDeliveryRequest request) =>
        new(request.DeliveryId, request.SessionId, request.ResultId, "CancellationToken");

    private static ClipboardDeliveryResult.TerminalFailure TerminalFailure(
        ClipboardDeliveryRequest request,
        Failure failure) =>
        new(request.DeliveryId, request.SessionId, request.ResultId, failure);

    private void Trace(
        ClipboardDeliveryRequest request,
        CompleteExecutionStage stage,
        Failure? failure = null,
        int attempt = 0,
        string component = nameof(WinRtClipboardDeliveryAdapter),
        string? diagnosticEvent = null,
        bool? dispatcherAvailable = null,
        bool? dispatcherHasThreadAccess = null,
        bool? dispatcherEnqueueSucceeded = null,
        int? nativeCode = null,
        string? exceptionType = null)
    {
        try
        {
            var metadata = request.ImageResult.Metadata;
            _trace.Record(new CompleteExecutionTraceEntry
            {
                TimestampUtc = DateTimeOffset.UtcNow,
                SessionId = request.SessionId,
                SelectionRevision = 0,
                WorkflowState = WorkflowState.Delivering,
                CompleteStage = stage,
                FailureCode = failure?.Code,
                FailureCategory = failure?.Category,
                NativeCode = failure?.NativeCode ?? nativeCode,
                Component = component,
                SelectionWidth = request.SelectionWidth,
                SelectionHeight = request.SelectionHeight,
                ResultWidth = metadata.PixelWidth,
                ResultHeight = metadata.PixelHeight,
                DisplayCount = request.DisplayCount,
                ClipboardAttempt = attempt,
                ManagedThreadId = Environment.CurrentManagedThreadId,
                DispatcherAvailable = dispatcherAvailable,
                DispatcherHasThreadAccess = dispatcherHasThreadAccess,
                DispatcherEnqueueSucceeded = dispatcherEnqueueSucceeded,
                DiagnosticEvent = diagnosticEvent,
                ExceptionType = exceptionType
            });
        }
        catch
        {
            // Trace failure must never affect Clipboard delivery.
        }
    }

    private sealed class NoOpCompleteExecutionTraceSink : ICompleteExecutionTraceSink
    {
        public static NoOpCompleteExecutionTraceSink Instance { get; } = new();

        public void Record(CompleteExecutionTraceEntry entry)
        {
        }
    }

    private sealed class NoOpClipboardRuntimeInitializer : IClipboardRuntimeInitializer
    {
        public static NoOpClipboardRuntimeInitializer Instance { get; } = new();

        public IDisposable Enter() => NoOpDisposable.Instance;

        private sealed class NoOpDisposable : IDisposable
        {
            public static NoOpDisposable Instance { get; } = new();

            public void Dispose()
            {
            }
        }
    }
}
