namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// One selectable target record of a relation, holding the key that is written back and the text
/// shown to the user.
/// </summary>
/// <param name="Value">Target key in invariant lexical form.</param>
/// <param name="Display">Display text of the target record.</param>
public sealed record RelationTargetEntry(string Value, string Display);

/// <summary>
/// Resolved rows of a relation target: every record for membership checks and the filtered subset
/// offered by pickers. The filter only narrows <see cref="Options"/>; <see cref="Entries"/> keeps the
/// complete target so existing values and validation stay independent of presentation filters.
/// </summary>
public sealed class RelationTarget
{
    private readonly Dictionary<string, RelationTargetEntry> _byValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelationTarget"/> class.
    /// </summary>
    /// <param name="tableName">Target table name.</param>
    /// <param name="valueField">Target field that stores the key.</param>
    /// <param name="displayField">Target field shown to the user, or <see langword="null"/>.</param>
    /// <param name="entries">All target records in document order.</param>
    /// <param name="options">Filtered records offered by pickers, in document order.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entries"/> or <paramref name="options"/> is null.
    /// </exception>
    public RelationTarget(
        TableName tableName,
        string valueField,
        string? displayField,
        IReadOnlyList<RelationTargetEntry> entries,
        IReadOnlyList<RelationTargetEntry> options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valueField);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(options);

        TableName = tableName;
        ValueField = valueField;
        DisplayField = displayField;
        Entries = entries;
        Options = options;
        _byValue = new Dictionary<string, RelationTargetEntry>(StringComparer.Ordinal);
        foreach (var entry in entries)
        {
            _byValue.TryAdd(entry.Value, entry);
        }
    }

    /// <summary>
    /// Gets the target table name.
    /// </summary>
    public TableName TableName { get; }

    /// <summary>
    /// Gets the target field that stores the key.
    /// </summary>
    public string ValueField { get; }

    /// <summary>
    /// Gets the target field shown to the user, or <see langword="null"/> when the key itself is shown.
    /// </summary>
    public string? DisplayField { get; }

    /// <summary>
    /// Gets every target record in document order.
    /// </summary>
    public IReadOnlyList<RelationTargetEntry> Entries { get; }

    /// <summary>
    /// Gets the filtered records offered by pickers, in document order.
    /// </summary>
    public IReadOnlyList<RelationTargetEntry> Options { get; }

    /// <summary>
    /// Finds a target record by its exact key.
    /// </summary>
    /// <param name="value">Raw key text.</param>
    /// <returns>The matching entry, or <see langword="null"/> when the key is absent.</returns>
    public RelationTargetEntry? Find(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : _byValue.GetValueOrDefault(trimmed);
    }

    /// <summary>
    /// Determines whether the target contains a record with the supplied key.
    /// </summary>
    /// <param name="value">Raw key text.</param>
    /// <returns><see langword="true"/> when the key exists; otherwise <see langword="false"/>.</returns>
    public bool Contains(string? value) => Find(value) is not null;

    /// <summary>
    /// Resolves a key from a display text, which supports remapping copied records by name when the
    /// target identifiers differ between data sets.
    /// </summary>
    /// <param name="displayText">Display text to look up, compared case-insensitively.</param>
    /// <returns>
    /// The key of the first matching record, or <see langword="null"/> when the text matches nothing.
    /// </returns>
    public string? FindValueByDisplay(string? displayText)
    {
        var trimmed = displayText?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return null;
        }

        foreach (var entry in Entries)
        {
            if (string.Equals(entry.Display, trimmed, StringComparison.OrdinalIgnoreCase))
            {
                return entry.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// Searches the filtered options by display text or key.
    /// </summary>
    /// <param name="term">Search term; an empty term returns every option.</param>
    /// <returns>Matching options in document order.</returns>
    public IReadOnlyList<RelationTargetEntry> Search(string? term)
    {
        var trimmed = term?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return Options;
        }

        return Options
            .Where(entry =>
                entry.Display.Contains(trimmed, StringComparison.OrdinalIgnoreCase)
                || entry.Value.Contains(trimmed, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }
}
