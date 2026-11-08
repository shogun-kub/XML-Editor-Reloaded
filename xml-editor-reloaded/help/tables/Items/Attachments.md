---
schema: 1
lang: en
family: items-attachments
title: "Attachments - item attachment mappings"
tables:
  - Attachments
table_ids:
  - Items.Attachments
files:
  - Items/Attachments.xml
root: ATTACHMENTLIST
record: ATTACHMENT
key: attachmentIndex + itemIndex
sources:
  - Attachment table data
  - Item metadata
  - Attachment-system field names
status: reviewed
updated: 2026-11-07
---

# Attachments - item attachment mappings

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ATTACHMENTLIST/ATTACHMENT/attachmentIndex`](#attachmentlistattachmentattachmentindex)
  - [`ATTACHMENTLIST/ATTACHMENT/itemIndex`](#attachmentlistattachmentitemindex)
  - [`ATTACHMENTLIST/ATTACHMENT/APCost`](#attachmentlistattachmentapcost)
  - [`ATTACHMENTLIST/ATTACHMENT/NASOnly`](#attachmentlistattachmentnasonly)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Attachments.xml` maps an attachment item to a host item and sets the AP cost of that mapping. The table is shared by the old attachment system (OAS) and the New Attachment System (NAS); `NASOnly` marks mappings that exist only under NAS.

## Tables

| Table | File | Notes |
|---|---|---|
| `Attachments` | `Items/Attachments.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
ATTACHMENTLIST
└── ATTACHMENT
    ├── attachmentIndex
    ├── itemIndex
    ├── APCost
    └── NASOnly
```

## Fields

### `ATTACHMENTLIST/ATTACHMENT/attachmentIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the attachment. The item must be an attachment item; this column identifies which attachment is being mapped.
- **Values:** Item ID in [Items](Items.md); the item must be an attachment.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTLIST/ATTACHMENT/itemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the host item (weapon, armour or other attachable item) that accepts the attachment.
- **Values:** Item ID in [Items](Items.md); the host item must accept the attachment.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTLIST/ATTACHMENT/APCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** AP cost charged when the attachment mapping is used. It is the cost of attaching or detaching this mapping, not the cost of firing the host item.
- **Numeric semantics:** Action-point cost; higher values mean a more expensive action, and negative values are not meaningful.
- **Values:** AP cost; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTLIST/ATTACHMENT/NASOnly`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the mapping exists only in the New Attachment System (NAS); the old attachment system (OAS) ignores it.
- **Only used with:** NAS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; the mapping is available in both attachment systems.
- **Source:** attachment table data; field metadata.
- **Confidence:** `source`

## Relations

- `attachmentIndex` and `itemIndex` point to [Items](Items.md).
- `NASOnly` is a NAS-only switch; OAS ignores the mapping when it is set.
- Attachment combinations are described by [AttachmentComboMerges](AttachmentComboMerges.md); exclusions by [IncompatibleAttachments](IncompatibleAttachments.md).

## Localization

This table has no display strings; item names come from `Items`.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Attachment table data.
- Item metadata.
- Attachment-system field names.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The exact event that charges `APCost` (attach, detach, or both) is not stated by the table; the description keeps the mapping-level meaning.
