---
schema: 1
lang: en
family: map-sam-sites
title: "SamSites - SAM sites and covered sectors"
tables:
  - SamSites
table_ids:
  - Map.SamSites
files:
  - Map/SamSites.xml
root: SAM_INFO
record: SAMLIST/SAM
key: samIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# SamSites - SAM sites and covered sectors

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SAM_INFO/SAMLIST/SAM/samIndex`](#sam_infosamlistsamsamindex)
  - [`SAM_INFO/SAMLIST/SAM/samSector/x`](#sam_infosamlistsamsamsectorx)
  - [`SAM_INFO/SAMLIST/SAM/samSector/y`](#sam_infosamlistsamsamsectory)
  - [`SAM_INFO/SAMLIST/SAM/samHidden`](#sam_infosamlistsamsamhidden)
  - [`SAM_INFO/SAMLIST/SAM/samOrientation`](#sam_infosamlistsamsamorientation)
  - [`SAM_INFO/SAMLIST/SAM/samGridNoA`](#sam_infosamlistsamsamgridnoa)
  - [`SAM_INFO/SAMLIST/SAM/samGridNoB`](#sam_infosamlistsamsamgridnob)
  - [`SAM_INFO/SAM_CONTROLLED_SECTORS/MAP_ROW`](#sam_infosam_controlled_sectorsmap_row)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SamSites.xml` defines SAM installations and the matrix of sectors they cover. Each SAM has a sector, direction and two grid positions; the coverage matrix maps each column to a SAM index.

## Tables

| Table | File | Notes |
|---|---|---|
| `SamSites` | `Map/SamSites.xml` | — |

## Structure

```
SAM_INFO
├── SAMLIST
│   └── SAM
│       ├── samIndex
│       ├── samSector
│       │   ├── x
│       │   └── y
│       ├── samHidden
│       ├── samOrientation
│       ├── samGridNoA
│       └── samGridNoB
└── SAM_CONTROLLED_SECTORS
    └── MAP_ROW
```

## Fields

### `SAM_INFO/SAMLIST/SAM/samIndex`

- **Type:** `Integer`
- **Description:** Index of the SAM site. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SAM_INFO/SAMLIST/SAM/samSector/x`

- **Type:** `Integer`
- **Description:** Map column of the SAM sector. Lower values are farther west; it is a coordinate.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SAM_INFO/SAMLIST/SAM/samSector/y`

- **Type:** `Integer`
- **Description:** Map row of the SAM sector. Lower values are farther north; it is a coordinate.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SAM_INFO/SAMLIST/SAM/samHidden`

- **Type:** `Boolean`
- **Description:** If set, the SAM site is hidden until discovered.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SAM_INFO/SAMLIST/SAM/samOrientation`

- **Type:** `Integer`
- **Description:** Facing direction of the SAM site, as a direction code. It is an orientation code.
- **Values:** Direction code; observed values `3` and `4`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SAM_INFO/SAMLIST/SAM/samGridNoA`

- **Type:** `Integer`
- **Description:** First map grid position of the SAM installation. It is a grid identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SAM_INFO/SAMLIST/SAM/samGridNoB`

- **Type:** `Integer`
- **Description:** Second map grid position of the SAM installation. It is a grid identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SAM_INFO/SAM_CONTROLLED_SECTORS/MAP_ROW`

- **Type:** `String`
- **Description:** One row of the SAM coverage matrix. Values are SAM-site indexes, or `0` for no coverage; positions are columns, not quantities.
- **Values:** Space-separated indexes, one per map column; `0` = no SAM.
- **Default:** Required; one row of the map.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `samSector/x` and `samSector/y` identify a sector named in [SectorNames](SectorNames.md); the coverage matrix uses the same map columns.

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
