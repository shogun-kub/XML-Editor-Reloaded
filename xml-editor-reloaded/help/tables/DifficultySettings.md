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
  - Game code (difficulty loader and its runtime consumers)
status: reviewed
updated: 2026-11-08
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

The loader keeps one working record for the whole file and copies it into the slot named by `uiIndex` when the record closes. An element omitted from a `DIFFICULTY` record therefore keeps the value from the previous record (or `0` for the first record), which also means `uiIndex` must be present in every record or that record overwrites the previous slot. All numeric elements are parsed with an integer parser, and only the low byte or word that the runtime field accepts survives, which makes some effective ranges much narrower than the signed business value suggests.

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
- **Description:** Difficulty-level index. It selects the difficulty record and is the array key; entry `0` is a placeholder and levels `1..4` are the playable Novice, Experienced, Expert and Insane levels.
- **Values:** `0..4`; the array has `MAX_DIF_LEVEL` (`5`) slots, and the table comment warns that these four original levels are still hardcoded.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; difficulty-level enum); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Short name of the difficulty level. It is converted from UTF-8 into a fixed 128-character buffer, so longer text is truncated.
- **Values:** Free text.
- **Default:** Empty string in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/ConfirmText`

- **Type:** `String` (`CHAR16`)
- **Description:** Confirmation text shown for the difficulty level. It is converted from UTF-8 into a fixed 1024-character buffer, so longer text is truncated.
- **Values:** Free text.
- **Default:** Empty string in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/StartingCash`

- **Type:** `Integer` (`INT32`)
- **Description:** Cash added to the player's account when the campaign starts. The table comment states that negative values are allowed.
- **Numeric semantics:** Higher values mean more starting money; negative values start the campaign in debt.
- **Values:** `-2147483648..2147483647`; the loader stores this field as `INT32`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; campaign-start deposit); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyAPBonus`

- **Type:** `Integer` (`INT8`)
- **Description:** Extra action points for enemy soldiers. The table comment notes that the value is not adjusted for the selected AP system.
- **Numeric semantics:** Signed modifier: positive values give enemies extra AP, negative values take AP away, and `0` is neutral.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; soldier AP calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/NumKillsPerProgressPoint`

- **Type:** `Integer` (`UINT16`)
- **Description:** Kills required to gain one progress point. The table comment adds that the total number of progress points is also capped by `GAME_PROGRESS_KILLS` in `Ja2_Options.ini`.
- **Numeric semantics:** Higher values mean more kills are needed per progress point; the game uses the value as a divisor, so it must be at least `1` at runtime.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; progress calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/InitialGarrisonPercentages`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage applied to the starting army group sizes, desired populations and patrol sizes. The table comment gives `100` = all default troops are placed and `200` = twice as many.
- **Numeric semantics:** Higher values mean more starting enemy troops and larger garrisons; negative values are not meaningful.
- **Values:** `-2147483648..2147483647`; the loader stores this field as `INT32`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI campaign initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/MinEnemyGroupSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Lower bound for enemy group sizes. The strategic AI also uses twice this value as its normal group size.
- **Numeric semantics:** Higher values mean larger minimum enemy groups; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI group sizing); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CounterAttackGroupSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Size of each of the four counter-attack groups that attack a captured city. The strategic AI raises the value to `MinEnemyGroupSize` when that is larger.
- **Numeric semantics:** Higher values mean larger counter-attack groups; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; counter-attack creation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/PercentElitesBonus`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage points moved from the regular-troop share to the elite share of every army composition. Only positive values are applied.
- **Numeric semantics:** Higher values mean more elites in enemy groups; `0` and negative values leave the compositions unchanged.
- **Values:** `-2147483648..2147483647`; the loader stores this field as `INT32`, but it is only used when greater than `0`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; army composition setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UnlimitedPoolOfTroops`

- **Type:** `Boolean`
- **Description:** If set, the queen has unlimited troops and the four `QueenPool*` settings are ignored.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI reinforcement pool); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueensInitialPoolOfTroops`

