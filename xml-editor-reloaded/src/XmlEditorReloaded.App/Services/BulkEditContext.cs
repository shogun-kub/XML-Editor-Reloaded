using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Data supplied by the table editor to the bulk edit dialog.
/// </summary>
public sealed class BulkEditContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkEditContext"/> class.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="columns">Columns that can be edited in bulk.</param>
    /// <param name="recordKeys">Primary keys of the rows in the current view.</param>
    /// <param name="preview">Callback that evaluates a request against the table without mutating it.</param>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    public BulkEditContext(
        string title,
        IReadOnlyList<BulkEditColumnOption> columns,
        IReadOnlyList<string> recordKeys,
        Func<BulkEditRequest, OperationResult<BulkEditPreview>> preview)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(recordKeys);
        ArgumentNullException.ThrowIfNull(preview);

        Title = title;
        Columns = columns.ToArray();
        RecordKeys = recordKeys.ToArray();
        Preview = preview;
    }

    /// <summary>
    /// Gets the dialog title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the columns that can be edited in bulk.
    /// </summary>
    public IReadOnlyList<BulkEditColumnOption> Columns { get; }

    /// <summary>
    /// Gets the primary keys of the rows in the current view.
    /// </summary>
    public IReadOnlyList<string> RecordKeys { get; }

    /// <summary>
    /// Gets the callback that evaluates a bulk request without mutating the table.
    /// </summary>
    public Func<BulkEditRequest, OperationResult<BulkEditPreview>> Preview { get; }
}

/// <summary>
/// One selectable column of the bulk edit dialog.
/// </summary>
/// <param name="FieldName">Exact XML field name.</param>
/// <param name="TypeText">Display name of the column type.</param>
/// <param name="DataType">Column data type used to enable arithmetic operations.</param>
public sealed record BulkEditColumnOption(string FieldName, string TypeText, ColumnDataType DataType)
{
    /// <summary>
    /// Gets the text shown in the column selector.
    /// </summary>
    public string DisplayText => $"{FieldName} — {TypeText}";

    /// <summary>
    /// Gets a value indicating whether add and multiply are meaningful for this column.
    /// </summary>
    public bool SupportsArithmetic => DataType is ColumnDataType.Integer or ColumnDataType.Decimal;
}
