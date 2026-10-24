using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Editable table view: a virtualized grid over <see cref="TableWorkspace"/> with schema-driven
/// columns, sparse cell editing, row operations, search, sorting, validation, and saving.
/// </summary>
public partial class TableEditorViewModel : ObservableObject, ITableCellEditor
{
    private readonly IDataSetSession _session;
    private readonly string? _languageCode;
    private readonly IDialogService _dialogService;
    private readonly List<TableRowViewModel> _allRows = new();
    private readonly List<TableColumnViewModel> _allColumns = new();
    private const int DefaultColumnBudget = 8;
    private const NumberStyles DecimalStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    [ObservableProperty]
    private TableWorkspace? _workspace;

    [ObservableProperty]
    private string _displayName;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private string _recordCountText = string.Empty;

    [ObservableProperty]
    private string _languageText = string.Empty;

    [ObservableProperty]
    private string _visibleColumnCountText = string.Empty;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _columnSearchText = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isDirty;

    [ObservableProperty]
    private bool _isEditable;

    [ObservableProperty]
    private bool _isValidationVisible;

    [ObservableProperty]
    private TableRowViewModel? _selectedRow;

    [ObservableProperty]
    private string _sortField = string.Empty;

    [ObservableProperty]
    private ListSortDirection _sortDirection = ListSortDirection.Ascending;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableEditorViewModel"/> class.
    /// </summary>
    /// <param name="session">Data-set session that opens the table.</param>
    /// <param name="descriptor">Table descriptor to edit.</param>
    /// <param name="languageCode">Optional language code for overlay editing.</param>
    /// <param name="dialogService">Dialog service used for confirmations.</param>
    public TableEditorViewModel(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode,
        IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(dialogService);

        _session = session;
        Descriptor = descriptor;
        _languageCode = languageCode;
        _dialogService = dialogService;

        Title = descriptor.TableName.Value;
        _displayName = Title;
        _statusText = Strings.Get("EditorLoading");

        AddRowText = Strings.Get("EditorAddRowText");
        DuplicateRowText = Strings.Get("EditorDuplicateRowText");
        DeleteRowText = Strings.Get("EditorDeleteRowText");
        SaveText = Strings.Get("EditorSaveText");
        ValidateText = Strings.Get("EditorValidateText");
        ReloadText = Strings.Get("EditorReloadText");
        SearchLabel = Strings.Get("EditorSearchLabel");
        ColumnsLabel = Strings.Get("EditorColumnsLabel");
        ColumnSearchLabel = Strings.Get("EditorColumnSearchLabel");
        DefaultColumnsText = Strings.Get("EditorDefaultColumnsText");
        AllColumnsText = Strings.Get("EditorAllColumnsText");
        NoColumnsText = Strings.Get("EditorNoColumnsText");
        ValidationLabel = Strings.Get("EditorValidationLabel");
    }

    /// <summary>
    /// Gets the table descriptor.
    /// </summary>
    public TableDescriptor Descriptor { get; }

    /// <summary>
    /// Gets the table title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets a value indicating whether the grid must be read-only.
    /// </summary>
    public bool IsReadOnly => !IsEditable;

    /// <summary>
    /// Gets the add-row button text.
    /// </summary>
    public string AddRowText { get; }

    /// <summary>
    /// Gets the duplicate-row button text.
    /// </summary>
    public string DuplicateRowText { get; }

    /// <summary>
    /// Gets the delete-row button text.
    /// </summary>
    public string DeleteRowText { get; }

    /// <summary>
    /// Gets the save button text.
    /// </summary>
    public string SaveText { get; }

    /// <summary>
    /// Gets the validate button text.
    /// </summary>
    public string ValidateText { get; }

    /// <summary>
    /// Gets the reload button text.
    /// </summary>
    public string ReloadText { get; }

    /// <summary>
    /// Gets the search label.
    /// </summary>
    public string SearchLabel { get; }

    /// <summary>
    /// Gets the column chooser label.
    /// </summary>
    public string ColumnsLabel { get; }

    /// <summary>
    /// Gets the column search label.
    /// </summary>
    public string ColumnSearchLabel { get; }

    /// <summary>
    /// Gets the default-columns button text.
    /// </summary>
    public string DefaultColumnsText { get; }

    /// <summary>
    /// Gets the show-all-columns button text.
    /// </summary>
    public string AllColumnsText { get; }

    /// <summary>
    /// Gets the empty-column-list text.
    /// </summary>
    public string NoColumnsText { get; }

    /// <summary>
    /// Gets the validation panel label.
    /// </summary>
    public string ValidationLabel { get; }

    /// <summary>
    /// Gets the filtered rows shown in the grid.
    /// </summary>
    public ObservableCollection<TableRowViewModel> Rows { get; } = new();

