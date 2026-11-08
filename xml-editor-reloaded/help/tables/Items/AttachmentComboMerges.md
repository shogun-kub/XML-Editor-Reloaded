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
  - Game code (combo-merge loader and merge application)
  - Attachment combo-merge table data
  - Item metadata
status: reviewed
updated: 2026-11-08
---

# AttachmentComboMerges - combined attachment results

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/uiIndex`](#attachmentcombomergelistattachmentcombomergeuiindex)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usItem`](#attachmentcombomergelistattachmentcombomergeusitem)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment1`](#attachmentcombomergelistattachmentcombomergeusattachment1)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment2`](#attachmentcombomergelistattachmentcombomergeusattachment2)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment3`](#attachmentcombomergelistattachmentcombomergeusattachment3)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment4`](#attachmentcombomergelistattachmentcombomergeusattachment4)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment5`](#attachmentcombomergelistattachmentcombomergeusattachment5)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment6`](#attachmentcombomergelistattachmentcombomergeusattachment6)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment7`](#attachmentcombomergelistattachmentcombomergeusattachment7)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment8`](#attachmentcombomergelistattachmentcombomergeusattachment8)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment9`](#attachmentcombomergelistattachmentcombomergeusattachment9)
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
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment20`](#attachmentcombomergelistattachmentcombomergeusattachment20)
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
└── ATTACHMENTCOMBOMERGE
    ├── uiIndex
    ├── usItem
    ├── usAttachment1
    ├── usAttachment2
    ├── usAttachment3
    ├── usAttachment4
    ├── usAttachment5
    ├── usAttachment6
    ├── usAttachment7
    ├── usAttachment8
    ├── usAttachment9
    ├── usAttachment10
    ├── usAttachment11
    ├── usAttachment12
    ├── usAttachment13
    ├── usAttachment14
    ├── usAttachment15
    ├── usAttachment16
    ├── usAttachment17
    ├── usAttachment18
    ├── usAttachment19
    ├── usAttachment20
    └── usResult
```

## Fields

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the combo-merge row. The loader stores the row in `AttachmentComboMerge[uiIndex]`.
- **Values:** Non-negative integer below `MAXITEMS` (`16001`); rows at or above it are discarded, duplicates overwrite the earlier row, and the lookup stops at the first empty row, so indices should be contiguous from `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the base item on which the attachments are combined. A row with item ID `0` is treated as the end of the table.
- **Values:** Item ID of the base item; `0` terminates the lookup.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 1. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 2. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 3. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 4. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 5. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 6. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 7. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 8. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 9. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 10. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment11`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 11. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment12`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 12. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment13`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 13. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment14`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 14. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment15`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 15. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment16`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 16. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment17`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 17. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment18`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 18. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment19`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 19. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment20`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of attachment slot 20. Zero means the slot is not used. The loader fills the 20 slots in document order, ignoring the digit in the element name, and the row requires every non-zero slot to be present.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usResult`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced when all listed attachments are present on the base item. The merge is applied only if this item is legal (`ItemIsLegal`).
- **Values:** Item ID of the combined result.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

## Relations

- `usItem`, `usAttachment1..20` and `usResult` point to [Items](Items.md).
- Attachment order does not matter: every non-zero slot is searched by item ID, and the row matches when all of them are attached.
- Rows are evaluated in ascending index order and the first matching row wins; the scan stops at the first row whose `usItem` is `0`.
- The 20 attachment slots are filled in document order: the loader ignores the digit in the element name and does not bound-check the slot counter.
- `usResult` must be a legal item, otherwise the row is skipped.

## Localization

This table has no display strings; item names come from `Items`.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Game code: `XML_ComboMergeInfo.cpp` (loader), `Items.cpp` (`GetAttachmentComboMerge` and `PerformAttachmentComboMerge`), `Item Types.h` (`ComboMergeInfoStruct`, `MAX_DEFAULT_ATTACHMENTS`).
- Attachment combo-merge table data.
- Item metadata.
- Field types and limits: `ComboMergeInfoStruct` members and the loader casts.
- Descriptions cover the code path that consumes each field; no row counts or observed data ranges are used.

## Open questions

The loader accepts any element whose name contains `usAttachment` and never counts them, so more than 20 attachment elements overflow the row structure; the table format does not express that case.
