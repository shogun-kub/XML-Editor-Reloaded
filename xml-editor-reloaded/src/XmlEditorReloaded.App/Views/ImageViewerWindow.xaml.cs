using System.Windows;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Standalone window that previews big, medium, and small item images.
/// </summary>
public partial class ImageViewerWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageViewerWindow"/> class.
    /// </summary>
    /// <param name="viewModel">View model that supplies items and previews.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public ImageViewerWindow(ImageViewerViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
    }
}
