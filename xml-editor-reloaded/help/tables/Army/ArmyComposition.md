---
schema: 1
lang: en
family: army-composition
title: "ArmyComposition - army group composition"
tables:
  - ArmyComposition
table_ids:
  - Army.ArmyComposition
files:
  - Army/ArmyComposition.xml
root: COMPOSITION_INFO
record: COMPOSITION
key: Index
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# ArmyComposition - army group composition

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`COMPOSITION_INFO/COMPOSITION/Index`](#composition_infocompositionindex)
  - [`COMPOSITION_INFO/COMPOSITION/Priority`](#composition_infocompositionpriority)
  - [`COMPOSITION_INFO/COMPOSITION/ElitePercentage`](#composition_infocompositionelitepercentage)
  - [`COMPOSITION_INFO/COMPOSITION/TroopPercentage`](#composition_infocompositiontrooppercentage)
  - [`COMPOSITION_INFO/COMPOSITION/AdminPercentage`](#composition_infocompositionadminpercentage)
  - [`COMPOSITION_INFO/COMPOSITION/StartPopulation`](#composition_infocompositionstartpopulation)
  - [`COMPOSITION_INFO/COMPOSITION/DesiredPopulation`](#composition_infocompositiondesiredpopulation)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`ArmyComposition.xml` defines the mix of elite, regular and admin soldiers in an army group, its priority, and its starting and target population.

## Tables

| Table | File | Notes |
|---|---|---|
| `ArmyComposition` | `Army/ArmyComposition.xml` | — |

## Structure

```
COMPOSITION_INFO
└── COMPOSITION
    ├── Index
    ├── Priority
    ├── ElitePercentage
    ├── TroopPercentage
    ├── AdminPercentage
    ├── StartPopulation
    └── DesiredPopulation
```

## Fields

### `COMPOSITION_INFO/COMPOSITION/Index`

- **Type:** `Integer` (`INT32`)
- **Description:** Index of the army composition. It is an array key used by garrisons and patrols.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** game code (army composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/Priority`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Priority of this composition relative to the others.
- **Numeric semantics:** higher = more important; negative values are not meaningful.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/ElitePercentage`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Share of elite soldiers in this composition, in percent.
- **Numeric semantics:** higher = larger share; the loader clamps values below `0` to `0` and above `100` to `100`.
- **Values:** Percentage `0..100` after clamping.
- **Default:** Absent; the game default applies.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/TroopPercentage`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Share of regular troop soldiers in this composition, in percent.
- **Numeric semantics:** higher = larger share; the loader clamps values below `0` to `0` and above `100` to `100`.
- **Values:** Percentage `0..100` after clamping.
- **Default:** Absent; the game default applies.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/AdminPercentage`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Share or weight of administration soldiers in this composition, in percent.
- **Numeric semantics:** higher = larger admin share; the loader clamps values below `0` to `0` and above `100` to `100`.
- **Values:** Percentage `0..100` after clamping.
- **Default:** Absent; the game default applies.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/StartPopulation`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Number of soldiers in the composition at campaign start.
- **Numeric semantics:** higher = more soldiers; the loader clamps negative values to `1`.
- **Values:** `1..127` after clamping; the loader stores this field as `INT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/DesiredPopulation`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Target number of soldiers the composition tries to maintain.
- **Numeric semantics:** higher = larger target; the loader clamps negative values to `1`.
- **Values:** `1..127` after clamping; the loader stores this field as `INT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

## Relations

- [GarrisonGroups](GarrisonGroups.md) and [PatrolGroups](PatrolGroups.md) reference a composition by its `Index`; the resulting soldiers use [SoldierProfiles](../Profiles/SoldierProfiles.md).

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for inferred fields is not stated by the table data.
