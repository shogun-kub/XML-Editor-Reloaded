---
schema: 1
lang: ru
family: difficulty
title: "DifficultySettings — сложность кампании"
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
updated: 2026-11-12
---

# DifficultySettings — сложность кампании

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

`DifficultySettings.xml` задаёт уровни сложности кампании: стартовые деньги, параметры ОД и групп врагов, экономику королевы, поведение при тревоге, подкрепления, константы шанса попадания и стартовые силы секторов.

Загрузчик держит одну рабочую запись на весь файл и при закрытии записи копирует её в слот, указанный в `uiIndex`. Поэтому элемент, пропущенный в записи `DIFFICULTY`, сохраняет значение из предыдущей записи (или `0` для первой записи); отсюда же следует, что `uiIndex` должен присутствовать в каждой записи, иначе эта запись перезапишет предыдущий слот. Все числовые элементы разбираются целочисленным анализатором, и сохраняется только тот младший байт или слово, которые принимает поле времени выполнения, поэтому некоторые фактические диапазоны оказываются гораздо уже, чем предполагает знаковое значение.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `DifficultySettings` | `DifficultySettings.xml` | В v4 и v5 одинаковые пути полей; значения различаются. |

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
- **Description:** Индекс уровня сложности. Он выбирает запись сложности и является ключом массива; элемент `0` — заглушка, а уровни `1..4` — игровые уровни Novice, Experienced, Expert и Insane.
- **Values:** `0..4`; в массиве `MAX_DIF_LEVEL` (`5`) слотов, и комментарий таблицы предупреждает, что эти четыре исходных уровня по-прежнему захардкожены.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; difficulty-level enum); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Короткое название уровня сложности. Оно преобразуется из UTF-8 в буфер фиксированной длины 128 символов, поэтому более длинный текст обрезается.
- **Values:** Свободный текст.
- **Default:** Пустая строка в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/ConfirmText`

- **Type:** `String` (`CHAR16`)
- **Description:** Текст подтверждения, показываемый для уровня сложности. Он преобразуется из UTF-8 в буфер фиксированной длины 1024 символа, поэтому более длинный текст обрезается.
- **Values:** Свободный текст.
- **Default:** Пустая строка в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/StartingCash`

- **Type:** `Integer` (`INT32`)
- **Description:** Деньги, добавляемые на счёт игрока при старте кампании. Комментарий таблицы указывает, что отрицательные значения допустимы.
- **Numeric semantics:** Большие значения означают больше стартовых денег; отрицательные значения начинают кампанию с долгом.
- **Values:** `-2147483648..2147483647`; загрузчик хранит это поле как `INT32`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; campaign-start deposit); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyAPBonus`

- **Type:** `Integer` (`INT8`)
- **Description:** Дополнительные очки действия для вражеских солдат. Комментарий таблицы отмечает, что значение не корректируется под выбранную систему ОД.
- **Numeric semantics:** Знаковый модификатор: положительные значения добавляют врагам ОД, отрицательные — отнимают, а `0` нейтрально.
- **Values:** `-128..127`; загрузчик хранит это поле как `INT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; soldier AP calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/NumKillsPerProgressPoint`

- **Type:** `Integer` (`UINT16`)
- **Description:** Число убийств, необходимых для получения одного очка прогресса. Комментарий таблицы добавляет, что общее число очков прогресса также ограничено параметром `GAME_PROGRESS_KILLS` в `Ja2_Options.ini`.
- **Numeric semantics:** Большие значения означают, что на одно очко прогресса нужно больше убийств; игра использует значение как делитель, поэтому во время выполнения оно должно быть не меньше `1`.
- **Values:** `0..65535`; загрузчик хранит это поле как `UINT16`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; progress calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/InitialGarrisonPercentages`

