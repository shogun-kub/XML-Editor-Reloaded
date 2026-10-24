using System.Globalization;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests that numeric editing and key allocation do not depend on the current culture.
/// </summary>
public class CultureIndependenceTests
{
    /// <summary>
    /// Under a comma-decimal culture, a decimal value typed with an invariant point is stored verbatim
    /// and untouched values keep their lexical form.
    /// </summary>
    [Fact]
    public async Task EditDecimal_UnderCommaCulture_StoresInvariantText()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            using var workspace = new TestWorkspace();
            var path = workspace.CopyFixture("items-mini.xml");
            var document = await LoadAsync(path);
            var record = document.Records[1];

            Assert.True(record.SetFieldValue("szItemName", "Glock 18").IsSuccess);
            Assert.Equal("1.0", record.GetRawValue("ProjectionFactor"));

            var result = record.SetFieldValue("ProjectionFactor", "1.5");

            Assert.True(result.IsSuccess, result.ErrorText);
            Assert.Equal("1.5", record.GetRawValue("ProjectionFactor"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    /// <summary>
    /// A comma used as a decimal separator is rejected instead of being stored as culture-specific text.
    /// </summary>
    [Fact]
    public async Task EditDecimal_CommaSeparator_IsRejected()
    {
        using var workspace = new TestWorkspace();
        var path = workspace.CopyFixture("items-mini.xml");
        var document = await LoadAsync(path);
        var record = document.Records[1];

        var result = record.SetFieldValue("ProjectionFactor", "1,5");

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodes.XmlInvalidNumber, result.ErrorCode);
        Assert.Equal("1.0", record.GetRawValue("ProjectionFactor"));
        Assert.False(document.IsDirty);
    }

    /// <summary>
    /// Allocating the next primary key under an Arabic culture writes ASCII digits.
    /// </summary>
    [Fact]
    public async Task AddRecord_UnderArabicCulture_WritesAsciiPrimaryKey()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
            using var workspace = new TestWorkspace();
            var path = workspace.CopyFixture("items-mini.xml");
            var document = await LoadAsync(path);

            var added = document.AddRecord();

            Assert.True(added.IsSuccess, added.ErrorText);
            Assert.Equal("2", added.Data!.GetRawValue("uiIndex"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    private static async Task<TableDocument> LoadAsync(string path)
    {
        var metadata = new TableMetadata(
            TableName.Parse("Items.Items"),
            "ITEMLIST",
            "ITEM",
            isLookup: false,
            new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
            columns: new[] { new ColumnMetadata("ProjectionFactor", ColumnDataType.Decimal) });
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());

        var load = await new TableFileLoader().LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }
}
