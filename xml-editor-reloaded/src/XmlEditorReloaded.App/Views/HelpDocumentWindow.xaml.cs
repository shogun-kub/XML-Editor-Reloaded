using System.Windows;
using XmlEditorReloaded.App.Services;
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
        ApplyDocument(viewModel);
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is HelpDocumentViewModel viewModel)
        {
            ApplyDocument(viewModel);
        }
    }

    private void ApplyDocument(HelpDocumentViewModel viewModel)
    {
        Title = viewModel.Title;
        DocumentViewer.Document = MarkdownFlowDocumentRenderer.Render(viewModel.Text, viewModel.Title);
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
}
