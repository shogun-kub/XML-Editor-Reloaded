---
schema: 1
lang: en
family: map-delivery-methods
title: "DeliveryMethods - delivery methods, fees and delays"
tables:
  - DeliveryMethods
table_ids:
  - Map.DeliveryMethods
files:
  - Map/DeliveryMethods.xml
root: DELIVERYMETHODTABLE
record: DELIVERYMETHOD
key: uiIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# DeliveryMethods - delivery methods, fees and delays

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/uiIndex`](#deliverymethodtabledeliverymethoduiindex)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/description`](#deliverymethodtabledeliverymethoddescription)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/uiDestinationIndex`](#deliverymethodtabledeliverymethoddestinationdeliveryinfotabledestinationdeliveryinfouidestinationindex)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/usDestinationFee`](#deliverymethodtabledeliverymethoddestinationdeliveryinfotabledestinationdeliveryinfousdestinationfee)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/bDaysAhead`](#deliverymethodtabledeliverymethoddestinationdeliveryinfotabledestinationdeliveryinfobdaysahead)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`DeliveryMethods.xml` defines each Bobby Ray delivery option and, per destination, its fee and delay. The destination index refers to `ShippingDestinations.uiIndex`.

## Tables

| Table | File | Notes |
|---|---|---|
| `DeliveryMethods` | `Map/DeliveryMethods.xml` | — |

## Structure

```
DELIVERYMETHODTABLE
└── DELIVERYMETHOD
    ├── uiIndex
    ├── description
    └── DESTINATIONDELIVERYINFOTABLE
        └── DESTINATIONDELIVERYINFO
            ├── uiDestinationIndex
            ├── usDestinationFee
            └── bDaysAhead
```

## Fields

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Delivery-method index stored in the XML, but the loader does not read it: methods are stored in document order and the element is ignored.
- **Values:** Integer; changing it does not reorder the stored methods.
- **Default:** Required; identifies the record.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/description`

- **Type:** `String` (`CHAR16`)
- **Description:** Player-facing name of the delivery method, such as `Overnight express`.
- **Values:** Free text, truncated to `MAX_DELIVERYMETHOD_DESC_LENGTH`.
- **Default:** Absent; the game default applies.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/uiDestinationIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the shipping destination this price and delay applies to. It is an ID.
- **Values:** `0..4294967295`; destination ID.
- **Default:** Absent; the game default applies.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/usDestinationFee`

- **Type:** `Integer` (`UINT16`)
- **Description:** Delivery fee to this destination, in money.
- **Numeric semantics:** higher = more expensive; negative values are not meaningful.
- **Values:** `0..65535` money.
- **Default:** Absent; the game default applies.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/bDaysAhead`

- **Type:** `Integer` (`INT8`)
- **Description:** Number of days the shipment takes to arrive.
- **Numeric semantics:** higher = slower; the loader stores this field as `INT8`.
- **Values:** `-128..127` days.
- **Default:** Absent; the game default applies.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`
## Relations

- `uiDestinationIndex` references [ShippingDestinations](ShippingDestinations.md) `uiIndex`; the destination name is defined there.

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Map table data.
- Field names, XML comments and observed values.
- Descriptions are based on the field names and observed table values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for fields marked as inferred is not stated by the table data.
`

- **Type:** `Integer` (`INT8`)
- **Description:** Number of days the shipment takes to arrive.
- **Numeric semantics:** higher = slower; negative values are not meaningful.
- **Values:** `-128..127` days; the loader stores this field as `INT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

## Relations

- `uiDestinationIndex` references [ShippingDestinations](ShippingDestinations.md) `uiIndex`; the destination name is defined there.

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Map table data.
- Field names, XML comments and observed values.
- Descriptions are based on the field names and observed table values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for fields marked as inferred is not stated by the table data.
