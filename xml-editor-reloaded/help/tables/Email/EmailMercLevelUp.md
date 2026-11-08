---
schema: 1
lang: en
family: email-merc-level-up
title: "EmailMercLevelUp - mercenary level-up mail"
tables:
  - EmailMercLevelUp
table_ids:
  - Email.EmailMercLevelUp
files:
  - Email/EmailMercLevelUp.xml
root: EMAIL_MERC_LEVEL_UP
record: EMAIL
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# EmailMercLevelUp - mercenary level-up mail

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`EMAIL_MERC_LEVEL_UP/EMAIL/uiIndex`](#email_merc_level_upemailuiindex)
  - [`EMAIL_MERC_LEVEL_UP/EMAIL/Subject`](#email_merc_level_upemailsubject)
  - [`EMAIL_MERC_LEVEL_UP/EMAIL/Message`](#email_merc_level_upemailmessage)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EmailMercLevelUp.xml` contains the generated e-mail that Speck sends when a M.E.R.C mercenary gains a level. Each row has a subject and a message. Both are literal display text; this table does not substitute placeholders. See [Email text formatting](../../EMAIL-TEXT.md).

## Tables

| Table | File | Notes |
|---|---|---|
| `EmailMercLevelUp` | `Email/EmailMercLevelUp.xml` | — |

## Structure

```
EMAIL_MERC_LEVEL_UP
└── EMAIL
    ├── uiIndex
    ├── Subject
    └── Message
```

## Fields

### `EMAIL_MERC_LEVEL_UP/EMAIL/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the e-mail template. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `EMAIL_MERC_LEVEL_UP/EMAIL/Subject`

- **Type:** `String`
- **Description:** Subject line of the generated e-mail, used as literal text. See [Email text formatting](../../EMAIL-TEXT.md).
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `EMAIL_MERC_LEVEL_UP/EMAIL/Message`

- **Type:** `String`
- **Description:** Body of the generated e-mail. `±` marks a line break in the stored message. See [Email text formatting](../../EMAIL-TEXT.md).
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- The template is selected by its `uiIndex` when the corresponding game event fires.
- The sender display name comes from [MercProfiles](../MercProfiles.md) `zNickname` for the same profile index; it is not stored in this table.

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- None. The subject and message are used literally; see [Email text formatting](../../EMAIL-TEXT.md).
