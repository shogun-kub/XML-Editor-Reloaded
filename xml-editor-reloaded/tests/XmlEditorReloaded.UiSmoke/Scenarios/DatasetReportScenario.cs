using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S95: the data-set window validates every table of v4 in one report, filters its issues, opens the
/// affected table, searches all table values and overlays for a Russian word, opens a hit, and leaves
/// the data set unchanged.
/// </summary>
internal sealed class DatasetReportScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S95";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var before = DataOperationsUi.HashTree(v4.TableDataDirectory);

        ScenarioUi.OpenMenuItem(context, "DataMenu", "DatasetValidationButton");
        var window = ScenarioUi.WaitForWindowWithId(
            context.App,
            "DatasetValidateButton",
            "окно датасета",
            TimeSpan.FromSeconds(60));
        NativeInput.Activate(new IntPtr(window.Current.NativeWindowHandle));

        var summaryPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("DatasetValidationDoneFormat"));
        var summary = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                summary = window.WaitById("DatasetSummaryText").ReadName();
                return summary.StartsWith(summaryPrefix, StringComparison.Ordinal);
            },
            "проверка датасета завершена",
            TimeSpan.FromMinutes(3));
        context.Recorder.Check("Сводка проверки заполнена", summary.Length > 0, summary);

        var issues = window.WaitById("DatasetIssuesGrid");
        context.Recorder.Check(
            "Список замечаний содержит строки",
            issues.RowCount() > 0,
            $"строк: {issues.RowCount()}");

        var filter = window.WaitById("DatasetFilterBox");
        filter.Focus();
        filter.SetText("out_of_range");
        Thread.Sleep(500);
        context.Recorder.Check(
            "Фильтр оставляет известное предупреждение диапазона",
            issues.RowCount() == 1,
            $"строк: {issues.RowCount()}");
        var table = DataOperationsUi.GridCellText(issues, 0, 0);
        context.Recorder.Check(
            "Замечание относится к MercStartingGear",
            table == "Inventory.MercStartingGear",
            table);
        var severity = DataOperationsUi.GridCellText(issues, 0, 1);
        context.Recorder.Check(
            "Важность замечания — предупреждение",
            severity == context.Strings.Get("DatasetSeverityWarningOne"),
            severity);

        // The severity filter narrows the list to errors, which v4 does not have.
        window.WaitById("DatasetSeverityCombo")
            .SelectComboItem(context.Strings.Get("DatasetSeverityError"));
        Ui.WaitUntil(
            () => issues.RowCount() == 0,
            "фильтр важности скрыл предупреждения",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Фильтр «ошибки» не оставляет предупреждений",
            issues.RowCount() == 0);
        context.Recorder.Check(
            "Показано сообщение об отсутствии проблем",
            window.FindById("DatasetValidationEmptyText") is not null);

        // Back to every severity, clear the text filter, then open the filtered issue in the editor.
        window.WaitById("DatasetSeverityCombo").SelectComboItem(context.Strings.Get("DatasetSeverityAll"));
        Thread.Sleep(300);
        filter.Focus();
        filter.SetText(string.Empty);
        Ui.WaitUntil(() => issues.RowCount() > 1, "фильтр важности и текста сброшен", TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Полный список замечаний восстановлен",
            issues.RowCount() > 1,
            $"строк: {issues.RowCount()}");
        context.Recorder.Check(
            "Сообщение об отсутствии проблем скрыто",
            window.FindById("DatasetValidationEmptyText") is null);
        filter.Focus();
        filter.SetText("out_of_range");
        Thread.Sleep(500);
        var issueRow = issues.FindRow("Inventory.MercStartingGear")
            ?? throw new UiTimeoutException("Строка замечания MercStartingGear не найдена.");
        issueRow.SelectItem();
        Thread.Sleep(300);
        window.WaitById("DatasetOpenIssueButton").InvokeButton();
        Ui.WaitUntil(
            () => ScenarioUi.TabNames(context.Window)
                .Any(name => name.Contains("MercStartingGear", StringComparison.Ordinal)),
            "вкладка MercStartingGear открыта",
            TimeSpan.FromSeconds(120));

        var tabNames = ScenarioUi.TabNames(context.Window);
        context.Recorder.Check(
            "Из замечания открыта вкладка таблицы",
            tabNames.Any(name => name.Contains("MercStartingGear", StringComparison.Ordinal)),
            string.Join(" | ", tabNames));

        // Search the Russian overlay of Items for a word that exists only in the translation.
        NativeInput.Activate(new IntPtr(window.Current.NativeWindowHandle));
        var searchTab = ScenarioUi.SelectTab(window, context.Strings.Get("DatasetSearchTab"));
        context.Recorder.Check("Вкладка поиска доступна", searchTab is not null);
        var search = window.WaitById("DatasetSearchBox");
        search.Focus();
        search.SetText("Ничто");
        window.WaitById("DatasetSearchButton").InvokeButton();

        var searchPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("DatasetSearchDoneFormat"));
        var searchSummary = string.Empty;
        Ui.WaitUntil(
            () =>
            {
                searchSummary = window.WaitById("DatasetSearchSummaryText").ReadName();
                return searchSummary.StartsWith(searchPrefix, StringComparison.Ordinal);
            },
            "поиск по датасету завершён",
            TimeSpan.FromMinutes(3));

        var hits = window.WaitById("DatasetHitsGrid");
        context.Recorder.Check(
            "Поиск нашёл переводы в оверлее",
            hits.RowCount() > 0,
            $"{searchSummary}; строк: {hits.RowCount()}");

        var hitRow = FindGridRow(hits, "Items.Items");
        context.Recorder.Check(
            "Среди совпадений есть Items.Items",
            hitRow is not null,
            searchSummary);
        if (hitRow is not null)
        {
            var cells = hitRow.ReadRowCells();
            var hitLanguage = cells.ElementAtOrDefault(4) ?? string.Empty;
            context.Recorder.Check(
                "Значение Items помечено русским оверлеем",
                string.Equals(hitLanguage, "ru", StringComparison.OrdinalIgnoreCase),
                hitLanguage);
            hitRow.SelectItem();
            Thread.Sleep(300);
        }

        window.WaitById("DatasetOpenHitButton").InvokeButton();
        Ui.WaitUntil(
            () => ScenarioUi.TabNames(context.Window)
                .Any(name => name.Contains("Items", StringComparison.Ordinal)),
            "вкладка Items открыта",
            TimeSpan.FromSeconds(180));
        var allTabs = ScenarioUi.TabNames(context.Window);
        context.Recorder.Check(
            "Из результата поиска открыта вкладка Items",
            allTabs.Any(name => name.Contains("Items", StringComparison.Ordinal)),
            string.Join(" | ", allTabs));

        NativeInput.Activate(new IntPtr(window.Current.NativeWindowHandle));
        window.WaitById("DatasetCloseButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(
            context.App,
            "DatasetValidateButton",
            TimeSpan.FromSeconds(60));

        context.Recorder.Check(
            "Проверка и поиск не изменили данные v4",
            DataOperationsUi.HashTree(v4.TableDataDirectory) == before);
    }

    /// <summary>
    /// Finds a row of a virtualized grid by the text of its first cell, scrolling until the row is
    /// realized.
    /// </summary>
    /// <param name="grid">Data grid.</param>
    /// <param name="firstCellText">Text expected in the first cell.</param>
    /// <returns>The row, or <see langword="null"/> when no row shows that text.</returns>
    private static AutomationElement? FindGridRow(AutomationElement grid, string firstCellText)
    {
        grid.ScrollToTop();
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(20);
        while (DateTime.UtcNow < deadline)
        {
            var row = grid.FindRow(firstCellText);
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
}
