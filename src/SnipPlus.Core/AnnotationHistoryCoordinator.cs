using SnipPlus.Contracts;

namespace SnipPlus.Core;

/// <summary>
/// Owns the capture-session annotation history without owning the document collection.
/// Document and AnnotationRevision changes are still performed by
/// <see cref="AnnotationDocumentCoordinator"/>.
/// </summary>
public sealed class AnnotationHistoryCoordinator
{
    private readonly object _gate = new();
    private readonly AnnotationDocumentCoordinator _documents;
    private readonly List<AnnotationHistoryEntry> _undo = [];
    private readonly List<AnnotationHistoryEntry> _redo = [];
    private Guid? _sessionId;
    private string _coordinateVersion = string.Empty;
    private int _selectionRevision;
    private int _nextNumber = 1;

    public AnnotationHistoryCoordinator(AnnotationDocumentCoordinator documents)
    {
        _documents = documents ?? throw new ArgumentNullException(nameof(documents));
    }

    public AnnotationHistoryState CurrentState
    {
        get
        {
            lock (_gate)
            {
                return Snapshot();
            }
        }
    }

    public bool HasActiveSession
    {
        get
        {
            lock (_gate)
            {
                return _sessionId is not null;
            }
        }
    }

    public int CurrentNextNumber
    {
        get
        {
            lock (_gate)
            {
                return _nextNumber;
            }
        }
    }

    public void BeginSession(Guid sessionId, string coordinateVersion, int selectionRevision, int nextNumber = 1)
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

