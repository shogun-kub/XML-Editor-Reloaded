---
schema: 1
lang: en
family: items-food
title: "Food - food and drink values"
tables:
  - Food
table_ids:
  - Items.Food
files:
  - Items/Food.xml
root: FOODSLIST
record: FOOD
key: uiIndex
sources:
  - Food table data
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# Food - food and drink values

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`FOODSLIST/FOOD/uiIndex`](#foodslistfooduiindex)
  - [`FOODSLIST/FOOD/szName`](#foodslistfoodszname)
  - [`FOODSLIST/FOOD/bFoodPoints`](#foodslistfoodbfoodpoints)
  - [`FOODSLIST/FOOD/bDrinkPoints`](#foodslistfoodbdrinkpoints)
  - [`FOODSLIST/FOOD/usDecayRate`](#foodslistfoodusdecayrate)
  - [`FOODSLIST/FOOD/bMoraleMod`](#foodslistfoodbmoralemod)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Food.xml` defines the food and drink values of consumable items: food points, drink points, morale modifier and decay rate.

## Tables

| Table | File | Notes |
|---|---|---|
| `Food` | `Items/Food.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
FOODSLIST
└── FOOD
    ├── uiIndex
    ├── szName
    ├── bFoodPoints
    ├── bDrinkPoints
    ├── usDecayRate
    └── bMoraleMod
```

## Fields

### `FOODSLIST/FOOD/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the food row and identity of the XML record.
- **Only used with:** Food system.
- **Values:** Non-negative index; unique within the file. It is referenced by the item row through the food type.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** food table data; field metadata.
- **Confidence:** `inferred`

### `FOODSLIST/FOOD/szName`

- **Type:** `String` (`String`)
- **Description:** Display name of the food or drink.
- **Only used with:** Food system.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** food table data; field metadata.
- **Confidence:** `inferred`

### `FOODSLIST/FOOD/bFoodPoints`

- **Type:** `Integer` (`INT16`)
- **Description:** Food points supplied when the item is consumed.
- **Only used with:** Food system.
- **Values:** Food points supplied by the item; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** food table data; field metadata.
- **Confidence:** `inferred`

### `FOODSLIST/FOOD/bDrinkPoints`

- **Type:** `Integer` (`INT16`)
- **Description:** Drink points supplied when the item is consumed.
- **Only used with:** Food system.
- **Values:** Drink points supplied by the item; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** food table data; field metadata.
- **Confidence:** `inferred`

### `FOODSLIST/FOOD/usDecayRate`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Decay rate of the food item; the unit is defined by the food system.
- **Only used with:** Food system.
- **Values:** Decimal decay rate; the unit is defined by the food system.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** food table data; field metadata.
- **Confidence:** `inferred`

### `FOODSLIST/FOOD/bMoraleMod`

- **Type:** `Integer` (`INT8`)
- **Description:** Morale change applied when the item is consumed.
- **Only used with:** Food system.
- **Values:** Morale change applied when the food is consumed.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** food table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `uiIndex` is referenced by the item food type and by [FoodOpinion](FoodOpinion.md).
- The food system must be active for these values to affect gameplay.

## Localization

`szName` is a display label.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Food table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and the table schema comments; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The food-point scale and decay unit are defined by the food system, not by this table.
