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
updated: 2026-11-08
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

- **Type:** `Integer` (`UINT32`)
- **Description:** Profile identity and index in the profile array.
- **Values:** `0..4294967295`; the loader parses this field as `UINT32` and uses it as the array index.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/Type`

- **Type:** `Integer` (`UINT32`)
- **Description:** Profile kind: none, AIM, MERC, RPC, NPC, vehicle or IMP.
- **Values:** `0` None, `1` AIM, `2` MERC, `3` RPC, `4` NPC, `5` Vehicle, `6` IMP.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/zName`

- **Type:** `String` (`CHAR16`)
- **Description:** Full name of the mercenary or NPC.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/zNickname`

- **Type:** `String` (`CHAR16`)
- **Description:** Short name used in dialogue and the roster.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/ubFaceIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the face graphic used for the profile's portrait.
- **Values:** `0..254`; the loader defaults it to the profile's `uiIndex` when profiles are not read from XML.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usVoiceIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Voice-set number used to build the profile's dialogue and speech file names.
- **Values:** `0..254`; defaults to the profile ID when the profile is not read from XML.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usEyesX`

- **Type:** `Integer` (`UINT16`)
- **Description:** X coordinate of the eyes on the portrait.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usEyesY`

- **Type:** `Integer` (`UINT16`)
- **Description:** Y coordinate of the eyes on the portrait.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usMouthX`

- **Type:** `Integer` (`UINT16`)
- **Description:** X coordinate of the mouth on the portrait.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usMouthY`

- **Type:** `Integer` (`UINT16`)
- **Description:** Y coordinate of the mouth on the portrait.
- **Numeric semantics:** Coordinate; larger X/Y values move right/down in the map or portrait, and negative coordinates are outside the valid range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiEyeDelay`

- **Type:** `Integer` (`UINT32`)
- **Description:** Eye animation delay stored in the profile. The current face renderer ignores this value and uses a fixed `50 + Random(30)` ms delay.
- **Numeric semantics:** Milliseconds; higher values would mean a slower eye animation.
- **Values:** Milliseconds; no narrower range is declared.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiMouthDelay`

- **Type:** `Integer` (`UINT32`)
- **Description:** Mouth animation delay stored in the profile. The current face renderer ignores this value and uses a fixed 120 ms delay.
- **Numeric semantics:** Milliseconds; higher values would mean a slower mouth animation.
- **Values:** Milliseconds; no narrower range is declared.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiBlinkFrequency`

- **Type:** `Integer` (`UINT32`)
- **Description:** Interval between blinks in the portrait animation.
- **Numeric semantics:** Milliseconds; higher values mean a longer interval between blinks (less frequent blinking). IMP profiles default to `3000`.
- **Values:** Milliseconds; no narrower range is declared.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiExpressionFrequency`

- **Type:** `Integer` (`UINT32`)
- **Description:** Interval between facial expression changes in the portrait animation.
- **Numeric semantics:** Milliseconds; higher values mean a longer interval between expressions (less frequent changes). IMP profiles default to `2000`.
- **Values:** Milliseconds; no narrower range is declared.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/PANTS`

- **Type:** `String` (`CHAR8`)
- **Description:** Pants palette replacement ID (for example `BLUEPANTS` or `GREENPANTS`).
- **Values:** Palette replacement name, up to 29 characters.
- **Default:** empty string when omitted.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/VEST`

- **Type:** `String` (`CHAR8`)
- **Description:** Vest palette replacement ID (for example `BLUEPANTS` or `GREENPANTS`).
- **Values:** Palette replacement name, up to 29 characters.
- **Default:** empty string when omitted.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/SKIN`

- **Type:** `String` (`CHAR8`)
- **Description:** Skin palette replacement ID (for example `BLUEPANTS` or `GREENPANTS`).
- **Values:** Palette replacement name, up to 29 characters.
- **Default:** empty string when omitted.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/HAIR`

- **Type:** `String` (`CHAR8`)
- **Description:** Hair palette replacement ID (for example `BLUEPANTS` or `GREENPANTS`).
- **Values:** Palette replacement name, up to 29 characters.
- **Default:** empty string when omitted.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bSex`

