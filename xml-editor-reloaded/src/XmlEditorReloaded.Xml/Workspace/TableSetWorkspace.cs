using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Workspace;

/// <summary>
/// A table that belongs to a <see cref="TableSetWorkspace"/>, together with the relation that links it
/// to the primary table.
/// </summary>
/// <param name="Relation">Metadata relation that produced the member.</param>
/// <param name="Workspace">Loaded workspace of the related table.</param>
public sealed record TableSetMember(TableRelation Relation, TableWorkspace Workspace);

/// <summary>
/// Dependent records that a delete operation removes together with their owning record.
/// </summary>
/// <param name="Table">Table that stores the dependent records.</param>
/// <param name="Keys">Primary keys of the records that will be deleted.</param>
/// <param name="Field">Target field that links each record to the owner.</param>
public sealed record TableDependentDeletion(
    TableName Table,
    IReadOnlyList<string> Keys,
    string Field);

/// <summary>
/// A primary table and the related tables that are edited together, for example an item and its weapon,
/// magazine, attachment, and attachment-info records. All documents share one undo history, so an edit
/// that spans several files is undone and redone as a single step, and saving writes every dirty file
/// independently with per-file atomic replaces.
/// </summary>
/// <remarks>
/// <see cref="TableRelationKind.SubTable"/> and <see cref="TableRelationKind.ChildRows"/> relations of the
/// primary table always become members; <see cref="TableRelationKind.Reverse"/> targets become members when
/// the caller asks for them, for example to edit dealer stock from an item workspace. Reference relations
/// stay read-only lookups. A missing or malformed related file is reported through <see cref="Warnings"/>
/// and never prevents the primary table from opening.
/// </remarks>
public sealed class TableSetWorkspace
{
    private readonly Dictionary<TableName, TableWorkspace> _workspaces;
    private readonly IReadOnlyList<TableSetMember> _members;

    private TableSetWorkspace(
        TableWorkspace primary,
        IReadOnlyList<TableSetMember> members,
        IReadOnlyList<string> warnings)
    {
        Primary = primary;
        _members = members;
        Warnings = warnings;
        _workspaces = new Dictionary<TableName, TableWorkspace> { [primary.Descriptor.TableName] = primary };
        foreach (var member in members)
        {
            _workspaces.TryAdd(member.Workspace.Descriptor.TableName, member.Workspace);
        }
    }

    /// <summary>
    /// Gets the primary table workspace.
    /// </summary>
    public TableWorkspace Primary { get; }

    /// <summary>
    /// Gets the loaded related workspaces in metadata order.
    /// </summary>
    public IReadOnlyList<TableSetMember> Members => _members;

    /// <summary>
    /// Gets non-fatal problems found while loading related tables, for example a missing target file.
    /// </summary>
    public IReadOnlyList<string> Warnings { get; }

    /// <summary>
    /// Gets the history shared by every workspace of the set.
    /// </summary>
    public TableEditHistory History => Primary.History;

    /// <summary>
    /// Gets a value indicating whether any base or overlay document has unsaved changes.
    /// </summary>
    public bool IsDirty => Primary.IsDirty || _members.Any(static member => member.Workspace.IsDirty);

    /// <summary>
    /// Gets a value indicating whether the primary table accepts edits.
    /// </summary>
    public bool CanEdit => Primary.CanEdit;

    /// <summary>
    /// Gets a value indicating whether an edit can be undone.
    /// </summary>
    public bool CanUndo => History.CanUndo;

    /// <summary>
    /// Gets a value indicating whether an undone edit can be redone.
    /// </summary>
    public bool CanRedo => History.CanRedo;

