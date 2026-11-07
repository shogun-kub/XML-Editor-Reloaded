---
schema: 1
lang: en
family: items-structures
title: "StructureMove - movable structure tiles"
tables:
  - StructureMove
table_ids:
  - Items.StructureMove
files:
  - Items/StructureMove.xml
root: STRUCTURESLIST
record: STRUCTURE
key: none
sources:
  - Structure move table data
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# StructureMove - movable structure tiles

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`](#structuresliststructuresztilesetdisplayname)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetName`](#structuresliststructuresztilesetname)
  - [`STRUCTURESLIST/STRUCTURE/allowedtile`](#structuresliststructureallowedtile)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`StructureMove.xml` lists tilesets whose structures can be moved and the tile indices on which moving is allowed.

## Tables

| Table | File | Notes |
|---|---|---|
| `StructureMove` | `Items/StructureMove.xml` | v4 and v5 have the same field paths; values differ. `allowedtile` repeats. |

## Structure

```
STRUCTURESLIST
└── STRUCTURE
    ├── szTileSetDisplayName
    ├── szTileSetName
    └── allowedtile
```

`STRUCTURESLIST` has no stable per-row key; a structure row is identified by its tile-set
fields and document order.

## Fields

### `STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`

- **Type:** `String` (`String`)
- **Description:** Display name of the movable structure tileset.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** structure move table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/szTileSetName`

- **Type:** `String` (`String`)
- **Description:** STI tileset file used to draw the movable structure.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** structure move table data; field metadata.
- **Confidence:** `inferred`

### `STRUCTURESLIST/STRUCTURE/allowedtile`

- **Type:** `Integer` (`UINT16`)
- **Description:** Tile index on which moving is allowed. The element may repeat. Numeric semantics: Boolean switch stored as a number: `0` is off and `1` is on; larger or negative values are not meaningful.
- **Values:** Allowed tile index; the element may repeat.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** structure move table data; field metadata.
- **Confidence:** `inferred`

## Relations

- The tileset columns refer to STI assets, not to item rows.
- Moving a structure does not consume an item.

## Localization

`szTileSetDisplayName` is the player-facing tileset name; `szTileSetName` is an asset key.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Structure move table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The table lists allowed tile indices but does not state the movement cost or collision rules.
