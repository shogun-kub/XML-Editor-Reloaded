---
schema: 1
lang: en
family: difficulty
title: "DifficultySettings - campaign difficulty"
tables:
  - DifficultySettings
table_ids:
  - DifficultySettings
files:
  - DifficultySettings.xml
root: DIFFICULTY_SETTINGS
record: DIFFICULTY
key: uiIndex
sources:
  - Difficulty table data
  - Difficulty schema comments
status: reviewed
updated: 2026-11-07
---

# DifficultySettings - campaign difficulty

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/uiIndex`](#difficulty_settingsdifficultyuiindex)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/Name`](#difficulty_settingsdifficultyname)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/ConfirmText`](#difficulty_settingsdifficultyconfirmtext)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/StartingCash`](#difficulty_settingsdifficultystartingcash)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/EnemyAPBonus`](#difficulty_settingsdifficultyenemyapbonus)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/NumKillsPerProgressPoint`](#difficulty_settingsdifficultynumkillsperprogresspoint)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/InitialGarrisonPercentages`](#difficulty_settingsdifficultyinitialgarrisonpercentages)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/MinEnemyGroupSize`](#difficulty_settingsdifficultyminenemygroupsize)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CounterAttackGroupSize`](#difficulty_settingsdifficultycounterattackgroupsize)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/PercentElitesBonus`](#difficulty_settingsdifficultypercentelitesbonus)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/UnlimitedPoolOfTroops`](#difficulty_settingsdifficultyunlimitedpooloftroops)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/QueensInitialPoolOfTroops`](#difficulty_settingsdifficultyqueensinitialpooloftroops)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolMaxSizePerDifficultyLevel`](#difficulty_settingsdifficultyqueenpoolmaxsizeperdifficultylevel)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolIncrementDaysPerDifficultyLevel`](#difficulty_settingsdifficultyqueenpoolincrementdaysperdifficultylevel)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolBaseIncrementSizePerDifficultyLevel`](#difficulty_settingsdifficultyqueenpoolbaseincrementsizeperdifficultylevel)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolRecruitPercentPerDifficultyLevel`](#difficulty_settingsdifficultyqueenpoolrecruitpercentperdifficultylevel)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/EnemyStartingAlertLevel`](#difficulty_settingsdifficultyenemystartingalertlevel)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/EnemyAlertDecay`](#difficulty_settingsdifficultyenemyalertdecay)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/NumAwareBattles`](#difficulty_settingsdifficultynumawarebattles)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/BaseDelayInMinutesBetweenEvaluations`](#difficulty_settingsdifficultybasedelayinminutesbetweenevaluations)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/EvaluationDelayVariance`](#difficulty_settingsdifficultyevaluationdelayvariance)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/GracePeriodInHoursAfterSectorLiberation`](#difficulty_settingsdifficultygraceperiodinhoursaftersectorliberation)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/GracePeriodInDaysAfterPatrolDestroyed`](#difficulty_settingsdifficultygraceperiodindaysafterpatroldestroyed)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/AggressiveQueenAi`](#difficulty_settingsdifficultyaggressivequeenai)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/MaxMercDeaths`](#difficulty_settingsdifficultymaxmercdeaths)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/LootStatusModifier`](#difficulty_settingsdifficultylootstatusmodifier)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CreatureSpreadTime`](#difficulty_settingsdifficultycreaturespreadtime)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/QueenReproductionBase`](#difficulty_settingsdifficultyqueenreproductionbase)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/QueenReproductionBonus`](#difficulty_settingsdifficultyqueenreproductionbonus)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/QueenInitBonusSpread`](#difficulty_settingsdifficultyqueeninitbonusspread)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CreaturePopulationModifier`](#difficulty_settingsdifficultycreaturepopulationmodifier)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CreatureTownAggressiveness`](#difficulty_settingsdifficultycreaturetownaggressiveness)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/SectorJ9B1NumTroops`](#difficulty_settingsdifficultysectorj9b1numtroops)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/SectorJ9B2NumCreatures`](#difficulty_settingsdifficultysectorj9b2numcreatures)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/SectorK4B1NumTroops`](#difficulty_settingsdifficultysectork4b1numtroops)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/SectorK4B1NumElites`](#difficulty_settingsdifficultysectork4b1numelites)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/SectorO3B1NumTroops`](#difficulty_settingsdifficultysectoro3b1numtroops)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/SectorO3B1NumElites`](#difficulty_settingsdifficultysectoro3b1numelites)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/SectorP3B1NumElites`](#difficulty_settingsdifficultysectorp3b1numelites)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/StrategicAiActionWakeQueen`](#difficulty_settingsdifficultystrategicaiactionwakequeen)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/UpdateLastDayOfPlayerActivity`](#difficulty_settingsdifficultyupdatelastdayofplayeractivity)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/ChanceOfEnemyAmbushes`](#difficulty_settingsdifficultychanceofenemyambushes)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/AllowReinforcements`](#difficulty_settingsdifficultyallowreinforcements)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/AllowReinforcementsOmerta`](#difficulty_settingsdifficultyallowreinforcementsomerta)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/PopulationLevel2`](#difficulty_settingsdifficultypopulationlevel2)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/PopulationLevel3`](#difficulty_settingsdifficultypopulationlevel3)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops1`](#difficulty_settingsdifficultyweaponcachetroops1)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops2`](#difficulty_settingsdifficultyweaponcachetroops2)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops3`](#difficulty_settingsdifficultyweaponcachetroops3)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops4`](#difficulty_settingsdifficultyweaponcachetroops4)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops5`](#difficulty_settingsdifficultyweaponcachetroops5)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/UpgradeAdminsToTroops`](#difficulty_settingsdifficultyupgradeadminstotroops)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/UpgradeGarrisonsAdminsToTroops`](#difficulty_settingsdifficultyupgradegarrisonsadminstotroops)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/AlwaysUpGradeAdminsToTroopsProgress`](#difficulty_settingsdifficultyalwaysupgradeadminstotroopsprogress)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/LevelModifierLowLimit`](#difficulty_settingsdifficultylevelmodifierlowlimit)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/LevelModifierHighLimit`](#difficulty_settingsdifficultylevelmodifierhighlimit)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/AllowUnrestrictedXPLevels`](#difficulty_settingsdifficultyallowunrestrictedxplevels)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/QueenAttackLosingControlOfSector`](#difficulty_settingsdifficultyqueenattacklosingcontrolofsector)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/BloodcatAmbushSectors`](#difficulty_settingsdifficultybloodcatambushsectors)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/AirRaidLookForDive`](#difficulty_settingsdifficultyairraidlookfordive)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/NumberOfTurnsPowerGenFanWillBeStoppedFor`](#difficulty_settingsdifficultynumberofturnspowergenfanwillbestoppedfor)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/RadioSightings`](#difficulty_settingsdifficultyradiosightings)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/RadioSightings2`](#difficulty_settingsdifficultyradiosightings2)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/EnemyMoraleWorried`](#difficulty_settingsdifficultyenemymoraleworried)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/MaxMortarsPerTeam`](#difficulty_settingsdifficultymaxmortarsperteam)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficulty`](#difficulty_settingsdifficultycthconstantsaimdifficulty)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficulty`](#difficulty_settingsdifficultycthconstantsbasedifficulty)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyMilitia`](#difficulty_settingsdifficultycthconstantsaimdifficultymilitia)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyMilitia`](#difficulty_settingsdifficultycthconstantsbasedifficultymilitia)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyPlayer`](#difficulty_settingsdifficultycthconstantsaimdifficultyplayer)
  - [`DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyPlayer`](#difficulty_settingsdifficultycthconstantsbasedifficultyplayer)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`DifficultySettings.xml` defines the campaign difficulty levels: starting cash, enemy AP and group settings, queen economy, alert behavior, reinforcements, CTH constants and sector starting forces.

## Tables

| Table | File | Notes |
|---|---|---|
| `DifficultySettings` | `DifficultySettings.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
DIFFICULTY_SETTINGS
└── DIFFICULTY
    ├── uiIndex
    ├── Name
    ├── ConfirmText
    ├── StartingCash
    ├── EnemyAPBonus
    ├── NumKillsPerProgressPoint
    ├── InitialGarrisonPercentages
    ├── MinEnemyGroupSize
    ├── CounterAttackGroupSize
    ├── PercentElitesBonus
    ├── UnlimitedPoolOfTroops
    ├── QueensInitialPoolOfTroops
    ├── QueenPoolMaxSizePerDifficultyLevel
    ├── QueenPoolIncrementDaysPerDifficultyLevel
    ├── QueenPoolBaseIncrementSizePerDifficultyLevel
    ├── QueenPoolRecruitPercentPerDifficultyLevel
    ├── EnemyStartingAlertLevel
    ├── EnemyAlertDecay
    ├── NumAwareBattles
    ├── BaseDelayInMinutesBetweenEvaluations
    ├── EvaluationDelayVariance
    ├── GracePeriodInHoursAfterSectorLiberation
    ├── GracePeriodInDaysAfterPatrolDestroyed
    ├── AggressiveQueenAi
    ├── MaxMercDeaths
    ├── LootStatusModifier
    ├── CreatureSpreadTime
    ├── QueenReproductionBase
    ├── QueenReproductionBonus
    ├── QueenInitBonusSpread
    ├── CreaturePopulationModifier
    ├── CreatureTownAggressiveness
    ├── SectorJ9B1NumTroops
    ├── SectorJ9B2NumCreatures
    ├── SectorK4B1NumTroops
    ├── SectorK4B1NumElites
    ├── SectorO3B1NumTroops
    ├── SectorO3B1NumElites
    ├── SectorP3B1NumElites
    ├── StrategicAiActionWakeQueen
    ├── UpdateLastDayOfPlayerActivity
    ├── ChanceOfEnemyAmbushes
    ├── AllowReinforcements
    ├── AllowReinforcementsOmerta
    ├── PopulationLevel2
    ├── PopulationLevel3
    ├── WeaponCacheTroops1
    ├── WeaponCacheTroops2
    ├── WeaponCacheTroops3
    ├── WeaponCacheTroops4
    ├── WeaponCacheTroops5
    ├── UpgradeAdminsToTroops
    ├── UpgradeGarrisonsAdminsToTroops
    ├── AlwaysUpGradeAdminsToTroopsProgress
    ├── LevelModifierLowLimit
    ├── LevelModifierHighLimit
    ├── AllowUnrestrictedXPLevels
    ├── QueenAttackLosingControlOfSector
    ├── BloodcatAmbushSectors
    ├── AirRaidLookForDive
    ├── NumberOfTurnsPowerGenFanWillBeStoppedFor
    ├── RadioSightings
    ├── RadioSightings2
    ├── EnemyMoraleWorried
    ├── MaxMortarsPerTeam
    ├── CthConstantsAimDifficulty
    ├── CthConstantsBaseDifficulty
    ├── CthConstantsAimDifficultyMilitia
    ├── CthConstantsBaseDifficultyMilitia
    ├── CthConstantsAimDifficultyPlayer
    └── CthConstantsBaseDifficultyPlayer
```

## Fields

### `DIFFICULTY_SETTINGS/DIFFICULTY/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Difficulty-level index. The valid range is `0..4`; the table comment warns that the four original levels are still hardcoded.
- **Values:** `0..4`; the table comment states that these four original levels are hardcoded.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Short name of the difficulty level.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/ConfirmText`

- **Type:** `String` (`CHAR16`)
- **Description:** Confirmation text shown for the difficulty level.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/StartingCash`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting cash for the difficulty.
- **Numeric semantics:** Higher values mean more starting cash; negative values are allowed and represent starting debt.
- **Values:** Integer cash value; negative values are allowed.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyAPBonus`

- **Type:** `Integer` (`INT8`)
- **Description:** Extra APs for enemy soldiers. The value is not adjusted for the selected AP system.
- **Numeric semantics:** Signed modifier: positive values give enemies extra AP, negative values reduce their AP, and `0` is neutral.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/NumKillsPerProgressPoint`

- **Type:** `Integer` (`UINT16`)
- **Description:** Kills required to gain one progress point.
- **Numeric semantics:** Higher values mean more kills are needed per progress point; negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/InitialGarrisonPercentages`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage of troops placed on the map at game start; `100` means the default garrison.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/MinEnemyGroupSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Minimum enemy group size.
- **Numeric semantics:** Higher values mean a larger minimum group; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CounterAttackGroupSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Size of each of the four counter-attack groups.
- **Numeric semantics:** Higher values mean a larger group; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/PercentElitesBonus`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage of troops converted to extra elites.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UnlimitedPoolOfTroops`

- **Type:** `Integer` (`INT32`)
- **Description:** If set, the queen has an unlimited troop pool and the queen-pool limits are ignored.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueensInitialPoolOfTroops`

- **Type:** `Integer` (`INT32`)
- **Description:** Troops available to the queen at game start.
- **Numeric semantics:** Higher values mean a larger troop/weapon count, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolMaxSizePerDifficultyLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Maximum size of the queen’s reserve pool.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolIncrementDaysPerDifficultyLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Days between queen recruit-group training intervals; `0` uses the old simple schema.
- **Numeric semantics:** Higher values mean a higher level; negative values are outside the normal level range.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolBaseIncrementSizePerDifficultyLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Recruit capacity of one training camp.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolRecruitPercentPerDifficultyLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage of a sector population recruited by the army each day.
- **Numeric semantics:** Percentage; higher values mean a stronger effect, and negative values reduce the effect below the baseline.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyStartingAlertLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting enemy alert level.
- **Numeric semantics:** Higher values mean a higher level; negative values are outside the normal level range.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyAlertDecay`

- **Type:** `Integer` (`INT32`)
- **Description:** Amount by which alertness decays after an enemy chase.
- **Numeric semantics:** Higher values mean a longer delay/decay period, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/NumAwareBattles`

- **Type:** `Integer` (`INT32`)
- **Description:** Number of enemy-initiated battles before the queen leaves full-alert mode.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/BaseDelayInMinutesBetweenEvaluations`

- **Type:** `Integer` (`INT32`)
- **Description:** Base delay between queen decisions, in minutes.
- **Numeric semantics:** Time value; higher values mean a longer delay/duration, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EvaluationDelayVariance`

- **Type:** `Integer` (`INT32`)
- **Description:** Random variation added to the queen decision delay.
- **Numeric semantics:** Time value; higher values mean a longer delay/duration, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/GracePeriodInHoursAfterSectorLiberation`

- **Type:** `Integer` (`INT32`)
- **Description:** Delay before the queen may attack a newly liberated sector, in hours.
- **Numeric semantics:** Higher values mean a longer delay/decay period, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/GracePeriodInDaysAfterPatrolDestroyed`

- **Type:** `Integer` (`INT32`)
- **Description:** Delay before a destroyed patrol can be refilled, in days.
- **Numeric semantics:** Higher values mean a longer delay/decay period, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AggressiveQueenAi`

- **Type:** `Integer` (`INT32`)
- **Description:** Enables the aggressive queen AI behavior.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/MaxMercDeaths`

- **Type:** `Integer` (`INT32`)
- **Description:** Maximum mercenary deaths allowed by the difficulty.
- **Numeric semantics:** Higher values mean a larger troop/weapon count, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/LootStatusModifier`

- **Type:** `Integer` (`INT32`)
- **Description:** Modifier to dropped-loot status.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CreatureSpreadTime`

- **Type:** `Integer` (`INT32`)
- **Description:** Time for creatures to spread.
- **Numeric semantics:** Time value; higher values mean a longer delay/duration, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenReproductionBase`

- **Type:** `Integer` (`INT32`)
- **Description:** Base queen reproduction value.
- **Numeric semantics:** Higher values mean a stronger value/effect, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenReproductionBonus`

- **Type:** `Integer` (`INT32`)
- **Description:** Queen reproduction bonus.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenInitBonusSpread`

- **Type:** `Integer` (`INT32`)
- **Description:** Queen initiative bonus spread.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CreaturePopulationModifier`

- **Type:** `Integer` (`INT32`)
- **Description:** Modifier to creature population.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CreatureTownAggressiveness`

- **Type:** `Integer` (`INT32`)
- **Description:** Creature aggressiveness in towns.
- **Numeric semantics:** Higher values mean a stronger value/effect, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorJ9B1NumTroops`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting troop/creature/elite count for sector J9B1N.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorJ9B2NumCreatures`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting troop/creature/elite count for sector J9B2NumC.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorK4B1NumTroops`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting troop/creature/elite count for sector K4B1N.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorK4B1NumElites`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting troop/creature/elite count for sector K4B1N.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorO3B1NumTroops`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting troop/creature/elite count for sector O3B1N.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorO3B1NumElites`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting troop/creature/elite count for sector O3B1N.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorP3B1NumElites`

- **Type:** `Integer` (`INT32`)
- **Description:** Starting troop/creature/elite count for sector P3B1N.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/StrategicAiActionWakeQueen`

- **Type:** `Integer` (`INT32`)
- **Description:** Strategic AI action that wakes the queen.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UpdateLastDayOfPlayerActivity`

- **Type:** `Integer` (`INT32`)
- **Description:** Updates the last day of player activity.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/ChanceOfEnemyAmbushes`

- **Type:** `Integer` (`INT32`)
- **Description:** Chance of enemy ambushes.
- **Numeric semantics:** Higher values mean a greater chance; negative values are outside the valid chance range.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AllowReinforcements`

- **Type:** `Integer` (`INT32`)
- **Description:** Allows enemy reinforcements.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AllowReinforcementsOmerta`

- **Type:** `Integer` (`INT32`)
- **Description:** Allows enemy reinforcements in Omerta.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/PopulationLevel2`

- **Type:** `Integer` (`INT32`)
- **Description:** Population level 2 value.
- **Numeric semantics:** Higher values mean a higher level; negative values are outside the normal level range.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/PopulationLevel3`

- **Type:** `Integer` (`INT32`)
- **Description:** Population level 3 value.
- **Numeric semantics:** Higher values mean a higher level; negative values are outside the normal level range.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops1`

- **Type:** `Integer` (`INT32`)
- **Description:** Number of troops assigned to weapon cache 1.
- **Numeric semantics:** Higher values mean a larger troop/weapon count, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops2`

- **Type:** `Integer` (`INT32`)
- **Description:** Number of troops assigned to weapon cache 2.
- **Numeric semantics:** Higher values mean a larger troop/weapon count, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops3`

- **Type:** `Integer` (`INT32`)
- **Description:** Number of troops assigned to weapon cache 3.
- **Numeric semantics:** Higher values mean a larger troop/weapon count, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops4`

- **Type:** `Integer` (`INT32`)
- **Description:** Number of troops assigned to weapon cache 4.
- **Numeric semantics:** Higher values mean a larger troop/weapon count, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops5`

- **Type:** `Integer` (`INT32`)
- **Description:** Number of troops assigned to weapon cache 5.
- **Numeric semantics:** Higher values mean a larger troop/weapon count, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UpgradeAdminsToTroops`

- **Type:** `Integer` (`INT32`)
- **Description:** Upgrades admins to troops.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UpgradeGarrisonsAdminsToTroops`

- **Type:** `Integer` (`INT32`)
- **Description:** Upgrades garrison admins to troops.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AlwaysUpGradeAdminsToTroopsProgress`

- **Type:** `Integer` (`INT32`)
- **Description:** Always upgrades admins to troops as progress advances.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/LevelModifierLowLimit`

- **Type:** `Integer` (`INT32`)
- **Description:** Low limit for the level modifier.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/LevelModifierHighLimit`

- **Type:** `Integer` (`INT32`)
- **Description:** High limit for the level modifier.
- **Numeric semantics:** Signed modifier; higher values mean a stronger positive effect, and negative values apply the opposite effect (a penalty or reduction).
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AllowUnrestrictedXPLevels`

- **Type:** `Integer` (`INT32`)
- **Description:** Allows experience levels above the normal cap.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenAttackLosingControlOfSector`

- **Type:** `Integer` (`INT32`)
- **Description:** Queen attack behavior when losing control of a sector.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/BloodcatAmbushSectors`

- **Type:** `Integer` (`INT32`)
- **Description:** Bloodcat ambush sector setting.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AirRaidLookForDive`

- **Type:** `Integer` (`INT32`)
- **Description:** Air-raid AI behavior flag.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/NumberOfTurnsPowerGenFanWillBeStoppedFor`

- **Type:** `Integer` (`INT32`)
- **Description:** Number of turns the power-generation fan is stopped for.
- **Numeric semantics:** Higher values mean a larger quantity; negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/RadioSightings`

- **Type:** `Integer` (`INT32`)
- **Description:** Radio-sighting setting.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/RadioSightings2`

- **Type:** `Integer` (`INT32`)
- **Description:** Second radio-sighting setting.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyMoraleWorried`

- **Type:** `Integer` (`INT32`)
- **Description:** Enemy morale value for the worried state.
- **Numeric semantics:** Higher values mean a stronger value/effect, and negative values are not meaningful.
- **Values:** Integer difficulty value; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/MaxMortarsPerTeam`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum mortars per enemy team.
- **Numeric semantics:** Higher values mean a larger mortar allowance; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficulty`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH aim-difficulty constant override.
- **Only used with:** NCTH.
- **Numeric semantics:** Not confirmed; see Open questions.
- **Values:** Decimal NCTH constant override.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficulty`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH base-difficulty constant override.
- **Only used with:** NCTH.
- **Numeric semantics:** Not confirmed; see Open questions.
- **Values:** Decimal NCTH constant override.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyMilitia`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH aim-difficulty constant override for militia.
- **Only used with:** NCTH.
- **Numeric semantics:** Not confirmed; see Open questions.
- **Values:** Decimal NCTH constant override.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyMilitia`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH base-difficulty constant override for militia.
- **Only used with:** NCTH.
- **Numeric semantics:** Not confirmed; see Open questions.
- **Values:** Decimal NCTH constant override.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyPlayer`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH aim-difficulty constant override for the player.
- **Only used with:** NCTH.
- **Numeric semantics:** Not confirmed; see Open questions.
- **Values:** Decimal NCTH constant override.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyPlayer`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH base-difficulty constant override for the player.
- **Only used with:** NCTH.
- **Numeric semantics:** Not confirmed; see Open questions.
- **Values:** Decimal NCTH constant override.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** difficulty table data; table metadata.
- **Confidence:** `inferred`

## Relations

- This table is selected by the campaign difficulty setting; it does not reference item or profile rows.
- The `CthConstants*` fields are NCTH constants and are relevant only when NCTH is active.

## Localization

`Name` and `ConfirmText` are display text; numeric settings are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Difficulty table data.
- Difficulty schema comments.
- Field types: table data and field names; enum values come from the schema comments.
- Descriptions are conservative where no formula is stated; no observed value ranges are used.

## Open questions

- Numeric direction and negative-value semantics are not confirmed for: `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficulty`, `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficulty`, `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyMilitia`, `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyMilitia`, `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyPlayer`, `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyPlayer`.

The exact interaction between difficulty levels and the hardcoded four-level logic is outside this table; the comment warns that new entries must not be created.