    /// <summary>
    /// Loads the primary table and every 1:1 and 1:N table linked to it by metadata relations.
    /// </summary>
    /// <param name="primaryDescriptor">Descriptor of the primary table.</param>
    /// <param name="catalog">Catalog used to discover related table files.</param>
    /// <param name="metadataProvider">Metadata provider for table definitions and relations.</param>
    /// <param name="tableDataDirectory">Absolute <c>TableData</c> directory of the data set.</param>
    /// <param name="languageCode">Language overlay to load for tables that provide it, or <see langword="null"/>.</param>
    /// <param name="includeReverseRelations">
    /// When <see langword="true"/>, targets of reverse relations are loaded as editable members too,
    /// unless the same table is already loaded through a 1:N relation. The item workspace uses this
    /// to show and edit dealer inventories that reference the selected item.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the set. On failure,
    /// <c>error_code</c> is <c>io.file_not_found</c>, <c>xml.parse_error</c>, or a metadata error for the
    /// primary table. Related-table failures are reported through <see cref="Warnings"/> instead.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public static async Task<OperationResult<TableSetWorkspace>> LoadAsync(
        TableDescriptor primaryDescriptor,
        ITableCatalog catalog,
        ITableMetadataProvider metadataProvider,
        string tableDataDirectory,
        string? languageCode = null,
        bool includeReverseRelations = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(primaryDescriptor);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(metadataProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableDataDirectory);

        var catalogResult = await catalog.GetTablesAsync(tableDataDirectory, cancellationToken).ConfigureAwait(false);
        if (!catalogResult.IsSuccess)
        {
            return OperationResult<TableSetWorkspace>.Failure(catalogResult);
        }

        var descriptors = catalogResult.Data!
            .GroupBy(static descriptor => descriptor.TableName)
            .ToDictionary(static group => group.Key, static group => group.First());
        var primaryCatalogDescriptor = descriptors.GetValueOrDefault(primaryDescriptor.TableName)
            ?? primaryDescriptor;

        LanguageDescriptor? language = null;
        if (!string.IsNullOrWhiteSpace(languageCode))
        {
            var languages = await metadataProvider.GetLanguagesAsync(cancellationToken).ConfigureAwait(false);
            if (!languages.IsSuccess)
            {
                return OperationResult<TableSetWorkspace>.Failure(languages);
            }

            language = languages.Data!.FirstOrDefault(candidate => string.Equals(
                candidate.Code,
                languageCode,
                StringComparison.OrdinalIgnoreCase));
        }

        var primaryMetadataResult = await metadataProvider
            .GetTableAsync(primaryDescriptor.TableName, cancellationToken)
            .ConfigureAwait(false);
        TableMetadata? primaryMetadata = null;
        if (primaryMetadataResult.IsSuccess)
        {
            primaryMetadata = primaryMetadataResult.Data;
        }
        else if (primaryMetadataResult.ErrorCode != ErrorCodes.MetadataNotFound)
        {
            return OperationResult<TableSetWorkspace>.Failure(primaryMetadataResult);
        }

        var history = new TableEditHistory();
        var primaryLoad = await TableWorkspace
            .LoadAsync(
                primaryCatalogDescriptor,
                primaryMetadata,
                ResolveLanguage(primaryCatalogDescriptor, language),
                cancellationToken: cancellationToken,
                history: history)
            .ConfigureAwait(false);
        if (!primaryLoad.IsSuccess)
        {
            return OperationResult<TableSetWorkspace>.Failure(primaryLoad);
        }

        var primary = primaryLoad.Data!;
        var members = new List<TableSetMember>();
        var warnings = new List<string>();
        var loaded = new Dictionary<TableName, TableWorkspace>
        {
            [primary.Descriptor.TableName] = primary,
        };

