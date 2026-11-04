namespace XmlEditorReloaded.Xml.Documents;

/// <summary>
/// Location of one record element inside an <see cref="XmlSnapshot.Text"/> string.
/// </summary>
/// <param name="Key">Canonical record key, or <see langword="null"/> when the record has no key.</param>
/// <param name="KeyDisplay">User-facing key, with compound components separated for display.</param>
/// <param name="Start">Zero-based character offset of the record's opening tag.</param>
/// <param name="Length">Length of the record's opening tag, in characters.</param>
public sealed record XmlRecordSpan(string? Key, string KeyDisplay, int Start, int Length);

/// <summary>
/// Read-only textual view of a table document together with the positions of its record elements.
/// The snapshot is produced from the current in-memory document, including unsaved edits.
/// </summary>
/// <param name="Text">Serialized XML text, including the declaration when the document has one.</param>
/// <param name="Records">Record locations in document order.</param>
public sealed record XmlSnapshot(string Text, IReadOnlyList<XmlRecordSpan> Records)
{
    /// <summary>
    /// Gets an empty snapshot.
    /// </summary>
    public static XmlSnapshot Empty { get; } = new(string.Empty, Array.Empty<XmlRecordSpan>());

    /// <summary>
    /// Finds a record by its canonical key or by its user-facing key.
    /// </summary>
    /// <param name="key">Key text entered by the user; surrounding whitespace is ignored.</param>
    /// <returns>The matching record location, or <see langword="null"/> when it is absent.</returns>
    public XmlRecordSpan? FindRecord(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        var trimmed = key.Trim();
        foreach (var record in Records)
        {
            if (string.Equals(record.Key, trimmed, StringComparison.Ordinal)
                || string.Equals(record.KeyDisplay, trimmed, StringComparison.Ordinal))
            {
                return record;
            }
        }

        return null;
    }
}
