using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Search;
using XmlEditorReloaded.Xml.Validation;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Tab a data-set window opens on.
/// </summary>
public enum DatasetWindowTab
{
    /// <summary>Validate every table of the data set.</summary>
    Validation = 0,

    /// <summary>Search the values of every table.</summary>
    Search = 1,
}

/// <summary>
/// Request to open a table, and optionally select a record, from a data-set report.
/// </summary>
/// <param name="TableName">Table to open.</param>
/// <param name="RecordKey">Primary key to select, or <see langword="null"/> to keep the current selection.</param>
public sealed record DatasetNavigationRequest(TableName TableName, string? RecordKey);

/// <summary>
/// One entry of the severity filter of the validation tab.
/// </summary>
/// <param name="Label">Localized filter label.</param>
/// <param name="Severity">Severity to show, or <see langword="null"/> for all severities.</param>
public sealed record DatasetSeverityOption(string Label, ValidationSeverity? Severity);

/// <summary>
/// View model for the read-only data-set window: a single validation report for every table and a
/// global search across all table values and language overlays.
/// </summary>
public partial class DatasetViewModel : ObservableObject
{
    private readonly IDataSetSession _session;
    private readonly DatasetValidator _validator;
    private readonly DatasetSearchService _search;
    private readonly IDialogService _dialogService;
    private readonly DatasetWindowTab _initialTab;
    private IReadOnlyList<TableDescriptor> _tables = Array.Empty<TableDescriptor>();
    private IReadOnlyList<DatasetIssueItem> _allIssues = Array.Empty<DatasetIssueItem>();
    private DatasetValidationReport? _report;
    private CancellationTokenSource? _validationCts;
    private CancellationTokenSource? _searchCts;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ValidateCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelValidationCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportCommand))]
    private bool _isValidationBusy;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelSearchCommand))]
    private bool _isSearchBusy;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private string _validationSummaryText = string.Empty;

    [ObservableProperty]
    private string _validationNoteText = string.Empty;

    [ObservableProperty]
    private bool _hasValidationNote;

    [ObservableProperty]
    private string _issueFilterText = string.Empty;

    [ObservableProperty]
    private DatasetSeverityOption? _selectedSeverity;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenIssueCommand))]
    private DatasetIssueItem? _selectedIssue;

    [ObservableProperty]
    private bool _isValidationEmpty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private string _searchTerm = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private bool _matchCase;

    [ObservableProperty]
    private string _searchSummaryText = string.Empty;

    [ObservableProperty]
    private string _searchNoteText = string.Empty;

    [ObservableProperty]
    private bool _hasSearchNote;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenHitCommand))]
    private DatasetSearchHitItem? _selectedHit;

    [ObservableProperty]
    private bool _isSearchEmpty;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetViewModel"/> class.
    /// </summary>
    /// <param name="session">Data-set session whose tables are validated and searched.</param>
    /// <param name="validator">Data-set validator.</param>
    /// <param name="search">Data-set search service.</param>
    /// <param name="dialogService">Dialog service used to pick the export file.</param>
    /// <param name="initialTab">Tab the window opens on.</param>
    /// <exception cref="ArgumentNullException">Thrown when a dependency is null.</exception>
    public DatasetViewModel(
        IDataSetSession session,
        DatasetValidator validator,
        DatasetSearchService search,
        IDialogService dialogService,
        DatasetWindowTab initialTab = DatasetWindowTab.Validation)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(search);
        ArgumentNullException.ThrowIfNull(dialogService);

        _session = session;
        _validator = validator;
        _search = search;
        _dialogService = dialogService;
        _initialTab = initialTab;
        _statusText = initialTab == DatasetWindowTab.Validation
            ? Strings.Get("DatasetValidationRunning")
            : Strings.Get("DatasetSearchReady");

        Title = Strings.Format("DatasetTitle", DatasetNames.LabelFor(session));
        ValidationTabText = Strings.Get("DatasetValidationTab");
        SearchTabText = Strings.Get("DatasetSearchTab");
        ValidateText = Strings.Get("DatasetValidateText");
        CancelText = Strings.Get("DatasetCancelText");
        CloseText = Strings.Get("DatasetCloseText");
        ExportText = Strings.Get("DatasetExportText");
        SeverityLabel = Strings.Get("DatasetSeverityLabel");
        FilterLabel = Strings.Get("DatasetFilterLabel");
        TableHeader = Strings.Get("DatasetTableHeader");
        SeverityHeader = Strings.Get("DatasetSeverityHeader");
        RecordHeader = Strings.Get("DatasetKeyHeader");
        FieldHeader = Strings.Get("DatasetFieldHeader");
        MessageHeader = Strings.Get("DatasetMessageHeader");
        ValueHeader = Strings.Get("DatasetValueHeader");
        LanguageHeader = Strings.Get("DatasetLanguageHeader");
        SearchLabel = Strings.Get("DatasetSearchLabel");
        RunSearchText = Strings.Get("DatasetSearchRunText");
        MatchCaseText = Strings.Get("DatasetMatchCase");
        EmptyText = Strings.Get("DatasetValidationEmpty");
        SearchEmptyText = Strings.Get("DatasetSearchEmpty");
        OpenText = Strings.Get("DatasetOpenText");
        ValidationHintText = Strings.Get("DatasetValidationHint");
        SearchHintText = Strings.Get("DatasetSearchHint");

        SeverityOptions = new[]
        {
            new DatasetSeverityOption(Strings.Get("DatasetSeverityAll"), null),
            new DatasetSeverityOption(Strings.Get("DatasetSeverityError"), ValidationSeverity.Error),
            new DatasetSeverityOption(Strings.Get("DatasetSeverityWarning"), ValidationSeverity.Warning),
            new DatasetSeverityOption(Strings.Get("DatasetSeverityInfo"), ValidationSeverity.Info),
        };
        _selectedSeverity = SeverityOptions[0];
    }

    /// <summary>
    /// Occurs when the user asks to open a table from a validation issue or a search hit.
    /// </summary>
    public event EventHandler<DatasetNavigationRequest>? NavigationRequested;

    /// <summary>
    /// Gets the data-set session whose tables are shown.
    /// </summary>
    public IDataSetSession Session => _session;

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the validation tab header.
    /// </summary>
    public string ValidationTabText { get; }

    /// <summary>
    /// Gets the search tab header.
    /// </summary>
    public string SearchTabText { get; }

    /// <summary>
    /// Gets the run-validation button text.
    /// </summary>
    public string ValidateText { get; }

    /// <summary>
    /// Gets the cancel button text.
    /// </summary>
    public string CancelText { get; }

    /// <summary>
    /// Gets the close button text.
    /// </summary>
    public string CloseText { get; }

    /// <summary>
    /// Gets the export button text.
    /// </summary>
    public string ExportText { get; }

    /// <summary>
    /// Gets the severity filter label.
    /// </summary>
    public string SeverityLabel { get; }

    /// <summary>
    /// Gets the free-text filter label.
    /// </summary>
    public string FilterLabel { get; }

    /// <summary>
    /// Gets the table column header.
    /// </summary>
    public string TableHeader { get; }

    /// <summary>
    /// Gets the severity column header.
    /// </summary>
    public string SeverityHeader { get; }

    /// <summary>
    /// Gets the record-key column header.
    /// </summary>
    public string RecordHeader { get; }

    /// <summary>
    /// Gets the field-path column header.
    /// </summary>
    public string FieldHeader { get; }

    /// <summary>
    /// Gets the message column header.
    /// </summary>
    public string MessageHeader { get; }

    /// <summary>
    /// Gets the value column header of the search tab.
    /// </summary>
    public string ValueHeader { get; }

    /// <summary>
    /// Gets the language column header of the search tab.
    /// </summary>
    public string LanguageHeader { get; }

    /// <summary>
    /// Gets the search term label.
    /// </summary>
    public string SearchLabel { get; }

    /// <summary>
    /// Gets the run-search button text.
    /// </summary>
    public string RunSearchText { get; }

    /// <summary>
    /// Gets the match-case check box text.
    /// </summary>
    public string MatchCaseText { get; }

    /// <summary>
    /// Gets the text shown when the validation report has no issue under the current filter.
    /// </summary>
    public string EmptyText { get; }

    /// <summary>
    /// Gets the text shown when the search found nothing.
    /// </summary>
    public string SearchEmptyText { get; }

    /// <summary>
    /// Gets the open-table button text.
    /// </summary>
    public string OpenText { get; }

    /// <summary>
    /// Gets the hint of the validation tab.
    /// </summary>
    public string ValidationHintText { get; }

    /// <summary>
    /// Gets the hint of the search tab.
    /// </summary>
    public string SearchHintText { get; }

    /// <summary>
    /// Gets a value indicating whether a validation or a search is running.
    /// </summary>
    public bool IsBusy => IsValidationBusy || IsSearchBusy;

    /// <summary>
    /// Gets the index of the tab the window opens on.
    /// </summary>
    public int InitialTabIndex => (int)_initialTab;

    /// <summary>
    /// Gets the severity filter options.
    /// </summary>
    public IReadOnlyList<DatasetSeverityOption> SeverityOptions { get; }

    /// <summary>
    /// Gets the retained issues of the validation report under the current filter.
    /// </summary>
    public ObservableCollection<DatasetIssueItem> Issues { get; } = new();

    /// <summary>
    /// Gets the retained search hits.
    /// </summary>
    public ObservableCollection<DatasetSearchHitItem> Hits { get; } = new();

    /// <summary>
    /// Loads the table list and, when the window opens on the validation tab, runs the validation.
    /// </summary>
    /// <returns>A task that completes when the initial tab has content.</returns>
    public async Task InitializeAsync()
    {
        var tables = await _session.Catalog
            .GetTablesAsync(_session.TableDataDirectory, CancellationToken.None)
            .ConfigureAwait(true);
        if (!tables.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", tables.ErrorText ?? string.Empty);
            return;
        }

        _tables = tables.Data!;
        if (_initialTab == DatasetWindowTab.Validation)
        {
            await ValidateAsync().ConfigureAwait(true);
        }
        else
        {
            StatusText = Strings.Get("DatasetSearchReady");
        }
    }

    /// <summary>
    /// Cancels a running validation or search, if any. Called when the window closes.
    /// </summary>
    public void CancelRunningOperations()
    {
        _validationCts?.Cancel();
        _searchCts?.Cancel();
    }

    /// <summary>
    /// Selects the first issue of the current filter, so the open button is usable after a run.
    /// </summary>
    private void SelectFirstIssue() => SelectedIssue = Issues.FirstOrDefault();

    /// <summary>
    /// Selects the first search hit, so the open button is usable after a search.
    /// </summary>
    private void SelectFirstHit() => SelectedHit = Hits.FirstOrDefault();

    partial void OnIssueFilterTextChanged(string value) => ApplyIssueFilter();

    partial void OnIsValidationBusyChanged(bool value) => OnPropertyChanged(nameof(IsBusy));

    partial void OnIsSearchBusyChanged(bool value) => OnPropertyChanged(nameof(IsBusy));

    partial void OnSelectedSeverityChanged(DatasetSeverityOption? value) => ApplyIssueFilter();

    [RelayCommand(CanExecute = nameof(CanValidate))]
    private async Task ValidateAsync()
    {
        IsValidationBusy = true;
        StatusText = Strings.Get("DatasetValidationRunning");
        IsValidationEmpty = false;
        _validationCts = new CancellationTokenSource();
        var progress = new Progress<DatasetScanProgress>(OnValidationProgress);
        try
        {
            _report = await _validator
                .ValidateAsync(
                    _session.TableDataDirectory,
                    _tables,
                    _session.Lookups,
                    progress: progress,
                    cancellationToken: _validationCts.Token)
                .ConfigureAwait(true);
            ApplyReport(_report);
            StatusText = ValidationSummaryText;
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("DatasetValidationCancelled");
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("DatasetValidationFailedFormat", exception.Message);
        }
        finally
        {
            _validationCts?.Dispose();
            _validationCts = null;
            IsValidationBusy = false;
            ExportCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanValidate() => !IsValidationBusy;

    [RelayCommand(CanExecute = nameof(CanCancelValidation))]
    private void CancelValidation() => _validationCts?.Cancel();

    private bool CanCancelValidation() => IsValidationBusy;

    private void OnValidationProgress(DatasetScanProgress progress)
    {
        if (progress.TableName is { } table)
        {
            StatusText = Strings.Format(
                "DatasetProgressFormat",
                progress.Completed,
                progress.Total,
                table.Value);
            return;
        }

        StatusText = Strings.Format("DatasetProgressCountFormat", progress.Completed, progress.Total);
    }

    private void ApplyReport(DatasetValidationReport report)
    {
        _allIssues = report.Tables
            .SelectMany(static table => table.Issues.Select(issue => new DatasetIssueItem(table, issue)))
            .ToArray();
        ValidationSummaryText = Strings.Format(
            "DatasetValidationDoneFormat",
            report.Tables.Count,
            report.ValidatedTableCount,
            report.ErrorCount,
            report.WarningCount,
            report.InfoCount);
        HasValidationNote = report.SuppressedIssueCount > 0;
        ValidationNoteText = HasValidationNote
            ? Strings.Format(
                "DatasetTruncatedFormat",
                _allIssues.Count,
                report.IssueCount)
            : string.Empty;
        ApplyIssueFilter();
        ExportCommand.NotifyCanExecuteChanged();
    }

    private void ApplyIssueFilter()
    {
        var severity = SelectedSeverity?.Severity;
        var term = IssueFilterText.Trim();
        Issues.Clear();
        foreach (var issue in _allIssues)
        {
            if (severity is not null && issue.Severity != severity)
            {
                continue;
            }

            if (!issue.Matches(term))
            {
                continue;
            }

            Issues.Add(issue);
        }

        IsValidationEmpty = _report is not null && Issues.Count == 0;
        SelectFirstIssue();
    }

    [RelayCommand(CanExecute = nameof(CanOpenIssue))]
    private void OpenIssue()
    {
        if (SelectedIssue is null)
        {
            return;
        }

        NavigationRequested?.Invoke(
            this,
            new DatasetNavigationRequest(SelectedIssue.Table, SelectedIssue.NavigationRecordKey));
    }

    private bool CanOpenIssue() => SelectedIssue is not null;

    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task ExportAsync()
    {
        if (_report is null)
        {
            StatusText = Strings.Get("DatasetNoReportText");
            return;
        }

        var suggestedName = "tabledata-validation-"
            + _report.GeneratedAtUtc.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)
            + ".md";
        var path = _dialogService.PickSaveFile(
            Strings.Get("DatasetExportTitle"),
            Strings.Get("DatasetExportFilter"),
            suggestedName);
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        try
        {
            var text = string.Equals(Path.GetExtension(path), ".csv", StringComparison.OrdinalIgnoreCase)
                ? DatasetValidationReportFormatter.FormatCsv(_report)
                : DatasetValidationReportFormatter.FormatMarkdown(_report);
            await File.WriteAllTextAsync(
                    path,
                    text,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                    CancellationToken.None)
                .ConfigureAwait(true);
            StatusText = Strings.Format("DatasetExportedFormat", path);
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("DatasetExportFailedFormat", exception.Message);
        }
    }

    private bool CanExport() => _report is not null && !IsValidationBusy;

    [RelayCommand(CanExecute = nameof(CanSearch))]
    private async Task SearchAsync()
    {
        var term = SearchTerm.Trim();
        if (term.Length == 0)
        {
            StatusText = Strings.Get("DatasetSearchNoTerm");
            return;
        }

        IsSearchBusy = true;
        StatusText = Strings.Get("DatasetSearchRunning");
        IsSearchEmpty = false;
        _searchCts = new CancellationTokenSource();
        var progress = new Progress<DatasetScanProgress>(OnSearchProgress);
        try
        {
            var report = await _search
                .SearchAsync(_tables, term, MatchCase, progress, _searchCts.Token)
                .ConfigureAwait(true);
            Hits.Clear();
            foreach (var hit in report.Hits)
            {
                Hits.Add(new DatasetSearchHitItem(hit));
            }

            SearchSummaryText = Strings.Format(
                "DatasetSearchDoneFormat",
                report.MatchCount,
                report.MatchedTableCount,
                report.SearchedTableCount,
                report.FailedTableCount);
            HasSearchNote = report.SuppressedHitCount > 0;
            SearchNoteText = HasSearchNote
                ? Strings.Format("DatasetTruncatedFormat", report.Hits.Count, report.MatchCount)
                : string.Empty;
            IsSearchEmpty = report.MatchCount == 0;
            SelectFirstHit();
            StatusText = SearchSummaryText;
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("DatasetSearchCancelled");
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("DatasetSearchFailedFormat", exception.Message);
        }
        finally
        {
            _searchCts?.Dispose();
            _searchCts = null;
            IsSearchBusy = false;
        }
    }

    private bool CanSearch() => !IsSearchBusy && !string.IsNullOrWhiteSpace(SearchTerm);

    [RelayCommand(CanExecute = nameof(CanCancelSearch))]
    private void CancelSearch() => _searchCts?.Cancel();

    private bool CanCancelSearch() => IsSearchBusy;

    private void OnSearchProgress(DatasetScanProgress progress)
    {
        if (progress.TableName is { } table)
        {
            StatusText = Strings.Format(
                "DatasetProgressFormat",
                progress.Completed,
                progress.Total,
                table.Value);
            return;
        }

        StatusText = Strings.Format("DatasetProgressCountFormat", progress.Completed, progress.Total);
    }

    [RelayCommand(CanExecute = nameof(CanOpenHit))]
    private void OpenHit()
    {
        if (SelectedHit is null)
        {
            return;
        }

        NavigationRequested?.Invoke(
            this,
            new DatasetNavigationRequest(SelectedHit.Table, SelectedHit.NavigationRecordKey));
    }

    private bool CanOpenHit() => SelectedHit is not null;
}
