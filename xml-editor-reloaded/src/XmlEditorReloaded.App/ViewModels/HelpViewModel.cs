using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Help;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Content of the context-help window: table overview, field reference, search, and related links.
/// </summary>
public sealed partial class HelpViewModel : ObservableObject
{
    private readonly ITableHelpProvider _provider;
    private readonly List<HelpFieldItemViewModel> _allFields = [];
    private TableHelp? _table;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _fieldFilter = string.Empty;

    [ObservableProperty]
    private bool _isSearchMode;

    [ObservableProperty]
    private bool _hasTable;

    [ObservableProperty]
    private bool _hasSearchResults;

    [ObservableProperty]
    private bool _hasSelection;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _tableTitle = string.Empty;

    [ObservableProperty]
    private string _summary = string.Empty;

    [ObservableProperty]
    private string _documentPath = string.Empty;

    [ObservableProperty]
    private string _emptyText;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private HelpFieldItemViewModel? _selectedField;

    [ObservableProperty]
    private HelpSearchItemViewModel? _selectedSearchResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpViewModel"/> class.
    /// </summary>
    /// <param name="provider">Help data used to resolve tables, fields, and search results.</param>
    /// <param name="table">Optional table to show immediately.</param>
    /// <param name="fieldPath">Optional field to select inside <paramref name="table"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provider"/> is null.</exception>
    public HelpViewModel(ITableHelpProvider provider, TableName? table = null, string? fieldPath = null)
    {
        ArgumentNullException.ThrowIfNull(provider);
        _provider = provider;
        Title = Strings.Get("HelpWindowTitle");
        SearchLabel = Strings.Get("HelpSearchLabel");
        SearchButtonText = Strings.Get("HelpSearchButton");
        ClearSearchText = Strings.Get("HelpClearSearch");
        FieldFilterLabel = Strings.Get("HelpFieldFilterLabel");
        FieldsLabel = Strings.Get("HelpFieldsLabel");
        SummaryLabel = Strings.Get("HelpSummaryLabel");
        TableLabel = Strings.Get("HelpTableLabel");
        DocumentLabel = Strings.Get("HelpDocumentLabel");
        TypeLabel = Strings.Get("HelpTypeLabel");
        DescriptionLabel = Strings.Get("HelpDescriptionLabel");
        NumericLabel = Strings.Get("HelpNumericLabel");
        OnlyUsedWithLabel = Strings.Get("HelpOnlyUsedWithLabel");
        ValuesLabel = Strings.Get("HelpValuesLabel");
        DefaultLabel = Strings.Get("HelpDefaultLabel");
        SourceLabel = Strings.Get("HelpSourceLabel");
        ConfidenceLabel = Strings.Get("HelpConfidenceLabel");
        LinksLabel = Strings.Get("HelpLinksLabel");
        SectionsLabel = Strings.Get("HelpSectionsLabel");
        CloseText = Strings.Get("HelpClose");
        OpenResultText = Strings.Get("HelpOpenResult");
        NoResultsText = Strings.Get("HelpNoResults");
        _emptyText = Strings.Get("HelpNoTable");

        if (table is { } initialTable)
        {
            ShowTable(initialTable, fieldPath);
        }
    }

    /// <summary>Gets the field entries currently visible after filtering.</summary>
    public ObservableCollection<HelpFieldItemViewModel> Fields { get; } = [];

    /// <summary>Gets the current search results.</summary>
    public ObservableCollection<HelpSearchItemViewModel> SearchResults { get; } = [];

    /// <summary>Gets the related-document links of the current table.</summary>
    public ObservableCollection<HelpLinkItemViewModel> Links { get; } = [];

    /// <summary>Gets the prose sections of the current document.</summary>
    public ObservableCollection<HelpSectionItemViewModel> Sections { get; } = [];

    /// <summary>Gets the search label.</summary>
    public string SearchLabel { get; }

    /// <summary>Gets the search button text.</summary>
    public string SearchButtonText { get; }

    /// <summary>Gets the clear-search text.</summary>
    public string ClearSearchText { get; }

