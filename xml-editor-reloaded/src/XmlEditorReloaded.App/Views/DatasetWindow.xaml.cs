using System.Windows;
using System.Windows.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Read-only window that validates the whole data set in one report and searches every table value.
/// Double-clicking an issue or a hit asks the shell to open the table and select the record.
/// </summary>
public partial class DatasetWindow : Window
{
    private readonly DatasetViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Data-set view model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public DatasetWindow(DatasetViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        _viewModel = viewModel;
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;

        IssueTableColumn.Header = viewModel.TableHeader;
        IssueSeverityColumn.Header = viewModel.SeverityHeader;
        IssueRecordColumn.Header = viewModel.RecordHeader;
        IssueFieldColumn.Header = viewModel.FieldHeader;
        IssueMessageColumn.Header = viewModel.MessageHeader;

        HitTableColumn.Header = viewModel.TableHeader;
        HitRecordColumn.Header = viewModel.RecordHeader;
        HitFieldColumn.Header = viewModel.FieldHeader;
        HitValueColumn.Header = viewModel.ValueHeader;
        HitLanguageColumn.Header = viewModel.LanguageHeader;

        Tabs.SelectedIndex = viewModel.InitialTabIndex;
        Loaded += OnLoaded;
        Closed += (_, _) => _viewModel.CancelRunningOperations();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        try
        {
            await _viewModel.InitializeAsync();
        }
        catch (Exception exception)
        {
            _viewModel.StatusText = Strings.Format("StatusError", exception.Message);
        }
    }

    private void OnIssuesGridDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement { DataContext: DatasetIssueItem })
        {
            _viewModel.OpenIssueCommand.Execute(null);
        }
    }

    private void OnHitsGridDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement { DataContext: DatasetSearchHitItem })
        {
            _viewModel.OpenHitCommand.Execute(null);
        }
    }

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
