using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S23: with the Russian overlay selected, editing the name of item 12 writes only the overlay file,
/// undo and redo work on the translation, and the base table keeps its English value.
/// </summary>
internal sealed class ItemOverlayScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S23";

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
        var baseFile = data.TableFile("Items/Items.xml");
        var overlayFile = data.TableFile("Items/Russian.Items.xml");
        var baseHash = SaveDirtyTabScenario.HashFile(baseFile);
        var overlayBefore = XDocument.Load(overlayFile);
        var baseBefore = XDocument.Load(baseFile);

        WorkspaceUi.OpenItemWorkspace(context);
        var language = window.WaitById("ItemLanguageCombo").ReadComboSelection();
        context.Recorder.Check(
            "Рабочее место открыто с русским оверлеем",
            language == "Russian",
            language);

        WorkspaceUi.SelectItem(context, "12", "Коммандо");
        var name = WorkspaceUi.ShowField(context, "szItemName");
        context.Recorder.Check(
            "Имя предмета показано из оверлея",
            WorkspaceUi.ReadFieldValue(name) == "Коммандо",
            WorkspaceUi.ReadFieldValue(name));
        var longName = WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szLongItemName"));
        context.Recorder.Check(
            "Длинное имя переведено",
            longName == "Кольт M4 Коммандо",
            longName);

        var save = window.WaitById("ItemSaveButton");
        WorkspaceUi.SetTextAndCommit(WorkspaceUi.ShowField(context, "szItemName"), "Коммандо Тест");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("EditorCellUpdated", "szItemName", "12"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Правка перевода делает вкладку грязной", save.IsEnabled(), null);

        window.WaitById("ItemUndoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorUndone"));
        Ui.WaitUntil(
            () => WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemName")) == "Коммандо",
            "Undo вернул перевод",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Undo возвращает перевод", true);

        window.WaitById("ItemRedoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorRedone"));
        Ui.WaitUntil(
            () => WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemName")) == "Коммандо Тест",
            "Redo вернул изменённый перевод",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Redo повторяет правку перевода", true);

        WorkspaceUi.SaveItem(context);
        context.Recorder.Check("Сохраняется только оверлей: база не изменилась",
            SaveDirtyTabScenario.HashFile(baseFile) == baseHash,
            null);

        var overlayAfter = XDocument.Load(overlayFile);
        var overlayItem = Row(overlayAfter, "12");
        context.Recorder.Check(
            "В оверлее обновлено только имя записи 12",
            overlayItem?.Element("szItemName")?.Value == "Коммандо Тест"
                && overlayItem?.Element("szLongItemName")?.Value == "Кольт M4 Коммандо",
            overlayItem?.ToString(SaveOptions.DisableFormatting));
        context.Recorder.Check(
            "Число записей оверлея не изменилось",
            overlayAfter.Descendants("ITEM").Count() == overlayBefore.Descendants("ITEM").Count(),
            $"{overlayBefore.Descendants("ITEM").Count()} → {overlayAfter.Descendants("ITEM").Count()}");
        context.Recorder.Check(
            "Базовая таблица хранит английское имя",
            Row(baseBefore, "12")?.Element("szItemName")?.Value == "Commando",
            null);

        var combo = window.WaitById("ItemLanguageCombo");
        combo.SelectComboItem(context.Strings.Get("EditorBaseLanguageOption"));
        WorkspaceUi.WaitForStatus(
            context,
            "ItemDetailHeader",
            text => text == "12: Commando",
            "базовый режим загружен",
            TimeSpan.FromSeconds(60));
        var baseHeader = window.WaitById("ItemDetailHeader").ReadName();
        context.Recorder.Check(
            "В базовом режиме показано английское имя",
            baseHeader == "12: Commando",
            baseHeader);
        var baseName = WorkspaceUi.ReadFieldValue(WorkspaceUi.ShowField(context, "szItemName"));
        context.Recorder.Check(
            "Поле базового режима показывает Commando",
            baseName == "Commando",
            baseName);

        context.ExpectChange(DatasetKind.V4, "TableData/Items/Russian.Items.xml");
    }

    private static XElement? Row(XDocument document, string key) =>
        document.Descendants("ITEM")
            .FirstOrDefault(element => element.Element("uiIndex")?.Value.Trim() == key);
}
