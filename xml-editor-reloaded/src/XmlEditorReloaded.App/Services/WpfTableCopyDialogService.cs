using System.Windows;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Xml.Copy;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// WPF implementation of <see cref="ITableCopyDialogService"/>.
/// </summary>
public sealed class WpfTableCopyDialogService : ITableCopyDialogService
{
    private readonly ITableCopyService _copyService;
    private readonly ITableCatalog _catalog;
    private readonly IDialogService _dialogService;
    private readonly ITableMetadataProvider? _metadataProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfTableCopyDialogService"/> class.
    /// </summary>
    /// <param name="copyService">Copy service used by the dialog.</param>
    /// <param name="catalog">Catalog used to list tables common to both data sets.</param>
    /// <param name="dialogService">Dialog service used by the browse buttons.</param>
    /// <param name="metadataProvider">
    /// Optional metadata provider used to disable key allocation for compound-key tables.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when a dependency is null.</exception>
    public WpfTableCopyDialogService(
        ITableCopyService copyService,
        ITableCatalog catalog,
        IDialogService dialogService,
        ITableMetadataProvider? metadataProvider = null)
    {
        ArgumentNullException.ThrowIfNull(copyService);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(dialogService);

        _copyService = copyService;
        _catalog = catalog;
        _dialogService = dialogService;
        _metadataProvider = metadataProvider;
    }

    /// <inheritdoc />
    public async Task<TableCopyDialogOutcome?> ShowAsync(
        TableCopyDialogRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var viewModel = new TableCopyViewModel(
            _copyService,
            _catalog,
            _dialogService,
            request,
            _metadataProvider);
        await viewModel.InitializeAsync(cancellationToken).ConfigureAwait(true);

        var window = new TableCopyWindow(viewModel);
        if (Application.Current?.MainWindow is { } owner && !ReferenceEquals(owner, window))
        {
            window.Owner = owner;
        }

        return window.ShowDialog() == true ? viewModel.Outcome : null;
    }
}
