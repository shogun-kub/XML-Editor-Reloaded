using System.Windows;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Dialog that edits the unsigned 64-bit identifier of a lookup value.
/// </summary>
public partial class LookupIdWindow : Window
{
    private readonly LookupIdViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupIdWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Lookup identifier view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public LookupIdWindow(LookupIdViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
    }

    /// <summary>
    /// Gets the accepted canonical identifier, or <see langword="null"/> when the dialog was cancelled.
    /// </summary>
    public string? Value { get; private set; }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.TryAccept())
        {
            return;
        }

        Value = _viewModel.Value;
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
