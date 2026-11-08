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
  - Game code (weapon loader and `WEAPONTYPE`)
  - Item metadata and lookup tables
status: reviewed
updated: 2026-11-08
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
| `Weapons` | `Items/Weapons.xml` | v4 and v5 have the same 48 record fields. In the reference data the rows are identical apart from one extra record in v4; `BarrelConfiguration` is a repeated element. |

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

`bAccuracy`, `ubBurstPenalty` and `AutoPenalty` belong to Old Chance to Hit (OCTH). `nAccuracy`, `bRecoilX`, `bRecoilY`, `ubAimLevels`, `ubRecoilDelay` and `Handling` belong to New Chance to Hit (NCTH). NCTH is forced off in multiplayer, even when the option is enabled. See [OPTIONS](../../OPTIONS.md#chance-to-hit-ncth-and-octh).

## Fields

### `WEAPONLIST/WEAPON/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the weapon row in the runtime weapon table and the identity of the XML record. `Items.ubClassIndex` of a weapon item points here.
- **Values:** `0..16000`; the loader writes into `Weapon[MAXITEMS]` with `MAXITEMS` = `16001`, and rows with a larger `uiIndex` are silently ignored.
- **Default:** missing or empty means `0`; the parser zeroes the record, so such a row is stored at index `0`.
- **Source:** game code (weapon loader and runtime `Weapon` array); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/szWeaponName`

- **Type:** `String` (`CHAR8`)
- **Description:** Name stored in `WEAPONTYPE.szWeaponName`. No gameplay or interface code reads it; weapon display names come from the item master table.
- **Values:** Free text, truncated to `MAX_WEAPON_NAME_LENGTH` (`20`) characters.
- **Default:** empty string when omitted.
- **Source:** game code (weapon loader); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubWeaponClass`

- **Type:** `Integer` (`UINT8`)
- **Description:** Broad weapon class used by dealer and interface filters and by class comparisons such as handgun, SMG, shotgun, knife and monster. It is coarser than `ubWeaponType`.
- **Values:** IDs from `Lookup.WeaponClass`: `0` None, `1` Handgun, `2` Submachinegun, `3` Rifle, `4` Machinegun, `5` Shotgun, `6` Knife, `7` Monster.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader and `WeaponClass` checks); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubWeaponType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Specific weapon type within the class, used for per-type item modifiers, AI weapon preference and interface grouping.
- **Values:** IDs from `Lookup.WeaponType`: `0` None, `1` Pistol, `2` Machine Pistol, `3` Submachinegun, `4` Rifle, `5` Sniper Rifle, `6` Assault Rifle, `7` Light Machinegun, `8` Shotgun.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader and `WeaponType` enum); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubCalibre`

- **Type:** `Integer` (`UINT8`)
- **Description:** Calibre index. It is compared with `Magazines.ubCalibre` for ammunition compatibility and indexes the `AmmoCaliber` and `NWSSCaliber` strings in `AmmoStrings`.
- **Values:** Index of a row in [AmmoStrings](AmmoStrings.md); `0` is the empty calibre.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; ammunition matching and `AmmoCaliber` lookup); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubReadyTime`

- **Type:** `Integer` (`UINT8`)
- **Description:** AP cost to ready the weapon before firing. It is separate from the AP cost of the shot itself.
- **Numeric semantics:** Action-point cost, not a time. The loader rescales it from the 100-AP baseline to the configured `AP_MAXIMUM` with `DynamicAdjustAPConstants`; higher values mean a more expensive ready.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; AP constants); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubShotsPer4Turns`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Mechanical rate of fire expressed as shots per four turns. The game derives the AP cost from this value and the weapon's AP settings; it is not a direct "shots per turn" counter.
- **Numeric semantics:** Higher values mean a faster mechanical rate of fire (more shots per four turns) and therefore a lower AP cost per shot in `BaseAPsToShootOrStab`. The effective value must be positive; `0` makes the AP calculation return `-1` (invalid).
- **Values:** Decimal number; shipped values are positive for weapons and `0` for the placeholder row.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader and AP calculation); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubShotsPerBurst`

