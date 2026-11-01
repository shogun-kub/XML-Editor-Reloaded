using System.IO;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S43: the settings dialog lists the configured data directories, adds and removes a temporary
/// directory, applies the active directory without closing open tabs, and discards changes on Cancel.
/// </summary>
internal sealed class SettingsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S43";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var v5 = context.Data(DatasetKind.V5);
        var v4Hash = DataOperationsUi.HashTree(v4.TableDataDirectory);
        var v5Hash = DataOperationsUi.HashTree(v5.TableDataDirectory);

        ScenarioUi.OpenTable(context, "Backgrounds");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Backgrounds открыт");
        ScenarioUi.WaitForEditorTabCount(context.Window, 1);

        // The table exists only in v5, so the v4 list must not offer it.
        context.Recorder.Check(
            "В списке v4 нет таблицы, которой нет в этой версии",
            ScenarioUi.FindTableRow(context.Window, "Email.EmailSenderNameList") is null);

        var addable = CreateAddableDirectory(context, v4);

        var dialog = SettingsUi.Open(context);
        context.Recorder.Check("Окно настроек открыто", true);

        var directories = SettingsUi.ReadDirectories(dialog);
        context.Recorder.Check(
            "Настройки перечисляют оба каталога данных",
            directories.Count == 2
                && directories.Any(row => SamePath(row.Path, v4.DataDirectory))
                && directories.Any(row => SamePath(row.Path, v5.DataDirectory)),
            string.Join(" | ", directories.Select(static row => row.Path)));
        context.Recorder.Check(
            "Активным отмечен каталог v4",
            directories.FirstOrDefault(row => row.IsActive).Path is { } active
                && SamePath(active, v4.DataDirectory),
            string.Join(" | ", directories.Select(static row => $"{row.Path}: {row.IsActive}")));
        context.Recorder.Check(
            "Подпись активного каталога показывает v4",
            SamePath(dialog.WaitById("SettingsActiveDirectoryText").ReadName(), v4.DataDirectory),
            dialog.WaitById("SettingsActiveDirectoryText").ReadName());
        context.Recorder.Check(
            "Выбран русский язык интерфейса",
            SettingsUi.ReadLanguage(dialog) == context.Strings.Get("LanguageRussian"),
            SettingsUi.ReadLanguage(dialog));
        context.Recorder.Check(
            "Подсказка о перезапуске после смены языка показана",
            dialog.WaitById("SettingsLanguageNote").ReadName()
                == context.Strings.Get("SettingsUiLanguageNote"),
            dialog.WaitById("SettingsLanguageNote").ReadName());
        var recent = dialog
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List))
            .FirstOrDefault();
        context.Recorder.Check(
            "Список недавних каталогов доступен только для чтения",
            recent is not null && !recent.IsEnabled(),
            recent is null ? "список не найден" : $"IsEnabled: {recent.IsEnabled()}");

        SettingsUi.AddDirectory(context, dialog, addable);
        var addedStatus = context.Strings.Get("SettingsDirectoryAdded");
        Ui.WaitUntil(
            () => dialog.WaitById("SettingsStatusText").ReadName() == addedStatus,
            "каталог добавлен",
            TimeSpan.FromSeconds(20));
        var afterAdd = SettingsUi.ReadDirectories(dialog);
        context.Recorder.Check(
            "Добавленный каталог появился в списке",
            afterAdd.Count == 3 && afterAdd.Any(row => SamePath(row.Path, addable)),
            string.Join(" | ", afterAdd.Select(static row => row.Path)));

        SettingsUi.SelectDirectory(dialog, addable);
        SettingsUi.RemoveDirectory(dialog);
        var removedStatus = context.Strings.Get("SettingsDirectoryRemoved");
        Ui.WaitUntil(
            () => dialog.WaitById("SettingsStatusText").ReadName() == removedStatus,
            "каталог удалён",
            TimeSpan.FromSeconds(20));
        context.Recorder.Check(
            "Удалённый каталог исчез из списка",
            SettingsUi.ReadDirectories(dialog).Count == 2);

        // Cancel must discard the active-directory change made in the dialog.
        SettingsUi.SelectDirectory(dialog, v5.DataDirectory);
        SettingsUi.SetActiveDirectory(dialog);
        Ui.WaitUntil(
            () => SamePath(dialog.WaitById("SettingsActiveDirectoryText").ReadName(), v5.DataDirectory),
            "v5 выбран активным в диалоге");
        SettingsUi.Close(context, dialog, accept: false);

        var settingsAfterCancel = SettingsUi.ReadSettings(context.App);
        context.Recorder.Check(
            "«Отмена» не меняет активный каталог в настройках",
            settingsAfterCancel is not null
                && SamePath(settingsAfterCancel.ActiveDataDirectory, v4.DataDirectory),
            settingsAfterCancel?.ActiveDataDirectory ?? "settings.json не прочитан");
        context.Recorder.Check(
            "«Отмена» оставляет активный каталог v4 в главном окне",
            SamePath(context.Window.WaitById("DataDirectoryCombo").ReadComboSelection(), v4.DataDirectory),
            context.Window.WaitById("DataDirectoryCombo").ReadComboSelection());

        // OK applies the change without closing the open tab.
        dialog = SettingsUi.Open(context);
        SettingsUi.SelectDirectory(dialog, v5.DataDirectory);
        SettingsUi.SetActiveDirectory(dialog);
        SettingsUi.Close(context, dialog, accept: true);

        ScenarioUi.WaitForStatus(
            context,
            text => text == context.Strings.Get("SettingsApplied"),
            "настройки сохранены после смены активного каталога",
            TimeSpan.FromSeconds(90));
        context.Recorder.Check(
            "Смена активного каталога применяется без закрытия вкладки",
            ScenarioUi.TabNames(context.Window).Any(name => name.Contains("Backgrounds", StringComparison.Ordinal)),
            string.Join(" | ", ScenarioUi.TabNames(context.Window)));
        context.Recorder.Check(
            "Селектор главного окна показывает v5",
            SamePath(context.Window.WaitById("DataDirectoryCombo").ReadComboSelection(), v5.DataDirectory),
            context.Window.WaitById("DataDirectoryCombo").ReadComboSelection());
        Ui.WaitUntil(
            () => ScenarioUi.FindTableRow(context.Window, "Email.EmailSenderNameList") is not null,
            "список v5 содержит таблицу, которой нет в v4",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            "В списке v5 появилась таблица, которой нет в v4",
            ScenarioUi.FindTableRow(context.Window, "Email.EmailSenderNameList") is not null);

        var settingsAfterOk = SettingsUi.ReadSettings(context.App);
        context.Recorder.Check(
            "«ОК» сохраняет активный каталог v5",
            settingsAfterOk is not null
                && SamePath(settingsAfterOk.ActiveDataDirectory, v5.DataDirectory)
                && settingsAfterOk.DataDirectories.Count == 2,
            settingsAfterOk is null
                ? "settings.json не прочитан"
                : $"{settingsAfterOk.ActiveDataDirectory}; каталогов: {settingsAfterOk.DataDirectories.Count}");

        context.Recorder.Check(
            "Настройки не изменили данные v4",
            DataOperationsUi.HashTree(v4.TableDataDirectory) == v4Hash);
        context.Recorder.Check(
            "Настройки не изменили данные v5",
            DataOperationsUi.HashTree(v5.TableDataDirectory) == v5Hash);
    }

    /// <summary>
    /// Creates a minimal data directory outside the data-set fixtures so the folder picker can add it
    /// without touching the compared data.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="v4">Baseline data set that supplies one table file.</param>
    /// <returns>Absolute path of the game root that contains <c>Data-1.13\TableData</c>.</returns>
    private static string CreateAddableDirectory(ScenarioContext context, DatasetFixture v4)
    {
        var root = Path.Combine(context.Directory, "addable-data");
        var tableData = Path.Combine(root, "Data-1.13", "TableData");
        Directory.CreateDirectory(tableData);
        File.Copy(v4.TableFile("Lookup/Cursor.xml"), Path.Combine(tableData, "Lookup.Cursor.xml"), overwrite: true);
        return root;
    }

    private static bool SamePath(string? first, string? second)
    {
        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second))
        {
            return false;
        }

        return string.Equals(
            Path.TrimEndingDirectorySeparator(first),
            Path.TrimEndingDirectorySeparator(second),
            StringComparison.OrdinalIgnoreCase);
    }
}
