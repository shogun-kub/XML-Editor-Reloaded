using System.IO;
using Microsoft.Win32;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Services;

/// <summary>
/// WPF implementation of <see cref="IDialogService"/>.
/// </summary>
public sealed class WpfDialogService : IDialogService
{
    /// <inheritdoc />
    public string? PickFolder(string title, string? initialDirectory = null)
    {
        var dialog = new OpenFolderDialog
        {
            Title = title,
            Multiselect = false,
        };

        if (!string.IsNullOrWhiteSpace(initialDirectory) && Directory.Exists(initialDirectory))
        {
            dialog.InitialDirectory = initialDirectory;
        }

        return dialog.ShowDialog() == true ? dialog.FolderName : null;
    }

    /// <inheritdoc />
    public string? PickSaveFile(string title, string filter, string defaultFileName)
    {
        var dialog = new SaveFileDialog
        {
            Title = title,
            Filter = filter,
            FileName = defaultFileName,
            AddExtension = true,
            OverwritePrompt = true,
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <inheritdoc />
    public bool Confirm(string message, string title)
    {
        var result = System.Windows.MessageBox.Show(
            message,
            title,
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question,
            System.Windows.MessageBoxResult.No);
        return result == System.Windows.MessageBoxResult.Yes;
    }

    /// <inheritdoc />
    public uint? EditBits(
        string title,
        uint value,
        IReadOnlyDictionary<int, string>? labels = null,
        string? valueName = null)
    {
        var window = new Views.BitEditorWindow(
            new ViewModels.BitEditorViewModel(title, value, labels, valueName));
        if (System.Windows.Application.Current?.MainWindow is { } owner)
        {
            window.Owner = owner;
        }

        return window.ShowDialog() == true ? window.Value : null;
    }

    /// <inheritdoc />
    public void ShowTranslationReport(TranslationReport report, string title)
    {
        ArgumentNullException.ThrowIfNull(report);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        var window = new Views.TranslationReportWindow(
            new ViewModels.TranslationReportViewModel(report, title));
        if (System.Windows.Application.Current?.MainWindow is { } owner)
        {
            window.Owner = owner;
        }

        window.ShowDialog();
    }

    /// <inheritdoc />
    public BulkEditRequest? EditBulk(BulkEditContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var window = new Views.BulkEditWindow(new ViewModels.BulkEditViewModel(context));
        if (System.Windows.Application.Current?.MainWindow is { } owner)
        {
            window.Owner = owner;
        }

        return window.ShowDialog() == true ? window.Request : null;
    }
}
