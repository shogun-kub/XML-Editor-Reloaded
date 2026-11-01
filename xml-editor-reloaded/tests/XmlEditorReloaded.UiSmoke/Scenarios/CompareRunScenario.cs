using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S40: comparing the two data sets fills the summary and the table list, shows the malformed v5
/// lookup as damaged instead of failing, and never writes into either data set.
/// </summary>
internal sealed class CompareRunScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S40";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4, DatasetKind.V5];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var v4 = context.Data(DatasetKind.V4);
        var v5 = context.Data(DatasetKind.V5);
        var leftHash = DataOperationsUi.HashTree(v4.TableDataDirectory);
        var rightHash = DataOperationsUi.HashTree(v5.TableDataDirectory);

        var window = DataOperationsUi.OpenCompareWindow(context);
        DataOperationsUi.SetCompareDirectories(window, v4, v5);
        var summary = DataOperationsUi.RunCompare(context, window);
        var summaryPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("CompareSummaryText"));
        context.Recorder.Check("Сводка сравнения заполнена", summary.StartsWith(summaryPrefix, StringComparison.Ordinal), summary);

        var fields = window.WaitById("CompareSummaryFieldsText").ReadName();
        var fieldsPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("CompareSummaryFieldsText"));
        context.Recorder.Check(
            "Сводка полей заполнена",
            fields.StartsWith(fieldsPrefix, StringComparison.Ordinal),
            fields);

        var grid = window.WaitById("TablesGrid");
        context.Recorder.Check("Список таблиц сравнения непустой", grid.RowCount() > 30, $"строк: {grid.RowCount()}");

        var items = ScenarioUi.FindTableRow(window, "Items.Items");
        context.Recorder.Check("Items.Items есть в списке сравнения", items is not null);
        if (items is not null)
        {
            var cells = items.ReadRowCells();
            context.Recorder.Check(
                "Items.Items присутствует в обеих версиях",
                cells.ElementAtOrDefault(1) == context.Strings.Get("CompareStatusBoth"),
                string.Join(" | ", cells));
            context.Recorder.Check(
                "Число записей слева и справа различается",
                cells.ElementAtOrDefault(2) == "1776" && cells.ElementAtOrDefault(3) == "1775",
                string.Join(" | ", cells));
        }

        var malformed = ScenarioUi.FindTableRow(window, "Lookup.AttachmentPoint");
        context.Recorder.Check("Lookup.AttachmentPoint есть в списке сравнения", malformed is not null);
        if (malformed is not null)
        {
            var cells = malformed.ReadRowCells();
            context.Recorder.Check(
                "Повреждённый v5-файл показан как «Повреждена»",
                cells.ElementAtOrDefault(1) == context.Strings.Get("CompareStatusMalformed"),
                string.Join(" | ", cells));
        }

        // A table that exists only in v5 has no left side; the record count comes from the file itself.
        var rightOnlyFile = v5.TableFile("Email/EmailSenderNameList.xml");
        var rightOnlyRecords = System.Xml.Linq.XDocument.Load(rightOnlyFile).Root?.Elements().Count() ?? 0;
        var rightOnly = ScenarioUi.FindTableRow(window, "Email.EmailSenderNameList");
        context.Recorder.Check(
            "Таблица, которой нет в v4, есть в списке сравнения",
            rightOnly is not null);
        if (rightOnly is not null)
        {
            var cells = rightOnly.ReadRowCells();
            context.Recorder.Check(
                "Статус таблицы — «Только справа»",
                cells.ElementAtOrDefault(1) == context.Strings.Get("CompareStatusRightOnly"),
                string.Join(" | ", cells));
            context.Recorder.Check(
                "Слева записей нет",
                cells.ElementAtOrDefault(2) == "—",
                string.Join(" | ", cells));
            context.Recorder.Check(
                "Справа показано число записей файла",
                cells.ElementAtOrDefault(3) == rightOnlyRecords.ToString(
                    System.Globalization.CultureInfo.CurrentCulture),
                $"{cells.ElementAtOrDefault(3)} против {rightOnlyRecords}");
        }

        // Selecting a table fills the read-only details without any save control.
        items?.SelectItem();
        Thread.Sleep(500);
        var detailsHeader = window.FindAll(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .FirstOrDefault(text => text.Contains("Items.Items", StringComparison.Ordinal)
                && text.Contains(context.Strings.Get("CompareStatusBoth"), StringComparison.Ordinal));
        context.Recorder.Check("Детали показывают выбранную таблицу и её статус", detailsHeader is not null, detailsHeader);
        context.Recorder.Check(
            "Окно сравнения не содержит кнопок сохранения",
            window.FindById("SaveButton") is null && window.FindById("ItemSaveButton") is null);

        DataOperationsUi.CloseCompareWindow(context, window);

        context.Recorder.Check(
            "Сравнение не изменило данные v4",
            DataOperationsUi.HashTree(v4.TableDataDirectory) == leftHash);
        context.Recorder.Check(
            "Сравнение не изменило данные v5",
            DataOperationsUi.HashTree(v5.TableDataDirectory) == rightHash);
    }
}
