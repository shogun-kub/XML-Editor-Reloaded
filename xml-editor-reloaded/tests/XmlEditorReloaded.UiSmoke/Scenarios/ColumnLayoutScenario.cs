using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S11: column search, visibility, defaults, resizing, reordering, and sorting are persisted in
/// <c>layouts.json</c>, survive a tab reopen, and are reset without touching the XML.
/// </summary>
internal sealed class ColumnLayoutScenario : SmokeScenario
{
    private const string Table = "Items.Weapons";

    /// <inheritdoc />
    public override string Id => "S11";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/Weapons.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);
        // The weapon table has 389 rows, but uiIndex values are global item indexes and are neither
        // contiguous nor dense; the expected sort ends come from the file itself.
        var weaponIndexes = ReadWeaponIndexes(file);
        var descendingFirst = weaponIndexes.Max().ToString(CultureInfo.InvariantCulture);
        var fileOrderFirst = weaponIndexes[0].ToString(CultureInfo.InvariantCulture);

        ScenarioUi.OpenTable(context, Table);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            $"{Table} открыт");

        // The column list and its counter live inside a collapsed expander, and a wide table realizes only
        // the visible column headers. The persisted layout is therefore the source of truth for the column
        // count and order, while the headers are used only for the realized prefix.
        //
        // Opening a table does not write a layout: the file is created by the first layout change. The
        // explicit default restores the very set that a fresh table already shows, so the scenario gets a
        // persisted baseline without altering the state it is about to verify.
        ScenarioUi.SetExpanded(window.WaitById("ColumnsExpander"), expanded: true);
        window.WaitById("DefaultColumnsButton").InvokeButton();
        var initialLayout = WaitForLayout(
            context,
            layout => layout.HasEntry && layout.Columns.Count > 0,
            "раскладка Items.Weapons сохранена");
        var initialNames = initialLayout.VisibleNames();
        var initialCounter = ReadColumnCounter(window);
        context.Recorder.Check(
            "Счётчик колонок совпадает с раскладкой",
            initialCounter.Visible == initialLayout.VisibleCount
                && initialCounter.Total == initialLayout.Columns.Count,
            $"счётчик: {initialCounter.Visible} из {initialCounter.Total}, "
                + $"раскладка: {initialLayout.VisibleCount} из {initialLayout.Columns.Count}");
        context.Recorder.Check("Колонка ubCalibre видна по умолчанию", initialNames.Contains("ubCalibre"), null);

        // Column search narrows the list, and clearing the checkbox hides the column.
        SearchColumn(window, "ubCalibre");
        ColumnCheckBox(window).SetChecked(false);
        Ui.WaitUntil(
            () => !ScenarioUi.ColumnHeaderCaptions(window).Contains("ubCalibre"),
            "колонка ubCalibre скрыта");
        var counterAfterHide = ReadColumnCounter(window);
        context.Recorder.Check(
            "Снятый флажок скрывает колонку и уменьшает счётчик",
            counterAfterHide.Visible == initialCounter.Visible - 1,
            $"Колонок: {counterAfterHide.Visible} из {counterAfterHide.Total}");
        WaitForLayout(
            context,
            layout => layout.TryGetColumn("ubCalibre") is { Visible: false },
            "layouts.json сохраняет скрытую колонку");

        window.WaitById("ColumnSearchBox").SetText(string.Empty);
        window.WaitById("AllColumnsButton").InvokeButton();
        Ui.WaitUntil(
            () => ReadColumnCounter(window).Visible == initialCounter.Total,
            "кнопка «Все» показывает все колонки");
        var all = ReadColumnCounter(window);
        context.Recorder.Check(
            "«Все» показывает все колонки",
            all.Visible == all.Total && all.Total == initialCounter.Total,
            $"Колонок: {all.Visible} из {all.Total}");

        window.WaitById("DefaultColumnsButton").InvokeButton();
        WaitForLayout(
            context,
            layout => layout.VisibleNames().SequenceEqual(initialNames, StringComparer.Ordinal),
            "«По умолчанию» возвращает исходный набор колонок");
        context.Recorder.Check("«По умолчанию» возвращает исходный набор", true, null);

        // Hide the column again so the layout that is reopened carries a non-default state.
        SearchColumn(window, "ubCalibre");
        ColumnCheckBox(window).SetChecked(false);
        window.WaitById("ColumnSearchBox").SetText(string.Empty);
        Ui.WaitUntil(
            () => !ScenarioUi.ColumnHeaderCaptions(window).Contains("ubCalibre"),
            "колонка ubCalibre снова скрыта");

        // Sorting through the column header is persisted.
        ScenarioUi.WaitForColumnHeader(window, "uiIndex").InvokeButton();
        Thread.Sleep(400);
        ScenarioUi.WaitForColumnHeader(window, "uiIndex").InvokeButton();
        var sorted = WaitForLayout(
            context,
            layout => layout.SortField == "uiIndex" && layout.SortDirection == "descending",
            "layouts.json сохраняет сортировку по uiIndex");
        context.Recorder.Check(
            "Сортировка по убыванию сохранена",
            sorted.SortField == "uiIndex" && sorted.SortDirection == "descending",
            $"{sorted.SortField} {sorted.SortDirection}");
        var uiIndexColumn = ColumnIndex(initialLayout, "uiIndex");
        Ui.WaitUntil(
            () => window.WaitById("RowsGrid").Cell(0, uiIndexColumn).ReadName() == descendingFirst,
            "первая строка отсортирована по убыванию");

        // Resizing the header gripper changes the persisted width.
        var widthBefore = sorted.TryGetColumn("szWeaponName")?.Width ?? 0;
        var nameHeader = ScenarioUi.WaitForColumnHeader(window, "szWeaponName");
        NativeInput.DragRightEdge(nameHeader, 60);
        var resized = WaitForLayout(
            context,
            layout => (layout.TryGetColumn("szWeaponName")?.Width ?? 0) > widthBefore + 20,
            "layouts.json сохраняет новую ширину колонки");
        var resizedWidth = resized.TryGetColumn("szWeaponName")?.Width ?? 0;
        context.Recorder.Check(
            "Ширина колонки увеличена и сохранена",
            resizedWidth > widthBefore,
            $"{widthBefore:0} → {resizedWidth:0}");

        // Reordering the headers changes the display order. The drop point is the left quarter of the
        // target header: dropping on its center would insert the column after it, which is where it already
        // stands. The automation tree keeps the column creation order after a reorder, so the headers are
        // compared by their screen position.
        var sourceHeader = ScenarioUi.WaitForColumnHeader(window, "szWeaponName");
        var targetHeader = ScenarioUi.WaitForColumnHeader(window, "uiIndex");
        NativeInput.Drag(sourceHeader, targetHeader, targetFraction: 0.25);
        Ui.WaitUntil(
            () =>
            {
                var order = VisualHeaderOrder(window);
                var moved = order.IndexOf("szWeaponName");
                return moved >= 0 && moved < order.IndexOf("uiIndex");
            },
            "колонка szWeaponName перенесена перед uiIndex");
        var reordered = WaitForLayout(
            context,
            layout => layout.TryGetColumn("szWeaponName") is { } column
                && layout.TryGetColumn("uiIndex") is { } key
                && column.Order < key.Order,
            "layouts.json сохраняет порядок колонок");
        context.Recorder.Check(
            "Порядок колонок сохранён",
            reordered.TryGetColumn("szWeaponName")!.Value.Order
                < reordered.TryGetColumn("uiIndex")!.Value.Order,
            $"szWeaponName: {reordered.TryGetColumn("szWeaponName")!.Value.Order}, "
                + $"uiIndex: {reordered.TryGetColumn("uiIndex")!.Value.Order}");

        // Reopening the table restores visibility, width, order, and sort.
        ScenarioUi.CloseTab(window, Table);
        ScenarioUi.WaitForTabClosed(window, Table);
        ScenarioUi.OpenTable(context, Table);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            $"{Table} открыт повторно");
        Ui.WaitUntil(
            () =>
            {
                var order = VisualHeaderOrder(window);
                var moved = order.IndexOf("szWeaponName");
                return moved >= 0 && moved < order.IndexOf("uiIndex");
            },
            "порядок колонок восстановлен после повторного открытия");
        var reopenedHeaders = VisualHeaderOrder(window);
        context.Recorder.Check(
            "Скрытая колонка осталась скрытой",
            !reopenedHeaders.Contains("ubCalibre"),
            string.Join(", ", reopenedHeaders.Take(5)));
        var reopenedWidth = ScenarioUi.WaitForColumnHeader(window, "szWeaponName").Current.BoundingRectangle.Width;
        context.Recorder.Check(
            "Ширина колонки восстановлена",
            Math.Abs(reopenedWidth - resizedWidth) < 25,
            $"{reopenedWidth:0} против {resizedWidth:0}");
        var reopenedUiIndex = reopenedHeaders.IndexOf("uiIndex");
        Ui.WaitUntil(
            () => window.WaitById("RowsGrid").Cell(0, reopenedUiIndex).ReadName() == descendingFirst,
            "сортировка восстановлена после повторного открытия");
        context.Recorder.Check("Сортировка восстановлена", true, null);

        // Reset removes the stored layout and restores defaults.
        ScenarioUi.SetExpanded(window.WaitById("FilterExpander"), expanded: true);
        ScenarioUi.OpenMenuItem(context, "ViewMenu", "ResetLayoutButton");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorLayoutResetDone"),
            "статус сброса раскладки");
        // The header presenter virtualizes its columns and may realize a non-contiguous subset, so only the
        // first headers are compared: they prove that the default order is back and that the hidden column
        // is visible again. The counter and the removed layout entry cover the rest.
        IReadOnlyList<string> resetHeaders = [];
        Ui.WaitUntil(
            () =>
            {
                resetHeaders = VisualHeaderOrder(window);
                const int RealizedPrefix = 5;
                return resetHeaders.Count >= RealizedPrefix
                    && resetHeaders.Take(RealizedPrefix).SequenceEqual(initialNames.Take(RealizedPrefix));
            },
            "сброс раскладки вернул исходные колонки");
        context.Recorder.Check(
            "Сброс раскладки возвращает исходные колонки",
            resetHeaders.Take(5).SequenceEqual(initialNames.Take(5)),
            string.Join(", ", resetHeaders));
        context.Recorder.Check(
            "Сброс раскладки снова показывает скрытую колонку",
            resetHeaders.Contains("ubCalibre"),
            string.Join(", ", resetHeaders));
        var resetWidth = ScenarioUi.WaitForColumnHeader(window, "szWeaponName").Current.BoundingRectangle.Width;
        context.Recorder.Check(
            "Сброс раскладки возвращает исходную ширину колонки",
            Math.Abs(resetWidth - widthBefore) < 5,
            $"{resetWidth:0} против {widthBefore:0}");
        ScenarioUi.SetExpanded(window.WaitById("ColumnsExpander"), expanded: true);
        var resetCounter = ReadColumnCounter(window);
        context.Recorder.Check(
            "Сброс раскладки возвращает исходное число колонок",
            resetCounter.Visible == initialCounter.Visible && resetCounter.Total == initialCounter.Total,
            $"Колонок: {resetCounter.Visible} из {resetCounter.Total}");
        Ui.WaitUntil(
            () => window.WaitById("RowsGrid").Cell(0, uiIndexColumn).ReadName() == fileOrderFirst,
            "сброс раскладки убрал сортировку");
        WaitForLayout(context, layout => !layout.HasEntry, "layouts.json больше не содержит таблицу");
        context.Recorder.Check("Запись таблицы удалена из layouts.json", true, null);
        context.Recorder.Check(
            "Работа с колонками и раскладкой не изменила XML",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);
    }

    /// <summary>
    /// Returns the captions of the realized column headers in screen order. After a column reorder the
    /// automation tree keeps the column creation order, so the visual position is the reliable order.
    /// </summary>
    /// <param name="window">Main window.</param>
    /// <returns>Realized header captions from left to right.</returns>
    private static List<string> VisualHeaderOrder(AutomationElement window) =>
        window.WaitById("RowsGrid")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.HeaderItem))
            .Select(static header => (Header: header, Caption: header.ReadName()))
            .Where(static pair => !string.IsNullOrEmpty(pair.Caption))
            .OrderBy(static pair => pair.Header.Current.BoundingRectangle.Left)
            .Select(static pair => pair.Caption)
            .ToList();

    private static void SearchColumn(AutomationElement window, string name)
    {
        window.WaitById("ColumnSearchBox").SetText(name);
        Ui.WaitUntil(
            () => window.WaitById("ColumnsList")
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
                .Count == 1,
            $"поиск колонки «{name}» оставляет одну строку");
    }

    private static AutomationElement ColumnCheckBox(AutomationElement window) =>
        window.WaitById("ColumnsList")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
            .Single()
            .FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.CheckBox))
        ?? throw new UiTimeoutException("Флажок колонки не найден.");

    private static IReadOnlyList<long> ReadWeaponIndexes(string file)
    {
        var root = XDocument.Load(file).Root
            ?? throw new UiTimeoutException($"Файл «{file}» не содержит корневого элемента.");
        return root
            .Elements()
            .Select(static element => element.Element("uiIndex")?.Value)
            .Where(static value => value is not null)
            .Select(static value => long.Parse(value!, CultureInfo.InvariantCulture))
            .ToList();
    }

    private static int ColumnIndex(LayoutSnapshot layout, string name)
    {
        var index = layout.VisibleNames().IndexOf(name);
        if (index < 0)
        {
            throw new UiTimeoutException($"Колонка «{name}» отсутствует в раскладке.");
        }

        return index;
    }

    private static readonly Regex NumberPattern = new(
        @"\d+",
        RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(2));

    private static (int Visible, int Total) ReadColumnCounter(AutomationElement window)
    {
        var text = window.WaitById("VisibleColumnCount").ReadName();
        var numbers = NumberPattern.Matches(text)
            .Select(match => int.Parse(match.Value, System.Globalization.CultureInfo.InvariantCulture))
            .ToList();
        return numbers.Count >= 2 ? (numbers[0], numbers[1]) : (0, 0);
    }

    private static LayoutSnapshot WaitForLayout(
        ScenarioContext context,
        Func<LayoutSnapshot, bool> predicate,
        string description)
    {
        var path = Path.Combine(context.App.AppDataDirectory, "layouts.json");
        LayoutSnapshot? snapshot = null;
        Ui.WaitUntil(
            () =>
            {
                snapshot = TryReadLayout(path);
                return snapshot is not null && predicate(snapshot);
            },
            description,
            TimeSpan.FromSeconds(20));
        return snapshot!;
    }

    private static LayoutSnapshot? TryReadLayout(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using var document = JsonDocument.Parse(File.ReadAllText(path));
            if (!document.RootElement.TryGetProperty("layouts", out var layouts))
            {
                return new LayoutSnapshot(false, new Dictionary<string, ColumnState>(StringComparer.Ordinal), null, null);
            }

            foreach (var entry in layouts.EnumerateArray())
            {
                if (!entry.TryGetProperty("table", out var table)
                    || !string.Equals(table.GetString(), Table, StringComparison.Ordinal))
                {
                    continue;
                }

                var columns = new Dictionary<string, ColumnState>(StringComparer.Ordinal);
                if (entry.TryGetProperty("columns", out var columnArray))
                {
                    foreach (var column in columnArray.EnumerateArray())
                    {
                        if (!column.TryGetProperty("name", out var name) || name.GetString() is not { } field)
                        {
                            continue;
                        }

                        columns[field] = new ColumnState(
                            column.TryGetProperty("isVisible", out var visible) && visible.GetBoolean(),
                            column.TryGetProperty("width", out var width) ? width.GetDouble() : 0,
                            column.TryGetProperty("displayOrder", out var order) ? order.GetInt32() : 0);
                    }
                }

                string? sortField = null;
                string? sortDirection = null;
                if (entry.TryGetProperty("sort", out var sort) && sort.ValueKind == JsonValueKind.Object)
                {
                    sortField = sort.TryGetProperty("field", out var field) ? field.GetString() : null;
                    sortDirection = sort.TryGetProperty("direction", out var direction)
                        ? direction.GetString()
                        : null;
                }

                return new LayoutSnapshot(true, columns, sortField, sortDirection);
            }

            return new LayoutSnapshot(false, new Dictionary<string, ColumnState>(StringComparer.Ordinal), null, null);
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private sealed record LayoutSnapshot(
        bool HasEntry,
        IReadOnlyDictionary<string, ColumnState> Columns,
        string? SortField,
        string? SortDirection)
    {
        public int VisibleCount => Columns.Values.Count(static column => column.Visible);

        public ColumnState? TryGetColumn(string name) =>
            Columns.TryGetValue(name, out var column) ? column : null;

        public List<string> VisibleNames() => Columns
            .Where(static pair => pair.Value.Visible)
            .OrderBy(static pair => pair.Value.Order)
            .Select(static pair => pair.Key)
            .ToList();
    }

    private readonly record struct ColumnState(bool Visible, double Width, int Order);
}
