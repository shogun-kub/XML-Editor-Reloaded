---
schema: 1
lang: en
family: inventory-weapon-drops
title: "EnemyWeaponDrops - weapon drop chances"
tables:
  - EnemyWeaponDrops
table_ids:
  - Inventory.EnemyWeaponDrops
files:
  - Inventory/EnemyWeaponDrops.xml
root: WEAPONDROPLIST
record: DROPITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# EnemyWeaponDrops - weapon drop chances

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`WEAPONDROPLIST/DROPITEM/uiIndex`](#weapondroplistdropitemuiindex)
  - [`WEAPONDROPLIST/DROPITEM/ubWeaponType`](#weapondroplistdropitemubweapontype)
  - [`WEAPONDROPLIST/DROPITEM/ubEnemyDropRate`](#weapondroplistdropitemubenemydroprate)
  - [`WEAPONDROPLIST/DROPITEM/ubMilitiaDropRate`](#weapondroplistdropitemubmilitiadroprate)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyWeaponDrops.xml` sets the chance that enemy and militia soldiers drop items from the weapon category. Each record selects the weapon type and gives separate enemy and militia rates.

## Tables

| Table | File | Notes |
|---|---|---|
| `EnemyWeaponDrops` | `Inventory/EnemyWeaponDrops.xml` | — |

## Structure

```
WEAPONDROPLIST
└── DROPITEM
    ├── uiIndex
    ├── ubWeaponType
    ├── ubEnemyDropRate
    └── ubMilitiaDropRate
```

## Fields

### `WEAPONDROPLIST/DROPITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the drop rule. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `WEAPONDROPLIST/DROPITEM/ubWeaponType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Weapon type selected by this drop rule. It is a category/reference ID.
- **Values:** Identifier of the weapon type.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `WEAPONDROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that an enemy soldier drops this category, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `WEAPONDROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a militia soldier drops this category, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

## Relations

- `ubWeaponType` references [Lookup.WeaponType](../Lookup.md#weapontype).

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
