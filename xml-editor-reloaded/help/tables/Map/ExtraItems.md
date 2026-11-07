---
schema: 1
lang: en
family: map-extra-items
title: "ExtraItems - extra sector items by difficulty"
tables:
  - A2_0_ExtraItems_Experienced
  - A2_0_ExtraItems_Expert
  - A2_0_ExtraItems_Insane
  - A2_0_ExtraItems_Novice
  - A9_0_ExtraItems_Experienced
  - A9_0_ExtraItems_Expert
  - A9_0_ExtraItems_Insane
  - A9_0_ExtraItems_Novice
  - C5_0_ExtraItems_Experienced
  - C5_0_ExtraItems_Expert
  - C5_0_ExtraItems_Insane
  - C5_0_ExtraItems_Novice
  - H14_0_ExtraItems_Experienced
  - H14_0_ExtraItems_Expert
  - H14_0_ExtraItems_Insane
  - H14_0_ExtraItems_Novice
  - I13_0_ExtraItems_Experienced
  - I13_0_ExtraItems_Expert
  - I13_0_ExtraItems_Insane
  - I13_0_ExtraItems_Novice
table_ids:
  - Map.ExtraItems.A2_0_ExtraItems_Experienced
  - Map.ExtraItems.A2_0_ExtraItems_Expert
  - Map.ExtraItems.A2_0_ExtraItems_Insane
  - Map.ExtraItems.A2_0_ExtraItems_Novice
  - Map.ExtraItems.A9_0_ExtraItems_Experienced
  - Map.ExtraItems.A9_0_ExtraItems_Expert
  - Map.ExtraItems.A9_0_ExtraItems_Insane
  - Map.ExtraItems.A9_0_ExtraItems_Novice
  - Map.ExtraItems.C5_0_ExtraItems_Experienced
  - Map.ExtraItems.C5_0_ExtraItems_Expert
  - Map.ExtraItems.C5_0_ExtraItems_Insane
  - Map.ExtraItems.C5_0_ExtraItems_Novice
  - Map.ExtraItems.H14_0_ExtraItems_Experienced
  - Map.ExtraItems.H14_0_ExtraItems_Expert
  - Map.ExtraItems.H14_0_ExtraItems_Insane
  - Map.ExtraItems.H14_0_ExtraItems_Novice
  - Map.ExtraItems.I13_0_ExtraItems_Experienced
  - Map.ExtraItems.I13_0_ExtraItems_Expert
  - Map.ExtraItems.I13_0_ExtraItems_Insane
  - Map.ExtraItems.I13_0_ExtraItems_Novice
files:
  - Map/ExtraItems/A2_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/A2_0_ExtraItems_Expert.xml
  - Map/ExtraItems/A2_0_ExtraItems_Insane.xml
  - Map/ExtraItems/A2_0_ExtraItems_Novice.xml
  - Map/ExtraItems/A9_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/A9_0_ExtraItems_Expert.xml
  - Map/ExtraItems/A9_0_ExtraItems_Insane.xml
  - Map/ExtraItems/A9_0_ExtraItems_Novice.xml
  - Map/ExtraItems/C5_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/C5_0_ExtraItems_Expert.xml
  - Map/ExtraItems/C5_0_ExtraItems_Insane.xml
  - Map/ExtraItems/C5_0_ExtraItems_Novice.xml
  - Map/ExtraItems/H14_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/H14_0_ExtraItems_Expert.xml
  - Map/ExtraItems/H14_0_ExtraItems_Insane.xml
  - Map/ExtraItems/H14_0_ExtraItems_Novice.xml
  - Map/ExtraItems/I13_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/I13_0_ExtraItems_Expert.xml
  - Map/ExtraItems/I13_0_ExtraItems_Insane.xml
  - Map/ExtraItems/I13_0_ExtraItems_Novice.xml
