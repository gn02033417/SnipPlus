namespace SnipPlus.Contracts;

public enum FailureCategory
{
    Validation,
    Unsupported,
    Permission,
    Contention,
    Resource,
    Device,
    Session,
    IO,
    Cancelled,
    Unexpected
}

public enum FailureSeverity
{
    Info,
    Warning,
    Error
}

public enum FailureRecoverability
{
    RetrySameIntent,
    RetryNewIntent,
    UserActionRequired,
    TerminalForSession
}

public enum FailureCode
{
    InvalidStateTransition,
    InvalidCaptureIntent,
    InvalidCoordinateMapping,
    UnsupportedCapture,
    UnsupportedCapacity,
    CapturePermissionDenied,
    CaptureSourceUnavailable,
    CaptureSourceClosed,
    CaptureFrameTimeout,
    CaptureFrameSizeChanged,
    CaptureDeviceLost,
    DisplayTopologyUnavailable,
    DisplayTopologyInvalid,
    DisplayContextChanged,
    StaleSession,
    OverlayCreationFailed,
    OverlayPresentationFailed,
    InvalidPointerMapping,
    InvalidSelection,
    PartialAcquisitionFailed,
    ProtectedContentUnavailable,
    InvalidResultLifetime,
    RenderingResourceLost,
    RenderingFailed,
    InvalidWorkArea,
    BarMeasurementFailed,
    FunctionBarPlacementFailed,
    FunctionBarPresentationFailed,
    StaleSelectionRevision,
    EncodingFailed,
    ClipboardBusy,
    ClipboardPublicationRejected,
    OutputAccessDenied,
    OutputWriteFailed,
    Cancelled,
    UnexpectedFailure
}

public sealed record Failure(
    FailureCode Code,
    FailureCategory Category,
    FailureSeverity Severity,
    FailureRecoverability Recoverability,
    string UserMessageKey,
    string DiagnosticMessage,
    int? NativeCode,
    string Operation,
    Guid CorrelationId,
    DateTimeOffset OccurredAt,
    Failure? InnerFailure = null)
{
    public static Failure Create(
        FailureCode code,
        FailureCategory category,
        FailureRecoverability recoverability,
        string operation,
        Guid correlationId,
        string diagnosticMessage,
        FailureSeverity severity = FailureSeverity.Error,
        string? userMessageKey = null,
        int? nativeCode = null,
        Failure? innerFailure = null,
        DateTimeOffset? occurredAt = null)
    {
        return new Failure(
            code,
            category,
            severity,
            recoverability,
            userMessageKey ?? $"Failure.{code}",
            diagnosticMessage,
            nativeCode,
            operation,
            correlationId,
            occurredAt ?? DateTimeOffset.UtcNow,
            innerFailure);
    }
}