- **Type:** `Integer` (`INT32`)
- **Description:** Size of the queen's reserve at campaign start; part of it is issued to starting garrisons and patrols and the rest stays in the reserve.
- **Numeric semantics:** Higher values mean a larger starting reserve; negative values are clamped to `0` by the loader.
- **Values:** `0..2147483647`; negative input is clamped to `0`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI reinforcement pool); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolMaxSizePerDifficultyLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Maximum size the queen's reserve may be trained up to.
- **Numeric semantics:** Higher values mean a larger allowed reserve; negative values are clamped to `0` by the loader.
- **Values:** `0..2147483647`; negative input is clamped to `0`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic event `EVENT_ARMY_FINISH_TRAINING`); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolIncrementDaysPerDifficultyLevel`

- **Type:** `Integer` (`UINT16`)
- **Description:** Interval in days between recruit-training batches. `0` selects the old simple schema, in which the reserve is topped up as soon as it drops below `QueenPoolBaseIncrementSizePerDifficultyLevel`.
- **Numeric semantics:** Higher values mean longer intervals between training batches; `0` has the special old-schema meaning.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic event handler); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolBaseIncrementSizePerDifficultyLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Capacity of one training camp, and in the old simple schema the reserve threshold and top-up amount as well. The table comment warns that very large numbers can overflow.
- **Numeric semantics:** Higher values mean larger training batches and a larger old-schema top-up; negative values are clamped to `0` by the loader.
- **Values:** `0..2147483647`; negative input is clamped to `0`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI reinforcement pool); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolRecruitPercentPerDifficultyLevel`

- **Type:** `Integer` (`UINT8`)
- **Description:** Percentage of a sector's population that the army takes as recruits each day.
- **Numeric semantics:** Higher values mean faster recruitment from towns; negative values are not meaningful.
- **Values:** `0..100`; the loader stores this field as `UINT8` and clamps it to `100`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic event handler); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyStartingAlertLevel`

- **Type:** `Integer` (`UINT32`)
- **Description:** Starting army alertness: the percentage chance that an enemy group spots an adjacent player or militia group.
- **Numeric semantics:** Higher values mean a higher spotting chance; values above `100` make the check always succeed.
- **Values:** `0..4294967295`; the table comment describes it as a starting alert chance.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI spotting checks); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyAlertDecay`

- **Type:** `Integer` (`UINT32`)
- **Description:** Amount subtracted from the army alertness percentage each time a spot check succeeds. The table comment recommends `1..100` and a practical range of `20..60`.
- **Numeric semantics:** Higher values mean alertness falls faster after a successful spot; negative values are not meaningful.
- **Values:** `0..4294967295`; the table comment gives no hard limit.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI spotting checks); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/NumAwareBattles`

- **Type:** `Integer` (`UINT32`)
- **Description:** Number of enemy-initiated battles during which the queen stays in full-alert mode; each such battle decrements the counter. The strategic AI keeps the counter in a byte, so only `0..255` survive.
- **Numeric semantics:** Higher values mean the queen stays alert for longer; negative values are not meaningful.
- **Values:** `0..4294967295`; effective runtime range `0..255` because the strategic AI stores it as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic movement/alert logic); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/BaseDelayInMinutesBetweenEvaluations`

- **Type:** `Integer` (`UINT32`)
- **Description:** Base interval in minutes between the queen's strategic decisions.
- **Numeric semantics:** Higher values mean slower queen decisions; negative values are not meaningful.
- **Values:** `0..4294967295`; the loader stores this field as `UINT32`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; queen evaluation scheduling); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EvaluationDelayVariance`

- **Type:** `Integer` (`UINT32`)
- **Description:** Random extra delay added to the base interval between queen decisions: the game adds a value from `0` to this value minus one, in minutes.
- **Numeric semantics:** Higher values mean more variation in decision timing; negative values are not meaningful.
- **Values:** `0..4294967295`; the loader stores this field as `UINT32`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; queen evaluation scheduling); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/GracePeriodInHoursAfterSectorLiberation`

