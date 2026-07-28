namespace SnipPlus.Contracts;

public interface IDisplayTopologyProvider
{
    ValueTask<DisplayTopologyOutcome> GetSnapshotAsync(
        CaptureRequest request,
        CancellationToken cancellationToken);
}

public interface IForegroundContextProvider
{
    ValueTask<ForegroundContextReference?> CaptureAsync(
        CaptureRequest request,
        CancellationToken cancellationToken);
}

public interface IFrozenDisplayFrameProvider
{
    ValueTask<FrozenDisplayFrameAcquisitionOutcome> AcquireAsync(
        CaptureSessionContext session,
        DisplaySnapshot display,
        CancellationToken cancellationToken);
}

public interface IAllDisplayFrameProvider : IFrozenDisplayFrameProvider
{
    ValueTask<FrozenDisplayFrameSetAcquisitionOutcome> AcquireAllAsync(
        CaptureSessionContext session,
        CancellationToken cancellationToken);
}

public interface IFreezingBoundary
{
    ValueTask<CaptureFreezingOutcome> BeginFreezingAsync(
        CaptureRequest request,
        CancellationToken cancellationToken);

    ValueTask<CaptureFreezingOutcome> AcquireFrozenFramesAsync(
        CaptureSessionContext session,
        CancellationToken cancellationToken);
}

public abstract record DisplayTopologyOutcome
{
    private DisplayTopologyOutcome()
    {
    }

    public sealed record Succeeded(VirtualDesktopSnapshot Snapshot) : DisplayTopologyOutcome;

    public sealed record Cancelled(string CancellationOrigin) : DisplayTopologyOutcome;

    public sealed record Invalid(Failure Failure) : DisplayTopologyOutcome;
}

public abstract record FrozenDisplayFrameAcquisitionOutcome
{
    private FrozenDisplayFrameAcquisitionOutcome()
    {
    }

    public sealed record Succeeded(FrozenDisplayFrame Frame) : FrozenDisplayFrameAcquisitionOutcome;

    public sealed record Cancelled(string CancellationOrigin) : FrozenDisplayFrameAcquisitionOutcome;

    public sealed record Failed(Failure Failure) : FrozenDisplayFrameAcquisitionOutcome;
}

public abstract record FrozenDisplayFrameSetAcquisitionOutcome
{
    private FrozenDisplayFrameSetAcquisitionOutcome()
    {
    }

    public sealed record Succeeded(FrozenDisplayFrameSet FrameSet) : FrozenDisplayFrameSetAcquisitionOutcome;

    public sealed record Cancelled(
        string CancellationOrigin,
        bool CleanupCompleted) : FrozenDisplayFrameSetAcquisitionOutcome;

    public sealed record Failed(
        Failure Failure,
        bool CleanupCompleted) : FrozenDisplayFrameSetAcquisitionOutcome;
}

public abstract record CaptureFreezingOutcome
{
    private CaptureFreezingOutcome()
    {
    }

    public sealed record FreezingStarted(CaptureSessionContext Session) : CaptureFreezingOutcome;

    public sealed record FrozenFrameSetReady(CaptureSessionContext Session) : CaptureFreezingOutcome;

    public sealed record UnsupportedCapacity(CapacityValidationOutcome Validation) : CaptureFreezingOutcome;

    public sealed record TopologyInvalid(Failure Failure) : CaptureFreezingOutcome;

    public sealed record FrameFailed(Guid SessionId, Failure Failure, bool CleanupCompleted) : CaptureFreezingOutcome;

    public sealed record Cancelled(Guid RequestId, Guid? SessionId, string CancellationOrigin) : CaptureFreezingOutcome;

    public sealed record StaleRequest(Guid RequestId, string UserMessage) : CaptureFreezingOutcome;

    public sealed record AlreadyStarted(Guid RequestId, Guid? SessionId, string UserMessage) : CaptureFreezingOutcome;

    public sealed record Busy(Guid RequestId, string UserMessage) : CaptureFreezingOutcome;
}