        ValidateNextNumber(nextNumber);
        lock (_gate)
        {
            _sessionId = sessionId;
            _coordinateVersion = coordinateVersion;
            _selectionRevision = selectionRevision;
            _nextNumber = nextNumber;
            _undo.Clear();
            _redo.Clear();
        }
    }

    public void UpdateSelection(Guid sessionId, string coordinateVersion, int selectionRevision)
    {
        lock (_gate)
        {
            if (!MatchesSession(sessionId, coordinateVersion))
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfNegative(selectionRevision);

            _selectionRevision = selectionRevision;
        }
    }

    public void ClearSession(Guid sessionId)
    {
        lock (_gate)
        {
            if (_sessionId != sessionId)
            {
                return;
            }

            _sessionId = null;
            _coordinateVersion = string.Empty;
            _selectionRevision = 0;
            _nextNumber = 1;
            _undo.Clear();
            _redo.Clear();
        }
    }

    public void RecordAdd(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationDocument before,
        AnnotationDocument after,
        AnnotationObject annotationObject)
    {
        Record(sessionId, coordinateVersion, selectionRevision, before, after,
            new AnnotationHistoryEntry(
                sessionId, coordinateVersion, selectionRevision,
                AnnotationHistoryEntryKind.Add,
                afterObject: annotationObject));
    }

    public void RecordRemove(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationDocument before,
        AnnotationDocument after,
        AnnotationObject annotationObject)
    {
        Record(sessionId, coordinateVersion, selectionRevision, before, after,
            new AnnotationHistoryEntry(
                sessionId, coordinateVersion, selectionRevision,
                AnnotationHistoryEntryKind.Remove,
                beforeObject: annotationObject));
    }

    public void RecordReplace(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationDocument before,
        AnnotationDocument after,
        AnnotationObject beforeObject,
        AnnotationObject afterObject)
    {
        Record(sessionId, coordinateVersion, selectionRevision, before, after,
            new AnnotationHistoryEntry(
                sessionId, coordinateVersion, selectionRevision,
                AnnotationHistoryEntryKind.Replace,
                beforeObject,
                afterObject));
    }

    public void RecordNextNumber(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationDocument document,
        int beforeNextNumber,
        int afterNextNumber)
    {
        ValidateNextNumber(beforeNextNumber);
        ValidateNextNumber(afterNextNumber);
        Record(sessionId, coordinateVersion, selectionRevision, document, document,
            new AnnotationHistoryEntry(
                sessionId, coordinateVersion, selectionRevision,
                AnnotationHistoryEntryKind.NextNumberChange,
                beforeNextNumber: beforeNextNumber,
                afterNextNumber: afterNextNumber));
    }

    public void RecordNumberedMarkerCreation(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationDocument before,
        AnnotationDocument after,
        AnnotationObject annotationObject,
        int beforeNextNumber,
        int afterNextNumber)
    {
        ValidateNextNumber(beforeNextNumber);
        ValidateNextNumber(afterNextNumber);
        Record(sessionId, coordinateVersion, selectionRevision, before, after,
            new AnnotationHistoryEntry(
                sessionId, coordinateVersion, selectionRevision,
                AnnotationHistoryEntryKind.NumberedMarkerCreation,
                afterObject: annotationObject,
                beforeNextNumber: beforeNextNumber,
                afterNextNumber: afterNextNumber));
    }

    public AnnotationHistoryResult Execute(
        AnnotationHistoryRequest request,
        WorkflowState workflowState,
        bool hasActiveDraft)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            var current = _documents.Current;
            if (!MatchesSession(request.SessionId, request.CoordinateVersion))
            {
                return Result(request, AnnotationHistoryResultKind.StaleSession,
                    current, null, null, "The annotation history request belongs to a stale capture session.");
            }

            if (_selectionRevision != request.SelectionRevision)
            {
                return Result(request, AnnotationHistoryResultKind.StaleSelectionRevision,
                    current, null, null, "The annotation history request belongs to a stale Selection revision.");
            }

            if (current is null || current.Revision != request.ExpectedAnnotationRevision)
            {
                return Result(request, AnnotationHistoryResultKind.StaleAnnotationRevision,
                    current, null, null, "The annotation history request belongs to a stale Annotation revision.");
            }

            if (workflowState != WorkflowState.Editing)
            {
                return Result(request, AnnotationHistoryResultKind.InvalidWorkflowState,
                    current, null, null, "Annotation history is available only while the workflow is Editing.");
            }

            if (hasActiveDraft)
            {
                return Result(request, AnnotationHistoryResultKind.ActiveDraft,
                    current, null, null, "Finish or cancel the active annotation draft before using history.");
            }

            var source = request.Command == AnnotationHistoryCommand.Undo ? _undo : _redo;
            var destination = request.Command == AnnotationHistoryCommand.Undo ? _redo : _undo;
            if (source.Count == 0)
            {
                return Result(
                    request,
                    request.Command == AnnotationHistoryCommand.Undo
                        ? AnnotationHistoryResultKind.NothingToUndo
                        : AnnotationHistoryResultKind.NothingToRedo,
                    current,
                    null,
                    null,
                    request.Command == AnnotationHistoryCommand.Undo
                        ? "There is nothing to undo."
                        : "There is nothing to redo.");
            }

            var entry = source[^1];
            if (!TryReplay(entry, request.Command == AnnotationHistoryCommand.Undo,
                    current, out var document, out var nextNumber, out var failureMessage))
            {
                return Result(request, failureMessage == "Annotation revision overflow."
                        ? AnnotationHistoryResultKind.RevisionOverflow
                        : AnnotationHistoryResultKind.ObjectConflict,
                    current, entry.AffectedObjectId, entry.Kind, failureMessage);
            }

            source.RemoveAt(source.Count - 1);
            destination.Add(entry);
            _nextNumber = nextNumber;
            return Result(request, AnnotationHistoryResultKind.Succeeded,
                document, entry.AffectedObjectId, entry.Kind,
                request.Command == AnnotationHistoryCommand.Undo
                    ? "Annotation change undone."
                    : "Annotation change redone.");
        }
    }

    private void Record(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        AnnotationDocument before,
        AnnotationDocument after,
        AnnotationHistoryEntry entry)
    {
        ArgumentNullException.ThrowIfNull(before);
        ArgumentNullException.ThrowIfNull(after);
        if (before.SessionId != sessionId || after.SessionId != sessionId
            || before.Revision.Value >= after.Revision.Value && entry.Kind != AnnotationHistoryEntryKind.NextNumberChange
            || !MatchesEntry(entry))
        {
            throw new InvalidOperationException("The successful annotation mutation does not match the active history session.");
        }

        lock (_gate)
        {
            if (!MatchesSession(sessionId, coordinateVersion)
                || _selectionRevision != selectionRevision)
            {
                throw new InvalidOperationException("The successful annotation mutation belongs to a stale history context.");
            }

            _undo.Add(entry);
            _redo.Clear();
            if (entry.AfterNextNumber is int nextNumber)
            {
                _nextNumber = nextNumber;
            }
        }
    }

    private bool TryReplay(
        AnnotationHistoryEntry entry,
        bool undo,
        AnnotationDocument current,
        out AnnotationDocument? document,
        out int nextNumber,
        out string failureMessage)
    {
        document = current;
        nextNumber = _nextNumber;
        failureMessage = string.Empty;

        switch (entry.Kind)
        {
            case AnnotationHistoryEntryKind.NextNumberChange:
                nextNumber = undo ? entry.BeforeNextNumber!.Value : entry.AfterNextNumber!.Value;
                return true;
            case AnnotationHistoryEntryKind.Add:
            case AnnotationHistoryEntryKind.NumberedMarkerCreation:
                if (undo)
                {
                    if (!HasExact(current, entry.AfterObject!))
                    {
                        failureMessage = "The added annotation no longer matches the history entry.";
                        return false;
                    }

                    var removed = _documents.Remove(new RemoveAnnotationObjectRequest(
                        current.SessionId, current.Revision, entry.AfterObject!.ObjectId));
                    if (removed is not AnnotationMutationResult.Succeeded succeeded)
                    {
                        failureMessage = MutationFailureMessage(removed);
                        return false;
                    }

                    document = succeeded.Document;
                    if (entry.Kind == AnnotationHistoryEntryKind.NumberedMarkerCreation)
                    {
                        nextNumber = entry.BeforeNextNumber!.Value;
                    }

                    return true;
                }

                if (Find(current, entry.AfterObject!.ObjectId) is not null)
                {
                    failureMessage = "The annotation already exists and cannot be redone.";
                    return false;
                }

                var added = _documents.Add(new AddAnnotationObjectRequest(
                    current.SessionId, current.Revision, entry.AfterObject));
                if (added is not AnnotationMutationResult.Succeeded addedSucceeded)
                {
                    failureMessage = MutationFailureMessage(added);
                    return false;
                }

                document = addedSucceeded.Document;
                if (entry.Kind == AnnotationHistoryEntryKind.NumberedMarkerCreation)
                {
                    nextNumber = entry.AfterNextNumber!.Value;
                }

                return true;
            case AnnotationHistoryEntryKind.Remove:
                if (undo)
                {
                    if (Find(current, entry.BeforeObject!.ObjectId) is not null)
                    {
                        failureMessage = "The removed annotation already exists and cannot be undone.";
                        return false;
                    }

                    var restored = _documents.Add(new AddAnnotationObjectRequest(
                        current.SessionId, current.Revision, entry.BeforeObject));
                    if (restored is not AnnotationMutationResult.Succeeded restoredSucceeded)
                    {
                        failureMessage = MutationFailureMessage(restored);
                        return false;
                    }

                    document = restoredSucceeded.Document;
                    return true;
                }

                if (!HasExact(current, entry.BeforeObject!))
                {
                    failureMessage = "The removed annotation no longer matches the history entry.";
                    return false;
                }

                var deleted = _documents.Remove(new RemoveAnnotationObjectRequest(
                    current.SessionId, current.Revision, entry.BeforeObject!.ObjectId));
                if (deleted is not AnnotationMutationResult.Succeeded deletedSucceeded)
                {
                    failureMessage = MutationFailureMessage(deleted);
                    return false;
                }

                document = deletedSucceeded.Document;
                return true;
            case AnnotationHistoryEntryKind.Replace:
                var expected = undo ? entry.AfterObject! : entry.BeforeObject!;
                var replacement = undo ? entry.BeforeObject! : entry.AfterObject!;
                if (!HasExact(current, expected))
                {
                    failureMessage = "The replaced annotation no longer matches the history entry.";
                    return false;
                }

                var replaced = _documents.Replace(new ReplaceAnnotationObjectRequest(
                    current.SessionId, current.Revision, replacement));
                if (replaced is not AnnotationMutationResult.Succeeded replacedSucceeded)
                {
                    failureMessage = MutationFailureMessage(replaced);
                    return false;
                }

                document = replacedSucceeded.Document;
                return true;
            default:
                failureMessage = "The annotation history entry is unsupported.";
                return false;
        }
    }

    private bool MatchesEntry(AnnotationHistoryEntry entry) =>
        MatchesSession(entry.SessionId, entry.CoordinateVersion)
        && entry.SelectionRevision == _selectionRevision;

    private bool MatchesSession(Guid sessionId, string coordinateVersion) =>
        _sessionId == sessionId
        && string.Equals(_coordinateVersion, coordinateVersion, StringComparison.Ordinal);

    private AnnotationHistoryState Snapshot() => new(
        _sessionId,
        _coordinateVersion,
        _selectionRevision,
        _documents.Current?.Revision ?? AnnotationRevision.Initial,
        _nextNumber,
        _undo,
        _redo,
        _undo.Count > 0,
        _redo.Count > 0);

    private AnnotationHistoryResult Result(
        AnnotationHistoryRequest request,
        AnnotationHistoryResultKind kind,
        AnnotationDocument? document,
        AnnotationObjectId? affectedObjectId,
        AnnotationHistoryEntryKind? entryKind,
        string message) => new(
        kind,
        request.Command,
        request.SessionId,
        request.CoordinateVersion,
        request.SelectionRevision,
        document?.Revision ?? AnnotationRevision.Initial,
        _nextNumber,
        document,
        affectedObjectId,
        entryKind,
        _undo.Count > 0,
        _redo.Count > 0,
        null,
        message);

    private static AnnotationObject? Find(AnnotationDocument document, AnnotationObjectId objectId) =>
        document.Objects.FirstOrDefault(value => value.ObjectId == objectId);

    private static bool HasExact(AnnotationDocument document, AnnotationObject annotationObject) =>
        Find(document, annotationObject.ObjectId)?.Equals(annotationObject) == true;

    private static string MutationFailureMessage(AnnotationMutationResult result) => result switch
    {
        AnnotationMutationResult.RevisionOverflow => "Annotation revision overflow.",
        AnnotationMutationResult.DuplicateObjectId => "The annotation object already exists.",
        AnnotationMutationResult.ObjectNotFound => "The annotation object was not found.",
        _ => "The annotation history mutation failed."
    };

    private static void ValidateNextNumber(int nextNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(nextNumber);
    }
}
