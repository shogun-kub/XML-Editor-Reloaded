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
            var message = loadResult.Diagnostics.FirstOrDefault()?.Message ?? "The table file is malformed.";
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
    /// Merged root cells, or an empty list when no overlay is selected. The cell paths use the
    /// slash-separated form, for example <c>COUNTRY/countryName</c>.
    /// </returns>
    public IReadOnlyList<MergedCell> GetRootCells()
    {
        if (Metadata is null || Overlay is null)
        {
            return Array.Empty<MergedCell>();
        }

        return new OverlayMerger(Metadata).Merge(BaseDocument, Overlay.Document).RootCells;
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
    /// Sets a root-level translated field, for example <c>COUNTRY/countryName</c>.
    /// </summary>
    /// <param name="path">Path from the document root.</param>
    /// <param name="rawText">Translation text to store exactly.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>.
    /// </returns>
    public OperationResult SetRootValue(FieldPath path, string rawText)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(rawText);

        if (Session?.Overlay is null)
        {
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "No language overlay is selected.");
        }

        var transaction = new TableEditTransaction();
        var firstSegment = path.Segments[0];
        var before = CaptureOverlayRootElement(firstSegment);
        var result = Session.SetRootValue(path, rawText);
        if (result.IsSuccess)
        {
            CompleteOverlayRootElement(transaction, before, firstSegment);
            History.Record(transaction);
        }

        return result;
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
