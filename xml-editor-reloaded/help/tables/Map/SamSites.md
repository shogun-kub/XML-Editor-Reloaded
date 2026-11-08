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
  - Game code (SAM-site loader and SAM coverage code)
  - Map table data and XML comments
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

`SamSites.xml` defines SAM installations and the matrix of sectors they cover. Each SAM has a sector, hidden flag, terminal orientation and two terminal grid positions. The coverage matrix holds one row per map row; each cell is a bitmask of the SAMs covering that sector, not a single SAM index.

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

- **Type:** `Integer` (`UINT32`)
- **Description:** Unique 1-based SAM-site index used as the array key (stored internally at `samIndex - 1`). `NUMBER_OF_SAMS` is set to the highest index read; indices `1..4` also have hardcoded Meanwhile/Skyrider roles.
- **Values:** `1..50` (`MAX_NUMBER_OF_SAMS`); `0` is reserved for 'no SAM coverage' and is rejected as a site index.
- **Default:** Required; identifies the record.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samSector/x`

- **Type:** `Integer` (`INT16`)
- **Description:** Map column (`x`) of the SAM sector: `1` = west edge, `16` = east edge. Used with `y` to build the sector ID.
- **Values:** `1..16`.
- **Default:** Absent; `0` (invalid sector).
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samSector/y`

- **Type:** `Integer` (`INT16`)
- **Description:** Map row (`y`) of the SAM sector: `1` = north edge (`A`), `16` = south edge (`P`).
- **Values:** `1..16`.
- **Default:** Absent; `0` (invalid sector).
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samHidden`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If nonzero, the SAM site is hidden at the start of a new game (`fSamSiteFoundOrig` is set to `!samHidden`). Changing it in an existing save has no effect; the table comment requires a new game.
- **Values:** `0` or `1`; any nonzero value hides the site.
- **Default:** Absent; `0` (visible).
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samOrientation`

- **Type:** `Integer` (`INT8`)
- **Description:** Orientation of the SAM control terminal if it is repaired: `3` for `/` (from the middle to the lower left) and `4` for `\` (from the middle to the lower right). Any other value is forced to `3`.
- **Values:** `3` or `4`; other values are read as `3`.
- **Default:** Absent; `3`.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samGridNoA`

- **Type:** `Integer` (`INT32`)
- **Description:** Grid number of the lower terminal tile on the tactical map (the A tile in the table comment's A/B diagram).
- **Values:** Tactical grid number below `MAX_ALLOWED_WORLD_MAX`; values at or above it are replaced with `0`.
- **Default:** Absent; `0`.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samGridNoB`

- **Type:** `Integer` (`INT32`)
- **Description:** Grid number of the upper terminal tile (the B tile); `samGridNoA` is always lower on screen than `samGridNoB`.
- **Values:** Tactical grid number below `MAX_ALLOWED_WORLD_MAX`; values at or above it are replaced with `1`.
- **Default:** Absent; `1`.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAM_CONTROLLED_SECTORS/MAP_ROW`

- **Type:** `String` (`CHAR8`)
- **Description:** One row of the SAM coverage matrix. The `row` attribute selects the map row (`00`..`17`); the element text is a whitespace-separated list of 18 column cells. Each cell is a bitmask: bit 0 = SAM 1, bit 1 = SAM 2, bit 2 = SAM 3, and so on, so `5` means SAM 1 and SAM 3; `0` = no coverage.
- **Values:** 18 cells, one per column `0..17`; rows and columns `0` and `17` are world borders and are not read by the coverage checks.
- **Default:** Required; rows with a missing `row` attribute or `row >= 18` are skipped. A non-numeric value is parsed as `0`.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

## Relations

- `samSector/x` and `samSector/y` identify a sector named in [SectorNames](SectorNames.md); the coverage matrix is indexed by map row and column, and each cell is a bitmask of SAM indexes.

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Game code: SAM-site loader, coverage matrix and sector helpers.
- Map table data and XML comments.
- Field types follow the loader/local structure and runtime arrays; descriptions follow the code that consumes each value.

## Open questions

- The coverage matrix is a bitmask (`1 << (samIndex - 1)`), not a list of SAM indexes; shipped values include combinations such as `5`, `6`, `9`, `12` and `13`.
- `samIndex` 1..4 have hardcoded roles in the Meanwhile/Skyrider dialogue and retreat logic; changing them can break those scenes.
- The loader stores matrix rows/columns `0` and `17` even though the coverage checks only read `1..16`; whether a non-zero border value has any effect is unknown.
- The matrix parser has no column-count check; a row with more than 18 cells would write past the row array.
