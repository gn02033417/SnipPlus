using SnipPlus.Contracts;

namespace SnipPlus.Windows;

public sealed class WindowsFrozenDisplayFrameSetRenderer : IFrozenDisplayFrameSetRenderer
{
    public ValueTask<FrozenDisplayFrameSetRenderOutcome> RenderAsync(
        FrozenDisplayFrameSet frameSet,
        PhysicalRect selectionPhysicalBounds,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(frameSet);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (frameSet.IsDisposed)
            {
                return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                    frameSet.SessionId,
                    FailureCode.InvalidResultLifetime,
                    "The frozen display frame set has already been disposed."));
            }

            if (frameSet.Frames.Values.Any(frame => frame.IsDisposed))
            {
                return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                    frameSet.SessionId,
                    FailureCode.InvalidResultLifetime,
                    "A frozen display frame has already been disposed."));
            }

            if (!selectionPhysicalBounds.IsPositive
                || selectionPhysicalBounds.Width64 > SupportedCapacityPolicy.MaxSelectionWidth
                || selectionPhysicalBounds.Height64 > SupportedCapacityPolicy.MaxSelectionHeight
                || checked(selectionPhysicalBounds.Width64 * selectionPhysicalBounds.Height64)
                    > SupportedCapacityPolicy.MaxSelectionArea)
            {
                return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                    frameSet.SessionId,
                    FailureCode.InvalidSelection,
                    "The Selection bounds are empty or exceed the supported canonical image size."));
            }

            var composition = WindowsFrozenDisplayCompositor.Compose(
                frameSet,
                selectionPhysicalBounds,
                frameSet.Frames.Keys.OrderBy(key => key, StringComparer.Ordinal),
                cancellationToken);

            var metadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = frameSet.SessionId,
                PixelWidth = composition.PixelWidth,
                PixelHeight = composition.PixelHeight,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = 96,
                DpiY = 96,
                RowStride = composition.RowStride,
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = selectionPhysicalBounds,
                CropPhysicalBounds = selectionPhysicalBounds,
                CapturedAt = composition.CapturedAt,
                CursorIncluded = false
            };

            return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(
                new FrozenDisplayFrameSetRenderOutcome.Succeeded(
                    SoftwareBitmapFactory.CreateFromPremultipliedBgra(composition.Pixels, metadata)));
        }
        catch (OperationCanceledException)
        {
            return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(
                new FrozenDisplayFrameSetRenderOutcome.Cancelled("CancellationToken"));
        }
        catch (OverflowException exception)
        {
            return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                frameSet.SessionId,
                FailureCode.RenderingFailed,
                exception.GetType().Name,
                exception.HResult));
        }
        catch (Exception exception)
        {
            return ValueTask.FromResult<FrozenDisplayFrameSetRenderOutcome>(Failed(
                frameSet.SessionId,
                FailureCode.RenderingFailed,
                exception.GetType().Name,
                exception.HResult));
        }
    }

    private static FrozenDisplayFrameSetRenderOutcome.Failed Failed(
        Guid sessionId,
        FailureCode code,
        string message,
        int? nativeCode = null) => new(Failure.Create(
        code,
        FailureCategory.Resource,
        FailureRecoverability.RetrySameIntent,
        nameof(WindowsFrozenDisplayFrameSetRenderer),
        sessionId,
        message,
        nativeCode: nativeCode));
}
