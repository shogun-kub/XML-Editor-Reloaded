using XmlEditorReloaded.App.Resources;
using XmlEditorReloaded.Core.Filtering;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One selectable item-list filter preset: a stable key for tests and commands, a localized label, and
/// the structured filter group applied to the item rows. An empty group means "no restriction".
/// </summary>
public sealed class ItemListFilterOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemListFilterOption"/> class.
    /// </summary>
    /// <param name="key">Stable preset key, for example <c>class.guns</c>.</param>
    /// <param name="label">Localized text shown in the selector.</param>
    /// <param name="filter">Structured filter group; <see cref="RowFilterGroup.Empty"/> selects everything.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="key"/> or <paramref name="label"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filter"/> is null.</exception>
    public ItemListFilterOption(string key, string label, RowFilterGroup filter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentNullException.ThrowIfNull(filter);
        Key = key;
        Label = label;
        Filter = filter;
    }

    /// <summary>
    /// Gets the stable preset key, for example <c>class.guns</c> or <c>flag.attachments</c>.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the localized text shown in the selector.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the structured filter group applied by the preset.
    /// </summary>
    public RowFilterGroup Filter { get; }
}

/// <summary>
/// Item-list filter presets that reproduce the legacy <c>Items &gt; Show &gt; By Class</c> and
/// <c>By Other Filters</c> commands as structured predicates over the item rows. Class presets use exact
/// class values like the legacy menu; <c>Guns</c> additionally excludes records flagged as rocket
/// launchers, and <c>Launchers</c> accepts either the launcher class or the rocket-launcher flag.
/// The "off" flag presets use <c>NotEquals 1</c> because the legacy data table defaulted absent numeric
/// fields to zero, while the sparse document does not contain the element at all.
/// </summary>
public static class ItemListFilterPresets
{
    /// <summary>XML field that stores the item class bit mask.</summary>
    public const string ClassField = "usItemClass";

    /// <summary>XML field that marks an item as a rocket launcher.</summary>
    public const string RocketLauncherField = "RocketLauncher";

    /// <summary>XML field that marks an item as an attachment.</summary>
    public const string AttachmentField = "Attachment";

    /// <summary>XML field that marks an item as medical.</summary>
    public const string MedicalField = "Medical";

    /// <summary>XML field that marks an item as available in the "tons of guns" mode.</summary>
    public const string BigGunListField = "BigGunList";

    /// <summary>XML field that marks an item as available in the sci-fi mode.</summary>
    public const string SciFiField = "SciFi";

    /// <summary>Item class value of a gun.</summary>
    public const long GunClass = 2;

    /// <summary>Item class value of a knife.</summary>
    public const long KnifeClass = 4;

    /// <summary>Item class value of a throwing knife.</summary>
    public const long ThrowingKnifeClass = 8;

    /// <summary>Item class value of a launcher.</summary>
    public const long LauncherClass = 16;

    /// <summary>Item class value of a tentacle attack.</summary>
    public const long TentacleClass = 32;

    /// <summary>Item class value of a thrown weapon.</summary>
    public const long ThrownClass = 64;

    /// <summary>Item class value of a punch attack.</summary>
    public const long PunchClass = 128;

    /// <summary>Item class value of a grenade.</summary>
    public const long GrenadeClass = 256;

    /// <summary>Item class value of a bomb.</summary>
    public const long BombClass = 512;

    /// <summary>Item class value of ammunition.</summary>
    public const long AmmoClass = 1024;

    /// <summary>Item class value of armour.</summary>
    public const long ArmourClass = 2048;

    /// <summary>Item class value of a medical kit.</summary>
    public const long MedKitClass = 4096;

    /// <summary>Item class value of a kit.</summary>
    public const long KitClass = 8192;

    /// <summary>Item class value of facial gear.</summary>
    public const long FaceClass = 32768;

    /// <summary>Item class value of a key.</summary>
    public const long KeyClass = 65536;

    /// <summary>Item class value of load-bearing equipment.</summary>
    public const long LbeClass = 131072;

    /// <summary>Item class value of a miscellaneous item.</summary>
    public const long MiscClass = 268435456;

    /// <summary>Item class value of money.</summary>
    public const long MoneyClass = 536870912;

    /// <summary>Item class value of a random item.</summary>
    public const long RandomItemClass = 1073741824;

