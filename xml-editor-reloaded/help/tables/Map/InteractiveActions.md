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

`InteractiveActions.xml` binds tilesets and structure indexes to an action type, difficulty, optional sector/level restrictions and an optional Lua action. Repeated `usTileIndex` elements widen one action to several structures; for a sector-specific row, repeated `sStructureGridno` elements widen it to several exact locations.

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

`ACTION` has no key element. The loader stores rows in document order; when several rows match,
a sector-specific match takes precedence over an anywhere row.

## Fields

### `INTERACTIVEACTIONS/ACTION/szTileSetName`

- **Type:** `String` (`CHAR8`)
- **Description:** Tileset file that contains the interactive structure, for example `o_furn.sti`. It is matched case-insensitively against the tileset's tile-surface file names, comparing the first 11 characters.
- **Values:** Free text; stored in a 20-byte buffer without appending a terminator, so longer text is truncated and may fill the buffer.
- **Default:** Empty when omitted; an empty name matches no tileset.
- **Source:** game code (interactive-actions loader; tileset matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/usTileIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Structure index within `szTileSetName` that triggers this action; the game compares it with the structure's `usStructureNumber`. The element may repeat, and any matching entry applies.
- **Values:** `0..65535`; the loader stores this field as `UINT16`. The element may repeat.
- **Default:** Empty list when omitted (the row matches no structure); an empty element adds index `0`.
- **Source:** game code (interactive-actions loader; structure matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/sActionType`

- **Type:** `Integer` (`UINT16`)
- **Description:** Action type code used by the interaction system: `0` no action, `1` hack a computer, `2` read a file, `3` take water, `4` buy soda, `5` play a minigame, `6` various.
- **Values:** `0..6` have defined behaviour; the loader stores the value as `UINT16` without range validation.
- **Default:** `0` (no action); an empty element is parsed as `0`.
- **Source:** game code (interactive-actions loader; `INTERACTIVE_STRUCTURE_TYPE` enum); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/difficulty`

- **Type:** `Integer` (`INT32`)
- **Description:** Difficulty threshold compared with the acting soldier's interactive-action skill. The skill is derived from the action type (hacking background skill, wisdom, or a fixed value such as `100`) and is normally on a `0..100` scale. The default action succeeds when the skill is at least this value; the multi-turn hacking completion path passes `skill > difficulty` to the default result.
- **Numeric semantics:** Higher values mean harder. The loader stores the value without clamping or rejecting it; because the skill is unsigned and the comparison widens to `INT32`, a negative difficulty makes the check trivially true.
- **Values:** Integer (no code range).
- **Default:** `0`; an empty element is parsed as `0`.
- **Source:** game code (interactive-actions loader; interactive-action skill check and default result); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/SectorGrid`

- **Type:** `String`
- **Description:** Sector restriction, written as a row letter followed by a column number (`A1`..`P16`). The loader converts it to a sector ID and keeps it only when the row and column are both `1..16`; matching also requires `sectorlevel` to equal the current sector level.
- **Numeric semantics:** The first character is the row (masked with `0x1F`, so lower case also works) and the remaining characters are the decimal column. An absent or invalid value leaves the default sector `-1`, which means no sector restriction.
- **Values:** `A1`..`P16`.
- **Default:** Absent or invalid = `-1` (any sector); an empty element also leaves `-1`.
- **Source:** game code (interactive-actions loader; sector matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/sectorlevel`

- **Type:** `Integer` (`INT8`)
- **Description:** Sector level for the restriction: `0` surface, `1..3` basement, `-1` any. The loader clamps the value to `-1..3`. A sector-specific row matches only when this equals the current sector level, so a row with `-1` behaves as an anywhere row only when `SectorGrid` is also absent.
- **Values:** `-1..3` (clamped).
- **Default:** `-1` (any); an empty element is parsed as `0` (surface).
- **Source:** game code (interactive-actions loader; sector matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/sStructureGridno`

- **Type:** `Integer` (`INT32`)
- **Description:** Exact structure grid number (the structure's base grid) for a sector-specific action. The element may repeat, and any matching entry applies. It is consulted only when `SectorGrid` and `sectorlevel` are set; for an anywhere row the game ignores it.
- **Values:** Grid number; the loader stores this field as `INT32`. The element may repeat.
- **Default:** Empty list when omitted; an empty element adds grid number `0`.
- **Source:** game code (interactive-actions loader; structure matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/sLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** Map level used with `sStructureGridno` to refine an exact-location match for a sector-specific action: `0` floor, `1` roof, `-1` don't care. The loader clamps the value to `0..1` when the element is present. For an anywhere row the game ignores it.
- **Values:** `-1..1` (`-1` when omitted, `0..1` when present).
- **Default:** `-1` (don't care); an empty element is parsed as `0` (floor).
- **Source:** game code (interactive-actions loader; structure matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/luaactionid`

- **Type:** `Integer` (`INT32`)
- **Description:** Lua action ID. When it is `>= 0`, the game calls the Lua interactive-action result handler (`HandleInteractiveActionResult`) with this ID; `-1` means no Lua call and the fixed default result is used.
- **Values:** `-1` disables the Lua call; `0..2147483647` is the ID passed to Lua.
- **Default:** `-1`; an empty element is parsed as `0`, which requests a Lua call with ID `0`.
- **Source:** game code (interactive-actions loader; interactive-action result dispatch); table data and XML comment.
- **Confidence:** `source`

## Relations

- `usTileIndex` and `szTileSetName` identify the map structure; `luaactionid` is passed to the Lua interactive-action result handler in addition to the fixed result.

## Localization

This table contains no display text; all fields are structural identifiers or numeric values and are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Map table data.
- Game code: interactive-actions loader, tileset and structure matching, sector matching, interactive-action result dispatch.
- Field names, XML comments and the `INTERACTIVE_STRUCTURE_TYPE` enum.
- Descriptions cover the values the loader stores and the match logic that uses them; every field in this document is backed by the loader.

## Open questions

The Lua contract of `HandleInteractiveActionResult` is defined by the Lua scripts, not by this table.
