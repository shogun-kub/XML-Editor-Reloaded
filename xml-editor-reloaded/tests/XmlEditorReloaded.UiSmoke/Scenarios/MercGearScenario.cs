namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S26: the mercenary starting-gear workspace lists all mercenaries, finds Barry by name, shows his five
/// gear kits and the values of the standard kit.
/// </summary>
internal sealed class MercGearScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S26";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var count = WorkspaceUi.OpenMercGearWorkspace(context);
        context.Recorder.Check(
            "Рабочее место показывает 255 наёмников",
            count == "Наёмников: 255",
            count);

        var search = window.WaitById("MercSearchBox");
        WorkspaceUi.SetTextAndCommit(search, "Barry");
        var merc = WorkspaceUi.FindListItem(window.WaitById("MercList"), "0: Barry")
            ?? throw new UiTimeoutException("Barry не найден в списке наёмников.");
        merc.SelectItem();
        WorkspaceUi.WaitForStatus(
            context,
            "MercDetailHeader",
            text => text == "0: Barry",
            "выбран Barry",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Поиск наёмника находит Barry", true);

        var kits = WorkspaceUi.TabCaptions(window, "MercKitTabs");
        context.Recorder.Check(
            "У Barry пять комплектов с именами из базы",
            kits.SequenceEqual(new[] { "Standard", "Mechanic", "Combat", "Sapper", "Premium" }),
            string.Join(" | ", kits));

        var tabs = window.WaitById("MercKitTabs");
        var headers = tabs
            .FindAll(new System.Windows.Automation.PropertyCondition(
                System.Windows.Automation.AutomationElement.ControlTypeProperty,
                System.Windows.Automation.ControlType.Text))
            .Select(static text => text.ReadName())
            .ToList();
        foreach (var label in new[] { "Шлем", "Оружие", "Малый слот 8", "Разгрузочный жилет" })
        {
            context.Recorder.Check(
                $"Слот «{label}» показан",
                headers.Contains(label),
                string.Join(" | ", headers));
        }

        context.Recorder.Check(
            "Шлем стандартного комплекта — Steel Helmet",
            window.WaitById("mHelmet").ReadValue() == "Steel Helmet",
            window.WaitById("mHelmet").ReadValue());
        context.Recorder.Check(
            "Статус шлема 97",
            window.WaitById("mHelmetStatus").ReadValue() == "97",
            window.WaitById("mHelmetStatus").ReadValue());
        context.Recorder.Check(
            "Оружие стандартного комплекта — .38 Special",
            window.WaitById("mWeapon").ReadValue() == ".38 Special",
            window.WaitById("mWeapon").ReadValue());
        context.Recorder.Check(
            "Первый большой слот — .38 Speed Loader",
            window.WaitById("mBig0").ReadValue() == ".38 Speed Loader",
            window.WaitById("mBig0").ReadValue());
        context.Recorder.Check(
            "Количество в большом слоте 3",
            window.WaitById("mBig0Quantity").ReadValue() == "3",
            window.WaitById("mBig0Quantity").ReadValue());
        context.Recorder.Check(
            "Пустой слот показывает Nothing",
            window.WaitById("mSmall0").ReadValue() == "Nothing",
            window.WaitById("mSmall0").ReadValue());

        WorkspaceUi.SelectTab(window, "MercKitTabs", "Premium");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            _ => true,
            "комплект Premium выбран",
            TimeSpan.FromSeconds(10));
        context.Recorder.Check(
            "Переключение на другой комплект показывает его шлем",
            window.WaitById("mHelmet").ReadValue() == "Steel Helmet",
            window.WaitById("mHelmet").ReadValue());
    }
}
