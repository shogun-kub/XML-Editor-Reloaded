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
status: draft
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

- **Type:** `Integer`
- **Description:** Index of the delivery method. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/description`

- **Type:** `String`
- **Description:** Player-facing name of the delivery method, such as `Overnight express`.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/uiDestinationIndex`

- **Type:** `Integer`
- **Description:** Index of the shipping destination this price and delay applies to. It is an ID.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/usDestinationFee`

- **Type:** `Integer`
- **Description:** Delivery fee to this destination, in money.
- **Numeric semantics:** higher = more expensive; negative values are not meaningful.
- **Values:** Money.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/bDaysAhead`

- **Type:** `Integer`
- **Description:** Number of days the shipment takes to arrive.
- **Numeric semantics:** higher = slower; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `uiDestinationIndex` references `ShippingDestinations.uiIndex`; the destination name is defined there.

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
