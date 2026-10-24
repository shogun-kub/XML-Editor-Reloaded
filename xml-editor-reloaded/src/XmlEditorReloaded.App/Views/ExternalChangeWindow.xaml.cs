using System.Windows;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Dialog that asks how to resolve a table file that changed on disk after it was loaded.
/// </summary>
public partial class ExternalChangeWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalChangeWindow"/> class.
    /// </summary>
    /// <param name="message">Text shown to the user, usually with the failed file names.</param>
    /// <param name="title">Dialog title.</param>
    public ExternalChangeWindow(string message, string title)
    {
        InitializeComponent();
        Title = title;
        MessageText.Text = message;
        ReloadButton.Content = Strings.Get("ExternalChangeReload");
        OverwriteButton.Content = Strings.Get("ExternalChangeOverwrite");
        CancelButton.Content = Strings.Get("ExternalChangeCancel");
    }

    /// <summary>
    /// Gets the decision selected by the user; <see cref="ExternalChangeChoice.Cancel"/> when the window
    /// is closed without a decision.
    /// </summary>
    public ExternalChangeChoice Choice { get; private set; } = ExternalChangeChoice.Cancel;

    private void OnReload(object sender, RoutedEventArgs e)
    {
        Choice = ExternalChangeChoice.Reload;
        DialogResult = true;
    }

    private void OnOverwrite(object sender, RoutedEventArgs e)
    {
        Choice = ExternalChangeChoice.Overwrite;
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Choice = ExternalChangeChoice.Cancel;
        DialogResult = false;
    }
}