- **Type:** `Integer` (`UINT32`)
- **Description:** Delay in hours between the player liberating a sector and the queen being allowed to decide to attack it. The strategic AI keeps this value in a byte, so only `0..255` survive.
- **Numeric semantics:** Higher values mean a longer grace period and an easier game; negative values are not meaningful.
- **Values:** `0..4294967295`; effective runtime range `0..255` because the strategic AI stores it as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI grace-period check); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/GracePeriodInDaysAfterPatrolDestroyed`

- **Type:** `Integer` (`UINT32`)
- **Description:** Delay in days between the player destroying an enemy patrol and the queen being allowed to refill it; the game stores the resulting day modulo `100`.
- **Numeric semantics:** Higher values mean defeated patrols reappear less often; negative values are not meaningful.
- **Values:** `0..4294967295`; the refill day is a day number modulo `100`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; patrol refill scheduling); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AggressiveQueenAi`

- **Type:** `Boolean`
- **Description:** If set, the queen prefers attacking player sectors over defending her own.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; queen AI aggression flag); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/MaxMercDeaths`

- **Type:** `Integer` (`INT32`)
- **Description:** Maximum number of not-yet-hired mercenaries that may die from the daily off-screen death roll applied to mercenaries working elsewhere. The table comment gives `0..10`.
- **Numeric semantics:** Higher values mean more off-screen merc deaths; negative values are not meaningful.
- **Values:** `-2147483648..2147483647`; the loader stores this field as `INT32`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; unhired-merc death handler); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/LootStatusModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum random damage applied to equipment dropped by dead enemies: the game subtracts a random value from `0` to this value minus one from the item's status. The table comment gives `0..100`.
- **Numeric semantics:** Higher values mean more damaged loot; a negative written value wraps into the large end of the byte range because of the loader cast.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; loot status reduction); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CreatureSpreadTime`

- **Type:** `Integer` (`UINT16`)
- **Description:** Period in minutes between creature spreads once the creatures are active. The table comment gives `5..14400`.
- **Numeric semantics:** Higher values mean slower spread; negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; creature spread event); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenReproductionBase`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of creatures produced at each spread, before the random bonus. The table comment gives `1..30`.
- **Numeric semantics:** Higher values mean faster creature growth; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; creature reproduction); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenReproductionBonus`

- **Type:** `Integer` (`UINT8`)
- **Description:** Upper bound of the random bonus added at each spread; the game adds a value from `0` to this value. The table comment gives `0..10`.
- **Numeric semantics:** Higher values mean a larger random bonus and faster growth; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; creature reproduction); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenInitBonusSpread`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of immediate spreads performed when the creature lair is initialised, giving the creatures a head start. The table comment gives `1..10`.
- **Numeric semantics:** Higher values mean a bigger initial head start; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; creature lair initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CreaturePopulationModifier`

- **Type:** `Integer` (`INT8`)
- **Description:** Intended signed modifier of the chance to populate a sector, per the table comment. The current code copies it into a campaign variable that is saved and loaded, but no population formula reads it, so changing the value has no effect in this build.
- **Numeric semantics:** Not confirmed; the value is not read by any gameplay code in this build, so no direction is claimed.
- **Values:** `-128..127`; the table comment recommends `-5..5`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; creature spreading); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CreatureTownAggressiveness`

- **Type:** `Integer` (`INT8`)
- **Description:** Chance adjustment for creature attacks on towns. A group with `n` creatures attacks when `10 * n` plus this value exceeds a random number from `0` to `99`. The table comment gives `-100..100`.
- **Numeric semantics:** Higher values mean town attacks are more likely; negative values make them less likely.
- **Values:** `-128..127`; the loader stores this field as `INT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; creature town-attack check); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorJ9B1NumTroops`

- **Type:** `Integer` (`UINT8`)
- **Description:** Intended enemy soldier count for underground sector J9 B1. The hardcoded underground setup that reads it is unreachable in this build: `BuildUndergroundSectorInfoList` returns immediately after the `initunderground.lua` script builds the sector list, and the shipped script sets the counts from its own tables. The value therefore has no effect.
- **Numeric semantics:** Higher values would mean more starting enemy soldiers, but the value is not applied in this build.
- **Values:** `0..255`; the loader stores this field as `UINT8`, but no reachable code consumes it.
- **Default:** No effect in this build.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorJ9B2NumCreatures`

