namespace SnipPlus.Contracts;

public enum RenderTargetKind
{
    Display,
    CanonicalRaster
}

public readonly record struct RgbaColor(byte R, byte G, byte B, byte A);

public abstract record RenderNode
{
    private RenderNode()
    {
    }

    public sealed record Rectangle(PhysicalRect Bounds, RgbaColor Color, bool Filled) : RenderNode;
}

public sealed record RenderIntent
{
    public required Guid SceneId { get; init; }
    public required int PixelWidth { get; init; }
    public required int PixelHeight { get; init; }
    public double Dpi { get; init; } = 96;
    public RenderTargetKind Target { get; init; } = RenderTargetKind.CanonicalRaster;
    public RgbaColor Background { get; init; } = new(0, 0, 0, 0);
    public IReadOnlyList<RenderNode> Nodes { get; init; } = Array.Empty<RenderNode>();
    public CancellationToken Cancellation { get; init; }
}

public abstract record RenderOutcome(Guid SceneId)
{
    public sealed record Succeeded(
        Guid SceneId,
        RenderTargetKind Target,
        int PixelWidth,
        int PixelHeight,
        IImageResult? CanonicalRaster) : RenderOutcome(SceneId);

    public sealed record Cancelled(Guid SceneId) : RenderOutcome(SceneId);

    public sealed record Failed(Guid SceneId, Failure Failure) : RenderOutcome(SceneId);
}

public interface IRenderingAdapter
{
    ValueTask<RenderOutcome> RenderAsync(RenderIntent intent, CancellationToken cancellationToken);
}

public abstract record FrozenDisplayFrameSetRenderOutcome
{
    private FrozenDisplayFrameSetRenderOutcome()
    {
    }

    public sealed record Succeeded(IImageResult ImageResult)
        : FrozenDisplayFrameSetRenderOutcome;

    public sealed record Cancelled(string CancellationOrigin)
        : FrozenDisplayFrameSetRenderOutcome;

    public sealed record Failed(Failure Failure)
        : FrozenDisplayFrameSetRenderOutcome;
}

public interface IFrozenDisplayFrameSetRenderer
{
    ValueTask<FrozenDisplayFrameSetRenderOutcome> RenderAsync(
        FrozenDisplayFrameSet frameSet,
        PhysicalRect selectionPhysicalBounds,
        CancellationToken cancellationToken);
}
