using System.Xml.Linq;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Overlays;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Saving;
using XmlEditorReloaded.Xml.Validation;

namespace XmlEditorReloaded.Xml.Workspace;

/// <summary>
/// A loaded table ready for editing: the base document, an optional language overlay, the discovered
/// schema, and the operations that route edits, row changes, saving, validation, and translation reports.
/// </summary>
public sealed partial class TableWorkspace
{
    private readonly TableFileSaver _saver;

    private TableWorkspace(
        TableDescriptor descriptor,
        TableDocument baseDocument,
        TableMetadata? metadata,
        OverlayDocument? overlay,
        TableFileSaver saver,
        TableEditHistory? history = null)
    {
        Descriptor = descriptor;
        BaseDocument = baseDocument;
        Metadata = metadata;
        Overlay = overlay;
        Schema = new TableSchemaBuilder().Build(baseDocument);
        Session = metadata is null ? null : new TableEditSession(baseDocument, metadata, overlay);
        _saver = saver;
        History = history ?? new TableEditHistory();
    }

    /// <summary>
    /// Gets the descriptor of the base table, including all discovered overlays.
    /// </summary>
    public TableDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the loaded base document.
    /// </summary>
    public TableDocument BaseDocument { get; }

    /// <summary>
    /// Gets the table metadata, or <see langword="null"/> for document-driven tables.
    /// </summary>
    public TableMetadata? Metadata { get; }

    /// <summary>
    /// Gets the loaded language overlay, or <see langword="null"/> when editing base values only.
    /// </summary>
    public OverlayDocument? Overlay { get; }

    /// <summary>
    /// Gets the language of the selected overlay, or <see langword="null"/>.
    /// </summary>
    public LanguageDescriptor? Language => Overlay?.Descriptor.Language;

    /// <summary>
    /// Gets the edit session that routes translatable fields to the overlay, or <see langword="null"/>
    /// for document-driven tables without metadata.
    /// </summary>
    public TableEditSession? Session { get; }

    /// <summary>
    /// Gets the discovered schema of the table.
    /// </summary>
    public TableSchema Schema { get; }

    /// <summary>
    /// Gets the undo and redo history of record edits applied through the workspace. Workspaces that
    /// belong to one <see cref="TableSetWorkspace"/> share the same history, so a multi-file edit is
    /// undone and redone as a single step.
    /// </summary>
    public TableEditHistory History { get; }

    /// <summary>
    /// Gets a value indicating whether a record edit can be undone.
    /// </summary>
    public bool CanUndo => History.CanUndo;

    /// <summary>
    /// Gets a value indicating whether an undone record edit can be redone.
    /// </summary>
    public bool CanRedo => History.CanRedo;

    /// <summary>
    /// Gets the load status of the base document.
    /// </summary>
    public TableDocumentStatus Status => BaseDocument.Status;

    /// <summary>
    /// Gets the load diagnostics of the base document.
    /// </summary>
    public IReadOnlyList<LoadDiagnostic> Diagnostics => BaseDocument.Diagnostics;

    /// <summary>
    /// Gets a value indicating whether the base and overlay documents have unsaved changes.
    /// </summary>
    public bool IsDirty => BaseDocument.IsDirty || Overlay?.IsDirty == true;

    /// <summary>
    /// Gets a value indicating whether the table accepts edits. Read-only and malformed documents do not.
    /// </summary>
    public bool CanEdit => BaseDocument.Status == TableDocumentStatus.Loaded;

