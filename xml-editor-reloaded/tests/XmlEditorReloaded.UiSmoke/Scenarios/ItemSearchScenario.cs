using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S16: the item list filters by name and by identifier, and "Go to ID" selects item 12 with its name,
/// long name, identifier, and class.
/// </summary>
internal sealed class ItemSearchScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S16";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        WorkspaceUi.OpenItemWorkspace(context);
        var list = window.WaitById("ItemList");
        var search = window.WaitById("ItemSearchBox");

        WorkspaceUi.SetTextAndCommit(search, "Commando");
        var byName = context.Strings.Format("ItemCountShortFormat", 2, 1776);
        Ui.WaitUntil(
            () => window.WaitById("ItemCountText").ReadName() == byName,
            $"поиск по имени оставляет две записи («{byName}»)",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Поиск по имени находит два предмета", true, byName);
        var names = WorkspaceUi.ListItemTexts(list);
        context.Recorder.Check(
            "Поиск по имени оставляет только совпадающие записи",
            names.Count == 2
                && names.Contains("12: Commando")
                && names.Contains("901: Commando Mortar"),
            string.Join(" | ", names));

        WorkspaceUi.SetTextAndCommit(search, "901");
        var byId = context.Strings.Format("ItemCountShortFormat", 1, 1776);
        Ui.WaitUntil(
            () => window.WaitById("ItemCountText").ReadName() == byId,
            $"поиск по ID оставляет одну запись («{byId}»)",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Поиск по ID находит предмет 901",
            WorkspaceUi.ListItemTexts(list).Contains("901: Commando Mortar"),
            string.Join(" | ", WorkspaceUi.ListItemTexts(list)));

        WorkspaceUi.SelectItem(context, "12", "Commando");
        var header = window.WaitById("ItemDetailHeader").ReadName();
        context.Recorder.Check("Заголовок показывает Commando", header == "12: Commando", header);
        var id = window.WaitById("ItemIdBox").ReadValue();
        context.Recorder.Check("Поле ID показывает 12", id == "12", id);
        var classText = window.WaitById("ItemClassText").ReadName();
        context.Recorder.Check("Класс отображается как Gun", classText == "Gun", classText);

        var longName = WorkspaceUi.ShowField(context, "szLongItemName");
        var longNameValue = WorkspaceUi.ReadFieldValue(longName);
        context.Recorder.Check(
            "Длинное имя предмета — Colt M4 Commando",
            longNameValue == "Colt M4 Commando",
            longNameValue);

        var fullCount = context.Strings.Format("ItemCountShortFormat", 1776, 1776);
        Ui.WaitUntil(
            () => window.WaitById("ItemCountText").ReadName() == fullCount,
            "переход к ID сбрасывает поиск");
        context.Recorder.Check(
            "После перехода к ID поиск сброшен и список снова полный",
            window.WaitById("ItemCountText").ReadName() == fullCount,
            window.WaitById("ItemCountText").ReadName());
    }
}
