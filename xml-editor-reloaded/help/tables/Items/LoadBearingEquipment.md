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
  - Item metadata
status: reviewed
updated: 2026-11-07
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
- **Description:** Index of the LBE row and identity of the XML record.
- **Only used with:** NIV.
- **Values:** Non-negative index; unique within the file. It is referenced by `Items.ubClassIndex` for LBE items.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeClass`

- **Type:** `Integer` (`UINT16`)
- **Description:** LBE class used to decide which body slot the equipment occupies.
- **Only used with:** NIV.
- **Values:** IDs from `Lookup.LBEClass`: `0` Nothing, `1` Thigh Pack, `2` Vest, `3` Combat Pack, `4` Backpack.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeCombo`

- **Type:** `Integer` (`UINT16`)
- **Description:** Combination/merge index for LBE pieces that can be combined.
- **Only used with:** NIV.
- **Numeric semantics:** Numeric code/count; larger values mean a larger number or a different listed combination, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeFilledSize`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pre-filled size of the LBE, used when the item starts with contents.
- **Only used with:** NIV.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeAvailableVolume`

- **Type:** `Integer` (`UINT16`)
- **Description:** Total volume available in the LBE.
- **Only used with:** NIV.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketsAvailable`

- **Type:** `Integer` (`UINT16`)
- **Description:** Bitmask of pocket slots that are available on the LBE.
- **Only used with:** NIV.
- **Numeric semantics:** Numeric code/count; larger values mean a larger number or a different listed combination, and negative values are not meaningful.
- **Values:** Bitmask of available pocket slots; bit `n` corresponds to slot `n`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 1. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Pocket row index in [Pockets](Pockets.md); `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 2. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 3. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 4. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 5. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 6. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 7. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 8. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 9. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 10. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex11`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 11. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex12`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket row assigned to slot 12. `0` leaves the slot empty.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** lbe table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `lbeIndex` is referenced by [Items](Items.md) `ubClassIndex` for LBE items.
- `lbeClass` uses [Lookup.LBEClass](../Lookup.md#lbeclass).
- `lbePocketIndex1..12` refer to [Pockets](Pockets.md).

## Localization

This table has no display strings; LBE names come from `Items`.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- LBE table data.
- LBEClass lookup table.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and the table schema comments; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The exact meaning of `lbeCombo` and `lbeFilledSize` depends on the LBE combination code; the table only stores the values.
