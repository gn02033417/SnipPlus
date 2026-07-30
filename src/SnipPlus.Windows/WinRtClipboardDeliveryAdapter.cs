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

    public WinRtClipboardDeliveryAdapter(
        Func<DataPackage, ClipboardContentOptions, bool>? setContent = null,
        Action? flush = null,
        ICompleteExecutionTraceSink? traceSink = null,
        IClipboardDeliveryDispatcher? dispatcher = null)
    {
        _setContent = setContent ?? Clipboard.SetContentWithOptions;
        _flush = flush ?? Clipboard.Flush;
        _trace = traceSink ?? NoOpCompleteExecutionTraceSink.Instance;
        _dispatcher = dispatcher;
    }

    public ValueTask<ClipboardDeliveryResult> DeliverAsync(
        ClipboardDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (_dispatcher is not null && !_dispatcher.HasThreadAccess)
        {
            return DeliverOnDispatcherAsync(request, cancellationToken);
        }

        return DeliverCoreAsync(request, cancellationToken);
    }

    private async ValueTask<ClipboardDeliveryResult> DeliverOnDispatcherAsync(
        ClipboardDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource<ClipboardDeliveryResult>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            if (!_dispatcher!.TryEnqueue(async () =>
            {
                try
                {
                    completion.TrySetResult(
                        await DeliverCoreAsync(request, cancellationToken));
                }
                catch (OperationCanceledException)
                {
                    completion.TrySetResult(Cancelled(request));
                }
                catch (Exception exception)
                {
                    completion.TrySetResult(DispatcherFailure(request, exception));
                }
            }))
            {
                return DispatcherFailure(
                    request,
                    new InvalidOperationException("Clipboard dispatcher is unavailable."));
            }
        }
        catch (Exception exception)
        {
            return DispatcherFailure(request, exception);
        }

        return await completion.Task;
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

                    Trace(request, CompleteExecutionStage.ClipboardPublishing, attempt: attempt, component: nameof(Clipboard));
                    if (!_setContent(package, options))
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
                        Trace(request, CompleteExecutionStage.ClipboardFlushing, attempt: attempt, component: nameof(Clipboard));
                        _flush();
                        Trace(request, CompleteExecutionStage.ClipboardDelivered, attempt: attempt, component: nameof(Clipboard));
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

    private ClipboardDeliveryResult.TerminalFailure DispatcherFailure(
        ClipboardDeliveryRequest request,
        Exception exception)
    {
        var failure = Failure.Create(
            FailureCode.ClipboardPublicationRejected,
            FailureCategory.IO,
            FailureRecoverability.TerminalForSession,
            "WinRtClipboardDeliveryAdapter.Dispatcher",
            request.DeliveryId,
            exception.Message,
            nativeCode: exception.HResult);
        Trace(request, CompleteExecutionStage.ClipboardFailed, failure, component: nameof(_dispatcher));
        return TerminalFailure(request, failure);
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
        string component = nameof(WinRtClipboardDeliveryAdapter))
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
                NativeCode = failure?.NativeCode,
                Component = component,
                SelectionWidth = request.SelectionWidth,
                SelectionHeight = request.SelectionHeight,
                ResultWidth = metadata.PixelWidth,
                ResultHeight = metadata.PixelHeight,
                DisplayCount = request.DisplayCount,
                ClipboardAttempt = attempt
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
}
