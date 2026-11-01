using System.IO;
using System.Text.Json;
using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Drives the modal settings dialog, the folder picker it opens, and the settings file the application
/// writes. The dialog is modal, so opening and closing it is dispatched in the background while the
/// scenario answers the controls from the main thread.
/// </summary>
internal static class SettingsUi
{
    /// <summary>
    /// Automation identifier of the settings dialog, taken from its OK button.
    /// </summary>
    public const string DialogMarker = "SettingsOkButton";

    private static readonly JsonSerializerOptions SettingsJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    /// <summary>
    /// Opens the settings dialog from the main toolbar.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <returns>The settings window.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the dialog does not appear in time.</exception>
    public static AutomationElement Open(ScenarioContext context)
    {
        var button = context.Window.WaitById("SettingsButton");
        _ = Task.Run(() =>
        {
            try
            {
                button.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The button is disabled while the modal dialog runs; the dialog itself is the result.
            }
        });

        var dialog = ScenarioUi.WaitForWindowWithId(
            context.App,
            DialogMarker,
            "окно настроек",
            TimeSpan.FromSeconds(60));
        NativeInput.Activate(new IntPtr(dialog.Current.NativeWindowHandle));
        return dialog;
    }

    /// <summary>
    /// Closes the settings dialog by accepting or cancelling it.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dialog">Settings window.</param>
    /// <param name="accept">Whether to press OK; otherwise Cancel.</param>
    public static void Close(ScenarioContext context, AutomationElement dialog, bool accept)
    {
        dialog.WaitById(accept ? "SettingsOkButton" : "SettingsCancelButton").InvokeButton();
        ScenarioUi.WaitForWindowWithIdClosed(context.App, DialogMarker, TimeSpan.FromSeconds(60));
        context.App.RefreshWindow();
    }

    /// <summary>
    /// Reads the configured data directories and their active flag from the settings grid.
    /// </summary>
    /// <param name="dialog">Settings window.</param>
    /// <returns>Rows in grid order.</returns>
    public static IReadOnlyList<(string Path, bool IsActive)> ReadDirectories(AutomationElement dialog)
    {
        var grid = dialog.WaitById("SettingsDirectoriesGrid");
        var rows = new List<(string Path, bool IsActive)>();
        for (var index = 0; index < grid.RowCount(); index++)
        {
            var path = DataOperationsUi.GridCellText(grid, index, 1);
            var checkBox = grid
                .Cell(index, 0)
                .FindFirst(
                    TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.CheckBox));
            rows.Add((path, checkBox?.IsChecked() == true));
        }

        return rows;
    }

    /// <summary>
    /// Selects the row of a configured data directory.
    /// </summary>
    /// <param name="dialog">Settings window.</param>
    /// <param name="path">Absolute directory path.</param>
    /// <exception cref="UiTimeoutException">Thrown when the directory is not listed.</exception>
    public static void SelectDirectory(AutomationElement dialog, string path)
    {
        var grid = dialog.WaitById("SettingsDirectoriesGrid");
        var row = grid
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem))
            .FirstOrDefault(candidate => candidate
                .ReadRowCells()
                .Any(cell => string.Equals(cell, path, StringComparison.OrdinalIgnoreCase)))
            ?? throw new UiTimeoutException($"В настройках нет строки каталога «{path}».");
        row.SelectItem();
        Thread.Sleep(150);
    }

    /// <summary>
    /// Opens the folder picker of the Add button and enters a directory path into it.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dialog">Settings window.</param>
    /// <param name="path">Absolute directory path to add.</param>
    public static void AddDirectory(ScenarioContext context, AutomationElement dialog, string path)
    {
        var add = dialog.WaitById("SettingsAddButton");
        _ = Task.Run(() =>
        {
            try
            {
                add.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The command runs until the folder picker closes; the dialog is the visible result.
            }
        });

        var picker = NativeDialogUi.WaitForDialog(
            context.App,
            context.Strings.Get("SettingsChooseDirectory"),
            "окно выбора каталога",
            TimeSpan.FromSeconds(60));
        NativeDialogUi.AcceptPath(picker, path);
        NativeInput.Activate(new IntPtr(dialog.Current.NativeWindowHandle));
    }

    /// <summary>
    /// Selects a configured directory row and makes it the active data set.
    /// </summary>
    /// <param name="dialog">Settings window.</param>
    /// <exception cref="UiTimeoutException">Thrown when the row cannot be selected.</exception>
    public static void SetActiveDirectory(AutomationElement dialog)
    {
        dialog.WaitById("SettingsSetActiveButton").InvokeButton();
        Thread.Sleep(200);
    }

    /// <summary>
    /// Removes the selected directory row.
    /// </summary>
    /// <param name="dialog">Settings window.</param>
    public static void RemoveDirectory(AutomationElement dialog)
    {
        dialog.WaitById("SettingsRemoveButton").InvokeButton();
        Thread.Sleep(200);
    }

    /// <summary>
    /// Selects an interface language by its visible name.
    /// </summary>
    /// <param name="dialog">Settings window.</param>
    /// <param name="displayName">Visible language name, for example <c>English</c>.</param>
    public static void SelectLanguage(AutomationElement dialog, string displayName) =>
        dialog.WaitById("SettingsLanguageCombo").SelectComboItem(displayName);

    /// <summary>
    /// Reads the selected interface language name.
    /// </summary>
    /// <param name="dialog">Settings window.</param>
    /// <returns>Visible language name.</returns>
    public static string ReadLanguage(AutomationElement dialog) =>
        dialog.WaitById("SettingsLanguageCombo").ReadComboSelection();

    /// <summary>
    /// Presses "Reset all layouts" and confirms the native message box.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="dialog">Settings window.</param>
    /// <returns>The message text of the confirmation box.</returns>
    public static string ResetLayouts(ScenarioContext context, AutomationElement dialog)
    {
        var reset = dialog.WaitById("SettingsResetLayoutsButton");
        _ = Task.Run(() =>
        {
            try
            {
                reset.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The command waits for the confirmation message box.
            }
        });

        var box = NativeDialogUi.WaitForDialog(
            context.App,
            context.Strings.Get("SettingsResetLayoutsTitle"),
            "подтверждение сброса макетов",
            TimeSpan.FromSeconds(30));
        var message = ScenarioUi.ReadMessageBoxText(box);
        ScenarioUi.AnswerMessageBox(box, yes: true);
        Ui.WaitUntil(
            () => dialog.WaitById("SettingsResetLayoutsPendingText").ReadName()
                == context.Strings.Get("SettingsResetLayoutsPending"),
            "предупреждение о сбросе макетов",
            TimeSpan.FromSeconds(20));
        return message;
    }

    /// <summary>
    /// Reads the settings file the application wrote into the isolated application-data directory.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <returns>The parsed settings, or <see langword="null"/> when the file is missing or unreadable.</returns>
    public static SettingsModel? ReadSettings(UiApp app)
    {
        var path = Path.Combine(app.AppDataDirectory, "settings.json");
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<SettingsModel>(File.ReadAllText(path), SettingsJsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
