using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One dealer inventory row that stocks the selected item. The quantity is edited in place and written
/// back to the exact dealer row through the item workspace, so an edit or removal keeps addressing the
/// same row even when another dealer row repeats its <c>uiIndex</c>. The edit shares the workspace undo
/// history and the next save.
/// </summary>
public sealed partial class ShopkeeperEntryViewModel : ObservableObject
{
    private readonly ItemWorkspaceViewModel _host;
    private string _quantity;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopkeeperEntryViewModel"/> class.
    /// </summary>
    /// <param name="host">Item workspace that applies quantity edits and removals.</param>
    /// <param name="tableName">Dealer inventory table that owns the row.</param>
    /// <param name="row">Exact workspace row that stores the item reference and quantity.</param>
    /// <param name="dealerName">Display name of the dealer.</param>
    /// <param name="quantity">Current optimal quantity.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public ShopkeeperEntryViewModel(
        ItemWorkspaceViewModel host,
        TableName tableName,
        WorkspaceRow row,
        string dealerName,
        string quantity)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(dealerName);
        ArgumentNullException.ThrowIfNull(quantity);

        _host = host;
        TableName = tableName;
        Row = row;
        DealerName = dealerName;
        _quantity = quantity;
    }

    /// <summary>
    /// Gets the dealer inventory table that owns the row.
    /// </summary>
    public TableName TableName { get; }

    /// <summary>
    /// Gets the exact workspace row that stores the dealer stock entry. Unlike a key lookup, this
    /// reference cannot be confused with another row that repeats the same <c>uiIndex</c>.
    /// </summary>
    public WorkspaceRow Row { get; }

    /// <summary>
    /// Gets the primary key of the dealer row as displayed in status messages.
    /// </summary>
    public string RowKey => Row.Key ?? string.Empty;

    /// <summary>
    /// Gets the display name of the dealer.
    /// </summary>
    public string DealerName { get; }

    /// <summary>
    /// Gets or sets the optimal quantity. Setting the value writes it to the dealer row.
    /// </summary>
    public string Quantity
    {
        get => _quantity;
        set
        {
            if (!SetProperty(ref _quantity, value))
            {
                return;
            }

            _host.SetShopkeeperQuantity(this, value);
        }
    }

    /// <summary>
    /// Replaces the displayed quantity after an undo, reload, or successful edit without writing back.
    /// </summary>
    /// <param name="quantity">Stored quantity.</param>
    internal void Refresh(string quantity) => SetProperty(ref _quantity, quantity, nameof(Quantity));

    /// <summary>
    /// Removes this exact dealer row after the item workspace confirms the action.
    /// </summary>
    [RelayCommand]
    private void Remove() => _host.RemoveShopkeeperEntry(this);
}
