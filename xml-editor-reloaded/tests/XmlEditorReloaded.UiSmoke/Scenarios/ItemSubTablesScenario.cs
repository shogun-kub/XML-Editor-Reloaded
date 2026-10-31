using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S19: the sub-table tab of item 12 shows only the weapon section for its class, the weapon row is
/// editable, its combo fields open, and undo restores an edited value.
/// </summary>
internal sealed class ItemSubTablesScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S19";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        WorkspaceUi.SelectItemTab(window, context.Strings.Get("ItemTabSubTables"));

        var tabs = window.WaitById("ItemTabs");
        var texts = tabs
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .ToList();
        var weapon = context.Strings.Get("ItemSection.Items_Weapons");
        context.Recorder.Check(
            "Для класса оружия видна секция «Оружие»",
            texts.Contains(weapon),
            string.Join(" | ", texts.Take(20)));
        foreach (var hidden in new[]
                 {
                     "ItemSection.Items_Magazines",
                     "ItemSection.Items_Armours",
                     "ItemSection.Items_Explosives",
                     "ItemSection.Items_LoadBearingEquipment",
                 })
        {
            var label = context.Strings.Get(hidden);
            context.Recorder.Check(
                $"Секция «{label}» скрыта для класса оружия",
                !texts.Contains(label),
                string.Join(" | ", texts.Take(20)));
        }

        context.Recorder.Check(
            "Нет кнопки создания строки у существующей записи",
            window.FindByName(context.Strings.Get("ItemCreateRow")) is null,
            null);
        context.Recorder.Check(
            "Заметка об общей строке класса не показана",
            window.FindByName(context.Strings.Get("ItemSharedClassRow")) is null,
            null);
        context.Recorder.Check(
            "Повторяемое поле «Конфигурации стволов» видно",
            window.FindByName(context.Strings.Get("ItemListSection.BarrelConfiguration")) is not null,
            null);

        var range = window.WaitById("usRange");
        var before = WorkspaceUi.ReadFieldValue(range);
        context.Recorder.Check("Поле usRange показывает дальность 305", before == "305", before);

        var attack = window.WaitById("ubWeaponType");
        var lookupItems = ScenarioUi.ReadComboItems(attack);
        context.Recorder.Check(
            "Combo-поле ubWeaponType открывается со списком",
            lookupItems.Count > 1,
            $"пунктов: {lookupItems.Count}");

        WorkspaceUi.SetTextAndCommit(range, "310");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("EditorCellUpdated", "usRange", "12"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Правка подтаблицы сообщает об обновлении", true);
        context.Recorder.Check(
            "Правка подтаблицы делает вкладку грязной",
            window.WaitById("ItemSaveButton").IsEnabled(),
            null);

        window.WaitById("ItemUndoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorUndone"));
        Ui.WaitUntil(
            () => WorkspaceUi.ReadFieldValue(window.WaitById("usRange")) == "305",
            "отмена вернула дальность 305",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Undo возвращает значение подтаблицы", true);
        context.Recorder.Check(
            "После отмены вкладка снова чистая",
            !window.WaitById("ItemSaveButton").IsEnabled(),
            null);
    }
}
