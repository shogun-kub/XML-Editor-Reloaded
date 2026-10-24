using System.Windows;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Read-only window that lists translation issues for the selected language overlay.
/// </summary>
public partial class TranslationReportWindow : Window
{
    private readonly TranslationReportViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationReportWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Translation report view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public TranslationReportWindow(TranslationReportViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
        IssuesGrid.Columns[0].Header = viewModel.KindHeader;
        IssuesGrid.Columns[1].Header = viewModel.RecordHeader;
        IssuesGrid.Columns[2].Header = viewModel.FieldHeader;
        IssuesGrid.Columns[3].Header = viewModel.MessageHeader;
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
