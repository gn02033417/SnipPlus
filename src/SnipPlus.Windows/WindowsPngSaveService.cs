using SnipPlus.Contracts;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace SnipPlus.Windows;

public interface IWindowsPngSavePicker
{
    ValueTask<IWindowsPngSaveTarget?> PickAsync(
        string suggestedFileName,
        CancellationToken cancellationToken);
}

public interface IWindowsPngSaveTarget : IDisposable
{
    string FileName { get; }

    string Identifier { get; }

    ValueTask<IRandomAccessStream> OpenWriteAsync(CancellationToken cancellationToken);
}

public sealed class WindowsPngSaveService : IPngSavePlatformService
{
    private readonly IWindowsPngSavePicker _picker;
    private readonly Func<IImageResult, CancellationToken, ValueTask<IRandomAccessStream>> _encoder;

    public WindowsPngSaveService(
        nint ownerWindowHandle,
        IWindowsPngSavePicker? picker = null,
        Func<IImageResult, CancellationToken, ValueTask<IRandomAccessStream>>? encoder = null)
        : this(
            picker ?? new WindowsPngSavePicker(ownerWindowHandle),
            encoder ?? EncodeWithExistingPngEncoderAsync)
    {
    }

    public WindowsPngSaveService(
        IWindowsPngSavePicker picker,
        Func<IImageResult, CancellationToken, ValueTask<IRandomAccessStream>> encoder)
    {
        ArgumentNullException.ThrowIfNull(picker);
        ArgumentNullException.ThrowIfNull(encoder);
        _picker = picker;
        _encoder = encoder;
    }

    public async ValueTask<PngSaveResult> SaveAsync(
        PngSaveRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        IWindowsPngSaveTarget? target;
        try
        {
            target = await _picker.PickAsync(request.SuggestedFileName, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return new PngSaveResult.Cancelled(
                request.SessionId,
                request.ResultId,
                "SaveDialogCancellationToken");
        }
        catch (Exception exception)
        {
            return new PngSaveResult.SaveDialogFailed(
                request.SessionId,
                request.ResultId,
                CreateFailure(
                    FailureCode.OutputAccessDenied,
                    "WindowsPngSaveService.PickSaveFile",
                    request.ResultId,
                    exception));
        }

        if (target is null)
        {
            return new PngSaveResult.SaveDialogCancelled(
                request.SessionId,
                request.ResultId,
                "UserCancelledSaveDialog");
        }

        using (target)
        {
            if (!IsPngFileName(target.FileName))
            {
                return new PngSaveResult.PngWriteFailed(
                    request.SessionId,
                    request.ResultId,
                    Failure.Create(
                        FailureCode.OutputWriteFailed,
                        FailureCategory.Validation,
                        FailureRecoverability.RetryNewIntent,
                        "WindowsPngSaveService.FileExtension",
                        request.ResultId,
                        "The selected output file must use the .png extension."));
            }

            IRandomAccessStream? encodedStream = null;
            try
            {
                encodedStream = await _encoder(request.ImageResult, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return new PngSaveResult.Cancelled(
                    request.SessionId,
                    request.ResultId,
                    "PngEncodingCancellationToken");
            }
            catch (Exception exception)
            {
                return new PngSaveResult.PngEncodingFailed(
                    request.SessionId,
                    request.ResultId,
                    CreateFailure(
                        FailureCode.EncodingFailed,
                        "WindowsPngSaveService.EncodePng",
                        request.ResultId,
                        exception));
            }

            using (encodedStream)
            {
                try
                {
                    using var outputStream = await target.OpenWriteAsync(cancellationToken);
                    outputStream.Size = 0;
                    encodedStream.Seek(0);
                    await RandomAccessStream.CopyAsync(encodedStream, outputStream)
                        .AsTask(cancellationToken);
                    await outputStream.FlushAsync().AsTask(cancellationToken);

                    return new PngSaveResult.Saved(
                        request.SessionId,
                        request.ResultId,
                        target.Identifier,
                        new PngWriteSucceededEvidence
                        {
                            SessionId = request.SessionId,
                            ResultId = request.ResultId
                        });
                }
                catch (OperationCanceledException)
                {
                    return new PngSaveResult.Cancelled(
                        request.SessionId,
                        request.ResultId,
                        "PngWriteCancellationToken");
                }
                catch (Exception exception)
                {
                    return new PngSaveResult.PngWriteFailed(
                        request.SessionId,
                        request.ResultId,
                        CreateFailure(
                            FailureCode.OutputWriteFailed,
                            "WindowsPngSaveService.WritePng",
                            request.ResultId,
                            exception));
                }
            }
        }
    }

    private static async ValueTask<IRandomAccessStream> EncodeWithExistingPngEncoderAsync(
        IImageResult imageResult,
        CancellationToken cancellationToken)
    {
        return await PngEncoder.EncodeAsync(imageResult, cancellationToken);
    }

    private static bool IsPngFileName(string? fileName)
    {
        return !string.IsNullOrWhiteSpace(fileName)
            && fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
    }

    private static Failure CreateFailure(
        FailureCode code,
        string operation,
        Guid correlationId,
        Exception exception)
    {
        return Failure.Create(
            code,
            FailureCategory.IO,
            FailureRecoverability.RetryNewIntent,
            operation,
            correlationId,
            exception.GetType().Name,
            nativeCode: exception.HResult);
    }
}

public sealed class WindowsPngSavePicker : IWindowsPngSavePicker
{
    private readonly nint _ownerWindowHandle;

    public WindowsPngSavePicker(nint ownerWindowHandle)
    {
        _ownerWindowHandle = ownerWindowHandle;
    }

    public async ValueTask<IWindowsPngSaveTarget?> PickAsync(
        string suggestedFileName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(suggestedFileName);
        cancellationToken.ThrowIfCancellationRequested();

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.Downloads,
            SuggestedFileName = suggestedFileName,
            DefaultFileExtension = ".png"
        };
        picker.FileTypeChoices.Add("PNG image", new List<string> { ".png" });
        InitializeWithWindow.Initialize(picker, _ownerWindowHandle);

        var file = await picker.PickSaveFileAsync().AsTask(cancellationToken);
        return file is null ? null : new WindowsPngSaveTarget(file);
    }
}

public sealed class WindowsPngSaveTarget : IWindowsPngSaveTarget
{
    private readonly StorageFile _file;

    public WindowsPngSaveTarget(StorageFile file)
    {
        ArgumentNullException.ThrowIfNull(file);
        _file = file;
    }

    public string FileName => _file.Name;

    public string Identifier => _file.Name;

    public async ValueTask<IRandomAccessStream> OpenWriteAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _file.OpenAsync(FileAccessMode.ReadWrite).AsTask(cancellationToken);
    }

    public void Dispose()
    {
    }
}
