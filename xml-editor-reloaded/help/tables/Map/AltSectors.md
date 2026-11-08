---
schema: 1
lang: en
family: map-alt-sectors
title: "AltSectors - alternate sector map"
tables:
  - AltSectors
table_ids:
  - Map.AltSectors
files:
  - Map/AltSectors.xml
root: ALT_SECTORS_LIST
record: ROW
key: ROW
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# AltSectors - alternate sector map

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ALT_SECTORS_LIST/ROW`](#alt_sectors_listrow)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AltSectors.xml` is a 16×16 matrix. Each `ROW` element has a `y` attribute for the map row and 16 values, one per map column. A non-zero value selects the alternate-sector group used for that sector.

## Tables

| Table | File | Notes |
|---|---|---|
| `AltSectors` | `Map/AltSectors.xml` | — |

## Structure

```
ALT_SECTORS_LIST
└── ROW
```

## Fields

### `ALT_SECTORS_LIST/ROW`

- **Type:** `String`
- **Description:** One row of the alternate-sector matrix. The row attribute is the map row (`A`..`P`); the 16 space-separated values are the alternate sector group for columns 1..16. `0` means no alternate sector. The values are IDs, so larger values do not mean “more” alternate.
- **Values:** 16 space-separated indexes; `0` = none.
- **Default:** Required; one row of the map.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- Each matrix position uses the normal map row and column; the value selects an alternate-sector group.

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
