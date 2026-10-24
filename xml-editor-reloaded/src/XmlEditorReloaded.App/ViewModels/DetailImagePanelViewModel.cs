using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using XmlEditorReloaded.Core.Imaging;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One preview panel in the item detail form, for example the big or small STI image.
/// </summary>
public sealed partial class DetailImagePanelViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DetailImagePanelViewModel"/> class.
    /// </summary>
    /// <param name="size">Image size class requested from the image catalog.</param>
    /// <param name="label">Localized panel label.</param>
    /// <param name="panelHeight">Height of the image viewport in device-independent pixels.</param>
    public DetailImagePanelViewModel(ImageSize size, string label, double panelHeight)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        Size = size;
        Label = label;
        PanelHeight = panelHeight;
    }

    /// <summary>
    /// Gets the image size class.
    /// </summary>
    public ImageSize Size { get; }

    /// <summary>
    /// Gets the localized panel label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the height of the image viewport in device-independent pixels.
    /// </summary>
    public double PanelHeight { get; }

    /// <summary>
    /// Gets or sets the decoded bitmap, or <see langword="null"/> while loading or when the image is absent.
    /// </summary>
    [ObservableProperty]
    private BitmapSource? _image;

    /// <summary>
    /// Gets or sets the size or error text shown next to the panel label.
    /// </summary>
    [ObservableProperty]
    private string _infoText = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the panel is waiting for the decoder.
    /// </summary>
    [ObservableProperty]
    private bool _isLoading;
}
