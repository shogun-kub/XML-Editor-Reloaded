using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Schema-driven editable grid for one table tab.
/// </summary>
public partial class TableView : UserControl
{
    private static readonly DependencyPropertyDescriptor? ColumnWidthDescriptor =
        DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn));

    private TableEditorViewModel? _viewModel;
    private bool _subscribed;
    private bool _buildingColumns;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableView"/> class.
    /// </summary>
    public TableView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += (_, _) => AttachToViewModel();
        Unloaded += (_, _) =>
        {
            PushColumnWidths();
            _ = _viewModel?.FlushLayoutAsync();
            DetachFromViewModel();
        };
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
        _buildingColumns = true;
        try
        {
            foreach (var column in RowsGrid.Columns)
            {
                ColumnWidthDescriptor?.RemoveValueChanged(column, OnColumnWidthChanged);
            }

            RowsGrid.Columns.Clear();
            if (_viewModel is null)
            {
                return;
            }

            if (_viewModel.ShowThumbnailColumn)
            {
                RowsGrid.Columns.Add(BuildThumbnailColumn());
            }

            foreach (var column in _viewModel.VisibleColumns)
            {
                var isTranslationColumn = _viewModel.IsTranslationMode && column.IsTranslatable;
                var headerText = isTranslationColumn
                    ? column.Name + _viewModel.TranslatableColumnSuffix
                    : column.Name;
                if (column.HasLookupError || column.HasRelationError)
                {
                    headerText += " ⚠";
                }

                var header = new TextBlock
                {
                    Text = headerText,
                    ToolTip = column.HeaderTooltip,
                    FontWeight = column.IsKey ? FontWeights.SemiBold : FontWeights.Normal,
                };

                DataGridColumn gridColumn;
                if (column.IsBoolean)
                {
                    gridColumn = new DataGridCheckBoxColumn
                    {
                        Header = header,
                        Binding = new Binding($"[{column.Name}]")
                        {
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Converter = BooleanValueConverter.Instance,
                        },
                        IsReadOnly = column.IsReadOnly,
                        Width = new DataGridLength(column.Width),
                        SortMemberPath = column.Name,
                    };
                }
                else if (column.HasRelationPicker && column.Relation is { } relation)
                {
                    gridColumn = new DataGridTemplateColumn
                    {
                        Header = header,
                        CellTemplate = BuildRelationCellTemplate(column.Name, relation),
                        CellEditingTemplate = BuildRelationEditingTemplate(column.Name, relation),
                        IsReadOnly = column.IsReadOnly,
                        Width = new DataGridLength(column.Width),
                        SortMemberPath = column.Name,
                    };
                }
                else if (column.HasLookupCombo && column.Lookup is { } lookup)
                {
                    var templateColumn = new DataGridTemplateColumn
                    {
                        Header = header,
                        CellTemplate = BuildLookupCellTemplate(column.Name, lookup),
                        IsReadOnly = column.IsReadOnly,
                        Width = new DataGridLength(column.Width),
                        SortMemberPath = column.Name,
                    };
                    if (!column.IsReadOnly)
                    {
                        templateColumn.CellEditingTemplate = BuildLookupEditingTemplate(column.Name, lookup);
                    }

                    gridColumn = templateColumn;
                }
                else
                {
                    var textColumn = new DataGridTextColumn
                    {
                        Header = header,
                        Binding = new Binding($"[{column.Name}]")
                        {
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                        },
                        IsReadOnly = column.IsReadOnly,
                        Width = new DataGridLength(column.Width),
                        SortMemberPath = column.Name,
                    };
                    if (isTranslationColumn)
                    {
                        textColumn.ElementStyle = BuildTranslationCellStyle(column.Name);
                    }

                    gridColumn = textColumn;
                }

                RowsGrid.Columns.Add(gridColumn);
                ColumnWidthDescriptor?.AddValueChanged(gridColumn, OnColumnWidthChanged);
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
        finally
        {
            _buildingColumns = false;
        }
    }

    private static DataGridTemplateColumn BuildThumbnailColumn()
    {
        var image = new FrameworkElementFactory(typeof(Image));
        image.SetValue(FrameworkElement.WidthProperty, 24.0);
        image.SetValue(FrameworkElement.HeightProperty, 24.0);
        image.SetValue(Image.StretchProperty, Stretch.Uniform);
        image.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.NearestNeighbor);
        image.SetBinding(Image.SourceProperty, new Binding(nameof(TableRowViewModel.Thumbnail)));
        image.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(OnThumbnailCellLoaded));
        return new DataGridTemplateColumn
        {
            Header = string.Empty,
            Width = new DataGridLength(30),
            IsReadOnly = true,
            CanUserSort = false,
            CellTemplate = new DataTemplate { VisualTree = image },
        };
    }

    private static void OnThumbnailCellLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: TableRowViewModel row })
        {
            row.RequestThumbnail();
        }
    }

    private static DataTemplate BuildRelationCellTemplate(string fieldName, RelationColumnData relation)
    {
        var text = new FrameworkElementFactory(typeof(TextBlock));
        text.SetBinding(TextBlock.TextProperty, new Binding($"[{fieldName}]")
        {
            Mode = BindingMode.OneWay,
            Converter = new RelationDisplayConverter(relation),
        });
        text.SetBinding(TextBlock.ToolTipProperty, new Binding($"[{fieldName}]")
        {
            Mode = BindingMode.OneWay,
            Converter = new RelationTooltipConverter(relation),
        });
        text.SetValue(TextBlock.MarginProperty, new Thickness(4, 0, 4, 0));
        text.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
        text.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        return new DataTemplate { VisualTree = text };
    }

    private static DataTemplate BuildRelationEditingTemplate(string fieldName, RelationColumnData relation)
    {
        var picker = new FrameworkElementFactory(typeof(RelationPicker));
        picker.SetValue(RelationPicker.RelationDataProperty, relation);
        picker.SetBinding(RelationPicker.SelectedValueProperty, new Binding($"[{fieldName}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });
        picker.SetValue(FrameworkElement.MarginProperty, new Thickness(1, 0, 1, 0));
        picker.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
        return new DataTemplate { VisualTree = picker };
    }

    private static DataTemplate BuildLookupCellTemplate(string fieldName, LookupColumnData lookup)
    {
        var text = new FrameworkElementFactory(typeof(TextBlock));
        text.SetBinding(TextBlock.TextProperty, new Binding($"[{fieldName}]")
        {
            Mode = BindingMode.OneWay,
            Converter = new LookupDisplayConverter(lookup),
        });
        text.SetBinding(TextBlock.ToolTipProperty, new Binding($"[{fieldName}]")
        {
            Mode = BindingMode.OneWay,
            Converter = new LookupTooltipConverter(lookup),
        });
        text.SetValue(TextBlock.MarginProperty, new Thickness(4, 0, 4, 0));
        text.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
        text.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
        return new DataTemplate { VisualTree = text };
    }

    private static DataTemplate BuildLookupEditingTemplate(string fieldName, LookupColumnData lookup)
    {
        var combo = new FrameworkElementFactory(typeof(ComboBox));
        combo.SetValue(ItemsControl.ItemsSourceProperty, lookup.Options);
        combo.SetValue(ComboBox.DisplayMemberPathProperty, nameof(LookupOption.DropdownText));
        combo.SetValue(ComboBox.SelectedValuePathProperty, nameof(LookupOption.IdText));
        combo.SetBinding(ComboBox.SelectedValueProperty, new Binding($"[{fieldName}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });
        combo.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
        return new DataTemplate { VisualTree = combo };
    }

    private Style BuildTranslationCellStyle(string fieldName)
    {
        var background = new SolidColorBrush(Color.FromRgb(0xFF, 0xF4, 0xCE));
        background.Freeze();
        var style = new Style(typeof(TextBlock));
        var trigger = new DataTrigger
        {
            Binding = new Binding(nameof(TableRowViewModel.TranslationState))
            {
                Converter = TranslationStateConverter.Instance,
                ConverterParameter = fieldName,
            },
            Value = true,
        };
        trigger.Setters.Add(new Setter(TextBlock.BackgroundProperty, background));
        trigger.Setters.Add(new Setter(
            TextBlock.ToolTipProperty,
            _viewModel?.DetailUntranslatedText ?? string.Empty));
        style.Triggers.Add(trigger);
        return style;
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

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _viewModel?.UpdateSelectedRows(RowsGrid.SelectedItems.OfType<TableRowViewModel>());
    }

    private void OnCurrentCellChanged(object? sender, EventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.CurrentCellField = RowsGrid.CurrentCell.Column?.SortMemberPath;
        }
    }

    private void OnColumnReordered(object? sender, DataGridColumnEventArgs e)
    {
        if (_viewModel is null)
        {
            return;
        }

        var fields = RowsGrid.Columns
            .OrderBy(static column => column.DisplayIndex)
            .Select(static column => column.SortMemberPath)
            .Where(static field => !string.IsNullOrEmpty(field))
            .ToArray();
        _viewModel.ApplyColumnOrder(fields);
        PushColumnWidths();
    }

    private void OnColumnWidthChanged(object? sender, EventArgs e) => PushColumnWidths();

    private void PushColumnWidths()
    {
        if (_viewModel is null || _buildingColumns)
        {
            return;
        }

        var widths = new List<(string Field, double Width)>();
        foreach (var column in RowsGrid.Columns)
        {
            var field = column.SortMemberPath;
            if (string.IsNullOrEmpty(field))
            {
                continue;
            }

            var width = column.ActualWidth > 0 ? column.ActualWidth : column.Width.Value;
            if (width > 0)
            {
                widths.Add((field, width));
            }
        }

        if (widths.Count > 0)
        {
            _viewModel.ApplyColumnWidths(widths);
        }
    }

    private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel is null)
        {
            return;
        }

        var cell = FindAncestor<DataGridCell>(e.OriginalSource as DependencyObject);
        if (cell is null)
        {
            _viewModel.CurrentCellField = null;
            return;
        }

        cell.IsSelected = true;
        cell.Focus();
        if (!string.IsNullOrEmpty(cell.Column.SortMemberPath))
        {
            _viewModel.CurrentCellField = cell.Column.SortMemberPath;
        }
    }

    private static T? FindAncestor<T>(DependencyObject? current)
        where T : DependencyObject
    {
        while (current is not null)
        {
            if (current is T match)
            {
                return match;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
