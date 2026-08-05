namespace SnipPlus.Contracts;

public enum AnnotationHistoryEntryKind
{
    Add,
    Remove,
    Replace,
    NextNumberChange,
    NumberedMarkerCreation
}

public enum AnnotationHistoryCommand
{
    Undo,
    Redo
}

public sealed record AnnotationHistoryEntry
{
    public AnnotationHistoryEntry(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationHistoryEntryKind kind,
        AnnotationObject? beforeObject = null,
        AnnotationObject? afterObject = null,
        int? beforeNextNumber = null,
        int? afterNextNumber = null)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("A session identifier is required.", nameof(sessionId));
        }

        if (string.IsNullOrWhiteSpace(coordinateVersion))
        {
            throw new ArgumentException("A coordinate version is required.", nameof(coordinateVersion));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(selectionRevision);

        switch (kind)
        {
            case AnnotationHistoryEntryKind.Add:
                RequireObject(afterObject, nameof(afterObject));
                RequireNull(beforeObject, nameof(beforeObject));
                RequireNull(beforeNextNumber, nameof(beforeNextNumber));
                RequireNull(afterNextNumber, nameof(afterNextNumber));
                break;
            case AnnotationHistoryEntryKind.Remove:
                RequireObject(beforeObject, nameof(beforeObject));
                RequireNull(afterObject, nameof(afterObject));
                RequireNull(beforeNextNumber, nameof(beforeNextNumber));
                RequireNull(afterNextNumber, nameof(afterNextNumber));
                break;
            case AnnotationHistoryEntryKind.Replace:
                RequireObject(beforeObject, nameof(beforeObject));
                RequireObject(afterObject, nameof(afterObject));
                if (beforeObject!.ObjectId != afterObject!.ObjectId)
                {
                    throw new ArgumentException("Replace entries must retain the same ObjectId.", nameof(afterObject));
                }

                if (beforeObject.Equals(afterObject))
                {
                    throw new ArgumentException("Replace entries must represent a change.", nameof(afterObject));
                }

                RequireNull(beforeNextNumber, nameof(beforeNextNumber));
                RequireNull(afterNextNumber, nameof(afterNextNumber));
                break;
            case AnnotationHistoryEntryKind.NextNumberChange:
                RequireNull(beforeObject, nameof(beforeObject));
                RequireNull(afterObject, nameof(afterObject));
                RequirePositive(beforeNextNumber, nameof(beforeNextNumber));
                RequirePositive(afterNextNumber, nameof(afterNextNumber));
                if (beforeNextNumber == afterNextNumber)
                {
                    throw new ArgumentException("Next number entries must represent a change.", nameof(afterNextNumber));
                }

                break;
            case AnnotationHistoryEntryKind.NumberedMarkerCreation:
                RequireObject(afterObject, nameof(afterObject));
                RequireNull(beforeObject, nameof(beforeObject));
                RequirePositive(beforeNextNumber, nameof(beforeNextNumber));
                RequirePositive(afterNextNumber, nameof(afterNextNumber));
                if (afterObject!.ToolKind != AnnotationToolKind.NumberedMarker)
                {
                    throw new ArgumentException("Marker creation entries require a Numbered Marker object.", nameof(afterObject));
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind));
        }

        SessionId = sessionId;
        CoordinateVersion = coordinateVersion;
        SelectionRevision = selectionRevision;
        Kind = kind;
        BeforeObject = beforeObject;
        AfterObject = afterObject;
        BeforeNextNumber = beforeNextNumber;
        AfterNextNumber = afterNextNumber;
    }

    public Guid SessionId { get; }

    public string CoordinateVersion { get; }

    public int SelectionRevision { get; }

    public AnnotationHistoryEntryKind Kind { get; }

    public AnnotationObject? BeforeObject { get; }

    public AnnotationObject? AfterObject { get; }

    public int? BeforeNextNumber { get; }

    public int? AfterNextNumber { get; }

    public AnnotationObjectId? AffectedObjectId =>
        AfterObject?.ObjectId ?? BeforeObject?.ObjectId;

    private static void RequireObject(AnnotationObject? value, string parameterName)
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    private static void RequireNull<T>(T? value, string parameterName)
        where T : struct
    {
        if (value is not null)
        {
            throw new ArgumentException("This history entry does not support the supplied value.", parameterName);
        }
    }

    private static void RequireNull(AnnotationObject? value, string parameterName)
    {
        if (value is not null)
        {
            throw new ArgumentException("This history entry does not support the supplied value.", parameterName);
        }
    }

    private static void RequirePositive(int? value, string parameterName)
    {
        if (value is not > 0)
        {
            throw new ArgumentOutOfRangeException(parameterName);
        }
    }
}

public sealed record AnnotationHistoryRequest(
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision ExpectedAnnotationRevision,
    AnnotationHistoryCommand Command);

public enum AnnotationHistoryResultKind
{
    Succeeded,
    Disabled,
    NothingToUndo,
    NothingToRedo,
    StaleSession,
    StaleSelectionRevision,
    StaleAnnotationRevision,
    InvalidWorkflowState,
    ActiveDraft,
    ObjectConflict,
    RevisionOverflow,
    Failed
}

public sealed record AnnotationHistoryResult(
    AnnotationHistoryResultKind Kind,
    AnnotationHistoryCommand Command,
    Guid SessionId,
    string CoordinateVersion,
    int SelectionRevision,
    AnnotationRevision CurrentAnnotationRevision,
    int CurrentNextNumber,
    AnnotationDocument? Document,
    AnnotationObjectId? AffectedObjectId,
    AnnotationHistoryEntryKind? EntryKind,
    bool CanUndo,
    bool CanRedo,
    Failure? Failure,
    string Message);

public sealed class AnnotationHistoryState
{
    public AnnotationHistoryState(
        Guid? sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationRevision currentAnnotationRevision,
        int currentNextNumber,
        IReadOnlyList<AnnotationHistoryEntry> undoEntries,
        IReadOnlyList<AnnotationHistoryEntry> redoEntries,
        bool canUndo,
        bool canRedo)
    {
        SessionId = sessionId;
        CoordinateVersion = coordinateVersion;
        SelectionRevision = selectionRevision;
        CurrentAnnotationRevision = currentAnnotationRevision;
        CurrentNextNumber = currentNextNumber;
        UndoEntries = Array.AsReadOnly((undoEntries ?? throw new ArgumentNullException(nameof(undoEntries))).ToArray());
        RedoEntries = Array.AsReadOnly((redoEntries ?? throw new ArgumentNullException(nameof(redoEntries))).ToArray());
        CanUndo = canUndo;
        CanRedo = canRedo;
    }

    public Guid? SessionId { get; }

    public string CoordinateVersion { get; }

    public int SelectionRevision { get; }

    public AnnotationRevision CurrentAnnotationRevision { get; }

    public int CurrentNextNumber { get; }

    public IReadOnlyList<AnnotationHistoryEntry> UndoEntries { get; }

    public IReadOnlyList<AnnotationHistoryEntry> RedoEntries { get; }

    public bool CanUndo { get; }

    public bool CanRedo { get; }
}
