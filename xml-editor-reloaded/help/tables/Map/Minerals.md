---
schema: 1
lang: en
family: map-minerals
title: "Minerals - mine mineral types"
tables:
  - Minerals
table_ids:
  - Map.Minerals
files:
  - Map/Minerals.xml
root: MINERALS
record: TYPE
key: uiIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# Minerals - mine mineral types

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MINERALS/TYPE/uiIndex`](#mineralstypeuiindex)
  - [`MINERALS/TYPE/sNameType`](#mineralstypesnametype)
  - [`MINERALS/TYPE/sMineral`](#mineralstypesmineral)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Minerals.xml` names the mineral types produced by mines. The index is the value used by mine sectors and facilities.

## Tables

| Table | File | Notes |
|---|---|---|
| `Minerals` | `Map/Minerals.xml` | — |

## Structure

```
MINERALS
└── TYPE
    ├── uiIndex
    ├── sNameType
    └── sMineral
```

## Fields

### `MINERALS/TYPE/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the mineral type. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MINERALS/TYPE/sNameType`

- **Type:** `String`
- **Description:** Facility label used for this mineral type, for example `Mine`.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MINERALS/TYPE/sMineral`

- **Type:** `String`
- **Description:** Display name of the mineral, for example `Silver` or `Gold`.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- Mine sectors and facilities use the `uiIndex` of a mineral type to select the produced mineral; the owning town is defined in [Cities](Cities.md).

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Map table data.
- Field names, XML comments and observed values.
- Descriptions are based on the field names and observed table values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for fields marked as inferred is not stated by the table data.
