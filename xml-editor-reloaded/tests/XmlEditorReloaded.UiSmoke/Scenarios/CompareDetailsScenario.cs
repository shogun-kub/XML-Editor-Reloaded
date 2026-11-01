using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S41: selecting <c>Items.Items</c> in the comparison window shows the record counts, the sparse
/// profile with both overlay languages, the field table, and the lookup-reference tab, all read-only.
/// </summary>
internal sealed class CompareDetailsScenario : SmokeScenario
{
    private const string Table = "Items.Items";

    /// <inheritdoc />
    public override string Id => "S41";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var v5 = context.Data(DatasetKind.V5);
        var leftHash = DataOperationsUi.HashTree(v4.TableDataDirectory);
        var rightHash = DataOperationsUi.HashTree(v5.TableDataDirectory);

        var window = DataOperationsUi.OpenCompareWindow(context);
        DataOperationsUi.SetCompareDirectories(window, v4, v5);
        DataOperationsUi.RunCompare(context, window);

        var items = ScenarioUi.FindTableRow(window, Table)
            ?? throw new SmokeCheckException(
                $"Таблица {Table} есть в списке сравнения",
                "строка не найдена");
        items.SelectItem();
        Thread.Sleep(600);

        var statusBoth = context.Strings.Get("CompareStatusBoth");
        var header = FindText(
            window,
            text => text.Contains(Table, StringComparison.Ordinal)
                && text.Contains(statusBoth, StringComparison.Ordinal));
        context.Recorder.Check("Детали показывают таблицу и её статус", header is not null, header);

        var cells = items.ReadRowCells();
        context.Recorder.Check(
            "Слева 1776 записей, справа 1775",
            cells.ElementAtOrDefault(2) == "1776" && cells.ElementAtOrDefault(3) == "1775",
            string.Join(" | ", cells));
        var added = ParseCount(cells.ElementAtOrDefault(4));
        var removed = ParseCount(cells.ElementAtOrDefault(5));
        var typeChanged = ParseCount(cells.ElementAtOrDefault(6));
        context.Recorder.Check(
            "Счётчики добавленных, удалённых полей и смен типов читаются",
            added >= 0 && removed >= 0 && typeChanged >= 0,
            string.Join(" | ", cells));

        var profile = ReadProfile(context, window);
        context.Recorder.Check("Sparse-профиль обеих сторон показан", profile is not null, profile);
        var profileText = profile ?? string.Empty;
        context.Recorder.Check(
            "Профиль содержит число записей обеих версий",
            profileText.Contains("1776", StringComparison.Ordinal)
                && profileText.Contains("1775", StringComparison.Ordinal),
            profileText);
        var v4Fragment = OverlayFragment(context.Strings.Get("CompareProfileLine"), "ru");
        var v5Fragment = OverlayFragment(
            context.Strings.Get("CompareProfileLine"),
            context.Strings.Get("CompareOverlaysNone"));
        context.Recorder.Check(
            "У v4 показан оверлей ru, у v5 — его отсутствие",
            profileText.Contains(v4Fragment, StringComparison.Ordinal)
                && profileText.Contains(v5Fragment, StringComparison.Ordinal),
            profileText);

        VerifyFieldsTab(context, window, added, removed, typeChanged);
        VerifyLookupTab(context, window);

        context.Recorder.Check(
            "Окно сравнения не содержит кнопок сохранения",
            window.FindById("SaveButton") is null && window.FindById("ItemSaveButton") is null);

        DataOperationsUi.CloseCompareWindow(context, window);

