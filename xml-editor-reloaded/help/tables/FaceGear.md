---
schema: 1
lang: en
family: mercs-faces
title: "FaceGear - face overlay assets"
tables:
  - FaceGear
table_ids:
  - FaceGear
files:
  - FaceGear.xml
root: FACE_GEAR
record: ITEM
key: uiIndex
sources:
  - Face gear table data
status: draft
updated: 2026-11-07
---

# FaceGear - face overlay assets

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`FACE_GEAR/ITEM/szFile`](#face_gearitemszfile)
  - [`FACE_GEAR/ITEM/Type`](#face_gearitemtype)
  - [`FACE_GEAR/ITEM/uiIndex`](#face_gearitemuiindex)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`FaceGear.xml` maps face-gear types to the STI overlay used by the portrait renderer.

## Tables

| Table | File | Notes |
|---|---|---|
| `FaceGear` | `FaceGear.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
FACE_GEAR
└── ITEM                    one face-gear asset
    ├── uiIndex             row identity
    ├── Type                face-gear type
    └── szFile              STI overlay file
```

## Fields

### `FACE_GEAR/ITEM/szFile`

- **Type:** `String` (`String`)
- **Description:** STI file for the face-gear overlay.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** face gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `FACE_GEAR/ITEM/Type`

- **Type:** `Integer` (`INT32`)
- **Description:** Face-gear type used by the portrait renderer.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** face gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `FACE_GEAR/ITEM/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Face-gear row identity.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** face gear table data; item/profile metadata.
- **Confidence:** `inferred`

## Relations

- `Type` selects the face-gear overlay; `szFile` is an STI asset.

## Localization

`szFile` is an asset key, not localized text.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Face gear table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The renderer that consumes `Type` and `szFile` is outside this table.
