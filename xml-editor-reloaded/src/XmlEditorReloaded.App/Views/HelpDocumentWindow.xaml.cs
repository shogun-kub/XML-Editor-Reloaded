using System.Windows;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Modeless read-only window for a standalone help page.
/// </summary>
public partial class HelpDocumentWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpDocumentWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Page to display.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public HelpDocumentWindow(HelpDocumentViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        InitializeComponent();
        DataContext = viewModel;
        DataContextChanged += OnDataContextChanged;
        Title = viewModel.Title;
    }

    private HelpDocumentViewModel? ViewModel => DataContext as HelpDocumentViewModel;

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is HelpDocumentViewModel viewModel)
        {
            Title = viewModel.Title;
        }
    }

    private void OnDocumentLinkRequested(object sender, string target) => ViewModel?.OpenDocument(target);

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
}