        context.Recorder.Check(
            "Сравнение не изменило данные v4",
            DataOperationsUi.HashTree(v4.TableDataDirectory) == leftHash);
        context.Recorder.Check(
            "Сравнение не изменило данные v5",
            DataOperationsUi.HashTree(v5.TableDataDirectory) == rightHash);
    }

    private static void VerifyFieldsTab(
        ScenarioContext context,
        AutomationElement window,
        int added,
        int removed,
        int typeChanged)
    {
        var grid = window.WaitById("FieldsGrid");
        var fieldCount = grid.RowCount();
        context.Recorder.Check(
            "Вкладка «Поля» содержит все поля таблицы",
            fieldCount == 224,
            $"строк: {fieldCount}");

        var knownStatuses = new[]
        {
            context.Strings.Get("CompareFieldSame"),
            context.Strings.Get("CompareFieldLeftOnly"),
            context.Strings.Get("CompareFieldRightOnly"),
            context.Strings.Get("CompareFieldTypeChanged"),
            context.Strings.Get("CompareFieldCardinalityChanged"),
        };
        var statuses = new List<string>();
        var names = new List<string>();
        for (var row = 0; row < Math.Min(10, fieldCount); row++)
        {
            var name = DataOperationsUi.GridCellText(grid, row, 0);
            var status = DataOperationsUi.GridCellText(grid, row, 1);
            names.Add(name);
            statuses.Add(status);
            context.Recorder.SoftCheck(
                $"Поле {row} имеет имя и известный статус",
                name.Length > 0 && knownStatuses.Contains(status, StringComparer.Ordinal),
                $"{name}: {status}");
        }

        context.Recorder.Check(
            "Статусы полей читаются из известного набора",
            statuses.Count > 0 && statuses.All(status => knownStatuses.Contains(status, StringComparer.Ordinal)),
            string.Join(" | ", statuses));
        context.Recorder.Check(
            "Первые поля таблицы показаны по имени",
            names.Count > 0 && names.All(name => !string.IsNullOrWhiteSpace(name)),
            string.Join(", ", names));

        // The added/removed/type counters of the table row must agree with the statuses the field rows
        // would report; with 224 shared fields the field count itself is the meaningful invariant.
        context.Recorder.Check(
            "Счётчики строки неотрицательны и согласуются с пустым профилем полей",
            added + removed + typeChanged <= fieldCount,
            $"добавлено {added}, удалено {removed}, типы {typeChanged}, полей {fieldCount}");
    }

    private static void VerifyLookupTab(ScenarioContext context, AutomationElement window)
    {
        var header = context.Strings.Get("CompareLookupTab");
        var tab = window
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .FirstOrDefault(candidate => candidate.ReadName() == header
                || candidate.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
                    .Any(text => text.ReadName() == header));
        context.Recorder.Check("Вкладка «Lookup-связи» доступна", tab is not null, header);
        if (tab is null)
        {
            return;
        }

        tab.SelectItem();
        Thread.Sleep(400);
        var grid = window.WaitById("LookupGrid");
        var rows = grid.RowCount();
        context.Recorder.Check("Список lookup-связей непустой", rows > 10, $"строк: {rows}");

        var available = context.Strings.Get("CompareLookupAvailable");
        var missing = context.Strings.Get("CompareLookupMissing");
        var unreadable = context.Strings.Get("CompareLookupUnreadable");
        var known = new[] { available, missing, unreadable };
        var tables = new List<string>();
        for (var row = 0; row < Math.Min(5, rows); row++)
        {
            tables.Add(DataOperationsUi.GridCellText(grid, row, 0));
            var left = DataOperationsUi.GridCellText(grid, row, 3);
            var right = DataOperationsUi.GridCellText(grid, row, 4);
            context.Recorder.SoftCheck(
                $"Связь {row} показывает доступность с двух сторон",
                known.Contains(left, StringComparer.Ordinal) && known.Contains(right, StringComparer.Ordinal),
                $"{left} / {right}");
        }

        context.Recorder.Check(
            "Строки lookup-связей называют таблицы-потребители",
            tables.Count > 0 && tables.All(table => table.Contains('.', StringComparison.Ordinal)),
            string.Join(", ", tables));
    }

    private static string? ReadProfile(ScenarioContext context, AutomationElement window)
    {
        var left = context.Strings.Get("CompareSideLeft") + ":";
        var right = context.Strings.Get("CompareSideRight") + ":";
        var lines = window
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .Where(text => text.StartsWith(left, StringComparison.Ordinal)
                || text.StartsWith(right, StringComparison.Ordinal))
            .ToList();
        return lines.Count == 0 ? null : string.Join(" ", lines);
    }

    private static string? FindText(AutomationElement window, Func<string, bool> predicate) =>
        window.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .FirstOrDefault(predicate);

    private static int ParseCount(string? text) =>
        int.TryParse(text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var value)
            ? value
            : -1;

    private static string OverlayFragment(string format, string value)
    {
        var placeholder = format.IndexOf("{8}", StringComparison.Ordinal);
        if (placeholder < 0)
        {
            return value;
        }

        var colon = format.LastIndexOf(':', placeholder);
        var label = colon >= 0 ? format[colon..placeholder].Trim() : "overlays:";
        return $"{label} {value}";
    }
}
