---
schema: 1
lang: en
family: items-ammo
title: "AmmoStrings - calibre labels"
tables:
  - AmmoStrings
table_ids:
  - Items.AmmoStrings
files:
  - Items/AmmoStrings.xml
root: AMMOLIST
record: AMMO
key: uiIndex
sources:
  - Ammo string table data
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# AmmoStrings - calibre labels

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`AMMOLIST/AMMO/AmmoCaliber`](#ammolistammoammocaliber)
  - [`AMMOLIST/AMMO/BRCaliber`](#ammolistammobrcaliber)
  - [`AMMOLIST/AMMO/NWSSCaliber`](#ammolistammonwsscaliber)
  - [`AMMOLIST/AMMO/uiIndex`](#ammolistammouiindex)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AmmoStrings.xml` maps a numeric calibre index to the text shown for that calibre. Weapons and magazines store the index; the strings in this table are the user-visible labels.

## Tables

| Table | File | Notes |
|---|---|---|
| `AmmoStrings` | `Items/AmmoStrings.xml` | v5 adds `NWSSCaliber`; v4 records omit that element. |

## Structure

```
AMMOLIST
└── AMMO                    one calibre label row
    ├── uiIndex             calibre index
    ├── AmmoCaliber         main label
    ├── BRCaliber           Bobby Ray label
    └── NWSSCaliber         alternate NWS label (v5)
```

## Game options

No table-level global option is documented for this table. Its fields are read in both inventory models and in multiplayer; the only version gate is the field set described under [Versions](#versions).

## Fields

### `AMMOLIST/AMMO/AmmoCaliber`

- **Type:** `String` (`String`)
- **Description:** Main display label for the calibre. It is technical text, not a translated sentence.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** ammo string table data; field metadata.
- **Confidence:** `inferred`

### `AMMOLIST/AMMO/BRCaliber`

- **Type:** `String` (`String`)
- **Description:** Calibre label used by Bobby Ray’s gun shop.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** ammo string table data; field metadata.
- **Confidence:** `inferred`

### `AMMOLIST/AMMO/NWSSCaliber`

- **Type:** `String` (`String`)
- **Description:** Alternate calibre spelling used by the NWS sound path. v4 records do not contain this element.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** ammo string table data; field metadata.
- **Confidence:** `inferred`

### `AMMOLIST/AMMO/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Calibre index referenced by `Weapons.ubCalibre` and magazine rows.
- **Values:** Non-negative integer; unique within the file. `Weapons.ubCalibre` points here.
- **Default:** empty string when omitted.
- **Source:** ammo string table data; field metadata.
- **Confidence:** `source`

## Relations

- `Weapons.ubCalibre` points to `AmmoStrings.uiIndex`.
- Magazine and weapon rows carry the calibre index; the three strings are alternative labels for the same calibre.

## Related formulas

No formula in [FORMULAS](../../FORMULAS.md) applies to this table.

## Localization

The three calibre strings are display labels. They are technical names and are normally identical across languages.

## Versions

v5 adds `NWSSCaliber`; v4 rows omit it. `AmmoCaliber` and `BRCaliber` exist in both.

## Sources

- Ammo string table data.
- Item metadata.
- Field types and limits: item metadata; numeric limits use the metadata validation ranges.
- Semantic descriptions use the table schema comments and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

`BRCaliber` is labelled as the Bobby Ray variant; the exact UI path that selects it is not part of the table schema.
