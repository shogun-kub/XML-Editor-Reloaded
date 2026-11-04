using System.Globalization;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Lookups;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for <see cref="LookupUsageViewModel"/> and its item projection.
/// </summary>
public class LookupUsageViewModelTests
{
    /// <summary>
    /// A non-empty report projects every usage and summarizes exact versus retained counts.
    /// </summary>
    [Fact]
    public void Report_ProjectsUsagesAndSummary()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var report = new LookupUsageReport(
            TableName.Parse("Lookup.Cursor"),
            1,
            UsageCount: 3,
            new[]
            {
                new LookupUsage(TableName.Parse("Items.Items"), "1", "ubCursor", "1"),
                new LookupUsage(TableName.Parse("Items.Items"), "2", "ubCursor", "1"),
            },
            ScannedTableCount: 2,
            FailedTableCount: 0,
            SuppressedUsageCount: 1);

        var viewModel = new LookupUsageViewModel(report, "Where used: Lookup.Cursor, ID 1");

        Assert.False(viewModel.IsEmpty);
        Assert.Equal("Where used: Lookup.Cursor, ID 1", viewModel.Title);
        Assert.Contains("3", viewModel.SummaryText, StringComparison.Ordinal);
        Assert.Contains("2", viewModel.SummaryText, StringComparison.Ordinal);
        Assert.Equal(2, viewModel.Usages.Count);
        Assert.Equal("Items.Items", viewModel.Usages[0].TableText);
        Assert.Equal("1", viewModel.Usages[0].RecordText);
        Assert.Equal("ubCursor", viewModel.Usages[0].FieldPath);
        Assert.Equal("1", viewModel.Usages[0].Value);
        Assert.Same(viewModel.Usages[0], viewModel.SelectedUsage);
    }

    /// <summary>
    /// A report without usages is empty and still exposes the localization texts.
    /// </summary>
    [Fact]
    public void Report_WithoutUsages_IsEmpty()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var report = new LookupUsageReport(
            TableName.Parse("Lookup.Cursor"),
            99,
            UsageCount: 0,
            Array.Empty<LookupUsage>(),
            ScannedTableCount: 1,
            FailedTableCount: 0,
            SuppressedUsageCount: 0);

        var viewModel = new LookupUsageViewModel(report, "Where used");

        Assert.True(viewModel.IsEmpty);
        Assert.Empty(viewModel.Usages);
        Assert.Null(viewModel.SelectedUsage);
        Assert.NotEmpty(viewModel.EmptyText);
        Assert.NotEmpty(viewModel.TableHeader);
        Assert.NotEmpty(viewModel.OpenText);
        Assert.NotEmpty(viewModel.CloseText);
    }

    /// <summary>
    /// Opening the selected usage raises a navigation request with the table and record key.
    /// </summary>
    [Fact]
    public void OpenSelected_RaisesNavigationRequest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var report = new LookupUsageReport(
            TableName.Parse("Lookup.Cursor"),
            1,
            UsageCount: 1,
            new[] { new LookupUsage(TableName.Parse("Items.Items"), "42", "ubCursor", "1") },
            ScannedTableCount: 1,
            FailedTableCount: 0,
            SuppressedUsageCount: 0);
        var viewModel = new LookupUsageViewModel(report, "Where used");
        LookupUsageNavigationRequest? request = null;
        viewModel.NavigationRequested += (_, value) => request = value;

        viewModel.OpenSelected();

        Assert.NotNull(request);
        Assert.Equal("Items.Items", request!.TableName.Value);
        Assert.Equal("42", request.RecordKey);
    }

    /// <summary>
    /// A usage of a record without a key shows a placeholder and still navigates to the table.
    /// </summary>
    [Fact]
    public void Item_WithoutRecordKey_ShowsPlaceholder()
    {
        var item = new LookupUsageItem(new LookupUsage(TableName.Parse("Items.Items"), null, "ubCursor", "1"));

        Assert.Equal("—", item.RecordText);
        Assert.Null(item.RecordKey);
    }
}
