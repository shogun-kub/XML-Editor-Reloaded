---
schema: 1
lang: en
family: enemy-names
title: "EnemyNames - named enemy groups by sector"
tables:
  - EnemyNames
table_ids:
  - EnemyNames
files:
  - EnemyNames.xml
root: ENEMY_NAMES
record: NAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# EnemyNames - named enemy groups by sector

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ENEMY_NAMES/NAME/uiIndex`](#enemy_namesnameuiindex)
  - [`ENEMY_NAMES/NAME/Enabled`](#enemy_namesnameenabled)
  - [`ENEMY_NAMES/NAME/Sector`](#enemy_namesnamesector)
  - [`ENEMY_NAMES/NAME/szGroup`](#enemy_namesnameszgroup)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyNames.xml` gives enemy groups in specific sectors a custom display name. A disabled or missing entry leaves the default name.

## Tables

| Table | File | Notes |
|---|---|---|
| `EnemyNames` | `EnemyNames.xml` | — |

## Structure

```
ENEMY_NAMES
└── NAME
    ├── uiIndex
    ├── Enabled
    ├── Sector
    └── szGroup
```

## Fields

### `ENEMY_NAMES/NAME/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index of the enemy-name entry. It is an array key.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** Required; identifies the record.
- **Source:** game code (enemy-names loader); table data.
- **Confidence:** `source`

### `ENEMY_NAMES/NAME/Enabled`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the custom enemy group name is enabled. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-names loader); table data.
- **Confidence:** `source`

### `ENEMY_NAMES/NAME/Sector`

- **Type:** `String`
- **Description:** Sector code where the custom enemy group name applies. It is a map coordinate in `A1`..`P16` form: one letter for the row and a decimal column number.
- **Numeric semantics:** The loader accepts the first character as the row and the remaining digits as the column; it keeps the value only when both are in `1..16`.
- **Values:** A sector such as `A1`, `B5` or `P16`; an out-of-range value leaves the previous/default coordinate unchanged.
- **Default:** Required; identifies the record.
- **Source:** game code (enemy-names loader); table data.
- **Confidence:** `source`

### `ENEMY_NAMES/NAME/szGroup`

- **Type:** `String` (`CHAR16`)
- **Description:** Display name of the enemy group in that sector.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-names loader); table data.
- **Confidence:** `source`

## Relations

- This table is part of the strategic army and inventory data; references are described in the field notes.

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for inferred fields is not stated by the table data.
