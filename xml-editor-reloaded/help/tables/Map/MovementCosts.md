---
schema: 1
lang: en
family: map-movement-costs
title: "MovementCosts - strategic terrain and travel cost"
tables:
  - MovementCosts
table_ids:
  - Map.MovementCosts
files:
  - Map/MovementCosts.xml
root: StrategicMovementCosts
record: SectorTraversal/Sector
key: none
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# MovementCosts - strategic terrain and travel cost

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`StrategicMovementCosts/SectorTraversal/Sector/TravelRating`](#strategicmovementcostssectortraversalsectortravelrating)
  - [`StrategicMovementCosts/SectorTraversal/Sector/North`](#strategicmovementcostssectortraversalsectornorth)
  - [`StrategicMovementCosts/SectorTraversal/Sector/East`](#strategicmovementcostssectortraversalsectoreast)
  - [`StrategicMovementCosts/SectorTraversal/Sector/South`](#strategicmovementcostssectortraversalsectorsouth)
  - [`StrategicMovementCosts/SectorTraversal/Sector/West`](#strategicmovementcostssectortraversalsectorwest)
  - [`StrategicMovementCosts/SectorTraversal/Sector/Here`](#strategicmovementcostssectortraversalsectorhere)
  - [`StrategicMovementCosts/SectorTraversal/Sector/BadSector`](#strategicmovementcostssectortraversalsectorbadsector)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MovementCosts.xml` gives each sector its strategic `TravelRating` and the terrain category on each edge and in the sector itself. The record is identified by the `Sector` element’s `x` and `y` attributes, not by a child key.

## Tables

| Table | File | Notes |
|---|---|---|
| `MovementCosts` | `Map/MovementCosts.xml` | — |

## Structure

```
StrategicMovementCosts
└── SectorTraversal
    └── Sector
        ├── TravelRating
        ├── North
        ├── East
        ├── South
        ├── West
        ├── Here
        └── BadSector
```

## Fields

### `StrategicMovementCosts/SectorTraversal/Sector/TravelRating`

- **Type:** `Integer` (`UINT8`)
- **Description:** Strategic travel cost of entering this sector.
- **Numeric semantics:** higher = slower; negative input is accepted by the parser but stored as `UINT8`, and values above 255 truncate.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/North`

- **Type:** `String`
- **Description:** Terrain category on the northern edge of the sector, such as `PLAINS`, `ROAD`, `TOWN` or `EDGEOFWORLD`. It is a category code.
- **Values:** One of `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; unknown text triggers an assertion and leaves the parsed type undefined.
- **Default:** Absent; the game default applies.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/East`

- **Type:** `String`
- **Description:** Terrain category on the eastern edge of the sector. It is a category code.
- **Values:** One of `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; unknown text triggers an assertion and leaves the parsed type undefined.
- **Default:** Absent; the game default applies.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/South`

- **Type:** `String`
- **Description:** Terrain category on the southern edge of the sector. It is a category code.
- **Values:** One of `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; unknown text triggers an assertion and leaves the parsed type undefined.
- **Default:** Absent; the game default applies.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/West`

- **Type:** `String`
- **Description:** Terrain category on the western edge of the sector. It is a category code.
- **Values:** One of `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; unknown text triggers an assertion and leaves the parsed type undefined.
- **Default:** Absent; the game default applies.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/Here`

- **Type:** `String`
- **Description:** Terrain category of the sector itself. It is a category code.
- **Values:** One of `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; unknown text triggers an assertion and leaves the parsed type undefined.
- **Default:** Absent; the game default applies.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/BadSector`

- **Type:** `Boolean`
- **Description:** If set, the sector is marked as bad or unreachable for strategic movement. It is a switch, not a cost.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

## Relations

- The `Sector` record is keyed by the `x` and `y` attributes, which identify the same sector coordinates used by [SectorNames](SectorNames.md).

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
