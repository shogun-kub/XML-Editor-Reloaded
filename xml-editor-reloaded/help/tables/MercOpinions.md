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
status: reviewed
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
    └── AnOpinion   (repeated; attributes id and modifier)
```

## Fields

### `MERCOPINIONS/OPINION/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Profile identity for this opinion row.
- **Values:** Profile ID; the loader stores the row index as `UINT32`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (opinion loader); merc opinion table data.
- **Confidence:** `source`

### `MERCOPINIONS/OPINION/zNickname`

- **Type:** `String` (`String`)
- **Description:** Informational nickname of the profile. The loader reads the element but does not use it for gameplay.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (opinion loader ignores the value); merc opinion table data.
- **Confidence:** `source`

### `MERCOPINIONS/OPINION/AnOpinion`

- **Type:** `Element` (repeated; attributes `id` and `modifier`)
- **Description:** One opinion modifier for the profile. The element is empty; `id` selects the opinion slot (profile index) and `modifier` is the signed opinion delta.
- **Numeric semantics:** The `modifier` attribute is a signed value: positive values improve the opinion, negative values worsen it, and `0` is neutral.
- **Values:** Repeated `AnOpinion` elements; `id` `0..74`, `modifier` signed integer.
- **Default:** absent means no opinion modifier for that slot.
- **Source:** game code (`XML_Opinions.cpp` reads the `id` and `modifier` attributes); merc opinion table data.
- **Confidence:** `source`

## Relations

- `uiIndex` points to [MercProfiles](MercProfiles.md).

## Localization

`AnOpinion` contains no display text; `zNickname` is informational, and the game builds opinion strings elsewhere.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Merc opinion table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The table stores numeric opinion modifiers only; the rules that choose which opinion is shown are in the dialogue code.
