using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One dealer inventory row that stocks the selected item. The quantity is edited in place and written
/// back to the dealer table through the item workspace, so the edit shares the workspace undo history
/// and the next save.
/// </summary>
public sealed partial class ShopkeeperEntryViewModel : ObservableObject
{
    private readonly ItemWorkspaceViewModel _host;
    private string _quantity;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopkeeperEntryViewModel"/> class.
    /// </summary>
    /// <param name="host">Item workspace that applies quantity edits.</param>
    /// <param name="tableName">Dealer inventory table that owns the row.</param>
    /// <param name="rowKey">Primary key of the dealer row.</param>
    /// <param name="dealerName">Display name of the dealer.</param>
    /// <param name="quantity">Current optimal quantity.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public ShopkeeperEntryViewModel(
        ItemWorkspaceViewModel host,
        TableName tableName,
        string rowKey,
        string dealerName,
        string quantity)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(rowKey);
        ArgumentNullException.ThrowIfNull(dealerName);
        ArgumentNullException.ThrowIfNull(quantity);

        _host = host;
        TableName = tableName;
        RowKey = rowKey;
        DealerName = dealerName;
        _quantity = quantity;
    }

    /// <summary>
    /// Gets the dealer inventory table that owns the row.
    /// </summary>
    public TableName TableName { get; }

    /// <summary>
    /// Gets the primary key of the dealer row.
    /// </summary>
    public string RowKey { get; }

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
}
