---
schema: 1
lang: en
family: mercs-profiles
title: "MercProfiles - mercenary and NPC profiles"
tables:
  - MercProfiles
table_ids:
  - MercProfiles
files:
  - MercProfiles.xml
root: MERCPROFILES
record: PROFILE
key: uiIndex
sources:
  - Merc profile table data
  - MercProfiles schema comments
status: reviewed
updated: 2026-11-07
---

# MercProfiles - mercenary and NPC profiles

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERCPROFILES/PROFILE/uiIndex`](#mercprofilesprofileuiindex)
  - [`MERCPROFILES/PROFILE/Type`](#mercprofilesprofiletype)
  - [`MERCPROFILES/PROFILE/zName`](#mercprofilesprofilezname)
  - [`MERCPROFILES/PROFILE/zNickname`](#mercprofilesprofileznickname)
  - [`MERCPROFILES/PROFILE/ubFaceIndex`](#mercprofilesprofileubfaceindex)
  - [`MERCPROFILES/PROFILE/usVoiceIndex`](#mercprofilesprofileusvoiceindex)
  - [`MERCPROFILES/PROFILE/usEyesX`](#mercprofilesprofileuseyesx)
  - [`MERCPROFILES/PROFILE/usEyesY`](#mercprofilesprofileuseyesy)
  - [`MERCPROFILES/PROFILE/usMouthX`](#mercprofilesprofileusmouthx)
  - [`MERCPROFILES/PROFILE/usMouthY`](#mercprofilesprofileusmouthy)
  - [`MERCPROFILES/PROFILE/uiEyeDelay`](#mercprofilesprofileuieyedelay)
  - [`MERCPROFILES/PROFILE/uiMouthDelay`](#mercprofilesprofileuimouthdelay)
  - [`MERCPROFILES/PROFILE/uiBlinkFrequency`](#mercprofilesprofileuiblinkfrequency)
  - [`MERCPROFILES/PROFILE/uiExpressionFrequency`](#mercprofilesprofileuiexpressionfrequency)
  - [`MERCPROFILES/PROFILE/PANTS`](#mercprofilesprofilepants)
  - [`MERCPROFILES/PROFILE/VEST`](#mercprofilesprofilevest)
  - [`MERCPROFILES/PROFILE/SKIN`](#mercprofilesprofileskin)
  - [`MERCPROFILES/PROFILE/HAIR`](#mercprofilesprofilehair)
  - [`MERCPROFILES/PROFILE/bSex`](#mercprofilesprofilebsex)
  - [`MERCPROFILES/PROFILE/ubBodyType`](#mercprofilesprofileubbodytype)
  - [`MERCPROFILES/PROFILE/uiBodyTypeSubFlags`](#mercprofilesprofileuibodytypesubflags)
  - [`MERCPROFILES/PROFILE/bAttitude`](#mercprofilesprofilebattitude)
  - [`MERCPROFILES/PROFILE/bCharacterTrait`](#mercprofilesprofilebcharactertrait)
  - [`MERCPROFILES/PROFILE/bDisability`](#mercprofilesprofilebdisability)
  - [`MERCPROFILES/PROFILE/ubNeedForSleep`](#mercprofilesprofileubneedforsleep)
  - [`MERCPROFILES/PROFILE/bReputationTolerance`](#mercprofilesprofilebreputationtolerance)
  - [`MERCPROFILES/PROFILE/bDeathRate`](#mercprofilesprofilebdeathrate)
  - [`MERCPROFILES/PROFILE/bLifeMax`](#mercprofilesprofileblifemax)
  - [`MERCPROFILES/PROFILE/bLife`](#mercprofilesprofileblife)
  - [`MERCPROFILES/PROFILE/bStrength`](#mercprofilesprofilebstrength)
  - [`MERCPROFILES/PROFILE/bAgility`](#mercprofilesprofilebagility)
  - [`MERCPROFILES/PROFILE/bDexterity`](#mercprofilesprofilebdexterity)
  - [`MERCPROFILES/PROFILE/bWisdom`](#mercprofilesprofilebwisdom)
  - [`MERCPROFILES/PROFILE/bMarksmanship`](#mercprofilesprofilebmarksmanship)
  - [`MERCPROFILES/PROFILE/bExplosive`](#mercprofilesprofilebexplosive)
  - [`MERCPROFILES/PROFILE/bLeadership`](#mercprofilesprofilebleadership)
  - [`MERCPROFILES/PROFILE/bMedical`](#mercprofilesprofilebmedical)
  - [`MERCPROFILES/PROFILE/bMechanical`](#mercprofilesprofilebmechanical)
  - [`MERCPROFILES/PROFILE/bExpLevel`](#mercprofilesprofilebexplevel)
  - [`MERCPROFILES/PROFILE/fRegresses`](#mercprofilesprofilefregresses)
  - [`MERCPROFILES/PROFILE/bOldSkillTrait`](#mercprofilesprofileboldskilltrait)
  - [`MERCPROFILES/PROFILE/bOldSkillTrait2`](#mercprofilesprofileboldskilltrait2)
  - [`MERCPROFILES/PROFILE/bNewSkillTrait1`](#mercprofilesprofilebnewskilltrait1)
  - [`MERCPROFILES/PROFILE/bNewSkillTrait2`](#mercprofilesprofilebnewskilltrait2)
  - [`MERCPROFILES/PROFILE/bNewSkillTrait3`](#mercprofilesprofilebnewskilltrait3)
  - [`MERCPROFILES/PROFILE/bNewSkillTrait4`](#mercprofilesprofilebnewskilltrait4)
  - [`MERCPROFILES/PROFILE/bBuddy1`](#mercprofilesprofilebbuddy1)
  - [`MERCPROFILES/PROFILE/bBuddy2`](#mercprofilesprofilebbuddy2)
  - [`MERCPROFILES/PROFILE/bBuddy3`](#mercprofilesprofilebbuddy3)
  - [`MERCPROFILES/PROFILE/bBuddy4`](#mercprofilesprofilebbuddy4)
  - [`MERCPROFILES/PROFILE/bBuddy5`](#mercprofilesprofilebbuddy5)
  - [`MERCPROFILES/PROFILE/bLearnToLike`](#mercprofilesprofileblearntolike)
  - [`MERCPROFILES/PROFILE/bLearnToLikeTime`](#mercprofilesprofileblearntoliketime)
  - [`MERCPROFILES/PROFILE/bHated1`](#mercprofilesprofilebhated1)
  - [`MERCPROFILES/PROFILE/bHatedTime1`](#mercprofilesprofilebhatedtime1)
  - [`MERCPROFILES/PROFILE/bHated2`](#mercprofilesprofilebhated2)
  - [`MERCPROFILES/PROFILE/bHatedTime2`](#mercprofilesprofilebhatedtime2)
  - [`MERCPROFILES/PROFILE/bHated3`](#mercprofilesprofilebhated3)
  - [`MERCPROFILES/PROFILE/bHatedTime3`](#mercprofilesprofilebhatedtime3)
  - [`MERCPROFILES/PROFILE/bHated4`](#mercprofilesprofilebhated4)
  - [`MERCPROFILES/PROFILE/bHatedTime4`](#mercprofilesprofilebhatedtime4)
  - [`MERCPROFILES/PROFILE/bHated5`](#mercprofilesprofilebhated5)
  - [`MERCPROFILES/PROFILE/bHatedTime5`](#mercprofilesprofilebhatedtime5)
  - [`MERCPROFILES/PROFILE/bLearnToHate`](#mercprofilesprofileblearntohate)
  - [`MERCPROFILES/PROFILE/bLearnToHateTime`](#mercprofilesprofileblearntohatetime)
  - [`MERCPROFILES/PROFILE/bRace`](#mercprofilesprofilebrace)
  - [`MERCPROFILES/PROFILE/bNationality`](#mercprofilesprofilebnationality)
  - [`MERCPROFILES/PROFILE/bAppearance`](#mercprofilesprofilebappearance)
  - [`MERCPROFILES/PROFILE/bAppearanceCareLevel`](#mercprofilesprofilebappearancecarelevel)
  - [`MERCPROFILES/PROFILE/bRefinement`](#mercprofilesprofilebrefinement)
  - [`MERCPROFILES/PROFILE/bRefinementCareLevel`](#mercprofilesprofilebrefinementcarelevel)
  - [`MERCPROFILES/PROFILE/bHatedNationality`](#mercprofilesprofilebhatednationality)
  - [`MERCPROFILES/PROFILE/bHatedNationalityCareLevel`](#mercprofilesprofilebhatednationalitycarelevel)
  - [`MERCPROFILES/PROFILE/bRacist`](#mercprofilesprofilebracist)
  - [`MERCPROFILES/PROFILE/bSexist`](#mercprofilesprofilebsexist)
  - [`MERCPROFILES/PROFILE/sSalary`](#mercprofilesprofilessalary)
  - [`MERCPROFILES/PROFILE/uiWeeklySalary`](#mercprofilesprofileuiweeklysalary)
  - [`MERCPROFILES/PROFILE/uiBiWeeklySalary`](#mercprofilesprofileuibiweeklysalary)
  - [`MERCPROFILES/PROFILE/bMedicalDeposit`](#mercprofilesprofilebmedicaldeposit)
  - [`MERCPROFILES/PROFILE/sMedicalDepositAmount`](#mercprofilesprofilesmedicaldepositamount)
  - [`MERCPROFILES/PROFILE/usOptionalGearCost`](#mercprofilesprofileusoptionalgearcost)
  - [`MERCPROFILES/PROFILE/bArmourAttractiveness`](#mercprofilesprofilebarmourattractiveness)
  - [`MERCPROFILES/PROFILE/bMainGunAttractiveness`](#mercprofilesprofilebmaingunattractiveness)
  - [`MERCPROFILES/PROFILE/fGoodGuy`](#mercprofilesprofilefgoodguy)
  - [`MERCPROFILES/PROFILE/usApproachFactorFriendly`](#mercprofilesprofileusapproachfactorfriendly)
  - [`MERCPROFILES/PROFILE/usApproachFactorDirect`](#mercprofilesprofileusapproachfactordirect)
  - [`MERCPROFILES/PROFILE/usApproachFactorThreaten`](#mercprofilesprofileusapproachfactorthreaten)
  - [`MERCPROFILES/PROFILE/usApproachFactorRecruit`](#mercprofilesprofileusapproachfactorrecruit)
  - [`MERCPROFILES/PROFILE/sSectorX`](#mercprofilesprofilessectorx)
  - [`MERCPROFILES/PROFILE/sSectorY`](#mercprofilesprofilessectory)
  - [`MERCPROFILES/PROFILE/sSectorZ`](#mercprofilesprofilessectorz)
  - [`MERCPROFILES/PROFILE/ubCivilianGroup`](#mercprofilesprofileubciviliangroup)
  - [`MERCPROFILES/PROFILE/bTown`](#mercprofilesprofilebtown)
  - [`MERCPROFILES/PROFILE/bTownAttachment`](#mercprofilesprofilebtownattachment)
  - [`MERCPROFILES/PROFILE/usBackground`](#mercprofilesprofileusbackground)
  - [`MERCPROFILES/PROFILE/GrowthModifierLife`](#mercprofilesprofilegrowthmodifierlife)
  - [`MERCPROFILES/PROFILE/GrowthModifierStrength`](#mercprofilesprofilegrowthmodifierstrength)
  - [`MERCPROFILES/PROFILE/GrowthModifierAgility`](#mercprofilesprofilegrowthmodifieragility)
  - [`MERCPROFILES/PROFILE/GrowthModifierDexterity`](#mercprofilesprofilegrowthmodifierdexterity)
  - [`MERCPROFILES/PROFILE/GrowthModifierWisdom`](#mercprofilesprofilegrowthmodifierwisdom)
  - [`MERCPROFILES/PROFILE/GrowthModifierMarksmanship`](#mercprofilesprofilegrowthmodifiermarksmanship)
  - [`MERCPROFILES/PROFILE/GrowthModifierExplosive`](#mercprofilesprofilegrowthmodifierexplosive)
  - [`MERCPROFILES/PROFILE/GrowthModifierLeadership`](#mercprofilesprofilegrowthmodifierleadership)
  - [`MERCPROFILES/PROFILE/GrowthModifierMedical`](#mercprofilesprofilegrowthmodifiermedical)
  - [`MERCPROFILES/PROFILE/GrowthModifierMechanical`](#mercprofilesprofilegrowthmodifiermechanical)
  - [`MERCPROFILES/PROFILE/GrowthModifierExpLevel`](#mercprofilesprofilegrowthmodifierexplevel)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercProfiles.xml` defines the full character sheet of every mercenary, RPC and NPC: stats, skills, traits, appearance, salaries, starting sector, portrait coordinates and relationships.

