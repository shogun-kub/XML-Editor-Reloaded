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
status: draft
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

- **Type:** `Integer`
- **Description:** Index of the shipping destination. It is an array key and is referenced by `DeliveryMethods`.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `DESTINATIONLIST/DESTINATION/name`

- **Type:** `String`
- **Description:** Display name of the destination, such as `Drassen` or `London`.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `DESTINATIONLIST/DESTINATION/ubMapX`

- **Type:** `Integer`
- **Description:** Map column of a local destination. Lower values are farther west; absent for foreign destinations.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `DESTINATIONLIST/DESTINATION/ubMapY`

- **Type:** `Integer`
- **Description:** Map row of a local destination. Lower values are farther north; absent for foreign destinations.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `DESTINATIONLIST/DESTINATION/ubMapZ`

- **Type:** `Integer`
- **Description:** Map level of a local destination; `0` is the surface. It is a level.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `DESTINATIONLIST/DESTINATION/sGridNo`

- **Type:** `Integer`
- **Description:** Map grid number where a local shipment arrives. It is a grid identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `DeliveryMethods` references this table by `uiIndex` to set a fee and delay per destination.

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
