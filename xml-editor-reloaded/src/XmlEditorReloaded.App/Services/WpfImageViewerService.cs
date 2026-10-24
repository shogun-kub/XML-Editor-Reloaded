using System.Windows;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// WPF implementation of <see cref="IImageViewerService"/>.
/// </summary>
public sealed class WpfImageViewerService : IImageViewerService
{
    private readonly ImageViewerViewModel _viewModel;
    private ImageViewerWindow? _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfImageViewerService"/> class.
    /// </summary>
    /// <param name="viewModel">View model shared by all viewer windows.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public WpfImageViewerService(ImageViewerViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
    }

    /// <inheritdoc />
    public void Show(string? dataDirectory)
    {
        if (_window is null)
        {
            _window = new ImageViewerWindow(_viewModel);
            _window.Closed += (_, _) => _window = null;
            if (Application.Current?.MainWindow is { } owner)
            {
                _window.Owner = owner;
            }
        }

        _window.Show();
        _window.Activate();
        _ = _viewModel.InitializeAsync(dataDirectory);
    }
}
