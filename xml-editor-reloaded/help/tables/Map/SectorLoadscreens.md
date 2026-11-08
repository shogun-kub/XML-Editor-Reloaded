---
schema: 1
lang: en
family: map-sector-loadscreens
title: "SectorLoadscreens - sector loading screens"
tables:
  - SectorLoadscreens
table_ids:
  - Map.SectorLoadscreens
files:
  - Map/SectorLoadscreens.xml
root: LOADSCREENS
record: SECTOR
key: uiIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# SectorLoadscreens - sector loading screens

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`LOADSCREENS/SECTOR/uiIndex`](#loadscreenssectoruiindex)
  - [`LOADSCREENS/SECTOR/szLocation`](#loadscreenssectorszlocation)
  - [`LOADSCREENS/SECTOR/RandomAltSector`](#loadscreenssectorrandomaltsector)
  - [`LOADSCREENS/SECTOR/szImageFormat`](#loadscreenssectorszimageformat)
  - [`LOADSCREENS/SECTOR/szDay`](#loadscreenssectorszday)
  - [`LOADSCREENS/SECTOR/szNight`](#loadscreenssectorsznight)
  - [`LOADSCREENS/SECTOR/szImageFormatAlt`](#loadscreenssectorszimageformatalt)
  - [`LOADSCREENS/SECTOR/szDayAlt`](#loadscreenssectorszdayalt)
  - [`LOADSCREENS/SECTOR/szNightAlt`](#loadscreenssectorsznightalt)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SectorLoadscreens.xml` maps a sector to the image shown while it loads. Each record has a primary and an optional alternate day/night image; paths are relative to the game data directory. Missing images fall back to the generic loading screen.

## Tables

| Table | File | Notes |
|---|---|---|
| `SectorLoadscreens` | `Map/SectorLoadscreens.xml` | — |

## Structure

```
LOADSCREENS
└── SECTOR
    ├── uiIndex
    ├── szLocation
    ├── RandomAltSector
    ├── szImageFormat
    ├── szDay
    ├── szNight
    ├── szImageFormatAlt
    ├── szDayAlt
    └── szNightAlt
```

## Fields

### `LOADSCREENS/SECTOR/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the loading-screen record. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENS/SECTOR/szLocation`

- **Type:** `String`
- **Description:** Sector code this screen belongs to, or `START` for the landing zone. It is a map label.
- **Values:** Sector code or `START`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENS/SECTOR/RandomAltSector`

- **Type:** `Boolean`
- **Description:** If set, the game may choose the alternate sector screen instead of the primary one.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENS/SECTOR/szImageFormat`

- **Type:** `String`
- **Description:** File format of the primary day/night images, for example `STI` or `PNG`.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENS/SECTOR/szDay`

- **Type:** `String`
- **Description:** Path to the daytime loading-screen image for this sector. An empty value means no daytime image is defined.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENS/SECTOR/szNight`

- **Type:** `String`
- **Description:** Path to the nighttime loading-screen image for this sector. An empty value means no nighttime image is defined.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENS/SECTOR/szImageFormatAlt`

- **Type:** `String`
- **Description:** File format of the alternate day/night images.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENS/SECTOR/szDayAlt`

- **Type:** `String`
- **Description:** Path to the alternate daytime loading-screen image. Empty means no alternate image.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `LOADSCREENS/SECTOR/szNightAlt`

- **Type:** `String`
- **Description:** Path to the alternate nighttime loading-screen image. Empty means no alternate image.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `szLocation` identifies the sector whose loading screen is used; the sector itself is defined in `SectorNames`.

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
