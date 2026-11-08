---
schema: 1
lang: en
family: map-interactive-actions
title: "InteractiveActions - interactive tile actions"
tables:
  - InteractiveActions
table_ids:
  - Map.InteractiveActions
files:
  - Map/InteractiveActions.xml
root: INTERACTIVEACTIONS
record: ACTION
key: none
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# InteractiveActions - interactive tile actions

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`INTERACTIVEACTIONS/ACTION/szTileSetName`](#interactiveactionsactionsztilesetname)
  - [`INTERACTIVEACTIONS/ACTION/usTileIndex`](#interactiveactionsactionustileindex)
  - [`INTERACTIVEACTIONS/ACTION/sActionType`](#interactiveactionsactionsactiontype)
  - [`INTERACTIVEACTIONS/ACTION/difficulty`](#interactiveactionsactiondifficulty)
  - [`INTERACTIVEACTIONS/ACTION/SectorGrid`](#interactiveactionsactionsectorgrid)
  - [`INTERACTIVEACTIONS/ACTION/sectorlevel`](#interactiveactionsactionsectorlevel)
  - [`INTERACTIVEACTIONS/ACTION/sStructureGridno`](#interactiveactionsactionsstructuregridno)
  - [`INTERACTIVEACTIONS/ACTION/sLevel`](#interactiveactionsactionslevel)
  - [`INTERACTIVEACTIONS/ACTION/luaactionid`](#interactiveactionsactionluaactionid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`InteractiveActions.xml` binds tilesets and tile indexes to an action type, difficulty, optional sector/structure restrictions and an optional Lua action. Repeated `usTileIndex` or `sStructureGridno` elements widen one action to several tiles or structures.

## Tables

| Table | File | Notes |
|---|---|---|
| `InteractiveActions` | `Map/InteractiveActions.xml` | — |

## Structure

```
INTERACTIVEACTIONS
└── ACTION
    ├── szTileSetName
    ├── usTileIndex
    ├── sActionType
    ├── difficulty
    ├── SectorGrid
    ├── sectorlevel
    ├── sStructureGridno
    ├── sLevel
    └── luaactionid
```

## Fields

### `INTERACTIVEACTIONS/ACTION/szTileSetName`

- **Type:** `String`
- **Description:** Tileset file that contains the interactive tile, for example `o_furn.sti`.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `INTERACTIVEACTIONS/ACTION/usTileIndex`

- **Type:** `Integer`
- **Description:** Tile index inside `szTileSetName` that triggers this action. The element may repeat to cover several tiles; it is an ID.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `INTERACTIVEACTIONS/ACTION/sActionType`

- **Type:** `Integer`
- **Description:** Action type code used by the interaction system. It is an enum code.
- **Values:** Action code; observed value `1` for computer/hacking actions.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `INTERACTIVEACTIONS/ACTION/difficulty`

- **Type:** `Integer`
- **Description:** Difficulty of the interaction, in percent.
- **Numeric semantics:** higher = harder; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `INTERACTIVEACTIONS/ACTION/SectorGrid`

- **Type:** `String`
- **Description:** Optional sector restriction for the action. An empty value means the tile works in any sector.
- **Values:** Free text.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `INTERACTIVEACTIONS/ACTION/sectorlevel`

- **Type:** `Integer`
- **Description:** Optional underground level restriction for the sector. It is a level.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `INTERACTIVEACTIONS/ACTION/sStructureGridno`

- **Type:** `Integer`
- **Description:** Optional map structure grid number that must match for the action. The element may repeat; it is a grid ID.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `INTERACTIVEACTIONS/ACTION/sLevel`

- **Type:** `Integer`
- **Description:** Optional map level restriction for the structure. It is a level.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `INTERACTIVEACTIONS/ACTION/luaactionid`

- **Type:** `Integer`
- **Description:** Lua action identifier invoked by this interaction. It is an ID.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- The action uses tile-set and tile indexes from the map graphics; `luaactionid` refers to the Lua action implementation.

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
