using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Diagnostics;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Lookups;
using XmlEditorReloaded.Xml.Saving;

namespace XmlEditorReloaded.Xml.Copy;

/// <summary>
/// Metadata-driven copy of records between two <c>TableData</c> directories. The source directory is
/// read-only: it is loaded, never saved. The target is planned in memory first, then written file by
/// file with atomic replaces; a failure restores the files that were already replaced.
/// </summary>
/// <remarks>
/// Records are copied as XML elements, so unknown fields, repeated elements, sparse shapes, and
/// lexical values survive unchanged. Reference fields can be rewritten by display text when the two
/// data sets assign different identifiers to the same lookup or relation target. A lookup row whose
/// key is absent from the target is additionally matched by the curated comparison field, normally the
/// display name, so an existing row is updated or skipped instead of duplicated.
/// </remarks>
public sealed class TableCopyService : ITableCopyService
{
    private static readonly string[] PreferredDisplayFields =
    {
        "szItemName",
        "szLongItemName",
        "szName",
        "Name",
        "name",
        "szMenuRowText",
        "szDescription",
    };

    private readonly ITableCatalog _catalog;
    private readonly ITableMetadataProvider _metadataProvider;
    private readonly TableFileLoader _loader = new();
    private readonly TableFileSaver _saver = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TableCopyService"/> class.
    /// </summary>
    /// <param name="catalog">Catalog used to discover tables in both data sets.</param>
    /// <param name="metadataProvider">Metadata provider shared by both data sets.</param>
    /// <exception cref="ArgumentNullException">Thrown when a dependency is null.</exception>
    public TableCopyService(ITableCatalog catalog, ITableMetadataProvider metadataProvider)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(metadataProvider);
        _catalog = catalog;
        _metadataProvider = metadataProvider;
    }

    /// <inheritdoc />
    public async Task<OperationResult<TableCopyPreview>> PreviewAsync(
        TableCopyRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var plan = await BuildPlanAsync(request, cancellationToken).ConfigureAwait(false);
        if (!plan.IsSuccess)
        {
            return OperationResult<TableCopyPreview>.Failure(plan);
        }

        var context = plan.Data!;
        var records = context.Records
            .Select(static record => new TableCopyRecordPreview(
                record.SourceKeyDisplay,
                record.SourceDisplay,
                record.TargetKeyDisplay,
                record.Action,
                record.Remaps,
                record.Related.Select(CreateRelatedPreview).ToArray(),
                record.Warnings))
            .ToArray();

        return OperationResult<TableCopyPreview>.Success(new TableCopyPreview(
            request.TableName,
            context.Source.Directory,
            context.Target.Directory,
            records,
            context.Warnings));
    }

    /// <inheritdoc />
    public async Task<OperationResult<TableCopyResult>> ApplyAsync(
        TableCopyRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var plan = await BuildPlanAsync(request, cancellationToken).ConfigureAwait(false);
        if (!plan.IsSuccess)
        {
            return OperationResult<TableCopyResult>.Failure(plan);
        }

        var context = plan.Data!;
        if (context.Records.Count == 0)
        {
            return OperationResult<TableCopyResult>.Failure(
                ErrorCodes.CopyNoRecords,
                $"No source records were selected from table '{request.TableName}'.");
        }

        var counts = new Dictionary<TableName, TableCounts>();
        var order = new List<TableName> { context.TargetTable.Descriptor.TableName };
        var dirty = new List<TableDocument>();
        var dirtySet = new HashSet<TableDocument>();

        var primaryKeyField = context.TargetTable.HasCompoundKey ? null : context.TargetTable.KeyField;
        var primaryExcluded = context.SourceTable.KeyFields.Count > 0
            ? context.SourceTable.KeyFields.ToArray()
            : primaryKeyField is null
                ? Array.Empty<string>()
                : new[] { primaryKeyField };

        foreach (var record in context.Records)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var primaryCounts = GetCounts(counts, context.TargetTable.Descriptor.TableName);

            switch (record.Action)
            {
                case TableCopyAction.Add:
                    var clone = new XElement(record.Source.Element);
                    ApplyRemapsToClone(clone, record.Remaps);
                    context.TargetTable.Document.InsertRecord(
                        clone,
                        primaryKeyField,
                        record.TargetKey);
                    primaryCounts.Added++;
                    MarkDirty(context.TargetTable.Document, dirty, dirtySet);
                    break;

                case TableCopyAction.Overwrite:
                case TableCopyAction.Merge:
                    ApplyFields(
                        record.Target!.Element,
                        record.Source.Element,
                        overwrite: record.Action == TableCopyAction.Overwrite,
                        record.Remaps,
                        primaryExcluded);
                    primaryCounts.Updated++;
                    MarkDirty(context.TargetTable.Document, dirty, dirtySet);
                    break;

                default:
                    primaryCounts.Skipped++;
                    break;
            }

            foreach (var related in record.Related)
            {
                var relatedCounts = GetCounts(counts, related.TargetTable.Descriptor.TableName);
                if (!order.Contains(related.TargetTable.Descriptor.TableName))
                {
                    order.Add(related.TargetTable.Descriptor.TableName);
                }

                var relatedExcluded = related.TargetTable.KeyField is { } relatedKeyField
                    ? new[] { related.Relation.TargetValueField, relatedKeyField }
                    : new[] { related.Relation.TargetValueField };

                foreach (var row in related.Rows)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (row.Action == TableCopyAction.Skip)
                    {
                        relatedCounts.Skipped++;
                        continue;
                    }

                    if (row.Existing is null)
                    {
                        var rowClone = new XElement(row.Source.Element);
                        SetSingleField(rowClone, related.Relation.TargetValueField, related.ParentTargetKey);
                        ApplyRemapsToClone(rowClone, row.Remaps);
                        related.TargetTable.Document.InsertRecord(
                            rowClone,
                            related.TargetTable.HasCompoundKey ? null : related.TargetTable.KeyField,
                            row.TargetKey);
                        relatedCounts.Added++;
                    }
                    else
                    {
                        ApplyFields(
                            row.Existing.Element,
                            row.Source.Element,
                            overwrite: row.Action == TableCopyAction.Overwrite,
                            row.Remaps,
                            relatedExcluded);
                        SetSingleField(row.Existing.Element, related.Relation.TargetValueField, related.ParentTargetKey);
                        relatedCounts.Updated++;
                    }

                    MarkDirty(related.TargetTable.Document, dirty, dirtySet);
                }
            }
        }

        var savedFiles = new List<string>();
        var savedOriginals = new List<(string Path, byte[] Bytes)>();
        try
        {
            foreach (var document in dirty)
            {
                var originalBytes = document.OriginalBytes.ToArray();
                var originalHash = document.OriginalSha256;
                var save = await _saver.SaveAsync(document, cancellationToken).ConfigureAwait(false);
                if (!save.IsSuccess)
                {
                    var rollbackFailures = await RestoreFilesAsync(savedOriginals).ConfigureAwait(false);
                    var message = save.ErrorText ?? "The target file could not be saved.";
                    if (rollbackFailures.Count > 0)
                    {
                        message += " Rollback failed for: " + string.Join("; ", rollbackFailures);
                        OperationLog.Error(
                            "TableCopyService",
                            "Copy rollback failed for: " + string.Join("; ", rollbackFailures),
                            save.ErrorCode ?? ErrorCodes.IoError);
                    }

                    return OperationResult<TableCopyResult>.Failure(save.ErrorCode ?? ErrorCodes.IoError, message);
                }

                if (!string.Equals(originalHash, document.OriginalSha256, StringComparison.Ordinal))
                {
                    savedFiles.Add(document.Descriptor.FilePath);
                    savedOriginals.Add((document.Descriptor.FilePath, originalBytes));
                }
            }
        }
        catch (OperationCanceledException)
        {
            await RestoreFilesAsync(savedOriginals).ConfigureAwait(false);
            throw;
        }

        var tables = order
            .Select(name => new TableCopyAppliedTable(
                name,
                counts[name].Added,
                counts[name].Updated,
                counts[name].Skipped))
            .ToArray();

        return OperationResult<TableCopyResult>.Success(new TableCopyResult(
            request.TableName,
            tables,
            savedFiles,
            context.Warnings));
    }

    private async Task<OperationResult<CopyContext>> BuildPlanAsync(
        TableCopyRequest request,
        CancellationToken cancellationToken)
    {
        if (SameDirectory(request.SourceTableDataDirectory, request.TargetTableDataDirectory))
        {
            return OperationResult<CopyContext>.Failure(
                ErrorCodes.CopySameDirectory,
                "The source and target data sets are the same directory; the source must stay read-only.");
        }

        var source = new DataSetAccessor(
            request.SourceTableDataDirectory,
            _catalog,
            _metadataProvider,
            _loader);
        var target = new DataSetAccessor(
            request.TargetTableDataDirectory,
            _catalog,
            _metadataProvider,
            _loader);

        var sourceResult = await source.LoadAsync(request.TableName, cancellationToken).ConfigureAwait(false);
        if (!sourceResult.IsSuccess)
        {
            return OperationResult<CopyContext>.Failure(sourceResult);
        }

        var sourceTable = sourceResult.Data!;
        if (sourceTable.Document.Records.Count == 0
            && sourceTable.Document.Xml.Root?.HasElements == true)
        {
            return OperationResult<CopyContext>.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Table '{request.TableName}' stores nested records and cannot be copied as flat rows.");
        }

        var targetResult = await target.LoadAsync(request.TableName, cancellationToken).ConfigureAwait(false);
        if (!targetResult.IsSuccess)
        {
            if (targetResult.ErrorCode == ErrorCodes.IoFileNotFound)
            {
                return OperationResult<CopyContext>.Failure(
                    ErrorCodes.CopyTargetTableMissing,
                    $"The target data set does not contain table '{request.TableName}'.");
            }

            return OperationResult<CopyContext>.Failure(targetResult);
        }

        var targetTable = targetResult.Data!;
        var context = new CopyContext(request, source, target, sourceTable, targetTable);
        context.Remapper = new TableCopyRemapper(
            sourceTable.Metadata,
            _metadataProvider,
            source.LoadDocumentTableAsync,
            target.LoadDocumentTableAsync,
            source.LoadLookupAsync,
            target.LoadLookupAsync);
        await context.Remapper.PrepareRelationsAsync(cancellationToken).ConfigureAwait(false);
        await LoadRelatedRelationsAsync(context, cancellationToken).ConfigureAwait(false);

        var selected = SelectRecords(context, out var missingKeys);
        foreach (var missing in missingKeys)
        {
            context.Warnings.Add($"Record '{missing}' was not found in the source table.");
        }

        if (request.KeyMode == TableCopyKeyMode.Allocate
            && (sourceTable.HasCompoundKey || targetTable.HasCompoundKey))
        {
            return OperationResult<CopyContext>.Failure(
                ErrorCodes.CopyAllocateUnsupported,
                $"Table '{request.TableName}' uses a compound record key; keys cannot be allocated automatically.");
        }

        var allocate = !sourceTable.HasCompoundKey
            && (request.KeyMode == TableCopyKeyMode.Allocate
                || sourceTable.PositionalKey
                || sourceTable.KeyField is null);

        foreach (var sourceRecord in selected)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sourceKey = sourceRecord.GetKey() ?? string.Empty;
            var targetRecord = !allocate && !string.IsNullOrWhiteSpace(sourceKey)
                ? targetTable.Document.FindRecord(sourceKey)
                : null;

            var comparisonMatchCount = 0;
            if (targetRecord is null && !string.IsNullOrWhiteSpace(sourceKey))
            {
                var comparisonMatches = FindComparisonMatches(targetTable, sourceRecord, sourceTable.ComparisonField);
                comparisonMatchCount = comparisonMatches.Count;
                if (comparisonMatches.Count == 1)
                {
                    targetRecord = comparisonMatches[0];
                }
            }

            TableCopyAction action;
            string? targetKey;
            if (allocate && targetRecord is null)
            {
                action = TableCopyAction.Add;
                targetKey = AllocateKey(context, targetTable);
            }
            else if (string.IsNullOrWhiteSpace(sourceKey))
            {
                action = TableCopyAction.Add;
                targetKey = AllocateKey(context, targetTable);
            }
            else if (targetRecord is null)
            {
                action = TableCopyAction.Add;
                targetKey = sourceKey;
            }
            else
            {
                action = request.ConflictPolicy switch
                {
                    TableCopyConflictPolicy.Overwrite => TableCopyAction.Overwrite,
                    TableCopyConflictPolicy.Merge => TableCopyAction.Merge,
                    _ => TableCopyAction.Skip,
                };
                targetKey = targetRecord.GetKey() ?? sourceKey;
            }

            var excluded = sourceTable.KeyFields.Count > 0
                ? sourceTable.KeyFields.ToArray()
                : sourceTable.KeyField is { } sourceKeyField
                    ? new[] { sourceKeyField }
                    : Array.Empty<string>();
            var fields = GetFieldsToCopy(sourceRecord, targetRecord, action, excluded);
            var remaps = await context.Remapper
                .MapRecordAsync(sourceRecord, fields, excluded, cancellationToken)
                .ConfigureAwait(false);

            var plan = new CopyRecordPlan
            {
                Source = sourceRecord,
                Target = targetRecord,
                SourceKey = sourceKey,
                SourceKeyDisplay = sourceRecord.KeyDisplay,
                SourceDisplay = ReadDisplay(sourceRecord, sourceTable.DisplayField),
                TargetKey = targetKey,
                TargetKeyDisplay = targetRecord?.KeyDisplay
                    ?? (string.Equals(targetKey, sourceKey, StringComparison.Ordinal)
                        ? sourceRecord.KeyDisplay
                        : targetKey),
                Action = action,
                Remaps = remaps,
            };

            if (comparisonMatchCount > 1)
            {
                var warning =
                    $"Comparison field '{sourceTable.ComparisonField}' matches {comparisonMatchCount} "
                    + "target records; the row was added instead of updated.";
                plan.Warnings.Add(warning);
                context.AddWarningOnce($"comparison.ambiguous:{sourceKey}", warning);
            }

            AddRemapWarnings(plan.Warnings, remaps);

            if (request.CopyRelatedRecords
                && !string.IsNullOrWhiteSpace(sourceKey)
                && targetKey is not null
                && action != TableCopyAction.Skip)
            {
                plan.Related = await BuildRelatedPlansAsync(
                        context,
                        sourceRecord,
                        sourceKey,
                        targetKey,
                        action,
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            context.Records.Add(plan);
        }

        return OperationResult<CopyContext>.Success(context);
    }

    private async Task LoadRelatedRelationsAsync(CopyContext context, CancellationToken cancellationToken)
    {
        if (!context.Request.CopyRelatedRecords || context.SourceTable.KeyField is null)
        {
            return;
        }

        var relations = await _metadataProvider
            .GetRelationsAsync(context.Request.TableName, cancellationToken)
            .ConfigureAwait(false);
        if (!relations.IsSuccess)
        {
            context.Warnings.Add($"{relations.ErrorCode}: {relations.ErrorText}");
            return;
        }

        context.RelatedRelations = relations.Data!
            .Where(relation =>
                relation.Kind is TableRelationKind.SubTable or TableRelationKind.ChildRows
                && string.Equals(
                    relation.SourceField,
                    context.SourceTable.KeyField,
                    StringComparison.Ordinal))
            .ToArray();
    }

    private async Task<IReadOnlyList<RelatedPlan>> BuildRelatedPlansAsync(
        CopyContext context,
        TableRecord sourceRecord,
        string sourceKey,
        string parentTargetKey,
        TableCopyAction primaryAction,
        CancellationToken cancellationToken)
    {
        var plans = new List<RelatedPlan>();
        foreach (var relation in context.RelatedRelations)
        {
            var sourceRelated = await context.Source
                .LoadAsync(relation.TargetTable, cancellationToken)
                .ConfigureAwait(false);
            if (!sourceRelated.IsSuccess)
            {
                context.AddWarningOnce(
                    $"source:{relation.TargetTable}:{sourceRelated.ErrorCode}",
                    $"Related table '{relation.TargetTable}' could not be read from the source: {sourceRelated.ErrorText}");
                continue;
            }

            var targetRelated = await context.Target
                .LoadAsync(relation.TargetTable, cancellationToken)
                .ConfigureAwait(false);
            if (!targetRelated.IsSuccess)
            {
                context.AddWarningOnce(
                    $"target:{relation.TargetTable}:{targetRelated.ErrorCode}",
                    $"Related table '{relation.TargetTable}' could not be read from the target: {targetRelated.ErrorText}");
                continue;
            }

            var sourceRows = sourceRelated.Data!.Document.Records
                .Where(row => string.Equals(
                    row.GetRawValue(relation.TargetValueField)?.Trim(),
                    sourceKey.Trim(),
                    StringComparison.Ordinal))
                .ToArray();
            if (sourceRows.Length == 0)
            {
                continue;
            }

            var targetTable = targetRelated.Data!;
            var remapper = await context
                .GetRemapperAsync(relation.TargetTable, sourceRelated.Data.Metadata, cancellationToken)
                .ConfigureAwait(false);
            var relatedKeyField = targetTable.KeyField;
            var excluded = targetTable.KeyFields.Count > 1
                ? targetTable.KeyFields
                    .Append(relation.TargetValueField)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray()
                : relatedKeyField is null
                    ? new[] { relation.TargetValueField }
                    : new[] { relation.TargetValueField, relatedKeyField };
            var rows = new List<RelatedRowPlan>();
            var warnings = new List<string>();

            if (relation.Kind == TableRelationKind.ChildRows
                && primaryAction != TableCopyAction.Add)
            {
                warnings.Add(
                    $"Related rows of '{relation.TargetTable}' were not copied because record "
                    + $"'{sourceKey}' already exists in the target.");
                rows.AddRange(sourceRows.Select(static row => new RelatedRowPlan
                {
                    Source = row,
                    Existing = null,
                    TargetKey = null,
                    Action = TableCopyAction.Skip,
                    Remaps = Array.Empty<TableCopyRemap>(),
                }));
                plans.Add(CreateRelatedPlan(relation, sourceRelated.Data, targetTable, parentTargetKey, rows, warnings));
                continue;
            }

            var existingForParent = targetTable.Document.Records.FirstOrDefault(row =>
                string.Equals(
                    row.GetRawValue(relation.TargetValueField)?.Trim(),
                    parentTargetKey.Trim(),
                    StringComparison.Ordinal));

            if (relation.Kind == TableRelationKind.ChildRows
                && existingForParent is not null)
            {
                warnings.Add(
                    $"Related rows of '{relation.TargetTable}' were not copied because the target already "
                    + $"contains rows for key '{parentTargetKey}'.");
                rows.AddRange(sourceRows.Select(static row => new RelatedRowPlan
                {
                    Source = row,
                    Existing = null,
                    TargetKey = null,
                    Action = TableCopyAction.Skip,
                    Remaps = Array.Empty<TableCopyRemap>(),
                }));
                plans.Add(CreateRelatedPlan(relation, sourceRelated.Data, targetTable, parentTargetKey, rows, warnings));
                continue;
            }

            if (relation.Kind == TableRelationKind.ChildRows
                && targetTable.KeyField is not null
                && string.Equals(targetTable.KeyField, relation.TargetValueField, StringComparison.Ordinal))
            {
                warnings.Add(
                    $"Related rows of '{relation.TargetTable}' were not copied because its key is the "
                    + "parent link field; copying several rows would collide.");
                rows.AddRange(sourceRows.Select(static row => new RelatedRowPlan
                {
                    Source = row,
                    Existing = null,
                    TargetKey = null,
                    Action = TableCopyAction.Skip,
                    Remaps = Array.Empty<TableCopyRemap>(),
                }));
                plans.Add(CreateRelatedPlan(relation, sourceRelated.Data, targetTable, parentTargetKey, rows, warnings));
                continue;
            }

            foreach (var sourceRow in sourceRows)
            {
                cancellationToken.ThrowIfCancellationRequested();
                TableRecord? existing;
                TableCopyAction action;
                string? rowTargetKey;
                if (relation.Kind == TableRelationKind.SubTable)
                {
                    existing = existingForParent;
                    action = existing is null
                        ? TableCopyAction.Add
                        : context.Request.ConflictPolicy switch
                        {
                            TableCopyConflictPolicy.Overwrite => TableCopyAction.Overwrite,
                            TableCopyConflictPolicy.Merge => TableCopyAction.Merge,
                            _ => TableCopyAction.Skip,
                        };
                    rowTargetKey = existing?.GetKey()
                        ?? BuildRelatedTargetKey(targetTable, sourceRow, relation.TargetValueField, parentTargetKey)
                        ?? (string.Equals(targetTable.KeyField, relation.TargetValueField, StringComparison.Ordinal)
                            ? parentTargetKey
                            : AllocateKey(context, targetTable));
                }
                else
                {
                    existing = null;
                    action = TableCopyAction.Add;
                    rowTargetKey = BuildRelatedTargetKey(targetTable, sourceRow, relation.TargetValueField, parentTargetKey)
                        ?? AllocateKey(context, targetTable);
                }

                var rowFields = GetFieldsToCopy(sourceRow, existing, action, excluded);
                var rowRemaps = action == TableCopyAction.Skip
                    ? Array.Empty<TableCopyRemap>()
                    : await remapper
                        .MapRecordAsync(sourceRow, rowFields, excluded, cancellationToken)
                        .ConfigureAwait(false);

                rows.Add(new RelatedRowPlan
                {
                    Source = sourceRow,
                    Existing = existing,
                    TargetKey = rowTargetKey,
                    Action = action,
                    Remaps = rowRemaps,
                });
                AddRemapWarnings(warnings, rowRemaps);
            }

            plans.Add(CreateRelatedPlan(relation, sourceRelated.Data, targetTable, parentTargetKey, rows, warnings));
        }

        return plans;
    }

    private static RelatedPlan CreateRelatedPlan(
        TableRelation relation,
        TableLoad sourceTable,
        TableLoad targetTable,
        string parentTargetKey,
        IReadOnlyList<RelatedRowPlan> rows,
        IReadOnlyList<string> warnings) =>
        new()
        {
            Relation = relation,
            SourceTable = sourceTable,
            TargetTable = targetTable,
            ParentTargetKey = parentTargetKey,
            Rows = rows,
            Warnings = warnings,
            RemapCount = rows.Sum(static row =>
                row.Remaps.Count(static remap => remap.Status == TableCopyRemapStatus.Remapped)),
        };

    private static TableCopyRelatedPreview CreateRelatedPreview(RelatedPlan plan) =>
        new(
            plan.Relation.TargetTable,
            plan.Relation.Kind,
            plan.Rows.Count,
            plan.Rows.Count(static row => row.Action == TableCopyAction.Add),
            plan.Rows.Count(static row => row.Action is TableCopyAction.Overwrite or TableCopyAction.Merge),
            plan.Rows.Count(static row => row.Action == TableCopyAction.Skip),
            plan.RemapCount,
            plan.Warnings);

    private static IReadOnlyList<TableRecord> SelectRecords(CopyContext context, out IReadOnlyList<string> missingKeys)
    {
        if (context.Request.RecordKeys is null)
        {
            missingKeys = Array.Empty<string>();
            return context.SourceTable.Document.Records;
        }

        var records = new List<TableRecord>();
        var missing = new List<string>();
        foreach (var key in context.Request.RecordKeys.Distinct(StringComparer.Ordinal))
        {
            var record = FindSourceRecord(context.SourceTable, key);
            if (record is null)
            {
                missing.Add(key);
            }
            else
            {
                records.Add(record);
            }
        }

        missingKeys = missing;
        return records;
    }

    private static TableRecord? FindSourceRecord(TableLoad sourceTable, string key)
    {
        var record = sourceTable.Document.FindRecord(key);
        if (record is not null || !sourceTable.HasCompoundKey)
        {
            return record;
        }

        // The copy dialog shows compound keys as "a | b"; accept that form as well as the canonical
        // internal encoding.
        var components = key.Split('|', StringSplitOptions.TrimEntries);
        return components.Length == sourceTable.KeyFields.Count
            ? sourceTable.Document.FindRecord(RecordKeyCodec.Compose(components))
            : null;
    }

    /// <summary>
    /// Finds target records whose comparison field holds the same raw text as the source record. The
    /// comparison is ordinal and case-sensitive: values are neither trimmed nor case-folded, so two
    /// names that differ only in whitespace or casing stay distinct. The legacy editor resolved a
    /// primary-key miss through <c>TableProperty.ComparisonField</c> the same way.
    /// </summary>
    /// <param name="targetTable">Loaded target table that is searched.</param>
    /// <param name="sourceRecord">Source record whose comparison value is matched.</param>
    /// <param name="comparisonField">Comparison field name, or <see langword="null"/> when undeclared.</param>
    /// <returns>The matching target records in document order; an empty list when nothing matches.</returns>
    private static IReadOnlyList<TableRecord> FindComparisonMatches(
        TableLoad targetTable,
        TableRecord sourceRecord,
        string? comparisonField)
    {
        if (comparisonField is null)
        {
            return Array.Empty<TableRecord>();
        }

        var sourceValue = sourceRecord.GetRawValue(comparisonField);
        if (string.IsNullOrEmpty(sourceValue))
        {
            return Array.Empty<TableRecord>();
        }

        return targetTable.Document.Records
            .Where(record => string.Equals(
                record.GetRawValue(comparisonField),
                sourceValue,
                StringComparison.Ordinal))
            .ToArray();
    }

    private static IReadOnlyList<string> GetFieldsToCopy(
        TableRecord source,
        TableRecord? target,
        TableCopyAction action,
        IReadOnlyCollection<string> excluded)
    {
        if (action == TableCopyAction.Skip)
        {
            return Array.Empty<string>();
        }

        return source.FieldNames
            .Where(field => !excluded.Contains(field))
            .Where(field => action == TableCopyAction.Overwrite
                || action == TableCopyAction.Add
                || target is null
                || !target.HasField(field))
            .ToArray();
    }

    private static string? ReadDisplay(TableRecord record, string? displayField) =>
        displayField is null ? null : record.GetRawValue(displayField)?.Trim();

    private static string? BuildRelatedTargetKey(
        TableLoad targetTable,
        TableRecord sourceRow,
        string parentLinkField,
        string parentTargetKey)
    {
        if (!targetTable.HasCompoundKey)
        {
            return null;
        }

        var components = new string?[targetTable.KeyFields.Count];
        for (var index = 0; index < targetTable.KeyFields.Count; index++)
        {
            var field = targetTable.KeyFields[index];
            components[index] = string.Equals(field, parentLinkField, StringComparison.Ordinal)
                ? parentTargetKey
                : sourceRow.GetRawValue(field);
        }

        return components.All(static component => !string.IsNullOrEmpty(component))
            ? RecordKeyCodec.Compose(components)
            : null;
    }

    private static string? AllocateKey(CopyContext context, TableLoad table)
    {
        if (table.KeyField is null || table.HasCompoundKey)
        {
            return null;
        }

        var tableName = table.Descriptor.TableName;
        if (!context.NextKeys.TryGetValue(tableName, out var next))
        {
            next = ComputeNextKey(table.Document, table.KeyField);
        }

        context.NextKeys[tableName] = next + 1;
        return next.ToString(CultureInfo.InvariantCulture);
    }

    private static long ComputeNextKey(TableDocument document, string keyField)
    {
        long maximum = -1;
        var hasInteger = false;
        foreach (var record in document.Records)
        {
            if (long.TryParse(
                    record.GetRawValue(keyField)?.Trim(),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var value)
                && value >= 0)
            {
                maximum = Math.Max(maximum, value);
                hasInteger = true;
            }
        }

        return hasInteger ? maximum + 1 : document.Records.Count;
    }

    private static void ApplyRemapsToClone(
        XElement clone,
        IReadOnlyList<TableCopyRemap> remaps)
    {
        var map = remaps
            .Where(static remap => remap.Status == TableCopyRemapStatus.Remapped)
            .GroupBy(static remap => remap.Field, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.Last().TargetValue,
                StringComparer.Ordinal);
        if (map.Count == 0)
        {
            return;
        }

        foreach (var element in clone.Elements())
        {
            if (!element.HasElements && map.TryGetValue(element.Name.LocalName, out var value))
            {
                element.Value = value;
            }
        }
    }

    private static void ApplyFields(
        XElement target,
        XElement source,
        bool overwrite,
        IReadOnlyList<TableCopyRemap> remaps,
        IReadOnlyCollection<string> excluded)
    {
        var map = remaps
            .Where(static remap => remap.Status == TableCopyRemapStatus.Remapped)
            .GroupBy(static remap => remap.Field, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.Last().TargetValue,
                StringComparer.Ordinal);

        foreach (var attribute in source.Attributes())
        {
            if (attribute.IsNamespaceDeclaration)
            {
                continue;
            }

            var existing = target.Attribute(attribute.Name);
            if (existing is null)
            {
                target.SetAttributeValue(attribute.Name, attribute.Value);
            }
            else if (overwrite)
            {
                existing.Value = attribute.Value;
            }
        }

        foreach (var group in source.Elements().GroupBy(
                     static element => element.Name.LocalName,
                     StringComparer.Ordinal))
        {
            if (excluded.Contains(group.Key))
            {
                continue;
            }

            var clones = group
                .Select(element => CloneField(element, map))
                .ToList();
            var existing = target.Elements()
                .Where(element => string.Equals(
                    element.Name.LocalName,
                    group.Key,
                    StringComparison.Ordinal))
                .ToList();

            if (existing.Count == 0)
            {
                foreach (var clone in clones)
                {
                    target.Add(clone);
                }
            }
            else if (overwrite)
            {
                var anchor = existing[0];
                foreach (var clone in clones)
                {
                    anchor.AddBeforeSelf(clone);
                }

                foreach (var element in existing)
                {
                    element.Remove();
                }
            }
        }
    }

    private static XElement CloneField(XElement element, IReadOnlyDictionary<string, string> remaps)
    {
        var clone = new XElement(element);
        if (!clone.HasElements && remaps.TryGetValue(clone.Name.LocalName, out var value))
        {
            clone.Value = value;
        }

        return clone;
    }

    private static void SetSingleField(XElement record, string field, string value)
    {
        var element = record.Elements().FirstOrDefault(child =>
            string.Equals(child.Name.LocalName, field, StringComparison.Ordinal));
        if (element is null)
        {
            record.Add(new XElement(XName.Get(field), value));
        }
        else
        {
            element.Value = value;
        }
    }

    private static void MarkDirty(
        TableDocument document,
        ICollection<TableDocument> dirty,
        ISet<TableDocument> seen)
    {
        document.MarkDirty();
        if (seen.Add(document))
        {
            dirty.Add(document);
        }
    }

    private static TableCounts GetCounts(
        IDictionary<TableName, TableCounts> counts,
        TableName tableName)
    {
        if (!counts.TryGetValue(tableName, out var value))
        {
            value = new TableCounts();
            counts[tableName] = value;
        }

        return value;
    }

    private static void AddRemapWarnings(
        List<string> warnings,
        IReadOnlyList<TableCopyRemap> remaps)
    {
        var unresolved = remaps
            .Where(static remap => remap.Status != TableCopyRemapStatus.Remapped)
            .Take(5)
            .ToArray();
        foreach (var remap in unresolved)
        {
            warnings.Add(DescribeRemap(remap));
        }

        var remaining = remaps.Count(static remap => remap.Status != TableCopyRemapStatus.Remapped)
            - unresolved.Length;
        if (remaining > 0)
        {
            warnings.Add($"... and {remaining} more unresolved references.");
        }
    }

    private static string DescribeRemap(TableCopyRemap remap) => remap.Status switch
    {
        TableCopyRemapStatus.Ambiguous =>
            $"Field '{remap.Field}': display text '{remap.DisplayText}' matches several target records; "
            + $"value '{remap.SourceValue}' was kept.",
        TableCopyRemapStatus.NotFound =>
            $"Field '{remap.Field}': display text '{remap.DisplayText}' was not found in the target; "
            + $"value '{remap.SourceValue}' was kept.",
        _ => $"Field '{remap.Field}': value '{remap.SourceValue}' was remapped to '{remap.TargetValue}'.",
    };

    private static bool SameDirectory(string first, string second)
    {
        try
        {
            return string.Equals(
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(first)),
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(second)),
                StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception exception) when (
            exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static async Task<IReadOnlyList<string>> RestoreFilesAsync(
        IReadOnlyList<(string Path, byte[] Bytes)> files)
    {
        var failures = new List<string>();
        for (var index = files.Count - 1; index >= 0; index--)
        {
            var (path, bytes) = files[index];
            var temporary = path + "." + Guid.NewGuid().ToString("N") + ".rollback";
            try
            {
                await File.WriteAllBytesAsync(temporary, bytes, CancellationToken.None).ConfigureAwait(false);
                File.Move(temporary, path, overwrite: true);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                failures.Add($"{path}: {exception.Message}");
            }
            finally
            {
                if (File.Exists(temporary))
                {
                    try
                    {
                        File.Delete(temporary);
                    }
                    catch (IOException)
                    {
                        // A leftover temporary file does not affect the rollback report.
                    }
                }
            }
        }

        return failures;
    }

    private static string? InferKeyField(TableDocument document)
    {
        var first = document.Records.FirstOrDefault();
        return first?.Element.Elements().FirstOrDefault()?.Name.LocalName;
    }

    private static string? InferDisplayField(TableDocument document, TableMetadata? metadata)
    {
        var first = document.Records.FirstOrDefault();
        if (first is null)
        {
            return null;
        }

        foreach (var candidate in PreferredDisplayFields)
        {
            if (first.GetRawValue(candidate) is not null)
            {
                return candidate;
            }
        }

        var named = first.FieldNames.FirstOrDefault(name =>
            name.Contains("name", StringComparison.OrdinalIgnoreCase)
            && first.GetChildFieldNames(name).Count == 0);
        if (named is not null)
        {
            return named;
        }

        if (metadata is not null)
        {
            foreach (var column in metadata.Columns)
            {
                if (column.DataType == ColumnDataType.String && first.HasField(column.XmlFieldName))
                {
                    return column.XmlFieldName;
                }
            }

            foreach (var hint in metadata.ColumnHints)
            {
                if (hint.DataType is null or ColumnDataType.String
                    && first.HasField(hint.XmlFieldName))
                {
                    return hint.XmlFieldName;
                }
            }
        }

        return first.FieldNames.FirstOrDefault(name => first.GetChildFieldNames(name).Count == 0);
    }

    /// <summary>
    /// Resolves the field marked with <see cref="ColumnFlags.ComparisonField"/> from the curated hints
    /// first and from the declared columns second. The result is <see langword="null"/> when no field is
    /// marked or when the marked field already identifies the record, because a key match is resolved
    /// before the alternate search.
    /// </summary>
    /// <param name="metadata">Metadata of the loaded table, or <see langword="null"/> when unmapped.</param>
    /// <param name="keyField">Effective single key field of the loaded table, or <see langword="null"/>.</param>
    /// <param name="keyFields">Compound key fields of the loaded table; empty for a single key.</param>
    /// <returns>The comparison field name, or <see langword="null"/> when it cannot be used.</returns>
    private static string? ResolveComparisonField(
        TableMetadata? metadata,
        string? keyField,
        IReadOnlyList<string> keyFields)
    {
        if (metadata is null)
        {
            return null;
        }

        var candidate = metadata.ColumnHints
                .FirstOrDefault(static hint => hint.Flags?.HasFlag(ColumnFlags.ComparisonField) == true)
                ?.XmlFieldName
            ?? metadata.Columns
                .FirstOrDefault(static column => column.Flags.HasFlag(ColumnFlags.ComparisonField))
                ?.XmlFieldName;
        if (candidate is null
            || string.Equals(candidate, keyField, StringComparison.Ordinal)
            || keyFields.Contains(candidate, StringComparer.Ordinal))
        {
            return null;
        }

        return candidate;
    }

    private sealed class CopyContext
    {
        private readonly HashSet<string> _warningKeys = new(StringComparer.Ordinal);

        public CopyContext(
            TableCopyRequest request,
            DataSetAccessor source,
            DataSetAccessor target,
            TableLoad sourceTable,
            TableLoad targetTable)
        {
            Request = request;
            Source = source;
            Target = target;
            SourceTable = sourceTable;
            TargetTable = targetTable;
        }

        public TableCopyRequest Request { get; }

        public DataSetAccessor Source { get; }

        public DataSetAccessor Target { get; }

        public TableLoad SourceTable { get; }

        public TableLoad TargetTable { get; }

        public TableCopyRemapper Remapper { get; set; } = null!;

        public IReadOnlyList<TableRelation> RelatedRelations { get; set; } = Array.Empty<TableRelation>();

        public List<CopyRecordPlan> Records { get; } = new();

        public List<string> Warnings { get; } = new();

        public Dictionary<TableName, long> NextKeys { get; } = new();

        public Dictionary<TableName, TableCopyRemapper> Remappers { get; } = new();

        public async Task<TableCopyRemapper> GetRemapperAsync(
            TableName tableName,
            TableMetadata? metadata,
            CancellationToken cancellationToken)
        {
            if (Remappers.TryGetValue(tableName, out var existing))
            {
                return existing;
            }

            var remapper = new TableCopyRemapper(
                metadata,
                Source.MetadataProvider,
                Source.LoadDocumentTableAsync,
                Target.LoadDocumentTableAsync,
                Source.LoadLookupAsync,
                Target.LoadLookupAsync);
            await remapper.PrepareRelationsAsync(cancellationToken).ConfigureAwait(false);
            Remappers[tableName] = remapper;
            return remapper;
        }

        public void AddWarningOnce(string key, string message)
        {
            if (_warningKeys.Add(key))
            {
                Warnings.Add(message);
            }
        }
    }

    private sealed class DataSetAccessor
    {
        private readonly ITableCatalog _catalog;
        private readonly ITableMetadataProvider _metadataProvider;
        private readonly TableFileLoader _loader;
        private readonly XmlFileLookupRepository _lookups;
        private readonly Dictionary<TableName, Task<OperationResult<TableLoad>>> _tables = new();
        private readonly Dictionary<string, Task<IReadOnlyList<LookupEntry>?>> _lookupEntries =
            new(StringComparer.Ordinal);
        private Task<OperationResult<IReadOnlyList<TableDescriptor>>>? _descriptors;

        public DataSetAccessor(
            string directory,
            ITableCatalog catalog,
            ITableMetadataProvider metadataProvider,
            TableFileLoader loader)
        {
            Directory = directory;
            _catalog = catalog;
            _metadataProvider = metadataProvider;
            _loader = loader;
            _lookups = new XmlFileLookupRepository(directory, metadataProvider: metadataProvider);
        }

        public string Directory { get; }

        public ITableMetadataProvider MetadataProvider => _metadataProvider;

        public Task<OperationResult<TableLoad>> LoadAsync(
            TableName tableName,
            CancellationToken cancellationToken)
        {
            if (!_tables.TryGetValue(tableName, out var pending))
            {
                pending = LoadCoreAsync(tableName, cancellationToken);
                _tables[tableName] = pending;
            }

            return pending;
        }

        public async Task<DocumentTable?> LoadDocumentTableAsync(
            TableName tableName,
            CancellationToken cancellationToken)
        {
            var result = await LoadAsync(tableName, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess
                ? new DocumentTable(result.Data!.Descriptor, result.Data.Metadata, result.Data.Document)
                : null;
        }

        public async Task<IReadOnlyList<LookupEntry>?> LoadLookupAsync(
            string lookupName,
            CancellationToken cancellationToken)
        {
            if (!_lookupEntries.TryGetValue(lookupName, out var pending))
            {
                pending = LoadLookupCoreAsync(lookupName, cancellationToken);
                _lookupEntries[lookupName] = pending;
            }

            return await pending.ConfigureAwait(false);
        }

        private async Task<IReadOnlyList<LookupEntry>?> LoadLookupCoreAsync(
            string lookupName,
            CancellationToken cancellationToken)
        {
            TableName tableName;
            try
            {
                tableName = TableName.Parse(lookupName);
            }
            catch (ArgumentException)
            {
                return null;
            }

            var result = await _lookups
                .GetEntriesAsync(tableName, languageCode: null, cancellationToken)
                .ConfigureAwait(false);
            return result.IsSuccess ? result.Data : null;
        }

        private async Task<OperationResult<TableLoad>> LoadCoreAsync(
            TableName tableName,
            CancellationToken cancellationToken)
        {
            _descriptors ??= _catalog.GetTablesAsync(Directory, cancellationToken);
            var descriptors = await _descriptors.ConfigureAwait(false);
            if (!descriptors.IsSuccess)
            {
                return OperationResult<TableLoad>.Failure(descriptors);
            }

            var descriptor = descriptors.Data!.FirstOrDefault(candidate =>
                candidate.TableName == tableName);
            if (descriptor is null)
            {
                return OperationResult<TableLoad>.Failure(
                    ErrorCodes.IoFileNotFound,
                    $"Table '{tableName}' was not found in '{Directory}'.");
            }

            TableMetadata? metadata = null;
            var metadataResult = await _metadataProvider
                .GetTableAsync(tableName, cancellationToken)
                .ConfigureAwait(false);
            if (metadataResult.IsSuccess)
            {
                metadata = metadataResult.Data;
            }
            else if (metadataResult.ErrorCode != ErrorCodes.MetadataNotFound)
            {
                return OperationResult<TableLoad>.Failure(metadataResult);
            }

            if (!descriptor.IsMapped && metadata is null)
            {
                descriptor = descriptor with { IsMapped = false };
            }

            var load = await _loader.LoadAsync(descriptor, metadata, cancellationToken).ConfigureAwait(false);
            if (!load.IsSuccess)
            {
                return OperationResult<TableLoad>.Failure(load);
            }

            if (load.Data!.Status == TableDocumentStatus.Malformed)
            {
                var diagnostic = load.Data.Diagnostics.FirstOrDefault();
                return OperationResult<TableLoad>.Failure(
                    ErrorCodes.XmlParseError,
                    diagnostic is null
                        ? $"Table '{tableName}' is malformed."
                        : $"{diagnostic.Code}: {diagnostic.Message}");
            }

            var document = load.Data.GetDocument<TableDocument>();
            if (document is null)
            {
                return OperationResult<TableLoad>.Failure(
                    ErrorCodes.XmlParseError,
                    $"Table '{tableName}' did not produce a document.");
            }

            if (!document.TryEnableEditing() && document.Status != TableDocumentStatus.Loaded)
            {
                var diagnostic = document.Diagnostics.FirstOrDefault();
                return OperationResult<TableLoad>.Failure(
                    ErrorCodes.XmlNotSaveable,
                    diagnostic is null
                        ? $"Table '{tableName}' cannot be edited."
                        : $"{diagnostic.Code}: {diagnostic.Message}");
            }

            var primary = metadata?.RecordKey?.Primary;
            var positional = primary?.Kind == RecordKeyKind.Positional;
            var keyFields = metadata?.RecordKeyFields ?? Array.Empty<string>();
            var keyField = keyFields.Count == 1
                ? keyFields[0]
                : primary is { Kind: RecordKeyKind.Scalar } ? primary.KeyField : null;
            if (!positional && keyFields.Count == 0 && string.IsNullOrEmpty(keyField))
            {
                keyField = InferKeyField(document);
            }

            return OperationResult<TableLoad>.Success(new TableLoad(
                descriptor,
                metadata,
                document,
                keyField,
                keyFields,
                positional,
                InferDisplayField(document, metadata),
                ResolveComparisonField(metadata, keyField, keyFields)));
        }
    }

    private sealed record TableLoad(
        TableDescriptor Descriptor,
        TableMetadata? Metadata,
        TableDocument Document,
        string? KeyField,
        IReadOnlyList<string> KeyFields,
        bool PositionalKey,
        string? DisplayField,
        string? ComparisonField)
    {
        public bool HasCompoundKey => KeyFields.Count > 1;
    }

    private sealed class CopyRecordPlan
    {
        public required TableRecord Source { get; init; }

        public TableRecord? Target { get; init; }

        public required string SourceKey { get; init; }

        public required string SourceKeyDisplay { get; init; }

        public string? SourceDisplay { get; init; }

        public string? TargetKey { get; init; }

        public string? TargetKeyDisplay { get; init; }

        public required TableCopyAction Action { get; init; }

        public required IReadOnlyList<TableCopyRemap> Remaps { get; init; }

        public IReadOnlyList<RelatedPlan> Related { get; set; } = Array.Empty<RelatedPlan>();

        public List<string> Warnings { get; } = new();
    }

    private sealed class RelatedPlan
    {
        public required TableRelation Relation { get; init; }

        public required TableLoad SourceTable { get; init; }

        public required TableLoad TargetTable { get; init; }

        public required string ParentTargetKey { get; init; }

        public required IReadOnlyList<RelatedRowPlan> Rows { get; init; }

        public required IReadOnlyList<string> Warnings { get; init; }

        public required int RemapCount { get; init; }
    }

    private sealed class RelatedRowPlan
    {
        public required TableRecord Source { get; init; }

        public required TableRecord? Existing { get; init; }

        public required string? TargetKey { get; init; }

        public required TableCopyAction Action { get; init; }

        public required IReadOnlyList<TableCopyRemap> Remaps { get; init; }
    }

    private sealed class TableCounts
    {
        public int Added { get; set; }

        public int Updated { get; set; }

        public int Skipped { get; set; }
    }
}
