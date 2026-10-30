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
    /// A mapped flat table with no record at all uses its curated hints as starter columns, with key
    /// fields first, so the first row can be filled in without reopening the table.
    /// </summary>
    [Fact]
    public async Task Build_EmptyMappedTable_UsesHintsAsStarterSchema()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "empty.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><INVENTORYLIST></INVENTORYLIST>");
        var metadata = new TableMetadata(
            TableName.Parse("NPCInventory.AlbertoInventory"),
            "INVENTORYLIST",
            "INVENTORY",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columnHints: new[]
            {
                new ColumnHint("sItemIndex")
                {
                    DataType = ColumnDataType.Integer,
                    EditorKind = ColumnEditorKind.IntegerBox,
                },
                new ColumnHint("uiIndex")
                {
                    DataType = ColumnDataType.Integer,
                    EditorKind = ColumnEditorKind.IntegerBox,
                },
                new ColumnHint("ubOptimalNumber")
                {
                    DataType = ColumnDataType.Integer,
                    EditorKind = ColumnEditorKind.IntegerBox,
                },
            });
        var document = await LoadAsync(path, metadata);

        var schema = new TableSchemaBuilder().Build(document);

        Assert.Equal(
            new[] { "uiIndex", "sItemIndex", "ubOptimalNumber" },
            schema.Columns.Select(static column => column.XmlFieldName));
        Assert.True(schema.FindColumn("uiIndex")!.IsKey);
        Assert.Equal(0, schema.FindColumn("sItemIndex")!.PresenceCount);
        Assert.Equal(ColumnDataType.Integer, schema.FindColumn("sItemIndex")!.DataType);
        Assert.Equal(ColumnEditorKind.IntegerBox, schema.FindColumn("sItemIndex")!.EditorKind);
    }

    /// <summary>
    /// An empty nested-record table does not expand its hints: the flat grid presents nested records
    /// through the workspace rows, not as direct fields of the root.
    /// </summary>
    [Fact]
    public async Task Build_EmptyNestedTable_DoesNotDeclareHintColumns()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "cities.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><CITY_INFO></CITY_INFO>");
        var metadata = new TableMetadata(
            TableName.Parse("Map.Cities"),
            "CITY_INFO",
            "COUNTRY",
            isLookup: false,
            new RecordKeySpec(new[]
            {
                new RecordKeySegment("CITYLIST/CITY", RecordKeyKind.Scalar, "uiIndex"),
            }),
            columnHints: new[]
            {
                new ColumnHint("uiIndex") { DataType = ColumnDataType.Integer },
            });
        var document = await LoadAsync(path, metadata);

        var schema = new TableSchemaBuilder().Build(document);

        Assert.Empty(schema.Columns);
    }

    /// <summary>
    /// A table whose records live under a nested key path discovers its columns from that path even
    /// though the direct record list of the document is empty.
    /// </summary>
    [Fact]
    public async Task Build_NestedRecordPath_DiscoversColumnsFromDeclaredPath()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("Overlays/cities-base.xml");
        var metadata = OverlayTestData.CitiesMetadata();
        var document = await OverlayTestData.LoadBaseAsync(path, metadata);

        var schema = new TableSchemaBuilder().Build(document);

        Assert.Empty(document.Records);
        Assert.Equal(2, schema.RecordCount);
        Assert.Equal(
            new[] { "uiIndex", "townName", "townUsesLoyalty" },
            schema.Columns.Select(static column => column.XmlFieldName));
        Assert.Equal("uiIndex", schema.KeyColumn!.XmlFieldName);
        Assert.Equal(2, schema.FindColumn("townName")!.PresenceCount);
        Assert.Equal(ColumnDataType.Integer, schema.FindColumn("uiIndex")!.DataType);
        Assert.Equal(ColumnDataType.String, schema.FindColumn("townName")!.DataType);
    }

    /// <summary>
    /// Nested fields of a nested record stay in the schema for the detail form but are hidden from the
    /// default grid until a dedicated nested editor exists, so they do not become empty pseudo-columns.
    /// </summary>
    [Fact]
    public async Task Build_NestedRecordField_IsHiddenNestedColumn()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "cities-nested.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><CITY_INFO><CITYLIST>"
            + "<CITY><uiIndex>1</uiIndex><townName>Omerta</townName>"
            + "<baseSector><x>9</x><y>1</y></baseSector></CITY>"
            + "</CITYLIST></CITY_INFO>");
        var metadata = OverlayTestData.CitiesMetadata();
        var document = await OverlayTestData.LoadBaseAsync(path, metadata);

        var schema = new TableSchemaBuilder().Build(document);

        var sector = schema.FindColumn("baseSector")!;
        Assert.True(sector.IsNested);
        Assert.Equal(ColumnDataType.Nested, sector.DataType);
        Assert.True(sector.IsHidden);
        Assert.True(sector.Flags.HasFlag(ColumnFlags.Hidden));
        Assert.False(schema.FindColumn("townName")!.IsHidden);
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
        Assert.True(nested.IsHidden);
        Assert.Equal(ColumnDataType.Nested, nested.DataType);
        Assert.Equal(ColumnEditorKind.SubTableGrid, nested.EditorKind);
        Assert.Equal("nested", nested.GroupKey);
        Assert.Equal(210, nested.GroupOrder);
    }

    /// <summary>
    /// Every compound key field becomes a key column in declaration order, while the first key column
    /// remains available through <see cref="TableSchema.KeyColumn"/>.
    /// </summary>
    [Fact]
    public async Task Build_CompoundKey_MarksAllKeyColumns()
    {
        using var workspace = new TestWorkspace();
        var path = Path.Combine(workspace.DirectoryPath, "attachments.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><ATTACHMENTLIST>"
            + "<ATTACHMENT><attachmentIndex>10</attachmentIndex><itemIndex>1</itemIndex><APCost>5</APCost></ATTACHMENT>"
            + "</ATTACHMENTLIST>");
        var metadata = new TableMetadata(
            TableName.Parse("Items.Attachments"),
            "ATTACHMENTLIST",
            "ATTACHMENT",
            isLookup: false,
            recordKeyFields: new[] { "attachmentIndex", "itemIndex" });
        var document = await LoadAsync(path, metadata);

        var schema = new TableSchemaBuilder().Build(document);

        Assert.True(schema.HasCompoundKey);
        Assert.Equal(
            new[] { "attachmentIndex", "itemIndex" },
            schema.KeyColumns.Select(static column => column.XmlFieldName));
        Assert.Same(schema.KeyColumns[0], schema.KeyColumn);
        Assert.All(schema.KeyColumns, static column => Assert.True(column.IsCompoundKey));
        Assert.All(schema.KeyColumns, static column => Assert.Equal("identity", column.GroupKey));
        Assert.False(schema.FindColumn("APCost")!.IsCompoundKey);
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
