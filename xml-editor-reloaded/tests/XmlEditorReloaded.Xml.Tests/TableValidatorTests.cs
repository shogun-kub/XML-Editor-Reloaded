using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Validation;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableValidator"/>.
/// </summary>
public class TableValidatorTests
{
    /// <summary>
    /// Fields absent from metadata are reported as informational issues, never errors.
    /// </summary>
    [Fact]
    public async Task Validate_UnknownField_IsInfo()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(
            path,
            Metadata(new ColumnMetadata("szItemName", ColumnDataType.String)));

        var report = await new TableValidator().ValidateAsync(document);

        var issue = Assert.Single(
            report.Issues,
            item => item.Code == ValidationCodes.UnknownField
                && item.FieldPath == "ubWeight"
                && item.RecordKey == "1");
        Assert.Equal(ValidationSeverity.Info, issue.Severity);
    }

    /// <summary>
    /// A non-numeric value in an integer column is a warning with record and path.
    /// </summary>
    [Fact]
    public async Task Validate_InvalidInteger_IsWarning()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace("<ubWeight>10</ubWeight>", "<ubWeight>abc</ubWeight>", StringComparison.Ordinal));
        var document = await LoadAsync(path, Metadata(new ColumnMetadata("ubWeight", ColumnDataType.Integer)));

        var report = await new TableValidator().ValidateAsync(document);

        var issue = Assert.Single(report.Issues, item => item.Code == ValidationCodes.InvalidType);
        Assert.Equal(ValidationSeverity.Warning, issue.Severity);
        Assert.Equal("1", issue.RecordKey);
        Assert.Equal("ubWeight", issue.FieldPath);
    }

    /// <summary>
    /// Duplicate primary keys are reported once per duplicated key.
    /// </summary>
    [Fact]
    public async Task Validate_DuplicateKey_IsWarning()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var text = await File.ReadAllTextAsync(path);
        await File.WriteAllTextAsync(
            path,
            text.Replace("<uiIndex>1</uiIndex>", "<uiIndex>0</uiIndex>", StringComparison.Ordinal));
        var document = await LoadAsync(path, Metadata());

        var report = await new TableValidator().ValidateAsync(document);

        Assert.Contains(report.Issues, issue =>
            issue.Code == ValidationCodes.DuplicateKey && issue.RecordKey == "0");
    }

    /// <summary>
    /// A record without its key field is reported as missing a key.
    /// </summary>
    [Fact]
    public async Task Validate_MissingKey_IsWarning()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var text = await File.ReadAllTextAsync(path);
        await File.WriteAllTextAsync(
            path,
            text.Replace("<uiIndex>1</uiIndex>", string.Empty, StringComparison.Ordinal));
        var document = await LoadAsync(path, Metadata());

        var report = await new TableValidator().ValidateAsync(document);

        Assert.Contains(report.Issues, issue => issue.Code == ValidationCodes.MissingKey);
    }

    /// <summary>
    /// Lookup references that do not exist in the lookup table are warnings.
    /// </summary>
    [Fact]
    public async Task Validate_MissingLookupReference_IsWarning()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace("<ubWeight>10</ubWeight>", "<ubWeight>99</ubWeight>", StringComparison.Ordinal));
        var metadata = Metadata(new ColumnMetadata("ubWeight", ColumnDataType.Lookup)
        {
            LookupTable = "Lookup.ItemClass",
        });
        var document = await LoadAsync(path, metadata);

        var report = await new TableValidator().ValidateAsync(document, new StubLookupRepository());

        Assert.Contains(report.Issues, issue =>
            issue.Code == ValidationCodes.LookupMissing && issue.RecordKey == "1" && issue.FieldPath == "ubWeight");
    }

    /// <summary>
    /// A dictionary mask with more than one placeholder is rejected.
    /// </summary>
    [Fact]
    public async Task Validate_MalformedDictMask_IsWarning()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var metadata = MetadataWithDict(
            new[] { new DictFieldDefinition("Item%Index%Size%Index%") });
        var document = await LoadAsync(path, metadata);

        var report = await new TableValidator().ValidateAsync(document);

        Assert.Contains(report.Issues, issue => issue.Code == ValidationCodes.DictMask);
    }

    /// <summary>
    /// Document-driven tables return a single informational unmapped issue.
    /// </summary>
    [Fact]
    public async Task Validate_Unmapped_ReturnsInfo()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path, metadata: null);

        var report = await new TableValidator().ValidateAsync(document);

        var issue = Assert.Single(report.Issues);
        Assert.Equal(ValidationCodes.Unmapped, issue.Code);
        Assert.Equal(ValidationSeverity.Info, issue.Severity);
    }

    /// <summary>
    /// Validation is read-only: it does not mark the document dirty or change the file.
    /// </summary>
    [Fact]
    public async Task Validate_DoesNotMutateDocument()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var before = await File.ReadAllBytesAsync(path);
        var document = await LoadAsync(path, Metadata());

        _ = await new TableValidator().ValidateAsync(document);

        Assert.False(document.IsDirty);
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
    }

    private static TableMetadata Metadata(params ColumnMetadata[] columns) =>
        new(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columns: columns);

    private static TableMetadata MetadataWithDict(DictFieldDefinition[] dictFields) =>
        new(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columns: Array.Empty<ColumnMetadata>(),
            dictFields: dictFields);

    private static async Task<TableDocument> LoadAsync(string path, TableMetadata? metadata)
    {
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: metadata is not null,
            Array.Empty<Core.Localization.OverlayDescriptor>());
        var load = await new TableFileLoader().LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }

    private sealed class StubLookupRepository : ILookupRepository
    {
        public Task<OperationResult<IReadOnlyList<LookupEntry>>> GetEntriesAsync(
            TableName lookupName,
            string? languageCode = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<LookupEntry>>.Success(
                new[] { new LookupEntry(1, "Gun") }));
    }
}
