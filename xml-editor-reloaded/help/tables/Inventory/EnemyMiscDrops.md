---
schema: 1
lang: en
family: inventory-misc-drops
title: "EnemyMiscDrops - misc drop chances"
tables:
  - EnemyMiscDrops
table_ids:
  - Inventory.EnemyMiscDrops
files:
  - Inventory/EnemyMiscDrops.xml
root: MISCDROPLIST
record: DROPITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# EnemyMiscDrops - misc drop chances

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MISCDROPLIST/DROPITEM/uiIndex`](#miscdroplistdropitemuiindex)
  - [`MISCDROPLIST/DROPITEM/usItemClass`](#miscdroplistdropitemusitemclass)
  - [`MISCDROPLIST/DROPITEM/ubEnemyDropRate`](#miscdroplistdropitemubenemydroprate)
  - [`MISCDROPLIST/DROPITEM/ubMilitiaDropRate`](#miscdroplistdropitemubmilitiadroprate)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyMiscDrops.xml` sets the chance that enemy and militia soldiers drop items from the misc category. Each record selects the item class and gives separate enemy and militia rates.

## Tables

| Table | File | Notes |
|---|---|---|
| `EnemyMiscDrops` | `Inventory/EnemyMiscDrops.xml` | — |

## Structure

```
MISCDROPLIST
└── DROPITEM
    ├── uiIndex
    ├── usItemClass
    ├── ubEnemyDropRate
    └── ubMilitiaDropRate
```

## Fields

### `MISCDROPLIST/DROPITEM/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the drop rule. It is an array key, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MISCDROPLIST/DROPITEM/usItemClass`

- **Type:** `Integer`
- **Description:** Item class selected by this drop rule. It is a category/reference ID, not a quantity.
- **Values:** Identifier of the item class.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MISCDROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer`
- **Description:** Chance that an enemy soldier drops this category, in percent. Higher values make drops more likely; `0` disables them. Numeric semantics: higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MISCDROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer`
- **Description:** Chance that a militia soldier drops this category, in percent. Higher values make drops more likely; `0` disables them. Numeric semantics: higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `usItemClass` references the corresponding item class in the item data.

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
