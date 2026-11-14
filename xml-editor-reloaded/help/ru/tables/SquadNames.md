---
schema: 1
lang: ru
family: squad-names
title: "SquadNames — отображаемые названия отрядов"
tables:
  - SquadNames
table_ids:
  - SquadNames
files:
  - SquadNames.xml
root: SQUAD_NAMES
record: SQUADNAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# SquadNames — отображаемые названия отрядов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SQUAD_NAMES/SQUADNAME/uiIndex`](#squad_namessquadnameuiindex)
  - [`SQUAD_NAMES/SQUADNAME/Squad`](#squad_namessquadnamesquad)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SquadNames.xml` сопоставляет индексы отрядов с отображаемыми именами, используемыми в
стратегическом интерфейсе.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `SquadNames` | `SquadNames.xml` | — |

## Structure

```
SQUAD_NAMES
└── SQUADNAME
    ├── uiIndex
    └── Squad
```

## Fields

### `SQUAD_NAMES/SQUADNAME/uiIndex`

- **Type:** `Integer`
- **Description:** Элемент индекса, разбираемый загрузчиком, но его значение игнорируется: названия
  отрядов сохраняются в порядке файла, поэтому позиция записи `<SQUADNAME>` определяет слот отряда.
- **Values:** Целое число; изменение значения не меняет порядок сохранённых названий.
- **Default:** Отсутствует; используется следующая запись в порядке файла.
- **Source:** game code (`XML_SquadNames.cpp`); table data.
- **Confidence:** `source`

### `SQUAD_NAMES/SQUADNAME/Squad`

- **Type:** `String` (`CHAR16`)
- **Only used with:** параметр `USE_XML_SQUADNAMES` в INI; без него игра использует встроенные
  названия отрядов.
- **Description:** Отображаемое название отряда, например `Alpha`. Названия сохраняются в порядке
  файла и усекаются загрузчиком до 29 символов.
- **Values:** Свободный текст, фактически ограничен 29 символами.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (`XML_SquadNames.cpp`); table data.
- **Confidence:** `source`

## Relations

- Эта таблица является частью данных стратегической армии и инвентаря; ссылки описаны в
  примечаниях к полям.

## Localization

Отображаемые имена и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не заявляется.

## Open questions

- Точная формула или сопоставление перечисления для выведенных полей в данных таблицы не указаны.
