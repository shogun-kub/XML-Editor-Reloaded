using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App;

/// <summary>
/// Main application window.
/// </summary>
public partial class MainWindow : Window
{
    private bool _closeApproved;
    private bool _closeInProgress;

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

    /// <inheritdoc />
    protected override async void OnClosing(CancelEventArgs e)
    {
        if (_closeApproved || DataContext is not MainViewModel viewModel)
        {
            base.OnClosing(e);
            return;
        }

        if (_closeInProgress)
        {
            e.Cancel = true;
            return;
        }

        var request = viewModel.RequestCloseAsync();
        if (request.IsCompletedSuccessfully)
        {
            if (request.Result)
            {
                _closeApproved = true;
                base.OnClosing(e);
            }
            else
            {
                e.Cancel = true;
            }

            return;
        }

        e.Cancel = true;
        _closeInProgress = true;
        try
        {
            if (await request)
            {
                _closeApproved = true;
                Close();
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                Strings.Format("StatusError", exception.Message),
                Strings.Get("AppTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            _closeInProgress = false;
        }
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
