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
}
