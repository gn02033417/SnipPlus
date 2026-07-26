using SnipPlus.Contracts;

namespace SnipPlus.Core;

public abstract record CoordinateMappingResult
{
    private CoordinateMappingResult()
    {
    }

    public sealed record Success(CaptureIntent Intent) : CoordinateMappingResult;

    public sealed record FailureResult(Failure Failure) : CoordinateMappingResult;
}

public static class CoordinateMapper
{
    public static CoordinateMappingResult CreateMonitorIntent(
        DisplayContextSnapshot displayContext,
        DipRect selectionDipBounds,
        Guid requestId,
        Guid sessionId,
        DateTimeOffset requestedAt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(displayContext.SourceId)
            || string.IsNullOrWhiteSpace(displayContext.CoordinateVersion)
            || !displayContext.SourcePhysicalBounds.IsPositive
            || !selectionDipBounds.IsPositive
            || !double.IsFinite(displayContext.DpiScaleX)
            || !double.IsFinite(displayContext.DpiScaleY)
            || displayContext.DpiScaleX <= 0
            || displayContext.DpiScaleY <= 0)
        {
            return Invalid(displayContext.SourceId, requestId, "Display context or selection is invalid.");
        }

        var source = displayContext.SourcePhysicalBounds;
        var sourceDipWidth = source.Width / displayContext.DpiScaleX;
        var sourceDipHeight = source.Height / displayContext.DpiScaleY;
        if (selectionDipBounds.Left < 0
            || selectionDipBounds.Top < 0
            || selectionDipBounds.Right > sourceDipWidth
            || selectionDipBounds.Bottom > sourceDipHeight)
        {
            return Invalid(displayContext.SourceId, requestId, "Selection is outside the captured source bounds.");
        }

        try
        {
            var cropLeft = checked((int)Math.Floor(selectionDipBounds.Left * displayContext.DpiScaleX));
            var cropTop = checked((int)Math.Floor(selectionDipBounds.Top * displayContext.DpiScaleY));
            var cropRight = checked((int)Math.Ceiling(selectionDipBounds.Right * displayContext.DpiScaleX));
            var cropBottom = checked((int)Math.Ceiling(selectionDipBounds.Bottom * displayContext.DpiScaleY));
            var crop = new PhysicalRect(cropLeft, cropTop, cropRight, cropBottom);
            if (!crop.IsPositive || !source.Contains(new PhysicalRect(source.Left + crop.Left, source.Top + crop.Top, source.Left + crop.Right, source.Top + crop.Bottom)))
            {
                return Invalid(displayContext.SourceId, requestId, "Rounded crop is empty or outside the source bounds.");
            }

            var selectionPhysical = new PhysicalRect(
                checked(source.Left + crop.Left),
                checked(source.Top + crop.Top),
                checked(source.Left + crop.Right),
                checked(source.Top + crop.Bottom));

            return new CoordinateMappingResult.Success(new CaptureIntent
            {
                RequestId = requestId,
                SessionId = sessionId,
                SourceKind = SourceKind.Monitor,
                SourceId = displayContext.SourceId,
                SourcePhysicalBounds = source,
                SelectionDipBounds = selectionDipBounds,
                SelectionPhysicalBounds = selectionPhysical,
                CropBoundsInSource = crop,
                DpiScaleX = displayContext.DpiScaleX,
                DpiScaleY = displayContext.DpiScaleY,
                CoordinateVersion = displayContext.CoordinateVersion,
                IncludeCursor = false,
                RequestedAt = requestedAt,
                Cancellation = cancellationToken
            });
        }
        catch (OverflowException)
        {
            return Invalid(displayContext.SourceId, requestId, "Coordinate conversion overflowed the physical-pixel range.");
        }
    }

    private static CoordinateMappingResult.FailureResult Invalid(string sourceId, Guid requestId, string diagnosticMessage)
    {
        return new CoordinateMappingResult.FailureResult(Failure.Create(
            FailureCode.InvalidCoordinateMapping,
            FailureCategory.Validation,
            FailureRecoverability.RetryNewIntent,
            "CoordinateMapper.CreateMonitorIntent",
            requestId,
            $"Source '{sourceId}' coordinate mapping rejected: {diagnosticMessage}"));
    }
}
