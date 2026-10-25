using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Relations;

/// <summary>
/// Resolves relation targets from the data directory: loads the target table, reads the configured
/// value and display fields, applies the structured filter to picker options, merges language
/// overlay values into display text, and caches loaded targets until the underlying files change.
/// </summary>
/// <remarks>
/// <para>
/// Target loads are isolated from the source table: a missing, malformed, or unreadable target only
/// fails the relation picker and never prevents the source table from opening.
/// </para>
/// <para>
/// The reserved token <see cref="RecordFields.RecordKey"/> (<c>#Key</c>) is accepted as the target
/// value field and as the filter field. It resolves to the metadata-defined record key, so tables
/// with a positional key expose their zero-based record position as the reference value; the token
/// never reads a child element.
/// </para>
/// <para>
/// <see cref="TableRelation.IncludeKeyZero"/> additionally offers the target key <c>0</c> first,
/// even when that record fails the relation filter. <see cref="TableRelation.NoneValue"/> adds a
/// synthetic entry after the document order so the "no reference" value stays selectable and is
/// accepted by membership checks even though the target file has no such record.
/// </para>
/// </remarks>
public sealed class TableRelationResolver : ITableRelationResolver
{
    private readonly ITableCatalog _catalog;
    private readonly ITableRepository _repository;
    private readonly ITableMetadataProvider? _metadataProvider;
    private readonly string _tableDataDirectory;
    private readonly Dictionary<TableName, TableDescriptor?> _descriptors = new();
    private readonly Dictionary<TargetCacheKey, TargetCacheEntry> _cache = new();
    private readonly SemaphoreSlim _descriptorLock = new(1, 1);
    private readonly object _cacheLock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TableRelationResolver"/> class.
    /// </summary>
    /// <param name="catalog">Catalog used to discover target table files.</param>
    /// <param name="repository">Repository used to load target tables.</param>
    /// <param name="tableDataDirectory">Absolute <c>TableData</c> directory of the data set.</param>
    /// <param name="metadataProvider">
    /// Optional metadata provider used to read target metadata and language overlays.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="tableDataDirectory"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="catalog"/> or <paramref name="repository"/> is null.</exception>
    public TableRelationResolver(
        ITableCatalog catalog,
        ITableRepository repository,
        string tableDataDirectory,
        ITableMetadataProvider? metadataProvider = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableDataDirectory);
        _catalog = catalog;
        _repository = repository;
        _tableDataDirectory = tableDataDirectory;
        _metadataProvider = metadataProvider;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="relation"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<RelationTarget>> ResolveAsync(
        TableRelation relation,
        string? languageCode = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(relation);

        var descriptor = await FindDescriptorAsync(relation.TargetTable, cancellationToken).ConfigureAwait(false);
        if (descriptor is null)
        {
            return OperationResult<RelationTarget>.Failure(
                ErrorCodes.IoFileNotFound,
                $"Target table '{relation.TargetTable}' was not found in '{_tableDataDirectory}'.");
        }

        var key = new TargetCacheKey(
            relation.TargetTable,
            languageCode,
            relation.TargetValueField,
            relation.TargetDisplayField,
            relation.Filter?.Field,
            relation.Filter?.Operator,
            relation.Filter?.Value,
            relation.IncludeKeyZero,
            relation.NoneValue,
            relation.NoneDisplay);
        var stamp = BuildStamp(descriptor, languageCode);
        lock (_cacheLock)
        {
            if (_cache.TryGetValue(key, out var cached) && string.Equals(cached.Stamp, stamp, StringComparison.Ordinal))
            {
                return OperationResult<RelationTarget>.Success(cached.Target);
            }
        }

        var load = await _repository.LoadTableAsync(descriptor, cancellationToken).ConfigureAwait(false);
        if (!load.IsSuccess)
        {
            return OperationResult<RelationTarget>.Failure(load);
        }

        var document = load.Data!.GetDocument<TableDocument>();
        if (document is null || load.Data.Status == TableDocumentStatus.Malformed)
        {
            var diagnostic = load.Data.Diagnostics.FirstOrDefault();
            return OperationResult<RelationTarget>.Failure(
                ErrorCodes.XmlParseError,
                diagnostic is null
                    ? $"Target table '{relation.TargetTable}' could not be loaded."
                    : $"{diagnostic.Code}: {diagnostic.Message}");
        }

        var translatedDisplays = await LoadOverlayDisplaysAsync(
                descriptor,
                relation,
                languageCode,
                cancellationToken)
            .ConfigureAwait(false);

        var target = BuildTarget(relation, document, translatedDisplays);
        lock (_cacheLock)
        {
            _cache[key] = new TargetCacheEntry(stamp, target);
        }

        return OperationResult<RelationTarget>.Success(target);
    }

    /// <inheritdoc />
    public void Invalidate(TableName tableName)
    {
        lock (_cacheLock)
        {
            foreach (var key in _cache.Keys.Where(key => key.TableName == tableName).ToArray())
            {
                _cache.Remove(key);
            }

            _descriptors.Remove(tableName);
        }
    }

