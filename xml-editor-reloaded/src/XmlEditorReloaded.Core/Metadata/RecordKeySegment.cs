namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Describes how a single segment of a record key is resolved inside a table or nested structure.
/// </summary>
public sealed class RecordKeySegment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecordKeySegment"/> class.
    /// </summary>
    /// <param name="path">
    /// Slash-separated path from the record element to the keyed collection,
    /// for example <c>CITYLIST/CITY</c> or <c>ASSIGNMENT</c>. An empty string means the record element itself.
    /// </param>
    /// <param name="kind">How the key value is derived for each element of the collection.</param>
    /// <param name="keyField">
    /// Name of the child element holding the key value when <paramref name="kind"/> is
    /// <see cref="RecordKeyKind.Scalar"/>; otherwise <see langword="null"/>.
    /// </param>
    /// <param name="isPrimary">Whether this segment identifies the primary record key of the table.</param>
    public RecordKeySegment(string path, RecordKeyKind kind, string? keyField = null, bool isPrimary = true)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (kind == RecordKeyKind.Scalar && string.IsNullOrWhiteSpace(keyField))
        {
            throw new ArgumentException("Scalar key segments require a key field name.", nameof(keyField));
        }

        if (kind != RecordKeyKind.Scalar && keyField is not null)
        {
            throw new ArgumentException("Only scalar key segments may specify a key field.", nameof(keyField));
        }

        Path = path;
        Kind = kind;
        KeyField = keyField;
        IsPrimary = isPrimary;
    }

    /// <summary>
    /// Gets the slash-separated path from the record element to the keyed collection; empty for the record itself.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the strategy used to derive key values.
    /// </summary>
    public RecordKeyKind Kind { get; }

    /// <summary>
    /// Gets the child element name that holds the key value for scalar segments.
    /// </summary>
    public string? KeyField { get; }

    /// <summary>
    /// Gets a value indicating whether this segment is the primary record key.
    /// </summary>
    public bool IsPrimary { get; }
}

/// <summary>
/// Strategy used to derive a key value for elements of a keyed collection.
/// </summary>
public enum RecordKeyKind
{
    /// <summary>Key is the text of a named child element, for example <c>uiIndex</c>.</summary>
    Scalar,

    /// <summary>Key is the zero-based ordinal position of the element among its siblings with the same name.</summary>
    Positional,

    /// <summary>The element is a singleton with no key; identified by its path alone.</summary>
    Singleton,
}
