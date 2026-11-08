using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Data.Help;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the context-help view model.
/// </summary>
public class HelpViewModelTests
{
    /// <summary>
    /// A table opens with its summary, fields, sections, and selected cell field.
    /// </summary>
    [Fact]
    public void ShowTable_SelectsRequestedField()
    {
        var viewModel = new HelpViewModel(
            new JsonTableHelpProvider(),
            TableName.Parse("Items.Weapons"),
            "ubWeaponType");

        Assert.True(viewModel.HasTable);
        Assert.True(viewModel.HasSelection);
        Assert.Equal("Items.Weapons", viewModel.Table!.Value.Value);
        Assert.Equal("ubWeaponType", viewModel.SelectedField!.Name);
        Assert.Contains("weapon type", viewModel.SelectedField.Description, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Lookup.WeaponType", viewModel.SelectedField.Values, StringComparison.Ordinal);
        Assert.NotEmpty(viewModel.Fields);
        Assert.NotEmpty(viewModel.Links);
        Assert.NotEmpty(viewModel.Sections);
        Assert.Equal("tables/Items/Weapons.md", viewModel.DocumentPath);
    }

    /// <summary>
    /// Search switches the left pane to results, and opening a result selects its field.
    /// </summary>
    [Fact]
    public void Search_OpensMatchingField()
    {
        var viewModel = new HelpViewModel(new JsonTableHelpProvider())
        {
            SearchText = "recoil",
        };

        viewModel.SearchCommand.Execute(null);

        Assert.True(viewModel.IsSearchMode);
        Assert.True(viewModel.HasSearchResults);
        Assert.NotEmpty(viewModel.SearchResults);
        var result = viewModel.SearchResults.First(result =>
            result.Table.Value == "Items.Weapons"
            && result.FieldPath?.Contains("Recoil", StringComparison.Ordinal) == true);

        viewModel.OpenSearchResultCommand.Execute(result);

        Assert.False(viewModel.IsSearchMode);
        Assert.Equal("Items.Weapons", viewModel.Table!.Value.Value);
        Assert.Contains("Recoil", viewModel.SelectedField!.Path, StringComparison.Ordinal);
    }

    /// <summary>
    /// The field filter limits the list without losing the current table.
    /// </summary>
    [Fact]
    public void FieldFilter_LimitsFields()
    {
        var viewModel = new HelpViewModel(
            new JsonTableHelpProvider(),
            TableName.Parse("Items.Weapons"),
            "uiIndex");
        var before = viewModel.Fields.Count;

        viewModel.FieldFilter = "calibre";

        Assert.True(viewModel.Fields.Count < before);
        Assert.All(viewModel.Fields, field =>
            Assert.Contains("calibre", field.Path + field.Description, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// A related link opens the target table document in the same window.
    /// </summary>
    [Fact]
    public void RelatedLink_OpensTargetTable()
    {
        var viewModel = new HelpViewModel(
            new JsonTableHelpProvider(),
            TableName.Parse("Items.Weapons"),
            "ubCalibre");
        var link = viewModel.Links.First(link =>
            link.Target == "tables/Items/AmmoStrings.md");

        viewModel.OpenLinkCommand.Execute(link);

        Assert.Equal("Items.AmmoStrings", viewModel.Table!.Value.Value);
        Assert.True(viewModel.HasTable);
    }

    /// <summary>
    /// An undocumented field reports the missing description instead of silently selecting another field.
    /// </summary>
    [Fact]
    public void ShowTable_UnknownField_ReportsMissingDescription()
    {
        var viewModel = new HelpViewModel(
            new JsonTableHelpProvider(),
            TableName.Parse("Items.Weapons"),
            "FutureField");

        Assert.True(viewModel.HasTable);
        Assert.True(viewModel.HasMissingField);
        Assert.False(viewModel.HasSelection);
        Assert.Null(viewModel.SelectedField);
        Assert.Contains("FutureField", viewModel.MissingFieldText, StringComparison.Ordinal);
        Assert.Contains("COVERAGE.md", viewModel.MissingFieldText, StringComparison.Ordinal);
    }

    /// <summary>
    /// The open-table command reports the currently displayed table to the shell.
    /// </summary>
    [Fact]
    public void OpenTableCommand_ReportsCurrentTable()
    {
        var viewModel = new HelpViewModel(
            new JsonTableHelpProvider(),
            TableName.Parse("Items.Weapons"),
            "uiIndex");
        TableName? requested = null;
        viewModel.TableOpenRequested += table => requested = table;

        viewModel.OpenTableCommand.Execute(null);

        Assert.Equal("Items.Weapons", requested?.Value);
    }

    /// <summary>
    /// The coverage command asks the shell to open the coverage document.
    /// </summary>
    [Fact]
    public void OpenCoverageCommand_ReportsRequest()
    {
        var viewModel = new HelpViewModel(
            new JsonTableHelpProvider(),
            TableName.Parse("Items.Weapons"));
        var requested = false;
        viewModel.CoverageRequested += () => requested = true;

        viewModel.OpenCoverageCommand.Execute(null);

        Assert.True(requested);
    }

    /// <summary>
    /// Without a selected table the window shows an instruction instead of an empty field list.
    /// </summary>
    [Fact]
    public void NoTable_ShowsEmptyInstruction()
    {
        var viewModel = new HelpViewModel(new JsonTableHelpProvider());

        Assert.False(viewModel.HasTable);
        Assert.False(viewModel.HasSelection);
        Assert.Empty(viewModel.Fields);
        Assert.NotEmpty(viewModel.EmptyText);
    }

    /// <summary>
    /// An unknown table reports a clear message.
    /// </summary>
    [Fact]
    public void UnknownTable_ReportsMissingHelp()
    {
        var viewModel = new HelpViewModel(new JsonTableHelpProvider());
        viewModel.ShowTable(TableName.Parse("Missing.Table"));

        Assert.False(viewModel.HasTable);
        Assert.Contains("Missing.Table", viewModel.StatusText, StringComparison.Ordinal);
    }
}
