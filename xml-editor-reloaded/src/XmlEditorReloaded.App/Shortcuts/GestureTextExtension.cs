using System.Windows.Markup;

namespace XmlEditorReloaded.App.Shortcuts;

/// <summary>
/// Returns the display text of a catalog shortcut for menu gesture labels.
/// </summary>
[MarkupExtensionReturnType(typeof(string))]
public sealed class GestureTextExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the action identifier from <see cref="ShortcutCatalog"/>.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider) =>
        ShortcutCatalog.GetText(Action);
}
