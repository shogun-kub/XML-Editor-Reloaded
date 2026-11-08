---
schema: 1
lang: en
family: map-bloodcat-placements
title: "BloodcatPlacements - bloodcat sector placements"
tables:
  - BloodcatPlacements
table_ids:
  - Map.BloodcatPlacements
files:
  - Map/BloodcatPlacements.XML
root: BLOODCAT_PLACEMENTS
record: SECTOR
key: SectorGrid
sources:
  - Map table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# BloodcatPlacements - bloodcat sector placements

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`BLOODCAT_PLACEMENTS/SECTOR/SectorGrid`](#bloodcat_placementssectorsectorgrid)
  - [`BLOODCAT_PLACEMENTS/SECTOR/ubPlacementType`](#bloodcat_placementssectorubplacementtype)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/fRespawn`](#bloodcat_placementssectornovicefrespawn)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubAmbushChance`](#bloodcat_placementssectornoviceubambushchance)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubFactionAffiliation`](#bloodcat_placementssectornoviceubfactionaffiliation)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMinBloodcats`](#bloodcat_placementssectornoviceubminbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMaxBloodcats`](#bloodcat_placementssectornoviceubmaxbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/fRespawn`](#bloodcat_placementssectorexperiencedfrespawn)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubAmbushChance`](#bloodcat_placementssectorexperiencedubambushchance)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubFactionAffiliation`](#bloodcat_placementssectorexperiencedubfactionaffiliation)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMinBloodcats`](#bloodcat_placementssectorexperiencedubminbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMaxBloodcats`](#bloodcat_placementssectorexperiencedubmaxbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/fRespawn`](#bloodcat_placementssectorexpertfrespawn)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubAmbushChance`](#bloodcat_placementssectorexpertubambushchance)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubFactionAffiliation`](#bloodcat_placementssectorexpertubfactionaffiliation)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMinBloodcats`](#bloodcat_placementssectorexpertubminbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMaxBloodcats`](#bloodcat_placementssectorexpertubmaxbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/fRespawn`](#bloodcat_placementssectorinsanefrespawn)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubAmbushChance`](#bloodcat_placementssectorinsaneubambushchance)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubFactionAffiliation`](#bloodcat_placementssectorinsaneubfactionaffiliation)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMinBloodcats`](#bloodcat_placementssectorinsaneubminbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMaxBloodcats`](#bloodcat_placementssectorinsaneubmaxbloodcats)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`BloodcatPlacements.xml` lists sectors where bloodcats can appear. Each sector has a placement type and, for each difficulty, a respawn switch, ambush chance, faction affiliation and minimum/maximum group size.

## Tables

| Table | File | Notes |
|---|---|---|
| `BloodcatPlacements` | `Map/BloodcatPlacements.XML` | — |

## Structure

```
BLOODCAT_PLACEMENTS
└── SECTOR
    ├── SectorGrid
    ├── ubPlacementType
    ├── NOVICE
    │   ├── fRespawn
    │   ├── ubAmbushChance
    │   ├── ubFactionAffiliation
    │   ├── ubMinBloodcats
    │   └── ubMaxBloodcats
    ├── EXPERIENCED
    │   ├── fRespawn
    │   ├── ubAmbushChance
    │   ├── ubFactionAffiliation
    │   ├── ubMinBloodcats
    │   └── ubMaxBloodcats
    ├── EXPERT
    │   ├── fRespawn
    │   ├── ubAmbushChance
    │   ├── ubFactionAffiliation
    │   ├── ubMinBloodcats
    │   └── ubMaxBloodcats
    └── INSANE
        ├── fRespawn
        ├── ubAmbushChance
        ├── ubFactionAffiliation
        ├── ubMinBloodcats
        └── ubMaxBloodcats
```

## Fields

### `BLOODCAT_PLACEMENTS/SECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Sector code in `A1`..`P16` form. It identifies the sector that can contain bloodcats; it is a map coordinate.
- **Values:** `A1`..`P16`.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/ubPlacementType`

- **Type:** `Integer`
- **Description:** Placement rule code for the sector. `0` is the normal placement; other codes are scenario-specific variants. It is an enum code.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/fRespawn`

- **Type:** `Boolean`
- **Description:** If set, bloodcats in this sector can respawn on the Novice difficulty. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubAmbushChance`

- **Type:** `Integer`
- **Description:** Chance that bloodcats ambush the player in this sector on Novice difficulty, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubFactionAffiliation`

- **Type:** `Integer`
- **Description:** Faction affiliation ID used for the bloodcat group on Novice difficulty. It is a faction reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMinBloodcats`

- **Type:** `Integer`
- **Description:** Minimum number of bloodcats spawned in this sector on Novice difficulty. It must not exceed `ubMaxBloodcats`.
- **Numeric semantics:** higher = more bloodcats; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMaxBloodcats`

- **Type:** `Integer`
- **Description:** Maximum number of bloodcats spawned in this sector on Novice difficulty. It must not be below `ubMinBloodcats`.
- **Numeric semantics:** higher = more bloodcats; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/fRespawn`

- **Type:** `Boolean`
- **Description:** If set, bloodcats in this sector can respawn on the Experienced difficulty. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubAmbushChance`

- **Type:** `Integer`
- **Description:** Chance that bloodcats ambush the player in this sector on Experienced difficulty, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubFactionAffiliation`

- **Type:** `Integer`
- **Description:** Faction affiliation ID used for the bloodcat group on Experienced difficulty. It is a faction reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMinBloodcats`

- **Type:** `Integer`
- **Description:** Minimum number of bloodcats spawned in this sector on Experienced difficulty. It must not exceed `ubMaxBloodcats`.
- **Numeric semantics:** higher = more bloodcats; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMaxBloodcats`

- **Type:** `Integer`
- **Description:** Maximum number of bloodcats spawned in this sector on Experienced difficulty. It must not be below `ubMinBloodcats`.
- **Numeric semantics:** higher = more bloodcats; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/fRespawn`

- **Type:** `Boolean`
- **Description:** If set, bloodcats in this sector can respawn on the Expert difficulty. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubAmbushChance`

- **Type:** `Integer`
- **Description:** Chance that bloodcats ambush the player in this sector on Expert difficulty, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubFactionAffiliation`

- **Type:** `Integer`
- **Description:** Faction affiliation ID used for the bloodcat group on Expert difficulty. It is a faction reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMinBloodcats`

- **Type:** `Integer`
- **Description:** Minimum number of bloodcats spawned in this sector on Expert difficulty. It must not exceed `ubMaxBloodcats`.
- **Numeric semantics:** higher = more bloodcats; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMaxBloodcats`

- **Type:** `Integer`
- **Description:** Maximum number of bloodcats spawned in this sector on Expert difficulty. It must not be below `ubMinBloodcats`.
- **Numeric semantics:** higher = more bloodcats; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/fRespawn`

- **Type:** `Boolean`
- **Description:** If set, bloodcats in this sector can respawn on the Insane difficulty. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubAmbushChance`

- **Type:** `Integer`
- **Description:** Chance that bloodcats ambush the player in this sector on Insane difficulty, in percent.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage `0..100`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubFactionAffiliation`

- **Type:** `Integer`
- **Description:** Faction affiliation ID used for the bloodcat group on Insane difficulty. It is a faction reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMinBloodcats`

- **Type:** `Integer`
- **Description:** Minimum number of bloodcats spawned in this sector on Insane difficulty. It must not exceed `ubMaxBloodcats`.
- **Numeric semantics:** higher = more bloodcats; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMaxBloodcats`

- **Type:** `Integer`
- **Description:** Maximum number of bloodcats spawned in this sector on Insane difficulty. It must not be below `ubMinBloodcats`.
- **Numeric semantics:** higher = more bloodcats; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `SectorGrid` identifies a sector whose names and terrain are defined in `SectorNames`; difficulty sections mirror the campaign difficulty levels.

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
