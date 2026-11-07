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
  - Game code (disease loader and hourly update)
  - Disease table data
  - Disease schema comments
status: reviewed
updated: 2026-11-08
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

`Disease.xml` defines each disease as a set of disease points plus the effects those points produce. Infection adds initial points; the points then rise or fall each hour until they cross the outbreak threshold, and the full threshold scales the symptoms. The table also defines the base chance for each infection source and the flags that control cure, reinfection, visibility and severe limitations. It is active only when the disease system is enabled.

Infection chances are percentages, not points: each field is the base chance for one qualifying event, such as an hourly check in a swamp, a wound, or a bad meal. Disease resistance and situational modifiers reduce the final chance. Effects are defined at full infection and are interpolated from `1.0` down to the current magnitude.

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
- **Description:** Index of this disease in the `Disease` array. It is an array key, not a magnitude; indexes must be unique and non-negative.
- **Only used with:** DISEASE.
- **Values:** Integer disease value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/szName`

- **Type:** `String` (`String`)
- **Description:** Display name shown in the disease list and diagnosis messages.
- **Only used with:** DISEASE.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/szFatName`

- **Type:** `String` (`String`)
- **Description:** Alternate name used by the diagnosis/status display. The `|` characters control letter-by-letter highlighting; they are not part of the displayed name.
- **Only used with:** DISEASE.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/szDescription`

- **Type:** `String` (`String`)
- **Description:** Player-facing description of the disease, shown in the disease details panel.
- **Only used with:** DISEASE.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sInfectionPtsInitial`

- **Type:** `Integer` (`INT32`)
- **Description:** Disease points added when the mercenary is first infected. Points accumulate toward `sInfectionPtsOutbreak` and `sInfectionPtsFull`. Numeric semantics: higher values start the infection closer to an outbreak; negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Integer disease points.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sInfectionPtsOutbreak`

- **Type:** `Integer` (`INT32`)
- **Description:** Disease points at which the infection breaks out: symptoms and stat effects begin, and the disease becomes visible. Numeric semantics: higher values delay the outbreak; negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Integer disease points.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sInfectionPtsFull`

- **Type:** `Integer` (`INT32`)
- **Description:** Nominal disease points of a full-blown infection. Effects are scaled by current points divided by this value. Numeric semantics: higher values require more points to reach full effect; negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Integer disease points.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sInfectionPtsGainPerHour`

- **Type:** `Integer` (`INT32`)
- **Description:** Disease points added each hour while infected. A negative value removes points each hour, so the disease improves. With `fReverseOnFull`, the sign of this gain is reversed at the full stage, so a normal positive gain starts removing points. Numeric semantics: higher values progress faster; negative values recover.
- **Only used with:** DISEASE.
- **Values:** Integer points per hour; negative values remove points.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_SWAMP`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, of infection during each hourly disease check while the mercenary is in a swamp or swamp-road sector. Disease resistance and fear of insects modify the final chance. Numeric semantics: higher values are more infectious; `0.5` means 0.5%, and negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`; fractional values are allowed (`0.5` = 0.5%).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_TROPICS`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, of infection during each hourly disease check while the mercenary is in a tropical or tropical-road sector. Disease resistance and fear of insects modify the final chance. Numeric semantics: higher values are more infectious; `0.25` means 0.25%, and negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`; fractional values are allowed (`0.25` = 0.25%).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_SEX`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, of infection from one sexual encounter. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_CONTACT_HUMAN`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, of infection per hourly check when another infected mercenary is in the same sector. Sharing a squad or vehicle, or both being patients, increases the final chance. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_CONTACT_CORPSE`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, of infection per hourly check when the sector contains disease points from corpses. The final chance also scales with the sector disease load. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_ANIMAL`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when the mercenary is wounded by an animal or creature, such as a bloodcat or tentacles. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_OPEN`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when a blade, hand-to-hand or explosion wound leaves a fresh open wound. The final chance is modified by the damage dealt. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_GUNSHOT`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when a gunshot wound deals more than 20 damage. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_FIRE`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when the mercenary is wounded by fire. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_GAS`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when the mercenary is wounded by gas that is not fire. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_AGI`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when a wound causes critical Agility loss, not merely any wound. This is the `WOUND_AGI` event from the disease system. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_DEX`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when a wound causes critical Dexterity loss, not merely any wound. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_STR`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when a wound causes critical Strength loss, not merely any wound. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_WIS`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when a wound causes critical Wisdom loss, not merely any wound. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_TRAUMATIC`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, for a traumatic event rather than a physical wound: life falling below `OKLIFE` after damage, or a severe morale shock with morale below 15. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_BADFOOD`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when eating food in bad condition. The final chance rises as the food condition worsens. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_BADWATER`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Base chance, in percent, when drinking water in bad condition. The final chance rises as the water condition worsens. Numeric semantics: higher values are more infectious; negative values are outside the valid range.
- **Only used with:** DISEASE.
- **Values:** Percentage, clamped to `0..100`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fCanBeCured`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, doctors can cure the disease. If clear, the infection can only run its course or reverse through its point gain.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fReverseOnFull`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, once disease points reach `sInfectionPtsFull`, the sign of `sInfectionPtsGainPerHour` is reversed. With a normal positive gain this makes the infection decline.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fCanReInfect`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the disease can be applied again while the mercenary is already infected; if clear, an existing infection blocks a new one.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fHideSymbol`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, no disease symbol is shown on the mercenary portrait, even after diagnosis.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fDisgusting`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, other mercenaries react with disgust when the disease is diagnosed and has broken out.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fSpecialFlagPTSDBuns`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set and the mercenary is Buns (or her chaotic personality), this disease can trigger her PTSD personality change.
- **Only used with:** DISEASE.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fSpecialFlagContractDisability`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, contracting this disease adds a new disability to the mercenary. Only read when `DISEASE_SEVERE_LIMITATIONS` is enabled.
- **Only used with:** DISEASE, DISEASE_SEVERE_LIMITATIONS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fSpecialFlagLimitedUseArms`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the disease limits use of the arms. A splint can improve the condition. Only read when `DISEASE_SEVERE_LIMITATIONS` is enabled.
- **Only used with:** DISEASE, DISEASE_SEVERE_LIMITATIONS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fSpecialFlagLimitedUseLegs`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, the disease limits use of the legs. A splint can improve the condition. Only read when `DISEASE_SEVERE_LIMITATIONS` is enabled.
- **Only used with:** DISEASE, DISEASE_SEVERE_LIMITATIONS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatAGI`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage change to effective Agility at full infection. The game applies it to the effective-stat calculation, scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: `+10` means +10% effective Agility, `-10` means -10%, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Percentage; `+10` = +10% effective Agility at full, `-10` = -10%.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatDEX`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage change to effective Dexterity at full infection. The game applies it to the effective-stat calculation, scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: `+10` means +10% effective Dexterity, `-10` means -10%, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Percentage; `+10` = +10% effective Dexterity at full, `-10` = -10%.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatSTR`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage change to effective Strength at full infection. The game applies it to the effective-stat calculation, scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: `+10` means +10% effective Strength, `-10` means -10%, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Percentage; `+10` = +10% effective Strength at full, `-10` = -10%.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatWIS`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage change to effective Wisdom at full infection. The game applies it to the effective-stat calculation, scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: `+10` means +10% effective Wisdom, `-10` means -10%, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Percentage; `+10` = +10% effective Wisdom at full, `-10` = -10%.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatEXP`