- **Type:** `Integer` (`INT32`)
- **Description:** Процент, применяемый к размерам стартовых армейских групп, целевой численности населения и размерам патрулей. Комментарий таблицы приводит `100` — размещаются все войска по умолчанию, `200` — вдвое больше.
- **Numeric semantics:** Большие значения означают больше стартовых вражеских войск и крупнее гарнизоны; отрицательные значения не имеют смысла.
- **Values:** `-2147483648..2147483647`; загрузчик хранит это поле как `INT32`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI campaign initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/MinEnemyGroupSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Нижняя граница размеров вражеских групп. Стратегический ИИ также использует удвоенное значение как обычный размер группы.
- **Numeric semantics:** Большие значения означают больший минимальный размер вражеских групп; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI group sizing); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CounterAttackGroupSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Размер каждой из четырёх групп контратаки, атакующих захваченный город. Стратегический ИИ повышает значение до `MinEnemyGroupSize`, если оно больше.
- **Numeric semantics:** Большие значения означают большие группы контратаки; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; counter-attack creation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/PercentElitesBonus`

- **Type:** `Integer` (`INT32`)
- **Description:** Процентные пункты, переносимые из доли обычных солдат в долю элитных в составе каждой армии. Применяются только положительные значения.
- **Numeric semantics:** Большие значения означают больше элитных бойцов во вражеских группах; `0` и отрицательные значения оставляют состав без изменений.
- **Values:** `-2147483648..2147483647`; загрузчик хранит это поле как `INT32`, но использует его только когда оно больше `0`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; army composition setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UnlimitedPoolOfTroops`

- **Type:** `Boolean`
- **Description:** Если включено, у королевы неограниченные войска, а четыре параметра `QueenPool*` игнорируются.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI reinforcement pool); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueensInitialPoolOfTroops`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер резерва королевы на старте кампании; часть его выдаётся стартовым гарнизонам и патрулям, остальное остаётся в резерве.
- **Numeric semantics:** Большие значения означают больший стартовый резерв; отрицательные значения загрузчик ограничивает до `0`.
- **Values:** `0..2147483647`; отрицательный ввод ограничивается до `0`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI reinforcement pool); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolMaxSizePerDifficultyLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Максимальный размер, до которого можно обучить резерв королевы.
- **Numeric semantics:** Большие значения означают больший допустимый резерв; отрицательные значения загрузчик ограничивает до `0`.
- **Values:** `0..2147483647`; отрицательный ввод ограничивается до `0`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic event `EVENT_ARMY_FINISH_TRAINING`); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolIncrementDaysPerDifficultyLevel`

- **Type:** `Integer` (`UINT16`)
- **Description:** Интервал в днях между партиями обучения новобранцев. `0` выбирает старую простую схему, в которой резерв пополняется, как только опускается ниже `QueenPoolBaseIncrementSizePerDifficultyLevel`.
- **Numeric semantics:** Большие значения означают более длительные интервалы между партиями обучения; `0` имеет особое значение старой схемы.
- **Values:** `0..65535`; загрузчик хранит это поле как `UINT16`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic event handler); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolBaseIncrementSizePerDifficultyLevel`

- **Type:** `Integer` (`INT32`)
- **Description:** Ёмкость одного учебного лагеря, а в старой простой схеме — ещё и порог резерва и величина пополнения. Комментарий таблицы предупреждает, что очень большие числа могут вызвать переполнение.
- **Numeric semantics:** Большие значения означают большие партии обучения и большее пополнение по старой схеме; отрицательные значения загрузчик ограничивает до `0`.
- **Values:** `0..2147483647`; отрицательный ввод ограничивается до `0`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI reinforcement pool); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenPoolRecruitPercentPerDifficultyLevel`

- **Type:** `Integer` (`UINT8`)
- **Description:** Процент населения сектора, который армия забирает в новобранцы каждый день.
- **Numeric semantics:** Большие значения означают более быстрый набор новобранцев в городах; отрицательные значения не имеют смысла.
- **Values:** `0..100`; загрузчик хранит это поле как `UINT8` и ограничивает его до `100`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic event handler); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyStartingAlertLevel`

- **Type:** `Integer` (`UINT32`)
- **Description:** Стартовая бдительность армии: процентная вероятность того, что вражеская группа обнаружит соседнюю группу игрока или ополчения.
- **Numeric semantics:** Большие значения означают более высокий шанс обнаружения; значения выше `100` делают проверку всегда успешной.
- **Values:** `0..4294967295`; загрузчик хранит это поле как `UINT32`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI spotting checks); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyAlertDecay`

