---
schema: 1
lang: ru
family: army-composition
title: "ArmyComposition — состав армейской группы"
tables:
  - ArmyComposition
table_ids:
  - Army.ArmyComposition
files:
  - Army/ArmyComposition.xml
root: COMPOSITION_INFO
record: COMPOSITION
key: Index
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# ArmyComposition — состав армейской группы

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`COMPOSITION_INFO/COMPOSITION/Index`](#composition_infocompositionindex)
  - [`COMPOSITION_INFO/COMPOSITION/Priority`](#composition_infocompositionpriority)
  - [`COMPOSITION_INFO/COMPOSITION/ElitePercentage`](#composition_infocompositionelitepercentage)
  - [`COMPOSITION_INFO/COMPOSITION/TroopPercentage`](#composition_infocompositiontrooppercentage)
  - [`COMPOSITION_INFO/COMPOSITION/AdminPercentage`](#composition_infocompositionadminpercentage)
  - [`COMPOSITION_INFO/COMPOSITION/StartPopulation`](#composition_infocompositionstartpopulation)
  - [`COMPOSITION_INFO/COMPOSITION/DesiredPopulation`](#composition_infocompositiondesiredpopulation)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`ArmyComposition.xml` определяет смесь элитных, обычных и административных солдат в армейской
группе, её приоритет, а также начальную и целевую численность.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `ArmyComposition` | `Army/ArmyComposition.xml` | — |

## Structure

```
COMPOSITION_INFO
└── COMPOSITION
    ├── Index
    ├── Priority
    ├── ElitePercentage
    ├── TroopPercentage
    ├── AdminPercentage
    ├── StartPopulation
    └── DesiredPopulation
```

## Fields

### `COMPOSITION_INFO/COMPOSITION/Index`

- **Type:** `Integer` (`INT32`)
- **Description:** Индекс состава армии. Это ключ массива, используемый гарнизонами и патрулями.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (army composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/Priority`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Приоритет этого состава относительно остальных.
- **Numeric semantics:** больше = важнее; отрицательные значения не имеют смысла.
- **Values:** `-128..127`; загрузчик сохраняет это поле как `INT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/ElitePercentage`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Доля элитных солдат в этом составе, в процентах.
- **Numeric semantics:** больше = больше доля; загрузчик ограничивает значения ниже `0` до `0`, а
  выше `100` — до `100`.
- **Values:** Процент `0..100` после ограничения.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/TroopPercentage`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Доля обычных солдат в этом составе, в процентах.
- **Numeric semantics:** больше = больше доля; загрузчик ограничивает значения ниже `0` до `0`, а
  выше `100` — до `100`.
- **Values:** Процент `0..100` после ограничения.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/AdminPercentage`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Доля или вес административных солдат в этом составе, в процентах.
- **Numeric semantics:** больше = больше административная доля; загрузчик ограничивает значения ниже
  `0` до `0`, а выше `100` — до `100`.
- **Values:** Процент `0..100` после ограничения.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/StartPopulation`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Количество солдат в составе на начало кампании.
- **Numeric semantics:** больше = больше солдат; загрузчик ограничивает отрицательные значения до
  `1`.
- **Values:** `1..127` после ограничения; загрузчик сохраняет это поле как `INT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

### `COMPOSITION_INFO/COMPOSITION/DesiredPopulation`

- **Type:** `Integer` (`INT32`) (`INT8`)
- **Description:** Целевое количество солдат, которое состав старается поддерживать.
- **Numeric semantics:** больше = больше цель; загрузчик ограничивает отрицательные значения до `1`.
- **Values:** `1..127` после ограничения; загрузчик сохраняет это поле как `INT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (army-composition loader); table data.
- **Confidence:** `source`

## Relations

- [GarrisonGroups](GarrisonGroups.md) и [PatrolGroups](PatrolGroups.md) ссылаются на состав по его
  `Index`; получившиеся солдаты используют
  [SoldierProfiles](../Profiles/SoldierProfiles.md).

## Localization

Отображаемые имена и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, комментарии XML и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не утверждается.

## Open questions

- Точная формула или сопоставление перечисления для выведенных полей не указаны в данных таблицы.
