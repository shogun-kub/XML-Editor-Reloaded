---
schema: 1
lang: en
family: army-garrison-groups
title: "GarrisonGroups - garrison composition per sector"
tables:
  - GarrisonGroups
table_ids:
  - Army.GarrisonGroups
files:
  - Army/GarrisonGroups.xml
root: GARRISON_INFO
record: GARRISON
key: Sector
sources:
  - Table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# GarrisonGroups - garrison composition per sector

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`GARRISON_INFO/GARRISON/Sector`](#garrison_infogarrisonsector)
  - [`GARRISON_INFO/GARRISON/Composition`](#garrison_infogarrisoncomposition)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`GarrisonGroups.xml` assigns an army composition to a sector garrison. The sector is the key; the composition selects the troop mix from `ArmyComposition.xml`.

## Tables

| Table | File | Notes |
|---|---|---|
| `GarrisonGroups` | `Army/GarrisonGroups.xml` | — |

## Structure

```
GARRISON_INFO
└── GARRISON
    ├── Sector
    └── Composition
```

## Fields

### `GARRISON_INFO/GARRISON/Sector`

- **Type:** `String`
- **Description:** Sector that hosts the garrison, in `A1`..`P16` form. It identifies the map location; it is not a magnitude.
- **Values:** Sector code such as `P3`.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `GARRISON_INFO/GARRISON/Composition`

- **Type:** `String`
- **Description:** Army composition index assigned to this garrison. It references `ArmyComposition.Index`; it is an ID, not a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `Composition` references `ArmyComposition.Index`.

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
