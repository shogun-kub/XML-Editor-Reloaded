using System.IO;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Shared operations for the "file changed on disk" scenarios: opening <c>Lookup.Cursor</c>, editing a
/// cell, changing the file behind the editor's back, and answering the resolution dialog.
/// </summary>
internal static class ExternalChangeUi
{
    /// <summary>
    /// Table used by the external-change scenarios.
    /// </summary>
    public const string Table = "Lookup.Cursor";

    /// <summary>
    /// Opens the cursor lookup table and returns its grid.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>The rows grid of the editor.</returns>
    public static AutomationElement OpenCursorTable(ScenarioContext context)
    {
        ScenarioUi.OpenTable(context, Table);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            $"{Table} открыт");
        return context.Window.WaitById("RowsGrid");
    }

    /// <summary>
    /// Replaces the name of the first record in the table file, simulating another program that edited
    /// the file while the editor had it open.
    /// </summary>
    /// <param name="file">Table file path.</param>
    /// <param name="newName">Value written into the first <c>name</c> element.</param>
    /// <returns>The new file content.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the file has no <c>name</c> element.</exception>
    public static string EditFirstRecordName(string file, string newName)
    {
        var text = File.ReadAllText(file);
        const string Open = "<name>";
        const string Close = "</name>";
        var start = text.IndexOf(Open, StringComparison.Ordinal);
        var end = start < 0 ? -1 : text.IndexOf(Close, start, StringComparison.Ordinal);
        if (start < 0 || end < 0)
        {
            throw new InvalidOperationException($"В файле «{file}» нет элемента name.");
        }

        var updated = text[..(start + Open.Length)] + newName + text[end..];
        File.WriteAllText(file, updated, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        return updated;
    }

    /// <summary>
    /// Waits until the "file changed on disk" dialog appears.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>The dialog window.</returns>
    public static AutomationElement WaitForDialog(ScenarioContext context) =>
        ScenarioUi.WaitForWindowWithId(
            context.App,
            "OverwriteButton",
            "окно внешнего изменения файла",
            TimeSpan.FromSeconds(30));

    /// <summary>
    /// Returns the fragment of the external-change message that names the table file and explains why
    /// saving was refused, without the localized prefix.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>Text expected inside the dialog message.</returns>
    public static string ExpectedMessageFragment(ScenarioContext context)
    {
        var format = context.Strings.Get("ExternalChangeMessageFormat");
        var from = format.IndexOf("{0}", StringComparison.Ordinal);
        var to = format.IndexOf("{1}", StringComparison.Ordinal);
        return from >= 0 && to > from
            ? format[(from + 3)..to].Trim()
            : "changed on disk";
    }

    /// <summary>
    /// Reads the first cell value of a grid row through the text child, which is what the user sees.
    /// </summary>
    /// <param name="grid">Rows grid.</param>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <returns>Cell text.</returns>
    public static string CellText(AutomationElement grid, int row, int column) =>
        DataOperationsUi.GridCellText(grid, row, column);

    /// <summary>
    /// Waits until a cell shows the expected text.
    /// </summary>
    /// <param name="grid">Rows grid.</param>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <param name="expected">Expected text.</param>
    /// <param name="description">Description used in the timeout message.</param>
    public static void WaitForCell(
        AutomationElement grid,
        int row,
        int column,
        string expected,
        string description) =>
        Ui.WaitUntil(
            () => string.Equals(CellText(grid, row, column), expected, StringComparison.Ordinal),
            description,
            TimeSpan.FromSeconds(20));
}
