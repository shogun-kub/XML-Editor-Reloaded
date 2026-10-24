using System.Windows;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// WPF implementation of <see cref="ISettingsDialogService"/>.
/// </summary>
public sealed class WpfSettingsDialogService : ISettingsDialogService
{
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfSettingsDialogService"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service used by the settings view model for folder picking.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogService"/> is null.</exception>
    public WpfSettingsDialogService(IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(dialogService);
        _dialogService = dialogService;
    }

    /// <inheritdoc />
    public SettingsResult? Show(SettingsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var window = new Views.SettingsWindow(new SettingsViewModel(request, _dialogService));
        if (Application.Current?.MainWindow is { } owner)
        {
            window.Owner = owner;
        }

        return window.ShowDialog() == true ? window.Result : null;
    }
}
