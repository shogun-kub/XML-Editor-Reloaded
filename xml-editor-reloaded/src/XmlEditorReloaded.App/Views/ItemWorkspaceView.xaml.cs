using System.Windows;
using System.Windows.Controls;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Item master-detail workspace: selector, item form, sub-tables, child tables, images, and validation.
/// </summary>
public partial class ItemWorkspaceView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemWorkspaceView"/> class.
    /// </summary>
    public ItemWorkspaceView()
    {
        InitializeComponent();
    }

    private void OnItemThumbnailLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ItemListEntry entry })
        {
            entry.RequestThumbnail();
        }
    }
}
