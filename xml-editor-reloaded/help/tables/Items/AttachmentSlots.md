---
schema: 1
lang: en
family: items-attachments
title: "AttachmentSlots - NAS attachment slots"
tables:
  - AttachmentSlots
table_ids:
  - Items.AttachmentSlots
files:
  - Items/AttachmentSlots.xml
root: ATTACHMENTSLOTLIST
record: ATTACHMENTSLOT
key: uiSlotIndex
sources:
  - Attachment slot table data
  - NAS lookup tables
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# AttachmentSlots - NAS attachment slots

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/uiSlotIndex`](#attachmentslotlistattachmentslotuislotindex)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/szSlotName`](#attachmentslotlistattachmentslotszslotname)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasAttachmentClass`](#attachmentslotlistattachmentslotnasattachmentclass)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasLayoutClass`](#attachmentslotlistattachmentslotnaslayoutclass)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosX`](#attachmentslotlistattachmentslotusdescpanelposx)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosY`](#attachmentslotlistattachmentslotusdescpanelposy)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fMultiShot`](#attachmentslotlistattachmentslotfmultishot)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fBigSlot`](#attachmentslotlistattachmentslotfbigslot)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/ubPocketMapping`](#attachmentslotlistattachmentslotubpocketmapping)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AttachmentSlots.xml` defines the slots used by the New Attachment System: their names, accepted attachment classes, layout classes, panel positions and pocket mapping.

## Tables

| Table | File | Notes |
|---|---|---|
| `AttachmentSlots` | `Items/AttachmentSlots.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
ATTACHMENTSLOTLIST
└── ATTACHMENTSLOT
    ├── uiSlotIndex
    ├── szSlotName
    ├── nasAttachmentClass
    ├── nasLayoutClass
    ├── usDescPanelPosX
    ├── usDescPanelPosY
    ├── fMultiShot
    ├── fBigSlot
    └── ubPocketMapping
```

## Fields

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/uiSlotIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Identity of the attachment slot. Slots are referenced by the NAS attachment layout and by the slot presentation code.
- **Only used with:** NAS.
- **Values:** Non-negative slot index; unique within the file.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment slot table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/szSlotName`

- **Type:** `String` (`String`)
- **Description:** Display name of the slot in the attachment interface.
- **Only used with:** NAS.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** attachment slot table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasAttachmentClass`

- **Type:** `Flags` (`UINT64`)
- **Description:** Bitmask of attachment classes that the slot accepts. It is read by the New Attachment System (NAS).
- **Only used with:** NAS.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bitmask of IDs from `Lookup.NasAttachmentClass`: `1` Default, `2` Barrel, `4` Laser, `8` Sight, `16` Scope, `32` Stock, `64` Ammo, `128` Internal, `256` External, `512` Underbarrel, `1024` Grenade, `2048` Rocket, `4096` MOLLE Small, `8192` MOLLE Medium, `16384` Batteries. Combine bits with OR.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment slot table data; field metadata.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasLayoutClass`

- **Type:** `Flags` (`UINT64`)
- **Description:** Bitmask of layout classes to which the slot belongs. It is read by the New Attachment System (NAS).
- **Only used with:** NAS.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bitmask of IDs from `Lookup.NasLayoutClass`: `1` Weapons, `2` Multi-Shot Launchers, `4` Molle Leg Rigs, `8` Molle Vests, `16` Molle Combat Packs, `32` Molle Backpacks. Combine bits with OR.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment slot table data; field metadata.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosX`

- **Type:** `Integer` (`UINT16`)
- **Description:** Horizontal position of the slot icon in the attachment description panel.
- **Only used with:** NAS.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment slot table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosY`

- **Type:** `Integer` (`UINT16`)
- **Description:** Vertical position of the slot icon in the attachment description panel.
- **Only used with:** NAS.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment slot table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fMultiShot`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Marks the slot as a multi-shot layout slot, used by the NAS presentation for multi-shot launchers.
- **Only used with:** NAS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment slot table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fBigSlot`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Marks the slot as a big slot, using the larger slot presentation.
- **Only used with:** NAS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment slot table data; field metadata.
- **Confidence:** `inferred`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/ubPocketMapping`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket index used by the NAS slot layout. The value maps the attachment slot to an inventory pocket.
- **Only used with:** NAS.
- **Values:** Pocket index used by the NAS layout; no fixed range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** attachment slot table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `nasAttachmentClass` uses [Lookup.NasAttachmentClass](../Lookup.md#nasattachmentclass).
- `nasLayoutClass` uses [Lookup.NasLayoutClass](../Lookup.md#naslayoutclass).
- Slots are referenced by the NAS layout, not by the `Attachments` mapping table.

## Localization

`szSlotName` is shown in the attachment interface.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Attachment slot table data.
- NAS lookup tables.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The exact pixel origin of `usDescPanelPosX/Y` and the use of `fMultiShot` in the NAS layout are presentation details; the field meanings are documented at the table level.
