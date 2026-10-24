using System.IO;
using XmlEditorReloaded.App.Services;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for <see cref="DataSetPaths"/>.
/// </summary>
public class DataSetPathsTests
{
    /// <summary>
    /// A Data-1.13 directory resolves to its nested TableData and to itself as the game data directory.
    /// </summary>
    [Fact]
    public void Resolve_Data13Directory_ReturnsNestedTableData()
    {
        using var workspace = new TestWorkspace();
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        Directory.CreateDirectory(tableData);

        Assert.Equal(tableData, DataSetPaths.ResolveTableDataDirectory(data13));
        Assert.Equal(data13, DataSetPaths.ResolveGameDataDirectory(data13));
    }

    /// <summary>
    /// A TableData directory resolves to itself, and its parent is the game data directory.
    /// </summary>
    [Fact]
    public void Resolve_TableDataDirectory_ReturnsItselfAndParent()
    {
        using var workspace = new TestWorkspace();
        var data13 = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        Directory.CreateDirectory(tableData);

        Assert.Equal(tableData, DataSetPaths.ResolveTableDataDirectory(tableData));
        Assert.Equal(data13, DataSetPaths.ResolveGameDataDirectory(tableData));
    }

    /// <summary>
    /// A game root that contains Data-1.13/TableData resolves through the nested path.
    /// </summary>
    [Fact]
    public void Resolve_GameRoot_ReturnsNestedPath()
    {
        using var workspace = new TestWorkspace();
        var data13 = Path.Combine(workspace.DirectoryPath, "Data-1.13");
        var tableData = Path.Combine(data13, "TableData");
        Directory.CreateDirectory(tableData);

        Assert.Equal(tableData, DataSetPaths.ResolveTableDataDirectory(workspace.DirectoryPath));
        Assert.Equal(data13, DataSetPaths.ResolveGameDataDirectory(workspace.DirectoryPath));
    }

    /// <summary>
    /// Missing and empty paths resolve to <see langword="null"/>.
    /// </summary>
    [Fact]
    public void Resolve_MissingPath_ReturnsNull()
    {
        using var workspace = new TestWorkspace();

        Assert.Null(DataSetPaths.ResolveTableDataDirectory(null));
        Assert.Null(DataSetPaths.ResolveTableDataDirectory(string.Empty));
        Assert.Null(DataSetPaths.ResolveTableDataDirectory(workspace.PathOf("missing")));
        Assert.Null(DataSetPaths.ResolveGameDataDirectory(workspace.PathOf("missing")));
    }
}
