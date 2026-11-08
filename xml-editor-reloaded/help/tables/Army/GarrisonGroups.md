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
status: reviewed
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
- **Description:** Sector that hosts the garrison, in `A1`..`P16` form. It identifies the map location.
- **Values:** Sector code such as `P3`.
- **Default:** Required; identifies the record.
- **Source:** game code (garrison loader); table data.
- **Confidence:** `source`

### `GARRISON_INFO/GARRISON/Composition`

- **Type:** `Integer` (`UINT8`)
- **Description:** Army composition index assigned to this garrison. It references `ArmyComposition.Index`; it is a numeric ID, not free text.
- **Values:** `ArmyComposition.Index`; the loader stores this field as `UINT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (garrison loader); table data.
- **Confidence:** `source`

## Relations

- `Composition` references [ArmyComposition](ArmyComposition.md) `Index`; the group is filled from [SoldierProfiles](../Profiles/SoldierProfiles.md).

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
