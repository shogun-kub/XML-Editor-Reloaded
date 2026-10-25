namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Declares that a table stores symmetric pairs of records: for every record with key values
/// (<see cref="FirstField"/>, <see cref="SecondField"/>), a record with the two values reversed is
/// expected to exist as well.
/// </summary>
/// <remarks>
/// The legacy editor applied this invariant before saving two tables: <c>Items.IncompatibleAttachments</c>
/// and <c>Items.CompatibleFaceItems</c> (<c>DuplicateEntryTable.VerifyData</c>). The rewrite maintains the
/// pair during add, delete, and key edits and reports a validation warning for data that is already
/// asymmetric; it never normalizes existing files on load or save.
/// </remarks>
public sealed class SymmetricKeyPair
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SymmetricKeyPair"/> class.
    /// </summary>
    /// <param name="firstField">Case-sensitive XML field of the first key component.</param>
    /// <param name="secondField">Case-sensitive XML field of the second key component.</param>
    /// <exception cref="ArgumentException">Thrown when a field name is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a field name is null.</exception>
    public SymmetricKeyPair(string firstField, string secondField)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstField);
        ArgumentException.ThrowIfNullOrWhiteSpace(secondField);
        FirstField = firstField;
        SecondField = secondField;
    }

    /// <summary>
    /// Gets the case-sensitive XML field of the first key component.
    /// </summary>
    public string FirstField { get; }

    /// <summary>
    /// Gets the case-sensitive XML field of the second key component.
    /// </summary>
    public string SecondField { get; }
}
