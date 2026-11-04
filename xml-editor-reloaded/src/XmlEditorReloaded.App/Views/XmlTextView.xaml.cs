using System.Windows;
using System.Windows.Controls;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Shared read-only XML presentation for every table tab.
/// </summary>
public partial class XmlTextView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlTextView"/> class.
    /// </summary>
    public XmlTextView() => InitializeComponent();

    private void OnCopyXmlClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel { XmlText.Length: > 0 } viewModel)
        {
            Clipboard.SetText(viewModel.XmlText);
        }
    }
}
