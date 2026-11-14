---
schema: 1
lang: ru
family: army-patrol-groups
title: "PatrolGroups — маршруты армейских патрулей"
tables:
  - PatrolGroups
table_ids:
  - Army.PatrolGroups
files:
  - Army/PatrolGroups.xml
root: PATROL_INFO
record: PATROL
key: none
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# PatrolGroups — маршруты армейских патрулей

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`PATROL_INFO/PATROL/Priority`](#patrol_infopatrolpriority)
  - [`PATROL_INFO/PATROL/Size`](#patrol_infopatrolsize)
  - [`PATROL_INFO/PATROL/Waypoints/Sector1`](#patrol_infopatrolwaypointssector1)
  - [`PATROL_INFO/PATROL/Waypoints/Sector2`](#patrol_infopatrolwaypointssector2)
  - [`PATROL_INFO/PATROL/Waypoints/Sector3`](#patrol_infopatrolwaypointssector3)
  - [`PATROL_INFO/PATROL/Waypoints/Sector4`](#patrol_infopatrolwaypointssector4)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`PatrolGroups.xml` определяет армейские патрульные группы по приоритету, размеру и до четырёх
секторов путевых точек. Патруль проходит свои перечисленные путевые точки по порядку;
неиспользуемые путевые точки остаются пустыми или равны `0`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `PatrolGroups` | `Army/PatrolGroups.xml` | — |

## Structure

```
PATROL_INFO
└── PATROL
    ├── Priority
    ├── Size
    └── Waypoints
        ├── Sector1
        ├── Sector2
        ├── Sector3
        └── Sector4
```

## Fields

### `PATROL_INFO/PATROL/Priority`

- **Type:** `Integer` (`INT8`)
- **Description:** Приоритет патруля: насколько важна патрульная группа для Королевы.
- **Numeric semantics:** больше = важнее; более важные патрули получают больше шансов на
  усиление/улучшение. Комментарий XML задаёт предполагаемый диапазон `0..100`.
- **Values:** `0..100`; загрузчик сохраняет это поле как `INT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (patrol loader); table XML comment.
- **Confidence:** `source`

### `PATROL_INFO/PATROL/Size`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое количество солдат в патруле. Фактический размер группы — это значение
  плюс или минус один, затем ограниченный пределами размера группы из INI.
- **Numeric semantics:** больше = больше патруль; базовое значение изменяется на ±1 перед
  ограничением.
- **Values:** Целочисленный базовый размер; загрузчик сохраняет это поле как `INT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (patrol loader); table XML comment.
- **Confidence:** `source`

### `PATROL_INFO/PATROL/Waypoints/Sector1`

- **Type:** `String` (`CHAR8`)
- **Description:** Сектор путевой точки патруля в форме `A1..P16`. Буквальное значение `0` явно
  помечает путевую точку как неиспользуемую; пустое или некорректное значение — ошибка разбора, а не
  неиспользуемая путевая точка.
- **Values:** `0` или сектор, например `A1`, `B5`, `P16`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (patrol loader); table data.
- **Confidence:** `source`

### `PATROL_INFO/PATROL/Waypoints/Sector2`

- **Type:** `String` (`CHAR8`)
- **Description:** Сектор путевой точки патруля в форме `A1..P16`. Буквальное значение `0` явно
  помечает путевую точку как неиспользуемую; пустое или некорректное значение — ошибка разбора, а не
  неиспользуемая путевая точка.
- **Values:** `0` или сектор, например `A1`, `B5`, `P16`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (patrol loader); table data.
- **Confidence:** `source`

### `PATROL_INFO/PATROL/Waypoints/Sector3`

- **Type:** `String` (`CHAR8`)
- **Description:** Сектор путевой точки патруля в форме `A1..P16`. Буквальное значение `0` явно
  помечает путевую точку как неиспользуемую; пустое или некорректное значение — ошибка разбора, а не
  неиспользуемая путевая точка.
- **Values:** `0` или сектор, например `A1`, `B5`, `P16`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (patrol loader); table data.
- **Confidence:** `source`

### `PATROL_INFO/PATROL/Waypoints/Sector4`

- **Type:** `String` (`CHAR8`)
- **Description:** Сектор путевой точки патруля в форме `A1..P16`. Буквальное значение `0` явно
  помечает путевую точку как неиспользуемую; пустое или некорректное значение — ошибка разбора, а не
  неиспользуемая путевая точка.
- **Values:** `0` или сектор, например `A1`, `B5`, `P16`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (patrol loader); table data.
- **Confidence:** `source`

## Relations

- Составы патрулей используют [ArmyComposition](ArmyComposition.md); солдаты берутся из
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
