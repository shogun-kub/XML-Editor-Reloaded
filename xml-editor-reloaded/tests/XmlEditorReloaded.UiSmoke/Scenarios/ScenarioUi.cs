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
        catch (Exception exception) when (
            exception is ElementNotAvailableException or ElementNotEnabledException
                or System.Runtime.InteropServices.COMException)
        {
            // The control was rebuilt, or the popup closed itself.
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
        Ui.WaitFor(() => FindDialog(app), "диалог подтверждения", timeout);

    /// <summary>
    /// Waits until a native message box of the application appears, for example the delete-row
    /// confirmation created by <c>MessageBox.Show</c>.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The message-box window.</returns>
    /// <exception cref="UiTimeoutException">Thrown when no message box appears in time.</exception>
    public static AutomationElement WaitForMessageBox(UiApp app, TimeSpan? timeout = null) =>
        Ui.WaitFor(() => FindMessageBox(app), "окно подтверждения удаления", timeout);

    /// <summary>
    /// Answers a native message box. The Yes button is identified by the Win32 control identifier
    /// and, as a fallback, by its localized caption.
    /// </summary>
    /// <param name="messageBox">Message-box window.</param>
    /// <param name="yes">Whether to press Yes; otherwise No.</param>
    /// <exception cref="UiTimeoutException">Thrown when the requested button is not found.</exception>
    public static void AnswerMessageBox(AutomationElement messageBox, bool yes)
    {
        var expectedId = yes ? "6" : "7";
        var buttons = messageBox
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
        var button = buttons.FirstOrDefault(candidate =>
                string.Equals(candidate.Current.AutomationId, expectedId, StringComparison.Ordinal))
            ?? buttons.FirstOrDefault(candidate => IsYesNoCaption(candidate.ReadName(), yes))
            ?? throw new UiTimeoutException($"Кнопка {(yes ? "«Да»" : "«Нет»")} не найдена в окне подтверждения.");
        button.InvokeButton();
        Thread.Sleep(200);
    }

    /// <summary>
    /// Returns the visible text of a native message box, combining every text descendant.
    /// </summary>
    /// <param name="messageBox">Message-box window.</param>
    /// <returns>Message text with single spaces.</returns>
    public static string ReadMessageBoxText(AutomationElement messageBox) =>
        string.Join(
            " ",
            messageBox
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
                .Select(static text => text.ReadName())
                .Where(static text => !string.IsNullOrWhiteSpace(text)));

    private static bool IsYesNoCaption(string caption, bool yes)
    {
        var trimmed = caption.Replace("&", string.Empty, StringComparison.Ordinal).Trim();
        return yes
            ? trimmed is "Да" or "Yes" or "OK"
            : trimmed is "Нет" or "No" or "Cancel" or "Отмена";
    }

    /// <summary>
    /// Expands or collapses a WPF expander.
    /// </summary>
    /// <param name="expander">Expander element.</param>
    /// <param name="expanded">Whether the expander should be expanded.</param>
    public static void SetExpanded(AutomationElement expander, bool expanded)
    {
        if (expander.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var raw)
            && raw is ExpandCollapsePattern pattern)
        {
            var state = pattern.Current.ExpandCollapseState;
            if (expanded && state != ExpandCollapseState.Expanded)
            {
                pattern.Expand();
            }
            else if (!expanded && state != ExpandCollapseState.Collapsed)
            {
                pattern.Collapse();
            }

            Thread.Sleep(250);
            return;
        }

        expander.Click();
    }

    /// <summary>
    /// Waits until the context-menu item with the given caption appears and returns it.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="caption">Menu item caption.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The menu item.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the item does not appear in time.</exception>
    public static AutomationElement WaitForMenuItem(
        UiApp app,
        string caption,
        TimeSpan? timeout = null) =>
        Ui.WaitFor(
            () => AutomationElement.RootElement
                .FindAll(
                    TreeScope.Descendants,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem)))
                .Cast<AutomationElement>()
                .FirstOrDefault(item => string.Equals(item.ReadName(), caption, StringComparison.Ordinal)),
            $"пункт меню «{caption}»",
            timeout);

    /// <summary>
    /// Waits until the grid shows the column header with the given caption.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="caption">Column header caption.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The header element.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the header does not appear in time.</exception>
    public static AutomationElement WaitForColumnHeader(
        AutomationElement window,
        string caption,
        TimeSpan? timeout = null) =>
        Ui.WaitFor(
            () => window.WaitById("RowsGrid")
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.HeaderItem))
                .FirstOrDefault(header => string.Equals(header.ReadName(), caption, StringComparison.Ordinal)),
            $"заголовок колонки «{caption}»",
            timeout);

    /// <summary>
    /// Returns the captions of the visible grid column headers in display order.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <returns>Header captions.</returns>
    public static List<string> ColumnHeaderCaptions(AutomationElement window) =>
        window.WaitById("RowsGrid")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.HeaderItem))
            .Select(static header => header.ReadName())
            .Where(static caption => !string.IsNullOrEmpty(caption))
            .ToList();

    /// <summary>
    /// Waits until no native message box of the application is open.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <exception cref="UiTimeoutException">Thrown when the message box stays open.</exception>
    public static void WaitForMessageBoxClosed(UiApp app, TimeSpan? timeout = null)
    {
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(10));
        while (DateTime.UtcNow < deadline)
        {
            if (app.HasExited || FindMessageBox(app) is null)
            {
                return;
            }

            Thread.Sleep(100);
        }

        throw new UiTimeoutException("Окно подтверждения не закрылось.");
    }

    private static AutomationElement? FindMessageBox(UiApp app)
    {
        try
        {
            // A message box is owned by the main window, so the automation tree may nest it under that
            // window instead of listing it among the root's children.
            return AutomationElement.RootElement
                .FindAll(
                    TreeScope.Descendants,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)))
                .Cast<AutomationElement>()
                .FirstOrDefault(static window => window.FindById("StatusText") is null
                    && window.FindById("MessageText") is null
                    && window.FindById("BulkEditApplyButton") is null
                    && window.FindFirst(
                        TreeScope.Descendants,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button)) is not null);
        }
        catch (Exception exception) when (
            exception is ElementNotAvailableException or InvalidOperationException)
        {
            return null;
        }
    }

    /// <summary>
    /// Waits until a modal dialog whose message contains the given text appears. Several save dialogs
    /// open one after another during application close, so the text distinguishes them.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="fragment">Text expected inside the dialog message.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The matching dialog window.</returns>
    /// <exception cref="UiTimeoutException">Thrown when no matching dialog appears in time.</exception>
    public static AutomationElement WaitForDialogWithText(
        UiApp app,
        string fragment,
        TimeSpan? timeout = null) =>
        Ui.WaitFor(
            () =>
            {
                var dialog = FindDialog(app);
                return dialog?.FindById("MessageText") is { } message
                    && message.ReadName().Contains(fragment, StringComparison.Ordinal)
                    ? dialog
                    : null;
            },
            $"диалог с текстом «{fragment}»",
            timeout);

    /// <summary>
    /// Waits until any top-level window of the application contains the given automation identifier,
    /// for example a modeless dialog with its own controls.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="automationId">Identifier that identifies the dialog.</param>
    /// <param name="description">Description used in the timeout message.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The window that contains the identifier.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the window does not appear in time.</exception>
    public static AutomationElement WaitForWindowWithId(
        UiApp app,
        string automationId,
        string description,
        TimeSpan? timeout = null) =>
        Ui.WaitFor(
            () => AutomationElement.RootElement
                .FindAll(
                    TreeScope.Descendants,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)))
                .Cast<AutomationElement>()
                .FirstOrDefault(window => window.FindById(automationId) is not null),
            description,
            timeout);

    /// <summary>
    /// Waits until no top-level window of the application contains the given automation identifier.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="automationId">Identifier that identifies the dialog.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <exception cref="UiTimeoutException">Thrown when the window stays open.</exception>
    public static void WaitForWindowWithIdClosed(UiApp app, string automationId, TimeSpan? timeout = null)
    {
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(10));
        while (DateTime.UtcNow < deadline)
        {
            if (app.HasExited || FindWindowWithId(app, automationId) is null)
            {
                return;
            }

            Thread.Sleep(100);
        }

        throw new UiTimeoutException($"Окно с элементом '{automationId}' не закрылось.");
    }

    /// <summary>
    /// Determines whether any top-level window of the application contains the given automation
    /// identifier.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="automationId">Identifier that identifies the dialog.</param>
    /// <returns><see langword="true"/> while such a window exists.</returns>
    public static bool IsWindowWithIdOpen(UiApp app, string automationId) =>
        FindWindowWithId(app, automationId) is not null;

    private static AutomationElement? FindWindowWithId(UiApp app, string automationId)
    {
        try
        {
            return AutomationElement.RootElement
                .FindAll(
                    TreeScope.Descendants,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)))
                .Cast<AutomationElement>()
                .FirstOrDefault(window => window.FindById(automationId) is not null);
        }
        catch (Exception exception) when (
            exception is ElementNotAvailableException or InvalidOperationException)
        {
            return null;
        }
    }

    /// <summary>
    /// Waits until no modal dialog of the application is open, or the application exits.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <exception cref="UiTimeoutException">Thrown when the dialog stays open.</exception>
    public static void WaitForDialogClosed(UiApp app, TimeSpan? timeout = null)
    {
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(10));
        while (DateTime.UtcNow < deadline)
        {
            if (app.HasExited || FindDialog(app) is null)
            {
                return;
            }

            Thread.Sleep(100);
        }

        throw new UiTimeoutException("Диалог подтверждения не закрылся.");
    }

    /// <summary>
    /// Selects an open editor tab by the table name in its header.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="tableName">Table name shown in the tab header.</param>
    /// <exception cref="UiTimeoutException">Thrown when the tab is not open.</exception>
    public static void ActivateTab(AutomationElement window, string tableName)
    {
        var tab = window.WaitById("EditorsTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .FirstOrDefault(candidate => candidate.ReadName().Contains(tableName, StringComparison.Ordinal)
                || candidate
                    .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
                    .Any(text => text.ReadName().Contains(tableName, StringComparison.Ordinal)))
            ?? throw new UiTimeoutException($"Вкладка {tableName} не найдена.");
        tab.SelectItem();
        Thread.Sleep(250);
    }

    /// <summary>
    /// Clicks the close button of an editor tab. The click is dispatched in the background because a
    /// dirty tab blocks the UI thread with the unsaved-changes dialog until the caller answers it.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="tableName">Table name shown in the tab header.</param>
    /// <exception cref="UiTimeoutException">Thrown when the tab or its close button is not found.</exception>
    public static void CloseTab(AutomationElement window, string tableName)
    {
        var tab = window.WaitById("EditorsTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .FirstOrDefault(candidate => candidate.ReadName().Contains(tableName, StringComparison.Ordinal)
                || candidate
                    .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
                    .Any(text => text.ReadName().Contains(tableName, StringComparison.Ordinal)))
            ?? throw new UiTimeoutException($"Вкладка {tableName} не найдена.");
        var close = tab.FindFirst(TreeScope.Descendants, Ui.ByName("✕"))
            ?? throw new UiTimeoutException("Кнопка закрытия вкладки не найдена.");
        _ = Task.Run(() =>
        {
            try
            {
                close.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The tab disappeared before the invoke returned.
            }
        });
        Thread.Sleep(400);
    }

    /// <summary>
    /// Closes the first editor tab. The header of a master-detail workspace tab exposes the type name of
    /// its view model instead of the table name, so it cannot be found by name like a generic table tab.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <exception cref="UiTimeoutException">Thrown when no editor tab or close button exists.</exception>
    public static void CloseActiveEditorTab(AutomationElement window)
    {
        var tab = window.WaitById("EditorsTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .FirstOrDefault()
            ?? throw new UiTimeoutException("Открытых вкладок нет.");
        var close = tab.FindFirst(TreeScope.Descendants, Ui.ByName("✕"))
            ?? throw new UiTimeoutException("Кнопка закрытия вкладки не найдена.");
        _ = Task.Run(() =>
        {
            try
            {
                close.InvokeButton();
            }
            catch (Exception exception) when (exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The tab disappeared before the invoke returned.
            }
        });
        Thread.Sleep(400);
    }

    /// <summary>
    /// Waits until the editor tab strip holds the expected number of tabs.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="expected">Expected tab count.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <exception cref="UiTimeoutException">Thrown when the count does not match in time.</exception>
    public static void WaitForEditorTabCount(AutomationElement window, int expected, TimeSpan? timeout = null) =>
        Ui.WaitUntil(
            () => window.WaitById("EditorsTabs")
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
                .Count == expected,
            $"в редакторе {expected} вкладок",
            timeout);

    /// <summary>
    /// Waits until the tab with the given table name is closed.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="tableName">Table name shown in the tab header.</param>
    /// <param name="timeout">Optional timeout.</param>
    public static void WaitForTabClosed(AutomationElement window, string tableName, TimeSpan? timeout = null)
    {
        Ui.WaitUntil(
            () => !TabNames(window).Any(name => name.Contains(tableName, StringComparison.Ordinal)),
            $"вкладка {tableName} закрыта",
            timeout);
    }

    /// <summary>
    /// Finds the visible modal dialog of the application, if one is open.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <returns>The dialog window, or <see langword="null"/>.</returns>
    private static AutomationElement? FindDialog(UiApp app)
    {
        try
        {
            return AutomationElement.RootElement
                .FindAll(
                    TreeScope.Children,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window)))
                .Cast<AutomationElement>()
                .FirstOrDefault(static window => window.FindById("MessageText") is not null);
        }
        catch (Exception exception) when (
            exception is ElementNotAvailableException or InvalidOperationException)
        {
            return null;
        }
    }

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