- **Type:** `Integer` (`UINT8`)
- **Description:** Intended creature count for the J9 B2 feeding zone, `2 + 2 * value + Random(2)`. The hardcoded underground setup that reads it is unreachable in this build: the Lua `initunderground.lua` path returns before it, and the shipped script sets the counts from its own tables. The value therefore has no effect.
- **Numeric semantics:** Higher values would mean more starting creatures, but the value is not applied in this build.
- **Values:** `0..255`; the loader stores this field as `UINT8`, but no reachable code consumes it.
- **Default:** No effect in this build.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorK4B1NumTroops`

- **Type:** `Integer` (`UINT8`)
- **Description:** Intended enemy soldier count for K4 B1, `6 + 2 * value + Random(3)`. The hardcoded underground setup that reads it is unreachable in this build: the Lua `initunderground.lua` path returns before it, and the shipped script sets the counts from its own tables. The value therefore has no effect.
- **Numeric semantics:** Higher values would mean more starting enemy soldiers, but the value is not applied in this build.
- **Values:** `0..255`; the loader stores this field as `UINT8`, but no reachable code consumes it.
- **Default:** No effect in this build.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorK4B1NumElites`

- **Type:** `Integer` (`UINT8`)
- **Description:** Intended elite count for K4 B1, `4 + value + Random(2)`. Two defects make it ineffective: the loader's read list names `SectorK4B2NumElites` instead of this element, so a shipped `SectorK4B1NumElites` element is skipped, and the hardcoded underground setup that would apply it is unreachable because the Lua `initunderground.lua` path returns before it. The value stays `0` and has no effect.
- **Numeric semantics:** Higher values would mean more starting elites, but the value is neither read nor applied in this build.
- **Values:** `0..255`; the loader stores this field as `UINT8`, but the element is not read in the shipped loader.
- **Default:** Always `0` with the shipped loader, and no effect in this build.
- **Source:** game code (difficulty loader read list; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorO3B1NumTroops`

- **Type:** `Integer` (`UINT8`)
- **Description:** Intended enemy soldier count for O3 B1, `6 + 2 * value + Random(3)`. The hardcoded underground setup that reads it is unreachable in this build: the Lua `initunderground.lua` path returns before it, and the shipped script sets the counts from its own tables. The value therefore has no effect.
- **Numeric semantics:** Higher values would mean more starting enemy soldiers, but the value is not applied in this build.
- **Values:** `0..255`; the loader stores this field as `UINT8`, but no reachable code consumes it.
- **Default:** No effect in this build.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorO3B1NumElites`

- **Type:** `Integer` (`UINT8`)
- **Description:** Intended elite count for O3 B1, `4 + value + Random(2)`. Two defects make it ineffective: the loader's read list names `SectorO3B2NumElites` instead of this element, so a shipped `SectorO3B1NumElites` element is skipped, and the hardcoded underground setup that would apply it is unreachable because the Lua `initunderground.lua` path returns before it. The value stays `0` and has no effect.
- **Numeric semantics:** Higher values would mean more starting elites, but the value is neither read nor applied in this build.
- **Values:** `0..255`; the loader stores this field as `UINT8`, but the element is not read in the shipped loader.
- **Default:** Always `0` with the shipped loader, and no effect in this build.
- **Source:** game code (difficulty loader read list; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorP3B1NumElites`

- **Type:** `Integer` (`UINT8`)
- **Description:** Intended elite count for P3 B1: the game would add `Random(3)` on Novice, `Random(5)` on Experienced, `Random(6)` on Expert and nothing on Insane. The hardcoded underground setup that reads it is unreachable in this build: the Lua `initunderground.lua` path returns before it, and the shipped script sets the counts from its own tables. The value therefore has no effect.
- **Numeric semantics:** Higher values would mean more starting elites, but the value is not applied in this build.
- **Values:** `0..255`; the loader stores this field as `UINT8`, but no reachable code consumes it.
- **Default:** No effect in this build.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/StrategicAiActionWakeQueen`

- **Type:** `Boolean`
- **Description:** If set, the queen is woken as soon as the player's first battle ends, instead of following the normal wake-up schedule.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; meanwhile event handler); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UpdateLastDayOfPlayerActivity`

- **Type:** `Integer` (`UINT16`)
- **Description:** Number of days added to the current world day when the player first captures a sector; the result is recorded as the last day of player activity.
- **Numeric semantics:** Higher values push the recorded activity day further into the future; negative values are not meaningful.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; player-command handling of a captured sector); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/ChanceOfEnemyAmbushes`

- **Type:** `Integer` (`INT32`)
- **Description:** Percentage-point modifier added to the enemy ambush chance when the player enters a sector.
- **Numeric semantics:** Signed modifier: positive values make ambushes more likely, negative values less likely, and `0` is neutral.
- **Values:** `-2147483648..2147483647`; the loader stores this field as `INT32`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; pre-battle ambush chance); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AllowReinforcements`

- **Type:** `Boolean`
- **Description:** If set, the enemy may send reinforcements to a sector.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; reinforcement system flag); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AllowReinforcementsOmerta`

- **Type:** `Boolean`
- **Description:** If set, the enemy may also send reinforcements to Omerta.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; reinforcement system flag); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/PopulationLevel2`

