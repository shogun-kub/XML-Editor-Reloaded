using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

        // The tooltip must be a visual tree element, not a MultiBinding object: a raw binding object
        // would be displayed as "System.Windows.Data.MultiBinding". A ToolTip is detached from the
        // owner's logical tree, so its DataContext is bound explicitly to the owner.
        var text = new TextBlock();
        var actionText = new Run();
        if (Text is Binding actionBinding)
        {
            // Run.Text defaults to a read/write binding for some targets; the view-model property is
            // read-only, so force a one-way expression.
            actionBinding.Mode = BindingMode.OneWay;
        }

        BindingOperations.SetBinding(actionText, Run.TextProperty, Text);
        text.Inlines.Add(actionText);
        text.Inlines.Add(new Run($" ({gesture})"));
        var toolTip = new ToolTip { Content = text };
        if (serviceProvider?.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target
            && target.TargetObject is FrameworkElement owner)
        {
            actionText.SetBinding(
                FrameworkContentElement.DataContextProperty,
                new Binding(nameof(FrameworkElement.DataContext)) { Source = owner });
            toolTip.SetBinding(
                FrameworkElement.DataContextProperty,
                new Binding(nameof(FrameworkElement.DataContext)) { Source = owner });
        }

        return toolTip;
    }
}
