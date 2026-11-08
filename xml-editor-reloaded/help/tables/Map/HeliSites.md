---
schema: 1
lang: en
family: map-heli-sites
title: "HeliSites - helicopter refuelling sites"
tables:
  - HeliSites
table_ids:
  - Map.HeliSites
files:
  - Map/HeliSites.xml
root: HELI_INFO
record: REFUELLIST/REFUEL
key: refuelIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# HeliSites - helicopter refuelling sites

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelIndex`](#heli_inforefuellistrefuelrefuelindex)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelSector/x`](#heli_inforefuellistrefuelrefuelsectorx)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelSector/y`](#heli_inforefuellistrefuelrefuelsectory)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelHidden`](#heli_inforefuellistrefuelrefuelhidden)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelHeliTileIndex`](#heli_inforefuellistrefuelrefuelhelitileindex)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelHeliGridNo`](#heli_inforefuellistrefuelrefuelheligridno)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelSkyriderGridNo`](#heli_inforefuellistrefuelrefuelskyridergridno)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`HeliSites.xml` lists helicopter refuelling sites. Each record places the site in a sector and gives the grid positions for the helicopter and Skyrider plus an optional hidden flag.

## Tables

| Table | File | Notes |
|---|---|---|
| `HeliSites` | `Map/HeliSites.xml` | — |

## Structure

```
HELI_INFO
└── REFUELLIST
    └── REFUEL
        ├── refuelIndex
        ├── refuelSector
        │   ├── x
        │   └── y
        ├── refuelHidden
        ├── refuelHeliTileIndex
        ├── refuelHeliGridNo
        └── refuelSkyriderGridNo
```

## Fields

### `HELI_INFO/REFUELLIST/REFUEL/refuelIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the helicopter refuelling site. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelSector/x`

- **Type:** `Integer` (`INT16`)
- **Description:** Map column of the refuelling sector. Lower values are farther west; it is a coordinate.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelSector/y`

- **Type:** `Integer` (`INT16`)
- **Description:** Map row of the refuelling sector. Lower values are farther north; it is a coordinate.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelHidden`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the refuelling site is hidden from the player until discovered.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelHeliTileIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Tile index of the helicopter landing/refuelling graphic. It is a tile identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelHeliGridNo`

- **Type:** `Integer` (`INT32`)
- **Description:** Map grid number where the helicopter lands for refuelling. It is a grid identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelSkyriderGridNo`

- **Type:** `Integer` (`INT32`)
- **Description:** Map grid number where Skyrider stands at the refuelling site. It is a grid identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

## Relations

- `refuelSector/x` and `refuelSector/y` identify a sector whose names and weather are defined in [SectorNames](SectorNames.md).

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Map table data.
- Field names, XML comments and observed values.
- Descriptions are based on the field names and observed table values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for fields marked as inferred is not stated by the table data.