- **Type:** `Integer` (`INT8`)
- **Description:** Sex of the profile.
- **Values:** `0` Male, `1` Female.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/ubBodyType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Body type used for animations and portraits.
- **Values:** `0` Regular male, `1` Big male, `2` Stocky male, `3` Regular female; other values are creature, civilian and vehicle body types.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiBodyTypeSubFlags`

- **Type:** `Integer` (`UINT32`)
- **Description:** Animation sub-flags for the body type (body-type substitutions).
- **Numeric semantics:** Bitmask or enumerated code; individual bits/choices carry the meaning, so a larger number is not simply “more”. Negative values are not meaningful.
- **Values:** Bitmask; no individual bits are documented in the profile code.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bAttitude`

- **Type:** `Integer` (`INT8`)
- **Description:** Original attitude (personality). Applied only when the new (STOMP) trait system is disabled; with the new system `bCharacterTrait` is used instead.
- **Values:** `0` Normal, `1` Friendly, `2` Loner, `3` Optimist, `4` Pessimist, `5` Aggressive, `6` Arrogant, `7` Big Shot, `8` Asshole, `9` Coward.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bCharacterTrait`

- **Type:** `Integer` (`INT8`)
- **Description:** STOMP character trait. Applied when the new (STOMP) trait system is enabled; otherwise `bAttitude` is used.
- **Values:** STOMP character trait: `0` Neutral, `1` Sociable, `2` Loner, `3` Optimist, `4` Assertive, `5` Intellectual, `6` Primitive, `7` Aggressive, `8` Phlegmatic, `9` Dauntless, `10` Pacifist, `11` Malicious, `12` Show-off, `13` Coward.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bDisability`

- **Type:** `Integer` (`INT8`)
- **Description:** STOMP disability, or none.
- **Values:** Disability: `0` None, `1` Heat Intolerant, `2` Nervous, `3` Claustrophobic, `4` Non Swimmer, `5` Fear of Insects, `6` Forgetful, `7` Psycho, `8` Deaf, `9` Shortsighted, `10` Hemophiliac, `11` Fear of Heights, `12` Self-Harming.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/ubNeedForSleep`

- **Type:** `Integer` (`UINT8`)
- **Description:** Base hours of sleep the profile needs per day.
- **Numeric semantics:** Hours per day; the game clamps the base value to `3..12` before applying injury, background and disease modifiers (final maximum `18`). Higher values make the character tire sooner.
- **Values:** Hours per day, normally `3..12`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bReputationTolerance`

- **Type:** `Integer` (`INT8`)
- **Description:** Tolerance for the player's bad reputation: if the bad-reputation score exceeds this value, the merc refuses or complains. `101` means the merc does not care.
- **Numeric semantics:** Higher values tolerate a worse reputation; `101` disables the check.
- **Values:** `0..100` normally; `101` = ignore.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bDeathRate`

- **Type:** `Integer` (`INT8`)
- **Description:** Tolerance for the calculated death rate: if the calculated rate exceeds this value, the merc refuses or complains. `101` means the merc does not care.
- **Numeric semantics:** Higher values tolerate a higher death rate; `101` disables the check.
- **Values:** `0..100` normally; `101` = ignore.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLifeMax`

- **Type:** `Integer` (`INT8`)
- **Description:** Maximum life (hit points) of the profile.
- **Numeric semantics:** Higher values mean more hit points; negative values are outside the normal life range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLife`

- **Type:** `Integer` (`INT8`)
- **Description:** Initial current life at soldier creation.
- **Numeric semantics:** Higher values mean more hit points; negative values are outside the normal life range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bStrength`

- **Type:** `Integer` (`INT8`)
- **Description:** Base Strength stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bAgility`

- **Type:** `Integer` (`INT8`)
- **Description:** Base Agility stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bDexterity`

- **Type:** `Integer` (`INT8`)
- **Description:** Base Dexterity stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bWisdom`

- **Type:** `Integer` (`INT8`)
- **Description:** Base Wisdom stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMarksmanship`

- **Type:** `Integer` (`INT8`)
- **Description:** Base Marksmanship stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bExplosive`

- **Type:** `Integer` (`INT8`)
- **Description:** Base Explosives stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLeadership`

- **Type:** `Integer` (`INT8`)
- **Description:** Base Leadership stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMedical`

- **Type:** `Integer` (`INT8`)
- **Description:** Base Medical stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMechanical`

- **Type:** `Integer` (`INT8`)
- **Description:** Base Mechanical stat at profile creation.
- **Numeric semantics:** Higher values mean a better stat; negative values are outside the normal stat range and have no defined meaning.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bExpLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** Starting experience level.
- **Numeric semantics:** Higher values mean a more experienced profile; negative values are outside the normal level range.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/fRegresses`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** When `TRUE`, the profile does not evolve: stat-improvement rolls are skipped in `ProcessStatChange`.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bOldSkillTrait`

