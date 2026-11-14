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
updated: 2026-11-08
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

`uiSlotIndex` is the key: the loader stores each row at that array index. NAS slot lookups start
at index `1` and stop at the first index whose `uiSlotIndex` is `0`, so slot indexes must be
contiguous from `1` for later slots to be reachable.

## Fields

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/uiSlotIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Identity of the attachment slot. Rows are stored at this index, and the slot presentation and layout code refers to the slot by this value.
- **Only used with:** NAS.
- **Numeric semantics:** Array index. The loader parses the text with `strtoul` and stores the result in a `UINT16` member; values outside the `MAXITEMS` bound are discarded, and duplicate indexes overwrite earlier rows. The slot search starts at `1` and stops at the first index whose `uiSlotIndex` is `0`, so index `0` is not offered as a slot.
- **Values:** `0..16000` (`MAXITEMS` bound); the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element is parsed as `0`, so the row is stored at index `0` and is not offered.
- **Source:** game code (attachment-slot loader; NAS slot lookup); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/szSlotName`

- **Type:** `String` (`CHAR16`)
- **Description:** Display name of the slot, used as the fast-help (tooltip) text for an empty attachment slot in the item description.
- **Only used with:** NAS.
- **Values:** Free text, converted from UTF-8; stored in a 200-character buffer, and longer text is truncated.
- **Default:** Empty string when omitted.
- **Source:** game code (attachment-slot loader; item-description fast help); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasAttachmentClass`

- **Type:** `Flags` (`UINT64`)
- **Description:** Bitmask of attachment classes that the slot accepts. The NAS slot search tests it with a bitwise AND against each attachment-class bit that the item accepts.
- **Only used with:** NAS.
- **Numeric semantics:** Bitmask; individual bits carry the meaning, so a larger number is not simply “more”. `0` means None.
- **Values:** Bitmask of IDs from `Lookup.NasAttachmentClass`: `0` None, `1` Default, `2` Barrel, `4` Laser, `8` Sight, `16` Scope, `32` Stock, `64` Ammo, `128` Internal, `256` External, `512` Underbarrel, `1024` Grenade, `2048` Rocket, `4096` MOLLE Small, `8192` MOLLE Medium, `16384` Batteries. Combine bits with OR.
- **Default:** `0` (None) when omitted; an empty element is parsed as `0`.
- **Source:** game code (attachment-slot loader; NAS slot selection); `Lookup.NasAttachmentClass`; attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasLayoutClass`

- **Type:** `Flags` (`UINT64`)
- **Description:** Bitmask of layout classes to which the slot belongs. The NAS slot search tests it with a bitwise AND against the item's layout class. If the item has no slot for the specific layout, a slot with layout class `1` (Weapons) is used as the default fallback.
- **Only used with:** NAS.
- **Numeric semantics:** Bitmask; individual bits carry the meaning, so a larger number is not simply “more”. `0` means None.
- **Values:** Bitmask of IDs from `Lookup.NasLayoutClass`: `0` None, `1` Weapons, `2` Multi-Shot Launchers, `4` Molle Leg Rigs, `8` Molle Vests, `16` Molle Combat Packs, `32` Molle Backpacks. Combine bits with OR.
- **Default:** `0` (None) when omitted; an empty element is parsed as `0`.
- **Source:** game code (attachment-slot loader; NAS slot selection and default-layout fallback); `Lookup.NasLayoutClass`; attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosX`

- **Type:** `Integer` (`UINT16`)
- **Description:** Horizontal offset in pixels of the slot icon, relative to the item-description panel origin. The icon is drawn at the panel X plus this value minus `6`; a value above `300` suppresses drawing of the slot.
- **Only used with:** NAS.
- **Numeric semantics:** Pixel offset; larger values move right. Values `0..300` are drawn.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (attachment-slot loader; item-description slot drawing); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosY`

- **Type:** `Integer` (`UINT16`)
- **Description:** Vertical offset in pixels of the slot icon, relative to the item-description panel origin. The icon is drawn at the panel Y plus this value minus `1`; a value above `300` suppresses drawing of the slot.
- **Only used with:** NAS.
- **Numeric semantics:** Pixel offset; larger values move down. Values `0..300` are drawn.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (attachment-slot loader; item-description slot drawing); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fMultiShot`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Marks the slot as one magazine or tube of a multi-shot launcher. When the slot list is built for a grenade or rocket launcher, each such slot consumes one unit of the weapon's magazine capacity, and slots are skipped once the capacity is exhausted.
- **Only used with:** NAS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (attachment-slot loader; NAS slot generation for multi-shot launchers); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fBigSlot`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Marks the slot as a big slot; the item-description panel uses the big-slot graphic and the wide slot width for it.
- **Only used with:** NAS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (attachment-slot loader; item-description slot layout); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/ubPocketMapping`

- **Type:** `Integer` (`UINT8`)
- **Description:** For LBE items, the 1-based pocket index into the item's pocket-availability mask; the slot is offered only when that pocket is available. `0` means no pocket mapping, so the slot is not filtered by pocket availability.
- **Only used with:** NAS.
- **Numeric semantics:** `0` = none; `1..N` selects bit `1 << (value - 1)` in the LBE pocket mask.
- **Values:** `0..255`; the loader stores this field as `UINT8` (the usable range is the LBE pocket count).
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (attachment-slot loader; NAS LBE slot filtering); attachment slot table data.
- **Confidence:** `source`

## Relations

- `nasAttachmentClass` uses [Lookup.NasAttachmentClass](../Lookup.md#nasattachmentclass).
- `nasLayoutClass` uses [Lookup.NasLayoutClass](../Lookup.md#naslayoutclass).
- Slots are referenced by the NAS layout, not by the [Attachments](Attachments.md) mapping table.

## Localization

`szSlotName` is shown as the fast-help text for an empty attachment slot. The loader has a
localized pass that copies only `szSlotName` into the existing slot records, so a language
overlay changes the slot names and nothing else.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Attachment slot table data.
- NAS lookup tables.
- Game code: attachment-slot loader, NAS slot selection, item-description slot drawing.
- Descriptions cover the values the loader stores and the consumers that use them; every field in this document is backed by the loader.

## Open questions

The panel origin and slot dimensions (`gsInvDescX`/`gsInvDescY`, slot widths) are interface
constants, not table data.
