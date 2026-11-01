using System.IO;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S44: switching the interface language in the settings takes effect after a restart in both
/// directions, keeps the configured data and the open data set, and never touches the XML.
/// </summary>
internal sealed class UiLanguageScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S44";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var file = v4.TableFile("Lookup/Cursor.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);
        var english = UiStrings.Load(ResourcesDirectory(context), "en");

        var russianSaveAll = context.Strings.Get("EditorSaveAllText");
        Ui.WaitUntil(
            () => context.Window.WaitById("SaveAllButton").ReadName() == russianSaveAll,
            "стартовый интерфейс на русском",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check("Стартовый интерфейс на русском", true, russianSaveAll);

        // Switch to English and accept the dialog; the running UI keeps the old language.
        var dialog = SettingsUi.Open(context);
        context.Recorder.Check(
            "Подсказка о перезапуске показана на русском",
            dialog.WaitById("SettingsLanguageNote").ReadName() == context.Strings.Get("SettingsUiLanguageNote"),
            dialog.WaitById("SettingsLanguageNote").ReadName());
        SettingsUi.SelectLanguage(dialog, english.Get("LanguageEnglish"));
        context.Recorder.Check(
            "В селекторе выбран английский",
            SettingsUi.ReadLanguage(dialog) == english.Get("LanguageEnglish"),
            SettingsUi.ReadLanguage(dialog));
        SettingsUi.Close(context, dialog, accept: true);
        ScenarioUi.WaitForStatus(
            context,
            text => text == context.Strings.Get("SettingsApplied"),
            "настройки сохранены после смены языка");

        var englishSettings = SettingsUi.ReadSettings(context.App);
        context.Recorder.Check(
            "settings.json содержит английский язык",
            englishSettings is not null
                && string.Equals(englishSettings.UiLanguageCode, "en", StringComparison.OrdinalIgnoreCase),
            englishSettings?.UiLanguageCode ?? "settings.json не прочитан");

        // Restart with the saved language; the editor comes up in English.
        context.RestartApp(englishSettings);
        var englishSaveAll = english.Get("EditorSaveAllText");
        Ui.WaitUntil(
            () => context.Window.WaitById("SaveAllButton").ReadName() == englishSaveAll,
            "интерфейс переключился на английский после перезапуска",
            TimeSpan.FromSeconds(60));
        context.Recorder.Check("После перезапуска интерфейс на английском", true, englishSaveAll);

        var englishLoaded = ScenarioUi.FormatPrefix(english.Get("StatusTablesLoaded"));
        ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(englishLoaded, StringComparison.Ordinal),
            "список таблиц загружен на английском",
            TimeSpan.FromSeconds(90));
        context.Recorder.Check(
            "Активный каталог сохранился после перезапуска",
            string.Equals(
                context.Window.WaitById("DataDirectoryCombo").ReadComboSelection(),
                v4.DataDirectory,
                StringComparison.OrdinalIgnoreCase),
            context.Window.WaitById("DataDirectoryCombo").ReadComboSelection());
        context.Recorder.Check(
            "Таблицы данных на месте после смены языка",
            ScenarioUi.FindTableRow(context.Window, "Items.Items") is not null);

        dialog = SettingsUi.Open(context);
        context.Recorder.Check(
            "Подсказка о перезапуске показана на английском",
            dialog.WaitById("SettingsLanguageNote").ReadName() == english.Get("SettingsUiLanguageNote"),
            dialog.WaitById("SettingsLanguageNote").ReadName());
        context.Recorder.Check(
            "Язык в настройках остаётся английским",
            SettingsUi.ReadLanguage(dialog) == english.Get("LanguageEnglish"),
            SettingsUi.ReadLanguage(dialog));

        // Switch back to Russian and restart again.
        SettingsUi.SelectLanguage(dialog, english.Get("LanguageRussian"));
        SettingsUi.Close(context, dialog, accept: true);
        ScenarioUi.WaitForStatus(
            context,
            text => text == english.Get("SettingsApplied"),
            "английские настройки сохранены");

        var russianSettingsAfter = SettingsUi.ReadSettings(context.App);
        context.Recorder.Check(
            "settings.json содержит русский язык",
            russianSettingsAfter is not null
                && string.Equals(russianSettingsAfter.UiLanguageCode, "ru", StringComparison.OrdinalIgnoreCase),
            russianSettingsAfter?.UiLanguageCode ?? "settings.json не прочитан");

        context.RestartApp(russianSettingsAfter);
        Ui.WaitUntil(
            () => context.Window.WaitById("SaveAllButton").ReadName() == russianSaveAll,
            "интерфейс вернулся к русскому после перезапуска",
            TimeSpan.FromSeconds(60));
        context.Recorder.Check("Обратное переключение на русский после перезапуска", true, russianSaveAll);
        var russianLoaded = ScenarioUi.FormatPrefix(context.Strings.Get("StatusTablesLoaded"));
        ScenarioUi.WaitForStatus(
            context,
            text => text.StartsWith(russianLoaded, StringComparison.Ordinal),
            "список таблиц загружен на русском",
            TimeSpan.FromSeconds(90));

        context.Recorder.Check(
            "Смена языка не изменила данные v4",
            SaveDirtyTabScenario.HashFile(file) == hashBefore);
        context.Recorder.Note(
            "Вкладки редактора не переживают перезапуск приложения по замыслу; проверены список таблиц, "
            + "активный каталог и XML.");
    }

    private static string ResourcesDirectory(ScenarioContext context) =>
        Path.Combine(
            context.Options.RepositoryRoot,
            "xml-editor-reloaded",
            "src",
            "XmlEditorReloaded.App",
            "Resources");
}