## Tables

| Table | File | Notes |
|---|---|---|
| `MercProfiles` | `MercProfiles.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
MERCPROFILES
└── PROFILE
    ├── uiIndex
    ├── Type
    ├── zName
    ├── zNickname
    ├── ubFaceIndex
    ├── usVoiceIndex
    ├── usEyesX
    ├── usEyesY
    ├── usMouthX
    ├── usMouthY
    ├── uiEyeDelay
    ├── uiMouthDelay
    ├── uiBlinkFrequency
    ├── uiExpressionFrequency
    ├── PANTS
    ├── VEST
    ├── SKIN
    ├── HAIR
    ├── bSex
    ├── ubBodyType
    ├── uiBodyTypeSubFlags
    ├── bAttitude
    ├── bCharacterTrait
    ├── bDisability
    ├── ubNeedForSleep
    ├── bReputationTolerance
    ├── bDeathRate
    ├── bLifeMax
    ├── bLife
    ├── bStrength
    ├── bAgility
    ├── bDexterity
    ├── bWisdom
    ├── bMarksmanship
    ├── bExplosive
    ├── bLeadership
    ├── bMedical
    ├── bMechanical
    ├── bExpLevel
    ├── fRegresses
    ├── bOldSkillTrait
    ├── bOldSkillTrait2
    ├── bNewSkillTrait1
    ├── bNewSkillTrait2
    ├── bNewSkillTrait3
    ├── bNewSkillTrait4
    ├── bBuddy1
    ├── bBuddy2
    ├── bBuddy3
    ├── bBuddy4
    ├── bBuddy5
    ├── bLearnToLike
    ├── bLearnToLikeTime
    ├── bHated1
    ├── bHatedTime1
    ├── bHated2
    ├── bHatedTime2
    ├── bHated3
    ├── bHatedTime3
    ├── bHated4
    ├── bHatedTime4
    ├── bHated5
    ├── bHatedTime5
    ├── bLearnToHate
    ├── bLearnToHateTime
    ├── bRace
    ├── bNationality
    ├── bAppearance
    ├── bAppearanceCareLevel
    ├── bRefinement
    ├── bRefinementCareLevel
    ├── bHatedNationality
    ├── bHatedNationalityCareLevel
    ├── bRacist
    ├── bSexist
    ├── sSalary
    ├── uiWeeklySalary
    ├── uiBiWeeklySalary
    ├── bMedicalDeposit
    ├── sMedicalDepositAmount
    ├── usOptionalGearCost
    ├── bArmourAttractiveness
    ├── bMainGunAttractiveness
    ├── fGoodGuy
    ├── usApproachFactorFriendly
    ├── usApproachFactorDirect
    ├── usApproachFactorThreaten
    ├── usApproachFactorRecruit
    ├── sSectorX
    ├── sSectorY
    ├── sSectorZ
    ├── ubCivilianGroup
    ├── bTown
    ├── bTownAttachment
    ├── usBackground
    ├── GrowthModifierLife
    ├── GrowthModifierStrength
    ├── GrowthModifierAgility
    ├── GrowthModifierDexterity
    ├── GrowthModifierWisdom
    ├── GrowthModifierMarksmanship
    ├── GrowthModifierExplosive
    ├── GrowthModifierLeadership
    ├── GrowthModifierMedical
    ├── GrowthModifierMechanical
    └── GrowthModifierExpLevel
```

