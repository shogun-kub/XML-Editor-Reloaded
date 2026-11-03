using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S89: after the only dealer row is removed and saved, the file has an empty declared root; reopening the
/// item keeps the dealer available, enables Add, and the first added row survives a save and reopen.
/// </summary>
internal sealed class EmptyDealerFileScenario : ShopkeeperCrudScenarioBase
{
    private const string File = "NPCInventory/AdditionalDealer_10_Inventory.xml";
    private const string Dealer = "AdditionalDealer_10_";

    /// <inheritdoc />
    public override string Id => "S89";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var dealerFile = context.Data(DatasetKind.V5).TableFile(File);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "0", "Nada");
        OpenShopkeeperTab(context);
        var baseCount = ReadShopkeeperCount(context);
        context.Recorder.Check(
            "У item 0 есть строка AdditionalDealer_10",
            baseCount > 1 && HasDealerRow(context, Dealer),
            $"строк: {baseCount}");

        RemoveDealer(context, "0", Dealer, "0");
        WaitShopkeeperCount(context, baseCount - 1);
        WorkspaceUi.SaveItem(context);
        var empty = XDocument.Load(dealerFile);
        context.Recorder.Check(
            "После удаления в файле нет строк",
            RecordCount(dealerFile) == 0,
            RecordCount(dealerFile).ToString());
        context.Recorder.Check(
            "Объявленный корень файла сохранён",
            empty.Root?.Name.LocalName == "INVENTORYLIST",
            empty.Root?.Name.LocalName);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "0", "Nada");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, baseCount - 1);
        context.Recorder.Check(
            "Торговец остаётся доступным после опустошения файла",
            HasDealerOption(context, Dealer),
            null);
        context.Recorder.Check(
            "Пустой файл открылся без ошибки",
            window.WaitById("ItemStatus").ReadName().Contains(
                context.Strings.Get("EditorReady"),
                StringComparison.Ordinal),
            window.WaitById("ItemStatus").ReadName());

        SelectDealerOption(context, Dealer);
        context.Recorder.Check(
            "После выбора торговца «Добавить» активно",
            window.WaitById("ShopkeeperAddButton").IsEnabled(),
            $"Add.IsEnabled={window.WaitById("ShopkeeperAddButton").IsEnabled()}");
        SetNewQuantity(context, "5");
        AddDealer(context, Dealer);
        WaitShopkeeperCount(context, baseCount);
        context.Recorder.Check(
            "Первая строка добавлена с количеством 5",
            WorkspaceUi.ShopkeeperQuantity(context, Dealer).ReadValue() == "5",
            WorkspaceUi.ShopkeeperQuantity(context, Dealer).ReadValue());
        WorkspaceUi.SaveItem(context);

        var saved = XDocument.Load(dealerFile);
        var row = saved.Descendants("INVENTORY").Single();
        context.Recorder.Check(
            "Новая первая строка записана в файл",
            row.Element("sItemIndex")?.Value.Trim() == "0"
                && row.Element("ubOptimalNumber")?.Value.Trim() == "5",
            row.ToString(SaveOptions.DisableFormatting));
        context.Recorder.Check(
            "Новая строка получила uiIndex 0",
            row.Element("uiIndex")?.Value.Trim() == "0",
            row.Element("uiIndex")?.Value.Trim());

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "0", "Nada");
        OpenShopkeeperTab(context);
        WaitShopkeeperCount(context, baseCount);
        context.Recorder.Check(
            "После повторного открытия строка на месте",
            WorkspaceUi.ShopkeeperQuantity(context, Dealer).ReadValue() == "5",
            WorkspaceUi.ShopkeeperQuantity(context, Dealer).ReadValue());

        context.ExpectChange(DatasetKind.V5, "TableData/" + File);
    }
}
