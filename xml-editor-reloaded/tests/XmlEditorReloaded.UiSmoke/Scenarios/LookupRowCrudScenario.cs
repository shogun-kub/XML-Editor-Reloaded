using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S80: a new lookup row gets <c>id=max+1</c> and an empty name, the file stays readable after every
/// save, a consumer loads the new entry without <c>xml.invalid_lookup_entry</c>, and the row can be
/// renamed to a duplicate name and deleted again without a usage guard.
/// </summary>
internal sealed class LookupRowCrudScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S80";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Lookup/Cursor.xml");
        var before = XDocument.Load(file);

        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor открыт",
            TimeSpan.FromSeconds(30));
        var grid = window.WaitById("RowsGrid");
        context.Recorder.Check("Справочник содержит 23 записи", grid.RowCount() == 23, $"строк: {grid.RowCount()}");

        window.WaitById("AddRowButton").InvokeButton();
        WaitForRowCount(window, 24);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Format("EditorRowAdded", "24"),
            "статус добавления записи",
            TimeSpan.FromSeconds(20));
        grid.ScrollToBottom();
        context.Recorder.Check(
            "Новая запись получает ключ 24",
            DataOperationsUi.GridCellText(grid, 23, 0) == "24",
            DataOperationsUi.GridCellText(grid, 23, 0));

        window.WaitById("SaveButton").InvokeButton();
        WaitForSaved(context, "первое сохранение справочника");
        var afterAdd = XDocument.Load(file);
        var added = afterAdd.Descendants("Cursor").Last();
        context.Recorder.Check("После сохранения 24 записи", afterAdd.Descendants("Cursor").Count() == 24, null);
        context.Recorder.Check(
            "Новая запись хранит id 24",
            added.Element("id")?.Value.Trim() == "24",
            added.Element("id")?.Value.Trim());
        context.Recorder.Check(
            "Новая запись хранит пустое имя",
            added.Element("name") is not null && added.Element("name")!.Value.Length == 0,
            added.Element("name")?.Value);
        context.Recorder.Check(
            "Прежние записи справочника не изменились",
            LeadingRecordsUnchanged(before, afterAdd, 23),
            null);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor открыт после добавления",
            TimeSpan.FromSeconds(30));
        grid = window.WaitById("RowsGrid");
        context.Recorder.Check("После переоткрытия 24 записи", grid.RowCount() == 24, $"строк: {grid.RowCount()}");
        context.Recorder.Check("Запись 24 находится в конце", DataOperationsUi.FindRowIndex(grid, "24") == 23, null);

        // The new entry has an empty name, so the consumer must still load the lookup without the
        // xml.invalid_lookup_entry error and expose the new identifier in the dropdown list.
        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        var combo = WorkspaceUi.ShowField(context, "ubCursor");
        context.Recorder.Check("Поле ubCursor показывает combo", combo.Current.ControlType == ControlType.ComboBox, null);
        context.Recorder.Check("Combo справочника доступно", combo.IsEnabled(), null);
        context.Recorder.Check("В combo есть Quest", ComboHasItem(combo, text => text.Contains("Quest", StringComparison.Ordinal)), null);
        context.Recorder.Check("В combo есть новая запись 24", ComboHasItem(combo, text => text.Contains("(24)", StringComparison.Ordinal)), null);
        CheckNoLookupError(context);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor открыт для переименования",
            TimeSpan.FromSeconds(30));
        grid = window.WaitById("RowsGrid");
        grid.ScrollToBottom();
        var addedRow = FindRealizedRow(grid, "24");
        DetailsUi.EditField(context, grid, addedRow, "name", "Quest");
        Ui.WaitUntil(
            () => window.WaitById("SaveButton").IsEnabled(),
            "правка имени делает вкладку грязной",
            TimeSpan.FromSeconds(20));
        window.WaitById("SaveButton").InvokeButton();
        WaitForSaved(context, "сохранение имени");

        var afterRename = XDocument.Load(file);
        var questRows = afterRename.Descendants("Cursor")
            .Where(entry => entry.Element("name")?.Value.Trim() == "Quest")
            .ToList();
        context.Recorder.Check("После переименования 24 записи", afterRename.Descendants("Cursor").Count() == 24, null);
        context.Recorder.Check(
            "Дубликат имени Quest разрешён",
            questRows.Count == 2
                && questRows.Any(entry => entry.Element("id")?.Value.Trim() == "1")
                && questRows.Any(entry => entry.Element("id")?.Value.Trim() == "24"),
            $"записей Quest: {questRows.Count}");

        grid.ScrollToBottom();
        FindRealizedRow(grid, "24").SelectItem();
        var delete = window.WaitById("DeleteRowButton");
        _ = Task.Run(() =>
        {
            try
            {
                delete.InvokeButton();
            }
            catch (Exception exception) when (exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The confirmation closed the dialog and the button disappeared before the invoke returned.
            }
        });
        var messageBox = ScenarioUi.WaitForMessageBox(context.App, TimeSpan.FromSeconds(20));
        var message = ScenarioUi.ReadMessageBoxText(messageBox);
        context.Recorder.Check(
            "Подтверждение удаления называет запись 24",
            message.Contains(context.Strings.Format("EditorConfirmDelete", "24"), StringComparison.Ordinal),
            message);
        ScenarioUi.AnswerMessageBox(messageBox, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        WaitForRowCount(window, 23);
        window.WaitById("SaveButton").InvokeButton();
        WaitForSaved(context, "сохранение после удаления");

        var final = XDocument.Load(file);
        context.Recorder.Check("После удаления 23 записи", final.Descendants("Cursor").Count() == 23, null);
        context.Recorder.Check(
            "Записи 24 больше нет",
            final.Descendants("Cursor").All(entry => entry.Element("id")?.Value.Trim() != "24"),
            null);
        context.Recorder.Check(
            "Исходные записи справочника восстановлены",
            LeadingRecordsUnchanged(before, final, 23),
            null);

        context.Recorder.Note(
            "Пустое имя сохранено и загружено потребителем без xml.invalid_lookup_entry; "
            + "дубликат имени Quest разрешён, удаление не предупредило об использовании записи. "
            + "После удаления файл вернулся к исходному содержимому, поэтому изменение не заявляется.");
    }

    private static AutomationElement FindRealizedRow(AutomationElement grid, string key) =>
        grid.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .FirstOrDefault(row => row.ReadRowCells().Contains(key))
        ?? throw new UiTimeoutException($"Строка с ключом «{key}» не реализована в сетке.");

    private static void WaitForRowCount(AutomationElement window, int expected) =>
        Ui.WaitUntil(
            () => window.WaitById("RowsGrid").RowCount() == expected,
            $"в сетке {expected} строк",
            TimeSpan.FromSeconds(20));

    private static void WaitForSaved(ScenarioContext context, string description) =>
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorSaved"),
            description,
            TimeSpan.FromSeconds(30));

    private static bool ComboHasItem(AutomationElement combo, Func<string, bool> predicate)
    {
        combo.ExpandCombo();
        try
        {
            var scroller = combo.FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List))
                ?? combo;
            scroller.ScrollToTop();
            for (var step = 0; step < 30; step++)
            {
                if (combo
                    .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem))
                    .Any(item => predicate(Ui.ItemText(item))))
                {
                    return true;
                }

                if (!scroller.ScrollDown())
                {
                    break;
                }
            }

            return false;
        }
        finally
        {
            ScenarioUi.Collapse(combo);
        }
    }

    private static void CheckNoLookupError(ScenarioContext context)
    {
        var window = context.Window;
        var texts = window
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .Where(static text => !string.IsNullOrEmpty(text))
            .ToList();
        var error = texts.FirstOrDefault(
            text => text.Contains("xml.invalid_lookup_entry", StringComparison.Ordinal));
        context.Recorder.Check("Нет ошибки xml.invalid_lookup_entry", error is null, error);
    }

    private static bool LeadingRecordsUnchanged(XDocument before, XDocument after, int count)
    {
        var expected = before.Descendants("Cursor").Take(count).ToList();
        var actual = after.Descendants("Cursor").Take(count).ToList();
        return expected.Count == count
            && actual.Count == count
            && expected.Zip(actual).All(pair => XNode.DeepEquals(pair.First, pair.Second));
    }
}
