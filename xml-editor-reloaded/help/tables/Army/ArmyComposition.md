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
status: draft
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

- **Type:** `Integer`
- **Description:** Index of the army composition. It is an array key used by garrisons and patrols; it is not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `COMPOSITION_INFO/COMPOSITION/Priority`

- **Type:** `Integer`
- **Description:** Priority of this composition relative to the others. Higher values make the group more important when the army fills sectors; `0` is the lowest priority. Numeric semantics: higher = more important; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `COMPOSITION_INFO/COMPOSITION/ElitePercentage`

- **Type:** `Integer`
- **Description:** Share of elite soldiers in this composition, in percent. Higher values mean more elites; `0` excludes them. Numeric semantics: higher = larger share; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `COMPOSITION_INFO/COMPOSITION/TroopPercentage`

- **Type:** `Integer`
- **Description:** Share of regular troop soldiers in this composition, in percent. Higher values mean more regulars; `0` excludes them. Numeric semantics: higher = larger share; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `COMPOSITION_INFO/COMPOSITION/AdminPercentage`

- **Type:** `Integer`
- **Description:** Share or weight of administration soldiers in this composition, in percent. Higher values add more admin personnel; it is a separate share from the elite/troop split. Numeric semantics: higher = larger admin share; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `COMPOSITION_INFO/COMPOSITION/StartPopulation`

- **Type:** `Integer`
- **Description:** Number of soldiers in the composition at campaign start. Higher values start with a larger group; `0` starts empty. Numeric semantics: higher = more soldiers; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `COMPOSITION_INFO/COMPOSITION/DesiredPopulation`

- **Type:** `Integer`
- **Description:** Target number of soldiers the composition tries to maintain. Higher values aim for a larger group; `0` means no target. Numeric semantics: higher = larger target; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- Garrisons and patrols reference a composition by its `Index`.

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
