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
status: reviewed
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

`UniformColors.xml` assigns a vest and pants colour to each enemy and militia rank. All six uniform blocks are mandatory; the loader throws an error if any is missing. Values must be one of the case-sensitive palette names listed in the XML comment, not arbitrary text.

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
- **Description:** Vest colour code for the ENEMY_ADMIN uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_ADMIN/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the ENEMY_ADMIN uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_TROOP/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the ENEMY_TROOP uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_TROOP/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the ENEMY_TROOP uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_ELITE/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the ENEMY_ELITE uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_ELITE/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the ENEMY_ELITE uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_ROOKIE/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the MILITIA_ROOKIE uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_ROOKIE/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the MILITIA_ROOKIE uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_REGULAR/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the MILITIA_REGULAR uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_REGULAR/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the MILITIA_REGULAR uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_ELITE/Vest`

- **Type:** `String`
- **Description:** Vest colour code for the MILITIA_ELITE uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_ELITE/Pants`

- **Type:** `String`
- **Description:** Pants colour code for the MILITIA_ELITE uniform. It is a symbolic colour constant used by the soldier graphics.
- **Values:** Case-sensitive palette name from the XML comment: vest `YELLOWVEST`, `BLACKSHIRT`, `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`, `PURPLESHIRT` or `BROWNVEST`; pants `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`, `JEANPANTS` or `BLUEPANTS`. Free text is not valid.
- **Default:** Required; all six uniform blocks must define both `Vest` and `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

## Relations

- The colour names are shared with [SoldierProfiles](../Profiles/SoldierProfiles.md) graphics; they are display constants, not item references.

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
