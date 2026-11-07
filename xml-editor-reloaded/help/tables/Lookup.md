---
schema: 1
lang: en
family: lookup
title: "Lookup tables"
tables:
  - AmmoChoices
  - AmmoFlag
  - ArmourClass
  - AttachmentClass
  - AttachmentPoint
  - AttachmentSystem
  - Cursor
  - DrugType
  - ExplosionSize
  - ExplosionType
  - ItemClass
  - ItemFlag
  - LBEClass
  - MagazineType
  - MergeType
  - NasAttachmentClass
  - NasLayoutClass
  - PocketSize
  - Separability
  - Silhouette
  - SkillCheckType
  - WeaponClass
  - WeaponType
table_ids:
  - Lookup.AmmoChoices
  - Lookup.AmmoFlag
  - Lookup.ArmourClass
  - Lookup.AttachmentClass
  - Lookup.AttachmentPoint
  - Lookup.AttachmentSystem
  - Lookup.Cursor
  - Lookup.DrugType
  - Lookup.ExplosionSize
  - Lookup.ExplosionType
  - Lookup.ItemClass
  - Lookup.ItemFlag
  - Lookup.LBEClass
  - Lookup.MagazineType
  - Lookup.MergeType
  - Lookup.NasAttachmentClass
  - Lookup.NasLayoutClass
  - Lookup.PocketSize
  - Lookup.Separability
  - Lookup.Silhouette
  - Lookup.SkillCheckType
  - Lookup.WeaponClass
  - Lookup.WeaponType
files:
  - Lookup/AmmoChoices.xml
  - Lookup/AmmoFlag.xml
  - Lookup/ArmourClass.xml
  - Lookup/AttachmentClass.xml
  - Lookup/AttachmentPoint.xml
  - Lookup/AttachmentSystem.xml
  - Lookup/Cursor.xml
  - Lookup/DrugType.xml
  - Lookup/ExplosionSize.xml
  - Lookup/ExplosionType.xml
  - Lookup/ItemClass.xml
  - Lookup/ItemFlag.xml
  - Lookup/LBEClass.xml
  - Lookup/MagazineType.xml
  - Lookup/MergeType.xml
  - Lookup/NasAttachmentClass.xml
  - Lookup/NasLayoutClass.xml
  - Lookup/PocketSize.xml
  - Lookup/Separability.xml
  - Lookup/Silhouette.xml
  - Lookup/SkillCheckType.xml
  - Lookup/WeaponClass.xml
  - Lookup/WeaponType.xml
root: JA2Data
record: "<lookup element>"
key: id
path_mode: relative
sources:
  - Lookup XML data
  - Editor metadata for flag/sequence kind
status: reviewed
updated: 2026-11-07
---