    /// <summary>Gets the field-filter label.</summary>
    public string FieldFilterLabel { get; }

    /// <summary>Gets the fields heading.</summary>
    public string FieldsLabel { get; }

    /// <summary>Gets the summary heading.</summary>
    public string SummaryLabel { get; }

    /// <summary>Gets the table heading.</summary>
    public string TableLabel { get; }

    /// <summary>Gets the document heading.</summary>
    public string DocumentLabel { get; }

    /// <summary>Gets the type heading.</summary>
    public string TypeLabel { get; }

    /// <summary>Gets the description heading.</summary>
    public string DescriptionLabel { get; }

    /// <summary>Gets the numeric-semantics heading.</summary>
    public string NumericLabel { get; }

    /// <summary>Gets the option-gate heading.</summary>
    public string OnlyUsedWithLabel { get; }

    /// <summary>Gets the values heading.</summary>
    public string ValuesLabel { get; }

    /// <summary>Gets the default heading.</summary>
    public string DefaultLabel { get; }

    /// <summary>Gets the source heading.</summary>
    public string SourceLabel { get; }

    /// <summary>Gets the confidence heading.</summary>
    public string ConfidenceLabel { get; }

    /// <summary>Gets the related-links heading.</summary>
    public string LinksLabel { get; }

    /// <summary>Gets the sections heading.</summary>
    public string SectionsLabel { get; }

    /// <summary>Gets the close button text.</summary>
    public string CloseText { get; }

    /// <summary>Gets the open-result button text.</summary>
    public string OpenResultText { get; }

    /// <summary>Gets the empty-search result text.</summary>
    public string NoResultsText { get; }

    /// <summary>Gets a value indicating whether the current table has related links.</summary>
    public bool HasLinks => Links.Count > 0;

    /// <summary>Gets a value indicating whether the current table has prose sections.</summary>
    public bool HasSections => Sections.Count > 0;

    /// <summary>Gets a value indicating whether the current table has visible fields.</summary>
    public bool HasFields => Fields.Count > 0;

    /// <summary>Gets a value indicating whether search mode is active.</summary>
    public bool IsTableMode => !IsSearchMode && HasTable;

    /// <summary>
    /// Shows a table family and optionally selects one of its fields.
    /// </summary>
    /// <param name="table">Dotted table name.</param>
    /// <param name="fieldPath">Optional full field path or unique leaf name.</param>
    public void ShowTable(TableName table, string? fieldPath = null)
    {
        var help = _provider.GetTableHelp(table);
        if (help is null)
        {
            _table = null;
            HasTable = false;
            IsSearchMode = false;
            _allFields.Clear();
            Fields.Clear();
            Links.Clear();
            Sections.Clear();
            TableTitle = table.Value;
            Summary = string.Empty;
            DocumentPath = string.Empty;
            EmptyText = Strings.Format("HelpTableNotFoundFormat", table.Value);
            StatusText = EmptyText;
            SelectedField = null;
            OnCollectionStateChanged();
            return;
        }

        _table = help;
        HasTable = true;
        IsSearchMode = false;
        SearchText = string.Empty;
        FieldFilter = string.Empty;
        TableTitle = help.Tables.Count > 1
            ? Strings.Format("HelpTableAliasesFormat", help.Table.Value, help.Tables.Count - 1)
            : help.Table.Value;
        Summary = help.Summary;
        DocumentPath = help.DocumentPath;
        StatusText = string.Empty;

        _allFields.Clear();
        foreach (var field in help.Fields.Values.OrderBy(static field => field.Path, StringComparer.Ordinal))
        {
            _allFields.Add(new HelpFieldItemViewModel(field));
        }

        RebuildFields();
        Links.Clear();
        foreach (var link in help.Links)
        {
            Links.Add(new HelpLinkItemViewModel(link));
        }

        Sections.Clear();
        foreach (var section in help.Sections)
        {
            Sections.Add(new HelpSectionItemViewModel(section));
        }

        var requested = fieldPath is null ? null : ResolveField(fieldPath);
        SelectedField = requested ?? Fields.FirstOrDefault();
        OnCollectionStateChanged();
    }

