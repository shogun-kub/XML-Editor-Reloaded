using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// View model of the bulk edit dialog: selects a column and operation, shows a backend-computed
/// preview, and produces the request that the table editor applies as one undoable transaction.
/// </summary>
public partial class BulkEditViewModel : ObservableObject
{
    private readonly Func<BulkEditRequest, OperationResult<BulkEditPreview>> _preview;
    private readonly IReadOnlyList<string> _recordKeys;

    [ObservableProperty]
    private BulkEditColumnOption? _selectedColumn;

    [ObservableProperty]
    private BulkOperationOption? _selectedOperation;

    [ObservableProperty]
    private string _operand = string.Empty;

    [ObservableProperty]
    private bool _includeMissingFields;

    [ObservableProperty]
    private string _summaryText;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorText = string.Empty;

    [ObservableProperty]
    private bool _canApply;

    /// <summary>
    /// Initializes a new instance of the <see cref="BulkEditViewModel"/> class.
    /// </summary>
    /// <param name="context">Dialog data supplied by the table editor.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
    public BulkEditViewModel(BulkEditContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Title = context.Title;
        Columns = context.Columns;
        _recordKeys = context.RecordKeys;
        _preview = context.Preview;
        Operations = new[]
        {
            new BulkOperationOption(BulkEditOperation.Set, Strings.Get("BulkEditOperationSet")),
            new BulkOperationOption(BulkEditOperation.Add, Strings.Get("BulkEditOperationAdd")),
            new BulkOperationOption(BulkEditOperation.Multiply, Strings.Get("BulkEditOperationMultiply")),
        };

        TargetText = Strings.Format("BulkEditTargetFormat", _recordKeys.Count);
        SummaryText = Strings.Get("BulkEditEnterValue");
        ColumnLabel = Strings.Get("BulkEditColumnLabel");
        OperationLabel = Strings.Get("BulkEditOperationLabel");
        ValueLabel = Strings.Get("BulkEditValueLabel");
        IncludeMissingText = Strings.Get("BulkEditIncludeMissingText");
        IncludeMissingTooltip = Strings.Get("BulkEditIncludeMissingTooltip");
        ApplyText = Strings.Get("BulkEditApplyText");
        CancelText = Strings.Get("BulkEditCancelText");
        KeyHeader = Strings.Get("BulkEditKeyHeader");
        OldHeader = Strings.Get("BulkEditOldHeader");
        NewHeader = Strings.Get("BulkEditNewHeader");
        StatusHeader = Strings.Get("BulkEditStatusHeader");

        SelectedColumn = Columns.FirstOrDefault();
        SelectedOperation = Operations[0];
    }

    /// <summary>
    /// Gets the dialog title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the columns that can be edited in bulk.
    /// </summary>
    public IReadOnlyList<BulkEditColumnOption> Columns { get; }

    /// <summary>
    /// Gets the available bulk operations.
    /// </summary>
    public IReadOnlyList<BulkOperationOption> Operations { get; }

    /// <summary>
    /// Gets the preview rows that will change or fail. Unchanged rows are omitted.
    /// </summary>
    public ObservableCollection<BulkEditPreviewRowViewModel> PreviewRows { get; } = new();

    /// <summary>
    /// Gets the text describing how many rows the operation targets.
    /// </summary>
    public string TargetText { get; }

    /// <summary>
    /// Gets the column selector label.
    /// </summary>
    public string ColumnLabel { get; }

    /// <summary>
    /// Gets the operation selector label.
    /// </summary>
    public string OperationLabel { get; }

    /// <summary>
    /// Gets the operand label.
    /// </summary>
    public string ValueLabel { get; }

    /// <summary>
    /// Gets the include-missing checkbox label.
    /// </summary>
    public string IncludeMissingText { get; }

    /// <summary>
    /// Gets the include-missing checkbox tooltip.
    /// </summary>
    public string IncludeMissingTooltip { get; }

    /// <summary>
    /// Gets the apply button text.
    /// </summary>
    public string ApplyText { get; }

    /// <summary>
    /// Gets the cancel button text.
    /// </summary>
    public string CancelText { get; }

    /// <summary>
    /// Gets the record key column header.
    /// </summary>
    public string KeyHeader { get; }

    /// <summary>
    /// Gets the current value column header.
    /// </summary>
    public string OldHeader { get; }

    /// <summary>
    /// Gets the new value column header.
    /// </summary>
    public string NewHeader { get; }

    /// <summary>
    /// Gets the status column header.
    /// </summary>
    public string StatusHeader { get; }

    /// <summary>
    /// Gets a value indicating whether the current preview is blocked by an error.
    /// </summary>
    public bool HasError => ErrorText.Length > 0;

