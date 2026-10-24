namespace XmlEditorReloaded.Core.Metadata;

/// <summary>
/// Kind of item-workspace section governed by a <see cref="ClassVisibilityRule"/>.
/// </summary>
public enum ClassVisibilitySectionKind
{
    /// <summary>
    /// A detail-field group identified by a stable group key, for example <c>combat</c>.
    /// </summary>
    Group,

    /// <summary>
    /// A 1:1 sub-table section identified by the dotted target table name, for example
    /// <c>Items.Weapons</c>.
    /// </summary>
    SubTable,

    /// <summary>
    /// A 1:N child-table section identified by the dotted target table name, for example
    /// <c>Items.CompatibleFaceItems</c>.
    /// </summary>
    ChildTable,
}

/// <summary>
/// Metadata rule that limits an item-workspace group, sub-table, or child-table section to a set of
/// item classes. The rule is keyed by the section and expresses the classes as a bit mask over
/// <c>usItemClass</c> values, mirroring the legacy editor's per-class page removal without hardcoding
/// class names in the UI layer.
/// </summary>
public sealed class ClassVisibilityRule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassVisibilityRule"/> class.
    /// </summary>
    /// <param name="kind">Kind of the governed section.</param>
    /// <param name="sectionKey">
    /// Group key for <see cref="ClassVisibilitySectionKind.Group"/>; dotted table name for sub-table
    /// and child-table sections.
    /// </param>
    /// <param name="visibleClasses">
    /// Bit mask of item classes that show the section. A negative value shows the section for every
    /// class, including records where <c>usItemClass</c> is absent.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="sectionKey"/> is empty.</exception>
    public ClassVisibilityRule(
        ClassVisibilitySectionKind kind,
        string sectionKey,
        long visibleClasses)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionKey);
        Kind = kind;
        SectionKey = sectionKey;
        VisibleClasses = visibleClasses;
    }

    /// <summary>
    /// Gets the kind of the governed section.
    /// </summary>
    public ClassVisibilitySectionKind Kind { get; }

    /// <summary>
    /// Gets the stable section key: a group key or a dotted table name.
    /// </summary>
    public string SectionKey { get; }

    /// <summary>
    /// Gets the bit mask of item classes that show the section, or a negative value when the section is
    /// always visible.
    /// </summary>
    public long VisibleClasses { get; }

    /// <summary>
    /// Gets a value indicating whether the section is shown for the supplied class value.
    /// </summary>
    /// <param name="classBits">Raw <c>usItemClass</c> value; zero means the field is absent or empty.</param>
    /// <returns>
    /// <see langword="true"/> when the section is always visible or at least one class bit is included
    /// in <see cref="VisibleClasses"/>; otherwise <see langword="false"/>.
    /// </returns>
    public bool IsVisible(long classBits) =>
        VisibleClasses < 0 || (VisibleClasses & classBits) != 0;
}
