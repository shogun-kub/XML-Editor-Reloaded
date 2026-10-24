namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// The editor that hosts one field form: it applies field edits, follows relation targets, and opens the
/// bit editor. Implemented by the grid editor and the item workspace so that both can reuse
/// <see cref="DetailFieldViewModel"/>.
/// </summary>
public interface IDetailFieldHost
{
    /// <summary>
    /// Gets a value indicating whether the form accepts edits.
    /// </summary>
    bool IsEditable { get; }

    /// <summary>
    /// Gets a value indicating whether a language overlay is selected.
    /// </summary>
    bool IsTranslationMode { get; }

    /// <summary>
    /// Applies an edited field value.
    /// </summary>
    /// <param name="field">Field that was edited.</param>
    /// <param name="value">New raw value.</param>
    void SetDetailValue(DetailFieldViewModel field, string value);

    /// <summary>
    /// Opens the record referenced by the field.
    /// </summary>
    /// <param name="field">Relation field whose target should be opened.</param>
    void NavigateToRelation(DetailFieldViewModel field);

    /// <summary>
    /// Opens the bit editor for a flag field.
    /// </summary>
    /// <param name="field">Flag field to edit.</param>
    void EditBits(DetailFieldViewModel field);
}
