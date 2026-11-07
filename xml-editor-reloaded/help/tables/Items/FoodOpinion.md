---
schema: 1
lang: en
family: items-food
title: "FoodOpinion - profile food preferences"
tables:
  - FoodOpinion
table_ids:
  - Items.FoodOpinion
files:
  - Items/FoodOpinion.xml
root: FOODOPINIONSLIST
record: FOODOPINION
key: uProfile + FoodNumber
sources:
  - Food opinion table data
  - Item metadata
status: draft
updated: 2026-11-07
---

# FoodOpinion - profile food preferences

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`FOODOPINIONSLIST/FOODOPINION/uProfile`](#foodopinionslistfoodopinionuprofile)
  - [`FOODOPINIONSLIST/FOODOPINION/PROFILE_OPINION/FoodNumber`](#foodopinionslistfoodopinionprofile_opinionfoodnumber)
  - [`FOODOPINIONSLIST/FOODOPINION/PROFILE_OPINION/MoraleMod`](#foodopinionslistfoodopinionprofile_opinionmoralemod)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`FoodOpinion.xml` stores per-profile food preferences: each row links a profile to a food row and gives the morale modifier for that combination.

## Tables

| Table | File | Notes |
|---|---|---|
| `FoodOpinion` | `Items/FoodOpinion.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
FOODOPINIONSLIST
└── FOODOPINION
    ├── uProfile
    └── PROFILE_OPINION
        ├── FoodNumber
        └── MoraleMod
```

## Fields

### `FOODOPINIONSLIST/FOODOPINION/uProfile`

- **Type:** `Integer` (`UINT16`)
- **Description:** Profile whose food preference is described by this row.
- **Only used with:** Food system.
- **Values:** Profile ID; the same profile can have several `PROFILE_OPINION` rows.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** food opinion table data; field metadata.
- **Confidence:** `inferred`

### `FOODOPINIONSLIST/FOODOPINION/PROFILE_OPINION/FoodNumber`

- **Type:** `Integer` (`UINT16`)
- **Description:** Food row that the profile likes or dislikes.
- **Only used with:** Food system.
- **Values:** Food row index in [Food](Food.md).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** food opinion table data; field metadata.
- **Confidence:** `inferred`

### `FOODOPINIONSLIST/FOODOPINION/PROFILE_OPINION/MoraleMod`

- **Type:** `Integer` (`INT8`)
- **Description:** Morale modifier applied to this profile when it consumes the food.
- **Only used with:** Food system.
- **Values:** Morale modifier for this profile and food.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** food opinion table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `uProfile` points to a profile; `FoodNumber` points to [Food](Food.md).
- The food system must be active for these preferences to affect gameplay.

## Localization

This table has no display strings; profile and food names come from their own tables.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Food opinion table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and the table schema comments; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The table stores one direction per profile; the UI may merge preferences from several rows.
