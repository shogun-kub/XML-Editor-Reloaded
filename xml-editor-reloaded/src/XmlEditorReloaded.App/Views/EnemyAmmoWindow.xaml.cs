using System.ComponentModel;
using System.Windows;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Seven-row enemy and militia ammunition editor. Closing asks about unsaved changes and never discards
/// them silently.
/// </summary>
public partial class EnemyAmmoWindow : Window
{
    private readonly EnemyAmmoViewModel _viewModel;
    private readonly IDialogService _dialogService;
    private bool _allowClose;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnemyAmmoWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Loaded enemy-ammo view model.</param>
    /// <param name="dialogService">Dialog service used for the close prompt.</param>
    /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
    public EnemyAmmoWindow(EnemyAmmoViewModel viewModel, IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(dialogService);

        _viewModel = viewModel;
        _dialogService = dialogService;
        InitializeComponent();
        DataContext = viewModel;
        Closing += OnClosing;
    }

    private async void OnClosing(object? sender, CancelEventArgs e)
    {
        if (_allowClose || !_viewModel.IsDirty)
        {
            return;
        }

        e.Cancel = true;
        var choice = _dialogService.ConfirmUnsavedChanges(
            Strings.Get("EnemyAmmoClosePrompt"),
            _viewModel.Title);
        switch (choice)
        {
            case UnsavedChangesChoice.Save:
                if (await _viewModel.SaveAsync())
                {
                    _allowClose = true;
                    Close();
                }

                break;

            case UnsavedChangesChoice.Discard:
                _allowClose = true;
                Close();
                break;
        }
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
}