## Fields

### `MERCPROFILES/PROFILE/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Profile identity and index in the profile array.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/Type`

- **Type:** `Integer` (`INT32`)
- **Description:** Profile kind: AIM, MERC, RPC, NPC, vehicle or IMP.
- **Values:** `0` None, `1` AIM, `2` MERC, `3` RPC, `4` NPC, `5` Vehicle, `6` IMP.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/zName`

- **Type:** `String` (`String`)
- **Description:** Full name of the mercenary or NPC.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/zNickname`

- **Type:** `String` (`String`)
- **Description:** Short name used in dialogue and the roster.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/ubFaceIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Face index used by the portrait system.
- **Values:** Face index from the face tables.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usVoiceIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Voice-set index from [IMPVoices](IMPVoices.md).
- **Values:** Voice-set index from [IMPVoices](IMPVoices.md).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usEyesX`

- **Type:** `Integer` (`INT32`)
- **Description:** X coordinate of the eyes on the portrait.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usEyesY`

- **Type:** `Integer` (`INT32`)
- **Description:** Y coordinate of the eyes on the portrait.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usMouthX`

- **Type:** `Integer` (`INT32`)
- **Description:** X coordinate of the mouth on the portrait.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usMouthY`

- **Type:** `Integer` (`INT32`)
- **Description:** Y coordinate of the mouth on the portrait.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/uiEyeDelay`

- **Type:** `Integer` (`INT32`)
- **Description:** Eye animation delay.
- **Numeric semantics:** Higher values mean a longer animation delay (slower); negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/uiMouthDelay`

