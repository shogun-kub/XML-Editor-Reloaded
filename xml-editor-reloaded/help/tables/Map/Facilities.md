---
schema: 1
lang: en
family: map-facilities
title: "Map.Facilities - facility placement"
tables:
  - Map.Facilities
files:
  - Map/Facilities.xml
root: SECTORFACILITIES
record: FACILITY
key: SectorGrid + FacilityType
sources:
  - Strategic/XML_Facilities.cpp
  - Strategic/XML_FacilityTypes.cpp
status: draft
updated: 2026-11-07
---

# Map.Facilities - facility placement

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
type itself (name, assignment, effects) is defined in `Map.FacilityTypes`; this table only
binds a type to a sector.

The v5 file has 71 records. There are no duplicate sector/type pairs in the shipped data.

## Tables

| Table | File | Records | Notes |
|---|---|---:|---|
| `Map.Facilities` | `Map/Facilities.xml` | 71 | no metadata key; the logical key is sector + type |

`Map.Facilities` has no row in the editor metadata (it is one of the 66 tables without a
metadata entry), so the editor shows it as a document-driven table.

## Structure

```
SECTORFACILITIES
└── FACILITY          one facility placement
    ├── SectorGrid    sector such as B13
    ├── FacilityType  index into Map.FacilityTypes
    └── ubHidden      0 always, 1 after exploration, 2 never
```

The loader stores the record in `gFacilityLocations[sector][facilityType]`. The logical key is
the pair `SectorGrid + FacilityType`; the file itself has no key element. A duplicate pair
would overwrite the previous flags (last record wins); the shipped data has no duplicates.

## Fields

### `SECTORFACILITIES/FACILITY/SectorGrid`

- **Type:** `String` (sector code)
- **Description:** Sector where the facility is placed, for example `B13`. The loader takes
  the first character as the map row (`A`..`P`) and the remaining text as the column
  (`1`..`16`).
- **Values:** `A1`..`P16`; the loader accepts only rows/columns 1..16 and ignores an invalid
  sector.
- **Default:** none; an invalid value leaves the record unplaced.
- **Source:** `Strategic/XML_Facilities.cpp:115-128`
- **Confidence:** `source`

### `SECTORFACILITIES/FACILITY/FacilityType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the facility type in `Map.FacilityTypes.ubIndex`. It selects the
  facility's name, assignment, staff limits and effects.
- **Values:** valid `Map.FacilityTypes.ubIndex`; shipped data uses 1..25 (17 is not used).
  `0` means "no facility" and the loader skips the record.
- **Default:** `0` (skipped).
- **Source:** `Strategic/XML_Facilities.cpp:134`, `:113-114`
- **Confidence:** `source`

### `SECTORFACILITIES/FACILITY/ubHidden`

- **Type:** `Integer` (`UINT8`, declared as `BOOLEAN` in the parser struct)
- **Description:** When the facility appears in the sector info popup: always, after the
  sector has been explored, or never. The value `2` is stored even though the field is typed
  `BOOLEAN`, so do not treat it as a plain 0/1 flag.
- **Values:** `0` always show, `1` after exploration, `2` never.
- **Default:** `0` (always show).
- **Source:** `Strategic/XML_Facilities.cpp:140`, `:30-31`
- **Confidence:** `source`

## Relations

- `Map.Facilities.FacilityType` -> `Map.FacilityTypes.ubIndex` (future `Map/FacilityTypes.md`).
- The sector code determines which town/region the facility belongs to; town data is in
  [Map.Cities](Cities.md).
- Facility names, tooltips and assignment text live in `Map.FacilityTypes` and its
  `Russian.*` overlay, not in this table.

## Localization

No strings in this table. The visible facility name and assignment text come from
`Map.FacilityTypes`; see `Localization.md` for the overlay rules.

## Versions

v4 and v5 use the same three fields and the same `SECTORFACILITIES/FACILITY` structure. The
placed facilities may differ; compare the files when a specific sector matters.

## Sources

- `Strategic/XML_Facilities.cpp` - parser, sector decode, `gFacilityLocations` write.
- `Map/Facilities.xml` - shipped placements; the header comment documents the three fields.
- `Strategic/XML_FacilityTypes.cpp` - the type table referenced by `FacilityType`.

## Open questions

- There is no metadata key row for `Map.Facilities`; the composite key is a logical contract
  used by the editor, not a declared `TableKeys` entry. Adding a key would be a metadata task,
  not a data edit.
- The loader uses `BOOLEAN` for `ubHidden`; the value 2 survives because `BOOLEAN` is a byte
  typedef in this codebase, but the type name is misleading.
