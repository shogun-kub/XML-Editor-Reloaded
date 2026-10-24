using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Xml.Comparison;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// View model for the read-only cross-version comparison window. Compares two
/// <c>TableData</c> directories and exports the report as Markdown or CSV.
/// </summary>
public partial class CompareViewModel : ObservableObject
{
    private readonly ITableComparisonService _comparer;
    private readonly IDialogService _dialogService;
    private TableComparisonReport? _report;
    private CancellationTokenSource? _operationCts;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompareCommand))]
    private string _leftDirectory = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompareCommand))]
    private string _rightDirectory = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CompareCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCompareCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private string _summaryText = string.Empty;

    [ObservableProperty]
    private string _summaryFieldsText = string.Empty;

    [ObservableProperty]
    private ComparisonTableItem? _selectedTable;

    [ObservableProperty]
    private string _selectedDetailsHeader = string.Empty;

    [ObservableProperty]
    private string _profileText = string.Empty;

    [ObservableProperty]
    private string _diagnosticsText = string.Empty;

    [ObservableProperty]
    private bool _hasDiagnostics;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompareViewModel"/> class.
    /// </summary>
    /// <param name="comparer">Cross-version comparison service.</param>
    /// <param name="dialogService">Dialog service used for folder and save-file pickers.</param>
    /// <exception cref="ArgumentNullException">Thrown when a dependency is null.</exception>
    public CompareViewModel(ITableComparisonService comparer, IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(dialogService);

        _comparer = comparer;
        _dialogService = dialogService;
        _statusText = Strings.Get("CompareReadyText");

        Title = Strings.Get("CompareTitle");
        LeftLabel = Strings.Get("CompareLeftLabel");
        RightLabel = Strings.Get("CompareRightLabel");
        BrowseText = Strings.Get("CompareBrowseText");
        CompareText = Strings.Get("CompareRunText");
        CancelText = Strings.Get("CompareCancelText");
        ExportText = Strings.Get("CompareExportText");
        CloseText = Strings.Get("CompareCloseText");
        FieldsTabText = Strings.Get("CompareFieldsTab");
        LookupTabText = Strings.Get("CompareLookupTab");
        ProfileLabel = Strings.Get("CompareProfileLabel");
        DiagnosticsLabel = Strings.Get("CompareDiagnosticsLabel");

        TableHeader = Strings.Get("CompareTableHeader");
        StatusHeader = Strings.Get("CompareStatusHeader");
        LeftRecordsHeader = Strings.Get("CompareLeftRecordsHeader");
        RightRecordsHeader = Strings.Get("CompareRightRecordsHeader");
        AddedFieldsHeader = Strings.Get("CompareAddedFieldsHeader");
        RemovedFieldsHeader = Strings.Get("CompareRemovedFieldsHeader");
        ChangedTypesHeader = Strings.Get("CompareChangedTypesHeader");

        FieldHeader = Strings.Get("CompareFieldHeader");
        FieldStatusHeader = Strings.Get("CompareFieldStatusHeader");
        LeftTypeHeader = Strings.Get("CompareLeftTypeHeader");
        RightTypeHeader = Strings.Get("CompareRightTypeHeader");
        LeftPresenceHeader = Strings.Get("CompareLeftPresenceHeader");
        RightPresenceHeader = Strings.Get("CompareRightPresenceHeader");

        LookupTableHeader = Strings.Get("CompareTableHeader");
        LookupFieldHeader = Strings.Get("CompareFieldHeader");
        LookupNameHeader = Strings.Get("CompareLookupNameHeader");
        LookupLeftHeader = Strings.Get("CompareLeftHeader");
        LookupRightHeader = Strings.Get("CompareRightHeader");
    }

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the baseline directory label.
    /// </summary>
    public string LeftLabel { get; }

    /// <summary>
    /// Gets the candidate directory label.
    /// </summary>
    public string RightLabel { get; }

    /// <summary>
    /// Gets the browse button text.
    /// </summary>
    public string BrowseText { get; }

    /// <summary>
    /// Gets the compare button text.
    /// </summary>
    public string CompareText { get; }

    /// <summary>
    /// Gets the cancel button text.
    /// </summary>
    public string CancelText { get; }

    /// <summary>
    /// Gets the export button text.
    /// </summary>
    public string ExportText { get; }

    /// <summary>
    /// Gets the close button text.
    /// </summary>
    public string CloseText { get; }

    /// <summary>
    /// Gets the fields tab header.
    /// </summary>
    public string FieldsTabText { get; }

    /// <summary>
    /// Gets the lookup references tab header.
    /// </summary>
    public string LookupTabText { get; }

    /// <summary>
    /// Gets the sparse profile label.
    /// </summary>
    public string ProfileLabel { get; }

    /// <summary>
    /// Gets the diagnostics label.
    /// </summary>
    public string DiagnosticsLabel { get; }

    /// <summary>
    /// Gets the table column header.
    /// </summary>
    public string TableHeader { get; }

    /// <summary>
    /// Gets the status column header.
    /// </summary>
    public string StatusHeader { get; }

    /// <summary>
    /// Gets the left record-count column header.
    /// </summary>
    public string LeftRecordsHeader { get; }

    /// <summary>
    /// Gets the right record-count column header.
    /// </summary>
    public string RightRecordsHeader { get; }

    /// <summary>
    /// Gets the added-fields column header.
    /// </summary>
    public string AddedFieldsHeader { get; }

    /// <summary>
    /// Gets the removed-fields column header.
    /// </summary>
    public string RemovedFieldsHeader { get; }

    /// <summary>
    /// Gets the changed-types column header.
    /// </summary>
    public string ChangedTypesHeader { get; }

    /// <summary>
    /// Gets the field column header.
    /// </summary>
    public string FieldHeader { get; }

    /// <summary>
    /// Gets the field status column header.
    /// </summary>
    public string FieldStatusHeader { get; }

    /// <summary>
    /// Gets the left type column header.
    /// </summary>
    public string LeftTypeHeader { get; }

    /// <summary>
    /// Gets the right type column header.
    /// </summary>
    public string RightTypeHeader { get; }

    /// <summary>
    /// Gets the left presence column header.
    /// </summary>
    public string LeftPresenceHeader { get; }

    /// <summary>
    /// Gets the right presence column header.
    /// </summary>
    public string RightPresenceHeader { get; }

    /// <summary>
    /// Gets the lookup table column header.
    /// </summary>
    public string LookupTableHeader { get; }

    /// <summary>
    /// Gets the lookup field column header.
    /// </summary>
    public string LookupFieldHeader { get; }

    /// <summary>
    /// Gets the lookup name column header.
    /// </summary>
    public string LookupNameHeader { get; }

    /// <summary>
    /// Gets the lookup left-availability column header.
    /// </summary>
    public string LookupLeftHeader { get; }

    /// <summary>
    /// Gets the lookup right-availability column header.
    /// </summary>
    public string LookupRightHeader { get; }

    /// <summary>
    /// Gets the table comparison rows.
    /// </summary>
    public ObservableCollection<ComparisonTableItem> Tables { get; } = new();

    /// <summary>
    /// Gets the field comparisons of the selected table.
    /// </summary>
    public ObservableCollection<ComparisonFieldItem> Fields { get; } = new();

    /// <summary>
    /// Gets the lookup reference rows of the whole report.
    /// </summary>
    public ObservableCollection<ComparisonLookupItem> LookupReferences { get; } = new();

    /// <summary>
    /// Gets a value indicating whether a report is available for export.
    /// </summary>
    public bool HasReport => _report is not null;

    /// <summary>
    /// Suggests the baseline directory before the window is opened.
    /// </summary>
    /// <param name="directory">Directory selected in the main window; ignored when a baseline is already set.</param>
    public void SetInitialDirectory(string? directory)
    {
        if (string.IsNullOrWhiteSpace(LeftDirectory) && !string.IsNullOrWhiteSpace(directory))
        {
            LeftDirectory = directory;
        }
    }

    /// <summary>
    /// Cancels a running comparison, if any. Called when the window closes.
    /// </summary>
    public void CancelRunningComparison() => _operationCts?.Cancel();

    [RelayCommand]
    private void BrowseLeft()
    {
        var folder = _dialogService.PickFolder(
            Strings.Get("CompareChooseLeft"),
            string.IsNullOrWhiteSpace(LeftDirectory) ? null : LeftDirectory);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            LeftDirectory = folder;
        }
    }

    [RelayCommand]
    private void BrowseRight()
    {
        var folder = _dialogService.PickFolder(
            Strings.Get("CompareChooseRight"),
            string.IsNullOrWhiteSpace(RightDirectory) ? null : RightDirectory);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            RightDirectory = folder;
        }
    }

    [RelayCommand(CanExecute = nameof(CanCompare))]
    private async Task CompareAsync()
    {
        var leftTableData = DataSetPaths.ResolveTableDataDirectory(LeftDirectory);
        var rightTableData = DataSetPaths.ResolveTableDataDirectory(RightDirectory);
        if (leftTableData is null || rightTableData is null)
        {
            StatusText = Strings.Get("CompareInvalidDirectory");
            return;
        }

        IsBusy = true;
        StatusText = Strings.Get("CompareRunningText");
        _operationCts = new CancellationTokenSource();
        try
        {
            var result = await _comparer
                .CompareAsync(leftTableData, rightTableData, _operationCts.Token)
                .ConfigureAwait(true);
            if (!result.IsSuccess)
            {
                StatusText = Strings.Format("CompareFailedText", result.ErrorText ?? string.Empty);
                return;
            }

            ShowReport(result.Data!);
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("CompareCancelledText");
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("CompareFailedText", exception.Message);
        }
        finally
        {
            _operationCts?.Dispose();
            _operationCts = null;
            IsBusy = false;
            ExportCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanCompare() =>
        !IsBusy
        && !string.IsNullOrWhiteSpace(LeftDirectory)
        && !string.IsNullOrWhiteSpace(RightDirectory);

    [RelayCommand(CanExecute = nameof(CanCancelCompare))]
    private void CancelCompare() => _operationCts?.Cancel();

    private bool CanCancelCompare() => IsBusy;

    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task ExportAsync()
    {
        if (_report is null)
        {
            StatusText = Strings.Get("CompareNoReportText");
            return;
        }

        var suggestedName = "tabledata-comparison-"
            + _report.GeneratedAtUtc.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)
            + ".md";
        var path = _dialogService.PickSaveFile(
            Strings.Get("CompareExportTitle"),
            Strings.Get("CompareExportFilter"),
            suggestedName);
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        try
        {
            var text = string.Equals(Path.GetExtension(path), ".csv", StringComparison.OrdinalIgnoreCase)
                ? TableComparisonReportFormatter.FormatCsv(_report)
                : TableComparisonReportFormatter.FormatMarkdown(_report);
            await File.WriteAllTextAsync(path, text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
                .ConfigureAwait(true);
            StatusText = Strings.Format("CompareExportedText", path);
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("CompareExportFailedText", exception.Message);
        }
    }

    private bool CanExport() => _report is not null && !IsBusy;

    partial void OnSelectedTableChanged(ComparisonTableItem? value)
    {
        Fields.Clear();
        if (value is null)
        {
            SelectedDetailsHeader = string.Empty;
            ProfileText = string.Empty;
            DiagnosticsText = string.Empty;
            HasDiagnostics = false;
            return;
        }

        SelectedDetailsHeader = $"{value.TableName} — {value.StatusText}";
        foreach (var field in value.Entry.Fields)
        {
            Fields.Add(new ComparisonFieldItem(field));
        }

        ProfileText = value.ProfileText;
        DiagnosticsText = value.DiagnosticsText;
        HasDiagnostics = !string.IsNullOrEmpty(DiagnosticsText);
    }

    private void ShowReport(TableComparisonReport report)
    {
        _report = report;
        Tables.Clear();
        LookupReferences.Clear();
        SelectedTable = null;
        Fields.Clear();

        foreach (var entry in report.Tables)
        {
            Tables.Add(new ComparisonTableItem(entry));
        }

        foreach (var reference in report.LookupReferences)
        {
            LookupReferences.Add(new ComparisonLookupItem(reference));
        }

        var summary = report.Summary;
        SummaryText = Strings.Format(
            "CompareSummaryText",
            summary.CommonTableCount,
            summary.IdenticalTableCount,
            summary.ChangedTableCount,
            summary.LeftOnlyTableCount,
            summary.RightOnlyTableCount,
            summary.MalformedTableCount + summary.FailedTableCount);
        SummaryFieldsText = Strings.Format(
            "CompareSummaryFieldsText",
            summary.AddedFieldCount,
            summary.RemovedFieldCount,
            summary.TypeChangedFieldCount,
            summary.CardinalityChangedFieldCount,
            summary.LeftRecordTotal,
            summary.RightRecordTotal);
        StatusText = SummaryText;
        SelectedTable = Tables.FirstOrDefault(static item => item.HasDifferences)
            ?? Tables.FirstOrDefault();
        OnPropertyChanged(nameof(HasReport));
        ExportCommand.NotifyCanExecuteChanged();
    }
}