root: ExtraItems
record: Item
key: uiIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# ExtraItems - extra sector items by difficulty

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ExtraItems/Item/uiIndex`](#extraitemsitemuiindex)
  - [`ExtraItems/Item/quantity`](#extraitemsitemquantity)
  - [`ExtraItems/Item/condition`](#extraitemsitemcondition)
  - [`ExtraItems/Item/gridno`](#extraitemsitemgridno)
  - [`ExtraItems/Item/visible`](#extraitemsitemvisible)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`ExtraItems` files add items to specific sectors. Each file is named by sector and difficulty: `A2_0` is sector A2 level 0, and `Novice`, `Experienced`, `Expert` or `Insane` selects the difficulty. All files share the same five item fields.

## Tables

| Table | File | Notes |
|---|---|---|
| `A2_0_ExtraItems_Experienced` | `Map/ExtraItems/A2_0_ExtraItems_Experienced.xml` | — |
| `A2_0_ExtraItems_Expert` | `Map/ExtraItems/A2_0_ExtraItems_Expert.xml` | — |
| `A2_0_ExtraItems_Insane` | `Map/ExtraItems/A2_0_ExtraItems_Insane.xml` | — |
| `A2_0_ExtraItems_Novice` | `Map/ExtraItems/A2_0_ExtraItems_Novice.xml` | — |
| `A9_0_ExtraItems_Experienced` | `Map/ExtraItems/A9_0_ExtraItems_Experienced.xml` | — |
| `A9_0_ExtraItems_Expert` | `Map/ExtraItems/A9_0_ExtraItems_Expert.xml` | — |
| `A9_0_ExtraItems_Insane` | `Map/ExtraItems/A9_0_ExtraItems_Insane.xml` | — |
| `A9_0_ExtraItems_Novice` | `Map/ExtraItems/A9_0_ExtraItems_Novice.xml` | — |
| `C5_0_ExtraItems_Experienced` | `Map/ExtraItems/C5_0_ExtraItems_Experienced.xml` | — |
| `C5_0_ExtraItems_Expert` | `Map/ExtraItems/C5_0_ExtraItems_Expert.xml` | — |
| `C5_0_ExtraItems_Insane` | `Map/ExtraItems/C5_0_ExtraItems_Insane.xml` | — |
| `C5_0_ExtraItems_Novice` | `Map/ExtraItems/C5_0_ExtraItems_Novice.xml` | — |
| `H14_0_ExtraItems_Experienced` | `Map/ExtraItems/H14_0_ExtraItems_Experienced.xml` | — |
| `H14_0_ExtraItems_Expert` | `Map/ExtraItems/H14_0_ExtraItems_Expert.xml` | — |
| `H14_0_ExtraItems_Insane` | `Map/ExtraItems/H14_0_ExtraItems_Insane.xml` | — |
| `H14_0_ExtraItems_Novice` | `Map/ExtraItems/H14_0_ExtraItems_Novice.xml` | — |
| `I13_0_ExtraItems_Experienced` | `Map/ExtraItems/I13_0_ExtraItems_Experienced.xml` | — |
| `I13_0_ExtraItems_Expert` | `Map/ExtraItems/I13_0_ExtraItems_Expert.xml` | — |
| `I13_0_ExtraItems_Insane` | `Map/ExtraItems/I13_0_ExtraItems_Insane.xml` | — |
| `I13_0_ExtraItems_Novice` | `Map/ExtraItems/I13_0_ExtraItems_Novice.xml` | — |

## Structure

```
ExtraItems
└── Item
    ├── uiIndex
    ├── quantity
    ├── condition
    ├── gridno
    └── visible
```

## Fields

### `ExtraItems/Item/uiIndex`

- **Type:** `Integer`
- **Description:** Item ID placed in the sector. It is an item reference, not a quantity.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ExtraItems/Item/quantity`

- **Type:** `Integer`
- **Description:** Number of items placed. Higher values mean more copies; `0` places none. Numeric semantics: higher = more items; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ExtraItems/Item/condition`

- **Type:** `Integer`
- **Description:** Condition of the placed item, in percent. Higher values mean better condition; `100` is pristine. Numeric semantics: higher = better condition; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ExtraItems/Item/gridno`

- **Type:** `Integer`
- **Description:** Map grid number where the item is placed. It is a grid identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ExtraItems/Item/visible`

- **Type:** `Boolean`
- **Description:** If set, the item is visible on the map from the start. It is a switch, not a quantity.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `Item/uiIndex` references an item in `Items.xml`; the sector and difficulty are encoded in the file name, not in a field.

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
