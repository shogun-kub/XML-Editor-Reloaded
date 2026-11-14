---
schema: 1
lang: ru
family: history
title: "History — строки истории кампании"
tables:
  - History
table_ids:
  - History
files:
  - History.xml
root: STRINGS
record: TEXT
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# History — строки истории кампании

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`STRINGS/TEXT/uiIndex`](#stringstextuiindex)
  - [`STRINGS/TEXT/sHistory`](#stringstextshistory)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`History.xml` сопоставляет ID событий истории с текстом, показываемым в журнале истории кампании.
Индекс — это ID события; текст может содержать заполнители `%s`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `History` | `History.xml` | — |

## Structure

```
STRINGS
└── TEXT
    ├── uiIndex
    └── sHistory
```

## Fields

### `STRINGS/TEXT/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс строки истории. Это ID события, используемый журналом истории.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (`XML_History.cpp`); table data.
- **Confidence:** `source`

### `STRINGS/TEXT/sHistory`

- **Type:** `String` (`CHAR16`)
- **Description:** Текст, записываемый в журнал истории для этого события. Заполнители `%s`
  заменяются игрой на имена наёмников, городов или шахт.
- **Values:** Свободный текст с необязательными заполнителями `%s`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_History.cpp`); table data.
- **Confidence:** `source`

## Relations

- Аргументы `%s` заполняются именами из [MercProfiles](MercProfiles.md),
  [Cities](Map/Cities.md) и [Minerals](Map/Minerals.md), в зависимости от события.

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