- **Type:** `Boolean`
- **Description:** If set, the level-2 perimeter defence population (the army groups two sectors away from Meduna) keeps its troops; when clear, that defence population is zeroed at campaign start.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI campaign initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/PopulationLevel3`

- **Type:** `Boolean`
- **Description:** If set, the level-3 perimeter defence population (the army groups three sectors away from Meduna) keeps its troops; when clear, that defence population is zeroed at campaign start.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI campaign initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops1`

- **Type:** `Integer` (`UINT8`)
- **Description:** Troop count for the first weapon-cache group; the game sets the group to `6 + 2 * value`.
- **Numeric semantics:** Higher values mean more troops at the cache; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops2`

- **Type:** `Integer` (`UINT8`)
- **Description:** Troop count for the second weapon-cache group; the game sets the group to `6 + 2 * value`.
- **Numeric semantics:** Higher values mean more troops at the cache; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops3`

- **Type:** `Integer` (`UINT8`)
- **Description:** Troop count for the third weapon-cache group; the game sets the group to `6 + 2 * value`.
- **Numeric semantics:** Higher values mean more troops at the cache; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops4`

- **Type:** `Integer` (`UINT8`)
- **Description:** Troop count for the fourth weapon-cache group; the game sets the group to `6 + 2 * value`.
- **Numeric semantics:** Higher values mean more troops at the cache; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops5`

- **Type:** `Integer` (`UINT8`)
- **Description:** Troop count for the fifth weapon-cache group; the game sets the group to `6 + 2 * value`.
- **Numeric semantics:** Higher values mean more troops at the cache; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UpgradeAdminsToTroops`

- **Type:** `Boolean`
- **Description:** If set, the AI may upgrade administrators to regular troops in moving enemy groups.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI admin upgrade); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UpgradeGarrisonsAdminsToTroops`

