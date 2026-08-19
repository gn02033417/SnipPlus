using System.Globalization;
using SnipPlus.Contracts;

namespace SnipPlus.Core;

public static class PngSaveFileNamePolicy
{
    public static string CreateSuggestedFileName(DateTimeOffset timestamp)
    {
        return $"SnipPlus_{timestamp.ToString("yyyy-MM-dd_HHmmss", CultureInfo.InvariantCulture)}.png";
    }

    public static bool IsPngFileName(string? fileName)
    {
        return !string.IsNullOrWhiteSpace(fileName)
            && fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
    }
}

public abstract record PngSavePreparationResult(Guid SessionId, Guid ResultId)
{
    public sealed record Ready(Guid SessionId, Guid ResultId, PngSaveRequest Request)
        : PngSavePreparationResult(SessionId, ResultId);

    public sealed record Failed(Guid SessionId, Guid ResultId, Failure Failure)
        : PngSavePreparationResult(SessionId, ResultId);
}

public sealed class PngSaveRequestFactory
{
    private readonly Func<DateTimeOffset> _clock;

    public PngSaveRequestFactory(Func<DateTimeOffset>? clock = null)
    {
        _clock = clock ?? (() => DateTimeOffset.UtcNow);
    }

    public PngSavePreparationResult Prepare(IImageResult? imageResult)
    {
        if (imageResult is null)
        {
            return Failed(
                Guid.Empty,
                Guid.Empty,
                FailureCode.InvalidResultLifetime,
                "PngSaveRequestFactory.NullImageResult",
                "A canonical image result is required before Save As.");
        }

        if (imageResult.IsDisposed)
        {
            return Failed(
                ReadSessionId(imageResult),
                ReadResultId(imageResult),
                FailureCode.InvalidResultLifetime,
                "PngSaveRequestFactory.DisposedImageResult",
                "The canonical image result has already been disposed.");
        }

        ImageResultMetadata metadata;
        try
        {
            metadata = imageResult.Metadata;
        }
        catch (Exception exception)
        {
            return Failed(
                Guid.Empty,
                Guid.Empty,
                FailureCode.InvalidResultLifetime,
                "PngSaveRequestFactory.ReadMetadata",
                exception.GetType().Name);
        }

        if (metadata.SessionId == Guid.Empty || metadata.ResultId == Guid.Empty)
        {
            return Failed(
                metadata.SessionId,
                metadata.ResultId,
                FailureCode.InvalidResultLifetime,
                "PngSaveRequestFactory.Identity",
                "Canonical image identity is incomplete.");
        }

        if (metadata.PixelWidth <= 0
            || metadata.PixelHeight <= 0
            || metadata.PixelWidth > int.MaxValue / 4
            || metadata.PixelFormat != ImagePixelFormat.Bgra8
            || metadata.AlphaMode != ImageAlphaMode.Premultiplied
            || metadata.ColorSpace != ImageColorSpace.SrgbSdr
            || metadata.RowStride < metadata.PixelWidth * 4)
        {
            return Failed(
                metadata.SessionId,
                metadata.ResultId,
                FailureCode.InvalidResultLifetime,
                "PngSaveRequestFactory.CanonicalMetadata",
                "PNG Save As requires a canonical BGRA8 premultiplied sRGB SDR image result.");
        }

        return new PngSavePreparationResult.Ready(
            metadata.SessionId,
            metadata.ResultId,
            new PngSaveRequest
            {
                SessionId = metadata.SessionId,
                ResultId = metadata.ResultId,
                ImageResult = imageResult,
                SuggestedFileName = PngSaveFileNamePolicy.CreateSuggestedFileName(_clock())
            });
    }

    private static PngSavePreparationResult.Failed Failed(
        Guid sessionId,
        Guid resultId,
        FailureCode code,
        string operation,
        string diagnosticMessage)
    {
        return new PngSavePreparationResult.Failed(
            sessionId,
            resultId,
            Failure.Create(
                code,
                FailureCategory.Validation,
                FailureRecoverability.TerminalForSession,
                operation,
                resultId == Guid.Empty ? sessionId : resultId,
                diagnosticMessage));
    }

    private static Guid ReadSessionId(IImageResult imageResult)
    {
        try
        {
            return imageResult.Metadata.SessionId;
        }
        catch
        {
            return Guid.Empty;
        }
    }

    private static Guid ReadResultId(IImageResult imageResult)
    {
        try
        {
            return imageResult.Metadata.ResultId;
        }
        catch
        {
            return Guid.Empty;
        }
    }
}

public sealed class PngSaveCoordinator
{
    private readonly PngSaveRequestFactory _requestFactory;
    private readonly IPngSavePlatformService _platformService;

    public PngSaveCoordinator(
        IPngSavePlatformService platformService,
        PngSaveRequestFactory? requestFactory = null)
    {
        ArgumentNullException.ThrowIfNull(platformService);
        _platformService = platformService;
        _requestFactory = requestFactory ?? new PngSaveRequestFactory();
    }

    public ValueTask<PngSaveResult> SaveAsync(
        IImageResult? imageResult,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var preparation = _requestFactory.Prepare(imageResult);
        if (preparation is PngSavePreparationResult.Failed failed)
        {
            return ValueTask.FromResult<PngSaveResult>(new PngSaveResult.Rejected(
                failed.SessionId,
                failed.ResultId,
                failed.Failure));
        }

        var ready = (PngSavePreparationResult.Ready)preparation;
        return SavePlatformAsync(ready.Request, cancellationToken);
    }

    private async ValueTask<PngSaveResult> SavePlatformAsync(
        PngSaveRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _platformService.SaveAsync(request, cancellationToken);
        if (result.SessionId != request.SessionId || result.ResultId != request.ResultId)
        {
            return RejectedPlatformResult(request, "Platform Save As result identity does not match the request.");
        }

        if (result is PngSaveResult.Saved saved
            && (saved.PngWrite.SessionId != request.SessionId
                || saved.PngWrite.ResultId != request.ResultId))
        {
            return RejectedPlatformResult(request, "PNG write evidence does not match the canonical result.");
        }

        return result;
    }

    private static PngSaveResult.Rejected RejectedPlatformResult(
        PngSaveRequest request,
        string diagnosticMessage)
    {
        return new PngSaveResult.Rejected(
            request.SessionId,
            request.ResultId,
            Failure.Create(
                FailureCode.InvalidResultLifetime,
                FailureCategory.Validation,
                FailureRecoverability.RetryNewIntent,
                "PngSaveCoordinator.ResultIdentity",
                request.ResultId,
                diagnosticMessage));
    }
}