    /// <summary>Gets the currently displayed table, or <see langword="null"/> for the empty state.</summary>
    public TableName? Table => _table?.Table;

    /// <summary>
    /// Runs the global help search for <see cref="SearchText"/>.
    /// </summary>
    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            ClearSearch();
            return;
        }

        SearchResults.Clear();
        foreach (var result in _provider.Search(SearchText, 50))
        {
            SearchResults.Add(new HelpSearchItemViewModel(result));
        }

        IsSearchMode = true;
        HasSearchResults = SearchResults.Count > 0;
        StatusText = HasSearchResults
            ? Strings.Format("HelpSearchCountFormat", SearchResults.Count)
            : NoResultsText;
        SelectedSearchResult = SearchResults.FirstOrDefault();
    }

    /// <summary>
    /// Leaves search mode and restores the field list of the current table.
    /// </summary>
    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
        SearchResults.Clear();
        SelectedSearchResult = null;
        HasSearchResults = false;
        IsSearchMode = false;
        StatusText = string.Empty;
    }

    /// <summary>
    /// Opens a search result and selects its table and field.
    /// </summary>
    /// <param name="result">Search result selected by the user.</param>
    [RelayCommand]
    private void OpenSearchResult(HelpSearchItemViewModel? result)
    {
        if (result is null)
        {
            return;
        }

        ShowTable(result.Table, result.FieldPath);
    }

    /// <summary>
    /// Opens a related document when it belongs to a documented table.
    /// </summary>
    /// <param name="link">Related-document link selected by the user.</param>
    [RelayCommand]
    private void OpenLink(HelpLinkItemViewModel? link)
    {
        if (link is null)
        {
            return;
        }

        var help = _provider.GetTableHelpByDocument(link.Target);
        if (help is null)
        {
            StatusText = Strings.Format("HelpLinkUnavailableFormat", link.Target);
            return;
        }

        ShowTable(help.Table);
    }

    partial void OnFieldFilterChanged(string value) => RebuildFields();

    partial void OnSelectedFieldChanged(HelpFieldItemViewModel? value) => HasSelection = value is not null;

    partial void OnIsSearchModeChanged(bool value)
    {
        OnPropertyChanged(nameof(IsTableMode));
        OnCollectionStateChanged();
    }

    private HelpFieldItemViewModel? ResolveField(string fieldPath)
    {
        if (_table is null)
        {
            return null;
        }

        var help = _provider.GetFieldHelp(_table.Table, fieldPath);
        return help is null ? null : Fields.FirstOrDefault(field => field.Path == help.Path);
    }

    private void RebuildFields()
    {
        var filter = FieldFilter.Trim();
        var selected = SelectedField;
        Fields.Clear();
        foreach (var field in _allFields)
        {
            if (filter.Length == 0
                || field.Path.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || field.Description.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                Fields.Add(field);
            }
        }

        OnPropertyChanged(nameof(HasFields));
        if (selected is not null && Fields.Contains(selected))
        {
            SelectedField = selected;
        }
        else if (selected is not null)
        {
            SelectedField = Fields.FirstOrDefault();
        }
    }

    private void OnCollectionStateChanged()
    {
        OnPropertyChanged(nameof(HasFields));
        OnPropertyChanged(nameof(HasLinks));
        OnPropertyChanged(nameof(HasSections));
        OnPropertyChanged(nameof(IsTableMode));
    }
}

