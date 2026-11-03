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

    // A virtualizing list reuses its containers, so Loaded fires only for the first item a container
    // shows. The recycled container receives a new data context without another Loaded event; request
    // the thumbnail here as well, otherwise scrolled-in items keep an empty image.
    private void OnItemThumbnailDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ItemListEntry entry })
        {
            entry.RequestThumbnail();
        }
    }

    private void OnItemListSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox { SelectedItem: not null } listBox)
        {
            listBox.ScrollIntoView(listBox.SelectedItem);
        }
    }

    private void OnGraphicImageThumbnailLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ItemImageOption option })
        {
            option.RequestThumbnail();
        }
    }

    private void OnGraphicImageThumbnailDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ItemImageOption option })
        {
            option.RequestThumbnail();
        }
    }
}
