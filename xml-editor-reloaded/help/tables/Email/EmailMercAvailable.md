---
schema: 1
lang: en
family: email-merc-available
title: "EmailMercAvailable - mercenary availability mail"
tables:
  - EmailMercAvailable
table_ids:
  - Email.EmailMercAvailable
files:
  - Email/EmailMercAvailable.xml
root: EMAIL_MERC_AVAILABLE
record: EMAIL
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# EmailMercAvailable - mercenary availability mail

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`EMAIL_MERC_AVAILABLE/EMAIL/uiIndex`](#email_merc_availableemailuiindex)
  - [`EMAIL_MERC_AVAILABLE/EMAIL/Subject`](#email_merc_availableemailsubject)
  - [`EMAIL_MERC_AVAILABLE/EMAIL/Message`](#email_merc_availableemailmessage)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EmailMercAvailable.xml` contains the generated e-mail sent when an AIM mercenary becomes available again. Each row has a subject and a message; `±` in the message marks a line break.

## Tables

| Table | File | Notes |
|---|---|---|
| `EmailMercAvailable` | `Email/EmailMercAvailable.xml` | — |

## Structure

```
EMAIL_MERC_AVAILABLE
└── EMAIL
    ├── uiIndex
    ├── Subject
    └── Message
```

## Fields

### `EMAIL_MERC_AVAILABLE/EMAIL/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the e-mail template. It is an array key, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `EMAIL_MERC_AVAILABLE/EMAIL/Subject`

- **Type:** `String`
- **Description:** Subject line of the generated e-mail. It is display text and may contain placeholders.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `EMAIL_MERC_AVAILABLE/EMAIL/Message`

- **Type:** `String`
- **Description:** Body of the generated e-mail. It is display text; `±` marks a line break in the stored message.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- The template is selected by its `uiIndex` when the corresponding game event fires.

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
