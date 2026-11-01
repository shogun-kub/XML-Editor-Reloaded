using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Shared operations for the cross-data-set copy dialog and the version comparison window: opening a
/// window, choosing directories and tables, running a preview or a comparison, and checking that a
/// document changed in exactly one field.
/// </summary>
internal static class DataOperationsUi
{
    /// <summary>
    /// Opens the copy dialog from the main toolbar and waits until the common table list is loaded.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>The copy dialog window.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the dialog does not appear in time.</exception>
    public static AutomationElement OpenCopyDialog(ScenarioContext context)
    {
        var window = context.Window;
        Ui.WaitUntil(
            () => window.WaitById("TablesGrid").RowCount() > 0,
            "список таблиц главного окна загружен",
            TimeSpan.FromSeconds(120));

        var button = window.WaitById("CopyDataButton");
        _ = Task.Run(() =>
        {
            try
            {
                button.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The button disappeared while the modal dialog was opening.
            }
        });

        var dialog = ScenarioUi.WaitForWindowWithId(
            context.App,
            "CopyPreviewButton",
            "окно копирования данных",
            TimeSpan.FromSeconds(90));
        WaitCopyTablesLoaded(context, dialog);
        return dialog;
    }

    /// <summary>
    /// Waits until the copy dialog reports the number of tables shared by the selected directories.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dialog">Copy dialog.</param>
    /// <param name="timeout">Optional timeout.</param>
    public static void WaitCopyTablesLoaded(
        ScenarioContext context,
        AutomationElement dialog,
        TimeSpan? timeout = null)
    {
        var prefix = ScenarioUi.FormatPrefix(context.Strings.Get("CopyTablesLoaded"));
        Ui.WaitUntil(
            () => dialog.WaitById("CopyStatusText").ReadName().StartsWith(prefix, StringComparison.Ordinal),
            "список общих таблиц загружен",
            timeout ?? TimeSpan.FromSeconds(90));
    }

    /// <summary>
    /// Selects one of the offered data directories in the source or target selector.
    /// </summary>
    /// <param name="dialog">Copy dialog.</param>
    /// <param name="comboId">Automation identifier of the selector.</param>
    /// <param name="data">Data set whose <c>TableData</c> directory must be selected.</param>
    /// <exception cref="UiTimeoutException">Thrown when the selector offers no matching directory.</exception>
    public static void SelectCopyDirectory(AutomationElement dialog, string comboId, DatasetFixture data)
    {
        var combo = dialog.WaitById(comboId);
        var target = Path.GetFullPath(data.TableDataDirectory);
        var items = ScenarioUi.ReadComboItems(combo);
        var match = items.FirstOrDefault(item => string.Equals(
            ResolveTableDataDirectory(item),
            target,
            StringComparison.OrdinalIgnoreCase));
        if (match is null)
        {
            throw new UiTimeoutException(
                $"В селекторе нет каталога «{target}»; предложено: {string.Join(" | ", items)}");
        }

        combo.SelectComboItem(match);
        Thread.Sleep(400);
    }

    /// <summary>
    /// Selects a table in the copy dialog and waits until the dialog can build a preview again.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dialog">Copy dialog.</param>
    /// <param name="tableName">Dotted table name.</param>
    public static void SelectCopyTable(ScenarioContext context, AutomationElement dialog, string tableName)
    {
        WaitCopyTablesLoaded(context, dialog);
        dialog.WaitById("CopyTableCombo").SelectComboItem(tableName);
        Ui.WaitUntil(
            () => dialog.WaitById("CopyPreviewButton").IsEnabled(),
            $"таблица «{tableName}» выбрана",
            TimeSpan.FromSeconds(60));
    }

    /// <summary>
    /// Sets the record key filter of the copy dialog.
    /// </summary>
    /// <param name="dialog">Copy dialog.</param>
    /// <param name="keys">Comma-separated keys, or an empty string for every record.</param>
    public static void SetCopyRecordKeys(AutomationElement dialog, string keys)
    {
        var box = dialog.WaitById("CopyRecordKeysBox");
        box.Focus();
        box.SetText(keys);
        Thread.Sleep(150);
    }

    /// <summary>
    /// Selects the conflict policy of the copy dialog.
    /// </summary>
    /// <param name="dialog">Copy dialog.</param>
    /// <param name="label">Localized policy label.</param>
    public static void SelectCopyPolicy(AutomationElement dialog, string label) =>
        dialog.WaitById("CopyConflictCombo").SelectComboItem(label);

