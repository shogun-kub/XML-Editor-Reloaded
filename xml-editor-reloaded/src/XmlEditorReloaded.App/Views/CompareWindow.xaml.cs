using System.Windows;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Read-only window that compares two <c>TableData</c> directories and exports the report.
/// </summary>
public partial class CompareWindow : Window
{
    private readonly CompareViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompareWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Comparison view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public CompareWindow(CompareViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;

        TableColumn.Header = viewModel.TableHeader;
        TableStatusColumn.Header = viewModel.StatusHeader;
        LeftRecordsColumn.Header = viewModel.LeftRecordsHeader;
        RightRecordsColumn.Header = viewModel.RightRecordsHeader;
        AddedFieldsColumn.Header = viewModel.AddedFieldsHeader;
        RemovedFieldsColumn.Header = viewModel.RemovedFieldsHeader;
        ChangedTypesColumn.Header = viewModel.ChangedTypesHeader;

        FieldNameColumn.Header = viewModel.FieldHeader;
        FieldStatusColumn.Header = viewModel.FieldStatusHeader;
        LeftTypeColumn.Header = viewModel.LeftTypeHeader;
        RightTypeColumn.Header = viewModel.RightTypeHeader;
        LeftPresenceColumn.Header = viewModel.LeftPresenceHeader;
        RightPresenceColumn.Header = viewModel.RightPresenceHeader;

        LookupTableColumn.Header = viewModel.LookupTableHeader;
        LookupFieldColumn.Header = viewModel.LookupFieldHeader;
        LookupNameColumn.Header = viewModel.LookupNameHeader;
        LookupLeftColumn.Header = viewModel.LookupLeftHeader;
        LookupRightColumn.Header = viewModel.LookupRightHeader;

        Closed += (_, _) => _viewModel.CancelRunningComparison();
    }

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
