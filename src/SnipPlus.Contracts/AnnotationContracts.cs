using System.Collections.ObjectModel;

namespace SnipPlus.Contracts;

public readonly record struct AnnotationObjectId
{
    public AnnotationObjectId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Annotation object identifier is required.", nameof(value));
        }

        Value = value;
    }

    public Guid Value { get; }

    public bool IsValid => Value != Guid.Empty;

    public static AnnotationObjectId New() => new(Guid.NewGuid());
}

public readonly record struct AnnotationRevision(long Value)
{
    public static AnnotationRevision Initial => new(0);

    public bool IsValid => Value >= 0;

    public bool TryIncrement(out AnnotationRevision next)
    {
        if (Value == long.MaxValue)
        {
            next = this;
            return false;
        }

        next = new AnnotationRevision(Value + 1);
        return true;
    }
}

public enum AnnotationToolKind
{
    Rectangle,
    ArrowLine,
    HighlighterStroke,
    Text,
    PrivacyRegion,
    NumberedMarker
}

public sealed class AnnotationObject : IEquatable<AnnotationObject>
{
    public AnnotationObject(
        AnnotationObjectId objectId,
        Guid sessionId,
        AnnotationToolKind toolKind,
        PhysicalRect geometry,
        int zOrder,
        IAnnotationContent? content = null)
    {
        if (!objectId.IsValid)
        {
            throw new ArgumentException("Annotation object identifier is required.", nameof(objectId));
        }

        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Annotation object session identifier is required.", nameof(sessionId));
        }

        if (!geometry.IsPositive)
        {
            throw new ArgumentException("Annotation object geometry must be a positive physical-pixel rectangle.", nameof(geometry));
        }

        if (zOrder < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(zOrder), "Annotation object Z-order cannot be negative.");
        }

        if (toolKind == AnnotationToolKind.Rectangle)
        {
            content ??= new RectangleAnnotationContent(RectangleAnnotationStyle.Default);
        }
        else if (toolKind == AnnotationToolKind.ArrowLine)
        {
            content ??= new ArrowLineAnnotationContent(
                new PhysicalLineSegment(
                    new PhysicalPoint(geometry.Left, geometry.Top),
                    new PhysicalPoint(geometry.Right, geometry.Bottom)),
                ArrowLineAnnotationStyle.Default);
        }
        else if (toolKind == AnnotationToolKind.HighlighterStroke)
        {
            content ??= new HighlighterStrokeContent(
                new PhysicalPolyline(
                [
                    new PhysicalPoint(geometry.Left, geometry.Top),
                    new PhysicalPoint(geometry.Right, geometry.Bottom)
                ]),
                HighlighterAnnotationStyle.Default);
        }
        else if (content is not null)
        {
            throw new ArgumentException(
                "Only supported annotation objects can carry annotation content.",
                nameof(content));
        }

        if (toolKind == AnnotationToolKind.Rectangle
            && content is not RectangleAnnotationContent)
        {
            throw new ArgumentException(
                "Rectangle annotation objects require Rectangle content.",
                nameof(content));
        }

        if (toolKind == AnnotationToolKind.ArrowLine
            && content is not ArrowLineAnnotationContent)
        {
            throw new ArgumentException(
                "ArrowLine annotation objects require ArrowLine content.",
                nameof(content));
        }
        else if (toolKind == AnnotationToolKind.ArrowLine
            && ((ArrowLineAnnotationContent)content!).Segment.Bounds != geometry)
        {
            throw new ArgumentException(
                "ArrowLine content bounds must match annotation geometry.",
                nameof(content));
        }

        if (toolKind == AnnotationToolKind.HighlighterStroke
            && content is not HighlighterStrokeContent)
        {
            throw new ArgumentException(
                "Highlighter annotation objects require Highlighter content.",
                nameof(content));
        }

        if (toolKind == AnnotationToolKind.HighlighterStroke
            && ((HighlighterStrokeContent)content!).Path.Bounds != geometry)
        {
            throw new ArgumentException(
                "Highlighter content bounds must match annotation geometry.",
                nameof(content));
        }

        ObjectId = objectId;
        SessionId = sessionId;
        ToolKind = toolKind;
        Geometry = geometry;
        ZOrder = zOrder;
        Content = content;
    }

    public AnnotationObjectId ObjectId { get; }

    public Guid SessionId { get; }

    public AnnotationToolKind ToolKind { get; }

    public PhysicalRect Geometry { get; }

    public int ZOrder { get; }

    public IAnnotationContent? Content { get; }

    public bool Equals(AnnotationObject? other) => other is not null
        && ObjectId == other.ObjectId
        && SessionId == other.SessionId
        && ToolKind == other.ToolKind
        && Geometry == other.Geometry
        && ZOrder == other.ZOrder
        && Equals(Content, other.Content);

    public override bool Equals(object? obj) => Equals(obj as AnnotationObject);

    public override int GetHashCode() => HashCode.Combine(
        ObjectId,
        SessionId,
        ToolKind,
        Geometry,
        ZOrder,
        Content);
}

