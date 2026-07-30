using SnipPlus.Contracts;

namespace SnipPlus.Core;

public sealed class AnnotationDocumentCoordinator
{
    private readonly object _gate = new();
    private AnnotationDocument? _current;

    public AnnotationDocument? Current
    {
        get
        {
            lock (_gate)
            {
                return _current;
            }
        }
    }

    public AnnotationDocument BeginSession(Guid sessionId)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Annotation session identifier is required.", nameof(sessionId));
        }

        lock (_gate)
        {
            if (_current is not null)
            {
                if (_current.SessionId != sessionId)
                {
                    throw new InvalidOperationException(
                        "The current Annotation Document belongs to another capture session.");
                }

                return _current;
            }

            return _current = AnnotationDocument.CreateEmpty(sessionId);
        }
    }

    public bool ClearSession(Guid sessionId)
    {
        lock (_gate)
        {
            if (_current?.SessionId != sessionId)
            {
                return false;
            }

            _current = null;
            return true;
        }
    }

    public AnnotationMutationResult Add(AddAnnotationObjectRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            if (!TryGetCurrent(request, out var document, out var rejection))
            {
                return rejection!;
            }

            if (request.AnnotationObject is not AnnotationObject annotationObject)
            {
                return new AnnotationMutationResult.InvalidObject(
                    "An Annotation object is required.",
                    document!);
            }

            if (!IsValidObject(annotationObject, document!.SessionId))
            {
                return new AnnotationMutationResult.InvalidObject(
                    "The Annotation object does not belong to this session or has invalid geometry.",
                    document);
            }

            if (document.Objects.Any(existing => existing.ObjectId == annotationObject.ObjectId))
            {
                return new AnnotationMutationResult.DuplicateObjectId(
                    annotationObject.ObjectId,
                    document);
            }

            if (!document.Revision.TryIncrement(out var nextRevision))
            {
                return new AnnotationMutationResult.RevisionOverflow(document);
            }

            _current = new AnnotationDocument(
                document.SessionId,
                nextRevision,
                document.Objects.Append(annotationObject));
            return new AnnotationMutationResult.Succeeded(_current);
        }
    }

    public AnnotationMutationResult Replace(ReplaceAnnotationObjectRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            if (!TryGetCurrent(request, out var document, out var rejection))
            {
                return rejection!;
            }

            if (request.AnnotationObject is not AnnotationObject annotationObject)
            {
                return new AnnotationMutationResult.InvalidObject(
                    "An Annotation object is required.",
                    document!);
            }

            if (!IsValidObject(annotationObject, document!.SessionId))
            {
                return new AnnotationMutationResult.InvalidObject(
                    "The Annotation object does not belong to this session or has invalid geometry.",
                    document);
            }

            var index = document.Objects
                .Select((existing, index) => (existing, index))
                .FirstOrDefault(pair => pair.existing.ObjectId == annotationObject.ObjectId)
                .index;
            if (index < 0 || index >= document.Objects.Count || document.Objects[index].ObjectId != annotationObject.ObjectId)
            {
                return new AnnotationMutationResult.ObjectNotFound(
                    annotationObject.ObjectId,
                    document);
            }

            if (document.Objects[index].Equals(annotationObject))
            {
                return new AnnotationMutationResult.NoChange(document);
            }

            if (!document.Revision.TryIncrement(out var nextRevision))
            {
                return new AnnotationMutationResult.RevisionOverflow(document);
            }

            var objects = document.Objects.ToArray();
            objects[index] = annotationObject;
            _current = new AnnotationDocument(document.SessionId, nextRevision, objects);
            return new AnnotationMutationResult.Succeeded(_current);
        }
    }

    public AnnotationMutationResult Remove(RemoveAnnotationObjectRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        lock (_gate)
        {
            if (!TryGetCurrent(request, out var document, out var rejection))
            {
                return rejection!;
            }

            var index = document!.Objects
                .Select((existing, index) => (existing, index))
                .FirstOrDefault(pair => pair.existing.ObjectId == request.ObjectId)
                .index;
            if (index < 0 || index >= document.Objects.Count || document.Objects[index].ObjectId != request.ObjectId)
            {
                return new AnnotationMutationResult.ObjectNotFound(request.ObjectId, document);
            }

            if (!document.Revision.TryIncrement(out var nextRevision))
            {
                return new AnnotationMutationResult.RevisionOverflow(document);
            }

            var objects = document.Objects
                .Where(annotationObject => annotationObject.ObjectId != request.ObjectId);
            _current = new AnnotationDocument(document.SessionId, nextRevision, objects);
            return new AnnotationMutationResult.Succeeded(_current);
        }
    }

    private bool TryGetCurrent(
        AnnotationMutationRequest request,
        out AnnotationDocument? document,
        out AnnotationMutationResult? rejection)
    {
        document = _current;
        if (document is null || document.SessionId != request.SessionId)
        {
            rejection = new AnnotationMutationResult.StaleSession(
                request.SessionId,
                document?.SessionId,
                document);
            return false;
        }

        if (document.Revision != request.ExpectedAnnotationRevision)
        {
            rejection = new AnnotationMutationResult.StaleAnnotationRevision(
                request.ExpectedAnnotationRevision,
                document.Revision,
                document);
            return false;
        }

        rejection = null;
        return true;
    }

    private static bool IsValidObject(AnnotationObject annotationObject, Guid sessionId) =>
        annotationObject.ObjectId.IsValid
        && annotationObject.SessionId == sessionId
        && annotationObject.Geometry.IsPositive
        && annotationObject.ZOrder >= 0;
}
