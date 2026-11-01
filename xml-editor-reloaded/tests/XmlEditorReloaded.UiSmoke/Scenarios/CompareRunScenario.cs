using System.IO;
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

        VerifyAsymmetricMissingTables(context, v4, v5);

        context.Recorder.Check(
            "Сравнение не изменило данные v4",
            DataOperationsUi.HashTree(v4.TableDataDirectory) == leftHash);
        context.Recorder.Check(
            "Сравнение не изменило данные v5",
            DataOperationsUi.HashTree(v5.TableDataDirectory) == rightHash);
    }

    /// <summary>
    /// Compares two synthetic directories, one containing a table that the other lacks, so both missing
    /// sides are visible: "Only left" for the left table and "Only right" for the right table.
    /// </summary>
    /// <param name="context">Scenario context.</param>
    /// <param name="v4">Reference data set that supplies the left table file.</param>
    /// <param name="v5">Reference data set that supplies the right table file.</param>
    private static void VerifyAsymmetricMissingTables(
        ScenarioContext context,
        DatasetFixture v4,
        DatasetFixture v5)
    {
        var root = Path.Combine(context.Directory, "asymmetric");
        var left = Path.Combine(root, "left", "TableData");
        var right = Path.Combine(root, "right", "TableData");
        Directory.CreateDirectory(Path.Combine(left, "Lookup"));
        Directory.CreateDirectory(Path.Combine(right, "Email"));
        var leftFile = Path.Combine(left, "Lookup", "Cursor.xml");
        var rightFile = Path.Combine(right, "Email", "EmailSenderNameList.xml");
        File.Copy(v4.TableFile("Lookup/Cursor.xml"), leftFile);
        File.Copy(v5.TableFile("Email/EmailSenderNameList.xml"), rightFile);
        var leftRecords = CountRecords(leftFile);
        var rightRecords = CountRecords(rightFile);

        var window = DataOperationsUi.OpenCompareWindow(context);
        DataOperationsUi.SetCompareDirectories(window, left, right);
        DataOperationsUi.RunCompare(context, window);

        var leftOnly = ScenarioUi.FindTableRow(window, "Lookup.Cursor");
        context.Recorder.Check("Левая таблица есть в списке сравнения", leftOnly is not null);
        if (leftOnly is not null)
        {
            var cells = leftOnly.ReadRowCells();
            context.Recorder.Check(
                "Таблица только слева помечена как «Только слева»",
                cells.ElementAtOrDefault(1) == context.Strings.Get("CompareStatusLeftOnly"),
                string.Join(" | ", cells));
            context.Recorder.Check(
                "Слева показано число записей, справа — прочерк",
                cells.ElementAtOrDefault(2) == leftRecords.ToString()
                    && cells.ElementAtOrDefault(3) == "—",
                string.Join(" | ", cells));
        }

        var rightOnly = ScenarioUi.FindTableRow(window, "Email.EmailSenderNameList");
        context.Recorder.Check("Правая таблица есть в списке сравнения", rightOnly is not null);
        if (rightOnly is not null)
        {
            var cells = rightOnly.ReadRowCells();
            context.Recorder.Check(
                "Таблица только справа помечена как «Только справа»",
                cells.ElementAtOrDefault(1) == context.Strings.Get("CompareStatusRightOnly"),
                string.Join(" | ", cells));
            context.Recorder.Check(
                "Справа показано число записей, слева — прочерк",
                cells.ElementAtOrDefault(2) == "—"
                    && cells.ElementAtOrDefault(3) == rightRecords.ToString(),
                string.Join(" | ", cells));
        }

        // The details of a left-only table show the missing right side and left-only fields.
        leftOnly?.SelectItem();
        Thread.Sleep(500);
        var status = context.Strings.Get("CompareStatusLeftOnly");
        var header = window.FindAll(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .FirstOrDefault(text => text.Contains("Lookup.Cursor", StringComparison.Ordinal)
                && text.Contains(status, StringComparison.Ordinal));
        context.Recorder.Check("Детали показывают статус «Только слева»", header is not null, header);
        var missingRight = context.Strings.Format(
            "CompareProfileMissing",
            context.Strings.Get("CompareSideRight"));
        var profile = window.FindAll(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text))
            .Select(static text => text.ReadName())
            .FirstOrDefault(text => text.Contains(missingRight, StringComparison.Ordinal));
        context.Recorder.Check(
            "Профиль сообщает об отсутствии правой стороны",
            profile is not null,
            missingRight);

        var fields = window.WaitById("FieldsGrid");
        var leftOnlyField = context.Strings.Get("CompareFieldLeftOnly");
        var fieldStatuses = new List<string>();
        for (var index = 0; index < Math.Min(5, fields.RowCount()); index++)
        {
            fieldStatuses.Add(DataOperationsUi.GridCellText(fields, index, 1));
        }

        context.Recorder.Check(
            "Поля таблицы без правой стороны помечены как «Только слева»",
            fieldStatuses.Count > 0 && fieldStatuses.All(item => item == leftOnlyField),
            string.Join(" | ", fieldStatuses));

        DataOperationsUi.CloseCompareWindow(context, window);
    }

    private static int CountRecords(string path) =>
        System.Xml.Linq.XDocument.Load(path).Root?.Elements().Count() ?? 0;
}
