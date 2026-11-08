---
schema: 1
lang: en
family: items-structures
title: "StructureConstruct - buildable structures"
tables:
  - StructureConstruct
table_ids:
  - Items.StructureConstruct
files:
  - Items/StructureConstruct.xml
root: STRUCTURESLIST
record: STRUCTURE
key: none
sources:
  - Structure construction table data
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# StructureConstruct - buildable structures

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`STRUCTURESLIST/STRUCTURE/usCreationItem`](#structuresliststructureuscreationitem)
  - [`STRUCTURESLIST/STRUCTURE/usItemStatusLoss`](#structuresliststructureusitemstatusloss)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`](#structuresliststructuresztilesetdisplayname)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetName`](#structuresliststructuresztilesetname)
  - [`STRUCTURESLIST/STRUCTURE/dCreationCost`](#structuresliststructuredcreationcost)
  - [`STRUCTURESLIST/STRUCTURE/fFortifyAdjacentAdjustment`](#structuresliststructureffortifyadjacentadjustment)
  - [`STRUCTURESLIST/STRUCTURE/northfacingtile`](#structuresliststructurenorthfacingtile)
  - [`STRUCTURESLIST/STRUCTURE/southfacingtile`](#structuresliststructuresouthfacingtile)
  - [`STRUCTURESLIST/STRUCTURE/eastfacingtile`](#structuresliststructureeastfacingtile)
  - [`STRUCTURESLIST/STRUCTURE/westfacingtile`](#structuresliststructurewestfacingtile)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`StructureConstruct.xml` defines structures built from an item: the consumed item, its status loss, the tileset and facing tiles, the cost and the fortification adjustment.

## Tables

| Table | File | Notes |
|---|---|---|
| `StructureConstruct` | `Items/StructureConstruct.xml` | v4 and v5 have the same field paths; values differ. `*facingtile` elements repeat. |

## Structure

```
STRUCTURESLIST
└── STRUCTURE
    ├── usCreationItem
    ├── usItemStatusLoss
    ├── szTileSetDisplayName
    ├── szTileSetName
    ├── dCreationCost
    ├── fFortifyAdjacentAdjustment
    ├── northfacingtile
    ├── southfacingtile
    ├── eastfacingtile
    └── westfacingtile
```

`STRUCTURESLIST` has no stable per-row key; a structure row is identified by its tile-set
fields and document order.

## Fields

### `STRUCTURESLIST/STRUCTURE/usCreationItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID consumed to build the structure.
- **Values:** Item ID consumed to build the structure.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/usItemStatusLoss`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item status lost by the creation item when the structure is built.
- **Values:** Status lost by the creation item; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`

- **Type:** `String` (`String`)
- **Description:** Display name of the structure tileset.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/szTileSetName`

- **Type:** `String` (`String`)
- **Description:** STI tileset file used to draw the structure.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/dCreationCost`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Creation cost of the structure. The unit is defined by the construction system; it is a decimal value.
- **Numeric semantics:** Amount of money or cost; higher values mean a larger amount, and negative values are not meaningful.
- **Values:** Decimal creation cost.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/fFortifyAdjacentAdjustment`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Fortification adjustment applied to adjacent tiles when the structure is built.
- **Numeric semantics:** Positive values strengthen adjacent fortification; negative values weaken it, and `0` is neutral.
- **Values:** Decimal fortification adjustment applied to adjacent tiles.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/northfacingtile`

- **Type:** `Integer` (`UINT16`)
- **Description:** Tile index used for the north-facing variant. The element may repeat for several tiles.
- **Values:** Tile index in the tileset; the element may repeat for several tiles.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/southfacingtile`

- **Type:** `Integer` (`UINT16`)
- **Description:** Tile index used for the south-facing variant. The element may repeat for several tiles.
- **Values:** Tile index in the tileset; the element may repeat for several tiles.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/eastfacingtile`

- **Type:** `Integer` (`UINT16`)
- **Description:** Tile index used for the east-facing variant. The element may repeat for several tiles.
- **Values:** Tile index in the tileset; the element may repeat for several tiles.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/westfacingtile`

- **Type:** `Integer` (`UINT16`)
- **Description:** Tile index used for the west-facing variant. The element may repeat for several tiles.
- **Values:** Tile index in the tileset; the element may repeat for several tiles.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure construction table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `usCreationItem` points to [Items](Items.md).
- The tileset columns refer to STI assets, not to item rows.

## Localization

`szTileSetDisplayName` is the player-facing tileset name; `szTileSetName` is an asset key.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Structure construction table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The unit of `dCreationCost` and the exact fortification formula are defined outside the table.
