using System.Windows.Controls;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Editable row of one detail field: translation marker, name, editor, target navigation, and the bit
/// editor. The control is shared by the table detail panel and the item workspace.
/// </summary>
public partial class FieldEditor : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldEditor"/> class.
    /// </summary>
    public FieldEditor()
    {
        InitializeComponent();
    }
}
