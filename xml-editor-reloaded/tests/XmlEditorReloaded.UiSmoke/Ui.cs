using System.Windows.Automation;
using System.IO;
using XmlEditorReloaded.UiSmoke.Scenarios;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// UI Automation helpers shared by the scenarios: waiting, locating, invoking, reading, and editing.
/// </summary>
internal static class Ui
{
    /// <summary>
    /// Default time to wait for an element or a state change.
    /// </summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Returns a condition that matches an automation identifier.
    /// </summary>
    /// <param name="automationId">Identifier assigned by the application.</param>
    /// <returns>Property condition.</returns>
    public static Condition ById(string automationId) =>
        new PropertyCondition(AutomationElement.AutomationIdProperty, automationId);

    /// <summary>
    /// Returns a condition that matches the visible name of an element.
    /// </summary>
    /// <param name="name">Visible name.</param>
    /// <returns>Property condition.</returns>
    public static Condition ByName(string name) =>
        new PropertyCondition(AutomationElement.NameProperty, name);

    /// <summary>
    /// Finds the first descendant with the given automation identifier.
    /// </summary>
    /// <param name="root">Root element.</param>
    /// <param name="automationId">Identifier assigned by the application.</param>
    /// <returns>The element, or <see langword="null"/> when it is not present.</returns>
    public static AutomationElement? FindById(this AutomationElement root, string automationId) =>
        SafeFind(root, TreeScope.Descendants, ById(automationId));

    /// <summary>
    /// Finds the first descendant with the given visible name.
    /// </summary>
    /// <param name="root">Root element.</param>
    /// <param name="name">Visible name.</param>
    /// <returns>The element, or <see langword="null"/> when it is not present.</returns>
    public static AutomationElement? FindByName(this AutomationElement root, string name) =>
        SafeFind(root, TreeScope.Descendants, ByName(name));

    /// <summary>
    /// Finds every descendant that matches a condition.
    /// </summary>
    /// <param name="root">Root element.</param>
    /// <param name="condition">Condition to match.</param>
    /// <returns>The matching elements in UI Automation order.</returns>
    public static IReadOnlyList<AutomationElement> FindAll(this AutomationElement root, Condition condition) =>
        SafeFindAll(root, TreeScope.Descendants, condition);

    /// <summary>
    /// Finds the immediate children that match a condition. Use this for tab items: descendants also
    /// include the tab items of nested tab controls inside an editor.
    /// </summary>
    /// <param name="root">Root element.</param>
    /// <param name="condition">Condition to match.</param>
    /// <returns>The matching children in UI Automation order.</returns>
    public static IReadOnlyList<AutomationElement> FindChildren(
        this AutomationElement root,
        Condition condition) =>
        SafeFindAll(root, TreeScope.Children, condition);

    /// <summary>
    /// Finds every descendant with the given automation identifier. Repeated identifiers occur inside
    /// data templates, for example one filter row per criterion.
    /// </summary>
    /// <param name="root">Root element.</param>
    /// <param name="automationId">Identifier assigned by the application.</param>
    /// <returns>The matching elements in UI Automation order.</returns>
    public static IReadOnlyList<AutomationElement> FindAllById(
        this AutomationElement root,
        string automationId) =>
        SafeFindAll(root, TreeScope.Descendants, ById(automationId));

    /// <summary>
    /// Waits until a descendant with the given automation identifier exists.
    /// </summary>
    /// <param name="root">Root element.</param>
    /// <param name="automationId">Identifier assigned by the application.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The element.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the element does not appear in time.</exception>
    public static AutomationElement WaitById(
        this AutomationElement root,
        string automationId,
        TimeSpan? timeout = null) =>
        WaitFor(() => root.FindById(automationId), $"element '{automationId}'", timeout);

    /// <summary>
    /// Waits until a probe returns a non-null element.
    /// </summary>
    /// <param name="probe">Probe evaluated on every poll.</param>
    /// <param name="description">Description used in the timeout message.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The element produced by the probe.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the probe does not succeed in time.</exception>
    public static AutomationElement WaitFor(
        Func<AutomationElement?> probe,
        string description,
        TimeSpan? timeout = null)
    {
        var deadline = DateTime.UtcNow + (timeout ?? DefaultTimeout);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var element = probe();
                if (element is not null)
                {
                    return element;
                }
            }
            catch (ElementNotAvailableException)
            {
                // The window or the element is being rebuilt; poll again.
            }

