using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;

namespace XmlEditorReloaded.Xml.Comparison;

/// <summary>
/// Compares two discovered <c>TableData</c> directories table by table. The comparison reads
/// files only: it never opens them for writing and never modifies either data set.
/// </summary>
public sealed class TableDataComparer : ITableComparisonService
{
    private readonly ITableCatalog _catalog;
    private readonly ITableMetadataProvider? _metadataProvider;
    private readonly TableFileLoader _loader;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableDataComparer"/> class.
    /// </summary>
    /// <param name="catalog">Catalog used to discover base tables in both directories.</param>
    /// <param name="metadataProvider">Optional metadata provider used for record elements and lookup references.</param>
    /// <param name="loader">Optional table file loader; a default loader is used when omitted.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="catalog"/> is null.</exception>
    public TableDataComparer(
        ITableCatalog catalog,
        ITableMetadataProvider? metadataProvider = null,
        TableFileLoader? loader = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        _catalog = catalog;
        _metadataProvider = metadataProvider;
        _loader = loader ?? new TableFileLoader();
    }

    /// <inheritdoc />
    public async Task<OperationResult<TableComparisonReport>> CompareAsync(
        string leftDirectory,
        string rightDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(leftDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(rightDirectory);
        cancellationToken.ThrowIfCancellationRequested();

        var leftCatalog = await _catalog.GetTablesAsync(leftDirectory, cancellationToken).ConfigureAwait(false);
        if (!leftCatalog.IsSuccess)
        {
            return OperationResult<TableComparisonReport>.Failure(leftCatalog);
        }

        var rightCatalog = await _catalog.GetTablesAsync(rightDirectory, cancellationToken).ConfigureAwait(false);
        if (!rightCatalog.IsSuccess)
        {
            return OperationResult<TableComparisonReport>.Failure(rightCatalog);
        }

        var warnings = new List<string>();
        var metadata = await LoadMetadataAsync(warnings, cancellationToken).ConfigureAwait(false);

        var leftTables = leftCatalog.Data!.ToDictionary(static descriptor => descriptor.TableName);
        var rightTables = rightCatalog.Data!.ToDictionary(static descriptor => descriptor.TableName);
        var names = leftTables.Keys
            .Union(rightTables.Keys)
            .OrderBy(static name => name.Value, StringComparer.Ordinal)
            .ToList();

        var entries = new List<TableComparisonEntry>(names.Count);
        foreach (var name in names)
        {
            cancellationToken.ThrowIfCancellationRequested();
            leftTables.TryGetValue(name, out var leftDescriptor);
            rightTables.TryGetValue(name, out var rightDescriptor);
            metadata.TryGetValue(name, out var tableMetadata);

            var left = leftDescriptor is null
                ? null
                : await LoadSideAsync(leftDescriptor, tableMetadata, cancellationToken).ConfigureAwait(false);
            var right = rightDescriptor is null
                ? null
                : await LoadSideAsync(rightDescriptor, tableMetadata, cancellationToken).ConfigureAwait(false);

            var status = leftDescriptor is not null && rightDescriptor is not null
                ? TableComparisonStatus.Both
                : leftDescriptor is not null
                    ? TableComparisonStatus.LeftOnly
                    : TableComparisonStatus.RightOnly;
            var relativePath = leftDescriptor?.RelativePath ?? rightDescriptor!.RelativePath;
            // Field comparison is meaningful only when both sides parsed. A malformed or
            // unreadable side would otherwise make every field of the other side look removed.
            var fields = leftDescriptor is not null
                && rightDescriptor is not null
                && (left?.Analysis is null || right?.Analysis is null)
                    ? Array.Empty<FieldComparison>()
                    : CompareFields(left?.Analysis, right?.Analysis);
            var value = left?.Document is not null && right?.Document is not null
                ? TableValueComparer.Compare(left.Document, right.Document, cancellationToken)
                : TableValueComparison.Empty;
            var overlays = await OverlayFileComparer
                .CompareAsync(left?.Document, right?.Document, tableMetadata, _loader, cancellationToken)
                .ConfigureAwait(false);

            entries.Add(new TableComparisonEntry(
                name,
                relativePath,
                status,
                left?.Comparison,
                right?.Comparison,
                fields)
            {
                Value = value,
                Overlays = overlays,
            });
        }

        var lookupReferences = BuildLookupReferences(
            entries,
            leftTables.Keys.ToHashSet(),
            rightTables.Keys.ToHashSet(),
            metadata);
        var summary = BuildSummary(entries);

        return OperationResult<TableComparisonReport>.Success(new TableComparisonReport(
            leftDirectory,
            rightDirectory,
            DateTime.UtcNow,
            summary,
            entries,
            lookupReferences,
            warnings));
    }

    private async Task<Dictionary<TableName, TableMetadata>> LoadMetadataAsync(
        List<string> warnings,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<TableName, TableMetadata>();
        if (_metadataProvider is null)
        {
            warnings.Add(
                "Table metadata is not available in this composition; records were inferred from the documents "
                + "and lookup references were not reported.");
            return result;
        }

        var all = await _metadataProvider.GetAllTablesAsync(cancellationToken).ConfigureAwait(false);
        if (!all.IsSuccess)
        {
            warnings.Add($"Table metadata could not be read: {all.ErrorText}");
            return result;
        }

        foreach (var table in all.Data!)
        {
            result[table.TableName] = table;
        }

        return result;
    }

    private async Task<SideLoad> LoadSideAsync(
        TableDescriptor descriptor,
        TableMetadata? metadata,
        CancellationToken cancellationToken)
    {
        var load = await _loader.LoadAsync(descriptor, metadata, cancellationToken).ConfigureAwait(false);
        if (!load.IsSuccess)
        {
            var code = load.ErrorCode ?? ErrorCodes.IoError;
            var text = load.ErrorText ?? "The table file could not be read.";
            return new SideLoad(
                new TableSideComparison(
                    TableSideStatus.Failed,
                    null,
                    new[] { new LoadDiagnostic(code, text) },
                    text),
                null,
                null);
        }

        var result = load.Data!;
        if (result.Status == TableDocumentStatus.Malformed)
        {
            return new SideLoad(
                new TableSideComparison(TableSideStatus.Malformed, null, result.Diagnostics, null),
                null,
                null);
        }

        var document = result.GetDocument<TableDocument>();
        if (document is null)
        {
            return new SideLoad(
                new TableSideComparison(
                    TableSideStatus.Failed,
                    null,
                    result.Diagnostics,
                    "The load result has no analyzable document."),
                null,
                null);
        }

        var analysis = TableSnapshotAnalyzer.Analyze(document);
        var status = result.Status == TableDocumentStatus.LoadedReadOnly
            ? TableSideStatus.LoadedReadOnly
            : TableSideStatus.Loaded;
        return new SideLoad(
            new TableSideComparison(status, analysis.Snapshot, result.Diagnostics, null),
            analysis,
            document);
    }

    private static IReadOnlyList<FieldComparison> CompareFields(AnalyzedTable? left, AnalyzedTable? right)
    {
        var rightByName = right?.Fields.ToDictionary(static field => field.Name, StringComparer.Ordinal);
        var leftByName = left?.Fields.ToDictionary(static field => field.Name, StringComparer.Ordinal);
        var ordered = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        if (left is not null)
        {
            foreach (var field in left.Fields)
            {
                if (seen.Add(field.Name))
                {
                    ordered.Add(field.Name);
                }
            }
        }

        if (right is not null)
        {
            foreach (var field in right.Fields)
            {
                if (seen.Add(field.Name))
                {
                    ordered.Add(field.Name);
                }
            }
        }

        var fields = new List<FieldComparison>(ordered.Count);
        foreach (var name in ordered)
        {
            var leftField = leftByName?.GetValueOrDefault(name);
            var rightField = rightByName?.GetValueOrDefault(name);
            fields.Add(new FieldComparison(
                name,
                GetFieldStatus(leftField, rightField),
                leftField?.DataType,
                rightField?.DataType,
                leftField?.PresenceCount ?? 0,
                rightField?.PresenceCount ?? 0,
                leftField?.MaxOccurrences ?? 0,
                rightField?.MaxOccurrences ?? 0));
        }

        return fields;
    }

    private static FieldComparisonStatus GetFieldStatus(AnalyzedField? left, AnalyzedField? right)
    {
        if (right is null)
        {
            return FieldComparisonStatus.LeftOnly;
        }

        if (left is null)
        {
            return FieldComparisonStatus.RightOnly;
        }

        if (left.DataType != right.DataType)
        {
            return FieldComparisonStatus.TypeChanged;
        }

        return (left.MaxOccurrences > 1) != (right.MaxOccurrences > 1)
            ? FieldComparisonStatus.CardinalityChanged
            : FieldComparisonStatus.Same;
    }

    private static IReadOnlyList<LookupReferenceComparison> BuildLookupReferences(
        IReadOnlyList<TableComparisonEntry> entries,
        IReadOnlySet<TableName> leftTables,
        IReadOnlySet<TableName> rightTables,
        IReadOnlyDictionary<TableName, TableMetadata> metadata)
    {
        var entriesByName = entries.ToDictionary(static entry => entry.TableName);
        var references = new List<LookupReferenceComparison>();

        foreach (var table in metadata.Values.OrderBy(static table => table.TableName.Value, StringComparer.Ordinal))
        {
            if (!leftTables.Contains(table.TableName) && !rightTables.Contains(table.TableName))
            {
                continue;
            }

            var seen = new HashSet<(string Field, string Lookup)>();
            foreach (var column in table.Columns)
            {
                AddReference(table.TableName, column.XmlFieldName, column.LookupTable);
            }

            foreach (var hint in table.ColumnHints)
            {
                AddReference(table.TableName, hint.XmlFieldName, hint.LookupTable);
            }

            void AddReference(TableName owner, string fieldName, string? lookupTable)
            {
                if (string.IsNullOrWhiteSpace(lookupTable) || !seen.Add((fieldName, lookupTable)))
                {
                    return;
                }

                var lookupName = TableName.Parse(lookupTable);
                references.Add(new LookupReferenceComparison(
                    owner,
                    fieldName,
                    lookupTable,
                    GetAvailability(entriesByName, lookupName, useLeft: true),
                    GetAvailability(entriesByName, lookupName, useLeft: false)));
            }
        }

        return references
            .OrderBy(static reference => reference.Table.Value, StringComparer.Ordinal)
            .ThenBy(static reference => reference.FieldName, StringComparer.Ordinal)
            .ToArray();
    }

    private static LookupAvailability GetAvailability(
        IReadOnlyDictionary<TableName, TableComparisonEntry> entries,
        TableName lookupName,
        bool useLeft)
    {
        if (!entries.TryGetValue(lookupName, out var entry))
        {
            return LookupAvailability.Missing;
        }

        var side = useLeft ? entry.Left : entry.Right;
        if (side is null)
        {
            return LookupAvailability.Missing;
        }

        return side.Status is TableSideStatus.Malformed or TableSideStatus.Failed
            ? LookupAvailability.Unreadable
            : LookupAvailability.Available;
    }

    private static TableComparisonSummary BuildSummary(IReadOnlyList<TableComparisonEntry> entries)
    {
        var common = entries.Where(static entry => entry.Status == TableComparisonStatus.Both).ToList();
        return new TableComparisonSummary(
            entries.Count(static entry => entry.Left is not null),
            entries.Count(static entry => entry.Right is not null),
            common.Count,
            entries.Count(static entry => entry.Status == TableComparisonStatus.LeftOnly),
            entries.Count(static entry => entry.Status == TableComparisonStatus.RightOnly),
            common.Count(static entry => !entry.HasDifferences),
            common.Count(static entry => entry.HasDifferences),
            entries.Count(static entry => entry.HasMalformedSide),
            entries.Count(static entry => entry.HasLoadFailure),
            common.Sum(static entry => entry.AddedFieldCount),
            common.Sum(static entry => entry.RemovedFieldCount),
            common.Sum(static entry => entry.TypeChangedFieldCount),
            common.Sum(static entry => entry.CardinalityChangedFieldCount),
            common.Sum(static entry => entry.Value.AddedRecordCount),
            common.Sum(static entry => entry.Value.RemovedRecordCount),
            common.Sum(static entry => entry.Value.ChangedRecordCount),
            common.Sum(static entry => entry.Value.ChangedFieldCount),
            common.Sum(static entry => entry.Value.RootFieldChangeCount),
            common.Sum(static entry => entry.Overlays.Sum(static overlay =>
                overlay.AddedCellCount + overlay.RemovedCellCount + overlay.ChangedCellCount)),
            entries.Sum(static entry => (long)(entry.Left?.Snapshot?.RecordCount ?? 0)),
            entries.Sum(static entry => (long)(entry.Right?.Snapshot?.RecordCount ?? 0)));
    }

    private sealed record SideLoad(
        TableSideComparison Comparison,
        AnalyzedTable? Analysis,
        TableDocument? Document);
}