    private async Task<TableDescriptor?> FindDescriptorAsync(
        TableName tableName,
        CancellationToken cancellationToken)
    {
        lock (_cacheLock)
        {
            if (_descriptors.TryGetValue(tableName, out var cached))
            {
                return cached;
            }
        }

        await _descriptorLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            lock (_cacheLock)
            {
                if (_descriptors.TryGetValue(tableName, out var scanned))
                {
                    return scanned;
                }
            }

            var tables = await _catalog.GetTablesAsync(_tableDataDirectory, cancellationToken).ConfigureAwait(false);
            if (!tables.IsSuccess)
            {
                return null;
            }

            foreach (var descriptor in tables.Data!)
            {
                lock (_cacheLock)
                {
                    _descriptors[descriptor.TableName] = descriptor;
                }
            }

            lock (_cacheLock)
            {
                // Tables that are absent from the catalog are remembered as missing so repeated
                // pickers do not rescan the directory.
                _descriptors.TryAdd(tableName, null);
                return _descriptors.GetValueOrDefault(tableName);
            }
        }
        finally
        {
            _descriptorLock.Release();
        }
    }

    private async Task<Dictionary<string, string>> LoadOverlayDisplaysAsync(
        TableDescriptor descriptor,
        TableRelation relation,
        string? languageCode,
        CancellationToken cancellationToken)
    {
        var displays = new Dictionary<string, string>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(languageCode)
            || _metadataProvider is null
            || relation.TargetDisplayField is null)
        {
            return displays;
        }

        var overlay = descriptor.Overlays.FirstOrDefault(candidate =>
            string.Equals(candidate.Language.Code, languageCode, StringComparison.OrdinalIgnoreCase));
        if (overlay is null)
        {
            return displays;
        }

        var metadata = await _metadataProvider
            .GetTableAsync(relation.TargetTable, cancellationToken)
            .ConfigureAwait(false);
        if (!metadata.IsSuccess)
        {
            return displays;
        }

        // A broken overlay must not hide the base display values.
        var loaded = await OverlayDocument
            .LoadAsync(overlay, metadata.Data!, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        if (!loaded.IsSuccess)
        {
            return displays;
        }

        foreach (var record in loaded.Data!.Document.Records)
        {
            var key = GetTargetValue(record, relation.TargetValueField)?.Trim();
            var display = record.GetRawValue(relation.TargetDisplayField)?.Trim();
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(display))
            {
                displays.TryAdd(key, display);
            }
        }

        return displays;
    }

    private static RelationTarget BuildTarget(
        TableRelation relation,
        TableDocument document,
        IReadOnlyDictionary<string, string> translatedDisplays)
    {
        var entries = new List<RelationTargetEntry>();
        var options = new List<RelationTargetEntry>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var record in document.Records)
        {
            var value = GetTargetValue(record, relation.TargetValueField)?.Trim();
            if (string.IsNullOrEmpty(value) || !seen.Add(value))
            {
                continue;
            }

            var display = relation.TargetDisplayField is null
                ? value
                : translatedDisplays.TryGetValue(value, out var translated)
                    ? translated
                    : record.GetRawValue(relation.TargetDisplayField)?.Trim() ?? value;

            var entry = new RelationTargetEntry(value, display);
            entries.Add(entry);

            var filterValue = relation.Filter is null
                ? null
                : GetTargetValue(record, relation.Filter.Field);
            if (relation.Filter is null || relation.Filter.Matches(filterValue))
            {
                options.Add(entry);
            }
        }

        if (relation.IncludeKeyZero)
        {
            // Legacy item slots used "uiIndex = 0 OR usItemClass = …" so the empty slot stayed
            // selectable even though its class failed the filter.
            var zero = entries.FirstOrDefault(static entry =>
                string.Equals(entry.Value, "0", StringComparison.Ordinal));
            if (zero is not null
                && !options.Exists(entry => string.Equals(entry.Value, zero.Value, StringComparison.Ordinal)))
            {
                options.Insert(0, zero);
            }
        }

        var noneValue = relation.NoneValue?.Trim();
        if (!string.IsNullOrEmpty(noneValue))
        {
            var noneEntry = entries.FirstOrDefault(entry =>
                string.Equals(entry.Value, noneValue, StringComparison.Ordinal));
            if (noneEntry is null)
            {
                var noneDisplay = string.IsNullOrWhiteSpace(relation.NoneDisplay)
                    ? noneValue
                    : relation.NoneDisplay.Trim();
                noneEntry = new RelationTargetEntry(noneValue, noneDisplay);
                entries.Add(noneEntry);
                options.Add(noneEntry);
            }
            else if (!options.Exists(entry =>
                         string.Equals(entry.Value, noneValue, StringComparison.Ordinal)))
            {
                options.Add(noneEntry);
            }
        }

        return new RelationTarget(
            relation.TargetTable,
            relation.TargetValueField,
            relation.TargetDisplayField,
            entries,
            options);
    }

    private static string? GetTargetValue(TableRecord record, string field) =>
        string.Equals(field, RecordFields.RecordKey, StringComparison.Ordinal)
            ? record.GetKey()
            : record.GetRawValue(field);

    private static string BuildStamp(TableDescriptor descriptor, string? languageCode)
    {
        var stamp = FileStamp(descriptor.FilePath);
        if (!string.IsNullOrWhiteSpace(languageCode))
        {
            var overlay = descriptor.Overlays.FirstOrDefault(candidate =>
                string.Equals(candidate.Language.Code, languageCode, StringComparison.OrdinalIgnoreCase));
            if (overlay is not null)
            {
                stamp += "|" + FileStamp(overlay.OverlayFilePath);
            }
        }

        return stamp;
    }

    private static string FileStamp(string path)
    {
        try
        {
            var info = new FileInfo(path);
            return info.Exists
                ? $"{info.Length}:{info.LastWriteTimeUtc.Ticks}"
                : "missing";
        }
        catch (IOException)
        {
            return "unreadable";
        }
        catch (UnauthorizedAccessException)
        {
            return "denied";
        }
    }

    private readonly record struct TargetCacheKey(
        TableName TableName,
        string? LanguageCode,
        string TargetValueField,
        string? TargetDisplayField,
        string? FilterField,
        RelationFilterOperator? FilterOperator,
        string? FilterValue,
        bool IncludeKeyZero,
        string? NoneValue,
        string? NoneDisplay);

    private readonly record struct TargetCacheEntry(string Stamp, RelationTarget Target);
}
