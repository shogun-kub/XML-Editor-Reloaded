using XmlEditorReloaded.Core.Filtering;

namespace XmlEditorReloaded.Core.Layout;

/// <summary>
/// Persisted presentation state of one grid: column visibility, width and order, sorting, and the
/// structured row filter. The layout never contains data values from the table itself.
/// </summary>
public sealed class TableLayout
{
    /// <summary>
    /// Gets the layout with default columns, no sorting, and no filter.
    /// </summary>
    public static TableLayout Empty { get; } = new(
        Array.Empty<ColumnLayout>(),
        sortField: null,
        TableSortDirection.Ascending,
        RowFilterGroup.Empty);

    /// <summary>
    /// Initializes a new instance of the <see cref="TableLayout"/> class.
    /// </summary>
    /// <param name="columns">Persisted column states; <see langword="null"/> means none.</param>
    /// <param name="sortField">Sorted field name, or <see langword="null"/> when unsorted.</param>
    /// <param name="sortDirection">Sort direction; ignored when <paramref name="sortField"/> is null.</param>
    /// <param name="filter">Structured row filter; <see langword="null"/> means no filter.</param>
    public TableLayout(
        IReadOnlyList<ColumnLayout>? columns,
        string? sortField,
        TableSortDirection sortDirection,
        RowFilterGroup? filter)
    {
        Columns = columns ?? Array.Empty<ColumnLayout>();
        SortField = string.IsNullOrWhiteSpace(sortField) ? null : sortField;
        SortDirection = sortDirection;
        Filter = filter ?? RowFilterGroup.Empty;
    }

    /// <summary>
    /// Gets the persisted column states in schema order as written.
    /// </summary>
    public IReadOnlyList<ColumnLayout> Columns { get; }

    /// <summary>
    /// Gets the sorted field name, or <see langword="null"/> when the grid is unsorted.
    /// </summary>
    public string? SortField { get; }

    /// <summary>
    /// Gets the sort direction.
    /// </summary>
    public TableSortDirection SortDirection { get; }

    /// <summary>
    /// Gets the structured row filter.
    /// </summary>
    public RowFilterGroup Filter { get; }

    /// <summary>
    /// Gets a value indicating whether the layout carries no column, sort, or filter state.
    /// </summary>
    public bool IsEmpty => Columns.Count == 0 && SortField is null && Filter.IsEmpty;
}
