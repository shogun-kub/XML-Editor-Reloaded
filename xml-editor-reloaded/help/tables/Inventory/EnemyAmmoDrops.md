---
schema: 1
lang: en
family: inventory-ammo-drops
title: "EnemyAmmoDrops - ammo drop chances"
tables:
  - EnemyAmmoDrops
table_ids:
  - Inventory.EnemyAmmoDrops
files:
  - Inventory/EnemyAmmoDrops.xml
root: AMMODROPLIST
record: DROPITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# EnemyAmmoDrops - ammo drop chances

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`AMMODROPLIST/DROPITEM/uiIndex`](#ammodroplistdropitemuiindex)
  - [`AMMODROPLIST/DROPITEM/uiType`](#ammodroplistdropitemuitype)
  - [`AMMODROPLIST/DROPITEM/ubEnemyDropRate`](#ammodroplistdropitemubenemydroprate)
  - [`AMMODROPLIST/DROPITEM/ubMilitiaDropRate`](#ammodroplistdropitemubmilitiadroprate)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyAmmoDrops.xml` sets the chance that enemy and militia soldiers drop items from the ammo category. Each record selects the ammo type and gives separate enemy and militia rates.

## Tables

| Table | File | Notes |
|---|---|---|
| `EnemyAmmoDrops` | `Inventory/EnemyAmmoDrops.xml` | — |

## Structure

```
AMMODROPLIST
└── DROPITEM
    ├── uiIndex
    ├── uiType
    ├── ubEnemyDropRate
    └── ubMilitiaDropRate
```

## Fields

### `AMMODROPLIST/DROPITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the drop rule. It is an array key.
- **Values:** `0..4294967295`; the loader stores this field as `UINT32`.
- **Default:** Required; identifies the record.
- **Source:** game code (ammo-drop loader); table data.
- **Confidence:** `source`

### `AMMODROPLIST/DROPITEM/uiType`

- **Type:** `Integer` (`UINT32`)
- **Description:** Ammo type selected by this drop rule. It is a category/reference ID.
- **Values:** Ammo-type ID; the loader stores this field as `UINT32`.
- **Default:** Absent; the game default applies.
- **Source:** game code (ammo-drop loader); table data.
- **Confidence:** `source`

### `AMMODROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that an enemy soldier drops this category, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`; the loader stores this field as `UINT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (ammo-drop loader); table data.
- **Confidence:** `source`

### `AMMODROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a militia soldier drops this category, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`; the loader stores this field as `UINT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (ammo-drop loader); table data.
- **Confidence:** `source`

## Relations

- `uiType` references an ammo type in [AmmoTypes](../Items/AmmoTypes.md).

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
