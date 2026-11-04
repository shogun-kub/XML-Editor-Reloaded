using System.Windows.Data;
using System.Windows.Markup;

namespace XmlEditorReloaded.App.Shortcuts;

/// <summary>
/// Builds a tooltip that combines action text with the configured shortcut, for example
/// <c>Save (Ctrl+S)</c>.
/// </summary>
[MarkupExtensionReturnType(typeof(object))]
public sealed class ShortcutTipExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the action identifier from <see cref="ShortcutCatalog"/>.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the binding that supplies the action text.
    /// </summary>
    public BindingBase? Text { get; set; }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var gesture = ShortcutCatalog.GetText(Action);
        if (Text is null)
        {
            return gesture;
        }

        var binding = new MultiBinding
        {
            StringFormat = "{0} ({1})",
        };
        binding.Bindings.Add(Text);
        binding.Bindings.Add(new Binding { Source = gesture, Mode = BindingMode.OneTime });
        return binding;
    }
}
