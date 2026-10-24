using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Filtering;
using XmlEditorReloaded.Core.Layout;
using XmlEditorReloaded.Data.Settings;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Tests for <see cref="JsonLayoutStore"/>.
/// </summary>
public class LayoutStoreTests
{
    private const string DataSet = @"E:\Games\JA2\Data-1.13\TableData";

    /// <summary>
    /// A missing layouts file yields the empty layout.
    /// </summary>
    [Fact]
    public async Task Load_MissingFile_ReturnsEmptyLayout()
    {
        using var workspace = new TestWorkspace();
        var store = new JsonLayoutStore(workspace.PathOf("layouts.json"));

        var result = await store.LoadAsync(DataSet, "Items.Items");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.True(result.Data!.IsEmpty);
    }

    /// <summary>
    /// Columns, sorting, and the structured filter round-trip unchanged.
    /// </summary>
    [Fact]
    public async Task SaveAndLoad_RoundTrip()
    {
        using var workspace = new TestWorkspace();
        var store = new JsonLayoutStore(workspace.PathOf("nested/layouts.json"));
        var layout = CreateLayout();

        var save = await store.SaveAsync(DataSet, "Items.Items", layout);
        var load = await store.LoadAsync(DataSet, "Items.Items");

        Assert.True(save.IsSuccess, save.ErrorText);
        Assert.True(load.IsSuccess, load.ErrorText);
        var actual = load.Data!;
        Assert.Equal(3, actual.Columns.Count);
        Assert.Equal("szItemName", actual.Columns[1].Name);
        Assert.True(actual.Columns[1].IsVisible);
        Assert.Equal(240, actual.Columns[1].Width);
        Assert.Equal(2, actual.Columns[1].DisplayOrder);
        Assert.Equal("uiIndex", actual.SortField);
        Assert.Equal(TableSortDirection.Descending, actual.SortDirection);
        Assert.Equal(RowFilterLogicalOperator.Or, actual.Filter.Operator);
        Assert.Collection(
            actual.Filter.Criteria,
            criterion =>
            {
                Assert.Equal("ubWeight", criterion.Field);
                Assert.Equal(RowFilterOperator.GreaterThan, criterion.Operator);
                Assert.Equal("5", criterion.Operand);
            },
            criterion =>
            {
                Assert.Equal("szItemName", criterion.Field);
                Assert.Equal(RowFilterOperator.Contains, criterion.Operator);
                Assert.Equal("Glock", criterion.Operand);
            });
    }

    /// <summary>
    /// Layouts of different tables and data sets stay independent.
    /// </summary>
    [Fact]
    public async Task Save_KeepsOtherKeysSeparate()
    {
        using var workspace = new TestWorkspace();
        var store = new JsonLayoutStore(workspace.PathOf("layouts.json"));
        var items = CreateLayout();
        var other = new TableLayout(
            Array.Empty<ColumnLayout>(),
            "uiIndex",
            TableSortDirection.Ascending,
            RowFilterGroup.Empty);

        Assert.True((await store.SaveAsync(DataSet, "Items.Items", items)).IsSuccess);
        Assert.True((await store.SaveAsync(DataSet, "Weapons.Weapons", other)).IsSuccess);
        Assert.True((await store.SaveAsync(@"E:\Games\JA2-v5\Data-1.13\TableData", "Items.Items", TableLayout.Empty)).IsSuccess);

        var itemsLoad = await store.LoadAsync(DataSet, "Items.Items");
        var weaponsLoad = await store.LoadAsync(DataSet, "Weapons.Weapons");
        var otherDataSetLoad = await store.LoadAsync(@"E:\Games\JA2-v5\Data-1.13\TableData", "Items.Items");

        Assert.Equal("uiIndex", itemsLoad.Data!.SortField);
        Assert.Equal(TableSortDirection.Ascending, weaponsLoad.Data!.SortDirection);
        Assert.True(otherDataSetLoad.Data!.IsEmpty);
        Assert.Empty(otherDataSetLoad.Data.Columns);
    }