- **Type:** `Integer` (`INT8`)
- **Description:** First original (pre-STOMP) skill trait. Used only when the new trait system is disabled.
- **Values:** Original skill ID: `0` None, `1` Lock Picking, `2` Hand-to-Hand, `3` Electronics, `4` Night Ops, `5` Throwing, `6` Teaching, `7` Heavy Weapons, `8` Auto Weapons, `9` Stealthy, `10` Ambidexterous, `11` Thiefing, `12` Martial Arts, `13` Knifing, `14` Rooftop Sniping, `15` Camouflaged.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bOldSkillTrait2`

- **Type:** `Integer` (`INT8`)
- **Description:** Second original (pre-STOMP) skill trait.
- **Values:** Second original skill ID; the same value list as `bOldSkillTrait`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNewSkillTrait1`

- **Type:** `Integer` (`INT8`)
- **Description:** STOMP skill or trait in slot 1. Used when the new trait system is enabled.
- **Values:** STOMP skill/trait ID. Major: `1` Auto Weapons, `2` Heavy Weapons, `3` Marksman, `4` Hunter, `5` Gunslinger, `6` Hand-to-Hand, `7` Deputy, `8` Technician, `9` Paramedic, `20` Covert Ops. Minor: `10` Ambidexterous, `11` Melee, `12` Throwing, `13` Night Ops, `14` Stealthy, `15` Athletics, `16` Bodybuilding, `17` Demolitions, `18` Teaching, `19` Scouting, `21` Radio Operator, `22` Snitch, `23` Survival.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNewSkillTrait2`

- **Type:** `Integer` (`INT8`)
- **Description:** STOMP skill or trait in slot 2. Used when the new trait system is enabled.
- **Values:** STOMP skill/trait ID; the same value list as `bNewSkillTrait1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNewSkillTrait3`

- **Type:** `Integer` (`INT8`)
- **Description:** STOMP skill or trait in slot 3. Used when the new trait system is enabled.
- **Values:** STOMP skill/trait ID; the same value list as `bNewSkillTrait1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNewSkillTrait4`

- **Type:** `Integer` (`INT8`)
- **Description:** STOMP skill or trait in slot 4. Used when the new trait system is enabled.
- **Values:** STOMP skill/trait ID; the same value list as `bNewSkillTrait1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy1`

- **Type:** `Integer` (`UINT8`)
- **Description:** Buddy profile ID in slot 1. `255` (written as `-1` in the table) means the slot is empty.
- **Values:** Profile ID; `255`/`-1` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy2`

- **Type:** `Integer` (`UINT8`)
- **Description:** Buddy profile ID in slot 2. `255` (written as `-1` in the table) means the slot is empty.
- **Values:** Profile ID; `255`/`-1` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy3`

- **Type:** `Integer` (`UINT8`)
- **Description:** Buddy profile ID in slot 3. `255` (written as `-1` in the table) means the slot is empty.
- **Values:** Profile ID; `255`/`-1` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy4`

- **Type:** `Integer` (`UINT8`)
- **Description:** Buddy profile ID in slot 4. `255` (written as `-1` in the table) means the slot is empty.
- **Values:** Profile ID; `255`/`-1` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bBuddy5`

- **Type:** `Integer` (`UINT8`)
- **Description:** Buddy profile ID in slot 5. `255` (written as `-1` in the table) means the slot is empty.
- **Values:** Profile ID; `255`/`-1` means empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLearnToLike`

- **Type:** `Integer` (`UINT8`)
- **Description:** Profile ID the character gradually learns to like while serving together; `255` means none.
- **Values:** Profile ID; `255`/`-1` means none.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLearnToLikeTime`

- **Type:** `Integer` (`INT8`)
- **Description:** Initial number of hours needed to learn to like `bLearnToLike` while serving together; counted down hourly.
- **Numeric semantics:** Hours; `0` applies the learned-like state from the start and higher values delay it. Negative values are not meaningful.
- **Values:** Hours, normally `0..100`; `255` (stored as `-1`) is used when `bLearnToLike` is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated1`

