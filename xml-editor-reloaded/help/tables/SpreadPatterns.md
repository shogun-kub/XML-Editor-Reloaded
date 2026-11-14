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
status: reviewed
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

- **Type:** `String` (`CHAR8`)
- **Description:** Name of the spread pattern, used by other XML files. The name is matched case-insensitively; leading and trailing spaces are trimmed on load.
- **Values:** Free text; the loader stores at most `SPREADPATTERN_NAME_SIZE - 1` characters.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/DESC`

- **Type:** `String`
- **Description:** Informational description of the pattern. It is not read by the loader and has no runtime effect.
- **Values:** Free text; ignored at load time.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/xspread`

- **Type:** `Decimal` (`DOUBLE`)
- **Description:** Maximum horizontal random spread, in radians. The loader applies `fabs()`, so a negative value is stored as its absolute value.
- **Numeric semantics:** higher = wider spread; `0` disables horizontal randomization.
- **Values:** Non-negative radians after loading.
- **Default:** Absent; the pattern starts at `0.0`.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/yspread`

- **Type:** `Decimal` (`DOUBLE`)
- **Description:** Maximum vertical random spread, in radians. The loader applies `fabs()`, so a negative value is stored as its absolute value.
- **Numeric semantics:** higher = wider spread; `0` disables vertical randomization.
- **Values:** Non-negative radians after loading.
- **Default:** Absent; the pattern starts at `0.0`.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/method`

- **Type:** `String`
- **Description:** Distribution shape for random pellets. The loader matches this value **case-sensitively** against `RECTANGLE`, `DIAMOND` and `ELLIPSE`; an unmatched value leaves the default `ELLIPSE`.
- **Numeric semantics:** Enum-like text; case and spelling must match the uppercase loader names exactly.
- **Values:** `RECTANGLE`, `DIAMOND` or `ELLIPSE`. The XML comment's `Rectangle`/`Diamond`/`Ellipse` spellings do not match and fall back to the default.
- **Default:** Absent; the loader keeps `ELLIPSE` (`giSpreadPatternMethod_Default`).
- **Source:** game code (`XML_SpreadPatterns.cpp`, `LOS.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/p/x`

- **Type:** `Decimal` (`DOUBLE`)
- **Description:** Horizontal angle of one explicit pellet, in radians. Positive values deviate one way, negative the other; it is signed.
- **Values:** Signed decimal angle.
- **Default:** Absent; the pellet entry starts at `0.0`.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/p/y`

- **Type:** `Decimal` (`DOUBLE`)
- **Description:** Vertical angle of one explicit pellet, in radians. Positive values deviate one way, negative the other; it is signed.
- **Values:** Signed decimal angle.
- **Default:** Absent; the pellet entry starts at `0.0`.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

## Relations

- `NAME` is referenced by [AmmoTypes](Items/AmmoTypes.md), [Weapons](Items/Weapons.md) and [Items](Items/Items.md) when they select a spread pattern.

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
