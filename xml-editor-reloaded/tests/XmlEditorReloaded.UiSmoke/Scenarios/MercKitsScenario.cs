using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S27: the five kit tabs show the stored kit names, renaming a kit renames its tab after the name is
/// committed, an empty name falls back to "Комплект N", and the name field accepts at most 80 characters.
/// </summary>
internal sealed class MercKitsScenario : SmokeScenario
{
    private const string KitNameLimit = "Комплект 1";

    /// <inheritdoc />
    public override string Id => "S27";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        WorkspaceUi.OpenMercGearWorkspace(context);
        var search = window.WaitById("MercSearchBox");
        WorkspaceUi.SetTextAndCommit(search, "Barry");
        Ui.WaitUntil(
            () => window.WaitById("MercDetailHeader").ReadName() == "0: Barry",
            "выбран Barry");

        var tabs = WorkspaceUi.TabCaptions(window, "MercKitTabs");
        var expected = new[] { "Standard", "Mechanic", "Combat", "Sapper", "Premium" };
        context.Recorder.Check(
            "Пять вкладок комплектов названы по именам наборов",
            tabs.SequenceEqual(expected),
            string.Join(" | ", tabs));

        for (var index = 0; index < expected.Length; index++)
        {
            WorkspaceUi.SelectTab(window, "MercKitTabs", expected[index]);
            var name = window.WaitById("MercKitNameBox").ReadValue();
            context.Recorder.Check(
                $"Комплект {index + 1} показывает имя {expected[index]}",
                name == expected[index],
                name);
        }

        WorkspaceUi.SelectTab(window, "MercKitTabs", "Standard");
        var nameBox = window.WaitById("MercKitNameBox");
        WorkspaceUi.SetTextAndCommit(nameBox, "Полевой");
        WorkspaceUi.WaitForStatus(
            context,
            "MercStatus",
            text => text == context.Strings.Format("EditorCellUpdated", "mGearKitName", "0"),
            "статус переименования комплекта",
            TimeSpan.FromSeconds(20));
        var renamed = WorkspaceUi.TabCaptions(window, "MercKitTabs");
        context.Recorder.Check(
            "Заголовок вкладки меняется на введённое имя",
            renamed[0] == "Полевой",
            string.Join(" | ", renamed));
        context.Recorder.Check(
            "Переименование делает вкладку грязной",
            window.WaitById("MercSaveButton").IsEnabled(),
            null);

        nameBox = window.WaitById("MercKitNameBox");
        WorkspaceUi.SetTextAndCommit(nameBox, string.Empty);
        Ui.WaitUntil(
            () => WorkspaceUi.TabCaptions(window, "MercKitTabs").FirstOrDefault() == KitNameLimit,
            $"пустое имя возвращает «{KitNameLimit}»",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check("Пустое имя возвращает «Комплект 1»", true);

        nameBox = window.WaitById("MercKitNameBox");
        var longName = new string('К', 120);
        Paste(nameBox, longName);
        var stored = window.WaitById("MercKitNameBox").ReadValue();
        context.Recorder.Check(
            "Вставка длиннее 80 символов обрезается до 80",
            stored.Length == 80,
            $"длина: {stored.Length}");

        WorkspaceUi.UndoAll(context, "MercUndoButton", "MercSaveButton");
        context.Recorder.Check(
            "После отмены всех правок вкладка снова чистая",
            !window.WaitById("MercSaveButton").IsEnabled(),
            null);
    }

    /// <summary>
    /// Replaces the text of an input through the clipboard, so the field applies its length limit exactly
    /// as it does for a user paste.
    /// </summary>
    /// <param name="input">Editable element.</param>
    /// <param name="text">Text to paste.</param>
    private static void Paste(AutomationElement input, string text)
    {
        System.Windows.Clipboard.SetText(text);
        input.Focus();
        NativeInput.PressIn(input, NativeInput.VirtualKey.A, control: true);
        NativeInput.PressIn(input, NativeInput.VirtualKey.V, control: true);
        Thread.Sleep(200);
        NativeInput.PressIn(input, NativeInput.VirtualKey.Tab);
        Thread.Sleep(300);
    }
}
