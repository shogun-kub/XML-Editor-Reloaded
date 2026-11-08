---
schema: 1
lang: en
family: items-attachments
title: "AttachmentInfo - attachment skill checks"
tables:
  - AttachmentInfo
table_ids:
  - Items.AttachmentInfo
files:
  - Items/AttachmentInfo.xml
root: ATTACHMENTINFOLIST
record: ATTACHMENTINFO
key: uiIndex
sources:
  - Attachment info table data
  - Item and skill lookup tables
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# AttachmentInfo - attachment skill checks

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/uiIndex`](#attachmentinfolistattachmentinfouiindex)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/usItem`](#attachmentinfolistattachmentinfousitem)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/uiItemClass`](#attachmentinfolistattachmentinfouiitemclass)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/bAttachmentSkillCheck`](#attachmentinfolistattachmentinfobattachmentskillcheck)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/bAttachmentSkillCheckMod`](#attachmentinfolistattachmentinfobattachmentskillcheckmod)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AttachmentInfo.xml` adds attachment-specific metadata: which item class an attachment can attach to and which skill check (with modifier) is required.

## Tables

| Table | File | Notes |
|---|---|---|
| `AttachmentInfo` | `Items/AttachmentInfo.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
ATTACHMENTINFOLIST
└── ATTACHMENTINFO
    ├── uiIndex
    ├── usItem
    ├── uiItemClass
    ├── bAttachmentSkillCheck
    └── bAttachmentSkillCheckMod
```

## Fields

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the attachment-information row and identity of the XML record.
- **Values:** Non-negative integer; unique within the file.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment info table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/usItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the attachment described by this row.
- **Values:** Item ID in [Items](Items.md); the item must be an attachment.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment info table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/uiItemClass`

- **Type:** `Integer` (`UINT32`)
- **Description:** Bitmask of item classes this attachment can attach to. The host item class must intersect this mask.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bitmask of `Lookup.ItemClass` IDs. The attachment can be mounted on items whose class intersects this mask.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment info table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/bAttachmentSkillCheck`

- **Type:** `Integer` (`UINT8`)
- **Description:** Skill check used when attaching the item. `None` disables the check.
- **Values:** IDs from `Lookup.SkillCheckType`: `0` None, `1` Lockpick, `2` Elec. Lockpick, `3` Attach Timed Detonator, `4` Attach Remote Detonator, `5` Plant Timed Bomb, `6` Plant Remote Bomb, `7` Open With Crowbar, `8` Smash Door, `9` Disarm Trap, `10` Unjam Gun, `11` Notice Dart, `12` Lie to Queen, `13` Attach Special Item, `14` Attach Special Elec. Item, `15` Disarm Elec. Trap.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment info table data; field metadata.
- **Confidence:** `source`

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/bAttachmentSkillCheckMod`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier added to the selected attachment skill check.
- **Numeric semantics:** Signed modifier added to the skill check: positive values make the check easier, negative values make it harder.
- **Values:** Integer modifier added to the selected skill check.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment info table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `usItem` points to [Items](Items.md).
- `uiItemClass` is a bitmask of [Lookup.ItemClass](../Lookup.md#itemclass).
- `bAttachmentSkillCheck` uses [Lookup.SkillCheckType](../Lookup.md#skillchecktype).

## Localization

This table has no display strings.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Attachment info table data.
- Item and skill lookup tables.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The sign convention of `bAttachmentSkillCheckMod` depends on the skill-check implementation; the table only stores the integer modifier.
