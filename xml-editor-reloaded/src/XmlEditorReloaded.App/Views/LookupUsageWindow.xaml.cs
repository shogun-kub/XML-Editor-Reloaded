using System.Windows;
using System.Windows.Input;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Read-only window that lists the records using one lookup value and opens a selected record.
/// </summary>
public partial class LookupUsageWindow : Window
{
    private readonly LookupUsageViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupUsageWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Lookup usage view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public LookupUsageWindow(LookupUsageViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
        UsageGrid.Columns[0].Header = viewModel.TableHeader;
        UsageGrid.Columns[1].Header = viewModel.RecordHeader;
        UsageGrid.Columns[2].Header = viewModel.FieldHeader;
        UsageGrid.Columns[3].Header = viewModel.ValueHeader;
        _viewModel.NavigationRequested += OnNavigationRequested;
    }

    private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement { DataContext: LookupUsageItem })
        {
            _viewModel.OpenSelected();
        }
    }

    private void OnNavigationRequested(object? sender, LookupUsageNavigationRequest request) => Close();

    private void OnOpen(object sender, RoutedEventArgs e) => _viewModel.OpenSelected();

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