- **Type:** `Integer` (`UINT32`)
- **Description:** Величина, вычитаемая из процента бдительности армии при каждой успешной проверке обнаружения. Комментарий таблицы рекомендует `1..100` и практический диапазон `20..60`.
- **Numeric semantics:** Большие значения означают, что после успешного обнаружения бдительность падает быстрее; отрицательные значения не имеют смысла.
- **Values:** `0..4294967295`; загрузчик хранит это поле как `UINT32`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI spotting checks); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/NumAwareBattles`

- **Type:** `Integer` (`UINT32`)
- **Description:** Число инициированных врагом боёв, в течение которых королева остаётся в режиме полной тревоги; каждый такой бой уменьшает счётчик. Стратегический ИИ хранит счётчик в байте, поэтому сохраняются только `0..255`.
- **Numeric semantics:** Большие значения означают, что королева остаётся настороже дольше; отрицательные значения не имеют смысла.
- **Values:** `0..4294967295`; фактический диапазон во время выполнения `0..255`, потому что стратегический ИИ хранит его как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic movement/alert logic); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/BaseDelayInMinutesBetweenEvaluations`

- **Type:** `Integer` (`UINT32`)
- **Description:** Базовый интервал в минутах между стратегическими решениями королевы.
- **Numeric semantics:** Большие значения означают более медленные решения королевы; отрицательные значения не имеют смысла.
- **Values:** `0..4294967295`; загрузчик хранит это поле как `UINT32`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; queen evaluation scheduling); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EvaluationDelayVariance`

- **Type:** `Integer` (`UINT32`)
- **Description:** Случайная дополнительная задержка, добавляемая к базовому интервалу между решениями королевы: игра добавляет значение от `0` до этого значения минус один, в минутах.
- **Numeric semantics:** Большие значения означают больший разброс времени решений; отрицательные значения не имеют смысла.
- **Values:** `0..4294967295`; загрузчик хранит это поле как `UINT32`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; queen evaluation scheduling); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/GracePeriodInHoursAfterSectorLiberation`

- **Type:** `Integer` (`UINT32`)
- **Description:** Задержка в часах между освобождением сектора игроком и моментом, когда королеве разрешено решить атаковать его. Стратегический ИИ хранит это значение в байте, поэтому сохраняются только `0..255`.
- **Numeric semantics:** Большие значения означают более длительный льготный период и более лёгкую игру; отрицательные значения не имеют смысла.
- **Values:** `0..4294967295`; фактический диапазон во время выполнения `0..255`, потому что стратегический ИИ хранит его как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI grace-period check); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/GracePeriodInDaysAfterPatrolDestroyed`

- **Type:** `Integer` (`UINT32`)
- **Description:** Задержка в днях между уничтожением вражеского патруля игроком и моментом, когда королеве разрешено восполнить его; игра хранит получившийся день по модулю `100`.
- **Numeric semantics:** Большие значения означают, что разбитые патрули появляются реже; отрицательные значения не имеют смысла.
- **Values:** `0..4294967295`; день восполнения — это номер дня по модулю `100`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; patrol refill scheduling); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AggressiveQueenAi`

