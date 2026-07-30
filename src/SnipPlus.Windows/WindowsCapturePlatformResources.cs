using Microsoft.Graphics.Canvas;

using SnipPlus.Contracts;

namespace SnipPlus.Windows;

public sealed class WindowsCapturePlatformResources : IDisposable
{
    public WindowsCapturePlatformResources(IFunctionBarPlacementService functionBarPlacementService)
    {
        ArgumentNullException.ThrowIfNull(functionBarPlacementService);
        CanvasDevice = CanvasDevice.GetSharedDevice();
        TopologyProvider = new WindowsDisplayTopologyProvider();
        AdapterFactory = new WindowsDisplayCaptureAdapterFactory(CanvasDevice);
        FrameProvider = new WindowsFrozenDisplayFrameSetProvider(
            AdapterFactory,
            TopologyProvider);
        OverlayCoordinator = new WindowsFrozenDisplayOverlayCoordinator(functionBarPlacementService);
        FinalRenderer = new WindowsFrozenDisplayFrameSetRenderer();
        ClipboardDelivery = new WinRtClipboardDeliveryAdapter();
    }

    public CanvasDevice CanvasDevice { get; }

    public WindowsDisplayTopologyProvider TopologyProvider { get; }

    public WindowsDisplayCaptureAdapterFactory AdapterFactory { get; }

    public WindowsFrozenDisplayFrameSetProvider FrameProvider { get; }

    public WindowsFrozenDisplayOverlayCoordinator OverlayCoordinator { get; }

    public WindowsFrozenDisplayFrameSetRenderer FinalRenderer { get; }

    public WinRtClipboardDeliveryAdapter ClipboardDelivery { get; }

    public void Dispose()
    {
        OverlayCoordinator.Dispose();
        FrameProvider.Dispose();
        AdapterFactory.Dispose();
        CanvasDevice.Dispose();
        GC.SuppressFinalize(this);
    }
}
