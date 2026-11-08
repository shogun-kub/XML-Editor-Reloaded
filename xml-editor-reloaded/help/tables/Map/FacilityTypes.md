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
  - Facility and assignment terminology from the map UI
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

`FacilityTypes.xml` defines facility types and the assignments that can be performed there. A facility has a name, staff limits and optional PMC entry point; each assignment has a type, tooltip, staff limit, performance, cost, fatigue/sleep values and optional morale, loyalty, drunkenness and injury effects. Facilities can also define production lines that turn inputs into items at an hourly cost and loyalty requirement. Chance fields use a 0–1000 scale in the data (`500` = 50%, `700` = 70%).

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

- **Type:** `Integer`
- **Description:** Index of the facility type. It is an array key and is referenced by placed facilities.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record or lane.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/szFacilityName`

- **Type:** `String`
- **Description:** Full display name of the facility, such as `Hospital` or `Munitions Factory`.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/szFacilityShortName`

- **Type:** `String`
- **Description:** Short display name for the facility.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ubTotalStaffLimit`

- **Type:** `Integer`
- **Description:** Maximum number of mercenaries that can work in the facility across all its assignments.
- **Numeric semantics:** higher = more staff; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ubMilitiaTrainersAllowed`

- **Type:** `Integer`
- **Description:** Number of militia trainers the facility can support.
- **Numeric semantics:** higher = more trainers; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/pmcentrypoint`

- **Type:** `Integer`
- **Description:** Entry-point index for the PMC (private military company) system. `0` means the facility is not a PMC entry point; non-zero values are entry-point identifiers, not distances.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubAssignmentType`

- **Type:** `String`
- **Description:** Assignment type code, such as `DOCTOR`, `REST` or `REPAIR_ITEMS`. It is an enum key; each code selects the assignment logic and UI text.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/szTooltipText`

- **Type:** `String`
- **Description:** Tooltip shown for the assignment in the facility UI. It is display text, not a rule value.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubStaffLimit`

- **Type:** `Integer`
- **Description:** Maximum number of mercenaries assigned to this particular assignment.
- **Numeric semantics:** higher = more workers; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPerformance`

- **Type:** `Integer`
- **Description:** Performance factor of the assignment. `100` is normal speed/efficiency.
- **Numeric semantics:** `100` = normal, `200` = twice normal; higher values are more effective, and negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usKitDegrade`

- **Type:** `Integer`
- **Description:** How quickly the assignment wears out its medical or repair kit.
- **Numeric semantics:** higher = faster wear; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCostPerHour`

- **Type:** `Integer`
- **Description:** Hourly cost of running this assignment, in money.
- **Numeric semantics:** higher = more expensive; negative values are not meaningful.
- **Values:** Money or points per hour, as named.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usSleep`

- **Type:** `Integer`
- **Description:** Sleep or rest quality provided by this assignment.
- **Numeric semantics:** higher = better rest; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usFatigue`

- **Type:** `Integer`
- **Description:** Fatigue factor of the assignment.
- **Numeric semantics:** higher = more fatigue; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPrisonBaseLimit`

- **Type:** `Integer`
- **Description:** Base prisoner capacity for a prison assignment.
- **Numeric semantics:** higher = more capacity; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCantinaFoodModifier`

- **Type:** `Integer`
- **Description:** Relative food and drink modifier for a cantina assignment.
- **Numeric semantics:** higher positive values strengthen the effect, negative values weaken it, and `0` is neutral; the table uses values such as `500`, so this is not a plain percentage.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fCountEnemiesInWild`

- **Type:** `Boolean`
- **Description:** If set, this assignment counts enemy activity in the wilderness around the facility. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fDetectEnemiesDynamic`

- **Type:** `Boolean`
- **Description:** If set, enemy detection from this assignment updates dynamically instead of using a fixed result. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubDetectEnemiesImmediate`

- **Type:** `Integer`
- **Description:** Immediate enemy-detection strength for the assignment. It is a strength/level, not a count of enemies.
- **Numeric semantics:** higher = wider detection; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS`

- **Type:** `Container`
- **Description:** Optional container for the assignment’s skill requirements. An empty `CONDITIONS` element means the assignment has no skill requirement; the container itself has no value.
- **Values:** Empty, or one or more minimum-stat children.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumAgility`

- **Type:** `Integer`
- **Description:** Minimum Agility a mercenary needs for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumDexterity`

- **Type:** `Integer`
- **Description:** Minimum Dexterity required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumExplosives`

- **Type:** `Integer`
- **Description:** Minimum Explosives skill required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumHealth`

- **Type:** `Integer`
- **Description:** Minimum Health required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLeadership`

- **Type:** `Integer`
- **Description:** Minimum Leadership required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLevel`

- **Type:** `Integer`
- **Description:** Minimum experience level required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLoyaltyHere`

- **Type:** `Integer`
- **Description:** Minimum local loyalty required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMechanical`

- **Type:** `Integer`
- **Description:** Minimum Mechanical skill required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMedical`

- **Type:** `Integer`
- **Description:** Minimum Medical skill required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumStrength`

- **Type:** `Integer`
- **Description:** Minimum Strength required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumWisdom`

