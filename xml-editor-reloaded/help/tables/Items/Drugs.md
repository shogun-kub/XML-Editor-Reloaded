---
schema: 1
lang: en
family: items-drugs
title: "Drugs - drug effects"
tables:
  - Drugs
table_ids:
  - Items.Drugs
files:
  - Items/Drugs.xml
root: DRUGSLIST
record: DRUG
key: uiIndex
sources:
  - Drug table data
  - Drug schema comment
  - Game code (drug loader, drug table struct and effect application)
  - Item metadata
status: reviewed
updated: 2026-11-08
---

# Drugs - drug effects

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`DRUGSLIST/DRUG/uiIndex`](#drugslistdruguiindex)
  - [`DRUGSLIST/DRUG/szName`](#drugslistdrugszname)
  - [`DRUGSLIST/DRUG/opinionevent`](#drugslistdrugopinionevent)
  - [`DRUGSLIST/DRUG/DRUG_EFFECT/effect`](#drugslistdrugdrug_effecteffect)
  - [`DRUGSLIST/DRUG/DRUG_EFFECT/duration`](#drugslistdrugdrug_effectduration)
  - [`DRUGSLIST/DRUG/DRUG_EFFECT/size`](#drugslistdrugdrug_effectsize)
  - [`DRUGSLIST/DRUG/DRUG_EFFECT/chance`](#drugslistdrugdrug_effectchance)
  - [`DRUGSLIST/DRUG/DISEASE_EFFECT/disease`](#drugslistdrugdisease_effectdisease)
  - [`DRUGSLIST/DRUG/DISEASE_EFFECT/size`](#drugslistdrugdisease_effectsize)
  - [`DRUGSLIST/DRUG/DISEASE_EFFECT/chance`](#drugslistdrugdisease_effectchance)
  - [`DRUGSLIST/DRUG/DISABILITY_EFFECT/disability`](#drugslistdrugdisability_effectdisability)
  - [`DRUGSLIST/DRUG/DISABILITY_EFFECT/duration`](#drugslistdrugdisability_effectduration)
  - [`DRUGSLIST/DRUG/DISABILITY_EFFECT/chance`](#drugslistdrugdisability_effectchance)
  - [`DRUGSLIST/DRUG/PERSONALITY_EFFECT/personality`](#drugslistdrugpersonality_effectpersonality)
  - [`DRUGSLIST/DRUG/PERSONALITY_EFFECT/duration`](#drugslistdrugpersonality_effectduration)
  - [`DRUGSLIST/DRUG/PERSONALITY_EFFECT/chance`](#drugslistdrugpersonality_effectchance)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Drugs.xml` defines drug effects: stat changes, disease changes, temporary disabilities and personality changes. A drug can contain several effect blocks of the same or different types.

## Tables

| Table | File | Notes |
|---|---|---|
| `Drugs` | `Items/Drugs.xml` | v4 and v5 have the same field paths and, in the reference data, the same values. |

## Structure

```
DRUGSLIST
└── DRUG
    ├── uiIndex
    ├── szName
    ├── opinionevent
    ├── DRUG_EFFECT
    │   ├── effect
    │   ├── duration
    │   ├── size
    │   └── chance
    ├── DISEASE_EFFECT
    │   ├── disease
    │   ├── size
    │   └── chance
    ├── DISABILITY_EFFECT
    │   ├── disability
    │   ├── duration
    │   └── chance
    └── PERSONALITY_EFFECT
        ├── personality
        ├── duration
        └── chance
```

## Fields

### `DRUGSLIST/DRUG/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the drug row and identity of the XML record. It is referenced by the item's drug-type field in [Items](Items.md).
- **Values:** `0..99`; the runtime `NewDrug` array has `NEW_DRUGS_MAX` (`100`) slots. The loader stores this field as `UINT8` and does not range-check it before writing.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader and `NewDrug` array); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/szName`

- **Type:** `String` (`CHAR16`)
- **Description:** Name copied into `DRUG.szName`. The loader's comment calls it informational, and no display or gameplay code reads it; the item row carries the player-visible name.
- **Values:** Free text, truncated to the 80-character `CHAR16` buffer.
- **Default:** empty string when omitted.
- **Source:** game code (drug loader and `DRUG` struct); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/opinionevent`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set and dynamic opinions are enabled, consuming the drug triggers the addiction opinion event, which other mercenaries judge negatively. The event fires only when at least one `DRUG_EFFECT` block was applied.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; drug application and dynamic-opinion code); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DRUG_EFFECT/effect`

- **Type:** `Integer` (`UINT8`)
- **Description:** Stat or resource changed by this effect.
- **Values:** IDs from the drug-effect enum: `0` HP, `1` BP, `2` AP, `3` Morale, `4` Physical resistance, `5` Strength, `6` Agility, `7` Dexterity, `8` Wisdom.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; `DRUG_EFFECT_*` enum); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DRUG_EFFECT/duration`

- **Type:** `Integer` (`UINT16`)
- **Description:** Duration of the effect in turns. The game decrements it during the end-turn drug update.
- **Numeric semantics:** Time value; higher values mean a longer-lasting effect.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; end-turn drug update); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DRUG_EFFECT/size`

- **Type:** `Integer` (`INT16`)
- **Description:** Strength of the effect. Positive values raise the affected stat or resource, negative values lower it.
- **Numeric semantics:** Signed strength; for a newly applied effect the game multiplies it by the used item percentage before storing it. Shipped data uses negative values for effects such as alcohol's Wisdom penalty.
- **Values:** `-32768..32767`; the loader stores this field as `INT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; effect application); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DRUG_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that this effect is applied when the drug is consumed. `0` and `100` both mean the effect always applies; values `1..99` are a percentage.
- **Numeric semantics:** Higher values up to `99` mean a greater chance; `0` is the always-apply shortcut.
- **Values:** `0..100`; `0` and `100` both apply unconditionally.
- **Default:** `100` when omitted; the loader initializes every effect block to `100`.
- **Source:** game code (drug loader and chance check); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISEASE_EFFECT/disease`

- **Type:** `Integer` (`UINT8`)
- **Description:** Disease row affected by this drug. The value indexes the runtime disease table.
- **Only used with:** Disease system.
- **Values:** Disease row index in [Disease](../Disease.md); `0` is Arulcan plague, not an empty row.
- **Default:** `0` (Arulcan plague) when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; disease-point application); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISEASE_EFFECT/size`

- **Type:** `Integer` (`INT32`)
- **Description:** Disease points added. The game multiplies the value by the used item percentage, then adds it; positive values increase the disease, negative values cure it, and the result is clamped by the disease's outbreak and full-infection thresholds.
- **Only used with:** Disease system.
- **Numeric semantics:** Signed points; higher positive values advance the disease faster, and negative values reduce it.
- **Values:** `-2147483648..2147483647`; the loader stores this field as `INT32`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; `AddDiseasePoints`); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISEASE_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that this disease effect is applied. `0` and `100` both mean the effect always applies; values `1..99` are a percentage.
- **Only used with:** Disease system.
- **Numeric semantics:** Higher values up to `99` mean a greater chance; `0` is the always-apply shortcut.
- **Values:** `0..100`; `0` and `100` both apply unconditionally.
- **Default:** `100` when omitted; the loader initializes every effect block to `100`.
- **Source:** game code (drug loader and chance check); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISABILITY_EFFECT/disability`

- **Type:** `Integer` (`UINT8`)
- **Description:** Disability added for the duration of the effect. Only one disability is active at a time: a later different disability replaces the earlier one, while the same disability accumulates duration.
- **Values:** Disability ID: `0` None, `1` Heat intolerant, `2` Nervous, `3` Claustrophobic, `4` Nonswimmer, `5` Fear of insects, `6` Forgetful, `7` Psycho, `8` Deaf, `9` Shortsighted, `10` Hemophiliac, `11` Afraid of heights, `12` Self-harm.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; disability enum and application); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISABILITY_EFFECT/duration`

- **Type:** `Integer` (`UINT16`)
- **Description:** Duration of the disability in turns. The game scales it by the percentage of the item that was used.
- **Numeric semantics:** Time value; higher values mean a longer disability.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; disability application and end-turn update); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISABILITY_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that this disability is applied. `0` and `100` both mean the effect always applies; values `1..99` are a percentage.
- **Numeric semantics:** Higher values up to `99` mean a greater chance; `0` is the always-apply shortcut.
- **Values:** `0..100`; `0` and `100` both apply unconditionally.
- **Default:** `100` when omitted; the loader initializes every effect block to `100`.
- **Source:** game code (drug loader and chance check); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/PERSONALITY_EFFECT/personality`

- **Type:** `Integer` (`UINT8`)
- **Description:** Personality trait added for the duration of the effect. Only one personality is active at a time: a later different trait replaces the earlier one, while the same trait accumulates duration.
- **Values:** Personality trait ID: `0` Normal, `1` Sociable, `2` Loner, `3` Optimist, `4` Assertive, `5` Intellectual, `6` Primitive, `7` Aggressive, `8` Phlegmatic, `9` Dauntless, `10` Pacifist, `11` Malicious, `12` Show-off. The engine enum also defines `13` Coward, although the table comment stops at `12`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; character-trait enum and application); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/PERSONALITY_EFFECT/duration`

- **Type:** `Integer` (`UINT16`)
- **Description:** Duration of the personality effect in turns. The game scales it by the percentage of the item that was used.
- **Numeric semantics:** Time value; higher values mean a longer personality change.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (drug loader; personality application and end-turn update); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/PERSONALITY_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that this personality effect is applied. `0` and `100` both mean the effect always applies; values `1..99` are a percentage.
- **Numeric semantics:** Higher values up to `99` mean a greater chance; `0` is the always-apply shortcut.
- **Values:** `0..100`; `0` and `100` both apply unconditionally.
- **Default:** `100` when omitted; the loader initializes every effect block to `100`.
- **Source:** game code (drug loader and chance check); drug table data.
- **Confidence:** `source`

## Relations

- `uiIndex` is referenced by the item's `DrugType` value in [Items](Items.md); the game uses that value directly as an index into `NewDrug`.
- `DISEASE_EFFECT/disease` refers to a row in [Disease](../Disease.md).
- Multiple effect blocks may be repeated. Repeated `DRUG_EFFECT` blocks of the same type merge their duration and strength; for disabilities and personalities, only the last successfully applied effect is active, while a repeated identical effect adds duration.

## Localization

`szName` is stored only and is not displayed by the game; effect IDs and enum values are not localized.

## Versions

v4 and v5 use the same field paths, and the reference files contain the same values.

## Sources

- Drug table data.
- Drug schema comment.
- Game code (drug loader, drug table struct, effect application and disease handling).
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use the game code and XML element names; entries marked `inferred` do not claim a code-verified formula.

## Open questions

Repeated `DRUG_EFFECT` blocks of the same type are merged in code rather than replaced: the durations are averaged and the resulting size is recomputed from the combined effect total. The XML comment does not describe that merge, and the table cannot express the merge weights directly.

The editor metadata maps `Items.DrugType` to `Lookup.DrugType`, whose IDs are bit flags, but the game uses the item value as a direct index into `NewDrug`; shipped item values are sequential indices. The table records the code behaviour and does not resolve that metadata mismatch.
