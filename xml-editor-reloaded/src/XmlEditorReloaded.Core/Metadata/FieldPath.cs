namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// A path to a field inside a record, expressed as slash-separated element names,
/// for example <c>szItemName</c> or <c>GEARKIT/mGearKitName</c>.
/// </summary>
public sealed class FieldPath : IEquatable<FieldPath>
{
    private readonly string[] _segments;
    private readonly string _text;

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldPath"/> class.
    /// </summary>
    /// <param name="segments">Non-empty sequence of element names forming the path.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segments"/> is empty or contains an empty segment.</exception>
    public FieldPath(IEnumerable<string> segments)
    {
        ArgumentNullException.ThrowIfNull(segments);
        _segments = segments.ToArray();
        if (_segments.Length == 0)
        {
            throw new ArgumentException("A field path must contain at least one segment.", nameof(segments));
        }

        foreach (var segment in _segments)
        {
            if (string.IsNullOrWhiteSpace(segment) || segment.Contains('/') || segment.Contains('\\'))
            {
                throw new ArgumentException($"Invalid field path segment '{segment}'.", nameof(segments));
            }
        }

        _text = string.Join('/', _segments);
    }

    /// <summary>
    /// Gets the individual element names forming the path, from the outermost to the field itself.
    /// </summary>
    public IReadOnlyList<string> Segments => _segments;

    /// <summary>
    /// Gets the number of segments in the path. A value of <c>1</c> means a direct child of the record element.
    /// </summary>
    public int Depth => _segments.Length;

    /// <summary>
    /// Gets the last segment, which is the field (leaf element) name.
    /// </summary>
    public string FieldName => _segments[^1];

    /// <summary>
    /// Gets the slash-joined textual form of the path, for example <c>GEARKIT/mGearKitName</c>.
    /// </summary>
    public string Text => _text;

    /// <summary>
    /// Parses a slash-separated path such as <c>GEARKIT/mGearKitName</c>.
    /// </summary>
    /// <param name="text">The path text.</param>
    /// <returns>A parsed field path.</returns>
    /// <exception cref="ArgumentException">Thrown when the text is empty or contains empty segments.</exception>
    public static FieldPath Parse(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        return new FieldPath(text.Split('/', StringSplitOptions.None));
    }

    /// <summary>
    /// Attempts to parse a slash-separated path.
    /// </summary>
    /// <param name="text">The path text.</param>
    /// <param name="fieldPath">The parsed path when successful.</param>
    /// <returns><see langword="true"/> when parsing succeeded; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(string? text, out FieldPath? fieldPath)
    {
        fieldPath = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var segments = text.Split('/', StringSplitOptions.None);
        if (segments.Any(static s => string.IsNullOrWhiteSpace(s) || s.Contains('\\')))
        {
            return false;
        }

        fieldPath = new FieldPath(segments);
        return true;
    }

    /// <inheritdoc />
    public bool Equals(FieldPath? other) => other is not null && string.Equals(_text, other._text, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is FieldPath other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(_text);

    /// <inheritdoc />
    public override string ToString() => _text;
}
