---
schema: 1
lang: en
family: mercs-dialogue
title: "MercOpinions - mercenary opinions"
tables:
  - MercOpinions
table_ids:
  - MercOpinions
files:
  - MercOpinions.xml
root: MERCOPINIONS
record: OPINION
key: uiIndex
sources:
  - Merc opinion table data
status: draft
updated: 2026-11-07
---

# MercOpinions - mercenary opinions

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERCOPINIONS/OPINION/uiIndex`](#mercopinionsopinionuiindex)
  - [`MERCOPINIONS/OPINION/zNickname`](#mercopinionsopinionznickname)
  - [`MERCOPINIONS/OPINION/AnOpinion`](#mercopinionsopinionanopinion)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercOpinions.xml` stores the opinion text associated with a mercenary profile.

## Tables

| Table | File | Notes |
|---|---|---|
| `MercOpinions` | `MercOpinions.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
MERCOPINIONS
└── OPINION
    ├── uiIndex
    ├── zNickname
    └── AnOpinion
```

## Fields

### `MERCOPINIONS/OPINION/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Profile identity for this opinion row.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc opinion table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCOPINIONS/OPINION/zNickname`

- **Type:** `String` (`String`)
- **Description:** Nickname of the profile.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc opinion table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCOPINIONS/OPINION/AnOpinion`

- **Type:** `String` (`String`)
- **Description:** Opinion text for the profile. It may contain several lines.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc opinion table data; item/profile metadata.
- **Confidence:** `inferred`

## Relations

- `uiIndex` points to [MercProfiles](MercProfiles.md).

## Localization

`AnOpinion` is player-facing text.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Merc opinion table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The table stores opinion text only; the rules that choose which opinion is shown are in the dialogue code.
