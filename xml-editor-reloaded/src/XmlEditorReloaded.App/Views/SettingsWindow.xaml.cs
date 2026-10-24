using System.Windows;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Modal dialog that edits data directories, the active directory, the interface language, and the
/// stored grid layouts.
/// </summary>
public partial class SettingsWindow : Window
{
    private readonly SettingsViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Settings view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public SettingsWindow(SettingsViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
        ActiveColumn.Header = viewModel.ActiveColumn;
        PathColumn.Header = viewModel.PathColumn;
    }

    /// <summary>
    /// Gets the confirmed settings after the dialog is accepted, or <see langword="null"/> when cancelled.
    /// </summary>
    public SettingsResult? Result { get; private set; }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        Result = _viewModel.CreateResult();
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
