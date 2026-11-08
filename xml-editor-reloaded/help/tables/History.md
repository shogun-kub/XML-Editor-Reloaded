---
schema: 1
lang: en
family: history
title: "History - campaign history strings"
tables:
  - History
table_ids:
  - History
files:
  - History.xml
root: STRINGS
record: TEXT
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# History - campaign history strings

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`STRINGS/TEXT/uiIndex`](#stringstextuiindex)
  - [`STRINGS/TEXT/sHistory`](#stringstextshistory)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`History.xml` maps history event IDs to the text shown in the campaign history log. The index is the event ID; the text may contain `%s` placeholders.

## Tables

| Table | File | Notes |
|---|---|---|
| `History` | `History.xml` | — |

## Structure

```
STRINGS
└── TEXT
    ├── uiIndex
    └── sHistory
```

## Fields

### `STRINGS/TEXT/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the history string. It is the event ID used by the history log.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `STRINGS/TEXT/sHistory`

- **Type:** `String`
- **Description:** Text written to the history log for this event. `%s` placeholders are filled with mercenary, town or mine names by the game.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `%s` arguments are filled with names from [MercProfiles](MercProfiles.md), [Cities](Map/Cities.md) and [Minerals](Map/Minerals.md), depending on the event.

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