- **Type:** `Integer` (`INT32`)
- **Description:** Mouth animation delay.
- **Numeric semantics:** Higher values mean a longer animation delay (slower); negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/uiBlinkFrequency`

- **Type:** `Integer` (`INT32`)
- **Description:** Blink frequency used by the portrait animation.
- **Numeric semantics:** Higher values mean a more frequent animation; negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/uiExpressionFrequency`

- **Type:** `Integer` (`INT32`)
- **Description:** Expression frequency used by the portrait animation.
- **Numeric semantics:** Higher values mean a more frequent animation; negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/PANTS`

- **Type:** `String` (`String`)
- **Description:** Pants palette key.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/VEST`

- **Type:** `String` (`String`)
- **Description:** Vest palette key.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/SKIN`

- **Type:** `String` (`String`)
- **Description:** Skin palette key.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/HAIR`

- **Type:** `String` (`String`)
- **Description:** Hair palette key.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bSex`

- **Type:** `Integer` (`INT32`)
- **Description:** Sex of the profile.
- **Values:** `0` male, `1` female.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/ubBodyType`

- **Type:** `Integer` (`INT32`)
- **Description:** Body type used by the soldier animation and portrait system.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/uiBodyTypeSubFlags`

- **Type:** `Integer` (`INT32`)
- **Description:** Sub-flags for the selected body type.
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bAttitude`

