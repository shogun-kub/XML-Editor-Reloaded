---
schema: 1
lang: en
family: map-cities
title: "Map.Cities - towns, country and sector-to-town map"
tables:
  - Map.Cities
files:
  - Map/Cities.xml
root: CITY_INFO
record: CITY
key: uiIndex
sources:
  - Strategic/strategicmap.cpp
  - Map Screen Interface Map.cpp
status: draft
updated: 2026-11-07
---

# Map.Cities - towns, country and sector-to-town map

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CITY_INFO/COUNTRY/countryName`](#cityinfocountrycountryname)
  - [`CITY_INFO/COUNTRY/countryNoun`](#cityinfocountrycountrynoun)
  - [`CITY_INFO/CITYLIST/CITY/uiIndex`](#cityinfocitylistcityuiindex)
  - [`CITY_INFO/CITYLIST/CITY/townName`](#cityinfocitylistcitytownname)
  - [`CITY_INFO/CITYLIST/CITY/townUsesLoyalty`](#cityinfocitylistcitytownusesloyalty)
  - [`CITY_INFO/CITYLIST/CITY/townRebelSentiment`](#cityinfocitylistcitytownrebelsentiment)
  - [`CITY_INFO/CITYLIST/CITY/townMilitiaAllowed`](#cityinfocitylistcitytownmilitiaallowed)
  - [`CITY_INFO/CITYLIST/CITY/hiddenTown`](#cityinfocitylistcityhiddentown)
  - [`CITY_INFO/CITYLIST/CITY/townIcon`](#cityinfocitylistcitytownicon)
  - [`CITY_INFO/CITYLIST/CITY/szIconFile`](#cityinfocitylistcitysziconfile)
  - [`CITY_INFO/CITYLIST/CITY/iconPosition/x`](#cityinfocitylistcityiconpositionx)
  - [`CITY_INFO/CITYLIST/CITY/iconPosition/y`](#cityinfocitylistcityiconpositiony)
  - [`CITY_INFO/CITYLIST/CITY/baseSector/x`](#cityinfocitylistcitybasesectorx)
  - [`CITY_INFO/CITYLIST/CITY/baseSector/y`](#cityinfocitylistcitybasesectory)
  - [`CITY_INFO/CITYLIST/CITY/townPoint/x`](#cityinfocitylistcitytownpointx)
  - [`CITY_INFO/CITYLIST/CITY/townPoint/y`](#cityinfocitylistcitytownpointy)
  - [`CITY_INFO/CITY_TABLE/CITY_TABLE_ROW`](#cityinfocitytablecitytablerow)
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
12 towns, and a `CITY_TABLE` matrix of 18 rows by 18 columns.

## Tables

| Table | File | Records | Notes |
|---|---|---:|---|
| `Map.Cities` | `Map/Cities.xml` | 12 `CITY` records | plus `COUNTRY` and `CITY_TABLE` |

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
    └── CITY_TABLE_ROW    row="00".."17", 18 town IDs per row
```

`uiIndex` is the town array index. The loader ignores `0` (reserved for "no town") and values
outside the array; the highest valid index + 1 becomes `NUM_TOWNS`. Duplicate indexes would
overwrite earlier rows; the shipped data has no duplicates.

## Fields

### `CITY_INFO/COUNTRY/countryName`

- **Type:** `String`
- **Description:** Name of the country shown in the strategic UI and in some dialogues.
- **Values:** free text; a Russian overlay provides a translated value in v4.
- **Default:** empty.
- **Source:** `Strategic/strategicmap.cpp:1317-1322`
- **Confidence:** `source`

### `CITY_INFO/COUNTRY/countryNoun`

- **Type:** `String`
- **Description:** Noun for a person from the country (demonym), used in dialogue text.
- **Values:** free text; localized in v4.
- **Default:** empty.
- **Source:** `Strategic/strategicmap.cpp:1324-1329`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Town identity and array index. `0` is reserved (no town); the loader
  skips it. The highest index sets `NUM_TOWNS`.
- **Values:** `1..MAX_TOWNS-1`; shipped data uses 1..12.
- **Default:** none; a missing value is treated as invalid and the town is skipped.
- **Source:** `Strategic/strategicmap.cpp:1182-1186`, `:1126`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townName`

- **Type:** `String`
- **Description:** Display name of the town. It is copied into `pTownNames[uiIndex]` and is
  translated by the Russian overlay in v4.
- **Values:** free text, limited by `MAX_TOWN_NAME_LENGHT`.
- **Default:** empty.
- **Source:** `Strategic/strategicmap.cpp:1189-1199`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townUsesLoyalty`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Whether the town maintains and displays loyalty. When false, loyalty
  events do not change the town's value.
