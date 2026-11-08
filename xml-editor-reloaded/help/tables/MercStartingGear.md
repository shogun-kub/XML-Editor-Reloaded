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
updated: 2026-11-07
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

`MercStartingGear.xml` defines each mercenary’s starting equipment. A mercenary can have several named `GEARKIT` blocks; each block lists the items, condition, quantity and drop flags for the kit.

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
- **Values:** Profile ID `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (starting-gear loader); merc starting gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mGearKitName`

- **Type:** `String` (`String`)
- **Description:** Name of the starting-gear kit.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mPriceMod`

- **Type:** `Integer` (`INT16`)
- **Description:** Price modifier applied to the kit.
- **Numeric semantics:** Signed percentage modifier: `0` means no change, positive values increase the price, and negative values discount it. The UI accepts `-100..200`.
- **Values:** `-100..200`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (kit-price handling in `AimMembers.cpp`); merc starting gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mAbsolutePrice`

- **Type:** `Integer` (`INT16`)
- **Description:** Absolute kit price; `-1` leaves the price unset.
- **Numeric semantics:** `-1` means “not set”; non-negative values are money amounts, and higher values mean a more expensive kit.
- **Values:** `-1..32767`; the UI treats values up to `32000` as valid.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (kit-price handling in `AimMembers.cpp`); merc starting gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mHelmet`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID of the starting helmet.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetDrop`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Drop flag for the helmet.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition/status of the helmet.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mVest`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID of the starting vest.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mVestDrop`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Drop flag for the vest.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mVestStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition/status of the vest.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mLeg`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID of the starting leggings.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mLegDrop`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Drop flag for the leggings.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mLegStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition/status of the leggings.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mWeapon`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID of the starting weapon.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponDrop`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Drop flag for the weapon.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition/status of the weapon.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV big slot 0.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV big slot 0.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV big slot 0.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Drop`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Drop value for OIV big slot 0.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV big slot 1.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV big slot 1.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV big slot 1.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Drop`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Drop value for OIV big slot 1.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV big slot 2.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV big slot 2.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV big slot 2.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Drop`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Drop value for OIV big slot 2.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV big slot 3.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV big slot 3.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV big slot 3.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Drop`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Drop value for OIV big slot 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV small slot 0.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV small slot 0.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV small slot 0.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV small slot 1.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV small slot 1.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV small slot 1.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV small slot 2.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV small slot 2.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV small slot 2.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV small slot 3.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV small slot 3.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV small slot 3.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV small slot 4.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV small slot 4.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV small slot 4.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV small slot 5.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV small slot 5.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV small slot 5.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV small slot 6.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV small slot 6.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV small slot 6.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID in OIV small slot 7.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Status value for OIV small slot 7.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Quantity value for OIV small slot 7.
- **Numeric semantics:** Higher values mean a larger quantity/count, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lVest`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID of the NIV vest slot.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lVestStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition/status of the NIV vest slot item.
- **Only used with:** NIV.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThigh`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID of the NIV left-thigh slot.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThighStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition/status of the NIV left-thigh item.
- **Only used with:** NIV.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lRightThigh`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID of the NIV right-thigh slot.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lRightThighStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition/status of the NIV right-thigh item.
- **Only used with:** NIV.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lCPack`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID of the NIV combat-pack slot.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lCPackStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition/status of the NIV combat-pack item.
- **Only used with:** NIV.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lBPack`

- **Type:** `Integer` (`INT32`)
- **Description:** Item ID of the NIV backpack slot.
- **Only used with:** NIV.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lBPackStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Condition/status of the NIV backpack item.
- **Only used with:** NIV.
- **Numeric semantics:** Higher values mean better reliability/condition; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCGEARLIST/MERCGEAR/mName`

- **Type:** `String` (`String`)
- **Description:** Mercenary name used by the starting-gear table.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc starting gear table data; item/profile metadata.
- **Confidence:** `inferred`

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
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The drop and price semantics of `m*Drop`, `mPriceMod` and `mAbsolutePrice` depend on the starting-gear loader; the table only stores the values.
