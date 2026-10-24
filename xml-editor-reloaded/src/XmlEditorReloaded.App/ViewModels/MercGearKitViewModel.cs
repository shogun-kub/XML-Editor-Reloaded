using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One mercenary in the gear list.
/// </summary>
/// <param name="Key">Primary record key, the <c>mIndex</c> value.</param>
/// <param name="Name">Mercenary name from <c>mName</c>.</param>
public sealed record MercGearEntryViewModel(string Key, string Name)
{
    /// <summary>
    /// Gets the text shown in the mercenary list.
    /// </summary>
    public string DisplayText => Name.Length == 0 ? Key : $"{Key}: {Name}";
}

/// <summary>
/// Editable state of one gear kit slot, for example a helmet or the first big item slot.
/// </summary>
public sealed partial class MercGearSlotViewModel : ObservableObject
{
    private readonly MercGearWorkspaceViewModel _host;
    private readonly int _kitIndex;
    private string _itemId;
    private string _status;
    private string _quantity;
    private bool _isDrop;
    private bool _suppress;

    /// <summary>
    /// Initializes a new instance of the <see cref="MercGearSlotViewModel"/> class.
    /// </summary>
    /// <param name="host">Workspace that applies edits.</param>
    /// <param name="kitIndex">Zero-based index of the gear kit.</param>
    /// <param name="itemField">XML field that stores the item identifier.</param>
    /// <param name="statusField">XML field that stores the item status.</param>
    /// <param name="dropField">XML field that stores the drop flag, or <see langword="null"/> when the slot has none.</param>
    /// <param name="quantityField">XML field that stores the quantity, or <see langword="null"/> when the slot has none.</param>
    /// <param name="label">Localized slot label.</param>
    /// <param name="relation">Optional item picker data.</param>
    /// <param name="state">Current slot values.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public MercGearSlotViewModel(
        MercGearWorkspaceViewModel host,
        int kitIndex,
        string itemField,
        string statusField,
        string? dropField,
        string? quantityField,
        string label,
        RelationColumnData? relation,
        MercGearSlotState state)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(itemField);
        ArgumentNullException.ThrowIfNull(statusField);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(state);

        _host = host;
        _kitIndex = kitIndex;
        ItemField = itemField;
        StatusField = statusField;
        DropField = dropField;
        QuantityField = quantityField;
        Label = label;
        Relation = relation;
        _itemId = state.ItemId;
        _status = state.Status;
        _quantity = state.Quantity;
        _isDrop = state.IsDrop;
    }

    /// <summary>
    /// Gets the XML field that stores the item identifier.
    /// </summary>
    public string ItemField { get; }

    /// <summary>
    /// Gets the XML field that stores the item status.
    /// </summary>
    public string StatusField { get; }

    /// <summary>
    /// Gets the XML field that stores the drop flag, or <see langword="null"/> when the slot has none.
    /// </summary>
    public string? DropField { get; }

    /// <summary>
    /// Gets the XML field that stores the quantity, or <see langword="null"/> when the slot has none.
    /// </summary>
    public string? QuantityField { get; }

    /// <summary>
    /// Gets the localized slot label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the item picker data, or <see langword="null"/> when no relation is configured.
    /// </summary>
    public RelationColumnData? Relation { get; }

    /// <summary>
    /// Gets a value indicating whether the slot uses a searchable item picker.
    /// </summary>
    public bool HasRelation => Relation is { IsAvailable: true };

    /// <summary>
    /// Gets a value indicating whether the slot is edited as plain numeric text.
    /// </summary>
    public bool IsPlainValue => !HasRelation;

    /// <summary>
    /// Gets a value indicating whether the slot has a quantity field.
    /// </summary>
    public bool SupportsQuantity => QuantityField is not null;

    /// <summary>
    /// Gets a value indicating whether the slot has a drop flag.
    /// </summary>
    public bool SupportsDrop => DropField is not null;

    /// <summary>
    /// Gets or sets the raw item identifier. The value is written to the selected gear kit.
    /// </summary>
    public string ItemId
    {
        get => _itemId;
        set
        {
            if (_suppress || !SetProperty(ref _itemId, value))
            {
                return;
            }

            _host.SetKitField(_kitIndex, ItemField, value);
        }
    }

    /// <summary>
    /// Gets or sets the item status. The value is written to the selected gear kit.
    /// </summary>
    public string Status
    {
        get => _status;
        set
        {
            if (_suppress || !SetProperty(ref _status, value))
            {
                return;
            }

            _host.SetKitField(_kitIndex, StatusField, value);
        }
    }

    /// <summary>
    /// Gets or sets the item quantity. The value is written to the selected gear kit.
    /// </summary>
    public string Quantity
    {
        get => _quantity;
        set
        {
            if (_suppress || QuantityField is null || !SetProperty(ref _quantity, value))
            {
                return;
            }

            _host.SetKitField(_kitIndex, QuantityField, value);
        }
    }

    /// <summary>
    /// Gets or sets the drop flag as <c>1</c> or <c>0</c>. The value is written to the selected gear kit.
    /// </summary>
    public bool IsDrop
    {
        get => _isDrop;
        set
        {
            if (_suppress || DropField is null || !SetProperty(ref _isDrop, value))
            {
                return;
            }

            _host.SetKitField(_kitIndex, DropField, value ? "1" : "0");
        }
    }

    /// <summary>
    /// Replaces the displayed values after an edit, undo, or reload without writing back.
    /// </summary>
    /// <param name="state">Stored slot values.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="state"/> is null.</exception>
    internal void Refresh(MercGearSlotState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _suppress = true;
        try
        {
            SetProperty(ref _itemId, state.ItemId, nameof(ItemId));
            SetProperty(ref _status, state.Status, nameof(Status));
            SetProperty(ref _quantity, state.Quantity, nameof(Quantity));
            SetProperty(ref _isDrop, state.IsDrop, nameof(IsDrop));
        }
        finally
        {
            _suppress = false;
        }
    }
}

