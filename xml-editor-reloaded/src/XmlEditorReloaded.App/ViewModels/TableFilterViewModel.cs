using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Filtering;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Structured grid filter editor: a flat group of field/operator/operand criteria combined with
/// AND or OR. The view model only builds a <see cref="RowFilterGroup"/>; evaluation happens in the
/// XML layer.
/// </summary>
public sealed partial class TableFilterViewModel : ObservableObject
{
    private readonly Action _changed;
    private bool _isLoading;

    [ObservableProperty]
    private FilterLogicalOperatorOption? _selectedLogicalOperator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableFilterViewModel"/> class.
    /// </summary>
    /// <param name="columns">Columns that can participate in the filter.</param>
    /// <param name="changed">Callback invoked when a user changes the filter.</param>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    public TableFilterViewModel(IReadOnlyList<TableColumnViewModel> columns, Action changed)
    {
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(changed);

        Columns = columns;
        _changed = changed;
        LogicalOperators = new[]
        {
            new FilterLogicalOperatorOption(RowFilterLogicalOperator.And, Strings.Get("FilterAnd")),
            new FilterLogicalOperatorOption(RowFilterLogicalOperator.Or, Strings.Get("FilterOr")),
        };
        SelectedLogicalOperator = LogicalOperators[0];
    }

    /// <summary>
    /// Gets the columns that can participate in the filter.
    /// </summary>
    public IReadOnlyList<TableColumnViewModel> Columns { get; }

    /// <summary>
    /// Gets the criteria of the group in display order.
    /// </summary>
    public ObservableCollection<FilterCriterionViewModel> Criteria { get; } = new();

    /// <summary>
    /// Gets the available AND/OR options.
    /// </summary>
    public IReadOnlyList<FilterLogicalOperatorOption> LogicalOperators { get; }

    /// <summary>
    /// Gets a value indicating whether the filter contains at least one criterion.
    /// </summary>
    public bool HasCriteria => Criteria.Count > 0;

    /// <summary>
    /// Gets a value indicating whether the AND/OR selector is meaningful.
    /// </summary>
    public bool HasMultipleCriteria => Criteria.Count > 1;

    /// <summary>
    /// Gets the localized filter summary.
    /// </summary>
    public string SummaryText => HasCriteria
        ? Strings.Format("EditorFilterSummaryFormat", Criteria.Count)
        : Strings.Get("EditorFilterSummaryEmpty");

    /// <summary>
    /// Adds an empty criterion to the group.
    /// </summary>
    [RelayCommand]
    public void AddCriterion()
    {
        if (Columns.Count == 0)
        {
            return;
        }

        var criterion = new FilterCriterionViewModel(Columns, OnCriterionChanged, RemoveCriterion);
        Criteria.Add(criterion);
        NotifySummaryChanged();
        _changed();
    }

    /// <summary>
    /// Removes every criterion from the group.
    /// </summary>
    [RelayCommand]
    public void Clear()
    {
        if (Criteria.Count == 0)
        {
            return;
        }

        Criteria.Clear();
        NotifySummaryChanged();
        _changed();
    }

    /// <summary>
    /// Rebuilds the editor rows from a stored filter without invoking change callbacks.
    /// </summary>
    /// <param name="group">Stored filter group.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="group"/> is null.</exception>
    public void Load(RowFilterGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        _isLoading = true;
        try
        {
            Criteria.Clear();
            SelectedLogicalOperator = group.Operator == RowFilterLogicalOperator.Or
                ? LogicalOperators[1]
                : LogicalOperators[0];
            foreach (var stored in group.Criteria)
            {
                var criterion = new FilterCriterionViewModel(Columns, OnCriterionChanged, RemoveCriterion);
                criterion.Load(stored);
                if (criterion.SelectedColumn is not null)
                {
                    Criteria.Add(criterion);
                }
            }
        }
        finally
        {
            _isLoading = false;
        }

        NotifySummaryChanged();
    }

    /// <summary>
    /// Builds the filter group that should be applied to the grid.
    /// </summary>
    /// <returns>The current filter group; empty when no complete criteria exist.</returns>
    public RowFilterGroup ToGroup() => new(
        SelectedLogicalOperator?.Operator ?? RowFilterLogicalOperator.And,
        Criteria
            .Select(static criterion => criterion.ToCriterion())
            .OfType<RowFilterCriterion>()
            .ToArray());

    partial void OnSelectedLogicalOperatorChanged(FilterLogicalOperatorOption? value)
    {
        if (!_isLoading)
        {
            _changed();
        }
    }

    private void RemoveCriterion(FilterCriterionViewModel criterion)
    {
        if (Criteria.Remove(criterion))
        {
            NotifySummaryChanged();
            _changed();
        }
    }

    private void OnCriterionChanged()
    {
        if (_isLoading)
        {
            return;
        }

        NotifySummaryChanged();
        _changed();
    }

    private void NotifySummaryChanged()
    {
        OnPropertyChanged(nameof(HasCriteria));
        OnPropertyChanged(nameof(HasMultipleCriteria));
        OnPropertyChanged(nameof(SummaryText));
    }
}