- **Type:** `Integer` (`UINT8`)
- **Description:** Hated profile ID in slot 1; `255` means the slot is empty.
- **Values:** Profile ID `0..255`; `255` means empty. The loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime1`

- **Type:** `Integer` (`INT8`)
- **Description:** Duration of the hate relationship in slot 1, counted down hourly.
- **Numeric semantics:** Hours; higher values mean the hate lasts longer. `0` means no tolerance: the complaint/quit branch triggers as soon as the hated profile is on the team. Empty slots also store `0`.
- **Values:** Hours; the loader stores this field as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated2`

- **Type:** `Integer` (`UINT8`)
- **Description:** Hated profile ID in slot 2; `255` means the slot is empty.
- **Values:** Profile ID `0..255`; `255` means empty. The loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime2`

- **Type:** `Integer` (`INT8`)
- **Description:** Duration of the hate relationship in slot 2, counted down hourly.
- **Numeric semantics:** Hours; higher values mean the hate lasts longer. `0` means no tolerance: the complaint/quit branch triggers as soon as the hated profile is on the team. Empty slots also store `0`.
- **Values:** Hours; the loader stores this field as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated3`

- **Type:** `Integer` (`UINT8`)
- **Description:** Hated profile ID in slot 3; `255` means the slot is empty.
- **Values:** Profile ID `0..255`; `255` means empty. The loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime3`

- **Type:** `Integer` (`INT8`)
- **Description:** Duration of the hate relationship in slot 3, counted down hourly.
- **Numeric semantics:** Hours; higher values mean the hate lasts longer. `0` means no tolerance: the complaint/quit branch triggers as soon as the hated profile is on the team. Empty slots also store `0`.
- **Values:** Hours; the loader stores this field as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated4`

- **Type:** `Integer` (`UINT8`)
- **Description:** Hated profile ID in slot 4; `255` means the slot is empty.
- **Values:** Profile ID `0..255`; `255` means empty. The loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime4`

- **Type:** `Integer` (`INT8`)
- **Description:** Duration of the hate relationship in slot 4, counted down hourly.
- **Numeric semantics:** Hours; higher values mean the hate lasts longer. `0` means no tolerance: the complaint/quit branch triggers as soon as the hated profile is on the team. Empty slots also store `0`.
- **Values:** Hours; the loader stores this field as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHated5`

- **Type:** `Integer` (`UINT8`)
- **Description:** Hated profile ID in slot 5; `255` means the slot is empty.
- **Values:** Profile ID `0..255`; `255` means empty. The loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedTime5`

- **Type:** `Integer` (`INT8`)
- **Description:** Duration of the hate relationship in slot 5, counted down hourly.
- **Numeric semantics:** Hours; higher values mean the hate lasts longer. `0` means no tolerance: the complaint/quit branch triggers as soon as the hated profile is on the team. Empty slots also store `0`.
- **Values:** Hours; the loader stores this field as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLearnToHate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Profile ID the character gradually learns to hate while serving together; `255` means none.
- **Values:** Profile ID; `255`/`-1` means none.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bLearnToHateTime`

- **Type:** `Integer` (`INT8`)
- **Description:** Initial number of hours needed to learn to hate `bLearnToHate` while serving together; counted down hourly.
- **Numeric semantics:** Hours; `0` applies the learned-hate state from the start and higher values delay it. Negative values are not meaningful.
- **Values:** Hours; `0..120` in the shipped table.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bRace`

- **Type:** `Integer` (`INT8`)
- **Description:** Race category used by racism checks.
- **Values:** `0` White, `1` Black, `2` Asian, `3` Eskimo, `4` Hispanic.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bNationality`

- **Type:** `Integer` (`INT8`)
- **Description:** Nationality ID used by nationality and prejudice checks.
- **Values:** Nationality ID `0..112` (the list is in the table comment); `-1` is only valid for `bHatedNationality`. Values are clamped to `0..112` on load.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bAppearance`

- **Type:** `Integer` (`INT8`)
- **Description:** Appearance category used by the profile and dialogue.
- **Values:** `0` Average, `1` Ugly, `2` Homely, `3` Attractive, `4` Babe.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bAppearanceCareLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** How much the profile cares about appearance: `0` None, `1` Some, `2` Extreme.
- **Values:** `0` None, `1` Some, `2` Extreme; clamped by the loader.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bRefinement`

