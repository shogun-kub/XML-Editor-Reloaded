---
schema: 1
lang: ru
family: load-screen-hints
title: "LoadScreenHints — подсказки на экране загрузки"
tables:
  - LoadScreenHints
table_ids:
  - LoadScreenHints
files:
  - LoadScreenHints.xml
root: LOADSCREENHINTS
record: LOADSCREENHINT
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# LoadScreenHints — подсказки на экране загрузки

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`LOADSCREENHINTS/LOADSCREENHINT/uiIndex`](#loadscreenhintsloadscreenhintuiindex)
  - [`LOADSCREENHINTS/LOADSCREENHINT/szName`](#loadscreenhintsloadscreenhintszname)
  - [`LOADSCREENHINTS/LOADSCREENHINT/lore`](#loadscreenhintsloadscreenhintlore)
  - [`LOADSCREENHINTS/LOADSCREENHINT/weapons`](#loadscreenhintsloadscreenhintweapons)
  - [`LOADSCREENHINTS/LOADSCREENHINT/item`](#loadscreenhintsloadscreenhintitem)
  - [`LOADSCREENHINTS/LOADSCREENHINT/items`](#loadscreenhintsloadscreenhintitems)
  - [`LOADSCREENHINTS/LOADSCREENHINT/keybind`](#loadscreenhintsloadscreenhintkeybind)
  - [`LOADSCREENHINTS/LOADSCREENHINT/food`](#loadscreenhintsloadscreenhintfood)
  - [`LOADSCREENHINTS/LOADSCREENHINT/zombie`](#loadscreenhintsloadscreenhintzombie)
  - [`LOADSCREENHINTS/LOADSCREENHINT/overheat`](#loadscreenhintsloadscreenhintoverheat)
  - [`LOADSCREENHINTS/LOADSCREENHINT/ncth`](#loadscreenhintsloadscreenhintncth)
  - [`LOADSCREENHINTS/LOADSCREENHINT/covertops`](#loadscreenhintsloadscreenhintcovertops)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`LoadScreenHints.xml` хранит советы, показываемые во время загрузки. У каждой подсказки есть текст и
один или несколько переключателей категорий; игра выбирает подсказки из включённых категорий.
Устаревший элемент `item` встречается в поставляемых данных, но текущий загрузчик его игнорирует;
флаг категории предметов задаёт элемент `items`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `LoadScreenHints` | `LoadScreenHints.xml` | — |

## Structure

```
LOADSCREENHINTS
└── LOADSCREENHINT
    ├── uiIndex
    ├── szName
    ├── lore
    ├── weapons
    ├── item
    ├── items
    ├── keybind
    ├── food
    ├── zombie
    ├── overheat
    ├── ncth
    └── covertops
```

## Fields

### `LOADSCREENHINTS/LOADSCREENHINT/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс подсказки экрана загрузки. Это ключ массива.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/szName`

- **Type:** `String` (`CHAR16`)
- **Description:** Текст подсказки, показываемый на экране загрузки. Несмотря на имя `szName`, это полное отображаемое предложение.
- **Values:** Свободный текст, до 190 символов.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/lore`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, добавляет по ИЛИ бит категории `0x0001` во внутренние флаги подсказки.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/weapons`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, добавляет по ИЛИ бит категории `0x0002` во внутренние флаги подсказки.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/item`

- **Type:** `Boolean`
- **Description:** Текущий загрузчик его не читает; бит категории предметов задаёт только `items`. Хранится как поле данных для совместимости.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/items`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, добавляет по ИЛИ бит категории `0x0004` во внутренние флаги подсказки.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/keybind`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, добавляет по ИЛИ бит категории `0x0008` во внутренние флаги подсказки.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/food`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, добавляет по ИЛИ бит категории `0x0010` во внутренние флаги подсказки.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/zombie`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, добавляет по ИЛИ бит категории `0x0020` во внутренние флаги подсказки.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/overheat`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, добавляет по ИЛИ бит категории `0x0040` во внутренние флаги подсказки.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/ncth`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, добавляет по ИЛИ бит категории `0x0080` во внутренние флаги подсказки.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

### `LOADSCREENHINTS/LOADSCREENHINT/covertops`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, добавляет по ИЛИ бит категории `0x0100` во внутренние флаги подсказки.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (load-screen hint loader); table data.
- **Confidence:** `source`

## Relations

- Эта таблица относится к стратегическим данным армии и инвентаря; ссылки описаны в примечаниях к полям.

## Localization

Отображаемые имена и подписи локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формулы времени выполнения не утверждаются.

## Open questions

- Точная формула или сопоставление перечисления для выведенных полей в данных таблиц не указаны.
