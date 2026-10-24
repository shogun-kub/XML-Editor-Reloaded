using System.Windows;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Dialog that edits a 32-bit integer as a set of bit flags.
/// </summary>
public partial class BitEditorWindow : Window
{
    private readonly BitEditorViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="BitEditorWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Bit editor view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public BitEditorWindow(BitEditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
    }

    /// <summary>
    /// Gets the edited value after the dialog is accepted, or <see langword="null"/> when cancelled.
    /// </summary>
    public uint? Value { get; private set; }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        Value = _viewModel.ComposeValue();
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
