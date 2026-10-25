using System.Globalization;
using System.Xml.Linq;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Workspace;

/// <summary>
/// Bulk preview and application over a scalar column of the workspace.
/// </summary>
public sealed partial class TableWorkspace
{
    private const NumberStyles DecimalStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    /// <summary>
    /// Evaluates a bulk edit without mutating the table.
    /// </summary>
    /// <param name="request">Bulk request with the target field, operation, operand, and record keys.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the preview.
    /// On failure, <c>error_code</c> is <c>xml.not_saveable</c>, <c>bulk.no_records</c>,
    /// <c>bulk.field_not_found</c>, <c>bulk.field_not_editable</c>,
    /// <c>bulk.unsupported_operation</c>, or <c>bulk.invalid_operand</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    public OperationResult<BulkEditPreview> PreviewBulk(BulkEditRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!CanEdit)
        {
            return OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.XmlNotSaveable,
                "The table is not editable.");
        }

        if (request.RecordKeys is null || request.RecordKeys.Count == 0)
        {
            return OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.BulkNoRecords,
                "No records were selected for the bulk operation.");
        }

        if (string.IsNullOrWhiteSpace(request.FieldName))
        {
            return OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.BulkFieldNotFound,
                "The bulk field name is empty.");
        }

        var column = Schema.FindColumn(request.FieldName);
        if (column is null)
        {
            return OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.BulkFieldNotFound,
                $"Field '{request.FieldName}' is not part of table '{Descriptor.TableName}'.");
        }

        if (column.IsKey || column.IsList || column.IsNested)
        {
            return OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.BulkFieldNotEditable,
                $"Field '{column.XmlFieldName}' cannot be edited in bulk.");
        }

        if (request.Operation != BulkEditOperation.Set
            && column.DataType is not (ColumnDataType.Integer or ColumnDataType.Decimal))
        {
            return OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.BulkUnsupportedOperation,
                $"Operation '{request.Operation}' requires an Integer or Decimal field, but "
                + $"'{column.XmlFieldName}' is {column.DataType}.");
        }

        if (!TryValidateOperand(column.DataType, request.Operation, request.Operand, out var operand, out var error))
        {
            return OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.BulkInvalidOperand,
                error ?? $"'{request.Operand}' is not valid for field '{column.XmlFieldName}'.");
        }

        var rowsByKey = new Dictionary<string, WorkspaceRow>(StringComparer.Ordinal);
        foreach (var row in GetRows())
        {
            if (row.Key is not null)
            {
                rowsByKey.TryAdd(row.Key, row);
            }
        }

        var previewRows = new List<BulkEditPreviewRow>(request.RecordKeys.Count);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var key in request.RecordKeys)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                previewRows.Add(new BulkEditPreviewRow(
                    key ?? string.Empty,
                    null,
                    null,
                    BulkEditRowStatus.Error,
                    "The record key is empty."));
                continue;
            }

            if (!seen.Add(key))
            {
                continue;
            }

            if (!rowsByKey.TryGetValue(key, out var row))
            {
                previewRows.Add(new BulkEditPreviewRow(
                    key,
                    null,
                    null,
                    BulkEditRowStatus.Error,
                    $"Record '{key}' was not found."));
                continue;
            }

            previewRows.Add(EvaluateRow(row, column, request, operand));
        }

        return OperationResult<BulkEditPreview>.Success(new BulkEditPreview(
            column.XmlFieldName,
            request.Operation,
            request.Operand,
            request.IncludeMissingFields,
            previewRows));
    }

    /// <summary>
    /// Applies a bulk edit as one undoable transaction. The request is evaluated again before any
    /// mutation; rows with errors or an empty change set are refused.
    /// </summary>
    /// <param name="request">Bulk request with the target field, operation, operand, and record keys.</param>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/> and <c>data</c> carries the applied preview.
    /// On failure, <c>error_code</c> is the same as for <see cref="PreviewBulk"/>, or
    /// <c>bulk.preview_has_errors</c> and <c>bulk.no_changes</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    public OperationResult<BulkEditPreview> ApplyBulk(BulkEditRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var previewResult = PreviewBulk(request);
        if (!previewResult.IsSuccess)
        {
            return OperationResult<BulkEditPreview>.Failure(previewResult);
        }

        var preview = previewResult.Data!;
        if (preview.ErrorCount > 0)
        {
            return OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.BulkPreviewHasErrors,
                $"The bulk preview contains {preview.ErrorCount.ToString(CultureInfo.InvariantCulture)} error(s).");
        }

        if (preview.ChangedCount == 0)
        {
            return OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.BulkNoChanges,
                "The bulk operation would not change any value.");
        }

        var path = FieldPath.Parse(request.FieldName);
        var isOverlayEdit = Session is not null
            && Session.Overlay is not null
            && Session.IsTranslatable(path);
        var baseRecords = isOverlayEdit
            ? null
            : IndexBaseRecords();
        var overlayRecords = isOverlayEdit
            ? IndexOverlayTopLevel()
            : null;
        var transaction = new TableEditTransaction();
        foreach (var row in preview.Rows)
        {
            if (row.Status != BulkEditRowStatus.Changed)
            {
                continue;
            }

            var addIfMissing = row.OldValue is null && request.IncludeMissingFields;
            if (isOverlayEdit)
            {
                var before = overlayRecords is not null
                    && overlayRecords.TryGetValue(row.RecordKey, out var existingElement)
                    ? ElementSnapshotState.Capture(Overlay!.Document, existingElement)
                    : null;
                var result = new OverlayWriter().SetValue(
                    Overlay!,
                    OverlayRecordKey.ForRecord(row.RecordKey),
                    path,
                    row.NewValue!,
                    synchronizeRecords: false);
                if (!result.IsSuccess)
                {
                    transaction.Undo();
                    transaction.RefreshDocuments();
                    return OperationResult<BulkEditPreview>.Failure(result);
                }

                if (before is not null)
                {
                    before.CompleteAfter(before.Current!);
                    transaction.Add(before);
                }
                else
                {
                    var created = FindOverlayTopLevel(row.RecordKey);
                    if (created is not null)
                    {
                        transaction.Add(ElementSnapshotState.CaptureInsertion(Overlay!.Document, created));
                    }
                }

                continue;
            }

            if (baseRecords is null || !baseRecords.TryGetValue(row.RecordKey, out var record))
            {
                transaction.Undo();
                transaction.RefreshDocuments();
                return OperationResult<BulkEditPreview>.Failure(
                    ErrorCodes.KeyNotFound,
                    $"Record '{row.RecordKey}' was not found.");
            }

            var baseBefore = ElementSnapshotState.Capture(BaseDocument, record.Element);
            var baseResult = record.SetFieldValue(path, row.NewValue!, addIfMissing);
            if (!baseResult.IsSuccess)
            {
                transaction.Undo();
                transaction.RefreshDocuments();
                return OperationResult<BulkEditPreview>.Failure(baseResult);
            }

            baseBefore.CompleteAfter(record.Element);
            transaction.Add(baseBefore);
        }

        if (isOverlayEdit)
        {
            transaction.RefreshDocuments();
        }

        History.Record(transaction);
        return OperationResult<BulkEditPreview>.Success(preview);
    }

    private Dictionary<string, TableRecord> IndexBaseRecords()
    {
        var records = new Dictionary<string, TableRecord>(
            BaseDocument.Records.Count,
            StringComparer.Ordinal);
        foreach (var record in BaseDocument.Records)
        {
            var key = record.GetKey();
            if (key is not null)
            {
                records.TryAdd(key, record);
            }
        }

        return records;
    }

    private Dictionary<string, XElement> IndexOverlayTopLevel()
    {
        var elements = new Dictionary<string, XElement>(StringComparer.Ordinal);
        if (Overlay is null || Metadata is null)
        {
            return elements;
        }

        var root = Overlay.Document.Xml.Root;
        if (root is null)
        {
            return elements;
        }

        foreach (var record in new OverlayKeyResolver(Metadata).ResolvePrimary(Overlay.Document.Xml))
        {
            if (record.Key is null)
            {
                continue;
            }

            var element = record.Element;
            while (element.Parent is not null && element.Parent != root)
            {
                element = element.Parent;
            }

            if (element.Parent == root)
            {
                elements.TryAdd(record.Key, element);
            }
        }

        return elements;
    }

    private static BulkEditPreviewRow EvaluateRow(
        WorkspaceRow row,
        XmlEditorReloaded.Xml.Schema.TableSchemaColumn column,
        BulkEditRequest request,
        OperandValue operand)
    {
        var oldValue = row.GetValue(column.XmlFieldName);
        if (request.Operation == BulkEditOperation.Set)
        {
            if (oldValue is null && !request.IncludeMissingFields)
            {
                return new BulkEditPreviewRow(
                    row.Key!,
                    null,
                    null,
                    BulkEditRowStatus.SkippedMissing);
            }

            var changed = !string.Equals(oldValue, request.Operand, StringComparison.Ordinal);
            return new BulkEditPreviewRow(
                row.Key!,
                oldValue,
                request.Operand,
                changed ? BulkEditRowStatus.Changed : BulkEditRowStatus.Unchanged);
        }

        if (oldValue is null && !request.IncludeMissingFields)
        {
            return new BulkEditPreviewRow(
                row.Key!,
                null,
                null,
                BulkEditRowStatus.SkippedMissing);
        }

        if (!TryComputeArithmetic(column.DataType, request.Operation, oldValue, operand, out var newValue, out var error))
        {
            return new BulkEditPreviewRow(
                row.Key!,
                oldValue,
                null,
                BulkEditRowStatus.Error,
                error);
        }

        var status = string.Equals(oldValue, newValue, StringComparison.Ordinal)
            ? BulkEditRowStatus.Unchanged
            : BulkEditRowStatus.Changed;
        return new BulkEditPreviewRow(row.Key!, oldValue, newValue, status);
    }

    private static bool TryValidateOperand(
        ColumnDataType dataType,
        BulkEditOperation operation,
        string operand,
        out OperandValue parsed,
        out string? error)
    {
        parsed = default;
        error = null;
        if (operation == BulkEditOperation.Set)
        {
            if (operand.Length == 0)
            {
                return true;
            }

            var valid = dataType switch
            {
                ColumnDataType.Integer =>
                    int.TryParse(operand, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
                ColumnDataType.Decimal =>
                    decimal.TryParse(operand, DecimalStyles, CultureInfo.InvariantCulture, out _),
                ColumnDataType.Lookup =>
                    long.TryParse(operand, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
                ColumnDataType.Flags =>
                    uint.TryParse(operand, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
                ColumnDataType.Boolean => operand.Trim() is "0" or "1",
                _ => true,
            };

            if (!valid)
            {
                error = $"'{operand}' is not a valid {dataType} value.";
            }

            return valid;
        }

        if (operand.Length == 0)
        {
            error = "An arithmetic bulk operation requires a numeric operand.";
            return false;
        }

        if (dataType == ColumnDataType.Integer
            && long.TryParse(operand, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
        {
            parsed = new OperandValue(integer, 0m);
            return true;
        }

        if (dataType == ColumnDataType.Decimal
            && decimal.TryParse(operand, DecimalStyles, CultureInfo.InvariantCulture, out var decimalValue))
        {
            parsed = new OperandValue(0L, decimalValue);
            return true;
        }

        error = $"'{operand}' is not a valid numeric value for a {dataType} field.";
        return false;
    }

    private static bool TryComputeArithmetic(
        ColumnDataType dataType,
        BulkEditOperation operation,
        string? oldValue,
        OperandValue operand,
        out string? newValue,
        out string? error)
    {
        newValue = null;
        error = null;
        if (dataType == ColumnDataType.Integer)
        {
            var existing = 0L;
            if (!string.IsNullOrEmpty(oldValue)
                && (!long.TryParse(oldValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out existing)
                    || existing < int.MinValue
                    || existing > int.MaxValue))
            {
                error = $"'{oldValue}' is not a valid Integer value.";
                return false;
            }

            long computed;
            try
            {
                computed = operation == BulkEditOperation.Add
                    ? checked(existing + operand.Integer)
                    : checked(existing * operand.Integer);
            }
            catch (OverflowException)
            {
                error = "The computed value does not fit into a 32-bit integer.";
                return false;
            }

            if (computed < int.MinValue || computed > int.MaxValue)
            {
                error = "The computed value does not fit into a 32-bit integer.";
                return false;
            }

            newValue = ((int)computed).ToString(CultureInfo.InvariantCulture);
            return true;
        }

        if (dataType == ColumnDataType.Decimal)
        {
            var existing = 0m;
            if (!string.IsNullOrEmpty(oldValue)
                && !decimal.TryParse(oldValue, DecimalStyles, CultureInfo.InvariantCulture, out existing))
            {
                error = $"'{oldValue}' is not a valid Decimal value.";
                return false;
            }

            decimal computed;
            try
            {
                computed = operation == BulkEditOperation.Add
                    ? existing + operand.Decimal
                    : existing * operand.Decimal;
            }
            catch (OverflowException)
            {
                error = "The computed decimal value is out of range.";
                return false;
            }

            newValue = computed.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        error = $"Arithmetic is not supported for {dataType} values.";
        return false;
    }

    private readonly record struct OperandValue(long Integer, decimal Decimal);
}
