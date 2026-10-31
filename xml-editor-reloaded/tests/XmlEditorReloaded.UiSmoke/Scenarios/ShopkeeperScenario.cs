using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S30: the shopkeeper tab of item 1422 shows its single dealer row with the stored optimal quantity, a
/// remove button, and the dealer picker for adding another row.
/// </summary>
internal sealed class ShopkeeperScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S30";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V5];

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V5;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1422", "12g Box BS");
        WorkspaceUi.SelectItemTab(window, context.Strings.Get("ItemTabShopkeepers"));

        var count = window.WaitById("ShopkeeperCountText").ReadName();
        var expected = context.Strings.Format("ShopkeeperCountFormat", 1);
        context.Recorder.Check(
            "У предмета 1422 одна строка торговца",
            count == expected,
            count);

        var quantity = WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_");
        context.Recorder.Check(
            "Строка принадлежит AdditionalDealer_43",
            quantity.ReadName() == "AdditionalDealer_43_",
            quantity.ReadName());
        var stored = quantity.ReadValue();
        context.Recorder.Check("Оптимальное количество равно 6", stored == "6", stored);
        context.Recorder.Check(
            "Имя торговца видно в списке",
            window.FindByName("AdditionalDealer_43_") is not null,
            null);

        var entries = window.WaitById("ShopkeeperEntries");
        var remove = entries
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
            .FirstOrDefault(button => button.ReadName() == context.Strings.Get("ShopkeeperRemoveText"));
        context.Recorder.Check(
            "У строки есть кнопка «Удалить»",
            remove is not null,
            null);

        var dealerCombo = window.WaitById("ShopkeeperDealerCombo");
        context.Recorder.Check(
            "Список торговцев для добавления доступен",
            dealerCombo.IsEnabled(),
            null);
        var add = window.WaitById("ShopkeeperAddButton");
        context.Recorder.Check(
            "Кнопка «Добавить» на месте",
            add.ReadName() == context.Strings.Get("ShopkeeperAddText"),
            add.ReadName());
    }
}
