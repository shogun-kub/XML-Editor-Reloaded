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
  - Game code (sector-names loader, weather, disease, workers, radio scan and prison code)
  - Map table data and XML comments
status: reviewed
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

`SectorNames.xml` gives each map sector its explored and unexplored names, weather and snake chances, water and dirt values, civilian population, wealth, curfew, prison room and raid switches. The table is keyed by `SectorGrid`.

All four `sz*Name` fields must be non-empty for a sector's custom names to be used; if any one is empty, the game ignores the custom set and generates default names for all four. `rainchance`, `sandstormchance` and `snowchance` are per-sector weights in localized weather, not standalone daily probabilities.

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
- **Description:** Sector code in `A1`..`P16` form: the letter is the map row (`y`) and the number is the column (`x`). It is the table key.
- **Values:** `A1`..`P16`; both coordinates must be `1..16`, otherwise the parser keeps its previous coordinates.
- **Default:** Required; identifies the record.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/szUnexploredName`

- **Type:** `String`
- **Description:** Short unexplored-sector name; stored in slot 0 and used in the non-detailed strategic-map sector name.
- **Values:** Free text (UTF-8, converted to `CHAR16`).
- **Default:** Absent; empty string. All four name fields must be non-empty or the whole custom set is ignored.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/szDetailedUnexploredName`

- **Type:** `String`
- **Description:** Detailed unexplored-sector name; stored in slot 1 and used in the detailed strategic-map sector name.
- **Values:** Free text (UTF-8, converted to `CHAR16`).
- **Default:** Absent; empty string. All four name fields must be non-empty or the whole custom set is ignored.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/szExploredName`

- **Type:** `String`
- **Description:** Short explored-sector name; stored in slot 2 and used in the non-detailed name after the sector is visited or a SAM site there is known.
- **Values:** Free text (UTF-8, converted to `CHAR16`).
- **Default:** Absent; empty string. All four name fields must be non-empty or the whole custom set is ignored.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/szDetailedExploredName`

- **Type:** `String`
- **Description:** Detailed explored-sector name; stored in slot 3 and used in the detailed name after the sector is visited or a SAM site there is known.
- **Values:** Free text (UTF-8, converted to `CHAR16`).
- **Default:** Absent; empty string. All four name fields must be non-empty or the whole custom set is ignored.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/sWaterType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Water category: `0` = no water, `1` = drinkable water, `2` = salt water, `3` = water that may pose a health risk (poisoned or swamp water).
- **Values:** `0..3`.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/usNaturalDirt`

- **Type:** `Integer` (`UINT16`)
- **Description:** Dirt-accumulation percentage for guns fired in a surface sector: `100` is a clean city, and higher values accumulate dirt proportionally faster (`1000` = desert, ten times faster). The firing code multiplies the item's dirt factor by this value and divides by `5000`, with a minimum factor of `1`; being indoors halves the value, and underground sectors use a fixed `100`.
- **Numeric semantics:** higher = dirtier.
- **Values:** Non-negative percentage; shipped values range `100..1000`.
- **Default:** Absent; `100` (loader reset value).
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/usCivilianPopulation`

- **Type:** `Integer` (`UINT16`)
- **Description:** Estimated civilian population of the sector, including civilians not physically present on the tactical map. Used by disease and volunteer calculations.
- **Values:** Non-negative count.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/rainchance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Per-sector weather weight used when localizing rain: it biases the starting sector and is the chance that an adjacent sector also gets the weather. It is not a standalone daily rain probability.
- **Values:** `0..100`; the loader stores `UINT8` and applies `min(100, value)` after the cast.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/sandstormchance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Per-sector weather weight used when localizing sandstorms: it biases the starting sector and is the chance that an adjacent sector also gets the weather. It is not a standalone daily sandstorm probability.
- **Values:** `0..100`; the loader stores `UINT8` and applies `min(100, value)` after the cast.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/snowchance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Per-sector weather weight used when localizing snow: it biases the starting sector and is the chance that an adjacent sector also gets the weather. It is not a standalone daily snow probability.
- **Values:** `0..100`; the loader stores `UINT8` and applies `min(100, value)` after the cast.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/snakechance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Per-turn chance, in percent, that snakes can attack people in water in this sector; evaluated only on the surface (`z = 0`) when snakes are enabled.
- **Values:** `0..100`; the loader stores `UINT8` and caps at `100` after the cast.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/numsnakes`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of snakes for a snake attack. A positive attack creates `1 + Random(numsnakes)` snakes, so the actual count is `1..numsnakes`.
- **Values:** `0..20`; the loader caps at `20` even though the table comment says `0..100`.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/bloodcatraidpossible`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If nonzero, bloodcats can raid this sector; the loader sets `SECTORFLAG_RAIDPOSSIBLE_BLOODCAT` in the level-0 sector flag mask.
- **Values:** `0` or `1`; any nonzero value sets the flag.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/zombieraidpossible`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If nonzero, zombies can raid this sector; the loader sets `SECTORFLAG_RAIDPOSSIBLE_ZOMBIE` in the level-0 sector flag mask.
- **Values:** `0` or `1`; any nonzero value sets the flag.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/banditraidpossible`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If nonzero, bandits can raid this sector; the loader sets `SECTORFLAG_RAIDPOSSIBLE_BANDIT` in the level-0 sector flag mask.
- **Values:** `0` or `1`; any nonzero value sets the flag.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/maxworkers`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of workers that can be employed in this sector. Used as the town-worker cap and to seed the initial workforce (`maxworkers * dInitialWorkerRate`).
- **Values:** `0..255` (`UINT8`).
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/wealth`

