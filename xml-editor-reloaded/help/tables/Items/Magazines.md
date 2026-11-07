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
  - Game code (magazine loader and ammo lookups)
  - Ammo type and caliber data
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
- [Related formulas](#related-formulas)
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
| `Magazines` | `Items/Magazines.xml` | 613 | v4 and v5 have the same schema; values may differ |

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
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubCalibre`

- **Type:** `Integer` (`UINT8`)
- **Description:** Caliber group of the ammo. The game uses it together with `ubMagSize` and
  `ubAmmoType` to find a matching magazine for a weapon and to name the caliber in the UI.
  It indexes the `AmmoStrings` list, which is filled in file order by the caliber loader
  (the `uiIndex` element is not read by that loader).
- **Values:** `1..50` in the shipped data; the `AmmoStrings` file has a row for each used
  caliber.
- **Default:** `0` (no caliber).
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubMagSize`

- **Type:** `Integer` (`UINT16`)
- **Description:** Capacity of the magazine in rounds. Loose bullets use 1; boxes and crates
  use a large capacity (the shipped data reaches 5000). The loader casts the value to
  `UINT16`.
- **Values:** `1..65535`; shipped data uses 1..5000.
- **Default:** `0`.
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubAmmoType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Ammo type index into `Items.AmmoTypes`. It selects the damage, armour
  interaction, tracer and other projectile properties.
- **Values:** `0..34` in the shipped data; the valid range is the `AmmoTypes` row count.
- **Default:** `0` (regular ammo).
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubMagType`

- **Type:** `Integer` (`UINT8`), enum `AMMO_*`
- **Description:** Kind of ammo item. Values at or above `AMMO_BOX` are treated as bulk ammo
  and use different inventory, reload and transport rules.
- **Values:** `0` magazine (`AMMO_MAGAZINE`), `1` bullet(s) (`AMMO_BULLET`), `2` box
  (`AMMO_BOX`), `3` crate (`AMMO_CRATE`).
- **Default:** `0` (magazine).
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

## Relations

- `Items.Items.ubClassIndex` -> `Items.Magazines.uiIndex` for `IC_AMMO` items.
- `Items.Magazines.ubCalibre` -> `Items.AmmoStrings` (caliber name), filled in file order.
- `Items.Magazines.ubAmmoType` -> `Items.AmmoTypes.uiIndex`.
- `Items.Magazines.ubMagType` -> `Lookup.MagazineType`
  (`../Lookup.md#lookupmagazinetype`).
- `Items.Weapons` uses the same caliber/capacity/ammo type triple to find a magazine.

## Related formulas

The ammo values feed a multi-stage damage pipeline documented in
[Related formulas](../../FORMULAS.md):

- [Armour protection](../../FORMULAS.md#armour-protection) - how `ubAmmoType` changes the
  protection value and armour wear.
- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt) - where
  `ubAmmoType` and the projectile count from the ammo type enter the damage result.

The field entries above define the inputs; they do not repeat the formulas.

## Localization

No strings in this table. Caliber names are in `Items.AmmoStrings`, which the game's language
files provide; the table itself is not covered by `Russian.*` overlays.

## Versions

v4 and v5 use the same five fields and the same `MAGAZINELIST/MAGAZINE` structure. Values
differ; compare the files when a specific magazine matters.

## Sources

- Game code: the magazine loader and the ammo/caliber lookup code.
- Table data: `Items/Magazines.xml`, `Items/AmmoStrings.xml`, `Items/AmmoTypes.xml`.
- Damage and armour interaction: [Related formulas](../../FORMULAS.md).

## Open questions

- `ubCalibre` is an index into a list filled in file order, not an explicit ID. Reordering
  `AmmoStrings.xml` would silently change every caliber name. This is a data-integrity risk
  worth reporting, but the loader is authoritative.
- The exact ammo match rule (caliber, capacity, ammo type) is in the item lookup code; this
  page documents the fields, not the search algorithm.
