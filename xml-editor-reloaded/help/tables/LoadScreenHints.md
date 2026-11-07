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
status: draft
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

`LoadScreenHints.xml` stores the tips shown during loading. Each hint has text and one or more category switches; the game selects hints from the enabled categories.

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

- **Type:** `Integer`
- **Description:** Index of the loading-screen hint. It is an array key, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/szName`

- **Type:** `String`
- **Description:** Hint text shown on the loading screen. Despite the `szName` name, this is the full display sentence.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/lore`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `lore` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/weapons`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `weapons` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/item`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `item` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/items`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `items` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/keybind`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `keybind` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/food`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `food` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/zombie`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `zombie` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/overheat`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `overheat` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/ncth`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `ncth` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENHINTS/LOADSCREENHINT/covertops`

- **Type:** `Boolean`
- **Description:** If set, this hint belongs to the `covertops` category and can be selected when that category is enabled. It is a category switch, not a count.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

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
