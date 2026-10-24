namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Values entered in the new-item dialog.
/// </summary>
/// <param name="Id">Lexical item identifier written to <c>uiIndex</c>.</param>
/// <param name="Name">Item name written to <c>szItemName</c>.</param>
/// <param name="ClassValue">Lexical item class bit mask written to <c>usItemClass</c>.</param>
public sealed record ItemCreationRequest(string Id, string Name, string ClassValue);
