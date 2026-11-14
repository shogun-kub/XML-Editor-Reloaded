using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S25: create, duplicate, delete and rename an item. Dependent rows follow every operation, undo and
/// redo work, the final save writes only the tables that own the duplicated rows, and a light create
/// plus undo on v5 leaves the reference data untouched.
/// </summary>
internal sealed class ItemLifecycleScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S25";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var data = context.Data(DatasetKind.V4);
        string ItemCount(int value) => context.Strings.Format("ItemCountShortFormat", value, value);
        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");

        // Duplicate: the new record keeps the name and gets the next free key.
        window.WaitById("ItemDuplicateButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemDuplicated", "12", "1776"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Дубликат получил ключ 1776",
            window.WaitById("ItemDetailHeader").ReadName() == "1776: Commando",
            window.WaitById("ItemDetailHeader").ReadName());
        context.Recorder.Check(
            "Счётчик вырос до 1777 предметов",
            window.WaitById("ItemCountText").ReadName() == ItemCount(1777),
            window.WaitById("ItemCountText").ReadName());
        var attachments = context.Strings.Get("ItemSection.Items_Attachments");
        WorkspaceUi.SelectChildSection(
            window,
            context.Strings.Get("ItemTabChildTables"),
            attachments);
        var caption27 = $"{attachments} {context.Strings.Format("ItemChildRowCountFormat", 27)}";
        Ui.WaitUntil(
            () => WorkspaceUi.ChildSectionCaptions(window, context.Strings.Get("ItemTabChildTables")).Contains(caption27),
            "у дубликата скопированы 27 строк навески",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Зависимые строки скопированы вместе с предметом", true);

        window.WaitById("ItemUndoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorUndone"));
        context.Recorder.Check(
            "Undo удаляет дубликат",
            window.WaitById("ItemCountText").ReadName() == ItemCount(1776),
            window.WaitById("ItemCountText").ReadName());
        window.WaitById("ItemRedoButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(context, context.Strings.Get("EditorRedone"));
        context.Recorder.Check(
            "Redo возвращает дубликат",
            window.WaitById("ItemCountText").ReadName() == ItemCount(1777),
            window.WaitById("ItemCountText").ReadName());

        // Delete the duplicate: the confirmation names the dependent rows that disappear with it.
        WorkspaceUi.SelectItem(context, "1776", "Commando");
        window.WaitById("ItemDeleteButton").InvokeButton();
        var deleteDialog = ScenarioUi.WaitForMessageBox(context.App);
        var deleteText = ScenarioUi.ReadMessageBoxText(deleteDialog);
        context.Recorder.Check(
            "Подтверждение удаления перечисляет зависимые строки",
            deleteText.Contains("Items.Attachments (27)", StringComparison.Ordinal),
            deleteText);
        ScenarioUi.AnswerMessageBox(deleteDialog, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemDeleted", "1776"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Удаление возвращает 1776 предметов",
            window.WaitById("ItemCountText").ReadName() == ItemCount(1776),
            window.WaitById("ItemCountText").ReadName());
        window.WaitById("ItemUndoButton").InvokeButton();
        Ui.WaitUntil(
            () => window.WaitById("ItemCountText").ReadName() == ItemCount(1777),
            "Undo удаления возвращает предмет",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Undo возвращает удалённый предмет", true);

        WorkspaceUi.UndoAll(context, "ItemUndoButton", "ItemSaveButton");
        context.Recorder.Check(
            "Все правки отменены перед сменой ID",
            window.WaitById("ItemCountText").ReadName() == ItemCount(1776),
            window.WaitById("ItemCountText").ReadName());

        // Rename to a free key and back: dependent rows follow the key in memory.
        var idBox = window.WaitById("ItemIdBox");
        WorkspaceUi.SelectItem(context, "12", "Commando");
        idBox.SetText("100000");
        window.WaitById("ItemChangeIdButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemIdChanged", "12", "100000"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Смена ID обновляет заголовок",
            window.WaitById("ItemDetailHeader").ReadName() == "100000: Commando",
            window.WaitById("ItemDetailHeader").ReadName());
        Ui.WaitUntil(
            () => WorkspaceUi.ChildSectionCaptions(window, context.Strings.Get("ItemTabChildTables")).Contains(caption27),
            "навеска осталась привязана к переименованному предмету",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Связанные строки переехали на новый ID", true);

        idBox.SetText("12");
        window.WaitById("ItemChangeIdButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemIdChanged", "100000", "12"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Обратное переименование возвращает ключ 12",
            window.WaitById("ItemDetailHeader").ReadName() == "12: Commando",
            window.WaitById("ItemDetailHeader").ReadName());

        // Swap with an occupied key asks for confirmation and swaps both records.
        idBox.SetText("13");
        window.WaitById("ItemChangeIdButton").InvokeButton();
        var swapDialog = ScenarioUi.WaitForMessageBox(context.App);
        var swapText = ScenarioUi.ReadMessageBoxText(swapDialog);
        context.Recorder.Check(
            "Swap предупреждает о занятом ID",
            swapText.Contains("ID предмета 12 уже занят", StringComparison.Ordinal),
            swapText);
        ScenarioUi.AnswerMessageBox(swapDialog, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemIdChanged", "12", "13"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Swap переносит предмет на ключ 13",
            window.WaitById("ItemDetailHeader").ReadName() == "13: Commando",
            window.WaitById("ItemDetailHeader").ReadName());

        idBox.SetText("12");
        window.WaitById("ItemChangeIdButton").InvokeButton();
        var backDialog = ScenarioUi.WaitForMessageBox(context.App);
        ScenarioUi.AnswerMessageBox(backDialog, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemIdChanged", "13", "12"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Обратный swap восстанавливает предмет 12",
            window.WaitById("ItemDetailHeader").ReadName() == "12: Commando",
            window.WaitById("ItemDetailHeader").ReadName());

        WorkspaceUi.UndoAll(context, "ItemUndoButton", "ItemSaveButton");
        WorkspaceUi.SelectItem(context, "12", "Commando");
        context.Recorder.Check(
            "Правки ID отменены",
            window.WaitById("ItemIdBox").ReadValue() == "12"
                && window.WaitById("ItemDetailHeader").ReadName() == "12: Commando",
            window.WaitById("ItemIdBox").ReadValue());

        // New item dialog: identifier, name and class are applied.
        window.WaitById("ItemNewButton").InvokeButton();
        var dialog = ScenarioUi.WaitForWindowWithId(
            context.App,
            "ItemCreationIdBox",
            "диалог создания предмета",
            TimeSpan.FromSeconds(20));
        dialog.WaitById("ItemCreationIdBox").SetText("1776");
        dialog.WaitById("ItemCreationNameBox").SetText("Smoke Item");
        dialog.WaitById("ItemCreationClassCombo").SelectComboItem("Gun");
        dialog.WaitById("ItemCreationOkButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "ItemCreationIdBox");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemCreated", "1776"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Диалог создаёт предмет с заданным именем и классом",
            window.WaitById("ItemDetailHeader").ReadName() == "1776: Smoke Item"
                && window.WaitById("ItemClassText").ReadName() == "Gun",
            $"заголовок: {window.WaitById("ItemDetailHeader").ReadName()}, класс: {window.WaitById("ItemClassText").ReadName()}");
        context.Recorder.Check(
            "Создание добавляет предмет в список",
            window.WaitById("ItemCountText").ReadName() == ItemCount(1777),
            window.WaitById("ItemCountText").ReadName());

        window.WaitById("ItemDeleteButton").InvokeButton();
        var createdDelete = ScenarioUi.WaitForMessageBox(context.App);
        ScenarioUi.AnswerMessageBox(createdDelete, yes: true);
        ScenarioUi.WaitForMessageBoxClosed(context.App);
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemDeleted", "1776"),
            TimeSpan.FromSeconds(20));
        WorkspaceUi.UndoAll(context, "ItemUndoButton", "ItemSaveButton");
        context.Recorder.Check(
            "Создание и удаление отменены",
            window.WaitById("ItemCountText").ReadName() == ItemCount(1776),
            window.WaitById("ItemCountText").ReadName());

        // Final duplicate is saved: only the three tables that own the copied rows may change.
        var itemsFile = data.TableFile("Items/Items.xml");
        var weaponsFile = data.TableFile("Items/Weapons.xml");
        var attachmentsFile = data.TableFile("Items/Attachments.xml");
        var itemsBefore = XDocument.Load(itemsFile);
        var weaponsBefore = XDocument.Load(weaponsFile);
        var attachmentsBefore = XDocument.Load(attachmentsFile);
        var attachmentIndexes = attachmentsBefore.Descendants("ATTACHMENT")
            .Where(row => row.Element("itemIndex")?.Value.Trim() == "12")
            .Select(row => row.Element("attachmentIndex")?.Value ?? string.Empty)
            .ToHashSet(StringComparer.Ordinal);

        WorkspaceUi.SelectItem(context, "12", "Commando");
        window.WaitById("ItemDuplicateButton").InvokeButton();
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemDuplicated", "12", "1776"),
            TimeSpan.FromSeconds(20));
        WorkspaceUi.SaveItem(context);

        var itemsAfter = XDocument.Load(itemsFile);
        var weaponsAfter = XDocument.Load(weaponsFile);
        var attachmentsAfter = XDocument.Load(attachmentsFile);
        var added = itemsAfter.Descendants("ITEM")
            .FirstOrDefault(row => row.Element("uiIndex")?.Value.Trim() == "1776");
        context.Recorder.Check(
            "Сохранённый дубликат имеет имя и класс оригинала",
            added?.Element("szItemName")?.Value == "Commando" && added?.Element("usItemClass")?.Value == "2",
            added?.ToString(SaveOptions.DisableFormatting));
        context.Recorder.Check(
            "Все исходные записи предметов не изменились",
            OriginalRecordsIntact(itemsBefore, itemsAfter, "ITEM", "uiIndex", "1776"),
            null);
        context.Recorder.Check(
            "Строка оружия скопирована без потери исходной",
            weaponsAfter.Descendants("WEAPON").Count(row => row.Element("uiIndex")?.Value.Trim() == "1776") == 1
                && OriginalRecordsIntact(weaponsBefore, weaponsAfter, "WEAPON", "uiIndex", "1776"),
            null);
        var copiedAttachments = attachmentsAfter.Descendants("ATTACHMENT")
            .Where(row => row.Element("itemIndex")?.Value.Trim() == "1776")
            .Select(row => row.Element("attachmentIndex")?.Value ?? string.Empty)
            .ToHashSet(StringComparer.Ordinal);
        context.Recorder.Check(
            "Навеска дубликата образует ровно тот же набор",
            copiedAttachments.SetEquals(attachmentIndexes),
            $"скопировано: {copiedAttachments.Count}, ожидалось: {attachmentIndexes.Count}");
        context.Recorder.Check(
            "Сиротских строк с ключом 1777 нет",
            !attachmentsAfter.Descendants("ATTACHMENT")
                .Any(row => row.Element("itemIndex")?.Value.Trim() == "1777")
                && !itemsAfter.Descendants("ITEM").Any(row => row.Element("uiIndex")?.Value.Trim() == "1777"),
            null);

        context.ExpectChange(DatasetKind.V4, "TableData/Items/Items.xml");
        context.ExpectChange(DatasetKind.V4, "TableData/Items/Weapons.xml");
        context.ExpectChange(DatasetKind.V4, "TableData/Items/Attachments.xml");

        // A light create and undo on the v5 data set: the operation stays in memory, so the reference
        // copy keeps its files and the workspace ends clean.
        ScenarioUi.SelectDataDirectory(window, context.Data(DatasetKind.V5).DataDirectory);
        var loaded = ScenarioUi.FormatPrefix(context.Strings.Get("StatusTablesLoaded"));
        ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(loaded, StringComparison.Ordinal),
            "список v5 загружен",
            TimeSpan.FromSeconds(60));
        ScenarioUi.OpenTable(context, "Items.Items");
        Ui.WaitUntil(
            () => window.WaitById("ItemCountText").ReadName() == ItemCount(1775),
            "рабочее место предмета v5 загружено",
            TimeSpan.FromSeconds(90));
        WorkspaceUi.WaitForStatus(
            context,
            "ItemStatus",
            text => text == context.Strings.Get("EditorReady"),
            "статус v5 «Готово.»",
            TimeSpan.FromSeconds(60));
        context.Recorder.Check("Предметы v5 открыты в отдельной вкладке", true);

        window.WaitById("ItemNewButton").InvokeButton();
        var v5Dialog = ScenarioUi.WaitForWindowWithId(
            context.App,
            "ItemCreationIdBox",
            "диалог создания предмета v5",
            TimeSpan.FromSeconds(20));
        v5Dialog.WaitById("ItemCreationIdBox").SetText("1775");
        v5Dialog.WaitById("ItemCreationNameBox").SetText("Smoke V5");
        v5Dialog.WaitById("ItemCreationClassCombo").SelectComboItem("Gun");
        v5Dialog.WaitById("ItemCreationOkButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, "ItemCreationIdBox");
        WorkspaceUi.WaitItemStatus(
            context,
            context.Strings.Format("ItemCreated", "1775"),
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Создание предмета на v5 добавляет запись",
            window.WaitById("ItemCountText").ReadName() == ItemCount(1776),
            window.WaitById("ItemCountText").ReadName());
        WorkspaceUi.UndoAll(context, "ItemUndoButton", "ItemSaveButton");
        context.Recorder.Check(
            "Undo возвращает v5 к исходным 1775 предметам",
            window.WaitById("ItemCountText").ReadName() == ItemCount(1775),
            window.WaitById("ItemCountText").ReadName());
    }

    private static bool OriginalRecordsIntact(
        XDocument before,
        XDocument after,
        string elementName,
        string keyElement,
        string addedKey)
    {
        var current = after.Descendants(elementName)
            .Where(row => row.Element(keyElement)?.Value.Trim() != addedKey)
            .GroupBy(row => row.Element(keyElement)?.Value.Trim() ?? string.Empty)
            .ToDictionary(group => group.Key, group => group.First().ToString(SaveOptions.DisableFormatting), StringComparer.Ordinal);
        foreach (var row in before.Descendants(elementName))
        {
            var key = row.Element(keyElement)?.Value.Trim() ?? string.Empty;
            if (!current.TryGetValue(key, out var saved)
                || !string.Equals(saved, row.ToString(SaveOptions.DisableFormatting), StringComparison.Ordinal))
            {
                return false;
            }
        }

        return current.Count >= before.Descendants(elementName).Count();
    }
}
