using System.IO;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S18: a field edited in the item workspace marks the tab dirty, undo and redo restore and reapply the
/// value, and saving changes only that one XML value on the writable copy.
/// </summary>
internal sealed class ItemFieldEditScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S18";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Items/Items.xml");
        var original = File.ReadAllText(file);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        var field = WorkspaceUi.ShowField(context, "szItemName");
        var before = WorkspaceUi.ReadFieldValue(field);
        context.Recorder.Check("Поле szItemName показывает имя предмета", before == "Commando", before);

        var save = window.WaitById("ItemSaveButton");
        context.Recorder.Check("На чистой вкладке сохранение недоступно", !save.IsEnabled(), null);

        WorkspaceUi.SetTextAndCommit(field, "Commando Smoke");
        var updated = context.Strings.Format("EditorCellUpdated", "szItemName", "12");
        WorkspaceUi.WaitItemStatus(context, updated, TimeSpan.FromSeconds(20));
        context.Recorder.Check("Статус сообщает об обновлении поля", true, updated);
        context.Recorder.Check("Вкладка стала грязной", save.IsEnabled(), null);
        context.Recorder.Check(
            "Правка в памяти не пишет файл",
            File.ReadAllText(file) == original,
            null);

        window.WaitById("ItemUndoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Get("EditorUndone"),
            TimeSpan.FromSeconds(20));
        Ui.WaitUntil(
            () => WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemName")) == "Commando",
            "отмена вернула прежнее имя");
        context.Recorder.Check("Undo возвращает имя Commando", true);

        window.WaitById("ItemRedoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Get("EditorRedone"),
            TimeSpan.FromSeconds(20));
        Ui.WaitUntil(
            () => WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemName")) == "Commando Smoke",
            "повтор вернул изменённое имя");
        context.Recorder.Check("Redo повторяет правку", true);

        WorkspaceUi.SaveItem(context);
        context.Recorder.Check("После сохранения вкладка снова чистая", !save.IsEnabled(), null);

        var saved = File.ReadAllText(file);
        var restored = saved.Replace(
            "<szItemName>Commando Smoke</szItemName>",
            "<szItemName>Commando</szItemName>",
            StringComparison.Ordinal);
        context.Recorder.Check(
            "В файле изменилось только значение szItemName предмета 12",
            restored == original,
            restored == original ? null : FirstDifference(original, restored));
        context.Recorder.Check(
            "Остальные элементы и их порядок не тронуты",
            saved.Contains("<szLongItemName>Colt M4 Commando</szLongItemName>", StringComparison.Ordinal),
            null);

        context.ExpectChange(DatasetKind.V4, "TableData/Items/Items.xml");
    }

    /// <summary>
    /// Returns a short description of the first difference between two texts, for failure details.
    /// </summary>
    /// <param name="expected">Original text.</param>
    /// <param name="actual">Rewritten text.</param>
    /// <returns>Position and surrounding text of the first difference.</returns>
    private static string FirstDifference(string expected, string actual)
    {
        var length = Math.Min(expected.Length, actual.Length);
        for (var index = 0; index < length; index++)
        {
            if (expected[index] != actual[index])
            {
                var start = Math.Max(0, index - 40);
                return $"позиция {index}: ожидалось «{expected.Substring(start, Math.Min(80, expected.Length - start))}», "
                    + $"получено «{actual.Substring(start, Math.Min(80, actual.Length - start))}»";
            }
        }

        return $"длина изменилась: {expected.Length} → {actual.Length}";
    }
}
