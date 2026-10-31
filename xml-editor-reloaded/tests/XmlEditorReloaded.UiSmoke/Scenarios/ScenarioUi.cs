using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Higher-level operations for the smoke scenarios: table rows, opening tables, tabs, and status
/// messages.
/// </summary>
internal static class ScenarioUi
{
    /// <summary>
    /// Finds a row of the main table list, scrolling the virtualized grid until the row is realized.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="tableName">Table name shown in the first column.</param>
    /// <param name="timeout">Optional overall timeout.</param>
    /// <returns>The row, or <see langword="null"/> when the table is not in the list.</returns>
    public static AutomationElement? FindTableRow(
        AutomationElement window,
        string tableName,
        TimeSpan? timeout = null)
    {
        var grid = window.WaitById("TablesGrid");
        grid.ScrollToTop();
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(40));
        while (DateTime.UtcNow < deadline)
        {
            var row = grid.FindRow(tableName);
            if (row is not null)
            {
                return row;
            }

            if (!grid.ScrollDown())
            {
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Selects a table in the main list and opens it.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="tableName">Table name shown in the first column.</param>
    /// <returns>The selected row.</returns>
    /// <exception cref="SmokeCheckException">Thrown when the table is missing or does not open.</exception>
    public static AutomationElement OpenTable(ScenarioContext context, string tableName)
    {
        var window = context.Window;
        var row = FindTableRow(window, tableName)
            ?? throw new SmokeCheckException(
                $"Таблица {tableName} есть в списке",
                "строка не найдена в виртуализированном списке");

        context.Recorder.Check($"Таблица {tableName} есть в списке", true);
        row.SelectItem();
        var open = window.WaitById("OpenTableButton");
        open.InvokeButton();
        return row;
    }

    /// <summary>
    /// Returns the visible names of every open editor tab.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <returns>Tab names in tab order.</returns>
    public static IReadOnlyList<string> TabNames(AutomationElement window) =>
        window.WaitById("EditorsTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .Select(static tab => ReadTabText(tab))
            .ToList();

    /// <summary>
    /// Reads the visible text of a tab header, which combines the table name, the data-set label, and
    /// the close button glyph.
    /// </summary>
    /// <param name="tab">Tab item.</param>
    /// <returns>Header text with single spaces.</returns>
    private static string ReadTabText(AutomationElement tab)
    {
        var parts = tab
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .Where(static text => !string.IsNullOrWhiteSpace(text))
            .ToList();
        return string.Join(" ", parts);
    }

    /// <summary>
    /// Waits until the status text satisfies a predicate.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="predicate">Predicate applied to the status text.</param>
    /// <param name="description">Description used in the timeout message.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The status text that satisfied the predicate.</returns>
    public static string WaitForStatus(
        ScenarioContext context,
        Func<string, bool> predicate,
        string description,
        TimeSpan? timeout = null)
    {
        var window = context.Window;
        string last = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                last = window.WaitById("StatusText").ReadName();
                return predicate(last);
            },
            description,
            timeout);
        return last;
    }

    /// <summary>
    /// Waits until the editor status text satisfies a predicate.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="predicate">Predicate applied to the status text.</param>
    /// <param name="description">Description used in the timeout message.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The status text that satisfied the predicate.</returns>
    public static string WaitForEditorStatus(
        ScenarioContext context,
        Func<string, bool> predicate,
        string description,
        TimeSpan? timeout = null)
    {
        var window = context.Window;
        string last = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                last = window.WaitById("EditorStatus").ReadName();
                return predicate(last);
            },
            description,
            timeout);
        return last;
    }

    /// <summary>
    /// Selects a data directory in the main selector.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="dataDirectory">Absolute data directory path shown by the selector item.</param>
    public static void SelectDataDirectory(AutomationElement window, string dataDirectory)
    {
        var combo = window.WaitById("DataDirectoryCombo");
        combo.SelectComboItem(dataDirectory);
    }

    /// <summary>
    /// Expands a combo box, returns the item texts, and collapses it again.
    /// </summary>
    /// <param name="combo">Combo box.</param>
    /// <returns>Item texts in list order.</returns>
    public static IReadOnlyList<string> ReadComboItems(AutomationElement combo)
    {
        combo.ExpandCombo();
        Thread.Sleep(150);
        var items = combo
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .Select(static item => item
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
                .Select(static text => text.ReadName())
                .FirstOrDefault(static text => !string.IsNullOrWhiteSpace(text))
                ?? item.ReadName())
            .ToList();
        Collapse(combo);
        return items;
    }

    /// <summary>
    /// Collapses a combo box when it supports the expand/collapse pattern.
    /// </summary>
    /// <param name="combo">Combo box.</param>
    public static void Collapse(AutomationElement combo)
    {
        try
        {
            if (combo.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var pattern)
                && pattern is ExpandCollapsePattern expand)
            {
                expand.Collapse();
                Thread.Sleep(80);
            }
        }
        catch (ElementNotAvailableException)
        {
            // The control is being rebuilt.
        }
    }

    /// <summary>
    /// Waits until a modal dialog of the application appears and returns its window.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The dialog window, which contains the dialog-specific automation identifiers.</returns>
    /// <exception cref="UiTimeoutException">Thrown when no dialog appears in time.</exception>
    public static AutomationElement WaitForDialog(UiApp app, TimeSpan? timeout = null) =>
        Ui.WaitFor(
            () =>
            {
                var windows = AutomationElement.RootElement.FindAll(
                    TreeScope.Children,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)));
                return windows
                    .Cast<AutomationElement>()
                    .FirstOrDefault(static window => window.FindById("MessageText") is not null);
            },
            "диалог подтверждения",
            timeout);

    /// <summary>
    /// Counts the top-level windows of a process; a modal dialog adds a second window.
    /// </summary>
    /// <param name="processId">Process identifier.</param>
    /// <returns>Number of top-level windows.</returns>
    public static int TopLevelWindowCount(int processId) =>
        AutomationElement.RootElement.FindAll(
            TreeScope.Children,
            new AndCondition(
                new PropertyCondition(AutomationElement.ProcessIdProperty, processId),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)))
            .Count;

    /// <summary>
    /// Returns the files next to a table file that look like a backup or a temporary copy. The
    /// Windows search pattern <c>name.*</c> also matches the file itself, so the file is excluded.
    /// </summary>
    /// <param name="file">Table file that was saved or opened.</param>
    /// <returns>Paths of the suspicious files, empty when there are none.</returns>
    public static IReadOnlyList<string> BackupCopies(string file)
    {
        var directory = System.IO.Path.GetDirectoryName(file);
        var name = System.IO.Path.GetFileName(file);
        if (string.IsNullOrEmpty(directory))
        {
            return [];
        }

        return System.IO.Directory
            .GetFiles(directory, name + ".*")
            .Where(path => !string.Equals(
                System.IO.Path.GetFileName(path),
                name,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Returns the prefix of a localized format string, up to its first placeholder.
    /// </summary>
    /// <param name="format">Format string such as <c>Loaded {0} tables.</c></param>
    /// <returns>Text before the first placeholder.</returns>
    public static string FormatPrefix(string format)
    {
        var index = format.IndexOf('{');
        return index < 0 ? format : format[..index];
    }
}
