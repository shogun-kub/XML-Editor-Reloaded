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
  - Game code (explosive loader and `EXPLOSIVETYPE`)
  - Item metadata and lookup tables
status: reviewed
updated: 2026-11-08
---

# Explosives - explosion definitions

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`EXPLOSIVELIST/EXPLOSIVE/uiIndex`](#explosivelistexplosiveuiindex)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubType`](#explosivelistexplosiveubtype)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubDamage`](#explosivelistexplosiveubdamage)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubStunDamage`](#explosivelistexplosiveubstundamage)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubRadius`](#explosivelistexplosiveubradius)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubVolume`](#explosivelistexplosiveubvolume)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubVolatility`](#explosivelistexplosiveubvolatility)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubAnimationID`](#explosivelistexplosiveubanimationid)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubDuration`](#explosivelistexplosiveubduration)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubStartRadius`](#explosivelistexplosiveubstartradius)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubMagSize`](#explosivelistexplosiveubmagsize)
  - [`EXPLOSIVELIST/EXPLOSIVE/fExplodeOnImpact`](#explosivelistexplosivefexplodeonimpact)
  - [`EXPLOSIVELIST/EXPLOSIVE/usNumFragments`](#explosivelistexplosiveusnumfragments)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubFragType`](#explosivelistexplosiveubfragtype)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubFragDamage`](#explosivelistexplosiveubfragdamage)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubFragRange`](#explosivelistexplosiveubfragrange)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubHorizontalDegree`](#explosivelistexplosiveubhorizontaldegree)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubVerticalDegree`](#explosivelistexplosiveubverticaldegree)
  - [`EXPLOSIVELIST/EXPLOSIVE/bIndoorModifier`](#explosivelistexplosivebindoormodifier)
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
| `Explosives` | `Items/Explosives.xml` | v4 and v5 have the same 19 field paths, including `bIndoorModifier`; the reference files contain the same values. |

## Structure

```
EXPLOSIVELIST
└── EXPLOSIVE
    ├── uiIndex
    ├── ubType
    ├── ubDamage
    ├── ubStunDamage
    ├── ubRadius
    ├── ubVolume
    ├── ubVolatility
    ├── ubAnimationID
    ├── ubDuration
    ├── ubStartRadius
    ├── ubMagSize
    ├── fExplodeOnImpact
    ├── usNumFragments
    ├── ubFragType
    ├── ubFragDamage
    ├── ubFragRange
    ├── ubHorizontalDegree
    ├── ubVerticalDegree
    └── bIndoorModifier
```

## Game options

No table-level global option is documented for this table. The records are independent of the inventory model. Fragment spawning is disabled in multiplayer (`FireFragments` returns immediately when networked); the other fields are used normally.

## Fields

### `EXPLOSIVELIST/EXPLOSIVE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the explosion row in the runtime explosion table. `Items.ubClassIndex` of an explosive item points here.
- **Values:** `0..16000`; the loader writes into `Explosive[MAXITEMS]` with `MAXITEMS` = `16001`, and rows with a larger `uiIndex` are silently ignored.
- **Default:** missing or empty means `0`; the parser zeroes the record, so such a row is stored at index `0`.
- **Source:** game code (explosive loader and runtime `Explosive` array); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Explosion type from `Lookup.ExplosionType`; it selects damage handling, gas/smoke behaviour and visual/sound treatment. The engine enum names for the last entries are `EXPLOSV_BURNABLEGAS` (`8`), `EXPLOSV_SIGNAL_SMOKE` (`10`), `EXPLOSV_SMOKE_DEBRIS` (`11`) and `EXPLOSV_SMOKE_FIRERETARDANT` (`12`).
- **Values:** IDs from `Lookup.ExplosionType`: `0` Normal, `1` Stun, `2` Tear Gas, `3` Mustard Gas, `4` Flare, `5` Noise, `6` Smoke, `7` Creature Gas, `8` Fire, `9` Flashbang, `10` Signal Shell, `11` Roof Collapse, `12` White Smoke.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; `EXPLOSV_*` enum); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubDamage`

- **Type:** `Integer` (`UINT8`)
- **Description:** Base wound damage applied by the explosion. The game adds a random `0..99%` roll and reduces the result by distance within `ubRadius`; when the explosion is indoors, `bIndoorModifier` is applied first.
- **Numeric semantics:** Higher values mean more damage; the loader stores this field as `UINT8`.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; explosion damage calculation); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubStunDamage`

- **Type:** `Integer` (`UINT8`)
- **Description:** Base stun (breath) damage applied by the explosion. The same random roll and radius falloff as `ubDamage` apply, and `bIndoorModifier` is applied indoors.
- **Numeric semantics:** Higher values mean more stun/breath damage; the loader stores this field as `UINT8`.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; explosion breath-damage calculation); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubRadius`

- **Type:** `Integer` (`UINT8`)
- **Description:** Effect radius in tiles. Damage falls off linearly with distance up to this radius and is divided by the radius beyond it. `0` leaves the damage unreduced.
- **Numeric semantics:** Higher values mean a wider blast; the loader clamps the parsed value to `0..255`.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; distance falloff); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Noise volume of the explosion passed to `MakeNoise` with `NOISE_EXPLOSION`. It affects AI hearing and is not the sound-playback volume.
- **Numeric semantics:** Higher values mean a louder explosion; `0` makes it silent to the noise system.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; explosion noise); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubVolatility`

- **Type:** `Integer` (`UINT8`)
- **Description:** Base volatility used by the chain-reaction check when the explosive item takes damage. It is not a direct percentage: the game converts it to `50 + (value - 1) * 10` on the ground or `3 + (value - 1) * 3` when carried, then scales that by the item's condition and the damage taken.
- **Numeric semantics:** Higher values mean a more volatile explosive; `0` disables accidental detonation.
- **Values:** `0..255`; observed values are `0`, `1`, `2` and `10`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; `CheckForChainReaction` and `CalcTotalVolatility`); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubAnimationID`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the blast animation and sound row in `ExplosionData`. It becomes the explosion's `ubTypeID` and selects the blast STI, frame counts and sound IDs.
- **Values:** Index of a row in [ExplosionData](ExplosionData.md); shipped values are `0..15`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; `GenerateExplosion` and `ExplosionData` lookup); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubDuration`

- **Type:** `Integer` (`UINT8`)
- **Description:** Duration of a lingering effect. It is reused as the smoke/gas duration (in turns), the light-effect duration, the flashbang blind counter and the signal-shell wave count; `0` means no lingering effect.
- **Numeric semantics:** Higher values mean a longer-lasting effect, but the unit depends on the explosion type that consumes it.
- **Values:** `0..255`; observed values are `0..10`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; smoke, light and blinding code); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubStartRadius`

- **Type:** `Integer` (`UINT8`)
- **Description:** Initial radius of the effect at the moment of detonation. Smoke and light start at this radius and the smoke effect expands toward `ubRadius`.
- **Numeric semantics:** Higher values mean a larger initial area; the loader stores this field as `UINT8`.
- **Values:** `0..255`; observed values are `0..7`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; smoke and light effect setup); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubMagSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of charges an explosive item contributes when loaded into a multi-shot grenade launcher. Values above `1` let the launcher fire the item repeatedly, and each shot reduces the item's status by `100 / ubMagSize`. It is not the blast radius.
- **Numeric semantics:** Higher values mean more shots per loaded item.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; `GetExpMagSize` and launcher ammo deduction); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/fExplodeOnImpact`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, an `IC_EXPLOSV` item detonates on impact instead of waiting for a fuse or remote trigger. Items of class `IC_BOMB` always detonate on impact regardless of this flag.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; `OBJECT_DETONATE_ON_IMPACT`); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/usNumFragments`

- **Type:** `Integer` (`UINT16`)
- **Description:** Number of fragment projectiles produced by the explosion. Fragments spawn only for `EXPLOSV_NORMAL` explosions, and they are suppressed in water and in multiplayer.
- **Numeric semantics:** Higher values mean more fragments; each fragment uses `ubFragType`, `ubFragDamage` and `ubFragRange`.
- **Values:** `0..10000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; `FireFragments` and fragment damage); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubFragType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Ammo type used for the fragment projectiles; it supplies the fragment's damage and impact behaviour.
- **Values:** Index of a row in [AmmoTypes](AmmoTypes.md); the fragment projectile uses that ammo type.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; fragment bullet setup); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubFragDamage`

- **Type:** `Integer` (`UINT16`)
- **Description:** Impact/damage value used for each fragment projectile.
- **Numeric semantics:** Higher values mean more damage per fragment; the loader stores this field as `UINT16`.
- **Values:** `0..250`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; fragment bullet impact); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubFragRange`

