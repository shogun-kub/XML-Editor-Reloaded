---
schema: 1
lang: en
family: items-explosives
title: "Explosives - explosion definitions"
tables:
  - Explosives
table_ids:
  - Items.Explosives
files:
  - Items/Explosives.xml
root: EXPLOSIVELIST
record: EXPLOSIVE
key: uiIndex
sources:
  - Explosive table data
  - Item metadata and lookup tables
status: reviewed
updated: 2026-11-07
---

# Explosives - explosion definitions

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`EXPLOSIVELIST/EXPLOSIVE/bIndoorModifier`](#explosivelistexplosivebindoormodifier)
  - [`EXPLOSIVELIST/EXPLOSIVE/fExplodeOnImpact`](#explosivelistexplosivefexplodeonimpact)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubAnimationID`](#explosivelistexplosiveubanimationid)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubDamage`](#explosivelistexplosiveubdamage)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubDuration`](#explosivelistexplosiveubduration)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubFragDamage`](#explosivelistexplosiveubfragdamage)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubFragRange`](#explosivelistexplosiveubfragrange)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubFragType`](#explosivelistexplosiveubfragtype)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubHorizontalDegree`](#explosivelistexplosiveubhorizontaldegree)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubMagSize`](#explosivelistexplosiveubmagsize)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubRadius`](#explosivelistexplosiveubradius)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubStartRadius`](#explosivelistexplosiveubstartradius)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubStunDamage`](#explosivelistexplosiveubstundamage)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubType`](#explosivelistexplosiveubtype)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubVerticalDegree`](#explosivelistexplosiveubverticaldegree)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubVolatility`](#explosivelistexplosiveubvolatility)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubVolume`](#explosivelistexplosiveubvolume)
  - [`EXPLOSIVELIST/EXPLOSIVE/uiIndex`](#explosivelistexplosiveuiindex)
  - [`EXPLOSIVELIST/EXPLOSIVE/usNumFragments`](#explosivelistexplosiveusnumfragments)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Explosives.xml` defines the blast produced by explosive items, grenades, launcher warheads and environmental explosions. An explosive item points here through `ubClassIndex`; the row controls damage, radius, stun, fragments, gas/smoke duration and the animation used.

## Tables

| Table | File | Notes |
|---|---|---|
| `Explosives` | `Items/Explosives.xml` | v5 adds `bIndoorModifier`; v4 records omit that element. |

## Structure

```
EXPLOSIVELIST
└── EXPLOSIVE               one explosion definition
    ├── uiIndex             runtime explosion index
    ├── ubType              ExplosionType lookup
    ├── ubDamage            direct damage
    ├── ubRadius            potential radius
    ├── ...                 stun, fragments, gas and angles
    └── bIndoorModifier     indoor damage multiplier (v5)
```

## Game options

No table-level global option is documented for this table. Its fields are read in both inventory models and in multiplayer; the only version gate is the field set described under [Versions](#versions).

## Fields

### `EXPLOSIVELIST/EXPLOSIVE/bIndoorModifier`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Multiplier applied to the explosion when it occurs indoors. `1.0` leaves the blast unchanged. v4 records do not contain this element.
- **Values:** `-1..10`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/fExplodeOnImpact`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the explosive detonates on impact instead of waiting for a fuse or remote trigger.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubAnimationID`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the blast animation and sound row in `ExplosionData`.
- **Values:** Index of a row in [ExplosionData](ExplosionData.md).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubDamage`

- **Type:** `Integer` (`UINT8`)
- **Description:** Direct damage applied by the explosion.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubDuration`

- **Type:** `Integer` (`UINT8`)
- **Description:** Duration of a lingering effect such as smoke, gas or fire. Zero means no lingering effect.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubFragDamage`

- **Type:** `Integer` (`UINT8`)
- **Description:** Damage of each fragment.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubFragRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum travel range of fragments.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubFragType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Ammo type used for the fragments; it supplies the fragment projectile’s damage and impact behaviour.
- **Values:** Index of a row in [AmmoTypes](AmmoTypes.md); the fragment projectile uses that ammo type.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubHorizontalDegree`

- **Type:** `Integer` (`UINT8`)
- **Description:** Horizontal spread of the fragment cone, in degrees.
- **Values:** `0..360`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubMagSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Magazine capacity used by NAS explosive items. It is not the blast radius.
- **Values:** `0..65000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubRadius`

- **Type:** `Integer` (`UINT8`)
- **Description:** Potential radius of the explosion. The actual effect can grow toward this radius from `ubStartRadius`.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubStartRadius`

- **Type:** `Integer` (`UINT8`)
- **Description:** Initial radius of the effect at the moment of detonation. The effect expands toward `ubRadius`.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubStunDamage`

- **Type:** `Integer` (`UINT8`)
- **Description:** Stun damage applied by the explosion.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Explosion type from `Lookup.ExplosionType`; it selects damage handling, gas/smoke behaviour and visual/sound treatment.
- **Values:** IDs from `Lookup.ExplosionType`: `0` Normal, `1` Stun, `2` Tear Gas, `3` Mustard Gas, `4` Flare, `5` Noise, `6` Smoke, `7` Creature Gas, `8` Fire, `9` Flashbang, `10` Signal Shell, `11` Roof Collapse, `12` White Smoke.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubVerticalDegree`

- **Type:** `Integer` (`UINT8`)
- **Description:** Vertical spread of the fragment cone, in degrees.
- **Values:** `0..360`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubVolatility`

- **Type:** `Integer` (`UINT8`)
- **Description:** Volatility rating of the explosive. It expresses how sensitive the explosive is in the explosion model.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/ubVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Volume of the explosion sound/report. It is a presentation value, not damage.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

### `EXPLOSIVELIST/EXPLOSIVE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the explosion row in the runtime explosion table. `Items.ubClassIndex` of an explosive item points here.
- **Values:** Non-negative integer; unique within the file. `Items.ubClassIndex` of an explosive item points here.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/usNumFragments`

- **Type:** `Integer` (`UINT16`)
- **Description:** Number of fragments produced by the explosion.
- **Values:** `0..10000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosive table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `Items.ubClassIndex` of an explosive item points to `Explosives.uiIndex`.
- `ubAnimationID` points to [ExplosionData](ExplosionData.md).
- `ubFragType` points to [AmmoTypes](AmmoTypes.md) for fragment behaviour.
- `ubType` uses [Lookup.ExplosionType](../Lookup.md#explosiontype).

## Related formulas

No formula in [FORMULAS](../../FORMULAS.md) applies to this table.

## Localization

This table has no display strings; localized names and descriptions live on the item row in `Items`.

## Versions

v5 adds `bIndoorModifier`; v4 rows omit it. The other 18 field paths exist in both.

## Sources

- Explosive table data.
- Item metadata and lookup tables.
- Field types and limits: item metadata; numeric limits use the metadata validation ranges.
- Semantic descriptions use the table schema comments and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

`ubVolatility`, `ubVolume` and `bIndoorModifier` are documented by name and legacy labels, not by a formula in the table schema. Their exact formulas remain an open question.
