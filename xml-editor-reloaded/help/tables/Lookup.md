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
status: draft
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

There are 23 lookups with one shared structure. Eight are flag/bitmask dictionaries
(`IsFlagLookup = 1`); the other fifteen are sequential value lists. All 23 files parse in
both v4 and v5; `Lookup/AttachmentPoint.xml` was repaired on 2026-11-07.

## Tables

All 23 files use `JA2Data` as the document root and the lookup name as the record element
(`AmmoChoices`, `ItemClass`, ...). The record count is the number of values.

| Lookup | File | Values | Kind | ID range | Notes |
|---|---|---:|---|---|---|
| `AmmoChoices` | `Lookup/AmmoChoices.xml` | 7 | sequence | 19..79 | sparse IDs for enemy/militia ammo choices |
| `AmmoFlag` | `Lookup/AmmoFlag.xml` | 5 | flags | 0..4 | ammo property bits |
| `ArmourClass` | `Lookup/ArmourClass.xml` | 6 | sequence | 0..5 | matches `ARMOURCLASS_*` |
| `AttachmentClass` | `Lookup/AttachmentClass.xml` | 36 | flags | 0..33554432 | OAS attachment classes |
| `AttachmentPoint` | `Lookup/AttachmentPoint.xml` | 8 | flags | 0..16 | v5 repaired; same eight values as v4 |
| `AttachmentSystem` | `Lookup/AttachmentSystem.xml` | 3 | sequence | 0..2 | Any / OAS / NAS |
| `Cursor` | `Lookup/Cursor.xml` | 23 | sequence | 0..23 | cursor IDs |
| `DrugType` | `Lookup/DrugType.xml` | 26 | flags | 0..1081324 | drug effect bits |
| `ExplosionSize` | `Lookup/ExplosionSize.xml` | 5 | sequence | 0..4 | explosion size |
| `ExplosionType` | `Lookup/ExplosionType.xml` | 13 | sequence | 0..12 | explosion kind |
| `ItemClass` | `Lookup/ItemClass.xml` | 23 | flags | 1..1073741824 | `IC_*` bit flags |
| `ItemFlag` | `Lookup/ItemFlag.xml` | 36 | flags | 0..8589934592 | item flags, 64-bit |
| `LBEClass` | `Lookup/LBEClass.xml` | 5 | sequence | 0..4 | load-bearing equipment class |
| `MagazineType` | `Lookup/MagazineType.xml` | 4 | sequence | 0..3 | magazine / bullet / box / crate |
| `MergeType` | `Lookup/MergeType.xml` | 16 | sequence | 0..15 | merge transformation kind |
| `NasAttachmentClass` | `Lookup/NasAttachmentClass.xml` | 16 | flags | 0..16384 | NAS attachment classes |
| `NasLayoutClass` | `Lookup/NasLayoutClass.xml` | 7 | flags | 0..32 | NAS layout classes |
| `PocketSize` | `Lookup/PocketSize.xml` | 4 | sequence | 0..3 | pocket size |
| `Separability` | `Lookup/Separability.xml` | 3 | sequence | 0..2 | removable / inseparable / replaceable |
| `Silhouette` | `Lookup/Silhouette.xml` | 56 | sequence | 0..55 | item silhouette |
| `SkillCheckType` | `Lookup/SkillCheckType.xml` | 16 | sequence | 0..15 | skill check kind |
| `WeaponClass` | `Lookup/WeaponClass.xml` | 8 | sequence | 0..7 | `WEAPONCLASS_*` |
| `WeaponType` | `Lookup/WeaponType.xml` | 9 | sequence | 0..8 | `WEAPONTYPE_*` |

## Lookup details

Each entry repeats the kind and the source of its values. The full value lists stay in the
XML files; the editor and the future help window read them from there.

### AmmoChoices

Sequence lookup with 7 sparse IDs (19, 29, ... 79). Used by enemy/militia ammo choice tables,
not by the item data directly.

### AmmoFlag

Flag lookup with 5 values (0..4). Ammo property bits used by `AmmoTypes.ammoflag`.

### ArmourClass

Sequence lookup with 6 values (0..5). Matches `ARMOURCLASS_*`; used by
[Armours](Items/Armours.md).

### AttachmentClass

Flag lookup with 36 values. OAS attachment class bits; the largest value is `0x02000000`.

### AttachmentPoint

Flag lookup with 8 values (0..16). Same values in v4 and v5. Used by the attachment slot
and NAS fields.

### AttachmentSystem

Sequence lookup with 3 values: Any, OAS only, NAS only.

### Cursor

Sequence lookup with 23 values (0..23). Cursor IDs used by the UI.

### DrugType

Flag lookup with 26 values. Drug effect bits used by `Drugs`.

### ExplosionSize

Sequence lookup with 5 values (0..4), from none to large.

### ExplosionType

Sequence lookup with 13 values (0..12), such as normal, stun and tear gas.

### ItemClass

Flag lookup with 23 values. `IC_*` item class bits; the largest value is `0x40000000`.

### ItemFlag

Flag lookup with 36 values. Item flags, including 64-bit values; the largest is
`0x200000000`.

### LBEClass

Sequence lookup with 5 values (0..4). Load-bearing equipment class.

### MagazineType

Sequence lookup with 4 values: magazine, bullet(s), box, crate. Matches `AMMO_MAGAZINE`,
`AMMO_BULLET`, `AMMO_BOX`, `AMMO_CRATE`; used by [Magazines](Items/Magazines.md).

### MergeType

Sequence lookup with 16 values (0..15). Merge/transformation kind.

### NasAttachmentClass

Flag lookup with 16 values. NAS attachment class bits.

### NasLayoutClass

Flag lookup with 7 values. NAS layout class bits.

### PocketSize

Sequence lookup with 4 values: none, small, medium, large.

### Separability

Sequence lookup with 3 values: removable, inseparable, replaceable.

### Silhouette

Sequence lookup with 56 values (0..55). Item silhouette used by the inventory display.

### SkillCheckType

Sequence lookup with 16 values (0..15). Skill check kind.

### WeaponClass

Sequence lookup with 8 values (0..7). `WEAPONCLASS_*` used by `Weapons.ubWeaponClass`.

### WeaponType

Sequence lookup with 9 values (0..8). `WEAPONTYPE_*` used by `Weapons.ubWeaponType`.

## Structure

```
JA2Data
└── <LookupName>
    ├── id     numeric value or bit mask
    └── name   display label
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
  combination of bits; for sequence lookups it is an ordinal or an explicit ID.
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

- v4 and v5 have the same 23 lookup files and the same `id`/`name` schema.
- `Lookup/AttachmentPoint.xml` in v5 was repaired on 2026-11-07; it now parses and contains
  the same eight values as v4:

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

- Do all 23 lookup ID sets exactly match the current code constants? The B0 pass verified
  counts, kinds and ranges, but not every constant against its header.
- Should combined flag rows be listed as separate values or kept only in the lookup file?
  The editor shows them as values; the documents do not duplicate the full value list.
