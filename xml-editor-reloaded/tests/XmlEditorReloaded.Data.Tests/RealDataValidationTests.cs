using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Data.Metadata;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Loading;
using XmlEditorReloaded.Xml.Lookups;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Validation;

namespace XmlEditorReloaded.Data.Tests;

/// <summary>
/// Environment-gated validation check against a real game <c>TableData</c> directory.
/// Set <c>XER_CORPUS_DIR</c> to a v4 or v5 TableData directory to enable it.
/// </summary>
public class RealDataValidationTests
{
    /// <summary>
    /// Mapped tables validate with the verified hints and produce no type or lookup warnings on
    /// either reference data set. The known malformed v5 lookup file remains a load diagnostic.
    /// </summary>
    [Fact]
    public async Task MappedTables_ProduceNoFalseTypeOrLookupWarnings()
    {
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var tables = await provider.GetAllTablesAsync();
        Assert.True(tables.IsSuccess, tables.ErrorText);
        var languages = await provider.GetLanguagesAsync();
        Assert.True(languages.IsSuccess, languages.ErrorText);
        var lookups = new XmlFileLookupRepository(tableData, languages.Data!, provider);
        var loader = new TableFileLoader();
        var validator = new TableValidator();
        var failures = new List<string>();
        var checkedTables = 0;

        foreach (var metadata in tables.Data!)
        {
            var relativePath = metadata.TableName.ToRelativePath();
            var filePath = Path.Combine(tableData, relativePath);
            if (!File.Exists(filePath))
            {
                continue;
            }

            var descriptor = new TableDescriptor(
                metadata.TableName,
                filePath,
                relativePath,
                IsMapped: true,
                Array.Empty<OverlayDescriptor>());
            var load = await loader.LoadAsync(descriptor, metadata);
            if (!load.IsSuccess)
            {
                failures.Add($"{relativePath}: load failed: {load.ErrorCode} {load.ErrorText}");
                continue;
            }

            if (load.Data!.Status == TableDocumentStatus.Malformed)
            {
                if (relativePath != "Lookup/AttachmentPoint.xml")
                {
                    failures.Add($"{relativePath}: unexpected malformed document");
                }

                continue;
            }

            var document = load.Data.GetDocument<TableDocument>();
            if (document is null)
            {
                failures.Add($"{relativePath}: no document returned");
                continue;
            }

            checkedTables++;
            var report = await validator.ValidateAsync(document, lookups);
            foreach (var issue in report.Issues.Where(static issue =>
                         issue.Code is ValidationCodes.InvalidType or ValidationCodes.LookupMissing))
            {
                failures.Add($"{relativePath}: {issue.Code} at {issue.FieldPath}: {issue.Message}");
            }
        }

        Assert.True(checkedTables > 50, $"Only {checkedTables} mapped tables were checked.");
        Assert.True(
            failures.Count == 0,
            $"False validation warnings ({failures.Count}):{Environment.NewLine}{string.Join(Environment.NewLine, failures.Take(25))}");
    }

    /// <summary>
    /// The real Items schema receives the verified hint types, lookup references, and flag editors.
    /// </summary>
    [Fact]
    public async Task ItemsSchema_UsesVerifiedHints()
    {
        var tableData = Environment.GetEnvironmentVariable("XER_CORPUS_DIR");
        if (string.IsNullOrWhiteSpace(tableData) || !Directory.Exists(tableData))
        {
            return;
        }

        using var workspace = new TestWorkspace();
        var databasePath = workspace.PathOf("metadata.sqlite");
        Assert.True((await new DatabaseMigrator().MigrateAsync(databasePath)).IsSuccess);
        var provider = new SqliteMetadataProvider(databasePath);
        var items = await provider.GetTableAsync(TableName.Parse("Items.Items"));
        Assert.True(items.IsSuccess, items.ErrorText);
        var filePath = Path.Combine(tableData, "Items/Items.xml");
        var descriptor = new TableDescriptor(
            items.Data!.TableName,
            filePath,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await new TableFileLoader().LoadAsync(descriptor, items.Data);
        Assert.True(load.IsSuccess, load.ErrorText);
        var document = load.Data!.GetDocument<TableDocument>()!;

        var schema = new TableSchemaBuilder().Build(document);

        Assert.Equal(224, schema.Columns.Count);
        Assert.Equal(ColumnDataType.Flags, Assert.Single(schema.Columns, column => column.XmlFieldName == "usItemClass").DataType);
        Assert.Equal(ColumnDataType.Flags, Assert.Single(schema.Columns, column => column.XmlFieldName == "DrugType").DataType);
        Assert.Equal(ColumnDataType.Integer, Assert.Single(schema.Columns, column => column.XmlFieldName == "DefaultAttachment").DataType);
        Assert.Equal(ColumnDataType.Lookup, Assert.Single(schema.Columns, column => column.XmlFieldName == "Inseparable").DataType);
        Assert.Equal("Lookup.Separability", Assert.Single(schema.Columns, column => column.XmlFieldName == "Inseparable").LookupTable);
        Assert.Equal(8, schema.Columns.Count(column => column.EditorKind == ColumnEditorKind.FlagEditor));
        Assert.False(document.IsDirty);
    }
}
