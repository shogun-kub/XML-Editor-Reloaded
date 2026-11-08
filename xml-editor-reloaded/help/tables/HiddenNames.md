---
schema: 1
lang: en
family: hidden-names
title: "HiddenNames - hidden display names"
tables:
  - HiddenNames
table_ids:
  - HiddenNames
files:
  - HiddenNames.xml
root: HIDDENNAMES
record: NAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# HiddenNames - hidden display names

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`HIDDENNAMES/NAME/uiIndex`](#hiddennamesnameuiindex)
  - [`HIDDENNAMES/NAME/description`](#hiddennamesnamedescription)
  - [`HIDDENNAMES/NAME/Hidden`](#hiddennamesnamehidden)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`HiddenNames.xml` stores names that are hidden from normal display, keyed by `uiIndex`.

## Tables

| Table | File | Notes |
|---|---|---|
| `HiddenNames` | `HiddenNames.xml` | — |

## Structure

```
HIDDENNAMES
└── NAME
    ├── uiIndex
    ├── description
    └── Hidden
```

## Fields

### `HIDDENNAMES/NAME/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the hidden-name entry. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `HIDDENNAMES/NAME/description`

- **Type:** `String`
- **Description:** Display description/name for the hidden entry.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `HIDDENNAMES/NAME/Hidden`

- **Type:** `Boolean`
- **Description:** If set, the name is hidden from normal display. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- This table is part of the strategic army and inventory data; references are described in the field notes.

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
