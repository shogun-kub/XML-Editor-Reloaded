---
schema: 1
lang: ru
family: map-minerals
title: "Minerals — типы полезных ископаемых шахт"
tables:
  - Minerals
table_ids:
  - Map.Minerals
files:
  - Map/Minerals.xml
root: MINERALS
record: TYPE
key: uiIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# Minerals — типы полезных ископаемых шахт

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MINERALS/TYPE/uiIndex`](#mineralstypeuiindex)
  - [`MINERALS/TYPE/sNameType`](#mineralstypesnametype)
  - [`MINERALS/TYPE/sMineral`](#mineralstypesmineral)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Minerals.xml` задаёт названия типов полезных ископаемых, добываемых шахтами. Индекс — это
значение, используемое шахтными секторами и объектами.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Minerals` | `Map/Minerals.xml` | — |

## Structure

```
MINERALS
└── TYPE
    ├── uiIndex
    ├── sNameType
    └── sMineral
```

## Fields

### `MINERALS/TYPE/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс типа ископаемого. Это ключ массива.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Обязательно; идентифицирует запись или строку.
- **Source:** game code (minerals loader); map table data.
- **Confidence:** `source`

### `MINERALS/TYPE/sNameType`

- **Type:** `String` (`CHAR16`)
- **Description:** Метка объекта, используемая для этого типа ископаемого, например `Mine`.
- **Values:** Свободный текст.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (minerals loader); map table data.
- **Confidence:** `source`

### `MINERALS/TYPE/sMineral`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое название ископаемого, например `Silver` или `Gold`.
- **Values:** Свободный текст.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (minerals loader); map table data.
- **Confidence:** `source`

## Relations

- Шахтные секторы и объекты используют `uiIndex` типа ископаемого, чтобы выбрать добываемое
  ископаемое; принадлежащий город определён в [Cities](Cities.md).

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