- **Type:** `Boolean`
- **Description:** If set, the AI may upgrade administrators to regular troops in garrisons.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI admin upgrade); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AlwaysUpGradeAdminsToTroopsProgress`

- **Type:** `Integer` (`UINT8`)
- **Description:** Player-progress percentage at which all administrators may be upgraded regardless of the two `Upgrade*AdminsToTroops` switches.
- **Numeric semantics:** Higher values delay the automatic upgrade until later progress; negative values are not meaningful.
- **Values:** `0..100`; the loader stores this field as `UINT8` and clamps it to `100`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI admin upgrade); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/LevelModifierLowLimit`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of levels a combatant's randomized level may roll below its map/progress level. Both limits at `0` reproduce the unmodified levels.
- **Numeric semantics:** Higher values allow lower random levels; negative values are not meaningful.
- **Values:** `0..5`; the loader stores this field as `UINT8` and clamps it to `5`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; soldier level randomisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/LevelModifierHighLimit`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of levels a combatant's randomized level may roll above its map/progress level. Both limits at `0` reproduce the unmodified levels.
- **Numeric semantics:** Higher values allow higher random levels; negative values are not meaningful.
- **Values:** `0..5`; the loader stores this field as `UINT8` and clamps it to `5`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; soldier level randomisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AllowUnrestrictedXPLevels`

- **Type:** `Boolean`
- **Description:** If set, the per-difficulty experience-level cap is ignored, so combatants on easier difficulties can reach level `10`. The table comment notes that Insane has no cap anyway.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; soldier creation level cap); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenAttackLosingControlOfSector`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of times the player may capture a town sector before the queen stops probing and attacking it; `0` allows unlimited attacks.
- **Numeric semantics:** Higher values allow the queen to keep attacking for longer; negative values are not meaningful.
- **Values:** `0..255`; the runtime field is `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; strategic AI sector-investigation check); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/BloodcatAmbushSectors`

- **Type:** `Boolean`
- **Description:** Flag element with no confirmed meaning: the loader stores it but no gameplay code reads it in this build, and the table comment leaves its purpose as `?`.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; no reader found); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AirRaidLookForDive`

- **Type:** `Boolean`
- **Description:** If set, the scripted beginning-of-game air raid uses the random dive/bombing selection; when clear, that raid follows a fixed dive, bombing, dive sequence.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; air-raid dive selection); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/NumberOfTurnsPowerGenFanWillBeStoppedFor`

- **Type:** `Integer` (`UINT32`)
- **Description:** Number of turns the power-generation fan is stopped for. The value is only compiled and used in the UB build; the table comment marks it as UB-only.
- **Numeric semantics:** Higher values mean the fan stays off for longer; negative values are not meaningful.
- **Values:** `0..4294967295`; the loader stores this field as `UINT32`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; UB tactical code); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/RadioSightings`

- **Type:** `Boolean`
- **Description:** If set, an alerted enemy soldier calls in help over the radio.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; enemy sighting/radio logic); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/RadioSightings2`

- **Type:** `Boolean`
- **Description:** If set, every enemy individual calls in help over the radio, not only alerted soldiers.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; enemy sighting/radio logic); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyMoraleWorried`

