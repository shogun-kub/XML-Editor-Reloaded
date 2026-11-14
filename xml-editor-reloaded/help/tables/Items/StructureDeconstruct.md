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
updated: 2026-11-08
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

`STRUCTURESLIST` has no key element. The loader stores rows in document order; a row is
selected by the held deconstruction item (`usDeconstructItem`) and then checked against the
tileset name and allowed structure number, or, for automatic fortification removal, by the
tileset name with the structure number deciding whether removal is allowed.

## Fields

### `STRUCTURESLIST/STRUCTURE/usDeconstructItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID required to deconstruct the structure. A value of `0` matches no real item, so it disables item-based deconstruction for the row.
- **Values:** Item ID; `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (structure-deconstruct loader; deconstruction item matching); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/usItemToCreate`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID created when the structure is deconstructed. The item is created only when both this field and `usCreatedItemStatus` are non-zero.
- **Values:** Item ID; `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted (no item is created); an empty element is parsed as `0`.
- **Source:** game code (structure-deconstruct loader; deconstruction result); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/usCreatedItemStatus`

- **Type:** `Integer` (`UINT8`)
- **Description:** Condition given to the created item, in item-status (condition) percent. A value of `0` means no item is created; a non-zero value is passed to item creation as the starting condition.
- **Numeric semantics:** Percentage points of item condition on the `objectStatus` scale. The item-creation call guards against zero with `max(1, status)`, but the stored value is an unsigned byte, so a non-zero value is already at least `1`.
- **Values:** `0..255`; the loader stores this field as `UINT8`, and the item code treats it as a condition percentage (`0..100` is the normal range).
- **Default:** `0` when omitted (no item is created); an empty element is parsed as `0`.
- **Source:** game code (structure-deconstruct loader; item creation on deconstruction); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`

- **Type:** `String` (`CHAR8`)
- **Description:** Name of the structure intended for the player (the struct comment calls it the structure name, because tileset names are obscure). The loader stores it, but no gameplay or UI consumer of this field was found in this source tree.
- **Values:** Free text; stored in a 20-byte buffer without appending a terminator, so longer text is truncated and may fill the buffer.
- **Default:** Empty string when omitted.
- **Source:** game code (structure-deconstruct loader and `STRUCTURE_DECONSTRUCT` comment); table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/szTileSetName`

- **Type:** `String` (`CHAR8`)
- **Description:** Tileset file name of the structure. It is matched case-insensitively against the tileset's tile-surface file names, comparing the first 11 characters.
- **Values:** Free text such as `sandbag.sti`; stored in a 20-byte buffer without appending a terminator, so longer text is truncated.
- **Default:** Empty string when omitted; an empty name matches no tileset.
- **Source:** game code (structure-deconstruct loader; tileset matching); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/dCreationCost`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Cost of removing one structure, expressed in fortification work (progress) points. The cost is summed into the sector's maximum fortification progress for a removal plan and is subtracted as each structure is removed. It is not a money value.
- **Numeric semantics:** Fractional progress points; higher values mean more work. The loader stores the parsed value without clamping or rejecting it.
- **Values:** Decimal (no code range).
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (structure-deconstruct loader; fortification progress accounting and consumption); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/allowedtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Structure number that may be removed by this row. The game compares it with the structure's `usStructureNumber`, truncated to `UINT8`, after the tileset name matches; the element may repeat, and any matching entry allows removal.
- **Numeric semantics:** Structure number (`usStructureNumber`) within the tileset; `0..255`.
- **Values:** `0..255`; the element may repeat.
- **Default:** Empty list when omitted (no structure is removable by the row); an empty element adds entry `0`.
- **Source:** game code (structure-deconstruct loader; deconstruction tile matching); structure deconstruction table data.
- **Confidence:** `source`

## Relations

- `usDeconstructItem` and `usItemToCreate` point to [Items](Items.md).
- The tileset columns refer to structures in the named tileset, not to item rows.

## Localization

`szTileSetDisplayName` is intended as the player-facing structure name, but no consumer was
found in this source tree; `szTileSetName` is an asset key.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Structure deconstruction table data.
- Game code: structure-deconstruct loader, deconstruction item matching, item creation, fortification progress accounting.
- Defaults from the `STRUCTURE_DECONSTRUCT` record.
- Descriptions cover the values the code reads, stores and uses; every field in this document is backed by the loader.

## Open questions

- Whether `szTileSetDisplayName` is used by any UI outside this source tree is unknown; no read of it was found in the shipped code.
- The number of construction points generated per assignment work tick, and therefore the practical balance of `dCreationCost` values, is defined by the assignment system outside this table.
