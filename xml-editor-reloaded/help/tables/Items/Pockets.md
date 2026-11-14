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
  - Game code (pocket loader and inventory/LBE use)
  - Pocket table data
  - Pocket lookup tables
  - Item metadata
status: reviewed
updated: 2026-11-08
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

`Pockets.xml` defines the pocket types used by the New Inventory System: the silhouette drawn for the empty pocket, the relative size weight, the accepted item classes, the volume the pocket takes on its carrier and the maximum number of items of each item size that fit. LBE items point at these rows through `lbePocketIndex*`.

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
- **Values:** Non-negative index, unique within the file. It is referenced by `LoadBearingEquipment.lbePocketIndex*`; those references are stored as `UINT8`, so only the first 255 rows are addressable.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `XML_LoadBearingEquipment.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pName`

- **Type:** `String` (`String`)
- **Description:** Display label of the pocket type, used as the pocket caption in LBE item descriptions.
- **Only used with:** NIV.
- **Values:** Free text, read as UTF-8 into an 80-character wide buffer; longer text is truncated.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_LBEPocket.cpp`, `BobbyRGuns.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pSilhouette`

- **Type:** `Integer` (`UINT8`)
- **Description:** Subimage index of the pocket silhouette in `INTERFACE\Silhouettes.sti`; it is drawn for an empty pocket.
- **Only used with:** NIV.
- **Values:** `0..255`; `Lookup.Silhouette` names the same IDs.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`Interface Items.cpp`, `Interface.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pType`

- **Type:** `Integer` (`UINT16`)
- **Description:** Relative size weight of the pocket, used when an LBE item's size is recalculated from its contents. It is not the item-size capacity index; that is `ItemCapacityPerSize*`.
- **Only used with:** NIV.
- **Values:** IDs from `Lookup.PocketSize`: `0` None, `1` Small, `2` Medium, `3` Large. `1`, `2` and `3` are weighted as small/medium/large; `0` has no weight.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`Items.cpp`, `XML_LBEPocket.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pRestriction`

- **Type:** `Flags` (`UINT32`)
- **Description:** Bitmask of item classes the pocket accepts. Zero means the pocket has no class restriction.
- **Only used with:** NIV.
- **Values:** Bitmask of `Lookup.ItemClass` values (the `IC_*` bits stored in `Items.usItemClass`). `0` means no restriction; a non-zero mask must intersect the item class, otherwise the pocket accepts nothing.
- **Numeric semantics:** parsed with `strtoul` base 0, so decimal and `0x` hexadecimal text are both accepted.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Volume the pocket occupies on its LBE carrier. A pocket can be attached only while the carrier's free volume covers the sum of the attached pockets' volumes.
- **Only used with:** NIV.
- **Numeric semantics:** relative volume units; higher means a larger pocket, and negative values are impossible.
- **Values:** `0..255`; the sum is compared with `LoadBearingEquipment.lbeAvailableVolume`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`Items.cpp`, `BobbyRGuns.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize0`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 0 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize1`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 1 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize2`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 2 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize3`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 3 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize4`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 4 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize5`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 5 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize6`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 6 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize7`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 7 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize8`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 8 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize9`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 9 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize10`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 10 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize11`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 11 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize12`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 12 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize13`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 13 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize14`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 14 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize15`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 15 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize16`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 16 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize17`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 17 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize18`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 18 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize19`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 19 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize20`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 20 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize21`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 21 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize22`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 22 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize23`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 23 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize24`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 24 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize25`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 25 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize26`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 26 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize27`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 27 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize28`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 28 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize29`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 29 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize30`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 30 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize31`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 31 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize32`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 32 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize33`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 33 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize34`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of items of item-size 34 that fit in the pocket. Zero means the pocket cannot hold that size.
- **Only used with:** NIV.
- **Values:** `0..255`. The suffix is the item-size index; the legacy dotted spelling (`ItemCapacityPerSize.N`) is accepted, and suffixes above the configured maximum item size (`MAX_ITEM_SIZE`, default `34`) are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

## Relations

- `pIndex` is referenced by `LoadBearingEquipment.lbePocketIndex*`; those fields are `UINT8`, so a pocket row above 255 cannot be referenced. Loaded pockets are appended in document order, and language overlays use `pIndex` as the row slot, so `pIndex` should equal the zero-based record position.
- `pType` uses [Lookup.PocketSize](../Lookup.md#pocketsize); `pSilhouette` uses [Lookup.Silhouette](../Lookup.md#silhouette); `pRestriction` uses [Lookup.ItemClass](../Lookup.md#itemclass).
- The item size class that indexes `ItemCapacityPerSize*` is `Items.ItemSize`.
- `pVolume` is checked against `LoadBearingEquipment.lbeAvailableVolume` when a pocket is attached to LBE gear.

## Localization

`pName` is a display label translated through language overlays; the numeric fields and the silhouette/size codes are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Game code: `XML_LBEPocket.cpp` (loader), `Item Types.h` (`POCKETTYPE`), `Items.cpp` (restriction, volume and capacity use), `Interface Items.cpp` (silhouette drawing).
- Pocket table data.
- Pocket lookup tables (`Lookup/PocketSize.xml`, `Lookup/Silhouette.xml`, `Lookup/ItemClass.xml`).
- Field types and limits: `POCKETTYPE` members and the loader casts.
- Descriptions cover the code path that consumes each field; no row counts or observed data ranges are used.

## Open questions

The physical unit behind `pVolume` is not defined by the code; only its relative comparison with `lbeAvailableVolume` matters. `pType` supplies a relative weight, so the capacity curve itself is set per item size in `ItemCapacityPerSize*`.
