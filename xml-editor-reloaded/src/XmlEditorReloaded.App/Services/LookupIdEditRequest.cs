namespace XmlEditorReloaded.App.Services;

/// <summary>
/// Data for the free-form lookup identifier dialog.
/// </summary>
/// <param name="Title">Dialog title, usually including the lookup table name.</param>
/// <param name="TableName">Dotted lookup table name, for example <c>Lookup.AttachmentPoint</c>.</param>
/// <param name="CurrentId">Identifier of the selected value before the edit.</param>
/// <param name="SuggestedId">Free identifier suggested for a new value.</param>
/// <param name="IsFlagLookup">Whether the lookup ids are bit flags, so a combination is not a single flag.</param>
/// <param name="ExistingIds">Identifiers already used by the lookup; the dialog rejects duplicates.</param>
public sealed record LookupIdEditRequest(
    string Title,
    string TableName,
    string CurrentId,
    string SuggestedId,
    bool IsFlagLookup,
    IReadOnlyCollection<string> ExistingIds);
