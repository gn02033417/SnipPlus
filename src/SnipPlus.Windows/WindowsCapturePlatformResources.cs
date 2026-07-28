using Microsoft.Graphics.Canvas;

namespace SnipPlus.Windows;

public sealed class WindowsCapturePlatformResources : IDisposable
{
    public WindowsCapturePlatformResources()
    {
        CanvasDevice = CanvasDevice.GetSharedDevice();
        TopologyProvider = new WindowsDisplayTopologyProvider();
        AdapterFactory = new WindowsDisplayCaptureAdapterFactory(CanvasDevice);
        FrameProvider = new WindowsFrozenDisplayFrameSetProvider(
            AdapterFactory,
            TopologyProvider);
        OverlayCoordinator = new WindowsFrozenDisplayOverlayCoordinator();
    }

    public CanvasDevice CanvasDevice { get; }

    public WindowsDisplayTopologyProvider TopologyProvider { get; }

    public WindowsDisplayCaptureAdapterFactory AdapterFactory { get; }

    public WindowsFrozenDisplayFrameSetProvider FrameProvider { get; }

    public WindowsFrozenDisplayOverlayCoordinator OverlayCoordinator { get; }

    public void Dispose()
    {
        OverlayCoordinator.Dispose();
        FrameProvider.Dispose();
        AdapterFactory.Dispose();
        CanvasDevice.Dispose();
        GC.SuppressFinalize(this);
    }
}