        if (primaryMetadata is not null)
        {
            var relationsResult = await metadataProvider
                .GetRelationsAsync(primaryDescriptor.TableName, cancellationToken)
                .ConfigureAwait(false);
            if (!relationsResult.IsSuccess)
            {
                warnings.Add($"{relationsResult.ErrorCode}: {relationsResult.ErrorText}");
                return OperationResult<TableSetWorkspace>.Success(
                    new TableSetWorkspace(primary, members, warnings));
            }

            foreach (var relation in relationsResult.Data!)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var isReverseMember = includeReverseRelations
                    && relation.Kind == TableRelationKind.Reverse;
                if (relation.Kind is not (TableRelationKind.SubTable or TableRelationKind.ChildRows)
                    && !isReverseMember)
                {
                    continue;
                }

                if (loaded.TryGetValue(relation.TargetTable, out var alreadyLoaded))
                {
                    // Reverse views of tables that are already loaded through a 1:N relation add no
                    // editable member; the child editor already owns the same workspace.
                    if (!isReverseMember)
                    {
                        members.Add(new TableSetMember(relation, alreadyLoaded));
                    }

                    continue;
                }

                if (!descriptors.TryGetValue(relation.TargetTable, out var descriptor))
                {
                    // Reverse targets are optional views: dealer inventories may legitimately be absent
                    // from a minimal install, and one warning per missing dealer would flood the UI.
                    if (!isReverseMember)
                    {
                        warnings.Add($"Related table '{relation.TargetTable}' was not found in '{tableDataDirectory}'.");
                    }

                    continue;
                }

                var targetMetadata = await metadataProvider
                    .GetTableAsync(relation.TargetTable, cancellationToken)
                    .ConfigureAwait(false);
                if (!targetMetadata.IsSuccess && targetMetadata.ErrorCode != ErrorCodes.MetadataNotFound)
                {
                    warnings.Add($"{relation.TargetTable}: {targetMetadata.ErrorCode}: {targetMetadata.ErrorText}");
                    continue;
                }

                var targetLoad = await TableWorkspace
                    .LoadAsync(
                        descriptor,
                        targetMetadata.IsSuccess ? targetMetadata.Data : null,
                        ResolveLanguage(descriptor, language),
                        cancellationToken: cancellationToken,
                        history: history)
                    .ConfigureAwait(false);
                if (!targetLoad.IsSuccess)
                {
                    warnings.Add($"{relation.TargetTable}: {targetLoad.ErrorCode}: {targetLoad.ErrorText}");
                    continue;
                }

                loaded.Add(relation.TargetTable, targetLoad.Data!);
                members.Add(new TableSetMember(relation, targetLoad.Data!));
            }
        }

        return OperationResult<TableSetWorkspace>.Success(
            new TableSetWorkspace(primary, members, warnings));
    }

    /// <summary>
    /// Returns the loaded workspace of a table, or <see langword="null"/> when it is not part of the set.
    /// </summary>
    /// <param name="tableName">Dotted table name.</param>
    /// <returns>The workspace, or <see langword="null"/>.</returns>
    public TableWorkspace? GetWorkspace(TableName tableName) =>
        _workspaces.GetValueOrDefault(tableName);

    /// <summary>
    /// Returns every relation of the primary table that produced a loaded member.
    /// </summary>
    /// <param name="kind">Kind to filter by.</param>
    /// <returns>Relations in metadata order.</returns>
    public IReadOnlyList<TableSetMember> GetMembers(TableRelationKind kind) =>
        _members.Where(member => member.Relation.Kind == kind).ToArray();

    /// <summary>
    /// Sets a field value in one table of the set.
    /// </summary>
    /// <param name="tableName">Dotted table name.</param>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="path">Field path relative to the record.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">Whether a missing field, including a nested container, is created.</param>
    /// <returns>The result of the underlying workspace edit.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> or <paramref name="rawText"/> is null.</exception>
    public OperationResult SetValue(
        TableName tableName,
        string recordKey,
        FieldPath path,
        string rawText,
        bool addIfMissing = false)
    {
        var workspace = GetWorkspace(tableName);
        return workspace is null
            ? OperationResult.Failure(
                ErrorCodes.MetadataNotFound,
                $"Table '{tableName}' is not part of the loaded set.")
            : workspace.SetValue(recordKey, path, rawText, addIfMissing);
    }

    /// <summary>
    /// Sets a field value using a slash-separated field path.
    /// </summary>
    /// <param name="tableName">Dotted table name.</param>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="fieldPath">Slash-separated field path relative to the record.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">Whether a missing field, including a nested container, is created.</param>
    /// <returns>The result of the underlying workspace edit.</returns>
    public OperationResult SetValue(
        TableName tableName,
        string recordKey,
        string fieldPath,
        string rawText,
        bool addIfMissing = false) =>
        SetValue(tableName, recordKey, FieldPath.Parse(fieldPath), rawText, addIfMissing);

    /// <summary>
    /// Replaces a repeated field of one record in one table of the set.
    /// </summary>
    /// <param name="tableName">Dotted table name.</param>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="fieldName">Case-sensitive XML element name of the repeated field.</param>
    /// <param name="values">New values in the desired order.</param>
    /// <returns>The result of the underlying workspace edit.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
    public OperationResult SetListValues(
        TableName tableName,
        string recordKey,
        string fieldName,
        IReadOnlyList<string> values)
    {
        var workspace = GetWorkspace(tableName);
        return workspace is null
            ? OperationResult.Failure(
                ErrorCodes.MetadataNotFound,
                $"Table '{tableName}' is not part of the loaded set.")
            : workspace.SetListValues(recordKey, fieldName, values);
    }

    /// <summary>
    /// Appends a record with the next primary key to one table of the set.
    /// </summary>
    /// <param name="tableName">Dotted table name.</param>
    /// <param name="initialValues">Optional direct child values of the new record.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the new record.
    /// </returns>
    public OperationResult<TableRecord> AddRecord(
        TableName tableName,
        IReadOnlyDictionary<string, string>? initialValues = null)
    {
        var workspace = GetWorkspace(tableName);
        return workspace is null
            ? OperationResult<TableRecord>.Failure(
                ErrorCodes.MetadataNotFound,
                $"Table '{tableName}' is not part of the loaded set.")
            : workspace.AddRecord(initialValues);
    }

    /// <summary>
    /// Deletes the most recent edit of the whole set.
    /// </summary>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>history.empty</c>.
    /// </returns>
    public OperationResult Undo() => History.Undo();

    /// <summary>
    /// Restores the most recently undone edit of the whole set.
    /// </summary>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>history.empty</c>.
    /// </returns>
    public OperationResult Redo() => History.Redo();

    /// <summary>
    /// Returns the records that a delete of <paramref name="recordKey"/> would remove together with it.
    /// </summary>
    /// <param name="recordKey">Primary key of the record to inspect.</param>
    /// <returns>
    /// One entry per related table that owns dependent records. Class-keyed 1:1 rows are shared between
    /// items and are therefore never reported.
    /// </returns>
    public IReadOnlyList<TableDependentDeletion> DescribeDependents(string recordKey)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        var result = new List<TableDependentDeletion>();
        foreach (var relation in GetKeyedRelations(includeReverse: true))
        {
            var workspace = GetWorkspace(relation.TargetTable);
            if (workspace is null)
            {
                continue;
            }

            var keys = FindDependingKeys(workspace, relation, recordKey);
            if (keys.Count > 0)
            {
                result.Add(new TableDependentDeletion(relation.TargetTable, keys, relation.TargetValueField));
            }
        }

        return result;
    }

    /// <summary>
    /// Duplicates a record of the primary table and the dependent rows that store its key, as one
    /// undoable transaction. Class-keyed 1:1 rows are shared and therefore not copied.
    /// </summary>
    /// <param name="recordKey">Primary key of the record to duplicate.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the new record key.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult<string> DuplicateRecord(string recordKey)
    {
        ArgumentNullException.ThrowIfNull(recordKey);

        if (!CanEdit)
        {
            return OperationResult<string>.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (Primary.BaseDocument.FindRecord(recordKey) is null)
        {
            return OperationResult<string>.Failure(
                ErrorCodes.KeyNotFound,
                $"Record '{recordKey}' was not found.");
        }

        var transaction = new TableEditTransaction();
        var duplicated = Primary.BaseDocument.DuplicateRecord(recordKey);
        if (!duplicated.IsSuccess)
        {
            return OperationResult<string>.Failure(duplicated);
        }

        var newKey = duplicated.Data!.GetKey()
            ?? throw new InvalidOperationException("The duplicated record has no primary key.");
        transaction.Add(ElementSnapshotState.CaptureInsertion(Primary.BaseDocument, duplicated.Data.Element));

        foreach (var relation in GetKeyedRelations())
        {
            var workspace = GetWorkspace(relation.TargetTable);
            if (workspace is null)
            {
                continue;
            }

            if (relation.Kind == TableRelationKind.SubTable)
            {
                var sourceKey = FindValue(workspace, relation.TargetValueField, recordKey);
                if (sourceKey is null)
                {
                    continue;
                }

                var copy = workspace.BaseDocument.DuplicateRecord(sourceKey);
                if (!copy.IsSuccess)
                {
                    History.Record(transaction);
                    return OperationResult<string>.Failure(copy);
                }

                copy.Data!.TrySetValue(relation.TargetValueField, newKey, addIfMissing: true);
                transaction.Add(ElementSnapshotState.CaptureInsertion(workspace.BaseDocument, copy.Data.Element));
                continue;
            }

            foreach (var dependentKey in FindDependingKeys(workspace, relation, recordKey))
            {
                var copy = workspace.BaseDocument.DuplicateRecord(dependentKey);
                if (!copy.IsSuccess)
                {
                    History.Record(transaction);
                    return OperationResult<string>.Failure(copy);
                }

                copy.Data!.TrySetValue(relation.TargetValueField, newKey, addIfMissing: true);
                transaction.Add(ElementSnapshotState.CaptureInsertion(workspace.BaseDocument, copy.Data.Element));
            }
        }

        History.Record(transaction);
        return OperationResult<string>.Success(newKey);
    }

    /// <summary>
    /// Deletes a record of the primary table together with the dependent rows that store its key, as one
    /// undoable transaction. Class-keyed 1:1 rows are shared and therefore kept.
    /// </summary>
    /// <param name="recordKey">Primary key of the record to delete.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult DeleteRecord(string recordKey)
    {
        ArgumentNullException.ThrowIfNull(recordKey);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (Primary.BaseDocument.FindRecord(recordKey) is null)
        {
            return OperationResult.Failure(ErrorCodes.KeyNotFound, $"Record '{recordKey}' was not found.");
        }

        var transaction = new TableEditTransaction();
        foreach (var relation in GetKeyedRelations(includeReverse: true))
        {
            var workspace = GetWorkspace(relation.TargetTable);
            if (workspace is null)
            {
                continue;
            }

            foreach (var dependentKey in FindDependingKeys(workspace, relation, recordKey))
            {
                CaptureDeletion(transaction, workspace, dependentKey);
            }
        }

        CaptureDeletion(transaction, Primary, recordKey);
        History.Record(transaction);
        return OperationResult.Success();
    }

    /// <summary>
    /// Renames the primary key of a record and rewrites the dependent rows that store it, as one
    /// undoable transaction.
    /// </summary>
    /// <param name="recordKey">Current primary key.</param>
    /// <param name="newKey">New primary key.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>key.conflict</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult RenameRecord(string recordKey, string newKey)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(newKey);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (Primary.BaseDocument.FindRecord(recordKey) is null)
        {
            return OperationResult.Failure(ErrorCodes.KeyNotFound, $"Record '{recordKey}' was not found.");
        }

        if (string.Equals(recordKey, newKey, StringComparison.Ordinal))
        {
            return OperationResult.Success();
        }

        if (Primary.BaseDocument.FindRecord(newKey) is not null)
        {
            return OperationResult.Failure(
                ErrorCodes.KeyConflict,
                $"Record '{newKey}' already exists.");
        }

        var keyField = Primary.Metadata?.RecordKey?.Primary.KeyField;
        if (string.IsNullOrEmpty(keyField))
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                "The table has no primary key field to rename.");
        }

        // Conflicts are detected before any mutation so a failed rename leaves the set untouched.
        foreach (var relation in GetKeyedRelations())
        {
            if (relation.Kind != TableRelationKind.SubTable)
            {
                continue;
            }

            var candidate = GetWorkspace(relation.TargetTable);
            if (candidate is not null && FindValue(candidate, relation.TargetValueField, newKey) is not null)
            {
                return OperationResult.Failure(
                    ErrorCodes.KeyConflict,
                    $"Related table '{relation.TargetTable}' already has a row for '{newKey}'.");
            }
        }

        var transaction = new TableEditTransaction();
        var primaryRecord = Primary.BaseDocument.FindRecord(recordKey)!;
        var primaryState = Capture(Primary, recordKey);
        primaryRecord.TrySetValue(keyField, newKey);
        primaryState.CompleteAfter(primaryRecord.Element);
        transaction.Add(primaryState);

        foreach (var relation in GetKeyedRelations(includeReverse: true))
        {
            var workspace = GetWorkspace(relation.TargetTable);
            if (workspace is null)
            {
                continue;
            }

            foreach (var dependentKey in FindDependingKeys(workspace, relation, recordKey))
            {
                RewriteDependent(transaction, workspace, relation, dependentKey, newKey);
            }
        }

        History.Record(transaction);
        return OperationResult.Success();
    }

    /// <summary>
    /// Exchanges the primary keys of two records and rewrites the dependent rows that store them, as one
    /// undoable transaction.
    /// </summary>
    /// <param name="firstKey">First primary key.</param>
    /// <param name="secondKey">Second primary key.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SwapRecords(string firstKey, string secondKey)
    {
        ArgumentNullException.ThrowIfNull(firstKey);
        ArgumentNullException.ThrowIfNull(secondKey);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (string.Equals(firstKey, secondKey, StringComparison.Ordinal))
        {
            return OperationResult.Success();
        }

        var keyField = Primary.Metadata?.RecordKey?.Primary.KeyField;
        if (string.IsNullOrEmpty(keyField))
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                "The table has no primary key field to swap.");
        }

        var first = Primary.BaseDocument.FindRecord(firstKey);
        var second = Primary.BaseDocument.FindRecord(secondKey);
        if (first is null || second is null)
        {
            return OperationResult.Failure(
                ErrorCodes.KeyNotFound,
                $"One of the records '{firstKey}' and '{secondKey}' was not found.");
        }

        var transaction = new TableEditTransaction();
        var firstState = Capture(Primary, firstKey);
        var secondState = Capture(Primary, secondKey);
        first.TrySetValue(keyField, secondKey);
        second.TrySetValue(keyField, firstKey);
        firstState.CompleteAfter(first.Element);
        secondState.CompleteAfter(second.Element);
        transaction.Add(firstState);
        transaction.Add(secondState);

        foreach (var relation in GetKeyedRelations(includeReverse: true))
        {
            var workspace = GetWorkspace(relation.TargetTable);
            if (workspace is null)
            {
                continue;
            }

            // Both owner sets are collected before any value changes, because a second lookup after the
            // first rewrite would also see the rows that already moved to the other key.
            var firstDependents = FindDependingKeys(workspace, relation, firstKey);
            var secondDependents = FindDependingKeys(workspace, relation, secondKey);

            foreach (var dependentKey in firstDependents)
            {
                RewriteDependent(transaction, workspace, relation, dependentKey, secondKey);
            }

            foreach (var dependentKey in secondDependents)
            {
                RewriteDependent(transaction, workspace, relation, dependentKey, firstKey);
            }
        }

        History.Record(transaction);
        return OperationResult.Success();
    }

    /// <summary>
    /// Creates the 1:1 sub-table row that a source value requires, for example the <c>Weapons</c> row of a
    /// new item. An existing row is reused and no duplicate is created.
    /// </summary>
    /// <param name="relation">Sub-table relation of the primary table.</param>
    /// <param name="sourceValue">Value of the relation source field.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the record key.
    /// On failure, <c>error_code</c> is <c>metadata.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult<string> EnsureSubTableRecord(TableRelation relation, string sourceValue)
    {
        ArgumentNullException.ThrowIfNull(relation);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceValue);

        if (relation.Kind != TableRelationKind.SubTable)
        {
            return OperationResult<string>.Failure(
                ErrorCodes.MetadataNotFound,
                $"Relation '{relation.TargetTable}' is not a sub-table relation.");
        }

        var workspace = GetWorkspace(relation.TargetTable);
        if (workspace is null)
        {
            return OperationResult<string>.Failure(
                ErrorCodes.MetadataNotFound,
                $"Table '{relation.TargetTable}' is not part of the loaded set.");
        }

        var existing = FindValue(workspace, relation.TargetValueField, sourceValue);
        if (existing is not null)
        {
            return OperationResult<string>.Success(existing);
        }

        var created = workspace.BaseDocument.AddRecord();
        if (!created.IsSuccess)
        {
            return OperationResult<string>.Failure(created);
        }

        created.Data!.TrySetValue(relation.TargetValueField, sourceValue, addIfMissing: true);
        var transaction = new TableEditTransaction();
        transaction.Add(ElementSnapshotState.CaptureInsertion(workspace.BaseDocument, created.Data.Element));
        History.Record(transaction);
        return OperationResult<string>.Success(sourceValue);
    }

    /// <summary>
    /// Saves every dirty document of the set. Files with no changes are not rewritten, and each file is
    /// replaced atomically.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the save.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is the first file error and <c>error_text</c> lists every failed file.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task<OperationResult> SaveAsync(CancellationToken cancellationToken = default)
    {
        var failures = new List<string>();
        string? firstCode = null;

        foreach (var workspace in _workspaces.Values)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!workspace.IsDirty)
            {
                continue;
            }

            OperationResult result;
            try
            {
                result = await workspace.SaveAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                result = OperationResult.Failure(ErrorCodes.IoError, exception.Message);
            }

            if (!result.IsSuccess)
            {
                firstCode ??= result.ErrorCode;
                failures.Add($"{workspace.Descriptor.TableName}: {result.ErrorCode}: {result.ErrorText}");
            }
        }

        return failures.Count == 0
            ? OperationResult.Success()
            : OperationResult.Failure(firstCode ?? ErrorCodes.IoError, string.Join("; ", failures));
    }

    private static LanguageDescriptor? ResolveLanguage(TableDescriptor descriptor, LanguageDescriptor? language) =>
        language is not null
        && descriptor.Overlays.Any(overlay =>
            string.Equals(overlay.Language.Code, language.Code, StringComparison.OrdinalIgnoreCase))
            ? language
            : null;

    private IEnumerable<TableRelation> GetKeyedRelations(bool includeReverse = false)
    {
        var keyField = Primary.Metadata?.RecordKey?.Primary.KeyField;
        if (string.IsNullOrEmpty(keyField))
        {
            return Array.Empty<TableRelation>();
        }

        return _members
            .Select(static member => member.Relation)
            .Where(relation =>
                (relation.Kind is TableRelationKind.SubTable or TableRelationKind.ChildRows
                    || (includeReverse && relation.Kind == TableRelationKind.Reverse))
                && string.Equals(relation.SourceField, keyField, StringComparison.Ordinal));
    }

    private IReadOnlyList<string> FindDependingKeys(
        TableWorkspace workspace,
        TableRelation relation,
        string value)
    {
        var keys = new List<string>();
        foreach (var record in workspace.BaseDocument.Records)
        {
            var stored = record.GetRawValue(relation.TargetValueField)?.Trim();
            if (!string.Equals(stored, value, StringComparison.Ordinal))
            {
                continue;
            }

            var key = record.GetKey();
            if (!string.IsNullOrEmpty(key))
            {
                keys.Add(key);
            }
        }

        return keys;
    }

    private static string? FindValue(TableWorkspace workspace, string field, string value)
    {
        foreach (var record in workspace.BaseDocument.Records)
        {
            if (string.Equals(record.GetRawValue(field)?.Trim(), value, StringComparison.Ordinal))
            {
                return record.GetKey();
            }
        }

        return null;
    }

    private static ElementSnapshotState Capture(TableWorkspace workspace, string key)
    {
        var record = workspace.BaseDocument.FindRecord(key)
            ?? throw new InvalidOperationException($"Record '{key}' was not found in '{workspace.Descriptor.TableName}'.");
        return ElementSnapshotState.Capture(workspace.BaseDocument, record.Element);
    }

    private static void RewriteDependent(
        TableEditTransaction transaction,
        TableWorkspace workspace,
        TableRelation relation,
        string dependentKey,
        string replacement)
    {
        var record = workspace.BaseDocument.FindRecord(dependentKey);
        if (record is null)
        {
            return;
        }

        var state = Capture(workspace, dependentKey);
        record.TrySetValue(relation.TargetValueField, replacement, addIfMissing: true);
        state.CompleteAfter(record.Element);
        transaction.Add(state);
    }

    private static void CaptureDeletion(
        TableEditTransaction transaction,
        TableWorkspace workspace,
        string key)
    {
        var state = Capture(workspace, key);
        if (!workspace.BaseDocument.DeleteRecord(key).IsSuccess)
        {
            return;
        }

        state.MarkRemoved();
        transaction.Add(state);
    }
}
