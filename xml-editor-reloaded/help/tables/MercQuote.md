---
schema: 1
lang: en
family: mercs-dialogue
title: "MercQuote - quote event indices"
tables:
  - MercQuote
table_ids:
  - MercQuote
files:
  - MercQuote.xml
root: QARRAY
record: PROFILE
key: uiIndex
sources:
  - Merc quote table data
status: draft
updated: 2026-11-07
---

# MercQuote - quote event indices

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`QARRAY/PROFILE/uiIndex`](#qarrayprofileuiindex)
  - [`QARRAY/PROFILE/Name`](#qarrayprofilename)
  - [`QARRAY/PROFILE/QuoteExpHeadShotOnly`](#qarrayprofilequoteexpheadshotonly)
  - [`QARRAY/PROFILE/QuoteExpTeamSpecific`](#qarrayprofilequoteexpteamspecific)
  - [`QARRAY/PROFILE/QuoteExpGenderCode`](#qarrayprofilequoteexpgendercode)
  - [`QARRAY/PROFILE/QuoteExpGotGunOrUsedGun`](#qarrayprofilequoteexpgotgunorusedgun)
  - [`QARRAY/PROFILE/QuoteExpPassingDislike`](#qarrayprofilequoteexppassingdislike)
  - [`QARRAY/PROFILE/QuoteExpWitnessQueenBugDeath`](#qarrayprofilequoteexpwitnessqueenbugdeath)
  - [`QARRAY/PROFILE/QuoteExpWitnessDeidrannaDeath`](#qarrayprofilequoteexpwitnessdeidrannadeath)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercQuote.xml` maps a profile to the quote-event indices used by dialogue: gender, gun use, head shots, passing dislike, team-specific lines and witness reactions.

## Tables

| Table | File | Notes |
|---|---|---|
| `MercQuote` | `MercQuote.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
QARRAY
└── PROFILE
    ├── uiIndex
    ├── Name
    ├── QuoteExpHeadShotOnly
    ├── QuoteExpTeamSpecific
    ├── QuoteExpGenderCode
    ├── QuoteExpGotGunOrUsedGun
    ├── QuoteExpPassingDislike
    ├── QuoteExpWitnessQueenBugDeath
    └── QuoteExpWitnessDeidrannaDeath
```

## Fields

### `QARRAY/PROFILE/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Profile identity for this quote row.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc quote table data; item/profile metadata.
- **Confidence:** `inferred`

### `QARRAY/PROFILE/Name`

- **Type:** `String` (`String`)
- **Description:** Profile name used by the quote table.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc quote table data; item/profile metadata.
- **Confidence:** `inferred`

### `QARRAY/PROFILE/QuoteExpHeadShotOnly`

- **Type:** `Integer` (`INT32`)
- **Description:** Quote event index for a head-shot-only situation.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc quote table data; item/profile metadata.
- **Confidence:** `inferred`

### `QARRAY/PROFILE/QuoteExpTeamSpecific`

- **Type:** `Integer` (`INT32`)
- **Description:** Quote event index for team-specific dialogue.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc quote table data; item/profile metadata.
- **Confidence:** `inferred`

### `QARRAY/PROFILE/QuoteExpGenderCode`

- **Type:** `Integer` (`INT32`)
- **Description:** Gender code used when selecting the quote.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc quote table data; item/profile metadata.
- **Confidence:** `inferred`

### `QARRAY/PROFILE/QuoteExpGotGunOrUsedGun`

- **Type:** `Integer` (`INT32`)
- **Description:** Quote event index for getting or using a gun.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc quote table data; item/profile metadata.
- **Confidence:** `inferred`

### `QARRAY/PROFILE/QuoteExpPassingDislike`

- **Type:** `Integer` (`INT32`)
- **Description:** Quote event index for passing a disliked character.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc quote table data; item/profile metadata.
- **Confidence:** `inferred`

### `QARRAY/PROFILE/QuoteExpWitnessQueenBugDeath`

- **Type:** `Integer` (`INT32`)
- **Description:** Quote event index for witnessing the queen bug’s death.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc quote table data; item/profile metadata.
- **Confidence:** `inferred`

### `QARRAY/PROFILE/QuoteExpWitnessDeidrannaDeath`

- **Type:** `Integer` (`INT32`)
- **Description:** Quote event index for witnessing Deidranna’s death.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc quote table data; item/profile metadata.
- **Confidence:** `inferred`

## Relations

- `uiIndex` points to [MercProfiles](MercProfiles.md).
- The quote-expression indices are resolved by the dialogue system, not by another XML table.

## Localization

`Name` is a profile label; quote text itself is stored in the dialogue files.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Merc quote table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The quote-expression indices map to external dialogue files; the table does not contain the quote text.
