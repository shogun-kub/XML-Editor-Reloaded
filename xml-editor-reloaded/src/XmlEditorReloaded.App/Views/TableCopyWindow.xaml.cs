using System.Windows;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Modal dialog that previews and commits a cross-data-set table copy.
/// </summary>
public partial class TableCopyWindow : Window
{
    private readonly TableCopyViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableCopyWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Copy view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public TableCopyWindow(TableCopyViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;

        SourceKeyColumn.Header = viewModel.SourceKeyHeader;
        SourceDisplayColumn.Header = viewModel.SourceDisplayHeader;
        TargetKeyColumn.Header = viewModel.TargetKeyHeader;
        ActionColumn.Header = viewModel.ActionHeader;
        ReferencesColumn.Header = viewModel.ReferencesHeader;
        RelatedColumn.Header = viewModel.RelatedHeader;
        WarningsColumn.Header = viewModel.WarningsColumnHeader;

        viewModel.Completed += OnCompleted;
        Closed += (_, _) => viewModel.Completed -= OnCompleted;
    }

    private void OnCompleted(object? sender, EventArgs e) => DialogResult = true;

    private void OnClose(object sender, RoutedEventArgs e) => DialogResult = false;
}