- **Type:** `Integer` (`INT32`)
- **Description:** Original attitude used by dialogue and AI.
- **Values:** `0` Normal, `1` Friendly, `2` Loner, `3` Optimist, `4` Pessimist, `5` Aggressive, `6` Arrogant, `7` Big Shot, `8` Asshole, `9` Coward.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bCharacterTrait`

- **Type:** `Integer` (`INT32`)
- **Description:** STOMP character trait.
- **Values:** STOMP character trait: `0` Neutral, `1` Sociable, `2` Loner, `3` Optimist, `4` Assertive, `5` Intellectual, `6` Primitive, `7` Aggressive, `8` Phlegmatic, `9` Dauntless, `10` Pacifist, `11` Malicious, `12` Show-off, `13` Coward.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bDisability`

- **Type:** `Integer` (`INT32`)
- **Description:** STOMP disability, or none.
- **Values:** Disability: `0` None, `1` Heat Intolerant, `2` Nervous, `3` Claustrophobic, `4` Non Swimmer, `5` Fear of Insects, `6` Forgetful, `7` Psycho, `8` Deaf, `9` Shortsighted, `10` Hemophiliac, `11` Fear of Heights, `12` Self-Harming.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/ubNeedForSleep`

- **Type:** `Integer` (`INT32`)
- **Description:** Sleep need used by the fatigue system.
- **Numeric semantics:** Higher values mean a greater sleep need (the character tires sooner); lower values mean less need. Negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bReputationTolerance`

- **Type:** `Integer` (`INT32`)
- **Description:** Tolerance for the player’s reputation.
- **Numeric semantics:** Higher values mean greater tolerance for a worse reputation; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bDeathRate`

- **Type:** `Integer` (`INT32`)
- **Description:** Death-rate value used by the profile’s aging/death checks.
- **Numeric semantics:** Higher values mean a higher death chance; negative values are outside the normal range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bLifeMax`

- **Type:** `Integer` (`INT32`)
- **Description:** Maximum life value for the profile.
- **Numeric semantics:** Higher values mean more hit points; negative values are outside the normal life range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bLife`

- **Type:** `Integer` (`INT32`)
- **Description:** Current/base life value at profile creation.
- **Numeric semantics:** Higher values mean more hit points; negative values are outside the normal life range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bStrength`

- **Type:** `Integer` (`INT32`)
- **Description:** Base Strength stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bAgility`

- **Type:** `Integer` (`INT32`)
- **Description:** Base Agility stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bDexterity`

- **Type:** `Integer` (`INT32`)
- **Description:** Base Dexterity stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bWisdom`

- **Type:** `Integer` (`INT32`)
- **Description:** Base Wisdom stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bMarksmanship`

