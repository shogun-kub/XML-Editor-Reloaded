using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Lookups;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Navigation request raised when the user opens a record from the lookup usage list.
/// </summary>
/// <param name="TableName">Table that references the lookup.</param>
/// <param name="RecordKey">Record key to select, or <see langword="null"/> when the record has no key.</param>
public sealed record LookupUsageNavigationRequest(TableName TableName, string? RecordKey);

/// <summary>
/// Read-only view model of the "where used" window for one lookup value.
/// </summary>
public sealed partial class LookupUsageViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupUsageViewModel"/> class.
    /// </summary>
    /// <param name="report">Usage report to display.</param>
    /// <param name="title">Window title, including the lookup name and id.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is null.</exception>
    public LookupUsageViewModel(LookupUsageReport report, string title)
    {
        ArgumentNullException.ThrowIfNull(report);

        Title = title;
        Usages = report.Usages.Select(static usage => new LookupUsageItem(usage)).ToArray();
        IsEmpty = report.UsageCount == 0;
        SummaryText = Strings.Format(
            "LookupUsageSummaryFormat",
            report.UsageCount,
            report.Usages.Count,
            report.ScannedTableCount,
            report.FailedTableCount);
        EmptyText = Strings.Get("LookupUsageEmpty");
        TableHeader = Strings.Get("LookupUsageTableColumn");
        RecordHeader = Strings.Get("LookupUsageRecordColumn");
        FieldHeader = Strings.Get("LookupUsageFieldColumn");
        ValueHeader = Strings.Get("LookupUsageValueColumn");
        OpenText = Strings.Get("LookupUsageOpenText");
        CloseText = Strings.Get("LookupUsageCloseText");
        SelectedUsage = Usages.FirstOrDefault();
    }

    /// <summary>
    /// Occurs when the user asks to open the selected usage in the main window.
    /// </summary>
    public event EventHandler<LookupUsageNavigationRequest>? NavigationRequested;

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the usage summary with exact and retained counts.
    /// </summary>
    public string SummaryText { get; }

    /// <summary>
    /// Gets the text shown when the value is not used.
    /// </summary>
    public string EmptyText { get; }

    /// <summary>
    /// Gets a value indicating whether the value is not used anywhere.
    /// </summary>
    public bool IsEmpty { get; }

    /// <summary>
    /// Gets the table column header.
    /// </summary>
    public string TableHeader { get; }

    /// <summary>
    /// Gets the record column header.
    /// </summary>
    public string RecordHeader { get; }

    /// <summary>
    /// Gets the field column header.
    /// </summary>
    public string FieldHeader { get; }

    /// <summary>
    /// Gets the value column header.
    /// </summary>
    public string ValueHeader { get; }

    /// <summary>
    /// Gets the open button text.
    /// </summary>
    public string OpenText { get; }

    /// <summary>
    /// Gets the close button text.
    /// </summary>
    public string CloseText { get; }

    /// <summary>
    /// Gets the retained usages in table and document order.
    /// </summary>
    public IReadOnlyList<LookupUsageItem> Usages { get; }

    /// <summary>
    /// Gets or sets the usage selected in the grid.
    /// </summary>
    [ObservableProperty]
    private LookupUsageItem? _selectedUsage;

    /// <summary>
    /// Raises <see cref="NavigationRequested"/> for the selected usage.
    /// </summary>
    public void OpenSelected()
    {
        if (SelectedUsage is { } usage)
        {
            NavigationRequested?.Invoke(this, new LookupUsageNavigationRequest(usage.TableName, usage.RecordKey));
        }
    }
}

/// <summary>
/// One row of the lookup usage list.
/// </summary>
public sealed class LookupUsageItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupUsageItem"/> class.
    /// </summary>
    /// <param name="usage">Usage to project.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="usage"/> is null.</exception>
    public LookupUsageItem(LookupUsage usage)
    {
        ArgumentNullException.ThrowIfNull(usage);

        TableName = usage.TableName;
        TableText = usage.TableName.Value;
        RecordKey = usage.RecordKey;
        RecordText = usage.RecordKey ?? "—";
        FieldPath = usage.FieldPath;
        Value = usage.Value;
    }

    /// <summary>
    /// Gets the referencing table.
    /// </summary>
    public TableName TableName { get; }

    /// <summary>
    /// Gets the dotted table name shown in the grid.
    /// </summary>
    public string TableText { get; }

    /// <summary>
    /// Gets the record key, or <see langword="null"/> when the record has no declared key.
    /// </summary>
    public string? RecordKey { get; }

    /// <summary>
    /// Gets the record key shown in the grid.
    /// </summary>
    public string RecordText { get; }

    /// <summary>
    /// Gets the field that stores the lookup id or mask.
    /// </summary>
    public string FieldPath { get; }

    /// <summary>
    /// Gets the exact stored value.
    /// </summary>
    public string Value { get; }
}
