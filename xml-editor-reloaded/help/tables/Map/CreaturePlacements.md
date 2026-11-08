---
schema: 1
lang: en
family: map-creature-placements
title: "CreaturePlacements - creature compositions and placements"
tables:
  - CreaturePlacements
table_ids:
  - Map.CreaturePlacements
files:
  - Map/CreaturePlacements.xml
root: CREATURE_INFO
record: PLACEMENT_LIST/PLACEMENT
key: Index
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# CreaturePlacements - creature compositions and placements

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/Index`](#creature_infoplacement_listplacementindex)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/SectorGrid`](#creature_infoplacement_listplacementattacksourcesectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/GridNo`](#creature_infoplacement_listplacementattacksourcegridno)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorGrid`](#creature_infoplacement_listplacementalternatemapsectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorZ`](#creature_infoplacement_listplacementalternatemapsectorz)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorGrid`](#creature_infoplacement_listplacementwarptosectorsectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorZ`](#creature_infoplacement_listplacementwarptosectorsectorz)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/GridNo`](#creature_infoplacement_listplacementwarptosectorgridno)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorGrid`](#creature_infoplacement_listplacementcreaturesectorqueensectorsectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorZ`](#creature_infoplacement_listplacementcreaturesectorqueensectorsectorz)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorGrid`](#creature_infoplacement_listplacementcreaturesectorhabitatsectorsectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorZ`](#creature_infoplacement_listplacementcreaturesectorhabitatsectorsectorz)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/Composition`](#creature_infoplacement_listplacementcreaturesectorhabitatsectorcomposition)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/CompIndex`](#creature_infocomposition_listcompositioncompindex)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultFemalePercent`](#creature_infocomposition_listcompositionadultfemalepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultMalePercent`](#creature_infocomposition_listcompositionadultmalepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungFemalePercent`](#creature_infocomposition_listcompositionyoungfemalepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungMalePercent`](#creature_infocomposition_listcompositionyoungmalepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/InfantPercent`](#creature_infocomposition_listcompositioninfantpercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/LarvaePercent`](#creature_infocomposition_listcompositionlarvaepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/MaxPopulation`](#creature_infocomposition_listcompositionmaxpopulation)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`CreaturePlacements.xml` has two parts. `COMPOSITION_LIST` defines colony compositions and their population caps; `PLACEMENT_LIST` defines an attack source, alternate map, warp destination, queen sector and habitat sectors for each creature infestation. Habitats reference a composition by `CompIndex`.

## Tables

| Table | File | Notes |
|---|---|---|
| `CreaturePlacements` | `Map/CreaturePlacements.xml` | — |

## Structure

```
CREATURE_INFO
├── PLACEMENT_LIST
│   └── PLACEMENT
│       ├── Index
│       ├── ATTACKSOURCE
│       │   ├── SectorGrid
│       │   └── GridNo
│       ├── ALTERNATEMAP
│       │   ├── SectorGrid
│       │   └── SectorZ
│       ├── WARPTOSECTOR
│       │   ├── SectorGrid
│       │   ├── SectorZ
│       │   └── GridNo
│       └── CREATURESECTOR
│           ├── QUEENSECTOR
│           │   ├── SectorGrid
│           │   └── SectorZ
│           └── HABITATSECTOR
│               ├── SectorGrid
│               ├── SectorZ
│               └── Composition
└── COMPOSITION_LIST
    └── COMPOSITION
        ├── CompIndex
        ├── AdultFemalePercent
        ├── AdultMalePercent
        ├── YoungFemalePercent
        ├── YoungMalePercent
        ├── InfantPercent
        ├── LarvaePercent
        └── MaxPopulation
```

## Fields

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/Index`

- **Type:** `Integer`
- **Description:** Index of the creature placement record. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/SectorGrid`

- **Type:** `String`
- **Description:** Sector from which the creatures begin their attack. It is a map coordinate.
- **Values:** Free text.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/GridNo`

- **Type:** `Integer`
- **Description:** Map grid number of the attack origin. It is a grid identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorGrid`

- **Type:** `String`
- **Description:** Sector of the alternate map used by this placement. It is a map coordinate.
- **Values:** Free text.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorZ`

- **Type:** `Integer`
- **Description:** Underground level of the alternate map; `0` is the surface. It is a level.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Sector the creatures warp to. It is a map coordinate.
- **Values:** Free text.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorZ`

- **Type:** `Integer`
- **Description:** Underground level of the warp destination; `0` is the surface. It is a level.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/GridNo`

- **Type:** `Integer`
- **Description:** Map grid number of the warp destination. It is a grid identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Sector containing the queen’s lair. It is a map coordinate.
- **Values:** Free text.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorZ`

- **Type:** `Integer`
- **Description:** Underground level of the queen’s lair; `0` is the surface. It is a level.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Sector of one creature habitat. It is a map coordinate.
- **Values:** Free text.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorZ`

- **Type:** `Integer`
- **Description:** Underground level of the habitat; `0` is the surface. It is a level.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/Composition`

- **Type:** `Integer`
- **Description:** Composition index used by this habitat. It references `CompIndex`; it is an ID.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/CompIndex`

- **Type:** `Integer`
- **Description:** Index of the creature composition. It is an array key and is referenced by `HABITATSECTOR/Composition`.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultFemalePercent`

- **Type:** `Integer`
- **Description:** Share of adult females in this composition, in percent.
- **Numeric semantics:** higher = larger share; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultMalePercent`

- **Type:** `Integer`
- **Description:** Share of adult males in this composition, in percent.
- **Numeric semantics:** higher = larger share; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungFemalePercent`

- **Type:** `Integer`
- **Description:** Share of young females in this composition, in percent.
- **Numeric semantics:** higher = larger share; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungMalePercent`

- **Type:** `Integer`
- **Description:** Share of young males in this composition, in percent.
- **Numeric semantics:** higher = larger share; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/InfantPercent`

- **Type:** `Integer`
- **Description:** Share of infants in this composition, in percent.
- **Numeric semantics:** higher = larger share; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/LarvaePercent`

- **Type:** `Integer`
- **Description:** Share of larvae in this composition, in percent.
- **Numeric semantics:** higher = larger share; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/MaxPopulation`

- **Type:** `Integer`
- **Description:** Maximum population this composition may reach.
- **Numeric semantics:** higher = larger colony; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `HABITATSECTOR/Composition` references `COMPOSITION/CompIndex`; all sector fields use the normal map sector coordinates.

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
