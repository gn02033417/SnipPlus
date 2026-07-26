using Microsoft.Graphics.Canvas;
using SnipPlus.Contracts;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics;
using Windows.Security.Authorization.AppCapabilityAccess;

namespace SnipPlus.Windows;

public sealed class WindowsGraphicsCaptureAdapter : ICaptureService
{
    private readonly CanvasDevice _canvasDevice;
    private readonly GraphicsCaptureItem _captureItem;

    public WindowsGraphicsCaptureAdapter(CanvasDevice canvasDevice, GraphicsCaptureItem captureItem)
    {
        _canvasDevice = canvasDevice ?? throw new ArgumentNullException(nameof(canvasDevice));
        _captureItem = captureItem ?? throw new ArgumentNullException(nameof(captureItem));
    }

    public static bool IsSupported => GraphicsCaptureSession.IsSupported();

    public static async ValueTask<WindowsGraphicsCaptureAdapter?> CreateForDisplayAsync(
        CanvasDevice canvasDevice,
        DisplayId displayId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(canvasDevice);
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsSupported)
        {
            return null;
        }

        var accessStatus = await GraphicsCaptureAccess.RequestAccessAsync(GraphicsCaptureAccessKind.Programmatic);
        cancellationToken.ThrowIfCancellationRequested();
        if (accessStatus != AppCapabilityAccessStatus.Allowed)
        {
            return null;
        }

        var captureItem = GraphicsCaptureItem.TryCreateFromDisplayId(displayId);
        return captureItem is null ? null : new WindowsGraphicsCaptureAdapter(canvasDevice, captureItem);
    }

    public async ValueTask<CaptureOutcome> CaptureAsync(
        CaptureIntent intent,
        CancellationToken cancellationToken)
    {
        Direct3D11CaptureFramePool? framePool = null;
        GraphicsCaptureSession? captureSession = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsSupported)
            {
                return Failed(intent, Failure.Create(
                    FailureCode.UnsupportedCapture,
                    FailureCategory.Unsupported,
                    FailureRecoverability.UserActionRequired,
                    "WindowsGraphicsCaptureAdapter.Support",
                    intent.RequestId,
                    "Windows.Graphics.Capture is not supported."), true, true);
            }

            var sourceSize = _captureItem.Size;
            if (sourceSize.Width <= 0 || sourceSize.Height <= 0)
            {
                return Failed(intent, Failure.Create(
                    FailureCode.CaptureSourceUnavailable,
                    FailureCategory.Device,
                    FailureRecoverability.RetryNewIntent,
                    "WindowsGraphicsCaptureAdapter.Source",
                    intent.RequestId,
                    "Capture source returned an empty size."), true, true);
            }

            framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(
                _canvasDevice,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                1,
                sourceSize);
            captureSession = framePool.CreateCaptureSession(_captureItem);
            captureSession.IsCursorCaptureEnabled = false;
            captureSession.StartCapture();

            using var frame = await WaitForFrameAsync(framePool, cancellationToken);
            if (frame is null)
            {
                return Failed(intent, Failure.Create(
                    FailureCode.CaptureFrameTimeout,
                    FailureCategory.Device,
                    FailureRecoverability.RetryNewIntent,
                    "WindowsGraphicsCaptureAdapter.Frame",
                    intent.RequestId,
                    "No usable frame arrived before the bounded timeout."), true, true);
            }

            var contentSize = frame.ContentSize;
            var contentWidth = checked((int)contentSize.Width);
            var contentHeight = checked((int)contentSize.Height);
            if (contentWidth <= 0
                || contentHeight <= 0
                || intent.CropBoundsInSource.Right > contentWidth
                || intent.CropBoundsInSource.Bottom > contentHeight)
            {
                return Failed(intent, Failure.Create(
                    FailureCode.CaptureFrameSizeChanged,
                    FailureCategory.Device,
                    FailureRecoverability.RetryNewIntent,
                    "WindowsGraphicsCaptureAdapter.ContentSize",
                    intent.RequestId,
                    "Frame content size does not contain the capture intent."), true, true);
            }

            using var canvasBitmap = CanvasBitmap.CreateFromDirect3D11Surface(_canvasDevice, frame.Surface);
            var pixelWidth = checked((int)canvasBitmap.SizeInPixels.Width);
            var pixelHeight = checked((int)canvasBitmap.SizeInPixels.Height);
            var sourceMetadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = intent.SessionId,
                PixelWidth = pixelWidth,
                PixelHeight = pixelHeight,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = intent.DpiScaleX * 96,
                DpiY = intent.DpiScaleY * 96,
                RowStride = checked(pixelWidth * 4),
                SourceKind = intent.SourceKind,
                SourcePhysicalBounds = intent.SourcePhysicalBounds,
                CropPhysicalBounds = intent.SourcePhysicalBounds,
                CapturedAt = DateTimeOffset.UtcNow,
                CursorIncluded = false
            };
            using var fullResult = SoftwareBitmapFactory.CreateFromPremultipliedBgra(canvasBitmap.GetPixelBytes(), sourceMetadata);
            var croppedResult = SoftwareBitmapCropper.Crop(
                fullResult,
                intent.CropBoundsInSource,
                Guid.NewGuid(),
                sourceMetadata.CapturedAt);

            return new CaptureOutcome.Succeeded(
                intent.RequestId,
                intent.SessionId,
                pixelWidth,
                pixelHeight,
                intent.SourcePhysicalBounds,
                intent.CropBoundsInSource,
                sourceMetadata.CapturedAt,
                croppedResult,
                Array.Empty<string>());
        }
        catch (OperationCanceledException)
        {
            return new CaptureOutcome.Cancelled(intent.RequestId, intent.SessionId, "CancellationToken", true, true);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Failed(intent, Failure.Create(
                FailureCode.CapturePermissionDenied,
                FailureCategory.Permission,
                FailureRecoverability.UserActionRequired,
                "WindowsGraphicsCaptureAdapter.Capture",
                intent.RequestId,
                exception.GetType().Name), true, true);
        }
        catch (Exception exception)
        {
            return Failed(intent, Failure.Create(
                FailureCode.UnexpectedFailure,
                FailureCategory.Unexpected,
                FailureRecoverability.RetryNewIntent,
                "WindowsGraphicsCaptureAdapter.Capture",
                intent.RequestId,
                exception.GetType().Name,
                nativeCode: exception.HResult), true, true);
        }
        finally
        {
            captureSession?.Dispose();
            framePool?.Dispose();
        }
    }

    private static async ValueTask<Direct3D11CaptureFrame?> WaitForFrameAsync(
        Direct3D11CaptureFramePool framePool,
        CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(2));
        while (true)
        {
            timeout.Token.ThrowIfCancellationRequested();
            var frame = framePool.TryGetNextFrame();
            if (frame is not null)
            {
                return frame;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(16), timeout.Token);
        }
    }

    private static CaptureOutcome.Failed Failed(
        CaptureIntent intent,
        Failure failure,
        bool cleanupCompleted,
        bool requiresNewIntent) => new(
            intent.RequestId,
            intent.SessionId,
            failure,
            cleanupCompleted,
            requiresNewIntent);
}
