---
schema: 1
lang: en
family: items-pockets
title: "Pockets - NIV pocket capacity"
tables:
  - Pockets
table_ids:
  - Items.Pockets
files:
  - Items/Pockets.xml
root: POCKETLIST
record: POCKET
key: pIndex
sources:
  - Pocket table data
  - Pocket lookup tables
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# Pockets - NIV pocket capacity

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`POCKETLIST/POCKET/pIndex`](#pocketlistpocketpindex)
  - [`POCKETLIST/POCKET/pName`](#pocketlistpocketpname)
  - [`POCKETLIST/POCKET/pSilhouette`](#pocketlistpocketpsilhouette)
  - [`POCKETLIST/POCKET/pType`](#pocketlistpocketptype)
  - [`POCKETLIST/POCKET/pRestriction`](#pocketlistpocketprestriction)
  - [`POCKETLIST/POCKET/pVolume`](#pocketlistpocketpvolume)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize0`](#pocketlistpocketitemcapacitypersize0)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize1`](#pocketlistpocketitemcapacitypersize1)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize2`](#pocketlistpocketitemcapacitypersize2)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize3`](#pocketlistpocketitemcapacitypersize3)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize4`](#pocketlistpocketitemcapacitypersize4)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize5`](#pocketlistpocketitemcapacitypersize5)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize6`](#pocketlistpocketitemcapacitypersize6)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize7`](#pocketlistpocketitemcapacitypersize7)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize8`](#pocketlistpocketitemcapacitypersize8)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize9`](#pocketlistpocketitemcapacitypersize9)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize10`](#pocketlistpocketitemcapacitypersize10)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize11`](#pocketlistpocketitemcapacitypersize11)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize12`](#pocketlistpocketitemcapacitypersize12)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize13`](#pocketlistpocketitemcapacitypersize13)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize14`](#pocketlistpocketitemcapacitypersize14)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize15`](#pocketlistpocketitemcapacitypersize15)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize16`](#pocketlistpocketitemcapacitypersize16)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize17`](#pocketlistpocketitemcapacitypersize17)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize18`](#pocketlistpocketitemcapacitypersize18)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize19`](#pocketlistpocketitemcapacitypersize19)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize20`](#pocketlistpocketitemcapacitypersize20)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize21`](#pocketlistpocketitemcapacitypersize21)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize22`](#pocketlistpocketitemcapacitypersize22)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize23`](#pocketlistpocketitemcapacitypersize23)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize24`](#pocketlistpocketitemcapacitypersize24)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize25`](#pocketlistpocketitemcapacitypersize25)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize26`](#pocketlistpocketitemcapacitypersize26)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize27`](#pocketlistpocketitemcapacitypersize27)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize28`](#pocketlistpocketitemcapacitypersize28)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize29`](#pocketlistpocketitemcapacitypersize29)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize30`](#pocketlistpocketitemcapacitypersize30)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize31`](#pocketlistpocketitemcapacitypersize31)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize32`](#pocketlistpocketitemcapacitypersize32)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize33`](#pocketlistpocketitemcapacitypersize33)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize34`](#pocketlistpocketitemcapacitypersize34)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Pockets.xml` defines the pocket types used by the New Inventory System: their size class, accepted item classes, volume and the maximum number of items of each item size that fit.

## Tables

| Table | File | Notes |
|---|---|---|
| `Pockets` | `Items/Pockets.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
POCKETLIST
└── POCKET
    ├── pIndex
    ├── pName
    ├── pSilhouette
    ├── pType
    ├── pRestriction
    ├── pVolume
    ├── ItemCapacityPerSize0
    ├── ItemCapacityPerSize1
    ├── ItemCapacityPerSize2
    ├── ItemCapacityPerSize3
    ├── ItemCapacityPerSize4
    ├── ItemCapacityPerSize5
    ├── ItemCapacityPerSize6
    ├── ItemCapacityPerSize7
    ├── ItemCapacityPerSize8
    ├── ItemCapacityPerSize9
    ├── ItemCapacityPerSize10
    ├── ItemCapacityPerSize11
    ├── ItemCapacityPerSize12
    ├── ItemCapacityPerSize13
    ├── ItemCapacityPerSize14
    ├── ItemCapacityPerSize15
    ├── ItemCapacityPerSize16
    ├── ItemCapacityPerSize17
    ├── ItemCapacityPerSize18
    ├── ItemCapacityPerSize19
    ├── ItemCapacityPerSize20
    ├── ItemCapacityPerSize21
    ├── ItemCapacityPerSize22
    ├── ItemCapacityPerSize23
    ├── ItemCapacityPerSize24
    ├── ItemCapacityPerSize25
    ├── ItemCapacityPerSize26
    ├── ItemCapacityPerSize27
    ├── ItemCapacityPerSize28
    ├── ItemCapacityPerSize29
    ├── ItemCapacityPerSize30
    ├── ItemCapacityPerSize31
    ├── ItemCapacityPerSize32
    ├── ItemCapacityPerSize33
    └── ItemCapacityPerSize34
```

## Fields

### `POCKETLIST/POCKET/pIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index of the pocket row and identity of the XML record.
- **Only used with:** NIV.
- **Values:** Non-negative index; unique within the file. It is referenced by `LoadBearingEquipment.lbePocketIndex*`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/pName`

- **Type:** `String` (`String`)
- **Description:** Display name of the pocket type.
- **Only used with:** NIV.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/pSilhouette`

- **Type:** `Integer` (`UINT16`)
- **Description:** Silhouette/icon used to draw the pocket in the inventory interface.
- **Only used with:** NIV.
- **Values:** IDs from `Lookup.Silhouette`; the lookup names the pocket icon/silhouette.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pType`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket size class used by the inventory UI and by item-fitting checks.
- **Only used with:** NIV.
- **Values:** IDs from `Lookup.PocketSize`: `0` None, `1` Small, `2` Medium, `3` Large.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pRestriction`

- **Type:** `Flags` (`UINT32`)
- **Description:** Bitmask of item classes the pocket accepts. Zero means the pocket has no class restriction.
- **Only used with:** NIV.
- **Values:** Bitmask of `Lookup.ItemClass` IDs. `0` means no restriction; non-zero requires the item class to intersect the mask.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pVolume`

- **Type:** `Integer` (`UINT16`)
- **Description:** Volume of the pocket. It is used by the LBE volume checks. Numeric semantics: Higher values mean a larger quantity; negative values are not meaningful.
- **Only used with:** NIV.
- **Values:** Pocket volume; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize0`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 0 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 1 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 2 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 3 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 4 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 5 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 6 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 7 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 8 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 9 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 10 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize11`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 11 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize12`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 12 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize13`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 13 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize14`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 14 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize15`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 15 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize16`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 16 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize17`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 17 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize18`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 18 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize19`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 19 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize20`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 20 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize21`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 21 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize22`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 22 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize23`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 23 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize24`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 24 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize25`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 25 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize26`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 26 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize27`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 27 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize28`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 28 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize29`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 29 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize30`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 30 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize31`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 31 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize32`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 32 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize33`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 33 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

### `POCKETLIST/POCKET/ItemCapacityPerSize34`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum number of items of item-size 34 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** Non-negative capacity; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `pIndex` is referenced by `LoadBearingEquipment.lbePocketIndex*`.
- `pType` uses [Lookup.PocketSize](../Lookup.md#pocketsize); `pSilhouette` uses [Lookup.Silhouette](../Lookup.md#silhouette); `pRestriction` uses [Lookup.ItemClass](../Lookup.md#itemclass).
- The item size class that indexes `ItemCapacityPerSize*` is `Items.ItemSize`.

## Localization

`pName` is a display label. Capacity and restriction fields are numeric.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Pocket table data.
- Pocket lookup tables.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and the table schema comments; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The exact volume unit and the shape of the capacity curve are defined by the inventory code, not by this table.
