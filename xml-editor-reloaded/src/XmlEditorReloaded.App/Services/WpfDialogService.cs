using System.IO;
using Microsoft.Win32;

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
    public uint? EditBits(string title, uint value)
    {
        var window = new Views.BitEditorWindow(new ViewModels.BitEditorViewModel(title, value));
        if (System.Windows.Application.Current?.MainWindow is { } owner)
        {
            window.Owner = owner;
        }

        return window.ShowDialog() == true ? window.Value : null;
    }
}
