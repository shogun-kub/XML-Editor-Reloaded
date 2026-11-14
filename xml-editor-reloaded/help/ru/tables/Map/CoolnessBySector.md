---
schema: 1
lang: ru
family: map-coolness-by-sector
title: "CoolnessBySector — карта крутости по секторам"
tables:
  - CoolnessBySector
table_ids:
  - Map.CoolnessBySector
files:
  - Map/CoolnessBySector.xml
root: COOLNESS_BY_SECTOR
record: MAP_ROW
key: MAP_ROW
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# CoolnessBySector — карта крутости по секторам

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`COOLNESS_BY_SECTOR/MAP_ROW`](#coolness_by_sectormap_row)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`CoolnessBySector.xml` — матрица крутости секторов 16×16. Крутость управляет качеством и уровнем
снаряжения, доступного в этом секторе; каждая строка соответствует строке карты, а каждая позиция —
столбцу карты.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `CoolnessBySector` | `Map/CoolnessBySector.xml` | — |

## Structure

```
COOLNESS_BY_SECTOR
└── MAP_ROW
```

## Fields

### `COOLNESS_BY_SECTOR/MAP_ROW`

- **Type:** `String` (`CHAR8`)
- **Description:** Одна строка матрицы крутости секторов. Атрибут строки — это строка карты
  (`01`.`16`); 16 значений — уровни крутости для столбцов 1.16.
- **Numeric semantics:** больше = выше крутость; отрицательные значения не имеют смысла.
- **Values:** 16 разделённых пробелами уровней крутости; наблюдаемый диапазон `1..20`.
- **Default:** Обязательно; одна строка карты.
- **Source:** game code (coolness-by-sector loader); map table data.
- **Confidence:** `source`

## Relations

- Каждая позиция матрицы использует обычные строку и столбец карты; значение управляет крутостью
  предметов [Items](../Items/Items.md), доступных в этом секторе.

## Localization

Отображаемые имена, подсказки и описания локализуются через языковые оверлеи; числовые ID, коды
секторов и значения матриц — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблиц карты.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания основаны на именах полей и наблюдаемых значениях таблиц; формула времени выполнения не
  заявляется.

## Open questions

- Точная формула или сопоставление перечисления для полей, помеченных как выведенные, в данных
  таблицы не указаны.
