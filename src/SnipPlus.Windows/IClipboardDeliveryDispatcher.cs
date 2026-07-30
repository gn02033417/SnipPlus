using Microsoft.UI.Dispatching;

namespace SnipPlus.Windows;

public interface IClipboardDeliveryDispatcher
{
    bool HasThreadAccess { get; }

    bool TryEnqueue(Func<Task> callback);
}

public sealed class DispatcherQueueClipboardDeliveryDispatcher : IClipboardDeliveryDispatcher
{
    private readonly DispatcherQueue _dispatcherQueue;

    public DispatcherQueueClipboardDeliveryDispatcher(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
    }

    public bool HasThreadAccess => _dispatcherQueue.HasThreadAccess;

    public bool TryEnqueue(Func<Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return _dispatcherQueue.TryEnqueue(() => _ = callback());
    }
}
