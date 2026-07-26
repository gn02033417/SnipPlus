using Microsoft.Graphics.Canvas;
using SnipPlus.Contracts;
using Windows.Foundation;

namespace SnipPlus.Windows;

public sealed class Win2DRenderingAdapter : IRenderingAdapter
{
    public ValueTask<RenderOutcome> RenderAsync(RenderIntent intent, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (intent.PixelWidth <= 0
                || intent.PixelHeight <= 0
                || !double.IsFinite(intent.Dpi)
                || intent.Dpi <= 0)
            {
                return ValueTask.FromResult<RenderOutcome>(new RenderOutcome.Failed(
                    intent.SceneId,
                    Failure.Create(
                        FailureCode.RenderingFailed,
                        FailureCategory.Validation,
                        FailureRecoverability.RetryNewIntent,
                        "Win2DRenderingAdapter.Validate",
                        intent.SceneId,
                        "Render dimensions or DPI are invalid.")));
            }

            using var device = CanvasDevice.GetSharedDevice();
            using var target = new CanvasRenderTarget(
                device,
                intent.PixelWidth,
                intent.PixelHeight,
                (float)intent.Dpi,
                global::Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                CanvasAlphaMode.Premultiplied);
            using (var drawingSession = target.CreateDrawingSession())
            {
                drawingSession.Clear(ToColor(intent.Background));
                foreach (var node in intent.Nodes)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (node is not RenderNode.Rectangle rectangle || !rectangle.Bounds.IsPositive)
                    {
                        continue;
                    }

                    var bounds = new Rect(
                        rectangle.Bounds.Left,
                        rectangle.Bounds.Top,
                        rectangle.Bounds.Width,
                        rectangle.Bounds.Height);
                    var color = ToColor(rectangle.Color);
                    if (rectangle.Filled)
                    {
                        drawingSession.FillRectangle(bounds, color);
                    }
                    else
                    {
                        drawingSession.DrawRectangle(bounds, color, 1);
                    }
                }
            }

            if (intent.Target == RenderTargetKind.Display)
            {
                return ValueTask.FromResult<RenderOutcome>(new RenderOutcome.Succeeded(
                    intent.SceneId,
                    intent.Target,
                    intent.PixelWidth,
                    intent.PixelHeight,
                    null));
            }

            var metadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = Guid.Empty,
                PixelWidth = intent.PixelWidth,
                PixelHeight = intent.PixelHeight,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = intent.Dpi,
                DpiY = intent.Dpi,
                RowStride = checked(intent.PixelWidth * 4),
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = new PhysicalRect(0, 0, intent.PixelWidth, intent.PixelHeight),
                CropPhysicalBounds = new PhysicalRect(0, 0, intent.PixelWidth, intent.PixelHeight),
                CapturedAt = DateTimeOffset.UtcNow
            };
            var result = SoftwareBitmapFactory.CreateFromPremultipliedBgra(target.GetPixelBytes(), metadata);
            return ValueTask.FromResult<RenderOutcome>(new RenderOutcome.Succeeded(
                intent.SceneId,
                intent.Target,
                intent.PixelWidth,
                intent.PixelHeight,
                result));
        }
        catch (OperationCanceledException)
        {
            return ValueTask.FromResult<RenderOutcome>(new RenderOutcome.Cancelled(intent.SceneId));
        }
        catch (Exception exception)
        {
            return ValueTask.FromResult<RenderOutcome>(new RenderOutcome.Failed(
                intent.SceneId,
                Failure.Create(
                    FailureCode.RenderingFailed,
                    FailureCategory.Resource,
                    FailureRecoverability.RetrySameIntent,
                    "Win2DRenderingAdapter.Render",
                    intent.SceneId,
                    exception.GetType().Name)));
        }
    }

    private static global::Windows.UI.Color ToColor(RgbaColor color) => new()
    {
        R = color.R,
        G = color.G,
        B = color.B,
        A = color.A
    };
}
