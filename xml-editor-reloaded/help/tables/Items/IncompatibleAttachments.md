---
schema: 1
lang: en
family: items-attachments
title: "IncompatibleAttachments - attachment exclusions"
tables:
  - IncompatibleAttachments
table_ids:
  - Items.IncompatibleAttachments
files:
  - Items/IncompatibleAttachments.xml
root: INCOMPATIBLEATTACHMENTLIST
record: INCOMPATIBLEATTACHMENT
key: itemIndex + incompatibleattachmentIndex
sources:
  - Incompatible attachment table data
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# IncompatibleAttachments - attachment exclusions

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`INCOMPATIBLEATTACHMENTLIST/INCOMPATIBLEATTACHMENT/itemIndex`](#incompatibleattachmentlistincompatibleattachmentitemindex)
  - [`INCOMPATIBLEATTACHMENTLIST/INCOMPATIBLEATTACHMENT/incompatibleattachmentIndex`](#incompatibleattachmentlistincompatibleattachmentincompatibleattachmentindex)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`IncompatibleAttachments.xml` declares pairs of attachments that cannot be mounted together. The pair is an exclusion rule, not a replacement for `AttachmentInfo` class masks.

## Tables

| Table | File | Notes |
|---|---|---|
| `IncompatibleAttachments` | `Items/IncompatibleAttachments.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
INCOMPATIBLEATTACHMENTLIST
└── INCOMPATIBLEATTACHMENT
    ├── itemIndex
    └── incompatibleattachmentIndex
```

## Fields

### `INCOMPATIBLEATTACHMENTLIST/INCOMPATIBLEATTACHMENT/itemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the attachment. The row declares that this attachment cannot be mounted together with the incompatible attachment.
- **Values:** Item ID of the attachment.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** incompatible attachment table data; field metadata.
- **Confidence:** `inferred`

### `INCOMPATIBLEATTACHMENTLIST/INCOMPATIBLEATTACHMENT/incompatibleattachmentIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the attachment that may not be mounted together with `itemIndex`.
- **Values:** Item ID of the attachment that may not be mounted together with `itemIndex`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** incompatible attachment table data; field metadata.
- **Confidence:** `inferred`

## Relations

- Both columns point to [Items](Items.md).
- The pair is an exclusion rule; the table does not add a positive compatibility relation.

## Localization

This table has no display strings.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Incompatible attachment table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The table is written as unordered pairs; it does not state whether the exclusion is automatically symmetric at runtime.
