---
schema: 1
lang: en
family: mercs-availability
title: "MercAvailability - MERC availability schedule"
tables:
  - MercAvailability
table_ids:
  - MercAvailability
files:
  - MercAvailability.xml
root: MERC_AVAILABLES
record: MERC
key: uiIndex
sources:
  - MERC availability table data
status: reviewed
updated: 2026-11-07
---

# MercAvailability - MERC availability schedule

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERC_AVAILABLES/MERC/uiIndex`](#merc_availablesmercuiindex)
  - [`MERC_AVAILABLES/MERC/Name`](#merc_availablesmercname)
  - [`MERC_AVAILABLES/MERC/Drunk`](#merc_availablesmercdrunk)
  - [`MERC_AVAILABLES/MERC/uiAlternateIndex`](#merc_availablesmercuialternateindex)
  - [`MERC_AVAILABLES/MERC/StartMercsAvailable`](#merc_availablesmercstartmercsavailable)
  - [`MERC_AVAILABLES/MERC/NewMercsAvailable`](#merc_availablesmercnewmercsavailable)
  - [`MERC_AVAILABLES/MERC/MercBioID`](#merc_availablesmercmercbioid)
  - [`MERC_AVAILABLES/MERC/ProfilId`](#merc_availablesmercprofilid)
  - [`MERC_AVAILABLES/MERC/usMoneyPaid`](#merc_availablesmercusmoneypaid)
  - [`MERC_AVAILABLES/MERC/usDay`](#merc_availablesmercusday)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercAvailability.xml` defines when MERC mercenaries become available, how much they cost and their alternate availability row.

## Tables

| Table | File | Notes |
|---|---|---|
| `MercAvailability` | `MercAvailability.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
MERC_AVAILABLES
└── MERC
    ├── uiIndex
    ├── Name
    ├── Drunk
    ├── uiAlternateIndex
    ├── StartMercsAvailable
    ├── NewMercsAvailable
    ├── MercBioID
    ├── ProfilId
    ├── usMoneyPaid
    └── usDay
```

## Fields

### `MERC_AVAILABLES/MERC/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Availability row identity.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERC_AVAILABLES/MERC/Name`

- **Type:** `String` (`String`)
- **Description:** Display name of the mercenary.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERC_AVAILABLES/MERC/Drunk`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Marks the MERC as drunk for its availability/dialogue state.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERC_AVAILABLES/MERC/uiAlternateIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Alternate availability index; `-1` means none.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERC_AVAILABLES/MERC/StartMercsAvailable`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Whether the mercenary is available at the start.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERC_AVAILABLES/MERC/NewMercsAvailable`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Whether the mercenary becomes available later.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERC_AVAILABLES/MERC/MercBioID`

- **Type:** `Integer` (`INT32`)
- **Description:** Biography ID for the MERC site.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERC_AVAILABLES/MERC/ProfilId`

- **Type:** `Integer` (`INT32`)
- **Description:** Profile ID of the MERC mercenary.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERC_AVAILABLES/MERC/usMoneyPaid`

- **Type:** `Integer` (`INT32`)
- **Description:** Money paid value associated with the availability row.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERC_AVAILABLES/MERC/usDay`

- **Type:** `Integer` (`INT32`)
- **Description:** Day on which the availability changes. Numeric semantics: Day number; higher values mean a later day, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc availability table data; item/profile metadata.
- **Confidence:** `inferred`

## Relations

- `ProfilId` points to [MercProfiles](MercProfiles.md).

## Localization

`Name` is a display label.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- MERC availability table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The exact schedule calculation using `usDay` and `usMoneyPaid` is defined by the MERC availability code.
