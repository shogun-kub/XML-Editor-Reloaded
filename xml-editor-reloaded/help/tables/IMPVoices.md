---
schema: 1
lang: en
family: mercs-faces
title: "IMPVoices - IMP voice sets"
tables:
  - IMPVoices
table_ids:
  - IMPVoices
files:
  - IMPVoices.xml
root: IMPVOICELIST
record: VOICE
key: voiceset
sources:
  - IMP voice table data
status: draft
updated: 2026-11-07
---

# IMPVoices - IMP voice sets

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`IMPVOICELIST/VOICE/voiceset`](#impvoicelistvoicevoiceset)
  - [`IMPVOICELIST/VOICE/szVoiceSetName`](#impvoicelistvoiceszvoicesetname)
  - [`IMPVOICELIST/VOICE/bSex`](#impvoicelistvoicebsex)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`IMPVoices.xml` maps an IMP voice-set ID and sex to the voice-set name used by character creation.

## Tables

| Table | File | Notes |
|---|---|---|
| `IMPVoices` | `IMPVoices.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
IMPVOICELIST
└── VOICE
    ├── voiceset
    ├── szVoiceSetName
    └── bSex
```

## Fields

### `IMPVOICELIST/VOICE/voiceset`

- **Type:** `Integer` (`INT32`)
- **Description:** Voice-set ID used by the IMP voice selection.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp voice table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPVOICELIST/VOICE/szVoiceSetName`

- **Type:** `String` (`String`)
- **Description:** Display name of the voice set.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** imp voice table data; item/profile metadata.
- **Confidence:** `inferred`

### `IMPVOICELIST/VOICE/bSex`

- **Type:** `Integer` (`INT32`)
- **Description:** Sex for which the voice set is intended.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** imp voice table data; item/profile metadata.
- **Confidence:** `inferred`

## Relations

- `voiceset` is used as `MercProfiles.usVoiceIndex` and by the IMP voice selection.

## Localization

`szVoiceSetName` is a display label.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- IMP voice table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The voice files are selected by the voice-set ID; the file naming is outside this table.
