using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Localization;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One entry of the table language selector: the base table or a language overlay.
/// </summary>
public sealed class LanguageOptionViewModel
{
    internal LanguageOptionViewModel(LanguageDescriptor? language, bool hasOverlay)
    {
        Language = language;
        HasOverlay = hasOverlay;
        if (language is null)
        {
            DisplayText = Strings.Get("EditorBaseLanguageOption");
            Tooltip = Strings.Get("EditorBaseLanguageTooltip");
        }
        else if (hasOverlay)
        {
            DisplayText = language.DisplayName;
            Tooltip = Strings.Format("EditorLanguageOverlayTooltip", language.DisplayName);
        }
        else
        {
            DisplayText = Strings.Format("EditorLanguageCreateFormat", language.DisplayName);
            Tooltip = Strings.Format("EditorLanguageCreateTooltip", language.DisplayName);
        }
    }

    /// <summary>
    /// Gets the language descriptor, or <see langword="null"/> for the base table.
    /// </summary>
    public LanguageDescriptor? Language { get; }

    /// <summary>
    /// Gets the language code, or <see langword="null"/> for the base table.
    /// </summary>
    public string? Code => Language?.Code;

    /// <summary>
    /// Gets a value indicating whether this option has an overlay file.
    /// </summary>
    public bool HasOverlay { get; }

    /// <summary>
    /// Gets the localized text shown in the selector.
    /// </summary>
    public string DisplayText { get; }

    /// <summary>
    /// Gets the localized tooltip shown in the selector.
    /// </summary>
    public string Tooltip { get; }
}
