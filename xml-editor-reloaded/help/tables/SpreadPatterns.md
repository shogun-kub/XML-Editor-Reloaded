---
schema: 1
lang: en
family: spread-patterns
title: "SpreadPatterns - shotgun spread patterns"
tables:
  - SpreadPatterns
table_ids:
  - SpreadPatterns
files:
  - SpreadPatterns.xml
root: SPREADPATTERNLIST
record: SPREADPATTERN
key: NAME
sources:
  - Table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# SpreadPatterns - shotgun spread patterns

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SPREADPATTERNLIST/SPREADPATTERN/NAME`](#spreadpatternlistspreadpatternname)
  - [`SPREADPATTERNLIST/SPREADPATTERN/DESC`](#spreadpatternlistspreadpatterndesc)
  - [`SPREADPATTERNLIST/SPREADPATTERN/xspread`](#spreadpatternlistspreadpatternxspread)
  - [`SPREADPATTERNLIST/SPREADPATTERN/yspread`](#spreadpatternlistspreadpatternyspread)
  - [`SPREADPATTERNLIST/SPREADPATTERN/method`](#spreadpatternlistspreadpatternmethod)
  - [`SPREADPATTERNLIST/SPREADPATTERN/p/x`](#spreadpatternlistspreadpatternpx)
  - [`SPREADPATTERNLIST/SPREADPATTERN/p/y`](#spreadpatternlistspreadpatternpy)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SpreadPatterns.xml` defines shotgun pellet patterns: explicit pellet angles plus optional random `xspread`/`yspread` ranges and a distribution shape. The first pattern is the default when no other is selected.

## Tables

| Table | File | Notes |
|---|---|---|
| `SpreadPatterns` | `SpreadPatterns.xml` | — |

## Structure

```
SPREADPATTERNLIST
└── SPREADPATTERN
    ├── NAME
    ├── DESC
    ├── xspread
    ├── yspread
    ├── method
    └── p
        ├── x
        └── y
```

## Fields

### `SPREADPATTERNLIST/SPREADPATTERN/NAME`

- **Type:** `String`
- **Description:** Name of the spread pattern, used by other XML files. The name is case-insensitive; spaces are trimmed on load.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SPREADPATTERNLIST/SPREADPATTERN/DESC`

- **Type:** `String`
- **Description:** Informational description of the pattern. It is not used by the game.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SPREADPATTERNLIST/SPREADPATTERN/xspread`

- **Type:** `Decimal`
- **Description:** Maximum horizontal random spread, in radians. Larger values widen the pellet cloud; negative values are made positive on load. Numeric semantics: higher = wider spread; negative = absolute value.
- **Values:** Decimal multiplier.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SPREADPATTERNLIST/SPREADPATTERN/yspread`

- **Type:** `Decimal`
- **Description:** Maximum vertical random spread, in radians. Larger values widen the pellet cloud; negative values are made positive on load. Numeric semantics: higher = wider spread; negative = absolute value.
- **Values:** Decimal multiplier.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SPREADPATTERNLIST/SPREADPATTERN/method`

- **Type:** `String`
- **Description:** Distribution shape for random pellets: `Rectangle`, `Diamond` or `Ellipse`. It is an enum, not a magnitude.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SPREADPATTERNLIST/SPREADPATTERN/p/x`

- **Type:** `Decimal`
- **Description:** Horizontal angle of one explicit pellet, in radians. Positive values deviate one way, negative the other; it is signed, not a magnitude.
- **Values:** Decimal multiplier.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SPREADPATTERNLIST/SPREADPATTERN/p/y`

- **Type:** `Decimal`
- **Description:** Vertical angle of one explicit pellet, in radians. Positive values deviate one way, negative the other; it is signed, not a magnitude.
- **Values:** Decimal multiplier.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `NAME` is referenced by ammo/item tables that select a spread pattern.

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for inferred fields is not stated by the table data.
