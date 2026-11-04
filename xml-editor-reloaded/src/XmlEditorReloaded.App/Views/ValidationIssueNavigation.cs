using System.Windows;
using System.Windows.Controls;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Navigates from a validation issue list to the record referenced by the selected issue.
/// </summary>
public static class ValidationIssueNavigation
{
    /// <summary>
    /// Attached property that enables navigation when the selected validation issue changes.
    /// </summary>
    public static readonly DependencyProperty NavigateOnSelectionProperty = DependencyProperty.RegisterAttached(
        "NavigateOnSelection",
        typeof(bool),
        typeof(ValidationIssueNavigation),
        new PropertyMetadata(false, OnNavigateOnSelectionChanged));

    /// <summary>
    /// Gets whether navigation on selection is enabled for a list.
    /// </summary>
    /// <param name="element">List element.</param>
    /// <returns>The current value.</returns>
    public static bool GetNavigateOnSelection(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(NavigateOnSelectionProperty);
    }

    /// <summary>
    /// Enables or disables navigation on selection for a list.
    /// </summary>
    /// <param name="element">List element.</param>
    /// <param name="value">Whether to navigate.</param>
    public static void SetNavigateOnSelection(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(NavigateOnSelectionProperty, value);
    }

    private static void OnNavigateOnSelectionChanged(
        DependencyObject element,
        DependencyPropertyChangedEventArgs e)
    {
        if (element is not ListBox list)
        {
            return;
        }

        list.SelectionChanged -= OnSelectionChanged;
        if (e.NewValue is true)
        {
            list.SelectionChanged += OnSelectionChanged;
        }
    }

    private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox
            {
                DataContext: IEditorTab editor,
                SelectedItem: ValidationIssueItem issue,
            })
        {
            editor.NavigateToValidationIssue(issue);
        }
    }
}
