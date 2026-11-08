---
schema: 1
lang: en
family: inventory-explosive-drops
title: "EnemyExplosiveDrops - explosive drop chances"
tables:
  - EnemyExplosiveDrops
table_ids:
  - Inventory.EnemyExplosiveDrops
files:
  - Inventory/EnemyExplosiveDrops.xml
root: EXPLOSIVEDROPLIST
record: DROPITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# EnemyExplosiveDrops - explosive drop chances

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`EXPLOSIVEDROPLIST/DROPITEM/uiIndex`](#explosivedroplistdropitemuiindex)
  - [`EXPLOSIVEDROPLIST/DROPITEM/ubType`](#explosivedroplistdropitemubtype)
  - [`EXPLOSIVEDROPLIST/DROPITEM/ubEnemyDropRate`](#explosivedroplistdropitemubenemydroprate)
  - [`EXPLOSIVEDROPLIST/DROPITEM/ubMilitiaDropRate`](#explosivedroplistdropitemubmilitiadroprate)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyExplosiveDrops.xml` sets the chance that enemy and militia soldiers drop items from the explosive category. Each record selects the explosive type and gives separate enemy and militia rates.

## Tables

| Table | File | Notes |
|---|---|---|
| `EnemyExplosiveDrops` | `Inventory/EnemyExplosiveDrops.xml` | — |

## Structure

```
EXPLOSIVEDROPLIST
└── DROPITEM
    ├── uiIndex
    ├── ubType
    ├── ubEnemyDropRate
    └── ubMilitiaDropRate
```

## Fields

### `EXPLOSIVEDROPLIST/DROPITEM/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the drop rule. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `EXPLOSIVEDROPLIST/DROPITEM/ubType`

- **Type:** `Integer`
- **Description:** Explosive type selected by this drop rule. It is a category/reference ID.
- **Values:** Identifier of the explosive type.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `EXPLOSIVEDROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer`
- **Description:** Chance that an enemy soldier drops this category, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `EXPLOSIVEDROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer`
- **Description:** Chance that a militia soldier drops this category, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `ubType` references an explosive type in [Explosives](../Items/Explosives.md).

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
