namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Defines how records of a table (or of nested structures inside a record) are keyed,
/// primarily for overlay matching and row identity.
/// </summary>
public sealed class RecordKeySpec
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecordKeySpec"/> class.
    /// </summary>
    /// <param name="segments">One or more key segments; the primary segment must come first.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segments"/> is empty.</exception>
    public RecordKeySpec(IEnumerable<RecordKeySegment> segments)
    {
        ArgumentNullException.ThrowIfNull(segments);
        Segments = segments.ToArray();
        if (Segments.Count == 0)
        {
            throw new ArgumentException("A record key specification requires at least one segment.", nameof(segments));
        }
    }

    /// <summary>
    /// Gets the key segments, with the primary record key first.
    /// </summary>
    public IReadOnlyList<RecordKeySegment> Segments { get; }

    /// <summary>
    /// Gets the primary key segment used to identify records of the table itself.
    /// </summary>
    public RecordKeySegment Primary => Segments[0];
}
