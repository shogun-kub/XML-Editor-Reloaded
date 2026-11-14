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
status: reviewed
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
- **Description:** Index element parsed by the loader, but its value is ignored: squad names are stored in file order, so the position of the `<SQUADNAME>` record determines the squad slot.
- **Values:** Integer; changing it does not reorder the stored names.
- **Default:** Absent; the next record in file order is used.
- **Source:** game code (`XML_SquadNames.cpp`); table data.
- **Confidence:** `source`

### `SQUAD_NAMES/SQUADNAME/Squad`

- **Type:** `String` (`CHAR16`)
- **Only used with:** the `USE_XML_SQUADNAMES` INI option; without it the game keeps its built-in squad names.
- **Description:** Display name of the squad, such as `Alpha`. Names are stored in file order and truncated to 29 characters by the loader.
- **Values:** Free text, effectively limited to 29 characters.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_SquadNames.cpp`); table data.
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