- **Type:** `Integer` (`INT8`)
- **Description:** Refinement category: average, slob or snob.
- **Values:** `0` Average, `1` Slob, `2` Snob.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bRefinementCareLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** How much the profile cares about refinement: `0` None, `1` Some, `2` Extreme.
- **Values:** `0` None, `1` Some, `2` Extreme; clamped by the loader.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedNationality`

- **Type:** `Integer` (`INT8`)
- **Description:** Nationality hated by the profile, or `-1` for none.
- **Values:** Nationality ID `0..112`; `-1` means no hated nation.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bHatedNationalityCareLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** How strongly the profile cares about the hated nationality: `0` None, `1` Some, `2` Extreme.
- **Values:** `0` None, `1` Some, `2` Extreme; clamped by the loader.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bRacist`

- **Type:** `Integer` (`INT8`)
- **Description:** Racism level.
- **Values:** `0` None, `1` Some, `2` Very.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bSexist`

- **Type:** `Integer` (`UINT8`)
- **Description:** Sexism level.
- **Values:** `0` Not, `1` Somewhat, `2` Very, `3` Gentleman.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sSalary`

- **Type:** `Integer` (`INT16`)
- **Description:** Daily salary of the mercenary.
- **Numeric semantics:** Money per day; used as the contract charge for daily contracts and as the basis for the insurance premium. Negative values are not meaningful amounts.
- **Values:** Money amount.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiWeeklySalary`

- **Type:** `Integer` (`UINT32`)
- **Description:** Weekly salary.
- **Numeric semantics:** Money per week; the daily equivalent is `uiWeeklySalary / 7`.
- **Values:** Money amount.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/uiBiWeeklySalary`

- **Type:** `Integer` (`UINT32`)
- **Description:** Bi-weekly salary.
- **Numeric semantics:** Money per two weeks; the daily equivalent is `uiBiWeeklySalary / 14`.
- **Values:** Money amount.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMedicalDeposit`

- **Type:** `Integer` (`INT8`)
- **Description:** Whether the merc requires a medical deposit (`0` = no, non-zero = yes).
- **Values:** `0` No deposit, `1` Deposit required.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sMedicalDepositAmount`

- **Type:** `Integer` (`UINT16`)
- **Description:** Medical-deposit amount. When profiles are not read from XML it is calculated from the merc's competence; when XML data is read the table value is used.
- **Numeric semantics:** Money amount; higher values mean a larger deposit.
- **Values:** Money amount, stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usOptionalGearCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** Cost of the optional starting gear kit. It is computed from the selected kit's items and its price fields (`mAbsolutePrice` or `mPriceMod`) and added to the contract charge.
- **Numeric semantics:** Money amount; the XML value is overwritten by the computed kit price at load.
- **Values:** Money amount, stored as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bArmourAttractiveness`

- **Type:** `Integer` (`INT8`)
- **Description:** Baseline armour quality. The merc complains when the best armour's protection (adjusted by morale) falls below half this value; `-1` means the kit has no armour.
- **Numeric semantics:** Armour protection value; higher values expect better armour. `-1` means no armour baseline.
- **Values:** `-1` when no armour is present; otherwise the computed protection value.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bMainGunAttractiveness`

- **Type:** `Integer` (`INT8`)
- **Description:** Baseline main-gun quality. The merc complains when the best gun's deadliness (adjusted by morale) falls below half this value; `-1` means the kit has no gun.
- **Numeric semantics:** Weapon deadliness value; higher values expect a better gun. `-1` means no gun baseline.
- **Values:** `-1` when no gun is present; otherwise the computed weapon-deadliness value.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/fGoodGuy`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Marks the profile as a good guy: sets `PROFILE_MISC_FLAG3_GOODGUY` in `ubMiscFlags3`.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usApproachFactorFriendly`

- **Type:** `Integer` (`UINT16`)
- **Description:** Approach-factor percentage used for friendly dialogue and recruitment checks.
- **Numeric semantics:** Percent: `100` = ×1.0 (normal); the value is divided by 100 and multiplied with the merc's leadership (or threatening effectiveness). Higher values improve the approach, lower values penalize it.
- **Values:** Percent, `0..300` in the shipped table.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usApproachFactorDirect`

- **Type:** `Integer` (`UINT16`)
- **Description:** Approach-factor percentage used for direct dialogue and recruitment checks.
- **Numeric semantics:** Percent: `100` = ×1.0 (normal); the value is divided by 100 and multiplied with the merc's leadership (or threatening effectiveness). Higher values improve the approach, lower values penalize it.
- **Values:** Percent, `0..300` in the shipped table.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usApproachFactorThreaten`

