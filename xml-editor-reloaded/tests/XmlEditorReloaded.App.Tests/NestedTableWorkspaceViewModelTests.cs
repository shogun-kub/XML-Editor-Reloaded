using System.IO;
using System.Xml.Linq;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Catalog;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Repository;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the nested city and facility workspace.
/// </summary>
public class NestedTableWorkspaceViewModelTests
{
    /// <summary>
    /// Cities keep their records under <c>CITYLIST/CITY</c>; the workspace edits city fields, nested
    /// coordinates, and the root country block.
    /// </summary>
    [Fact]
    public async Task CityWorkspace_EditsCityNestedCoordinatesAndCountry()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(dataDirectory, "TableData");
        var mapDirectory = Path.Combine(tableData, "Map");
        Directory.CreateDirectory(mapDirectory);
        var path = Path.Combine(mapDirectory, "Cities.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><CITY_INFO>"
            + "<COUNTRY><countryName>Arulco</countryName><countryNoun>Arulcans</countryNoun></COUNTRY>"
            + "<CITYLIST>"
            + "<CITY><uiIndex>1</uiIndex><townName>Omerta</townName><baseSector><x>9</x><y>1</y></baseSector></CITY>"
            + "<CITY><uiIndex>2</uiIndex><townName>Drassen</townName><baseSector><x>12</x><y>2</y></baseSector></CITY>"
            + "</CITYLIST></CITY_INFO>");

        var session = await CreateSessionAsync(workspace, dataDirectory, tableData);
        var descriptor = await FindDescriptorAsync(session, tableData, "Map.Cities");
        var viewModel = new NestedTableWorkspaceViewModel(
            session,
            descriptor,
            null,
            new FakeDialogService());

        await viewModel.InitializeAsync();

        Assert.True(viewModel.IsEditable);
        Assert.Equal(2, viewModel.Records.Count);
        Assert.Equal("Omerta", viewModel.Records[0].DisplayName);
        Assert.Equal("1", viewModel.SelectedRecordKey);

        var townName = FindField(viewModel, "townName");
        townName.Value = "Omerta City";
        Assert.Equal("Omerta City", FindField(viewModel, "townName").Value);

        FindField(viewModel, "baseSector/x").Value = "10";
        FindField(viewModel, "COUNTRY/countryName").Value = "Arulco New";
        Assert.DoesNotContain("error", viewModel.StatusText, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("Arulco New", FindField(viewModel, "COUNTRY/countryName").Value);

        Assert.True(await viewModel.SaveWithRecoveryAsync());
        var saved = XDocument.Load(path);
        var city = Assert.Single(
            saved.Root!.Element("CITYLIST")!.Elements("CITY"),
            element => element.Element("uiIndex")?.Value == "1");
        Assert.Equal("Omerta City", city.Element("townName")?.Value);
        Assert.Equal("10", city.Element("baseSector")?.Element("x")?.Value);
        Assert.Equal("Arulco New", saved.Root.Element("COUNTRY")?.Element("countryName")?.Value);
    }

    /// <summary>
    /// A facility workspace edits assignment fields and writes a translated tooltip only to the overlay.
    /// </summary>
    [Fact]
    public async Task FacilityWorkspace_EditsAssignmentsAndOverlayTooltips()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(dataDirectory, "TableData");
        var mapDirectory = Path.Combine(tableData, "Map");
        Directory.CreateDirectory(mapDirectory);
        var path = Path.Combine(mapDirectory, "FacilityTypes.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><FACILITYTYPES><FACILITYTYPE>"
            + "<ubIndex>0</ubIndex><szFacilityName>Hospital</szFacilityName>"
            + "<szFacilityShortName>Hosp</szFacilityShortName>"
            + "<ASSIGNMENT><ubAssignmentType>DOCTOR</ubAssignmentType>"
            + "<szTooltipText>Doctor</szTooltipText><ubStaffLimit>2</ubStaffLimit>"
            + "<CONDITIONS><ubMinimumMedical>1</ubMinimumMedical></CONDITIONS></ASSIGNMENT>"
            + "<ASSIGNMENT><ubAssignmentType>NURSE</ubAssignmentType>"
            + "<szTooltipText>Nurse</szTooltipText><ubStaffLimit>1</ubStaffLimit></ASSIGNMENT>"
            + "</FACILITYTYPE></FACILITYTYPES>");
        await File.WriteAllTextAsync(
            Path.Combine(mapDirectory, "Russian.FacilityTypes.xml"),
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><FACILITYTYPES><FACILITYTYPE>"
            + "<ubIndex>0</ubIndex>"
            + "<ASSIGNMENT><ubAssignmentType>DOCTOR</ubAssignmentType>"
            + "<szTooltipText>Доктор</szTooltipText></ASSIGNMENT>"
            + "</FACILITYTYPE></FACILITYTYPES>");

        var session = await CreateSessionAsync(workspace, dataDirectory, tableData);
        var descriptor = await FindDescriptorAsync(session, tableData, "Map.FacilityTypes");
        var viewModel = new NestedTableWorkspaceViewModel(
            session,
            descriptor,
            "ru",
            new FakeDialogService());

        await viewModel.InitializeAsync();

        Assert.True(viewModel.IsEditable);
        Assert.True(viewModel.IsTranslationMode);
        Assert.Single(viewModel.Records);
        Assert.Equal("Hospital", viewModel.Records[0].DisplayName);
        Assert.Contains(viewModel.Sections, section => section.Header == "ASSIGNMENT: DOCTOR");
        Assert.Contains(viewModel.Sections, section => section.Header == "ASSIGNMENT: NURSE");

