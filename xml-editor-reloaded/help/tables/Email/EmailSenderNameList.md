---
schema: 1
lang: en
family: email-sender-names
title: "EmailSenderNameList - e-mail sender names"
tables:
  - EmailSenderNameList
table_ids:
  - Email.EmailSenderNameList
files:
  - Email/EmailSenderNameList.xml
root: SENDER_LIST
record: NAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# EmailSenderNameList - e-mail sender names

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SENDER_LIST/NAME/uiIndex`](#sender_listnameuiindex)
  - [`SENDER_LIST/NAME/Name`](#sender_listnamename)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EmailSenderNameList.xml` is the list of display names used by the e-mail system. The index selects a sender name; the text is localized.

## Tables

| Table | File | Notes |
|---|---|---|
| `EmailSenderNameList` | `Email/EmailSenderNameList.xml` | — |

## Structure

```
SENDER_LIST
└── NAME
    ├── uiIndex
    └── Name
```

## Fields

### `SENDER_LIST/NAME/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the sender-name entry. It is an array key, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SENDER_LIST/NAME/Name`

- **Type:** `String`
- **Description:** Display name used as an e-mail sender. It is display text.
- **Values:** Free text or a symbolic code.
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
