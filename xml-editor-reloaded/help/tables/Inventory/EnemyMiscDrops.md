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
status: reviewed
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

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the drop rule. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `MISCDROPLIST/DROPITEM/usItemClass`

- **Type:** `Flags` (`UINT32`)
- **Description:** Bitmask of item classes selected by this drop rule.
- **Numeric semantics:** Bitmask; combine item-class bits with OR, so a larger number is not simply “more”.
- **Values:** Bits from `Lookup.ItemClass`.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `MISCDROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that an enemy soldier drops this category, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `MISCDROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a militia soldier drops this category, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

## Relations

- `usItemClass` references [Lookup.ItemClass](../Lookup.md#itemclass).

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