    /// <summary>
    /// Creates the class-filter options in legacy menu order.
    /// </summary>
    /// <param name="hasRocketLauncherField">
    /// Whether the item schema contains the <c>RocketLauncher</c> field. When the field is absent the
    /// launcher presets fall back to the class value alone, so the filter never hides every row.
    /// </param>
    /// <returns>The options, starting with the unrestricted entry.</returns>
    public static IReadOnlyList<ItemListFilterOption> CreateClassOptions(bool hasRocketLauncherField)
    {
        var options = new List<ItemListFilterOption>
        {
            new("all", Strings.Get("ItemFilterAllClasses"), RowFilterGroup.Empty),
            new(
                "class.guns",
                Strings.Get("ItemFilterClassGuns"),
                All(Criterion(ClassField, RowFilterOperator.Equals, GunClass), RocketLauncherCriterion(
                    hasRocketLauncherField,
                    RowFilterOperator.NotEquals,
                    "1"))),
            new(
                "class.ammo",
                Strings.Get("ItemFilterClassAmmo"),
                All(Criterion(ClassField, RowFilterOperator.Equals, AmmoClass))),
            new(
                "class.launchers",
                Strings.Get("ItemFilterClassLaunchers"),
                Any(Criterion(ClassField, RowFilterOperator.Equals, LauncherClass), RocketLauncherCriterion(
                    hasRocketLauncherField,
                    RowFilterOperator.Equals,
                    "1"))),
            new(
                "class.grenades",
                Strings.Get("ItemFilterClassGrenades"),
                All(Criterion(ClassField, RowFilterOperator.Equals, GrenadeClass))),
            new(
                "class.explosives",
                Strings.Get("ItemFilterClassExplosives"),
                All(Criterion(ClassField, RowFilterOperator.Equals, BombClass))),
            new(
                "class.knives",
                Strings.Get("ItemFilterClassKnives"),
                Any(
                    Criterion(ClassField, RowFilterOperator.Equals, KnifeClass),
                    Criterion(ClassField, RowFilterOperator.Equals, ThrowingKnifeClass))),
            new(
                "class.other-weapons",
                Strings.Get("ItemFilterClassOtherWeapons"),
                Any(
                    Criterion(ClassField, RowFilterOperator.Equals, ThrownClass),
                    Criterion(ClassField, RowFilterOperator.Equals, PunchClass),
                    Criterion(ClassField, RowFilterOperator.Equals, TentacleClass))),
            new(
                "class.armour",
                Strings.Get("ItemFilterClassArmour"),
                All(Criterion(ClassField, RowFilterOperator.Equals, ArmourClass))),
            new(
                "class.face",
                Strings.Get("ItemFilterClassFace"),
                All(Criterion(ClassField, RowFilterOperator.Equals, FaceClass))),
            new(
                "class.kits",
                Strings.Get("ItemFilterClassKits"),
                Any(
                    Criterion(ClassField, RowFilterOperator.Equals, MedKitClass),
                    Criterion(ClassField, RowFilterOperator.Equals, KitClass))),
            new(
                "class.keys",
                Strings.Get("ItemFilterClassKeys"),
                All(Criterion(ClassField, RowFilterOperator.Equals, KeyClass))),
            new(
                "class.lbe",
                Strings.Get("ItemFilterClassLoadBearing"),
                All(Criterion(ClassField, RowFilterOperator.Equals, LbeClass))),
            new(
                "class.misc",
                Strings.Get("ItemFilterClassMisc"),
                Any(
                    Criterion(ClassField, RowFilterOperator.Equals, MiscClass),
                    Criterion(ClassField, RowFilterOperator.Equals, MoneyClass))),
            new(
                "class.none",
                Strings.Get("ItemFilterClassNone"),
                All(Criterion(ClassField, RowFilterOperator.Equals, 1))),
            new(
                "class.random",
                Strings.Get("ItemFilterClassRandom"),
                All(Criterion(ClassField, RowFilterOperator.Equals, RandomItemClass))),
        };
        return options;
    }

    /// <summary>
    /// Creates the item-flag filter options in legacy menu order.
    /// </summary>
    /// <returns>The options, starting with the unrestricted entry.</returns>
    public static IReadOnlyList<ItemListFilterOption> CreateFlagOptions() => new[]
    {
        new ItemListFilterOption("all", Strings.Get("ItemFilterAllFlags"), RowFilterGroup.Empty),
        new ItemListFilterOption(
            "flag.attachments",
            Strings.Get("ItemFilterFlagAttachments"),
            All(Criterion(AttachmentField, RowFilterOperator.Equals, 1))),
        new ItemListFilterOption(
            "flag.drugs",
            Strings.Get("ItemFilterFlagDrugs"),
            All(Criterion(MedicalField, RowFilterOperator.Equals, 1))),
        new ItemListFilterOption(
            "flag.big-gun-list",
            Strings.Get("ItemFilterFlagTonsOfGuns"),
            All(Criterion(BigGunListField, RowFilterOperator.Equals, 1))),
        new ItemListFilterOption(
            "flag.normal-guns",
            Strings.Get("ItemFilterFlagNormalGuns"),
            All(Criterion(BigGunListField, RowFilterOperator.NotEquals, 1))),
        new ItemListFilterOption(
            "flag.scifi",
            Strings.Get("ItemFilterFlagSciFi"),
            All(Criterion(SciFiField, RowFilterOperator.Equals, 1))),
        new ItemListFilterOption(
            "flag.realistic",
            Strings.Get("ItemFilterFlagRealistic"),
            All(Criterion(SciFiField, RowFilterOperator.NotEquals, 1))),
    };

    private static RowFilterCriterion Criterion(string field, RowFilterOperator @operator, object value) =>
        new(field, @operator, Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)!);

    private static RowFilterCriterion? RocketLauncherCriterion(
        bool hasRocketLauncherField,
        RowFilterOperator @operator,
        string value) =>
        hasRocketLauncherField ? Criterion(RocketLauncherField, @operator, value) : null;

    private static RowFilterGroup All(params RowFilterCriterion?[] criteria) =>
        new(RowFilterLogicalOperator.And, criteria.OfType<RowFilterCriterion>().ToArray());

    private static RowFilterGroup Any(params RowFilterCriterion?[] criteria) =>
        new(RowFilterLogicalOperator.Or, criteria.OfType<RowFilterCriterion>().ToArray());
}
