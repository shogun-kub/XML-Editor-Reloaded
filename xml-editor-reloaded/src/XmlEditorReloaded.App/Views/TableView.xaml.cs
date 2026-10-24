using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Schema-driven editable grid for one table tab.
/// </summary>
public partial class TableView : UserControl
{
    private TableEditorViewModel? _viewModel;
    private bool _subscribed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableView"/> class.
    /// </summary>
    public TableView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += (_, _) => AttachToViewModel();
        Unloaded += (_, _) => DetachFromViewModel();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        DetachFromViewModel();
        _viewModel = e.NewValue as TableEditorViewModel;
        AttachToViewModel();
    }

    private void AttachToViewModel()
    {
        if (_viewModel is null || _subscribed)
        {
            return;
        }

        _viewModel.VisibleColumns.CollectionChanged += OnVisibleColumnsChanged;
        _subscribed = true;
        BuildColumns();
    }

    private void DetachFromViewModel()
    {
        if (_viewModel is null || !_subscribed)
        {
            return;
        }

        _viewModel.VisibleColumns.CollectionChanged -= OnVisibleColumnsChanged;
        _subscribed = false;
    }

    private void OnVisibleColumnsChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        BuildColumns();

    private void BuildColumns()
    {
        RowsGrid.Columns.Clear();
        if (_viewModel is null)
        {
            return;
        }

        foreach (var column in _viewModel.VisibleColumns)
        {
            var binding = new Binding($"[{column.Name}]")
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
            };
            var header = new TextBlock
            {
                Text = column.Name,
                ToolTip = column.Tooltip,
                FontWeight = column.IsKey ? FontWeights.SemiBold : FontWeights.Normal,
            };
            RowsGrid.Columns.Add(new DataGridTextColumn
            {
                Header = header,
                Binding = binding,
                IsReadOnly = column.IsReadOnly,
                Width = new DataGridLength(column.Width),
                SortMemberPath = column.Name,
            });
        }

        if (_viewModel.SortField.Length == 0)
        {
            return;
        }

        foreach (var column in RowsGrid.Columns)
        {
            if (string.Equals(column.SortMemberPath, _viewModel.SortField, StringComparison.Ordinal))
            {
                column.SortDirection = _viewModel.SortDirection;
            }
        }
    }

    private void OnSorting(object sender, DataGridSortingEventArgs e)
    {
        if (_viewModel is null || string.IsNullOrEmpty(e.Column.SortMemberPath))
        {
            return;
        }

        e.Handled = true;
        var direction = e.Column.SortDirection == ListSortDirection.Ascending
            ? ListSortDirection.Descending
            : ListSortDirection.Ascending;
        foreach (var column in RowsGrid.Columns)
        {
            column.SortDirection = null;
        }

        e.Column.SortDirection = direction;
        _viewModel.SortBy(e.Column.SortMemberPath, direction);
    }
}
