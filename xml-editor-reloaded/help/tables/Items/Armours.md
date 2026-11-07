---
schema: 1
lang: en
family: items-armours
title: "Items.Armours - body armour"
tables:
  - Items.Armours
files:
  - Items/Armours.xml
root: ARMOURLIST
record: ARMOUR
key: uiIndex
sources:
  - Tactical/XML_Armour.cpp
  - Tactical/Weapons.h
  - Tactical/Weapons.cpp
  - Ja2/GameSettings.cpp
status: draft
updated: 2026-11-07
---

# Items.Armours - body armour

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ARMOURLIST/ARMOUR/uiIndex`](#armourlistarmouruiindex)
  - [`ARMOURLIST/ARMOUR/ubArmourClass`](#armourlistarmourubarmourclass)
  - [`ARMOURLIST/ARMOUR/ubProtection`](#armourlistarmourubprotection)
  - [`ARMOURLIST/ARMOUR/ubCoverage`](#armourlistarmourubcoverage)
  - [`ARMOURLIST/ARMOUR/ubDegradePercent`](#armourlistarmourubdegradepercent)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Armours.xml` defines the protective values of body armour. An armour item in `Items.Items`
points to one row through its `ubClassIndex`; the row is stored in the runtime `Armour[]`
array. The values drive damage reduction, armour wear, Bobby Ray's filters and the item
description panel.

`Items.Armours` is a flat table: one `ARMOUR` record per armour type, keyed by `uiIndex`.
The v5 file has 201 records.

## Tables

| Table | File | Records | Notes |
|---|---|---:|---|
| `Items.Armours` | `Items/Armours.xml` | 201 | v4 and v5 have the same schema; values may differ |

## Structure

```
ARMOURLIST
└── ARMOUR            one record per armour row
    ├── uiIndex       array index and key
    ├── ubArmourClass body slot / class
    ├── ubProtection  raw protection value
    ├── ubCoverage    covered area, percent
    └── ubDegradePercent wear per hit, percent
```

The loader writes each record to `Armour[uiIndex]` when the `ARMOUR` element closes. A
duplicate `uiIndex` overwrites the earlier row; the data has no duplicates. Records with an
`uiIndex` outside the array are ignored.

## Fields

### `ARMOURLIST/ARMOUR/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the row in the runtime `Armour[]` array and the identity of the
  XML record. `Items.Items.ubClassIndex` of an armour item points here.
- **Values:** `0..MAXITEMS-1`; the shipped data uses 0..200.
- **Default:** none; a missing value leaves the array slot at zero.
- **Source:** `Tactical/XML_Armour.cpp:100`, `:93-95`
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubArmourClass`

- **Type:** `Integer` (`UINT8`), enum `ARMOURCLASS_*`
- **Description:** Body slot/class of the armour. It is not the protection value; it tells
  the UI and equipment checks whether this is a helmet, vest, leggings or plate.
- **Values:** `0` helmet, `1` vest, `2` leggings, `3` plate, `4` monster, `5` vehicle
  (`Tactical/Weapons.h:102-110`). Shipped data uses 0..4.
- **Default:** `0` (helmet) when the element is missing, because the record is zeroed first.
- **Source:** `Tactical/XML_Armour.cpp:105`, `Tactical/Weapons.h:104-109`
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubProtection`

- **Type:** `Integer` (`UINT16` in the struct, cast to `UINT8` by the loader)
- **Description:** Raw protection value of the armour; higher values absorb more damage.
  The loader casts the parsed value to `UINT8`, so values above 255 are truncated even though
  the struct field is `UINT16`.
- **Values:** `0..255` effectively; shipped data uses 1..54.
- **Default:** `0`.
- **Source:** `Tactical/XML_Armour.cpp:110`, `Tactical/Weapons.h:408`
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubCoverage`

- **Type:** `Integer` (`UINT8`), percent
- **Description:** Percentage of the body slot covered by the armour. It is used as a
  multiplier in protection calculations and in the item comparison panel.
- **Values:** `0..100`; shipped data uses 25..100.
- **Default:** `100` when `ENABLE_ARMOR_COVERAGE` is false. The option defaults to `FALSE`
  (`Ja2/GameSettings.cpp:2258`), so a default game forces every row to 100 and ignores the XML
  value. When coverage is enabled and the element is missing, the zeroed record yields `0`.
- **Source:** `Tactical/XML_Armour.cpp:114-118`, `Ja2/GameSettings.cpp:2258`
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubDegradePercent`

- **Type:** `Integer` (`UINT8`), percent
- **Description:** Percentage of the applied protection subtracted from the armour's status
  when it absorbs a hit. Higher values wear out faster.
- **Values:** `0..100`; shipped data uses 5..50.
- **Default:** `0`.
- **Source:** `Tactical/XML_Armour.cpp:123`, `Tactical/Weapons.cpp:7929`, `:7945`
- **Confidence:** `source`

## Relations

- `Items.Items.ubClassIndex` -> `Items.Armours.uiIndex` for items whose `usItemClass` is
  `IC_ARMOUR`.
- `Items.Armours.ubArmourClass` -> `Lookup.ArmourClass` (`../Lookup.md#lookuparmourclass`).
- The item's `usItemClass` and `ubClassIndex` live in `Items.Items`; see the future
  `Items/Items.md`.

## Localization

No strings in this table. Armour names and descriptions are fields of `Items.Items` and are
translated there.

## Versions

v4 and v5 use the same five fields and the same `ARMOURLIST/ARMOUR` structure. The shipped
values differ (for example v5 has 201 records; compare the two files when a specific value
matters). No overlay covers `Items/Armours.xml`.

## Sources

- `Tactical/XML_Armour.cpp` - parser and save function.
- `Tactical/Weapons.h:102-110` - `ARMOURCLASS_*`; `:405-412` - `ARMOURTYPE`.
- `Tactical/Weapons.cpp:7929`, `:7945` - degradation formula.
- `Ja2/GameSettings.cpp:2258` - `ENABLE_ARMOR_COVERAGE` default.
- `TableData/Items/Armours.xml` - shipped values.

## Open questions

- The exact damage formula and how `ubProtection`, `ubCoverage` and ammo type combine is
  spread across `Weapons.cpp`; a full formula is outside this table page.
- Duplicate `uiIndex` handling is "last record wins" by code; the shipped data has no
  duplicates, so this is not exercised.
