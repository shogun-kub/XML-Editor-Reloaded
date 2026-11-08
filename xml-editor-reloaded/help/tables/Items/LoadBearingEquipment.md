---
schema: 1
lang: en
family: items-pockets
title: "LoadBearingEquipment - NIV LBE layouts"
tables:
  - LoadBearingEquipment
table_ids:
  - Items.LoadBearingEquipment
files:
  - Items/LoadBearingEquipment.xml
root: LOADBEARINGEQUIPMENTLIST
record: LOADBEARINGEQUIPMENT
key: lbeIndex
sources:
  - LBE table data
  - LBEClass lookup table
  - Game code (LBE loader, item-size calculation, pocket and volume checks)
status: reviewed
updated: 2026-11-08
---

# LoadBearingEquipment - NIV LBE layouts

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeIndex`](#loadbearingequipmentlistloadbearingequipmentlbeindex)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeClass`](#loadbearingequipmentlistloadbearingequipmentlbeclass)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeCombo`](#loadbearingequipmentlistloadbearingequipmentlbecombo)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeFilledSize`](#loadbearingequipmentlistloadbearingequipmentlbefilledsize)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeAvailableVolume`](#loadbearingequipmentlistloadbearingequipmentlbeavailablevolume)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketsAvailable`](#loadbearingequipmentlistloadbearingequipmentlbepocketsavailable)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex1`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex1)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex2`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex2)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex3`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex3)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex4`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex4)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex5`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex5)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex6`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex6)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex7`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex7)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex8`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex8)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex9`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex9)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex10`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex10)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex11`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex11)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex12`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex12)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`LoadBearingEquipment.xml` defines the pocket layout of LBE items in the New Inventory System: class, volume, available pocket bitmask and the pocket row assigned to each slot.

## Tables

| Table | File | Notes |
|---|---|---|
| `LoadBearingEquipment` | `Items/LoadBearingEquipment.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
LOADBEARINGEQUIPMENTLIST
└── LOADBEARINGEQUIPMENT
    ├── lbeIndex
    ├── lbeClass
    ├── lbeCombo
    ├── lbeFilledSize
    ├── lbeAvailableVolume
    ├── lbePocketsAvailable
    ├── lbePocketIndex1
    ├── lbePocketIndex2
    ├── lbePocketIndex3
    ├── lbePocketIndex4
    ├── lbePocketIndex5
    ├── lbePocketIndex6
    ├── lbePocketIndex7
    ├── lbePocketIndex8
    ├── lbePocketIndex9
    ├── lbePocketIndex10
    ├── lbePocketIndex11
    └── lbePocketIndex12
```

## Fields

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Ordinal of the LBE row. The loader appends each record to the game's `LoadBearingEquipment` list in document order and stores this value without using it for placement, so it has to match the row's 0-based position; the game itself never reads the stored value.
- **Only used with:** NIV.
- **Values:** `0..65535`; write the rows in ascending order. `Items.ubClassIndex` selects a row by its position in that list, and the debug writer re-emits the loop counter as `lbeIndex`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (the LBE loader; the item lookup uses the row position, not this value).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeClass`

- **Type:** `Integer` (`UINT32`) — the game stores this field in a `UINT32`.
- **Description:** LBE class. Together with the item class it decides which body slot the piece occupies (thigh, vest, combat pack, backpack).
- **Only used with:** NIV.
- **Values:** The game enum `eLBE_CLASS` is `1` `THIGH_PACK`, `2` `VEST_PACK`, `3` `COMBAT_PACK`, `4` `BACKPACK`, `5` `LBE_POCKET`, `6` `OTHER_POCKET`. `Lookup.LBEClass` names `0` Nothing through `4` Backpack; the shipped table only uses `1..4`, and `0` is not a class the slot checks recognise.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (the LBE class enum; per-slot class checks); `Lookup.LBEClass`.
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeCombo`

- **Type:** `Integer` (`UINT32`) — parsed with a base-0 conversion, so decimal or `0x` hex.
- **Description:** Compatibility bitmask for LBE pieces worn at the same time (for example a combat pack over a backpack). Two pieces may be worn together only when their masks share at least one bit, and the piece already worn must itself have a non-zero mask.
- **Only used with:** NIV.
- **Numeric semantics:** Bitmask; combine bits with OR. A larger value is not “more” of anything. `0` shares no bit with anything, so it cannot be combined.
- **Values:** Up to 32 bits; the shipped table uses `0..4` and most rows are `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (the `lbeCombo` field comment “Increased the LBE combo to 32bit … bitwise system”; combat-pack/backpack checks).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeFilledSize`

- **Type:** `Integer` (`UINT8`) — the game stores this field in a `UINT8`, so values above `255` truncate.
- **Description:** Item size the LBE counts as when its pockets are full. `CalculateItemSize` starts from `Item[usItem].ItemSize`, adds the share of `lbeFilledSize - ItemSize` that matches how much of the pockets is used, and never exceeds `max(ItemSize, lbeFilledSize)`.
- **Only used with:** NIV.
- **Numeric semantics:** An item-size value, not a quantity; the shipped rows use `0`, `10` and `22..34`, which are item sizes.
- **Values:** `0..255`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (the LBE loader; the item-size calculation).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeAvailableVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Volume capacity of the LBE. Contents are rejected when the volume already taken plus the volume of the pocket being filled exceeds this value, and the item description shows the used/available volume.
- **Only used with:** NIV.
- **Numeric semantics:** Volume units; `0` means the item is not a volume-limited carrier (the shipped table leaves most rows at `0` and uses `12..100` for the carriers).
- **Values:** `0..255`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (the volume check; the item-description line).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketsAvailable`

- **Type:** `Integer` (`UINT16`)
- **Description:** Bitmask of pocket slots whose pocket is supplied by an LBE attachment worn on the item (MOLLE). When `lbePocketIndex` for a slot is `0` and the matching bit is set, the game looks for an attachment in the attachment slot whose `ubPocketMapping - 1` equals that pocket number and uses the attachment's first non-zero pocket; without the bit the slot stays inactive.
- **Only used with:** NIV.
- **Numeric semantics:** Bitmask, not a count. Bit `n` corresponds to pocket slot `n`, the same 0-based index as `lbePocketIndex1..12`; combine bits with OR.
- **Values:** `0..4095` (12 slots); the shipped table uses values such as `15`, `115`, `2099` and `4067`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (the pocket fallback and the attachment pocket lookup).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex1`

- **Type:** `Integer` (`UINT8`) — the game stores each pocket index in a `UINT8`, so values above `255` truncate.
- **Description:** Pocket row assigned to slot 1. The loader accepts `lbePocketIndex1` and the dotted spelling `lbePocketIndex.1` and writes the value into the first pocket slot.
- **Only used with:** NIV.
- **Values:** Pocket row index in [Pockets](Pockets.md); `0` leaves the slot inactive.
- **Default:** `0` when omitted; the game allocates 12 pocket slots and zero-fills them.
- **Source:** game code (the LBE loader; 12 pocket slots).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex2`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 2 (`lbePocketIndex2` or `lbePocketIndex.2`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex3`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 3 (`lbePocketIndex3` or `lbePocketIndex.3`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex4`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 4 (`lbePocketIndex4` or `lbePocketIndex.4`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex5`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 5 (`lbePocketIndex5` or `lbePocketIndex.5`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex6`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 6 (`lbePocketIndex6` or `lbePocketIndex.6`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex7`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 7 (`lbePocketIndex7` or `lbePocketIndex.7`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex8`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 8 (`lbePocketIndex8` or `lbePocketIndex.8`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex9`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 9 (`lbePocketIndex9` or `lbePocketIndex.9`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex10`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 10 (`lbePocketIndex10` or `lbePocketIndex.10`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex11`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 11 (`lbePocketIndex11` or `lbePocketIndex.11`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex12`

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket row assigned to slot 12, the last of the 12 slots (`lbePocketIndex12` or `lbePocketIndex.12`). `0` leaves the slot inactive.
- **Only used with:** NIV.
- **Values:** Pocket row index in `Pockets`; `0` leaves the slot inactive.
- **Default:** `0` when omitted.
- **Source:** game code (the LBE loader; 12 pocket slots).
- **Confidence:** `source`

## Relations

- [Items](Items.md) `ubClassIndex` is the row's position in this table; `lbeIndex` records that position but is not read by the game.
- `lbeClass` uses [Lookup.LBEClass](../Lookup.md#lbeclass), while the code enum `eLBE_CLASS` also defines `5` and `6`.
- `lbePocketIndex1..12` refer to [Pockets](Pockets.md); `0` leaves a slot inactive.
- `lbePocketsAvailable` bit `n` works together with `lbePocketIndex` slot `n`: the bit supplies the pocket from a worn LBE attachment when the slot itself has no pocket row.
- `lbeCombo` pairs this row with other LBE rows that may be worn at the same time.

## Localization

This table has no display strings; LBE names come from `Items`.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- LBE table data.
- LBEClass lookup table.
- Game code: the LBE loader, the LBE and pocket structures, the item-size and volume calculations, and the inventory UI slot/pocket fallback.
- Field types are the storage types of the game structure, not the editor's generic `Integer` kind.
- Remaining statements about shipped values come from the XML data and are marked as such; they do not claim a code-verified formula.

## Open questions

- How a specific `lbeCombo` bit is assigned to a family of LBE pieces is a convention of the data author; the game only tests for a shared bit.
- The game picks an LBE row by its position in the list, so renumbering `lbeIndex` without keeping the rows in order would break the `Items.ubClassIndex` link; the table itself does not state that requirement.
