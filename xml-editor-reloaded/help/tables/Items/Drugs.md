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
  - Item metadata
status: draft
updated: 2026-11-07
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
| `Drugs` | `Items/Drugs.xml` | v4 and v5 have the same field paths; values differ. |

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

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the drug row and identity of the XML record.
- **Values:** Non-negative index; unique within the file. It is referenced by `DrugType` in [Items](Items.md); the schema comment allows up to 100 drugs.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/szName`

- **Type:** `String` (`String`)
- **Description:** Display name of the drug.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/opinionevent`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, other mercenaries react negatively to drug use.
- **Values:** `0` or `1`; when set, other mercenaries complain about the drug use.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/DRUG_EFFECT/effect`

- **Type:** `Integer` (`UINT8`)
- **Description:** Stat or resource changed by this effect.
- **Values:** IDs from the drug-effect list: `0` HP, `1` BP, `2` AP, `3` Morale, `4` Physical resistance, `5` Strength, `6` Agility, `7` Dexterity, `8` Wisdom.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DRUG_EFFECT/duration`

- **Type:** `Integer` (`UINT16`)
- **Description:** Duration of the effect in turns.
- **Values:** Duration in turns.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/DRUG_EFFECT/size`

- **Type:** `Integer` (`INT16`)
- **Description:** Strength of the effect. Positive and negative values are allowed.
- **Values:** Effect strength; positive and negative values are allowed.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/DRUG_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that this effect is applied when the drug is consumed. If omitted, the effect is applied without a chance roll.
- **Values:** Percentage chance that this effect is applied when the drug is consumed; the loader default applies when omitted.
- **Default:** absent: the effect applies without a chance roll.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/DISEASE_EFFECT/disease`

- **Type:** `Integer` (`UINT8`)
- **Description:** Disease row affected by this drug.
- **Only used with:** Disease system.
- **Values:** Disease row index in `Disease.xml`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/DISEASE_EFFECT/size`

- **Type:** `Integer` (`INT16`)
- **Description:** Disease points added; positive increases the disease, negative cures it.
- **Only used with:** Disease system.
- **Values:** Disease points added; positive increases the disease, negative cures it.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/DISEASE_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that this disease effect is applied. If omitted, the effect is applied without a chance roll.
- **Only used with:** Disease system.
- **Values:** Percentage chance that this disease effect is applied.
- **Default:** absent: the effect applies without a chance roll.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/DISABILITY_EFFECT/disability`

- **Type:** `Integer` (`UINT8`)
- **Description:** Disability added for the duration of the effect.
- **Values:** Disability ID: `1` Heat intolerant, `2` Nervous, `3` Claustrophobic, `4` Nonswimmer, `5` Fear of insects, `6` Forgetful, `7` Psycho, `8` Deaf, `9` Shortsighted, `10` Hemophiliac, `11` Afraid of heights, `12` Self-harm.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISABILITY_EFFECT/duration`

- **Type:** `Integer` (`UINT16`)
- **Description:** Duration of the disability in turns.
- **Values:** Duration of the disability in turns.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/DISABILITY_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that this disability is applied. If omitted, the effect is applied without a chance roll.
- **Values:** Percentage chance that this disability is applied.
- **Default:** absent: the effect applies without a chance roll.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/PERSONALITY_EFFECT/personality`

- **Type:** `Integer` (`UINT8`)
- **Description:** Personality trait added for the duration of the effect.
- **Values:** Personality trait ID: `1` Sociable, `2` Loner, `3` Optimist, `4` Assertive, `5` Intellectual, `6` Primitive, `7` Aggressive, `8` Phlegmatic, `9` Dauntless, `10` Pacifist, `11` Malicious, `12` Show-off.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/PERSONALITY_EFFECT/duration`

- **Type:** `Integer` (`UINT16`)
- **Description:** Duration of the personality effect in turns.
- **Values:** Duration of the personality effect in turns.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

### `DRUGSLIST/DRUG/PERSONALITY_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance that this personality effect is applied. If omitted, the effect is applied without a chance roll.
- **Values:** Percentage chance that this personality effect is applied.
- **Default:** absent: the effect applies without a chance roll.
- **Source:** drug table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `uiIndex` is referenced by `DrugType` in [Items](Items.md).
- `DISEASE_EFFECT/disease` refers to a row in `Disease.xml`.
- Multiple effect blocks may be repeated; the XML comment notes that only one disability/personality effect is active at a time.

## Localization

`szName` is a display label; effect IDs and enum values are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Drug table data.
- Drug schema comment.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and the table schema comments; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The loader default for an omitted `chance` is not stated in the table comment; the description does not assume 100%.
