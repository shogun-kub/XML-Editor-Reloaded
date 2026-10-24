namespace XmlEditorReloaded.Xml.Workspace;

/// <summary>
/// The arithmetic or assignment applied by a bulk edit.
/// </summary>
public enum BulkEditOperation
{
    /// <summary>Replace every target value with the operand.</summary>
    Set,

    /// <summary>Add the operand to every target numeric value.</summary>
    Add,

    /// <summary>Multiply every target numeric value by the operand.</summary>
    Multiply,
}

/// <summary>
/// Describes one previewed or applied value change.
/// </summary>
public enum BulkEditRowStatus
{
    /// <summary>The value will change.</summary>
    Changed,

    /// <summary>The computed value is identical to the current value.</summary>
    Unchanged,

    /// <summary>The field is absent and missing fields were not included.</summary>
    SkippedMissing,

    /// <summary>The row cannot be changed, for example because it is missing or contains invalid data.</summary>
    Error,
}

/// <summary>
/// A bulk edit request for one scalar column.
/// </summary>
/// <param name="FieldName">Exact XML field name of the target column.</param>
/// <param name="Operation">Operation to apply.</param>
/// <param name="Operand">Raw operand text, parsed with invariant culture.</param>
/// <param name="IncludeMissingFields">
/// When <see langword="true"/>, absent fields are created. For arithmetic operations an absent
/// value is treated as zero.
/// </param>
/// <param name="RecordKeys">Primary keys of the records to process, in display order.</param>
public sealed record BulkEditRequest(
    string FieldName,
    BulkEditOperation Operation,
    string Operand,
    bool IncludeMissingFields,
    IReadOnlyList<string> RecordKeys);

/// <summary>
/// One row of a <see cref="BulkEditPreview"/>.
/// </summary>
/// <param name="RecordKey">Primary record key.</param>
/// <param name="OldValue">Current effective value, or <see langword="null"/> when the field is absent.</param>
/// <param name="NewValue">Computed value, or <see langword="null"/> when the row is skipped or invalid.</param>
/// <param name="Status">Outcome of the preview for this row.</param>
/// <param name="ErrorText">Human-readable error detail, or <see langword="null"/> when there is no error.</param>
public sealed record BulkEditPreviewRow(
    string RecordKey,
    string? OldValue,
    string? NewValue,
    BulkEditRowStatus Status,
    string? ErrorText = null);

/// <summary>
/// The deterministic result of evaluating a <see cref="BulkEditRequest"/> without mutating the table.
/// </summary>
public sealed class BulkEditPreview
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkEditPreview"/> class and computes the summary counts.
    /// </summary>
    /// <param name="fieldName">Exact XML field name of the target column.</param>
    /// <param name="operation">Operation that was evaluated.</param>
    /// <param name="operand">Raw operand text.</param>
    /// <param name="includeMissingFields">Whether absent fields are included.</param>
    /// <param name="rows">Preview rows in request order.</param>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    public BulkEditPreview(
        string fieldName,
        BulkEditOperation operation,
        string operand,
        bool includeMissingFields,
        IReadOnlyList<BulkEditPreviewRow> rows)
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(operand);
        ArgumentNullException.ThrowIfNull(rows);

        FieldName = fieldName;
        Operation = operation;
        Operand = operand;
        IncludeMissingFields = includeMissingFields;
        Rows = rows;
        ChangedCount = rows.Count(static row => row.Status == BulkEditRowStatus.Changed);
        UnchangedCount = rows.Count(static row => row.Status == BulkEditRowStatus.Unchanged);
        SkippedCount = rows.Count(static row => row.Status == BulkEditRowStatus.SkippedMissing);
        ErrorCount = rows.Count(static row => row.Status == BulkEditRowStatus.Error);
    }

    /// <summary>
    /// Gets the exact XML field name of the target column.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Gets the evaluated operation.
    /// </summary>
    public BulkEditOperation Operation { get; }

    /// <summary>
    /// Gets the raw operand text.
    /// </summary>
    public string Operand { get; }

    /// <summary>
    /// Gets a value indicating whether absent fields are included.
    /// </summary>
    public bool IncludeMissingFields { get; }

    /// <summary>
    /// Gets the preview rows in request order.
    /// </summary>
    public IReadOnlyList<BulkEditPreviewRow> Rows { get; }

    /// <summary>
    /// Gets the number of rows whose value will change.
    /// </summary>
    public int ChangedCount { get; }

    /// <summary>
    /// Gets the number of rows whose computed value equals the current value.
    /// </summary>
    public int UnchangedCount { get; }

    /// <summary>
    /// Gets the number of rows skipped because the field is absent.
    /// </summary>
    public int SkippedCount { get; }

    /// <summary>
    /// Gets the number of rows with errors.
    /// </summary>
    public int ErrorCount { get; }

    /// <summary>
    /// Gets a value indicating whether the preview can be applied: there is at least one change and
    /// no errors.
    /// </summary>
    public bool CanApply => ErrorCount == 0 && ChangedCount > 0;
}
