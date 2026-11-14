---
schema: 1
lang: en
family: mercs-gear
title: "MercStartingGear - starting equipment kits"
tables:
  - MercStartingGear
table_ids:
  - Inventory.MercStartingGear
files:
  - Inventory/MercStartingGear.xml
root: MERCGEARLIST
record: MERCGEAR
key: mIndex + mGearKitName
sources:
  - Merc starting gear table data
  - Item metadata
status: reviewed
updated: 2026-11-08
---

# MercStartingGear - starting equipment kits

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERCGEARLIST/MERCGEAR/mIndex`](#mercgearlistmercgearmindex)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mGearKitName`](#mercgearlistmercgeargearkitmgearkitname)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mPriceMod`](#mercgearlistmercgeargearkitmpricemod)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mAbsolutePrice`](#mercgearlistmercgeargearkitmabsoluteprice)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mHelmet`](#mercgearlistmercgeargearkitmhelmet)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetDrop`](#mercgearlistmercgeargearkitmhelmetdrop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetStatus`](#mercgearlistmercgeargearkitmhelmetstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mVest`](#mercgearlistmercgeargearkitmvest)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mVestDrop`](#mercgearlistmercgeargearkitmvestdrop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mVestStatus`](#mercgearlistmercgeargearkitmveststatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mLeg`](#mercgearlistmercgeargearkitmleg)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mLegDrop`](#mercgearlistmercgeargearkitmlegdrop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mLegStatus`](#mercgearlistmercgeargearkitmlegstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mWeapon`](#mercgearlistmercgeargearkitmweapon)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponDrop`](#mercgearlistmercgeargearkitmweapondrop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponStatus`](#mercgearlistmercgeargearkitmweaponstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig0`](#mercgearlistmercgeargearkitmbig0)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Status`](#mercgearlistmercgeargearkitmbig0status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Quantity`](#mercgearlistmercgeargearkitmbig0quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Drop`](#mercgearlistmercgeargearkitmbig0drop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig1`](#mercgearlistmercgeargearkitmbig1)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Status`](#mercgearlistmercgeargearkitmbig1status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Quantity`](#mercgearlistmercgeargearkitmbig1quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Drop`](#mercgearlistmercgeargearkitmbig1drop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig2`](#mercgearlistmercgeargearkitmbig2)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Status`](#mercgearlistmercgeargearkitmbig2status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Quantity`](#mercgearlistmercgeargearkitmbig2quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Drop`](#mercgearlistmercgeargearkitmbig2drop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig3`](#mercgearlistmercgeargearkitmbig3)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Status`](#mercgearlistmercgeargearkitmbig3status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Quantity`](#mercgearlistmercgeargearkitmbig3quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Drop`](#mercgearlistmercgeargearkitmbig3drop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0`](#mercgearlistmercgeargearkitmsmall0)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Status`](#mercgearlistmercgeargearkitmsmall0status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Quantity`](#mercgearlistmercgeargearkitmsmall0quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1`](#mercgearlistmercgeargearkitmsmall1)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Status`](#mercgearlistmercgeargearkitmsmall1status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Quantity`](#mercgearlistmercgeargearkitmsmall1quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2`](#mercgearlistmercgeargearkitmsmall2)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Status`](#mercgearlistmercgeargearkitmsmall2status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Quantity`](#mercgearlistmercgeargearkitmsmall2quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3`](#mercgearlistmercgeargearkitmsmall3)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Status`](#mercgearlistmercgeargearkitmsmall3status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Quantity`](#mercgearlistmercgeargearkitmsmall3quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4`](#mercgearlistmercgeargearkitmsmall4)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Status`](#mercgearlistmercgeargearkitmsmall4status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Quantity`](#mercgearlistmercgeargearkitmsmall4quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5`](#mercgearlistmercgeargearkitmsmall5)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Status`](#mercgearlistmercgeargearkitmsmall5status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Quantity`](#mercgearlistmercgeargearkitmsmall5quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6`](#mercgearlistmercgeargearkitmsmall6)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Status`](#mercgearlistmercgeargearkitmsmall6status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Quantity`](#mercgearlistmercgeargearkitmsmall6quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7`](#mercgearlistmercgeargearkitmsmall7)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Status`](#mercgearlistmercgeargearkitmsmall7status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Quantity`](#mercgearlistmercgeargearkitmsmall7quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lVest`](#mercgearlistmercgeargearkitlvest)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lVestStatus`](#mercgearlistmercgeargearkitlveststatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThigh`](#mercgearlistmercgeargearkitlleftthigh)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThighStatus`](#mercgearlistmercgeargearkitlleftthighstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lRightThigh`](#mercgearlistmercgeargearkitlrightthigh)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lRightThighStatus`](#mercgearlistmercgeargearkitlrightthighstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lCPack`](#mercgearlistmercgeargearkitlcpack)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lCPackStatus`](#mercgearlistmercgeargearkitlcpackstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lBPack`](#mercgearlistmercgeargearkitlbpack)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lBPackStatus`](#mercgearlistmercgeargearkitlbpackstatus)
  - [`MERCGEARLIST/MERCGEAR/mName`](#mercgearlistmercgearmname)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercStartingGear.xml` defines each mercenary’s starting equipment. A mercenary can have up to five named `GEARKIT` blocks (`NUM_MERCSTARTINGGEAR_KITS`); each block lists the items, condition, quantity and drop flags for the kit.

## Tables

| Table | File | Notes |
|---|---|---|
| `MercStartingGear` | `Inventory/MercStartingGear.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
MERCGEARLIST
└── MERCGEAR
    ├── mIndex
    ├── GEARKIT
    │   ├── mGearKitName
    │   ├── mPriceMod
    │   ├── mAbsolutePrice
    │   ├── mHelmet
    │   ├── mHelmetDrop
    │   ├── mHelmetStatus
    │   ├── mVest
    │   ├── mVestDrop
    │   ├── mVestStatus
    │   ├── mLeg
    │   ├── mLegDrop
    │   ├── mLegStatus
    │   ├── mWeapon
    │   ├── mWeaponDrop
    │   ├── mWeaponStatus
    │   ├── mBig0
    │   ├── mBig0Status
    │   ├── mBig0Quantity
    │   ├── mBig0Drop
    │   ├── mBig1
    │   ├── mBig1Status
    │   ├── mBig1Quantity
    │   ├── mBig1Drop
    │   ├── mBig2
    │   ├── mBig2Status
    │   ├── mBig2Quantity
    │   ├── mBig2Drop
    │   ├── mBig3
    │   ├── mBig3Status
    │   ├── mBig3Quantity
    │   ├── mBig3Drop
    │   ├── mSmall0
    │   ├── mSmall0Status
    │   ├── mSmall0Quantity
    │   ├── mSmall1
    │   ├── mSmall1Status
    │   ├── mSmall1Quantity
    │   ├── mSmall2
    │   ├── mSmall2Status
    │   ├── mSmall2Quantity
    │   ├── mSmall3
    │   ├── mSmall3Status
    │   ├── mSmall3Quantity
    │   ├── mSmall4
    │   ├── mSmall4Status
    │   ├── mSmall4Quantity
    │   ├── mSmall5
    │   ├── mSmall5Status
    │   ├── mSmall5Quantity
    │   ├── mSmall6
    │   ├── mSmall6Status
    │   ├── mSmall6Quantity
    │   ├── mSmall7
    │   ├── mSmall7Status
    │   ├── mSmall7Quantity
    │   ├── lVest
    │   ├── lVestStatus
    │   ├── lLeftThigh
    │   ├── lLeftThighStatus
    │   ├── lRightThigh
    │   ├── lRightThighStatus
    │   ├── lCPack
    │   ├── lCPackStatus
    │   ├── lBPack
    │   └── lBPackStatus
    └── mName
```

## Fields

### `MERCGEARLIST/MERCGEAR/mIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Mercenary profile ID; it points to [MercProfiles](MercProfiles.md).
- **Values:** Profile ID `0..254`; the loader stores it as `UINT16` and uses the low 8 bits as the gear-table row index, so values above `254` are out of range.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mGearKitName`

- **Type:** `String` (`CHAR16`)
- **Description:** Name of the starting-gear kit. A mercenary can have up to five kits (`NUM_MERCSTARTINGGEAR_KITS`).
- **Values:** Free text, up to 79 characters.
- **Default:** empty string when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mPriceMod`

- **Type:** `Integer` (`INT16`)
- **Description:** Percentage adjustment applied to the computed kit price.
- **Numeric semantics:** The multiplier is `(mPriceMod + 100) / 100`: `0` = normal price, `50` = ×1.5, `-50` = ×0.5, `-100` = free, `100` = ×2. Values outside `-100..200` are ignored.
- **Values:** `-100..200`; values outside this range are ignored.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mAbsolutePrice`

- **Type:** `Integer` (`INT16`)
- **Description:** Absolute kit price. `-1` leaves the price to be computed from the kit's items; a value in `0..32000` overrides the item price and `mPriceMod`.
- **Numeric semantics:** `-1` = not set; `0..32000` = fixed money amount; values above `32000` are ignored.
- **Values:** `-1..32767`; the game accepts `-1` or `0..32000`.
- **Default:** `-1` when omitted; the loader resets it to `-1` at the start of every `GEARKIT`.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mHelmet`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the helmet slot (`HELMETPOS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetDrop`

- **Type:** `Integer` (`INT32`)
- **Description:** Drop flag for the helmet slot. `0` marks the item undroppable for RPC/NPC profiles (index above 56); non-zero keeps it droppable.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the helmet (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mVest`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the vest slot (`VESTPOS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mVestDrop`

- **Type:** `Integer` (`INT32`)
- **Description:** Drop flag for the vest slot. `0` marks the item undroppable for RPC/NPC profiles (index above 56); non-zero keeps it droppable.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mVestStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the vest (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mLeg`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the leg slot (`LEGPOS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mLegDrop`

- **Type:** `Integer` (`INT32`)
- **Description:** Drop flag for the leg slot. `0` marks the item undroppable for RPC/NPC profiles (index above 56); non-zero keeps it droppable.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mLegStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the leggings (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mWeapon`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the hand (weapon) slot (`HANDPOS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponDrop`

- **Type:** `Integer` (`INT32`)
- **Description:** Drop flag for the hand (weapon) slot. `0` marks the item undroppable for RPC/NPC profiles (index above 56); non-zero keeps it droppable.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the weapon (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the big pocket 1 (`BIGPOCK1POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in big pocket 1 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in big pocket 1.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Drop`

- **Type:** `Integer` (`INT32`)
- **Description:** Drop flag for the big pocket 1. `0` marks the item undroppable for RPC/NPC profiles (index above 56); non-zero keeps it droppable.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the big pocket 2 (`BIGPOCK2POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in big pocket 2 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in big pocket 2.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Drop`

- **Type:** `Integer` (`INT32`)
- **Description:** Drop flag for the big pocket 2. `0` marks the item undroppable for RPC/NPC profiles (index above 56); non-zero keeps it droppable.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the big pocket 3 (`BIGPOCK3POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in big pocket 3 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in big pocket 3.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Drop`

- **Type:** `Integer` (`INT32`)
- **Description:** Drop flag for the big pocket 3. `0` marks the item undroppable for RPC/NPC profiles (index above 56); non-zero keeps it droppable.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the big pocket 4 (`BIGPOCK4POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in big pocket 4 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in big pocket 4.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Drop`

- **Type:** `Integer` (`INT32`)
- **Description:** Drop flag for the big pocket 4. `0` marks the item undroppable for RPC/NPC profiles (index above 56); non-zero keeps it droppable.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the small pocket 1 (`SMALLPOCK1POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in small pocket 1 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in small pocket 1.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the small pocket 2 (`SMALLPOCK2POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in small pocket 2 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in small pocket 2.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the small pocket 3 (`SMALLPOCK3POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in small pocket 3 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in small pocket 3.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the small pocket 4 (`SMALLPOCK4POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in small pocket 4 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in small pocket 4.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the small pocket 5 (`SMALLPOCK5POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in small pocket 5 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in small pocket 5.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the small pocket 6 (`SMALLPOCK6POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in small pocket 6 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in small pocket 6.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the small pocket 7 (`SMALLPOCK7POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in small pocket 7 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in small pocket 7.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID placed in the small pocket 8 (`SMALLPOCK8POS`).
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the item in small pocket 8 (percent; `100` = new).
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Stack size for the item in small pocket 8.
- **Numeric semantics:** Count; higher values mean more items. Only the pocket slots have quantity fields; the helmet, vest, leg and hand slots always use `1`.
- **Values:** Count; `0` is used when the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lVest`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in the NIV vest LBE slot (`VESTPOCKPOS`).
- **Only used with:** NIV.
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lVestStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the NIV vest LBE item (percent; `100` = new).
- **Only used with:** NIV.
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created; LBE stack size is always `1`.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThigh`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in the NIV left-thigh LBE slot (`LTHIGHPOCKPOS`).
- **Only used with:** NIV.
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThighStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the NIV left-thigh LBE item (percent; `100` = new).
- **Only used with:** NIV.
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created; LBE stack size is always `1`.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lRightThigh`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in the NIV right-thigh LBE slot (`RTHIGHPOCKPOS`).
- **Only used with:** NIV.
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lRightThighStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the NIV right-thigh LBE item (percent; `100` = new).
- **Only used with:** NIV.
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created; LBE stack size is always `1`.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lCPack`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in the NIV combat-pack LBE slot (`CPACKPOCKPOS`).
- **Only used with:** NIV.
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lCPackStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the NIV combat-pack LBE item (percent; `100` = new).
- **Only used with:** NIV.
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created; LBE stack size is always `1`.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lBPack`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in the NIV backpack LBE slot (`BPACKPOCKPOS`).
- **Only used with:** NIV.
- **Values:** Item ID; `0` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lBPackStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition of the NIV backpack LBE item (percent; `100` = new).
- **Only used with:** NIV.
- **Numeric semantics:** Percentage; higher values mean better condition. The value is passed to the item's `objectStatus` when the soldier is created; LBE stack size is always `1`.
- **Values:** `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/mName`

- **Type:** `String` (`CHAR8`)
- **Description:** Mercenary name used by the starting-gear table.
- **Values:** Free text, up to 79 characters.
- **Default:** empty string when omitted.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

## Relations

- `mIndex` points to [MercProfiles](MercProfiles.md).
- `m*` item columns point to [Items](Items/Items.md); `l*` item columns point to [Items](Items/Items.md) and are used by NIV LBE slots.

## Localization

`mName` and `mGearKitName` are display labels; item names come from `Items`.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Merc starting gear table data.
- Item metadata.
- Game code: starting-gear XML loader, profile loader, soldier creation, item creation, AIM members screen.

## Open questions

None: the drop, status, quantity and price semantics are defined by the starting-gear loader, the profile loader and item creation.
