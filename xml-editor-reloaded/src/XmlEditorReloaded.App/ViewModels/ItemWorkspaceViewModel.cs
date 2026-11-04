using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Filtering;
using XmlEditorReloaded.Core.Imaging;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Core.Validation;
using XmlEditorReloaded.Xml.Documents;
using XmlEditorReloaded.Xml.Filtering;
using XmlEditorReloaded.Xml.Schema;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Item master-detail workspace: an item selector, the editable item form, its 1:1 sub-tables, its 1:N
/// child tables, nested modifiers, repeated fields, localization, and item images, all over one shared
/// document set with a single undo history and one save action.
/// </summary>
public sealed partial class ItemWorkspaceViewModel :
    ObservableObject,
    IEditorTab,
    ITableCellEditor,
    IDetailFieldHost
{
    private const string KeyField = "uiIndex";
    private const string NameField = "szItemName";
    private const string ClassField = "usItemClass";
    private const string GraphicTypeField = "ubGraphicType";
    private const string GraphicIndexField = "ubGraphicNum";
    private const string ClassLookup = "Lookup.ItemClass";

    private static readonly NumberStyles DecimalStyles =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    private readonly IDataSetSession _session;
    private readonly IDialogService _dialogService;
    private readonly IItemImageService _itemImages;
    private readonly TableDescriptor _descriptor;
    private readonly Dictionary<TableName, FieldSourceSet> _fieldSources = new();
    private readonly Dictionary<TableRowViewModel, TableName> _rowOwners = new();
    private readonly List<ItemListEntry> _allItems = new();
    private readonly List<DetailFieldViewModel> _allFields = new();
    private readonly Dictionary<ulong, string> _classNames = new();
    private readonly Dictionary<ulong, string> _classExactNames = new();
    private IReadOnlyList<LanguageDescriptor> _languages = Array.Empty<LanguageDescriptor>();
    private TableSetWorkspace? _set;
    private TableRowViewModel? _itemRow;
    private string? _languageCode;
    private bool _suppressLanguageChange;
    private bool _suppressGraphicType;
    private bool _suppressGraphicImage;
    private bool _graphicImageListLoading;
    private bool _suppressSelection;
    private bool _suppressItemFilter;
    private bool _isReloading;
    private CancellationTokenSource? _imageCts;
    private CancellationTokenSource? _imageListCts;
    private string? _pendingSelection;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private bool _isBusy;

    private bool _isDirty;

    [ObservableProperty]
    private bool _isEditable;

    [ObservableProperty]
    private bool _canUndo;

    [ObservableProperty]
    private bool _canRedo;

    [ObservableProperty]
    private bool _isValidationVisible;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _fieldFilter = string.Empty;

    [ObservableProperty]
    private bool _isItemFilterVisible;

    [ObservableProperty]
    private ItemListFilterOption? _selectedClassFilter;

    [ObservableProperty]
    private ItemListFilterOption? _selectedFlagFilter;

    [ObservableProperty]
    private TableFilterViewModel? _customItemFilter;

    [ObservableProperty]
    private string _itemIdText = string.Empty;

    [ObservableProperty]
    private string _goToIdText = string.Empty;

    [ObservableProperty]
    private string _countText = string.Empty;

    [ObservableProperty]
    private string _countShortText = string.Empty;

    [ObservableProperty]
    private string _languageText = string.Empty;

    [ObservableProperty]
    private string _warningsText = string.Empty;

    [ObservableProperty]
    private string _detailHeader = string.Empty;

    [ObservableProperty]
    private string _itemClassText = string.Empty;

    [ObservableProperty]
    private bool _isTranslationMode;

    [ObservableProperty]
    private ItemListEntry? _selectedItem;

    [ObservableProperty]
    private GraphicTypeOption? _selectedGraphicType;

    [ObservableProperty]
    private ItemImageOption? _selectedGraphicImage;

    [ObservableProperty]
    private int _graphicIndexMaximum = -1;

    [ObservableProperty]
    private string _graphicImageListInfo = string.Empty;

    [ObservableProperty]
    private LanguageOptionViewModel? _selectedLanguageOption;

    [ObservableProperty]
    private ShopkeeperDealerOptionViewModel? _selectedShopkeeperDealer;

    [ObservableProperty]
    private string _newShopkeeperQuantity = "0";

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemWorkspaceViewModel"/> class.
    /// </summary>
    /// <param name="session">Data-set session that opens the item set.</param>
    /// <param name="descriptor">Descriptor of the <c>Items.Items</c> table.</param>
    /// <param name="languageCode">Optional language overlay code.</param>
    /// <param name="dialogService">Dialog service used for confirmations and the bit editor.</param>
    /// <param name="itemImages">Item image service used by the previews.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required service or descriptor is null.</exception>
    public ItemWorkspaceViewModel(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode,
        IDialogService dialogService,
        IItemImageService itemImages)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(itemImages);

        _session = session;
        _descriptor = descriptor;
        _languageCode = languageCode;
        _dialogService = dialogService;
        _itemImages = itemImages;

        Title = descriptor.TableName.Value.Split('.').Last();
        _statusText = Strings.Get("EditorLoading");

        SaveText = Strings.Get("EditorSaveText");
        UndoText = Strings.Get("EditorUndoText");
        RedoText = Strings.Get("EditorRedoText");
        UndoButtonTooltip = Strings.Get("EditorUndoButtonTooltip");
        RedoButtonTooltip = Strings.Get("EditorRedoButtonTooltip");
        ValidateText = Strings.Get("EditorValidateText");
        ReloadText = Strings.Get("EditorReloadText");
        NewItemText = Strings.Get("ItemNewText");
        DuplicateItemText = Strings.Get("EditorDuplicateRowText");
        DeleteItemText = Strings.Get("EditorDeleteRowText");
        ChangeIdText = Strings.Get("ItemChangeIdText");
        GoToIdLabel = Strings.Get("ItemGoToIdText");
        SearchLabel = Strings.Get("ItemSearchLabel");
        LanguageLabel = Strings.Get("EditorLanguageLabel");
        DetailFilterLabel = Strings.Get("DetailFilterLabel");
        ValidationLabel = Strings.Get("EditorValidationLabel");
        IdLabel = Strings.Get("ItemIdLabel");
        NameLabel = Strings.Get("ItemNameLabel");
        ClassLabel = Strings.Get("ItemClassLabel");
        GraphicTypeLabel = Strings.Get("ItemGraphicTypeLabel");
        GraphicImageListLabel = Strings.Get("ItemImageListLabel");
        CompatibleItemText = Strings.Get("ItemCompatibleOnly");
        FieldsTabText = Strings.Get("ItemTabFields");
        SubTablesTabText = Strings.Get("ItemTabSubTables");
        ChildTablesTabText = Strings.Get("ItemTabChildTables");
        ImagesTabText = Strings.Get("ItemTabImages");
        NestedTabText = Strings.Get("ItemTabNested");
        ShopkeeperTabText = Strings.Get("ItemTabShopkeepers");
        ShopkeeperDealerColumn = Strings.Get("ShopkeeperDealerColumn");
        ShopkeeperQuantityColumn = Strings.Get("ShopkeeperQuantityColumn");
        ShopkeeperEmptyText = Strings.Get("ShopkeeperEmptyText");
        ShopkeeperAddDealerLabel = Strings.Get("ShopkeeperAddDealerLabel");
        ShopkeeperAddText = Strings.Get("ShopkeeperAddText");
        ShopkeeperRemoveText = Strings.Get("ShopkeeperRemoveText");
        ShopkeeperNoDealersText = Strings.Get("ShopkeeperNoDealersText");
        ShopkeeperRemoveTitle = Strings.Get("ShopkeeperRemoveTitle");
        ItemFilterLabel = Strings.Get("ItemFilterLabel");
        ClassFilterLabel = Strings.Get("ItemFilterClassLabel");
        FlagFilterLabel = Strings.Get("ItemFilterFlagLabel");
        ItemListFilterLabel = Strings.Get("ItemListFilterLabel");
        ItemFilterClearText = Strings.Get("ItemFilterClearText");
        FilterAddText = Strings.Get("EditorFilterAdd");
        FilterClearText = Strings.Get("EditorFilterClear");
        FilterRemoveText = Strings.Get("EditorFilterRemoveSymbol");

        DetailImages = new[]
        {
            new DetailImagePanelViewModel(ImageSize.Big, Strings.Get("ImageBigLabel"), 170),
            new DetailImagePanelViewModel(ImageSize.Medium, Strings.Get("ImageMediumLabel"), 90),
            new DetailImagePanelViewModel(ImageSize.Small, Strings.Get("ImageSmallLabel"), 50),
        };
    }

    /// <inheritdoc />
    public TableName TableName => _descriptor.TableName;

    /// <inheritdoc />
    public string TableDataDirectory => _session.TableDataDirectory;

    /// <inheritdoc />
    public string DatasetLabel => DatasetNames.LabelFor(_session);

    /// <inheritdoc />
    public string CreateXmlSnapshot() => _set?.Primary.BaseDocument.CreateXmlSnapshot() ?? string.Empty;

    /// <inheritdoc />
    public string? SelectedRecordKey => SelectedItem?.Key;

    /// <inheritdoc />
    public string Title { get; }

    /// <inheritdoc />
    public string DisplayName => IsDirty ? Title + " *" : Title;

    /// <inheritdoc />
    public bool IsDirty
    {
        get => _isDirty;
        private set
        {
            if (SetProperty(ref _isDirty, value))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the item set accepts edits.
    /// </summary>
    public bool CanEdit => IsEditable;

    /// <summary>
    /// Gets a value indicating whether the language selector can be used.
    /// </summary>
    public bool IsLanguageSelectorEnabled => !IsBusy && _set is not null;

    /// <summary>
    /// Gets the save button text.
    /// </summary>
    public string SaveText { get; }

    /// <summary>
    /// Gets the undo button text.
    /// </summary>
    public string UndoText { get; }

    /// <summary>
    /// Gets the redo button text.
    /// </summary>
    public string RedoText { get; }

    /// <summary>
    /// Gets the undo button tooltip.
    /// </summary>
    public string UndoButtonTooltip { get; }

    /// <summary>
    /// Gets the redo button tooltip.
    /// </summary>
    public string RedoButtonTooltip { get; }

    /// <summary>
    /// Gets the validate button text.
    /// </summary>
    public string ValidateText { get; }

    /// <summary>
    /// Gets the reload button text.
    /// </summary>
    public string ReloadText { get; }

    /// <summary>
    /// Gets the new-item button text.
    /// </summary>
    public string NewItemText { get; }

    /// <summary>
    /// Gets the duplicate-item button text.
    /// </summary>
    public string DuplicateItemText { get; }

    /// <summary>
    /// Gets the delete-item button text.
    /// </summary>
    public string DeleteItemText { get; }

    /// <summary>
    /// Gets the change-id button text.
    /// </summary>
    public string ChangeIdText { get; }

    /// <summary>
    /// Gets the go-to-id button text.
    /// </summary>
    public string GoToIdLabel { get; }

    /// <summary>
    /// Gets the item search label.
    /// </summary>
    public string SearchLabel { get; }

    /// <summary>
    /// Gets the language selector label.
    /// </summary>
    public string LanguageLabel { get; }

    /// <summary>
    /// Gets the field filter label.
    /// </summary>
    public string DetailFilterLabel { get; }

    /// <summary>
    /// Gets the validation panel label.
    /// </summary>
    public string ValidationLabel { get; }

    /// <summary>
    /// Gets the item identifier label.
    /// </summary>
    public string IdLabel { get; }

    /// <summary>
    /// Gets the item name label.
    /// </summary>
    public string NameLabel { get; }

    /// <summary>
    /// Gets the item class label.
    /// </summary>
    public string ClassLabel { get; }

    /// <summary>
    /// Gets the graphic type label.
    /// </summary>
    public string GraphicTypeLabel { get; }

    /// <summary>
    /// Gets the label above the image chooser.
    /// </summary>
    public string GraphicImageListLabel { get; }

    /// <summary>
    /// Gets the note that list-field pickers show only attachment items.
    /// </summary>
    public string CompatibleItemText { get; }

    /// <summary>
    /// Gets the fields tab header.
    /// </summary>
    public string FieldsTabText { get; }

    /// <summary>
    /// Gets the sub-tables tab header.
    /// </summary>
    public string SubTablesTabText { get; }

    /// <summary>
    /// Gets the child-tables tab header.
    /// </summary>
    public string ChildTablesTabText { get; }

    /// <summary>
    /// Gets the images tab header.
    /// </summary>
    public string ImagesTabText { get; }

    /// <summary>
    /// Gets the nested-structures tab header.
    /// </summary>
    public string NestedTabText { get; }

    /// <summary>
    /// Gets the shopkeeper-inventory tab header.
    /// </summary>
    public string ShopkeeperTabText { get; }

    /// <summary>
    /// Gets the dealer column header of the shopkeeper list.
    /// </summary>
    public string ShopkeeperDealerColumn { get; }

    /// <summary>
    /// Gets the quantity column header of the shopkeeper list.
    /// </summary>
    public string ShopkeeperQuantityColumn { get; }

    /// <summary>
    /// Gets the note shown when no dealer stocks the selected item.
    /// </summary>
    public string ShopkeeperEmptyText { get; }

    /// <summary>
    /// Gets the label of the control that adds the item to another dealer.
    /// </summary>
    public string ShopkeeperAddDealerLabel { get; }

    /// <summary>
    /// Gets the text of the button that adds a dealer row.
    /// </summary>
    public string ShopkeeperAddText { get; }

    /// <summary>
    /// Gets the text of the button that removes a dealer row.
    /// </summary>
    public string ShopkeeperRemoveText { get; }

    /// <summary>
    /// Gets the note shown when every loaded dealer already stocks the selected item.
    /// </summary>
    public string ShopkeeperNoDealersText { get; }

    /// <summary>
    /// Gets the confirmation-question title of a dealer row removal.
    /// </summary>
    public string ShopkeeperRemoveTitle { get; }

    /// <summary>
    /// Gets the field filter label.
    /// </summary>
    public string ItemFilterLabel { get; }

    /// <summary>
    /// Gets the item list class filter label.
    /// </summary>
    public string ClassFilterLabel { get; }

    /// <summary>
    /// Gets the item list flag filter label.
    /// </summary>
    public string FlagFilterLabel { get; }

    /// <summary>
    /// Gets the custom item filter expander header.
    /// </summary>
    public string ItemListFilterLabel { get; }

    /// <summary>
    /// Gets the text of the command that clears every item list filter.
    /// </summary>
    public string ItemFilterClearText { get; }

    /// <summary>
    /// Gets the add-criterion button text of the custom item filter.
    /// </summary>
    public string FilterAddText { get; }

    /// <summary>
    /// Gets the clear-criteria button text of the custom item filter.
    /// </summary>
    public string FilterClearText { get; }

    /// <summary>
    /// Gets the symbol shown on the button that removes one custom filter criterion.
    /// </summary>
    public string FilterRemoveText { get; }

    /// <summary>
    /// Gets a value indicating whether any item list filter restricts the selector.
    /// </summary>
    public bool HasActiveItemFilters =>
        SelectedClassFilter?.Filter.IsEmpty == false
        || SelectedFlagFilter?.Filter.IsEmpty == false
        || CustomItemFilter?.ToGroup().IsEmpty == false;

    /// <summary>
    /// Gets the class filter options in legacy menu order.
    /// </summary>
    public ObservableCollection<ItemListFilterOption> ClassFilterOptions { get; } = new();

    /// <summary>
    /// Gets the item-flag filter options in legacy menu order.
    /// </summary>
    public ObservableCollection<ItemListFilterOption> FlagFilterOptions { get; } = new();

    /// <summary>
    /// Gets the big, medium, and small image preview panels.
    /// </summary>
    public IReadOnlyList<DetailImagePanelViewModel> DetailImages { get; }

    /// <summary>
    /// Gets the items shown in the selector, filtered by <see cref="SearchText"/>, the class and flag
    /// presets, and the custom structured filter.
    /// </summary>
    public ObservableCollection<ItemListEntry> Items { get; } = new();

    /// <summary>
    /// Gets the item fields grouped by the schema group hints.
    /// </summary>
    public ObservableCollection<DetailFieldGroupViewModel> ItemGroups { get; } = new();

    /// <summary>
    /// Gets the editable nested structures of the item, for example the stance modifiers.
    /// </summary>
    public ObservableCollection<DetailFieldGroupViewModel> NestedGroups { get; } = new();

    /// <summary>
    /// Gets the 1:1 sub-table sections of the selected item.
    /// </summary>
    public ObservableCollection<ItemSubTableSectionViewModel> SubTableSections { get; } = new();

    /// <summary>
    /// Gets the repeated-field sections of the selected item.
    /// </summary>
    public ObservableCollection<ItemListSectionViewModel> ListSections { get; } = new();

    /// <summary>
    /// Gets the 1:N child-table sections of the item workspace.
    /// </summary>
    public ObservableCollection<ItemChildSectionViewModel> ChildSections { get; } = new();

    /// <summary>
    /// Gets the child-table sections that the selected item class shows. Binding the tab control to
    /// this collection instead of <see cref="ChildSections"/> prevents a hidden section from staying
    /// selected and rendering its grid.
    /// </summary>
    public ObservableCollection<ItemChildSectionViewModel> VisibleChildSections { get; } = new();

    /// <summary>
    /// Gets the dealer inventory rows that stock the selected item, with their quantities.
    /// </summary>
    public ObservableCollection<ShopkeeperEntryViewModel> ShopkeeperEntries { get; } = new();

    /// <summary>
    /// Gets the dealer inventories that could stock the selected item but do not have a row for it yet.
    /// </summary>
    public ObservableCollection<ShopkeeperDealerOptionViewModel> ShopkeeperDealerOptions { get; } = new();

    /// <summary>
    /// Gets a value indicating whether at least one dealer stocks the selected item.
    /// </summary>
    public bool HasShopkeeperEntries => ShopkeeperEntries.Count > 0;

    /// <summary>
    /// Gets a value indicating whether at least one dealer could still receive a row for the selected item.
    /// </summary>
    public bool HasShopkeeperDealerOptions => ShopkeeperDealerOptions.Count > 0;

    /// <summary>
    /// Gets a value indicating whether the shopkeeper add-row bar applies to a selected item.
    /// </summary>
    public bool CanShowShopkeeperAddBar => SelectedItem is not null;

    /// <summary>
    /// Gets the shopkeeper count text shown above the dealer list.
    /// </summary>
    public string ShopkeeperCountText =>
        Strings.Format("ShopkeeperCountFormat", ShopkeeperEntries.Count);

    /// <summary>
    /// Gets the selectable item graphic types.
    /// </summary>
    public ObservableCollection<GraphicTypeOption> GraphicTypes { get; } = new();

    /// <summary>
    /// Gets the frames offered for the selected graphic type. Entries are created from the small sheet
    /// length, so the list exposes exactly the valid <c>ubGraphicNum</c> range.
    /// </summary>
    public ObservableCollection<ItemImageOption> GraphicImages { get; } = new();

    /// <summary>
    /// Gets the language selector options: the base table and existing overlays.
    /// </summary>
    public ObservableCollection<LanguageOptionViewModel> LanguageOptions { get; } = new();

    /// <summary>
    /// Gets validation issues from the last validation run of the whole item set.
    /// </summary>
    public ObservableCollection<ValidationIssueItem> ValidationIssues { get; } = new();

    /// <summary>
    /// Gets the field names offered by the selected item's class, for tests and diagnostics.
    /// </summary>
    public IReadOnlyList<string> ActiveSubTableNames =>
        SubTableSections.Select(static section => section.Member.Relation.TargetTable.Value).ToArray();

    /// <inheritdoc />
    public event EventHandler<string?>? LanguageChanged;

    /// <inheritdoc />
    public event EventHandler<TableNavigationRequest>? TableNavigationRequested;

    /// <inheritdoc />
    public Task InitializeAsync(CancellationToken cancellationToken = default) =>
        InitializeCoreAsync(cancellationToken);

    /// <summary>
    /// Loads the item set, its field sources, and the embedded child grids.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the load.</param>
    /// <returns>A task that completes when the workspace is ready.</returns>
    private async Task InitializeCoreAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        StatusText = Strings.Get("EditorLoading");
        var selectedKey = _pendingSelection ?? _itemRow?.Key;
        try
        {
            var languagesResult = await _session.GetLanguagesAsync(cancellationToken).ConfigureAwait(true);
            if (languagesResult.IsSuccess)
            {
                _languages = languagesResult.Data!;
            }

            var load = await _session
                .OpenTableSetAsync(
                    _descriptor,
                    _languageCode,
                    includeReverseRelations: true,
                    cancellationToken)
                .ConfigureAwait(true);
            if (!load.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{load.ErrorCode}: {load.ErrorText}");
                IsEditable = false;
                return;
            }

            if (_set is not null)
            {
                _set.History.Changed -= OnHistoryChanged;
            }

            _set = load.Data!;
            _set.History.Changed += OnHistoryChanged;
            _languageCode = _set.Primary.Language?.Code;
            IsTranslationMode = _set.Primary.Language is not null;
            LanguageText = _set.Primary.Language is null
                ? Strings.Get("EditorBaseLanguage")
                : Strings.Format("EditorLanguageFormat", _set.Primary.Language.DisplayName);
            WarningsText = string.Join(Environment.NewLine, _set.Warnings);
            IsEditable = _set.CanEdit;

            await LoadClassNamesAsync(cancellationToken).ConfigureAwait(true);
            await LoadFieldSourcesAsync(cancellationToken).ConfigureAwait(true);
            await InitializeChildEditorsAsync(cancellationToken).ConfigureAwait(true);
            BuildGraphicTypes();
            BuildLanguageOptions();
            RebuildItemList(selectedKey);

            if (_set.Primary.Metadata is null)
            {
                StatusText = Strings.Get("ItemWorkspaceNoMetadata");
            }
            else
            {
                StatusText = Strings.Get("EditorReady");
            }
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
            IsEditable = false;
        }
        finally
        {
            _pendingSelection = null;
            RefreshHistoryState();
            IsDirty = _set?.IsDirty == true;
            IsBusy = false;
        }
    }

    /// <summary>
    /// Saves every dirty file of the item set.
    /// </summary>
    /// <returns>A task that completes when saving finishes.</returns>
    [RelayCommand]
    public async Task SaveAsync() => await SaveWithRecoveryAsync().ConfigureAwait(true);

    /// <inheritdoc />
    public async Task<bool> SaveWithRecoveryAsync()
    {
        if (_set is null)
        {
            return false;
        }

        if (!_set.IsDirty)
        {
            IsDirty = false;
            StatusText = Strings.Get("EditorNoChanges");
            return true;
        }

        IsBusy = true;
        try
        {
            var report = await _set.SaveDetailedAsync().ConfigureAwait(true);
            if (report.IsSuccess)
            {
                ApplySavedState();
                return true;
            }

            if (report.HasExternalChanges)
            {
                return await ResolveExternalChangeAsync(report).ConfigureAwait(true);
            }

            StatusText = Strings.Format("StatusError", FormatReport(report));
            return false;
        }
        catch (OperationCanceledException)
        {
            StatusText = Strings.Get("EditorLoadingCancelled");
            return false;
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <inheritdoc />
    public Task ReloadFromDiskAsync() => ReloadPreservingSelectionAsync();

    private async Task<bool> ResolveExternalChangeAsync(TableSetSaveReport failedReport)
    {
        var tables = string.Join(
            ", ",
            failedReport.Failures
                .Where(static failure => string.Equals(
                    failure.ErrorCode,
                    ErrorCodes.XmlFileChangedExternally,
                    StringComparison.Ordinal))
                .Select(static failure => failure.Table.Value));
        var choice = _dialogService.ResolveExternalChange(
            Strings.Format(
                "ExternalChangeMessageFormat",
                tables,
                string.Join(
                    Environment.NewLine,
                    failedReport.Failures.Select(static failure =>
                        $"{failure.Table}: {failure.ErrorCode}: {failure.ErrorText}"))),
            Strings.Get("ExternalChangeTitle"));
        switch (choice)
        {
            case ExternalChangeChoice.Reload:
                await ReloadFromDiskAsync().ConfigureAwait(true);
                if (_set is null)
                {
                    return false;
                }

                StatusText = Strings.Format("ExternalChangeReloaded", Title);
                return !IsDirty;

            case ExternalChangeChoice.Overwrite:
                if (_set is null)
                {
                    return false;
                }

                var forced = await _set.SaveDetailedAsync(forceOverwrite: true).ConfigureAwait(true);
                if (forced.IsSuccess)
                {
                    ApplySavedState();
                    StatusText = Strings.Format("ExternalChangeOverwritten", Title);
                    return true;
                }

                StatusText = Strings.Format("StatusError", FormatReport(forced));
                return false;

            default:
                StatusText = Strings.Format("ExternalChangeCancelled", Title);
                return false;
        }
    }

    private static string FormatReport(TableSetSaveReport report) =>
        string.Join(
            "; ",
            report.Failures.Select(static failure =>
                $"{failure.Table}: {failure.ErrorCode}: {failure.ErrorText}"));

    private void ApplySavedState()
    {
        if (_set is null)
        {
            return;
        }

        foreach (var tableName in new[] { _descriptor.TableName }
                     .Concat(_set.Members.Select(static member => member.Workspace.Descriptor.TableName)))
        {
            _session.RelationResolver.Invalidate(tableName);
        }

        IsDirty = false;
        StatusText = Strings.Get("EditorSaved");
        foreach (var section in ChildSections)
        {
            section.Editor.RefreshFieldValues();
        }
    }

    /// <summary>
    /// Reverses the most recent edit of the whole item set.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUndo))]
    public void Undo() => ApplyHistory(undo: true);

    /// <summary>
    /// Restores the most recently undone edit of the whole item set.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRedo))]
    public void Redo() => ApplyHistory(undo: false);

    /// <summary>
    /// Validates the item and every related table, and lists the issues.
    /// </summary>
    /// <returns>A task that completes when validation finishes.</returns>
    [RelayCommand]
    public async Task ValidateAsync()
    {
        if (_set is null)
        {
            return;
        }

        IsBusy = true;
        try
        {
            ValidationIssues.Clear();
            foreach (var workspace in AllWorkspaces())
            {
                var report = await workspace
                    .ValidateAsync(_session.Lookups, workspace.Language?.Code)
                    .ConfigureAwait(true);
                foreach (var issue in report.Issues)
                {
                    ValidationIssues.Add(new ValidationIssueItem(
                        issue.TableName is null
                            ? issue with { TableName = workspace.Descriptor.TableName }
                            : issue));
                }
            }

            IsValidationVisible = true;
            StatusText = Strings.Format("EditorValidationSummary", ValidationIssues.Count);
        }
        catch (Exception exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Discards unsaved changes after confirmation and reloads the item set.
    /// </summary>
    /// <returns>A task that completes when the workspace is reloaded.</returns>
    [RelayCommand]
    public async Task ReloadAsync()
    {
        if (IsDirty && !_dialogService.Confirm(
                Strings.Format("EditorConfirmReload", Title),
                Strings.Get("EditorReloadTitle")))
        {
            return;
        }

        await ReloadPreservingSelectionAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Creates a new item with the next identifier and asks for its class and name.
    /// </summary>
    [RelayCommand]
    public void NewItem()
    {
        if (_set is null || !IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return;
        }

        var nextId = _set.Primary.BaseDocument
            .GetNextPrimaryKey()
            .ToString(CultureInfo.InvariantCulture);
        var request = _dialogService.CreateItem(
            Strings.Get("ItemNewTitle"),
            nextId,
            Strings.Get("ItemNewDefaultName"),
            BuildClassOptions());
        if (request is null)
        {
            return;
        }

        var id = request.Id.Trim();
        if (!int.TryParse(id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numericId)
            || numericId < 0)
        {
            StatusText = Strings.Format("EditorInvalidValue", request.Id, KeyField);
            return;
        }

        if (_set.Primary.FindRecord(id) is not null)
        {
            StatusText = Strings.Format("ItemIdConflict", id);
            return;
        }

        var result = _set.AddRecord(
            _descriptor.TableName,
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [KeyField] = id,
                [NameField] = request.Name,
                [ClassField] = request.ClassValue,
            });
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = _set.IsDirty;
        RevealItem(id);
        StatusText = Strings.Format("ItemCreated", id);
    }

    /// <summary>
    /// Duplicates the selected item and the dependent rows that store its identifier.
    /// </summary>
    [RelayCommand]
    public void DuplicateItem()
    {
        if (_set is null || _itemRow?.Key is not { } key)
        {
            StatusText = Strings.Get("EditorSelectRow");
            return;
        }

        var result = _set.DuplicateRecord(key);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = _set.IsDirty;
        RefreshChildEditors();
        RevealItem(result.Data);
        StatusText = Strings.Format("ItemDuplicated", key, result.Data ?? string.Empty);
    }

    /// <summary>
    /// Deletes the selected item and the dependent rows that store its identifier after confirmation.
    /// </summary>
    [RelayCommand]
    public void DeleteItem()
    {
        if (_set is null || _itemRow?.Key is not { } key)
        {
            StatusText = Strings.Get("EditorSelectRow");
            return;
        }

        var dependents = _set.DescribeDependents(key);
        var summary = dependents.Count == 0
            ? Strings.Get("ItemNoDependents")
            : string.Join(
                ", ",
                dependents.Select(entry => $"{entry.Table.Value} ({entry.Keys.Count})"));
        if (!_dialogService.Confirm(
                Strings.Format("ItemConfirmDelete", key, summary),
                Strings.Get("ItemDeleteTitle")))
        {
            return;
        }

        var result = _set.DeleteRecord(key);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        IsDirty = _set.IsDirty;
        RefreshChildEditors();
        RebuildItemList(null);
        StatusText = Strings.Format("ItemDeleted", key);
    }

    /// <summary>
    /// Renames the selected item, or swaps identifiers when the new key is already used by another item.
    /// Related rows that store the identifier are rewritten in the same transaction.
    /// </summary>
    [RelayCommand]
    public void ChangeItemId()
    {
        if (_set is null || _itemRow?.Key is not { } oldKey)
        {
            return;
        }

        var newKey = ItemIdText.Trim();
        if (string.Equals(newKey, oldKey, StringComparison.Ordinal))
        {
            return;
        }

        if (!int.TryParse(newKey, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numericId)
            || numericId < 0)
        {
            StatusText = Strings.Format("EditorInvalidValue", ItemIdText, KeyField);
            ItemIdText = oldKey;
            return;
        }

        OperationResult result;
        if (_set.Primary.FindRecord(newKey) is not null)
        {
            if (!_dialogService.Confirm(
                    Strings.Format("ItemConfirmSwap", oldKey, newKey),
                    Strings.Get("ItemChangeIdTitle")))
            {
                ItemIdText = oldKey;
                return;
            }

            result = _set.SwapRecords(oldKey, newKey);
        }
        else
        {
            result = _set.RenameRecord(oldKey, newKey);
        }

        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            ItemIdText = oldKey;
            return;
        }

        IsDirty = _set.IsDirty;
        RefreshChildEditors();
        RevealItem(newKey);
        StatusText = Strings.Format("ItemIdChanged", oldKey, newKey);
    }

    /// <summary>
    /// Selects the item with the identifier typed in <see cref="GoToIdText"/>.
    /// </summary>
    [RelayCommand]
    public void GoToItemId()
    {
        var key = GoToIdText.Trim();
        if (key.Length == 0)
        {
            return;
        }

        var entry = _allItems.FirstOrDefault(candidate =>
            string.Equals(candidate.Key, key, StringComparison.Ordinal));
        if (entry is null)
        {
            StatusText = Strings.Format("EditorRecordNotFound", key);
            return;
        }

        SearchText = string.Empty;
        ResetItemFilters();
        ApplyItemFilter(entry.Key);
    }

    /// <summary>
    /// Selects the item with the supplied key.
    /// </summary>
    /// <param name="key">Primary item key.</param>
    public void SelectRecord(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        if (_set is null || _allItems.Count == 0)
        {
            _pendingSelection = key.Trim();
            return;
        }

        var entry = _allItems.FirstOrDefault(candidate =>
            string.Equals(candidate.Key, key.Trim(), StringComparison.Ordinal));
        if (entry is null)
        {
            StatusText = Strings.Format("EditorRecordNotFound", key);
            return;
        }

        if (!Items.Contains(entry))
        {
            SearchText = string.Empty;
            ResetItemFilters();
            ApplyItemFilter(entry.Key);
        }
        else
        {
            SelectedItem = entry;
        }
    }

    /// <inheritdoc />
    OperationResult ITableCellEditor.SetCellValue(TableRowViewModel row, string fieldName, string value) =>
        SetCellValue(row, fieldName, value);

    /// <inheritdoc />
    public void SetDetailValue(DetailFieldViewModel field, string value)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(value);
        SetCellValue(field.Row, field.PathText, value);
    }

    /// <inheritdoc />
    public void NavigateToRelation(DetailFieldViewModel field)
    {
        ArgumentNullException.ThrowIfNull(field);
        if (field.Relation is not { IsAvailable: true } relation
            || !relation.TryGetTargetValue(field.Value, out var value))
        {
            StatusText = Strings.Get("EditorRelationTargetMissing");
            return;
        }

        if (relation.Relation.TargetTable == _descriptor.TableName)
        {
            SelectRecord(value);
            return;
        }

        TableNavigationRequested?.Invoke(
            this,
            new TableNavigationRequest(relation.Relation.TargetTable, value, TableDataDirectory));
    }

    /// <inheritdoc />
    public void EditBits(DetailFieldViewModel field)
    {
        ArgumentNullException.ThrowIfNull(field);
        if (!field.CanEditBits)
        {
            return;
        }

        var currentText = field.Value;
        var current = ulong.TryParse(currentText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0UL;
        LookupColumnData? lookup = null;
        if (_rowOwners.TryGetValue(field.Row, out var table)
            && _fieldSources.TryGetValue(table, out var sources)
            && sources.Lookups.TryGetValue(field.Name, out var found))
        {
            lookup = found;
        }

        var edited = _dialogService.EditBits(
            Strings.Format("BitEditorTitleFormat", field.PathText),
            current,
            BuildBitLabels(lookup),
            lookup?.GetExactEntryName(currentText));
        if (edited is not { } value)
        {
            return;
        }

        field.Value = value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Creates the missing 1:1 row of a sub-table section.
    /// </summary>
    /// <param name="section">Section whose row should be created.</param>
    internal void CreateSubTableRow(ItemSubTableSectionViewModel section)
    {
        ArgumentNullException.ThrowIfNull(section);
        if (_set is null || section.SourceValue is not { Length: > 0 } sourceValue)
        {
            StatusText = Strings.Get("ItemNoClassIndex");
            return;
        }

        var result = _set.EnsureSubTableRecord(section.Member.Relation, sourceValue);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        RebuildSections();
        StatusText = Strings.Format("ItemSubTableRowCreated", section.Label, sourceValue);
    }

    /// <summary>
    /// Writes a repeated field of the selected item.
    /// </summary>
    /// <param name="tableName">Table that stores the field.</param>
    /// <param name="fieldName">Case-sensitive XML element name of the repeated field.</param>
    /// <param name="values">New values in list order.</param>
    internal void SetListValues(TableName tableName, string fieldName, IReadOnlyList<string> values) =>
        SetListValues(tableName, fieldName, recordKey: null, values);

    /// <summary>
    /// Writes a repeated field of one record.
    /// </summary>
    /// <param name="tableName">Table that stores the field.</param>
    /// <param name="fieldName">Case-sensitive XML element name of the repeated field.</param>
    /// <param name="recordKey">Record that owns the field; the selected item when omitted.</param>
    /// <param name="values">New values in list order.</param>
    internal void SetListValues(
        TableName tableName,
        string fieldName,
        string? recordKey,
        IReadOnlyList<string> values)
    {
        var key = recordKey ?? _itemRow?.Key;
        if (_set is null || key is null)
        {
            return;
        }

        var result = _set.SetListValues(tableName, key, fieldName, values);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
        }
    }

    /// <summary>
    /// Applies a quantity edit from the shopkeeper tab to the dealer row that owns it.
    /// </summary>
    /// <param name="entry">Dealer row being edited.</param>
    /// <param name="value">New quantity text.</param>
    internal void SetShopkeeperQuantity(ShopkeeperEntryViewModel entry, string value)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(value);

        if (_set is null || !IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return;
        }

        var member = FindReverseMember(entry.TableName);
        var workspace = member?.Workspace;
        if (member is null || workspace is null)
        {
            StatusText = Strings.Format("StatusError", ErrorCodes.MetadataNotFound);
            return;
        }

        var displayField = DisplayFieldOf(member.Relation);
        // The entry remembers the exact record, so a repeated uiIndex in the dealer file cannot route
        // the edit to another item's row.
        var current = workspace.GetRow(entry.Row);
        var wasAbsent = current?.GetValue(displayField) is null;
        if (wasAbsent && value.Length == 0)
        {
            return;
        }

        var result = _set.SetValue(
            entry.TableName,
            entry.Row,
            displayField,
            value,
            addIfMissing: wasAbsent);
        if (!result.IsSuccess)
        {
            entry.Refresh(workspace.GetRow(entry.Row)?.GetValue(displayField)?.Trim() ?? string.Empty);
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        var stored = workspace.GetRow(entry.Row)?.GetValue(displayField)?.Trim() ?? string.Empty;
        entry.Refresh(stored);
        IsDirty = _set.IsDirty;
        RefreshHistoryState();
        StatusText = Strings.Format("EditorCellUpdated", displayField, entry.RowKey);
    }

    /// <summary>
    /// Adds the selected item to the dealer chosen in the shopkeeper tab. The new row stores the item
    /// reference and the entered optimal quantity, and the edit shares the item-set undo history.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddShopkeeperEntry))]
    public void AddShopkeeperEntry()
    {
        if (_set is null || _itemRow?.Key is not { } key)
        {
            return;
        }

        if (!IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return;
        }

        if (SelectedShopkeeperDealer is not { } dealer
            || FindReverseMember(dealer.TableName) is not { } member)
        {
            StatusText = Strings.Format("StatusError", ErrorCodes.MetadataNotFound);
            return;
        }

        // A dealer file with no record elements loads read-only, so it cannot receive a new row even
        // though it has no stock row for this item. Such a dealer is not offered as a choice.
        if (!member.Workspace.CanEdit)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return;
        }

        var quantity = NewShopkeeperQuantity.Trim();
        if (!int.TryParse(quantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out var numeric)
            || numeric < 0)
        {
            StatusText = Strings.Format("EditorInvalidValue", NewShopkeeperQuantity, ShopkeeperQuantityColumn);
            return;
        }

        var quantityField = DisplayFieldOf(member.Relation);
        var defaults = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [member.Relation.TargetValueField] = key,
            [quantityField] = numeric.ToString(CultureInfo.InvariantCulture),
        };
        var result = _set.AddRecord(dealer.TableName, defaults);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        NewShopkeeperQuantity = "0";
        RebuildShopkeeperEntries();
        IsDirty = _set.IsDirty;
        RefreshHistoryState();
        StatusText = Strings.Format("ShopkeeperRowAddedFormat", dealer.DealerName);
    }

    /// <summary>
    /// Removes one exact dealer row so the dealer no longer stocks the selected item. The operation is
    /// confirmed first and is undone, like any other set edit, by the workspace undo command.
    /// </summary>
    /// <param name="entry">Dealer row to remove.</param>
    public void RemoveShopkeeperEntry(ShopkeeperEntryViewModel entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (_set is null || !IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return;
        }

        var member = FindReverseMember(entry.TableName);
        if (member is null || !member.Workspace.CanEdit)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return;
        }

        if (!_dialogService.Confirm(
                Strings.Format(
                    "ShopkeeperConfirmRemoveFormat",
                    entry.DealerName,
                    _itemRow?.Key ?? string.Empty,
                    entry.Quantity),
                ShopkeeperRemoveTitle))
        {
            return;
        }

        var result = _set.DeleteRow(entry.TableName, entry.Row);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return;
        }

        RebuildShopkeeperEntries();
        IsDirty = _set.IsDirty;
        RefreshHistoryState();
        StatusText = Strings.Format("ShopkeeperRowRemovedFormat", entry.DealerName);
    }

    private bool CanAddShopkeeperEntry =>
        _set is not null && IsEditable && SelectedShopkeeperDealer is not null;

    /// <summary>
    /// Applies a single cell edit to the item or one of its related records.
    /// </summary>
    /// <param name="row">Edited row.</param>
    /// <param name="fieldName">Field name or slash-separated nested path.</param>
    /// <param name="value">New raw value.</param>
    /// <returns>The underlying edit result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="row"/> or <paramref name="fieldName"/> is null.</exception>
    public OperationResult SetCellValue(TableRowViewModel row, string fieldName, string value)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(value);

        if (_set is null || !IsEditable)
        {
            StatusText = Strings.Get("EditorReadOnly");
            return OperationResult.Failure(ErrorCodes.XmlNotSaveable, "The item workspace is not editable.");
        }

        if (row.Key is null)
        {
            StatusText = Strings.Format("StatusError", ErrorCodes.KeyNotFound);
            return OperationResult.Failure(ErrorCodes.KeyNotFound, "The record has no primary key.");
        }

        var table = _rowOwners.TryGetValue(row, out var owner) ? owner : _descriptor.TableName;
        var workspace = _set.GetWorkspace(table);
        if (workspace is null)
        {
            return OperationResult.Failure(
                ErrorCodes.MetadataNotFound,
                $"Table '{table}' is not part of the item set.");
        }

        var wasAbsent = row.GetStoredValue(fieldName) is null;
        if (wasAbsent && value.Length == 0)
        {
            return OperationResult.Success();
        }

        if (table == _descriptor.TableName
            && string.Equals(fieldName, GraphicIndexField, StringComparison.Ordinal))
        {
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var requestedIndex))
            {
                StatusText = Strings.Get("ItemImageIndexInvalid");
                return OperationResult.Failure(ErrorCodes.XmlInvalidNumber, StatusText);
            }

            if (requestedIndex < 0)
            {
                StatusText = Strings.Get("ItemImageIndexNegative");
                return OperationResult.Failure(ErrorCodes.XmlInvalidNumber, StatusText);
            }

            if (_graphicImageListLoading)
            {
                StatusText = Strings.Get("ItemImageListLoading");
                return OperationResult.Failure(ErrorCodes.ImageNotReady, StatusText);
            }

            if (GraphicIndexMaximum >= 0 && requestedIndex > GraphicIndexMaximum)
            {
                StatusText = Strings.Format("ItemImageIndexOutOfRangeFormat", value, GraphicIndexMaximum);
                return OperationResult.Failure(ErrorCodes.XmlInvalidNumber, StatusText);
            }
        }

        var result = _set.SetValue(table, row.Key, fieldName, value, addIfMissing: wasAbsent);
        if (!result.IsSuccess)
        {
            StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
            return result;
        }

        var refreshed = workspace.GetRow(row.Key);
        if (refreshed is not null)
        {
            row.Refresh(refreshed);
        }

        if (fieldName is GraphicTypeField or GraphicIndexField)
        {
            var entry = _allItems.FirstOrDefault(candidate => ReferenceEquals(candidate.Row, row));
            entry?.InvalidateThumbnail();
        }

        IsDirty = _set.IsDirty;
        RefreshHistoryState();
        if (table == _descriptor.TableName)
        {
            ItemClassText = ResolveClassText(row);
            DetailHeader = BuildItemHeader(row);
            if (string.Equals(fieldName, ClassField, StringComparison.Ordinal))
            {
                RebuildSections();
                RefreshChildSectionState();
            }
            else if (string.Equals(fieldName, GraphicTypeField, StringComparison.Ordinal))
            {
                SyncGraphicType();
                GraphicIndexMaximum = -1;
                _ = LoadImagesAsync();
                _ = RefreshGraphicImagesAsync();
            }
            else if (string.Equals(fieldName, GraphicIndexField, StringComparison.Ordinal))
            {
                SyncGraphicImageSelection();
                _ = LoadImagesAsync();
            }
        }

        StatusText = Strings.Format("EditorCellUpdated", fieldName, row.Key);
        return result;
    }

    /// <summary>
    /// Creates or updates a field of the selected item directly, which is used by tests and by commands
    /// that are not bound to a form field.
    /// </summary>
    /// <param name="fieldName">Field name or slash-separated nested path.</param>
    /// <param name="value">New raw value.</param>
    /// <returns>The underlying edit result.</returns>
    public OperationResult SetItemValue(string fieldName, string value)
    {
        if (_itemRow is null)
        {
            return OperationResult.Failure(ErrorCodes.KeyNotFound, "No item is selected.");
        }

        return SetCellValue(_itemRow, fieldName, value);
    }

    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(IsLanguageSelectorEnabled));

    partial void OnIsEditableChanged(bool value) => AddShopkeeperEntryCommand.NotifyCanExecuteChanged();

    partial void OnSelectedShopkeeperDealerChanged(ShopkeeperDealerOptionViewModel? value) =>
        AddShopkeeperEntryCommand.NotifyCanExecuteChanged();

    partial void OnCanUndoChanged(bool value) => UndoCommand.NotifyCanExecuteChanged();

    partial void OnCanRedoChanged(bool value) => RedoCommand.NotifyCanExecuteChanged();

    partial void OnSearchTextChanged(string value)
    {
        if (_suppressItemFilter)
        {
            return;
        }

        ApplyItemFilter(_itemRow?.Key);
    }

    partial void OnSelectedClassFilterChanged(ItemListFilterOption? value) => OnPresetFilterChanged();

    partial void OnSelectedFlagFilterChanged(ItemListFilterOption? value) => OnPresetFilterChanged();

    private void OnPresetFilterChanged()
    {
        if (_suppressItemFilter)
        {
            return;
        }

        OnPropertyChanged(nameof(HasActiveItemFilters));
        ApplyItemFilter(_itemRow?.Key);
    }

    partial void OnFieldFilterChanged(string value)
    {
        if (_itemRow is not null)
        {
            RebuildSections();
        }
    }

    partial void OnSelectedItemChanged(ItemListEntry? value)
    {
        OnPropertyChanged(nameof(CanShowShopkeeperAddBar));
        if (_suppressSelection)
        {
            return;
        }

        LoadItemForm();
    }

    partial void OnSelectedGraphicTypeChanged(GraphicTypeOption? value)
    {
        if (_suppressGraphicType || value is null || _itemRow is null)
        {
            return;
        }

        var current = _itemRow.GetStoredValue(GraphicTypeField)?.Trim();
        var text = value.Number.ToString(CultureInfo.InvariantCulture);
        if (!string.Equals(current, text, StringComparison.Ordinal))
        {
            SetCellValue(_itemRow, GraphicTypeField, text);
        }
    }

    partial void OnSelectedGraphicImageChanged(ItemImageOption? value)
    {
        if (_suppressGraphicImage || value is null || _itemRow is null)
        {
            return;
        }

        var current = GetGraphicIndex(_itemRow);
        if (current == value.Index)
        {
            return;
        }

        SetCellValue(
            _itemRow,
            GraphicIndexField,
            value.Index.ToString(CultureInfo.InvariantCulture));
    }

    partial void OnSelectedLanguageOptionChanged(LanguageOptionViewModel? value)
    {
        if (_suppressLanguageChange || value is null || _isReloading)
        {
            return;
        }

        if (string.Equals(value.Code, _languageCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _ = ChangeLanguageAsync(value);
    }

    private async Task ChangeLanguageAsync(LanguageOptionViewModel option)
    {
        if (IsDirty && !_dialogService.Confirm(
                Strings.Format("EditorConfirmLanguage", Title),
                Strings.Get("EditorLanguageTitle")))
        {
            RestoreLanguageSelection();
            return;
        }

        if (!option.HasOverlay && option.Language is not null)
        {
            if (!_dialogService.Confirm(
                    Strings.Format(
                        "EditorConfirmCreateOverlay",
                        option.Language.DisplayName,
                        _descriptor.TableName.Value),
                    Strings.Get("EditorLanguageTitle")))
            {
                RestoreLanguageSelection();
                return;
            }

            IsBusy = true;
            try
            {
                var ensured = await _session
                    .EnsureOverlayAsync(_descriptor, option.Language)
                    .ConfigureAwait(true);
                if (!ensured.IsSuccess)
                {
                    StatusText = Strings.Format(
                        "StatusError",
                        $"{ensured.ErrorCode}: {ensured.ErrorText}");
                    RestoreLanguageSelection();
                    return;
                }
            }
            catch (Exception exception)
            {
                StatusText = Strings.Format("StatusError", exception.Message);
                RestoreLanguageSelection();
                return;
            }
            finally
            {
                IsBusy = false;
            }
        }

        _languageCode = option.Code;
        await ReloadPreservingSelectionAsync().ConfigureAwait(true);
        LanguageChanged?.Invoke(this, _languageCode);
    }

    private void RestoreLanguageSelection()
    {
        _suppressLanguageChange = true;
        try
        {
            SelectedLanguageOption = FindLanguageOption(_languageCode);
        }
        finally
        {
            _suppressLanguageChange = false;
        }
    }

    private LanguageOptionViewModel? FindLanguageOption(string? code) =>
        string.IsNullOrWhiteSpace(code)
            ? LanguageOptions.FirstOrDefault(static option => option.Language is null)
            : LanguageOptions.FirstOrDefault(option =>
                string.Equals(option.Code, code, StringComparison.OrdinalIgnoreCase));

    private async Task ReloadPreservingSelectionAsync()
    {
        if (_isReloading)
        {
            return;
        }

        _isReloading = true;
        try
        {
            var key = _itemRow?.Key;
            ResetItemFilterState();
            ClearAllSections();
            await InitializeCoreAsync(CancellationToken.None).ConfigureAwait(true);
            if (key is not null)
            {
                SelectRecord(key);
            }
        }
        finally
        {
            _isReloading = false;
        }
    }

    private void ClearAllSections()
    {
        Items.Clear();
        ItemGroups.Clear();
        NestedGroups.Clear();
        SubTableSections.Clear();
        ListSections.Clear();
        ChildSections.Clear();
        VisibleChildSections.Clear();
        ValidationIssues.Clear();
        ShopkeeperEntries.Clear();
        ShopkeeperDealerOptions.Clear();
        SelectedShopkeeperDealer = null;
        NotifyShopkeeperChanged();
        _allItems.Clear();
        _allFields.Clear();
        _rowOwners.Clear();
        _fieldSources.Clear();
        _itemRow = null;
    }

    private async Task LoadClassNamesAsync(CancellationToken cancellationToken)
    {
        _classNames.Clear();
        _classExactNames.Clear();
        var entries = await _session.Lookups
            .GetEntriesAsync(TableName.Parse(ClassLookup), _set?.Primary.Language?.Code, cancellationToken)
            .ConfigureAwait(true);
        if (!entries.IsSuccess)
        {
            return;
        }

        foreach (var entry in entries.Data!)
        {
            _classNames.TryAdd(entry.Id, entry.Name);
            _classExactNames.TryAdd(entry.Id, entry.Name);
        }
    }

    private async Task LoadFieldSourcesAsync(CancellationToken cancellationToken)
    {
        if (_set is null)
        {
            return;
        }

        _fieldSources.Clear();
        var workspaces = new[] { _set.Primary }
            .Concat(_set.GetMembers(TableRelationKind.SubTable)
                .Select(static member => member.Workspace))
            .DistinctBy(static workspace => workspace.Descriptor.TableName);
        foreach (var workspace in workspaces)
        {
            var sources = await FieldSourceSet
                .LoadAsync(_session, workspace, cancellationToken)
                .ConfigureAwait(true);
            _fieldSources[workspace.Descriptor.TableName] = sources;
        }
    }

    private async Task InitializeChildEditorsAsync(CancellationToken cancellationToken)
    {
        ChildSections.Clear();
        VisibleChildSections.Clear();
        if (_set is null)
        {
            return;
        }

        foreach (var member in ChildSectionMembers(_set))
        {
            var editor = new TableEditorViewModel(
                _session,
                member.Workspace.Descriptor,
                member.Workspace.Language?.Code,
                _dialogService,
                _itemImages)
            {
                IsEmbedded = true,
                ShowDetails = false,
                EmbeddedSave = SaveAsync,
                ParentRelationField = member.Relation.TargetValueField,
                RowActivationField = member.Relation.TargetDisplayField,
            };
            await editor
                .InitializeFromWorkspaceAsync(member.Workspace, _languages, cancellationToken)
                .ConfigureAwait(true);
            editor.ExternalRowFilter = row => IsRowOfSelectedItem(member.Relation, row);
            editor.TableNavigationRequested += OnChildEditorNavigationRequested;
            ChildSections.Add(new ItemChildSectionViewModel(this, member, LabelFor(member), editor));
        }
    }

    /// <summary>
    /// Returns the members that become child grids: the outgoing 1:N relations and the reverse
    /// relations that legacy exposed as a second direction of the same table.
    /// </summary>
    private static IEnumerable<TableSetMember> ChildSectionMembers(TableSetWorkspace set) =>
        set.GetMembers(TableRelationKind.ChildRows)
            .Concat(set.GetMembers(TableRelationKind.Reverse).Where(
                static member => IsReverseSection(member.Relation)));

    private static bool IsReverseSection(TableRelation relation) =>
        relation.Kind == TableRelationKind.Reverse
        && relation.LabelKey is { } labelKey
        && labelKey.StartsWith("ItemSectionReverse.", StringComparison.Ordinal);

    private static bool IsShopkeeperRelation(TableRelation relation) =>
        string.Equals(relation.LabelKey, "ShopkeeperInventory", StringComparison.Ordinal);

    /// <summary>
    /// Follows a navigation request raised by an embedded child grid. Targets in the same table are
    /// selected in this workspace; other targets are forwarded to the shell.
    /// </summary>
    private void OnChildEditorNavigationRequested(object? sender, TableNavigationRequest request)
    {
        if (request.TableName == _descriptor.TableName
            && string.Equals(request.TableDataDirectory, TableDataDirectory, StringComparison.OrdinalIgnoreCase))
        {
            SelectRecord(request.RecordKey);
            return;
        }

        TableNavigationRequested?.Invoke(this, request);
    }

    private bool IsRowOfSelectedItem(TableRelation relation, TableRowViewModel row) =>
        _itemRow?.Key is { } key
        && string.Equals(row.GetStoredValue(relation.TargetValueField), key, StringComparison.Ordinal);

    private void RefreshChildEditors()
    {
        foreach (var section in ChildSections)
        {
            section.Editor.RefreshRows();
        }

        RefreshChildSectionState();
    }

    private void RefreshChildSectionState()
    {
        var key = _itemRow?.Key;
        var classBits = ParseClassBits(_itemRow?.GetStoredValue(ClassField));
        foreach (var section in ChildSections)
        {
            section.IsVisible = IsSectionVisibleForClass(
                ClassVisibilitySectionKind.ChildTable,
                SectionKeyFor(section.Member.Relation),
                classBits);
            section.Editor.NewRowDefaults = key is null
                ? null
                : new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    [section.Member.Relation.TargetValueField] = key,
                };
            section.Editor.RefreshRowFilter();
            section.RefreshCount();
        }

        RefreshVisibleChildSections();
    }

    private void RefreshVisibleChildSections()
    {
        var visible = ChildSections.Where(static section => section.IsVisible).ToArray();
        if (visible.Length == VisibleChildSections.Count
            && visible.SequenceEqual(VisibleChildSections))
        {
            return;
        }

        VisibleChildSections.Clear();
        foreach (var section in visible)
        {
            VisibleChildSections.Add(section);
        }
    }

    private void RebuildItemList(string? selectKey)
    {
        if (_set is null)
        {
            return;
        }

        _rowOwners.Clear();
        _allItems.Clear();
        foreach (var row in _set.Primary.GetRows())
        {
            var viewModel = new TableRowViewModel(row, this);
            _rowOwners[viewModel] = _descriptor.TableName;
            var entry = new ItemListEntry(viewModel, ResolveClassText);
            entry.ConfigureThumbnail(LoadItemThumbnailAsync);
            _allItems.Add(entry);
        }

        EnsureItemFilterOptions();
        EnsureCustomItemFilter();
        ApplyItemFilter(selectKey);
    }

    private void EnsureItemFilterOptions()
    {
        if (ClassFilterOptions.Count > 0)
        {
            return;
        }

        var hasRocketLauncherField = _set?.Primary.Schema.FindColumn(
            ItemListFilterPresets.RocketLauncherField) is not null;
        _suppressItemFilter = true;
        try
        {
            foreach (var option in ItemListFilterPresets.CreateClassOptions(hasRocketLauncherField))
            {
                ClassFilterOptions.Add(option);
            }

            foreach (var option in ItemListFilterPresets.CreateFlagOptions())
            {
                FlagFilterOptions.Add(option);
            }

            SelectedClassFilter = ClassFilterOptions[0];
            SelectedFlagFilter = FlagFilterOptions[0];
        }
        finally
        {
            _suppressItemFilter = false;
        }
    }

    private void EnsureCustomItemFilter()
    {
        if (CustomItemFilter is not null || _set is null)
        {
            return;
        }

        var columns = _set.Primary.Schema.Columns
            .Where(static column => !column.IsList && !column.IsDict && !column.IsNested)
            .Select(static column => new TableColumnViewModel(column))
            .ToArray();
        CustomItemFilter = new TableFilterViewModel(columns, OnCustomItemFilterChanged);
    }

    private void ApplyItemFilter(string? selectKey)
    {
        var term = SearchText.Trim();
        IEnumerable<ItemListEntry> query = _allItems;
        if (term.Length > 0)
        {
            query = query.Where(entry =>
                entry.Key.Contains(term, StringComparison.OrdinalIgnoreCase)
                || entry.Name.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var schema = _set?.Primary.Schema;
        if (schema is not null)
        {
            query = ApplyFilterGroup(query, schema, SelectedClassFilter?.Filter);
            query = ApplyFilterGroup(query, schema, SelectedFlagFilter?.Filter);
            query = ApplyFilterGroup(query, schema, CustomItemFilter?.ToGroup());
        }

        var visible = query.ToList();
        var wanted = selectKey ?? _itemRow?.Key;
        _suppressSelection = true;
        try
        {
            Items.Clear();
            foreach (var entry in visible)
            {
                Items.Add(entry);
            }

            CountText = Strings.Format("ItemCountFormat", Items.Count, _allItems.Count);
            CountShortText = Strings.Format("ItemCountShortFormat", Items.Count, _allItems.Count);
        }
        finally
        {
            _suppressSelection = false;
        }

        var target = wanted is null
            ? Items.FirstOrDefault()
            : Items.FirstOrDefault(entry => string.Equals(entry.Key, wanted, StringComparison.Ordinal))
                ?? Items.FirstOrDefault();
        var same = ReferenceEquals(SelectedItem, target);
        SelectedItem = target;
        if (same)
        {
            LoadItemForm();
        }

        if (Items.Count == 0 && (term.Length > 0 || HasActiveItemFilters))
        {
            StatusText = Strings.Get("EditorNoMatches");
        }
        else if (string.Equals(StatusText, Strings.Get("EditorNoMatches"), StringComparison.Ordinal))
        {
            StatusText = Strings.Get("EditorReady");
        }
    }

    private static IEnumerable<ItemListEntry> ApplyFilterGroup(
        IEnumerable<ItemListEntry> entries,
        TableSchema schema,
        RowFilterGroup? filter) =>
        filter is null || filter.IsEmpty
            ? entries
            : entries.Where(entry => TableRowFilterMatcher.Matches(entry.Row.Row, schema, filter));

    private void OnCustomItemFilterChanged()
    {
        if (_suppressItemFilter || _set is null || CustomItemFilter is null)
        {
            return;
        }

        OnPropertyChanged(nameof(HasActiveItemFilters));
        ApplyItemFilter(_itemRow?.Key);
    }

    private void ResetItemFilters(bool clearSearch = false)
    {
        _suppressItemFilter = true;
        try
        {
            if (clearSearch)
            {
                SearchText = string.Empty;
            }

            SelectedClassFilter = ClassFilterOptions.FirstOrDefault();
            SelectedFlagFilter = FlagFilterOptions.FirstOrDefault();
            CustomItemFilter?.Clear();
        }
        finally
        {
            _suppressItemFilter = false;
        }

        OnPropertyChanged(nameof(HasActiveItemFilters));
    }

    private void ResetItemFilterState()
    {
        _suppressItemFilter = true;
        try
        {
            SelectedClassFilter = null;
            SelectedFlagFilter = null;
            ClassFilterOptions.Clear();
            FlagFilterOptions.Clear();
            CustomItemFilter = null;
        }
        finally
        {
            _suppressItemFilter = false;
        }

        OnPropertyChanged(nameof(HasActiveItemFilters));
    }

    private void RevealItem(string? key)
    {
        ResetItemFilters(clearSearch: true);
        RebuildItemList(key);
    }

    /// <summary>
    /// Clears the class preset, the flag preset, and the custom structured filter, restoring the full
    /// item list. The search term is kept.
    /// </summary>
    [RelayCommand]
    public void ClearItemFilters()
    {
        ResetItemFilters();
        ApplyItemFilter(_itemRow?.Key);
    }

    private void LoadItemForm()
    {
        _itemRow = SelectedItem?.Row;
        if (_itemRow is null || _set is null)
        {
            ItemIdText = string.Empty;
            DetailHeader = Strings.Get("ItemNoSelection");
            ItemClassText = string.Empty;
            ClearAllSections(keepItems: true);
            _rowOwners.Clear();
            CancelImageLoads();
            CancelGraphicImageListLoad();
            ClearImages();
            ClearGraphicImages();
            RebuildShopkeeperEntries();
            return;
        }

        ItemIdText = _itemRow.Key ?? string.Empty;
        DetailHeader = BuildItemHeader(_itemRow);
        ItemClassText = ResolveClassText(_itemRow);
        RebuildSections();
        RebuildShopkeeperEntries();
        RefreshChildSectionState();
        SyncGraphicType();
        GraphicIndexMaximum = -1;
        _ = LoadImagesAsync();
        _ = RefreshGraphicImagesAsync();
    }

    /// <summary>
    /// Rebuilds the dealer rows that stock the selected item from the reverse-related inventories.
    /// </summary>
    private void RebuildShopkeeperEntries()
    {
        ShopkeeperEntries.Clear();
        if (_set is null || _itemRow?.Key is not { } key)
        {
            RebuildShopkeeperDealerOptions(new HashSet<TableName>());
            NotifyShopkeeperChanged();
            return;
        }

        var entries = new List<ShopkeeperEntryViewModel>();
        foreach (var member in _set.GetMembers(TableRelationKind.Reverse))
        {
            var relation = member.Relation;
            if (!IsShopkeeperRelation(relation))
            {
                continue;
            }

            var displayField = DisplayFieldOf(relation);
            foreach (var row in member.Workspace.GetRows())
            {
                if (row.Key is null
                    || !string.Equals(row.GetValue(relation.TargetValueField)?.Trim(), key, StringComparison.Ordinal))
                {
                    continue;
                }

                entries.Add(new ShopkeeperEntryViewModel(
                    this,
                    relation.TargetTable,
                    row,
                    DealerNameOf(relation.TargetTable),
                    row.GetValue(displayField)?.Trim() ?? string.Empty));
            }
        }

        foreach (var entry in entries.OrderBy(
                     static entry => entry.DealerName,
                     StringComparer.OrdinalIgnoreCase))
        {
            ShopkeeperEntries.Add(entry);
        }

        RebuildShopkeeperDealerOptions(entries.Select(static entry => entry.TableName).ToHashSet());
        NotifyShopkeeperChanged();
    }

    /// <summary>
    /// Rebuilds the dealer choices for a new row from the loaded shopkeeper inventories that do not
    /// stock the selected item. The selection is kept when that dealer is still available.
    /// </summary>
    /// <param name="stocked">Dealer tables that already have a row for the selected item.</param>
    private void RebuildShopkeeperDealerOptions(IReadOnlySet<TableName> stocked)
    {
        var selected = SelectedShopkeeperDealer?.TableName;
        ShopkeeperDealerOptions.Clear();
        if (_set is not null && _itemRow is not null)
        {
            var available = _set.GetMembers(TableRelationKind.Reverse)
                .Where(static member => IsShopkeeperRelation(member.Relation))
                .Where(static member => member.Workspace.CanEdit)
                .Where(member => !stocked.Contains(member.Relation.TargetTable))
                .GroupBy(static member => member.Relation.TargetTable)
                .Select(static group => group.First())
                .OrderBy(
                    static member => DealerNameOf(member.Relation.TargetTable),
                    StringComparer.OrdinalIgnoreCase);
            foreach (var member in available)
            {
                ShopkeeperDealerOptions.Add(new ShopkeeperDealerOptionViewModel(
                    member.Relation.TargetTable,
                    DealerNameOf(member.Relation.TargetTable)));
            }
        }

        SelectedShopkeeperDealer = selected is null
            ? null
            : ShopkeeperDealerOptions.FirstOrDefault(option => option.TableName == selected);
        OnPropertyChanged(nameof(HasShopkeeperDealerOptions));
    }

    /// <summary>
    /// Refreshes the displayed quantities after an edit that changed several documents at once.
    /// </summary>
    private void RefreshShopkeeperQuantities()
    {
        var missing = false;
        foreach (var entry in ShopkeeperEntries)
        {
            var member = FindReverseMember(entry.TableName);
            var row = member?.Workspace.GetRow(entry.Row);
            if (row is null)
            {
                // Undo may replace the underlying record instance, and a delete removes it outright.
                // Rebuilding below restores the exact references instead of showing a ghost entry.
                missing = true;
                continue;
            }

            entry.Refresh(row.GetValue(DisplayFieldOf(member!.Relation))?.Trim() ?? string.Empty);
        }

        if (missing)
        {
            RebuildShopkeeperEntries();
        }
    }

    private TableSetMember? FindReverseMember(TableName tableName) =>
        _set?.GetMembers(TableRelationKind.Reverse)
            .FirstOrDefault(member => IsShopkeeperRelation(member.Relation)
                && member.Relation.TargetTable == tableName);

    private static string DisplayFieldOf(TableRelation relation) =>
        relation.TargetDisplayField ?? relation.TargetValueField;

    private static string DealerNameOf(TableName table)
    {
        var name = table.Value.Split('.').Last();
        const string suffix = "Inventory";
        return name.EndsWith(suffix, StringComparison.Ordinal) && name.Length > suffix.Length
            ? name[..^suffix.Length]
            : name;
    }

    private void NotifyShopkeeperChanged()
    {
        OnPropertyChanged(nameof(HasShopkeeperEntries));
        OnPropertyChanged(nameof(HasShopkeeperDealerOptions));
        OnPropertyChanged(nameof(ShopkeeperCountText));
    }

    private void RebuildSections()
    {
        ItemGroups.Clear();
        NestedGroups.Clear();
        SubTableSections.Clear();
        ListSections.Clear();
        _allFields.Clear();
        if (_set is null || _itemRow is null)
        {
            return;
        }

        var primary = _set.Primary;
        var sources = _fieldSources.GetValueOrDefault(primary.Descriptor.TableName);
        var classBits = ParseClassBits(_itemRow.GetStoredValue(ClassField));
        var fields = new List<DetailFieldViewModel>();
        var nestedColumns = new List<TableSchemaColumn>();
        var listColumns = new List<TableSchemaColumn>();
        foreach (var column in primary.Schema.Columns)
        {
            if (column.IsKey)
            {
                continue;
            }

            if (!IsGroupVisibleForClass(column.GroupKey, classBits))
            {
                continue;
            }

            if (column.IsNested)
            {
                nestedColumns.Add(column);
                continue;
            }

            if (column.IsList && column.MaxOccurrences > 1)
            {
                listColumns.Add(column);
                continue;
            }

            if (!MatchesFieldFilter(column.XmlFieldName, column.DataType))
            {
                continue;
            }

            LookupColumnData? lookup = null;
            RelationColumnData? relation = null;
            sources?.Lookups.TryGetValue(column.XmlFieldName, out lookup);
            sources?.Relations.TryGetValue(column.XmlFieldName, out relation);
            fields.Add(new DetailFieldViewModel(
                this,
                _itemRow,
                column,
                lookup,
                relation,
                DetailTextLimits.Resolve(primary, column.XmlFieldName)));
        }

        AddFieldGroups(ItemGroups, fields);
        _allFields.AddRange(fields);

        foreach (var column in listColumns)
        {
            if (!MatchesFieldFilter(column.XmlFieldName, ColumnDataType.String))
            {
                continue;
            }

            RelationColumnData? relation = null;
            sources?.Relations.TryGetValue(column.XmlFieldName, out relation);
            ListSections.Add(new ItemListSectionViewModel(
                this,
                primary.Descriptor.TableName,
                recordKey: null,
                column.XmlFieldName,
                ListSectionLabel(column.XmlFieldName),
                relation,
                _itemRow.GetValues(column.XmlFieldName)));
        }

        foreach (var column in nestedColumns)
        {
            AddNestedGroup(column);
        }

        BuildSubTableSections();
    }

    private void AddFieldGroups(
        ObservableCollection<DetailFieldGroupViewModel> target,
        IEnumerable<DetailFieldViewModel> fields)
    {
        foreach (var group in fields
                     .GroupBy(static field => field.GroupKey, StringComparer.Ordinal)
                     .OrderBy(static group => group.First().GroupOrder))
        {
            var key = group.Key;
            var label = Strings.Get("DetailGroup." + key);
            if (string.Equals(label, "DetailGroup." + key, StringComparison.Ordinal))
            {
                label = key;
            }

            target.Add(new DetailFieldGroupViewModel(
                key,
                label,
                group.First().GroupOrder,
                IsGroupExpandedByDefault(key),
                group.OrderBy(static field => field.Order)));
        }
    }

    private bool MatchesFieldFilter(string fieldName, ColumnDataType dataType)
    {
        var term = FieldFilter.Trim();
        return term.Length == 0
            || fieldName.Contains(term, StringComparison.OrdinalIgnoreCase)
            || dataType.ToString().Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsGroupExpandedByDefault(string groupKey) =>
        groupKey is "identity" or "classification" or "inventory" or "attachments" or "combat";

    private static string ListSectionLabel(string fieldName)
    {
        var key = "ItemListSection." + fieldName;
        var label = Strings.Get(key);
        return string.Equals(label, key, StringComparison.Ordinal) ? fieldName : label;
    }

    private void AddNestedGroup(TableSchemaColumn container)
    {
        if (_set is null || _itemRow is null)
        {
            return;
        }

        var names = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var record in _set.Primary.BaseDocument.Records)
        {
            foreach (var name in record.GetChildFieldNames(container.XmlFieldName))
            {
                if (seen.Add(name))
                {
                    names.Add(name);
                }
            }
        }

        if (names.Count == 0)
        {
            return;
        }

        var fields = new List<DetailFieldViewModel>();
        for (var index = 0; index < names.Count; index++)
        {
            var path = new FieldPath(new[] { container.XmlFieldName, names[index] });
            if (!MatchesFieldFilter(path.Text, ColumnDataType.Integer))
            {
                continue;
            }

            fields.Add(new DetailFieldViewModel(
                this,
                _itemRow,
                path,
                InferNestedType(path),
                index,
                "nested:" + container.XmlFieldName,
                container.GroupOrder,
                isReadOnly: false,
                isTranslatable: false));
        }

        _allFields.AddRange(fields);
        var label = Strings.Get("ItemStance." + container.XmlFieldName);
        if (string.Equals(label, "ItemStance." + container.XmlFieldName, StringComparison.Ordinal))
        {
            label = container.XmlFieldName;
        }

        NestedGroups.Add(new DetailFieldGroupViewModel(
            "nested:" + container.XmlFieldName,
            label,
            container.GroupOrder,
            isExpanded: false,
            fields));
    }

    private void BuildSubTableSections()
    {
        SubTableSections.Clear();
        if (_set is null || _itemRow is null)
        {
            return;
        }

        var classBits = ParseClassBits(_itemRow.GetStoredValue(ClassField));
        foreach (var member in _set.GetMembers(TableRelationKind.SubTable))
        {
            if (!IsSectionVisibleForClass(
                    ClassVisibilitySectionKind.SubTable,
                    member.Relation.TargetTable.Value,
                    classBits))
            {
                continue;
            }

            var workspace = member.Workspace;
            var sourceValue = _itemRow.GetStoredValue(member.Relation.SourceField)?.Trim();
            var record = string.IsNullOrEmpty(sourceValue)
                ? null
                : FindRecordByValue(workspace, member.Relation.TargetValueField, sourceValue);
            var row = record is null ? null : CreateRow(workspace, record.GetKey());
            var fields = row is null
                ? Array.Empty<DetailFieldViewModel>()
                : BuildSubTableFields(member, row);
            var listSections = row is null
                ? Array.Empty<ItemListSectionViewModel>()
                : BuildSubTableListSections(member, row);
            _allFields.AddRange(fields);
            SubTableSections.Add(new ItemSubTableSectionViewModel(
                this,
                member,
                LabelFor(member),
                row,
                sourceValue,
                fields,
                listSections));
        }
    }

    private IReadOnlyList<DetailFieldViewModel> BuildSubTableFields(
        TableSetMember member,
        TableRowViewModel row)
    {
        var sources = _fieldSources.GetValueOrDefault(member.Workspace.Descriptor.TableName);
        var fields = new List<DetailFieldViewModel>();
        foreach (var column in member.Workspace.Schema.Columns)
        {
            if (column.IsKey || column.IsNested || column.IsDict || (column.IsList && column.MaxOccurrences > 1))
            {
                continue;
            }

            if (!MatchesFieldFilter(column.XmlFieldName, column.DataType))
            {
                continue;
            }

            LookupColumnData? lookup = null;
            RelationColumnData? relation = null;
            sources?.Lookups.TryGetValue(column.XmlFieldName, out lookup);
            sources?.Relations.TryGetValue(column.XmlFieldName, out relation);
            fields.Add(new DetailFieldViewModel(
                this,
                row,
                column,
                lookup,
                relation,
                DetailTextLimits.Resolve(member.Workspace, column.XmlFieldName)));
        }

        return fields
            .OrderBy(static field => field.GroupOrder)
            .ThenBy(static field => field.Order)
            .ToArray();
    }

    /// <summary>
    /// Builds the repeated-field sections of one sub-table row. A list field is skipped by
    /// <see cref="BuildSubTableFields"/> because it is edited as a list, not as a scalar cell.
    /// </summary>
    private IReadOnlyList<ItemListSectionViewModel> BuildSubTableListSections(
        TableSetMember member,
        TableRowViewModel row)
    {
        var sources = _fieldSources.GetValueOrDefault(member.Workspace.Descriptor.TableName);
        var sections = new List<ItemListSectionViewModel>();
        foreach (var column in member.Workspace.Schema.Columns)
        {
            if (column.IsKey
                || column.IsNested
                || column.IsDict
                || !column.IsList
                || column.MaxOccurrences <= 1)
            {
                continue;
            }

            if (!MatchesFieldFilter(column.XmlFieldName, ColumnDataType.String))
            {
                continue;
            }

            RelationColumnData? relation = null;
            sources?.Relations.TryGetValue(column.XmlFieldName, out relation);
            sections.Add(new ItemListSectionViewModel(
                this,
                member.Workspace.Descriptor.TableName,
                row.Key,
                column.XmlFieldName,
                ListSectionLabel(column.XmlFieldName),
                relation,
                row.GetValues(column.XmlFieldName)));
        }

        return sections;
    }

    private TableRowViewModel? CreateRow(TableWorkspace workspace, string? key)
    {
        if (key is null)
        {
            return null;
        }

        var row = workspace.GetRow(key);
        if (row is null)
        {
            return null;
        }

        var viewModel = new TableRowViewModel(row, this);
        _rowOwners[viewModel] = workspace.Descriptor.TableName;
        return viewModel;
    }

    private static TableRecord? FindRecordByValue(
        TableWorkspace workspace,
        string field,
        string value) =>
        workspace.BaseDocument.Records.FirstOrDefault(candidate =>
            string.Equals(candidate.GetRawValue(field)?.Trim(), value, StringComparison.Ordinal));

    private ColumnDataType InferNestedType(FieldPath path)
    {
        var hasValue = false;
        var allInteger = true;
        var allDecimal = true;
        if (_set is not null)
        {
            foreach (var record in _set.Primary.BaseDocument.Records)
            {
                var raw = record.GetRawValue(path);
                if (string.IsNullOrEmpty(raw))
                {
                    continue;
                }

                hasValue = true;
                if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                {
                    allInteger = false;
                }

                if (!decimal.TryParse(raw, DecimalStyles, CultureInfo.InvariantCulture, out _))
                {
                    allDecimal = false;
                }
            }
        }

        return !hasValue ? ColumnDataType.Integer
            : allInteger ? ColumnDataType.Integer
            : allDecimal ? ColumnDataType.Decimal
            : ColumnDataType.String;
    }

    private ulong ParseClassBits(string? raw) =>
        ulong.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : 0;

    private string ResolveClassText(TableRowViewModel row)
    {
        var bits = ParseClassBits(row[ClassField]);
        if (bits == 0 || _classNames.Count == 0)
        {
            return bits == 0 ? string.Empty : bits.ToString(CultureInfo.InvariantCulture);
        }

        if (_classExactNames.TryGetValue(bits, out var exact))
        {
            return exact;
        }

        var names = _classNames
            .Where(pair => pair.Key > 0 && (bits & pair.Key) == pair.Key && IsPowerOfTwo(pair.Key))
            .OrderBy(static pair => pair.Key)
            .Select(static pair => pair.Value)
            .ToArray();
        return names.Length == 0 ? bits.ToString(CultureInfo.InvariantCulture) : string.Join(" | ", names);
    }

    private static bool IsPowerOfTwo(ulong value) => value > 0 && (value & (value - 1)) == 0;

    private string BuildItemHeader(TableRowViewModel row)
    {
        var key = row.Key ?? string.Empty;
        var name = row[NameField];
        return name.Length == 0 ? key : $"{key}: {name}";
    }

    private bool IsGroupVisibleForClass(string? groupKey, ulong classBits) =>
        groupKey is null
        || IsSectionVisibleForClass(ClassVisibilitySectionKind.Group, groupKey, classBits);

    private bool IsSectionVisibleForClass(
        ClassVisibilitySectionKind kind,
        string sectionKey,
        ulong classBits)
    {
        var metadata = _set?.Primary.Metadata;
        return metadata is null
            || !metadata.TryGetClassVisibilityRule(kind, sectionKey, out var rule)
            || rule!.IsVisible(classBits);
    }

    private string LabelFor(TableSetMember member)
    {
        var relation = member.Relation;
        var key = !string.IsNullOrWhiteSpace(relation.LabelKey)
            ? relation.LabelKey!
            : relation.Kind == TableRelationKind.Reverse
                ? "ItemSectionReverse." + relation.TargetTable.Value.Replace('.', '_')
                : "ItemSection." + relation.TargetTable.Value.Replace('.', '_');
        var label = Strings.Get(key);
        return string.Equals(label, key, StringComparison.Ordinal)
            ? relation.TargetTable.Value.Split('.').Last()
            : label;
    }

    /// <summary>
    /// Returns the class-visibility key of a related section. Child rows are keyed by table name; a
    /// reverse view of the same table adds the direction field so both directions can differ.
    /// </summary>
    private static string SectionKeyFor(TableRelation relation) =>
        relation.Kind == TableRelationKind.Reverse
            ? relation.TargetTable.Value + "|" + relation.TargetValueField
            : relation.TargetTable.Value;

    private IEnumerable<TableWorkspace> AllWorkspaces()
    {
        if (_set is null)
        {
            return Array.Empty<TableWorkspace>();
        }

        return new[] { _set.Primary }
            .Concat(_set.Members.Select(static member => member.Workspace))
            .DistinctBy(static workspace => workspace.Descriptor.TableName);
    }

    private void OnHistoryChanged(object? sender, EventArgs e)
    {
        if (_set is null)
        {
            return;
        }

        IsDirty = _set.IsDirty;
        RefreshHistoryState();
        foreach (var field in _allFields)
        {
            field.Refresh();
        }

        foreach (var section in ChildSections)
        {
            section.RefreshCount();
        }

        RefreshShopkeeperQuantities();

        if (_itemRow?.Key is { } key && _set.Primary.FindRecord(key) is null)
        {
            RebuildItemList(null);
            return;
        }

        if (_itemRow is not null)
        {
            DetailHeader = BuildItemHeader(_itemRow);
        }
    }

    private void ApplyHistory(bool undo)
    {
        if (_set is null)
        {
            return;
        }

        var selectedKey = _itemRow?.Key;
        try
        {
            var result = undo ? _set.Undo() : _set.Redo();
            if (!result.IsSuccess)
            {
                StatusText = Strings.Format("StatusError", $"{result.ErrorCode}: {result.ErrorText}");
                RefreshHistoryState();
                return;
            }

            IsDirty = _set.IsDirty;
            foreach (var section in ChildSections)
            {
                section.Editor.RefreshRows();
            }

            RebuildItemList(selectedKey);
            RebuildSections();
            RebuildShopkeeperEntries();
            RefreshChildSectionState();
            _ = LoadImagesAsync();
            _ = RefreshGraphicImagesAsync();
            RefreshHistoryState();
            StatusText = Strings.Get(undo ? "EditorUndone" : "EditorRedone");
        }
        catch (InvalidOperationException exception)
        {
            StatusText = Strings.Format("StatusError", exception.Message);
            RefreshHistoryState();
        }
    }

    private void RefreshHistoryState()
    {
        CanUndo = _set?.CanUndo == true;
        CanRedo = _set?.CanRedo == true;
    }

    private void BuildLanguageOptions()
    {
        _suppressLanguageChange = true;
        try
        {
            LanguageOptions.Clear();
            LanguageOptions.Add(new LanguageOptionViewModel(null, false));
            var overlays = _set?.Primary.Descriptor.Overlays ?? Array.Empty<OverlayDescriptor>();
            var canCreateOverlay = _set?.Primary.Metadata is { TranslatableFieldPaths.Count: > 0 };
            foreach (var language in _languages
                         .Where(language =>
                             canCreateOverlay
                             || overlays.Any(overlay => string.Equals(
                                 overlay.Language.Code,
                                 language.Code,
                                 StringComparison.OrdinalIgnoreCase)))
                         .OrderBy(static language => language.Ordinal)
                         .ThenBy(static language => language.DisplayName, StringComparer.Ordinal))
            {
                var hasOverlay = overlays.Any(overlay => string.Equals(
                    overlay.Language.Code,
                    language.Code,
                    StringComparison.OrdinalIgnoreCase));
                LanguageOptions.Add(new LanguageOptionViewModel(language, hasOverlay));
            }

            SelectedLanguageOption = FindLanguageOption(_languageCode)
                ?? LanguageOptions.FirstOrDefault();
        }
        finally
        {
            _suppressLanguageChange = false;
        }
    }

    private IReadOnlyList<LookupOption> BuildClassOptions()
    {
        var options = _classNames
            .Where(static pair => IsPowerOfTwo(pair.Key))
            .OrderBy(static pair => pair.Key)
            .Select(static pair => new LookupOption(
                pair.Key.ToString(CultureInfo.InvariantCulture),
                pair.Value,
                isUnknown: false))
            .ToArray();
        return options.Length > 0
            ? options
            : new[] { new LookupOption("0", string.Empty, isUnknown: false) };
    }

    private void BuildGraphicTypes()
    {
        GraphicTypes.Clear();
        var numbers = new SortedSet<int> { 0 };
        var gameDirectory = _session.GameDataDirectory;
        if (gameDirectory is not null)
        {
            var interfaceDirectory = Path.Combine(gameDirectory, "Interface");
            if (Directory.Exists(interfaceDirectory))
            {
                try
                {
                    foreach (var path in Directory.EnumerateFiles(
                                 interfaceDirectory,
                                 "mdp*.sti",
                                 SearchOption.TopDirectoryOnly))
                    {
                        var name = Path.GetFileNameWithoutExtension(path);
                        var middle = name[3..^"items".Length];
                        if (int.TryParse(middle, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
                            && number >= 0)
                        {
                            numbers.Add(number);
                        }
                    }
                }
                catch (IOException)
                {
                    // A missing or unreadable image directory only limits the picker.
                }
                catch (UnauthorizedAccessException)
                {
                    // A missing or unreadable image directory only limits the picker.
                }
            }
        }

        foreach (var number in numbers)
        {
            GraphicTypes.Add(new GraphicTypeOption(
                number,
                number == 0 ? Strings.Get("GraphicTypeGuns") : Strings.Format("GraphicTypeFormat", number)));
        }
    }

    private void SyncGraphicType()
    {
        _suppressGraphicType = true;
        try
        {
            var raw = _itemRow?.GetStoredValue(GraphicTypeField);
            var value = int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : 0;
            SelectedGraphicType = GraphicTypes.FirstOrDefault(option => option.Number == value)
                ?? GraphicTypes.FirstOrDefault();
        }
        finally
        {
            _suppressGraphicType = false;
        }
    }

    private async Task RefreshGraphicImagesAsync()
    {
        var token = BeginGraphicImageListLoad();
        // Drop the previous type's frames before awaiting, so a stale entry can never write an index that
        // belongs to another sheet while the new count is loading.
        ClearGraphicImages();
        var dataDirectory = _session.GameDataDirectory;
        if (_itemRow is null || dataDirectory is null)
        {
            return;
        }

        var graphicType = GetGraphicType(_itemRow);
        _graphicImageListLoading = true;
        GraphicImageListInfo = Strings.Get("ImageLoading");
        try
        {
            var count = await _itemImages
                .GetFrameCountAsync(dataDirectory, ImageSize.Small, graphicType, token)
                .ConfigureAwait(true);
            token.ThrowIfCancellationRequested();
            if (!count.IsSuccess || count.Data <= 0)
            {
                ClearGraphicImages();
                GraphicImageListInfo = Strings.Format(
                    "ImageNotAvailableFormat",
                    count.ErrorCode ?? string.Empty);
                return;
            }

            var maximum = count.Data - 1;
            var currentIndex = GetGraphicIndex(_itemRow);
            _suppressGraphicImage = true;
            try
            {
                GraphicImages.Clear();
                for (var index = 0; index <= maximum; index++)
                {
                    var option = new ItemImageOption(
                        graphicType,
                        index,
                        Strings.Format("ItemImageOptionFormat", index));
                    option.ConfigureThumbnail(LoadGraphicImageThumbnailAsync);
                    GraphicImages.Add(option);
                }

                SelectedGraphicImage = currentIndex is { } current
                    ? GraphicImages.FirstOrDefault(option => option.Index == current)
                    : null;
            }
            finally
            {
                _suppressGraphicImage = false;
            }

            GraphicIndexMaximum = maximum;
            // A stored index can be outside the sheet after a type change. The value is kept (the editor
            // never rewrites a field the user did not edit) but the chooser says why no frame is selected.
            GraphicImageListInfo = currentIndex is { } staleIndex && staleIndex > maximum
                ? Strings.Format("ItemImageIndexOutOfRangeFormat", staleIndex, maximum)
                : Strings.Format("ItemImageListInfoFormat", count.Data);
        }
        catch (OperationCanceledException)
        {
            // A newer type or item selection replaced this list.
        }
        catch (Exception exception)
        {
            if (!token.IsCancellationRequested)
            {
                ClearGraphicImages();
                GraphicImageListInfo = Strings.Format("StatusError", exception.Message);
            }
        }
        finally
        {
            // A cancelled load belongs to an older type or item; the newer load owns this flag.
            if (!token.IsCancellationRequested)
            {
                _graphicImageListLoading = false;
            }
        }
    }

    private async Task<BitmapSource?> LoadGraphicImageThumbnailAsync(ItemImageOption option)
    {
        var dataDirectory = _session.GameDataDirectory;
        if (dataDirectory is null)
        {
            return null;
        }

        try
        {
            var result = await _itemImages
                .GetImageAsync(
                    dataDirectory,
                    ImageSize.Big,
                    option.GraphicType,
                    option.Index,
                    CancellationToken.None)
                .ConfigureAwait(true);
            return result.Data;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private void SyncGraphicImageSelection()
    {
        var current = _itemRow is null ? null : GetGraphicIndex(_itemRow);
        if (current is { } validIndex
            && GraphicImages.Count > 0
            && validIndex <= GraphicIndexMaximum)
        {
            // A previous type change may have left an out-of-range warning; a valid frame replaces it.
            GraphicImageListInfo = Strings.Format("ItemImageListInfoFormat", GraphicImages.Count);
        }

        var selected = current is { } index
            ? GraphicImages.FirstOrDefault(option => option.Index == index)
            : null;
        if (ReferenceEquals(SelectedGraphicImage, selected))
        {
            return;
        }

        _suppressGraphicImage = true;
        try
        {
            SelectedGraphicImage = selected;
        }
        finally
        {
            _suppressGraphicImage = false;
        }
    }

    private void ClearGraphicImages()
    {
        _suppressGraphicImage = true;
        try
        {
            GraphicImages.Clear();
            SelectedGraphicImage = null;
        }
        finally
        {
            _suppressGraphicImage = false;
        }

        GraphicIndexMaximum = -1;
        GraphicImageListInfo = string.Empty;
        _graphicImageListLoading = false;
    }

    private void CancelGraphicImageListLoad()
    {
        _imageListCts?.Cancel();
        _imageListCts?.Dispose();
        _imageListCts = null;
    }

    private CancellationToken BeginGraphicImageListLoad()
    {
        _imageListCts?.Cancel();
        _imageListCts?.Dispose();
        _imageListCts = new CancellationTokenSource();
        return _imageListCts.Token;
    }

    private async Task LoadImagesAsync()
    {
        var token = BeginImageLoad();
        var dataDirectory = _session.GameDataDirectory;
        if (_itemRow is null || dataDirectory is null)
        {
            ClearImages();
            return;
        }

        if (!TryGetGraphic(_itemRow, out var graphicType, out var graphicIndex))
        {
            foreach (var panel in DetailImages)
            {
                panel.Image = null;
                panel.InfoText = Strings.Get("DetailNoImage");
                panel.IsLoading = false;
            }

            return;
        }

        await Task.WhenAll(DetailImages
                .Select(panel => LoadImageAsync(panel, dataDirectory, graphicType, graphicIndex, token)))
            .ConfigureAwait(true);
    }

    private async Task LoadImageAsync(
        DetailImagePanelViewModel panel,
        string dataDirectory,
        int graphicType,
        int graphicIndex,
        CancellationToken token)
    {
        panel.Image = null;
        panel.InfoText = Strings.Get("EditorLoading");
        panel.IsLoading = true;
        try
        {
            var result = await _itemImages
                .GetImageAsync(dataDirectory, panel.Size, graphicType, graphicIndex, token)
                .ConfigureAwait(true);
            token.ThrowIfCancellationRequested();
            panel.Image = result.Data;
            panel.InfoText = result.Data is null
                ? Strings.Format("ImageNotAvailableFormat", result.ErrorCode ?? string.Empty)
                : $"{result.Data.PixelWidth}×{result.Data.PixelHeight}";
        }
        catch (OperationCanceledException)
        {
            // A newer selection replaced this image load.
        }
        catch (Exception exception)
        {
            if (!token.IsCancellationRequested)
            {
                panel.Image = null;
                panel.InfoText = Strings.Format("StatusError", exception.Message);
            }
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                panel.IsLoading = false;
            }
        }
    }

    private async Task<BitmapSource?> LoadItemThumbnailAsync(TableRowViewModel row)
    {
        var dataDirectory = _session.GameDataDirectory;
        if (dataDirectory is null || !TryGetGraphic(row, out var graphicType, out var graphicIndex))
        {
            return null;
        }

        try
        {
            var result = await _itemImages
                .GetImageAsync(dataDirectory, ImageSize.Small, graphicType, graphicIndex, CancellationToken.None)
                .ConfigureAwait(true);
            return result.Data;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool TryGetGraphic(TableRowViewModel row, out int graphicType, out int graphicIndex)
    {
        graphicType = GetGraphicType(row);
        var index = GetGraphicIndex(row);
        graphicIndex = index ?? -1;
        return index is not null;
    }

    private static int GetGraphicType(TableRowViewModel row)
    {
        var rawType = row.GetStoredValue(GraphicTypeField);
        return int.TryParse(rawType, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedType)
            && parsedType >= 0
            ? parsedType
            : 0;
    }

    private static int? GetGraphicIndex(TableRowViewModel row)
    {
        var rawIndex = row.GetStoredValue(GraphicIndexField);
        return int.TryParse(rawIndex, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedIndex)
            && parsedIndex >= 0
            ? parsedIndex
            : null;
    }

    private void ClearImages()
    {
        foreach (var panel in DetailImages)
        {
            panel.Image = null;
            panel.InfoText = string.Empty;
            panel.IsLoading = false;
        }
    }

    private void CancelImageLoads()
    {
        _imageCts?.Cancel();
        _imageCts?.Dispose();
        _imageCts = null;
    }

    private CancellationToken BeginImageLoad()
    {
        _imageCts?.Cancel();
        _imageCts?.Dispose();
        _imageCts = new CancellationTokenSource();
        return _imageCts.Token;
    }

    private static IReadOnlyDictionary<int, string> BuildBitLabels(LookupColumnData? lookup)
    {
        if (lookup is not { IsAvailable: true })
        {
            return new Dictionary<int, string>();
        }

        var labels = new Dictionary<int, string>();
        foreach (var entry in lookup.Entries)
        {
            if (entry.Id == 0)
            {
                continue;
            }

            var mask = entry.Id;
            if ((mask & (mask - 1)) != 0)
            {
                continue;
            }

            var bit = BitOperations.Log2(mask);
            if (bit < 64)
            {
                labels.TryAdd(bit, entry.Name);
            }
        }

        return labels;
    }

    private void ClearAllSections(bool keepItems)
    {
        if (!keepItems)
        {
            Items.Clear();
            _allItems.Clear();
        }

        ItemGroups.Clear();
        NestedGroups.Clear();
        SubTableSections.Clear();
        ListSections.Clear();
        VisibleChildSections.Clear();
        ValidationIssues.Clear();
        ShopkeeperEntries.Clear();
        ShopkeeperDealerOptions.Clear();
        SelectedShopkeeperDealer = null;
        NotifyShopkeeperChanged();
        _allFields.Clear();
    }
}