- **Type:** `Integer` (`UINT8`)
- **Description:** Base number of projectiles fired by one burst. The runtime value is `GetShotsPerBurst`, which adds the attachment burst-size bonus.
- **Numeric semantics:** Higher values mean more projectiles in a burst; `0` means the weapon has no burst mode unless an attachment grants one.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetShotsPerBurst`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubBurstPenalty`

- **Type:** `Integer` (`UINT8`)
- **Description:** Accuracy penalty applied to burst fire. It is subtracted from the OCTH chance to hit once per shot after the first.
- **Only used with:** OCTH.
- **Numeric semantics:** Higher values mean a stronger penalty for each extra shot in the burst; the game reduces it by the burst-to-hit bonus and clamps the result at `0`.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; OCTH burst accuracy); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubBulletSpeed`

- **Type:** `Integer` (`UINT8`)
- **Description:** Controls the bullet's on-screen travel rate. The game divides it (plus the attachment/ammo bullet-speed bonus) by `10` and uses the result as the per-tile update interval; it does not affect damage or hit probability.
- **Numeric semantics:** A larger value produces a longer interval between one-tile steps, so the projectile appears slower despite the field name. A value of `0` gives a near-instant visual bullet.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; bullet travel interval); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubImpact`

- **Type:** `Integer` (`UINT8`)
- **Description:** Base damage of the weapon before item, ammunition and attachment damage bonuses, range modifiers and armour reduction.
- **Numeric semantics:** Higher values mean a stronger base hit; the final damage is clamped to `255` by `GetDamage`.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetDamage`/`GetBasicDamage`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubDeadliness`

- **Type:** `Integer` (`UINT8`)
- **Description:** Comparative deadliness rating used for a mercenary's gun attractiveness, AI threat evaluation and item valuation. It is not a direct damage value.
- **Numeric semantics:** Higher values mean a more desirable or threatening weapon; the value is a rating, not a damage amount.
- **Values:** `0..250`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; mercenary profile and AI threat code); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bAccuracy`

- **Type:** `Integer` (`INT8`)
- **Description:** Old Chance to Hit (OCTH) accuracy value. `nAccuracy` is the NCTH counterpart.
- **Only used with:** OCTH.
- **Numeric semantics:** Higher values make the weapon more accurate; lower values make it less accurate, and negative values act as an accuracy penalty.
- **Values:** `-100..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; OCTH hit calculation); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubMagSize`

- **Type:** `Integer` (`UINT16`)
- **Description:** Base magazine capacity used when the weapon is loaded. `GetMagSize` adds the attachment magazine bonus; this field describes the weapon's magazine, not the magazine item row.
- **Numeric semantics:** Higher values mean a larger capacity; `0` leaves the weapon dependent on an attachment-provided magazine.
- **Values:** `0..65000`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetMagSize`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usRange`

- **Type:** `Integer` (`UINT16`)
- **Description:** Effective range in world units, where `10` units equal one tile. `GunRange` and `InRange` compare it with the distance after applying range bonuses.
- **Numeric semantics:** Higher values mean a longer range; `0` makes `GetModifiedGunRange` return `0`, so the weapon cannot reach a target.
- **Values:** `0..5000`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GunRange`/`GetModifiedGunRange`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usReloadDelay`

- **Type:** `Integer` (`UINT16`)
- **Description:** Delay of the reload animation. The game adds it to `MANDATORY_WEAPON_DELAY` when it sets the soldier's reload delay; it is a presentation/timing value, not the AP cost of reloading.
- **Numeric semantics:** Time value; higher values mean a longer reload animation.
- **Values:** `0..500`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; reload animation timing); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/BurstAniDelay`

