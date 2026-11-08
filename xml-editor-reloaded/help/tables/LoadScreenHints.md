---
schema: 1
lang: en
family: load-screen-hints
title: "LoadScreenHints - loading-screen hints"
tables:
  - LoadScreenHints
table_ids:
  - LoadScreenHints
files:
  - LoadScreenHints.xml
root: LOADSCREENHINTS
record: LOADSCREENHINT
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# LoadScreenHints - loading-screen hints

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`LOADSCREENHINTS/LOADSCREENHINT/uiIndex`](#loadscreenhintsloadscreenhintuiindex)
  - [`LOADSCREENHINTS/LOADSCREENHINT/szName`](#loadscreenhintsloadscreenhintszname)
  - [`LOADSCREENHINTS/LOADSCREENHINT/lore`](#loadscreenhintsloadscreenhintlore)
  - [`LOADSCREENHINTS/LOADSCREENHINT/weapons`](#loadscreenhintsloadscreenhintweapons)
  - [`LOADSCREENHINTS/LOADSCREENHINT/item`](#loadscreenhintsloadscreenhintitem)
  - [`LOADSCREENHINTS/LOADSCREENHINT/items`](#loadscreenhintsloadscreenhintitems)
  - [`LOADSCREENHINTS/LOADSCREENHINT/keybind`](#loadscreenhintsloadscreenhintkeybind)
  - [`LOADSCREENHINTS/LOADSCREENHINT/food`](#loadscreenhintsloadscreenhintfood)
  - [`LOADSCREENHINTS/LOADSCREENHINT/zombie`](#loadscreenhintsloadscreenhintzombie)
  - [`LOADSCREENHINTS/LOADSCREENHINT/overheat`](#loadscreenhintsloadscreenhintoverheat)
  - [`LOADSCREENHINTS/LOADSCREENHINT/ncth`](#loadscreenhintsloadscreenhintncth)
  - [`LOADSCREENHINTS/LOADSCREENHINT/covertops`](#loadscreenhintsloadscreenhintcovertops)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`LoadScreenHints.xml` stores the tips shown during loading. Each hint has text and one or more category switches; the game selects hints from the enabled categories. The legacy `item` element occurs in shipped data but is ignored by the current loader; `items` is the element that sets the item-category flag.

## Tables

| Table | File | Notes |
|---|---|---|
| `LoadScreenHints` | `LoadScreenHints.xml` | — |

## Structure

```
LOADSCREENHINTS
└── LOADSCREENHINT
    ├── uiIndex
    ├── szName
    ├── lore
    ├── weapons
    ├── item
    ├── items
    ├── keybind
    ├── food
    ├── zombie
    ├── overheat
    ├── ncth
    └── covertops
```

## Fields

### `LOADSCREENHINTS/LOADSCREENHINT/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index of the loading-screen hint. It is an array key.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** Required; identifies the record.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/szName`

- **Type:** `String` (`CHAR16`)
- **Description:** Hint text shown on the loading screen. Despite the `szName` name, this is the full display sentence.
- **Values:** Free text, up to 190 characters.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/lore`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, ORs the `0x0001` category bit into the hint's internal flags.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/weapons`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, ORs the `0x0002` category bit into the hint's internal flags.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/item`

- **Type:** `Boolean`
- **Description:** Not read by the current loader; only `items` sets the item-category bit. Kept as a data field for compatibility.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/items`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, ORs the `0x0004` category bit into the hint's internal flags.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/keybind`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, ORs the `0x0008` category bit into the hint's internal flags.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/food`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, ORs the `0x0010` category bit into the hint's internal flags.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/zombie`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, ORs the `0x0020` category bit into the hint's internal flags.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/overheat`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, ORs the `0x0040` category bit into the hint's internal flags.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/ncth`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, ORs the `0x0080` category bit into the hint's internal flags.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/covertops`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, ORs the `0x0100` category bit into the hint's internal flags.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

## Relations

- This table is part of the strategic army and inventory data; references are described in the field notes.

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
