using System.Globalization;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;

namespace XmlEditorReloaded.Xml.Lookups;

/// <summary>
/// One place where a lookup identifier is used: a table record and the field that stores the id or mask.
/// </summary>
/// <param name="TableName">Table that references the lookup.</param>
/// <param name="RecordKey">Record key, or <see langword="null"/> for a record without a declared key.</param>
/// <param name="FieldPath">Field that stores the lookup id or bit mask.</param>
/// <param name="Value">Exact stored value.</param>
public sealed record LookupUsage(TableName TableName, string? RecordKey, string FieldPath, string Value);

/// <summary>
/// Result of a lookup usage scan. <see cref="UsageCount"/> is exact; <see cref="Usages"/> is capped by
/// <see cref="LookupUsageService.MaxUsages"/> and <see cref="LookupUsageService.MaxUsagesPerTable"/>.
/// </summary>
/// <param name="LookupTable">Lookup table that was scanned.</param>
/// <param name="LookupId">Lookup identifier that was searched for.</param>
/// <param name="UsageCount">Exact number of matching record fields.</param>
/// <param name="Usages">Retained usages in table and document order.</param>
/// <param name="ScannedTableCount">Number of tables that reference the lookup and were loaded.</param>
/// <param name="FailedTableCount">Referencing tables that could not be loaded.</param>
/// <param name="SuppressedUsageCount">Matching usages that were counted but not retained.</param>
public sealed record LookupUsageReport(
    TableName LookupTable,
    ulong LookupId,
    int UsageCount,
    IReadOnlyList<LookupUsage> Usages,
    int ScannedTableCount,
    int FailedTableCount,
    int SuppressedUsageCount)
{
    /// <summary>
    /// Gets a value indicating whether at least one record field references the lookup value.
    /// </summary>
    public bool IsUsed => UsageCount > 0;
}

/// <summary>
/// Finds the record fields that reference one lookup value. A plain lookup id is matched exactly. For a
/// bit-mask lookup a single-bit entry is matched when the mask contains that bit, and a named combination
/// is matched only when the mask equals it, because the combination names that exact set.
/// </summary>
public sealed class LookupUsageService
{
    /// <summary>
    /// Maximum number of retained usages across all tables.
    /// </summary>
    public const int MaxUsages = 1000;

    /// <summary>
    /// Maximum number of retained usages from one table.
    /// </summary>
    public const int MaxUsagesPerTable = 200;

    private readonly ITableMetadataProvider? _metadataProvider;
    private readonly TableFileLoader _loader;

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupUsageService"/> class.
    /// </summary>
    /// <param name="metadataProvider">
    /// Provider used to discover columns that reference a lookup. Without it the scan cannot prove
    /// anything and fails with <c>metadata.not_found</c>.
    /// </param>
    /// <param name="loader">Optional table loader; a default loader is used when omitted.</param>
    public LookupUsageService(ITableMetadataProvider? metadataProvider = null, TableFileLoader? loader = null)
    {
        _metadataProvider = metadataProvider;
        _loader = loader ?? new TableFileLoader();
    }

