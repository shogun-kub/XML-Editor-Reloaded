---
schema: 1
lang: en
family: mercs-faces
title: "IMPPortraits - IMP portrait defaults"
tables:
  - IMPPortraits
table_ids:
  - IMPPortraits
files:
  - IMPPortraits.xml
root: IMPPORTRAITSLIST
record: IMP
key: uiIndex
sources:
  - IMP portrait table data
status: reviewed
updated: 2026-11-07
---

# IMPPortraits - IMP portrait defaults

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`IMPPORTRAITSLIST/IMP/uiIndex`](#impportraitslistimpuiindex)
  - [`IMPPORTRAITSLIST/IMP/PortraitId`](#impportraitslistimpportraitid)
  - [`IMPPORTRAITSLIST/IMP/bSex`](#impportraitslistimpbsex)
  - [`IMPPORTRAITSLIST/IMP/usEyesX`](#impportraitslistimpuseyesx)
  - [`IMPPORTRAITSLIST/IMP/usEyesY`](#impportraitslistimpuseyesy)
  - [`IMPPORTRAITSLIST/IMP/usMouthX`](#impportraitslistimpusmouthx)
  - [`IMPPORTRAITSLIST/IMP/usMouthY`](#impportraitslistimpusmouthy)
  - [`IMPPORTRAITSLIST/IMP/DefaultSkin`](#impportraitslistimpdefaultskin)
  - [`IMPPORTRAITSLIST/IMP/DefaultHair`](#impportraitslistimpdefaulthair)
  - [`IMPPORTRAITSLIST/IMP/DefaultShirt`](#impportraitslistimpdefaultshirt)
  - [`IMPPORTRAITSLIST/IMP/DefaultPants`](#impportraitslistimpdefaultpants)
  - [`IMPPORTRAITSLIST/IMP/DefaultBigBody`](#impportraitslistimpdefaultbigbody)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`IMPPortraits.xml` defines the IMP portrait list and its default body, hair, pants, shirt and skin palettes plus the eye/mouth coordinates.

## Tables

| Table | File | Notes |
|---|---|---|
| `IMPPortraits` | `IMPPortraits.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
IMPPORTRAITSLIST
└── IMP
    ├── uiIndex
    ├── PortraitId
    ├── bSex
    ├── usEyesX
    ├── usEyesY
    ├── usMouthX
    ├── usMouthY
    ├── DefaultSkin
    ├── DefaultHair
    ├── DefaultShirt
    ├── DefaultPants
    └── DefaultBigBody
```

## Fields

### `IMPPORTRAITSLIST/IMP/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** IMP portrait row identity.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/PortraitId`

- **Type:** `Integer` (`INT32`)
- **Description:** Portrait ID used by the IMP portrait list.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/bSex`

- **Type:** `Integer` (`INT32`)
- **Description:** Sex of the portrait.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/usEyesX`

- **Type:** `Integer` (`INT32`)
- **Description:** X coordinate of the eyes. Numeric semantics: Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/usEyesY`

- **Type:** `Integer` (`INT32`)
- **Description:** Y coordinate of the eyes. Numeric semantics: Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/usMouthX`

- **Type:** `Integer` (`INT32`)
- **Description:** X coordinate of the mouth. Numeric semantics: Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/usMouthY`

- **Type:** `Integer` (`INT32`)
- **Description:** Y coordinate of the mouth. Numeric semantics: Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/DefaultSkin`

- **Type:** `Integer` (`INT32`)
- **Description:** Default skin palette index.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/DefaultHair`

- **Type:** `Integer` (`INT32`)
- **Description:** Default hair palette index.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/DefaultShirt`

- **Type:** `Integer` (`INT32`)
- **Description:** Default shirt palette index.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/DefaultPants`

- **Type:** `Integer` (`INT32`)
- **Description:** Default pants palette index.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPPORTRAITSLIST/IMP/DefaultBigBody`

- **Type:** `Integer` (`INT32`)
- **Description:** Default big-body type.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp portrait table data; item/profile metadata.
- **Confidence:** `inferred`

## Relations

- `PortraitId` points to the portrait assets; the `Default*` fields select palettes.

## Localization

No display strings; palette and coordinate fields are numeric.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- IMP portrait table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The portrait renderer and palette lookup are outside this table.
