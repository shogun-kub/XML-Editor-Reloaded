using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S31: editing the optimal quantity of AdditionalDealer_43 for item 1422 from 6 to 9 persists to the
/// copy, keeps the row of item 1456 at 3, leaves TonyInventory alone and survives a tab reopen.
/// </summary>
internal sealed class ShopkeeperEditScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S31";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V5];

    /// <inheritdoc />
    public override DatasetKind ActiveDataset => DatasetKind.V5;

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V5;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var data = context.Data(DatasetKind.V5);
        var dealerFile = data.TableFile("NPCInventory/AdditionalDealer_43_Inventory.xml");
        var tonyFile = data.TableFile("NPCInventory/TonyInventory.xml");
        var dealerBefore = XDocument.Load(dealerFile);
        var tonyHash = SaveDirtyTabScenario.HashFile(tonyFile);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1422", "12g Box BS");
        OpenShopkeeperTab(context);
        context.Recorder.Check(
            "У предмета 1422 одна строка торговца",
            window.WaitById("ShopkeeperCountText").ReadName() == context.Strings.Format("ShopkeeperCountFormat", 1),
            window.WaitById("ShopkeeperCountText").ReadName());

        var quantity = WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_");
        context.Recorder.Check("Количество дилера равно 6", quantity.ReadValue() == "6", quantity.ReadValue());
        WorkspaceUi.SetTextAndCommit(quantity, "9");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("EditorCellUpdated", "ubOptimalNumber", "26"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Правка количества делает вкладку грязной",
            window.WaitById("ItemSaveButton").IsEnabled(),
            null);

        WorkspaceUi.SaveItem(context);
        var dealerAfter = XDocument.Load(dealerFile);
        context.Recorder.Check(
            "Строка item 1422 сохранена со значением 9",
            Quantity(dealerAfter, "1422") == "9",
            Quantity(dealerAfter, "1422"));
        context.Recorder.Check(
            "Строка item 1456 осталась со значением 3",
            Quantity(dealerAfter, "1456") == "3",
            Quantity(dealerAfter, "1456"));
        context.Recorder.Check(
            "Число строк файла не изменилось",
            dealerAfter.Descendants("INVENTORY").Count() == dealerBefore.Descendants("INVENTORY").Count()
                && dealerAfter.Descendants("INVENTORY").Count() == 44,
            dealerAfter.Descendants("INVENTORY").Count().ToString());
        context.Recorder.Check(
            "TonyInventory не изменился",
            SaveDirtyTabScenario.HashFile(tonyFile) == tonyHash,
            null);
        context.Recorder.Check(
            "Обе строки с ключом 26 сохранены",
            dealerAfter.Descendants("INVENTORY")
                .Count(row => row.Element("uiIndex")?.Value.Trim() == "26") == 2,
            null);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1422", "12g Box BS");
        OpenShopkeeperTab(context);
        context.Recorder.Check(
            "После переоткрытия вкладки количество равно 9",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "9",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());

        context.ExpectChange(DatasetKind.V5, "TableData/NPCInventory/AdditionalDealer_43_Inventory.xml");
    }

    private static void OpenShopkeeperTab(ScenarioContext context)
    {
        WorkspaceUi.SelectItemTab(context.Window, context.Strings.Get("ItemTabShopkeepers"));
    }

    private static string Quantity(XDocument document, string itemIndex) =>
        document.Descendants("INVENTORY")
            .First(row => row.Element("sItemIndex")?.Value.Trim() == itemIndex)
            .Element("ubOptimalNumber")?.Value.Trim() ?? string.Empty;
}
