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
status: draft
updated: 2026-11-07
---

# RPCFacesSmall - small RPC portraits

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SMALLFACE/FACE/EyesX`](#smallfacefaceeyesx)
  - [`SMALLFACE/FACE/EyesY`](#smallfacefaceeyesy)
  - [`SMALLFACE/FACE/FaceIndex`](#smallfacefacefaceindex)
  - [`SMALLFACE/FACE/MouthX`](#smallfacefacemouthx)
  - [`SMALLFACE/FACE/MouthY`](#smallfacefacemouthy)
  - [`SMALLFACE/FACE/Name`](#smallfacefacename)
  - [`SMALLFACE/FACE/uiIndex`](#smallfacefaceuiindex)
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
└── FACE                    one small portrait
    ├── uiIndex             row identity
    ├── FaceIndex           portrait index
    └── Mouth/Eyes X/Y      animation coordinates
```

## Fields

### `SMALLFACE/FACE/EyesX`

- **Type:** `Integer` (`INT32`)
- **Description:** X coordinate of the eyes on the small face.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** rpc small-face table data; item/profile metadata.
- **Confidence:** `inferred`

### `SMALLFACE/FACE/EyesY`

- **Type:** `Integer` (`INT32`)
- **Description:** Y coordinate of the eyes on the small face.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** rpc small-face table data; item/profile metadata.
- **Confidence:** `inferred`

### `SMALLFACE/FACE/FaceIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Face index in the portrait tables.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** rpc small-face table data; item/profile metadata.
- **Confidence:** `inferred`

### `SMALLFACE/FACE/MouthX`

- **Type:** `Integer` (`INT32`)
- **Description:** X coordinate of the mouth on the small face.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** rpc small-face table data; item/profile metadata.
- **Confidence:** `inferred`

### `SMALLFACE/FACE/MouthY`

- **Type:** `Integer` (`INT32`)
- **Description:** Y coordinate of the mouth on the small face.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** rpc small-face table data; item/profile metadata.
- **Confidence:** `inferred`

### `SMALLFACE/FACE/Name`

- **Type:** `String` (`String`)
- **Description:** RPC name for the small face.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** rpc small-face table data; item/profile metadata.
- **Confidence:** `inferred`

### `SMALLFACE/FACE/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Small-face row identity.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** rpc small-face table data; item/profile metadata.
- **Confidence:** `inferred`

## Relations

- `FaceIndex` points to the portrait/face tables.

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