/// <summary>
/// Stored values of one gear slot.
/// </summary>
/// <param name="ItemId">Raw item identifier, or an empty string when absent.</param>
/// <param name="Status">Raw status, or an empty string when absent.</param>
/// <param name="Quantity">Raw quantity, or an empty string when absent.</param>
/// <param name="IsDrop">Whether the drop flag is set.</param>
public sealed record MercGearSlotState(string ItemId, string Status, string Quantity, bool IsDrop);

/// <summary>
/// Stored values of one gear kit.
/// </summary>
/// <param name="Name">Effective kit name: the translation when present, otherwise the base value.</param>
/// <param name="IsNameTranslated">Whether the selected overlay contains the kit name.</param>
/// <param name="PriceMod">Raw price modifier, or an empty string when absent.</param>
/// <param name="AbsolutePrice">Raw absolute price, or an empty string when absent.</param>
/// <param name="Slots">Slot values in display order.</param>
public sealed record MercGearKitState(
    string Name,
    bool IsNameTranslated,
    string PriceMod,
    string AbsolutePrice,
    IReadOnlyList<MercGearSlotState> Slots);

/// <summary>
/// One of the five gear kits of a mercenary. Kit names are translatable and are written to the language
/// overlay; item, status, quantity, drop, and price fields stay in the base table and are written to the
/// selected <c>GEARKIT</c> occurrence.
/// </summary>
public sealed partial class MercGearKitViewModel : ObservableObject
{
    private readonly MercGearWorkspaceViewModel _host;
    private string _name;
    private string _priceMod;
    private string _absolutePrice;
    private bool _isNameTranslated;
    private bool _suppress;

