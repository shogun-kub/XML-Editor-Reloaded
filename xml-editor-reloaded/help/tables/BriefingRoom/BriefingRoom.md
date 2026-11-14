---
schema: 1
lang: en
family: briefing-room
title: "BriefingRoom - mission briefing entries"
tables:
  - BriefingRoom
table_ids:
  - BriefingRoom.BriefingRoom
files:
  - BriefingRoom/BriefingRoom.xml
root: BRIEFINGROOM
record: DATA
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# BriefingRoom - mission briefing entries

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`BRIEFINGROOM/DATA/uiIndex`](#briefingroomdatauiindex)
  - [`BRIEFINGROOM/DATA/Name`](#briefingroomdataname)
  - [`BRIEFINGROOM/DATA/Hidden`](#briefingroomdatahidden)
  - [`BRIEFINGROOM/DATA/MaxPages`](#briefingroomdatamaxpages)
  - [`BRIEFINGROOM/DATA/MaxImages`](#briefingroomdatamaximages)
  - [`BRIEFINGROOM/DATA/ImagePositionX`](#briefingroomdataimagepositionx)
  - [`BRIEFINGROOM/DATA/ImagePositionY`](#briefingroomdataimagepositiony)
  - [`BRIEFINGROOM/DATA/NextMission`](#briefingroomdatanextmission)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`BriefingRoom.xml` defines the pages and images of mission briefings. Each entry is indexed, can be hidden, and points to the next mission with `NextMission` (`-1` means none).

## Tables

| Table | File | Notes |
|---|---|---|
| `BriefingRoom` | `BriefingRoom/BriefingRoom.xml` | — |

## Structure

```
BRIEFINGROOM
└── DATA
    ├── uiIndex
    ├── Name
    ├── Hidden
    ├── MaxPages
    ├── MaxImages
    ├── ImagePositionX
    ├── ImagePositionY
    └── NextMission
```

## Fields

### `BRIEFINGROOM/DATA/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the briefing-room entry. It is an array key.
- **Values:** `0..4294967295`; the loader stores this field as `UINT32`.
- **Default:** Required; identifies the record.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Display name of the mission/briefing.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/Hidden`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Visibility switch with inverted meaning in the loader: `0` hides the briefing entry, any non-zero value makes it visible.
- **Numeric semantics:** Boolean; `0` = hidden, non-zero = visible.
- **Values:** `0` or `1` in the shipped table.
- **Default:** Absent; the game default applies.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/MaxPages`

- **Type:** `Integer` (`UINT32`)
- **Description:** Maximum number of text pages shown for this briefing.
- **Numeric semantics:** higher = more pages; negative values are not meaningful.
- **Values:** `0..4294967295`; the loader stores this field as `UINT32`.
- **Default:** Absent; the game default applies.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/MaxImages`

- **Type:** `Integer` (`INT32`)
- **Description:** Maximum number of images shown for this briefing.
- **Numeric semantics:** higher = more images; the loader stores this field as signed `INT32`.
- **Values:** `-2147483648..2147483647`; the intended range is non-negative.
- **Default:** Absent; the game default applies.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/ImagePositionX`

- **Type:** `Integer` (`UINT32`)
- **Description:** Horizontal screen position of the briefing image, in pixels.
- **Numeric semantics:** increasing = right, decreasing = left.
- **Values:** `0..4294967295`; the loader stores this field as `UINT32`.
- **Default:** Absent; the game default applies.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/ImagePositionY`

- **Type:** `Integer` (`UINT32`)
- **Description:** Vertical screen position of the briefing image, in pixels.
- **Numeric semantics:** increasing = down, decreasing = up.
- **Values:** `0..4294967295`; the loader stores this field as `UINT32`.
- **Default:** Absent; the game default applies.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/NextMission`

- **Type:** `Integer` (`INT32`)
- **Description:** Index of the next briefing entry, or `-1` when there is no next mission. It is a reference.
- **Numeric semantics:** `-1` = no next mission; non-negative = index of the next briefing entry.
- **Values:** Briefing index or `-1`; the loader stores this field as `INT32`.
- **Default:** Absent; the game default applies.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

## Relations

- `NextMission` references another briefing `uiIndex`, or `-1` for the end of the chain.

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for inferred fields is not stated by the table data.