- **Type:** `Boolean`
- **Description:** Если включено, королева предпочитает атаковать сектора игрока, а не защищать свои.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; queen AI aggression flag); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/MaxMercDeaths`

- **Type:** `Integer` (`INT32`)
- **Description:** Максимальное число ещё не нанятых наёмников, которые могут погибнуть при ежедневном броске на смерть за кадром, применяемом к наёмникам, работающим в других местах. Комментарий таблицы приводит `0..10`.
- **Numeric semantics:** Большие значения означают больше смертей наёмников за кадром; отрицательные значения не имеют смысла.
- **Values:** `-2147483648..2147483647`; загрузчик хранит это поле как `INT32`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; unhired-merc death handler); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/LootStatusModifier`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальный случайный урон, применяемый к снаряжению, выпадающему из убитых врагов: игра вычитает из состояния предмета случайное значение от `0` до этого значения минус один. Комментарий таблицы приводит `0..100`.
- **Numeric semantics:** Большие значения означают более повреждённые трофеи; отрицательное записанное значение из-за приведения типа в загрузчике переходит в верхнюю часть диапазона байта.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; loot status reduction); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CreatureSpreadTime`

- **Type:** `Integer` (`UINT16`)
- **Description:** Период в минутах между распространениями существ, когда существа уже активны. Комментарий таблицы приводит `5..14400`.
- **Numeric semantics:** Большие значения означают более медленное распространение; отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик хранит это поле как `UINT16`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; creature spread event); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenReproductionBase`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число существ, производимых при каждом распространении, до случайного бонуса. Комментарий таблицы приводит `1..30`.
- **Numeric semantics:** Большие значения означают более быстрый рост численности существ; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; creature reproduction); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenReproductionBonus`

- **Type:** `Integer` (`UINT8`)
- **Description:** Верхняя граница случайного бонуса, добавляемого при каждом распространении; игра добавляет значение от `0` до этого значения. Комментарий таблицы приводит `0..10`.
- **Numeric semantics:** Большие значения означают больший случайный бонус и более быстрый рост; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; creature reproduction); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenInitBonusSpread`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число немедленных распространений при инициализации логова существ, дающее существам форы. Комментарий таблицы приводит `1..10`.
- **Numeric semantics:** Большие значения означают большую начальную фору; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; creature lair initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CreaturePopulationModifier`

- **Type:** `Integer` (`INT8`)
- **Description:** Предполагаемый знаковый модификатор вероятности заселения сектора, согласно комментарию таблицы. Текущий код копирует его в переменную кампании, которая сохраняется и загружается, но ни одна формула заселения его не читает, поэтому изменение значения в этой сборке ни на что не влияет.
- **Numeric semantics:** Не подтверждено; значение не читается ни одним игровым кодом в этой сборке, поэтому направление влияния не утверждается.
- **Values:** `-128..127`; комментарий таблицы рекомендует `-5..5`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; creature spreading); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CreatureTownAggressiveness`

- **Type:** `Integer` (`INT8`)
- **Description:** Поправка к вероятности нападения существ на города. Группа из `n` существ нападает, когда `10 * n` плюс это значение превышает случайное число от `0` до `99`. Комментарий таблицы приводит `-100..100`.
- **Numeric semantics:** Большие значения означают более вероятные нападения на города; отрицательные значения делают их менее вероятными.
- **Values:** `-128..127`; загрузчик хранит это поле как `INT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; creature town-attack check); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorJ9B1NumTroops`

- **Type:** `Integer` (`UINT8`)
- **Description:** Предполагаемое число вражеских солдат для подземного сектора J9 B1. Захардкоженная подземная инициализация, которая его читает, недостижима в этой сборке: `BuildUndergroundSectorInfoList` немедленно завершается после того, как скрипт `initunderground.lua` строит список секторов, а поставляемый скрипт задаёт численность из своих таблиц. Поэтому значение ни на что не влияет.
- **Numeric semantics:** Большие значения означали бы больше стартовых вражеских солдат, но в этой сборке значение не применяется.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`, но ни один достижимый код его не использует.
- **Default:** В этой сборке эффекта нет.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorJ9B2NumCreatures`

- **Type:** `Integer` (`UINT8`)
- **Description:** Предполагаемое число существ для зоны кормления J9 B2, `2 + 2 * value + Random(2)`. Захардкоженная подземная инициализация, которая его читает, недостижима в этой сборке: путь Lua `initunderground.lua` завершается раньше, а поставляемый скрипт задаёт численность из своих таблиц. Поэтому значение ни на что не влияет.
- **Numeric semantics:** Большие значения означали бы больше стартовых существ, но в этой сборке значение не применяется.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`, но ни один достижимый код его не использует.
- **Default:** В этой сборке эффекта нет.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorK4B1NumTroops`

