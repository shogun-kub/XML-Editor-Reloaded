---
schema: 1
lang: en
family: items-armours
title: "Armours - body armour"
tables:
  - Armours
table_ids:
  - Items.Armours
files:
  - Items/Armours.xml
root: ARMOURLIST
record: ARMOUR
key: uiIndex
sources:
  - Game code (armour loader and protection)
  - Armour table data
status: reviewed
updated: 2026-11-07
---

# Armours - body armour

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
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Armours.xml` defines the protective values of body armour. An armour item in `Items`
points to one row through its `ubClassIndex`; the row is stored in the runtime armour table.
The values drive damage reduction, armour wear, Bobby Ray's filters and the item description
panel.

`Armours` is a flat table: one `ARMOUR` record per armour type, keyed by `uiIndex`.

## Tables

| Table | File | Notes |
|---|---|---|
| `Armours` | `Items/Armours.xml` | v4 and v5 have the same schema; values may differ |

## Structure

```
ARMOURLIST
└── ARMOUR
    ├── uiIndex
    ├── ubArmourClass
    ├── ubProtection
    ├── ubCoverage
    └── ubDegradePercent
```

The loader writes each record to `Armour[uiIndex]` when the `ARMOUR` element closes. A
duplicate `uiIndex` overwrites the earlier row. Records with an
`uiIndex` outside the array are ignored.

## Fields

### `ARMOURLIST/ARMOUR/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the row in the runtime armour table and the identity of the
  XML record. `Items.ubClassIndex` of an armour item points here.
- **Values:** `0..N-1`; non-negative array index. The loader ignores values outside the array.
- **Default:** none; a missing value leaves the table slot at zero.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubArmourClass`

- **Type:** `Integer` (`UINT8`), enum `ARMOURCLASS_*`
- **Description:** Body slot/class of the armour. It is not the protection value; it tells
  the UI and equipment checks whether this is a helmet, vest, leggings or plate.
- **Values:** `0` helmet, `1` vest, `2` leggings, `3` plate, `4` monster, `5` vehicle.
- **Default:** `0` (helmet) when the element is missing, because the record is zeroed first.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubProtection`

- **Type:** `Integer` (`UINT16` in the struct, cast to `UINT8` by the loader)
- **Description:** Raw protection value of the armour. The loader casts the parsed value to `UINT8`, so values above 255 are truncated even though the struct field is `UINT16`.
- **Numeric semantics:** Higher values mean more protection/coverage, and negative values are not meaningful.
- **Values:** `0..255` effectively; the loader truncates to `UINT8`.
- **Default:** `0`.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubCoverage`

- **Type:** `Integer` (`UINT8`), percent
- **Description:** Percentage of the body slot covered by the armour. It is used as a multiplier in protection calculations and in the item comparison panel.
- **Numeric semantics:** Higher values mean more protection/coverage, and negative values are not meaningful.
- **Values:** percentage; stored as `UINT8`. No separate validation limit is documented.
- **Default:** `100` when `ENABLE_ARMOR_COVERAGE` is false. The option defaults to `FALSE`,
  so a default game forces every row to 100 and ignores the XML value. When coverage is
  enabled and the element is missing, the zeroed record yields `0`.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubDegradePercent`

- **Type:** `Integer` (`UINT8`), percent
- **Description:** Percentage of the applied protection subtracted from the armour's status when it absorbs a hit.
- **Numeric semantics:** Percentage; higher values wear the armour out faster, `0` means no degradation, and negative values are not meaningful for this unsigned percentage.
- **Values:** percentage; stored as `UINT8`. No separate validation limit is documented.
- **Default:** `0`.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

## Relations

- `Items.ubClassIndex` -> `Armours.uiIndex` for items whose `usItemClass` is
  `IC_ARMOUR`.
- `Armours.ubArmourClass` -> `ArmourClass` (`../Lookup.md#armourclass`).
- The item's `usItemClass` and `ubClassIndex` live in [Items](Items.md).

## Related formulas

The multi-stage damage model has its own chapters in [Related formulas](../../FORMULAS.md):

- [Armour protection](../../FORMULAS.md#armour-protection) - coverage, condition, ammo
  penetration and wear.
- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt) - how the
  armour result becomes final damage.

The field entries above define the inputs; they do not repeat the formulas.

## Localization

No strings in this table. Armour names and descriptions are fields of `Items` and are
translated there.

## Versions

v4 and v5 use the same five fields and the same `ARMOURLIST/ARMOUR` structure. Values
may differ; compare the two files when a specific value matters. No overlay covers
`Items/Armours.xml`.

## Sources

- Game code: the armour loader, the per-hit protection pass and the item setting that
  controls coverage.
- Table data: `Items/Armours.xml` values.
- The protection and wear calculation is described in
  [Related formulas](../../FORMULAS.md#armour-protection).

## Open questions

- The exact damage formula and how `ubProtection`, `ubCoverage` and ammo type combine are
  described in [Related formulas](../../FORMULAS.md); a full balance discussion is outside
  this table page.
- Duplicate `uiIndex` handling is "last record wins" in the loader; the consequence of a
  duplicate is the same as for any later row overwriting an earlier one.
