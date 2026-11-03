using System.IO;
using System.Security.Cryptography;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S88: closing the window with unsaved item and mercenary-kit edits asks for confirmation. Cancel keeps
/// the window, both dirty tabs, the data files, and settings; discarding both edits closes the application
/// and writes the window geometry.
/// </summary>
internal sealed class UnsavedChangesCloseScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S88";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var data = context.Data(DatasetKind.V4);
        var itemsFile = data.TableFile("Items/Items.xml");
        var mercFile = data.TableFile("Inventory/MercStartingGear.xml");
        var itemsHash = SaveDirtyTabScenario.HashFile(itemsFile);
        var mercHash = SaveDirtyTabScenario.HashFile(mercFile);
        var settingsPath = Path.Combine(context.App.AppDataDirectory, "settings.json");
        var settingsHash = HashFile(settingsPath);

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");
        WorkspaceUi.SetField(context, "szItemDesc", "Несохранённое описание");
        context.Recorder.Check(
            "Вкладка предметов стала грязной",
            window.WaitById("ItemSaveButton").IsEnabled(),
            null);

        WorkspaceUi.OpenMercGearWorkspace(context);
        WorkspaceUi.SetTextAndCommit(window.WaitById("MercSearchBox"), "Barry");
        Ui.WaitUntil(
            () => window.WaitById("MercDetailHeader").ReadName() == "0: Barry",
            "выбран Barry");
        WorkspaceUi.SelectTab(window, "MercKitTabs", "Standard");
        WorkspaceUi.SetTextAndCommit(window.WaitById("MercKitNameBox"), "Unsaved kit");
        context.Recorder.Check(
            "Вкладка экипировки стала грязной",
            window.WaitById("MercSaveButton").IsEnabled(),
            null);
        var dirtyTabs = ScenarioUi.TabNames(window).Where(name => name.Contains('*')).ToList();
        context.Recorder.Check(
            "Обе вкладки помечены как грязные",
            dirtyTabs.Count == 2,
            "вкладки: " + string.Join(" / ", ScenarioUi.TabNames(window)));

        context.App.RequestClose();
        var cancelDialog = ScenarioUi.WaitForDialog(context.App, TimeSpan.FromSeconds(20));
        var cancelMessage = cancelDialog.WaitById("MessageText").ReadName();
        cancelDialog.WaitById("CancelButton").InvokeButton();
        ScenarioUi.WaitForDialogClosed(context.App);
        context.Recorder.Check(
            "«Отмена» оставляет приложение открытым",
            !context.App.HasExited,
            null);
        context.Recorder.Check(
            "«Отмена» оставляет обе вкладки грязными",
            ScenarioUi.TabNames(window).Count(name => name.Contains('*')) == 2,
            string.Join(" / ", ScenarioUi.TabNames(window)));
        context.Recorder.Check(
            "«Отмена» не пишет игровые файлы",
            SaveDirtyTabScenario.HashFile(itemsFile) == itemsHash
                && SaveDirtyTabScenario.HashFile(mercFile) == mercHash,
            null);
        context.Recorder.Check(
            "«Отмена» не меняет settings.json",
            HashFile(settingsPath) == settingsHash,
            "settings.json изменился до закрытия");
        context.Recorder.Note("Первый диалог: " + cancelMessage);

        context.App.RequestClose();
        for (var index = 0; index < 2; index++)
        {
            var discardDialog = ScenarioUi.WaitForDialog(context.App, TimeSpan.FromSeconds(20));
            discardDialog.WaitById("DiscardButton").InvokeButton();
            Thread.Sleep(300);
        }

        Ui.WaitUntil(
            () => context.App.HasExited,
            "приложение закрылось после отказа от сохранения",
            TimeSpan.FromSeconds(40));
        context.Recorder.Check("«Не сохранять» закрывает приложение", true, null);
        context.Recorder.Check(
            "Отказ от сохранения не пишет игровые файлы",
            SaveDirtyTabScenario.HashFile(itemsFile) == itemsHash
                && SaveDirtyTabScenario.HashFile(mercFile) == mercHash,
            null);
        var placement = WindowUi.ReadPlacement(context.App);
        context.Recorder.Check(
            "Геометрия записана при закрытии",
            placement is not null && placement.Width > 0 && placement.Height > 0,
            placement is null
                ? "windowPlacement отсутствует"
                : $"{placement.Left:0},{placement.Top:0} {placement.Width:0}x{placement.Height:0}");
    }

    private static string HashFile(string path)
    {
        if (!File.Exists(path))
        {
            return string.Empty;
        }

        return Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
    }
}