public sealed class AnnotationDocument
{
    public AnnotationDocument(
        Guid sessionId,
        AnnotationRevision revision,
        IEnumerable<AnnotationObject> objects)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Annotation document session identifier is required.", nameof(sessionId));
        }

        if (!revision.IsValid)
        {
            throw new ArgumentException("Annotation revision must be non-negative.", nameof(revision));
        }

        ArgumentNullException.ThrowIfNull(objects);
        var ordered = objects
            .ToArray();
        if (ordered.Any(annotationObject => annotationObject is null))
        {
            throw new ArgumentException("Annotation document objects cannot contain null values.", nameof(objects));
        }

        if (ordered.Any(annotationObject => annotationObject.SessionId != sessionId))
        {
            throw new ArgumentException("Annotation document objects must belong to the document session.", nameof(objects));
        }

        if (ordered.Select(annotationObject => annotationObject.ObjectId).Distinct().Count() != ordered.Length)
        {
            throw new ArgumentException("Annotation document object identifiers must be unique.", nameof(objects));
        }

        SessionId = sessionId;
        Revision = revision;
        _objects = new ReadOnlyCollection<AnnotationObject>(
            ordered
                .OrderBy(annotationObject => annotationObject.ZOrder)
                .ThenBy(annotationObject => annotationObject.ObjectId.Value)
                .ToArray());
    }

    private readonly ReadOnlyCollection<AnnotationObject> _objects;

    public Guid SessionId { get; }

    public AnnotationRevision Revision { get; }

    public IReadOnlyList<AnnotationObject> Objects => _objects;

    public static AnnotationDocument CreateEmpty(Guid sessionId) => new(
        sessionId,
        AnnotationRevision.Initial,
        Array.Empty<AnnotationObject>());
}

public abstract record AnnotationMutationRequest
{
    protected AnnotationMutationRequest(
        Guid sessionId,
        AnnotationRevision expectedAnnotationRevision)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Annotation mutation session identifier is required.", nameof(sessionId));
        }

        if (!expectedAnnotationRevision.IsValid)
        {
            throw new ArgumentException("Expected annotation revision must be non-negative.", nameof(expectedAnnotationRevision));
        }

        SessionId = sessionId;
        ExpectedAnnotationRevision = expectedAnnotationRevision;
    }

    public Guid SessionId { get; }

    public AnnotationRevision ExpectedAnnotationRevision { get; }
}

public sealed record AddAnnotationObjectRequest : AnnotationMutationRequest
{
    public AddAnnotationObjectRequest(
        Guid sessionId,
        AnnotationRevision expectedAnnotationRevision,
        AnnotationObject? annotationObject)
        : base(sessionId, expectedAnnotationRevision)
    {
        AnnotationObject = annotationObject;
    }

    public AnnotationObject? AnnotationObject { get; }
}

public sealed record ReplaceAnnotationObjectRequest : AnnotationMutationRequest
{
    public ReplaceAnnotationObjectRequest(
        Guid sessionId,
        AnnotationRevision expectedAnnotationRevision,
        AnnotationObject? annotationObject)
        : base(sessionId, expectedAnnotationRevision)
    {
        AnnotationObject = annotationObject;
    }

    public AnnotationObject? AnnotationObject { get; }
}

public sealed record RemoveAnnotationObjectRequest : AnnotationMutationRequest
{
    public RemoveAnnotationObjectRequest(
        Guid sessionId,
        AnnotationRevision expectedAnnotationRevision,
        AnnotationObjectId objectId)
        : base(sessionId, expectedAnnotationRevision)
    {
        ObjectId = objectId;
    }

    public AnnotationObjectId ObjectId { get; }
}

public abstract record AnnotationMutationResult(AnnotationDocument? CurrentDocument)
{
    public sealed record Succeeded(AnnotationDocument Document)
        : AnnotationMutationResult(Document);

    public sealed record StaleSession(
        Guid RequestedSessionId,
        Guid? ActiveSessionId,
        AnnotationDocument? Document)
        : AnnotationMutationResult(Document);

    public sealed record StaleAnnotationRevision(
        AnnotationRevision ExpectedRevision,
        AnnotationRevision CurrentRevision,
        AnnotationDocument Document)
        : AnnotationMutationResult(Document);

    public sealed record DuplicateObjectId(
        AnnotationObjectId ObjectId,
        AnnotationDocument Document)
        : AnnotationMutationResult(Document);

    public sealed record ObjectNotFound(
        AnnotationObjectId ObjectId,
        AnnotationDocument Document)
        : AnnotationMutationResult(Document);

    public sealed record InvalidObject(
        string Reason,
        AnnotationDocument Document)
        : AnnotationMutationResult(Document);

    public sealed record NoChange(AnnotationDocument Document)
        : AnnotationMutationResult(Document);

    public sealed record RevisionOverflow(AnnotationDocument Document)
        : AnnotationMutationResult(Document);
}
