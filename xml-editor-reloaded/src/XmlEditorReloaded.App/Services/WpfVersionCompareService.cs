using System.Windows;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// WPF implementation of <see cref="IVersionCompareService"/>.
/// </summary>
public sealed class WpfVersionCompareService : IVersionCompareService
{
    private readonly CompareViewModel _viewModel;
    private CompareWindow? _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfVersionCompareService"/> class.
    /// </summary>
    /// <param name="viewModel">Comparison view model shared by the window.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public WpfVersionCompareService(CompareViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        _viewModel = viewModel;
    }

    /// <inheritdoc />
    public void Show(string? initialDirectory)
    {
        _viewModel.SetInitialDirectory(initialDirectory);
        if (_window is null)
        {
            _window = new CompareWindow(_viewModel);
            _window.Closed += (_, _) => _window = null;
            if (Application.Current?.MainWindow is { } owner)
            {
                _window.Owner = owner;
            }
        }

        _window.Show();
        _window.Activate();
    }
}
