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
        var window = context.Window;
        grid.Cell(row, 0).SelectItem();
        Thread.Sleep(300);

        window.WaitById("DetailFilterBox").SetText(field);
        Thread.Sleep(400);
        ExpandDetailGroups(window);

        var editor = window.WaitById(field, TimeSpan.FromSeconds(20));
        editor.SetText(value);
        Thread.Sleep(200);

        // The field text box updates its binding when it loses the keyboard focus.
        window.WaitById("DetailFilterBox").SetFocus();
        Thread.Sleep(300);
        return editor;
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