- **Type:** `Integer` (`INT32`)
- **Description:** Base Marksmanship stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bExplosive`

- **Type:** `Integer` (`INT32`)
- **Description:** Base Explosives stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bLeadership`

- **Type:** `Integer` (`INT32`)
- **Description:** Base Leadership stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bMedical`

- **Type:** `Integer` (`INT32`)
- **Description:** Base Medical stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bMechanical`

- **Type:** `Integer` (`INT32`)
- **Description:** Base Mechanical stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bExpLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting experience level.
- **Numeric semantics:** Higher values mean a more experienced profile; negative values are outside the normal level range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/fRegresses`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Marks the profile as regressing in progression.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bOldSkillTrait`

- **Type:** `Integer` (`INT32`)
- **Description:** First original skill trait.
- **Values:** Original skill ID: `0` None, `1` Lock Picking, `2` Hand-to-Hand, `3` Electronics, `4` Night Ops, `5` Throwing, `6` Teaching, `7` Heavy Weapons, `8` Auto Weapons, `9` Stealthy, `10` Ambidexterous, `11` Thiefing, `12` Martial Arts, `13` Knifing, `14` Rooftop Sniping, `15` Camouflaged.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bOldSkillTrait2`

- **Type:** `Integer` (`INT32`)
- **Description:** Second original skill trait.
- **Values:** Second original skill ID; the same value list as `bOldSkillTrait`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bNewSkillTrait1`

- **Type:** `Integer` (`INT32`)
- **Description:** STOMP skill or trait in slot 1.
- **Values:** STOMP skill/trait ID. Major: `1` Auto Weapons, `2` Heavy Weapons, `3` Marksman, `4` Hunter, `5` Gunslinger, `6` Hand-to-Hand, `7` Deputy, `8` Technician, `9` Paramedic, `20` Covert Ops. Minor: `10` Ambidexterous, `11` Melee, `12` Throwing, `13` Night Ops, `14` Stealthy, `15` Athletics, `16` Bodybuilding, `17` Demolitions, `18` Teaching, `19` Scouting, `21` Radio Operator, `22` Snitch, `23` Survival.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bNewSkillTrait2`

- **Type:** `Integer` (`INT32`)
- **Description:** STOMP skill or trait in slot 2.
- **Values:** STOMP skill/trait ID; the same value list as `bNewSkillTrait1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bNewSkillTrait3`

- **Type:** `Integer` (`INT32`)
- **Description:** STOMP skill or trait in slot 3.
- **Values:** STOMP skill/trait ID; the same value list as `bNewSkillTrait1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bNewSkillTrait4`

- **Type:** `Integer` (`INT32`)
- **Description:** STOMP skill or trait in slot 4.
- **Values:** STOMP skill/trait ID; the same value list as `bNewSkillTrait1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bBuddy1`

- **Type:** `Integer` (`INT32`)
- **Description:** Buddy profile ID in slot 1; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bBuddy2`

- **Type:** `Integer` (`INT32`)
- **Description:** Buddy profile ID in slot 2; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bBuddy3`

- **Type:** `Integer` (`INT32`)
- **Description:** Buddy profile ID in slot 3; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bBuddy4`

- **Type:** `Integer` (`INT32`)
- **Description:** Buddy profile ID in slot 4; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bBuddy5`

- **Type:** `Integer` (`INT32`)
- **Description:** Buddy profile ID in slot 5; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bLearnToLike`

- **Type:** `Integer` (`INT32`)
- **Description:** Profile ID that the character learns to like.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bLearnToLikeTime`

- **Type:** `Integer` (`INT32`)
- **Description:** Time value for learning to like.
- **Numeric semantics:** Time value for the relationship; higher values mean a longer delay, and negative values are not meaningful.
- **Values:** Integer time value; the unit is defined by the relationship system.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHated1`

- **Type:** `Integer` (`INT32`)
- **Description:** Hated profile ID in slot 1; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHatedTime1`

- **Type:** `Integer` (`INT32`)
- **Description:** Time value associated with hate slot 1.
- **Numeric semantics:** Time value for the relationship; higher values mean a longer delay, and negative values are not meaningful.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHated2`

- **Type:** `Integer` (`INT32`)
- **Description:** Hated profile ID in slot 2; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHatedTime2`

- **Type:** `Integer` (`INT32`)
- **Description:** Time value associated with hate slot 2.
- **Numeric semantics:** Time value for the relationship; higher values mean a longer delay, and negative values are not meaningful.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHated3`

