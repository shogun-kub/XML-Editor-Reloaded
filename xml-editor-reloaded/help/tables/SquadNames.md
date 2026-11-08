---
schema: 1
lang: en
family: squad-names
title: "SquadNames - squad display names"
tables:
  - SquadNames
table_ids:
  - SquadNames
files:
  - SquadNames.xml
root: SQUAD_NAMES
record: SQUADNAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# SquadNames - squad display names

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SQUAD_NAMES/SQUADNAME/uiIndex`](#squad_namessquadnameuiindex)
  - [`SQUAD_NAMES/SQUADNAME/Squad`](#squad_namessquadnamesquad)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SquadNames.xml` maps squad indexes to display names used in the strategic UI.

## Tables

| Table | File | Notes |
|---|---|---|
| `SquadNames` | `SquadNames.xml` | — |

## Structure

```
SQUAD_NAMES
└── SQUADNAME
    ├── uiIndex
    └── Squad
```

## Fields

### `SQUAD_NAMES/SQUADNAME/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the squad-name entry. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SQUAD_NAMES/SQUADNAME/Squad`

- **Type:** `String`
- **Description:** Display name of the squad, such as `Alpha`.
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
