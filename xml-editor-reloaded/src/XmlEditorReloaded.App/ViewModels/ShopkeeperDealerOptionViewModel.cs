using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One dealer inventory that does not stock the selected item yet. The shopkeeper tab offers these
/// tables as targets for a new item-centric dealer row.
/// </summary>
public sealed class ShopkeeperDealerOptionViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShopkeeperDealerOptionViewModel"/> class.
    /// </summary>
    /// <param name="tableName">Dealer inventory table that would own the new row.</param>
    /// <param name="dealerName">Display name of the dealer.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dealerName"/> is null.</exception>
    public ShopkeeperDealerOptionViewModel(TableName tableName, string dealerName)
    {
        ArgumentNullException.ThrowIfNull(dealerName);
        TableName = tableName;
        DealerName = dealerName;
    }

    /// <summary>
    /// Gets the dealer inventory table that would own the new row.
    /// </summary>
    public TableName TableName { get; }

    /// <summary>
    /// Gets the display name of the dealer.
    /// </summary>
    public string DealerName { get; }
}
