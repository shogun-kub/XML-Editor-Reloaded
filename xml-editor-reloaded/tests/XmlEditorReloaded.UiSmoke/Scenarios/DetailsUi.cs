using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Edits a field of the selected record through the details panel. The panel fields are ordinary
/// editable controls, so this path needs no synthetic keyboard input: the value is set through the
/// UI Automation value pattern and committed by moving the keyboard focus away from the field.
/// </summary>
internal static class DetailsUi
{
    /// <summary>
    /// Selects a grid row, filters the details panel to one field, expands the field groups, and sets
    /// the value of that field.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="grid">Rows grid of the active editor.</param>
    /// <param name="row">Zero-based row index to select.</param>
    /// <param name="field">XML field name, also used as the automation identifier of its editor.</param>
    /// <param name="value">New field value.</param>
    /// <returns>The field editor, after the value was committed.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the field editor cannot be found.</exception>
    public static AutomationElement EditField(
        ScenarioContext context,
        AutomationElement grid,
        int row,
        string field,
        string value)
    {
        var editor = ShowField(context, grid, row, field);
        editor.SetText(value);
        Thread.Sleep(200);

        // The field text box updates its binding when it loses the keyboard focus.
        context.Window.WaitById("DetailFilterBox").SetFocus();
        Thread.Sleep(300);
        return editor;
    }

    /// <summary>
    /// Selects a grid row, filters the details panel to one field, expands the field groups, and returns
    /// that field editor without changing its value.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="grid">Rows grid of the active editor.</param>
    /// <param name="row">Zero-based row index to select.</param>
    /// <param name="field">XML field name, also used as the automation identifier of its editor.</param>
    /// <param name="timeout">Optional timeout for the field editor.</param>
    /// <returns>The field editor.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the field editor cannot be found.</exception>
    public static AutomationElement ShowField(
        ScenarioContext context,
        AutomationElement grid,
        int row,
        string field,
        TimeSpan? timeout = null)
    {
        var rowElement = grid
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .ElementAtOrDefault(row)
            ?? throw new UiTimeoutException($"Строка {row} не реализована в сетке.");
        return ShowField(context, grid, rowElement, field, timeout);
    }

    /// <summary>
    /// Selects an already realized grid row, filters the details panel to one field, expands the field
    /// groups, and returns that field editor without changing its value.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="grid">Rows grid of the active editor.</param>
    /// <param name="row">Row element to select.</param>
    /// <param name="field">XML field name, also used as the automation identifier of its editor.</param>
    /// <param name="timeout">Optional timeout for the field editor.</param>
    /// <returns>The field editor.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the field editor cannot be found.</exception>
    public static AutomationElement ShowField(
        ScenarioContext context,
        AutomationElement grid,
        AutomationElement row,
        string field,
        TimeSpan? timeout = null)
    {
        var window = context.Window;
        row.SelectItem();
        Thread.Sleep(300);

        window.WaitById("DetailFilterBox").SetText(field);
        Thread.Sleep(400);
        ExpandDetailGroups(window);

        return window.WaitById(field, timeout ?? TimeSpan.FromSeconds(20));
    }

    private static void ExpandDetailGroups(AutomationElement window)
    {
        foreach (var group in window.FindAll(new PropertyCondition(
                     AutomationElement.ControlTypeProperty,
                     ControlType.Group)))
        {
            if (!string.IsNullOrEmpty(group.ReadName()))
            {
                continue;
            }

            ScenarioUi.SetExpanded(group, expanded: true);
        }
    }
}
