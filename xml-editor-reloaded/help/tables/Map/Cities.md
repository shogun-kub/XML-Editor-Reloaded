---
schema: 1
lang: en
family: map-cities
title: "Cities - towns, country and sector-to-town map"
tables:
  - Cities
table_ids:
  - Map.Cities
files:
  - Map/Cities.xml
root: CITY_INFO
record: CITY
key: uiIndex
sources:
  - Game code (strategic-map loader and map UI)
  - City table data
status: reviewed
updated: 2026-11-07
---

# Cities - towns, country and sector-to-town map

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CITY_INFO/COUNTRY/countryName`](#city_infocountrycountryname)
  - [`CITY_INFO/COUNTRY/countryNoun`](#city_infocountrycountrynoun)
  - [`CITY_INFO/CITYLIST/CITY/uiIndex`](#city_infocitylistcityuiindex)
  - [`CITY_INFO/CITYLIST/CITY/townName`](#city_infocitylistcitytownname)
  - [`CITY_INFO/CITYLIST/CITY/townUsesLoyalty`](#city_infocitylistcitytownusesloyalty)
  - [`CITY_INFO/CITYLIST/CITY/townRebelSentiment`](#city_infocitylistcitytownrebelsentiment)
  - [`CITY_INFO/CITYLIST/CITY/townMilitiaAllowed`](#city_infocitylistcitytownmilitiaallowed)
  - [`CITY_INFO/CITYLIST/CITY/hiddenTown`](#city_infocitylistcityhiddentown)
  - [`CITY_INFO/CITYLIST/CITY/townIcon`](#city_infocitylistcitytownicon)
  - [`CITY_INFO/CITYLIST/CITY/szIconFile`](#city_infocitylistcitysziconfile)
  - [`CITY_INFO/CITYLIST/CITY/iconPosition/x`](#city_infocitylistcityiconpositionx)
  - [`CITY_INFO/CITYLIST/CITY/iconPosition/y`](#city_infocitylistcityiconpositiony)
  - [`CITY_INFO/CITYLIST/CITY/baseSector/x`](#city_infocitylistcitybasesectorx)
  - [`CITY_INFO/CITYLIST/CITY/baseSector/y`](#city_infocitylistcitybasesectory)
  - [`CITY_INFO/CITYLIST/CITY/townPoint/x`](#city_infocitylistcitytownpointx)
  - [`CITY_INFO/CITYLIST/CITY/townPoint/y`](#city_infocitylistcitytownpointy)
  - [`CITY_INFO/CITY_TABLE/CITY_TABLE_ROW`](#city_infocity_tablecity_table_row)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Cities.xml` defines the towns of Arulco, the country name and the sector-to-town matrix. The
game reads it at the start of a new campaign; the values drive town loyalty, rebel sentiment,
militia permission, strategic-map icons and the town name shown for a sector.

The file is not a single flat list: it contains one `COUNTRY` singleton, a `CITYLIST` of
towns, and a `CITY_TABLE` sector-to-town matrix.

## Tables

| Table | File | Notes |
|---|---|---|
| `Cities` | `Map/Cities.xml` | plus `COUNTRY` and `CITY_TABLE` |

The editor treats `CITYLIST/CITY` as the record collection (key `uiIndex`) and `COUNTRY` as a
root singleton. `CITY_TABLE` is a special matrix, not a record collection.

## Structure

```
CITY_INFO
├── COUNTRY
│   ├── countryName
│   └── countryNoun
├── CITYLIST
│   └── CITY              one town, keyed by uiIndex
│       ├── uiIndex
│       ├── townName
│       ├── townUsesLoyalty
│       ├── townRebelSentiment
│       ├── townMilitiaAllowed
│       ├── hiddenTown
│       ├── townIcon
│       ├── szIconFile
│       ├── iconPosition { x, y }
│       ├── baseSector { x, y }
│       └── townPoint { x, y }
└── CITY_TABLE
    └── CITY_TABLE_ROW    row="00".."17", town IDs per sector column
```

`uiIndex` is the town array index. The loader ignores `0` (reserved for "no town") and values
outside the array; the highest valid index + 1 becomes `NUM_TOWNS`. Duplicate indexes would
overwrite earlier rows.

## Fields

### `CITY_INFO/COUNTRY/countryName`

- **Type:** `String`
- **Description:** Name of the country shown in the strategic UI and in some dialogues.
- **Values:** free text; a Russian overlay provides a translated value in v4.
- **Default:** empty.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/COUNTRY/countryNoun`

- **Type:** `String`
- **Description:** Noun for a person from the country (demonym), used in dialogue text.
- **Values:** free text; localized in v4.
- **Default:** empty.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Town identity and array index. `0` is reserved (no town); the loader
  skips it. The highest index sets `NUM_TOWNS`.
- **Values:** `1..MAX_TOWNS-1`; the loader skips `0` and indexes outside the array.
- **Default:** none; a missing value is treated as invalid and the town is skipped.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townName`

- **Type:** `String`
- **Description:** Display name of the town. It is copied into `pTownNames[uiIndex]` and is
  translated by the Russian overlay in v4.
