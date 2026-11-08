---
schema: 1
lang: en
family: mercs-faces
title: "RPCFacesSmall - small RPC portraits"
tables:
  - RPCFacesSmall
table_ids:
  - RPCFacesSmall
files:
  - RPCFacesSmall.xml
root: SMALLFACE
record: FACE
key: uiIndex
sources:
  - RPC small-face table data
status: reviewed
updated: 2026-11-07
---

# RPCFacesSmall - small RPC portraits

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SMALLFACE/FACE/uiIndex`](#smallfacefaceuiindex)
  - [`SMALLFACE/FACE/Name`](#smallfacefacename)
  - [`SMALLFACE/FACE/FaceIndex`](#smallfacefacefaceindex)
  - [`SMALLFACE/FACE/EyesX`](#smallfacefaceeyesx)
  - [`SMALLFACE/FACE/EyesY`](#smallfacefaceeyesy)
  - [`SMALLFACE/FACE/MouthX`](#smallfacefacemouthx)
  - [`SMALLFACE/FACE/MouthY`](#smallfacefacemouthy)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`RPCFacesSmall.xml` defines the small RPC portrait index and the mouth/eye coordinates used by the small-face animation.

## Tables

| Table | File | Notes |
|---|---|---|
| `RPCFacesSmall` | `RPCFacesSmall.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
SMALLFACE
└── FACE
    ├── uiIndex
    ├── Name
    ├── FaceIndex
    ├── EyesX
    ├── EyesY
    ├── MouthX
    └── MouthY
```

## Fields

### `SMALLFACE/FACE/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Small-face row identity.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/Name`

- **Type:** `String` (`String`)
- **Description:** RPC name stored in the table, but the loader does not read this element; it is ignored.
- **Values:** Free text; ignored at load time.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/FaceIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Face index in the portrait tables.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/EyesX`

- **Type:** `Integer` (`INT8`)
- **Description:** X coordinate of the eyes on the small face.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** `-128..127`; signed face coordinate.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/EyesY`

- **Type:** `Integer` (`INT8`)
- **Description:** Y coordinate of the eyes on the small face.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** `-128..127`; signed face coordinate.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/MouthX`

- **Type:** `Integer` (`INT8`)
- **Description:** X coordinate of the mouth on the small face.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** `-128..127`; signed face coordinate.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/MouthY`

- **Type:** `Integer` (`INT8`)
- **Description:** Y coordinate of the mouth on the small face.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** `-128..127`; signed face coordinate.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

## Relations

- `FaceIndex` points to the portrait/face tables used by [MercProfiles](MercProfiles.md).

## Localization

`Name` is a display label.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- RPC small-face table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The small-face renderer uses the coordinates; the exact scaling is outside this table.
