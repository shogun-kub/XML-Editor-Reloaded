using System.Windows;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Read-only About window showing the assembly product, version, description, and optional copyright.
/// </summary>
public partial class AboutWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutWindow"/> class.
    /// </summary>
    /// <param name="viewModel">Content of the window.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
    public AboutWindow(AboutViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        InitializeComponent();
        DataContext = viewModel;
        Title = viewModel.Title;
    }
}
