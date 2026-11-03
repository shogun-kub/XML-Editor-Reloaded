using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S87: renaming a mercenary gear kit updates its tab header after commit, an empty name falls back to the
/// localized "Kit N", the rename is one undo record, the name field accepts at most 80 characters, and the
/// saved name is written to the gear file.
/// </summary>
internal sealed class MercKitRenameScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S87";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        WorkspaceUi.OpenMercGearWorkspace(context);
        WorkspaceUi.SetTextAndCommit(window.WaitById("MercSearchBox"), "Barry");
        Ui.WaitUntil(
            () => window.WaitById("MercDetailHeader").ReadName() == "0: Barry",
            "выбран Barry");

        WorkspaceUi.SelectTab(window, "MercKitTabs", "Standard");
        var nameBox = window.WaitById("MercKitNameBox");
        WorkspaceUi.SetTextAndCommit(nameBox, "Field kit");
        Ui.WaitUntil(
            () => WorkspaceUi.TabCaptions(window, "MercKitTabs").FirstOrDefault() == "Field kit",
            "заголовок вкладки обновился",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Заголовок вкладки стал Field kit", true);

        window.WaitById("MercUndoButton").InvokeButton();
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Get("EditorUndone"),
            "переименование отменено",
            TimeSpan.FromSeconds(20));
        var afterUndo = WorkspaceUi.TabCaptions(window, "MercKitTabs").FirstOrDefault();
        context.Recorder.Check(
            "Одна отмена возвращает Standard, а не посимвольные шаги",
            afterUndo == "Standard"
                && window.WaitById("MercKitNameBox").ReadValue() == "Standard",
            $"вкладка: {afterUndo}, поле: {window.WaitById("MercKitNameBox").ReadValue()}");

        nameBox = window.WaitById("MercKitNameBox");
        WorkspaceUi.SetTextAndCommit(nameBox, string.Empty);
        Ui.WaitUntil(
            () => WorkspaceUi.TabCaptions(window, "MercKitTabs").FirstOrDefault() == "Комплект 1",
            "пустое имя вернуло Комплект 1",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Пустое имя возвращает Комплект 1", true);

        nameBox = window.WaitById("MercKitNameBox");
        nameBox.Focus();
        NativeInput.PressIn(nameBox, NativeInput.VirtualKey.A, control: true);
        NativeInput.TypeTextIn(nameBox, new string('K', 120));
        NativeInput.PressIn(nameBox, NativeInput.VirtualKey.Tab);
        Thread.Sleep(300);
        var stored = window.WaitById("MercKitNameBox").ReadValue();
        context.Recorder.Check(
            "Вставка длиннее 80 символов обрезается до 80",
            stored.Length == 80,
            $"длина: {stored.Length}");

        nameBox = window.WaitById("MercKitNameBox");
        WorkspaceUi.SetTextAndCommit(nameBox, "Field kit");
        Ui.WaitUntil(
            () => WorkspaceUi.TabCaptions(window, "MercKitTabs").FirstOrDefault() == "Field kit",
            "вкладка снова Field kit",
            TimeSpan.FromSeconds(20));
        WorkspaceUi.SaveMercGear(context);
        context.ExpectChange(DatasetKind.V4, "TableData/Inventory/MercStartingGear.xml");

        var file = context.Data(DatasetKind.V4).TableFile("Inventory/MercStartingGear.xml");
        var kit = XDocument.Load(file)
            .Descendants("MERCGEAR")
            .FirstOrDefault(element => element.Element("mIndex")?.Value.Trim() == "0")
            ?.Descendants("GEARKIT")
            .FirstOrDefault();
        context.Recorder.Check(
            "Сохранённое имя записано в файл",
            kit?.Element("mGearKitName")?.Value == "Field kit",
            kit?.Element("mGearKitName")?.Value);
    }
}
