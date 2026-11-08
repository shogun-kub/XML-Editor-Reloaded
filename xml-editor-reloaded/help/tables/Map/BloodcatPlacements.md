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
  - Game code (bloodcat placement loader and strategic/tactical use)
  - Map table data
  - Field names and XML comments
status: reviewed
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

`BloodcatPlacements.xml` lists sectors where bloodcats can appear. Each sector has one placement type and, for each difficulty, values for the group size, ambush chance, faction affiliation and respawn; which of those values matter depends on the placement type.

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
- **Description:** Sector code in `A1`..`P16` form: a row letter (`A`..`P`) followed by a column number (`1`..`16`). It selects `gBloodcatPlacements[sectorId]`; the grid text itself is not stored in the record.
- **Values:** `A1`..`P16`. The loader takes the first character as the row (`& 0x1F`) and parses the rest as the column.
- **Default:** Required; an omitted or malformed code is ignored (only coordinates that parse in range are stored), so the previous sector is reused.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/ubPlacementType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Placement rule for the sector: `0` ambush (bloodcats appear only when an ambush triggers), `1` static (they are present from the start and follow faction rules), `2` lair (they are present from the start and replenish over time). At least one lair sector must exist, otherwise game start fails.
- **Values:** `0` Ambush, `1` Static Placement, `2` Lair; other values are not handled.
- **Default:** `0` when omitted; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/fRespawn`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Respawn switch for Novice difficulty. The loader stores it only for lair placement (`ubPlacementType` `2`); no gameplay code in the 1.13 source reads the flag, so it does not currently change replenishment.
- **Numeric semantics:** `0` = off, any non-zero value = on; the byte is stored as written.
- **Values:** `0` or `1`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubAmbushChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that an ambush triggers in this sector on Novice difficulty, in percent. It applies only to ambush placement (`ubPlacementType` `0`). The effective chance is capped by the hours elapsed since the sector was last loaded, so it climbs by one point per hour up to the configured value.
- **Numeric semantics:** `0` disables the ambush; `100` or more always passes the `PreChance` roll; negative values are impossible.
- **Values:** Percentage `0..100`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubFactionAffiliation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Civilian-group affiliation applied to the sector's bloodcats on Novice difficulty. It is used only by static placement (`ubPlacementType` `1`): `0` means no group, a non-zero value assigns that group, and `14` (`QUEENS_CIV_GROUP`) makes bloodcats and the army ignore each other.
- **Values:** Civilian-group index from the `EditorMercs.h` enum (`0` = `NON_CIV_GROUP`, `14` = `QUEENS_CIV_GROUP`).
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `LOS.cpp`, `Overhead.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMinBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Lower bound of the group when a bloodcat ambush triggers on Novice difficulty. The count starts here and grows with player progress toward `ubMaxBloodcats`. Static and lair placement ignore it (the loader forces `0`).
- **Numeric semantics:** higher = more bloodcats; it should not exceed `ubMaxBloodcats`, and the loader does not validate the pair, so an inverted pair underflows the computed range.
- **Values:** Count `0..255`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMaxBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Upper bound of the ambush group on Novice difficulty; for static and lair placement it is the initial number of bloodcats set at game start and the sector's bloodcat capacity.
- **Numeric semantics:** higher = more bloodcats; it should be at least `ubMinBloodcats`.
- **Values:** Count `0..255`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/fRespawn`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Respawn switch for Experienced difficulty. The loader stores it only for lair placement (`ubPlacementType` `2`); no gameplay code in the 1.13 source reads the flag, so it does not currently change replenishment.
- **Numeric semantics:** `0` = off, any non-zero value = on; the byte is stored as written.
- **Values:** `0` or `1`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubAmbushChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that an ambush triggers in this sector on Experienced difficulty, in percent. It applies only to ambush placement (`ubPlacementType` `0`). The effective chance is capped by the hours elapsed since the sector was last loaded, so it climbs by one point per hour up to the configured value.
- **Numeric semantics:** `0` disables the ambush; `100` or more always passes the `PreChance` roll; negative values are impossible.
- **Values:** Percentage `0..100`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubFactionAffiliation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Civilian-group affiliation applied to the sector's bloodcats on Experienced difficulty. It is used only by static placement (`ubPlacementType` `1`): `0` means no group, a non-zero value assigns that group, and `14` (`QUEENS_CIV_GROUP`) makes bloodcats and the army ignore each other.
- **Values:** Civilian-group index from the `EditorMercs.h` enum (`0` = `NON_CIV_GROUP`, `14` = `QUEENS_CIV_GROUP`).
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `LOS.cpp`, `Overhead.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMinBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Lower bound of the group when a bloodcat ambush triggers on Experienced difficulty. The count starts here and grows with player progress toward `ubMaxBloodcats`. Static and lair placement ignore it (the loader forces `0`).
- **Numeric semantics:** higher = more bloodcats; it should not exceed `ubMaxBloodcats`, and the loader does not validate the pair, so an inverted pair underflows the computed range.
- **Values:** Count `0..255`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMaxBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Upper bound of the ambush group on Experienced difficulty; for static and lair placement it is the initial number of bloodcats set at game start and the sector's bloodcat capacity.
- **Numeric semantics:** higher = more bloodcats; it should be at least `ubMinBloodcats`.
- **Values:** Count `0..255`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/fRespawn`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Respawn switch for Expert difficulty. The loader stores it only for lair placement (`ubPlacementType` `2`); no gameplay code in the 1.13 source reads the flag, so it does not currently change replenishment.
- **Numeric semantics:** `0` = off, any non-zero value = on; the byte is stored as written.
- **Values:** `0` or `1`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubAmbushChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that an ambush triggers in this sector on Expert difficulty, in percent. It applies only to ambush placement (`ubPlacementType` `0`). The effective chance is capped by the hours elapsed since the sector was last loaded, so it climbs by one point per hour up to the configured value.
- **Numeric semantics:** `0` disables the ambush; `100` or more always passes the `PreChance` roll; negative values are impossible.
- **Values:** Percentage `0..100`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubFactionAffiliation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Civilian-group affiliation applied to the sector's bloodcats on Expert difficulty. It is used only by static placement (`ubPlacementType` `1`): `0` means no group, a non-zero value assigns that group, and `14` (`QUEENS_CIV_GROUP`) makes bloodcats and the army ignore each other.
- **Values:** Civilian-group index from the `EditorMercs.h` enum (`0` = `NON_CIV_GROUP`, `14` = `QUEENS_CIV_GROUP`).
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `LOS.cpp`, `Overhead.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMinBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Lower bound of the group when a bloodcat ambush triggers on Expert difficulty. The count starts here and grows with player progress toward `ubMaxBloodcats`. Static and lair placement ignore it (the loader forces `0`).
- **Numeric semantics:** higher = more bloodcats; it should not exceed `ubMaxBloodcats`, and the loader does not validate the pair, so an inverted pair underflows the computed range.
- **Values:** Count `0..255`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMaxBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Upper bound of the ambush group on Expert difficulty; for static and lair placement it is the initial number of bloodcats set at game start and the sector's bloodcat capacity.
- **Numeric semantics:** higher = more bloodcats; it should be at least `ubMinBloodcats`.
- **Values:** Count `0..255`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/fRespawn`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Respawn switch for Insane difficulty. The loader stores it only for lair placement (`ubPlacementType` `2`); no gameplay code in the 1.13 source reads the flag, so it does not currently change replenishment.
- **Numeric semantics:** `0` = off, any non-zero value = on; the byte is stored as written.
- **Values:** `0` or `1`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubAmbushChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that an ambush triggers in this sector on Insane difficulty, in percent. It applies only to ambush placement (`ubPlacementType` `0`). The effective chance is capped by the hours elapsed since the sector was last loaded, so it climbs by one point per hour up to the configured value.
- **Numeric semantics:** `0` disables the ambush; `100` or more always passes the `PreChance` roll; negative values are impossible.
- **Values:** Percentage `0..100`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubFactionAffiliation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Civilian-group affiliation applied to the sector's bloodcats on Insane difficulty. It is used only by static placement (`ubPlacementType` `1`): `0` means no group, a non-zero value assigns that group, and `14` (`QUEENS_CIV_GROUP`) makes bloodcats and the army ignore each other.
- **Values:** Civilian-group index from the `EditorMercs.h` enum (`0` = `NON_CIV_GROUP`, `14` = `QUEENS_CIV_GROUP`).
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `LOS.cpp`, `Overhead.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMinBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Lower bound of the group when a bloodcat ambush triggers on Insane difficulty. The count starts here and grows with player progress toward `ubMaxBloodcats`. Static and lair placement ignore it (the loader forces `0`).
- **Numeric semantics:** higher = more bloodcats; it should not exceed `ubMaxBloodcats`, and the loader does not validate the pair, so an inverted pair underflows the computed range.
- **Values:** Count `0..255`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMaxBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Upper bound of the ambush group on Insane difficulty; for static and lair placement it is the initial number of bloodcats set at game start and the sector's bloodcat capacity.
- **Numeric semantics:** higher = more bloodcats; it should be at least `ubMinBloodcats`.
- **Values:** Count `0..255`.
- **Default:** `0` when the section or element is absent; the placement arrays are cleared before loading.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

