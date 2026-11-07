---
schema: 1
lang: en
family: items-weapons
title: "Weapons - weapon statistics"
tables:
  - Weapons
table_ids:
  - Items.Weapons
files:
  - Items/Weapons.xml
root: WEAPONLIST
record: WEAPON
key: uiIndex
sources:
  - Weapon table data
  - Weapon table schema comments
  - Item metadata and lookup tables
status: reviewed
updated: 2026-11-07
---

# Weapons - weapon statistics

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`WEAPONLIST/WEAPON/uiIndex`](#weaponlistweaponuiindex)
  - [`WEAPONLIST/WEAPON/szWeaponName`](#weaponlistweaponszweaponname)
  - [`WEAPONLIST/WEAPON/ubWeaponClass`](#weaponlistweaponubweaponclass)
  - [`WEAPONLIST/WEAPON/ubWeaponType`](#weaponlistweaponubweapontype)
  - [`WEAPONLIST/WEAPON/ubCalibre`](#weaponlistweaponubcalibre)
  - [`WEAPONLIST/WEAPON/ubReadyTime`](#weaponlistweaponubreadytime)
  - [`WEAPONLIST/WEAPON/ubShotsPer4Turns`](#weaponlistweaponubshotsper4turns)
  - [`WEAPONLIST/WEAPON/ubShotsPerBurst`](#weaponlistweaponubshotsperburst)
  - [`WEAPONLIST/WEAPON/ubBurstPenalty`](#weaponlistweaponubburstpenalty)
  - [`WEAPONLIST/WEAPON/ubBulletSpeed`](#weaponlistweaponubbulletspeed)
  - [`WEAPONLIST/WEAPON/ubImpact`](#weaponlistweaponubimpact)
  - [`WEAPONLIST/WEAPON/ubDeadliness`](#weaponlistweaponubdeadliness)
  - [`WEAPONLIST/WEAPON/bAccuracy`](#weaponlistweaponbaccuracy)
  - [`WEAPONLIST/WEAPON/ubMagSize`](#weaponlistweaponubmagsize)
  - [`WEAPONLIST/WEAPON/usRange`](#weaponlistweaponusrange)
  - [`WEAPONLIST/WEAPON/usReloadDelay`](#weaponlistweaponusreloaddelay)
  - [`WEAPONLIST/WEAPON/BurstAniDelay`](#weaponlistweaponburstanidelay)
  - [`WEAPONLIST/WEAPON/ubAttackVolume`](#weaponlistweaponubattackvolume)
  - [`WEAPONLIST/WEAPON/ubHitVolume`](#weaponlistweaponubhitvolume)
  - [`WEAPONLIST/WEAPON/sSound`](#weaponlistweaponssound)
  - [`WEAPONLIST/WEAPON/sBurstSound`](#weaponlistweaponsburstsound)
  - [`WEAPONLIST/WEAPON/sSilencedBurstSound`](#weaponlistweaponssilencedburstsound)
  - [`WEAPONLIST/WEAPON/sReloadSound`](#weaponlistweaponsreloadsound)
  - [`WEAPONLIST/WEAPON/sLocknLoadSound`](#weaponlistweaponslocknloadsound)
  - [`WEAPONLIST/WEAPON/SilencedSound`](#weaponlistweaponsilencedsound)
  - [`WEAPONLIST/WEAPON/bBurstAP`](#weaponlistweaponbburstap)
  - [`WEAPONLIST/WEAPON/bAutofireShotsPerFiveAP`](#weaponlistweaponbautofireshotsperfiveap)
  - [`WEAPONLIST/WEAPON/APsToReload`](#weaponlistweaponapstoreload)
  - [`WEAPONLIST/WEAPON/SwapClips`](#weaponlistweaponswapclips)
  - [`WEAPONLIST/WEAPON/MaxDistForMessyDeath`](#weaponlistweaponmaxdistformessydeath)
  - [`WEAPONLIST/WEAPON/AutoPenalty`](#weaponlistweaponautopenalty)
  - [`WEAPONLIST/WEAPON/NoSemiAuto`](#weaponlistweaponnosemiauto)
  - [`WEAPONLIST/WEAPON/EasyUnjam`](#weaponlistweaponeasyunjam)
  - [`WEAPONLIST/WEAPON/APsToReloadManually`](#weaponlistweaponapstoreloadmanually)
  - [`WEAPONLIST/WEAPON/ManualReloadSound`](#weaponlistweaponmanualreloadsound)
  - [`WEAPONLIST/WEAPON/nAccuracy`](#weaponlistweaponnaccuracy)
  - [`WEAPONLIST/WEAPON/bRecoilX`](#weaponlistweaponbrecoilx)
  - [`WEAPONLIST/WEAPON/bRecoilY`](#weaponlistweaponbrecoily)
  - [`WEAPONLIST/WEAPON/ubAimLevels`](#weaponlistweaponubaimlevels)
  - [`WEAPONLIST/WEAPON/ubRecoilDelay`](#weaponlistweaponubrecoildelay)
  - [`WEAPONLIST/WEAPON/Handling`](#weaponlistweaponhandling)
  - [`WEAPONLIST/WEAPON/usOverheatingJamThreshold`](#weaponlistweaponusoverheatingjamthreshold)
  - [`WEAPONLIST/WEAPON/usOverheatingDamageThreshold`](#weaponlistweaponusoverheatingdamagethreshold)
  - [`WEAPONLIST/WEAPON/usOverheatingSingleShotTemperature`](#weaponlistweaponusoverheatingsingleshottemperature)
  - [`WEAPONLIST/WEAPON/HeavyGun`](#weaponlistweaponheavygun)
  - [`WEAPONLIST/WEAPON/fBurstOnlyByFanTheHammer`](#weaponlistweaponfburstonlybyfanthehammer)
  - [`WEAPONLIST/WEAPON/BarrelConfiguration`](#weaponlistweaponbarrelconfiguration)
  - [`WEAPONLIST/WEAPON/szNWSSSound`](#weaponlistweaponsznwsssound)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Weapons.xml` holds the combat and presentation statistics of every weapon. `Items` points to a weapon row through `ubClassIndex`; the row feeds AP costs, accuracy, range, sound selection, burst behaviour and the item description panel.

## Tables

| Table | File | Notes |
|---|---|---|
| `Weapons` | `Items/Weapons.xml` | v4 and v5 have the same field paths; values differ. `BarrelConfiguration` is a repeated element. |

## Structure

```
WEAPONLIST
└── WEAPON
    ├── uiIndex
    ├── szWeaponName
    ├── ubWeaponClass
    ├── ubWeaponType
    ├── ubCalibre
    ├── ubReadyTime
    ├── ubShotsPer4Turns
    ├── ubShotsPerBurst
    ├── ubBurstPenalty
    ├── ubBulletSpeed
    ├── ubImpact
    ├── ubDeadliness
    ├── bAccuracy
    ├── ubMagSize
    ├── usRange
    ├── usReloadDelay
    ├── BurstAniDelay
    ├── ubAttackVolume
    ├── ubHitVolume
    ├── sSound
    ├── sBurstSound
    ├── sSilencedBurstSound
    ├── sReloadSound
    ├── sLocknLoadSound
    ├── SilencedSound
    ├── bBurstAP
    ├── bAutofireShotsPerFiveAP
    ├── APsToReload
    ├── SwapClips
    ├── MaxDistForMessyDeath
    ├── AutoPenalty
    ├── NoSemiAuto
    ├── EasyUnjam
    ├── APsToReloadManually
    ├── ManualReloadSound
    ├── nAccuracy
    ├── bRecoilX
    ├── bRecoilY
    ├── ubAimLevels
    ├── ubRecoilDelay
    ├── Handling
    ├── usOverheatingJamThreshold
    ├── usOverheatingDamageThreshold
    ├── usOverheatingSingleShotTemperature
    ├── HeavyGun
    ├── fBurstOnlyByFanTheHammer
    ├── BarrelConfiguration
    └── szNWSSSound
```

## Game options

`bAccuracy` belongs to Old Chance to Hit (OCTH). `nAccuracy`, `bRecoilX`, `bRecoilY`, `ubAimLevels`, `ubRecoilDelay` and `Handling` belong to New Chance to Hit (NCTH). NCTH is forced off in multiplayer, even when the option is enabled. See [OPTIONS](../../OPTIONS.md#chance-to-hit-ncth-and-octh).

## Fields

### `WEAPONLIST/WEAPON/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the weapon row in the runtime weapon table and the identity of the XML record. `Items.ubClassIndex` of a weapon item points here. Numeric semantics: Identifier; larger numbers have no inherent meaning beyond row order, and negative values are invalid.
- **Values:** Non-negative integer; unique within the file. The loader uses the value as the runtime index; no fixed upper bound is declared by the schema.
- **Default:** none; a missing element leaves the runtime slot at `0`.
- **Source:** weapon table data; field metadata.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/szWeaponName`

- **Type:** `String` (`String`)
- **Description:** Weapon name stored in the weapon table. The item master table also carries display names; this string is the weapon-side label used by weapon-specific code and tools.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubWeaponClass`

- **Type:** `Integer` (`UINT8`)
- **Description:** Broad weapon class used by equipment checks, AI selection and UI grouping. It is coarser than `ubWeaponType`. Numeric semantics: Numeric code that selects a listed type/class; larger numbers have no inherent ordering, and negative values are outside the listed choices.
- **Values:** IDs from `Lookup.WeaponClass`: `0` None, `1` Handgun, `2` Submachinegun, `3` Rifle, `4` Machinegun, `5` Shotgun, `6` Knife, `7` Monster.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubWeaponType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Specific weapon type within the class, used for AI preferences, Bobby Ray categories and weapon-specific handling. Numeric semantics: Numeric code that selects a listed type/class; larger numbers have no inherent ordering, and negative values are outside the listed choices.
- **Values:** IDs from `Lookup.WeaponType`: `0` None, `1` Pistol, `2` Machine Pistol, `3` Submachinegun, `4` Rifle, `5` Sniper Rifle, `6` Assault Rifle, `7` Light Machinegun, `8` Shotgun.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubCalibre`

- **Type:** `Integer` (`UINT8`)
- **Description:** Calibre index. It selects the matching row in `AmmoStrings` for display and connects the weapon to its ammunition family. Numeric semantics: Numeric code that selects a listed type/class; larger numbers have no inherent ordering, and negative values are outside the listed choices.
- **Values:** Index of a row in [AmmoStrings](AmmoStrings.md); `AmmoCaliber` is the display string.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubReadyTime`

- **Type:** `Integer` (`UINT8`)
- **Description:** AP cost to ready the weapon before firing. It is separate from the AP cost of the shot itself. Numeric semantics: Time value; higher values mean a longer delay/duration, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubShotsPer4Turns`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Rate-of-fire value expressed as shots per four turns. The game derives the actual AP cost from this value and the weapon’s AP settings; it is not a direct “shots per turn” counter. Numeric semantics: Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubShotsPerBurst`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of projectiles fired by one burst. Numeric semantics: Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubBurstPenalty`

- **Type:** `Integer` (`UINT8`)
- **Description:** Accuracy penalty applied to burst fire. Numeric semantics: Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubBulletSpeed`

- **Type:** `Integer` (`UINT8`)
- **Description:** Projectile speed used by the bullet-flight and hit calculations. It is not the same as damage. Numeric semantics: Higher values mean a faster speed; negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubImpact`

- **Type:** `Integer` (`UINT8`)
- **Description:** Base damage of the weapon before ammunition modifiers and armour reduction. Numeric semantics: Higher values mean a stronger effect, and negative values are not meaningful.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubDeadliness`

- **Type:** `Integer` (`UINT8`)
- **Description:** Deadliness rating used by mercenary weapon-affinity and AI evaluation. It is not a direct damage value. Numeric semantics: Higher values mean a stronger effect, and negative values are not meaningful.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/bAccuracy`

- **Type:** `Integer` (`INT16`)
- **Description:** Old Chance to Hit (OCTH) accuracy value. `nAccuracy` is the NCTH counterpart. Numeric semantics: Higher values mean a stronger associated combat value; negative values reduce it and are not meaningful unless the field is explicitly signed.
- **Only used with:** OCTH.
- **Values:** `-100..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubMagSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Magazine capacity used when the weapon is loaded. It describes the weapon’s magazine, not the magazine item row. Numeric semantics: Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** `0..65000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/usRange`

- **Type:** `Integer` (`UINT16`)
- **Description:** Effective range in tiles used by the hit and AI range calculations. Numeric semantics: Higher values mean a longer range/radius, and negative values are not meaningful.
- **Values:** `0..5000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/usReloadDelay`

- **Type:** `Integer` (`UINT16`)
- **Description:** Delay of the reload animation. It is a presentation/timing value, not the AP cost of reloading. Numeric semantics: Time value; higher values mean a longer delay/duration, and negative values are not meaningful.
- **Values:** `0..500`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/BurstAniDelay`

- **Type:** `Integer` (`UINT16`)
- **Description:** Delay between shots in a burst animation. Numeric semantics: Time value; higher values mean a longer delay/duration, and negative values are not meaningful.
- **Values:** `0..500`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubAttackVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Noise volume produced by firing the weapon. It feeds sound propagation and AI hearing checks. Numeric semantics: Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubHitVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Hit-volume value used when resolving whether a shot connects. Numeric semantics: Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/sSound`

- **Type:** `Integer` (`INT16`)
- **Description:** Sound ID played for a normal shot. Numeric semantics: Sound index; larger values select a different sound asset rather than a stronger effect, and negative values are invalid.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/sBurstSound`

- **Type:** `Integer` (`INT16`)
- **Description:** Sound ID played for a burst. Numeric semantics: Sound index; larger values select a different sound asset rather than a stronger effect, and negative values are invalid.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/sSilencedBurstSound`

- **Type:** `Integer` (`INT16`)
- **Description:** Sound ID played for a burst from a silenced weapon. Numeric semantics: Sound index; larger values select a different sound asset rather than a stronger effect, and negative values are invalid.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/sReloadSound`

- **Type:** `Integer` (`INT16`)
- **Description:** Sound ID played while reloading. Numeric semantics: Sound index; larger values select a different sound asset rather than a stronger effect, and negative values are invalid.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/sLocknLoadSound`

- **Type:** `Integer` (`INT16`)
- **Description:** Sound ID played when a round is chambered or the weapon is readied. Numeric semantics: Sound index; larger values select a different sound asset rather than a stronger effect, and negative values are invalid.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/SilencedSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Sound ID played for a silenced shot. `0` means no separate silenced sound. Numeric semantics: Sound index; larger values select a different sound asset rather than a stronger effect, and negative values are invalid.
- **Values:** Sound ID from the sound table; `0` means no separate silenced sound.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/bBurstAP`

- **Type:** `Integer` (`INT16`)
- **Description:** AP cost of a burst. Numeric semantics: Action-point cost; higher values mean a more expensive action, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/bAutofireShotsPerFiveAP`

- **Type:** `Integer` (`INT16`)
- **Description:** Number of autofire shots per five AP. It is a rate value, not a magazine capacity. Numeric semantics: Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/APsToReload`

- **Type:** `Integer` (`UINT16`)
- **Description:** AP cost of a normal reload. Numeric semantics: Action-point cost; higher values mean a more expensive action, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/SwapClips`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the weapon can reload by swapping magazines instead of using the full reload animation.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/MaxDistForMessyDeath`

- **Type:** `Integer` (`UINT16`)
- **Description:** Distance limit for the messy-death gore effect. Beyond this distance the ordinary death animation is used. Numeric semantics: Higher values mean a longer distance, and negative values are not meaningful.
- **Values:** `0..500`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/AutoPenalty`

- **Type:** `Integer` (`UINT16`)
- **Description:** Accuracy penalty applied to autofire. Numeric semantics: Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/NoSemiAuto`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the weapon has no semi-automatic fire mode.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/EasyUnjam`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the weapon is easier to unjam.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/APsToReloadManually`

- **Type:** `Integer` (`UINT16`)
- **Description:** AP cost of a manual reload, used when the weapon is reloaded one round at a time. Numeric semantics: Action-point cost; higher values mean a more expensive action, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ManualReloadSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Sound ID played during a manual reload. Numeric semantics: Sound index; larger values select a different sound asset rather than a stronger effect, and negative values are invalid.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/nAccuracy`

- **Type:** `Integer` (`INT16`)
- **Description:** New Chance to Hit (NCTH) accuracy value. `bAccuracy` is the OCTH counterpart. Numeric semantics: Higher values mean a stronger associated combat value; negative values reduce it and are not meaningful unless the field is explicitly signed.
- **Only used with:** NCTH.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bRecoilX`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Horizontal recoil value used by NCTH. Numeric semantics: Higher values mean a stronger associated combat value; negative values reduce it and are not meaningful unless the field is explicitly signed.
- **Only used with:** NCTH.
- **Values:** `-20..20`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/bRecoilY`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Vertical recoil value used by NCTH. Numeric semantics: Higher values mean a stronger associated combat value; negative values reduce it and are not meaningful unless the field is explicitly signed.
- **Only used with:** NCTH.
- **Values:** `-20..20`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubAimLevels`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of aim levels available to the weapon in NCTH. Numeric semantics: Higher values mean a stronger associated combat value; negative values reduce it and are not meaningful unless the field is explicitly signed.
- **Only used with:** NCTH.
- **Values:** `0..8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/ubRecoilDelay`

- **Type:** `Integer` (`UINT8`)
- **Description:** Delay before recoil recovery begins. Numeric semantics: Time value; higher values mean a longer delay/duration, and negative values are not meaningful.
- **Only used with:** NCTH.
- **Values:** `0..5`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/Handling`

- **Type:** `Integer` (`UINT16`)
- **Description:** Weapon-handling value used by NCTH. Higher values represent a weapon that is harder to control. Numeric semantics: Higher values mean a stronger associated combat value; negative values reduce it and are not meaningful unless the field is explicitly signed.
- **Only used with:** NCTH.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/usOverheatingJamThreshold`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Heat threshold at which the weapon starts to jam in the overheating model. Numeric semantics: Higher values mean a higher overheating threshold/temperature, and negative values are not meaningful.
- **Values:** `0..60000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/usOverheatingDamageThreshold`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Heat threshold at which the weapon starts to take damage in the overheating model. Numeric semantics: Higher values mean a higher overheating threshold/temperature, and negative values are not meaningful.
- **Values:** Decimal number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/usOverheatingSingleShotTemperature`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Temperature contribution of a single shot in the overheating model. Numeric semantics: Higher values mean a higher overheating threshold/temperature, and negative values are not meaningful.
- **Values:** `0..60000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/HeavyGun`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Marks the weapon as heavy for strength, bipod and handling checks.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

### `WEAPONLIST/WEAPON/fBurstOnlyByFanTheHammer`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, burst mode is available only to a gunslinger using the weapon in hip-fire. The condition is part of the weapon’s fire-mode rules, not a UI preference.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** weapon table data; field metadata.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/BarrelConfiguration`

- **Type:** `UINT8` (`UINT16`)
- **Description:** Number of barrels/projectiles in one barrel configuration. The element may repeat to describe several configurations; if it is absent, one barrel is assumed. Numeric semantics: Numeric code that selects a barrel configuration; larger numbers have no inherent ordering, and negative values are invalid.
- **Values:** Positive barrel count; one element per barrel configuration. Absent means one barrel. The element may repeat.
- **Default:** absent means one barrel; repeated elements add barrel configurations.
- **Source:** weapon table data; field metadata.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/szNWSSSound`

- **Type:** `String` (`String`)
- **Description:** Sound name used by the NWS sound path. It is a string key, not a numeric sound-table ID.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** weapon table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `Items.ubClassIndex` of a weapon item points to `Weapons.uiIndex`.
- `ubCalibre` points to [AmmoStrings](AmmoStrings.md); the calibre label is display data, while ammunition behaviour comes from `AmmoTypes` and the magazine row.
- `Magazines.ubAmmoType` selects the ammo behaviour loaded by a magazine; `Weapons.ubCalibre` only selects the calibre family.
- NCTH fields are read only when the new chance-to-hit system is active; OCTH uses `bAccuracy` and ignores the NCTH group.

## Related formulas

- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt)
- [Armour protection](../../FORMULAS.md#armour-protection)

## Localization

The weapon name and calibre label are display strings; weapon names are also carried by the item master table. Numeric sound and lookup IDs are not localized.

## Versions

v4 and v5 use the same 48 field paths. `BarrelConfiguration` is repeated in both versions; values may differ.

## Sources

- Weapon table data.
- Weapon table schema comments.
- Item metadata and lookup tables.
- Field types and limits: item metadata; numeric limits use the metadata validation ranges.
- Semantic descriptions use the table schema comments and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The exact AP rounding for `ubShotsPer4Turns` and the heat units for the three overheating thresholds are not stated by the schema comment; the field descriptions stay at the level those sources support.
