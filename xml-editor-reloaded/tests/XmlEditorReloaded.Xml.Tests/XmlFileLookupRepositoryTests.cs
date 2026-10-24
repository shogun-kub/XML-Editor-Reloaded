using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Lookups;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="XmlFileLookupRepository"/>.
/// </summary>
public class XmlFileLookupRepositoryTests
{
    /// <summary>
    /// Base lookup entries are returned in file order.
    /// </summary>
    [Fact]
    public async Task GetEntries_ReadsBaseLookup()
    {
        using var workspace = new TestWorkspace();
        workspace.CopyFixtureDirectory("Lookup");
        var repository = new XmlFileLookupRepository(workspace.DirectoryPath);

        var result = await repository.GetEntriesAsync(TableName.Parse("Lookup.ItemClass"));

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(2, result.Data!.Count);
        Assert.Equal(new LookupEntry(1, "Gun"), result.Data[0]);
        Assert.Equal(new LookupEntry(2, "Knife"), result.Data[1]);
    }

    /// <summary>
    /// A language overlay replaces matching names and ignores orphan ids.
    /// </summary>
    [Fact]
    public async Task GetEntries_AppliesLanguageOverlay()
    {
        using var workspace = new TestWorkspace();
        workspace.CopyFixtureDirectory("Lookup");
        var repository = new XmlFileLookupRepository(
            workspace.DirectoryPath,
            new[] { new LanguageDescriptor("ru", "Russian", "Russian", 1) });

        var result = await repository.GetEntriesAsync(TableName.Parse("Lookup.ItemClass"), "ru");

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(2, result.Data!.Count);
        Assert.Equal(new LookupEntry(1, "Оружие"), result.Data[0]);
        Assert.Equal(new LookupEntry(2, "Knife"), result.Data[1]);
    }

    /// <summary>
    /// Lookup identifiers are 64-bit values; files such as ItemFlag exceed the signed 32-bit range.
    /// </summary>
    [Fact]
    public async Task GetEntries_Reads64BitId()
    {
        using var workspace = new TestWorkspace();
        var directory = workspace.CopyFixtureDirectory("Lookup");
        await File.WriteAllTextAsync(
            Path.Combine(directory, "Large.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data><ItemFlag><id>8589934592</id><name>Large flag</name></ItemFlag></JA2Data>");
        var repository = new XmlFileLookupRepository(workspace.DirectoryPath);

        var result = await repository.GetEntriesAsync(TableName.Parse("Lookup.Large"));

        Assert.True(result.IsSuccess, result.ErrorText);
        Assert.Equal(new LookupEntry(8589934592, "Large flag"), Assert.Single(result.Data!));
    }

    /// <summary>
    /// A missing lookup file is reported as an operation failure.
    /// </summary>
    [Fact]
    public async Task GetEntries_MissingFile_ReturnsFileNotFound()
    {
        using var workspace = new TestWorkspace();
        var repository = new XmlFileLookupRepository(workspace.DirectoryPath);

        var result = await repository.GetEntriesAsync(TableName.Parse("Lookup.Missing"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.IoFileNotFound, result.ErrorCode);
    }

    /// <summary>
    /// Malformed lookup XML is reported as a parse error.
    /// </summary>
    [Fact]
    public async Task GetEntries_MalformedFile_ReturnsParseError()
    {
        using var workspace = new TestWorkspace();
        var directory = workspace.CopyFixtureDirectory("Lookup");
        File.Copy(
            Path.Combine(AppContext.BaseDirectory, "Fixtures", "malformed.xml"),
            Path.Combine(directory, "Bad.xml"),
            overwrite: true);
        var repository = new XmlFileLookupRepository(workspace.DirectoryPath);

        var result = await repository.GetEntriesAsync(TableName.Parse("Lookup.Bad"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.XmlParseError, result.ErrorCode);
    }

    /// <summary>
    /// An entry without an integer id is reported as an invalid lookup entry.
    /// </summary>
    [Fact]
    public async Task GetEntries_EntryWithoutId_ReturnsInvalidEntry()
    {
        using var workspace = new TestWorkspace();
        var directory = workspace.CopyFixtureDirectory("Lookup");
        await File.WriteAllTextAsync(
            Path.Combine(directory, "Invalid.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><JA2Data><ItemClass><name>No id</name></ItemClass></JA2Data>");
        var repository = new XmlFileLookupRepository(workspace.DirectoryPath);

        var result = await repository.GetEntriesAsync(TableName.Parse("Lookup.Invalid"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.XmlInvalidLookupEntry, result.ErrorCode);
    }
}