- **Type:** `Integer` (`INT32`)
- **Description:** Hated profile ID in slot 3; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHatedTime3`

- **Type:** `Integer` (`INT32`)
- **Description:** Time value associated with hate slot 3.
- **Numeric semantics:** Time value for the relationship; higher values mean a longer delay, and negative values are not meaningful.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHated4`

- **Type:** `Integer` (`INT32`)
- **Description:** Hated profile ID in slot 4; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHatedTime4`

- **Type:** `Integer` (`INT32`)
- **Description:** Time value associated with hate slot 4.
- **Numeric semantics:** Time value for the relationship; higher values mean a longer delay, and negative values are not meaningful.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHated5`

- **Type:** `Integer` (`INT32`)
- **Description:** Hated profile ID in slot 5; `255` means the slot is empty.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHatedTime5`

- **Type:** `Integer` (`INT32`)
- **Description:** Time value associated with hate slot 5.
- **Numeric semantics:** Time value for the relationship; higher values mean a longer delay, and negative values are not meaningful.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bLearnToHate`

- **Type:** `Integer` (`INT32`)
- **Description:** Profile ID that the character learns to hate.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bLearnToHateTime`

- **Type:** `Integer` (`INT32`)
- **Description:** Time value for learning to hate.
- **Numeric semantics:** Time value for the relationship; higher values mean a longer delay, and negative values are not meaningful.
- **Values:** Integer time value; the unit is defined by the relationship system.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bRace`

- **Type:** `Integer` (`INT32`)
- **Description:** Race category used by racism checks.
- **Values:** `0` White, `1` Black, `2` Asian, `3` Eskimo, `4` Hispanic.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bNationality`

- **Type:** `Integer` (`INT32`)
- **Description:** Nationality ID used by nationality checks.
- **Values:** Nationality ID from the list documented in the table schema comment; `-1` is only meaningful for `bHatedNationality`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bAppearance`

- **Type:** `Integer` (`INT32`)
- **Description:** Appearance category used by the profile and dialogue.
- **Values:** `0` Average, `1` Ugly, `2` Homely, `3` Attractive, `4` Babe.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bAppearanceCareLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** How much the profile cares about appearance: `0` None, `1` Some, `2` Extreme.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bRefinement`

- **Type:** `Integer` (`INT32`)
- **Description:** Refinement category: average, slob or snob.
- **Values:** `0` Average, `1` Slob, `2` Snob.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bRefinementCareLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** How much the profile cares about refinement: `0` None, `1` Some, `2` Extreme.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHatedNationality`

- **Type:** `Integer` (`INT32`)
- **Description:** Nationality hated by the profile, or `-1`.
- **Values:** Hated nationality ID; `-1` means no hated nation.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bHatedNationalityCareLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** How strongly the profile cares about the hated nationality: `0` None, `1` Some, `2` Extreme.
- **Values:** Profile ID; `255` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bRacist`

- **Type:** `Integer` (`INT32`)
- **Description:** Racism level.
- **Values:** `0` None, `1` Some, `2` Very.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bSexist`

- **Type:** `Integer` (`INT32`)
- **Description:** Sexism level.
- **Values:** `0` Not, `1` Somewhat, `2` Very, `3` Gentleman.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/sSalary`

- **Type:** `Integer` (`INT32`)
- **Description:** Legacy salary value.
- **Numeric semantics:** Amount of money; higher values mean a larger salary/cost, and negative values are not meaningful amounts.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/uiWeeklySalary`

- **Type:** `Integer` (`INT32`)
- **Description:** Weekly salary.
- **Numeric semantics:** Amount of money; higher values mean a larger salary/cost, and negative values are not meaningful amounts.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/uiBiWeeklySalary`

- **Type:** `Integer` (`INT32`)
- **Description:** Bi-weekly salary.
- **Numeric semantics:** Amount of money; higher values mean a larger salary/cost, and negative values are not meaningful amounts.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bMedicalDeposit`

- **Type:** `Integer` (`INT32`)
- **Description:** Medical-deposit setting for the profile.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/sMedicalDepositAmount`

- **Type:** `Integer` (`INT32`)
- **Description:** Medical-deposit amount.
- **Numeric semantics:** Amount of money; higher values mean a larger salary/cost, and negative values are not meaningful amounts.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usOptionalGearCost`