            Thread.Sleep(100);
        }

        throw new UiTimeoutException($"Timed out waiting for {description}.");
    }

    /// <summary>
    /// Waits until a condition returns <see langword="true"/>.
    /// </summary>
    /// <param name="condition">Condition evaluated on every poll.</param>
    /// <param name="description">Description used in the timeout message.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <exception cref="UiTimeoutException">Thrown when the condition does not become true in time.</exception>
    public static void WaitUntil(Func<bool> condition, string description, TimeSpan? timeout = null)
    {
        var deadline = DateTime.UtcNow + (timeout ?? DefaultTimeout);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                if (condition())
                {
                    return;
                }
            }
            catch (ElementNotAvailableException)
            {
                // Re-evaluate against the rebuilt tree.
            }

            Thread.Sleep(100);
        }

        throw new UiTimeoutException($"Timed out waiting until {description}.");
    }

    /// <summary>
    /// Returns the visible name of an element, or an empty string when it is unavailable.
    /// </summary>
    /// <param name="element">Element to read.</param>
    /// <returns>Name text.</returns>
    public static string ReadName(this AutomationElement element)
    {
        try
        {
            return element.Current.Name ?? string.Empty;
        }
        catch (ElementNotAvailableException)
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Reads the value of an element through the value pattern, falling back to its name.
    /// </summary>
    /// <param name="element">Element to read.</param>
    /// <returns>Value text.</returns>
    public static string ReadValue(this AutomationElement element)
    {
        if (TryGet(element, ValuePattern.Pattern, out ValuePattern value))
        {
            try
            {
                return value.Current.Value;
            }
            catch (ElementNotAvailableException)
            {
                return string.Empty;
            }
        }

        return element.ReadName();
    }

    /// <summary>
    /// Determines whether an element is enabled.
    /// </summary>
    /// <param name="element">Element to inspect.</param>
    /// <returns><see langword="true"/> when enabled.</returns>
    public static bool IsEnabled(this AutomationElement element)
    {
        try
        {
            return element.Current.IsEnabled;
        }
        catch (ElementNotAvailableException)
        {
            return false;
        }
    }

    /// <summary>
    /// Invokes a button through the invoke pattern, with a mouse-click fallback.
    /// </summary>
    /// <param name="button">Button to invoke.</param>
    public static void InvokeButton(this AutomationElement button)
    {
        if (TryGet(button, InvokePattern.Pattern, out InvokePattern invoke))
        {
            try
            {
                invoke.Invoke();
                Thread.Sleep(60);
                return;
            }
            catch (ElementNotEnabledException)
            {
                throw new UiTimeoutException($"The control '{button.ReadName()}' is disabled.");
            }
            catch (Exception exception) when (
                exception is ElementNotAvailableException or System.Runtime.InteropServices.COMException)
            {
                // The provider cannot always open the client process token inside the sandbox; a
                // synthetic mouse click reaches the same button without that check.
                NativeInput.Click(button);
                return;
            }
        }

        NativeInput.Click(button);
    }

    /// <summary>
    /// Clicks an element with synthetic mouse input.
    /// </summary>
    /// <param name="element">Element to click.</param>
    public static void Click(this AutomationElement element) => NativeInput.Click(element);

    /// <summary>
    /// Selects an element through the selection-item pattern, with a click fallback.
    /// </summary>
    /// <param name="element">Selectable element.</param>
    public static void SelectItem(this AutomationElement element)
    {
        if (TryGet(element, SelectionItemPattern.Pattern, out SelectionItemPattern selection))
        {
            try
            {
                selection.Select();
                Thread.Sleep(60);
                return;
            }
            catch (Exception exception) when (
                exception is ElementNotAvailableException or ElementNotEnabledException
                    or System.Runtime.InteropServices.COMException)
            {
                // Fall back to a click on the current location.
            }
        }

        element.Click();
    }

    /// <summary>
    /// Adds an element to the current selection through the selection-item pattern. Used for
    /// multi-select grids when no keyboard input is involved.
    /// </summary>
    /// <param name="element">Selectable element.</param>
    /// <exception cref="UiTimeoutException">Thrown when the element does not support multi-selection.</exception>
    public static void AddToSelection(this AutomationElement element)
    {
        if (TryGet(element, SelectionItemPattern.Pattern, out SelectionItemPattern selection))
        {
            try
            {
                selection.AddToSelection();
                Thread.Sleep(60);
                return;
            }
            catch (Exception exception) when (
                exception is ElementNotAvailableException
                    or InvalidOperationException
                    or System.Runtime.InteropServices.COMException)
            {
                throw new UiTimeoutException(
                    $"The control '{element.ReadName()}' does not support adding to the selection: {exception.Message}");
            }
        }

        throw new UiTimeoutException($"The control '{element.ReadName()}' does not support selection.");
    }

    /// <summary>
    /// Reads the text shown by an editable combo box without expanding it. Selection patterns are not
    /// reliable for a combo whose text is bound directly instead of selecting an item.
    /// </summary>
    /// <param name="combo">Combo box.</param>
    /// <returns>Visible text, or an empty string.</returns>
    public static string ReadComboText(this AutomationElement combo)
    {
        var value = combo.ReadValue();
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var editor = combo.FindFirst(
            TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
        return editor?.ReadValue() ?? string.Empty;
    }

    /// <summary>
    /// Reads the visible text of the selected item of a combo box. The automation name of a WPF combo box
    /// with an item template is empty, so the selected item element is read instead.
    /// </summary>
    /// <param name="combo">Combo box.</param>
    /// <returns>Selected item text, or an empty string.</returns>
    public static string ReadComboSelection(this AutomationElement combo)
    {
        if (TryGet(combo, SelectionPattern.Pattern, out SelectionPattern selection))
        {
            try
            {
                var selected = selection.Current.GetSelection().FirstOrDefault();
                if (selected is not null)
                {
                    var text = selected
                        .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
                        .Select(static element => element.ReadName())
                        .FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value));
                    if (text is not null)
                    {
                        return text;
                    }
                }
            }
            catch (ElementNotAvailableException)
            {
                // The selection changed while it was being read.
            }
        }

        // WPF data-bound combos often expose only the item type name as the automation name, so the
        // selected row of the expanded list is the reliable source of the visible text.
        try
        {
            combo.ExpandCombo();
            Thread.Sleep(120);
            var items = combo
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
                .Cast<AutomationElement>()
                .ToList();
            foreach (var item in items)
            {
                if (TryGet(item, SelectionItemPattern.Pattern, out SelectionItemPattern itemSelection)
                    && itemSelection.Current.IsSelected)
                {
                    var text = ItemText(item);
                    ScenarioUi.Collapse(combo);
                    Thread.Sleep(100);
                    return text;
                }
            }

            ScenarioUi.Collapse(combo);
        }
        catch (ElementNotAvailableException)
        {
            // The popup closed itself; fall back to the element name.
        }

        var child = combo
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static element => element.ReadName())
            .FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value));
        return child ?? combo.ReadName();
    }

    /// <summary>
    /// Sets the text of an input control through the value pattern, falling back to the keyboard.
    /// </summary>
    /// <param name="element">Editable element.</param>
    /// <param name="text">Text to enter.</param>
    public static void SetText(this AutomationElement element, string text)
    {
        if (TryGet(element, ValuePattern.Pattern, out ValuePattern value) && !value.Current.IsReadOnly)
        {
            try
            {
                value.SetValue(text);
                Thread.Sleep(60);
                return;
            }
            catch (InvalidOperationException)
            {
                // Some composite controls do not accept SetValue; use the keyboard instead.
            }
        }

        element.SetFocus();
        NativeInput.PressIn(element, NativeInput.VirtualKey.A, control: true);
        NativeInput.TypeTextIn(element, text);
        Thread.Sleep(60);
    }

    /// <summary>
    /// Reads the checked state of a check box.
    /// </summary>
    /// <param name="checkBox">Check box element.</param>
    /// <returns><see langword="true"/> when checked; otherwise <see langword="false"/>.</returns>
    public static bool IsChecked(this AutomationElement checkBox) =>
        TryGet(checkBox, TogglePattern.Pattern, out TogglePattern toggle)
        && toggle.Current.ToggleState == ToggleState.On;

    /// <summary>
    /// Sets a check box to the requested state through the toggle pattern, with a click fallback.
    /// </summary>
    /// <param name="checkBox">Check box element.</param>
    /// <param name="isChecked">Whether the check box should be checked.</param>
    public static void SetChecked(this AutomationElement checkBox, bool isChecked)
    {
        if (TryGet(checkBox, TogglePattern.Pattern, out TogglePattern toggle))
        {
            var current = toggle.Current.ToggleState == ToggleState.On;
            if (current != isChecked)
            {
                toggle.Toggle();
                Thread.Sleep(120);
            }

            return;
        }

        checkBox.Click();
    }

    /// <summary>
    /// Focuses an element.
    /// </summary>
    /// <param name="element">Element to focus.</param>
    public static void Focus(this AutomationElement element)
    {
        try
        {
            element.SetFocus();
            Thread.Sleep(40);
        }
        catch (ElementNotAvailableException)
        {
            // Focus is best effort; the next input attempt reports the real failure.
        }
    }

    /// <summary>
    /// Expands a combo box.
    /// </summary>
    /// <param name="combo">Combo box to expand.</param>
    public static void ExpandCombo(this AutomationElement combo)
    {
        if (TryGet(combo, ExpandCollapsePattern.Pattern, out ExpandCollapsePattern expand))
        {
            expand.Expand();
            Thread.Sleep(150);
        }
    }

    /// <summary>
    /// Selects an item of a combo box by its visible text.
    /// </summary>
    /// <param name="combo">Combo box.</param>
    /// <param name="text">Text of the item to select.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <exception cref="UiTimeoutException">Thrown when the item cannot be found.</exception>
    public static void SelectComboItem(this AutomationElement combo, string text, TimeSpan? timeout = null) =>
        combo.SelectComboItemCore(
            candidate => string.Equals(ItemText(candidate), text, StringComparison.Ordinal),
            $"combo item '{text}'",
            timeout);

    /// <summary>
    /// Selects an item of a combo box whose visible text starts with the given prefix. Useful for
    /// display texts that combine a field name and a localized type, such as bulk-edit columns.
    /// </summary>
    /// <param name="combo">Combo box.</param>
    /// <param name="prefix">Prefix of the item text.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <exception cref="UiTimeoutException">Thrown when the item cannot be found.</exception>
    public static void SelectComboItemStartingWith(
        this AutomationElement combo,
        string prefix,
        TimeSpan? timeout = null) =>
        combo.SelectComboItemCore(
            candidate => ItemText(candidate).StartsWith(prefix, StringComparison.Ordinal),
            $"combo item starting with '{prefix}'",
            timeout);

    private static void SelectComboItemCore(
        this AutomationElement combo,
        Func<AutomationElement, bool> matches,
        string description,
        TimeSpan? timeout)
    {
        try
        {
            combo.ExpandCombo();
        }
        catch (Exception exception) when (
            exception is ElementNotEnabledException or ElementNotAvailableException
                or System.Runtime.InteropServices.COMException)
        {
            // The provider may refuse the pattern; a real click on the combo opens the same popup.
            combo.Click();
            Thread.Sleep(250);
        }

        var item = WaitFor(
            () => combo
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
                .FirstOrDefault(matches),
            description,
            timeout);
        item.SelectItem();
        Thread.Sleep(120);
        var collapsePattern = combo;
        if (collapsePattern.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var pattern)
            && pattern is ExpandCollapsePattern expand)
        {
            try
            {
                expand.Collapse();
            }
            catch (Exception exception) when (
                exception is ElementNotAvailableException or ElementNotEnabledException
                    or System.Runtime.InteropServices.COMException)
            {
                // The popup closes itself when the selection changes.
            }
        }
    }

    /// <summary>
    /// Reads the visible text of a list item, which is a text descendant rather than the automation
    /// name when the items are data objects rendered with a display template.
    /// </summary>
    /// <param name="item">List item.</param>
    /// <returns>Visible item text.</returns>
    public static string ItemText(AutomationElement item) =>
        item.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .FirstOrDefault(static text => !string.IsNullOrWhiteSpace(text))
        ?? item.ReadName();

    /// <summary>
    /// Reads the texts of every cell in a row.
    /// </summary>
    /// <param name="row">Row element.</param>
    /// <returns>Cell texts in column order.</returns>
    public static IReadOnlyList<string> ReadRowCells(this AutomationElement row) =>
        SafeFindAll(row, TreeScope.Children, Condition.TrueCondition)
            .Select(cell => cell.ReadName())
            .ToList();

    /// <summary>
    /// Finds a data row whose first cell equals the given text.
    /// </summary>
    /// <param name="grid">Grid element.</param>
    /// <param name="firstCellText">Text of the first cell.</param>
    /// <returns>The row, or <see langword="null"/> when it is not realized.</returns>
    public static AutomationElement? FindRow(this AutomationElement grid, string firstCellText) =>
        SafeFindAll(grid, TreeScope.Children, new PropertyCondition(
                AutomationElement.ControlTypeProperty,
                ControlType.DataItem))
            .FirstOrDefault(row => string.Equals(row.ReadRowCells().FirstOrDefault(), firstCellText, StringComparison.Ordinal));

    /// <summary>
    /// Returns the realized data row at the given index.
    /// </summary>
    /// <param name="grid">Grid element.</param>
    /// <param name="index">Zero-based row index among realized rows.</param>
    /// <returns>The row, or <see langword="null"/> when the index is outside the realized rows.</returns>
    public static AutomationElement? RowAt(this AutomationElement grid, int index) =>
        SafeFindAll(grid, TreeScope.Children, new PropertyCondition(
                AutomationElement.ControlTypeProperty,
                ControlType.DataItem))
            .ElementAtOrDefault(index);

    /// <summary>
    /// Scrolls a grid to the top when it supports the scroll pattern.
    /// </summary>
    /// <param name="grid">Grid element.</param>
    public static void ScrollToTop(this AutomationElement grid)
    {
        if (TryGet(grid, ScrollPattern.Pattern, out ScrollPattern scroll))
        {
            try
            {
                scroll.SetScrollPercent(ScrollPattern.NoScroll, 0);
                Thread.Sleep(150);
            }
            catch (InvalidOperationException)
            {
                // A grid that does not need scrolling rejects the call.
            }
        }
    }

    /// <summary>
    /// Scrolls a grid to the bottom when it supports the scroll pattern.
    /// </summary>
    /// <param name="grid">Grid element.</param>
    public static void ScrollToBottom(this AutomationElement grid)
    {
        if (TryGet(grid, ScrollPattern.Pattern, out ScrollPattern scroll))
        {
            try
            {
                scroll.SetScrollPercent(ScrollPattern.NoScroll, 100);
                Thread.Sleep(200);
            }
            catch (InvalidOperationException)
            {
                // A grid that does not need scrolling rejects the call.
            }
        }
    }

    /// <summary>
    /// Scrolls a grid down by one large step.
    /// </summary>
    /// <param name="grid">Grid element.</param>
    /// <returns><see langword="true"/> when the grid could be scrolled further.</returns>
    public static bool ScrollDown(this AutomationElement grid)
    {
        if (!TryGet(grid, ScrollPattern.Pattern, out ScrollPattern scroll))
        {
            return false;
        }

        try
        {
            if (scroll.Current.VerticalScrollPercent >= 99.5)
            {
                return false;
            }

            scroll.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
            Thread.Sleep(150);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the number of rows a grid reports.
    /// </summary>
    /// <param name="grid">Grid element.</param>
    /// <returns>Row count, or zero when the grid does not expose the grid pattern.</returns>
    public static int RowCount(this AutomationElement grid)
    {
        if (TryGet(grid, GridPattern.Pattern, out GridPattern pattern))
        {
            try
            {
                return pattern.Current.RowCount;
            }
            catch (InvalidOperationException)
            {
                return 0;
            }
        }

        return 0;
    }

    /// <summary>
    /// Returns a cell of a grid by row and column index.
    /// </summary>
    /// <param name="grid">Grid element.</param>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <returns>The cell.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the cell is not available.</exception>
    public static AutomationElement Cell(this AutomationElement grid, int row, int column)
    {
        if (!TryGet(grid, GridPattern.Pattern, out GridPattern pattern))
        {
            throw new UiTimeoutException("The grid does not expose the grid pattern.");
        }

        try
        {
            return pattern.GetItem(row, column);
        }
        catch (ElementNotAvailableException exception)
        {
            throw new UiTimeoutException($"Grid cell {row}:{column} is not available: {exception.Message}");
        }
    }

    /// <summary>
    /// Edits a grid cell by focusing it, entering edit mode with F2, replacing the text, and pressing
    /// Enter.
    /// </summary>
    /// <param name="grid">Grid element.</param>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <param name="value">New text.</param>
    public static void EditCell(this AutomationElement grid, int row, int column, string value)
    {
        var cell = grid.Cell(row, column);
        // A real mouse click both activates the window and makes the cell current; UIA focus alone does
        // not always give the window the foreground, and without it synthetic keys go elsewhere.
        cell.Click();
        Thread.Sleep(150);
        cell.SetFocus();
        Thread.Sleep(120);
        NativeInput.PressIn(grid, NativeInput.VirtualKey.F2);
        Thread.Sleep(150);
        NativeInput.PressIn(grid, NativeInput.VirtualKey.A, control: true);
        NativeInput.TypeTextIn(grid, value);
        Thread.Sleep(60);
        NativeInput.PressIn(grid, NativeInput.VirtualKey.Return);
        Thread.Sleep(200);
    }

    /// <summary>
    /// Hovers an element and returns the text of the tooltip window that appears. WPF renders tooltips
    /// in their own top-level window, so the text has to be read from the desktop tree.
    /// </summary>
    /// <param name="element">Element whose tooltip is read.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>Tooltip text.</returns>
    /// <exception cref="UiTimeoutException">Thrown when no tooltip appears in time.</exception>
    public static string HoverToolTip(AutomationElement element, TimeSpan? timeout = null)
    {
        var processId = element.Current.ProcessId;
        NativeInput.MoveTo(element);
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(6));
        try
        {
            while (DateTime.UtcNow < deadline)
            {
                Thread.Sleep(150);
                var tip = SafeFindAll(
                        AutomationElement.RootElement,
                        TreeScope.Descendants,
                        new AndCondition(
                            new PropertyCondition(AutomationElement.ProcessIdProperty, processId),
                            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolTip)))
                    .FirstOrDefault(candidate => !string.IsNullOrWhiteSpace(candidate.ReadName()));
                if (tip is not null)
                {
                    var text = tip.ReadName();
                    NativeInput.MoveAway();
                    Thread.Sleep(150);
                    return text;
                }
            }
        }
        finally
        {
            NativeInput.MoveAway();
        }

        throw new UiTimeoutException($"Подсказка элемента «{element.ReadName()}» не появилась.");
    }

    /// <summary>
    /// Writes the UI Automation subtree of an element to a text file for diagnostics.
    /// </summary>
    /// <param name="root">Root element.</param>
    /// <param name="path">Destination file.</param>
    public static void DumpTree(this AutomationElement root, string path)
    {
        var lines = new List<string>();
        Dump(root, 0, lines);
        File.WriteAllLines(path, lines);
    }

    private static void Dump(AutomationElement element, int depth, List<string> lines)
    {
        if (depth > 14)
        {
            return;
        }

        var indent = new string(' ', depth * 2);
        try
        {
            var current = element.Current;
            var type = current.ControlType.ProgrammaticName.Replace("ControlType.", string.Empty);
            var rectangle = current.BoundingRectangle;
            lines.Add(
                $"{indent}{type} name='{current.Name}' id='{current.AutomationId}' "
                + $"enabled={current.IsEnabled} rect={rectangle.Left:0},{rectangle.Top:0},{rectangle.Width:0}x{rectangle.Height:0}");
        }
        catch (ElementNotAvailableException)
        {
            return;
        }

        foreach (var child in SafeFindAll(element, TreeScope.Children, Condition.TrueCondition))
        {
            Dump(child, depth + 1, lines);
        }
    }

    private static AutomationElement? SafeFind(AutomationElement root, TreeScope scope, Condition condition)
    {
        try
        {
            return root.FindFirst(scope, condition);
        }
        catch (ElementNotAvailableException)
        {
            return null;
        }
    }

    private static IReadOnlyList<AutomationElement> SafeFindAll(
        AutomationElement root,
        TreeScope scope,
        Condition condition)
    {
        try
        {
            return root.FindAll(scope, condition).Cast<AutomationElement>().ToList();
        }
        catch (ElementNotAvailableException)
        {
            return [];
        }
    }

    private static bool TryGet<T>(AutomationElement element, AutomationPattern automationPattern, out T pattern)
        where T : class
    {
        pattern = null!;
        try
        {
            if (!element.TryGetCurrentPattern(automationPattern, out var value) || value is not T typed)
            {
                return false;
            }

            pattern = typed;
            return true;
        }
        catch (ElementNotAvailableException)
        {
            return false;
        }
    }
}

/// <summary>
/// Raised when a UI element or a state does not appear within the expected time.
/// </summary>
internal sealed class UiTimeoutException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UiTimeoutException"/> class.
    /// </summary>
    /// <param name="message">Failure description.</param>
    public UiTimeoutException(string message)
        : base(message)
    {
    }
}

