using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Diagnostics;

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

    /// <summary>
    /// Applies a saved size, position, and maximized state. A placement saved on a monitor that is no
    /// longer available is moved back inside the virtual screen instead of being restored off-screen.
    /// </summary>
    /// <param name="placement">Saved placement, or <see langword="null"/> to keep the designer defaults.</param>
    public void ApplySavedPlacement(WindowPlacement? placement)
    {
        var fitted = placement?.FitToScreen(
            SystemParameters.VirtualScreenLeft,
            SystemParameters.VirtualScreenTop,
            SystemParameters.VirtualScreenWidth,
            SystemParameters.VirtualScreenHeight,
            MinWidth,
            MinHeight);
        if (fitted is null)
        {
            return;
        }

        WindowStartupLocation = WindowStartupLocation.Manual;
        Left = fitted.Left;
        Top = fitted.Top;
        Width = fitted.Width;
        Height = fitted.Height;
        if (fitted.IsMaximized)
        {
            WindowState = WindowState.Maximized;
        }
    }

    /// <summary>
    /// Captures the placement to persist on close. A minimized window keeps its previously saved
    /// placement, because restoring an application minimized would hide it at start-up.
    /// </summary>
    /// <returns>The current placement, or <see langword="null"/> when there is nothing to persist.</returns>
    public WindowPlacement? CapturePlacement()
    {
        if (WindowState == WindowState.Minimized)
        {
            return null;
        }

        var width = ActualWidth > 0 ? ActualWidth : Width;
        var height = ActualHeight > 0 ? ActualHeight : Height;
        var bounds = WindowState == WindowState.Normal
            ? new Rect(Left, Top, width, height)
            : RestoreBounds;
        if (!double.IsFinite(bounds.Left)
            || !double.IsFinite(bounds.Top)
            || !double.IsFinite(bounds.Width)
            || !double.IsFinite(bounds.Height)
            || bounds.Width <= 0
            || bounds.Height <= 0)
        {
            return null;
        }

        return new WindowPlacement(
            bounds.Left,
            bounds.Top,
            bounds.Width,
            bounds.Height,
            WindowState == WindowState.Maximized);
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

        var request = viewModel.RequestCloseAsync(CapturePlacement());
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
            OperationLog.Error("MainWindow", "The window could not be closed.", exception: exception);
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

    private void OnExitMenuClick(object sender, RoutedEventArgs e) => Close();

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
