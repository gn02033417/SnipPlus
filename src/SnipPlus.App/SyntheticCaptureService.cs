using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.App;

internal sealed class SyntheticCaptureService : ICaptureService
{
    public ValueTask<CaptureFrameOutcome> CaptureFrameAsync(
        CaptureIntent fullFrameIntent,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var width = fullFrameIntent.SourcePhysicalBounds.Width;
            var height = fullFrameIntent.SourcePhysicalBounds.Height;
            if (width <= 0 || height <= 0)
            {
                return ValueTask.FromResult<CaptureFrameOutcome>(new CaptureFrameOutcome.Failed(
                    fullFrameIntent.RequestId,
                    fullFrameIntent.SessionId,
                    Failure.Create(
                        FailureCode.CaptureSourceUnavailable,
                        FailureCategory.Device,
                        FailureRecoverability.RetryNewIntent,
                        "SyntheticCaptureService.Source",
                        fullFrameIntent.RequestId,
                        "The synthetic source has an empty size."),
                    true,
                    true));
            }

            var pixels = CreateCheckerboard(width, height);
            var metadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = fullFrameIntent.SessionId,
                PixelWidth = width,
                PixelHeight = height,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = fullFrameIntent.DpiScaleX * 96,
                DpiY = fullFrameIntent.DpiScaleY * 96,
                RowStride = checked(width * 4),
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = fullFrameIntent.SourcePhysicalBounds,
                CropPhysicalBounds = fullFrameIntent.SourcePhysicalBounds,
                CapturedAt = DateTimeOffset.UnixEpoch,
                CursorIncluded = false,
                ContentVersion = 1
            };
            var image = SoftwareBitmapFactory.CreateFromPremultipliedBgra(pixels, metadata);
            return ValueTask.FromResult<CaptureFrameOutcome>(new CaptureFrameOutcome.Succeeded(
                fullFrameIntent.RequestId,
                fullFrameIntent.SessionId,
                new FrozenCaptureFrame(image)));
        }
        catch (OperationCanceledException)
        {
            return ValueTask.FromResult<CaptureFrameOutcome>(new CaptureFrameOutcome.Cancelled(
                fullFrameIntent.RequestId,
                fullFrameIntent.SessionId,
                "CancellationToken",
                false,
                true));
        }
    }

    public ValueTask<CaptureOutcome> CropFrameAsync(
        CaptureIntent intent,
        FrozenCaptureFrame frozenFrame,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (frozenFrame.ImageResult is not SoftwareBitmapImageResult source)
            {
                return ValueTask.FromResult<CaptureOutcome>(new CaptureOutcome.Failed(
                    intent.RequestId,
                    intent.SessionId,
                    Failure.Create(
                        FailureCode.InvalidCaptureIntent,
                        FailureCategory.Validation,
                        FailureRecoverability.TerminalForSession,
                        "SyntheticCaptureService.CropFrame",
                        intent.RequestId,
                        "The frozen source is not a SoftwareBitmap."),
                    true,
                    true));
            }

            var bounds = new PhysicalRect(0, 0, source.Metadata.PixelWidth, source.Metadata.PixelHeight);
            if (!bounds.Contains(intent.CropBoundsInSource))
            {
                return ValueTask.FromResult<CaptureOutcome>(new CaptureOutcome.Failed(
                    intent.RequestId,
                    intent.SessionId,
                    Failure.Create(
                        FailureCode.InvalidCaptureIntent,
                        FailureCategory.Validation,
                        FailureRecoverability.RetryNewIntent,
                        "SyntheticCaptureService.CropFrame",
                        intent.RequestId,
                        "The crop bounds exceed the synthetic frozen source."),
                    true,
                    true));
            }

            var cropped = SoftwareBitmapCropper.Crop(
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
                cropped,
                Array.Empty<string>()));
        }
        catch (OperationCanceledException)
        {
            return ValueTask.FromResult<CaptureOutcome>(new CaptureOutcome.Cancelled(
                intent.RequestId,
                intent.SessionId,
                "CancellationToken",
                false,
                true));
        }
    }

    private static byte[] CreateCheckerboard(int width, int height)
    {
        var pixels = new byte[checked(width * height * 4)];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var offset = checked((y * width + x) * 4);
                var block = ((x / 64) + (y / 64)) % 2;
                pixels[offset] = block == 0 ? (byte)24 : (byte)220;
                pixels[offset + 1] = block == 0 ? (byte)96 : (byte)224;
                pixels[offset + 2] = block == 0 ? (byte)192 : (byte)248;
                pixels[offset + 3] = 255;
            }
        }

        return pixels;
    }
}