- **Type:** `Integer` (`UINT8`)
- **Description:** Предполагаемое число вражеских солдат для K4 B1, `6 + 2 * value + Random(3)`. Захардкоженная подземная инициализация, которая его читает, недостижима в этой сборке: путь Lua `initunderground.lua` завершается раньше, а поставляемый скрипт задаёт численность из своих таблиц. Поэтому значение ни на что не влияет.
- **Numeric semantics:** Большие значения означали бы больше стартовых вражеских солдат, но в этой сборке значение не применяется.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`, но ни один достижимый код его не использует.
- **Default:** В этой сборке эффекта нет.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorK4B1NumElites`

- **Type:** `Integer` (`UINT8`)
- **Description:** Предполагаемое число элитных бойцов для K4 B1, `4 + value + Random(2)`. Значение не работает из-за двух дефектов: список чтения загрузчика называет `SectorK4B2NumElites` вместо этого элемента, поэтому поставляемый элемент `SectorK4B1NumElites` пропускается, а захардкоженная подземная инициализация, которая применила бы его, недостижима, потому что путь Lua `initunderground.lua` завершается раньше. Значение остаётся `0` и ни на что не влияет.
- **Numeric semantics:** Большие значения означали бы больше стартовых элитных бойцов, но в этой сборке значение не читается и не применяется.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`, но в поставляемом загрузчике элемент не читается.
- **Default:** Всегда `0` с поставляемым загрузчиком, и в этой сборке эффекта нет.
- **Source:** game code (difficulty loader read list; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorO3B1NumTroops`

- **Type:** `Integer` (`UINT8`)
- **Description:** Предполагаемое число вражеских солдат для O3 B1, `6 + 2 * value + Random(3)`. Захардкоженная подземная инициализация, которая его читает, недостижима в этой сборке: путь Lua `initunderground.lua` завершается раньше, а поставляемый скрипт задаёт численность из своих таблиц. Поэтому значение ни на что не влияет.
- **Numeric semantics:** Большие значения означали бы больше стартовых вражеских солдат, но в этой сборке значение не применяется.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`, но ни один достижимый код его не использует.
- **Default:** В этой сборке эффекта нет.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorO3B1NumElites`

- **Type:** `Integer` (`UINT8`)
- **Description:** Предполагаемое число элитных бойцов для O3 B1, `4 + value + Random(2)`. Значение не работает из-за двух дефектов: список чтения загрузчика называет `SectorO3B2NumElites` вместо этого элемента, поэтому поставляемый элемент `SectorO3B1NumElites` пропускается, а захардкоженная подземная инициализация, которая применила бы его, недостижима, потому что путь Lua `initunderground.lua` завершается раньше. Значение остаётся `0` и ни на что не влияет.
- **Numeric semantics:** Большие значения означали бы больше стартовых элитных бойцов, но в этой сборке значение не читается и не применяется.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`, но в поставляемом загрузчике элемент не читается.
- **Default:** Всегда `0` с поставляемым загрузчиком, и в этой сборке эффекта нет.
- **Source:** game code (difficulty loader read list; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/SectorP3B1NumElites`

- **Type:** `Integer` (`UINT8`)
- **Description:** Предполагаемое число элитных бойцов для P3 B1: игра добавляла бы `Random(3)` на Novice, `Random(5)` на Experienced, `Random(6)` на Expert и ничего на Insane. Захардкоженная подземная инициализация, которая его читает, недостижима в этой сборке: путь Lua `initunderground.lua` завершается раньше, а поставляемый скрипт задаёт численность из своих таблиц. Поэтому значение ни на что не влияет.
- **Numeric semantics:** Большие значения означали бы больше стартовых элитных бойцов, но в этой сборке значение не применяется.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`, но ни один достижимый код его не использует.
- **Default:** В этой сборке эффекта нет.
- **Source:** game code (difficulty loader; unreachable hardcoded underground initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/StrategicAiActionWakeQueen`

- **Type:** `Boolean`
- **Description:** Если включено, королева просыпается сразу после окончания первого боя игрока, а не по обычному графику пробуждения.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; meanwhile event handler); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UpdateLastDayOfPlayerActivity`

- **Type:** `Integer` (`UINT16`)
- **Description:** Число дней, добавляемых к текущему игровому дню, когда игрок впервые захватывает сектор; результат записывается как последний день активности игрока.
- **Numeric semantics:** Большие значения сдвигают записанный день активности дальше в будущее; отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик хранит это поле как `UINT16`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; player-command handling of a captured sector); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/ChanceOfEnemyAmbushes`

