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
  - Game code (sector-loadscreens loader and loading-screen display)
  - Map table data and XML comments
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

`SectorLoadscreens.xml` maps a sector to the image shown while it loads. Record `0` is the landing-zone/helicopter screen; records `1..256` are matched by `szLocation` against the current surface sector. Images are extensionless paths under the game data directory: the loader appends a resolution suffix and probes `.png` then `.sti`. A missing or empty path does not fall back to a generic screen; the game fills the screen black and prints a "loadscreen data file not found" message. The loader also parses alternate day/night fields, but the current display code uses only the day alternate.

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

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the loading-screen record and the array slot it is stored in; it must be unique. Record `0` is used directly for the helicopter/landing-zone screen.
- **Values:** `0..256` (`MAX_SECTOR_LOADSCREENS - 1`); the loader parses decimal or `0x` hexadecimal and drops records outside the range.
- **Default:** Required; identifies the record.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szLocation`

- **Type:** `String` (`CHAR8[10]`)
- **Description:** Sector code (`A1`..`P16`) whose loading screen this record supplies, or `START` for the landing zone in record `0`. Records `1..256` are matched against the current sector code; record `0` is read directly for the helicopter screen.
- **Values:** `A1`..`P16` or `START`; stored in a `CHAR8[10]` buffer.
- **Default:** Required for records `1..256`; record `0` uses the landing-zone screen regardless of its text.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/RandomAltSector`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the game makes a 50/50 choice between the primary and alternate screens for day or night (`Random(2) > 0`). With the current display code only the day alternate is effective (see `szNightAlt`).
- **Values:** `0` or `1`; any nonzero value is treated as set.
- **Default:** Absent; `0`.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szImageFormat`

- **Type:** `String` (`CHAR8[5]`)
- **Description:** Image-format label from the table (`STI` or `PNG` in shipped data). The current surface-sector selection does not read it; it probes `.png` and `.sti` filenames itself, so this field does not choose the file extension.
- **Values:** Free text up to 4 characters (`CHAR8[5]`).
- **Default:** Absent; empty string.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szDay`

- **Type:** `String` (`CHAR8[80]`)
- **Description:** Path to the primary daytime loading-screen image, relative to the game data directory and without an extension (for example `Loadscreens\LS_DayTropical`). The loader appends a resolution suffix and probes `.png` then `.sti`.
- **Values:** Free text path up to 79 characters (`CHAR8[80]`); empty = no daytime image (the display then reports a missing file).
- **Default:** Absent; empty string.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szNight`

- **Type:** `String` (`CHAR8[80]`)
- **Description:** Path to the primary nighttime loading-screen image, relative to the game data directory and without an extension. Same resolution-suffix and `.png`/`.sti` probing as `szDay`.
- **Values:** Free text path up to 79 characters (`CHAR8[80]`); empty = no nighttime image (the display then reports a missing file).
- **Default:** Absent; empty string.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szImageFormatAlt`

- **Type:** `String` (`CHAR8[5]`)
- **Description:** Alternate image-format label; when `RandomAltSector` picks the alternate and this value is non-empty, it replaces `szImageFormat`. Like `szImageFormat`, it is not used to choose the surface file extension in the current code.
- **Values:** Free text up to 4 characters (`CHAR8[5]`).
- **Default:** Absent; empty string.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szDayAlt`

- **Type:** `String` (`CHAR8[80]`)
- **Description:** Path to the alternate daytime loading-screen image, used when `RandomAltSector` picks the alternate and the screen is `DAY`. Same extensionless-path rules as `szDay`.
- **Values:** Free text path up to 79 characters; empty = no alternate day image (the display then reports a missing file instead of falling back to `szDay`).
- **Default:** Absent; empty string.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szNightAlt`

- **Type:** `String` (`CHAR8[80]`)
- **Description:** Path to the alternate nighttime loading-screen image in the table, but the current `DisplayLoadScreenWithID` uses `szNight` for the `NIGHT_ALT` case, so this value is parsed and never read by the display code. The shipped `B2` record, for example, keeps `LS_NightChitzena` instead of `LS_MineNight`.
- **Values:** Free text path up to 79 characters; ignored by the current display code.
- **Default:** Absent; ignored.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

## Relations

- `szLocation` identifies the surface sector whose loading screen is used; the sector itself is defined in [SectorNames](SectorNames.md). Record `0` is the landing-zone/helicopter entry and is not matched by sector code.

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Game code: sector-loadscreens loader and loading-screen display.
- Map table data and XML comments.
- Descriptions follow the current loading-screen path; no generic image fallback is claimed because the game draws a black diagnostic screen instead.

## Open questions

- `DisplayLoadScreenWithID` selects `szNight` for the `NIGHT_ALT` case instead of `szNightAlt`, so `szNightAlt` is parsed but never used; whether this is a bug or intended is unknown.
- `szImageFormat` and `szImageFormatAlt` are parsed but the surface-sector path chooses `.png`/`.sti` by file probing; whether the format fields are meant to influence selection is unknown.
- If no record matches, or the selected path is empty or missing, the game shows a black screen with a diagnostic; whether a generic fallback was intended is unknown.
