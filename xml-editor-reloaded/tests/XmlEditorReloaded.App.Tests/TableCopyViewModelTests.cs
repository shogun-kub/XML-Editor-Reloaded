using System.Globalization;
using System.IO;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Copy;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the cross-data-set copy dialog view model: table discovery, preview, invalidation, and
/// apply.
/// </summary>
public class TableCopyViewModelTests
{
    /// <summary>
    /// Initialization normalizes the source, picks another directory as target, lists the tables common
    /// to both data sets, and pre-fills the table and record keys.
    /// </summary>
    [Fact]
    public async Task Initialize_LoadsCommonTablesAndInitialState()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var source = CreateTableData(workspace, "v4");
        var target = CreateTableData(workspace, "v5");
        var catalog = new FixedCatalog(new Dictionary<string, IReadOnlyList<string>>
        {
            [source] = new[] { "Items.Items", "Items.Weapons", "Items.SourceOnly" },
            [target] = new[] { "Items.Items", "Items.Weapons", "Items.TargetOnly" },
        });
        var viewModel = new TableCopyViewModel(
            new FakeCopyService(),
            catalog,
            new FakeDialogService(),
            new TableCopyDialogRequest(
                new[] { source, target },
                source,
                "Items.Items",
                new[] { "5" }));

        await viewModel.InitializeAsync();