    /// <summary>
    /// Creates the request to apply when the preview is valid.
    /// </summary>
    /// <returns>The request, or <see langword="null"/> when the preview cannot be applied.</returns>
    public BulkEditRequest? CreateRequest() =>
        CanApply && SelectedColumn is not null && SelectedOperation is not null
            ? new BulkEditRequest(
                SelectedColumn.FieldName,
                SelectedOperation.Kind,
                Operand,
                IncludeMissingFields,
                _recordKeys)
            : null;

    partial void OnSelectedColumnChanged(BulkEditColumnOption? value)
    {
        if (value is { SupportsArithmetic: false }
            && SelectedOperation is { Kind: not BulkEditOperation.Set })
        {
            SelectedOperation = Operations[0];
            return;
        }

        RefreshPreview();
    }

    partial void OnSelectedOperationChanged(BulkOperationOption? value) => RefreshPreview();

    partial void OnOperandChanged(string value) => RefreshPreview();

    partial void OnIncludeMissingFieldsChanged(bool value) => RefreshPreview();

    private void RefreshPreview()
    {
        PreviewRows.Clear();
        if (SelectedColumn is null || SelectedOperation is null || Operand.Length == 0)
        {
            SummaryText = Strings.Get("BulkEditEnterValue");
            ErrorText = string.Empty;
            CanApply = false;
            return;
        }

        var request = new BulkEditRequest(
            SelectedColumn.FieldName,
            SelectedOperation.Kind,
            Operand,
            IncludeMissingFields,
            _recordKeys);
        var result = _preview(request);
        if (!result.IsSuccess)
        {
            SummaryText = Strings.Get("BulkEditNoChanges");
            ErrorText = $"{result.ErrorCode}: {result.ErrorText}";
            CanApply = false;
            return;
        }

        var preview = result.Data!;
        foreach (var row in preview.Rows.Where(static row =>
                     row.Status is BulkEditRowStatus.Changed or BulkEditRowStatus.Error))
        {
            PreviewRows.Add(new BulkEditPreviewRowViewModel(row));
        }

        SummaryText = Strings.Format(
            "BulkEditSummaryFormat",
            preview.Rows.Count,
            preview.ChangedCount,
            preview.UnchangedCount,
            preview.SkippedCount,
            preview.ErrorCount);
        ErrorText = string.Empty;
        CanApply = preview.CanApply;
    }
}

/// <summary>
/// One bulk operation offered by the dialog.
/// </summary>
public sealed class BulkOperationOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkOperationOption"/> class.
    /// </summary>
    /// <param name="kind">Operation kind.</param>
    /// <param name="displayText">Localized display text.</param>
    public BulkOperationOption(BulkEditOperation kind, string displayText)
    {
        Kind = kind;
        DisplayText = displayText;
    }

    /// <summary>
    /// Gets the operation kind.
    /// </summary>
    public BulkEditOperation Kind { get; }

    /// <summary>
    /// Gets the localized display text.
    /// </summary>
    public string DisplayText { get; }
}

/// <summary>
/// One row of the bulk preview grid.
/// </summary>
public sealed class BulkEditPreviewRowViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BulkEditPreviewRowViewModel"/> class.
    /// </summary>
    /// <param name="row">Backend preview row.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="row"/> is null.</exception>
    public BulkEditPreviewRowViewModel(BulkEditPreviewRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        var absent = Strings.Get("BulkEditAbsentValue");
        RecordKey = row.RecordKey;
        OldValue = row.OldValue ?? absent;
        NewValue = row.NewValue ?? absent;
        StatusText = row.Status == BulkEditRowStatus.Error
            ? row.ErrorText ?? Strings.Get("BulkEditStatusError")
            : row.Status switch
            {
                BulkEditRowStatus.Changed => Strings.Get("BulkEditStatusChanged"),
                BulkEditRowStatus.SkippedMissing => Strings.Get("BulkEditStatusSkipped"),
                BulkEditRowStatus.Unchanged => Strings.Get("BulkEditStatusUnchanged"),
                _ => Strings.Get("BulkEditStatusError"),
            };
    }

    /// <summary>
    /// Gets the record key.
    /// </summary>
    public string RecordKey { get; }

    /// <summary>
    /// Gets the current value, or the absent-value marker.
    /// </summary>
    public string OldValue { get; }

    /// <summary>
    /// Gets the computed value, or the absent-value marker.
    /// </summary>
    public string NewValue { get; }

    /// <summary>
    /// Gets the localized status or error detail.
    /// </summary>
    public string StatusText { get; }
}
