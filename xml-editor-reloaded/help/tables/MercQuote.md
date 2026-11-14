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
status: reviewed
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

- **Type:** `Integer` (`UINT16`)
- **Description:** Profile identity for this quote row.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/Name`

- **Type:** `String` (`String`)
- **Description:** Mercenary name stored in the table, but the loader does not read this element; it is ignored.
- **Values:** Free text; ignored at load time.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpHeadShotOnly`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Quote event index for a head-shot-only situation.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpTeamSpecific`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Quote event index for team-specific dialogue.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpGenderCode`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Gender code used when selecting the quote.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpGotGunOrUsedGun`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Quote event index for getting or using a gun.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpPassingDislike`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Quote event index for passing a disliked character.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpWitnessQueenBugDeath`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Quote event index for witnessing the queen bug’s death.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpWitnessDeidrannaDeath`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Quote event index for witnessing Deidranna’s death.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

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
