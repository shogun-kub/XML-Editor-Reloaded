using System.Windows;
using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.App.Services;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Dialog that asks whether unsaved changes should be saved, discarded, or kept.
/// </summary>
public partial class UnsavedChangesWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsavedChangesWindow"/> class.
    /// </summary>
    /// <param name="message">Text shown to the user.</param>
    /// <param name="title">Dialog title.</param>
    public UnsavedChangesWindow(string message, string title)
    {
        InitializeComponent();
        Title = title;
        MessageText.Text = message;
        SaveButton.Content = Strings.Get("UnsavedChangesSave");
        DiscardButton.Content = Strings.Get("UnsavedChangesDiscard");
        CancelButton.Content = Strings.Get("UnsavedChangesCancel");
    }

    /// <summary>
    /// Gets the decision selected by the user; <see cref="UnsavedChangesChoice.Cancel"/> when the window
    /// is closed without a decision.
    /// </summary>
    public UnsavedChangesChoice Choice { get; private set; } = UnsavedChangesChoice.Cancel;

    private void OnSave(object sender, RoutedEventArgs e)
    {
        Choice = UnsavedChangesChoice.Save;
        DialogResult = true;
    }

    private void OnDiscard(object sender, RoutedEventArgs e)
    {
        Choice = UnsavedChangesChoice.Discard;
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Choice = UnsavedChangesChoice.Cancel;
        DialogResult = false;
    }
}