## Relations

- `SectorGrid` identifies a sector whose names and terrain are defined in [SectorNames](SectorNames.md); the difficulty sections mirror the campaign difficulty levels (`NOVICE` = 0, `EXPERIENCED` = 1, `EXPERT` = 2, `INSANE` = 3).
- `ubPlacementType` is difficulty-independent: it is stored once per sector.
- `ubMaxBloodcats` is also the sector's bloodcat capacity (`SectorInfo.bBloodCatPlacements`), which clamps the ambush count.
- At game start at least one sector must have placement type `2` (lair), otherwise initialisation throws.
- `ubFactionAffiliation` is only applied to static placement; the values are civilian-group indices shared with [CivGroupNames](../CivGroupNames.md).

## Localization

The table has no display text; sector codes and numeric values are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Game code: `XML_Bloodcats.cpp` (loader), `Strategic Movement.cpp` (ambush and lair logic), `Game Init.cpp` (initial population and lair requirement), `LOS.cpp` and `Overhead.cpp` (static faction behaviour).
- Map table data.
- Field names and XML comments.
- Descriptions cover the code path that consumes each field; no row counts or observed data ranges are used.

## Open questions

- `fRespawn` is stored but not read by 1.13 gameplay code; whether lair replenishment was meant to be optional is not implemented in this source.
- The one-point-per-hour ramp of `ubAmbushChance` since the sector was last loaded is code-defined and not documented by the table.
