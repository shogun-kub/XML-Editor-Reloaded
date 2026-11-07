---
schema: 1
lang: en
family: army-uniform-colors
title: "UniformColors - enemy and militia uniform colours"
tables:
  - UniformColors
table_ids:
  - Army.UniformColors
files:
  - Army/UniformColors.XML
root: UNIFORMS
record: UNIFORMS
key: none
sources:
  - Table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# UniformColors - enemy and militia uniform colours

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`UNIFORMS/ENEMY_ADMIN/Vest`](#uniformsenemy_adminvest)
  - [`UNIFORMS/ENEMY_ADMIN/Pants`](#uniformsenemy_adminpants)
  - [`UNIFORMS/ENEMY_TROOP/Vest`](#uniformsenemy_troopvest)
  - [`UNIFORMS/ENEMY_TROOP/Pants`](#uniformsenemy_trooppants)
  - [`UNIFORMS/ENEMY_ELITE/Vest`](#uniformsenemy_elitevest)
  - [`UNIFORMS/ENEMY_ELITE/Pants`](#uniformsenemy_elitepants)
  - [`UNIFORMS/MILITIA_ROOKIE/Vest`](#uniformsmilitia_rookievest)
  - [`UNIFORMS/MILITIA_ROOKIE/Pants`](#uniformsmilitia_rookiepants)
  - [`UNIFORMS/MILITIA_REGULAR/Vest`](#uniformsmilitia_regularvest)
  - [`UNIFORMS/MILITIA_REGULAR/Pants`](#uniformsmilitia_regularpants)
  - [`UNIFORMS/MILITIA_ELITE/Vest`](#uniformsmilitia_elitevest)
  - [`UNIFORMS/MILITIA_ELITE/Pants`](#uniformsmilitia_elitepants)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`UniformColors.xml` assigns a vest and pants colour to each enemy and militia rank. The values are symbolic colour names consumed by the soldier-rendering code.

## Tables

| Table | File | Notes |
|---|---|---|
| `UniformColors` | `Army/UniformColors.XML` | — |

## Structure

```
UNIFORMS
├── ENEMY_ADMIN
│   ├── Vest
│   └── Pants
├── ENEMY_TROOP
│   ├── Vest
│   └── Pants
├── ENEMY_ELITE
│   ├── Vest
│   └── Pants
├── MILITIA_ROOKIE
│   ├── Vest
│   └── Pants
├── MILITIA_REGULAR
│   ├── Vest
│   └── Pants
└── MILITIA_ELITE
    ├── Vest
    └── Pants
```

## Fields

### `UNIFORMS/ENEMY_ADMIN/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the ENEMY_ADMIN uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/ENEMY_ADMIN/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the ENEMY_ADMIN uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/ENEMY_TROOP/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the ENEMY_TROOP uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/ENEMY_TROOP/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the ENEMY_TROOP uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/ENEMY_ELITE/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the ENEMY_ELITE uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/ENEMY_ELITE/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the ENEMY_ELITE uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/MILITIA_ROOKIE/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the MILITIA_ROOKIE uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/MILITIA_ROOKIE/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the MILITIA_ROOKIE uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/MILITIA_REGULAR/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the MILITIA_REGULAR uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/MILITIA_REGULAR/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the MILITIA_REGULAR uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/MILITIA_ELITE/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the MILITIA_ELITE uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `UNIFORMS/MILITIA_ELITE/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the MILITIA_ELITE uniform. It is a symbolic colour constant used by the soldier graphics, not a number or a quantity.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- The colour names are shared with the soldier-profile graphics; they are display constants, not item references.

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