    /// <summary>
    /// Initializes a new instance of the <see cref="MercGearKitViewModel"/> class.
    /// </summary>
    /// <param name="host">Workspace that applies edits.</param>
    /// <param name="index">Zero-based index of the kit among the mercenary's <c>GEARKIT</c> entries.</param>
    /// <param name="state">Current kit values.</param>
    /// <param name="slots">Slot view models in display order.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public MercGearKitViewModel(
        MercGearWorkspaceViewModel host,
        int index,
        MercGearKitState state,
        IReadOnlyList<MercGearSlotViewModel> slots)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(slots);

        _host = host;
        Index = index;
        _name = state.Name;
        _isNameTranslated = state.IsNameTranslated;
        _priceMod = state.PriceMod;
        _absolutePrice = state.AbsolutePrice;
        Slots = slots;
    }

    /// <summary>
    /// Gets the zero-based index of the kit among the mercenary's <c>GEARKIT</c> entries.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the kit tab header.
    /// </summary>
    public string Header => Resources.Strings.Format("MercGearKitTabFormat", Index + 1);

    /// <summary>
    /// Gets the localized kit name label.
    /// </summary>
    public string NameLabel => Resources.Strings.Get("MercGearKitNameLabel");

    /// <summary>
    /// Gets the localized price modifier label.
    /// </summary>
    public string PriceModLabel => Resources.Strings.Get("MercGearPriceModLabel");

    /// <summary>
    /// Gets the localized absolute price label.
    /// </summary>
    public string AbsolutePriceLabel => Resources.Strings.Get("MercGearAbsolutePriceLabel");

    /// <summary>
    /// Gets the columns of the slot list.
    /// </summary>
    public MercGearColumnHeaders Headers => MercGearColumnHeaders.Instance;

    /// <summary>
    /// Gets the editable slots of the kit.
    /// </summary>
    public IReadOnlyList<MercGearSlotViewModel> Slots { get; }

    /// <summary>
    /// Gets or sets the effective kit name. The value is written to the overlay when a language is
    /// selected, otherwise to the base table.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (_suppress || !SetProperty(ref _name, value))
            {
                return;
            }

            _host.SetKitName(this, value);
        }
    }

    /// <summary>
    /// Gets or sets the raw price modifier.
    /// </summary>
    public string PriceMod
    {
        get => _priceMod;
        set
        {
            if (_suppress || !SetProperty(ref _priceMod, value))
            {
                return;
            }

            _host.SetKitField(Index, "mPriceMod", value);
        }
    }

    /// <summary>
    /// Gets or sets the raw absolute price.
    /// </summary>
    public string AbsolutePrice
    {
        get => _absolutePrice;
        set
        {
            if (_suppress || !SetProperty(ref _absolutePrice, value))
            {
                return;
            }

            _host.SetKitField(Index, "mAbsolutePrice", value);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the selected overlay contains the kit name.
    /// </summary>
    public bool IsNameTranslated
    {
        get => _isNameTranslated;
        private set => SetProperty(ref _isNameTranslated, value);
    }

    /// <summary>
    /// Gets a value indicating whether the untranslated marker should be visible.
    /// </summary>
    public bool ShowNameMarker => _host.IsTranslationMode && !IsNameTranslated;

    /// <summary>
    /// Gets the untranslated marker text.
    /// </summary>
    public string NameMarker => ShowNameMarker ? "*" : string.Empty;

    /// <summary>
    /// Gets the tooltip of the untranslated marker.
    /// </summary>
    public string NameMarkerTooltip => Resources.Strings.Get("EditorUntranslated");

    /// <summary>
    /// Replaces the displayed values after an edit, undo, or reload without writing back.
    /// </summary>
    /// <param name="state">Stored kit values.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="state"/> is null.</exception>
    internal void Refresh(MercGearKitState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        _suppress = true;
        try
        {
            SetProperty(ref _name, state.Name, nameof(Name));
            SetProperty(ref _priceMod, state.PriceMod, nameof(PriceMod));
            SetProperty(ref _absolutePrice, state.AbsolutePrice, nameof(AbsolutePrice));
            IsNameTranslated = state.IsNameTranslated;
            var count = Math.Min(Slots.Count, state.Slots.Count);
            for (var index = 0; index < count; index++)
            {
                Slots[index].Refresh(state.Slots[index]);
            }

            OnPropertyChanged(nameof(ShowNameMarker));
            OnPropertyChanged(nameof(NameMarker));
        }
        finally
        {
            _suppress = false;
        }
    }
}

/// <summary>
/// Localized column headers shared by every gear kit.
/// </summary>
public sealed class MercGearColumnHeaders
{
    /// <summary>
    /// Gets the shared header instance.
    /// </summary>
    public static MercGearColumnHeaders Instance { get; } = new();

    private MercGearColumnHeaders()
    {
    }

    /// <summary>
    /// Gets the item column header.
    /// </summary>
    public string Item => Resources.Strings.Get("MercGearItemColumn");

    /// <summary>
    /// Gets the status column header.
    /// </summary>
    public string Status => Resources.Strings.Get("MercGearStatusColumn");

    /// <summary>
    /// Gets the quantity column header.
    /// </summary>
    public string Quantity => Resources.Strings.Get("MercGearQuantityColumn");

    /// <summary>
    /// Gets the drop column header.
    /// </summary>
    public string Drop => Resources.Strings.Get("MercGearDropColumn");
}