# Lookup tables

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Lookup details](#lookup-details)
  - [AmmoChoices](#ammochoices)
  - [AmmoFlag](#ammoflag)
  - [ArmourClass](#armourclass)
  - [AttachmentClass](#attachmentclass)
  - [AttachmentPoint](#attachmentpoint)
  - [AttachmentSystem](#attachmentsystem)
  - [Cursor](#cursor)
  - [DrugType](#drugtype)
  - [ExplosionSize](#explosionsize)
  - [ExplosionType](#explosiontype)
  - [ItemClass](#itemclass)
  - [ItemFlag](#itemflag)
  - [LBEClass](#lbeclass)
  - [MagazineType](#magazinetype)
  - [MergeType](#mergetype)
  - [NasAttachmentClass](#nasattachmentclass)
  - [NasLayoutClass](#naslayoutclass)
  - [PocketSize](#pocketsize)
  - [Separability](#separability)
  - [Silhouette](#silhouette)
  - [SkillCheckType](#skillchecktype)
  - [WeaponClass](#weaponclass)
  - [WeaponType](#weapontype)
- [Structure](#structure)
- [Fields](#fields)
  - [`id`](#id)
  - [`name`](#name)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

The `Lookup` directory contains small dictionaries that turn numeric values into labels for
the editor and for modders. They are **editor-side data**: the game code does not read these
files, it uses the numeric constants that the values mirror. The editor uses them for combo
boxes, bit editors and the "where used" views.

The lookups share one structure. Each is either a flag/bitmask dictionary
(`IsFlagLookup = 1`) or a sequential value list. The files parse in both v4 and v5;
`Lookup/AttachmentPoint.xml` was repaired on 2026-11-07.

## Tables

All lookup files use `JA2Data` as the document root and the lookup name as the record
element (`AmmoChoices`, `ItemClass`, ...).

| Lookup | File | Kind | Notes |
|---|---|---|---|
| `AmmoChoices` | `Lookup/AmmoChoices.xml` | sequence | sparse IDs for enemy/militia ammo choices |
| `AmmoFlag` | `Lookup/AmmoFlag.xml` | flags | ammo property bits |
| `ArmourClass` | `Lookup/ArmourClass.xml` | sequence | matches `ARMOURCLASS_*` |
| `AttachmentClass` | `Lookup/AttachmentClass.xml` | flags | OAS attachment classes |
| `AttachmentPoint` | `Lookup/AttachmentPoint.xml` | flags | v5 repaired file |
| `AttachmentSystem` | `Lookup/AttachmentSystem.xml` | sequence | Any / OAS / NAS |
| `Cursor` | `Lookup/Cursor.xml` | sequence | cursor IDs |
| `DrugType` | `Lookup/DrugType.xml` | flags | drug effect bits |
| `ExplosionSize` | `Lookup/ExplosionSize.xml` | sequence | explosion size |
| `ExplosionType` | `Lookup/ExplosionType.xml` | sequence | explosion kind |
| `ItemClass` | `Lookup/ItemClass.xml` | flags | `IC_*` bit flags |
| `ItemFlag` | `Lookup/ItemFlag.xml` | flags | item flags, 64-bit |
| `LBEClass` | `Lookup/LBEClass.xml` | sequence | load-bearing equipment class |
| `MagazineType` | `Lookup/MagazineType.xml` | sequence | magazine / bullet / box / crate |
| `MergeType` | `Lookup/MergeType.xml` | sequence | merge transformation kind |
| `NasAttachmentClass` | `Lookup/NasAttachmentClass.xml` | flags | NAS attachment classes |
| `NasLayoutClass` | `Lookup/NasLayoutClass.xml` | flags | NAS layout classes |
| `PocketSize` | `Lookup/PocketSize.xml` | sequence | pocket size |
| `Separability` | `Lookup/Separability.xml` | sequence | removable / inseparable / replaceable |
| `Silhouette` | `Lookup/Silhouette.xml` | sequence | item silhouette |
| `SkillCheckType` | `Lookup/SkillCheckType.xml` | sequence | skill check kind |
| `WeaponClass` | `Lookup/WeaponClass.xml` | sequence | `WEAPONCLASS_*` |
| `WeaponType` | `Lookup/WeaponType.xml` | sequence | `WEAPONTYPE_*` |

## Lookup details

Each entry repeats the kind and the source of its values. The full value lists stay in the
XML files; the editor and the future help window read them from there.

### AmmoChoices

Sequence lookup with sparse IDs. Used by enemy/militia ammo choice tables,
not by the item data directly.

### AmmoFlag

Flag lookup. Ammo property bits used by `AmmoTypes.ammoflag`.

### ArmourClass

Sequence lookup. Matches `ARMOURCLASS_*`; used by
[Armours](Items/Armours.md).

### AttachmentClass

Flag lookup. OAS attachment class bits.

### AttachmentPoint

Flag lookup. Used by the attachment slot
and NAS fields.

### AttachmentSystem

Sequence lookup: Any, OAS only, NAS only.

### Cursor

Sequence lookup. Cursor IDs used by the UI.

### DrugType

Flag lookup. Drug effect bits used by `Drugs`.

### ExplosionSize

Sequence lookup, from none to large.

### ExplosionType

Sequence lookup, such as normal, stun and tear gas.

### ItemClass

Flag lookup. `IC_*` item class bits.

### ItemFlag

Flag lookup. Item flags, including 64-bit values.

### LBEClass

Sequence lookup. Load-bearing equipment class.

### MagazineType

Sequence lookup: magazine, bullet(s), box, crate. Matches `AMMO_MAGAZINE`,
`AMMO_BULLET`, `AMMO_BOX`, `AMMO_CRATE`; used by [Magazines](Items/Magazines.md).

### MergeType

Sequence lookup. Merge/transformation kind.

### NasAttachmentClass

Flag lookup. NAS attachment class bits.

### NasLayoutClass

Flag lookup. NAS layout class bits.

### PocketSize

Sequence lookup: none, small, medium, large.

### Separability

Sequence lookup: removable, inseparable, replaceable.

### Silhouette

Sequence lookup. Item silhouette used by the inventory display.

### SkillCheckType

Sequence lookup. Skill check kind.

### WeaponClass

Sequence lookup. `WEAPONCLASS_*` used by `Weapons.ubWeaponClass`.

### WeaponType

Sequence lookup. `WEAPONTYPE_*` used by `Weapons.ubWeaponType`.

## Structure

```
JA2Data
├── id
└── name
```

- `id` may be sparse (`AmmoChoices` uses 19, 29, ... 79).
- `name` is a label, not a localization key. Names are English and are not translated by
  `Russian.*` overlays.
- Flag lookups may contain combined values in `id`; a row can document a group of bits as
  well as a single bit.
- Record order is not significant; the editor sorts by `id`.

## Fields

### `id`

- **Type:** `Integer` (up to 64-bit for `ItemFlag`)
- **Description:** Numeric identifier of the value. For flag lookups this is a bit or a
  combination of bits; for sequence lookups it is an ordinal or an explicit ID. Numeric semantics: Identifier; larger numbers have no inherent meaning beyond row order, and negative values are invalid.
- **Values:** `0..max`; sparse IDs are allowed (`AmmoChoices`).
- **Default:** none; the value must exist in the file.
- **Source:** lookup XML data; editor metadata for flag/sequence kind.
- **Confidence:** `source`

### `name`

- **Type:** `String`
- **Description:** Display label for the value, shown in editor combo boxes and bit lists.
  For a combined flag value it describes the group of bits.
- **Values:** free text; unique within one lookup in practice.
- **Default:** none.
- **Source:** lookup XML data; editor metadata for flag/sequence kind.
- **Confidence:** `source`

## Relations

The lookups mirror numeric constants used by item and weapon fields. Examples:

- `Items.usItemClass` and `Items.<flag fields>` use `ItemClass` and
  `ItemFlag`.
- `Weapons.ubWeaponClass` / `ubWeaponType` use `WeaponClass` and
  `WeaponType`.
- `Armours.ubArmourClass` uses `ArmourClass`.
- `Magazines.ubMagType` uses `MagazineType`.

The table documents are the authority for which field uses which lookup; this page only
defines the dictionaries.

## Localization

Lookup names are not translated by the game's overlay mechanism. A Russian UI may translate
the label shown around a lookup, but the `name` values in these files stay English.

## Versions

- v4 and v5 have the same lookup files and the same `id`/`name` schema.
- `Lookup/AttachmentPoint.xml` in v5 was repaired on 2026-11-07; it now parses and contains
  the attachment-point values:

| id | name |
|---:|---|
| 0 | None |
| 1 | Small Molle |
| 2 | Medium Molle |
| 3 | All Molle |
| 4 | Hydration Carrier |
| 7 | All Molle and Carrier |
| 8 | Surgical Mask and Gloves |
| 16 | Batteries |

The repair is a data fix, not a code change. `help-inventory.ps1` still falls back to v4 if
a future v5 file fails to parse; the fallback is not used by the current v5 tree.

## Sources

- `TableData/Lookup/*.xml` (v4 and v5) - the values and names.
- `metadata: Tables.IsFlagLookup` - flag vs sequence classification.
- Game constants that the values mirror: item classes and flags, armour classes, weapon
  classes and types, ammo flags, and skill-check kinds.
- No game loader reads `TableData/Lookup/*.xml`; they are editor/modding data.

## Open questions

- Do all lookup ID sets exactly match the current code constants? The B0 pass verified
  counts, kinds and ranges, but not every constant against its header.
- Should combined flag rows be listed as separate values or kept only in the lookup file?
  The editor shows them as values; the documents do not duplicate the full value list.
