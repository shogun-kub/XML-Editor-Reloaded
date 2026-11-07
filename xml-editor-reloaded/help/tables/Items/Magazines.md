---
schema: 1
lang: en
family: items-magazines
title: "Items.Magazines - magazines and ammo items"
tables:
  - Items.Magazines
files:
  - Items/Magazines.xml
root: MAGAZINELIST
record: MAGAZINE
key: uiIndex
sources:
  - Tactical/XML_Magazine.cpp
  - Tactical/Weapons.h
  - Tactical/Items.h
  - Tactical/XML_AmmoStrings.cpp
status: draft
updated: 2026-11-07
---

# Items.Magazines - magazines and ammo items

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MAGAZINELIST/MAGAZINE/uiIndex`](#magazinelistmagazineuiindex)
  - [`MAGAZINELIST/MAGAZINE/ubCalibre`](#magazinelistmagazineubcalibre)
  - [`MAGAZINELIST/MAGAZINE/ubMagSize`](#magazinelistmagazineubmagsize)
  - [`MAGAZINELIST/MAGAZINE/ubAmmoType`](#magazinelistmagazineubammotype)
  - [`MAGAZINELIST/MAGAZINE/ubMagType`](#magazinelistmagazineubmagtype)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Magazines.xml` defines ammo items: magazines, loose bullets, ammo boxes and crates. An item
whose `usItemClass` is `IC_AMMO` points to a row through `ubClassIndex`; the row is stored in
the runtime `Magazine[]` array. The row decides which weapon can load the item (`ubCalibre`,
`ubMagSize`, `ubAmmoType`) and how the item behaves in bulk (`ubMagType`).

The v5 file has 613 records.

## Tables

| Table | File | Records | Notes |
|---|---|---:|---|
| `Items.Magazines` | `Items/Magazines.xml` | 613 | v4 and v5 have the same schema; values may differ |

## Structure

```
MAGAZINELIST
└── MAGAZINE          one ammo item
    ├── uiIndex       table key / initial array index
    ├── ubCalibre     caliber group, indexes AmmoStrings
    ├── ubMagSize     capacity in rounds
    ├── ubAmmoType    ammo type, indexes AmmoTypes
    └── ubMagType     0 magazine, 1 bullets, 2 box, 3 crate
```

The loader writes each record to `Magazine[uiIndex]`. Records outside the array are ignored.
The editor key is `uiIndex`; after loading, the runtime `MAGTYPE.uiIndex` field is reused as a
back-reference to the owning item ID, so do not read the runtime field as the XML key.

## Fields

### `MAGAZINELIST/MAGAZINE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** XML identity and the index in the runtime `Magazine[]` array. `Items.Items`
  uses `ubClassIndex` to point at this row for ammo items.
- **Values:** `0..MAXITEMS-1`; shipped data is contiguous.
- **Default:** none; a missing value leaves the array slot at zero.
- **Source:** `Tactical/XML_Magazine.cpp:101`, `:93-95`
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubCalibre`

- **Type:** `Integer` (`UINT8`)
- **Description:** Caliber group of the ammo. The game uses it together with `ubMagSize` and
  `ubAmmoType` to find a matching magazine for a weapon and to name the caliber in the UI.
  It indexes the `AmmoStrings` list, which is filled in file order by `XML_AmmoStrings.cpp`
  (the `uiIndex` element is not read by that loader).
- **Values:** `1..50` in the shipped data; the `AmmoStrings` file has a row for each used
  caliber.
- **Default:** `0` (no caliber).
- **Source:** `Tactical/XML_Magazine.cpp:106`, `Tactical/Weapons.h:396`,
  `Tactical/XML_AmmoStrings.cpp:80-96`
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubMagSize`

- **Type:** `Integer` (`UINT16`)
- **Description:** Capacity of the magazine in rounds. Loose bullets use 1; boxes and crates
  use a large capacity (the shipped data reaches 5000). The loader casts the value to
  `UINT16`.
- **Values:** `1..65535`; shipped data uses 1..5000.
- **Default:** `0`.
- **Source:** `Tactical/XML_Magazine.cpp:111`, `Tactical/Weapons.h:397`
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubAmmoType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Ammo type index into `Items.AmmoTypes`. It selects the damage, armour
  interaction, tracer and other projectile properties.
- **Values:** `0..34` in the shipped data; the valid range is the `AmmoTypes` row count.
- **Default:** `0` (regular ammo).
- **Source:** `Tactical/XML_Magazine.cpp:116`, `TableData/Items/AmmoTypes.xml`
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubMagType`

- **Type:** `Integer` (`UINT8`), enum `AMMO_*`
- **Description:** Kind of ammo item. Values at or above `AMMO_BOX` are treated as bulk ammo
  and use different inventory, reload and transport rules.
- **Values:** `0` magazine (`AMMO_MAGAZINE`), `1` bullet(s) (`AMMO_BULLET`), `2` box
  (`AMMO_BOX`), `3` crate (`AMMO_CRATE`).
- **Default:** `0` (magazine).
- **Source:** `Tactical/XML_Magazine.cpp:121`, `Tactical/Weapons.h:139-144`
- **Confidence:** `source`

## Relations

- `Items.Items.ubClassIndex` -> `Items.Magazines.uiIndex` for `IC_AMMO` items.
- `Items.Magazines.ubCalibre` -> `Items.AmmoStrings` (caliber name), filled in file order.
- `Items.Magazines.ubAmmoType` -> `Items.AmmoTypes.uiIndex`.
- `Items.Magazines.ubMagType` -> `Lookup.MagazineType`
  (`../Lookup.md#lookupmagazinetype`).
- `Items.Weapons` uses the same caliber/capacity/ammo type triple to find a magazine.

## Localization

No strings in this table. Caliber names are in `Items.AmmoStrings`, which the game's language
files provide; the table itself is not covered by `Russian.*` overlays.

## Versions

v4 and v5 use the same five fields and the same `MAGAZINELIST/MAGAZINE` structure. Values
differ; compare the files when a specific magazine matters.

## Sources

- `Tactical/XML_Magazine.cpp` - parser, array write and save function.
- `Tactical/Weapons.h:394-403` - `MAGTYPE`; `:139-144` - `AMMO_*`.
- `Tactical/XML_AmmoStrings.cpp` - caliber list order.
- `TableData/Items/Magazines.xml`, `AmmoStrings.xml`, `AmmoTypes.xml` - shipped data.

## Open questions

- `ubCalibre` is an index into a list filled in file order, not an explicit ID. Reordering
  `AmmoStrings.xml` would silently change every caliber name. This is a data-integrity risk
  worth reporting, but the loader is authoritative.
- The exact match rule for `FindAmmo` (caliber, capacity, ammo type) is in `Items.cpp`; this
  page documents the fields, not the search algorithm.
