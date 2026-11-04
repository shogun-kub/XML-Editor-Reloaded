using XmlEditorReloaded.Core.Layout;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Lookups;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Creates <see cref="ViewModels.TableEditorViewModel"/> instances for table tabs.
/// </summary>
public interface ITableEditorFactory
{
    /// <summary>
    /// Creates an uninitialized editor for a table; call
    /// <see cref="ViewModels.TableEditorViewModel.InitializeAsync"/> before displaying it.
    /// </summary>
    /// <param name="session">Data-set session that owns the table.</param>
    /// <param name="descriptor">Table descriptor to open.</param>
    /// <param name="languageCode">Optional language code for overlay editing.</param>
    /// <returns>The editor view model.</returns>
    ViewModels.TableEditorViewModel Create(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode);

    /// <summary>
    /// Creates an uninitialized item master-detail workspace; call
    /// <see cref="ViewModels.ItemWorkspaceViewModel.InitializeAsync"/> before displaying it.
    /// </summary>
    /// <param name="session">Data-set session that owns the table set.</param>
    /// <param name="descriptor">Descriptor of the <c>Items.Items</c> table.</param>
    /// <param name="languageCode">Optional language code for overlay editing.</param>
    /// <returns>The item workspace view model.</returns>
    ViewModels.ItemWorkspaceViewModel CreateItemWorkspace(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode);

    /// <summary>
    /// Creates an uninitialized mercenary starting-gear workspace; call
    /// <see cref="ViewModels.MercGearWorkspaceViewModel.InitializeAsync"/> before displaying it.
    /// </summary>
    /// <param name="session">Data-set session that owns the gear table.</param>
    /// <param name="descriptor">Descriptor of the <c>Inventory.MercStartingGear</c> table.</param>
    /// <param name="languageCode">Optional language code for overlay editing.</param>
    /// <returns>The mercenary gear workspace view model.</returns>
    ViewModels.MercGearWorkspaceViewModel CreateMercGearWorkspace(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode);

    /// <summary>
    /// Creates an uninitialized nested-record workspace for tables such as <c>Map.Cities</c> and
    /// <c>Map.FacilityTypes</c>; call <see cref="ViewModels.NestedTableWorkspaceViewModel.InitializeAsync"/>
    /// before displaying it.
    /// </summary>
    /// <param name="session">Data-set session that owns the table.</param>
    /// <param name="descriptor">Table descriptor to open.</param>
    /// <param name="languageCode">Optional language code for overlay editing.</param>
    /// <returns>The nested-record workspace view model.</returns>
    ViewModels.NestedTableWorkspaceViewModel CreateNestedWorkspace(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode);
}

/// <summary>
/// Default <see cref="ITableEditorFactory"/> that uses the application dialog service.
/// </summary>
public sealed class TableEditorFactory : ITableEditorFactory
{
    private readonly IDialogService _dialogService;
    private readonly IItemImageService _itemImages;
    private readonly ITableLayoutStore _layoutStore;
    private readonly LookupUsageService? _lookupUsage;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableEditorFactory"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service used for editor confirmations and bit editing.</param>
    /// <param name="itemImages">Item image service used by the detail panel.</param>
    /// <param name="layoutStore">Per-table layout store used by generic grid tabs.</param>
    /// <param name="lookupUsage">Optional scanner used to protect referenced lookup values.</param>
    /// <exception cref="ArgumentNullException">Thrown when a dependency is null.</exception>
    public TableEditorFactory(
        IDialogService dialogService,
        IItemImageService itemImages,
        ITableLayoutStore layoutStore,
        LookupUsageService? lookupUsage = null)
    {
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(itemImages);
        ArgumentNullException.ThrowIfNull(layoutStore);
        _dialogService = dialogService;
        _itemImages = itemImages;
        _layoutStore = layoutStore;
        _lookupUsage = lookupUsage;
    }

    /// <inheritdoc />
    public ViewModels.TableEditorViewModel Create(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode) =>
        new(session, descriptor, languageCode, _dialogService, _itemImages, _layoutStore, lookupUsage: _lookupUsage);

    /// <inheritdoc />
    public ViewModels.ItemWorkspaceViewModel CreateItemWorkspace(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode) =>
        new(session, descriptor, languageCode, _dialogService, _itemImages);

    /// <inheritdoc />
    public ViewModels.MercGearWorkspaceViewModel CreateMercGearWorkspace(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode) =>
        new(session, descriptor, languageCode, _dialogService);

    /// <inheritdoc />
    public ViewModels.NestedTableWorkspaceViewModel CreateNestedWorkspace(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode) =>
        new(session, descriptor, languageCode, _dialogService);
}
