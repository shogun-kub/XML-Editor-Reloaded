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

    /// <summary>
    /// A facility assignment is appended from the toolbar, its translated tooltip is written to the
    /// overlay, and deleting it removes both records.
    /// </summary>
    [Fact]
    public async Task FacilityWorkspace_AddsAndDeletesAssignment()
    {
        using var workspace = new TestWorkspace();
        var (path, overlayPath, session, descriptor) = await CreateFacilityAsync(workspace, withOverlay: true);
        var viewModel = new NestedTableWorkspaceViewModel(
            session,
            descriptor,
            "ru",
            new FakeDialogService());

        await viewModel.InitializeAsync();

        Assert.True(viewModel.HasAssignmentSegment);
        Assert.Contains(viewModel.BlockAddActions, action => action.AutomationId == "NestedAddBlock_PRODUCTIONLINE");
        viewModel.NewAssignmentType = "CHEF";
        Assert.True(viewModel.AddAssignmentCommand.CanExecute(null));

        viewModel.AddAssignmentCommand.Execute(null);

        Assert.Contains(viewModel.Sections, section => section.Header == "ASSIGNMENT: CHEF");
        var tooltip = FindField(viewModel, "ASSIGNMENT[CHEF]/szTooltipText");
        Assert.Equal(string.Empty, tooltip.Value);
        tooltip.Value = "Шеф";
        Assert.True(viewModel.IsDirty);
        Assert.True(await viewModel.SaveWithRecoveryAsync());

        var baseDocument = XDocument.Load(path);
        Assert.Contains(
            baseDocument.Root!.Element("FACILITYTYPE")!.Elements("ASSIGNMENT"),
            element => element.Element("ubAssignmentType")?.Value == "CHEF");
        Assert.Equal(
            "Шеф",
            XDocument.Load(overlayPath).Root!
                .Element("FACILITYTYPE")!
                .Elements("ASSIGNMENT")
                .Single(element => element.Element("ubAssignmentType")?.Value == "CHEF")
                .Element("szTooltipText")?.Value);

        var chefSection = Assert.Single(viewModel.Sections, section => section.Header == "ASSIGNMENT: CHEF");
        Assert.True(viewModel.DeleteSectionCommand.CanExecute(chefSection));
        viewModel.DeleteSectionCommand.Execute(chefSection);
        Assert.DoesNotContain(viewModel.Sections, section => section.Header == "ASSIGNMENT: CHEF");
        Assert.True(await viewModel.SaveWithRecoveryAsync());
        Assert.DoesNotContain(
            XDocument.Load(path).Root!.Element("FACILITYTYPE")!.Elements("ASSIGNMENT"),
            element => element.Element("ubAssignmentType")?.Value == "CHEF");
        Assert.DoesNotContain(
            XDocument.Load(overlayPath).Root!.Element("FACILITYTYPE")!.Elements("ASSIGNMENT"),
            element => element.Element("ubAssignmentType")?.Value == "CHEF");
    }

    /// <summary>
    /// Repeated assembly lists and production-line blocks are edited occurrence-aware and can be added
    /// and deleted as whole blocks.
    /// </summary>
    [Fact]
    public async Task FacilityWorkspace_EditsRepeatedListsAndBlocks()
    {
        using var workspace = new TestWorkspace();
        var (path, _, session, descriptor) = await CreateFacilityAsync(workspace, withOverlay: false);
        var viewModel = new NestedTableWorkspaceViewModel(
            session,
            descriptor,
            null,
            new FakeDialogService());

        await viewModel.InitializeAsync();

        var doctorSection = Assert.Single(viewModel.Sections, section => section.Header == "ASSIGNMENT: DOCTOR");
        var drugList = Assert.Single(doctorSection.Lists, list => list.Label == "drugitem");
        Assert.Equal(new[] { "245", "255" }, drugList.Values.Select(value => value.Value));
        drugList.AddCommand.Execute(null);
        drugList = Assert.Single(
            Assert.Single(viewModel.Sections, section => section.Header == "ASSIGNMENT: DOCTOR").Lists,
            list => list.Label == "drugitem");
        Assert.Equal(3, drugList.Values.Count);
        drugList.Values[2].Value = "300";

        var firstBlock = Assert.Single(
            viewModel.Sections,
            section => section.AutomationId == "NestedSection_PRODUCTIONLINE_0");
        var itemField = Assert.Single(firstBlock.Fields, field => field.Name == "usItemToCreate");
        Assert.Equal("1446", itemField.Value);
        itemField.Value = "2000";
        var optionalList = Assert.Single(
            Assert.Single(viewModel.Sections, section => section.AutomationId == "NestedSection_PRODUCTIONLINE_0").Lists);
        Assert.Equal(new[] { "1", "2" }, optionalList.Values.Select(value => value.Value));
        optionalList.AddCommand.Execute(null);
        optionalList = Assert.Single(
            Assert.Single(viewModel.Sections, section => section.AutomationId == "NestedSection_PRODUCTIONLINE_0").Lists);
        optionalList.Values[2].Value = "9";

        var addBlock = Assert.Single(viewModel.BlockAddActions);
        addBlock.Command.Execute(null);
        Assert.Equal(3, viewModel.Sections.Count(section => section.BlockFieldName == "PRODUCTIONLINE"));
        var lastBlock = viewModel.Sections.Last(section => section.BlockFieldName == "PRODUCTIONLINE");
        Assert.True(viewModel.DeleteSectionCommand.CanExecute(lastBlock));
        viewModel.DeleteSectionCommand.Execute(lastBlock);
        Assert.Equal(2, viewModel.Sections.Count(section => section.BlockFieldName == "PRODUCTIONLINE"));

        Assert.True(await viewModel.SaveWithRecoveryAsync());
        var facility = XDocument.Load(path).Root!.Element("FACILITYTYPE")!;
        var doctor = facility.Elements("ASSIGNMENT")
            .Single(element => element.Element("ubAssignmentType")?.Value == "DOCTOR");
        Assert.Equal(
            new[] { "245", "255", "300" },
            doctor.Element("DRUNK")!
                .Element("drugitems")!
                .Elements("drugitem")
                .Select(element => element.Value));
        var lines = facility.Elements("PRODUCTIONLINE").ToArray();
        Assert.Equal(2, lines.Length);
        Assert.Equal("2000", lines[0].Element("usItemToCreate")?.Value);
        Assert.Equal(
            new[] { "1", "2", "9" },
            lines[0].Elements("usOptional_ItemUsedUp").Select(element => element.Value));
    }

    /// <summary>
    /// The nested city editor marks the qualified town name as translatable and writes it only to the
    /// selected overlay.
    /// </summary>
    [Fact]
    public async Task CityWorkspace_OverlayTownName_WritesOnlyOverlay()
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
        var overlayPath = Path.Combine(mapDirectory, "Russian.Cities.xml");
        await File.WriteAllTextAsync(
            overlayPath,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><CITY_INFO><CITYLIST>"
            + "<CITY><uiIndex>1</uiIndex><townName>Омерта</townName></CITY>"
            + "</CITYLIST></CITY_INFO>");

        var session = await CreateSessionAsync(workspace, dataDirectory, tableData);
        var descriptor = await FindDescriptorAsync(session, tableData, "Map.Cities");
        var viewModel = new NestedTableWorkspaceViewModel(
            session,
            descriptor,
            "ru",
            new FakeDialogService());

        await viewModel.InitializeAsync();

        var townName = FindField(viewModel, "townName");
        Assert.True(townName.IsTranslatable);
        Assert.True(townName.IsTranslated);
        townName.Value = "Омерта Новая";
        Assert.True(await viewModel.SaveWithRecoveryAsync());

        Assert.Equal(
            "Omerta",
            XDocument.Load(path).Root!
                .Element("CITYLIST")!
                .Element("CITY")!
                .Element("townName")?.Value);
        Assert.Equal(
            "Омерта Новая",
            XDocument.Load(overlayPath).Root!
                .Element("CITYLIST")!
                .Element("CITY")!
                .Element("townName")?.Value);
    }

    private static async Task<(string Path, string OverlayPath, IDataSetSession Session, TableDescriptor Descriptor)>
        CreateFacilityAsync(TestWorkspace workspace, bool withOverlay)
    {
        var dataDirectory = workspace.PathOf("Data-1.13");
        var tableData = Path.Combine(dataDirectory, "TableData");
        var mapDirectory = Path.Combine(tableData, "Map");
        Directory.CreateDirectory(mapDirectory);
        var path = Path.Combine(mapDirectory, "FacilityTypes.xml");
        await File.WriteAllTextAsync(
            path,
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><FACILITYTYPES><FACILITYTYPE>"
            + "<ubIndex>1</ubIndex><szFacilityName>Hospital</szFacilityName>"
            + "<szFacilityShortName>Hospital</szFacilityShortName>"
            + "<ASSIGNMENT><ubAssignmentType>DOCTOR</ubAssignmentType>"
            + "<szTooltipText>Doctor</szTooltipText><ubStaffLimit>2</ubStaffLimit>"
            + "<DRUNK><ubChance>100</ubChance><drugitems>"
            + "<drugitem>245</drugitem><drugitem>255</drugitem>"
            + "</drugitems></DRUNK></ASSIGNMENT>"
            + "<PRODUCTIONLINE><usItemToCreate>1446</usItemToCreate>"
            + "<usOptional_ItemUsedUp>1</usOptional_ItemUsedUp>"
            + "<usOptional_ItemUsedUp>2</usOptional_ItemUsedUp></PRODUCTIONLINE>"
            + "<PRODUCTIONLINE><usItemToCreate>1447</usItemToCreate></PRODUCTIONLINE>"
            + "</FACILITYTYPE></FACILITYTYPES>");
        var overlayPath = Path.Combine(mapDirectory, "Russian.FacilityTypes.xml");
        if (withOverlay)
        {
            await File.WriteAllTextAsync(
                overlayPath,
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><FACILITYTYPES><FACILITYTYPE>"
                + "<ubIndex>1</ubIndex>"
                + "<ASSIGNMENT><ubAssignmentType>DOCTOR</ubAssignmentType>"
                + "<szTooltipText>Доктор</szTooltipText></ASSIGNMENT>"
                + "</FACILITYTYPE></FACILITYTYPES>");
        }

        var session = await CreateSessionAsync(workspace, dataDirectory, tableData);
        var descriptor = await FindDescriptorAsync(session, tableData, "Map.FacilityTypes");
        return (path, overlayPath, session, descriptor);
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
