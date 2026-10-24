using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Requests that the shell opens a table and selects one of its records, for example when the user
/// follows a relation from the detail panel.
/// </summary>
/// <param name="TableName">Target table to open or activate.</param>
/// <param name="RecordKey">Primary key of the record to select.</param>
/// <param name="TableDataDirectory">
/// Absolute <c>TableData</c> directory of the data set that owns the target table. Navigation stays
/// inside the data set of the editor that raised the request.
/// </param>
public sealed record TableNavigationRequest(
    TableName TableName,
    string RecordKey,
    string TableDataDirectory);