/// <summary>
/// One field entry shown in the help window.
/// </summary>
public sealed class HelpFieldItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpFieldItemViewModel"/> class.
    /// </summary>
    /// <param name="field">Field help to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="field"/> is null.</exception>
    public HelpFieldItemViewModel(FieldHelp field)
    {
        ArgumentNullException.ThrowIfNull(field);
        Field = field;
        var separator = field.Path.LastIndexOf('/');
        Name = separator < 0 ? field.Path : field.Path[(separator + 1)..];
    }

    /// <summary>Gets the wrapped field help.</summary>
    public FieldHelp Field { get; }

    /// <summary>Gets the full field path.</summary>
    public string Path => Field.Path;

    /// <summary>Gets the leaf field name shown in the list.</summary>
    public string Name { get; }

    /// <summary>Gets the declared type.</summary>
    public string Type => Field.Type;

    /// <summary>Gets the description.</summary>
    public string Description => Field.Description;

    /// <summary>Gets numeric semantics, when documented.</summary>
    public string? NumericSemantics => Field.NumericSemantics;

    /// <summary>Gets the option or condition that gates the field, when documented.</summary>
    public string? OnlyUsedWith => Field.OnlyUsedWith;

    /// <summary>Gets accepted values, when documented.</summary>
    public string? Values => Field.Values;

    /// <summary>Gets default behaviour, when documented.</summary>
    public string? Default => Field.Default;

    /// <summary>Gets the evidence source.</summary>
    public string Source => Field.Source;

    /// <summary>Gets the confidence label.</summary>
    public string Confidence => Field.Confidence;

    /// <summary>Gets a value indicating whether numeric semantics are available.</summary>
    public bool HasNumericSemantics => !string.IsNullOrWhiteSpace(NumericSemantics);

    /// <summary>Gets a value indicating whether an option gate is available.</summary>
    public bool HasOnlyUsedWith => !string.IsNullOrWhiteSpace(OnlyUsedWith);

    /// <summary>Gets a value indicating whether accepted values are available.</summary>
    public bool HasValues => !string.IsNullOrWhiteSpace(Values);

    /// <summary>Gets a value indicating whether default behaviour is available.</summary>
    public bool HasDefault => !string.IsNullOrWhiteSpace(Default);
}

/// <summary>
/// One help search result shown in the help window.
/// </summary>
public sealed class HelpSearchItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpSearchItemViewModel"/> class.
    /// </summary>
    /// <param name="result">Search result to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is null.</exception>
    public HelpSearchItemViewModel(HelpSearchResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        Result = result;
    }

    /// <summary>Gets the wrapped search result.</summary>
    public HelpSearchResult Result { get; }

    /// <summary>Gets the table that owns the result.</summary>
    public TableName Table => Result.Table;

    /// <summary>Gets the matching field path, or <see langword="null"/> for a document match.</summary>
    public string? FieldPath => Result.FieldPath;

    /// <summary>Gets the display title.</summary>
    public string Title => Result.Title;

    /// <summary>Gets the excerpt that explains the match.</summary>
    public string Snippet => Result.Snippet;
}

/// <summary>
/// One related-document link shown in the help window.
/// </summary>
public sealed class HelpLinkItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpLinkItemViewModel"/> class.
    /// </summary>
    /// <param name="link">Link to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="link"/> is null.</exception>
    public HelpLinkItemViewModel(HelpLink link)
    {
        ArgumentNullException.ThrowIfNull(link);
        Link = link;
        DisplayName = Path.GetFileNameWithoutExtension(link.Target);
    }

    /// <summary>Gets the wrapped link.</summary>
    public HelpLink Link { get; }

    /// <summary>Gets the target path relative to the help root.</summary>
    public string Target => Link.Target;

    /// <summary>Gets the display name of the target document.</summary>
    public string DisplayName { get; }
}

/// <summary>
/// One prose section shown in the help window.
/// </summary>
public sealed class HelpSectionItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpSectionItemViewModel"/> class.
    /// </summary>
    /// <param name="section">Section to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is null.</exception>
    public HelpSectionItemViewModel(HelpSection section)
    {
        ArgumentNullException.ThrowIfNull(section);
        Section = section;
    }

    /// <summary>Gets the wrapped section.</summary>
    public HelpSection Section { get; }

    /// <summary>Gets the section heading.</summary>
    public string Heading => Section.Heading;

    /// <summary>Gets the section body in Markdown.</summary>
    public string Text => Section.Text;
}
