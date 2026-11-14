using System.Windows;
using System.Windows.Input;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Modeless context-help window for a table family.
/// </summary>
public partial class HelpWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Help content.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public HelpWindow(HelpViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
        Loaded += (_, _) => SearchBox.Focus();
    }

    private HelpViewModel? ViewModel => DataContext as HelpViewModel;

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();

    private void OnSearchKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || ViewModel is not { } viewModel)
        {
            return;
        }

        if (viewModel.SearchCommand.CanExecute(null))
        {
            viewModel.SearchCommand.Execute(null);
        }

        e.Handled = true;
    }

    private void OnSearchResultDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SearchResultsList.SelectedItem is HelpSearchItemViewModel result
            && ViewModel is { } viewModel
            && viewModel.OpenSearchResultCommand.CanExecute(result))
        {
            viewModel.OpenSearchResultCommand.Execute(result);
            e.Handled = true;
        }
    }

    private void OnMarkdownLinkRequested(object sender, string target) => ViewModel?.OpenDocument(target);
}
