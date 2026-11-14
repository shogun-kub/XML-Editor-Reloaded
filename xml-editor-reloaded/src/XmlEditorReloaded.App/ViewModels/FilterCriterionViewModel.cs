using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Filtering;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One operator offered by a filter criterion, with its localized display text.
/// </summary>
public sealed class FilterOperatorOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterOperatorOption"/> class.
    /// </summary>
    /// <param name="operator">Filter operator.</param>
    /// <param name="displayText">Localized operator text.</param>
    public FilterOperatorOption(RowFilterOperator @operator, string displayText)
    {
        Operator = @operator;
        DisplayText = displayText;
    }

    /// <summary>
    /// Gets the filter operator.
    /// </summary>
    public RowFilterOperator Operator { get; }

    /// <summary>
    /// Gets the localized operator text.
    /// </summary>
    public string DisplayText { get; }
}

/// <summary>
/// One AND/OR option of a filter group, with its localized display text.
/// </summary>
public sealed class FilterLogicalOperatorOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterLogicalOperatorOption"/> class.
    /// </summary>
    /// <param name="operator">Logical operator.</param>
    /// <param name="displayText">Localized operator text.</param>
    public FilterLogicalOperatorOption(RowFilterLogicalOperator @operator, string displayText)
    {
        Operator = @operator;
        DisplayText = displayText;
    }

    /// <summary>
    /// Gets the logical operator.
    /// </summary>
    public RowFilterLogicalOperator Operator { get; }

    /// <summary>
    /// Gets the localized operator text.
    /// </summary>
    public string DisplayText { get; }
}

/// <summary>
/// One editable row of the structured grid filter: field, operator, and operand.
/// </summary>
public sealed partial class FilterCriterionViewModel : ObservableObject
{
    private readonly Action _changed;
    private readonly Action<FilterCriterionViewModel> _remove;
    private bool _isLoading;
    private bool _isOperandEditing;
    private IReadOnlyList<AllowedValueOption> _allowedValues = Array.Empty<AllowedValueOption>();

    [ObservableProperty]
    private TableColumnViewModel? _selectedColumn;

    [ObservableProperty]
    private FilterOperatorOption? _selectedOperator;