- **Type:** `Integer` (`INT16`)
- **Description:** Per-frame animation delay used while a burst-fire animation plays. It is stored in `WEAPONTYPE.sAniDelay` and copied to the soldier's animation delay.
- **Numeric semantics:** Time value; higher values play the burst animation more slowly.
- **Values:** `0..500`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; soldier animation delay); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubAttackVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Noise volume produced by firing the weapon. It feeds sound propagation and AI hearing checks through `MakeNoise`.
- **Numeric semantics:** Higher values mean a louder shot; `0` makes the shot silent to the noise system.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; firing noise); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubHitVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Noise volume produced when a bullet strikes a target or structure, passed to `MakeNoise` with `NOISE_BULLET_IMPACT`. It is not a hit-resolution or accuracy value.
- **Numeric semantics:** Higher values make bullet impacts louder to the noise system; `0` makes them silent.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; bullet-impact noise); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Sound ID played for a normal shot. `0` (`NO_WEAPON_SOUND`) suppresses the shot sound.
- **Values:** Sound ID; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; shot sound selection); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sBurstSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index into the `gzBurstSndStrings[]` filename templates used for the burst sound in the original (non-NWSS) sound system. It is not a `PlayJA2Sample` sound ID; `0` means no burst sound.
- **Values:** Index into the burst-sound template table; `0` disables the burst sound.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; original burst sound path); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sSilencedBurstSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index into the `gzBurstSndStrings[]` filename templates used for the silenced burst sound in the original (non-NWSS) sound system. It is not a `PlayJA2Sample` sound ID; `0` means no silenced burst sound.
- **Values:** Index into the burst-sound template table; `0` disables the silenced burst sound.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; original silenced burst sound path); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sReloadSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Sound ID played while reloading. `0` suppresses the reload sound.
- **Values:** Sound ID; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; reload sound selection); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/sLocknLoadSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Sound ID played while a round is chambered or the weapon is readied. `0` suppresses the sound.
- **Values:** Sound ID; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; lock-and-load animation); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/SilencedSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Sound ID played instead of `sSound` when the shot is treated as silenced (noise below the configured threshold, or `ubAttackVolume` at most `10`). The game plays it directly, with no `0` guard and no fallback to `sSound`.
- **Values:** Sound ID; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; silenced shot sound); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bBurstAP`

- **Type:** `Integer` (`UINT8`)
- **Description:** Base AP cost of a burst. The loader rescales it from the 100-AP baseline to `AP_MAXIMUM`, and `CalcAPsToBurst` scales it by the shooter's action points and applies burst AP reductions.
- **Numeric semantics:** Action-point cost; higher values mean a more expensive burst.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `CalcAPsToBurst`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bAutofireShotsPerFiveAP`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of autofire shots per five AP. It is a rate value, not a magazine capacity; `0` disables autofire.
- **Numeric semantics:** Higher values mean a faster autofire rate. The game multiplies the value by the per-type INI modifier, adds `bAutofireBulletsPer5APModifier`, and clamps the result to at least `1` when the base value is positive.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetAutofireShotsPerFiveAPs` and autofire AP cost); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/APsToReload`

- **Type:** `Integer` (`UINT8`)
- **Description:** Base AP cost of a normal reload. The loader rescales it to `AP_MAXIMUM`; `GetAPsToReload` applies the per-type reload modifier and the reload-time reduction.
- **Numeric semantics:** Action-point cost; higher values mean a more expensive reload.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetAPsToReload`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/SwapClips`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, reloading with a full magazine of the same size uses a magazine swap (`RELOAD_SWAP`) instead of topping the weapon off round by round; a wrong-size magazine then uses the `AP_WRONG_MAG` cost. If clear, the weapon is reloaded one round at a time.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; reload-type selection); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/MaxDistForMessyDeath`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum distance at which head and torso hits can trigger the instant-kill and messy-death special effects (head explosion, chest explosion). Beyond this distance those specials do not trigger.
- **Numeric semantics:** Higher values allow the specials at a longer range; `0` restricts them to the same tile.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `CalcChanceToHitGun` hit-location specials); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/AutoPenalty`

