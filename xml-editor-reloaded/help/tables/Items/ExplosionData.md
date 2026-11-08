---
schema: 1
lang: en
family: items-explosives
title: "ExplosionData - blast animation and sound"
tables:
  - ExplosionData
table_ids:
  - Items.ExplosionData
files:
  - Items/ExplosionData.xml
root: EXPDATALIST
record: EXPDATA
key: uiIndex
sources:
  - Explosion data table
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# ExplosionData - blast animation and sound

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`EXPDATALIST/EXPDATA/uiIndex`](#expdatalistexpdatauiindex)
  - [`EXPDATALIST/EXPDATA/name`](#expdatalistexpdataname)
  - [`EXPDATALIST/EXPDATA/TransKeyFrame`](#expdatalistexpdatatranskeyframe)
  - [`EXPDATALIST/EXPDATA/DamageKeyFrame`](#expdatalistexpdatadamagekeyframe)
  - [`EXPDATALIST/EXPDATA/ExplosionSoundID`](#expdatalistexpdataexplosionsoundid)
  - [`EXPDATALIST/EXPDATA/AltExplosionSoundID`](#expdatalistexpdataaltexplosionsoundid)
  - [`EXPDATALIST/EXPDATA/BlastFilename`](#expdatalistexpdatablastfilename)
  - [`EXPDATALIST/EXPDATA/BlastSpeed`](#expdatalistexpdatablastspeed)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`ExplosionData.xml` describes the visual blast and its sound: which STI animation plays, which frames mark the transparency and damage moments, and which sound IDs accompany the explosion. Explosives select a row through `ubAnimationID`.

## Tables

| Table | File | Notes |
|---|---|---|
| `ExplosionData` | `Items/ExplosionData.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
EXPDATALIST
└── EXPDATA
    ├── uiIndex
    ├── name
    ├── TransKeyFrame
    ├── DamageKeyFrame
    ├── ExplosionSoundID
    ├── AltExplosionSoundID
    ├── BlastFilename
    └── BlastSpeed
```

## Game options

No table-level global option is documented for this table. Its fields are read in both inventory models and in multiplayer; the only version gate is the field set described under [Versions](#versions).

## Fields

### `EXPDATALIST/EXPDATA/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the blast presentation row. `Explosives.ubAnimationID` points here.
- **Values:** Non-negative integer; unique within the file. `Explosives.ubAnimationID` points here.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** explosion data table; field metadata.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/name`

- **Type:** `String` (`String`)
- **Description:** Internal label for the blast animation, used by editors and debugging; it is not a localized UI string.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** explosion data table; field metadata.
- **Confidence:** `inferred`

### `EXPDATALIST/EXPDATA/TransKeyFrame`

- **Type:** `Integer` (`UINT8`)
- **Description:** Animation frame at which the transparency key is applied.
- **Values:** Frame index; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/DamageKeyFrame`

- **Type:** `Integer` (`UINT8`)
- **Description:** Animation frame at which the blast begins to apply damage.
- **Values:** Frame index; `0` is the first frame. No damage scale is implied by the frame number.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/ExplosionSoundID`

- **Type:** `Integer` (`INT32`)
- **Description:** Primary sound ID played for the explosion.
- **Values:** Integer; the loader stores this field as `INT32`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/AltExplosionSoundID`

- **Type:** `Integer` (`INT32`)
- **Description:** Alternate sound ID used when the explosion needs a variant.
- **Values:** Integer; the loader stores this field as `INT32`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/BlastFilename`

- **Type:** `String` (`String`)
- **Description:** STI animation path for the blast, relative to the game’s table/image data.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** explosion data table; field metadata.
- **Confidence:** `inferred`

### `EXPDATALIST/EXPDATA/BlastSpeed`

- **Type:** `Integer` (`INT8`)
- **Description:** Playback speed of the blast animation.
- **Numeric semantics:** Higher values mean a faster speed; negative values are not meaningful.
- **Values:** Integer; the loader stores this field in a signed byte.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

## Relations

- `Explosives.ubAnimationID` points to `ExplosionData.uiIndex`.
- `ExplosionSoundID` and `AltExplosionSoundID` are sound-table IDs; the sound table is outside this document set.

## Related formulas

No formula in [FORMULAS](../../FORMULAS.md) applies to this table.

## Localization

`name` is an internal label, not a localized UI string. Sound IDs are not translated.

## Versions

v4 and v5 use the same 8 field paths; values may differ.

## Sources

- Explosion data table.
- Item metadata.
- Field types and limits: item metadata; numeric limits use the metadata validation ranges.
- Semantic descriptions use the table schema comments and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

`TransKeyFrame` and `DamageKeyFrame` are animation-frame indices; the frame timing depends on the STI and is not part of the table.
