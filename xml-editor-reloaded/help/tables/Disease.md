---
schema: 1
lang: en
family: disease
title: "Disease - disease definitions"
tables:
  - Disease
table_ids:
  - Disease
files:
  - Disease.xml
root: DISEASESLIST
record: DISEASE
key: uiIndex
sources:
  - Disease table data
  - Disease schema comments
status: draft
updated: 2026-11-07
---

# Disease - disease definitions

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`DISEASESLIST/DISEASE/uiIndex`](#diseaseslistdiseaseuiindex)
  - [`DISEASESLIST/DISEASE/szName`](#diseaseslistdiseaseszname)
  - [`DISEASESLIST/DISEASE/szFatName`](#diseaseslistdiseaseszfatname)
  - [`DISEASESLIST/DISEASE/szDescription`](#diseaseslistdiseaseszdescription)
  - [`DISEASESLIST/DISEASE/sInfectionPtsInitial`](#diseaseslistdiseasesinfectionptsinitial)
  - [`DISEASESLIST/DISEASE/sInfectionPtsOutbreak`](#diseaseslistdiseasesinfectionptsoutbreak)
  - [`DISEASESLIST/DISEASE/sInfectionPtsFull`](#diseaseslistdiseasesinfectionptsfull)
  - [`DISEASESLIST/DISEASE/sInfectionPtsGainPerHour`](#diseaseslistdiseasesinfectionptsgainperhour)
  - [`DISEASESLIST/DISEASE/InfectionChance_SWAMP`](#diseaseslistdiseaseinfectionchance_swamp)
  - [`DISEASESLIST/DISEASE/InfectionChance_TROPICS`](#diseaseslistdiseaseinfectionchance_tropics)
  - [`DISEASESLIST/DISEASE/InfectionChance_SEX`](#diseaseslistdiseaseinfectionchance_sex)
  - [`DISEASESLIST/DISEASE/InfectionChance_CONTACT_HUMAN`](#diseaseslistdiseaseinfectionchance_contact_human)
  - [`DISEASESLIST/DISEASE/InfectionChance_CONTACT_CORPSE`](#diseaseslistdiseaseinfectionchance_contact_corpse)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_ANIMAL`](#diseaseslistdiseaseinfectionchance_wound_animal)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_OPEN`](#diseaseslistdiseaseinfectionchance_wound_open)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_GUNSHOT`](#diseaseslistdiseaseinfectionchance_wound_gunshot)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_FIRE`](#diseaseslistdiseaseinfectionchance_wound_fire)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_GAS`](#diseaseslistdiseaseinfectionchance_wound_gas)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_AGI`](#diseaseslistdiseaseinfectionchance_wound_agi)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_DEX`](#diseaseslistdiseaseinfectionchance_wound_dex)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_STR`](#diseaseslistdiseaseinfectionchance_wound_str)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_WIS`](#diseaseslistdiseaseinfectionchance_wound_wis)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_TRAUMATIC`](#diseaseslistdiseaseinfectionchance_wound_traumatic)
  - [`DISEASESLIST/DISEASE/InfectionChance_BADFOOD`](#diseaseslistdiseaseinfectionchance_badfood)
  - [`DISEASESLIST/DISEASE/InfectionChance_BADWATER`](#diseaseslistdiseaseinfectionchance_badwater)
  - [`DISEASESLIST/DISEASE/fCanBeCured`](#diseaseslistdiseasefcanbecured)
  - [`DISEASESLIST/DISEASE/fReverseOnFull`](#diseaseslistdiseasefreverseonfull)
  - [`DISEASESLIST/DISEASE/fCanReInfect`](#diseaseslistdiseasefcanreinfect)
  - [`DISEASESLIST/DISEASE/fHideSymbol`](#diseaseslistdiseasefhidesymbol)
  - [`DISEASESLIST/DISEASE/fDisgusting`](#diseaseslistdiseasefdisgusting)
  - [`DISEASESLIST/DISEASE/fSpecialFlagPTSDBuns`](#diseaseslistdiseasefspecialflagptsdbuns)
  - [`DISEASESLIST/DISEASE/fSpecialFlagContractDisability`](#diseaseslistdiseasefspecialflagcontractdisability)
  - [`DISEASESLIST/DISEASE/fSpecialFlagLimitedUseArms`](#diseaseslistdiseasefspecialflaglimitedusearms)
  - [`DISEASESLIST/DISEASE/fSpecialFlagLimitedUseLegs`](#diseaseslistdiseasefspecialflaglimiteduselegs)
  - [`DISEASESLIST/DISEASE/sEffStatAGI`](#diseaseslistdiseaseseffstatagi)
  - [`DISEASESLIST/DISEASE/sEffStatDEX`](#diseaseslistdiseaseseffstatdex)
  - [`DISEASESLIST/DISEASE/sEffStatSTR`](#diseaseslistdiseaseseffstatstr)
  - [`DISEASESLIST/DISEASE/sEffStatWIS`](#diseaseslistdiseaseseffstatwis)
  - [`DISEASESLIST/DISEASE/sEffStatEXP`](#diseaseslistdiseaseseffstatexp)
  - [`DISEASESLIST/DISEASE/sEffAP`](#diseaseslistdiseaseseffap)
  - [`DISEASESLIST/DISEASE/usMaxBreath`](#diseaseslistdiseaseusmaxbreath)
  - [`DISEASESLIST/DISEASE/sEffCarryStrength`](#diseaseslistdiseaseseffcarrystrength)
  - [`DISEASESLIST/DISEASE/sLifeRegenHundreds`](#diseaseslistdiseasesliferegenhundreds)
  - [`DISEASESLIST/DISEASE/sNeedToSleep`](#diseaseslistdiseasesneedtosleep)
  - [`DISEASESLIST/DISEASE/sDrinkModifier`](#diseaseslistdiseasesdrinkmodifier)
  - [`DISEASESLIST/DISEASE/sFoodModifier`](#diseaseslistdiseasesfoodmodifier)
  - [`DISEASESLIST/DISEASE/moralemodifier`](#diseaseslistdiseasemoralemodifier)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Disease.xml` defines diseases: infection thresholds, infection chances by source, cure/reinfection flags, stat and AP effects, sleep/food/drink modifiers and special severe-limitation flags. The table is active only when the disease system is enabled.

## Tables

| Table | File | Notes |
|---|---|---|
| `Disease` | `Disease.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
DISEASESLIST
└── DISEASE
    ├── uiIndex
    ├── szName
    ├── szFatName
    ├── szDescription
    ├── sInfectionPtsInitial
    ├── sInfectionPtsOutbreak
    ├── sInfectionPtsFull
    ├── sInfectionPtsGainPerHour
    ├── InfectionChance_SWAMP
    ├── InfectionChance_TROPICS
    ├── InfectionChance_SEX
    ├── InfectionChance_CONTACT_HUMAN
    ├── InfectionChance_CONTACT_CORPSE
    ├── InfectionChance_WOUND_ANIMAL
    ├── InfectionChance_WOUND_OPEN
    ├── InfectionChance_WOUND_GUNSHOT
    ├── InfectionChance_WOUND_FIRE
    ├── InfectionChance_WOUND_GAS
    ├── InfectionChance_WOUND_AGI
    ├── InfectionChance_WOUND_DEX
    ├── InfectionChance_WOUND_STR
    ├── InfectionChance_WOUND_WIS
    ├── InfectionChance_WOUND_TRAUMATIC
    ├── InfectionChance_BADFOOD
    ├── InfectionChance_BADWATER
    ├── fCanBeCured
    ├── fReverseOnFull
    ├── fCanReInfect
    ├── fHideSymbol
    ├── fDisgusting
    ├── fSpecialFlagPTSDBuns
    ├── fSpecialFlagContractDisability
    ├── fSpecialFlagLimitedUseArms
    ├── fSpecialFlagLimitedUseLegs
    ├── sEffStatAGI
    ├── sEffStatDEX
    ├── sEffStatSTR
    ├── sEffStatWIS
    ├── sEffStatEXP
    ├── sEffAP
    ├── usMaxBreath
    ├── sEffCarryStrength
    ├── sLifeRegenHundreds
    ├── sNeedToSleep
    ├── sDrinkModifier
    ├── sFoodModifier
    └── moralemodifier
```

## Fields

### `DISEASESLIST/DISEASE/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Disease row identity.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/szName`

- **Type:** `String` (`String`)
- **Description:** Display name of the disease.
- **Only used with:** DISEASE.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/szFatName`

- **Type:** `String` (`String`)
- **Description:** Fat/alternate display name used by the disease UI.
- **Only used with:** DISEASE.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/szDescription`

- **Type:** `String` (`String`)
- **Description:** Player-facing disease description.
- **Only used with:** DISEASE.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sInfectionPtsInitial`

- **Type:** `Integer` (`INT32`)
- **Description:** Initial infection points. Numeric semantics: Infection points; higher values mean more infection or a faster infection gain, and negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sInfectionPtsOutbreak`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection points at which the disease breaks out. Numeric semantics: Infection points; higher values mean more infection or a faster infection gain, and negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sInfectionPtsFull`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection points at which the disease is fully developed. Numeric semantics: Infection points; higher values mean more infection or a faster infection gain, and negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sInfectionPtsGainPerHour`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection points gained per hour. Numeric semantics: Infection points; higher values mean more infection or a faster infection gain, and negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_SWAMP`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Infection chance from swamp. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_TROPICS`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Infection chance from tropics. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_SEX`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from sex. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_CONTACT_HUMAN`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from contact human. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_CONTACT_CORPSE`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from contact corpse. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_ANIMAL`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound animal. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_OPEN`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound open. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_GUNSHOT`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound gunshot. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_FIRE`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound fire. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_GAS`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound gas. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_AGI`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound agi. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_DEX`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound dex. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_STR`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound str. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_WIS`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound wis. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_TRAUMATIC`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound traumatic. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_BADFOOD`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from badfood. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_BADWATER`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from badwater. Numeric semantics: Higher values mean a greater chance; negative values are outside the valid chance range.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/fCanBeCured`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the disease can be cured.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/fReverseOnFull`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the disease reverses when the full stage is reached.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/fCanReInfect`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the mercenary can be reinfected.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/fHideSymbol`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the disease symbol is hidden in the UI.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/fDisgusting`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Marks the disease as disgusting for reactions and descriptions.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/fSpecialFlagPTSDBuns`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Special PTSB flag used by the disease system.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/fSpecialFlagContractDisability`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Adds a contract disability; only read when `DISEASE_SEVERE_LIMITATIONS` is enabled.
- **Only used with:** DISEASE, DISEASE_SEVERE_LIMITATIONS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/fSpecialFlagLimitedUseArms`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Limits use of the arms; only read when `DISEASE_SEVERE_LIMITATIONS` is enabled.
- **Only used with:** DISEASE, DISEASE_SEVERE_LIMITATIONS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/fSpecialFlagLimitedUseLegs`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Limits use of the legs; only read when `DISEASE_SEVERE_LIMITATIONS` is enabled.
- **Only used with:** DISEASE, DISEASE_SEVERE_LIMITATIONS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatAGI`

- **Type:** `Integer` (`INT32`)
- **Description:** Agility effect while infected. Numeric semantics: Signed stat effect: positive values increase the stat, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatDEX`

- **Type:** `Integer` (`INT32`)
- **Description:** Dexterity effect while infected. Numeric semantics: Signed stat effect: positive values increase the stat, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatSTR`

- **Type:** `Integer` (`INT32`)
- **Description:** Strength effect while infected. Numeric semantics: Signed stat effect: positive values increase the stat, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatWIS`

- **Type:** `Integer` (`INT32`)
- **Description:** Wisdom effect while infected. Numeric semantics: Signed stat effect: positive values increase the stat, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatEXP`

- **Type:** `Integer` (`INT32`)
- **Description:** Experience effect while infected. Numeric semantics: Signed stat effect: positive values increase the stat, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffAP`

- **Type:** `Integer` (`INT32`)
- **Description:** AP effect while infected. Numeric semantics: Signed effect: positive values increase the value, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/usMaxBreath`

- **Type:** `Integer` (`INT32`)
- **Description:** Maximum breath modifier while infected. Numeric semantics: Higher values mean a larger quantity; negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffCarryStrength`

- **Type:** `Integer` (`INT32`)
- **Description:** Carry-strength effect while infected. Numeric semantics: Signed effect: positive values increase the value, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sLifeRegenHundreds`

- **Type:** `Integer` (`INT32`)
- **Description:** Life-regeneration modifier, in hundredths. Numeric semantics: Signed life-regeneration value: positive values regenerate life, negative values drain it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sNeedToSleep`

- **Type:** `Integer` (`INT32`)
- **Description:** Sleep-need modifier while infected. Numeric semantics: Signed need modifier: higher values increase the need/effect, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sDrinkModifier`

- **Type:** `Integer` (`INT32`)
- **Description:** Drink modifier while infected. Numeric semantics: Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sFoodModifier`

- **Type:** `Integer` (`INT32`)
- **Description:** Food modifier while infected. Numeric semantics: Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/moralemodifier`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Morale modifier while infected. Numeric semantics: Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

## Relations

- `DISEASE_EFFECT/disease` in [Drugs](Items/Drugs.md) points to `Disease.uiIndex`.
- The disease system is controlled by `DISEASE`; the three special flags also require `DISEASE_SEVERE_LIMITATIONS`.

## Localization

`szName`, `szFatName` and `szDescription` are display text; numeric effects are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Disease table data.
- Disease schema comments.
- Field types: table data and field names; enum values come from the schema comments.
- Descriptions are conservative where no formula is stated; no observed value ranges are used.

## Open questions

The disease-point thresholds and the exact severe-limitation effects are partly defined by the disease code; the table stores the parameters.
