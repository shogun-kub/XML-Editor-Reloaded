namespace XmlEditorReloaded.App.Resources;

/// <summary>
/// Formats the legacy "remaining characters" counter next to a text field. The count follows the text
/// currently shown in the text box, so it updates while typing without committing every keystroke.
/// </summary>
public static class CharacterCounter
{
    /// <summary>
    /// Formats the remaining characters of a text value.
    /// </summary>
    /// <param name="maxLength">Maximum accepted length, or zero when the field is unlimited.</param>
    /// <param name="length">Current length of the visible text.</param>
    /// <returns>
    /// An empty string when the field is unlimited; otherwise the localized remaining count, or the
    /// localized excess when the stored value is already longer than the limit.
    /// </returns>
    public static string Format(int maxLength, int length)
    {
        if (maxLength <= 0)
        {
            return string.Empty;
        }

        var remaining = maxLength - Math.Max(length, 0);
        return remaining >= 0
            ? Strings.Format("DetailCharactersLeftFormat", remaining)
            : Strings.Format("DetailCharactersOverLimitFormat", -remaining);
    }
}
