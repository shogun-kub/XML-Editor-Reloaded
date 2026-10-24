using System.Globalization;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Copy;

/// <summary>
/// Resolves reference values of one table by display text so identifiers that differ between two data
/// sets are rewritten. Lookup columns are matched through lookup names; relations marked
/// <see cref="TableRelation.RemapByText"/> are matched through their display field.
/// </summary>
internal sealed class TableCopyRemapper
{
    private readonly TableMetadata? _metadata;
    private readonly ITableMetadataProvider _metadataProvider;
    private readonly Func<TableName, CancellationToken, Task<DocumentTable?>> _loadSourceTable;
    private readonly Func<TableName, CancellationToken, Task<DocumentTable?>> _loadTargetTable;
    private readonly Func<string, CancellationToken, Task<IReadOnlyList<LookupEntry>?>> _loadSourceLookup;
    private readonly Func<string, CancellationToken, Task<IReadOnlyList<LookupEntry>?>> _loadTargetLookup;
    private readonly Dictionary<string, string> _lookupByField = new(StringComparer.Ordinal);
    private readonly Dictionary<string, TableRelation> _relationByField = new(StringComparer.Ordinal);
    private readonly Dictionary<DisplayMapKey, Task<DisplayMap?>> _displayMaps = new();
    private readonly Dictionary<string, Task<IReadOnlyList<LookupEntry>?>> _sourceLookups = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Task<IReadOnlyList<LookupEntry>?>> _targetLookups = new(StringComparer.Ordinal);
    private Task<IReadOnlyList<TableRelation>>? _relations;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableCopyRemapper"/> class.
    /// </summary>
    /// <param name="metadata">Metadata of the table being copied, or <see langword="null"/>.</param>
    /// <param name="metadataProvider">Metadata provider used to read remappable relations.</param>
    /// <param name="loadSourceTable">Loads a referenced table from the source data set.</param>
    /// <param name="loadTargetTable">Loads a referenced table from the target data set.</param>
    /// <param name="loadSourceLookup">Loads a lookup table from the source data set.</param>
    /// <param name="loadTargetLookup">Loads a lookup table from the target data set.</param>
    /// <exception cref="ArgumentNullException">Thrown when a delegate is null.</exception>
    public TableCopyRemapper(
        TableMetadata? metadata,
        ITableMetadataProvider metadataProvider,
        Func<TableName, CancellationToken, Task<DocumentTable?>> loadSourceTable,
        Func<TableName, CancellationToken, Task<DocumentTable?>> loadTargetTable,
        Func<string, CancellationToken, Task<IReadOnlyList<LookupEntry>?>> loadSourceLookup,
        Func<string, CancellationToken, Task<IReadOnlyList<LookupEntry>?>> loadTargetLookup)
    {
        ArgumentNullException.ThrowIfNull(metadataProvider);
        ArgumentNullException.ThrowIfNull(loadSourceTable);
        ArgumentNullException.ThrowIfNull(loadTargetTable);
        ArgumentNullException.ThrowIfNull(loadSourceLookup);
        ArgumentNullException.ThrowIfNull(loadTargetLookup);

        _metadata = metadata;
        _metadataProvider = metadataProvider;
        _loadSourceTable = loadSourceTable;
        _loadTargetTable = loadTargetTable;
        _loadSourceLookup = loadSourceLookup;
        _loadTargetLookup = loadTargetLookup;
        CollectFields();
    }

    /// <summary>
    /// Resolves every reference field of one record that is about to be copied.
    /// </summary>
    /// <param name="record">Source record.</param>
    /// <param name="fields">Fields whose values will be written, in copy order.</param>
    /// <param name="excludedFields">Fields that are assigned explicitly and must not be remapped.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Resolved references in field order; unresolved references keep the source value.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<IReadOnlyList<TableCopyRemap>> MapRecordAsync(
        TableRecord record,
        IReadOnlyCollection<string> fields,
        IReadOnlyCollection<string> excludedFields,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentNullException.ThrowIfNull(excludedFields);

