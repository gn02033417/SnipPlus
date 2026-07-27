using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class CaptureRequestApplicationBoundary
{
    private readonly ICaptureRequestBoundary _captureRequestBoundary;

    public CaptureRequestApplicationBoundary(ICaptureRequestBoundary captureRequestBoundary)
    {
        _captureRequestBoundary =
            captureRequestBoundary ?? throw new ArgumentNullException(nameof(captureRequestBoundary));
    }

    public CaptureRequestResult SubmitPrintScreen(PrintScreenReceivedEventArgs args) =>
        _captureRequestBoundary.Submit(CaptureRequest.FromPrintScreen(args));

    public CaptureRequestResult SubmitSecondaryInAppCommand(Guid requestId, DateTimeOffset requestedAt) =>
        _captureRequestBoundary.Submit(CaptureRequest.CreateSecondary(requestId, requestedAt));
}
