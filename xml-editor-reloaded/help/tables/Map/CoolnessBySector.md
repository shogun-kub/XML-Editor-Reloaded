---
schema: 1
lang: en
family: map-coolness-by-sector
title: "CoolnessBySector - sector coolness map"
tables:
  - CoolnessBySector
table_ids:
  - Map.CoolnessBySector
files:
  - Map/CoolnessBySector.xml
root: COOLNESS_BY_SECTOR
record: MAP_ROW
key: MAP_ROW
sources:
  - Map table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# CoolnessBySector - sector coolness map

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`COOLNESS_BY_SECTOR/MAP_ROW`](#coolness_by_sectormap_row)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`CoolnessBySector.xml` is a 16×16 matrix of sector coolness. Coolness controls the quality/level of equipment available in that sector; each row corresponds to a map row and each position to a map column.

## Tables

| Table | File | Notes |
|---|---|---|
| `CoolnessBySector` | `Map/CoolnessBySector.xml` | — |

## Structure

```
COOLNESS_BY_SECTOR
└── MAP_ROW
```

## Fields

### `COOLNESS_BY_SECTOR/MAP_ROW`

- **Type:** `String`
- **Description:** One row of the sector-coolness matrix. The row attribute is the map row (`01`.`16`); the 16 values are the coolness levels for columns 1.16.
- **Numeric semantics:** higher = higher coolness; negative values are not meaningful.
- **Values:** 16 space-separated coolness levels; observed range `1..20`.
- **Default:** Required; one row of the map.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- Each matrix position uses the normal map row and column; the value controls sector equipment coolness.

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
