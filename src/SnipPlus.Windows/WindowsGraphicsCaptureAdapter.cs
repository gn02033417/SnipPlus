using Microsoft.Graphics.Canvas;
using SnipPlus.Contracts;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics;
using Windows.Security.Authorization.AppCapabilityAccess;

namespace SnipPlus.Windows;

public sealed class WindowsGraphicsCaptureAdapter : ICaptureService, IDisposable
{
    private readonly CanvasDevice _canvasDevice;
    private GraphicsCaptureItem? _captureItem;

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

        AppCapabilityAccessStatus accessStatus;
        try
        {
            accessStatus = await GraphicsCaptureAccess
                .RequestAccessAsync(GraphicsCaptureAccessKind.Programmatic)
                .AsTask(cancellationToken)
                .WaitAsync(TimeSpan.FromSeconds(3), cancellationToken);
        }
        catch (TimeoutException)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (accessStatus != AppCapabilityAccessStatus.Allowed)
        {
            return null;
        }

        var captureItem = GraphicsCaptureItem.TryCreateFromDisplayId(displayId);
        return captureItem is null ? null : new WindowsGraphicsCaptureAdapter(canvasDevice, captureItem);
    }

    public async ValueTask<CaptureFrameOutcome> CaptureFrameAsync(
        CaptureIntent fullFrameIntent,
        CancellationToken cancellationToken)
    {
        Direct3D11CaptureFramePool? framePool = null;
        GraphicsCaptureSession? captureSession = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsSupported)
            {
                return FrameFailed(fullFrameIntent, Failure.Create(
                    FailureCode.UnsupportedCapture,
                    FailureCategory.Unsupported,
                    FailureRecoverability.UserActionRequired,
                    "WindowsGraphicsCaptureAdapter.Support",
                    fullFrameIntent.RequestId,
                    "Windows.Graphics.Capture is not supported."), true, true);
            }

            var captureItem = _captureItem;
            if (captureItem is null)
            {
                return FrameFailed(fullFrameIntent, Failure.Create(
                    FailureCode.CaptureSourceClosed,
                    FailureCategory.Device,
                    FailureRecoverability.RetryNewIntent,
                    "WindowsGraphicsCaptureAdapter.CaptureFrame",
                    fullFrameIntent.RequestId,
                    "The frozen capture source has been closed."), true, true);
            }

            var sourceSize = captureItem.Size;
            if (sourceSize.Width <= 0 || sourceSize.Height <= 0)
            {
                return FrameFailed(fullFrameIntent, Failure.Create(
                    FailureCode.CaptureSourceUnavailable,
                    FailureCategory.Device,
                    FailureRecoverability.RetryNewIntent,
                    "WindowsGraphicsCaptureAdapter.Source",
                    fullFrameIntent.RequestId,
                    "Capture source returned an empty size."), true, true);
            }

            framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(
                _canvasDevice,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                1,
                sourceSize);
            captureSession = framePool.CreateCaptureSession(captureItem);
            captureSession.IsCursorCaptureEnabled = false;
            captureSession.StartCapture();

            using var frame = await WaitForFrameAsync(framePool, cancellationToken);
            if (frame is null)
            {
                return FrameFailed(fullFrameIntent, Failure.Create(
                    FailureCode.CaptureFrameTimeout,
                    FailureCategory.Device,
                    FailureRecoverability.RetryNewIntent,
                    "WindowsGraphicsCaptureAdapter.Frame",
                    fullFrameIntent.RequestId,
                    "No usable frame arrived before the bounded timeout."), true, true);
            }

            var contentSize = frame.ContentSize;
            var contentWidth = checked((int)contentSize.Width);
            var contentHeight = checked((int)contentSize.Height);
            if (contentWidth <= 0 || contentHeight <= 0)
            {
                return FrameFailed(fullFrameIntent, Failure.Create(
                    FailureCode.CaptureFrameSizeChanged,
                    FailureCategory.Device,
                    FailureRecoverability.RetryNewIntent,
                    "WindowsGraphicsCaptureAdapter.ContentSize",
                    fullFrameIntent.RequestId,
                    "Frame content size is empty."), true, true);
            }

            using var canvasBitmap = CanvasBitmap.CreateFromDirect3D11Surface(_canvasDevice, frame.Surface);
            var pixelWidth = checked((int)canvasBitmap.SizeInPixels.Width);
            var pixelHeight = checked((int)canvasBitmap.SizeInPixels.Height);
            var sourceMetadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = fullFrameIntent.SessionId,
                PixelWidth = pixelWidth,
                PixelHeight = pixelHeight,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = fullFrameIntent.DpiScaleX * 96,
                DpiY = fullFrameIntent.DpiScaleY * 96,
                RowStride = checked(pixelWidth * 4),
                SourceKind = fullFrameIntent.SourceKind,
                SourcePhysicalBounds = fullFrameIntent.SourcePhysicalBounds,
                CropPhysicalBounds = fullFrameIntent.SourcePhysicalBounds,
                CapturedAt = DateTimeOffset.UtcNow,
                CursorIncluded = false
            };
            var fullResult = SoftwareBitmapFactory.CreateFromPremultipliedBgra(
                canvasBitmap.GetPixelBytes(),
                sourceMetadata);
            return new CaptureFrameOutcome.Succeeded(
                fullFrameIntent.RequestId,
                fullFrameIntent.SessionId,
                new FrozenCaptureFrame(fullResult));
        }
        catch (OperationCanceledException)
        {
            return new CaptureFrameOutcome.Cancelled(
                fullFrameIntent.RequestId,
                fullFrameIntent.SessionId,
                "CancellationToken",
                true,
                true);
        }
        catch (UnauthorizedAccessException exception)
        {
            return FrameFailed(fullFrameIntent, Failure.Create(
                FailureCode.CapturePermissionDenied,
                FailureCategory.Permission,
                FailureRecoverability.UserActionRequired,
                "WindowsGraphicsCaptureAdapter.Capture",
                fullFrameIntent.RequestId,
                exception.GetType().Name), true, true);
        }
        catch (Exception exception)
        {
            return FrameFailed(fullFrameIntent, Failure.Create(
                FailureCode.UnexpectedFailure,
                FailureCategory.Unexpected,
                FailureRecoverability.RetryNewIntent,
                "WindowsGraphicsCaptureAdapter.Capture",
                fullFrameIntent.RequestId,
                exception.GetType().Name,
                nativeCode: exception.HResult), true, true);
        }
        finally
        {
            captureSession?.Dispose();
            framePool?.Dispose();
        }
    }

    public ValueTask<CaptureOutcome> CropFrameAsync(
        CaptureIntent intent,
        FrozenCaptureFrame frozenFrame,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(frozenFrame);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (frozenFrame.ImageResult is not SoftwareBitmapImageResult source)
            {
                return ValueTask.FromResult<CaptureOutcome>(Failed(intent, Failure.Create(
                    FailureCode.InvalidCaptureIntent,
                    FailureCategory.Validation,
                    FailureRecoverability.TerminalForSession,
                    "WindowsGraphicsCaptureAdapter.CropFrame",
                    intent.RequestId,
                    "The frozen frame is not a canonical SoftwareBitmap."), true, true));
            }

            var sourceBounds = new PhysicalRect(
                0,
                0,
                source.Metadata.PixelWidth,
                source.Metadata.PixelHeight);
            if (!sourceBounds.Contains(intent.CropBoundsInSource))
            {
                return ValueTask.FromResult<CaptureOutcome>(Failed(intent, Failure.Create(
                    FailureCode.InvalidCaptureIntent,
                    FailureCategory.Validation,
                    FailureRecoverability.RetryNewIntent,
                    "WindowsGraphicsCaptureAdapter.CropFrame",
                    intent.RequestId,
                    "The crop bounds exceed the frozen frame."), true, true));
            }

            var croppedResult = SoftwareBitmapCropper.Crop(
                source,
                intent.CropBoundsInSource,
                Guid.NewGuid(),
                source.Metadata.CapturedAt);
            return ValueTask.FromResult<CaptureOutcome>(new CaptureOutcome.Succeeded(
                intent.RequestId,
                intent.SessionId,
                source.Metadata.PixelWidth,
                source.Metadata.PixelHeight,
                intent.SourcePhysicalBounds,
                intent.CropBoundsInSource,
                source.Metadata.CapturedAt,
                croppedResult,
                Array.Empty<string>()));
        }
        catch (OperationCanceledException)
        {
            return ValueTask.FromResult<CaptureOutcome>(new CaptureOutcome.Cancelled(
                intent.RequestId,
                intent.SessionId,
                "CancellationToken",
                true,
                true));
        }
        catch (Exception exception)
        {
            return ValueTask.FromResult<CaptureOutcome>(Failed(intent, Failure.Create(
                FailureCode.UnexpectedFailure,
                FailureCategory.Unexpected,
                FailureRecoverability.RetryNewIntent,
                "WindowsGraphicsCaptureAdapter.CropFrame",
                intent.RequestId,
                exception.GetType().Name,
                nativeCode: exception.HResult), true, true));
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

    private static CaptureFrameOutcome.Failed FrameFailed(
        CaptureIntent intent,
        Failure failure,
        bool cleanupCompleted,
        bool requiresNewIntent) => new(
            intent.RequestId,
            intent.SessionId,
            failure,
            cleanupCompleted,
            requiresNewIntent);

    public void Dispose()
    {
        _captureItem = null;
        GC.SuppressFinalize(this);
    }
}