    [ObservableProperty]
    private string _operand = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterCriterionViewModel"/> class.
    /// </summary>
    /// <param name="columns">Columns the criterion can filter by.</param>
    /// <param name="changed">Callback invoked when a user changes the criterion.</param>
    /// <param name="remove">Callback that removes the criterion from its parent group.</param>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    public FilterCriterionViewModel(
        IReadOnlyList<TableColumnViewModel> columns,
        Action changed,
        Action<FilterCriterionViewModel> remove)
    {
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(changed);
        ArgumentNullException.ThrowIfNull(remove);

        Columns = columns;
        _changed = changed;
        _remove = remove;
        _selectedColumn = columns.Count > 0 ? columns[0] : null;
        RebuildOperators();
        RebuildAllowedValues();
    }

    /// <summary>
    /// Gets the columns the criterion can filter by.
    /// </summary>
    public IReadOnlyList<TableColumnViewModel> Columns { get; }

    /// <summary>
    /// Gets the operators supported by the selected column.
    /// </summary>
    public ObservableCollection<FilterOperatorOption> Operators { get; } = new();

    /// <summary>
    /// Gets the values offered by the operand field: relation, lookup, or Boolean choices of the
    /// selected column. An empty list keeps free text input.
    /// </summary>
    public IReadOnlyList<AllowedValueOption> AllowedValues
    {
        get => _allowedValues;
        private set
        {
            _allowedValues = value;
            OnPropertyChanged(nameof(AllowedValues));
            OnPropertyChanged(nameof(HasAllowedValues));
        }
    }

    /// <summary>
    /// Gets a value indicating whether the operand field offers a value dropdown.
    /// </summary>
    public bool HasAllowedValues => _allowedValues.Count > 0;

    /// <summary>
    /// Gets or sets the text shown by the operand field. While the field is being edited it shows the
    /// raw value, because the dropdown and free input write raw identifiers; when editing ends it
    /// shows the semantic name of a known lookup, Boolean, or relation value. Unknown raw values and
    /// plain text columns are shown unchanged.
    /// </summary>
    public string OperandText
    {
        get => _isOperandEditing ? Operand : AllowedValueOption.GetDisplay(Operand, _allowedValues);
        set
        {
            // WPF can echo the displayed text back into the source after a selection is normalized;
            // that is not a user edit, so it must not turn the raw identifier back on.
            if (!_isOperandEditing
                && string.Equals(
                    value,
                    AllowedValueOption.GetDisplay(Operand, _allowedValues),
                    StringComparison.Ordinal))
            {
                return;
            }

            _isOperandEditing = true;
            var resolved = AllowedValueOption.Resolve(value, _allowedValues);
            if (!string.Equals(Operand, resolved, StringComparison.Ordinal))
            {
                Operand = resolved;
            }
        }
    }

    /// <summary>
    /// Switches the operand field to raw editing: a selected lookup or relation name is replaced by
    /// the identifier the dropdown writes back, and free text stays untouched.
    /// </summary>
    public void BeginOperandEdit()
    {
        _isOperandEditing = true;
        OnPropertyChanged(nameof(OperandText));
    }

    /// <summary>
    /// Leaves operand editing and shows the semantic name of the current value when one is known.
    /// </summary>
    public void EndOperandEdit()
    {
        _isOperandEditing = false;
        OnPropertyChanged(nameof(OperandText));
    }

    /// <summary>
    /// Removes this criterion from its parent group.
    /// </summary>
    [RelayCommand]
    private void Remove() => _remove(this);

    /// <summary>
    /// Builds the criterion model, or returns <see langword="null"/> when the row is incomplete.
    /// A blank operand is not applied, so adding a condition does not empty the grid before the user
    /// enters a value.
    /// </summary>
    /// <returns>The criterion, or <see langword="null"/>.</returns>
    public RowFilterCriterion? ToCriterion() =>
        SelectedColumn is null || SelectedOperator is null || Operand.Length == 0
            ? null
            : new RowFilterCriterion(SelectedColumn.Name, SelectedOperator.Operator, Operand);

    /// <summary>
    /// Fills the row from a stored criterion without invoking change callbacks.
    /// </summary>
    /// <param name="criterion">Stored criterion to apply.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="criterion"/> is null.</exception>
    public void Load(RowFilterCriterion criterion)
    {
        ArgumentNullException.ThrowIfNull(criterion);

        _isLoading = true;
        try
        {
            SelectedColumn = Columns.FirstOrDefault(column =>
                string.Equals(column.Name, criterion.Field, StringComparison.Ordinal));
            RebuildOperators();
            SelectedOperator = Operators.FirstOrDefault(option => option.Operator == criterion.Operator)
                ?? Operators.FirstOrDefault();
            Operand = criterion.Operand;
        }
        finally
        {
            _isLoading = false;
        }
    }

    partial void OnSelectedColumnChanged(TableColumnViewModel? value)
    {
        RebuildOperators();
        RebuildAllowedValues();
        if (!_isLoading)
        {
            _changed();
        }
    }

    partial void OnSelectedOperatorChanged(FilterOperatorOption? value)
    {
        if (!_isLoading)
        {
            _changed();
        }
    }

    partial void OnOperandChanged(string value)
    {
        if (!_isLoading)
        {
            _changed();
        }

        if (!_isOperandEditing)
        {
            OnPropertyChanged(nameof(OperandText));
        }
    }

    private void RebuildAllowedValues()
    {
        AllowedValues = SelectedColumn?.AllowedValues ?? Array.Empty<AllowedValueOption>();
        if (!_isOperandEditing)
        {
            OnPropertyChanged(nameof(OperandText));
        }
    }

    private void RebuildOperators()
    {
        var previous = SelectedOperator?.Operator;
        _isLoading = true;
        try
        {
            Operators.Clear();
            foreach (var @operator in GetSupportedOperators(SelectedColumn?.DataType))
            {
                Operators.Add(new FilterOperatorOption(@operator, GetOperatorText(@operator)));
            }

            SelectedOperator = Operators.FirstOrDefault(option => option.Operator == previous)
                ?? Operators.FirstOrDefault();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private static RowFilterOperator[] GetSupportedOperators(ColumnDataType? dataType) =>
        dataType is ColumnDataType.Integer or ColumnDataType.Decimal or ColumnDataType.Lookup or ColumnDataType.Flags
            ? new[]
            {
                RowFilterOperator.Equals,
                RowFilterOperator.NotEquals,
                RowFilterOperator.GreaterThan,
                RowFilterOperator.GreaterOrEqual,
                RowFilterOperator.LessThan,
                RowFilterOperator.LessOrEqual,
            }
            : new[]
            {
                RowFilterOperator.Equals,
                RowFilterOperator.NotEquals,
                RowFilterOperator.Contains,
            };

    private static string GetOperatorText(RowFilterOperator @operator) => Strings.Get(@operator switch
    {
        RowFilterOperator.Equals => "FilterOperatorEquals",
        RowFilterOperator.NotEquals => "FilterOperatorNotEquals",
        RowFilterOperator.Contains => "FilterOperatorContains",
        RowFilterOperator.GreaterThan => "FilterOperatorGreaterThan",
        RowFilterOperator.GreaterOrEqual => "FilterOperatorGreaterOrEqual",
        RowFilterOperator.LessThan => "FilterOperatorLessThan",
        RowFilterOperator.LessOrEqual => "FilterOperatorLessOrEqual",
        _ => "FilterOperatorEquals",
    });
}
