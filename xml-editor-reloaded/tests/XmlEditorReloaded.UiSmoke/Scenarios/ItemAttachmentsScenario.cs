using System.Windows.Automation;
using System.Xml.Linq;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// S17: item 12 shows its 27 attachment rows, every <c>attachmentIndex</c> resolves to the attachment
/// name, and validation reports no false duplicate-key issue for the attachment table.
/// </summary>
internal sealed class ItemAttachmentsScenario : SmokeScenario
{
    /// <inheritdoc />
    public override string Id => "S17";

    /// <inheritdoc />
    public override IReadOnlyList<DatasetKind> Datasets => [DatasetKind.V4];

    /// <inheritdoc />
    public override void Run(ScenarioContext context)
    {
        var window = context.Window;
        var data = context.Data(DatasetKind.V4);
        var expected = AttachmentNames(data.TableFile("Items/Items.xml"), data.TableFile("Items/Attachments.xml"), "12");
        var itemLongName = XDocument.Load(data.TableFile("Items/Items.xml"))
            .Descendants("ITEM")
            .First(element => element.Element("uiIndex")?.Value.Trim() == "12")
            .Element("szLongItemName")?.Value ?? string.Empty;

        WorkspaceUi.OpenItemWorkspace(context);
        WorkspaceUi.SelectItem(context, "12", "Commando");

        var childTables = context.Strings.Get("ItemTabChildTables");
        var attachments = context.Strings.Get("ItemSection.Items_Attachments");
        WorkspaceUi.SelectChildSection(window, childTables, attachments);
        var sections = WorkspaceUi.ChildSectionCaptions(window, childTables);
        var expectedCaption = $"{attachments} {context.Strings.Format("ItemChildRowCountFormat", 27)}";
        context.Recorder.Check(
            "Раздел «Вложения» показывает «Строк: 27»",
            sections.Contains(expectedCaption),
            string.Join(" | ", sections));

        var grid = window.WaitById("RowsGrid");
        Ui.WaitUntil(() => grid.RowCount() == 27, "child-грид показывает 27 строк", TimeSpan.FromSeconds(20));
        context.Recorder.Check("В гриде 27 строк вложений", grid.RowCount() == 27, $"строк: {grid.RowCount()}");

        var headers = WorkspaceUi.VisualHeaders(grid);
        var attachmentColumn = headers.ToList().IndexOf("attachmentIndex");
        var itemColumn = headers.ToList().IndexOf("itemIndex");
        context.Recorder.Check(
            "Колонки attachmentIndex и itemIndex видны",
            attachmentColumn >= 0 && itemColumn >= 0,
            string.Join(" | ", headers));

        var actualNames = new List<string>();
        var wrongItems = new List<string>();
        for (var row = 0; row < 27; row++)
        {
            actualNames.Add(WorkspaceUi.ReadCell(grid, row, attachmentColumn));
            var owner = WorkspaceUi.ReadCell(grid, row, itemColumn);
            if (owner != itemLongName)
            {
                wrongItems.Add($"{row}: {owner}");
            }
        }

        context.Recorder.Check(
            "Все строки принадлежат предмету 12",
            wrongItems.Count == 0,
            string.Join(", ", wrongItems));
        context.Recorder.Check(
            "attachmentIndex разрешается в имена вложений",
            actualNames.SequenceEqual(expected),
            $"ожидалось: {string.Join(", ", expected.Take(5))}…; получено: {string.Join(", ", actualNames.Take(5))}…");
        context.Recorder.Check(
            "Ни одно значение вложения не осталось сырым ID",
            actualNames.All(name => name.Length > 0 && !name.All(char.IsDigit)),
            string.Join(", ", actualNames.Where(name => name.Length == 0 || name.All(char.IsDigit))));

        window.WaitById("ItemValidateButton").InvokeButton();
        var summaryPrefix = ScenarioUi.FormatPrefix(context.Strings.Get("EditorValidationSummary"));
        WorkspaceUi.WaitForStatus(
            context,
            "ItemStatus",
            text => text.StartsWith(summaryPrefix, StringComparison.Ordinal),
            "проверка предмета завершена",
            TimeSpan.FromSeconds(90));
        var issues = WorkspaceUi.ListItemTexts(window.WaitById("ItemValidationList"));
        var falseDuplicates = issues
            .Where(text => text.Contains("Items.Attachments", StringComparison.Ordinal)
                && text.Contains("duplicate_key", StringComparison.Ordinal))
            .ToList();
        context.Recorder.Check(
            "Ложных validation.duplicate_key по вложениям нет",
            falseDuplicates.Count == 0,
            falseDuplicates.Count == 0 ? null : string.Join(" | ", falseDuplicates));
        context.Recorder.Note($"Замечаний проверки всего: {issues.Count}.");
    }

    /// <summary>
    /// Returns the resolved attachment names of an item in document order: the long item name that the
    /// relation picker displays for each <c>attachmentIndex</c>.
    /// </summary>
    /// <param name="itemsFile">Path of <c>Items.xml</c>.</param>
    /// <param name="attachmentsFile">Path of <c>Attachments.xml</c>.</param>
    /// <param name="itemIndex">Item identifier.</param>
    /// <returns>Long names of the attachment items in document order.</returns>
    private static List<string> AttachmentNames(string itemsFile, string attachmentsFile, string itemIndex)
    {
        var names = XDocument.Load(itemsFile)
            .Descendants("ITEM")
            .ToDictionary(
                element => element.Element("uiIndex")?.Value.Trim() ?? string.Empty,
                element => element.Element("szLongItemName")?.Value ?? string.Empty,
                StringComparer.Ordinal);
        return XDocument.Load(attachmentsFile)
            .Descendants("ATTACHMENT")
            .Where(element => element.Element("itemIndex")?.Value.Trim() == itemIndex)
            .Select(element => names.GetValueOrDefault(element.Element("attachmentIndex")?.Value.Trim() ?? string.Empty, string.Empty))
            .ToList();
    }
}
