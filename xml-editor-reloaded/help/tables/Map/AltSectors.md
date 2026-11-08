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

- **Type:** `String` (`CHAR8`)
- **Description:** One row of the alternate-sector matrix. The row is taken from the `y` attribute (letter `A`..`P` or a number); the element text is read as a whitespace-separated sequence of numeric values, one per column, and written into the row starting at column 1. `0` means no alternate sector.
- **Numeric semantics:** Values are alternate-sector IDs, so larger is not “more”; `0` = none.
- **Values:** Up to 16 numeric values per row; the loader does not validate the count.
- **Default:** Required; one row of the map.
- **Source:** game code (alternate-sector loader); map table data.
- **Confidence:** `source`

## Relations

- Each matrix position uses the normal map row and column; the sector coordinates are shared with [SectorNames](SectorNames.md), and the value selects an alternate-sector group.

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
