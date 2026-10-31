using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Shared operations for the item, mercenary-gear, and shopkeeper workspaces: opening a workspace,
/// selecting tabs and records, reading the workspace status line, and committing field edits.
/// </summary>
internal static class WorkspaceUi
{
    /// <summary>
    /// Opens <c>Items.Items</c> and waits until the item master-detail workspace is ready.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>The item count text.</returns>
    /// <exception cref="SmokeCheckException">Thrown when the workspace does not open.</exception>
    public static string OpenItemWorkspace(ScenarioContext context)
    {
        ScenarioUi.OpenTable(context, "Items.Items");
        WaitForStatus(
            context,
            "ItemStatus",
            text => text == context.Strings.Get("EditorReady"),
            "рабочее место предмета загружено",
            TimeSpan.FromSeconds(90));
        var window = context.Window;
        var count = window.WaitById("ItemCountText", TimeSpan.FromSeconds(60)).ReadName();
        context.Recorder.Check("Открыто рабочее место предмета", true, count);
        return count;
    }

    /// <summary>
    /// Opens <c>Inventory.MercStartingGear</c> and waits until the mercenary-gear workspace is ready.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>The mercenary count text.</returns>
    public static string OpenMercGearWorkspace(ScenarioContext context)
    {
        ScenarioUi.OpenTable(context, "Inventory.MercStartingGear");
        WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Get("EditorReady"),
            "рабочее место экипировки наёмников загружено",
            TimeSpan.FromSeconds(60));
        var window = context.Window;
        var count = window.WaitById("MercCountText", TimeSpan.FromSeconds(30)).ReadName();
        context.Recorder.Check("Открыто рабочее место экипировки наёмников", true, count);
        return count;
    }

    /// <summary>
    /// Waits until the item workspace status line shows the supplied text.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="expected">Expected status text.</param>
    /// <param name="timeout">Optional timeout.</param>
    public static void WaitItemStatus(ScenarioContext context, string expected, TimeSpan? timeout = null) =>
        WaitForStatus(context, "ItemStatus", text => text == expected, $"статус «{expected}»", timeout);

    /// <summary>
    /// Filters the item field list and realizes the matching field editors.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="pathText">Field path used both as the filter term and as the automation identifier.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The field editor with the given automation identifier.</returns>
    public static AutomationElement ShowField(ScenarioContext context, string pathText, TimeSpan? timeout = null)
    {
        var window = context.Window;
        var filter = window.WaitById("ItemFieldFilterBox");
        SetTextAndCommit(filter, pathText);
        ExpandAll(window.WaitById("ItemTabs"));
        return window.WaitById(pathText, timeout);
    }

    /// <summary>
    /// Reads the current value of a field editor: the toggle state of a check box, the selected text of
    /// a combo box, or the text of a text box.
    /// </summary>
    /// <param name="field">Field editor element.</param>
    /// <returns>Value text.</returns>
    public static string ReadFieldValue(AutomationElement field)
    {
        if (field.Current.ControlType == ControlType.CheckBox
            && field.TryGetCurrentPattern(TogglePattern.Pattern, out var raw)
            && raw is TogglePattern toggle)
        {
            return toggle.Current.ToggleState == ToggleState.On ? "1" : "0";
        }

        return field.Current.ControlType == ControlType.ComboBox ? field.ReadName() : field.ReadValue();
    }

    /// <summary>
    /// Edits a text field of the item workspace and commits the value by leaving the field.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="pathText">Field path and automation identifier.</param>
    /// <param name="value">New value.</param>
    public static void SetField(ScenarioContext context, string pathText, string value) =>
        SetTextAndCommit(ShowField(context, pathText), value);

    /// <summary>
    /// Expands every expander below the supplied root, because WPF creates the content of a collapsed
    /// expander lazily and the fields would not be reachable otherwise.
    /// </summary>
    /// <param name="root">Root element that contains the expanders.</param>
    public static void ExpandAll(AutomationElement root)
    {
        foreach (var expander in root.FindAll(
                     new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Group)))
        {
            if (!expander.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var raw)
                || raw is not ExpandCollapsePattern pattern)
            {
                continue;
            }

            try
            {
                if (pattern.Current.ExpandCollapseState == ExpandCollapseState.Collapsed)
                {
                    pattern.Expand();
                }
            }
            catch (ElementNotAvailableException)
            {
                // The section was rebuilt while it was being expanded.
            }
        }

        Thread.Sleep(250);
    }

    /// <summary>
    /// Returns the captions of the section tabs inside the "Child tables" tab of the item workspace.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="childTablesCaption">Caption of the "Child tables" tab of the item workspace.</param>
    /// <returns>Section captions in tab order.</returns>
    public static IReadOnlyList<string> ChildSectionCaptions(
        AutomationElement window,
        string childTablesCaption) =>
        ChildSectionTabs(window, childTablesCaption)
            .Select(static tab => TabText(tab))
            .ToList();

    /// <summary>
    /// Selects a child-table section by the beginning of its caption, for example <c>Вложения</c>.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="childTablesCaption">Caption of the "Child tables" tab of the item workspace.</param>
    /// <param name="captionPrefix">Beginning of the section caption, without the row count.</param>
    /// <exception cref="UiTimeoutException">Thrown when the section is missing.</exception>
    public static void SelectChildSection(
        AutomationElement window,
        string childTablesCaption,
        string captionPrefix)
    {
        var tab = ChildSectionTabs(window, childTablesCaption)
            .FirstOrDefault(candidate => TabText(candidate).StartsWith(captionPrefix, StringComparison.Ordinal))
            ?? throw new UiTimeoutException($"Раздел «{captionPrefix}» не найден.");
        tab.SelectItem();
        Thread.Sleep(400);
    }

    private static IReadOnlyList<AutomationElement> ChildSectionTabs(
        AutomationElement window,
        string childTablesCaption)
    {
        SelectItemTab(window, childTablesCaption);
        var tabItem = window.WaitById("ItemTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .FirstOrDefault(candidate => string.Equals(TabText(candidate), childTablesCaption, StringComparison.Ordinal))
            ?? throw new UiTimeoutException($"Вкладка «{childTablesCaption}» не найдена.");
        return tabItem
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));
    }

    /// <summary>
    /// Waits until a workspace status line satisfies a predicate.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="automationId">Automation identifier of the status text.</param>
    /// <param name="predicate">Predicate applied to the status text.</param>
    /// <param name="description">Description used in the timeout message.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The matching status text.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the status does not match in time.</exception>
    public static string WaitForStatus(
        ScenarioContext context,
        string automationId,
        Func<string, bool> predicate,
        string description,
        TimeSpan? timeout = null)
    {
        var window = context.Window;
        string last = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                last = window.WaitById(automationId).ReadName();
                return predicate(last);
            },
            description,
            timeout);
        return last;
    }

    /// <summary>
    /// Returns every tab caption of the item workspace main tab control.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <returns>Captions in tab order.</returns>
    public static IReadOnlyList<string> ItemTabCaptions(AutomationElement window) =>
        TabCaptions(window, "ItemTabs");

    /// <summary>
    /// Returns the captions of one tab control.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="automationId">Automation identifier of the tab control.</param>
    /// <returns>Captions in tab order.</returns>
    public static IReadOnlyList<string> TabCaptions(AutomationElement window, string automationId) =>
        window.WaitById(automationId)
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .Select(static tab => TabText(tab))
            .ToList();

    /// <summary>
    /// Returns the visible text of a tab header. The content of a selected WPF tab is exposed as a child
    /// of its tab item, so only texts that lie inside the header bounds are considered part of the caption.
    /// </summary>
    /// <param name="tab">Tab item.</param>
    /// <returns>Header text with single spaces.</returns>
    public static string TabText(AutomationElement tab)
    {
        var bounds = tab.Current.BoundingRectangle;
        var parts = tab
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Where(text =>
            {
                var rectangle = text.Current.BoundingRectangle;
                return rectangle.Height > 0
                    && rectangle.Top >= bounds.Top - 1
                    && rectangle.Bottom <= bounds.Bottom + 1;
            })
            .Select(static text => text.ReadName())
            .Where(static text => !string.IsNullOrWhiteSpace(text))
            .ToList();
        return string.Join(" ", parts);
    }

    /// <summary>
    /// Selects a tab by its visible caption in the item workspace main tab control.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="caption">Tab caption.</param>
    /// <exception cref="UiTimeoutException">Thrown when the tab is missing.</exception>
    public static void SelectItemTab(AutomationElement window, string caption) =>
        SelectTab(window, "ItemTabs", caption);

    /// <summary>
    /// Selects a tab by its visible caption.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="tabControlId">Automation identifier of the tab control.</param>
    /// <param name="caption">Tab caption.</param>
    /// <exception cref="UiTimeoutException">Thrown when the tab is missing.</exception>
    public static void SelectTab(AutomationElement window, string tabControlId, string caption)
    {
        var tab = window.WaitById(tabControlId)
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .FirstOrDefault(candidate => string.Equals(TabText(candidate), caption, StringComparison.Ordinal))
            ?? throw new UiTimeoutException($"Вкладка «{caption}» не найдена.");
        tab.SelectItem();
        Thread.Sleep(350);
    }

    /// <summary>
    /// Selects an item by its identifier through the "Go to ID" control and waits for the form header.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="id">Item identifier.</param>
    /// <param name="expectedName">Expected value of <c>szItemName</c>.</param>
    public static void SelectItem(ScenarioContext context, string id, string expectedName)
    {
        var window = context.Window;
        var goTo = window.WaitById("ItemGoToIdBox");
        goTo.Focus();
        goTo.SetText(id);
        window.WaitById("ItemGoToIdButton").InvokeButton();
        var expected = $"{id}: {expectedName}";
        WaitForStatus(
            context,
            "ItemDetailHeader",
            text => text == expected,
            $"выбран предмет {expected}",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check($"Выбран предмет {expected}", true);
    }

    /// <summary>
    /// Sets a text input and moves focus away so a lost-focus binding writes the value.
    /// </summary>
    /// <param name="input">Editable element.</param>
    /// <param name="value">New text.</param>
    public static void SetTextAndCommit(AutomationElement input, string value)
    {
        input.Focus();
        input.SetText(value);
        NativeInput.PressIn(input, NativeInput.VirtualKey.Tab);
        Thread.Sleep(300);
    }

    /// <summary>
    /// Finds a list item whose visible text equals the supplied text, ignoring surrounding spaces.
    /// </summary>
    /// <param name="list">List box.</param>
    /// <param name="text">Item text.</param>
    /// <returns>The item, or <see langword="null"/> when it is not realized.</returns>
    public static AutomationElement? FindListItem(AutomationElement list, string text) =>
        list.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .FirstOrDefault(item => string.Equals(Ui.ItemText(item).Trim(), text, StringComparison.Ordinal));

    /// <summary>
    /// Clicks the item workspace save button and waits for the saved status.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    public static void SaveItem(ScenarioContext context)
    {
        context.Window.WaitById("ItemSaveButton").InvokeButton();
        WaitForStatus(
            context,
            "ItemStatus",
            text => text == context.Strings.Get("EditorSaved"),
            "статус «Сохранено.» рабочего места предмета",
            TimeSpan.FromSeconds(60));
    }

    /// <summary>
    /// Clicks the mercenary-gear save button and waits for the saved status.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    public static void SaveMercGear(ScenarioContext context)
    {
        context.Window.WaitById("MercSaveButton").InvokeButton();
        WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Get("EditorSaved"),
            "статус «Сохранено.» рабочего места наёмников",
            TimeSpan.FromSeconds(60));
    }

    /// <summary>
    /// Returns the quantity input of one dealer row of the shopkeeper tab. Every row shares the same
    /// automation identifier, so the dealer name that the input exposes as its name selects the row.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dealerName">Dealer name shown on the row, for example <c>Tony</c>.</param>
    /// <returns>The quantity input.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the dealer row is missing.</exception>
    public static AutomationElement ShopkeeperQuantity(ScenarioContext context, string dealerName) =>
        context.Window
            .FindAllById("ShopkeeperQuantityBox")
            .FirstOrDefault(box => string.Equals(box.ReadName(), dealerName, StringComparison.Ordinal))
        ?? throw new UiTimeoutException($"Строка торговца {dealerName} не найдена.");

    /// <summary>
    /// Returns the visible texts of every row of a list box.
    /// </summary>
    /// <param name="list">List box.</param>
    /// <returns>Row texts in display order.</returns>
    public static IReadOnlyList<string> ListItemTexts(AutomationElement list) =>
        list.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .Select(static item => Ui.ItemText(item).Trim())
            .ToList();

    /// <summary>
    /// Undoes every pending edit of a workspace so the tab can be closed without an unsaved-changes
    /// prompt and a read-only scenario leaves no changes behind.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="undoAutomationId">Automation identifier of the undo button.</param>
    /// <param name="saveAutomationId">Automation identifier of the save button, which signals dirty state.</param>
    /// <param name="maximumSteps">Safety limit of undo invocations.</param>
    /// <exception cref="UiTimeoutException">Thrown when the workspace stays dirty.</exception>
    public static void UndoAll(
        ScenarioContext context,
        string undoAutomationId,
        string saveAutomationId,
        int maximumSteps = 120)
    {
        var window = context.Window;
        var save = window.WaitById(saveAutomationId);
        var undo = window.WaitById(undoAutomationId);
        for (var step = 0; step < maximumSteps && save.IsEnabled(); step++)
        {
            if (!undo.IsEnabled())
            {
                break;
            }

            undo.InvokeButton();
            Thread.Sleep(120);
        }

        Ui.WaitUntil(() => !save.IsEnabled(), "все правки отменены", TimeSpan.FromSeconds(20));
    }

    /// <summary>
    /// Returns the captions of the realized table headers in screen order.
    /// </summary>
    /// <param name="grid">Data grid.</param>
    /// <returns>Header captions from left to right.</returns>
    public static IReadOnlyList<string> VisualHeaders(AutomationElement grid) =>
        grid.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.HeaderItem))
            .Select(static header => (Header: header, Caption: header.ReadName()))
            .Where(static pair => !string.IsNullOrEmpty(pair.Caption))
            .OrderBy(static pair => pair.Header.Current.BoundingRectangle.Left)
            .Select(static pair => pair.Caption)
            .ToList();

    /// <summary>
    /// Returns the zero-based display index of a table column.
    /// </summary>
    /// <param name="grid">Data grid.</param>
    /// <param name="caption">Column caption.</param>
    /// <returns>Column index, or <c>-1</c> when the column is not realized.</returns>
    public static int ColumnIndex(AutomationElement grid, string caption) =>
        VisualHeaders(grid).ToList().IndexOf(caption);

    /// <summary>
    /// Reads the visible value of a grid cell: the text of a plain cell or the display text of a relation
    /// picker cell. The automation name of a WPF data-grid cell is a diagnostic string, not its value.
    /// </summary>
    /// <param name="grid">Data grid.</param>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <returns>Visible cell text.</returns>
    public static string ReadCell(AutomationElement grid, int row, int column)
    {
        var cell = grid.Cell(row, column);
        var editor = cell
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit))
            .FirstOrDefault();
        if (editor is not null)
        {
            return editor.ReadValue();
        }

        var text = cell
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static element => element.ReadName())
            .FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value));
        return text ?? cell.ReadName();
    }
}