    /// <summary>
    /// Removing one layout keeps the rest of the file.
    /// </summary>
    [Fact]
    public async Task Remove_DeletesOnlyRequestedLayout()
    {
        using var workspace = new TestWorkspace();
        var store = new JsonLayoutStore(workspace.PathOf("layouts.json"));
        Assert.True((await store.SaveAsync(DataSet, "Items.Items", CreateLayout())).IsSuccess);
        Assert.True((await store.SaveAsync(
            DataSet,
            "Weapons.Weapons",
            new TableLayout(Array.Empty<ColumnLayout>(), "uiIndex", TableSortDirection.Ascending, RowFilterGroup.Empty))).IsSuccess);

        var remove = await store.RemoveAsync(DataSet, "Items.Items");

        Assert.True(remove.IsSuccess, remove.ErrorText);
        Assert.True((await store.LoadAsync(DataSet, "Items.Items")).Data!.IsEmpty);
        Assert.False((await store.LoadAsync(DataSet, "Weapons.Weapons")).Data!.IsEmpty);
    }

    /// <summary>
    /// Resetting removes every layout.
    /// </summary>
    [Fact]
    public async Task ResetAll_RemovesFile()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("layouts.json");
        var store = new JsonLayoutStore(path);
        Assert.True((await store.SaveAsync(DataSet, "Items.Items", CreateLayout())).IsSuccess);

        var reset = await store.ResetAllAsync();

        Assert.True(reset.IsSuccess, reset.ErrorText);
        Assert.False(File.Exists(path));
        Assert.True((await store.LoadAsync(DataSet, "Items.Items")).Data!.IsEmpty);
    }

    /// <summary>
    /// Invalid JSON is reported and a later save keeps a copy before writing fresh layouts.
    /// </summary>
    [Fact]
    public async Task InvalidJson_IsReportedAndBackedUpOnSave()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("layouts.json");
        await File.WriteAllTextAsync(path, "{ invalid");
        var store = new JsonLayoutStore(path);

        var load = await store.LoadAsync(DataSet, "Items.Items");
        var save = await store.SaveAsync(DataSet, "Items.Items", CreateLayout());

        Assert.False(load.IsSuccess);
        Assert.Equal(ErrorCodes.IoError, load.ErrorCode);
        Assert.True(save.IsSuccess, save.ErrorText);
        Assert.True(File.Exists(path + ".bad"));
        Assert.False((await store.LoadAsync(DataSet, "Items.Items")).Data!.IsEmpty);
    }

    /// <summary>
    /// A newer schema version is reported and never overwritten.
    /// </summary>
    [Fact]
    public async Task NewerSchemaVersion_IsNotOverwritten()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("layouts.json");
        const string Json = "{\"schemaVersion\":99,\"layouts\":[]}";
        await File.WriteAllTextAsync(path, Json);
        var store = new JsonLayoutStore(path);

        var load = await store.LoadAsync(DataSet, "Items.Items");
        var save = await store.SaveAsync(DataSet, "Items.Items", CreateLayout());

        Assert.False(load.IsSuccess);
        Assert.Equal(ErrorCodes.IoError, load.ErrorCode);
        Assert.False(save.IsSuccess);
        Assert.Equal(Json, await File.ReadAllTextAsync(path));
    }

    /// <summary>
    /// The layouts file is written as UTF-8 without a byte-order mark.
    /// </summary>
    [Fact]
    public async Task Save_WritesUtf8WithoutBom()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.PathOf("layouts.json");
        var store = new JsonLayoutStore(path);

        Assert.True((await store.SaveAsync(DataSet, "Items.Items", CreateLayout())).IsSuccess);

        var bytes = await File.ReadAllBytesAsync(path);
        Assert.True(bytes.Length > 3);
        Assert.False(bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
    }

    private static TableLayout CreateLayout() => new(
        new[]
        {
            new ColumnLayout("uiIndex", true, 80, 0),
            new ColumnLayout("szItemName", true, 240, 2),
            new ColumnLayout("CustomUnknown", false, 120, 1),
        },
        "uiIndex",
        TableSortDirection.Descending,
        new RowFilterGroup(
            RowFilterLogicalOperator.Or,
            new[]
            {
                new RowFilterCriterion("ubWeight", RowFilterOperator.GreaterThan, "5"),
                new RowFilterCriterion("szItemName", RowFilterOperator.Contains, "Glock"),
            }));
}