- **Type:** `Integer` (`INT32`)
- **Description:** Модификатор в процентных пунктах, добавляемый к вероятности вражеской засады, когда игрок входит в сектор.
- **Numeric semantics:** Знаковый модификатор: положительные значения повышают вероятность засад, отрицательные — понижают, а `0` нейтрально.
- **Values:** `-2147483648..2147483647`; загрузчик хранит это поле как `INT32`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; pre-battle ambush chance); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AllowReinforcements`

- **Type:** `Boolean`
- **Description:** Если включено, враг может отправлять подкрепления в сектор.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; reinforcement system flag); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AllowReinforcementsOmerta`

- **Type:** `Boolean`
- **Description:** Если включено, враг может также отправлять подкрепления в Омерту.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; reinforcement system flag); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/PopulationLevel2`

- **Type:** `Boolean`
- **Description:** Если включено, население обороны периметра второго уровня (армейские группы в двух секторах от Медуны) сохраняет свои войска; если выключено, это население обнуляется на старте кампании.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI campaign initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/PopulationLevel3`

- **Type:** `Boolean`
- **Description:** Если включено, население обороны периметра третьего уровня (армейские группы в трёх секторах от Медуны) сохраняет свои войска; если выключено, это население обнуляется на старте кампании.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI campaign initialisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops1`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число солдат для первой группы у склада оружия; игра задаёт группе значение `6 + 2 * value`.
- **Numeric semantics:** Большие значения означают больше солдат у склада; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops2`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число солдат для второй группы у склада оружия; игра задаёт группе значение `6 + 2 * value`.
- **Numeric semantics:** Большие значения означают больше солдат у склада; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops3`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число солдат для третьей группы у склада оружия; игра задаёт группе значение `6 + 2 * value`.
- **Numeric semantics:** Большие значения означают больше солдат у склада; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops4`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число солдат для четвёртой группы у склада оружия; игра задаёт группе значение `6 + 2 * value`.
- **Numeric semantics:** Большие значения означают больше солдат у склада; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/WeaponCacheTroops5`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число солдат для пятой группы у склада оружия; игра задаёт группе значение `6 + 2 * value`.
- **Numeric semantics:** Большие значения означают больше солдат у склада; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; weapon-cache group setup); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UpgradeAdminsToTroops`

- **Type:** `Boolean`
- **Description:** Если включено, ИИ может повышать администраторов до обычных солдат в движущихся вражеских группах.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI admin upgrade); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/UpgradeGarrisonsAdminsToTroops`

- **Type:** `Boolean`
- **Description:** Если включено, ИИ может повышать администраторов до обычных солдат в гарнизонах.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI admin upgrade); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AlwaysUpGradeAdminsToTroopsProgress`

- **Type:** `Integer` (`UINT8`)
- **Description:** Процент прогресса игрока, при котором всех администраторов можно повысить независимо от двух переключателей `Upgrade*AdminsToTroops`.
- **Numeric semantics:** Большие значения откладывают автоматическое повышение до более позднего прогресса; отрицательные значения не имеют смысла.
- **Values:** `0..100`; загрузчик хранит это поле как `UINT8` и ограничивает его до `100`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI admin upgrade); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/LevelModifierLowLimit`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число уровней, на которое случайный уровень бойца может оказаться ниже уровня, заданного картой/прогрессом. Оба предела на `0` воспроизводят неизменённые уровни.
- **Numeric semantics:** Большие значения допускают более низкие случайные уровни; отрицательные значения не имеют смысла.
- **Values:** `0..5`; загрузчик хранит это поле как `UINT8` и ограничивает его до `5`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; soldier level randomisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/LevelModifierHighLimit`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число уровней, на которое случайный уровень бойца может оказаться выше уровня, заданного картой/прогрессом. Оба предела на `0` воспроизводят неизменённые уровни.
- **Numeric semantics:** Большие значения допускают более высокие случайные уровни; отрицательные значения не имеют смысла.
- **Values:** `0..5`; загрузчик хранит это поле как `UINT8` и ограничивает его до `5`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; soldier level randomisation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AllowUnrestrictedXPLevels`

- **Type:** `Boolean`
- **Description:** Если включено, предел уровня опыта для уровня сложности игнорируется, поэтому бойцы на более лёгких сложностях могут достичь уровня `10`. Комментарий таблицы отмечает, что на Insane предела и так нет.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; soldier creation level cap); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/QueenAttackLosingControlOfSector`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число раз, которое игрок может захватить сектор города, прежде чем королева перестанет прощупывать его и атаковать; `0` допускает неограниченные атаки.
- **Numeric semantics:** Большие значения позволяют королеве атаковать дольше; отрицательные значения не имеют смысла.
- **Values:** `0..255`; поле времени выполнения — `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; strategic AI sector-investigation check); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/BloodcatAmbushSectors`

