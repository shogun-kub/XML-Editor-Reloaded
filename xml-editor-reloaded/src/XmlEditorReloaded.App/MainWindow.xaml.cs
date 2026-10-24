using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App;

/// <summary>
/// Main application window.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Main shell view model.</param>
    public MainWindow(MainViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        InitializeComponent();
        DataContext = viewModel;
        TableNameColumn.Header = viewModel.TableColumn;
        LanguagesColumn.Header = viewModel.LanguagesColumn;
        MappedColumn.Header = viewModel.MappedColumn;
    }

    private void OnTablesGridDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel || viewModel.SelectedTable is null)
        {
            return;
        }

        if (ItemsControl.ContainerFromElement(TablesGrid, e.OriginalSource as DependencyObject) is not DataGridRow)
        {
            return;
        }

        if (viewModel.OpenTableCommand.CanExecute(null))
        {
            viewModel.OpenTableCommand.Execute(null);
        }
    }
}
