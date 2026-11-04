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
    /// A non-declaring column hint participates in type validation for a present field.
    /// </summary>
    [Fact]
    public async Task Validate_HintType_IsWarning()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace("<ubWeight>10</ubWeight>", "<ubWeight>abc</ubWeight>", StringComparison.Ordinal));
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("ubWeight") { DataType = ColumnDataType.Integer }));

        var report = await new TableValidator().ValidateAsync(document);

        var issue = Assert.Single(report.Issues, item => item.Code == ValidationCodes.InvalidType);
        Assert.Equal(ValidationSeverity.Warning, issue.Severity);
        Assert.Equal("ubWeight", issue.FieldPath);
    }

    /// <summary>
    /// A hint-only table does not claim a complete field set, so unknown fields stay silent.
    /// </summary>
    [Fact]
    public async Task Validate_HintOnlyTable_DoesNotReportUnknownFields()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var before = await File.ReadAllBytesAsync(path);
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("szItemName") { DataType = ColumnDataType.String }));

        var report = await new TableValidator().ValidateAsync(document);

        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.UnknownField);
        Assert.False(document.IsDirty);
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
    }

    /// <summary>
    /// Flag masks are combinations of bits, so their values are not checked as lookup ids.
    /// </summary>
    [Fact]
    public async Task Validate_FlagMask_IsNotLookupChecked()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path))
                .Replace("<AvailableAttachmentPoint>1</AvailableAttachmentPoint>", "<AvailableAttachmentPoint>3</AvailableAttachmentPoint>", StringComparison.Ordinal)
                .Replace("<AvailableAttachmentPoint>2</AvailableAttachmentPoint>", "<AvailableAttachmentPoint>3</AvailableAttachmentPoint>", StringComparison.Ordinal));
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("AvailableAttachmentPoint")
            {
                DataType = ColumnDataType.Flags,
                EditorKind = ColumnEditorKind.FlagEditor,
                LookupTable = "Lookup.AttachmentPoint",
            }));

        var report = await new TableValidator().ValidateAsync(document, new StubLookupRepository());

        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.LookupMissing);
        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.InvalidType);
    }

    /// <summary>
    /// A flag mask with a bit that no lookup entry names is a warning, and the message lists the bit.
    /// </summary>
    [Fact]
    public async Task Validate_FlagMask_WithUnnamedBit_IsWarning()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path))
                .Replace("<AvailableAttachmentPoint>1</AvailableAttachmentPoint>", "<AvailableAttachmentPoint>16</AvailableAttachmentPoint>", StringComparison.Ordinal)
                .Replace("<AvailableAttachmentPoint>2</AvailableAttachmentPoint>", "<AvailableAttachmentPoint>16</AvailableAttachmentPoint>", StringComparison.Ordinal));
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("AvailableAttachmentPoint")
            {
                DataType = ColumnDataType.Flags,
                EditorKind = ColumnEditorKind.FlagEditor,
                LookupTable = "Lookup.AttachmentPoint",
            }));

        var report = await new TableValidator().ValidateAsync(document, new NamedLookupRepository(1, 2, 4));

        var issues = report.Issues.Where(item => item.Code == ValidationCodes.LookupFlagMissing).ToArray();
        Assert.NotEmpty(issues);
        Assert.All(issues, issue =>
        {
            Assert.Contains("16", issue.Message, StringComparison.Ordinal);
            Assert.Equal("AvailableAttachmentPoint", issue.FieldPath);
        });
    }

    /// <summary>
    /// A flag mask whose bits are all named, including a combination of them, is not reported.
    /// </summary>
    [Fact]
    public async Task Validate_FlagMask_AllBitsNamed_NoWarning()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path))
                .Replace("<AvailableAttachmentPoint>1</AvailableAttachmentPoint>", "<AvailableAttachmentPoint>3</AvailableAttachmentPoint>", StringComparison.Ordinal)
                .Replace("<AvailableAttachmentPoint>2</AvailableAttachmentPoint>", "<AvailableAttachmentPoint>3</AvailableAttachmentPoint>", StringComparison.Ordinal));
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("AvailableAttachmentPoint")
            {
                DataType = ColumnDataType.Flags,
                EditorKind = ColumnEditorKind.FlagEditor,
                LookupTable = "Lookup.AttachmentPoint",
            }));

        var report = await new TableValidator().ValidateAsync(document, new NamedLookupRepository(1, 2));

        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.LookupFlagMissing);
        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.LookupMissing);
    }

    /// <summary>
    /// A curated numeric range produces a warning but never changes the stored value.
    /// </summary>
    [Theory]
    [InlineData("100")]
    [InlineData("101")]
    public async Task Validate_OutOfRange_IsWarningWithoutMutation(string value)
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace(
                "<ubWeight>10</ubWeight>",
                $"<ubWeight>{value}</ubWeight>",
                StringComparison.Ordinal));
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("ubWeight")
            {
                DataType = ColumnDataType.Integer,
                Maximum = 99,
            }));
        var before = await File.ReadAllBytesAsync(path);

        var report = await new TableValidator().ValidateAsync(document);

        var issue = Assert.Single(report.Issues, item => item.Code == ValidationCodes.OutOfRange);
        Assert.Equal(ValidationSeverity.Warning, issue.Severity);
        Assert.Equal("1", issue.RecordKey);
        Assert.Equal("ubWeight", issue.FieldPath);
        Assert.Contains("99", issue.Message, StringComparison.Ordinal);
        Assert.False(document.IsDirty);
        Assert.Equal(before, await File.ReadAllBytesAsync(path));
    }

    /// <summary>
    /// Values within the inclusive range and values of unbounded fields stay silent.
    /// </summary>
    [Fact]
    public async Task Validate_WithinRange_IsSilent()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("ubWeight")
            {
                DataType = ColumnDataType.Integer,
                Minimum = 0,
                Maximum = 10,
            }));

        var report = await new TableValidator().ValidateAsync(document);

        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.OutOfRange);
    }

    /// <summary>
    /// Flag masks above the signed 32-bit range are valid 64-bit values, including the largest
    /// mask that occurs in the reference data.
    /// </summary>
    [Theory]
    [InlineData("4294967296")]
    [InlineData("8589934592")]
    [InlineData("2415919104")]
    [InlineData("18446744073709551615")]
    public async Task Validate_LargeFlagMask_IsNotInvalidType(string value)
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace(
                "<AvailableAttachmentPoint>1</AvailableAttachmentPoint>",
                $"<AvailableAttachmentPoint>{value}</AvailableAttachmentPoint>",
                StringComparison.Ordinal));
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("AvailableAttachmentPoint")
            {
                DataType = ColumnDataType.Flags,
                LookupTable = "Lookup.AttachmentPoint",
            }));

        var report = await new TableValidator().ValidateAsync(document, new StubLookupRepository());

        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.InvalidType);
        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.LookupMissing);
    }

    /// <summary>
    /// A negative lookup value keeps its previous meaning: it is reported as missing, never as an
    /// invalid type, because lookup identifiers themselves are unsigned.
    /// </summary>
    [Fact]
    public async Task Validate_NegativeLookupValue_IsMissingNotInvalid()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace(
                "<ubWeight>10</ubWeight>",
                "<ubWeight>-1</ubWeight>",
                StringComparison.Ordinal));
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("ubWeight")
            {
                DataType = ColumnDataType.Lookup,
                LookupTable = "Lookup.ItemClass",
            }));

        var report = await new TableValidator().ValidateAsync(document, new StubLookupRepository());

        var issue = Assert.Single(
            report.Issues,
            item => item.Code == ValidationCodes.LookupMissing && item.RecordKey == "1");
        Assert.Equal("ubWeight", issue.FieldPath);
        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.InvalidType);
    }

    /// <summary>
    /// Integer bounds also apply to decimal fields; the fractional part is compared, not truncated.
    /// </summary>
    [Theory]
    [InlineData("0.25", false)]
    [InlineData("10.5", true)]
    public async Task Validate_DecimalField_UsesInclusiveBounds(string value, bool outOfRange)
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace(
                "<ubWeight>10</ubWeight>",
                $"<ubWeight>{value}</ubWeight>",
                StringComparison.Ordinal));
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("ubWeight")
            {
                DataType = ColumnDataType.Decimal,
                Minimum = -1,
                Maximum = 10,
            }));

        var report = await new TableValidator().ValidateAsync(document);

        if (outOfRange)
        {
            var issue = Assert.Single(report.Issues, item => item.Code == ValidationCodes.OutOfRange);
            Assert.Equal("ubWeight", issue.FieldPath);
        }
        else
        {
            Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.OutOfRange);
        }
    }

    /// <summary>
    /// Several fields of an exclusive group are reported together; one set field is valid.
    /// </summary>
    [Fact]
    public async Task Validate_ExclusiveFlags_WarnsOnlyWhenSeveralAreSet()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace(
                "<ubWeight>10</ubWeight>",
                "<ubWeight>10</ubWeight><Cannon>1</Cannon><Mortar>1</Mortar><RocketLauncher>0</RocketLauncher>",
                StringComparison.Ordinal));
        var metadata = MetadataWithRules(
            new[]
            {
                new ExclusiveFlagGroup(
                    "launcher_type",
                    new[] { "Cannon", "Mortar", "GrenadeLauncher", "RocketLauncher" }),
            });
        var document = await LoadAsync(path, metadata);

        var report = await new TableValidator().ValidateAsync(document);

        var issue = Assert.Single(report.Issues, item => item.Code == ValidationCodes.ExclusiveFlags);
        Assert.Equal(ValidationSeverity.Warning, issue.Severity);
        Assert.Equal("1", issue.RecordKey);
        Assert.Equal("Cannon", issue.FieldPath);
        Assert.Contains("Cannon", issue.Message, StringComparison.Ordinal);
        Assert.Contains("Mortar", issue.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("GrenadeLauncher", issue.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// A single set field of an exclusive group is valid.
    /// </summary>
    [Fact]
    public async Task Validate_ExclusiveFlags_OneField_IsSilent()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace(
                "<ubWeight>10</ubWeight>",
                "<ubWeight>10</ubWeight><Cannon>1</Cannon><Mortar>0</Mortar>",
                StringComparison.Ordinal));
        var metadata = MetadataWithRules(
            new[]
            {
                new ExclusiveFlagGroup("launcher_type", new[] { "Cannon", "Mortar" }),
            });
        var document = await LoadAsync(path, metadata);

        var report = await new TableValidator().ValidateAsync(document);

        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.ExclusiveFlags);
    }

    /// <summary>
    /// An indexed field above the class-dependent limit is reported; later zero fields are not.
    /// </summary>
    [Fact]
    public async Task Validate_ClassValueLimit_WarnsAboveAllowedIndex()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace(
                "<ubWeight>10</ubWeight>",
                "<ubWeight>10</ubWeight><lbeClass>1</lbeClass><lbePocketIndex1>5</lbePocketIndex1><lbePocketIndex5>5</lbePocketIndex5><lbePocketIndex6>7</lbePocketIndex6><lbePocketIndex7>0</lbePocketIndex7>",
                StringComparison.Ordinal));
        var metadata = MetadataWithRules(
            classValueLimits: new[]
            {
                new ClassValueLimit("lbeClass", "1", "lbePocketIndex%Index%", 5),
            });
        var document = await LoadAsync(path, metadata);

        var report = await new TableValidator().ValidateAsync(document);

        var issue = Assert.Single(report.Issues, item => item.Code == ValidationCodes.ClassValueLimit);
        Assert.Equal(ValidationSeverity.Warning, issue.Severity);
        Assert.Equal("lbePocketIndex6", issue.FieldPath);
        Assert.Contains("first 5", issue.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// A class value without a declared limit does not restrict its indexed fields.
    /// </summary>
    [Fact]
    public async Task Validate_ClassValueLimit_OtherClass_IsSilent()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace(
                "<ubWeight>10</ubWeight>",
                "<ubWeight>10</ubWeight><lbeClass>2</lbeClass><lbePocketIndex6>7</lbePocketIndex6>",
                StringComparison.Ordinal));
        var metadata = MetadataWithRules(
            classValueLimits: new[]
            {
                new ClassValueLimit("lbeClass", "1", "lbePocketIndex%Index%", 5),
            });
        var document = await LoadAsync(path, metadata);

        var report = await new TableValidator().ValidateAsync(document);

        Assert.DoesNotContain(report.Issues, item => item.Code == ValidationCodes.ClassValueLimit);
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
    /// A relation value absent from the resolved target table is reported with the source field, while
    /// known values stay silent.
    /// </summary>
    [Fact]
    public async Task Validate_RelationValue_UsesTargetMembership()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        await File.WriteAllTextAsync(
            path,
            (await File.ReadAllTextAsync(path)).Replace(
                "<ubWeight>10</ubWeight>",
                "<ubWeight>10</ubWeight><DefaultAttachment>99</DefaultAttachment>",
                StringComparison.Ordinal));
        var document = await LoadAsync(
            path,
            MetadataWithHints(new ColumnHint("DefaultAttachment") { DataType = ColumnDataType.Integer }));
        var targets = new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            ["DefaultAttachment"] = new HashSet<string>(new[] { "1", "2" }, StringComparer.Ordinal),
        };

        var report = await new TableValidator().ValidateAsync(
            document,
            relationTargetValues: targets);

        var issue = Assert.Single(
            report.Issues,
            item => item.Code == ValidationCodes.LookupMissing && item.FieldPath == "DefaultAttachment");
        Assert.Equal(ValidationSeverity.Warning, issue.Severity);
        Assert.Equal("1", issue.RecordKey);
        Assert.Contains("target table", issue.Message, StringComparison.Ordinal);

        var known = await new TableValidator().ValidateAsync(
            document,
            relationTargetValues: new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
            {
                ["DefaultAttachment"] = new HashSet<string>(new[] { "99" }, StringComparer.Ordinal),
            });
        Assert.DoesNotContain(known.Issues, item => item.Code == ValidationCodes.LookupMissing);
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

    private static TableMetadata MetadataWithHints(params ColumnHint[] hints) =>
        new(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columns: Array.Empty<ColumnMetadata>(),
            columnHints: hints);

    private static TableMetadata MetadataWithRules(
        ExclusiveFlagGroup[]? exclusiveFlagGroups = null,
        ClassValueLimit[]? classValueLimits = null) =>
        new(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columns: Array.Empty<ColumnMetadata>(),
            exclusiveFlagGroups: exclusiveFlagGroups,
            classValueLimits: classValueLimits);

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

    private sealed class NamedLookupRepository(params ulong[] ids) : ILookupRepository
    {
        public Task<OperationResult<IReadOnlyList<LookupEntry>>> GetEntriesAsync(
            TableName lookupName,
            string? languageCode = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OperationResult<IReadOnlyList<LookupEntry>>.Success(
                ids.Select(static id => new LookupEntry(id, id.ToString())).ToArray()));
    }
}