- **Values:** `0` no loyalty, `1` use loyalty.
- **Default:** `0`.
- **Source:** `Strategic/strategicmap.cpp:1200`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townRebelSentiment`

- **Type:** `Integer` (`UINT8`)
- **Description:** How strongly the town hates the Queen's rule. It scales the rate of
  loyalty change: the same event changes loyalty more in a town with a high value.
- **Values:** `1..100`. The loader clamps values above 100 to 100 and values below 1 to 1.
  Shipped data uses 10..90.
- **Default:** `1` after clamping.
- **Source:** `Strategic/strategicmap.cpp:1206-1212`, `:1137`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townMilitiaAllowed`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Whether the player can train militia in the town.
- **Values:** `0` no militia, `1` militia allowed.
- **Default:** `0`.
- **Source:** `Strategic/strategicmap.cpp:1218`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/hiddenTown`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Whether the town starts hidden on the strategic map. `0` means visible,
  `1` means hidden. The loader stores the inverse in `gfHiddenTown`, whose own comment says
  "Visible town are TRUE, hidden towns are FALSE" (`Strategic/strategicmap.cpp:812`), so the
  internal flag name is misleading; trust the XML value and the comment, not the flag name.
- **Values:** `0` visible, `1` hidden.
- **Default:** `0` (visible).
- **Source:** `Strategic/strategicmap.cpp:1154-1163`, `:812`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townIcon`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Whether the town shows its custom icon on the strategic map.
- **Values:** `0` off, `1` on.
- **Default:** `0`.
- **Source:** `Strategic/strategicmap.cpp:1167-1168`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/szIconFile`

- **Type:** `String`
- **Description:** Path to the STI file used as the town icon, for example
  `interface\\PRISON.sti`. The loader copies it into `gHiddenIcon[uiIndex].IconSti` up to
  `MAX_ICON_CHARS`.
- **Values:** a path relative to the game data; empty means no custom icon.
- **Default:** empty.
- **Source:** `Strategic/strategicmap.cpp:1232-1240`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/iconPosition/x`

- **Type:** `Integer` (`UINT8`)
- **Description:** X position of the custom town icon on the strategic map.
- **Values:** `0..255`; screen coordinates.
- **Default:** `0`.
- **Source:** `Strategic/strategicmap.cpp:1252-1262`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/iconPosition/y`

- **Type:** `Integer` (`UINT8`)
- **Description:** Y position of the custom town icon on the strategic map.
- **Values:** `0..255`; screen coordinates.
- **Default:** `0`.
- **Source:** `Strategic/strategicmap.cpp:1264-1270`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/baseSector/x`

- **Type:** `Integer` (`UINT8`)
- **Description:** X coordinate of the top-left sector of the 3x3 town cutout in
  `Interface/MilitiaMaps.sti`. `1..16`; the loader ignores values outside that range.
- **Values:** `1..16`.
- **Default:** `0` (invalid; the row is not placed).
- **Source:** `Strategic/strategicmap.cpp:1269-1279`, `:1134`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/baseSector/y`

- **Type:** `Integer` (`UINT8`)
- **Description:** Y coordinate of the top-left sector of the town militia cutout.
- **Values:** `1..16`.
- **Default:** `0` (invalid).
- **Source:** `Strategic/strategicmap.cpp:1281-1289`, `:1134`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townPoint/x`

- **Type:** `Integer` (`INT32` via `atol`, stored in `POINT`)
- **Description:** X position of the town name label on the strategic map, in tenths of a
  sector (the XML comment describes it as `10 * (X, Y)`). Fractions are possible.
- **Values:** map coordinates; shipped data uses values such as 90, 125.
- **Default:** `0`.
- **Source:** `Strategic/strategicmap.cpp:1285-1295`, `:1135`
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townPoint/y`

- **Type:** `Integer` (`INT32` via `atol`, stored in `POINT`)
- **Description:** Y position of the town name label, in tenths of a sector.
- **Values:** map coordinates.
- **Default:** `0`.
- **Source:** `Strategic/strategicmap.cpp:1297-1303`, `:1135`
- **Confidence:** `source`

### `CITY_INFO/CITY_TABLE/CITY_TABLE_ROW`

- **Type:** `Matrix row` (text)
- **Description:** One row of the 18x18 sector-to-town matrix. The `row` attribute is the
  zero-based map Y; the text contains 18 whitespace-separated town `uiIndex` values (0 = no
  town). The loader writes each value into `StrategicMap[x + 18*y].bNameId`.
- **Values:** 18 integers per row; `row` is `00`..`17`.
- **Default:** none; a missing or short row leaves the remaining cells unchanged.
- **Source:** `Strategic/strategicmap.cpp:907-920`, `:1110-1125`
- **Confidence:** `source`

## Relations

- `CITY_INFO/CITY_TABLE/CITY_TABLE_ROW` cells -> `CITY_INFO/CITYLIST/CITY/uiIndex`.
- `Map.Facilities` places facilities in sectors; facility names come from
  `Map.FacilityTypes` (future docs).
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

- `Strategic/strategicmap.cpp` - parser, validation, array writes and save function.
- `Strategic/strategicmap.cpp:812` - comment on the `gfHiddenTown` flag meaning.
- `Map Screen Interface Map.cpp` - town display and hidden-town drawing.
- `TableData/Map/Cities.xml`, `Map/Russian.Cities.xml` - shipped data and overlay.

## Open questions

- `townPoint` is described by the XML comment as tenths of a sector; the code stores raw
  `atol` values in a `POINT`. The exact scaling to screen pixels is in the map UI, not in this
  table.
- `hiddenTown` is correct per the XML comment, but the internal flag name is inverted. Anyone
  reading `gfHiddenTown` directly must remember that `TRUE` means visible.
- The matrix is 18x18 for the shipped world. A total-conversion mod with a different world
  size would need a code change, not only XML edits.

