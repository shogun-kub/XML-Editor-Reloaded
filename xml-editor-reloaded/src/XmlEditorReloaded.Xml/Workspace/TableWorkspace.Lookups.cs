using System.Globalization;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Lookups;

namespace XmlEditorReloaded.Xml.Workspace;

/// <summary>
/// Lookup-specific editing: allocated identifiers, free-form identifier changes, and the identifier
/// list used by the lookup editor. Lookup ids are unsigned 64-bit values, so they do not go through the
/// 32-bit column validation that protects ordinary integer fields.
/// </summary>
public sealed partial class TableWorkspace
{
    /// <summary>
    /// Gets the direct child field that stores the identifier of a lookup table, or <see langword="null"/>
    /// when the table is not a lookup or has no single scalar key.
    /// </summary>
    public string? LookupKeyField
    {
        get
        {
            if (Metadata is not { IsLookup: true } metadata)
            {
                return null;
            }

            if (metadata.RecordKey?.Primary is { Kind: RecordKeyKind.Scalar, KeyField: { Length: > 0 } primary })
            {
                return primary;
            }

            return metadata.RecordKeyFields is { Count: 1 } fields ? fields[0] : null;
        }
    }

    /// <summary>
    /// Gets the identifier texts of all loaded lookup records in document order. Invalid or empty texts
    /// are omitted.
    /// </summary>
    /// <returns>The identifiers, or an empty list when the table is not a lookup.</returns>
    public IReadOnlyList<string> GetLookupIds()
    {
        if (LookupKeyField is not { } keyField)
        {
            return Array.Empty<string>();
        }

        var ids = new List<string>(BaseDocument.Records.Count);
        foreach (var record in BaseDocument.Records)
        {
            var value = record.GetRawValue(keyField);
            if (!string.IsNullOrEmpty(value))
            {
                ids.Add(value);
            }
        }

        return ids;
    }

    /// <summary>
    /// Suggests the identifier for a new lookup value: the lowest unused bit for a flag lookup and the
    /// maximum id plus one for a sequential lookup.
    /// </summary>
    /// <returns>The suggested identifier as invariant decimal text, or <see langword="null"/> when the
    /// table is not a lookup or the identifier range is exhausted.</returns>
    public string? SuggestLookupId()
    {
        if (LookupKeyField is null)
        {
            return null;
        }

        var suggested = LookupIdAllocator.SuggestNextId(GetLookupIds(), Metadata?.IsFlagLookup == true);
        return suggested?.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Appends a lookup value with an exact identifier and an empty name. The identifier is written
    /// verbatim as unsigned 64-bit decimal text and must not collide numerically with an existing value.
    /// </summary>
    /// <param name="lookupId">Identifier text, for example <c>32</c> or <c>8589934593</c>.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the new record.
    /// On failure, <c>error_code</c> is <c>xml.invalid_number</c>, <c>key.conflict</c>, or
    /// <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="lookupId"/> is null.</exception>
    public OperationResult<TableRecord> AddLookupRecord(string lookupId)
    {
        ArgumentNullException.ThrowIfNull(lookupId);

        if (!CanEdit)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlNotSaveable,
                "The table is not editable.");
        }

        if (LookupKeyField is not { } keyField)
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Table '{Metadata?.TableName.Value}' is not a lookup with a single scalar key.");
        }

        if (!LookupIdAllocator.TryParseId(lookupId, out var id))
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.XmlInvalidNumber,
                $"'{lookupId}' is not a valid unsigned 64-bit lookup identifier.");
        }

        var canonical = id.ToString(CultureInfo.InvariantCulture);
        if (HasLookupId(keyField, id, except: null))
        {
            return OperationResult<TableRecord>.Failure(
                ErrorCodes.KeyConflict,
                $"Lookup identifier '{canonical}' is already used.");
        }

        var result = BaseDocument.AddRecord(canonical);
        if (!result.IsSuccess)
        {
            return result;
        }

        var record = result.Data!;
        var transaction = new TableEditTransaction();
        transaction.Add(ElementSnapshotState.CaptureInsertion(BaseDocument, record.Element));
        History.Record(transaction);
        return result;
    }

    /// <summary>
    /// Changes the identifier of one lookup record. The new text is canonicalized to invariant decimal
    /// form and must not collide numerically with another record.
    /// </summary>
    /// <param name="row">Row backed by the lookup record.</param>
    /// <param name="newId">New identifier text, for example <c>64</c>.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>key.not_found</c>, <c>xml.invalid_number</c>,
    /// <c>key.conflict</c>, or <c>xml.not_saveable</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="row"/> or <paramref name="newId"/> is null.</exception>
    public OperationResult ChangeLookupId(WorkspaceRow row, string newId)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(newId);

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

        if (LookupKeyField is not { } keyField)
        {
            return OperationResult.Failure(
                ErrorCodes.XmlNotSaveable,
                $"Table '{Metadata?.TableName.Value}' is not a lookup with a single scalar key.");
        }

        if (!LookupIdAllocator.TryParseId(newId, out var id))
        {
            return OperationResult.Failure(
                ErrorCodes.XmlInvalidNumber,
                $"'{newId}' is not a valid unsigned 64-bit lookup identifier.");
        }

        var canonical = id.ToString(CultureInfo.InvariantCulture);
        if (HasLookupId(keyField, id, record))
        {
            return OperationResult.Failure(
                ErrorCodes.KeyConflict,
                $"Lookup identifier '{canonical}' is already used.");
        }

        var before = ElementSnapshotState.Capture(BaseDocument, record.Element);
        var result = record.SetFieldValueUnchecked(keyField, canonical, addIfMissing: true);
        if (result.IsSuccess)
        {
            var transaction = new TableEditTransaction();
            before.CompleteAfter(record.Element);
            transaction.Add(before);
            History.Record(transaction);
        }

        return result;
    }

    private bool HasLookupId(string keyField, ulong id, TableRecord? except)
    {
        foreach (var record in BaseDocument.Records)
        {
            if (ReferenceEquals(record, except))
            {
                continue;
            }

            if (LookupIdAllocator.TryParseId(record.GetRawValue(keyField), out var existing) && existing == id)
            {
                return true;
            }
        }

        return false;
    }
}