        var result = new List<TableCopyRemap>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var field in fields)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (excludedFields.Contains(field)
                || !seen.Add(field)
                || record.GetChildFieldNames(field).Count > 0)
            {
                continue;
            }

            var value = record.GetRawValue(field);
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var remap = await ResolveFieldAsync(field, value.Trim(), cancellationToken).ConfigureAwait(false);
            if (remap is not null)
            {
                result.Add(remap);
            }
        }

        return result;
    }

    private async Task<TableCopyRemap?> ResolveFieldAsync(
        string field,
        string value,
        CancellationToken cancellationToken)
    {
        if (_relationByField.TryGetValue(field, out var relation))
        {
            return await ResolveRelationAsync(field, value, relation, cancellationToken).ConfigureAwait(false);
        }

        if (_lookupByField.TryGetValue(field, out var lookupName))
        {
            return await ResolveLookupAsync(field, value, lookupName, cancellationToken).ConfigureAwait(false);
        }

        return null;
    }

    private async Task<TableCopyRemap?> ResolveRelationAsync(
        string field,
        string value,
        TableRelation relation,
        CancellationToken cancellationToken)
    {
        var sourceMap = await GetDisplayMapAsync(relation, isSource: true, cancellationToken).ConfigureAwait(false);
        if (sourceMap is null || !sourceMap.DisplayByValue.TryGetValue(value, out var display))
        {
            return null;
        }

        var targetMap = await GetDisplayMapAsync(relation, isSource: false, cancellationToken).ConfigureAwait(false);
        if (targetMap is null)
        {
            return new TableCopyRemap(
                field,
                value,
                value,
                display,
                TableCopyRemapStatus.NotFound);
        }

        if (!targetMap.ValuesByDisplay.TryGetValue(display, out var targetValues) || targetValues.Count == 0)
        {
            return new TableCopyRemap(
                field,
                value,
                value,
                display,
                TableCopyRemapStatus.NotFound);
        }

        if (targetValues.Count > 1)
        {
            return new TableCopyRemap(
                field,
                value,
                value,
                display,
                TableCopyRemapStatus.Ambiguous);
        }

        var targetValue = targetValues[0];
        return string.Equals(targetValue, value, StringComparison.Ordinal)
            ? null
            : new TableCopyRemap(field, value, targetValue, display, TableCopyRemapStatus.Remapped);
    }

    private async Task<TableCopyRemap?> ResolveLookupAsync(
        string field,
        string value,
        string lookupName,
        CancellationToken cancellationToken)
    {
        if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var sourceId))
        {
            return null;
        }

        var sourceEntries = await GetLookupAsync(lookupName, isSource: true, cancellationToken).ConfigureAwait(false);
        var sourceName = sourceEntries?
            .FirstOrDefault(entry => entry.Id == sourceId)
            ?.Name;
        if (string.IsNullOrWhiteSpace(sourceName))
        {
            return null;
        }

        var targetEntries = await GetLookupAsync(lookupName, isSource: false, cancellationToken).ConfigureAwait(false);
        if (targetEntries is null)
        {
            return new TableCopyRemap(
                field,
                value,
                value,
                sourceName,
                TableCopyRemapStatus.NotFound);
        }

        var matches = targetEntries
            .Where(entry => string.Equals(entry.Name, sourceName, StringComparison.OrdinalIgnoreCase))
            .Select(static entry => entry.Id)
            .Distinct()
            .ToArray();
        if (matches.Length == 0)
        {
            return new TableCopyRemap(
                field,
                value,
                value,
                sourceName,
                TableCopyRemapStatus.NotFound);
        }

        if (matches.Length > 1)
        {
            return new TableCopyRemap(
                field,
                value,
                value,
                sourceName,
                TableCopyRemapStatus.Ambiguous);
        }

        var targetValue = matches[0].ToString(CultureInfo.InvariantCulture);
        return string.Equals(targetValue, value, StringComparison.Ordinal)
            ? null
            : new TableCopyRemap(field, value, targetValue, sourceName, TableCopyRemapStatus.Remapped);
    }

    private Task<DisplayMap?> GetDisplayMapAsync(
        TableRelation relation,
        bool isSource,
        CancellationToken cancellationToken)
    {
        var key = new DisplayMapKey(
            relation.TargetTable,
            relation.TargetValueField,
            relation.TargetDisplayField,
            isSource);
        if (!_displayMaps.TryGetValue(key, out var pending))
        {
            pending = LoadDisplayMapAsync(relation, isSource, cancellationToken);
            _displayMaps[key] = pending;
        }

        return pending;
    }

    private async Task<DisplayMap?> LoadDisplayMapAsync(
        TableRelation relation,
        bool isSource,
        CancellationToken cancellationToken)
    {
        var load = isSource ? _loadSourceTable : _loadTargetTable;
        var table = await load(relation.TargetTable, cancellationToken).ConfigureAwait(false);
        if (table is null)
        {
            return null;
        }

        var displayByValue = new Dictionary<string, string>(StringComparer.Ordinal);
        var valuesByDisplay = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var record in table.Document.Records)
        {
            var value = record.GetRawValue(relation.TargetValueField)?.Trim();
            if (string.IsNullOrEmpty(value) || displayByValue.ContainsKey(value))
            {
                continue;
            }

            var display = relation.TargetDisplayField is null
                ? value
                : record.GetRawValue(relation.TargetDisplayField)?.Trim() ?? value;
            if (string.IsNullOrWhiteSpace(display))
            {
                continue;
            }

            displayByValue[value] = display;
            if (!valuesByDisplay.TryGetValue(display, out var values))
            {
                values = new List<string>();
                valuesByDisplay[display] = values;
            }

            if (!values.Contains(value, StringComparer.Ordinal))
            {
                values.Add(value);
            }
        }

        return new DisplayMap(displayByValue, valuesByDisplay);
    }

    private Task<IReadOnlyList<LookupEntry>?> GetLookupAsync(
        string lookupName,
        bool isSource,
        CancellationToken cancellationToken)
    {
        var cache = isSource ? _sourceLookups : _targetLookups;
        if (!cache.TryGetValue(lookupName, out var pending))
        {
            var load = isSource ? _loadSourceLookup : _loadTargetLookup;
            pending = load(lookupName, cancellationToken);
            cache[lookupName] = pending;
        }

        return pending;
    }

    private void CollectFields()
    {
        if (_metadata is null)
        {
            return;
        }

        foreach (var hint in _metadata.ColumnHints)
        {
            if (!string.IsNullOrWhiteSpace(hint.LookupTable))
            {
                _lookupByField[hint.XmlFieldName] = hint.LookupTable;
            }
        }

        foreach (var column in _metadata.Columns)
        {
            if (!string.IsNullOrWhiteSpace(column.LookupTable))
            {
                _lookupByField.TryAdd(column.XmlFieldName, column.LookupTable);
            }
        }
    }

    /// <summary>
    /// Collects relations flagged for display-text remapping.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the relation list has been read.</returns>
    public async Task PrepareRelationsAsync(CancellationToken cancellationToken)
    {
        _relations ??= LoadRelationsAsync(cancellationToken);
        var relations = await _relations.ConfigureAwait(false);
        foreach (var relation in relations)
        {
            if (relation.RemapByText)
            {
                _relationByField.TryAdd(relation.SourceField, relation);
            }
        }
    }

    private async Task<IReadOnlyList<TableRelation>> LoadRelationsAsync(CancellationToken cancellationToken)
    {
        if (_metadata is null)
        {
            return Array.Empty<TableRelation>();
        }

        var result = await _metadataProvider
            .GetRelationsAsync(_metadata.TableName, cancellationToken)
            .ConfigureAwait(false);
        return result.IsSuccess ? result.Data! : Array.Empty<TableRelation>();
    }

    private readonly record struct DisplayMapKey(
        TableName Table,
        string ValueField,
        string? DisplayField,
        bool IsSource);

    private sealed class DisplayMap
    {
        public DisplayMap(
            Dictionary<string, string> displayByValue,
            Dictionary<string, List<string>> valuesByDisplay)
        {
            DisplayByValue = displayByValue;
            ValuesByDisplay = valuesByDisplay;
        }

        public Dictionary<string, string> DisplayByValue { get; }

        public Dictionary<string, List<string>> ValuesByDisplay { get; }
    }
}

/// <summary>
/// A loaded table document with its path-independent metadata, used by the copy remapper.
/// </summary>
/// <param name="Descriptor">Table descriptor of the loaded document.</param>
/// <param name="Metadata">Table metadata, or <see langword="null"/> for document-driven tables.</param>
/// <param name="Document">Loaded document.</param>
internal sealed record DocumentTable(
    TableDescriptor Descriptor,
    TableMetadata? Metadata,
    TableDocument Document);