        var tooltip = FindField(viewModel, "ASSIGNMENT[DOCTOR]/szTooltipText");
        Assert.Equal("Доктор", tooltip.Value);
        tooltip.Value = "Врач";

        var staffLimit = FindField(viewModel, "ASSIGNMENT[DOCTOR]/ubStaffLimit");
        staffLimit.Value = "3";
        var condition = FindField(viewModel, "ASSIGNMENT[DOCTOR]/CONDITIONS/ubMinimumMedical");
        condition.Value = "2";

        Assert.True(await viewModel.SaveWithRecoveryAsync());

        var baseDocument = XDocument.Load(path);
        var assignment = Assert.Single(
            baseDocument.Root!.Element("FACILITYTYPE")!.Elements("ASSIGNMENT"),
            element => element.Element("ubAssignmentType")?.Value == "DOCTOR");
        Assert.Equal("Doctor", assignment.Element("szTooltipText")?.Value);
        Assert.Equal("3", assignment.Element("ubStaffLimit")?.Value);
        Assert.Equal("2", assignment.Element("CONDITIONS")?.Element("ubMinimumMedical")?.Value);

        var overlay = XDocument.Load(Path.Combine(mapDirectory, "Russian.FacilityTypes.xml"));
        Assert.Equal(
            "Врач",
            overlay.Root!.Element("FACILITYTYPE")!
                .Element("ASSIGNMENT")!
                .Element("szTooltipText")?.Value);
    }

    /// <summary>
    /// Adding and undoing a city appends the new record inside the existing city container.
    /// </summary>
    [Fact]
    public async Task CityWorkspace_AddsAndUndoesNestedRecord()
    {
        using var workspace = new TestWorkspace();
        var dataDirectory = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(dataDirectory, "TableData");
        var mapDirectory = Path.Combine(tableData, "Map");
        Directory.CreateDirectory(mapDirectory);
        var path = Path.Combine(mapDirectory, "Cities.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><CITY_INFO><CITYLIST>"
            + "<CITY><uiIndex>1</uiIndex><townName>Omerta</townName></CITY>"
            + "</CITYLIST></CITY_INFO>");

        var session = await CreateSessionAsync(workspace, dataDirectory, tableData);
        var descriptor = await FindDescriptorAsync(session, tableData, "Map.Cities");
        var viewModel = new NestedTableWorkspaceViewModel(
            session,
            descriptor,
            null,
            new FakeDialogService());

        await viewModel.InitializeAsync();
        Assert.True(viewModel.IsEditable);
        viewModel.AddRecord();
        Assert.True(viewModel.IsDirty, viewModel.StatusText);
        Assert.Equal(2, viewModel.Workspace!.BaseDocument.Records.Count);
        Assert.Equal(2, viewModel.Workspace.GetRows().Count);
        Assert.Equal(string.Empty, viewModel.SearchText);

        Assert.DoesNotContain("error", viewModel.StatusText, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(2, viewModel.Records.Count);
        Assert.True(await viewModel.SaveWithRecoveryAsync());
        var added = XDocument.Load(path);
        var cities = added.Root!.Element("CITYLIST")!.Elements("CITY").ToArray();
        Assert.Equal(2, cities.Length);
        Assert.Equal("2", cities[1].Element("uiIndex")?.Value);

        viewModel.UndoCommand.Execute(null);
        Assert.Single(viewModel.Records);
        Assert.True(await viewModel.SaveWithRecoveryAsync());
        Assert.Single(XDocument.Load(path).Root!.Element("CITYLIST")!.Elements("CITY"));
    }

    private static NestedFieldViewModel FindField(NestedTableWorkspaceViewModel viewModel, string path) =>
        Assert.Single(
            viewModel.Sections.SelectMany(static section => section.Fields),
            field => string.Equals(field.PathText, path, StringComparison.Ordinal));

    private static async Task<IDataSetSession> CreateSessionAsync(
        TestWorkspace workspace,
        string dataDirectory,
        string tableData)
    {
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var catalog = new FileSystemTableCatalog(metadataProvider: provider);
        var session = await new DataSetSessionFactory(provider, catalog, new TableRepository(provider))
            .OpenAsync(dataDirectory);
        Assert.True(session.IsSuccess, session.ErrorText);
        Assert.NotNull(tableData);
        return session.Data!;
    }

    private static async Task<TableDescriptor> FindDescriptorAsync(
        IDataSetSession session,
        string tableData,
        string tableName)
    {
        var tables = await session.Catalog.GetTablesAsync(tableData);
        Assert.True(tables.IsSuccess, tables.ErrorText);
        return Assert.Single(tables.Data!, table => table.TableName.Value == tableName);
    }

    private sealed class FakeDialogService : IDialogService
    {
        public bool Confirm(string message, string title) => true;

        public UnsavedChangesChoice ConfirmUnsavedChanges(string message, string title) =>
            UnsavedChangesChoice.Discard;

        public ExternalChangeChoice ResolveExternalChange(string message, string title) =>
            ExternalChangeChoice.Cancel;

        public string? PickFolder(string title, string? initialDirectory = null) => null;

        public string? PickSaveFile(string title, string filter, string defaultFileName) => null;

        public ulong? EditBits(
            string title,
            ulong value,
            IReadOnlyDictionary<int, string>? labels = null,
            string? valueName = null) => null;

        public void ShowTranslationReport(TranslationReport report, string title)
        {
        }

        public BulkEditRequest? EditBulk(BulkEditContext context) => null;

        public ItemCreationRequest? CreateItem(
            string title,
            string suggestedId,
            string suggestedName,
            IReadOnlyList<LookupOption> classes) => null;
    }
}