- **Values:** free text, limited by `MAX_TOWN_NAME_LENGHT`.
- **Default:** empty.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townUsesLoyalty`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Whether the town maintains and displays loyalty. When false, loyalty
  events do not change the town's value.
- **Values:** `0` no loyalty, `1` use loyalty.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townRebelSentiment`

- **Type:** `Integer` (`UINT8`)
- **Description:** How strongly the town hates the Queen's rule. It scales the rate of
  loyalty change: the same event changes loyalty more in a town with a high value.
- **Values:** `1..100`. The loader clamps values above 100 to 100 and values below 1 to 1.
- **Default:** `1` after clamping.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townMilitiaAllowed`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Whether the player can train militia in the town.
- **Values:** `0` no militia, `1` militia allowed.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/hiddenTown`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Whether the town starts hidden on the strategic map. `0` means visible,
  `1` means hidden. The loader stores the inverse in an internal visibility flag, and that
  flag's own comment uses the opposite naming, so the internal name is misleading; trust the
  XML value and the XML comment, not the internal flag name.
- **Values:** `0` visible, `1` hidden.
- **Default:** `0` (visible).
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townIcon`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Whether the town shows its custom icon on the strategic map.
- **Values:** `0` off, `1` on.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/szIconFile`

- **Type:** `String`
- **Description:** Path to the STI file used as the town icon, for example
  `interface\\PRISON.sti`. The loader copies it into `gHiddenIcon[uiIndex].IconSti` up to
  `MAX_ICON_CHARS`.
- **Values:** a path relative to the game data; empty means no custom icon.
- **Default:** empty.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/iconPosition/x`

- **Type:** `Integer` (`UINT8`)
- **Description:** X position of the custom town icon on the strategic map.
- **Values:** `0..255`; screen coordinates.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/iconPosition/y`

- **Type:** `Integer` (`UINT8`)
- **Description:** Y position of the custom town icon on the strategic map.
- **Values:** `0..255`; screen coordinates.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/baseSector/x`

- **Type:** `Integer` (`UINT8`)
- **Description:** X coordinate of the top-left sector of the 3x3 town cutout in
  `Interface/MilitiaMaps.sti`. `1..16`; the loader ignores values outside that range.
- **Values:** `1..16`.
- **Default:** `0` (invalid; the row is not placed).
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/baseSector/y`

- **Type:** `Integer` (`UINT8`)
- **Description:** Y coordinate of the top-left sector of the town militia cutout.
- **Values:** `1..16`.
- **Default:** `0` (invalid).
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townPoint/x`

- **Type:** `Integer` (32-bit map coordinate)
- **Description:** X position of the town name label on the strategic map, in tenths of a
  sector (the XML comment describes it as `10 * (X, Y)`). Fractions are possible.
- **Values:** map coordinates in tenths of a sector.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townPoint/y`

- **Type:** `Integer` (32-bit map coordinate)
- **Description:** Y position of the town name label, in tenths of a sector.
- **Values:** map coordinates.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITY_TABLE/CITY_TABLE_ROW`

- **Type:** `Matrix row` (text)
- **Description:** One row of the sector-to-town matrix. The `row` attribute is the
  zero-based map Y; the text contains whitespace-separated town `uiIndex` values (0 = no
  town). The loader writes each value into `StrategicMap[x + 18*y].bNameId`.
- **Values:** integer town IDs per row; `row` is `00`..`17`.
- **Default:** none; a missing or short row leaves the remaining cells unchanged.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

## Relations

- `CITY_INFO/CITY_TABLE/CITY_TABLE_ROW` cells -> `CITY_INFO/CITYLIST/CITY/uiIndex`.
- [Facilities](Facilities.md) places facilities in sectors; facility names come from
  `FacilityTypes` (future docs).
- `townName`, `countryName` and `countryNoun` are the only translatable fields; see
  `../../Localization.md`.

## Localization

`Map/Russian.Cities.xml` is the v4 overlay. It contains `countryName`, `countryNoun` and
`townName` only, keyed by `uiIndex`. Numeric fields, the `CITY_TABLE` matrix and the icon
fields stay in the base file.

## Versions

v4 and v5 have the same fields and structure. v4 ships the Russian overlay; v5 has no
`Russian.*` file, which is normal for the English base.

## Sources

- Game code: the strategic-map loader, validation and map UI.
- Table data: `Map/Cities.xml` and `Map/Russian.Cities.xml`.
- `hiddenTown` is a known naming trap: the XML value is correct, but the internal visibility
  flag is inverted. This is described in the field entry above.

## Open questions

- `townPoint` is described by the XML comment as tenths of a sector; the loader stores the
  parsed integer, and the exact scaling to screen pixels is in the map UI, not in this table.
- `hiddenTown` is correct per the XML comment, but the internal flag is inverted. Do not use
  the internal flag name as the meaning of the XML field.
- The matrix dimensions are fixed by the code. A total-conversion mod with a different
  world size would need a code change, not only XML edits.
