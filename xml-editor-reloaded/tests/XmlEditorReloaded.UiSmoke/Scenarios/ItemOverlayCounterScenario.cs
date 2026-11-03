using System.IO;
using System.Text;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S85: counters use the visible language overlay, editing a translation writes only the overlay, and a
/// Chinese overlay applies its 63/90 limits while an untranslated long base value is shown, not truncated.
/// </summary>
internal sealed class ItemOverlayCounterScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S85";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var data = context.Data(DatasetKind.V4);
        var baseFile = data.TableFile("Items/Items.xml");
        var russianFile = data.TableFile("Items/Russian.Items.xml");
        var chineseFile = data.TableFile("Items/Chinese.Items.xml");
        var baseDocument = XDocument.Load(baseFile);
        var baseRow = Row(baseDocument, "12") ?? throw new SmokeCheckException(
            "В базовой таблице есть item 12",
            "строка не найдена");
        var baseDescription = baseRow.Element("szItemDesc")?.Value ?? string.Empty;
        var baseBattleDescription = baseRow.Element("szBRDesc")?.Value ?? string.Empty;
        CreateChineseOverlay(chineseFile);
        var baseHash = SaveDirtyTabScenario.HashFile(baseFile);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        var combo = window.WaitById("ItemLanguageCombo");
        combo.SelectComboItem("Russian");
        Ui.WaitUntil(
            () => window.WaitById("ItemDetailHeader").ReadName() == "12: Коммандо",
            "русский оверлей загружен",
            TimeSpan.FromSeconds(60));

        var description = WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemDesc"));
        var descriptionCounter = window.WaitById("szItemDesc_counter");
        context.Recorder.Check(
            "Счётчик описания считает русский перевод",
            descriptionCounter.ReadName()
                == context.Strings.Format("DetailCharactersLeftFormat", 400 - description.Length),
            $"{descriptionCounter.ReadName()} / длина {description.Length}");
        var battleDescription = WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szBRDesc"));
        var battleCounter = window.WaitById("szBRDesc_counter");
        context.Recorder.Check(
            "Счётчик боевого описания считает русский перевод",
            battleCounter.ReadName()
                == context.Strings.Format("DetailCharactersLeftFormat", 400 - battleDescription.Length),
            $"{battleCounter.ReadName()} / длина {battleDescription.Length}");

        const string NewDescription = "Тестовое описание";
        const string NewBattleDescription = "Тестовое боевое описание";
        WorkspaceUi.SetField(context, "szItemDesc", NewDescription);
        WorkspaceUi.SetField(context, "szBRDesc", NewBattleDescription);
        WorkspaceUi.SaveItem(context);
        context.ExpectChange(DatasetKind.V4, "TableData/Items/Russian.Items.xml");
        context.ExpectChange(DatasetKind.V4, "TableData/Items/Chinese.Items.xml");

        context.Recorder.Check(
            "Правка перевода не изменила базовый файл",
            SaveDirtyTabScenario.HashFile(baseFile) == baseHash,
            null);
        var russianRow = Row(XDocument.Load(russianFile), "12");
        context.Recorder.Check(
            "Правка записана в русский оверлей",
            russianRow?.Element("szItemDesc")?.Value == NewDescription
                && russianRow?.Element("szBRDesc")?.Value == NewBattleDescription,
            russianRow?.ToString(SaveOptions.DisableFormatting));

        combo = window.WaitById("ItemLanguageCombo");
        combo.SelectComboItem("Chinese");
        Ui.WaitUntil(
            () => window.WaitById("ItemDetailHeader").ReadName() == "12: Commando",
            "китайский оверлей загружен",
            TimeSpan.FromSeconds(60));

        var chineseDescription = WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemDesc"));
        context.Recorder.Check(
            "Китайский szItemDesc показывает перевод",
            chineseDescription == "Коротко",
            chineseDescription);
        var chineseDescriptionCounter = window.WaitById("szItemDesc_counter");
        context.Recorder.Check(
            "Китайский лимит описания 63",
            chineseDescriptionCounter.ReadName()
                == context.Strings.Format("DetailCharactersLeftFormat", 63 - chineseDescription.Length),
            $"{chineseDescriptionCounter.ReadName()} / длина {chineseDescription.Length}");

        var fallback = WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szBRDesc"));
        var fallbackCounter = window.WaitById("szBRDesc_counter");
        context.Recorder.Check(
            "Непереведённое боевое описание показывает базовый текст",
            fallback == baseBattleDescription,
            $"длина {fallback.Length}, ожидалась {baseBattleDescription.Length}");
        context.Recorder.Check(
            "Непереведённый текст не обрезан и показывает превышение лимита",
            fallback.Length == baseBattleDescription.Length
                && fallbackCounter.ReadName()
                    == context.Strings.Format(
                        "DetailCharactersOverLimitFormat",
                        Math.Max(0, baseBattleDescription.Length - 90)),
            $"{fallbackCounter.ReadName()} / длина {fallback.Length}");
        context.Recorder.Note(
            "Базовые значения: " + baseDescription.Length + " и " + baseBattleDescription.Length + " символов.");
    }

    private static void CreateChineseOverlay(string path)
    {
        var text =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
            + "<ITEMLIST>\n"
            + "\t<ITEM>\n"
            + "\t\t<uiIndex>12</uiIndex>\n"
            + "\t\t<szItemDesc>Коротко</szItemDesc>\n"
            + "\t</ITEM>\n"
            + "</ITEMLIST>\n";
        File.WriteAllText(path, text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static XElement? Row(XDocument document, string key) =>
        document.Descendants("ITEM")
            .FirstOrDefault(element => element.Element("uiIndex")?.Value.Trim() == key);
}