- **Type:** `Boolean`
- **Description:** Элемент-флаг без подтверждённого смысла: загрузчик сохраняет его, но в этой сборке ни один игровой код его не читает, а комментарий таблицы оставляет его назначение как `?`.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; no reader found); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/AirRaidLookForDive`

- **Type:** `Boolean`
- **Description:** Если включено, скриптовый авианалёт в начале игры использует случайный выбор между пикированием и бомбометанием; если выключено, налёт идёт по фиксированной последовательности: пикирование, бомбометание, пикирование.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; air-raid dive selection); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/NumberOfTurnsPowerGenFanWillBeStoppedFor`

- **Type:** `Integer` (`UINT32`)
- **Description:** Число ходов, на которые останавливается вентилятор электрогенератора. Значение компилируется и используется только в сборке UB; комментарий таблицы помечает его как относящееся только к UB.
- **Numeric semantics:** Большие значения означают, что вентилятор остаётся выключенным дольше; отрицательные значения не имеют смысла.
- **Values:** `0..4294967295`; загрузчик хранит это поле как `UINT32`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; UB tactical code); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/RadioSightings`

- **Type:** `Boolean`
- **Description:** Если включено, вражеский солдат, обнаруживший цель, вызывает помощь по рации.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; enemy sighting/radio logic); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/RadioSightings2`