    /// <summary>
    /// Gets the columns shown in the column chooser, filtered by <see cref="ColumnSearchText"/>.
    /// </summary>
    public ObservableCollection<TableColumnViewModel> Columns { get; } = new();

    /// <summary>
    /// Gets the visible columns in schema order.
    /// </summary>
    public ObservableCollection<TableColumnViewModel> VisibleColumns { get; } = new();

    /// <summary>
    /// Gets validation issues from the last validation run.
    /// </summary>
    public ObservableCollection<ValidationIssueItem> ValidationIssues { get; } = new();

    /// <summary>
    /// Loads the workspace and builds the schema-driven grid.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>A task that completes when the table is ready.</returns>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        StatusText = Strings.Get("EditorLoading");
        try
        {
            var result = await _session
                .OpenTableAsync(Descriptor, _languageCode, cancellationToken)
                .ConfigureAwait(true);
            if (!result.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
                IsEditable = false;
                return;
            }

            Workspace = result.Data!;
            LanguageText = Workspace.Language is null
                ? Strings.Get("EditorBaseLanguage")
                : Strings.Format("EditorLanguageFormat", Workspace.Language.DisplayName);
            IsEditable = Workspace.CanEdit && Workspace.BaseDocument.Records.Count > 0;
            BuildColumns();
            RebuildRows(null);
            StatusText = IsEditable
                ? Strings.Get("EditorReady")
                : Workspace.BaseDocument.Records.Count == 0
                    ? Strings.Get("EditorReadOnlyNested")
                    : Strings.Get("EditorReadOnly");
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
            IsEditable = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Saves the base document and overlay when there are unsaved changes.
    /// </summary>
    /// <returns>A task that completes when saving finishes.</returns>
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (Workspace is null)
        {
            return;
        }

        if (!Workspace.IsDirty)
        {
            IsDirty = false;
            StatusText = Strings.Get("EditorNoChanges");
            return;
        }

        IsBusy = true;
        try
        {
            var result = await Workspace.SaveAsync().ConfigureAwait(true);
            if (result.IsSuccess)
            {
                IsDirty = false;
                StatusText = Strings.Get("EditorSaved");
            }
            else
            {
                StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            }
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
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

    /// <summary>
    /// Appends a new record with the next primary key.
    /// </summary>
    [RelayCommand]
    public void AddRow()
    {
        if (Workspace is null)
        {
            return;
        }

        var result = Workspace.AddRecord();
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        var key = result.Data!.GetKey();
        RebuildRows(key);
        StatusText = Strings.Format("EditorRowAdded", key ?? string.Empty);
    }

    /// <summary>
    /// Duplicates the selected record.
    /// </summary>
    [RelayCommand]
    public void DuplicateRow()
    {
        if (Workspace is null)
        {
            return;
        }

        if (SelectedRow?.Key is not { } key)
        {
            StatusText = Strings.Get("EditorSelectRow");
            return;
        }

        var result = Workspace.DuplicateRecord(key);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        var newKey = result.Data!.GetKey();
        RebuildRows(newKey);
        StatusText = Strings.Format("EditorRowDuplicated", key, newKey ?? string.Empty);
    }

    /// <summary>
    /// Deletes the selected record and its translation after confirmation.
    /// </summary>
    [RelayCommand]
    public void DeleteRow()
    {
        if (Workspace is null)
        {
            return;
        }

        if (SelectedRow?.Key is not { } key)
        {
            StatusText = Strings.Get("EditorSelectRow");
            return;
        }

        if (!_dialogService.Confirm(
                Strings.Format("EditorConfirmDelete", key),
                Strings.Get("EditorDeleteTitle")))
        {
            return;
        }

        var result = Workspace.DeleteRecord(key);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = Workspace.IsDirty;
        RebuildRows(null);
        StatusText = Strings.Format("EditorRowDeleted", key);
    }

    /// <summary>
    /// Runs the read-only validator and lists its issues.
    /// </summary>
    /// <returns>A task that completes when validation finishes.</returns>
    [RelayCommand]
    public async Task ValidateAsync()
    {
        if (Workspace is null)
        {
            return;
        }

        IsBusy = true;
        try
        {
            var report = await Workspace
                .ValidateAsync(_session.Lookups, Workspace.Language?.Code)
                .ConfigureAwait(true);
            ValidationIssues.Clear();
            foreach (var issue in report.Issues)
            {
                ValidationIssues.Add(new ValidationIssueItem(issue));
            }

            IsValidationVisible = true;
            StatusText = Strings.Format("EditorValidationSummary", ValidationIssues.Count);
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

    /// <summary>
    /// Discards unsaved changes after confirmation and reloads the table.
    /// </summary>
    /// <returns>A task that completes when the table is reloaded.</returns>
    [RelayCommand]
    public async Task ReloadAsync()
    {
        if (IsDirty && !_dialogService.Confirm(
                Strings.Format("EditorConfirmReload", Title),
                Strings.Get("EditorReloadTitle")))
        {
            return;
        }

        Workspace = null;
        IsDirty = false;
        _allRows.Clear();
        _allColumns.Clear();
        Rows.Clear();
        Columns.Clear();
        VisibleColumns.Clear();
        ValidationIssues.Clear();
        IsValidationVisible = false;
        await InitializeAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Restores the default visible column set.
    /// </summary>
    [RelayCommand]
    public void ShowDefaultColumns() => ApplyDefaultColumns();

    /// <summary>
    /// Shows every schema column.
    /// </summary>
    [RelayCommand]
    public void ShowAllColumns()
    {
        VisibleColumns.Clear();
        foreach (var column in _allColumns)
        {
            column.SetVisibleWithoutNotification(true);
            VisibleColumns.Add(column);
        }

        UpdateVisibleColumnCount();
    }

    /// <summary>
    /// Hides every schema column.
    /// </summary>
    [RelayCommand]
    public void HideAllColumns()
    {
        foreach (var column in _allColumns)
        {
            column.SetVisibleWithoutNotification(false);
        }

        VisibleColumns.Clear();
        UpdateVisibleColumnCount();
    }

    /// <summary>
    /// Sorts the rows by a column value.
    /// </summary>
    /// <param name="fieldName">XML field name to sort by.</param>
    /// <param name="direction">Sort direction.</param>
    public void SortBy(string fieldName, ListSortDirection direction)
    {
        SortField = fieldName;
        SortDirection = direction;
        ApplyRowView();
    }

    /// <inheritdoc />
    public OperationResult SetCellValue(TableRowViewModel row, string fieldName, string value)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(value);

        if (Workspace is null || !IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The table is not editable.");
        }

        if (row.Key is null)
        {
            StatusText = Strings.Format("StatusError", ErrorCodes.KeyNotFound);
            return OperationResult.Failure(ErrorCodes.KeyNotFound, "The record has no primary key.");
        }

        var wasAbsent = row.GetStoredValue(fieldName) is null;
        if (wasAbsent && value.Length == 0)
        {
            return OperationResult.Success();
        }

        var column = Workspace.Schema.FindColumn(fieldName);
        if (column is not null && !IsValidValue(column, value))
        {
            StatusText = Strings.Format("EditorInvalidValue", value, fieldName);
            return OperationResult.Failure(ErrorCodes.XmlInvalidNumber, StatusText);
        }

        var result = Workspace.SetValue(
            row.Key,
            fieldName,
            value,
            addIfMissing: wasAbsent);
        if (result.IsSuccess)
        {
            var refreshed = Workspace.GetRow(row.Key);
            if (refreshed is not null)
            {
                row.Refresh(refreshed);
            }

            IsDirty = Workspace.IsDirty;
            StatusText = Strings.Format("EditorCellUpdated", fieldName, row.Key);
        }
        else
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
        }

        return result;
    }

    partial void OnIsEditableChanged(bool value) => OnPropertyChanged(nameof(IsReadOnly));

    partial void OnIsDirtyChanged(bool value) =>
        DisplayName = value ? Title + " *" : Title;

    partial void OnSearchTextChanged(string value) => ApplyRowView();

    partial void OnColumnSearchTextChanged(string value) => ApplyColumnFilter();

    private void BuildColumns()
    {
        _allColumns.Clear();
        VisibleColumns.Clear();
        if (Workspace is not null)
        {
            foreach (var column in Workspace.Schema.Columns)
            {
                _allColumns.Add(new TableColumnViewModel(column, OnColumnVisibilityChanged));
            }
        }

        ApplyDefaultColumns();
        ApplyColumnFilter();
    }

    private void ApplyDefaultColumns()
    {
        VisibleColumns.Clear();
        var budget = DefaultColumnBudget;
        foreach (var column in _allColumns)
        {
            var visible = column.IsKey;
            if (!visible && budget > 0 && !column.IsHidden && !column.IsList && !column.IsDict)
            {
                visible = true;
                budget--;
            }

            column.SetVisibleWithoutNotification(visible);
            if (visible)
            {
                VisibleColumns.Add(column);
            }
        }

        UpdateVisibleColumnCount();
    }

    private void OnColumnVisibilityChanged(TableColumnViewModel column, bool visible)
    {
        if (visible)
        {
            if (!VisibleColumns.Contains(column))
            {
                var index = 0;
                while (index < VisibleColumns.Count
                       && VisibleColumns[index].Column.Order < column.Column.Order)
                {
                    index++;
                }

                VisibleColumns.Insert(index, column);
            }
        }
        else
        {
            VisibleColumns.Remove(column);
        }

        UpdateVisibleColumnCount();
    }

    private void UpdateVisibleColumnCount() =>
        VisibleColumnCountText = Strings.Format(
            "EditorColumnCount",
            VisibleColumns.Count,
            _allColumns.Count);

    private void ApplyColumnFilter()
    {
        Columns.Clear();
        var term = ColumnSearchText.Trim();
        foreach (var column in _allColumns)
        {
            if (term.Length == 0
                || column.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                || column.TypeText.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                Columns.Add(column);
            }
        }
    }

    private void RebuildRows(string? selectKey)
    {
        _allRows.Clear();
        if (Workspace is not null)
        {
            foreach (var row in Workspace.GetRows())
            {
                _allRows.Add(new TableRowViewModel(row, this));
            }
        }

        ApplyRowView(selectKey);
    }

    private void ApplyRowView(string? selectKey = null)
    {
        var selectedKey = selectKey ?? SelectedRow?.Key;
        IEnumerable<TableRowViewModel> rows = _allRows;
        var term = SearchText.Trim();
        if (term.Length > 0)
        {
            var fields = VisibleColumns.Select(static column => column.Name).ToArray();
            rows = rows.Where(row => Matches(row, term, fields));
        }

        if (SortField.Length > 0)
        {
            var column = Workspace?.Schema.FindColumn(SortField);
            rows = rows.OrderBy(
                static row => row,
                new RowValueComparer(SortField, column?.DataType, SortDirection));
        }

        Rows.Clear();
        foreach (var row in rows)
        {
            Rows.Add(row);
        }

        SelectedRow = null;
        SelectedRow = selectedKey is null
            ? Rows.FirstOrDefault()
            : Rows.FirstOrDefault(row => string.Equals(row.Key, selectedKey, StringComparison.Ordinal));
        UpdateRecordCount();
        if (term.Length > 0 && Rows.Count == 0)
        {
            StatusText = Strings.Get("EditorNoMatches");
        }
    }

    private static bool IsValidValue(TableSchemaColumn column, string value)
    {
        if (value.Length == 0)
        {
            return true;
        }

        return column.DataType switch
        {
            ColumnDataType.Integer or ColumnDataType.Lookup or ColumnDataType.Flags =>
                int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Decimal =>
                decimal.TryParse(value, DecimalStyles, CultureInfo.InvariantCulture, out _),
            ColumnDataType.Boolean => value is "0" or "1",
            _ => true,
        };
    }

    private void UpdateRecordCount() =>
        RecordCountText = Strings.Format("EditorRowCount", Rows.Count, _allRows.Count);

    private static bool Matches(
        TableRowViewModel row,
        string term,
        IReadOnlyList<string> fields)
    {
        if (row.Key?.Contains(term, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        foreach (var field in fields)
        {
            if (row[field].Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private sealed class RowValueComparer : IComparer<TableRowViewModel>
    {
        private readonly string _field;
        private readonly ColumnDataType? _dataType;
        private readonly int _direction;

        public RowValueComparer(string field, ColumnDataType? dataType, ListSortDirection direction)
        {
            _field = field;
            _dataType = dataType;
            _direction = direction == ListSortDirection.Ascending ? 1 : -1;
        }

        public int Compare(TableRowViewModel? left, TableRowViewModel? right)
        {
            if (ReferenceEquals(left, right))
            {
                return 0;
            }

            var leftValue = left?.GetStoredValue(_field);
            var rightValue = right?.GetStoredValue(_field);
            if (leftValue is null && rightValue is null)
            {
                return 0;
            }

            if (leftValue is null)
            {
                return 1;
            }

            if (rightValue is null)
            {
                return -1;
            }

            var comparison = _dataType switch
            {
                ColumnDataType.Integer or ColumnDataType.Lookup or ColumnDataType.Flags =>
                    CompareIntegers(leftValue, rightValue),
                ColumnDataType.Decimal => CompareDecimals(leftValue, rightValue),
                _ => string.Compare(leftValue, rightValue, StringComparison.OrdinalIgnoreCase),
            };

            return comparison * _direction;
        }

        private static int CompareIntegers(string left, string right)
        {
            if (int.TryParse(left, NumberStyles.Integer, CultureInfo.InvariantCulture, out var leftValue)
                && int.TryParse(right, NumberStyles.Integer, CultureInfo.InvariantCulture, out var rightValue))
            {
                return leftValue.CompareTo(rightValue);
            }

            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
        }

        private static int CompareDecimals(string left, string right)
        {
            if (decimal.TryParse(left, NumberStyles.Number, CultureInfo.InvariantCulture, out var leftValue)
                && decimal.TryParse(right, NumberStyles.Number, CultureInfo.InvariantCulture, out var rightValue))
            {
                return leftValue.CompareTo(rightValue);
            }

            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}
