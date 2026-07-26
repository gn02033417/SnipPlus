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

    public async ValueTask<ClipboardDeliveryResult> DeliverAsync(
        ClipboardDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (request.ImageResult is not SoftwareBitmapImageResult imageResult)
        {
            return TerminalFailure(request, Failure.Create(
                FailureCode.InvalidResultLifetime,
                FailureCategory.Validation,
                FailureRecoverability.TerminalForSession,
                "WinRtClipboardDeliveryAdapter.ImageResult",
                request.DeliveryId,
                "Clipboard delivery requires a canonical SoftwareBitmap image result."));
        }

        InMemoryRandomAccessStream pngStream;
        try
        {
            pngStream = await PngEncoder.EncodeAsync(imageResult, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Cancelled(request);
        }
        catch (Exception exception)
        {
            return TerminalFailure(request, Failure.Create(
                FailureCode.EncodingFailed,
                FailureCategory.IO,
                FailureRecoverability.TerminalForSession,
                "WinRtClipboardDeliveryAdapter.Encode",
                request.DeliveryId,
                exception.GetType().Name,
                nativeCode: exception.HResult));
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

                    if (!Clipboard.SetContentWithOptions(package, options))
                    {
                        lastContention = null;
                    }
                    else
                    {
                        Clipboard.Flush();
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
                }
                catch (Exception exception)
                {
                    return TerminalFailure(request, Failure.Create(
                        FailureCode.ClipboardPublicationRejected,
                        FailureCategory.IO,
                        FailureRecoverability.TerminalForSession,
                        "WinRtClipboardDeliveryAdapter.Publish",
                        request.DeliveryId,
                        exception.GetType().Name,
                        nativeCode: exception.HResult));
                }

                var decision = ClipboardRetryPolicy.Decide(
                    attempt,
                    maximumAttempts,
                    stopwatch.Elapsed,
                    retryBudget);
                if (!decision.ShouldRetry)
                {
                    return new ClipboardDeliveryResult.RetryableFailure(
                        request.DeliveryId,
                        request.SessionId,
                        request.ResultId,
                        Failure.Create(
                            FailureCode.ClipboardBusy,
                            FailureCategory.Contention,
                            FailureRecoverability.RetrySameIntent,
                            "WinRtClipboardDeliveryAdapter.Retry",
                            request.DeliveryId,
                            lastContention?.GetType().Name ?? "Clipboard.SetContentWithOptions returned false",
                            severity: FailureSeverity.Warning,
                            nativeCode: lastContention?.HResult),
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
}
