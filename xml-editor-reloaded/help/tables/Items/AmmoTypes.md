---
schema: 1
lang: en
family: items-ammo
title: "AmmoTypes - ammunition behaviour"
tables:
  - AmmoTypes
table_ids:
  - Items.AmmoTypes
files:
  - Items/AmmoTypes.xml
root: AMMOTYPELIST
record: AMMOTYPE
key: uiIndex
sources:
  - Ammo type table data
  - AmmoTypes schema comment
  - Item metadata and lookup tables
status: reviewed
updated: 2026-11-07
---

# AmmoTypes - ammunition behaviour

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`AMMOTYPELIST/AMMOTYPE/uiIndex`](#ammotypelistammotypeuiindex)
  - [`AMMOTYPELIST/AMMOTYPE/name`](#ammotypelistammotypename)
  - [`AMMOTYPELIST/AMMOTYPE/red`](#ammotypelistammotypered)
  - [`AMMOTYPELIST/AMMOTYPE/green`](#ammotypelistammotypegreen)
  - [`AMMOTYPELIST/AMMOTYPE/blue`](#ammotypelistammotypeblue)
  - [`AMMOTYPELIST/AMMOTYPE/structureImpactReductionMultiplier`](#ammotypelistammotypestructureimpactreductionmultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/structureImpactReductionDivisor`](#ammotypelistammotypestructureimpactreductiondivisor)
  - [`AMMOTYPELIST/AMMOTYPE/armourImpactReductionMultiplier`](#ammotypelistammotypearmourimpactreductionmultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/armourImpactReductionDivisor`](#ammotypelistammotypearmourimpactreductiondivisor)
  - [`AMMOTYPELIST/AMMOTYPE/beforeArmourDamageMultiplier`](#ammotypelistammotypebeforearmourdamagemultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/beforeArmourDamageDivisor`](#ammotypelistammotypebeforearmourdamagedivisor)
  - [`AMMOTYPELIST/AMMOTYPE/afterArmourDamageMultiplier`](#ammotypelistammotypeafterarmourdamagemultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/afterArmourDamageDivisor`](#ammotypelistammotypeafterarmourdamagedivisor)
  - [`AMMOTYPELIST/AMMOTYPE/zeroMinimumDamage`](#ammotypelistammotypezerominimumdamage)
  - [`AMMOTYPELIST/AMMOTYPE/usPiercePersonChanceModifier`](#ammotypelistammotypeuspiercepersonchancemodifier)
  - [`AMMOTYPELIST/AMMOTYPE/standardIssue`](#ammotypelistammotypestandardissue)
  - [`AMMOTYPELIST/AMMOTYPE/numberOfBullets`](#ammotypelistammotypenumberofbullets)
  - [`AMMOTYPELIST/AMMOTYPE/multipleBulletDamageMultiplier`](#ammotypelistammotypemultiplebulletdamagemultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/multipleBulletDamageDivisor`](#ammotypelistammotypemultiplebulletdamagedivisor)
  - [`AMMOTYPELIST/AMMOTYPE/highExplosive`](#ammotypelistammotypehighexplosive)
  - [`AMMOTYPELIST/AMMOTYPE/explosionSize`](#ammotypelistammotypeexplosionsize)
  - [`AMMOTYPELIST/AMMOTYPE/dart`](#ammotypelistammotypedart)
  - [`AMMOTYPELIST/AMMOTYPE/knife`](#ammotypelistammotypeknife)
  - [`AMMOTYPELIST/AMMOTYPE/monsterSpit`](#ammotypelistammotypemonsterspit)
  - [`AMMOTYPELIST/AMMOTYPE/acidic`](#ammotypelistammotypeacidic)
  - [`AMMOTYPELIST/AMMOTYPE/ignoreArmour`](#ammotypelistammotypeignorearmour)
  - [`AMMOTYPELIST/AMMOTYPE/lockBustingPower`](#ammotypelistammotypelockbustingpower)
  - [`AMMOTYPELIST/AMMOTYPE/tracerEffect`](#ammotypelistammotypetracereffect)
  - [`AMMOTYPELIST/AMMOTYPE/spreadPattern`](#ammotypelistammotypespreadpattern)
  - [`AMMOTYPELIST/AMMOTYPE/temperatureModificator`](#ammotypelistammotypetemperaturemodificator)
  - [`AMMOTYPELIST/AMMOTYPE/dirtModificator`](#ammotypelistammotypedirtmodificator)
  - [`AMMOTYPELIST/AMMOTYPE/ammoflag`](#ammotypelistammotypeammoflag)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierLife`](#ammotypelistammotypeddamagemodifierlife)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierBreath`](#ammotypelistammotypeddamagemodifierbreath)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierTank`](#ammotypelistammotypeddamagemodifiertank)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierArmouredVehicle`](#ammotypelistammotypeddamagemodifierarmouredvehicle)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierCivilianVehicle`](#ammotypelistammotypeddamagemodifiercivilianvehicle)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierZombie`](#ammotypelistammotypeddamagemodifierzombie)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AmmoTypes.xml` defines what a projectile does after it is fired: armour penetration, damage against different target classes, fragment behaviour, trails and special effects. Magazines select a row through `ubAmmoType`; explosives reuse a row as their fragment type.

## Tables

| Table | File | Notes |
|---|---|---|
| `AmmoTypes` | `Items/AmmoTypes.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
AMMOTYPELIST
└── AMMOTYPE
    ├── uiIndex
    ├── name
    ├── red
    ├── green
    ├── blue
    ├── structureImpactReductionMultiplier
    ├── structureImpactReductionDivisor
    ├── armourImpactReductionMultiplier
    ├── armourImpactReductionDivisor
    ├── beforeArmourDamageMultiplier
    ├── beforeArmourDamageDivisor
    ├── afterArmourDamageMultiplier
    ├── afterArmourDamageDivisor
    ├── zeroMinimumDamage
    ├── usPiercePersonChanceModifier
    ├── standardIssue
    ├── numberOfBullets
    ├── multipleBulletDamageMultiplier
    ├── multipleBulletDamageDivisor
    ├── highExplosive
    ├── explosionSize
    ├── dart
    ├── knife
    ├── monsterSpit
    ├── acidic
    ├── ignoreArmour
    ├── lockBustingPower
    ├── tracerEffect
    ├── spreadPattern
    ├── temperatureModificator
    ├── dirtModificator
    ├── ammoflag
    ├── dDamageModifierLife
    ├── dDamageModifierBreath
    ├── dDamageModifierTank
    ├── dDamageModifierArmouredVehicle
    ├── dDamageModifierCivilianVehicle
    └── dDamageModifierZombie
```

## Game options

No table-level global option is documented for this table. Its fields are read in both inventory models and in multiplayer; the only version gate is the field set described under [Versions](#versions).

## Fields

The schema comment is the primary description of this table; the damage-stage fields are multipliers/divisors, not flat penalties. See [FORMULAS](../../FORMULAS.md).

### `AMMOTYPELIST/AMMOTYPE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the ammunition behaviour row. Magazines reference it through `ubAmmoType`; explosives can reference it as their fragment type.
- **Values:** Non-negative integer; unique within the file. Referenced by `Magazines.ubAmmoType` and `Explosives.ubFragType`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/name`

- **Type:** `String` (`String`)
- **Description:** Display name of the ammunition behaviour, shown in item descriptions and editor lists.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/red`

- **Type:** `Integer` (`UINT8`)
- **Description:** Red component of the ammunition-count colour, `0..255`.
- **Values:** `0..255`; byte colour component.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/green`

- **Type:** `Integer` (`UINT8`)
- **Description:** Green component of the ammunition-count colour, `0..255`.
- **Values:** `0..255`; byte colour component.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/blue`

- **Type:** `Integer` (`UINT8`)
- **Description:** Blue component of the ammunition-count colour, `0..255`.
- **Values:** `0..255`; byte colour component.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/structureImpactReductionMultiplier`

- **Type:** `Integer` (`INT16`)
- **Description:** Multiplier in the structure-damage reduction fraction. Together with the divisor it controls how much bullet damage is lost when hitting a structure.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/structureImpactReductionDivisor`

- **Type:** `Integer` (`INT16`)
- **Description:** Divisor paired with `structureImpactReductionMultiplier` for structure damage reduction.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/armourImpactReductionMultiplier`

- **Type:** `Integer` (`UINT16`)
- **Description:** Multiplier applied to the protection value of body armour that the bullet hits.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/armourImpactReductionDivisor`

- **Type:** `Integer` (`UINT16`)
- **Description:** Divisor paired with `armourImpactReductionMultiplier` for armour protection.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/beforeArmourDamageMultiplier`

- **Type:** `Integer` (`INT16`)
- **Description:** Multiplier for damage applied before armour is hit. It is used only when `highExplosive` is greater than zero.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/beforeArmourDamageDivisor`

- **Type:** `Integer` (`INT16`)
- **Description:** Divisor paired with `beforeArmourDamageMultiplier` for pre-armour damage.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/afterArmourDamageMultiplier`

- **Type:** `Integer` (`UINT16`)
- **Description:** Multiplier for damage applied after armour is hit. It is not used against armed vehicles.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/afterArmourDamageDivisor`

- **Type:** `Integer` (`UINT16`)
- **Description:** Divisor paired with `afterArmourDamageMultiplier` for post-armour damage.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/zeroMinimumDamage`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, bullet damage may be zero; otherwise the damage floor is at least one.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/usPiercePersonChanceModifier`

- **Type:** `Integer` (`UINT16`)
- **Description:** Extra chance for the bullet to continue flying after hitting a person, provided the weapon is not a rocket launcher or cannon and the bullet is still powerful enough.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/standardIssue`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, magazines of this ammo are considered when the AI’s starting inventory is generated.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/numberOfBullets`

- **Type:** `Integer` (`INT16`)
- **Description:** Number of projectiles fired per shot. Values above one represent buckshot or a multi-projectile load.
- **Values:** Projectile count per shot; `1` is a single bullet, values above `1` are buckshot or a multi-projectile load.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/multipleBulletDamageMultiplier`

- **Type:** `Integer` (`UINT16`)
- **Description:** Multiplier applied immediately after firing when more than one projectile is fired. Together with the divisor it scales multi-projectile damage.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/multipleBulletDamageDivisor`

- **Type:** `Integer` (`UINT16`)
- **Description:** Divisor paired with `multipleBulletDamageMultiplier` for multi-projectile damage.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/highExplosive`

- **Type:** `Integer` (`UINT16`)
- **Description:** Number of explosive items used when this ammunition explodes. Zero means the round does not use the explosive path.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/explosionSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Explosion size class used by the ammunition’s explosive effect.
- **Values:** IDs from `Lookup.ExplosionSize`: `0` None, `1` Standard, `2` HighExplosive, `3` Big Bada Boom, `4` Flame / Flame retardant.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dart`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, damage above zero makes the target drowsy; the projectile is very accurate and can put the target to sleep. It does not apply to armed vehicles.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/knife`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the projectile behaves as a stealth knife attack: the target must be seen, the projectile is not a fragment, and a hit can instantly kill.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/monsterSpit`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the projectile uses creature-gas behaviour: increased damage from creature gas items, blinding, headgear damage and creature-gas spawning.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/acidic`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, armour is destroyed four times as fast.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/ignoreArmour`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, torso and leg armour are ignored.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/lockBustingPower`

- **Type:** `Integer` (`UINT16`)
- **Description:** Extra damage dealt to locks.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/tracerEffect`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, bullets can be tracers. Tracers are not selected for AI single-shot weapons, and flash suppressors do not hide them.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/spreadPattern`

- **Type:** `String` (`String`)
- **Description:** Spread-pattern key from `SpreadPatterns.xml`. It defines the shot dispersion pattern; an empty value uses the default pattern.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/temperatureModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Additive modifier for temperature generation. `1.0` means +100%.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dirtModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Additive modifier for dirt generation. `1.0` means +100%.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/ammoflag`

- **Type:** `Flags` (`UINT16`)
- **Description:** Bitmask of special ammunition properties: cryo, blind, anti-materiel, white-smoke trail and fire trail.
- **Values:** Bitmask: `1` cryo, `2` blind, `4` anti-materiel, `8` white-smoke trail, `16` fire trail. Combine bits with bitwise OR. The `AmmoFlag` lookup labels bit positions; it is not the raw mask value.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierLife`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Multiplier for damage dealt to health.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierBreath`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Multiplier for damage dealt to breath.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierTank`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Additional multiplier for health damage when the target is a tank.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierArmouredVehicle`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Additional multiplier for health damage when the target is an armed vehicle or robot but not a tank.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierCivilianVehicle`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Additional multiplier for health damage when the target is an unarmed vehicle.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierZombie`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Additional multiplier for health damage when the target is a zombie.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

## Relations

- `Magazines.ubAmmoType` points to `AmmoTypes.uiIndex`.
- An ammunition item in `Items` uses a magazine row, which in turn selects the ammo type; the ammo type is not the item row itself.
- `Explosives.ubFragType` can point to an `AmmoTypes.uiIndex` to reuse fragment behaviour.
- `spreadPattern` names a pattern in the future `SpreadPatterns` document (B3).

## Related formulas

- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt)
- [Armour protection](../../FORMULAS.md#armour-protection)

## Localization

`name` is a display label. The numeric and boolean fields are not localized.

## Versions

v4 and v5 use the same 38 field paths; values may differ.

## Sources

- Ammo type table data.
- AmmoTypes schema comment.
- Item metadata and lookup tables.
- Field types and limits: item metadata; numeric limits use the metadata validation ranges.
- Semantic descriptions use the table schema comments and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The order of the armour-damage stages and the precise mapping between `ammoflag` bits and the `AmmoFlag` lookup labels are not stated by the table comment. The bit values above come from the schema comment; treat lookup IDs as labels, not mask values.
