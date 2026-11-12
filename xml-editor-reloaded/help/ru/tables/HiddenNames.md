---
schema: 1
lang: ru
family: hidden-names
title: "HiddenNames — скрытые отображаемые имена"
tables:
  - HiddenNames
table_ids:
  - HiddenNames
files:
  - HiddenNames.xml
root: HIDDENNAMES
record: NAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# HiddenNames — скрытые отображаемые имена

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`HIDDENNAMES/NAME/uiIndex`](#hiddennamesnameuiindex)
  - [`HIDDENNAMES/NAME/description`](#hiddennamesnamedescription)
  - [`HIDDENNAMES/NAME/Hidden`](#hiddennamesnamehidden)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`HiddenNames.xml` хранит имена, скрытые от обычного отображения, с ключом `uiIndex`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `HiddenNames` | `HiddenNames.xml` | — |

## Structure

```
HIDDENNAMES
└── NAME
    ├── uiIndex
    ├── description
    └── Hidden
```

## Fields

### `HIDDENNAMES/NAME/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс записи скрытого имени. Это ключ массива.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (`XML_HiddenNames.cpp`); table data.
- **Confidence:** `source`

### `HIDDENNAMES/NAME/description`

- **Type:** `String`
- **Description:** Текстовая метка, хранящаяся в таблице, но текущий загрузчик её не читает;
  разбираются только `uiIndex` и `Hidden`.
- **Values:** Свободный текст; игнорируется при загрузке.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_HiddenNames.cpp`); table data.
- **Confidence:** `source`

### `HIDDENNAMES/NAME/Hidden`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, имя скрыто от обычного отображения. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_HiddenNames.cpp`); table data.
- **Confidence:** `source`

## Relations

- Эта таблица — часть стратегических данных армии и инвентаря; ссылки описаны в примечаниях к полям.

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
