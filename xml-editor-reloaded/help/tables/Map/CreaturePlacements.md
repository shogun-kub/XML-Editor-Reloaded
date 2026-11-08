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
  - Game code (creature-placements loader and creature spreading)
  - Map table data and XML comments
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

Within each composition the six `*Percent` fields are consecutive bands of a `Random(100)` roll, processed in the order Larvae, Infant, YoungMale, YoungFemale, AdultMale, AdultFemale; they must add up to exactly `100`. Placement `Index` values are 1-based, composition `CompIndex` values are 0-based, and a habitat whose `Composition` is `0` is ignored.

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

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the creature placement record. It is a 1-based array key; the highest valid index also sets the number of infectible sites.
- **Values:** `1..10` (`MAX_NUMBER_OF_INFECTIBLE_SITES`); `0` is invalid and the record is discarded. The loader's upper guard is `> 11`, so `11` is accepted but lies outside the 10-site array.
- **Default:** Required; identifies the record.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/SectorGrid`

- **Type:** `String`
- **Description:** Sector where the creatures surface to attack the town, in `A1`..`P16` form: the letter is the map row (`y`) and the number is the column (`x`). The table comment also requires the associated underground sector to be a habitat or queen sector.
- **Values:** `A1`..`P16`; both coordinates must be `1..16`, otherwise the stored sector is not updated.
- **Default:** Required in practice; invalid input is ignored.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/GridNo`

- **Type:** `Integer` (`INT32`)
- **Description:** Grid number inside the attack sector used to insert creatures when the attack starts in the attack-source sector itself.
- **Values:** Tactical grid number; `0` = unset.
- **Default:** Absent; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorGrid`

- **Type:** `String`
- **Description:** Underground sector whose alternate map is loaded and flagged (`SF_PENDING_ALTERNATE_MAP`) when the creature quest starts.
- **Values:** `A1`..`P16`; both coordinates must be `1..16`, otherwise the stored sector is not updated.
- **Default:** Required in practice; invalid input is ignored.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Underground level of the alternate map: `0` is the surface, `1`..`3` are underground.
- **Values:** `0..3` (`MAXIMUM_VALID_Z_COORDINATE`).
- **Default:** Absent; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Destination sector used when the player leaves the queen or a habitat sector at `z >= 2` after the creature quest is complete; `InitSectorExitMenu` checks both conditions.
- **Values:** `A1`..`P16`; both coordinates must be `1..16`, otherwise the stored sector is not updated.
- **Default:** Required in practice; invalid input is ignored.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Underground level of the warp destination: `0` is the surface, `1`..`3` are underground.
- **Values:** `0..3` (`MAXIMUM_VALID_Z_COORDINATE`).
- **Default:** Absent; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/GridNo`

- **Type:** `Integer` (`INT32`)
- **Description:** Insertion grid number at the warp destination.
- **Values:** Tactical grid number; `0` = unset.
- **Default:** Absent; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Sector containing the queen's lair, the root of the creature directive list.
- **Values:** `A1`..`P16`; both coordinates must be `1..16`, otherwise the stored sector is not updated.
- **Default:** Required in practice; invalid input is ignored.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Underground level of the queen's lair: `0` is the surface, `1`..`3` are underground.
- **Values:** `0..3` (`MAXIMUM_VALID_Z_COORDINATE`).
- **Default:** Absent; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorGrid`

- **Type:** `String`
- **Description:** One linked habitat sector where creatures can spread. A habitat is used only when its `Composition` is nonzero; up to 255 habitats are read per placement.
- **Values:** `A1`..`P16`; both coordinates must be `1..16`, otherwise the stored sector is not updated.
- **Default:** Required in practice; invalid input is ignored.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Underground level of the habitat: `0` is the surface, `1`..`3` are underground.
- **Values:** `0..3` (`MAXIMUM_VALID_Z_COORDINATE`).
- **Default:** Absent; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/Composition`

