using System.Globalization;
using System.IO;
using System.Text;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Xml.Comparison;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for <see cref="CompareViewModel"/>, its report projection, and report export.
/// </summary>
public class CompareViewModelTests
{
    /// <summary>
    /// Running the comparison fills the table and field projections and the summary.
    /// </summary>
    [Fact]
    public async Task Compare_PopulatesTablesFieldsAndSummary()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var left = CreateTableData(
            workspace,
            "left",
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>A</szItemName></ITEM></ITEMLIST>");
        var right = CreateTableData(
            workspace,
            "right",
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>A</szItemName><NewField>x</NewField></ITEM>"
            + "<ITEM><uiIndex>2</uiIndex><szItemName>B</szItemName></ITEM></ITEMLIST>");
        var viewModel = CreateViewModel();
        viewModel.LeftDirectory = left;
        viewModel.RightDirectory = right;

        await viewModel.CompareCommand.ExecuteAsync(null);

        Assert.True(viewModel.HasReport);
        var table = Assert.Single(viewModel.Tables);
        Assert.Equal("Items.Items", table.TableName);
        Assert.Equal("1", table.LeftRecordsText);
        Assert.Equal("2", table.RightRecordsText);
        Assert.Equal(1, table.AddedFields);
        Assert.True(table.HasDifferences);
        Assert.Contains("common tables", viewModel.SummaryText, StringComparison.Ordinal);
        Assert.Contains("Fields:", viewModel.SummaryFieldsText, StringComparison.Ordinal);
        Assert.NotNull(viewModel.SelectedTable);
        Assert.Contains(viewModel.Fields, field => field.Name == "NewField" && field.HasDifference);
        Assert.Contains("Left:", viewModel.ProfileText, StringComparison.Ordinal);
        Assert.False(viewModel.IsBusy);
    }

    /// <summary>
    /// A directory without <c>TableData</c> is rejected before the comparison starts.
    /// </summary>
    [Fact]
    public async Task Compare_InvalidDirectory_ShowsError()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var right = CreateTableData(
            workspace,
            "right",
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex></ITEM></ITEMLIST>");
        var viewModel = CreateViewModel();
        viewModel.LeftDirectory = workspace.PathOf("nowhere");
        viewModel.RightDirectory = right;

        await viewModel.CompareCommand.ExecuteAsync(null);

        Assert.False(viewModel.HasReport);
        Assert.Equal(
            "The selected folder does not contain TableData or Data-1.13\\TableData.",
            viewModel.StatusText);
    }

    /// <summary>
    /// The export command writes Markdown or CSV depending on the selected extension.
    /// </summary>
    [Fact]
    public async Task Export_WritesMarkdownAndCsv()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var left = CreateTableData(
            workspace,
            "left",
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubWeight>2</ubWeight></ITEM></ITEMLIST>");
        var right = CreateTableData(
            workspace,
            "right",
            "Items/Items.xml",
            "<ITEMLIST><ITEM><uiIndex>1</uiIndex><ubWeight>1.5</ubWeight></ITEM></ITEMLIST>");
        var dialog = new FakeDialogService();
        var viewModel = CreateViewModel(dialog);
        viewModel.LeftDirectory = left;
        viewModel.RightDirectory = right;
        await viewModel.CompareCommand.ExecuteAsync(null);

        var markdownPath = Path.Combine(workspace.DirectoryPath, "report.md");
        dialog.SaveFileResult = markdownPath;
        await viewModel.ExportCommand.ExecuteAsync(null);

        Assert.True(File.Exists(markdownPath));
        var markdown = await File.ReadAllTextAsync(markdownPath);
        Assert.StartsWith("# TableData comparison report", markdown, StringComparison.Ordinal);
        Assert.Contains("Items.Items", markdown, StringComparison.Ordinal);
        Assert.Contains(markdownPath, viewModel.StatusText, StringComparison.Ordinal);

        var csvPath = Path.Combine(workspace.DirectoryPath, "report.csv");
        dialog.SaveFileResult = csvPath;
        await viewModel.ExportCommand.ExecuteAsync(null);

        Assert.True(File.Exists(csvPath));
        var csv = await File.ReadAllTextAsync(csvPath);
        Assert.StartsWith("RecordType,Table,Name,Status", csv, StringComparison.Ordinal);
        Assert.Contains("Field,Items.Items,ubWeight,TypeChanged,Integer,Decimal", csv, StringComparison.Ordinal);
    }

    /// <summary>
    /// Export is unavailable until a report exists.
    /// </summary>
    [Fact]
    public void Export_WithoutReport_IsDisabled()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var viewModel = CreateViewModel();

        Assert.False(viewModel.HasReport);
        Assert.False(viewModel.ExportCommand.CanExecute(null));
    }

    /// <summary>
    /// The initial directory is applied only when no baseline has been chosen yet.
    /// </summary>
    [Fact]
    public void SetInitialDirectory_DoesNotOverwriteExistingBaseline()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var viewModel = CreateViewModel();

        viewModel.SetInitialDirectory("first");
        Assert.Equal("first", viewModel.LeftDirectory);

        viewModel.SetInitialDirectory("second");
        Assert.Equal("first", viewModel.LeftDirectory);
    }

    private static CompareViewModel CreateViewModel(FakeDialogService? dialog = null) =>
        new(
            new TableDataComparer(new FileSystemTableCatalog()),
            dialog ?? new FakeDialogService());

    private static string CreateTableData(
        TestWorkspace workspace,
        string dataDirectoryName,
        string relativePath,
        string content)
    {
        var tableData = Path.Combine(workspace.DirectoryPath, dataDirectoryName, "TableData");
        var path = Path.Combine(tableData, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        return tableData;
    }

    private sealed class FakeDialogService : IDialogService
    {
        public string? SaveFileResult { get; set; }

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public string? PickSaveFile(string title, string filter, string defaultFileName) => SaveFileResult;

        public bool Confirm(string message, string title) => true;

        public UnsavedChangesChoice ConfirmUnsavedChanges(string message, string title) =>
            UnsavedChangesChoice.Cancel;

        public ExternalChangeChoice ResolveExternalChange(string message, string title) =>
            ExternalChangeChoice.Cancel;

        public ulong? EditBits(
            string title,
            ulong value,
            IReadOnlyDictionary<int, string>? labels = null,
            string? valueName = null) => null;

        public void ShowTranslationReport(TranslationReport report, string title)
        {
        }

        public BulkEditRequest? EditBulk(BulkEditContext context) => null;

        public ItemCreationRequest? CreateItem(
            string title,
            string suggestedId,
            string suggestedName,
            IReadOnlyList<LookupOption> classes) => null;
    }
}