- **Type:** `Integer` (`INT32`)
- **Description:** Change to effective experience level at full infection, in levels. The game adds it to the effective-level calculation, scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: positive values raise the effective level, negative values lower it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Effective-level points; positive raises, negative lowers.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffAP`

- **Type:** `Integer` (`INT32`)
- **Description:** Action-point change at full infection. The actual change is scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: positive values add AP, negative values subtract AP, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** AP points; positive adds, negative subtracts.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/usMaxBreath`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage by which maximum breath is lowered at full infection. The actual reduction is scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: higher values reduce breath more; `20` means -20% at full, and negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Percentage `0..100`; `20` means -20% at full infection.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffCarryStrength`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage change to carrying strength at full infection. The game applies it to the carry-strength calculation, scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: `+10` means +10% carrying strength, `-10` means -10%, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Percentage; `+10` = +10% carrying strength at full, `-10` = -10%.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sLifeRegenHundreds`

- **Type:** `Integer` (`INT32`)
- **Description:** Hourly life-regeneration change at full infection, in hundredths of a life point. The actual change is scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: positive values heal, negative values drain life, and `0` is neutral; `-600` means -6 HP per hour at full.
- **Only used with:** DISEASE.
- **Values:** Hundredths of a life point per hour; negative values drain life.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sNeedToSleep`

- **Type:** `Integer` (`INT32`)
- **Description:** Sleep-need change at full infection. The actual change is scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: positive values increase the need for sleep, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Sleep-need points; positive increases, negative reduces.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sDrinkModifier`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage change to water consumption at full infection. The actual change is scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: positive values increase water use, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Percentage change to water consumption.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sFoodModifier`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage change to food consumption at full infection. The actual change is scaled by current disease points divided by `sInfectionPtsFull`. Numeric semantics: positive values increase food use, negative values reduce it, and `0` is neutral.
- **Only used with:** DISEASE.
- **Values:** Percentage change to food consumption.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/moralemodifier`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Morale multiplier at full infection. The game interpolates from `1.0` toward this value as the disease approaches full. Numeric semantics: `1.0` is neutral, `0.8` means 20% lower morale, and `1.2` means 20% higher morale; negative values are not meaningful.
- **Only used with:** DISEASE.
- **Values:** Float multiplier; `1.0` is neutral.
- **Default:** `1.0` when omitted.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

## Relations

- `DISEASE_EFFECT/disease` in [Drugs](Items/Drugs.md) points to `Disease.uiIndex`.
- The disease system is controlled by `DISEASE`; the three special flags also require `DISEASE_SEVERE_LIMITATIONS`.

## Localization

`szName`, `szFatName` and `szDescription` are display text; numeric effects are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Game code: `XML_Disease.cpp` (loader), `Disease.cpp` (hourly update and infection checks), `Soldier Control.cpp` (magnitude and effect application).
- Disease table data and schema comments.
- Field types and clamping: disease code and XML loader.
- Descriptions cover the code path that consumes each field; no row counts or observed data ranges are used.

## Open questions

The table stores balance values; it does not define how often a sex encounter or a bad meal occurs. The exact splint and severe-limitation behavior is implemented by the disease code, not by additional XML fields.