- **Type:** `Integer` (`UINT16`)
- **Description:** Approach-factor percentage used for threatening dialogue.
- **Numeric semantics:** Percent: `100` = ×1.0 (normal); the value is divided by 100 and multiplied with the merc's threatening effectiveness. Higher values improve the approach, lower values penalize it.
- **Values:** Percent, `0..300` in the shipped table.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usApproachFactorRecruit`

- **Type:** `Integer` (`UINT16`)
- **Description:** Approach-factor percentage used for recruitment dialogue.
- **Numeric semantics:** Percent: `100` = ×1.0 (normal); the value is divided by 100 and multiplied with the merc's leadership. Higher values improve the approach, lower values penalize it.
- **Values:** Percent, `0..300` in the shipped table.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sSectorX`

- **Type:** `Integer` (`UINT16`)
- **Description:** Sector X where the NPC or RPC starts. It is reset to `0` for AIM/MERC/IMP profiles when profiles are read from XML.
- **Numeric semantics:** Map X coordinate; negative values are outside the valid range.
- **Values:** Sector X `1..16`; `0` means not placed.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sSectorY`

- **Type:** `Integer` (`UINT16`)
- **Description:** Sector Y where the NPC or RPC starts. It is reset to `0` for AIM/MERC/IMP profiles when profiles are read from XML.
- **Numeric semantics:** Map Y coordinate; negative values are outside the valid range.
- **Values:** Sector Y `1..16` (rows A..P); `0` means not placed.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/sSectorZ`

- **Type:** `Integer` (`INT8`)
- **Description:** Sector Z (underground level) where the NPC or RPC starts. It is reset to `0` for AIM/MERC/IMP profiles when profiles are read from XML.
- **Numeric semantics:** `0` = surface; higher values select deeper underground levels. Negative values are outside the valid range.
- **Values:** `0` or `1` in the shipped table; deeper underground levels use higher values.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/ubCivilianGroup`

- **Type:** `Integer` (`UINT8`)
- **Description:** Civilian group ID used by civilian AI and faction handling.
- **Values:** Group ID; `0` = no civilian group (`NON_CIV_GROUP`).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bTown`

- **Type:** `Integer` (`INT8`)
- **Description:** Home town ID of the profile, used for town-loyalty and NPC willingness. It is reset to `0` for AIM/MERC/IMP profiles when profiles are read from XML.
- **Values:** Town ID; `0` = none.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/bTownAttachment`

- **Type:** `Integer` (`INT8`)
- **Description:** Percentage weight used when a local NPC’s contribution to town loyalty is calculated. `100` is the normal value; lower values reduce the local bonus.
- **Numeric semantics:** Percentage; `100` is normal, values below `100` reduce the town-loyalty contribution, and negative values are not meaningful.
- **Values:** Percentage, usually `0..100`; the loader stores this field as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/usBackground`

- **Type:** `Integer` (`UINT16`)
- **Description:** Row index into the background table (`Backgrounds.xml`).
- **Values:** Background row index; `0` means no background.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader); merc profile table data.
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierLife`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Life during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierStrength`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Strength during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierAgility`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Agility during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierDexterity`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Dexterity during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierWisdom`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Wisdom during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierMarksmanship`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Marksmanship during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierExplosive`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Explosives during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierLeadership`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Leadership during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierMedical`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Medical during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierMechanical`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for Mechanical during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

### `MERCPROFILES/PROFILE/GrowthModifierExpLevel`

- **Type:** `Integer` (`INT16`)
- **Description:** Growth modifier for experience level during character progression. Negative values make growth faster, `0` is neutral and positive values make growth slower. Ignored when `fRegresses` is `TRUE` or growth modifiers are disabled. Note: the loader in this source revision matches `bGrowthModifier*` tags, while the table uses `GrowthModifier*`, so the value may not be applied.
- **Numeric semantics:** Subpoints added to the per-point growth threshold: negative values lower it (faster growth), `0` is neutral, positive values raise it (slower growth). `fRegresses = TRUE` disables growth modifiers.
- **Values:** `-32768..32767`; the shipped table uses `0`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (profile loader; the `bGrowthModifier*` tag comparison was verified against this table tag).
- **Confidence:** `source`

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
- Game code: profile XML loader, profile struct and loader, soldier creation, campaign growth, face renderer, strategic merc handler, strategic status.

## Open questions

The full nationality list is maintained in the table schema comment; it is not repeated here. The growth-modifier tags are spelled `GrowthModifier*` in the table, while the loader in the analyzed source revision matches `bGrowthModifier*`, so whether the shipped build reads these values is unresolved.
