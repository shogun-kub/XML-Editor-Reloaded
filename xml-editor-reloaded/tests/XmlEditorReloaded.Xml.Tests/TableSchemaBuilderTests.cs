using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Schema;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for <see cref="TableSchemaBuilder"/>.
/// </summary>
public class TableSchemaBuilderTests
{
    /// <summary>
    /// Sparse records are unioned in first-appearance order with presence counts and inferred types.
    /// </summary>
    [Fact]
    public async Task Build_DocumentDriven_UnionsSparseFields()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path, metadata: null);

        var schema = new TableSchemaBuilder().Build(document);

        Assert.True(schema.IsDocumentDriven);
        Assert.Equal(2, schema.RecordCount);
        Assert.Equal(
            new[]
            {
                "uiIndex",
                "szItemName",
                "ubWeight",
                "spreadPattern",
                "CustomUnknown",
                "ProjectionFactor",
                "AvailableAttachmentPoint",
            },
            schema.Columns.Select(static column => column.XmlFieldName));

        Assert.Equal(2, schema.FindColumn("uiIndex")!.PresenceCount);
        Assert.Equal(1, schema.FindColumn("ProjectionFactor")!.PresenceCount);
        Assert.Equal(ColumnDataType.Integer, schema.FindColumn("uiIndex")!.DataType);
        Assert.Equal(ColumnDataType.String, schema.FindColumn("szItemName")!.DataType);
        Assert.Equal(ColumnDataType.String, schema.FindColumn("spreadPattern")!.DataType);
        Assert.Equal(ColumnDataType.Decimal, schema.FindColumn("ProjectionFactor")!.DataType);
        Assert.True(schema.FindColumn("AvailableAttachmentPoint")!.IsList);
        Assert.Equal(2, schema.FindColumn("AvailableAttachmentPoint")!.MaxOccurrences);
        Assert.True(schema.FindColumn("uiIndex")!.IsKey);
        Assert.Same(schema.KeyColumn, schema.FindColumn("uiIndex"));
    }

    /// <summary>
    /// Curated column metadata overrides inferred types and editor hints and contributes absent fields.
    /// </summary>
    [Fact]
    public async Task Build_WithMetadata_AppliesOverridesAndDeclaredFields()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "rows.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><LIST>"
            + "<ROW><id>1</id><name>A</name><flag>0</flag><ratio>1.5</ratio>"
            + "<Tag>x</Tag><Tag>y</Tag><ItemCapacityPerSize0>3</ItemCapacityPerSize0><empty></empty></ROW>"
            + "<ROW><id>2</id><name>B</name><count>7</count></ROW>"
            + "</LIST>");
        var metadata = new TableMetadata(
            TableName.Parse("Test.Rows"),
            "LIST",
            "ROW",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "id") }),
            columns: new[]
            {
                new ColumnMetadata("name", ColumnDataType.String, ColumnEditorKind.LookupCombo)
                {
                    DisplayWidth = 180,
                    LookupTable = "Lookup.ItemClass",
                    TooltipKey = "name_tip",
                },
                new ColumnMetadata("count", ColumnDataType.Integer) { Flags = ColumnFlags.Hidden },
                new ColumnMetadata("ratio", ColumnDataType.Decimal, ColumnEditorKind.DecimalBox),
                new ColumnMetadata("Extra", ColumnDataType.String),
            },
            listFields: new[] { new ListFieldDefinition("Tag") },
            dictFields: new[] { new DictFieldDefinition("ItemCapacityPerSize%Index%") },
            translatableFieldPaths: new[] { "name" });
        var document = await LoadAsync(path, metadata);

        var schema = new TableSchemaBuilder().Build(document);

        Assert.False(schema.IsDocumentDriven);
        Assert.Equal(
            new[]
            {
                "id",
                "name",
                "flag",
                "ratio",
                "Tag",
                "ItemCapacityPerSize0",
                "empty",
                "count",
                "Extra",
            },
            schema.Columns.Select(static column => column.XmlFieldName));

        var name = schema.FindColumn("name")!;
        Assert.Equal(ColumnEditorKind.LookupCombo, name.EditorKind);
        Assert.Equal(180, name.DisplayWidth);
        Assert.Equal("Lookup.ItemClass", name.LookupTable);
        Assert.Equal("name_tip", name.TooltipKey);
        Assert.True(name.IsTranslatable);
        Assert.True(name.Flags.HasFlag(ColumnFlags.Translatable));

        var count = schema.FindColumn("count")!;
        Assert.True(count.IsHidden);
        Assert.True(count.Flags.HasFlag(ColumnFlags.Hidden));

        Assert.Equal(ColumnDataType.Boolean, schema.FindColumn("flag")!.DataType);
        Assert.Equal(ColumnDataType.Decimal, schema.FindColumn("ratio")!.DataType);
        Assert.Equal(ColumnDataType.String, schema.FindColumn("empty")!.DataType);
        Assert.True(schema.FindColumn("Tag")!.IsList);
        Assert.Equal(2, schema.FindColumn("Tag")!.MaxOccurrences);

        var capacity = schema.FindColumn("ItemCapacityPerSize0")!;
        Assert.True(capacity.IsDict);
        Assert.Equal("ItemCapacityPerSize%Index%", capacity.DictMask);

        var declaredOnly = schema.FindColumn("Extra")!;
        Assert.Equal(0, declaredOnly.PresenceCount);
        Assert.Equal(ColumnDataType.String, declaredOnly.DataType);
        Assert.True(schema.FindColumn("id")!.IsKey);
    }

    /// <summary>
    /// Column hints refine fields that exist in the document and never declare absent fields.
    /// </summary>
    [Fact]
    public async Task Build_ColumnHints_RefinePresentFieldsOnly()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "hints.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><LIST>"
            + "<ROW><id>1</id><mask>3</mask></ROW>"
            + "</LIST>");
        var metadata = new TableMetadata(
            TableName.Parse("Test.Hints"),
            "LIST",
            "ROW",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "id") }),
            columnHints: new[]
            {
                new ColumnHint("mask")
                {
                    DataType = ColumnDataType.Flags,
                    EditorKind = ColumnEditorKind.FlagEditor,
                    GroupKey = "flags",
                    GroupOrder = 100,
                },
                new ColumnHint("absent")
                {
                    DataType = ColumnDataType.Flags,
                    EditorKind = ColumnEditorKind.FlagEditor,
                    GroupKey = "flags",
                },
            });
        var document = await LoadAsync(path, metadata);

        var schema = new TableSchemaBuilder().Build(document);

        Assert.Equal(new[] { "id", "mask" }, schema.Columns.Select(static column => column.XmlFieldName));
        var mask = schema.FindColumn("mask")!;
        Assert.Equal(ColumnDataType.Flags, mask.DataType);
        Assert.Equal(ColumnEditorKind.FlagEditor, mask.EditorKind);
        Assert.Equal("flags", mask.GroupKey);
        Assert.Equal(100, mask.GroupOrder);
        Assert.Null(schema.FindColumn("absent"));
        Assert.Equal("identity", schema.FindColumn("id")!.GroupKey);
        Assert.Equal(10, schema.FindColumn("id")!.GroupOrder);
    }

    /// <summary>
    /// Elements with child nodes are typed as nested and grouped as structures instead of being
    /// inferred from concatenated text.
    /// </summary>
    [Fact]
    public async Task Build_NestedElement_UsesNestedTypeAndGroup()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "nested.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><LIST>"
            + "<ROW><id>1</id><MODIFIERS><FlatBase>1</FlatBase><PercentAim>0.5</PercentAim></MODIFIERS></ROW>"
            + "</LIST>");
        var document = await LoadAsync(path, metadata: null);

        var schema = new TableSchemaBuilder().Build(document);
        var nested = schema.FindColumn("MODIFIERS")!;

        Assert.True(nested.IsNested);
        Assert.Equal(ColumnDataType.Nested, nested.DataType);
        Assert.Equal(ColumnEditorKind.SubTableGrid, nested.EditorKind);
        Assert.Equal("nested", nested.GroupKey);
        Assert.Equal(210, nested.GroupOrder);
    }

    private static async Task<TableDocument> LoadAsync(string path, TableMetadata? metadata)
    {
        var descriptor = new TableDescriptor(
            TableName.Parse(metadata?.TableName.Value ?? "Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: metadata is not null,
            Array.Empty<OverlayDescriptor>());
        var load = await new TableFileLoader().LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }
}