- **Type:** `Boolean`
- **Description:** If set, enemy morale cannot drop below the `worried` category; a hopeless result is raised to worried.
- **Values:** `0` or `1`.
- **Default:** `0` (false) in the first record; a later record keeps the previous value when the element is omitted.
- **Source:** game code (difficulty loader; morale category calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/MaxMortarsPerTeam`

- **Type:** `Integer` (`UINT8`)
- **Description:** Maximum number of AI soldiers per team, enemy or militia, that may spawn with a mortar.
- **Numeric semantics:** Higher values mean more mortars per team; negative values are not meaningful.
- **Values:** `0..255`; the loader stores this field as `UINT8`.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; enemy inventory choosing); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficulty`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH aim-difficulty modifier for enemy combatants: it is added to the aim part of the enemy chance-to-hit.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed percentage-point modifier: positive values make enemies more accurate with aim clicks, negative values less accurate. The loader parses the element with an integer parser, so only the integer part of a decimal value is stored.
- **Values:** `-100..100` per the table comment; the loader stores whole numbers only.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficulty`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH base-difficulty modifier for enemy combatants: it is added to the base part of the enemy chance-to-hit, which comes from attributes, condition and weapon handling.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed percentage-point modifier: positive values make enemies more accurate, negative values less accurate. The loader parses the element with an integer parser, so only the integer part of a decimal value is stored.
- **Values:** `-100..100` per the table comment; the loader stores whole numbers only.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyMilitia`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH aim-difficulty modifier for militia: it is added to the aim part of the militia chance-to-hit.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed percentage-point modifier: positive values make militia more accurate with aim clicks, negative values less accurate. The loader parses the element with an integer parser, so only the integer part of a decimal value is stored.
- **Values:** `-100..100` per the table comment; the loader stores whole numbers only.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyMilitia`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH base-difficulty modifier for militia: it is added to the base part of the militia chance-to-hit.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed percentage-point modifier: positive values make militia more accurate, negative values less accurate. The loader parses the element with an integer parser, so only the integer part of a decimal value is stored.
- **Values:** `-100..100` per the table comment; the loader stores whole numbers only.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyPlayer`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH aim-difficulty modifier for player mercenaries. The full value applies at game progress `0` and fades linearly to nothing at progress `30`.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed percentage-point modifier: positive values make the player more accurate with aim clicks, negative values less accurate. The loader parses the element with an integer parser, so only the integer part of a decimal value is stored.
- **Values:** `-100..100` per the table comment; the loader stores whole numbers only.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyPlayer`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** NCTH base-difficulty modifier for player mercenaries. The full value applies at game progress `0` and fades linearly to nothing at progress `30`.
- **Only used with:** NCTH.
- **Numeric semantics:** Signed percentage-point modifier: positive values make the player more accurate, negative values less accurate. The loader parses the element with an integer parser, so only the integer part of a decimal value is stored.
- **Values:** `-100..100` per the table comment; the loader stores whole numbers only.
- **Default:** `0` in the first record; a later record keeps the previous record's value when the element is omitted.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

## Relations

- This table is selected by the campaign difficulty setting; it does not reference item or profile rows.
- The `CthConstants*` fields are NCTH constants and are relevant only when NCTH is active.
- Difficulty level `0` is a placeholder; campaign setup uses levels `1..4`.

## Localization

`Name` and `ConfirmText` are display text; numeric settings are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Difficulty table data.
- Difficulty schema comments.
- Game code (difficulty loader, strategic AI, campaign initialisation, NCTH chance-to-hit, creature spreading and soldier creation).
- Field types are the types of the runtime `DIFFICULTY_SETTINGS_VALUES` fields and of the loader casts; descriptions follow the code that consumes each value.

## Open questions

- `BloodcatAmbushSectors` is loaded into a boolean that no other code reads; the table comment only says `?`. Its intended behaviour is unknown.
- `CreaturePopulationModifier` is loaded and saved but not used by any population formula in this build; whether it was meant to feed the spreading chance is unknown.
- The seven `Sector*` fields are dead in this build: `BuildUndergroundSectorInfoList` always returns after the `initunderground.lua` path, so the hardcoded setup that reads them never runs. Whether that fallback was meant to remain reachable is unknown.
- The loader's read list uses `SectorK4B2NumElites`/`SectorO3B2NumElites` while the shipped data and the assignment cases use `SectorK4B1NumElites`/`SectorO3B1NumElites`; whether the names were meant to match is unknown.
- The exact interaction between difficulty levels and the hardcoded four-level logic is outside this table; the comment warns that new entries must not be created.
