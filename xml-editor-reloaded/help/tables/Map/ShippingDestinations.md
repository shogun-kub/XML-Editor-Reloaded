---
schema: 1
lang: en
family: map-shipping-destinations
title: "ShippingDestinations - delivery destinations"
tables:
  - ShippingDestinations
table_ids:
  - Map.ShippingDestinations
files:
  - Map/ShippingDestinations.xml
root: DESTINATIONLIST
record: DESTINATION
key: uiIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# ShippingDestinations - delivery destinations

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`DESTINATIONLIST/DESTINATION/uiIndex`](#destinationlistdestinationuiindex)
  - [`DESTINATIONLIST/DESTINATION/name`](#destinationlistdestinationname)
  - [`DESTINATIONLIST/DESTINATION/ubMapX`](#destinationlistdestinationubmapx)
  - [`DESTINATIONLIST/DESTINATION/ubMapY`](#destinationlistdestinationubmapy)
  - [`DESTINATIONLIST/DESTINATION/ubMapZ`](#destinationlistdestinationubmapz)
  - [`DESTINATIONLIST/DESTINATION/sGridNo`](#destinationlistdestinationsgridno)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`ShippingDestinations.xml` lists the places Bobby Ray can ship to. Foreign destinations only need a name; local destinations also carry map coordinates and a grid number where the shipment arrives.

## Tables

| Table | File | Notes |
|---|---|---|
| `ShippingDestinations` | `Map/ShippingDestinations.xml` | — |

## Structure

```
DESTINATIONLIST
└── DESTINATION
    ├── uiIndex
    ├── name
    ├── ubMapX
    ├── ubMapY
    ├── ubMapZ
    └── sGridNo
```

## Fields

### `DESTINATIONLIST/DESTINATION/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the shipping destination. It is an array key and is referenced by `DeliveryMethods`.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** Required; identifies the record or lane.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/name`

- **Type:** `String` (`CHAR16`)
- **Description:** Display name of the destination, such as `Drassen` or `London`.
- **Values:** Free text, truncated to `MAX_DEST_NAME_LENGTH`.
- **Default:** Absent; the game default applies.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/ubMapX`

- **Type:** `Integer` (`UINT8`)
- **Description:** Map column of a local destination. Lower values are farther west; absent for foreign destinations.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/ubMapY`

- **Type:** `Integer` (`UINT8`)
- **Description:** Map row of a local destination. Lower values are farther north; absent for foreign destinations.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/ubMapZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Map level of a local destination; `0` is the surface. It is a level.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** Absent; the game default applies.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/sGridNo`

- **Type:** `Integer` (`UINT32`)
- **Description:** Map grid number where a local shipment arrives. It is a grid identifier.
- **Values:** `0..4294967295`; map grid number.
- **Default:** Absent; the game default applies.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

## Relations

- [DeliveryMethods](DeliveryMethods.md) references this table by `uiIndex` to set a fee and delay per destination.

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
