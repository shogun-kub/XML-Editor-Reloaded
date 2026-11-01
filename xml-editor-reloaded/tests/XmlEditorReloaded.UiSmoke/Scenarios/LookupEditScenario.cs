using System.IO;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S35: renaming a lookup entry on the writable copy is written to the lookup file, and a consumer
/// table opened afterwards shows the new name in its lookup field.
/// </summary>
internal sealed class LookupEditScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S35";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Lookup/Cursor.xml");
        var before = XDocument.Parse(File.ReadAllText(file));

        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor открыт",
            TimeSpan.FromSeconds(30));

        var grid = window.WaitById("RowsGrid");
        var rows = grid.RowCount();
        context.Recorder.Check("Справочник Cursor содержит 23 записи", rows == 23, $"строк: {rows}");

        var row = DataOperationsUi.FindRowIndex(grid, "2");
        var original = WorkspaceUi.ReadCell(grid, row, 1).Trim();
        context.Recorder.Check("Строка id=2 называется Punch", original == "Punch", original);

        var save = window.WaitById("SaveButton");
        DataOperationsUi.EditCellUntilDirty(window, grid, row, 1, "Punch Smoke");
        save.InvokeButton();
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorSaved"),
            "справочник сохранён",
            TimeSpan.FromSeconds(30));

        var after = XDocument.Load(file);
        var (onlyField, details) = DataOperationsUi.OnlyFieldChanged(
            before,
            after,
            recordIndex: row,
            field: "name",
            expectedOld: "Punch",
            expectedNew: "Punch Smoke");
        context.Recorder.Check("В справочнике изменено только имя записи id=2", onlyField, details);

        var entries = after.Root!.Elements().ToList();
        var renamed = entries
            .FirstOrDefault(entry => entry.Element("id")?.Value.Trim() == "2")
            ?.Element("name")?.Value.Trim();
        context.Recorder.Check("Имя id=2 записано в файл", renamed == "Punch Smoke", renamed);
        context.Recorder.Check("Число записей справочника не изменилось", entries.Count == 23, $"записей: {entries.Count}");
        context.ExpectChange(DatasetKind.V4, "TableData/Lookup/Cursor.xml");

        // A consumer opened afterwards must resolve the identifier through the updated lookup file.
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "0", "Nada");
        var field = ShowField(context, "ubCursor");
        var selection = field.ReadComboSelection();
        context.Recorder.Check(
            "Потребитель справочника показывает новое имя",
            selection.Contains("Punch Smoke", StringComparison.Ordinal),
            "выбрано: «" + selection + "»");
        context.Recorder.Check(
            "Идентификатор в потребителе не изменился",
            selection.Contains("(2)", StringComparison.Ordinal),
            "выбрано: «" + selection + "»");
    }

    /// <summary>
    /// Realizes a field editor of the item workspace, retrying while the details panel is still rebuilt
    /// after the record selection.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="pathText">Field path and automation identifier.</param>
    /// <returns>The field editor.</returns>
    private static System.Windows.Automation.AutomationElement ShowField(ScenarioContext context, string pathText)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(60);
        while (true)
        {
            try
            {
                return WorkspaceUi.ShowField(context, pathText);
            }
            catch (UiTimeoutException) when (DateTime.UtcNow < deadline)
            {
                Thread.Sleep(500);
            }
        }
    }
}
