using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace XmlEditorReloaded.App.Behaviors;

/// <summary>
/// Gives every editor grid explicit row navigation: Home/End move to the first/last row and
/// PageUp/PageDown move by visible pages. WPF DataGrid otherwise uses Home/End for column navigation,
/// which is surprising in a row-oriented editor.
/// </summary>
public static class DataGridNavigation
{
    /// <summary>
    /// Attached property that enables the navigation handler on a grid.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(DataGridNavigation),
            new PropertyMetadata(false, OnIsEnabledChanged));

    /// <summary>
    /// Gets whether navigation is enabled for a grid.
    /// </summary>
    /// <param name="grid">Grid to inspect.</param>
    /// <returns>The attached value.</returns>
    public static bool GetIsEnabled(DataGrid grid) =>
        (bool)grid.GetValue(IsEnabledProperty);

    /// <summary>
    /// Sets whether navigation is enabled for a grid.
    /// </summary>
    /// <param name="grid">Grid to configure.</param>
    /// <param name="value">Whether to enable the handler.</param>
    public static void SetIsEnabled(DataGrid grid, bool value) =>
        grid.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not DataGrid grid)
        {
            return;
        }

        grid.PreviewKeyDown -= OnPreviewKeyDown;
        if (e.NewValue is true)
        {
            grid.PreviewKeyDown += OnPreviewKeyDown;
        }
    }

    private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not DataGrid grid
            || Keyboard.Modifiers != ModifierKeys.None
            || IsTextEditing(e.OriginalSource)
            || grid.Items.Count == 0)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Home:
                SelectRow(grid, 0);
                e.Handled = true;
                break;
            case Key.End:
                SelectRow(grid, grid.Items.Count - 1);
                e.Handled = true;
                break;
            case Key.PageUp:
                SelectRow(grid, grid.SelectedIndex - PageRowCount(grid));
                e.Handled = true;
                break;
            case Key.PageDown:
                SelectRow(grid, grid.SelectedIndex + PageRowCount(grid));
                e.Handled = true;
                break;
        }
    }

    private static int PageRowCount(DataGrid grid) =>
        Math.Max(1, (int)(grid.ActualHeight / 24) - 1);

    private static void SelectRow(DataGrid grid, int index)
    {
        var target = Math.Clamp(index, 0, grid.Items.Count - 1);
        grid.SelectedIndex = target;
        grid.ScrollIntoView(grid.Items[target]);
        if (grid.Columns.Count > 0)
        {
            grid.CurrentCell = new DataGridCellInfo(grid.Items[target], grid.Columns[0]);
        }

        grid.Focus();
    }

    private static bool IsTextEditing(object? source) =>
        source is TextBoxBase
        || FindAncestor<TextBoxBase>(source as DependencyObject) is not null;

    private static T? FindAncestor<T>(DependencyObject? current)
        where T : DependencyObject
    {
        while (current is not null)
        {
            if (current is T match)
            {
                return match;
            }

            current = System.Windows.Media.VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
