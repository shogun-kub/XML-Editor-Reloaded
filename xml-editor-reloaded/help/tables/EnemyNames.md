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
status: draft
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

- **Type:** `Integer`
- **Description:** Index of the enemy-name entry. It is an array key, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMY_NAMES/NAME/Enabled`

- **Type:** `Boolean`
- **Description:** If set, the custom enemy group name is enabled. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMY_NAMES/NAME/Sector`

- **Type:** `String`
- **Description:** Sector code where the custom enemy group name applies. It is a map coordinate, not a magnitude.
- **Values:** Free text or a symbolic code.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMY_NAMES/NAME/szGroup`

- **Type:** `String`
- **Description:** Display name of the enemy group in that sector.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

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
