namespace XmlEditorReloaded.Core.Localization;

/// <summary>
/// Describes a language supported by the editor for data overlays and UI localization.
/// </summary>
/// <param name="Code">Stable language code, for example <c>ru</c>.</param>
/// <param name="DisplayName">Human-readable language name for the UI.</param>
/// <param name="OverlayPrefix">File-name prefix of overlay files, for example <c>Russian</c> in <c>Russian.Items.xml</c>.</param>
/// <param name="Ordinal">Sort order for language pickers.</param>
public sealed record LanguageDescriptor(string Code, string DisplayName, string OverlayPrefix, int Ordinal)
{
    /// <summary>
    /// Determines whether a base file name belongs to this language's overlay by prefixing it.
    /// </summary>
    /// <param name="baseFileName">Base file name with extension, for example <c>Items.xml</c>.</param>
    /// <returns>The overlay file name, for example <c>Russian.Items.xml</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="baseFileName"/> is <see langword="null"/>.</exception>
    public string ToOverlayFileName(string baseFileName)
    {
        ArgumentNullException.ThrowIfNull(baseFileName);
        return $"{OverlayPrefix}.{baseFileName}";
    }
}
