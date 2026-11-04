using System.Globalization;
using System.IO;
using System.Text;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Repository;
using XmlEditorReloaded.Xml.Search;
using XmlEditorReloaded.Xml.Validation;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for <see cref="DatasetViewModel"/>: the data-set-wide validation report, the severity filter,
/// the global search, and navigation from a report row.
/// </summary>
public class DatasetViewModelTests
{
    /// <summary>
    /// Initializing the validation tab validates every table and fills the issue list and summary.
    /// </summary>
    [Fact]
    public async Task Initialize_Validation_ReportsEveryTable()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var test = await CreateSessionAsync(
            workspace,
            ("Items/Items.xml", "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Glock</szItemName>"
                + "<ubWeight>abc</ubWeight></ITEM></ITEMLIST>"),
            ("Custom/Notes.xml", "<NOTES><NOTE><id>1</id><text>Hello</text></NOTE></NOTES>"));
        var dialog = new FakeDialogService();
        var viewModel = new DatasetViewModel(
            test.Session,
            new DatasetValidator(test.Provider),
            new DatasetSearchService(test.Provider),
            dialog,
            DatasetWindowTab.Validation);

        await viewModel.InitializeAsync();

        Assert.Contains(viewModel.Issues, issue =>
            issue.TableName == "Items.Items" && issue.Code == ValidationCodes.InvalidType);
        Assert.Contains(viewModel.Issues, issue => issue.Code == ValidationCodes.Unmapped);
        Assert.Contains("Tables:", viewModel.ValidationSummaryText, StringComparison.Ordinal);
        Assert.False(viewModel.IsValidationEmpty);
        Assert.False(viewModel.IsBusy);
        Assert.NotNull(viewModel.SelectedIssue);
        Assert.True(viewModel.ExportCommand.CanExecute(null));
    }

    /// <summary>
    /// The severity filter narrows the list without losing the exact report counts.
    /// </summary>
    [Fact]
    public async Task SeverityFilter_NarrowsIssues()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var test = await CreateSessionAsync(
            workspace,
            ("Items/Items.xml", "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Glock</szItemName>"
                + "<ubWeight>abc</ubWeight></ITEM></ITEMLIST>"),
            ("Custom/Notes.xml", "<NOTES><NOTE><id>1</id><text>Hello</text></NOTE></NOTES>"));
        var viewModel = new DatasetViewModel(
            test.Session,
            new DatasetValidator(test.Provider),
            new DatasetSearchService(test.Provider),
            new FakeDialogService());

        await viewModel.InitializeAsync();
        var all = viewModel.Issues.Count;

        viewModel.SelectedSeverity = viewModel.SeverityOptions
            .First(option => option.Severity == ValidationSeverity.Info);

        var info = Assert.Single(viewModel.Issues);
        Assert.Equal(ValidationSeverity.Info, info.Severity);
        Assert.True(all > 1);

        viewModel.SelectedSeverity = viewModel.SeverityOptions
            .First(option => option.Severity == ValidationSeverity.Error);

        Assert.Empty(viewModel.Issues);
        Assert.True(viewModel.IsValidationEmpty);
    }

    /// <summary>
    /// Opening an issue raises navigation with the table and the affected record.
    /// </summary>
    [Fact]
    public async Task OpenIssue_RaisesNavigationWithKey()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var test = await CreateSessionAsync(
            workspace,
            ("Items/Items.xml", "<ITEMLIST><ITEM><uiIndex>7</uiIndex><szItemName>Glock</szItemName>"
                + "<ubWeight>abc</ubWeight></ITEM></ITEMLIST>"));
        var viewModel = new DatasetViewModel(
            test.Session,
            new DatasetValidator(test.Provider),
            new DatasetSearchService(test.Provider),
            new FakeDialogService());
        DatasetNavigationRequest? request = null;
        viewModel.NavigationRequested += (_, value) => request = value;

        await viewModel.InitializeAsync();
        viewModel.SelectedIssue = viewModel.Issues.First(issue => issue.Code == ValidationCodes.InvalidType);
        viewModel.OpenIssueCommand.Execute(null);

        Assert.NotNull(request);
        Assert.Equal("Items.Items", request.TableName.Value);
        Assert.Equal("7", request.RecordKey);
    }

    /// <summary>
    /// Searching fills the hit list with table, record, field, and value, and stays empty until a term
    /// is entered.
    /// </summary>
    [Fact]
    public async Task Search_FindsValuesAndReportsCounts()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var test = await CreateSessionAsync(
            workspace,
            ("Items/Items.xml", "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Glock 17</szItemName></ITEM></ITEMLIST>"),
            ("Custom/Notes.xml", "<NOTES><NOTE><id>1</id><text>Hello</text></NOTE></NOTES>"));
        var viewModel = new DatasetViewModel(
            test.Session,
            new DatasetValidator(test.Provider),
            new DatasetSearchService(test.Provider),
            new FakeDialogService(),
            DatasetWindowTab.Search);

        await viewModel.InitializeAsync();
        Assert.False(viewModel.SearchCommand.CanExecute(null));

        viewModel.SearchTerm = "glock";
        Assert.True(viewModel.SearchCommand.CanExecute(null));
        await viewModel.SearchCommand.ExecuteAsync(null);

        var hit = Assert.Single(viewModel.Hits);
        Assert.Equal("Items.Items", hit.TableName);
        Assert.Equal("1", hit.RecordKey);
        Assert.Equal("szItemName", hit.FieldPath);
        Assert.Equal("Glock 17", hit.Value);
        Assert.Equal("Base", hit.LanguageLabel);
        Assert.Contains("Matches: 1", viewModel.SearchSummaryText, StringComparison.Ordinal);
        Assert.False(viewModel.IsSearchEmpty);
        Assert.NotNull(viewModel.SelectedHit);

        viewModel.SearchTerm = "no such text";
        await viewModel.SearchCommand.ExecuteAsync(null);

        Assert.Empty(viewModel.Hits);
        Assert.True(viewModel.IsSearchEmpty);
    }

    /// <summary>
    /// The export command writes the validation report as Markdown or CSV.
    /// </summary>
    [Fact]
    public async Task Export_WritesMarkdownAndCsv()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var test = await CreateSessionAsync(
            workspace,
            ("Items/Items.xml", "<ITEMLIST><ITEM><uiIndex>1</uiIndex><szItemName>Glock</szItemName>"
                + "<ubWeight>abc</ubWeight></ITEM></ITEMLIST>"));
        var dialog = new FakeDialogService();
        var viewModel = new DatasetViewModel(
            test.Session,
            new DatasetValidator(test.Provider),
            new DatasetSearchService(test.Provider),
            dialog);

        await viewModel.InitializeAsync();

        var markdownPath = Path.Combine(workspace.DirectoryPath, "validation.md");
        dialog.SaveFileResult = markdownPath;
        await viewModel.ExportCommand.ExecuteAsync(null);

        Assert.True(File.Exists(markdownPath));
        var markdown = await File.ReadAllTextAsync(markdownPath);
        Assert.StartsWith("# TableData validation report", markdown, StringComparison.Ordinal);
        Assert.Contains("Items.Items", markdown, StringComparison.Ordinal);

        var csvPath = Path.Combine(workspace.DirectoryPath, "validation.csv");
        dialog.SaveFileResult = csvPath;
        await viewModel.ExportCommand.ExecuteAsync(null);

        Assert.True(File.Exists(csvPath));
        var csv = await File.ReadAllTextAsync(csvPath);
        Assert.StartsWith("Table,State,Mapped,Records,Errors,Warnings,Info", csv, StringComparison.Ordinal);
    }

    /// <summary>
    /// The window opens on the requested tab.
    /// </summary>
    [Fact]
    public void InitialTabIndex_FollowsConstructorArgument()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var session = new DataSetSession(
            workspace.DirectoryPath,
            new StubMetadataProvider(),
            new FileSystemTableCatalog(),
            new TableRepository());

        var validation = new DatasetViewModel(
            session,
            new DatasetValidator(),
            new DatasetSearchService(),
            new FakeDialogService(),
            DatasetWindowTab.Validation);
        var search = new DatasetViewModel(
            session,
            new DatasetValidator(),
            new DatasetSearchService(),
            new FakeDialogService(),
            DatasetWindowTab.Search);

        Assert.Equal(0, validation.InitialTabIndex);
        Assert.Equal(1, search.InitialTabIndex);
    }

    private static async Task<TestSession> CreateSessionAsync(
        TestWorkspace workspace,
        params (string RelativePath, string Content)[] tables)
    {
        var tableData = Path.Combine(workspace.DirectoryPath, "Data-1.13", "TableData");
        foreach (var table in tables)
        {
            var path = Path.Combine(
                tableData,
                table.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(
                path,
                table.Content,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        var databasePath = workspace.PathOf("metadata.sqlite");
        var migration = await new DatabaseMigrator().MigrateAsync(databasePath);
        Assert.True(migration.IsSuccess, migration.ErrorText);
        var provider = new SqliteMetadataProvider(databasePath);
        return new TestSession(
            new DataSetSession(
                tableData,
                provider,
                new FileSystemTableCatalog(
                    new[] { new LanguageDescriptor("ru", "Russian", "Russian", 1) },
                    provider),
                new TableRepository(provider)),
            provider);
    }

    private sealed record TestSession(DataSetSession Session, SqliteMetadataProvider Provider);

    private sealed class StubMetadataProvider : XmlEditorReloaded.Core.Abstractions.ITableMetadataProvider
    {
        public Task<XmlEditorReloaded.Core.Api.OperationResult<TableMetadata>> GetTableAsync(
            TableName tableName,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(XmlEditorReloaded.Core.Api.OperationResult<TableMetadata>.Failure(
                XmlEditorReloaded.Core.Api.ErrorCodes.MetadataNotFound,
                "No metadata."));

        public Task<XmlEditorReloaded.Core.Api.OperationResult<IReadOnlyList<TableMetadata>>> GetAllTablesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(XmlEditorReloaded.Core.Api.OperationResult<IReadOnlyList<TableMetadata>>.Success(
                Array.Empty<TableMetadata>()));

        public Task<XmlEditorReloaded.Core.Api.OperationResult<IReadOnlyList<LanguageDescriptor>>> GetLanguagesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(XmlEditorReloaded.Core.Api.OperationResult<IReadOnlyList<LanguageDescriptor>>.Success(
                Array.Empty<LanguageDescriptor>()));

        public Task<XmlEditorReloaded.Core.Api.OperationResult<IReadOnlyList<TableRelation>>> GetRelationsAsync(
            TableName sourceTable,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(XmlEditorReloaded.Core.Api.OperationResult<IReadOnlyList<TableRelation>>.Success(
                Array.Empty<TableRelation>()));

        public Task<XmlEditorReloaded.Core.Api.OperationResult<IReadOnlyList<LookupEntry>>> GetLookupValuesAsync(
            TableName lookupName,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(XmlEditorReloaded.Core.Api.OperationResult<IReadOnlyList<LookupEntry>>.Success(
                Array.Empty<LookupEntry>()));
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

        public void ShowTranslationReport(XmlEditorReloaded.Core.Localization.TranslationReport report, string title)
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