- **Type:** `Integer` (`UINT8`)
- **Description:** Accuracy penalty applied to autofire. It is subtracted from the OCTH chance to hit once per extra shot in the volley.
- **Only used with:** OCTH.
- **Numeric semantics:** Higher values mean a stronger penalty; `0` means no autofire penalty. The game reduces it by the auto-to-hit bonus and clamps the result at `0`.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; OCTH autofire accuracy); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/NoSemiAuto`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the weapon has no semi-automatic fire mode; switching to it selects autofire.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; weapon-mode selection); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/EasyUnjam`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, unjamming uses a fixed `+100` skill-check modifier instead of the reliability-based modifier, so the check is capped at `99%` and effectively always succeeds. No experience is awarded for unjamming such a weapon.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; unjam skill check); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/APsToReloadManually`

- **Type:** `Integer` (`UINT8`)
- **Description:** AP cost to chamber or pump a round when the weapon has ammunition but no cartridge in the chamber (manual action). It is not the cost of a full reload.
- **Numeric semantics:** Action-point cost; the loader rescales it to `AP_MAXIMUM`, and the game applies the per-type manual-reload modifier and the Sniper/Ranger trait reductions.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetAPsToAutoReload` chambering path); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ManualReloadSound`

- **Type:** `Integer` (`UINT16`)
- **Description:** Sound ID played while a round is manually chambered or pumped.
- **Values:** Sound ID; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; manual-reload sound); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/nAccuracy`

- **Type:** `Integer` (`INT8`)
- **Description:** New Chance to Hit (NCTH) accuracy value. `bAccuracy` is the OCTH counterpart.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values make the weapon more accurate; lower values make it less accurate, and negative values act as an accuracy penalty.
- **Values:** `-128..127`; the loader stores this field as `INT8` and the field metadata declares no narrower range.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; NCTH accuracy); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bRecoilX`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Horizontal recoil deviation used by NCTH. Positive values kick right; the final value is clamped to at least `0`, so a negative configured value does not produce a leftward kick.
- **Only used with:** NCTH.
- **Numeric semantics:** A larger value is harder to control. Shipped values are non-negative; the metadata allows `-20..20`, but `GetRecoil` clamps the result at `0`.
- **Values:** `-20..20` in the metadata; observed values are `0..4`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetRecoil`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/bRecoilY`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Vertical recoil deviation used by NCTH. Positive values kick up; the final value is clamped to at least `0`, so a negative configured value does not produce a downward kick.
- **Only used with:** NCTH.
- **Numeric semantics:** A larger value is harder to control. Shipped values are non-negative; the metadata allows `-20..20`, but `GetRecoil` clamps the result at `0`.
- **Values:** `-20..20` in the metadata; observed values are `0..14`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetRecoil`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubAimLevels`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of aim levels available to the weapon in NCTH. A value outside `1..8` makes the game fall back to its class-and-range algorithm; the runtime result is always clamped to `1..8`.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values allow more aiming levels (finer aim); `0` or any value above `8` means automatic. Non-gun weapons such as throwing knives always use `2`.
- **Values:** `0..8`; `0` means automatic.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `AllowedAimingLevelsNCTH`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/ubRecoilDelay`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of leading bullets in a volley that produce no recoil. `GetRecoil` returns zero while the bullet number is at most this value.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values make the first few shots of a burst recoil-free; `0` gives only the first bullet no recoil.
- **Values:** `0..5` in the metadata; observed values are `0`, `2` and `3`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetRecoil`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/Handling`