- **Type:** `Integer` (`UINT8`)
- **Description:** Composition index used by this habitat. `0` is reserved for the queen lair and leaves the habitat invalid; a nonzero value marks the habitat valid and references `CompIndex`.
- **Values:** `1..49`; `0` = ignored habitat.
- **Default:** Absent or `0`; the habitat is ignored.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/CompIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Zero-based index of the creature composition; `0` is the queen-lair composition and `HABITATSECTOR/Composition` references this value. The highest index plus one sets the number of compositions.
- **Values:** `0..49` (`MAX_NUMBER_OF_CREATURE_COMPOSITIONS`); the loader's guard also accepts `50`, one past the array end.
- **Default:** Required; `0` = queen lair.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultFemalePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Share of adult females in this composition, in percent. The six shares form consecutive bands of a `Random(100)` roll (see Purpose).
- **Values:** `0..100`; the six fields must total `100`.
- **Default:** Absent; `0` unless the previous composition set the same tag.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultMalePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Share of adult males in this composition, in percent. The six shares form consecutive bands of a `Random(100)` roll (see Purpose).
- **Values:** `0..100`; the six fields must total `100`.
- **Default:** Absent; `0` unless the previous composition set the same tag.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungFemalePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Share of young females in this composition, in percent. The six shares form consecutive bands of a `Random(100)` roll (see Purpose).
- **Values:** `0..100`; the six fields must total `100`.
- **Default:** Absent; `0` unless the previous composition set the same tag.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungMalePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Share of young males in this composition, in percent. The six shares form consecutive bands of a `Random(100)` roll (see Purpose).
- **Values:** `0..100`; the six fields must total `100`.
- **Default:** Absent; `0` unless the previous composition set the same tag.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/InfantPercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Share of infants in this composition, in percent. The six shares form consecutive bands of a `Random(100)` roll (see Purpose).
- **Values:** `0..100`; the six fields must total `100`.
- **Default:** Absent; `0` unless the previous composition set the same tag.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/LarvaePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Share of larvae in this composition, in percent. The six shares form consecutive bands of a `Random(100)` roll (see Purpose).
- **Values:** `0..100`; the six fields must total `100`.
- **Default:** Absent; `0` unless the previous composition set the same tag.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/MaxPopulation`

- **Type:** `Integer` (`INT32`)
- **Description:** Base population cap for this composition before difficulty and distance scaling. The spreading code applies Easy 50%, Medium 80%, Hard 100% and Insane 200%, scales by distance from the lair (minimum 25%) and floors the result at `4`.
- **Numeric semantics:** higher = larger colony; the computed cap is floored at `4`.
- **Values:** Non-negative population; shipped compositions use `10..32`.
- **Default:** Absent; `0` unless the previous composition set it.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

## Relations

- `HABITATSECTOR/Composition` references `COMPOSITION/CompIndex` only when it is nonzero; `Composition` `0` leaves the habitat invalid.
- All sector fields use the normal map sector coordinates (`A1`..`P16`) and sector names from [SectorNames](SectorNames.md).

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Game code: creature-placements loader, creature-spreading structures and consumption.
- Map table data and XML comments.
- Field types follow the runtime structures and loader casts; descriptions follow the code that consumes each value.

## Open questions

- `GetWarpOutOfMineCodes` itself only rejects `z == 0`; the `z >= 2` and quest-complete checks live in its callers, so a future caller could warp at level 1.
- The loader's `Index` guard accepts `11` and its `CompIndex` guard accepts `50`, one past the end of the corresponding arrays; whether this is intentional is unknown.
- Omitted placement scalars and composition percentages inherit the previous record's value because the parse scratch structs are not cleared per record; whether the game relies on this is unknown.
- Whether `MaxPopulation` is also bounded by `MAX_NUMBER_CREATURES_IN_TACTICAL` in every spreading path is not fully traced here.
