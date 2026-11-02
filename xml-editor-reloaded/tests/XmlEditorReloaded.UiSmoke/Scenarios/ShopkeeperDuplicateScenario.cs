using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S32: item 1456 has two dealer rows, and editing the AdditionalDealer_43 quantity to 7 must hit exactly
/// its row even though another row repeats <c>uiIndex=26</c>; the other row keeps 6 and the duplicate-key
/// warning stays.
/// </summary>
internal sealed class ShopkeeperDuplicateScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S32";

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
        var dealerFile = context.Data(DatasetKind.V5).TableFile("NPCInventory/AdditionalDealer_43_Inventory.xml");

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1456", "Dart Box");
        OpenShopkeeperTab(context);
        context.Recorder.Check(
            "У предмета 1456 две строки торговцев",
            window.WaitById("ShopkeeperCountText").ReadName() == context.Strings.Format("ShopkeeperCountFormat", 2),
            window.WaitById("ShopkeeperCountText").ReadName());
        var dealer = WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_");
        var tony = WorkspaceUi.ShopkeeperQuantity(context, "Tony");
        context.Recorder.Check(
            "Строка AdditionalDealer_43 содержит 3",
            dealer.ReadValue() == "3",
            dealer.ReadValue());
        context.Recorder.Check(
            "Строка Tony содержит 1",
            tony.ReadValue() == "1",
            tony.ReadValue());

        WorkspaceUi.SetShopkeeperQuantity(context, "AdditionalDealer_43_", "7");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("EditorCellUpdated", "ubOptimalNumber", "26"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Правка делает вкладку грязной",
            window.WaitById("ItemSaveButton").IsEnabled(),
            null);
        WorkspaceUi.SaveItem(context);

        var after = XDocument.Load(dealerFile);
        context.Recorder.Check(
            "Правка попала в строку item 1456",
            Quantity(after, "1456") == "7",
            Quantity(after, "1456"));
        context.Recorder.Check(
            "Строка item 1422 осталась 6",
            Quantity(after, "1422") == "6",
            Quantity(after, "1422"));
        context.Recorder.Check(
            "Обе строки с ключом 26 сохранены",
            after.Descendants("INVENTORY").Count(row => row.Element("uiIndex")?.Value.Trim() == "26") == 2
                && after.Descendants("INVENTORY").Count() == 44,
            after.Descendants("INVENTORY").Count().ToString());

        ValidateDuplicate(context, after);

        ScenarioUi.CloseActiveEditorTab(window);
        ScenarioUi.WaitForEditorTabCount(window, 0);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "1422", "12g Box BS");
        OpenShopkeeperTab(context);
        context.Recorder.Check(
            "После переоткрытия item 1422 показывает 6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "6",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue());
        WorkspaceUi.SelectItem(context, "1456", "Dart Box");
        OpenShopkeeperTab(context);
        context.Recorder.Check(
            "После переоткрытия item 1456 показывает 7",
            WorkspaceUi.ShopkeeperQuantity(context, "AdditionalDealer_43_").ReadValue() == "7",
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

    private static void ValidateDuplicate(ScenarioContext context, XDocument saved)
    {
        var window = context.Window;
        window.WaitById("ItemValidateButton").InvokeButton();
        var list = window.WaitById("ItemValidationList");
        string? issue = null;
        for (var attempt = 0; attempt < 30 && issue is null; attempt++)
        {
            Thread.Sleep(500);
            var texts = WorkspaceUi.ListItemTexts(list);
            issue = texts.FirstOrDefault(
                text => text.Contains("validation.duplicate_key", StringComparison.Ordinal)
                    && text.Contains("[26]", StringComparison.Ordinal));
            if (issue is null && !Ui.ScrollDown(list))
            {
                break;
            }
        }

        context.Recorder.Check(
            "Валидатор предупреждает о дубликате ключа 26",
            issue is not null,
            issue ?? string.Join(" | ", WorkspaceUi.ListItemTexts(list).Take(3)));
        context.Recorder.Note(
            "Значения строк 26 после сохранения: "
            + $"1422={Quantity(saved, "1422")}, 1456={Quantity(saved, "1456")}.");
    }
}
