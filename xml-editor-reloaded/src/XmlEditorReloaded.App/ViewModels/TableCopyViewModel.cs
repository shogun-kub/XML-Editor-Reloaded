using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Copy;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// View model of the cross-data-set copy dialog. Every write is preceded by an explicit preview; the
/// source data set is only read.
/// </summary>
public sealed partial class TableCopyViewModel : ObservableObject
{
    private readonly ITableCopyService _copyService;
    private readonly ITableCatalog _catalog;
    private readonly IDialogService _dialogService;
    private readonly string? _initialTableName;
    private TableCopyPreview? _preview;
    private int _tableLoadGeneration;
    private bool _initialized;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviewCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviewCommand))]
    private string _sourceDirectory;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviewCommand))]
    private string _targetDirectory;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviewCommand))]
    private TableCopyTableItem? _selectedTable;

    [ObservableProperty]
    private string _recordKeysText;

    [ObservableProperty]
    private TableCopyPolicyOption? _selectedConflictPolicy;

    [ObservableProperty]
    private TableCopyKeyModeOption? _selectedKeyMode;

    [ObservableProperty]
    private bool _copyRelatedRecords;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviewCommand))]
    private bool _isTablesLoading;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private string _summaryText = string.Empty;

    [ObservableProperty]
    private bool _hasWarnings;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableCopyViewModel"/> class.
    /// </summary>
    /// <param name="copyService">Copy service shared with the main window.</param>
    /// <param name="catalog">Catalog used to list tables common to both data sets.</param>
    /// <param name="dialogService">Dialog service used to add a target directory.</param>
    /// <param name="request">Initial dialog state.</param>
    /// <exception cref="ArgumentNullException">Thrown when a dependency or the request is null.</exception>
    public TableCopyViewModel(
        ITableCopyService copyService,
        ITableCatalog catalog,
        IDialogService dialogService,
        TableCopyDialogRequest request)
    {
        ArgumentNullException.ThrowIfNull(copyService);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(request);

        _copyService = copyService;
        _catalog = catalog;
        _dialogService = dialogService;
        _initialTableName = request.InitialTableName;

        var source = Normalize(request.InitialSourceDirectory);
        var directories = new List<string>();
        void AddDirectory(string path)
        {
            if (!string.IsNullOrWhiteSpace(path)
                && !directories.Any(existing => SamePath(existing, path)))
            {
                directories.Add(path);
            }
        }

        AddDirectory(source);
        foreach (var directory in request.DataDirectories)
        {
            AddDirectory(Normalize(directory));
        }

        if (directories.Count == 0)
        {
            directories.Add(source);
        }

        Directories = new ObservableCollection<string>(directories);
        _sourceDirectory = source;
        _targetDirectory = directories.FirstOrDefault(path => !SamePath(path, source)) ?? source;
        _recordKeysText = request.InitialRecordKeys is { Count: > 0 } keys
            ? string.Join(", ", keys)
            : string.Empty;
        _statusText = Strings.Get("CopyReady");

        Title = Strings.Get("CopyTitle");
        SourceLabel = Strings.Get("CopySourceLabel");
        TargetLabel = Strings.Get("CopyTargetLabel");
        BrowseText = Strings.Get("CopyBrowseText");
        TableLabel = Strings.Get("CopyTableLabel");
        RecordsLabel = Strings.Get("CopyRecordsLabel");
        RecordsHint = Strings.Get("CopyRecordsHint");
        ConflictLabel = Strings.Get("CopyConflictLabel");
        KeyModeLabel = Strings.Get("CopyKeyModeLabel");
        RelatedLabel = Strings.Get("CopyRelatedLabel");
        PreviewText = Strings.Get("CopyPreviewButton");
        ApplyText = Strings.Get("CopyApplyButton");
        CloseText = Strings.Get("CopyCloseButton");
        PreviewHeader = Strings.Get("CopyPreviewHeader");
        WarningsHeader = Strings.Get("CopyWarningsHeader");
        SourceKeyHeader = Strings.Get("CopySourceKeyColumn");
        SourceDisplayHeader = Strings.Get("CopySourceDisplayColumn");
        TargetKeyHeader = Strings.Get("CopyTargetKeyColumn");
        ActionHeader = Strings.Get("CopyActionColumn");
        ReferencesHeader = Strings.Get("CopyReferencesColumn");
        RelatedHeader = Strings.Get("CopyRelatedColumn");
        WarningsColumnHeader = Strings.Get("CopyWarningsColumn");

        ConflictPolicies = new[]
        {
            new TableCopyPolicyOption(Strings.Get("CopyPolicySkip"), TableCopyConflictPolicy.Skip),
            new TableCopyPolicyOption(Strings.Get("CopyPolicyOverwrite"), TableCopyConflictPolicy.Overwrite),
            new TableCopyPolicyOption(Strings.Get("CopyPolicyMerge"), TableCopyConflictPolicy.Merge),
        };
        KeyModes = new[]
        {
            new TableCopyKeyModeOption(Strings.Get("CopyKeyPreserve"), TableCopyKeyMode.Preserve),
            new TableCopyKeyModeOption(Strings.Get("CopyKeyAllocate"), TableCopyKeyMode.Allocate),
        };
        _selectedConflictPolicy = ConflictPolicies[0];
        _selectedKeyMode = KeyModes[0];
    }

    /// <summary>
    /// Occurs after the copy has been committed, so the window can close with a positive result.
    /// </summary>
    public event EventHandler? Completed;

    /// <summary>
    /// Gets the dialog title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the source directory label.
    /// </summary>
    public string SourceLabel { get; }

    /// <summary>
    /// Gets the target directory label.
    /// </summary>
    public string TargetLabel { get; }

    /// <summary>
    /// Gets the browse button text.
    /// </summary>
    public string BrowseText { get; }

    /// <summary>
    /// Gets the table label.
    /// </summary>
    public string TableLabel { get; }

    /// <summary>
    /// Gets the record filter label.
    /// </summary>
    public string RecordsLabel { get; }

    /// <summary>
    /// Gets the record filter hint.
    /// </summary>
    public string RecordsHint { get; }

    /// <summary>
    /// Gets the conflict policy label.
    /// </summary>
    public string ConflictLabel { get; }

    /// <summary>
    /// Gets the key mode label.
    /// </summary>
    public string KeyModeLabel { get; }

    /// <summary>
    /// Gets the related-records checkbox label.
    /// </summary>
    public string RelatedLabel { get; }

    /// <summary>
    /// Gets the preview button text.
    /// </summary>
    public string PreviewText { get; }

    /// <summary>
    /// Gets the copy button text.
    /// </summary>
    public string ApplyText { get; }

    /// <summary>
    /// Gets the close button text.
    /// </summary>
    public string CloseText { get; }

    /// <summary>
    /// Gets the preview section header.
    /// </summary>
    public string PreviewHeader { get; }

    /// <summary>
    /// Gets the warnings section header.
    /// </summary>
    public string WarningsHeader { get; }

    /// <summary>
    /// Gets the source key column header.
    /// </summary>
    public string SourceKeyHeader { get; }

    /// <summary>
    /// Gets the source display column header.
    /// </summary>
    public string SourceDisplayHeader { get; }

    /// <summary>
    /// Gets the target key column header.
    /// </summary>
    public string TargetKeyHeader { get; }

    /// <summary>
    /// Gets the action column header.
    /// </summary>
    public string ActionHeader { get; }

    /// <summary>
    /// Gets the references column header.
    /// </summary>
    public string ReferencesHeader { get; }

    /// <summary>
    /// Gets the related rows column header.
    /// </summary>
    public string RelatedHeader { get; }

    /// <summary>
    /// Gets the warnings column header.
    /// </summary>
    public string WarningsColumnHeader { get; }

    /// <summary>
    /// Gets the data directories offered as source and target.
    /// </summary>
    public ObservableCollection<string> Directories { get; }

    /// <summary>
    /// Gets the tables present in both data sets.
    /// </summary>
    public ObservableCollection<TableCopyTableItem> Tables { get; } = new();

    /// <summary>
    /// Gets the selectable conflict policies.
    /// </summary>
    public IReadOnlyList<TableCopyPolicyOption> ConflictPolicies { get; }

    /// <summary>
    /// Gets the selectable key modes.
    /// </summary>
    public IReadOnlyList<TableCopyKeyModeOption> KeyModes { get; }

    /// <summary>
    /// Gets the per-record preview rows.
    /// </summary>
    public ObservableCollection<TableCopyPreviewRow> PreviewRows { get; } = new();

    /// <summary>
    /// Gets the plan warnings.
    /// </summary>
    public ObservableCollection<string> Warnings { get; } = new();

    /// <summary>
    /// Gets a value indicating whether a preview has been computed.
    /// </summary>
    public bool HasPreview => _preview is not null;

    /// <summary>
    /// Gets the copy result after the dialog has been accepted, or <see langword="null"/>.
    /// </summary>
    public TableCopyDialogOutcome? Outcome { get; private set; }

    /// <summary>
    /// Loads the tables common to the initial source and target directories.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the catalog reads.</param>
    /// <returns>A task that completes when the table list is ready.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        await LoadTablesAsync(cancellationToken).ConfigureAwait(true);
    }

    private async Task LoadTablesAsync(CancellationToken cancellationToken = default)
    {
        var generation = ++_tableLoadGeneration;
        var source = SourceDirectory;
        var target = TargetDirectory;
        var previous = SelectedTable?.TableName;

        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(target))
        {
            Tables.Clear();
            SelectedTable = null;
            IsTablesLoading = false;
            return;
        }

        StatusText = Strings.Get("CopyLoadingTables");
        IsTablesLoading = true;
        try
        {
            var sourceTables = await _catalog.GetTablesAsync(source, cancellationToken).ConfigureAwait(true);
            var targetTables = await _catalog.GetTablesAsync(target, cancellationToken).ConfigureAwait(true);
            if (generation != _tableLoadGeneration)
            {
                return;
            }

            if (!sourceTables.IsSuccess || !targetTables.IsSuccess)
            {
                Tables.Clear();
                SelectedTable = null;
                StatusText = Strings.Format(
                    "StatusError",
                    (sourceTables.ErrorText ?? targetTables.ErrorText) ?? string.Empty);
                return;
            }

            var targetNames = targetTables.Data!
                .Select(static descriptor => descriptor.TableName)
                .ToHashSet();
            var common = sourceTables.Data!
                .Where(descriptor => targetNames.Contains(descriptor.TableName))
                .OrderBy(static descriptor => descriptor.TableName)
                .Select(static descriptor => new TableCopyTableItem(descriptor.TableName))
                .ToArray();

            Tables.Clear();
            foreach (var item in common)
            {
                Tables.Add(item);
            }

            var wanted = previous ?? (_initialTableName is null
                ? null
                : TableName.Parse(_initialTableName));
            SelectedTable = wanted is null
                ? Tables.FirstOrDefault()
                : Tables.FirstOrDefault(item => item.TableName == wanted) ?? Tables.FirstOrDefault();

            StatusText = Tables.Count == 0
                ? Strings.Get("CopyNoCommonTables")
                : Strings.Format("CopyTablesLoaded", Tables.Count);
        }
        catch (OperationCanceledException)
        {
            // A newer directory change replaced this load.
        }
        catch (Exception exception)
        {
            if (generation == _tableLoadGeneration)
            {
                Tables.Clear();
                SelectedTable = null;
                StatusText = Strings.Format("StatusError", exception.Message);
            }
        }
        finally
        {
            if (generation == _tableLoadGeneration)
            {
                IsTablesLoading = false;
            }
        }
    }

    private bool CanPreview() =>
        !IsBusy
        && !IsTablesLoading
        && !string.IsNullOrWhiteSpace(SourceDirectory)
        && !string.IsNullOrWhiteSpace(TargetDirectory)
        && SelectedTable is not null;

    [RelayCommand(CanExecute = nameof(CanPreview))]
    private async Task PreviewAsync()
    {
        var request = CreateRequest();
        if (request is null)
        {
            return;
        }

        IsBusy = true;
        StatusText = Strings.Get("CopyPreviewing");
        try
        {
            var result = await _copyService.PreviewAsync(request).ConfigureAwait(true);
            if (!result.IsSuccess)
            {
                ClearPreview();
                StatusText = Strings.Format("StatusError", result.ErrorText ?? string.Empty);
                return;
            }

            ApplyPreview(result.Data!);
        }
        catch (Exception exception)
        {
            ClearPreview();
            StatusText = Strings.Format("StatusError", exception.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanApply() => !IsBusy && _preview is { HasChanges: true };

    [RelayCommand(CanExecute = nameof(CanApply))]
    private async Task ApplyAsync()
    {
        var request = CreateRequest();
        if (request is null)
        {
            return;
        }

        IsBusy = true;
        StatusText = Strings.Get("CopyApplying");
        try
        {
            var result = await _copyService.ApplyAsync(request).ConfigureAwait(true);
            if (!result.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", result.ErrorText ?? string.Empty);
                return;
            }

            var applied = result.Data!;
            var added = applied.Tables.Sum(static table => table.Added);
            var updated = applied.Tables.Sum(static table => table.Updated);
            var skipped = applied.Tables.Sum(static table => table.Skipped);
            StatusText = Strings.Format("CopyResultSummary", added, updated, skipped);
            Outcome = new TableCopyDialogOutcome(request.TargetTableDataDirectory, applied);
            Completed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void BrowseSource() => Browse(isTarget: false);

    [RelayCommand]
    private void BrowseTarget() => Browse(isTarget: true);

    private void Browse(bool isTarget)
    {
        var current = isTarget ? TargetDirectory : SourceDirectory;
        var folder = _dialogService.PickFolder(
            Strings.Get(isTarget ? "CopyChooseTarget" : "CopyChooseSource"),
            string.IsNullOrWhiteSpace(current) ? null : current);
        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        var resolved = Normalize(folder);
        if (!Directories.Any(path => SamePath(path, resolved)))
        {
            Directories.Add(resolved);
        }

        if (isTarget)
        {
            TargetDirectory = resolved;
        }
        else
        {
            SourceDirectory = resolved;
        }
    }

    private TableCopyRequest? CreateRequest()
    {
        if (SelectedTable is null)
        {
            StatusText = Strings.Get("CopyNoTable");
            return null;
        }

        var keys = ParseRecordKeys();
        return new TableCopyRequest(
            SourceDirectory.Trim(),
            TargetDirectory.Trim(),
            SelectedTable.TableName,
            keys,
            SelectedConflictPolicy?.Policy ?? TableCopyConflictPolicy.Skip,
            SelectedKeyMode?.Mode ?? TableCopyKeyMode.Preserve,
            CopyRelatedRecords);
    }

    private IReadOnlyList<string>? ParseRecordKeys()
    {
        var text = RecordKeysText.Trim();
        if (text.Length == 0)
        {
            return null;
        }

        return text
            .Split(new[] { ',', ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private void ApplyPreview(TableCopyPreview preview)
    {
        _preview = preview;
        PreviewRows.Clear();
        foreach (var record in preview.Records)
        {
            PreviewRows.Add(new TableCopyPreviewRow(
                record.SourceKey,
                record.SourceDisplay,
                record.TargetKey,
                DescribeAction(record.Action),
                DescribeRemaps(record.Remaps),
                DescribeRelated(record.Related),
                string.Join(" ", record.Warnings)));
        }

        Warnings.Clear();
        foreach (var warning in preview.Warnings)
        {
            Warnings.Add(warning);
        }

        HasWarnings = Warnings.Count > 0;
        SummaryText = Strings.Format(
            "CopyPreviewSummary",
            preview.AddCount,
            preview.OverwriteCount,
            preview.MergeCount,
            preview.SkipCount,
            preview.RelatedRecordCount);
        StatusText = SummaryText;
        OnPropertyChanged(nameof(HasPreview));
        ApplyCommand.NotifyCanExecuteChanged();
    }

    private void ClearPreview()
    {
        _preview = null;
        PreviewRows.Clear();
        Warnings.Clear();
        HasWarnings = false;
        SummaryText = string.Empty;
        OnPropertyChanged(nameof(HasPreview));
        ApplyCommand.NotifyCanExecuteChanged();
    }

    private static string DescribeAction(TableCopyAction action) => action switch
    {
        TableCopyAction.Add => Strings.Get("CopyActionAdd"),
        TableCopyAction.Overwrite => Strings.Get("CopyActionOverwrite"),
        TableCopyAction.Merge => Strings.Get("CopyActionMerge"),
        _ => Strings.Get("CopyActionSkip"),
    };

    private static string DescribeRemaps(IReadOnlyList<TableCopyRemap> remaps)
    {
        if (remaps.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(
            "; ",
            remaps.Select(remap => remap.Status switch
            {
                TableCopyRemapStatus.Remapped => $"{remap.Field}: {remap.SourceValue} → {remap.TargetValue}",
                TableCopyRemapStatus.Ambiguous =>
                    $"{remap.Field}: {remap.SourceValue} ({Strings.Get("CopyRemapAmbiguous")})",
                _ => $"{remap.Field}: {remap.SourceValue} ({Strings.Get("CopyRemapNotFound")})",
            }));
    }

    private static string DescribeRelated(IReadOnlyList<TableCopyRelatedPreview> related)
    {
        var rows = related
            .Where(static item => item.RecordCount > 0)
            .Select(item => $"{item.Table.Value}: +{item.AddCount} ~{item.UpdateCount} / {item.SkippedCount}")
            .ToArray();
        return rows.Length == 0 ? string.Empty : string.Join("; ", rows);
    }

    partial void OnSourceDirectoryChanged(string value)
    {
        ClearPreview();
        _ = LoadTablesAsync();
    }

    partial void OnTargetDirectoryChanged(string value)
    {
        ClearPreview();
        _ = LoadTablesAsync();
    }

    partial void OnSelectedTableChanged(TableCopyTableItem? value) => ClearPreview();

    partial void OnRecordKeysTextChanged(string value) => ClearPreview();

    partial void OnSelectedConflictPolicyChanged(TableCopyPolicyOption? value) => ClearPreview();

    partial void OnSelectedKeyModeChanged(TableCopyKeyModeOption? value) => ClearPreview();

    partial void OnCopyRelatedRecordsChanged(bool value) => ClearPreview();

    private static string Normalize(string path) =>
        DataSetPaths.ResolveTableDataDirectory(path) ?? path;

    private static bool SamePath(string? first, string? second)
    {
        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second))
        {
            return false;
        }

        return string.Equals(
            Path.TrimEndingDirectorySeparator(first),
            Path.TrimEndingDirectorySeparator(second),
            StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// One table offered by the copy dialog.
/// </summary>
/// <param name="TableName">Dotted table name.</param>
public sealed record TableCopyTableItem(TableName TableName)
{
    /// <summary>
    /// Gets the text shown in the table selector.
    /// </summary>
    public string DisplayName => TableName.Value;

    /// <inheritdoc />
    public override string ToString() => DisplayName;
}

/// <summary>
/// One selectable conflict policy with a localized label.
/// </summary>
/// <param name="Label">Text shown in the selector.</param>
/// <param name="Policy">Policy applied when the target already contains the key.</param>
public sealed record TableCopyPolicyOption(string Label, TableCopyConflictPolicy Policy)
{
    /// <inheritdoc />
    public override string ToString() => Label;
}

/// <summary>
/// One selectable key mode with a localized label.
/// </summary>
/// <param name="Label">Text shown in the selector.</param>
/// <param name="Mode">Key mode applied to copied records.</param>
public sealed record TableCopyKeyModeOption(string Label, TableCopyKeyMode Mode)
{
    /// <inheritdoc />
    public override string ToString() => Label;
}

/// <summary>
/// One row of the copy preview grid.
/// </summary>
public sealed class TableCopyPreviewRow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableCopyPreviewRow"/> class.
    /// </summary>
    /// <param name="sourceKey">Primary key in the source table.</param>
    /// <param name="sourceDisplay">Display text of the source record, or <see langword="null"/>.</param>
    /// <param name="targetKey">Key the record will have in the target, or <see langword="null"/>.</param>
    /// <param name="actionText">Localized planned action.</param>
    /// <param name="remapText">Reference rewrites, or an empty string.</param>
    /// <param name="relatedText">Related-row plan, or an empty string.</param>
    /// <param name="warningText">Record warnings, or an empty string.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public TableCopyPreviewRow(
        string sourceKey,
        string? sourceDisplay,
        string? targetKey,
        string actionText,
        string remapText,
        string relatedText,
        string warningText)
    {
        ArgumentNullException.ThrowIfNull(sourceKey);
        ArgumentNullException.ThrowIfNull(actionText);
        ArgumentNullException.ThrowIfNull(remapText);
        ArgumentNullException.ThrowIfNull(relatedText);
        ArgumentNullException.ThrowIfNull(warningText);

        SourceKey = sourceKey;
        SourceDisplay = sourceDisplay ?? string.Empty;
        TargetKey = targetKey ?? string.Empty;
        ActionText = actionText;
        RemapText = remapText;
        RelatedText = relatedText;
        WarningText = warningText;
    }

    /// <summary>
    /// Gets the primary key in the source table.
    /// </summary>
    public string SourceKey { get; }

    /// <summary>
    /// Gets the display text of the source record.
    /// </summary>
    public string SourceDisplay { get; }

    /// <summary>
    /// Gets the key the record will have in the target.
    /// </summary>
    public string TargetKey { get; }

    /// <summary>
    /// Gets the localized planned action.
    /// </summary>
    public string ActionText { get; }

    /// <summary>
    /// Gets the reference rewrites.
    /// </summary>
    public string RemapText { get; }

    /// <summary>
    /// Gets the related-row plan.
    /// </summary>
    public string RelatedText { get; }

    /// <summary>
    /// Gets the record warnings.
    /// </summary>
    public string WarningText { get; }
}
