using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
                    FontWeight = column.IsKey ? FontWeights.Bold : FontWeights.Normal,
                };
                AutomationProperties.SetHelpText(header, column.HeaderTooltip);

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
        picker.SetValue(RelationPicker.AutomationIdProperty, fieldName);
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
        var background = ThemeBrush("Theme.Translation.Background", Brushes.Transparent);
        var foreground = ThemeBrush("Theme.Translation.Foreground", Brushes.Black);
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
        trigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, foreground));
        trigger.Setters.Add(new Setter(
            TextBlock.ToolTipProperty,
            _viewModel?.DetailUntranslatedText ?? string.Empty));
        style.Triggers.Add(trigger);
        return style;
    }

    private static Brush ThemeBrush(string key, Brush fallback) =>
        Application.Current?.TryFindResource(key) as Brush ?? fallback;

    private void OnFilterOperandFocusWithinChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: FilterCriterionViewModel criterion })
        {
            return;
        }

        if (e.NewValue is true)
        {
            criterion.BeginOperandEdit();
        }
        else
        {
            criterion.EndOperandEdit();
        }
    }

    private void OnFilterOperandDropDownClosed(object? sender, EventArgs e)
    {
        if (sender is FrameworkElement { DataContext: FilterCriterionViewModel criterion })
        {
            criterion.EndOperandEdit();
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

    /// <summary>
    /// Blocks editing of a row that has no base record to write to, for example a city shown from the
    /// nested <c>CITYLIST</c> container. Column-level rules cannot express this, and trying to write
    /// such a row would end in a misleading key error.
    /// </summary>
    private void OnRowsGridBeginningEdit(object? sender, DataGridBeginningEditEventArgs e)
    {
        if (e.Row.Item is TableRowViewModel { CanEdit: false })
        {
            e.Cancel = true;
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

        // The grid selects whole rows (SelectionUnit=FullRow). Assigning DataGridCell.IsSelected makes
        // WPF try to add the cell to SelectedCells, which throws InvalidOperationException when the row
        // is already selected. Select the row through the grid instead: an already selected row keeps
        // the whole selection, an unselected row extends it with Ctrl or replaces it without Ctrl.
        if (FindAncestor<DataGridRow>(cell) is { } row && !row.IsSelected)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                RowsGrid.SelectedItems.Add(row.Item);
            }
            else
            {
                RowsGrid.SelectedItem = row.Item;
            }
        }

        cell.Focus();
        _viewModel.CurrentCellField = string.IsNullOrEmpty(cell.Column.SortMemberPath)
            ? null
            : cell.Column.SortMemberPath;
    }

    /// <summary>
    /// Opens the record referenced by an embedded child row when its row header is double-clicked,
    /// matching the legacy row-header navigation without taking over double-click cell editing.
    /// </summary>
    private void OnRowsGridDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel is not { IsEmbedded: true } || e.ChangedButton != MouseButton.Left)
        {
            return;
        }

        var source = e.OriginalSource as DependencyObject;
        if (FindAncestor<DataGridRowHeader>(source) is not { } rowHeader)
        {
            return;
        }

        var row = rowHeader.DataContext as TableRowViewModel
            ?? (rowHeader.TemplatedParent as DataGridRow)?.Item as TableRowViewModel;
        if (row is not null && _viewModel.NavigateToRelationTarget(row))
        {
            e.Handled = true;
        }
    }

    /// <summary>
    /// Handles view-level shortcuts that need focus context: Ctrl+F focuses the search box, Ctrl+G
    /// reveals and focuses the first filter value, and Insert/Delete only act when no text editor owns
    /// the key.
    /// </summary>
    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F1)
        {
            if (Window.GetWindow(this)?.DataContext is MainViewModel main
                && main.OpenHelpCommand.CanExecute(null))
            {
                main.OpenHelpCommand.Execute(null);
            }

            e.Handled = true;
            return;
        }

        if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.G && Keyboard.Modifiers == ModifierKeys.Control)
        {
            FocusFilterOperand();
            e.Handled = true;
            return;
        }

        if (IsTextEditing(e.OriginalSource))
        {
            return;
        }

        if (e.Key == Key.D && Keyboard.Modifiers == ModifierKeys.Control)
        {
            Execute(_viewModel?.DuplicateRowCommand);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.L && Keyboard.Modifiers == ModifierKeys.Control)
        {
            Execute(_viewModel?.ChangeLookupIdCommand);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.U && Keyboard.Modifiers == ModifierKeys.Control)
        {
            Execute(_viewModel?.ShowLookupUsagesCommand);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.A && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
        {
            Execute(_viewModel?.Filter?.AddCriterionCommand);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.X && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
        {
            Execute(_viewModel?.ClearFilterCommand);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.F5 && Keyboard.Modifiers == ModifierKeys.None)
        {
            Execute(_viewModel?.ReloadCommand);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.F8 && Keyboard.Modifiers == ModifierKeys.None)
        {
            Execute(_viewModel?.ValidateCommand);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete
            && Keyboard.Modifiers == ModifierKeys.None
            && RowsGrid.IsKeyboardFocusWithin)
        {
            Execute(_viewModel?.DeleteRowCommand);
            e.Handled = true;
        }
        else if (e.Key == Key.Insert
                 && Keyboard.Modifiers == ModifierKeys.None
                 && RowsGrid.IsKeyboardFocusWithin)
        {
            Execute(_viewModel?.AddRowCommand);
            e.Handled = true;
        }
    }

    private static void Execute(ICommand? command)
    {
        if (command?.CanExecute(null) == true)
        {
            command.Execute(null);
        }
    }

    private void FocusFilterOperand()
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.IsFilterVisible = true;
        FilterExpander.IsExpanded = true;
        if (_viewModel.Filter is { Criteria.Count: 0 } filter)
        {
            filter.AddCriterionCommand.Execute(null);
        }

        FilterCriteriaList.UpdateLayout();
        var operand = FindDescendant<ComboBox>(
            FilterCriteriaList,
            combo => AutomationProperties.GetAutomationId(combo) == "FilterOperandBox");
        if (operand is not null)
        {
            operand.Focus();
            return;
        }

        FilterAddButton.Focus();
    }

    private static bool IsTextEditing(object? source) =>
        source is TextBoxBase
        || FindAncestor<TextBoxBase>(source as DependencyObject) is not null;

    private static T? FindDescendant<T>(DependencyObject? root, Func<T, bool>? match = null)
        where T : DependencyObject
    {
        if (root is null)
        {
            return null;
        }

        for (var index = 0; index < VisualTreeHelper.GetChildrenCount(root); index++)
        {
            var child = VisualTreeHelper.GetChild(root, index);
            if (child is T typed && (match?.Invoke(typed) ?? true))
            {
                return typed;
            }

            var nested = FindDescendant(child, match);
            if (nested is not null)
            {
                return nested;
            }
        }

        return null;
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
