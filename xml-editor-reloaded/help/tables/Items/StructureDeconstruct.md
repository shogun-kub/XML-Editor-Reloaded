---
schema: 1
lang: en
family: items-structures
title: "StructureDeconstruct - deconstructable structures"
tables:
  - StructureDeconstruct
table_ids:
  - Items.StructureDeconstruct
files:
  - Items/StructureDeconstruct.xml
root: STRUCTURESLIST
record: STRUCTURE
key: none
sources:
  - Structure deconstruction table data
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# StructureDeconstruct - deconstructable structures

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`STRUCTURESLIST/STRUCTURE/usDeconstructItem`](#structuresliststructureusdeconstructitem)
  - [`STRUCTURESLIST/STRUCTURE/usItemToCreate`](#structuresliststructureusitemtocreate)
  - [`STRUCTURESLIST/STRUCTURE/usCreatedItemStatus`](#structuresliststructureuscreateditemstatus)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`](#structuresliststructuresztilesetdisplayname)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetName`](#structuresliststructuresztilesetname)
  - [`STRUCTURESLIST/STRUCTURE/dCreationCost`](#structuresliststructuredcreationcost)
  - [`STRUCTURESLIST/STRUCTURE/allowedtile`](#structuresliststructureallowedtile)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`StructureDeconstruct.xml` defines how a structure is removed and what item it yields: the tool item, the created item and status, the tileset, the cost and the allowed tiles.

## Tables

| Table | File | Notes |
|---|---|---|
| `StructureDeconstruct` | `Items/StructureDeconstruct.xml` | v4 and v5 have the same field paths; values differ. `allowedtile` repeats. |

## Structure

```
STRUCTURESLIST
└── STRUCTURE
    ├── usDeconstructItem
    ├── usItemToCreate
    ├── usCreatedItemStatus
    ├── szTileSetDisplayName
    ├── szTileSetName
    ├── dCreationCost
    └── allowedtile
```

`STRUCTURESLIST` has no stable per-row key; a structure row is identified by its tile-set
fields and document order.

## Fields

### `STRUCTURESLIST/STRUCTURE/usDeconstructItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID used to deconstruct the structure. Numeric semantics: Item ID; it selects an item row rather than measuring an amount, and negative values are invalid.
- **Values:** Item ID used to deconstruct the structure.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure deconstruction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/usItemToCreate`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID created when the structure is deconstructed. Numeric semantics: Item ID; it selects an item row rather than measuring an amount, and negative values are invalid.
- **Values:** Item ID created by deconstruction.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure deconstruction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/usCreatedItemStatus`

- **Type:** `Integer` (`UINT16`)
- **Description:** Status of the created item. It uses the same status units as item condition. Numeric semantics: Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Status of the created item, in the same status units as item condition.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure deconstruction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`

- **Type:** `String` (`String`)
- **Description:** Display name of the structure tileset.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** structure deconstruction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/szTileSetName`

- **Type:** `String` (`String`)
- **Description:** STI tileset file used to draw the structure.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** structure deconstruction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/dCreationCost`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Deconstruction cost. The unit is defined by the construction system; it is a decimal value. Numeric semantics: Amount of money or cost; higher values mean a larger amount, and negative values are not meaningful.
- **Values:** Decimal deconstruction cost.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure deconstruction table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/allowedtile`

- **Type:** `Integer` (`UINT16`)
- **Description:** Tile index on which deconstruction is allowed. The element may repeat. Numeric semantics: Boolean switch stored as a number: `0` is off and `1` is on; larger or negative values are not meaningful.
- **Values:** Allowed tile index; the element may repeat.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure deconstruction table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `usDeconstructItem` and `usItemToCreate` point to [Items](Items.md).
- The tileset columns refer to STI assets, not to item rows.

## Localization

`szTileSetDisplayName` is the player-facing tileset name; `szTileSetName` is an asset key.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Structure deconstruction table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The unit of `dCreationCost` and the exact status rounding for the created item are defined outside the table.
