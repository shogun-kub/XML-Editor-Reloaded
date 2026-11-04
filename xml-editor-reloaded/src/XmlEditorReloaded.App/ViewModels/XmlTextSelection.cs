namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Character range selected in the read-only XML text view.
/// </summary>
/// <param name="Start">Zero-based start offset.</param>
/// <param name="Length">Number of selected characters.</param>
public sealed record XmlTextSelection(int Start, int Length);
