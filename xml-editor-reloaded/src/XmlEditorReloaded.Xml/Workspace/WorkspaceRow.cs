using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Workspace;

/// <summary>
/// One editable row of a <see cref="TableWorkspace"/>: the base record plus, when a language overlay
/// is selected, the merged translation view for the same record.
/// </summary>
public sealed class WorkspaceRow
{
    private readonly Dictionary<string, MergedCell[]>? _cells;

    internal WorkspaceRow(string? key, TableRecord? record, MergedRecord? merged)
    {
        Key = key;
        Record = record;
        Merged = merged;
        _cells = merged?.Cells
            .GroupBy(static cell => cell.FieldPath, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.ToArray(),
                StringComparer.Ordinal);
    }

    /// <summary>
    /// Gets the primary record key, or <see langword="null"/> when the record has no key.
    /// </summary>
    public string? Key { get; }

    /// <summary>
    /// Gets the base record, or <see langword="null"/> for nested records that are not exposed by
    /// <see cref="TableDocument.Records"/>.
    /// </summary>
    public TableRecord? Record { get; }

    /// <summary>
    /// Gets the merged base/translation record, or <see langword="null"/> when no overlay is selected.
    /// </summary>
    public MergedRecord? Merged { get; }

    /// <summary>
    /// Gets the effective value of a field: the overlay translation when present, otherwise the base value.
    /// </summary>
    /// <param name="fieldPath">Field path relative to the record, for example <c>szItemName</c>.</param>
    /// <returns>The value, or <see langword="null"/> when the field is absent.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldPath"/> is empty.</exception>
    public string? GetValue(string fieldPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath);

        if (_cells is not null && _cells.TryGetValue(fieldPath, out var cells) && cells.Length > 0)
        {
            return cells[0].Value;
        }

        return Record?.GetRawValue(fieldPath);
    }

    /// <summary>
    /// Gets all values of a repeated field: the merged translation values when an overlay is selected,
    /// otherwise the base values.
    /// </summary>
    /// <param name="fieldPath">Field path relative to the record.</param>
    /// <returns>The values in document order; empty when the field is absent.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldPath"/> is empty.</exception>
    public IReadOnlyList<string> GetValues(string fieldPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath);

        if (_cells is not null && _cells.TryGetValue(fieldPath, out var cells))
        {
            return cells.Select(static cell => cell.Value).ToArray();
        }

        return Record?.GetValues(fieldPath).Select(static value => value.RawText).ToArray()
            ?? Array.Empty<string>();
    }

    /// <summary>
    /// Gets a value indicating whether the field value comes from the overlay.
    /// </summary>
    /// <param name="fieldPath">Field path relative to the record.</param>
    /// <returns><see langword="true"/> when the field is translated; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldPath"/> is empty.</exception>
    public bool IsTranslated(string fieldPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldPath);

        return _cells is not null
            && _cells.TryGetValue(fieldPath, out var cells)
            && cells.Any(static cell => cell.OverlayValue is not null);
    }
}