    /// <summary>
    /// Loads a table and, when a language is supplied, its overlay.
    /// </summary>
    /// <param name="descriptor">Base table descriptor with discovered overlays.</param>
    /// <param name="metadata">Table metadata, or <see langword="null"/> for document-driven tables.</param>
    /// <param name="language">Language whose overlay should be loaded, or <see langword="null"/> for base-only editing.</param>
    /// <param name="loader">Optional table loader.</param>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <param name="history">
    /// Optional history shared with other workspaces. When omitted the workspace owns a private history.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the workspace.
    /// On failure, <c>error_code</c> is <c>io.file_not_found</c>, <c>xml.parse_error</c>,
    /// <c>metadata.not_found</c>, or <c>overlay.not_found</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public static async Task<OperationResult<TableWorkspace>> LoadAsync(
        TableDescriptor descriptor,
        TableMetadata? metadata,
        LanguageDescriptor? language = null,
        TableFileLoader? loader = null,
        CancellationToken cancellationToken = default,
        TableEditHistory? history = null)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var effectiveLoader = loader ?? new TableFileLoader();
        var load = await effectiveLoader
            .LoadAsync(descriptor, metadata, cancellationToken)
            .ConfigureAwait(false);
        if (!load.IsSuccess)
        {
            return OperationResult<TableWorkspace>.Failure(load);
        }

        var loadResult = load.Data!;
        if (loadResult.Status == TableDocumentStatus.Malformed)
        {
            var message = loadResult.Diagnostics.Count > 0
                ? loadResult.Diagnostics[0].Message
                : "The table file is malformed.";
            return OperationResult<TableWorkspace>.Failure(ErrorCodes.XmlParseError, message);
        }

        var baseDocument = loadResult.GetDocument<TableDocument>();
        if (baseDocument is null)
        {
            return OperationResult<TableWorkspace>.Failure(
                ErrorCodes.XmlParseError,
                "The table file did not produce a document.");
        }

        OverlayDocument? overlay = null;
        if (language is not null)
        {
            if (metadata is null)
            {
                return OperationResult<TableWorkspace>.Failure(
                    ErrorCodes.MetadataNotFound,
                    "A language overlay requires table metadata.");
            }

            var overlayDescriptor = descriptor.Overlays.FirstOrDefault(candidate =>
                string.Equals(candidate.Language.Code, language.Code, StringComparison.OrdinalIgnoreCase));
            if (overlayDescriptor is null)
            {
                return OperationResult<TableWorkspace>.Failure(
                    ErrorCodes.OverlayNotFound,
                    $"Table '{descriptor.TableName}' has no overlay for language '{language.Code}'.");
            }

            var overlayLoad = await OverlayDocument
                .LoadAsync(overlayDescriptor, metadata, effectiveLoader, cancellationToken)
                .ConfigureAwait(false);
            if (!overlayLoad.IsSuccess)
            {
                return OperationResult<TableWorkspace>.Failure(overlayLoad);
            }

            overlay = overlayLoad.Data;
        }

        return OperationResult<TableWorkspace>.Success(
            new TableWorkspace(descriptor, baseDocument, metadata, overlay, new TableFileSaver(), history));
    }

    /// <summary>
    /// Builds the rows the editor should display, in base-document order.
    /// </summary>
    /// <returns>
    /// One row per base record with merged translation cells, or one row per merged nested record when
    /// the base records are nested and therefore not exposed by <see cref="TableDocument.Records"/>.
    /// </returns>
    public IReadOnlyList<WorkspaceRow> GetRows()
    {
        if (Metadata is null)
        {
            return BaseDocument.Records
                .Select(static record => new WorkspaceRow(record.GetKey(), record, null))
                .ToArray();
        }

        if (Overlay is null && BaseDocument.Records.Count > 0)
        {
            // Without an overlay there is nothing to merge. Building merged records would group rows by
            // key and could let a compound row inherit the cells of another row that shares its first
            // component.
            return BaseDocument.Records
                .Select(static record => new WorkspaceRow(record.GetKey(), record, null))
                .ToArray();
        }

        var merged = new OverlayMerger(Metadata).Merge(BaseDocument, Overlay?.Document);
        if (BaseDocument.Records.Count == 0)
        {
            return merged.Records
                .Select(static record => new WorkspaceRow(record.Key, null, record))
                .ToArray();
        }

        var mergedByKey = merged.Records
            .Where(static record => record.Key is not null)
            .GroupBy(static record => record.Key!, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.Ordinal);

        return BaseDocument.Records
            .Select(record =>
            {
                var key = record.GetKey();
                var mergedRecord = key is not null && mergedByKey.TryGetValue(key, out var found)
                    ? found
                    : null;
                return new WorkspaceRow(key, record, mergedRecord);
            })
            .ToArray();
    }

    /// <summary>
    /// Builds the root-level singleton cells for tables such as <c>Map.Cities</c>, where a country
    /// block is translated outside the repeated city list.
    /// </summary>
    /// <returns>
    /// Merged root cells, or an empty list for a table without root singleton segments. The cell paths
    /// use the slash-separated form, for example <c>COUNTRY/countryName</c>.
    /// </returns>
    public IReadOnlyList<MergedCell> GetRootCells()
    {
        if (Metadata is null)
        {
            return Array.Empty<MergedCell>();
        }

        return new OverlayMerger(Metadata).Merge(BaseDocument, Overlay?.Document).RootCells;
    }

    /// <summary>
    /// Determines whether a field path can be translated through the selected language overlay.
    /// </summary>
    /// <param name="path">Path relative to the record or from the document root.</param>
    /// <returns><see langword="true"/> when the table declares the path as translatable.</returns>
    public bool IsTranslatable(FieldPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return Session?.IsTranslatable(path) == true;
    }

    /// <summary>
    /// Rebuilds one row from the current base and overlay state. Use it after an edit that creates
    /// an overlay record, because a previously captured row does not know about the new record.
    /// </summary>
    /// <param name="key">Primary record key.</param>
    /// <returns>The refreshed row, or <see langword="null"/> when the key is not present.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
    public WorkspaceRow? GetRow(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        return GetRows().FirstOrDefault(row =>
            string.Equals(row.Key, key, StringComparison.Ordinal));
    }

    /// <summary>
    /// Rebuilds the current state of a row from its record, without looking it up by a key that may have
    /// changed. The row record is authoritative; a row without a base record falls back to its key.
    /// </summary>
    /// <param name="row">Row previously returned by <see cref="GetRows"/>.</param>
    /// <returns>The refreshed row, or <see langword="null"/> when the record is no longer present.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="row"/> is null.</exception>
    public WorkspaceRow? GetRow(WorkspaceRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        if (row.Record is not { } record)
        {
            return row.Key is null ? null : GetRow(row.Key);
        }

        if (!BaseDocument.Records.Contains(record))
        {
            return null;
        }

        if (Overlay is null)
        {
            return new WorkspaceRow(record.GetKey(), record, null);
        }

        var key = record.GetKey();
        if (key is null)
        {
            return new WorkspaceRow(null, record, null);
        }

        var merged = new OverlayMerger(Metadata!).Merge(BaseDocument, Overlay.Document).Records
            .FirstOrDefault(candidate => string.Equals(candidate.Key, key, StringComparison.Ordinal));
        return new WorkspaceRow(key, record, merged);
    }

    /// <summary>
    /// Sets a field value on one exact row, even when another row shares its key. Edits of key fields
    /// change the identity of the row, so the caller must keep using the same row reference afterwards.
    /// </summary>
    /// <param name="row">Row to edit.</param>
    /// <param name="path">Field path relative to the record.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, a missing base field is appended.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>,
    /// <c>overlay.unresolvable_key</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public OperationResult SetValue(
        WorkspaceRow row,
        FieldPath path,
        string rawText,
        bool addIfMissing = false)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (row.Record is not { } record)
        {
            return OperationResult.Failure(
                ErrorCodes.KeyNotFound,
                "Only rows backed by a base record can be edited in the grid.");
        }

        var isOverlayEdit = Session?.Overlay is not null && Session.IsTranslatable(path);
        if (isOverlayEdit)
        {
            var key = record.GetKey();
            return key is null
                ? OperationResult.Failure(
                    ErrorCodes.OverlayUnresolvableKey,
                    "The record has no key that could address an overlay record.")
                : SetValue(key, path, rawText, addIfMissing);
        }

        var pairBefore = CaptureSymmetricPairValues(record);
        var before = ElementSnapshotState.Capture(BaseDocument, record.Element);
        var result = record.SetFieldValue(path, rawText, addIfMissing);
        if (result.IsSuccess)
        {
            var transaction = new TableEditTransaction();
            before.CompleteAfter(record.Element);
            transaction.Add(before);
            ReconcileSymmetricPair(transaction, record, pairBefore);
            History.Record(transaction);
        }

        return result;
    }

    /// <summary>
    /// Sets a field value on one exact row using a slash-separated field path.
    /// </summary>
    /// <param name="row">Row to edit.</param>
    /// <param name="fieldPath">Slash-separated field path relative to the record.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, a missing base field is appended.</param>
    /// <returns>The result of <see cref="SetValue(WorkspaceRow, FieldPath, string, bool)"/>.</returns>
    public OperationResult SetValue(
        WorkspaceRow row,
        string fieldPath,
        string rawText,
        bool addIfMissing = false) =>
        SetValue(row, FieldPath.Parse(fieldPath), rawText, addIfMissing);

    /// <summary>
    /// Deletes one exact row and, when present, its translation record in the overlay.
    /// </summary>
    /// <param name="row">Row to delete.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="row"/> is null.</exception>
    public OperationResult DeleteRow(WorkspaceRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (row.Record is not { } record)
        {
            return OperationResult.Failure(
                ErrorCodes.KeyNotFound,
                "Only rows backed by a base record can be deleted from the grid.");
        }

        var key = record.GetKey() ?? string.Empty;
        var overlayState = CaptureOverlayState(key);
        var baseState = ElementSnapshotState.Capture(BaseDocument, record.Element);
        var transaction = new TableEditTransaction();
        DeleteSymmetricCounterpart(transaction, record);
        var deleted = BaseDocument.DeleteRecord(record);
        if (!deleted.IsSuccess)
        {
            return deleted;
        }

        baseState.MarkRemoved();
        if (Overlay is not null && Metadata is not null)
        {
            var overlayRecord = new OverlayKeyResolver(Metadata).FindPrimary(Overlay.Xml, key);
            if (overlayRecord is not null)
            {
                overlayRecord.Remove();
                Overlay.MarkDirty();
                Overlay.Document.SynchronizeRecords();
                overlayState?.MarkRemoved();
            }
        }

        transaction.Add(baseState);
        transaction.Add(overlayState);
        History.Record(transaction);
        return OperationResult.Success();
    }

    /// <summary>
    /// Sets a field value, routing translatable fields to the overlay when one is selected.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="path">Field path relative to the record.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, a missing base field is appended to the record.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>,
    /// <c>overlay.unresolvable_key</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public OperationResult SetValue(
        string recordKey,
        FieldPath path,
        string rawText,
        bool addIfMissing = false) =>
        SetValue(recordKey, path, occurrenceIndex: 0, rawText, addIfMissing);

    /// <summary>
    /// Sets a field value at a specific occurrence of the first path segment, for example one
    /// <c>GEARKIT</c> of a mercenary record.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="path">Field path relative to the record.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment among same-named siblings.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, a missing base leaf is appended to the record.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>,
    /// <c>overlay.unresolvable_key</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public OperationResult SetValue(
        string recordKey,
        FieldPath path,
        int occurrenceIndex,
        string rawText,
        bool addIfMissing = false)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        var isOverlayEdit = Session is not null
            && Session.Overlay is not null
            && Session.IsTranslatable(path);
        if (isOverlayEdit && occurrenceIndex > 0)
        {
            return OperationResult.Failure(
                ErrorCodes.OverlayUnresolvableKey,
                $"Field '{path.Text}' is translatable; write occurrence {occurrenceIndex} through SetNestedValue with a nested record key.");
        }

        var transaction = new TableEditTransaction();
        var recordBefore = BaseDocument.FindRecord(recordKey);
        var pairBefore = recordBefore is null ? null : CaptureSymmetricPairValues(recordBefore);
        var before = isOverlayEdit
            ? CaptureOverlayState(recordKey)
            : CaptureBaseState(recordKey);

        var result = SetValueCore(recordKey, path, occurrenceIndex, rawText, addIfMissing);
        if (result.IsSuccess)
        {
            if (isOverlayEdit)
            {
                CompleteOverlayState(transaction, before, recordKey);
            }
            else if (recordBefore is not null)
            {
                // The record key itself may have changed, so the snapshot is completed through the
                // captured element instead of looking the record up again by its old key.
                if (before is null)
                {
                    transaction.Add(ElementSnapshotState.CaptureInsertion(BaseDocument, recordBefore.Element));
                }
                else
                {
                    before.CompleteAfter(recordBefore.Element);
                    transaction.Add(before);
                }

                ReconcileSymmetricPair(transaction, recordBefore, pairBefore);
            }
            else
            {
                CompleteBaseState(transaction, before, recordKey);
            }

            History.Record(transaction);
        }

        return result;
    }

    /// <summary>
    /// Reverses the most recent edit recorded by the workspace.
    /// </summary>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c> or <c>history.empty</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a recorded element is no longer attached to its document.
    /// </exception>
    public OperationResult Undo() => CanEdit
        ? History.Undo()
        : OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");

    /// <summary>
    /// Restores the most recently undone edit.
    /// </summary>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c> or <c>history.empty</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a recorded element is no longer attached to its document.
    /// </exception>
    public OperationResult Redo() => CanEdit
        ? History.Redo()
        : OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");

    /// <summary>
    /// Sets a field value using a slash-separated field path.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="fieldPath">Slash-separated field path relative to the record.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, a missing base field is appended to the record.</param>
    /// <returns>The result of <see cref="SetValue(string, FieldPath, string, bool)"/>.</returns>
    public OperationResult SetValue(
        string recordKey,
        string fieldPath,
        string rawText,
        bool addIfMissing = false) =>
        SetValue(recordKey, FieldPath.Parse(fieldPath), rawText, addIfMissing);

    /// <summary>
    /// Sets a field value at a specific occurrence using a slash-separated field path.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="fieldPath">Slash-separated field path relative to the record.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment among same-named siblings.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <param name="addIfMissing">When <see langword="true"/>, a missing base leaf is appended to the record.</param>
    /// <returns>The result of <see cref="SetValue(string, FieldPath, int, string, bool)"/>.</returns>
    public OperationResult SetValue(
        string recordKey,
        string fieldPath,
        int occurrenceIndex,
        string rawText,
        bool addIfMissing = false) =>
        SetValue(recordKey, FieldPath.Parse(fieldPath), occurrenceIndex, rawText, addIfMissing);

    /// <summary>
    /// Sets a nested translated field in the overlay, for example a gear-kit name.
    /// </summary>
    /// <param name="key">Primary and nested record keys.</param>
    /// <param name="path">Field path relative to the resolved nested record.</param>
    /// <param name="rawText">Translation text to store exactly.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c> or <c>overlay.unresolvable_key</c>.
    /// </returns>
    public OperationResult SetNestedValue(OverlayRecordKey key, FieldPath path, string rawText)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        if (Session?.Overlay is null)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "No language overlay is selected.");
        }

        var transaction = new TableEditTransaction();
        var before = key.PrimaryKey is null ? null : CaptureOverlayState(key.PrimaryKey);
        var result = Session.SetNestedValue(key, path, rawText);
        if (result.IsSuccess && key.PrimaryKey is not null)
        {
            CompleteOverlayState(transaction, before, key.PrimaryKey);
            History.Record(transaction);
        }

        return result;
    }

    /// <summary>
    /// Enumerates the nested records of one primary record for a nested key segment, for example the
    /// <c>ASSIGNMENT</c> entries of a facility type.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="segmentIndex">Key-segment index of the nested collection, where the primary is zero.</param>
    /// <returns>The nested records in document order; empty when the record or segment is absent.</returns>
    public IReadOnlyList<NestedWorkspaceRecord> GetNestedRecords(string recordKey, int segmentIndex)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentOutOfRangeException.ThrowIfNegative(segmentIndex);

        if (Metadata?.RecordKey is not { } spec
            || segmentIndex <= 0
            || segmentIndex >= spec.Segments.Count)
        {
            return Array.Empty<NestedWorkspaceRecord>();
        }

        var record = BaseDocument.FindRecord(recordKey);
        if (record is null)
        {
            return Array.Empty<NestedWorkspaceRecord>();
        }

        var resolved = new OverlayKeyResolver(Metadata).ResolveNested(record.Element, segmentIndex);
        return resolved
            .Select((nested, index) => new NestedWorkspaceRecord(
                segmentIndex,
                index,
                nested.Key,
                new TableRecord(BaseDocument, nested.Element)))
            .ToArray();
    }

    /// <summary>
    /// Gets the effective value of a nested record field, using the merged translation value when an
    /// overlay is selected and the key is known.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="nested">Nested record returned by <see cref="GetNestedRecords"/>.</param>
    /// <param name="path">Field path relative to the nested record.</param>
    /// <returns>The effective value, or <see langword="null"/> when the field is absent.</returns>
    public string? GetNestedValue(string recordKey, NestedWorkspaceRecord nested, FieldPath path)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentNullException.ThrowIfNull(nested);
        ArgumentNullException.ThrowIfNull(path);

        if (Overlay is not null
            && Metadata?.RecordKey is { } spec
            && nested.Key is { Length: > 0 }
            && nested.SegmentIndex > 0
            && nested.SegmentIndex < spec.Segments.Count)
        {
            var segment = spec.Segments[nested.SegmentIndex];
            var keyedPath = segment.Path + "[" + nested.Key + "]/" + path.Text;
            var row = GetRow(recordKey);
            var cell = row?.Merged?.Cells
                .FirstOrDefault(candidate => string.Equals(candidate.FieldPath, keyedPath, StringComparison.Ordinal));
            if (cell is not null)
            {
                return cell.Value;
            }
        }

        return nested.Record.GetRawValue(path);
    }

    /// <summary>
    /// Determines whether a nested record field comes from the selected language overlay.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="nested">Nested record returned by <see cref="GetNestedRecords"/>.</param>
    /// <param name="path">Field path relative to the nested record.</param>
    /// <returns><see langword="true"/> when the overlay contains the field.</returns>
    public bool IsNestedTranslated(string recordKey, NestedWorkspaceRecord nested, FieldPath path)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentNullException.ThrowIfNull(nested);
        ArgumentNullException.ThrowIfNull(path);

        if (Overlay is null
            || Metadata?.RecordKey is not { } spec
            || nested.Key is not { Length: > 0 }
            || nested.SegmentIndex <= 0
            || nested.SegmentIndex >= spec.Segments.Count)
        {
            return false;
        }

        var segment = spec.Segments[nested.SegmentIndex];
        var keyedPath = segment.Path + "[" + nested.Key + "]/" + path.Text;
        var row = GetRow(recordKey);
        var cell = row?.Merged?.Cells
            .FirstOrDefault(candidate => string.Equals(candidate.FieldPath, keyedPath, StringComparison.Ordinal));
        return cell?.OverlayValue is not null;
    }

    /// <summary>
    /// Appends a nested record for a key segment, for example one <c>ASSIGNMENT</c> of a facility type.
    /// The nested key must be unique within the parent record; the new element is inserted after the
    /// last same-named sibling and recorded as one undoable transaction. The overlay is not modified.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="segmentIndex">Key-segment index of the nested collection, where the primary is zero.</param>
    /// <param name="key">Nested key value, written into the segment key field.</param>
    /// <param name="initialValues">
    /// Optional additional leaf values of the new nested record in document order; a value for the key
    /// field is ignored because the key argument is authoritative.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>key.conflict</c>,
    /// <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recordKey"/> or <paramref name="key"/> is null.</exception>
    public OperationResult AddNestedRecord(
        string recordKey,
        int segmentIndex,
        string key,
        IReadOnlyList<KeyValuePair<string, string>>? initialValues = null)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentOutOfRangeException.ThrowIfNegative(segmentIndex);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (Metadata?.RecordKey is not { } spec
            || segmentIndex <= 0
            || segmentIndex >= spec.Segments.Count)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                "The requested key segment is not a nested key segment of this table.");
        }

        var segment = spec.Segments[segmentIndex];
        if (segment.Kind != RecordKeyKind.Scalar || string.IsNullOrEmpty(segment.KeyField))
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Nested segment '{segment.Path}' has no scalar key field that could accept a new key.");
        }

        var record = BaseDocument.FindRecord(recordKey);
        if (record is null)
        {
            return OperationResult.Failure(ErrorCodes.KeyNotFound, $"Record '{recordKey}' was not found.");
        }

        var resolver = new OverlayKeyResolver(Metadata);
        var existing = resolver.ResolveNested(record.Element, segmentIndex);
        if (existing.Any(candidate => string.Equals(candidate.Key, key, StringComparison.Ordinal)))
        {
            return OperationResult.Failure(
                ErrorCodes.KeyConflict,
                $"Nested record '{segment.Path}' with key '{key}' already exists.");
        }

        var element = new XElement(XName.Get(segment.Path.Split('/')[^1]));
        element.Add(new XElement(XName.Get(segment.KeyField), key));
        var nestedRecord = new TableRecord(BaseDocument, element);
        if (initialValues is not null)
        {
            foreach (var pair in initialValues)
            {
                if (string.IsNullOrEmpty(pair.Key)
                    || string.Equals(pair.Key, segment.KeyField, StringComparison.Ordinal))
                {
                    continue;
                }

                var applied = nestedRecord.TrySetValue(pair.Key, pair.Value, addIfMissing: true);
                if (!applied)
                {
                    return OperationResult.Failure(
                        ErrorCodes.XmlInvalidNumber,
                        $"'{pair.Value}' is not a valid value for field '{pair.Key}'.");
                }
            }
        }

        var before = ElementSnapshotState.Capture(BaseDocument, record.Element);
        var parent = record.Element;
        var parts = segment.Path.Split('/');
        for (var index = 0; index < parts.Length - 1; index++)
        {
            var child = parent.Elements()
                .FirstOrDefault(candidate => string.Equals(
                    candidate.Name.LocalName,
                    parts[index],
                    StringComparison.Ordinal));
            if (child is null)
            {
                child = new XElement(XName.Get(parts[index]));
                parent.Add(child);
            }

            parent = child;
        }

        var last = parent.Elements()
            .LastOrDefault(candidate => string.Equals(
                candidate.Name.LocalName,
                parts[^1],
                StringComparison.Ordinal));
        if (last is not null)
        {
            last.AddAfterSelf(element);
        }
        else
        {
            parent.Add(element);
        }

        BaseDocument.MarkDirty();
        var transaction = new TableEditTransaction();
        before.CompleteAfter(record.Element);
        transaction.Add(before);
        History.Record(transaction);
        return OperationResult.Success();
    }

    /// <summary>
    /// Deletes one nested record of a key segment and, when present, its matching translation record in
    /// the selected overlay. The base and overlay changes are recorded as one undoable transaction.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="segmentIndex">Key-segment index of the nested collection, where the primary is zero.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the nested record in document order.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recordKey"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an index is negative.</exception>
    public OperationResult DeleteNestedRecord(string recordKey, int segmentIndex, int occurrenceIndex)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentOutOfRangeException.ThrowIfNegative(segmentIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (Metadata?.RecordKey is not { } spec
            || segmentIndex <= 0
            || segmentIndex >= spec.Segments.Count)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                "The requested key segment is not a nested key segment of this table.");
        }

        var record = BaseDocument.FindRecord(recordKey);
        if (record is null)
        {
            return OperationResult.Failure(ErrorCodes.KeyNotFound, $"Record '{recordKey}' was not found.");
        }

        var resolver = new OverlayKeyResolver(Metadata);
        var nested = resolver.ResolveNested(record.Element, segmentIndex);
        if (occurrenceIndex >= nested.Count)
        {
            return OperationResult.Failure(
                ErrorCodes.KeyNotFound,
                $"Nested record #{occurrenceIndex + 1} of '{spec.Segments[segmentIndex].Path}' was not found.");
        }

        var target = nested[occurrenceIndex];
        var baseBefore = ElementSnapshotState.Capture(BaseDocument, record.Element);
        var overlayBefore = CaptureOverlayState(recordKey);
        target.Element.Remove();
        BaseDocument.MarkDirty();

        if (Overlay is not null && target.Key is not null)
        {
            var overlayRecord = resolver.FindPrimary(Overlay.Xml, recordKey);
            var overlayNested = overlayRecord is null
                ? null
                : resolver.ResolveNested(overlayRecord, segmentIndex)
                    .FirstOrDefault(candidate => string.Equals(
                        candidate.Key,
                        target.Key,
                        StringComparison.Ordinal));
            if (overlayNested is not null)
            {
                overlayNested.Element.Remove();
                Overlay.MarkDirty();
                Overlay.Document.SynchronizeRecords();
            }
        }

        var transaction = new TableEditTransaction();
        baseBefore.CompleteAfter(record.Element);
        transaction.Add(baseBefore);
        if (Overlay is not null)
        {
            CompleteOverlayState(transaction, overlayBefore, recordKey);
        }

        History.Record(transaction);
        return OperationResult.Success();
    }

    /// <summary>
    /// Appends one repeated nested block such as a facility <c>PRODUCTIONLINE</c> as one undoable
    /// transaction. Only the supplied leaf values are written; no fields are invented.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="fieldName">Case-sensitive XML element name of the repeated block.</param>
    /// <param name="initialValues">Optional leaf values of the new block in document order.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recordKey"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is empty.</exception>
    public OperationResult AddRepeatedBlock(
        string recordKey,
        string fieldName,
        IReadOnlyList<KeyValuePair<string, string>>? initialValues = null)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentException.ThrowIfNullOrEmpty(fieldName);

        return ApplyRecordEdit(
            recordKey,
            record => record.AddElement(fieldName, initialValues));
    }

    /// <summary>
    /// Deletes one repeated nested block such as a facility <c>PRODUCTIONLINE</c> as one undoable
    /// transaction.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="fieldName">Case-sensitive XML element name of the repeated block.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence among same-named blocks.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recordKey"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public OperationResult DeleteRepeatedBlock(string recordKey, string fieldName, int occurrenceIndex)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        return ApplyRecordEdit(
            recordKey,
            record => record.RemoveElement(fieldName, occurrenceIndex));
    }

    /// <summary>
    /// Gets the values of a repeated leaf field of a record, selecting an occurrence of the first path
    /// segment, for example <c>PRODUCTIONLINE/usOptional_ItemUsedUp</c> of one production line.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="itemPath">Path to the repeated leaf.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment.</param>
    /// <returns>The values in document order; empty when the record or path is absent.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recordKey"/> or <paramref name="itemPath"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public IReadOnlyList<string> GetRepeatedValues(
        string recordKey,
        FieldPath itemPath,
        int occurrenceIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentNullException.ThrowIfNull(itemPath);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        return BaseDocument.FindRecord(recordKey)?.GetRepeatedValues(itemPath, occurrenceIndex)
            ?? Array.Empty<string>();
    }

    /// <summary>
    /// Replaces all values of a repeated leaf field of a record as one undoable transaction.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="itemPath">Path to the repeated leaf.</param>
    /// <param name="values">New values in the desired order; an empty sequence removes the occurrences.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recordKey"/>, <paramref name="itemPath"/>, or <paramref name="values"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public OperationResult SetRepeatedValues(
        string recordKey,
        FieldPath itemPath,
        IReadOnlyList<string> values,
        int occurrenceIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentNullException.ThrowIfNull(itemPath);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        return ApplyRecordEdit(
            recordKey,
            record => record.SetRepeatedValues(itemPath, values, occurrenceIndex));
    }

    /// <summary>
    /// Gets the values of a repeated leaf field of a nested record, for example the
    /// <c>DRUNK/drugitems/drugitem</c> ids of an <c>ASSIGNMENT</c>.
    /// </summary>
    /// <param name="nested">Nested record returned by <see cref="GetNestedRecords"/>.</param>
    /// <param name="itemPath">Path to the repeated leaf relative to the nested record.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment.</param>
    /// <returns>The values in document order; empty when the path is absent.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="nested"/> or <paramref name="itemPath"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public IReadOnlyList<string> GetNestedRepeatedValues(
        NestedWorkspaceRecord nested,
        FieldPath itemPath,
        int occurrenceIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(nested);
        ArgumentNullException.ThrowIfNull(itemPath);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        return nested.Record.GetRepeatedValues(itemPath, occurrenceIndex);
    }

    /// <summary>
    /// Replaces all values of a repeated leaf field of a nested record as one undoable transaction of
    /// the parent record.
    /// </summary>
    /// <param name="recordKey">Primary record key that owns the nested record.</param>
    /// <param name="nested">Nested record returned by <see cref="GetNestedRecords"/>.</param>
    /// <param name="itemPath">Path to the repeated leaf relative to the nested record.</param>
    /// <param name="values">New values in the desired order; an empty sequence removes the occurrences.</param>
    /// <param name="occurrenceIndex">Zero-based occurrence of the first path segment.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="occurrenceIndex"/> is negative.</exception>
    public OperationResult SetNestedRepeatedValues(
        string recordKey,
        NestedWorkspaceRecord nested,
        FieldPath itemPath,
        IReadOnlyList<string> values,
        int occurrenceIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentNullException.ThrowIfNull(nested);
        ArgumentNullException.ThrowIfNull(itemPath);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfNegative(occurrenceIndex);

        return ApplyRecordEdit(
            recordKey,
            _ => nested.Record.SetRepeatedValues(itemPath, values, occurrenceIndex));
    }

    private OperationResult ApplyRecordEdit(
        string recordKey,
        Func<TableRecord, OperationResult> edit)
    {
        ArgumentNullException.ThrowIfNull(edit);
        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        var record = BaseDocument.FindRecord(recordKey);
        if (record is null)
        {
            return OperationResult.Failure(ErrorCodes.KeyNotFound, $"Record '{recordKey}' was not found.");
        }

        var before = ElementSnapshotState.Capture(BaseDocument, record.Element);
        var result = edit(record);
        if (!result.IsSuccess)
        {
            return result;
        }

        var transaction = new TableEditTransaction();
        before.CompleteAfter(record.Element);
        transaction.Add(before);
        History.Record(transaction);
        return result;
    }

    /// <summary>
    /// Sets a root-level field, for example <c>COUNTRY/countryName</c>. A translatable path is written
    /// only to the selected language overlay; every other path is written to the base table.
    /// </summary>
    /// <param name="path">Path from the document root.</param>
    /// <param name="rawText">Value to store exactly.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetRootValue(FieldPath path, string rawText)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        var firstSegment = path.Segments[0];
        var isOverlayEdit = Session?.Overlay is not null && Session.IsTranslatable(path);
        var transaction = new TableEditTransaction();
        if (isOverlayEdit)
        {
            var before = CaptureOverlayRootElement(firstSegment);
            var result = Session!.SetRootValue(path, rawText);
            if (result.IsSuccess)
            {
                CompleteOverlayRootElement(transaction, before, firstSegment);
                History.Record(transaction);
            }

            return result;
        }

        var root = BaseDocument.Xml.Root;
        if (root is null)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The document has no root element.");
        }

        if (path.Depth < 2)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "A root value path needs a field.");
        }

        var element = root.Elements()
            .FirstOrDefault(child => string.Equals(child.Name.LocalName, firstSegment, StringComparison.Ordinal));
        var baseBefore = element is null ? null : ElementSnapshotState.Capture(BaseDocument, element);
        var created = element is null;
        if (element is null)
        {
            element = new XElement(XName.Get(firstSegment));
            root.Add(element);
        }

        var record = new TableRecord(BaseDocument, element);
        var relative = new FieldPath(path.Segments.Skip(1).ToArray());
        var baseResult = record.SetFieldValue(relative, rawText, addIfMissing: true);
        if (!baseResult.IsSuccess)
        {
            if (created)
            {
                element.Remove();
            }

            return baseResult;
        }

        if (baseBefore is null)
        {
            transaction.Add(ElementSnapshotState.CaptureInsertion(BaseDocument, element));
        }
        else
        {
            baseBefore.CompleteAfter(element);
            transaction.Add(baseBefore);
        }

        History.Record(transaction);
        return baseResult;
    }

    /// <summary>
    /// Appends a record with the next primary key.
    /// </summary>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the new record.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult<TableRecord> AddRecord() => AddRecord(null);

    /// <summary>
    /// Appends a record with the next primary key and the supplied initial field values. The record and
    /// its values are recorded as one undoable transaction.
    /// </summary>
    /// <param name="initialValues">
    /// Direct child values to set on the new record, keyed by case-sensitive XML element name. Values are
    /// written without validation so a caller can prefill a key field with a non-numeric identity.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the new record.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult<TableRecord> AddRecord(IReadOnlyDictionary<string, string>? initialValues)
    {
        if (!CanEdit)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlNotSaveable,
                "The table is not editable.");
        }

        var result = BaseDocument.AddRecord();
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = result.Data!;
        if (initialValues is not null)
        {
            foreach (var pair in initialValues)
            {
                if (string.IsNullOrEmpty(pair.Key))
                {
                    continue;
                }

                record.TrySetValue(pair.Key, pair.Value, addIfMissing: true);
            }
        }

        var transaction = new TableEditTransaction();
        transaction.Add(ElementSnapshotState.CaptureInsertion(BaseDocument, record.Element));
        ReconcileSymmetricPair(transaction, record, before: null);
        History.Record(transaction);
        return result;
    }

    /// <summary>
    /// Replaces all values of a repeated direct child field of one record as one undoable transaction.
    /// </summary>
    /// <param name="recordKey">Primary record key.</param>
    /// <param name="fieldName">Case-sensitive XML element name of the repeated field.</param>
    /// <param name="values">New values in the desired order; an empty sequence removes the occurrences.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>, or
    /// <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recordKey"/> or <paramref name="values"/> is null.</exception>
    public OperationResult SetListValues(string recordKey, string fieldName, IReadOnlyList<string> values)
    {
        ArgumentNullException.ThrowIfNull(recordKey);
        ArgumentException.ThrowIfNullOrEmpty(fieldName);
        ArgumentNullException.ThrowIfNull(values);

        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        var record = BaseDocument.FindRecord(recordKey);
        if (record is null)
        {
            return OperationResult.Failure(ErrorCodes.KeyNotFound, $"Record '{recordKey}' was not found.");
        }

        var before = ElementSnapshotState.Capture(BaseDocument, record.Element);
        var result = record.SetListField(fieldName, values);
        if (result.IsSuccess)
        {
            var transaction = new TableEditTransaction();
            before.CompleteAfter(record.Element);
            transaction.Add(before);
            History.Record(transaction);
        }

        return result;
    }

    /// <summary>
    /// Finds a loaded base record by its primary key.
    /// </summary>
    /// <param name="key">Primary key text.</param>
    /// <returns>The record, or <see langword="null"/> when no record has that key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
    public TableRecord? FindRecord(string key) => BaseDocument.FindRecord(key);

    /// <summary>
    /// Duplicates a record directly after its source and assigns the next primary key.
    /// </summary>
    /// <param name="key">Primary key of the record to duplicate.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the copy.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult<TableRecord> DuplicateRecord(string key)
    {
        if (!CanEdit)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlNotSaveable,
                "The table is not editable.");
        }

        var result = BaseDocument.DuplicateRecord(key);
        if (result.IsSuccess)
        {
            var transaction = new TableEditTransaction();
            transaction.Add(ElementSnapshotState.CaptureInsertion(BaseDocument, result.Data!.Element));
            History.Record(transaction);
        }

        return result;
    }

    /// <summary>
    /// Deletes a record and, when present, its translation record in the overlay.
    /// </summary>
    /// <param name="key">Primary key of the record to delete.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c> or <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult DeleteRecord(string key)
    {
        if (!CanEdit)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        var baseState = CaptureBaseState(key);
        var overlayState = CaptureOverlayState(key);
        var transaction = new TableEditTransaction();
        var recordBefore = BaseDocument.FindRecord(key);
        if (recordBefore is not null)
        {
            DeleteSymmetricCounterpart(transaction, recordBefore);
        }

        var deleted = BaseDocument.DeleteRecord(key);
        if (!deleted.IsSuccess)
        {
            return deleted;
        }

        baseState?.MarkRemoved();
        if (Overlay is not null && Metadata is not null)
        {
            var overlayRecord = new OverlayKeyResolver(Metadata).FindPrimary(Overlay.Xml, key);
            if (overlayRecord is not null)
            {
                overlayRecord.Remove();
                Overlay.MarkDirty();
                Overlay.Document.SynchronizeRecords();
                overlayState?.MarkRemoved();
            }
        }

        transaction.Add(baseState);
        transaction.Add(overlayState);
        History.Record(transaction);
        return OperationResult.Success();
    }

    /// <summary>
    /// Saves the base document and the overlay independently. Clean documents are not rewritten.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the save.</param>
    /// <param name="forceOverwrite">
    /// When <see langword="true"/>, external file changes do not block the save; the written bytes become
    /// the new baseline. The caller must obtain the user's confirmation first.
    /// </param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, the error from the base or overlay saver is returned.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled; the current file is left unchanged.</exception>
    public Task<OperationResult> SaveAsync(
        CancellationToken cancellationToken = default,
        bool forceOverwrite = false) =>
        Session is not null
            ? Session.SaveAsync(cancellationToken, forceOverwrite)
            : _saver.SaveAsync(BaseDocument, cancellationToken, forceOverwrite);

    /// <summary>
    /// Validates the base document without mutating it.
    /// </summary>
    /// <param name="lookupRepository">Optional lookup repository used to validate lookup references.</param>
    /// <param name="languageCode">Optional language code for lookup overlays.</param>
    /// <param name="cancellationToken">Token to cancel validation.</param>
    /// <param name="relationTargetValues">
    /// Optional map from a relation source field to the keys of its resolved target table.
    /// </param>
    /// <returns>The validation report.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public Task<ValidationReport> ValidateAsync(
        ILookupRepository? lookupRepository = null,
        string? languageCode = null,
        CancellationToken cancellationToken = default,
        IReadOnlyDictionary<string, IReadOnlySet<string>>? relationTargetValues = null) =>
        new TableValidator().ValidateAsync(
            BaseDocument,
            lookupRepository,
            languageCode,
            cancellationToken,
            relationTargetValues);

    /// <summary>
    /// Analyzes missing, orphaned, duplicate, and empty translations.
    /// </summary>
    /// <returns>
    /// The translation report, or <see langword="null"/> when no overlay is selected or the table has no metadata.
    /// </returns>
    public TranslationReport? AnalyzeTranslations() =>
        Metadata is not null && Overlay is not null
            ? new TranslationAnalyzer().Analyze(BaseDocument, Overlay.Document)
            : null;

    private OperationResult SetValueCore(
        string recordKey,
        FieldPath path,
        int occurrenceIndex,
        string rawText,
        bool addIfMissing)
    {
        if (Session is not null)
        {
            return Session.SetValue(recordKey, path, occurrenceIndex, rawText, addIfMissing);
        }

        var record = BaseDocument.FindRecord(recordKey);
        return record is null
            ? OperationResult.Failure(ErrorCodes.KeyNotFound, $"Record '{recordKey}' was not found.")
            : record.SetFieldValue(path, occurrenceIndex, rawText, addIfMissing);
    }

    private ElementSnapshotState? CaptureBaseState(string recordKey)
    {
        var record = BaseDocument.FindRecord(recordKey);
        return record is null ? null : ElementSnapshotState.Capture(BaseDocument, record.Element);
    }

    private ElementSnapshotState? CaptureOverlayState(string recordKey)
    {
        var element = FindOverlayTopLevel(recordKey);
        return element is null || Overlay is null
            ? null
            : ElementSnapshotState.Capture(Overlay.Document, element);
    }

    private void CompleteBaseState(
        TableEditTransaction transaction,
        ElementSnapshotState? before,
        string recordKey)
    {
        var record = BaseDocument.FindRecord(recordKey);
        if (record is null)
        {
            return;
        }

        if (before is null)
        {
            transaction.Add(ElementSnapshotState.CaptureInsertion(BaseDocument, record.Element));
        }
        else
        {
            before.CompleteAfter(record.Element);
            transaction.Add(before);
        }
    }

    private void CompleteOverlayState(
        TableEditTransaction transaction,
        ElementSnapshotState? before,
        string recordKey)
    {
        var element = FindOverlayTopLevel(recordKey);
        if (Overlay is null)
        {
            return;
        }

        if (element is null)
        {
            if (before is not null)
            {
                before.MarkRemoved();
                transaction.Add(before);
            }

            return;
        }

        if (before is null)
        {
            transaction.Add(ElementSnapshotState.CaptureInsertion(Overlay.Document, element));
        }
        else
        {
            before.CompleteAfter(element);
            transaction.Add(before);
        }
    }

    private XElement? FindOverlayTopLevel(string recordKey)
    {
        if (Overlay is null || Metadata is null)
        {
            return null;
        }

        var root = Overlay.Document.Xml.Root;
        if (root is null)
        {
            return null;
        }

        var element = new OverlayKeyResolver(Metadata).FindPrimary(Overlay.Document.Xml, recordKey);
        if (element is null)
        {
            return null;
        }

        while (element.Parent is not null && element.Parent != root)
        {
            element = element.Parent;
        }

        return element.Parent == root ? element : null;
    }

    private ElementSnapshotState? CaptureOverlayRootElement(string firstSegment)
    {
        var element = FindOverlayRootElement(firstSegment);
        return element is null || Overlay is null
            ? null
            : ElementSnapshotState.Capture(Overlay.Document, element);
    }

    private void CompleteOverlayRootElement(
        TableEditTransaction transaction,
        ElementSnapshotState? before,
        string firstSegment)
    {
        if (Overlay is null)
        {
            return;
        }

        var element = FindOverlayRootElement(firstSegment);
        if (element is null)
        {
            return;
        }

        if (before is null)
        {
            transaction.Add(ElementSnapshotState.CaptureInsertion(Overlay.Document, element));
        }
        else
        {
            before.CompleteAfter(element);
            transaction.Add(before);
        }
    }

    private XElement? FindOverlayRootElement(string firstSegment)
    {
        var root = Overlay?.Document.Xml.Root;
        return root?.Elements()
            .FirstOrDefault(element =>
                string.Equals(element.Name.LocalName, firstSegment, StringComparison.Ordinal));
    }

    /// <summary>
    /// Creates the reversed counterpart of a complete pair row when it is missing. The caller owns the
    /// transaction, so the row change and the counterpart stay one undo step.
    /// </summary>
    internal void EnsureSymmetricCounterpart(TableEditTransaction transaction, TableRecord record)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(record);
        ReconcileSymmetricPair(transaction, record, before: null);
    }

    private SymmetricPairValues? CaptureSymmetricPairValues(TableRecord record)
    {
        if (Metadata?.SymmetricKeyPair is not { } pair)
        {
            return null;
        }

        return new SymmetricPairValues(
            record.GetRawValue(pair.FirstField)?.Trim(),
            record.GetRawValue(pair.SecondField)?.Trim());
    }

    private TableRecord? FindSymmetricCounterpart(SymmetricPairValues values)
    {
        if (Metadata?.SymmetricKeyPair is null || !values.IsComplete || values.IsSelfPair)
        {
            return null;
        }

        var key = RecordKeyCodec.Compose(new[] { values.Second, values.First });
        return BaseDocument.FindRecord(key);
    }

    private void ReconcileSymmetricPair(
        TableEditTransaction transaction,
        TableRecord record,
        SymmetricPairValues? before)
    {
        if (Metadata?.SymmetricKeyPair is not { } pair
            || CaptureSymmetricPairValues(record) is not { } after)
        {
            return;
        }

        if (before is { IsComplete: true } oldValues && !oldValues.Equals(after))
        {
            var oldCounterpart = FindSymmetricCounterpart(oldValues);
            var newCounterpart = FindSymmetricCounterpart(after);
            if (oldCounterpart is not null
                && !ReferenceEquals(oldCounterpart.Element, record.Element)
                && !ReferenceEquals(oldCounterpart.Element, newCounterpart?.Element))
            {
                var state = ElementSnapshotState.Capture(BaseDocument, oldCounterpart.Element);
                if (BaseDocument.DeleteRecord(oldCounterpart).IsSuccess)
                {
                    state.MarkRemoved();
                    transaction.Add(state);
                }
            }
        }

        if (!after.IsComplete || after.IsSelfPair || FindSymmetricCounterpart(after) is not null)
        {
            return;
        }

        var created = BaseDocument.AddRecord();
        if (!created.IsSuccess || created.Data is null)
        {
            return;
        }

        created.Data.TrySetValue(pair.FirstField, after.Second!, addIfMissing: true);
        created.Data.TrySetValue(pair.SecondField, after.First!, addIfMissing: true);
        transaction.Add(ElementSnapshotState.CaptureInsertion(BaseDocument, created.Data.Element));
    }

    private void DeleteSymmetricCounterpart(TableEditTransaction transaction, TableRecord record)
    {
        if (CaptureSymmetricPairValues(record) is not { } values)
        {
            return;
        }

        var counterpart = FindSymmetricCounterpart(values);
        if (counterpart is null || ReferenceEquals(counterpart.Element, record.Element))
        {
            return;
        }

        var state = ElementSnapshotState.Capture(BaseDocument, counterpart.Element);
        if (BaseDocument.DeleteRecord(counterpart).IsSuccess)
        {
            state.MarkRemoved();
            transaction.Add(state);
        }
    }

    private readonly record struct SymmetricPairValues(string? First, string? Second)
    {
        public bool IsComplete => !string.IsNullOrEmpty(First) && !string.IsNullOrEmpty(Second);

        public bool IsSelfPair => string.Equals(First, Second, StringComparison.Ordinal);
    }
}
