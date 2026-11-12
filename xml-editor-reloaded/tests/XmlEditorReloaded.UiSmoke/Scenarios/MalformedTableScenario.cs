using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S48: a malformed v5 table is only diagnosed; it is never made editable, never saved, and no
/// repaired copy appears next to it.
/// </summary>
internal sealed class MalformedTableScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S48";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V5];

    /// <inheritdoc />
    public override bool WritesTo(DatasetKind kind) => true;

    /// <inheritdoc />
    public override bool CorruptsV5AttachmentPoint => true;

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var file = context.Data(DatasetKind.V5).TableFile("Lookup/AttachmentPoint.xml");
        var hashBefore = SaveDirtyTabScenario.HashFile(file);

        ScenarioUi.OpenTable(context, "Lookup.AttachmentPoint");
        var malformed = context.Strings.Get("StatusMalformed");
        // The error status is a format string such as "Ошибка: {0}", so only its prefix is compared.
        var errorPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("StatusError"));
        bool ReportsMalformed(string text) =>
            text.Contains(malformed, StringComparison.Ordinal)
            || text.Contains(errorPrefix, StringComparison.Ordinal);
        var status = ScenarioUi.WaitForEditorStatus(
            context,
            ReportsMalformed,
            "редактор сообщает о повреждённом файле",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            "Повреждённый файл диагностируется",
            ReportsMalformed(status),
            status);

        var save = window.WaitById("SaveButton");
        context.Recorder.Check(
            "Сохранение повреждённой таблицы недоступно",
            !save.IsEnabled(),
            $"IsEnabled: {save.IsEnabled()}");

        var rows = window.WaitById("RowsGrid").RowCount();
        context.Recorder.Check(
            "Повреждённая таблица не показывает редактируемых строк",
            rows == 0,
            $"строк: {rows}");

        // The validation expander starts collapsed, so the parse error is read from the always visible
        // diagnostics list of the details panel, with the validation list added when it is expanded.
        var diagnostics = new List<string>();
        var detailsList = window.WaitById("DiagnosticsList");
        diagnostics.AddRange(ReadListItems(detailsList));
        var validationList = window.FindById("ValidationList");
        if (validationList is not null)
        {
            diagnostics.AddRange(ReadListItems(validationList));
        }

        context.Recorder.Check(
            "Диагностика перечисляет ошибку разбора",
            diagnostics.Any(text => text.Contains("xml.parse_error", StringComparison.Ordinal)),
            diagnostics.Count == 0 ? "диагностика пуста" : string.Join(" | ", diagnostics));

        Ui.WaitUntil(
            () => SaveDirtyTabScenario.HashFile(file) == hashBefore,
            "файл не перезаписан");
        context.Recorder.Check("Хеш файла не изменился", true);

        var extras = ScenarioUi.BackupCopies(file);
        context.Recorder.Check(
            "Рядом с файлом не появилось исправленных копий",
            extras.Count == 0,
            string.Join(", ", extras));

        // Closing the tab must not ask to save and must not write anything.
        var tab = window.WaitById("EditorsTabs")
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))
            .FirstOrDefault(candidate => candidate
                .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
                .Any(text => text.ReadName().Contains("AttachmentPoint", StringComparison.Ordinal)));
        context.Recorder.Check("Вкладка повреждённой таблицы открыта", tab is not null);
        if (tab is not null)
        {
            var close = tab.FindFirst(TreeScope.Descendants, Ui.ByName("✕"))
                ?? throw new UiTimeoutException("Кнопка закрытия вкладки не найдена.");
            // A confirmation dialog would block the synchronous invoke, so the close runs in the
            // background and the timeout below reports the unexpected dialog instead of hanging.
            _ = Task.Run(() =>
            {
                try
                {
                    close.InvokeButton();
                }
                catch (Exception exception) when (
                    exception is UiTimeoutException or ElementNotAvailableException)
                {
                    // The tab was closed and the button disappeared before the invoke returned.
                }
            });
            Ui.WaitUntil(
                () => !ScenarioUi.TabNames(window).Any(
                    name => name.Contains("AttachmentPoint", StringComparison.Ordinal)),
                "вкладка повреждённой таблицы закрыта без запроса сохранения",
                TimeSpan.FromSeconds(10));
            context.Recorder.Check(
                "Закрытие не спрашивает сохранение",
                ScenarioUi.TopLevelWindowCount(context.App.ProcessId) == 1,
                $"окон процесса: {ScenarioUi.TopLevelWindowCount(context.App.ProcessId)}");
        }

        // Reopening still shows the same diagnostic and the same bytes.
        ScenarioUi.OpenTable(context, "Lookup.AttachmentPoint");
        ScenarioUi.WaitForEditorStatus(
            context,
            ReportsMalformed,
            "повторное открытие снова показывает повреждение",
            TimeSpan.FromSeconds(30));
        context.Recorder.Check(
            "Повторное открытие снова диагностирует файл",
            SaveDirtyTabScenario.HashFile(file) == hashBefore,
            null);
    }

    /// <summary>
    /// Reads the visible texts of the items of a list control.
    /// </summary>
    /// <param name="list">List control.</param>
    /// <returns>Item texts in list order.</returns>
    private static IReadOnlyList<string> ReadListItems(AutomationElement list) =>
        list.FindAll(
                new OrCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem)))
            .Select(static item =>
            {
                var name = item.ReadName();
                return string.IsNullOrWhiteSpace(name) ? Ui.ItemText(item) : name;
            })
            .Where(static text => !string.IsNullOrWhiteSpace(text))
            .ToList();
}
