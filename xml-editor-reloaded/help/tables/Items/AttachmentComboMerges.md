---
schema: 1
lang: en
family: items-attachments
title: "AttachmentComboMerges - combined attachment results"
tables:
  - AttachmentComboMerges
table_ids:
  - Items.AttachmentComboMerges
files:
  - Items/AttachmentComboMerges.xml
root: ATTACHMENTCOMBOMERGELIST
record: ATTACHMENTCOMBOMERGE
key: uiIndex
sources:
  - Attachment combo-merge table data
  - Item metadata
status: draft
updated: 2026-11-07
---

# AttachmentComboMerges - combined attachment results

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/uiIndex`](#attachmentcombomergelistattachmentcombomergeuiindex)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment1`](#attachmentcombomergelistattachmentcombomergeusattachment1)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment10`](#attachmentcombomergelistattachmentcombomergeusattachment10)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment11`](#attachmentcombomergelistattachmentcombomergeusattachment11)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment12`](#attachmentcombomergelistattachmentcombomergeusattachment12)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment13`](#attachmentcombomergelistattachmentcombomergeusattachment13)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment14`](#attachmentcombomergelistattachmentcombomergeusattachment14)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment15`](#attachmentcombomergelistattachmentcombomergeusattachment15)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment16`](#attachmentcombomergelistattachmentcombomergeusattachment16)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment17`](#attachmentcombomergelistattachmentcombomergeusattachment17)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment18`](#attachmentcombomergelistattachmentcombomergeusattachment18)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment19`](#attachmentcombomergelistattachmentcombomergeusattachment19)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment2`](#attachmentcombomergelistattachmentcombomergeusattachment2)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment20`](#attachmentcombomergelistattachmentcombomergeusattachment20)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment3`](#attachmentcombomergelistattachmentcombomergeusattachment3)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment4`](#attachmentcombomergelistattachmentcombomergeusattachment4)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment5`](#attachmentcombomergelistattachmentcombomergeusattachment5)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment6`](#attachmentcombomergelistattachmentcombomergeusattachment6)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment7`](#attachmentcombomergelistattachmentcombomergeusattachment7)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment8`](#attachmentcombomergelistattachmentcombomergeusattachment8)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment9`](#attachmentcombomergelistattachmentcombomergeusattachment9)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usItem`](#attachmentcombomergelistattachmentcombomergeusitem)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usResult`](#attachmentcombomergelistattachmentcombomergeusresult)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AttachmentComboMerges.xml` defines a base item plus up to twenty attachments and the item produced when the complete combination is present. It is the attachment-combination counterpart of `Merges`.

## Tables

| Table | File | Notes |
|---|---|---|
| `AttachmentComboMerges` | `Items/AttachmentComboMerges.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
ATTACHMENTCOMBOMERGELIST
└── ATTACHMENTCOMBOMERGE    one combination
    ├── uiIndex               row identity
    ├── usItem                base item ID
    ├── usAttachment1..20     required attachment item IDs
    └── usResult              result item ID
```

## Fields

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the combo-merge row and identity of the XML record.
- **Values:** Non-negative integer; unique within the file.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 1. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 10. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment11`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 11. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment12`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 12. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment13`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 13. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment14`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 14. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment15`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 15. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment16`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 16. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment17`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 17. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment18`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 18. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment19`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 19. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 2. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment20`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 20. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 3. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 4. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 5. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 6. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 7. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 8. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 9. Zero means the slot is not used. The row requires all non-zero slots to be present.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the base item on which the attachments are combined.
- **Values:** Item ID of the base item.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usResult`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced when all listed attachments are present on the base item.
- **Values:** Item ID of the combined result.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment combo-merge table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `usItem`, `usAttachment1..20` and `usResult` point to [Items](Items.md).
- `usAttachment1..20` are ordered slots; zero means the slot is not used.

## Localization

This table has no display strings; item names come from `Items`.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Attachment combo-merge table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

Whether attachment order matters for the result is not expressed by the table; the row only lists the required non-zero attachment IDs.
