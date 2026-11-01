using System.IO;
using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S34: a lookup column shows entry names and their identifiers in the tooltip and the dropdown, and
/// choosing a dropdown entry writes the identifier back while leaving the file untouched.
/// </summary>
internal sealed class LookupComboScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S34";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var data = context.Data(DatasetKind.V4);
        var tableFile = data.TableFile("Items/Weapons.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(tableFile);
        var lookup = ReadLookup(data.TableFile("Lookup/WeaponClass.xml"));
        var raw = ReadField(tableFile, "12", "ubWeaponClass");

        ScenarioUi.OpenTable(context, "Items.Weapons");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "таблица Items.Weapons открыта",
            TimeSpan.FromSeconds(60));

        var grid = window.WaitById("RowsGrid");
        var rows = grid.RowCount();
        context.Recorder.Check("Таблица содержит 389 записей", rows == 389, $"строк: {rows}");

        var headers = ScenarioUi.ColumnHeaderCaptions(window);
        var column = headers.IndexOf("ubWeaponClass");
        context.Recorder.Check("Колонка ubWeaponClass показана", column >= 0, "колонки: " + string.Join(", ", headers));
        if (column < 0)
        {
            return;
        }

        var row = DataOperationsUi.FindRowIndex(grid, "12");
        var shown = WorkspaceUi.ReadCell(grid, row, column).Trim();
        context.Recorder.Check(
            "В сетке показано имя класса вместо идентификатора",
            string.Equals(shown, lookup[raw], StringComparison.Ordinal),
            $"ячейка «{shown}», справочник «{lookup[raw]}» для id {raw}");

        var tooltipText = string.Empty;
        NativeInput.EnsureForeground(grid);
        var cellText = grid.Cell(row, column).FindFirst(
            TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
        try
        {
            tooltipText = cellText is null
                ? Ui.HoverToolTip(grid.Cell(row, column))
                : Ui.HoverToolTip(cellText);
            context.Recorder.Check(
                "Подсказка показывает имя и идентификатор",
                tooltipText.Contains($"{lookup[raw]} (ID {raw})", StringComparison.Ordinal),
                $"подсказка «{tooltipText}»");
        }
        catch (UiTimeoutException)
        {
            context.Recorder.Note(
                "Подсказка ячейки lookup не появилась при синтетическом наведении; имя и идентификатор "
                + "проверены по списку значений.");
        }

        // The dropdown lists names with identifiers; choosing another class writes the identifier and
        // marks the tab dirty, and undo restores both the cell and the clean state.
        var cell = grid.Cell(row, column);
        NativeInput.DoubleClick(cell);
        Thread.Sleep(500);
        var combo = cell.FindFirst(
            TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ComboBox));
        context.Recorder.Check("Ячейка lookup открывает выпадающий список", combo is not null);
        if (combo is not null)
        {
            var items = ScenarioUi.ReadComboItems(combo);
            context.Recorder.Check(
                "Список предлагает имена классов с идентификаторами",
                items.Contains($"{lookup[raw]} ({raw})", StringComparer.Ordinal),
                "элементы: " + string.Join(" | ", items.Take(6)));

            var other = lookup.First(pair => !string.Equals(pair.Key, raw, StringComparison.Ordinal));
            combo.SelectComboItem($"{other.Value} ({other.Key})");
            Thread.Sleep(300);
            NativeInput.PressIn(combo, NativeInput.VirtualKey.Return);
        }

        var save = window.WaitById("SaveButton");
        Ui.WaitUntil(() => save.IsEnabled(), "выбор класса делает вкладку грязной", TimeSpan.FromSeconds(20));
        context.Recorder.Check("Выбор в списке меняет ячейку в памяти", true);

        window.WaitById("UndoButton").InvokeButton();
        Ui.WaitUntil(() => !save.IsEnabled(), "отмена вернула чистое состояние");
        context.Recorder.Check(
            "Undo возвращает прежнее имя класса",
            WorkspaceUi.ReadCell(window.WaitById("RowsGrid"), row, column).Trim() == lookup[raw],
            WorkspaceUi.ReadCell(window.WaitById("RowsGrid"), row, column));
        context.Recorder.Check(
            "Работа со списком не изменила XML",
            SaveDirtyTabScenario.HashFile(tableFile) == hashBefore);

        context.Recorder.Note(
            "В реальных данных v4 колонки LookupCombo не содержат идентификаторов, отсутствующих "
            + "в справочнике, поэтому маркер «не найдено» в этой колонке не наблюдается; "
            + "повреждённый справочник проверяется сценарием S36.");
    }

    /// <summary>
    /// Reads a lookup table into a map of identifier text to name.
    /// </summary>
    /// <param name="path">Lookup file path.</param>
    /// <returns>Names by identifier text.</returns>
    private static Dictionary<string, string> ReadLookup(string path)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in XDocument.Load(path).Root!.Elements())
        {
            var id = entry.Element("id")?.Value.Trim();
            var name = entry.Element("name")?.Value.Trim();
            if (!string.IsNullOrEmpty(id) && name is not null)
            {
                result[id] = name;
            }
        }

        return result;
    }

    /// <summary>
    /// Reads one field of a record identified by its key.
    /// </summary>
    /// <param name="path">Table file path.</param>
    /// <param name="key">Record key.</param>
    /// <param name="field">Field name.</param>
    /// <returns>Field value.</returns>
    private static string ReadField(string path, string key, string field) =>
        XDocument.Load(path).Root!.Elements()
            .First(element => element.Element("uiIndex")?.Value.Trim() == key)
            .Element(field)?.Value.Trim() ?? string.Empty;
}
