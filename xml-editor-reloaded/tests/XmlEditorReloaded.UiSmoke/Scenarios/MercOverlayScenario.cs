using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S29: with the Russian overlay the gear kits show the translated names, and renaming a kit writes only
/// the overlay, leaves the base table untouched and updates the tab header.
/// </summary>
internal sealed class MercOverlayScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S29";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override string? OverlayLanguage => "ru";

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var data = context.Data(DatasetKind.V4);
        var baseFile = data.TableFile("Inventory/MercStartingGear.xml");
        var overlayFile = data.TableFile("Inventory/Russian.MercStartingGear.xml");
        var baseHash = SaveDirtyTabScenario.HashFile(baseFile);
        var baseBefore = XDocument.Load(baseFile);
        var overlayBefore = XDocument.Load(overlayFile);

        WorkspaceUi.OpenMercGearWorkspace(context);
        var language = window.WaitById("MercLanguageCombo").ReadComboSelection();
        context.Recorder.Check("Рабочее место открыто с русским оверлеем", language == "Russian", language);
        var search = window.WaitById("MercSearchBox");
        WorkspaceUi.SetTextAndCommit(search, "Barry");
        Ui.WaitUntil(
            () => window.WaitById("MercDetailHeader").ReadName() == "0: Barry",
            "выбран Barry");

        var kits = WorkspaceUi.TabCaptions(window, "MercKitTabs");
        context.Recorder.Check(
            "Комплекты Barry названы по-русски",
            kits.SequenceEqual(new[] { "Стандарт", "Механик", "Боец", "Разгрузка", "Элитный" }),
            string.Join(" | ", kits));

        WorkspaceUi.SelectTab(window, "MercKitTabs", "Стандарт");
        var nameBox = window.WaitById("MercKitNameBox");
        context.Recorder.Check(
            "Поле имени показывает перевод",
            nameBox.ReadValue() == "Стандарт",
            nameBox.ReadValue());

        WorkspaceUi.SetTextAndCommit(nameBox, "Стандарт Тест");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "mGearKitName", "0"),
            "имя комплекта изменено");
        context.Recorder.Check(
            "Заголовок вкладки показывает новый перевод",
            WorkspaceUi.TabCaptions(window, "MercKitTabs").FirstOrDefault() == "Стандарт Тест",
            string.Join(" | ", WorkspaceUi.TabCaptions(window, "MercKitTabs")));

        window.WaitById("MercUndoButton").InvokeButton();
        Ui.WaitUntil(
            () => WorkspaceUi.TabCaptions(window, "MercKitTabs").FirstOrDefault() == "Стандарт",
            "Undo вернул перевод",
            TimeSpan.FromSeconds(20));
        window.WaitById("MercRedoButton").InvokeButton();
        Ui.WaitUntil(
            () => WorkspaceUi.TabCaptions(window, "MercKitTabs").FirstOrDefault() == "Стандарт Тест",
            "Redo вернул правку",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Undo и Redo работают с переводом", true);

        WorkspaceUi.SaveMercGear(context);
        var overlayAfter = XDocument.Load(overlayFile);
        context.Recorder.Check(
            "В оверлее обновлено имя первого комплекта Barry",
            KitName(overlayAfter, "0", 0) == "Стандарт Тест",
            KitName(overlayAfter, "0", 0));
        context.Recorder.Check(
            "Остальные комплекты оверлея не изменились",
            overlayBefore.Descendants("MERCGEAR")
                .Where(row => row.Element("mIndex")?.Value.Trim() != "0")
                .Select(static row => row.ToString(SaveOptions.DisableFormatting))
                .SequenceEqual(overlayAfter.Descendants("MERCGEAR")
                    .Where(row => row.Element("mIndex")?.Value.Trim() != "0")
                    .Select(static row => row.ToString(SaveOptions.DisableFormatting)))
                && overlayBefore.Descendants("MERCGEAR")
                    .First(row => row.Element("mIndex")?.Value.Trim() == "0")
                    .Elements("GEARKIT")
                    .Skip(1)
                    .Select(static kit => kit.ToString(SaveOptions.DisableFormatting))
                    .SequenceEqual(overlayAfter.Descendants("MERCGEAR")
                        .First(row => row.Element("mIndex")?.Value.Trim() == "0")
                        .Elements("GEARKIT")
                        .Skip(1)
                        .Select(static kit => kit.ToString(SaveOptions.DisableFormatting))),
            null);
        context.Recorder.Check(
            "Число записей оверлея не изменилось",
            overlayBefore.Descendants("MERCGEAR").Count() == overlayAfter.Descendants("MERCGEAR").Count(),
            null);
        context.Recorder.Check(
            "Базовый MercStartingGear.xml не изменился",
            SaveDirtyTabScenario.HashFile(baseFile) == baseHash
                && KitName(baseBefore, "0", 0) == "Standard",
            KitName(baseBefore, "0", 0));

        var combo = window.WaitById("MercLanguageCombo");
        combo.SelectComboItem(context.Strings.Get("EditorBaseLanguageOption"));
        Ui.WaitUntil(
            () => WorkspaceUi.TabCaptions(window, "MercKitTabs").FirstOrDefault() == "Standard",
            "базовый режим показывает английские имена комплектов",
            TimeSpan.FromSeconds(60));
        context.Recorder.Check(
            "В базовом режиме имя комплекта Standard",
            window.WaitById("MercKitNameBox").ReadValue() == "Standard",
            window.WaitById("MercKitNameBox").ReadValue());

        context.ExpectChange(DatasetKind.V4, "TableData/Inventory/Russian.MercStartingGear.xml");
    }

    private static string KitName(XDocument document, string mercKey, int kitIndex)
    {
        var kit = document.Descendants("MERCGEAR")
            .First(row => row.Element("mIndex")?.Value.Trim() == mercKey)
            .Elements("GEARKIT")
            .Skip(kitIndex)
            .FirstOrDefault();
        return kit?.Element("mGearKitName")?.Value.Trim() ?? string.Empty;
    }
}