    /// <summary>
    /// Selects the key mode of the copy dialog.
    /// </summary>
    /// <param name="dialog">Copy dialog.</param>
    /// <param name="label">Localized key-mode label.</param>
    public static void SelectCopyKeyMode(AutomationElement dialog, string label) =>
        dialog.WaitById("CopyKeyModeCombo").SelectComboItem(label);

    /// <summary>
    /// Turns copying of related rows on or off.
    /// </summary>
    /// <param name="dialog">Copy dialog.</param>
    /// <param name="enabled">Whether related rows are copied.</param>
    public static void SetCopyRelated(AutomationElement dialog, bool enabled) =>
        dialog.WaitById("CopyRelatedCheckBox").SetChecked(enabled);

    /// <summary>
    /// Builds the copy preview and returns its summary line.
    /// </summary>
    /// <param name="dialog">Copy dialog.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The localized preview summary.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the preview does not appear in time.</exception>
    public static string PreviewCopy(AutomationElement dialog, TimeSpan? timeout = null)
    {
        Ui.WaitUntil(
            () => dialog.WaitById("CopySummaryText").ReadName().Length == 0,
            "предыдущий предпросмотр сброшен");
        dialog.WaitById("CopyPreviewButton").InvokeButton();
        var summary = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                summary = dialog.WaitById("CopySummaryText").ReadName();
                return summary.Length > 0;
            },
            "предпросмотр построен",
            timeout ?? TimeSpan.FromSeconds(120));
        return summary;
    }

    /// <summary>
    /// Returns the text of one cell of the preview grid.
    /// </summary>
    /// <param name="dialog">Copy dialog.</param>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <returns>Visible cell text.</returns>
    public static string PreviewCell(AutomationElement dialog, int row, int column)
    {
        var grid = dialog.WaitById("PreviewGrid");
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(20);
        while (DateTime.UtcNow < deadline && grid.RowCount() <= row)
        {
            Thread.Sleep(100);
        }

        return WorkspaceUi.ReadCell(grid, row, column);
    }

    /// <summary>
    /// Clicks the copy button and waits until the dialog closes.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dialog">Copy dialog.</param>
    public static void ApplyCopy(ScenarioContext context, AutomationElement dialog)
    {
        dialog.WaitById("CopyApplyButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(
            context.App,
            "CopyPreviewButton",
            TimeSpan.FromSeconds(180));
        context.App.RefreshWindow();
    }

    /// <summary>
    /// Closes the copy dialog without writing anything.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dialog">Copy dialog.</param>
    public static void CloseCopyDialog(ScenarioContext context, AutomationElement dialog)
    {
        dialog.WaitById("CopyCloseButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(
            context.App,
            "CopyPreviewButton",
            TimeSpan.FromSeconds(60));
        context.App.RefreshWindow();
    }

    /// <summary>
    /// Opens the version comparison window from the main toolbar.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>The comparison window.</returns>
    public static AutomationElement OpenCompareWindow(ScenarioContext context)
    {
        context.Window.WaitById("CompareButton").InvokeButton();
        var window = ScenarioUi.WaitForWindowWithId(
            context.App,
            "CompareRunButton",
            "окно сравнения версий",
            TimeSpan.FromSeconds(60));
        NativeInput.Activate(new IntPtr(window.Current.NativeWindowHandle));
        return window;
    }

    /// <summary>
    /// Fills the left and right directory boxes of the comparison window.
    /// </summary>
    /// <param name="window">Comparison window.</param>
    /// <param name="left">Baseline data set.</param>
    /// <param name="right">Candidate data set.</param>
    public static void SetCompareDirectories(
        AutomationElement window,
        DatasetFixture left,
        DatasetFixture right)
    {
        SetCompareDirectory(window.WaitById("CompareLeftBox"), left.TableDataDirectory);
        SetCompareDirectory(window.WaitById("CompareRightBox"), right.TableDataDirectory);
    }

    /// <summary>
    /// Waits until the comparison summary appears after the compare button was pressed.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="window">Comparison window.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The localized summary line.</returns>
    public static string RunCompare(
        ScenarioContext context,
        AutomationElement window,
        TimeSpan? timeout = null)
    {
        var run = window.WaitById("CompareRunButton");
        Ui.WaitUntil(() => run.IsEnabled(), "каталоги сравнения выбраны");
        run.InvokeButton();

        var prefix = ScenarioUi.FormatPrefix(context.Strings.Get("CompareSummaryText"));
        var summary = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                summary = window.WaitById("CompareSummaryText").ReadName();
                return summary.StartsWith(prefix, StringComparison.Ordinal);
            },
            "сравнение завершено",
            timeout ?? TimeSpan.FromMinutes(5));
        return summary;
    }

    /// <summary>
    /// Closes the comparison window.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="window">Comparison window.</param>
    public static void CloseCompareWindow(ScenarioContext context, AutomationElement window)
    {
        window.WaitById("CompareCloseButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(
            context.App,
            "CompareRunButton",
            TimeSpan.FromSeconds(60));
    }

    /// <summary>
    /// Edits a grid cell and waits until the editor tab becomes dirty. The synthetic keyboard input of a
    /// fresh window is occasionally swallowed by the first activation click, so the edit is retried
    /// while the tab stays clean.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <param name="grid">Data grid.</param>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="column">Zero-based column index.</param>
    /// <param name="value">New cell text.</param>
    /// <param name="timeout">Optional overall timeout.</param>
    /// <exception cref="UiTimeoutException">Thrown when no attempt makes the tab dirty.</exception>
    public static void EditCellUntilDirty(
        AutomationElement window,
        AutomationElement grid,
        int row,
        int column,
        string value,
        TimeSpan? timeout = null)
    {
        var save = window.WaitById("SaveButton");
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(30));
        while (DateTime.UtcNow < deadline)
        {
            NativeInput.EnsureForeground(grid);
            grid.EditCell(row, column, value);
            var waitUntil = DateTime.UtcNow + TimeSpan.FromSeconds(6);
            while (DateTime.UtcNow < waitUntil)
            {
                if (save.IsEnabled())
                {
                    return;
                }

                Thread.Sleep(150);
            }
        }

        throw new UiTimeoutException($"Ячейка {row}:{column} не стала редактируемой за отведённое время.");
    }

    /// <summary>
    /// Returns the zero-based index of the row whose first cell shows the supplied key.
    /// </summary>
    /// <param name="grid">Data grid.</param>
    /// <param name="key">Key text shown in the first column.</param>
    /// <returns>Row index.</returns>
    /// <exception cref="UiTimeoutException">Thrown when no row shows the key.</exception>
    public static int FindRowIndex(AutomationElement grid, string key)
    {
        var rows = grid.RowCount();
        for (var row = 0; row < rows; row++)
        {
            if (string.Equals(WorkspaceUi.ReadCell(grid, row, 0).Trim(), key, StringComparison.Ordinal))
            {
                return row;
            }
        }

        throw new UiTimeoutException($"Строка с ключом «{key}» не найдена среди {rows} строк.");
    }

    /// <summary>
    /// Computes a single hash over every XML file of a directory tree, so a comparison run can be shown
    /// to leave the data sets untouched.
    /// </summary>
    /// <param name="directory">Directory whose <c>.xml</c> files are hashed.</param>
    /// <returns>Lowercase hexadecimal hash.</returns>
    public static string HashTree(string directory)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (var file in Directory
                     .EnumerateFiles(directory, "*.xml", SearchOption.AllDirectories)
                     .Order(StringComparer.Ordinal))
        {
            hash.AppendData(Encoding.UTF8.GetBytes(Path.GetRelativePath(directory, file)));
            hash.AppendData(File.ReadAllBytes(file));
        }

        return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
    }

    /// <summary>
    /// Verifies that two versions of a table differ only in one field value of one record: record and
    /// field counts, element order, names, attributes, and every other value must be identical.
    /// </summary>
    /// <param name="before">Document text as it was before the edit.</param>
    /// <param name="after">Document text as it is after the edit.</param>
    /// <param name="recordIndex">Zero-based index of the edited record.</param>
    /// <param name="field">Name of the edited element.</param>
    /// <param name="expectedOld">Value expected before the edit.</param>
    /// <param name="expectedNew">Value expected after the edit.</param>
    /// <returns><see langword="true"/> when only that value changed, with a description of the first difference.</returns>
    public static (bool OnlyFieldChanged, string? Details) OnlyFieldChanged(
        XDocument before,
        XDocument after,
        int recordIndex,
        string field,
        string expectedOld,
        string expectedNew)
    {
        if (before.Root is null || after.Root is null)
        {
            return (false, "у документа нет корневого элемента");
        }

        if (!string.Equals(before.Root.Name.LocalName, after.Root.Name.LocalName, StringComparison.Ordinal))
        {
            return (false, $"корень изменился: {before.Root.Name.LocalName} → {after.Root.Name.LocalName}");
        }

        var beforeRecords = before.Root.Elements().ToList();
        var afterRecords = after.Root.Elements().ToList();
        if (beforeRecords.Count != afterRecords.Count)
        {
            return (false, $"число записей изменилось: {beforeRecords.Count} → {afterRecords.Count}");
        }

        var changed = 0;
        for (var index = 0; index < beforeRecords.Count; index++)
        {
            var left = beforeRecords[index];
            var right = afterRecords[index];
            if (left.Name != right.Name)
            {
                return (false, $"запись {index}: элемент {left.Name.LocalName} → {right.Name.LocalName}");
            }

            if (!AttributesEqual(left, right))
            {
                return (false, $"запись {index}: атрибуты элемента {left.Name.LocalName} изменились");
            }

            var leftFields = left.Elements().ToList();
            var rightFields = right.Elements().ToList();
            if (leftFields.Count != rightFields.Count)
            {
                return (false, $"запись {index}: число полей {leftFields.Count} → {rightFields.Count}");
            }

            for (var fieldIndex = 0; fieldIndex < leftFields.Count; fieldIndex++)
            {
                var leftField = leftFields[fieldIndex];
                var rightField = rightFields[fieldIndex];
                if (leftField.Name != rightField.Name)
                {
                    return (false, $"запись {index}, поле {fieldIndex}: {leftField.Name.LocalName} → {rightField.Name.LocalName}");
                }

                if (string.Equals(leftField.Value, rightField.Value, StringComparison.Ordinal))
                {
                    continue;
                }

                changed++;
                if (index != recordIndex || !string.Equals(leftField.Name.LocalName, field, StringComparison.Ordinal))
                {
                    return (false, $"запись {index}, поле {leftField.Name.LocalName}: «{leftField.Value}» → «{rightField.Value}»");
                }

                if (!string.Equals(leftField.Value, expectedOld, StringComparison.Ordinal)
                    || !string.Equals(rightField.Value, expectedNew, StringComparison.Ordinal))
                {
                    return (false, $"поле {field}: ожидалось «{expectedOld}» → «{expectedNew}», получено «{leftField.Value}» → «{rightField.Value}»");
                }
            }
        }

        return changed == 1
            ? (true, null)
            : (false, changed == 0 ? "файл не изменился" : $"изменено полей: {changed}");
    }

    /// <summary>
    /// Resolves a data directory, a <c>Data-1.13</c> directory, or a <c>TableData</c> directory to the
    /// absolute <c>TableData</c> path used by the copy dialog.
    /// </summary>
    /// <param name="path">Directory offered by the dialog.</param>
    /// <returns>Absolute <c>TableData</c> path.</returns>
    private static string ResolveTableDataDirectory(string path)
    {
        var full = Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
        if (string.Equals(Path.GetFileName(full), "TableData", StringComparison.OrdinalIgnoreCase))
        {
            return full;
        }

        var nested = Path.Combine(full, "TableData");
        return Directory.Exists(nested) ? nested : full;
    }

    /// <summary>
    /// Sets a directory box and commits the value so the view model receives it.
    /// </summary>
    /// <param name="box">Directory text box.</param>
    /// <param name="directory">Directory path.</param>
    private static void SetCompareDirectory(AutomationElement box, string directory)
    {
        box.Focus();
        box.SetText(directory);
        NativeInput.PressIn(box, NativeInput.VirtualKey.Tab);
        Thread.Sleep(200);
    }

    /// <summary>
    /// Compares the attributes of two elements.
    /// </summary>
    /// <param name="left">First element.</param>
    /// <param name="right">Second element.</param>
    /// <returns><see langword="true"/> when both elements carry the same attributes and values.</returns>
    private static bool AttributesEqual(XElement left, XElement right)
    {
        var leftAttributes = left.Attributes().ToList();
        var rightAttributes = right.Attributes().ToList();
        if (leftAttributes.Count != rightAttributes.Count)
        {
            return false;
        }

        for (var index = 0; index < leftAttributes.Count; index++)
        {
            if (leftAttributes[index].Name != rightAttributes[index].Name
                || !string.Equals(leftAttributes[index].Value, rightAttributes[index].Value, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