- **Type:** `Integer` (`INT32`)
- **Description:** Cost of the optional starting gear.
- **Numeric semantics:** Amount of money; higher values mean a larger salary/cost, and negative values are not meaningful amounts.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bArmourAttractiveness`

- **Type:** `Integer` (`INT32`)
- **Description:** How attractive armour is to the profile when it chooses equipment.
- **Numeric semantics:** Higher values make the profile more likely to choose that equipment; lower values make it less likely, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bMainGunAttractiveness`

- **Type:** `Integer` (`INT32`)
- **Description:** How attractive the main gun is to the profile when it chooses equipment.
- **Numeric semantics:** Higher values make the profile more likely to choose that equipment; lower values make it less likely, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/fGoodGuy`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Marks the profile as a good guy for AI and targeting.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usApproachFactorFriendly`

- **Type:** `Integer` (`INT32`)
- **Description:** Approach factor used for friendly dialogue and recruitment checks.
- **Numeric semantics:** Percentage-style factor: `100` is the neutral baseline, higher values improve the approach and lower/negative values penalize it.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usApproachFactorDirect`

- **Type:** `Integer` (`INT32`)
- **Description:** Approach factor used for direct dialogue and recruitment checks.
- **Numeric semantics:** Percentage-style factor: `100` is the neutral baseline, higher values improve the approach and lower/negative values penalize it.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usApproachFactorThreaten`

- **Type:** `Integer` (`INT32`)
- **Description:** Approach factor used for threatening dialogue.
- **Numeric semantics:** Percentage-style factor: `100` is the neutral baseline, higher values improve the approach and lower/negative values penalize it.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usApproachFactorRecruit`

- **Type:** `Integer` (`INT32`)
- **Description:** Approach factor used for recruitment dialogue.
- **Numeric semantics:** Percentage-style factor: `100` is the neutral baseline, higher values improve the approach and lower/negative values penalize it.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/sSectorX`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting sector X coordinate.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/sSectorY`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting sector Y coordinate.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/sSectorZ`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting sector Z coordinate.
- **Numeric semantics:** Higher values mean a deeper underground level; negative values are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/ubCivilianGroup`

- **Type:** `Integer` (`INT32`)
- **Description:** Civilian group ID used by civilian AI.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bTown`

- **Type:** `Integer` (`INT32`)
- **Description:** Town associated with the profile.
- **Values:** Town ID used by the profile.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/bTownAttachment`

- **Type:** `Integer` (`INT32`)
- **Description:** Attachment value for the profile’s associated town.
- **Numeric semantics:** Signed attachment value: higher values mean a stronger bond to the town, and negative values are not meaningful.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/usBackground`

- **Type:** `Integer` (`INT32`)
- **Description:** Background row ID that selects the character background.
- **Values:** Background row ID.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierLife`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Life during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierStrength`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Strength during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierAgility`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Agility during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierDexterity`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Dexterity during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierWisdom`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Wisdom during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierMarksmanship`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Marksmanship during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierExplosive`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Explosive during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierLeadership`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Leadership during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierMedical`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Medical during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierMechanical`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for Mechanical during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

### `MERCPROFILES/PROFILE/GrowthModifierExpLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Growth modifier for ExpLevel during character progression.
- **Numeric semantics:** Negative values make growth faster, `0` is neutral and positive values make growth slower; no other negative meaning is defined.
- **Values:** Integer growth modifier.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merc profile table data; item/profile metadata.
- **Confidence:** `inferred`

## Relations

- `uiIndex` is referenced by `Inventory.MercStartingGear.mIndex`, `MercOpinions.uiIndex`, `MercQuote.uiIndex`, `MercAvailability.ProfilId` and `AIMAvailability.ProfilId`.
- `usBackground` points to [Backgrounds](Backgrounds.md); `ubCivilianGroup` points to [CivGroupNames](CivGroupNames.md).
- `ubFaceIndex` points to [RPCFacesSmall](RPCFacesSmall.md); `usVoiceIndex` points to [IMPVoices](IMPVoices.md).
- `bTown` and town-related fields use the town indexes in [Cities](Map/Cities.md).

## Localization

`zName`, `zNickname` and the palette keys are display/presentation data. Numeric stats, traits and IDs are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Merc profile table data.
- MercProfiles schema comments.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The full nationality list is maintained in the table schema comment; it is not repeated here. The exact effect of several attractiveness and time fields depends on the profile/AI code.
