---
schema: 1
lang: en
family: map-sector-names
title: "SectorNames - sector names, weather and population"
tables:
  - SectorNames
table_ids:
  - Map.SectorNames
files:
  - Map/SectorNames.xml
root: SECTOR_NAMES
record: SECTOR
key: SectorGrid
sources:
  - Map table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# SectorNames - sector names, weather and population

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SECTOR_NAMES/SECTOR/SectorGrid`](#sector_namessectorsectorgrid)
  - [`SECTOR_NAMES/SECTOR/szUnexploredName`](#sector_namessectorszunexploredname)
  - [`SECTOR_NAMES/SECTOR/szDetailedUnexploredName`](#sector_namessectorszdetailedunexploredname)
  - [`SECTOR_NAMES/SECTOR/szExploredName`](#sector_namessectorszexploredname)
  - [`SECTOR_NAMES/SECTOR/szDetailedExploredName`](#sector_namessectorszdetailedexploredname)
  - [`SECTOR_NAMES/SECTOR/sWaterType`](#sector_namessectorswatertype)
  - [`SECTOR_NAMES/SECTOR/usNaturalDirt`](#sector_namessectorusnaturaldirt)
  - [`SECTOR_NAMES/SECTOR/usCivilianPopulation`](#sector_namessectoruscivilianpopulation)
  - [`SECTOR_NAMES/SECTOR/rainchance`](#sector_namessectorrainchance)
  - [`SECTOR_NAMES/SECTOR/sandstormchance`](#sector_namessectorsandstormchance)
  - [`SECTOR_NAMES/SECTOR/snowchance`](#sector_namessectorsnowchance)
  - [`SECTOR_NAMES/SECTOR/snakechance`](#sector_namessectorsnakechance)
  - [`SECTOR_NAMES/SECTOR/numsnakes`](#sector_namessectornumsnakes)
  - [`SECTOR_NAMES/SECTOR/bloodcatraidpossible`](#sector_namessectorbloodcatraidpossible)
  - [`SECTOR_NAMES/SECTOR/zombieraidpossible`](#sector_namessectorzombieraidpossible)
  - [`SECTOR_NAMES/SECTOR/banditraidpossible`](#sector_namessectorbanditraidpossible)
  - [`SECTOR_NAMES/SECTOR/maxworkers`](#sector_namessectormaxworkers)
  - [`SECTOR_NAMES/SECTOR/wealth`](#sector_namessectorwealth)
  - [`SECTOR_NAMES/SECTOR/usCurfewValue`](#sector_namessectoruscurfewvalue)
  - [`SECTOR_NAMES/SECTOR/sRadioScanModifier`](#sector_namessectorsradioscanmodifier)
  - [`SECTOR_NAMES/SECTOR/usPrisonRoomNumber`](#sector_namessectorusprisonroomnumber)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SectorNames.xml` gives each map sector its explored and unexplored names, weather and snake chances, water and dirt values, civilian population, wealth, curfew, prison room and raid switches. The table is keyed by `SectorGrid`; absent optional values keep the game default.

## Tables

| Table | File | Notes |
|---|---|---|
| `SectorNames` | `Map/SectorNames.xml` | — |

## Structure

```
SECTOR_NAMES
└── SECTOR
    ├── SectorGrid
    ├── szUnexploredName
    ├── szDetailedUnexploredName
    ├── szExploredName
    ├── szDetailedExploredName
    ├── sWaterType
    ├── usNaturalDirt
    ├── usCivilianPopulation
    ├── rainchance
    ├── sandstormchance
    ├── snowchance
    ├── snakechance
    ├── numsnakes
    ├── bloodcatraidpossible
    ├── zombieraidpossible
    ├── banditraidpossible
    ├── maxworkers
    ├── wealth
    ├── usCurfewValue
    ├── sRadioScanModifier
    └── usPrisonRoomNumber
```

## Fields

### `SECTOR_NAMES/SECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Sector code in `A1`..`P16` form. It identifies the row and column this record describes; it is a map coordinate.
- **Values:** `A1`..`P16`.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/szUnexploredName`

- **Type:** `String`
- **Description:** Short display name shown while the sector is unexplored.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/szDetailedUnexploredName`

- **Type:** `String`
- **Description:** Longer display name used while the sector is unexplored.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/szExploredName`

- **Type:** `String`
- **Description:** Short display name shown after the sector has been explored.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/szDetailedExploredName`

- **Type:** `String`
- **Description:** Longer display name used after the sector has been explored.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/sWaterType`

- **Type:** `Integer`
- **Description:** Water category of the sector. It is a small code: `0` is dry, while `1`..`3` are water categories used by the map code.
- **Values:** Small category code; `0` = dry.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/usNaturalDirt`

- **Type:** `Integer`
- **Description:** Natural dirt level of the sector.
- **Numeric semantics:** higher = dirtier; negative values are not meaningful.
- **Values:** Non-negative dirt level; observed values range from `100` to `1000`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/usCivilianPopulation`

- **Type:** `Integer`
- **Description:** Number of civilians living in the sector.
- **Numeric semantics:** higher = more civilians; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/rainchance`

- **Type:** `Integer`
- **Description:** Chance of rain in the sector, in percent.
- **Numeric semantics:** higher = more rain; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/sandstormchance`

- **Type:** `Integer`
- **Description:** Chance of a sandstorm in the sector, in percent.
- **Numeric semantics:** higher = more sandstorms; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/snowchance`

- **Type:** `Integer`
- **Description:** Chance of snow in the sector, in percent.
- **Numeric semantics:** higher = more snow; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/snakechance`

- **Type:** `Integer`
- **Description:** Chance that a snake encounter occurs in the sector, in percent.
- **Numeric semantics:** higher = more encounters; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/numsnakes`

- **Type:** `Integer`
- **Description:** Number of snakes spawned for a snake encounter.
- **Numeric semantics:** higher = more snakes; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/bloodcatraidpossible`

- **Type:** `Boolean`
- **Description:** If set, bloodcats can raid this sector. It is a switch; the chance is separate.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/zombieraidpossible`

- **Type:** `Boolean`
- **Description:** If set, zombies can raid this sector. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/banditraidpossible`

- **Type:** `Boolean`
- **Description:** If set, bandits can raid this sector. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/maxworkers`

- **Type:** `Integer`
- **Description:** Maximum number of mine workers the sector can support.
- **Numeric semantics:** higher = more workers; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/wealth`

- **Type:** `Integer`
- **Description:** Relative wealth of the sector.
- **Numeric semantics:** higher = richer; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/usCurfewValue`

- **Type:** `Integer`
- **Description:** Curfew level for the sector.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/sRadioScanModifier`

- **Type:** `Integer`
- **Description:** Multiplier applied to radio scanning in the sector.
- **Numeric semantics:** higher = stronger; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SECTOR_NAMES/SECTOR/usPrisonRoomNumber`

- **Type:** `Integer`
- **Description:** Room number used for prisoners in this sector. It is a map room identifier; `0` means no prison room.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `SectorNames.SectorGrid` is the canonical sector key used by `SectorLoadscreens`, `MovementCosts`, `BloodcatPlacements` and `CreaturePlacements`.

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Map table data.
- Field names, XML comments and observed values.
- Descriptions are based on the field names and observed table values; no runtime formula is claimed.

## Open questions

- Which UI surface uses the short vs detailed sector names (strategic-map tooltip, sector panel or another screen) is not confirmed; this needs a code/UI check.
- The exact formula or enum mapping for fields marked as inferred is not stated by the table data.
