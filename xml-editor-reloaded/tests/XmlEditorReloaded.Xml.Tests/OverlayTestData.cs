using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Overlays;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Shared metadata and loading helpers for overlay tests.
/// </summary>
internal static class OverlayTestData
{
    /// <summary>
    /// Gets the Russian language descriptor used by the fixtures.
    /// </summary>
    public static LanguageDescriptor Russian { get; } = new("ru", "Russian", "Russian", 1);

    /// <summary>
    /// Creates metadata for the Items fixture.
    /// </summary>
    /// <returns>Items metadata.</returns>
    public static TableMetadata ItemsMetadata() => new(
        TableName.Parse("Items.Items"),
        "ITEMLIST",
        "ITEM",
        isLookup: false,
        new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "uiIndex") }),
        translatableFieldPaths: new[] { "szItemName", "szLongItemName", "szItemDesc", "szBRName", "szBRDesc" });

    /// <summary>
    /// Creates metadata for the PocketPopups fixture.
    /// </summary>
    /// <returns>Pocket popup metadata.</returns>
    public static TableMetadata PocketsMetadata() => new(
        TableName.Parse("Items.PocketPopups"),
        "POCKETPOPUPS",
        "POCKET",
        isLookup: false,
        new RecordKeySpec(new[] { new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "pIndex") }),
        translatableFieldPaths: new[] { "pName" });

    /// <summary>
    /// Creates metadata for the MercStartingGear fixture.
    /// </summary>
    /// <returns>Merc gear metadata with a positional GEARKIT segment.</returns>
    public static TableMetadata MercGearMetadata() => new(
        TableName.Parse("Inventory.MercStartingGear"),
        "MERCGEARLIST",
        "MERCGEAR",
        isLookup: false,
        new RecordKeySpec(new[]
        {
            new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "mIndex"),
            new RecordKeySegment("GEARKIT", RecordKeyKind.Positional),
        }),
        translatableFieldPaths: new[] { "mName", "GEARKIT/mGearKitName" });

    /// <summary>
    /// Creates metadata for the Cities fixture.
    /// </summary>
    /// <returns>Cities metadata with nested city keys and a country singleton.</returns>
    public static TableMetadata CitiesMetadata() => new(
        TableName.Parse("Map.Cities"),
        "CITY_INFO",
        "CITY",
        isLookup: false,
        new RecordKeySpec(new[]
        {
            new RecordKeySegment("CITYLIST/CITY", RecordKeyKind.Scalar, "uiIndex"),
            new RecordKeySegment("COUNTRY", RecordKeyKind.Singleton),
        }),
        translatableFieldPaths: new[] { "COUNTRY/countryName", "COUNTRY/countryNoun", "CITYLIST/CITY/townName" });

    /// <summary>
    /// Creates metadata for the FacilityTypes fixture.
    /// </summary>
    /// <returns>Facility metadata with a nested assignment key.</returns>
    public static TableMetadata FacilityMetadata() => new(
        TableName.Parse("Map.FacilityTypes"),
        "FACILITYTYPES",
        "FACILITYTYPE",
        isLookup: false,
        new RecordKeySpec(new[]
        {
            new RecordKeySegment(string.Empty, RecordKeyKind.Scalar, "ubIndex"),
            new RecordKeySegment("ASSIGNMENT", RecordKeyKind.Scalar, "ubAssignmentType"),
        }),
        translatableFieldPaths: new[] { "szFacilityName", "szFacilityShortName", "ASSIGNMENT/szTooltipText" });

    /// <summary>
    /// Loads a base fixture with metadata.
    /// </summary>
    /// <param name="path">Base file path.</param>
    /// <param name="metadata">Table metadata.</param>
    /// <returns>Loaded base document.</returns>
    public static async Task<TableDocument> LoadBaseAsync(string path, TableMetadata metadata)
    {
        var descriptor = new TableDescriptor(
            metadata.TableName,
            path,
            Path.GetFileName(path),
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await new TableFileLoader().LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!.GetDocument<TableDocument>()!;
    }

    /// <summary>
    /// Loads an overlay fixture with base metadata.
    /// </summary>
    /// <param name="basePath">Base file path.</param>
    /// <param name="overlayPath">Overlay file path.</param>
    /// <param name="metadata">Table metadata.</param>
    /// <returns>Loaded overlay document.</returns>
    public static async Task<OverlayDocument> LoadOverlayAsync(
        string basePath,
        string overlayPath,
        TableMetadata metadata)
    {
        var descriptor = new OverlayDescriptor(Russian, basePath, overlayPath);
        var load = await OverlayDocument.LoadAsync(descriptor, metadata);
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!;
    }
}