- **Type:** `Integer` (`UINT8`)
- **Description:** Weapon-handling difficulty used by NCTH. Higher values raise the gun-difficulty term in `CalcNewChanceToHitGun`, which lowers the chance to hit.
- **Only used with:** NCTH.
- **Numeric semantics:** Higher values represent a weapon that is harder to control; lower values are easier.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; NCTH handling difficulty); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usOverheatingJamThreshold`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Temperature at which the weapon starts to jam in the overheating model. The game compares it with `bTemperature` and returns the ratio as a jam percentage.
- **Numeric semantics:** Temperature value on the `0..OVERHEATING_MAX_TEMPERATURE` scale (`60000`); higher values tolerate more heat before jamming.
- **Values:** `0..60000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetOverheatJamThreshold`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usOverheatingDamageThreshold`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Temperature at which the weapon starts to take damage in the overheating model. The game compares it with `bTemperature` and returns the ratio as a damage percentage.
- **Numeric semantics:** Temperature value on the `0..OVERHEATING_MAX_TEMPERATURE` scale (`60000`); higher values tolerate more heat before damage.
- **Values:** `0..60000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetOverheatDamageThreshold`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/usOverheatingSingleShotTemperature`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Temperature added by one shot in the overheating model. Attachments and ammunition can modify it at use.
- **Numeric semantics:** Temperature value on the `0..OVERHEATING_MAX_TEMPERATURE` scale (`60000`); higher values add more heat per shot.
- **Values:** `0..60000`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `GetSingleShotTemperature`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/HeavyGun`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Marks a two-handed weapon as heavy. While standing it must be fired from the hip (alternative weapon holding) and cannot be shouldered; the flag is consulted by the alternative-holding logic, not by strength or bipod checks.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; alternative weapon-holding and aim-level logic); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/fBurstOnlyByFanTheHammer`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, burst mode is available only to a gunslinger using the weapon in hip-fire. The condition is part of the weapon's fire-mode rules, not a UI preference: it requires the new trait system, the fan-the-hammer option, the GUNSLINGER trait, alternative weapon holding and an empty second hand.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (weapon loader; `IsGunBurstCapable` and alternative holding); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/BarrelConfiguration`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of barrels/projectiles in one barrel configuration. The element may repeat to describe several configurations; each nonzero value is a selectable barrel count. Only weapons with the `IC_BOBBY_GUN` class use it.
- **Values:** `1..255`; the loader skips `0` entries. If no nonzero value is present, one barrel is assumed.
- **Default:** absent or all-zero means one barrel; repeated elements add barrel configurations.
- **Source:** game code (weapon loader; `GetFittingBarrelMode`/`GetNextBarrelMode`); weapon table data.
- **Confidence:** `source`

### `WEAPONLIST/WEAPON/szNWSSSound`

- **Type:** `String` (`CHAR8`)
- **Description:** Base name used by the New Weapon Sound System. The game builds `<calibre>_<szNWSSSound>_<loop|single>.ogg`; when the name is empty or the file does not exist it falls back to the weapon-type name and then to the generic file.
- **Values:** Free text, truncated to `MAX_WEAPON_NAME_LENGTH` (`20`) characters.
- **Default:** empty string when omitted.
- **Source:** game code (weapon loader; NWSS filename construction); weapon table data.
- **Confidence:** `source`

## Relations

- [Items](Items.md) `ubClassIndex` of a weapon item points to `Weapons.uiIndex`.
- `ubCalibre` points to [AmmoStrings](AmmoStrings.md) and is compared with `Magazines.ubCalibre`; the calibre label is display data, while ammunition behaviour comes from [AmmoTypes](AmmoTypes.md) and the [Magazines](Magazines.md) row.
- `Magazines.ubAmmoType` selects the ammo behaviour loaded by a magazine; `Weapons.ubCalibre` only selects the calibre family.
- NCTH fields are read only when the new chance-to-hit system is active; OCTH uses `bAccuracy`, `ubBurstPenalty` and `AutoPenalty`.

## Related formulas

- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt)
- [Armour protection](../../FORMULAS.md#armour-protection)

## Localization

The calibre label comes from `AmmoStrings` and is localized there. `szWeaponName` is stored but not displayed; weapon display names live in the item master table. Numeric sound and lookup IDs are not localized.

## Versions

v4 and v5 use the same 48 field paths. `BarrelConfiguration` is repeated in both versions. In the reference data the values are identical apart from one extra weapon record in v4.

## Sources

- Weapon table data.
- Weapon table schema comments.
- Game code (weapon loader, weapon table struct, AP/accuracy/overheating/sound functions).
- Item metadata and lookup tables.
- Field types and limits: item metadata; numeric limits use the metadata validation ranges.
- Semantic descriptions use the table schema comments and the game code; entries marked `inferred` do not claim a code-verified formula.

## Open questions

No unresolved field semantics remain. The only design-level oddity is `ubBulletSpeed`: the code uses the value as a per-tile update interval (larger values slow the visual projectile), which is opposite to what the field name suggests.
