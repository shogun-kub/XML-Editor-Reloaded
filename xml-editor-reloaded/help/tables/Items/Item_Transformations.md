---
schema: 1
lang: en
family: items-merges
title: "Item_Transformations - item transformations"
tables:
  - Item_Transformations
table_ids:
  - Items.Item_Transformations
files:
  - Items/Item_Transformations.xml
root: TRANSFORMATIONS_LIST
record: TRANSFORM
key: usItem
sources:
  - Item transformation table data
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# Item_Transformations - item transformations

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usItem`](#transformations_listtransformusitem)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult1`](#transformations_listtransformusresult1)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult2`](#transformations_listtransformusresult2)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult3`](#transformations_listtransformusresult3)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult4`](#transformations_listtransformusresult4)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult5`](#transformations_listtransformusresult5)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult6`](#transformations_listtransformusresult6)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult7`](#transformations_listtransformusresult7)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult8`](#transformations_listtransformusresult8)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult9`](#transformations_listtransformusresult9)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult10`](#transformations_listtransformusresult10)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usAPCost`](#transformations_listtransformusapcost)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/iBPCost`](#transformations_listtransformibpcost)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/szMenuRowText`](#transformations_listtransformszmenurowtext)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/szTooltipText`](#transformations_listtransformsztooltiptext)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Item_Transformations.xml` defines a transformation menu for an item: AP and BP costs, up to ten result items, and the menu/tooltip text shown to the player.

## Tables

| Table | File | Notes |
|---|---|---|
| `Item_Transformations` | `Items/Item_Transformations.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
TRANSFORMATIONS_LIST
└── TRANSFORM
    ├── usItem
    ├── usResult1
    ├── usResult2
    ├── usResult3
    ├── usResult4
    ├── usResult5
    ├── usResult6
    ├── usResult7
    ├── usResult8
    ├── usResult9
    ├── usResult10
    ├── usAPCost
    ├── iBPCost
    ├── szMenuRowText
    └── szTooltipText
```

## Fields

### `TRANSFORMATIONS_LIST/TRANSFORM/usItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the item being transformed.
- **Values:** Item ID of the item being transformed.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 1. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID of the first result; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 2. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 3. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 4. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 5. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 6. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 7. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 8. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 9. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 10. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/usAPCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** AP cost of the transformation. Numeric semantics: Amount of money or cost; higher values mean a larger amount, and negative values are not meaningful.
- **Values:** AP cost; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/iBPCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** Blueprint/creation-point cost of the transformation. Numeric semantics: Amount of money or cost; higher values mean a larger amount, and negative values are not meaningful.
- **Values:** Blueprint/creation-point cost; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/szMenuRowText`

- **Type:** `String` (`String`)
- **Description:** Text shown in the transformation menu row. It is a short action label, not a full sentence.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

### `TRANSFORMATIONS_LIST/TRANSFORM/szTooltipText`

- **Type:** `String` (`String`)
- **Description:** Tooltip shown for the transformation menu row.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** item transformation table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `usItem` and `usResult1..10` point to [Items](Items.md).
- The menu and tooltip strings are presentation data; the action itself is defined by the costs and results.

## Localization

`szMenuRowText` and `szTooltipText` are player-facing text and may be localized by the item overlay.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Item transformation table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The unit of `iBPCost` and the exact menu ordering are not stated by the table.