- **Type:** `Integer` (`UINT16`)
- **Description:** Rating of how rich the sector's population is; also used as the money scale for scrounging (`wealth + Random(4 * wealth)`), so `0` disables that behaviour.
- **Values:** `0..65535` (`UINT16`).
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/usCurfewValue`

- **Type:** `Integer` (`UINT8`)
- **Description:** Curfew category used for covert-civilian detection: `0` = no curfew, `1` = night curfew, `2` = civilians never allowed. The tactical AI raises the effective value to at least `1` at night and to at least `2` underground.
- **Values:** `0`, `1` or `2`.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/sRadioScanModifier`

- **Type:** `Integer` (`INT8`)
- **Description:** Additive modifier to the radio-scan assignment range in this sector; the base range is the `RADIO_OPERATOR_ASSIGNMENT_SCAN_BASE_RANGE` setting in `Ja2_Options.ini` (default `5`) and the result must be at least `1` for the scan to run.
- **Numeric semantics:** positive = longer range, negative = shorter range. The loader clamps values above `3` to `3`; the intended `-3` lower clamp is overwritten in the current source, so lower negative values are kept.
- **Values:** `INT8`; effective upper bound `3`.
- **Default:** Absent; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/usPrisonRoomNumber`

- **Type:** `Integer` (`UINT16`)
- **Description:** Room number of a prison in this sector. The element may be repeated; every nonzero value is appended to the sector's prison-room list and prisoners are placed round-robin in those rooms. `0` adds nothing.
- **Values:** `1..65535`; `0` = no room.
- **Default:** Absent; no prison rooms.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

## Relations

- `SectorNames.SectorGrid` is the canonical sector key used by [SectorLoadscreens](SectorLoadscreens.md), [MovementCosts](MovementCosts.md), [BloodcatPlacements](BloodcatPlacements.md) and [CreaturePlacements](CreaturePlacements.md).
- The row's `SectorGrid` is parsed as a letter (row `y`) followed by a number (column `x`); the resulting sector ID keys the name and external-data arrays.

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Game code: sector-names loader; weather, dirt, disease, worker, radio-scan, prison-room and curfew systems.
- Map table data and XML comments.
- Field types follow the runtime sector-data structure and loader casts; descriptions follow the code that consumes each value.

## Open questions

- The loader computes `max(-3, value)` and then immediately overwrites it with `min(3, value)`, so the intended `-3` lower clamp is not applied; whether this is intended is unknown.
- An invalid or missing `SectorGrid` is not rejected: the parser reuses the previous record's coordinates (or `0,0` for the first record), and the record can overwrite another sector. Whether the loader should reject it is unknown.
- `rainchance`, `sandstormchance` and `snowchance` feed `DetermineLocalizedWeatherGivenHelperArray`, which uses them both to pick a starting sector and to spread weather to adjacent sectors; the full localization formula is not described here.
- Prison rooms and raid flags are written to the level-0 (`[0]`) runtime slot only; the level-1..3 slots are left at zero, and whether the level files are meant to define them is unknown.
- The loader resets `usNaturalDirt` to `100` after each record, so an omitted value in the first record starts from `0` while later records start from `100`; whether that asymmetry is intended is unknown.
