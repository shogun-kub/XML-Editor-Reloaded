---
schema: 1
lang: en
family: mercs-dialogue
title: "OldAIMArchive - legacy AIM biographies"
tables:
  - OldAIMArchive
table_ids:
  - OldAIMArchive
files:
  - OldAIMArchive.xml
root: OLD_MERC
record: MERC
key: uiIndex
sources:
  - Old AIM archive table data
status: draft
updated: 2026-11-07
---

# OldAIMArchive - legacy AIM biographies

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`OLD_MERC/MERC/uiIndex`](#old_mercmercuiindex)
  - [`OLD_MERC/MERC/NickName`](#old_mercmercnickname)
  - [`OLD_MERC/MERC/Name`](#old_mercmercname)
  - [`OLD_MERC/MERC/Bio`](#old_mercmercbio)
  - [`OLD_MERC/MERC/FaceID`](#old_mercmercfaceid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`OldAIMArchive.xml` keeps the legacy AIM mercenary biographies and face IDs used by the archive/import path.

## Tables

| Table | File | Notes |
|---|---|---|
| `OldAIMArchive` | `OldAIMArchive.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
OLD_MERC
└── MERC
    ├── uiIndex
    ├── NickName
    ├── Name
    ├── Bio
    └── FaceID
```

## Fields

### `OLD_MERC/MERC/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Archive row identity.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `inferred`

### `OLD_MERC/MERC/NickName`

- **Type:** `String` (`String`)
- **Description:** Nickname of the archived mercenary.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `inferred`

### `OLD_MERC/MERC/Name`

- **Type:** `String` (`String`)
- **Description:** Full name of the archived mercenary.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `inferred`

### `OLD_MERC/MERC/Bio`

- **Type:** `String` (`String`)
- **Description:** Biography text for the archived mercenary.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `inferred`

### `OLD_MERC/MERC/FaceID`

- **Type:** `Integer` (`INT32`)
- **Description:** Face index for the archived mercenary.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `inferred`

## Relations

- `uiIndex` is the archive identity; `FaceID` points to the face assets.

## Localization

`Name`, `NickName` and `Bio` are display text.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Old AIM archive table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The archive table is a legacy migration source; the active AIM data lives in the profile tables.
