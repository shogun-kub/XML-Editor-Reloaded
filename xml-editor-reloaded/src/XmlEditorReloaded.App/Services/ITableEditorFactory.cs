using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Creates <see cref="ViewModels.TableEditorViewModel"/> instances for table tabs.
/// </summary>
public interface ITableEditorFactory
{
    /// <summary>
    /// Creates an uninitialized editor for a table; call
    /// <see cref="ViewModels.TableEditorViewModel.InitializeAsync"/> before displaying it.
    /// </summary>
    /// <param name="session">Data-set session that owns the table.</param>
    /// <param name="descriptor">Table descriptor to open.</param>
    /// <param name="languageCode">Optional language code for overlay editing.</param>
    /// <returns>The editor view model.</returns>
    ViewModels.TableEditorViewModel Create(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode);
}

/// <summary>
/// Default <see cref="ITableEditorFactory"/> that uses the application dialog service.
/// </summary>
public sealed class TableEditorFactory : ITableEditorFactory
{
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableEditorFactory"/> class.
    /// </summary>
    /// <param name="dialogService">Dialog service used for editor confirmations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogService"/> is null.</exception>
    public TableEditorFactory(IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(dialogService);
        _dialogService = dialogService;
    }

    /// <inheritdoc />
    public ViewModels.TableEditorViewModel Create(
        IDataSetSession session,
        TableDescriptor descriptor,
        string? languageCode) =>
        new(session, descriptor, languageCode, _dialogService);
}