- **Type:** `Boolean`
- **Description:** Если включено, помощь по рации вызывает каждый вражеский солдат, а не только обнаружившие цель.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; enemy sighting/radio logic); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/EnemyMoraleWorried`

- **Type:** `Boolean`
- **Description:** Если включено, боевой дух врагов не может опуститься ниже категории `worried`; результат «безнадёжно» поднимается до категории `worried`.
- **Values:** `0` или `1`.
- **Default:** `0` (false) в первой записи; последующая запись сохраняет предыдущее значение, если элемент отсутствует.
- **Source:** game code (difficulty loader; morale category calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/MaxMortarsPerTeam`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число солдат ИИ в команде (вражеских или ополчения), которые могут появиться с миномётом.
- **Numeric semantics:** Большие значения означают больше миномётов на команду; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; enemy inventory choosing); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficulty`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Модификатор сложности прицеливания NCTH для вражеских бойцов: он добавляется к части шанса попадания врага, отвечающей за прицеливание.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый модификатор в процентных пунктах: положительные значения повышают точность врагов при прицеливании, отрицательные — понижают. Загрузчик разбирает элемент целочисленным анализатором, поэтому сохраняется только целая часть десятичного значения.
- **Values:** `-100..100` согласно комментарию таблицы; загрузчик сохраняет только целые числа.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficulty`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Модификатор базовой сложности NCTH для вражеских бойцов: он добавляется к базовой части шанса попадания врага, которая складывается из характеристик, состояния и обращения с оружием.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый модификатор в процентных пунктах: положительные значения повышают точность врагов, отрицательные — понижают. Загрузчик разбирает элемент целочисленным анализатором, поэтому сохраняется только целая часть десятичного значения.
- **Values:** `-100..100` согласно комментарию таблицы; загрузчик сохраняет только целые числа.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyMilitia`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Модификатор сложности прицеливания NCTH для ополчения: он добавляется к части шанса попадания ополчения, отвечающей за прицеливание.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый модификатор в процентных пунктах: положительные значения повышают точность ополчения при прицеливании, отрицательные — понижают. Загрузчик разбирает элемент целочисленным анализатором, поэтому сохраняется только целая часть десятичного значения.
- **Values:** `-100..100` согласно комментарию таблицы; загрузчик сохраняет только целые числа.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyMilitia`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Модификатор базовой сложности NCTH для ополчения: он добавляется к базовой части шанса попадания ополчения.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый модификатор в процентных пунктах: положительные значения повышают точность ополчения, отрицательные — понижают. Загрузчик разбирает элемент целочисленным анализатором, поэтому сохраняется только целая часть десятичного значения.
- **Values:** `-100..100` согласно комментарию таблицы; загрузчик сохраняет только целые числа.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsAimDifficultyPlayer`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Модификатор сложности прицеливания NCTH для наёмников игрока. Полное значение действует при прогрессе игры `0` и линейно убывает до нуля при прогрессе `30`.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый модификатор в процентных пунктах: положительные значения повышают точность игрока при прицеливании, отрицательные — понижают. Загрузчик разбирает элемент целочисленным анализатором, поэтому сохраняется только целая часть десятичного значения.
- **Values:** `-100..100` согласно комментарию таблицы; загрузчик сохраняет только целые числа.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

### `DIFFICULTY_SETTINGS/DIFFICULTY/CthConstantsBaseDifficultyPlayer`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Модификатор базовой сложности NCTH для наёмников игрока. Полное значение действует при прогрессе игры `0` и линейно убывает до нуля при прогрессе `30`.
- **Only used with:** NCTH.
- **Numeric semantics:** Знаковый модификатор в процентных пунктах: положительные значения повышают точность игрока, отрицательные — понижают. Загрузчик разбирает элемент целочисленным анализатором, поэтому сохраняется только целая часть десятичного значения.
- **Values:** `-100..100` согласно комментарию таблицы; загрузчик сохраняет только целые числа.
- **Default:** `0` в первой записи; последующая запись сохраняет значение предыдущей записи, если элемент отсутствует.
- **Source:** game code (difficulty loader; NCTH chance-to-hit calculation); difficulty table data.
- **Confidence:** `source`

## Relations

- Эта таблица выбирается настройкой сложности кампании; она не ссылается на строки предметов или профилей.
- Поля `CthConstants*` — это константы NCTH, и они важны только когда NCTH активна.
- Уровень сложности `0` — заглушка; настройка кампании использует уровни `1..4`.

## Localization

`Name` и `ConfirmText` — отображаемый текст; числовые настройки не локализуются.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы сложности.
- Комментарии схемы таблицы сложности.
- Игровой код (загрузчик сложности, стратегический ИИ, инициализация кампании, шанс попадания NCTH, распространение существ и создание солдат).
- Типы полей — это типы полей времени выполнения `DIFFICULTY_SETTINGS_VALUES` и приведений в загрузчике; описания следуют коду, который использует каждое значение.

## Open questions

- `BloodcatAmbushSectors` загружается в булево значение, которое не читает никакой другой код; комментарий таблицы говорит только `?`. Его предполагаемое поведение неизвестно.
- `CreaturePopulationModifier` загружается и сохраняется, но в этой сборке не используется ни одной формулой заселения; неизвестно, должно ли оно было влиять на вероятность распространения.
- Семь полей `Sector*` в этой сборке мертвы: `BuildUndergroundSectorInfoList` всегда завершается после пути `initunderground.lua`, поэтому захардкоженная инициализация, которая их читает, никогда не выполняется. Неизвестно, должен ли был этот запасной путь оставаться достижимым.
- Список чтения загрузчика использует `SectorK4B2NumElites`/`SectorO3B2NumElites`, тогда как поставляемые данные и ветви присваивания используют `SectorK4B1NumElites`/`SectorO3B1NumElites`; неизвестно, должны ли были имена совпадать.
- Точное взаимодействие уровней сложности с захардкоженной логикой четырёх уровней выходит за рамки этой таблицы; комментарий предупреждает, что новые записи создавать нельзя.