        Assert.Equal(source, viewModel.SourceDirectory);
        Assert.Equal(target, viewModel.TargetDirectory);
        Assert.Equal(
            new[] { "Items.Items", "Items.Weapons" },
            viewModel.Tables.Select(static item => item.TableName.Value).ToArray());
        Assert.Equal("Items.Items", viewModel.SelectedTable!.TableName.Value);
        Assert.Equal("5", viewModel.RecordKeysText);
        Assert.True(viewModel.PreviewCommand.CanExecute(null));
        Assert.False(viewModel.ApplyCommand.CanExecute(null));
    }

    /// <summary>
    /// A preview fills the grid and summary, changing an option invalidates it, and applying returns
    /// the committed result with the selected policies.
    /// </summary>
    [Fact]
    public async Task PreviewAndApply_UseSelectedOptionsAndInvalidateOnChange()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var source = CreateTableData(workspace, "v4");
        var target = CreateTableData(workspace, "v5");
        var itemTable = TableName.Parse("Items.Items");
        var service = new FakeCopyService
        {
            PreviewResult = OperationResult<TableCopyPreview>.Success(new TableCopyPreview(
                itemTable,
                source,
                target,
                new[]
                {
                    new TableCopyRecordPreview(
                        "5",
                        "Glock 17",
                        "12",
                        TableCopyAction.Add,
                        new[]
                        {
                            new TableCopyRemap(
                                "ubLinkedItem",
                                "6",
                                "11",
                                "Clip",
                                TableCopyRemapStatus.Remapped),
                        },
                        new[]
                        {
                            new TableCopyRelatedPreview(
                                TableName.Parse("Items.Attachments"),
                                TableRelationKind.ChildRows,
                                2,
                                2,
                                0,
                                0,
                                0,
                                Array.Empty<string>()),
                        },
                        Array.Empty<string>()),
                    new TableCopyRecordPreview(
                        "6",
                        "Clip",
                        "6",
                        TableCopyAction.Skip,
                        Array.Empty<TableCopyRemap>(),
                        Array.Empty<TableCopyRelatedPreview>(),
                        Array.Empty<string>()),
                },
                new[] { "A source warning." })),
            ApplyResult = OperationResult<TableCopyResult>.Success(new TableCopyResult(
                itemTable,
                new[] { new TableCopyAppliedTable(itemTable, 1, 0, 1) },
                new[] { Path.Combine(target, "Items", "Items.xml") },
                Array.Empty<string>())),
        };
        var viewModel = new TableCopyViewModel(
            service,
            new FixedCatalog(new Dictionary<string, IReadOnlyList<string>>
            {
                [source] = new[] { "Items.Items" },
                [target] = new[] { "Items.Items" },
            }),
            new FakeDialogService(),
            new TableCopyDialogRequest(new[] { source, target }, source, "Items.Items"))
        {
            RecordKeysText = "5, 6",
            CopyRelatedRecords = true,
        };
        await viewModel.InitializeAsync();

        await viewModel.PreviewCommand.ExecuteAsync(null);

        Assert.True(viewModel.HasPreview);
        Assert.Equal(2, viewModel.PreviewRows.Count);
        Assert.Contains("Add: 1", viewModel.SummaryText, StringComparison.Ordinal);
        Assert.Contains("related rows: 2", viewModel.SummaryText, StringComparison.Ordinal);
        Assert.True(viewModel.HasWarnings);
        Assert.Single(viewModel.Warnings);
        Assert.Contains("→", viewModel.PreviewRows[0].RemapText, StringComparison.Ordinal);
        Assert.True(viewModel.ApplyCommand.CanExecute(null));
        Assert.Equal(new[] { "5", "6" }, service.LastPreviewRequest!.RecordKeys);
        Assert.True(service.LastPreviewRequest.CopyRelatedRecords);

        viewModel.SelectedConflictPolicy = viewModel.ConflictPolicies[1];
        Assert.False(viewModel.HasPreview);
        Assert.False(viewModel.ApplyCommand.CanExecute(null));

        await viewModel.PreviewCommand.ExecuteAsync(null);
        viewModel.SelectedKeyMode = viewModel.KeyModes[1];
        await viewModel.PreviewCommand.ExecuteAsync(null);

        var completed = false;
        viewModel.Completed += (_, _) => completed = true;
        await viewModel.ApplyCommand.ExecuteAsync(null);

        Assert.True(completed);
        Assert.NotNull(viewModel.Outcome);
        Assert.Equal(target, viewModel.Outcome!.TargetTableDataDirectory);
        Assert.Equal(TableCopyKeyMode.Allocate, service.LastApplyRequest!.KeyMode);
        Assert.Equal(TableCopyConflictPolicy.Overwrite, service.LastApplyRequest.ConflictPolicy);
    }

    /// <summary>
    /// A failed preview reports the error and keeps the copy action disabled.
    /// </summary>
    [Fact]
    public async Task Preview_Failure_DisablesApply()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var source = CreateTableData(workspace, "v4");
        var target = CreateTableData(workspace, "v5");
        var service = new FakeCopyService
        {
            PreviewResult = OperationResult<TableCopyPreview>.Failure(
                ErrorCodes.CopySameDirectory,
                "The source and target data sets are the same directory."),
        };
        var viewModel = new TableCopyViewModel(
            service,
            new FixedCatalog(new Dictionary<string, IReadOnlyList<string>>
            {
                [source] = new[] { "Items.Items" },
                [target] = new[] { "Items.Items" },
            }),
            new FakeDialogService(),
            new TableCopyDialogRequest(new[] { source, target }, source, "Items.Items"));
        await viewModel.InitializeAsync();

        await viewModel.PreviewCommand.ExecuteAsync(null);

        Assert.False(viewModel.HasPreview);
        Assert.False(viewModel.ApplyCommand.CanExecute(null));
        Assert.Contains("same directory", viewModel.StatusText, StringComparison.Ordinal);
    }

    /// <summary>
    /// Browsing for a target resolves a game directory to its <c>TableData</c> directory.
    /// </summary>
    [Fact]
    public async Task BrowseTarget_ResolvesTableDataDirectory()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        using var workspace = new TestWorkspace();
        var source = CreateTableData(workspace, "v4");
        var targetGame = workspace.PathOf("v5");
        var target = CreateTableData(workspace, "v5");
        var dialog = new FakeDialogService { FolderResult = targetGame };
        var viewModel = new TableCopyViewModel(
            new FakeCopyService(),
            new FixedCatalog(new Dictionary<string, IReadOnlyList<string>>
            {
                [source] = new[] { "Items.Items" },
                [target] = new[] { "Items.Items" },
            }),
            dialog,
            new TableCopyDialogRequest(new[] { source }, source, "Items.Items"));
        await viewModel.InitializeAsync();

        viewModel.BrowseTargetCommand.Execute(null);

        Assert.Equal(target, viewModel.TargetDirectory);
        Assert.Contains(target, viewModel.Directories);
        Assert.Single(viewModel.Tables);
    }

    private static string CreateTableData(TestWorkspace workspace, string gameFolder)
    {
        var tableData = workspace.PathOf(Path.Combine(gameFolder, "Data-1.13", "TableData"));
        Directory.CreateDirectory(tableData);
        return tableData;
    }

    private sealed class FixedCatalog : ITableCatalog
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> _tables;

        public FixedCatalog(IReadOnlyDictionary<string, IReadOnlyList<string>> tables)
        {
            _tables = tables;
        }

        public Task<OperationResult<IReadOnlyList<TableDescriptor>>> GetTablesAsync(
            string tableDataDirectory,
            CancellationToken cancellationToken = default)
        {
            var names = _tables.GetValueOrDefault(tableDataDirectory) ?? Array.Empty<string>();
            IReadOnlyList<TableDescriptor> descriptors = names
                .Select(name => new TableDescriptor(
                    TableName.Parse(name),
                    Path.Combine(tableDataDirectory, TableName.Parse(name).ToRelativePath()),
                    TableName.Parse(name).ToRelativePath(),
                    IsMapped: true,
                    Array.Empty<OverlayDescriptor>()))
                .ToArray();
            return Task.FromResult(OperationResult<IReadOnlyList<TableDescriptor>>.Success(descriptors));
        }
    }

    private sealed class FakeCopyService : ITableCopyService
    {
        public TableCopyRequest? LastPreviewRequest { get; private set; }

        public TableCopyRequest? LastApplyRequest { get; private set; }

        public OperationResult<TableCopyPreview>? PreviewResult { get; set; }

        public OperationResult<TableCopyResult>? ApplyResult { get; set; }

        public Task<OperationResult<TableCopyPreview>> PreviewAsync(
            TableCopyRequest request,
            CancellationToken cancellationToken = default)
        {
            LastPreviewRequest = request;
            return Task.FromResult(
                PreviewResult
                ?? throw new InvalidOperationException("No preview result was configured."));
        }

        public Task<OperationResult<TableCopyResult>> ApplyAsync(
            TableCopyRequest request,
            CancellationToken cancellationToken = default)
        {
            LastApplyRequest = request;
            return Task.FromResult(
                ApplyResult
                ?? throw new InvalidOperationException("No apply result was configured."));
        }
    }

    private sealed class FakeDialogService : IDialogService
    {
        public string? FolderResult { get; set; }

        public string? PickFolder(string title, string? initialDirectory = null) => FolderResult;

        public bool Confirm(string message, string title) => true;

        public uint? EditBits(
            string title,
            uint value,
            IReadOnlyDictionary<int, string>? labels = null,
            string? valueName = null) => null;

        public void ShowTranslationReport(TranslationReport report, string title)
        {
        }

        public string? PickSaveFile(string title, string filter, string defaultFileName) => null;

        public BulkEditRequest? EditBulk(BulkEditContext context) => null;

        public ItemCreationRequest? CreateItem(
            string title,
            string suggestedId,
            string suggestedName,
            IReadOnlyList<LookupOption> classes) => null;
    }
}
