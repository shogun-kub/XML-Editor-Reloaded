---
schema: 1
lang: ru
family: random-stats
title: "RandomStats — случайные характеристики по профилям"
tables:
  - RandomStats
table_ids:
  - RandomStats
files:
  - RandomStats.xml
root: RANDOM_STATS
record: PROFILE
key: uiIndex
sources:
  - Random stats table data
  - RandomStats schema comments
status: reviewed
updated: 2026-11-12
---

# RandomStats — случайные характеристики по профилям

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`RANDOM_STATS/PROFILE/uiIndex`](#random_statsprofileuiindex)
  - [`RANDOM_STATS/PROFILE/Name`](#random_statsprofilename)
  - [`RANDOM_STATS/PROFILE/Enabled`](#random_statsprofileenabled)
  - [`RANDOM_STATS/PROFILE/BaseAttribute`](#random_statsprofilebaseattribute)
  - [`RANDOM_STATS/PROFILE/RandomExpLevel`](#random_statsprofilerandomexplevel)
  - [`RANDOM_STATS/PROFILE/RandomLife`](#random_statsprofilerandomlife)
  - [`RANDOM_STATS/PROFILE/RandomAgility`](#random_statsprofilerandomagility)
  - [`RANDOM_STATS/PROFILE/RandomDexterity`](#random_statsprofilerandomdexterity)
  - [`RANDOM_STATS/PROFILE/RandomStrength`](#random_statsprofilerandomstrength)
  - [`RANDOM_STATS/PROFILE/RandomLeadership`](#random_statsprofilerandomleadership)
  - [`RANDOM_STATS/PROFILE/RandomWisdom`](#random_statsprofilerandomwisdom)
  - [`RANDOM_STATS/PROFILE/RandomMarksmanship`](#random_statsprofilerandommarksmanship)
  - [`RANDOM_STATS/PROFILE/RandomMechanical`](#random_statsprofilerandommechanical)
  - [`RANDOM_STATS/PROFILE/RandomExplosive`](#random_statsprofilerandomexplosive)
  - [`RANDOM_STATS/PROFILE/RandomMedical`](#random_statsprofilerandommedical)
  - [`RANDOM_STATS/PROFILE/RandomScientific`](#random_statsprofilerandomscientific)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`RandomStats.xml` управляет тем, какие характеристики рандомизируются для каждого профиля, когда
`MERCS_RANDOM_STATS` равен 2 или 3. Профиль может включать рандомизацию для каждой характеристики;
`BaseAttribute` — базовое значение для режима 3.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `RandomStats` | `RandomStats.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
RANDOM_STATS
└── PROFILE
    ├── uiIndex
    ├── Name
    ├── Enabled
    ├── BaseAttribute
    ├── RandomExpLevel
    ├── RandomLife
    ├── RandomAgility
    ├── RandomDexterity
    ├── RandomStrength
    ├── RandomLeadership
    ├── RandomWisdom
    ├── RandomMarksmanship
    ├── RandomMechanical
    ├── RandomExplosive
    ├── RandomMedical
    └── RandomScientific
```

## Fields

### `RANDOM_STATS/PROFILE/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Идентификатор профиля.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/Name`

- **Type:** `String`
- **Description:** Текст имени профиля, хранящийся в таблице, но загрузчик не читает этот элемент; он
  игнорируется.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** Свободный текст; игнорируется при загрузке.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/Enabled`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, случайные характеристики включены для этого профиля.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/BaseAttribute`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовый атрибут, используемый для построения случайных характеристик;
  используется только когда `MERCS_RANDOM_STATS` равен 3.
- **Only used with:** MERCS_RANDOM_STATS 3.
- **Numeric semantics:** Большие значения означают более высокий базовый атрибут для построения
  случайной характеристики; отрицательные значения вне обычного диапазона.
- **Values:** `-128..127`; загрузчик сохраняет это поле как `INT8`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomExpLevel`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать уровень опыта.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomLife`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать здоровье.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomAgility`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать ловкость.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomDexterity`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать проворство.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomStrength`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать силу.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomLeadership`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать лидерство.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomWisdom`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать мудрость.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomMarksmanship`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать меткость.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomMechanical`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать навык механики.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomExplosive`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать навык взрывчатки.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomMedical`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать навык медицины.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

### `RANDOM_STATS/PROFILE/RandomScientific`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Рандомизировать навык науки.
- **Only used with:** MERCS_RANDOM_STATS 2 или 3.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_RandomStats.cpp`, `RANDOM_STATS_VALUES`); table data.
- **Confidence:** `source`

## Relations

- `uiIndex` указывает на [MercProfiles](MercProfiles.md).

## Localization

`Name` — метка профиля; флаги случайных характеристик не локализуются.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы случайных характеристик.
- Комментарии схемы RandomStats.
- Типы полей: данные таблицы и имена полей; значения перечислений берутся из комментариев схемы.
- Описания осторожны там, где формула не указана; наблюдаемые диапазоны значений не используются.

## Open questions

Случайный диапазон для каждой характеристики выводится из уровня профиля и выбранного режима
`MERCS_RANDOM_STATS`, а не хранится в этой таблице.
