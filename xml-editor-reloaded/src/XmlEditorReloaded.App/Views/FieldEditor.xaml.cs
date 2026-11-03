using System.Windows;
using System.Windows.Controls;
using XmlEditorReloaded.App.ViewModels;

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

    private void OnValueFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: DetailFieldViewModel field })
        {
            return;
        }

        if (e.NewValue is true)
        {
            field.BeginValueEdit();
        }
        else
        {
            field.EndValueEdit();
        }
    }

    private void OnValueDropDownClosed(object? sender, EventArgs e)
    {
        if (sender is FrameworkElement { DataContext: DetailFieldViewModel field })
        {
            field.EndValueEdit();
        }
    }
}
