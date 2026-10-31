using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S15: the item workspace opens in master-detail mode with the item counter, the search and navigation
/// controls, the record buttons, and all six tabs of the item form.
/// </summary>
internal sealed class ItemWorkspaceScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S15";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var count = WorkspaceUi.OpenItemWorkspace(context);
        var expectedCount = context.Strings.Format("ItemCountFormat", 1776, 1776);
        context.Recorder.Check(
            "Счётчик показывает все 1776 предметов",
            count == expectedCount,
            count);

        var expectedTabs = new[]
        {
            context.Strings.Get("ItemTabFields"),
            context.Strings.Get("ItemTabSubTables"),
            context.Strings.Get("ItemTabChildTables"),
            context.Strings.Get("ItemTabShopkeepers"),
            context.Strings.Get("ItemTabNested"),
            context.Strings.Get("ItemTabImages"),
        };
        var tabs = WorkspaceUi.ItemTabCaptions(window);
        context.Recorder.Check(
            "Вкладки «Поля», «Подтаблицы», «Дочерние таблицы», «Торговцы», «Вложенные структуры», «Изображения»",
            tabs.SequenceEqual(expectedTabs),
            string.Join(" | ", tabs));

        var list = window.WaitById("ItemList");
        var firstEntry = WorkspaceUi.ListItemTexts(list).FirstOrDefault() ?? string.Empty;
        context.Recorder.Check(
            "Список предметов показывает записи вида «ID: имя»",
            firstEntry.StartsWith("0: ", StringComparison.Ordinal),
            firstEntry);
        context.Recorder.Check(
            "Поиск предмета доступен",
            window.FindById("ItemSearchBox") is not null,
            context.Strings.Get("ItemSearchLabel"));
        context.Recorder.Check(
            "Переход к ID доступен",
            window.FindById("ItemGoToIdBox") is not null
                && window.FindById("ItemGoToIdButton") is not null,
            context.Strings.Get("ItemGoToIdText"));

        var newButton = window.WaitById("ItemNewButton");
        var duplicateButton = window.WaitById("ItemDuplicateButton");
        var deleteButton = window.WaitById("ItemDeleteButton");
        context.Recorder.Check(
            "Кнопки «Новый предмет», «Дублировать», «Удалить» на месте и доступны",
            newButton.ReadName() == context.Strings.Get("ItemNewText")
                && duplicateButton.ReadName() == context.Strings.Get("EditorDuplicateRowText")
                && deleteButton.ReadName() == context.Strings.Get("EditorDeleteRowText")
                && newButton.IsEnabled()
                && duplicateButton.IsEnabled()
                && deleteButton.IsEnabled(),
            $"{newButton.ReadName()} / {duplicateButton.ReadName()} / {deleteButton.ReadName()}");

        var header = window.WaitById("ItemDetailHeader").ReadName();
        var id = window.WaitById("ItemIdBox").ReadValue();
        context.Recorder.Check(
            "Мастер-деталь показывает выбранный предмет",
            header.StartsWith("0: ", StringComparison.Ordinal) && id == "0",
            $"{header} (ID {id})");
        context.Recorder.Check(
            "Класс выбранного предмета отображается",
            window.WaitById("ItemClassText").ReadName().Length > 0,
            window.WaitById("ItemClassText").ReadName());
        context.Recorder.Check(
            "Сохранение на чистом рабочем месте недоступно",
            !window.WaitById("ItemSaveButton").IsEnabled(),
            null);

        context.Recorder.Note(
            "Миниатюры списка — визуальная часть: их загрузку UIA не подтверждает, кадр проверяется в S22.");
    }
}
