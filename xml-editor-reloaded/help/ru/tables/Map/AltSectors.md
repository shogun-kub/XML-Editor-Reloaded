---
schema: 1
lang: ru
family: map-alt-sectors
title: "AltSectors — карта альтернативных секторов"
tables:
  - AltSectors
table_ids:
  - Map.AltSectors
files:
  - Map/AltSectors.xml
root: ALT_SECTORS_LIST
record: ROW
key: ROW
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# AltSectors — карта альтернативных секторов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ALT_SECTORS_LIST/ROW`](#alt_sectors_listrow)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AltSectors.xml` — матрица 16×16. Каждый элемент `ROW` имеет атрибут `y` для строки карты и 16
значений, по одному на столбец карты. Ненулевое значение выбирает группу альтернативных секторов,
используемую для этого сектора.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `AltSectors` | `Map/AltSectors.xml` | — |

## Structure

```
ALT_SECTORS_LIST
└── ROW
```

## Fields

### `ALT_SECTORS_LIST/ROW`

- **Type:** `String` (`CHAR8`)
- **Description:** Одна строка матрицы альтернативных секторов. Строка берётся из атрибута `y`
  (буква `A`..`P` или число); текст элемента читается как разделённая пробелами последовательность
  числовых значений, по одному на столбец, и записывается в строку начиная со столбца 1. `0`
  означает отсутствие альтернативного сектора.
- **Numeric semantics:** Значения — это ID альтернативных секторов, поэтому большее значение не
  означает «больше»; `0` = нет.
- **Values:** до 16 числовых значений в строке; загрузчик не проверяет их количество.
- **Default:** Обязательно; одна строка карты.
- **Source:** game code (alternate-sector loader); map table data.
- **Confidence:** `source`

## Relations

- Каждая позиция матрицы использует обычные строку и столбец карты; координаты секторов общие с
  [SectorNames](SectorNames.md), а значение выбирает группу альтернативных секторов.

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
