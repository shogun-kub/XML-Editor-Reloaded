namespace XmlEditorReloaded.Xml.Documents;

/// <summary>
/// Base type for a change recorded in a <see cref="DocumentChangeTracker"/>.
/// </summary>
/// <param name="Kind">Stable kind name, for example <c>FieldValueChanged</c>.</param>
/// <param name="RecordedAt">UTC time when the change was recorded.</param>
public abstract record DocumentChange(string Kind, DateTimeOffset RecordedAt);

/// <summary>
/// A scalar field value changed from one lexical form to another.
/// </summary>
/// <param name="RecordKey">Key of the affected record, or an empty string when no key is available.</param>
/// <param name="FieldPath">Slash-separated field path.</param>
/// <param name="OldValue">Previous raw text, or <see langword="null"/> when the field was added.</param>
/// <param name="NewValue">New raw text.</param>
public sealed record FieldValueChanged(string RecordKey, string FieldPath, string? OldValue, string NewValue)
    : DocumentChange(nameof(FieldValueChanged), DateTimeOffset.UtcNow);

/// <summary>
/// A repeated list field was replaced with a new sequence of values.
/// </summary>
/// <param name="RecordKey">Key of the affected record.</param>
/// <param name="FieldName">XML element name of the list field.</param>
/// <param name="OldValues">Previous values in document order.</param>
/// <param name="NewValues">New values in document order.</param>
public sealed record ListFieldChanged(string RecordKey, string FieldName, IReadOnlyList<string> OldValues, IReadOnlyList<string> NewValues)
    : DocumentChange(nameof(ListFieldChanged), DateTimeOffset.UtcNow);

/// <summary>
/// A record was appended to the document.
/// </summary>
/// <param name="RecordKey">Key of the new record, or <see langword="null"/> when the table has no key.</param>
public sealed record RecordAdded(string? RecordKey)
    : DocumentChange(nameof(RecordAdded), DateTimeOffset.UtcNow);

/// <summary>
/// A record was duplicated.
/// </summary>
/// <param name="SourceKey">Key of the source record.</param>
/// <param name="NewKey">Key assigned to the copy, or <see langword="null"/> when the table has no key.</param>
public sealed record RecordDuplicated(string SourceKey, string? NewKey)
    : DocumentChange(nameof(RecordDuplicated), DateTimeOffset.UtcNow);

/// <summary>
/// A record was deleted, retaining its XML snapshot for a future undo implementation.
/// </summary>
/// <param name="RecordKey">Key of the deleted record.</param>
/// <param name="SnapshotXml">Serialized copy of the deleted record.</param>
public sealed record RecordDeleted(string RecordKey, string SnapshotXml)
    : DocumentChange(nameof(RecordDeleted), DateTimeOffset.UtcNow);

/// <summary>
/// Records document mutations in order so a future undo implementation can replay or reverse them.
/// </summary>
public sealed class DocumentChangeTracker
{
    private readonly List<DocumentChange> _changes = new();

    /// <summary>
    /// Gets the recorded changes in chronological order.
    /// </summary>
    public IReadOnlyList<DocumentChange> Changes => _changes;

    /// <summary>
    /// Gets a value indicating whether any change has been recorded.
    /// </summary>
    public bool HasChanges => _changes.Count > 0;

    /// <summary>
    /// Appends a change to the journal.
    /// </summary>
    /// <param name="change">Change to record.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="change"/> is <see langword="null"/>.</exception>
    public void Record(DocumentChange change)
    {
        ArgumentNullException.ThrowIfNull(change);
        _changes.Add(change);
    }

    /// <summary>
    /// Clears the journal.
    /// </summary>
    public void Clear() => _changes.Clear();
}
