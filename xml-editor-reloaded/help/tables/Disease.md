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
  - [`DISEASESLIST/DISEASE/fCanBeCured`](#diseaseslistdiseasefcanbecured)
  - [`DISEASESLIST/DISEASE/fCanReInfect`](#diseaseslistdiseasefcanreinfect)
  - [`DISEASESLIST/DISEASE/fDisgusting`](#diseaseslistdiseasefdisgusting)
  - [`DISEASESLIST/DISEASE/fHideSymbol`](#diseaseslistdiseasefhidesymbol)
  - [`DISEASESLIST/DISEASE/fReverseOnFull`](#diseaseslistdiseasefreverseonfull)
  - [`DISEASESLIST/DISEASE/fSpecialFlagContractDisability`](#diseaseslistdiseasefspecialflagcontractdisability)
  - [`DISEASESLIST/DISEASE/fSpecialFlagLimitedUseArms`](#diseaseslistdiseasefspecialflaglimitedusearms)
  - [`DISEASESLIST/DISEASE/fSpecialFlagLimitedUseLegs`](#diseaseslistdiseasefspecialflaglimiteduselegs)
  - [`DISEASESLIST/DISEASE/fSpecialFlagPTSDBuns`](#diseaseslistdiseasefspecialflagptsdbuns)
  - [`DISEASESLIST/DISEASE/InfectionChance_BADFOOD`](#diseaseslistdiseaseinfectionchance_badfood)
  - [`DISEASESLIST/DISEASE/InfectionChance_BADWATER`](#diseaseslistdiseaseinfectionchance_badwater)
  - [`DISEASESLIST/DISEASE/InfectionChance_CONTACT_CORPSE`](#diseaseslistdiseaseinfectionchance_contact_corpse)
  - [`DISEASESLIST/DISEASE/InfectionChance_CONTACT_HUMAN`](#diseaseslistdiseaseinfectionchance_contact_human)
  - [`DISEASESLIST/DISEASE/InfectionChance_SEX`](#diseaseslistdiseaseinfectionchance_sex)
  - [`DISEASESLIST/DISEASE/InfectionChance_SWAMP`](#diseaseslistdiseaseinfectionchance_swamp)
  - [`DISEASESLIST/DISEASE/InfectionChance_TROPICS`](#diseaseslistdiseaseinfectionchance_tropics)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_AGI`](#diseaseslistdiseaseinfectionchance_wound_agi)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_ANIMAL`](#diseaseslistdiseaseinfectionchance_wound_animal)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_DEX`](#diseaseslistdiseaseinfectionchance_wound_dex)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_FIRE`](#diseaseslistdiseaseinfectionchance_wound_fire)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_GAS`](#diseaseslistdiseaseinfectionchance_wound_gas)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_GUNSHOT`](#diseaseslistdiseaseinfectionchance_wound_gunshot)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_OPEN`](#diseaseslistdiseaseinfectionchance_wound_open)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_STR`](#diseaseslistdiseaseinfectionchance_wound_str)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_TRAUMATIC`](#diseaseslistdiseaseinfectionchance_wound_traumatic)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_WIS`](#diseaseslistdiseaseinfectionchance_wound_wis)
  - [`DISEASESLIST/DISEASE/moralemodifier`](#diseaseslistdiseasemoralemodifier)
  - [`DISEASESLIST/DISEASE/sDrinkModifier`](#diseaseslistdiseasesdrinkmodifier)
  - [`DISEASESLIST/DISEASE/sEffAP`](#diseaseslistdiseaseseffap)
  - [`DISEASESLIST/DISEASE/sEffCarryStrength`](#diseaseslistdiseaseseffcarrystrength)
  - [`DISEASESLIST/DISEASE/sEffStatAGI`](#diseaseslistdiseaseseffstatagi)
  - [`DISEASESLIST/DISEASE/sEffStatDEX`](#diseaseslistdiseaseseffstatdex)
  - [`DISEASESLIST/DISEASE/sEffStatEXP`](#diseaseslistdiseaseseffstatexp)
  - [`DISEASESLIST/DISEASE/sEffStatSTR`](#diseaseslistdiseaseseffstatstr)
  - [`DISEASESLIST/DISEASE/sEffStatWIS`](#diseaseslistdiseaseseffstatwis)
  - [`DISEASESLIST/DISEASE/sFoodModifier`](#diseaseslistdiseasesfoodmodifier)
  - [`DISEASESLIST/DISEASE/sInfectionPtsFull`](#diseaseslistdiseasesinfectionptsfull)
  - [`DISEASESLIST/DISEASE/sInfectionPtsGainPerHour`](#diseaseslistdiseasesinfectionptsgainperhour)
  - [`DISEASESLIST/DISEASE/sInfectionPtsInitial`](#diseaseslistdiseasesinfectionptsinitial)
  - [`DISEASESLIST/DISEASE/sInfectionPtsOutbreak`](#diseaseslistdiseasesinfectionptsoutbreak)
  - [`DISEASESLIST/DISEASE/sLifeRegenHundreds`](#diseaseslistdiseasesliferegenhundreds)
  - [`DISEASESLIST/DISEASE/sNeedToSleep`](#diseaseslistdiseasesneedtosleep)
  - [`DISEASESLIST/DISEASE/szDescription`](#diseaseslistdiseaseszdescription)
  - [`DISEASESLIST/DISEASE/szFatName`](#diseaseslistdiseaseszfatname)
  - [`DISEASESLIST/DISEASE/szName`](#diseaseslistdiseaseszname)
  - [`DISEASESLIST/DISEASE/uiIndex`](#diseaseslistdiseaseuiindex)
  - [`DISEASESLIST/DISEASE/usMaxBreath`](#diseaseslistdiseaseusmaxbreath)
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
└── DISEASE                 one disease
    ├── uiIndex             disease identity
    ├── szName/szFatName    names
    ├── InfectionChance_*   chance by source
    ├── sEffStat*/sEffAP    effects
    └── fSpecialFlag*       special flags
```

## Fields

### `DISEASESLIST/DISEASE/fCanBeCured`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the disease can be cured.
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

### `DISEASESLIST/DISEASE/fDisgusting`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Marks the disease as disgusting for reactions and descriptions.
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

### `DISEASESLIST/DISEASE/fReverseOnFull`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the disease reverses when the full stage is reached.
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

### `DISEASESLIST/DISEASE/fSpecialFlagPTSDBuns`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Special PTSB flag used by the disease system.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_BADFOOD`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from badfood.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_BADWATER`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from badwater.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_CONTACT_CORPSE`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from contact corpse.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_CONTACT_HUMAN`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from contact human.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_SEX`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from sex.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_SWAMP`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Infection chance from swamp.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_TROPICS`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Infection chance from tropics.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_AGI`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound agi.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_ANIMAL`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound animal.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_DEX`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound dex.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_FIRE`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound fire.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_GAS`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound gas.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_GUNSHOT`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound gunshot.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_OPEN`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound open.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_STR`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound str.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_TRAUMATIC`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound traumatic.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_WIS`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection chance from wound wis.
- **Only used with:** DISEASE.
- **Values:** Decimal infection chance.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/moralemodifier`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Morale modifier while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sDrinkModifier`

- **Type:** `Integer` (`INT32`)
- **Description:** Drink modifier while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffAP`

- **Type:** `Integer` (`INT32`)
- **Description:** AP effect while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffCarryStrength`

- **Type:** `Integer` (`INT32`)
- **Description:** Carry-strength effect while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatAGI`

- **Type:** `Integer` (`INT32`)
- **Description:** Agility effect while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatDEX`

- **Type:** `Integer` (`INT32`)
- **Description:** Dexterity effect while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatEXP`

- **Type:** `Integer` (`INT32`)
- **Description:** Experience effect while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatSTR`

- **Type:** `Integer` (`INT32`)
- **Description:** Strength effect while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sEffStatWIS`

- **Type:** `Integer` (`INT32`)
- **Description:** Wisdom effect while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sFoodModifier`

- **Type:** `Integer` (`INT32`)
- **Description:** Food modifier while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sInfectionPtsFull`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection points at which the disease is fully developed.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sInfectionPtsGainPerHour`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection points gained per hour.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sInfectionPtsInitial`

- **Type:** `Integer` (`INT32`)
- **Description:** Initial infection points.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sInfectionPtsOutbreak`

- **Type:** `Integer` (`INT32`)
- **Description:** Infection points at which the disease breaks out.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sLifeRegenHundreds`

- **Type:** `Integer` (`INT32`)
- **Description:** Life-regeneration modifier, in hundredths.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/sNeedToSleep`

- **Type:** `Integer` (`INT32`)
- **Description:** Sleep-need modifier while infected.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
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

### `DISEASESLIST/DISEASE/szFatName`

- **Type:** `String` (`String`)
- **Description:** Fat/alternate display name used by the disease UI.
- **Only used with:** DISEASE.
- **Values:** Free text.
- **Default:** empty string when omitted.
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

### `DISEASESLIST/DISEASE/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Disease row identity.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** disease table data; table metadata.
- **Confidence:** `inferred`

### `DISEASESLIST/DISEASE/usMaxBreath`

- **Type:** `Integer` (`INT32`)
- **Description:** Maximum breath modifier while infected.
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
