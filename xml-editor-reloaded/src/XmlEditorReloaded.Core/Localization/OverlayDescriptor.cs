namespace XmlEditorReloaded.Core.Localization;

/// <summary>
/// Describes one language overlay file found next to its base table file.
/// </summary>
/// <param name="Language">The language the overlay belongs to.</param>
/// <param name="BaseFilePath">Absolute path of the base table file.</param>
/// <param name="OverlayFilePath">Absolute path of the overlay file.</param>
public sealed record OverlayDescriptor(LanguageDescriptor Language, string BaseFilePath, string OverlayFilePath);
