using XmlEditorReloaded.Core.Api;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Applies a committed grid cell edit to the underlying workspace.
/// </summary>
internal interface ITableCellEditor
{
    /// <summary>
    /// Applies one cell value, adding a missing field when the cell was empty.
    /// </summary>
    /// <param name="row">Row being edited.</param>
    /// <param name="fieldName">XML field name.</param>
    /// <param name="value">New text value.</param>
    /// <returns>The operation result from the workspace.</returns>
    OperationResult SetCellValue(TableRowViewModel row, string fieldName, string value);
}