- **Type:** `Integer`
- **Description:** Minimum Wisdom required for this assignment.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/usPerformance`

- **Type:** `Integer`
- **Description:** Performance factor used when this assignment’s conditions are present.
- **Numeric semantics:** `100` = normal; higher values are more effective, and negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubChance`

- **Type:** `Integer`
- **Description:** Chance that the assignment changes morale, in chance units (the table uses a 0–1000 scale, so `500` = 50% and `700` = 70%).
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage or chance units as described; `0` disables the effect.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/usChance`

- **Type:** `Integer`
- **Description:** Alternative morale-change chance for the assignment.
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage or chance units as described; `0` disables the effect.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/bBaseEffect`

- **Type:** `Integer`
- **Description:** Base morale change when the morale event fires.
- **Numeric semantics:** positive = better morale; negative = worse.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubRange`

- **Type:** `Integer`
- **Description:** Random spread added to the morale effect.
- **Numeric semantics:** higher = more variation; negative values are not meaningful.
- **Values:** Non-negative integer.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubChance`

- **Type:** `Integer`
- **Description:** Chance that the assignment changes local loyalty, in chance units (the table uses a 0–1000 scale, so `40` = 4%).
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage or chance units as described; `0` disables the effect.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/bBaseEffect`

- **Type:** `Integer`
- **Description:** Base local-loyalty change when the event fires.
- **Numeric semantics:** positive = better loyalty; negative = worse.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubRange`

- **Type:** `Integer`
- **Description:** Random spread added to the local-loyalty effect.
- **Numeric semantics:** higher = more variation; negative values are not meaningful.
- **Values:** Non-negative integer.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubChance`

- **Type:** `Integer`
- **Description:** Chance that a mercenary gets drunk on this assignment, in chance units (the table uses a 0–1000 scale, so `500` = 50%).
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage or chance units as described; `0` disables the effect.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/bBaseEffect`

- **Type:** `Integer`
- **Description:** Base drunkenness effect.
- **Numeric semantics:** higher = stronger effect; negative = opposite effect.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubRange`

- **Type:** `Integer`
- **Description:** Random spread added to the drunkenness effect.
- **Numeric semantics:** higher = more variation; negative values are not meaningful.
- **Values:** Non-negative integer.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/drugitems/drugitem`

- **Type:** `Integer`
- **Description:** Item ID of a drug or drink that can be consumed during this assignment. It is an item reference; the element may repeat to offer several items.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/bBaseEffect`

- **Type:** `Integer`
- **Description:** Base injury effect of the assignment.
- **Numeric semantics:** positive = safer; negative = more dangerous.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubChance`

- **Type:** `Integer`
- **Description:** Chance that the assignment causes an injury, in chance units (the table uses a 0–1000 scale, so `50` = 5%).
- **Numeric semantics:** higher = more likely; negative values are not meaningful.
- **Values:** Percentage or chance units as described; `0` disables the effect.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubRange`

- **Type:** `Integer`
- **Description:** Random spread added to the injury effect.
- **Numeric semantics:** higher = more variation; negative values are not meaningful.
- **Values:** Non-negative integer.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/requires_staff`

- **Type:** `Boolean`
- **Description:** If set, the production line only runs while staff are assigned. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sGridNo_Creation`

- **Type:** `Integer`
- **Description:** Map grid number where the produced item appears. It is a grid identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sHourlyCost`

- **Type:** `Integer`
- **Description:** Hourly cost of running this production line, in money.
- **Numeric semantics:** higher = more expensive; negative values are not meaningful.
- **Values:** Money or points per hour, as named.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sMinutesRequired`

- **Type:** `Integer`
- **Description:** Minutes needed to produce one batch.
- **Numeric semantics:** higher = slower; negative values are not meaningful.
- **Values:** Minutes.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szAdditionalRequirementTips`

- **Type:** `String`
- **Description:** Extra requirement text shown for the production line, for example `- Requires storage access`. It is display text, not a rule value.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szProductionName`

- **Type:** `String`
- **Description:** Optional display name for the produced item. When absent, the item’s own name is used.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usItemToCreate`

- **Type:** `Integer`
- **Description:** Item ID produced by this line. It is an item reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_ItemUsedUp`

- **Type:** `Integer`
- **Description:** Item ID consumed by each production run, or `0` when nothing is consumed. It is an item reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_LoyaltyRequired`

- **Type:** `Integer`
- **Description:** Minimum local loyalty needed to run this production line.
- **Numeric semantics:** higher = stricter; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** map table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- Placed facilities in [Facilities](Facilities.md) reference `FacilityTypes.ubIndex`; each `ASSIGNMENT` selects an assignment type used by the facility UI and the strategic assignment system. Facility placement uses [Cities](Cities.md) data.

## Localization

Display names, tooltips and descriptions are localized through language overlays; numeric IDs, sector codes and matrix values are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Map table data.
- Field names, XML comments and observed values.
- Facility and assignment terminology from the map UI.
- Descriptions are based on the field names and observed table values; no runtime formula is claimed.

## Open questions

- Which facility UI surface uses `szFacilityShortName` instead of `szFacilityName` is not confirmed; this needs a code/UI check.
- The 0–1000 chance interpretation, the `100` baseline for performance and the exact scale of `sCantinaFoodModifier` are inferred from the observed values, not stated by an XML comment.
- `pmcentrypoint` and the severe-injury/assignment formulas are implemented by the game code, not by additional fields.
