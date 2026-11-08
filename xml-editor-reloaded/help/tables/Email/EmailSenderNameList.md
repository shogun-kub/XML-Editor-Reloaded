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
status: reviewed
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

`EmailSenderNameList.xml` is the list of display names used by generic `Emails.xml` mail. The index selects a sender name; the text is used verbatim. See [Email text formatting](../../EMAIL-TEXT.md).

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
- **Description:** Index of the sender-name entry. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `SENDER_LIST/NAME/Name`

- **Type:** `String`
- **Description:** Display name used as an e-mail sender, used verbatim. See [Email text formatting](../../EMAIL-TEXT.md).
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- The `Sender` field in `Emails.xml` references a `uiIndex` in this table.

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- None. The names are literal display text; see [Email text formatting](../../EMAIL-TEXT.md).
