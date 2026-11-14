---
schema: 1
lang: en
family: militia
title: "MilitiaIndividual - individual militia data"
tables:
  - MilitiaIndividual
table_ids:
  - MilitiaIndividual
files:
  - MilitiaIndividual.xml
root: MILITIAORIGINDATA
record: ORIGINDATA
key: none
sources:
  - Militia individual table data
  - MilitiaIndividual schema comment
status: reviewed
updated: 2026-11-07
---

# MilitiaIndividual - individual militia data

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MILITIAORIGINDATA/ORIGINDATA/male_forename`](#militiaorigindataorigindatamale_forename)
  - [`MILITIAORIGINDATA/ORIGINDATA/male_surname`](#militiaorigindataorigindatamale_surname)
  - [`MILITIAORIGINDATA/ORIGINDATA/female_forename`](#militiaorigindataorigindatafemale_forename)
  - [`MILITIAORIGINDATA/ORIGINDATA/female_surname`](#militiaorigindataorigindatafemale_surname)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_REGMALE`](#militiaorigindataorigindatachance_bodytype_regmale)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_BIGMALE`](#militiaorigindataorigindatachance_bodytype_bigmale)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_STOCKYMALE`](#militiaorigindataorigindatachance_bodytype_stockymale)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_REGFEMALE`](#militiaorigindataorigindatachance_bodytype_regfemale)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_skin_PINKSKIN`](#militiaorigindataorigindatachance_skin_pinkskin)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_skin_TANSKIN`](#militiaorigindataorigindatachance_skin_tanskin)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_skin_DARKSKIN`](#militiaorigindataorigindatachance_skin_darkskin)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_skin_BLACKSKIN`](#militiaorigindataorigindatachance_skin_blackskin)
  - [`MILITIAORIGINDATA/ORIGINDATA/dailycost_green`](#militiaorigindataorigindatadailycost_green)
  - [`MILITIAORIGINDATA/ORIGINDATA/dailycost_regular`](#militiaorigindataorigindatadailycost_regular)
  - [`MILITIAORIGINDATA/ORIGINDATA/dailycost_elite`](#militiaorigindataorigindatadailycost_elite)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MilitiaIndividual.xml` defines the visual and naming data for individual militia: body-type and skin chances, daily wages and male/female name pools. It does not change combat abilities.

## Tables

| Table | File | Notes |
|---|---|---|
| `MilitiaIndividual` | `MilitiaIndividual.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
MILITIAORIGINDATA
└── ORIGINDATA
    ├── male_forename
    ├── male_surname
    ├── female_forename
    ├── female_surname
    ├── chance_bodytype_REGMALE
    ├── chance_bodytype_BIGMALE
    ├── chance_bodytype_STOCKYMALE
    ├── chance_bodytype_REGFEMALE
    ├── chance_skin_PINKSKIN
    ├── chance_skin_TANSKIN
    ├── chance_skin_DARKSKIN
    ├── chance_skin_BLACKSKIN
    ├── dailycost_green
    ├── dailycost_regular
    └── dailycost_elite
```

`MILITIAORIGINDATA` has no stable per-row key. `ORIGINDATA` rows are selected by origin and
document order; `key: none` records that fact.

## Fields

### `MILITIAORIGINDATA/ORIGINDATA/male_forename`

- **Type:** `String` (`CHAR16`)
- **Description:** Male forename pool used by the random militia generator.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Values:** Free text name, truncated to 29 characters; repeated elements form a pool.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/male_surname`

- **Type:** `String` (`CHAR16`)
- **Description:** Male surname pool.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Values:** Free text name, truncated to 29 characters; repeated elements form a pool.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/female_forename`

- **Type:** `String` (`CHAR16`)
- **Description:** Female forename pool.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Values:** Free text name, truncated to 29 characters; repeated elements form a pool.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/female_surname`

- **Type:** `String` (`CHAR16`)
- **Description:** Female surname pool.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Values:** Free text name, truncated to 29 characters; repeated elements form a pool.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_REGMALE`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a new militia gets the regular-male body type.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Percentage `0..100`; the loader clamps lower values to `0` and higher values to `100`.
- **Values:** `0..100` after clamping; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_BIGMALE`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a new militia gets the big-male body type; body-type chances should sum to 100.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Percentage `0..100`; the loader clamps lower values to `0` and higher values to `100`.
- **Values:** `0..100` after clamping; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_STOCKYMALE`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a new militia gets the stocky-male body type.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Percentage `0..100`; the loader clamps lower values to `0` and higher values to `100`.
- **Values:** `0..100` after clamping; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_REGFEMALE`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a new militia gets the regular-female body type.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Percentage `0..100`; the loader clamps lower values to `0` and higher values to `100`.
- **Values:** `0..100` after clamping; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_skin_PINKSKIN`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a new militia gets pink skin.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Percentage `0..100`; the loader clamps lower values to `0` and higher values to `100`.
- **Values:** `0..100` after clamping; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_skin_TANSKIN`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a new militia gets tan skin.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Percentage `0..100`; the loader clamps lower values to `0` and higher values to `100`.
- **Values:** `0..100` after clamping; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_skin_DARKSKIN`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a new militia gets dark skin.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Percentage `0..100`; the loader clamps lower values to `0` and higher values to `100`.
- **Values:** `0..100` after clamping; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_skin_BLACKSKIN`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that a new militia gets black skin; skin chances should sum to 100.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Percentage `0..100`; the loader clamps lower values to `0` and higher values to `100`.
- **Values:** `0..100` after clamping; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/dailycost_green`

- **Type:** `Integer` (`UINT16`)
- **Description:** Daily wage of a green militia.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Daily wage in money; higher values mean a larger wage, and negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/dailycost_regular`

- **Type:** `Integer` (`UINT16`)
- **Description:** Daily wage of a regular militia.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Daily wage in money; higher values mean a larger wage, and negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/dailycost_elite`

- **Type:** `Integer` (`UINT16`)
- **Description:** Daily wage of an elite militia.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Daily wage in money; higher values mean a larger wage, and negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

## Relations

- The table has no item/profile keys; it selects the militia origin through its own row order.
- The `INDIVIDUAL_MILITIA` option controls whether this data is used.

## Localization

The forename/surname pools and body/skin/wage values are not localized; they are data for the generator.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Militia individual table data.
- MilitiaIndividual schema comment.
- Field types: table data and field names; enum values come from the schema comments.
- Descriptions are conservative where no formula is stated; no observed value ranges are used.

## Open questions

The generated name and appearance selection is code-driven; the table supplies pools and weights.
