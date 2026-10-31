using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S13: closing a dirty tab asks for confirmation; cancel keeps the tab, discard closes it without
/// writing, and save writes the file before closing.
/// </summary>
internal sealed class CloseDirtyTabScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S13";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => kind == DatasetKind.V4;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V4).TableFile("Lookup/Cursor.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);
        context.Recorder.Note("Ветка ошибки сохранения отдельно не воспроизводится.");

        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor открыт");
        var grid = window.WaitById("RowsGrid");

        // Round 1: cancel keeps the tab and its edits.
        grid.EditCell(0, 1, "Cancel Keeps This");
        CloseActiveTab(window, "Lookup.Cursor");
        var dialog = ScenarioUi.WaitForDialog(context.App, TimeSpan.FromSeconds(15));
        var message = dialog.WaitById("MessageText").ReadName();
        context.Recorder.Check(
            "Диалог называет таблицу с несохранёнными изменениями",
            message.Contains("Lookup.Cursor", StringComparison.Ordinal),
            message);
        dialog.WaitById("CancelButton").InvokeButton();
        Ui.WaitUntil(() => window.FindById("RowsGrid") is not null, "вкладка осталась открытой");
        context.Recorder.Check("«Отмена» оставляет вкладку открытой", true);
        context.Recorder.Check(
            "«Отмена» не пишет файл",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);

        // Round 2: discard closes without writing.
        CloseActiveTab(window, "Lookup.Cursor");
        var discardDialog = ScenarioUi.WaitForDialog(context.App, TimeSpan.FromSeconds(15));
        discardDialog.WaitById("DiscardButton").InvokeButton();
        Ui.WaitUntil(
            () => !ScenarioUi.TabNames(window).Any(name => name.Contains("Lookup.Cursor", StringComparison.Ordinal)),
            "вкладка закрыта без сохранения");
        context.Recorder.Check(
            "«Не сохранять» закрывает вкладку и не пишет файл",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);

        // Round 3: save writes the file and closes the tab.
        ScenarioUi.OpenTable(context, "Lookup.Cursor");
        ScenarioUi.WaitForEditorStatus(
            context,
            text => text == context.Strings.Get("EditorReady"),
            "Lookup.Cursor открыт повторно");
        window.WaitById("RowsGrid").EditCell(0, 1, "Saved On Close");
        CloseActiveTab(window, "Lookup.Cursor");
        var saveDialog = ScenarioUi.WaitForDialog(context.App, TimeSpan.FromSeconds(15));
        saveDialog.WaitById("SaveButton").InvokeButton();
        Ui.WaitUntil(
            () => !ScenarioUi.TabNames(window).Any(name => name.Contains("Lookup.Cursor", StringComparison.Ordinal)),
            "вкладка закрыта после сохранения",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check("«Сохранить» закрывает вкладку", true);
        context.Recorder.Check(
            "«Сохранить» пишет файл",
            SaveDirtyTabScenario.HashFile(file) != hashBefore,
            null);
        context.ExpectChange(DatasetKind.V4, "TableData/Lookup/Cursor.xml");

        var backups = ScenarioUi.BackupCopies(file);
        context.Recorder.Check(
            "Рядом с файлом не появилось резервных копий",
            backups.Count == 0,
            string.Join(", ", backups));
    }

    private static void CloseActiveTab(AutomationElement window, string tableName)
    {
        var tab = window.WaitById("EditorsTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .FirstOrDefault(candidate => candidate.ReadName().Contains(tableName, StringComparison.Ordinal)
                || candidate.FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
                    .Any(text => text.ReadName().Contains(tableName, StringComparison.Ordinal)));
        if (tab is null)
        {
            throw new UiTimeoutException($"Вкладка {tableName} не найдена.");
        }

        var close = tab.FindFirst(TreeScope.Descendants, Ui.ByName("✕"))
            ?? throw new UiTimeoutException("Кнопка закрытия вкладки не найдена.");
        // InvokePattern.Invoke returns only after the modal confirmation dialog closes, so the call is
        // moved off the scenario thread and the dialog is handled while the invoke is still pending.
        _ = Task.Run(() =>
        {
            try
            {
                close.InvokeButton();
            }
            catch (Exception exception) when (
                exception is UiTimeoutException or ElementNotAvailableException)
            {
                // The dialog closed the tab and the button disappeared before the invoke returned.
            }
        });
        Thread.Sleep(500);
    }
}
