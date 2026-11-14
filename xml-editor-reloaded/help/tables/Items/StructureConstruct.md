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
updated: 2026-11-08
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

`STRUCTURESLIST` has no key element. The loader stores rows in document order; when a
soldier builds with an item, the game selects the first row whose `usCreationItem` matches the
held item and whose facing list for the soldier's direction is non-empty, and the fortification
plan stores the resulting row index.

## Fields

### `STRUCTURESLIST/STRUCTURE/usCreationItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID consumed to build the structure. A value of `0` means no item is consumed by the automatic fortification builder, and a row with `0` cannot be selected by holding an item.
- **Values:** Item ID; `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (structure-construct loader; construction item consumption); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/usItemStatusLoss`

- **Type:** `Integer` (`UINT8`)
- **Description:** Item status (condition) removed from the creation item each time the structure is built. If the item's remaining status is less than or equal to this value, the item is deleted instead of reduced.
- **Numeric semantics:** Percentage points of item condition on the `objectStatus` scale. `0` means the item is not damaged.
- **Values:** `0..255`; the loader stores this field as `UINT8`, and the item code treats it as a condition percentage (`0..100` is the normal range).
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (structure-construct loader; construction item consumption and fortification resource consumption); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`

- **Type:** `String` (`CHAR8`)
- **Description:** Player-facing name of the structure, listed in the fortification build menu for construct entries whose tileset is available in the current tileset set (or in the generic set).
- **Values:** Free text; stored in a 20-byte buffer without appending a terminator, so longer text is truncated and may fill the buffer.
- **Default:** Empty string when omitted.
- **Source:** game code (structure-construct loader; fortification build-menu list); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/szTileSetName`

- **Type:** `String` (`CHAR8`)
- **Description:** Tileset file name whose tiles represent the structure. It is matched case-insensitively against a prefix of the tileset's tile-surface file names: 10 characters in the hand-build path and 11 in the build-menu and plan paths.
- **Values:** Free text such as `sandbag.sti`; stored in a 20-byte buffer without appending a terminator, so longer text is truncated.
- **Default:** Empty string when omitted; an empty name matches no tileset.
- **Source:** game code (structure-construct loader; tileset matching); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/dCreationCost`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Cost of building one structure, expressed in fortification work (progress) points. The cost is summed into the sector's maximum fortification progress when the plan is evaluated and is subtracted as each structure is built. It is not a money value.
- **Numeric semantics:** Fractional progress points; higher values mean more work. The loader stores the parsed value without clamping or rejecting it.
- **Values:** Decimal (no code range).
- **Default:** `1.0` from the `STRUCTURE_CONSTRUCT` constructor when omitted. An empty element is parsed as `0`.
- **Source:** game code (structure-construct loader; fortification progress accounting and consumption); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/fFortifyAdjacentAdjustment`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the game tries to make a newly built fortification node match adjacent nodes of the same construct row by choosing compatible direction variants from the facing-tile lists.
- **Values:** `0` or `1`.
- **Default:** `1` (TRUE) from the `STRUCTURE_CONSTRUCT` constructor when omitted. An empty element is parsed as `0`.
- **Source:** game code (structure-construct loader; adjacent-node adjustment); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/northfacingtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Tile index in the tileset used when the builder faces north. The element may repeat; the game picks one entry at random from this list.
- **Numeric semantics:** Index within the tileset; `0..255`. The union of the four facing lists is the set of structures offered in the fortification build menu. A build for a cardinal direction needs that direction's list; for a diagonal facing the game tries north, then south, then west, then east. When `fFortifyAdjacentAdjustment` is set, the lists also supply the compatible variants for adjacent nodes.
- **Values:** `0..255`; the element may repeat.
- **Default:** Empty list when omitted (not `0`); an empty element adds index `0`.
- **Source:** game code (structure-construct loader; fortification build selection); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/southfacingtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Tile index in the tileset used when the builder faces south. The element may repeat; the game picks one entry at random from this list.
- **Numeric semantics:** Index within the tileset; `0..255`. The union of the four facing lists is the set of structures offered in the fortification build menu. A build for a cardinal direction needs that direction's list; for a diagonal facing the game tries north, then south, then west, then east. When `fFortifyAdjacentAdjustment` is set, the lists also supply the compatible variants for adjacent nodes.
- **Values:** `0..255`; the element may repeat.
- **Default:** Empty list when omitted (not `0`); an empty element adds index `0`.
- **Source:** game code (structure-construct loader; fortification build selection); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/eastfacingtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Tile index in the tileset used when the builder faces east. The element may repeat; the game picks one entry at random from this list.
- **Numeric semantics:** Index within the tileset; `0..255`. The union of the four facing lists is the set of structures offered in the fortification build menu. A build for a cardinal direction needs that direction's list; for a diagonal facing the game tries north, then south, then west, then east. When `fFortifyAdjacentAdjustment` is set, the lists also supply the compatible variants for adjacent nodes.
- **Values:** `0..255`; the element may repeat.
- **Default:** Empty list when omitted (not `0`); an empty element adds index `0`.
- **Source:** game code (structure-construct loader; fortification build selection); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/westfacingtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Tile index in the tileset used when the builder faces west. The element may repeat; the game picks one entry at random from this list.
- **Numeric semantics:** Index within the tileset; `0..255`. The union of the four facing lists is the set of structures offered in the fortification build menu. A build for a cardinal direction needs that direction's list; for a diagonal facing the game tries north, then south, then west, then east. When `fFortifyAdjacentAdjustment` is set, the lists also supply the compatible variants for adjacent nodes.
- **Values:** `0..255`; the element may repeat.
- **Default:** Empty list when omitted (not `0`); an empty element adds index `0`.
- **Source:** game code (structure-construct loader; fortification build selection); structure construction table data.
- **Confidence:** `source`

## Relations

- `usCreationItem` points to [Items](Items.md).
- The facing-tile columns refer to tiles (structure indexes) in the named tileset, not to item rows.

## Localization

`szTileSetDisplayName` is the player-facing structure name and is converted from UTF-8 to UTF-16 when the build menu is built; `szTileSetName` is an asset key.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Structure construction table data.
- Game code: structure-construct loader, construction item consumption, fortification build menu, fortification progress accounting.
- Defaults from the `STRUCTURE_CONSTRUCT` record.
- Descriptions cover the values the code reads, stores and uses; every field in this document is backed by the loader and its consumer.

## Open questions

- The tileset-name prefix comparison is 10 characters in the hand-build path and 11 characters in the build-menu and plan paths, so a name that differs only at position 11 can behave differently between those paths.
- The number of construction points generated per assignment work tick, and therefore the practical balance of `dCreationCost` values, is defined by the assignment system outside this table.
