using System.Windows;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Dialog that previews and confirms a bulk change of one column.
/// </summary>
public partial class BulkEditWindow : Window
{
    private readonly BulkEditViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="BulkEditWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Bulk edit view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public BulkEditWindow(BulkEditViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
        PreviewGrid.Columns[0].Header = viewModel.KeyHeader;
        PreviewGrid.Columns[1].Header = viewModel.OldHeader;
        PreviewGrid.Columns[2].Header = viewModel.NewHeader;
        PreviewGrid.Columns[3].Header = viewModel.StatusHeader;
    }

    /// <summary>
    /// Gets the request to apply after the dialog is accepted, or <see langword="null"/> when cancelled.
    /// </summary>
    public BulkEditRequest? Request { get; private set; }

    private void OnOperandFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            _viewModel.BeginOperandEdit();
        }
        else
        {
            _viewModel.EndOperandEdit();
        }
    }

    private void OnOperandDropDownClosed(object? sender, EventArgs e) => _viewModel.EndOperandEdit();

    private void OnApply(object sender, RoutedEventArgs e)
    {
        var request = _viewModel.CreateRequest();
        if (request is null)
        {
            return;
        }

        Request = request;
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