- **Type:** `Integer` (`UINT16`)
- **Description:** Maximum travel range of fragments, in tenths of a tile (`10` units = one tile). The game uses it as the fragment bullet's range.
- **Numeric semantics:** Higher values mean a longer fragment range; the display divides the value by `CELL_X_SIZE` (`10`) to show tiles.
- **Values:** `0..250`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; `FireFragments` and fragment range display); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubHorizontalDegree`

- **Type:** `Integer` (`UINT16`)
- **Description:** Full horizontal arc width of the fragment cone, in degrees. The code takes the value modulo `360`, so `360` behaves like `0` and `359` gives an almost full circle.
- **Numeric semantics:** Higher values mean a wider horizontal spread; the loader stores this field as `UINT16`.
- **Values:** `0..360`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; directional fragment spread); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubVerticalDegree`

- **Type:** `Integer` (`UINT16`)
- **Description:** Full vertical arc width of the fragment cone, in degrees. The code takes the value modulo `180`, so `180` behaves like `0`.
- **Numeric semantics:** Higher values mean a wider vertical spread; the loader stores this field as `UINT16`.
- **Values:** `0..360` in the metadata; the code uses the value modulo `180`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; directional fragment spread); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/bIndoorModifier`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Multiplier applied to damage and stun when the explosion occurs indoors. `0` leaves the blast unchanged; `0.25` adds `25%`, and `-1` makes the indoor damage zero.
- **Numeric semantics:** Positive values increase the indoor blast, negative values reduce it, and `0` is neutral. The modifier is applied only when `InARoom` is true.
- **Values:** `-1..10`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosive loader; `InARoom` damage adjustment); explosive table data.
- **Confidence:** `source`

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

v4 and v5 use the same 19 field paths, and `bIndoorModifier` is present in both. The reference v4 and v5 files contain identical values.

## Sources

- Explosive table data.
- Game code (explosive loader, explosion table struct, explosion, fragment and chain-reaction functions).
- Item metadata and lookup tables.
- Field types and limits: item metadata; numeric limits use the metadata validation ranges.
- Semantic descriptions use the game code; entries marked `inferred` do not claim a code-verified formula.

## Open questions

`ubDuration` is a single value reused for smoke duration, light-effect duration, the flashbang blind counter and signal-shell waves. The table cannot express different durations for those effects; the unit and meaning depend on the explosion type that consumes it.
