---
schema: 1
lang: en
family: map-facilities
title: "Facilities - facility placement"
tables:
  - Facilities
table_ids:
  - Map.Facilities
files:
  - Map/Facilities.xml
root: SECTORFACILITIES
record: FACILITY
key: SectorGrid + FacilityType
sources:
  - Game code (facility placement loader)
  - Facility type data
status: reviewed
updated: 2026-11-07
---

# Facilities - facility placement

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SECTORFACILITIES/FACILITY/SectorGrid`](#sectorfacilitiesfacilitysectorgrid)
  - [`SECTORFACILITIES/FACILITY/FacilityType`](#sectorfacilitiesfacilityfacilitytype)
  - [`SECTORFACILITIES/FACILITY/ubHidden`](#sectorfacilitiesfacilityubhidden)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Facilities.xml` places facility instances on the strategic map. Each record says which
facility type is present in a sector and how it is revealed in the sector popup. The facility
type itself (name, assignment, effects) is defined in `FacilityTypes`; this table only
binds a type to a sector.

## Tables

| Table | File | Notes |
|---|---|---|
| `Facilities` | `Map/Facilities.xml` | no metadata key; the logical key is sector + type |

`Facilities` has no row in the editor metadata, so the editor shows it as a
document-driven table.

## Structure

```
SECTORFACILITIES
└── FACILITY
    ├── SectorGrid
    ├── FacilityType
    └── ubHidden
```

The loader stores the record in `gFacilityLocations[sector][facilityType]`. The logical key is
the pair `SectorGrid + FacilityType`; the file itself has no key element. A duplicate pair
would overwrite the previous flags (last record wins).

## Fields

### `SECTORFACILITIES/FACILITY/SectorGrid`

- **Type:** `String` (sector code)
- **Description:** Sector where the facility is placed, for example `B13`. The loader takes
  the first character as the map row (`A`..`P`) and the remaining text as the column
  (`1`..`16`).
- **Values:** `A1`..`P16`; the loader accepts only rows/columns 1..16 and ignores an invalid
  sector.
- **Default:** none; an invalid value leaves the record unplaced.
- **Source:** game code (facility placement loader); facility type data.
- **Confidence:** `source`

### `SECTORFACILITIES/FACILITY/FacilityType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the facility type in `FacilityTypes.ubIndex`. It selects the
  facility's name, assignment, staff limits and effects.
- **Values:** `0` means "no facility" and the loader skips the record; non-zero values are
  valid `FacilityTypes.ubIndex` IDs.
- **Default:** `0` (skipped).
- **Source:** game code (facility placement loader); facility type data.
- **Confidence:** `source`

### `SECTORFACILITIES/FACILITY/ubHidden`

- **Type:** `Integer` (`0..2`)
- **Description:** When the facility appears in the sector info popup: always, after the
  sector has been explored, or never. `2` is a valid value, so do not treat this as a plain
  0/1 flag.
- **Values:** `0` always show, `1` after exploration, `2` never.
- **Default:** `0` (always show).
- **Source:** game code (facility placement loader); facility type data.
- **Confidence:** `source`

## Relations

- `Facilities.FacilityType` -> [FacilityTypes](FacilityTypes.md) `ubIndex`.
- The sector code determines which town/region the facility belongs to; town data is in
  [Cities](Cities.md).
- Facility names, tooltips and assignment text live in [FacilityTypes](FacilityTypes.md) and its
  `Russian.*` overlay, not in this table.

## Localization

No strings in this table. The visible facility name and assignment text come from
`FacilityTypes`; see `Localization.md` for the overlay rules.

## Versions

v4 and v5 use the same three fields and the same `SECTORFACILITIES/FACILITY` structure. The
placed facilities may differ; compare the files when a specific sector matters.

## Sources

- Game code: the facility placement loader and the facility type loader.
- Table data: `Map/Facilities.xml`; its header comment documents the three fields.

## Open questions

- There is no metadata key row for `Facilities`; the composite key is a logical contract
  used by the editor, not a declared `TableKeys` entry. Adding a key would be a metadata task,
  not a data edit.
- The parser stores `ubHidden` in a byte-like flag; the value 2 survives, but the internal
  type name is misleading.
