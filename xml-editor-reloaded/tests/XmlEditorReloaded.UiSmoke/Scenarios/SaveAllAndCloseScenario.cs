using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S14: "Save all" writes every dirty tab, and closing the window asks about each dirty tab, keeps the
/// application open on "Cancel", saves on "Save", and never asks about clean tabs.
/// </summary>
internal sealed class SaveAllAndCloseScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S14";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var data = context.Data(DatasetKind.V4);
        var cursorFile = data.TableFile("Lookup/Cursor.xml");
        var itemClassFile = data.TableFile("Lookup/ItemClass.xml");
        var weaponClassFile = data.TableFile("Lookup/WeaponClass.xml");
        var cursorBefore = SaveDirtyTabScenario.HashFile(cursorFile);
        var itemClassBefore = SaveDirtyTabScenario.HashFile(itemClassFile);
        var weaponClassBefore = SaveDirtyTabScenario.HashFile(weaponClassFile);
        var cursorNamesBefore = CountElements(cursorFile, "name");
        var itemClassNamesBefore = CountElements(itemClassFile, "name");

        OpenAndEdit(context, "Lookup.Cursor", "Save All Cursor");
        OpenAndEdit(context, "Lookup.ItemClass", "Save All ItemClass");
        ScenarioUi.OpenTable(context, "Lookup.WeaponClass");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.WeaponClass открыт");

        var saveAll = window.WaitById("SaveAllButton");
        context.Recorder.Check("«Сохранить всё» доступна", saveAll.IsEnabled(), null);
        saveAll.InvokeButton();
        var summary = context.Strings.Format("EditorSaveAllSummary", 2);
        var status = ScenarioUi.WaitForStatus(
            context,
            text => text == summary,
            "статус сохранения всех таблиц",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check("Статус сообщает о двух сохранённых таблицах", status == summary, status);

        var tabs = ScenarioUi.TabNames(window);
        context.Recorder.Check(
            "После «Сохранить всё» вкладки чистые",
            tabs.All(name => !name.Contains('*')),
            "вкладки: " + string.Join(" / ", tabs));
        var save = window.WaitById("SaveButton");
        context.Recorder.Check(
            "Кнопка сохранения активной вкладки неактивна",
            !save.IsEnabled(),
            $"IsEnabled: {save.IsEnabled()}");
        context.Recorder.Check(
            "Файл Lookup.Cursor записан",
            SaveDirtyTabScenario.HashFile(cursorFile) != cursorBefore,
            null);
        context.Recorder.Check(
            "Файл Lookup.ItemClass записан",
            SaveDirtyTabScenario.HashFile(itemClassFile) != itemClassBefore,
            null);
        context.ExpectChange(DatasetKind.V4, "TableData/Lookup/Cursor.xml");
        context.ExpectChange(DatasetKind.V4, "TableData/Lookup/ItemClass.xml");

        var cursorAfterSaveAll = SaveDirtyTabScenario.HashFile(cursorFile);
        var itemClassAfterSaveAll = SaveDirtyTabScenario.HashFile(itemClassFile);

        // Dirty two tabs again; the third tab stays clean and must not be asked about.
        ScenarioUi.ActivateTab(window, "Lookup.Cursor");
        window.WaitById("RowsGrid").EditCell(0, 1, "Close Cursor");
        ScenarioUi.ActivateTab(window, "Lookup.ItemClass");
        window.WaitById("RowsGrid").EditCell(0, 1, "Close ItemClass");
        Ui.WaitUntil(() => window.WaitById("SaveButton").IsEnabled(), "вкладка ItemClass стала грязной");

        // "Cancel" keeps the application and the edits.
        context.App.RequestClose();
        var cancelDialog = ScenarioUi.WaitForDialog(context.App, TimeSpan.FromSeconds(20));
        var cancelMessage = cancelDialog.WaitById("MessageText").ReadName();
        context.Recorder.Check(
            "Диалог закрытия называет первую грязную вкладку",
            cancelMessage.Contains("Lookup.Cursor", StringComparison.Ordinal),
            cancelMessage);
        cancelDialog.WaitById("CancelButton").InvokeButton();
        ScenarioUi.WaitForDialogClosed(context.App);
        context.Recorder.Check("«Отмена» не закрывает приложение", !context.App.HasExited, null);
        context.Recorder.Check(
            "«Отмена» оставляет вкладки грязными",
            ScenarioUi.TabNames(window).Any(name => name.Contains('*')),
            "вкладки: " + string.Join(" / ", ScenarioUi.TabNames(window)));
        context.Recorder.Check(
            "«Отмена» не пишет файлы",
            SaveDirtyTabScenario.HashFile(cursorFile) == cursorAfterSaveAll
                && SaveDirtyTabScenario.HashFile(itemClassFile) == itemClassAfterSaveAll,
            null);

        // "Save" writes each dirty tab and closes the application after the second dialog.
        context.App.RequestClose();
        var cursorDialog = ScenarioUi.WaitForDialogWithText(
            context.App,
            "Lookup.Cursor",
            TimeSpan.FromSeconds(20));
        cursorDialog.WaitById("SaveButton").InvokeButton();
        var itemClassDialog = ScenarioUi.WaitForDialogWithText(
            context.App,
            "Lookup.ItemClass",
            TimeSpan.FromSeconds(20));
        itemClassDialog.WaitById("SaveButton").InvokeButton();
        Ui.WaitUntil(
            () => context.App.HasExited,
            "приложение закрылось после сохранения грязных вкладок",
            TimeSpan.FromSeconds(40));
        context.Recorder.Check("«Сохранить» закрывает приложение", true, null);

        context.Recorder.Check(
            "Файлы записаны при закрытии",
            SaveDirtyTabScenario.HashFile(cursorFile) != cursorAfterSaveAll
                && SaveDirtyTabScenario.HashFile(itemClassFile) != itemClassAfterSaveAll,
            null);
        context.Recorder.Check(
            "Чистая вкладка Lookup.WeaponClass не перезаписана",
            SaveDirtyTabScenario.HashFile(weaponClassFile) == weaponClassBefore,
            null);
        context.Recorder.Check(
            "Число записей в Lookup.Cursor не изменилось",
            CountElements(cursorFile, "name") == cursorNamesBefore,
            $"name: {CountElements(cursorFile, "name")}");
        context.Recorder.Check(
            "Число записей в Lookup.ItemClass не изменилось",
            CountElements(itemClassFile, "name") == itemClassNamesBefore,
            $"name: {CountElements(itemClassFile, "name")}");
        context.Recorder.Check(
            "Значения, сохранённые при закрытии, записаны в файлы",
            ReadValues(cursorFile, "name").Contains("Close Cursor", StringComparer.Ordinal)
                && ReadValues(itemClassFile, "name").Contains("Close ItemClass", StringComparer.Ordinal),
            null);
    }

    private static void OpenAndEdit(ScenarioContext context, string tableName, string value)
    {
        ScenarioUi.OpenTable(context, tableName);
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            $"{tableName} открыт");
        context.Window.WaitById("RowsGrid").EditCell(0, 1, value);
        Ui.WaitUntil(
            () => context.Window.WaitById("SaveButton").IsEnabled(),
            $"вкладка {tableName} стала грязной");
    }

    private static int CountElements(string path, string name) =>
        ReadValues(path, name).Count;

    private static IReadOnlyList<string> ReadValues(string path, string name) =>
        XDocument.Load(path).Descendants(name).Select(static element => element.Value).ToList();
}
