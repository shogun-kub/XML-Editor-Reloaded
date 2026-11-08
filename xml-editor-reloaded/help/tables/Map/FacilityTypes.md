---
schema: 1
lang: en
family: map-facility-types
title: "FacilityTypes - facilities, assignments and production lines"
tables:
  - FacilityTypes
table_ids:
  - Map.FacilityTypes
files:
  - Map/FacilityTypes.xml
root: FACILITYTYPES
record: FACILITYTYPE
key: ubIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
  - Game code (facility type loader and facility runtime)
status: reviewed
updated: 2026-11-08
---

# FacilityTypes - facilities, assignments and production lines

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`FACILITYTYPES/FACILITYTYPE/ubIndex`](#facilitytypesfacilitytypeubindex)
  - [`FACILITYTYPES/FACILITYTYPE/szFacilityName`](#facilitytypesfacilitytypeszfacilityname)
  - [`FACILITYTYPES/FACILITYTYPE/szFacilityShortName`](#facilitytypesfacilitytypeszfacilityshortname)
  - [`FACILITYTYPES/FACILITYTYPE/ubTotalStaffLimit`](#facilitytypesfacilitytypeubtotalstafflimit)
  - [`FACILITYTYPES/FACILITYTYPE/ubMilitiaTrainersAllowed`](#facilitytypesfacilitytypeubmilitiatrainersallowed)
  - [`FACILITYTYPES/FACILITYTYPE/pmcentrypoint`](#facilitytypesfacilitytypepmcentrypoint)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubAssignmentType`](#facilitytypesfacilitytypeassignmentubassignmenttype)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/szTooltipText`](#facilitytypesfacilitytypeassignmentsztooltiptext)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubStaffLimit`](#facilitytypesfacilitytypeassignmentubstafflimit)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPerformance`](#facilitytypesfacilitytypeassignmentusperformance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usKitDegrade`](#facilitytypesfacilitytypeassignmentuskitdegrade)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCostPerHour`](#facilitytypesfacilitytypeassignmentscostperhour)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usSleep`](#facilitytypesfacilitytypeassignmentussleep)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usFatigue`](#facilitytypesfacilitytypeassignmentusfatigue)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPrisonBaseLimit`](#facilitytypesfacilitytypeassignmentusprisonbaselimit)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCantinaFoodModifier`](#facilitytypesfacilitytypeassignmentscantinafoodmodifier)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fCountEnemiesInWild`](#facilitytypesfacilitytypeassignmentfcountenemiesinwild)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fDetectEnemiesDynamic`](#facilitytypesfacilitytypeassignmentfdetectenemiesdynamic)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubDetectEnemiesImmediate`](#facilitytypesfacilitytypeassignmentubdetectenemiesimmediate)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS`](#facilitytypesfacilitytypeassignmentconditions)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumAgility`](#facilitytypesfacilitytypeassignmentconditionsubminimumagility)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumDexterity`](#facilitytypesfacilitytypeassignmentconditionsubminimumdexterity)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumExplosives`](#facilitytypesfacilitytypeassignmentconditionsubminimumexplosives)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumHealth`](#facilitytypesfacilitytypeassignmentconditionsubminimumhealth)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLeadership`](#facilitytypesfacilitytypeassignmentconditionsubminimumleadership)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLevel`](#facilitytypesfacilitytypeassignmentconditionsubminimumlevel)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLoyaltyHere`](#facilitytypesfacilitytypeassignmentconditionsubminimumloyaltyhere)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMechanical`](#facilitytypesfacilitytypeassignmentconditionsubminimummechanical)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMedical`](#facilitytypesfacilitytypeassignmentconditionsubminimummedical)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumStrength`](#facilitytypesfacilitytypeassignmentconditionsubminimumstrength)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumWisdom`](#facilitytypesfacilitytypeassignmentconditionsubminimumwisdom)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/usPerformance`](#facilitytypesfacilitytypeassignmentconditionsusperformance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubChance`](#facilitytypesfacilitytypeassignmentmoraleubchance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/usChance`](#facilitytypesfacilitytypeassignmentmoraleuschance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/bBaseEffect`](#facilitytypesfacilitytypeassignmentmoralebbaseeffect)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubRange`](#facilitytypesfacilitytypeassignmentmoraleubrange)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubChance`](#facilitytypesfacilitytypeassignmentloyalty_localubchance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/bBaseEffect`](#facilitytypesfacilitytypeassignmentloyalty_localbbaseeffect)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubRange`](#facilitytypesfacilitytypeassignmentloyalty_localubrange)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubChance`](#facilitytypesfacilitytypeassignmentdrunkubchance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/bBaseEffect`](#facilitytypesfacilitytypeassignmentdrunkbbaseeffect)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubRange`](#facilitytypesfacilitytypeassignmentdrunkubrange)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/drugitems/drugitem`](#facilitytypesfacilitytypeassignmentdrunkdrugitemsdrugitem)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/bBaseEffect`](#facilitytypesfacilitytypeassignmentinjurybbaseeffect)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubChance`](#facilitytypesfacilitytypeassignmentinjuryubchance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubRange`](#facilitytypesfacilitytypeassignmentinjuryubrange)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/requires_staff`](#facilitytypesfacilitytypeproductionlinerequires_staff)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sGridNo_Creation`](#facilitytypesfacilitytypeproductionlinesgridno_creation)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sHourlyCost`](#facilitytypesfacilitytypeproductionlineshourlycost)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sMinutesRequired`](#facilitytypesfacilitytypeproductionlinesminutesrequired)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szAdditionalRequirementTips`](#facilitytypesfacilitytypeproductionlineszadditionalrequirementtips)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szProductionName`](#facilitytypesfacilitytypeproductionlineszproductionname)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usItemToCreate`](#facilitytypesfacilitytypeproductionlineusitemtocreate)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_ItemUsedUp`](#facilitytypesfacilitytypeproductionlineusoptional_itemusedup)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_LoyaltyRequired`](#facilitytypesfacilitytypeproductionlineusoptional_loyaltyrequired)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`FacilityTypes.xml` defines facility types and the assignments that can be performed there. A facility has a name, staff limits and optional PMC entry point; each assignment has a type, tooltip, staff limit, performance, cost, fatigue/sleep values and optional morale, loyalty, drunkenness and injury effects. Facilities can also define production lines that turn inputs into items at an hourly cost and loyalty requirement.

Risk chances are expressed against the INI setting `FACILITY_EVENT_RARITY` (default `1000`, so a stored `250` is about 25% per hour). The loader stores every `ubChance` through a one-byte cast, so written values are taken modulo 256: `500` becomes `244`, not 50%. The shipped `usChance` element is not on the loader's element list at all, and `usPerformance` nested inside `CONDITIONS` is skipped for the same reason.

## Tables

| Table | File | Notes |
|---|---|---|
| `FacilityTypes` | `Map/FacilityTypes.xml` | — |

## Structure

```
FACILITYTYPES
└── FACILITYTYPE
    ├── ubIndex
    ├── szFacilityName
    ├── szFacilityShortName
    ├── ubTotalStaffLimit
    ├── ubMilitiaTrainersAllowed
    ├── pmcentrypoint
    ├── ASSIGNMENT
    │   ├── ubAssignmentType
    │   ├── szTooltipText
    │   ├── ubStaffLimit
    │   ├── usPerformance
    │   ├── usKitDegrade
    │   ├── sCostPerHour
    │   ├── usSleep
    │   ├── usFatigue
    │   ├── usPrisonBaseLimit
    │   ├── sCantinaFoodModifier
    │   ├── fCountEnemiesInWild
    │   ├── fDetectEnemiesDynamic
    │   ├── ubDetectEnemiesImmediate
    │   ├── CONDITIONS
    │   │   ├── ubMinimumAgility
    │   │   ├── ubMinimumDexterity
    │   │   ├── ubMinimumExplosives
    │   │   ├── ubMinimumHealth
    │   │   ├── ubMinimumLeadership
    │   │   ├── ubMinimumLevel
    │   │   ├── ubMinimumLoyaltyHere
    │   │   ├── ubMinimumMechanical
    │   │   ├── ubMinimumMedical
    │   │   ├── ubMinimumStrength
    │   │   ├── ubMinimumWisdom
    │   │   └── usPerformance
    │   ├── MORALE
    │   │   ├── ubChance
    │   │   ├── usChance
    │   │   ├── bBaseEffect
    │   │   └── ubRange
    │   ├── LOYALTY_LOCAL
    │   │   ├── ubChance
    │   │   ├── bBaseEffect
    │   │   └── ubRange
    │   ├── DRUNK
    │   │   ├── ubChance
    │   │   ├── bBaseEffect
    │   │   ├── ubRange
    │   │   └── drugitems
    │   │       └── drugitem
    │   └── INJURY
    │       ├── bBaseEffect
    │       ├── ubChance
    │       └── ubRange
    └── PRODUCTIONLINE
        ├── requires_staff
        ├── sGridNo_Creation
        ├── sHourlyCost
        ├── sMinutesRequired
        ├── szAdditionalRequirementTips
        ├── szProductionName
        ├── usItemToCreate
        ├── usOptional_ItemUsedUp
        └── usOptional_LoyaltyRequired
```

## Fields

### `FACILITYTYPES/FACILITYTYPE/ubIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Index of the facility type. It is an array key and is referenced by placed facilities; the loader parses a record but only stores it when the index is at least `1`.
- **Values:** `1..254`; the facility array holds `MAX_NUM_FACILITY_TYPES` (`255`) entries and index `0` is not stored.
- **Default:** Required; identifies the record or lane.
- **Source:** game code (facility type loader); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/szFacilityName`

- **Type:** `String` (`CHAR16`)
- **Description:** Full display name of the facility, such as `Hospital` or `Munitions Factory`. It is converted from UTF-8 into a 30-character buffer, so longer text is truncated.
- **Values:** Free text.
- **Default:** Empty string when the element is omitted.
- **Source:** game code (facility type loader); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/szFacilityShortName`

- **Type:** `String` (`CHAR16`)
- **Description:** Short display name for the facility. It is converted into a 15-character buffer and is used in the strategic merc panel's assignment line and in encyclopedia buttons, where it is truncated to 7 characters.
- **Values:** Free text.
- **Default:** Empty string when the element is omitted.
- **Source:** game code (facility type loader; strategic screen and encyclopedia); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ubTotalStaffLimit`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of mercenaries that can operate the facility at the same time across all its assignments. `0` means nobody can work there.
- **Numeric semantics:** Higher values allow more staff; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; free-slot counting); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ubMilitiaTrainersAllowed`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of militia trainers the facility can host simultaneously; it is added to the sector's militia-trainer allowance.
- **Numeric semantics:** Higher values allow more trainers; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; militia training allowance); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/pmcentrypoint`

- **Type:** `Boolean`
- **Description:** Flag element. Any non-zero value sets the facility's PMC entry-point flag, and sectors containing such a facility are offered as PMC landing sites; the number itself is not stored.
- **Values:** `0` or `1`; any non-zero value sets the flag.
- **Default:** `0` (flag clear) when omitted.
- **Source:** game code (facility type loader; PMC landing-site list); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubAssignmentType`

- **Type:** `String`
- **Description:** Assignment type code. It is a closed, case-sensitive enum rather than free text: the loader maps the exact string to the assignment index and aborts loading with an error for an unknown code.
- **Values:** `AMBIENT`, `STAFF`, `FACILITY_EAT`, `REST`, `DOCTOR`, `PATIENT`, `REPAIR_ITEMS`, `REPAIR_VEHICLE`, `REPAIR_ROBOT`, `PRACTICE_STRENGTH`, `PRACTICE_DEXTERITY`, `PRACTICE_AGILITY`, `PRACTICE_HEALTH`, `PRACTICE_MARKSMANSHIP`, `PRACTICE_LEADERSHIP`, `PRACTICE_MEDICAL`, `PRACTICE_MECHANICAL`, `PRACTICE_EXPLOSIVES`, `TRAINER_STRENGTH`, `TRAINER_DEXTERITY`, `TRAINER_AGILITY`, `TRAINER_HEALTH`, `TRAINER_MARKSMANSHIP`, `TRAINER_LEADERSHIP`, `TRAINER_MECHANICAL`, `TRAINER_MEDICAL`, `TRAINER_EXPLOSIVES`, `STUDENT_STRENGTH`, `STUDENT_DEXTERITY`, `STUDENT_AGILITY`, `STUDENT_HEALTH`, `STUDENT_MARKSMANSHIP`, `STUDENT_LEADERSHIP`, `STUDENT_MECHANICAL`, `STUDENT_MEDICAL`, `STUDENT_EXPLOSIVES`, `INTERROGATE_PRISONERS`, `PRISON_SNITCH`, `SPREAD_PROPAGANDA`, `SPREAD_PROPAGANDA_GLOBAL`, `GATHER_RUMOURS`, `STRATEGIC_MILITIA_MOVEMENT`.
- **Default:** Required; an `ASSIGNMENT` without this element aborts loading.
- **Source:** game code (facility type loader assignment-type mapping); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/szTooltipText`

- **Type:** `String` (`CHAR16`)
- **Description:** Tooltip shown for the assignment in the facility UI. It is display text, not a rule value, and is converted into a 300-character buffer.
- **Values:** Free text.
- **Default:** Empty string when the element is omitted.
- **Source:** game code (facility type loader); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubStaffLimit`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of mercenaries assigned to this particular assignment. The loader requires a non-zero value for every assignment except `AMBIENT` (language overlays are exempt) and aborts loading when it is missing or `0`.
- **Numeric semantics:** Higher values allow more workers on the assignment; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted, which is invalid for non-`AMBIENT` assignments.
- **Source:** game code (facility type loader; assignment slot counting); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPerformance`

- **Type:** `Integer` (`UINT16`)
- **Description:** Percentage effectiveness of the assignment. The assignment value and the facility's `AMBIENT` (index `0`) value combine additively as offsets from `100`.
- **Numeric semantics:** `100` = normal, `200` = twice normal; higher values are more effective, and negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `100` when omitted.
- **Source:** game code (facility type loader; facility performance modifier); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usKitDegrade`

- **Type:** `Integer` (`UINT16`)
- **Description:** Percentage speed at which the assignment wears out its medical or repair kit.
- **Numeric semantics:** `100` = normal wear, higher = faster wear; negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `100` when omitted.
- **Source:** game code (facility type loader; kit-degradation modifier); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCostPerHour`

- **Type:** `Integer` (`INT16`)
- **Description:** Money charged per hour while a mercenary performs the assignment. Negative values are a flat profit.
- **Numeric semantics:** Higher values are more expensive; negative values pay the player instead.
- **Values:** `-32768..32767`; the loader stores this field as `INT16`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; facility running costs); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usSleep`

- **Type:** `Integer` (`UINT16`)
- **Description:** Percentage effectiveness of sleeping while on this assignment.
- **Numeric semantics:** `100` = normal rest, higher = better rest; negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `100` when omitted.
- **Source:** game code (facility type loader; sleep modifier); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usFatigue`

- **Type:** `Integer` (`UINT16`)
- **Description:** Percentage speed at which the mercenary loses maximum breath while on the assignment.
- **Numeric semantics:** `100` = normal fatigue, higher = faster breath loss; negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `100` when omitted.
- **Source:** game code (facility type loader; fatigue modifier); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPrisonBaseLimit`

- **Type:** `Integer` (`UINT16`)
- **Description:** Baseline prisoner capacity for a prison assignment. A non-zero value is also how the game recognises a facility as a prison.
- **Numeric semantics:** Higher values mean more prisoners; negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; prison capacity and prison detection); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCantinaFoodModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** Food points added when a mercenary eats at a facility with a `FACILITY_EAT` assignment; the drink amount is three times this value. `0` or less disables facility eating.
- **Numeric semantics:** Higher values give more food and drink; non-positive values disable the effect rather than reducing it.
- **Values:** `-32768..32767`; the loader stores this field as `INT16`.
- **Default:** `100` when omitted.
- **Source:** game code (facility type loader; facility food handling); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fCountEnemiesInWild`

- **Type:** `Boolean`
- **Description:** If set, the assignment enables counting enemy groups in wilderness sectors on the strategic map.
- **Values:** `0` or `1`.
- **Default:** `0` (false) when omitted.
- **Source:** game code (facility type loader; strategic detection flags); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fDetectEnemiesDynamic`

- **Type:** `Boolean`
- **Description:** If set, the assignment enables detection of enemies in Mobile Militia areas.
- **Values:** `0` or `1`.
- **Default:** `0` (false) when omitted.
- **Source:** game code (facility type loader; strategic detection flags); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubDetectEnemiesImmediate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Immediate detection radius in sectors. Every sector within that distance of the facility is marked as detected while the assignment is staffed. The loader casts through `BOOLEAN` (a byte type), so values above `1` are preserved.
- **Numeric semantics:** Higher values mean a wider detection radius; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; sector detection loop); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS`

- **Type:** `Container`
- **Description:** Optional container for the assignment's minimum-stat requirements. An empty `CONDITIONS` element means the assignment has no skill requirement; the container itself has no value.
- **Values:** Empty, or one or more minimum-stat children.
- **Default:** Absent; the assignment has no requirements.
- **Source:** game code (facility type loader parse stage); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumAgility`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum Agility a mercenary needs to take this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumDexterity`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum Dexterity required for this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumExplosives`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum Explosives skill required for this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumHealth`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum Health required for this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLeadership`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum Leadership required for this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLevel`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum experience level required for this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLoyaltyHere`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum loyalty in the town where the facility is placed. It only has an effect in town sectors.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMechanical`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum Mechanical skill required for this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMedical`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum Medical skill required for this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumStrength`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum Strength required for this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumWisdom`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum Wisdom required for this assignment.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/usPerformance`

- **Type:** `Integer` (`UINT16`)
- **Description:** Performance value nested inside `CONDITIONS`. The loader's `CONDITIONS` element list does not include `usPerformance`, so the element is skipped and has no effect; put `usPerformance` directly under `ASSIGNMENT` instead.
- **Numeric semantics:** Higher values would mean a more effective assignment, but this position is not read.
- **Values:** `0..65535`; the loader would store it as `UINT16` if it were read.
- **Default:** No effect when omitted.
- **Source:** game code (facility type loader `CONDITIONS` parse stage); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance per hour that the morale risk triggers, expressed against the INI setting `FACILITY_EVENT_RARITY` (default `1000`). The loader stores the value in one byte, so a written value is taken modulo 256: `500` is stored as `244`, and only values up to `255` behave as their face value.
- **Numeric semantics:** Higher values are more likely; `0` disables the effect, and negative values wrap into the large end of the byte range.
- **Values:** Intended `0..1000`; effective stored range `0..255` because of the loader cast.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader `(UINT8)` cast; facility risk roll); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/usChance`

- **Type:** `Integer`
- **Description:** Alternative chance element present in shipped `FacilityTypes.xml`, but the loader has no `usChance` element case and no `usChance` assignment. It is not read and has no gameplay effect.
- **Numeric semantics:** Intended chance units, but no loader path applies this element.
- **Values:** `0..1000` in the shipped data; ignored by the current loader.
- **Default:** Ignored either way.
- **Source:** game code (facility type loader reads only `ubChance`); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/bBaseEffect`

- **Type:** `Integer` (`INT8`)
- **Description:** Base morale change when the morale event fires. It is added to the mercenary's morale (clamped to `0..100`) and to the strategic morale modifier (clamped to `-50..50`).
- **Numeric semantics:** Positive values raise morale, negative values lower it; `0` lets the result take either sign.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Random deviation added around the base morale effect.
- **Numeric semantics:** Higher values mean more variation; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance per hour that the local-loyalty risk triggers, expressed against the INI setting `FACILITY_EVENT_RARITY` (default `1000`). As with every `ubChance`, the loader takes the written value modulo 256.
- **Numeric semantics:** Higher values are more likely; `0` disables the effect, and negative values wrap into the large end of the byte range.
- **Values:** Intended `0..1000`; effective stored range `0..255` because of the loader cast.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader `(UINT8)` cast; facility risk roll); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/bBaseEffect`

- **Type:** `Integer` (`INT8`)
- **Description:** Base local-loyalty change when the event fires. The game multiplies the value by the loyalty-points-per-percent constant before applying it.
- **Numeric semantics:** Positive values raise loyalty, negative values lower it; `0` lets the result take either sign.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Random deviation added around the base local-loyalty effect.
- **Numeric semantics:** Higher values mean more variation; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance per hour that a mercenary gets drunk on this assignment, expressed against the INI setting `FACILITY_EVENT_RARITY` (default `1000`). As with every `ubChance`, the loader takes the written value modulo 256.
- **Numeric semantics:** Higher values are more likely; `0` disables the effect, and negative values wrap into the large end of the byte range.
- **Values:** Intended `0..1000`; effective stored range `0..255` because of the loader cast.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader `(UINT8)` cast; facility risk roll); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/bBaseEffect`

- **Type:** `Integer` (`INT8`)
- **Description:** Base result of the drunkenness event. The current code only tests whether the result is non-zero, then applies one of the listed `drugitem` entries, or alcohol when the list is empty.
- **Numeric semantics:** Any non-zero value enables the event; the sign and magnitude do not change the effect.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Random deviation added around the base drunkenness result.
- **Numeric semantics:** Higher values mean more variation; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/drugitems/drugitem`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID of a drug or drink that can be consumed when the drunk event fires. The element may repeat, and the game picks one entry at random, so repeating an ID makes it more likely rather than adding a quantity.
- **Values:** `-32768..32767`; the loader stores each entry as `INT16`.
- **Default:** Empty list; the game then applies alcohol.
- **Source:** game code (facility type loader drug-item vector; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/bBaseEffect`

- **Type:** `Integer` (`INT8`)
- **Description:** Base injury result. Only negative results are applied, as direct damage plus bleeding; a positive value triggers nothing.
- **Numeric semantics:** More negative values cause more damage; `0` and positive values are ignored.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Chance per hour that the injury risk triggers, expressed against the INI setting `FACILITY_EVENT_RARITY` (default `1000`). As with every `ubChance`, the loader takes the written value modulo 256.
- **Numeric semantics:** Higher values are more likely; `0` disables the effect, and negative values wrap into the large end of the byte range.
- **Values:** Intended `0..1000`; effective stored range `0..255` because of the loader cast.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader `(UINT8)` cast; facility risk roll); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Random deviation added around the base injury effect.
- **Numeric semantics:** Higher values mean more variation; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/requires_staff`

- **Type:** `Boolean`
- **Description:** If set, the production line only runs while a mercenary is operating the facility; otherwise it keeps running unattended.
- **Values:** `0` or `1`; any non-zero value sets the flag.
- **Default:** `0` (false) when omitted.
- **Source:** game code (facility type loader `REQUIRES_STAFFING` flag; hourly factory update); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sGridNo_Creation`

- **Type:** `Integer` (`INT32`)
- **Description:** Map grid number where the produced item appears. `-1` is the loader default and is not a real grid number.
- **Values:** `-2147483648..2147483647`; `-1` is the default sentinel.
- **Default:** `-1` when omitted.
- **Source:** game code (facility type loader; hourly factory update item placement); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sHourlyCost`

- **Type:** `Integer` (`INT32`)
- **Description:** Money charged per hour while the production line runs; it is not charged per produced item. Negative values mean the line earns money.
- **Numeric semantics:** Higher values are more expensive; negative values produce income.
- **Values:** `-2147483648..2147483647`; the loader stores this field as `INT32`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; hourly factory update cost); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sMinutesRequired`

- **Type:** `Integer` (`INT32`)
- **Description:** Minutes needed to produce one item. The loader clamps the value to at least `1`.
- **Numeric semantics:** Higher values mean slower production; `0` and negative values become `1`.
- **Values:** `1..2147483647`; the loader applies `max(1, value)`.
- **Default:** `60` when omitted.
- **Source:** game code (facility type loader; hourly factory update); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szAdditionalRequirementTips`

- **Type:** `String` (`CHAR16`)
- **Description:** Extra requirement text shown for the production line, for example `- Requires storage access`. It is display text, not a rule value, and is converted into an 80-character buffer.
- **Values:** Free text.
- **Default:** Empty string when the element is omitted.
- **Source:** game code (facility type loader); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szProductionName`

- **Type:** `String` (`CHAR16`)
- **Description:** Optional display name for the produced item; when empty, the game shows the item's own name. It is converted into a 30-character buffer.
- **Values:** Free text.
- **Default:** Empty string when the element is omitted.
- **Source:** game code (facility type loader; hourly factory update); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usItemToCreate`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced by this line. `0` means no item is set.
- **Values:** `0..65535`; the loader stores this field as `UINT16` and defaults it to `NOTHING` (`0`).
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; item creation); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_ItemUsedUp`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID consumed per produced item. The element repeats once per required unit, and repeating the same ID increases the required quantity for that item.
- **Values:** `0..65535`; each entry is stored as `UINT16`.
- **Default:** Absent; the line consumes nothing.
- **Source:** game code (facility type loader pre-product list; hourly factory update); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_LoyaltyRequired`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum local loyalty needed to run the production line. It is only checked in town sectors; `0` means no requirement.
- **Numeric semantics:** Higher values are stricter; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted.
- **Source:** game code (facility type loader; hourly factory update loyalty check); map table data.
- **Confidence:** `source`

## Relations

- Placed facilities in [Facilities](Facilities.md) reference `FacilityTypes.ubIndex`; each `ASSIGNMENT` selects an assignment type used by the facility UI and the strategic assignment system. Facility placement uses [Cities](Cities.md) data.

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ. Both versions ship `usChance` and `CONDITIONS/usPerformance` elements that the current loader does not read.

## Sources

- Map table data.
- Field names, XML comments and observed values.
- Game code (facility type loader, facility risk roll, hourly factory update, PMC landing list, strategic detection and facility food handling).
- Field types are the types of the runtime `FACILITYTYPE`, `FACILITYASSIGNMENTTYPE` and `PRODUCTION_LINE` fields and of the loader casts.

## Open questions

- The loader stores `ubChance` in one byte, so the intended `0..1000` scale cannot be used for values above `255`; a shipped `500` is meant as 50% on that scale but is actually stored as `244`. The shipped `usChance` value `700` is not read at all. Whether these values were authored for a loader that honoured the full scale is unknown.
- `usChance` has no loader case and no effect; whether it is a leftover of an older schema or a planned alternative is unknown.
- `usPerformance` inside `CONDITIONS` is skipped by the loader; whether the shipped values were expected to apply is unknown.