    /// <summary>
    /// Scans every table that declares a column referencing the lookup and collects fields whose value
    /// references <paramref name="lookupId"/>.
    /// </summary>
    /// <param name="tables">All table descriptors of the data set.</param>
    /// <param name="lookupTable">Lookup table that owns the value.</param>
    /// <param name="lookupId">Identifier to search for.</param>
    /// <param name="isFlagLookup">Whether the lookup ids are bit flags.</param>
    /// <param name="progress">Optional progress over the referencing tables.</param>
    /// <param name="cancellationToken">Token to cancel the scan.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the exact usage count
    /// and the capped usage list. On failure, <c>error_code</c> is <c>metadata.not_found</c> or a metadata
    /// reading error.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tables"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult<LookupUsageReport>> FindUsagesAsync(
        IReadOnlyList<TableDescriptor> tables,
        TableName lookupTable,
        ulong lookupId,
        bool isFlagLookup,
        IProgress<DatasetScanProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tables);
        cancellationToken.ThrowIfCancellationRequested();

        if (_metadataProvider is null)
        {
            return OperationResult<LookupUsageReport>.Failure(
                ErrorCodes.MetadataNotFound,
                "Lookup usage requires table metadata.");
        }

        var metadataResult = await _metadataProvider.GetAllTablesAsync(cancellationToken).ConfigureAwait(false);
        if (!metadataResult.IsSuccess)
        {
            return OperationResult<LookupUsageReport>.Failure(
                metadataResult.ErrorCode ?? ErrorCodes.IoError,
                metadataResult.ErrorText ?? "Table metadata could not be read.");
        }

        var metadataByName = metadataResult.Data!
            .ToDictionary(static metadata => metadata.TableName.Value, StringComparer.Ordinal);
        var candidates = new List<(TableDescriptor Descriptor, TableMetadata Metadata, IReadOnlyList<string> Fields)>();
        foreach (var descriptor in tables)
        {
            if (string.Equals(descriptor.TableName.Value, lookupTable.Value, StringComparison.Ordinal)
                || !metadataByName.TryGetValue(descriptor.TableName.Value, out var metadata))
            {
                continue;
            }

            var fields = FindLookupFields(metadata, lookupTable);
            if (fields.Count > 0)
            {
                candidates.Add((descriptor, metadata, fields));
            }
        }

        var usages = new List<LookupUsage>();
        var usageCount = 0;
        var suppressed = 0;
        var failed = 0;
        var scanned = 0;
        foreach (var candidate in candidates)
        {
            cancellationToken.ThrowIfCancellationRequested();
            scanned++;
            progress?.Report(new DatasetScanProgress(
                scanned,
                candidates.Count,
                candidate.Descriptor.TableName));

            var load = await _loader
                .LoadAsync(candidate.Descriptor, candidate.Metadata, cancellationToken)
                .ConfigureAwait(false);
            var document = load.IsSuccess ? load.Data?.GetDocument<TableDocument>() : null;
            if (document is null
                || document.Status is not (TableDocumentStatus.Loaded or TableDocumentStatus.LoadedReadOnly))
            {
                failed++;
                continue;
            }

            var tableHits = 0;
            foreach (var record in document.Records)
            {
                foreach (var field in candidate.Fields)
                {
                    var raw = record.GetRawValue(field);
                    if (string.IsNullOrWhiteSpace(raw)
                        || !ulong.TryParse(raw.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var value)
                        || !Matches(value, lookupId, isFlagLookup))
                    {
                        continue;
                    }

                    usageCount++;
                    if (usages.Count < MaxUsages && tableHits < MaxUsagesPerTable)
                    {
                        usages.Add(new LookupUsage(
                            candidate.Descriptor.TableName,
                            record.GetKey(),
                            field,
                            raw.Trim()));
                        tableHits++;
                    }
                    else
                    {
                        suppressed++;
                    }
                }
            }
        }

        return OperationResult<LookupUsageReport>.Success(new LookupUsageReport(
            lookupTable,
            lookupId,
            usageCount,
            usages,
            scanned,
            failed,
            suppressed));
    }

    private static IReadOnlyList<string> FindLookupFields(TableMetadata metadata, TableName lookupTable)
    {
        var fields = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var column in metadata.Columns)
        {
            if (string.Equals(column.LookupTable, lookupTable.Value, StringComparison.Ordinal)
                && seen.Add(column.XmlFieldName))
            {
                fields.Add(column.XmlFieldName);
            }
        }

        foreach (var hint in metadata.ColumnHints)
        {
            if (string.Equals(hint.LookupTable, lookupTable.Value, StringComparison.Ordinal)
                && seen.Add(hint.XmlFieldName))
            {
                fields.Add(hint.XmlFieldName);
            }
        }

        return fields;
    }

    private static bool Matches(ulong value, ulong lookupId, bool isFlagLookup)
    {
        if (!isFlagLookup)
        {
            return value == lookupId;
        }

        if (lookupId == 0)
        {
            return value == 0;
        }

        return LookupIdAllocator.IsSingleBit(lookupId)
            ? (value & lookupId) != 0
            : value == lookupId;
    }
}
