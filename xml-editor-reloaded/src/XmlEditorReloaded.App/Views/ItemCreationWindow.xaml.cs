using System.Windows;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Dialog that collects the identifier, name, and class of a new item.
/// </summary>
public partial class ItemCreationWindow : Window
{
    private readonly ItemCreationViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemCreationWindow"/> class.
    /// </summary>
    /// <param name="viewModel">New-item view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public ItemCreationWindow(ItemCreationViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
    }

    /// <summary>
    /// Gets the creation request after the dialog is accepted, or <see langword="null"/> when cancelled.
    /// </summary>
    public ItemCreationRequest? Request { get; private set; }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        Request = _viewModel.ToRequest();
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
