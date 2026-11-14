---
schema: 1
lang: ru
family: briefing-room
title: "BriefingRoom — записи брифинга миссии"
tables:
  - BriefingRoom
table_ids:
  - BriefingRoom.BriefingRoom
files:
  - BriefingRoom/BriefingRoom.xml
root: BRIEFINGROOM
record: DATA
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# BriefingRoom — записи брифинга миссии

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`BRIEFINGROOM/DATA/uiIndex`](#briefingroomdatauiindex)
  - [`BRIEFINGROOM/DATA/Name`](#briefingroomdataname)
  - [`BRIEFINGROOM/DATA/Hidden`](#briefingroomdatahidden)
  - [`BRIEFINGROOM/DATA/MaxPages`](#briefingroomdatamaxpages)
  - [`BRIEFINGROOM/DATA/MaxImages`](#briefingroomdatamaximages)
  - [`BRIEFINGROOM/DATA/ImagePositionX`](#briefingroomdataimagepositionx)
  - [`BRIEFINGROOM/DATA/ImagePositionY`](#briefingroomdataimagepositiony)
  - [`BRIEFINGROOM/DATA/NextMission`](#briefingroomdatanextmission)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`BriefingRoom.xml` определяет страницы и изображения брифингов миссий. Каждая запись
индексируется, может быть скрыта и указывает на следующую миссию через `NextMission` (`-1`
означает отсутствие).

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `BriefingRoom` | `BriefingRoom/BriefingRoom.xml` | — |

## Structure

```
BRIEFINGROOM
└── DATA
    ├── uiIndex
    ├── Name
    ├── Hidden
    ├── MaxPages
    ├── MaxImages
    ├── ImagePositionX
    ├── ImagePositionY
    └── NextMission
```

## Fields

### `BRIEFINGROOM/DATA/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс записи комнаты брифинга. Это ключ массива.
- **Values:** `0..4294967295`; загрузчик сохраняет это поле как `UINT32`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое название миссии/брифинга.
- **Values:** Свободный текст.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/Hidden`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Переключатель видимости с обратным смыслом в загрузчике: `0` скрывает запись
  брифинга, любое ненулевое значение делает её видимой.
- **Numeric semantics:** Логическое; `0` = скрыто, ненулевое = видимо.
- **Values:** `0` или `1` в поставляемой таблице.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/MaxPages`

- **Type:** `Integer` (`UINT32`)
- **Description:** Максимальное количество текстовых страниц, показываемых для этого брифинга.
- **Numeric semantics:** больше = больше страниц; отрицательные значения не имеют смысла.
- **Values:** `0..4294967295`; загрузчик сохраняет это поле как `UINT32`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/MaxImages`

- **Type:** `Integer` (`INT32`)
- **Description:** Максимальное количество изображений, показываемых для этого брифинга.
- **Numeric semantics:** больше = больше изображений; загрузчик сохраняет это поле как знаковый
  `INT32`.
- **Values:** `-2147483648..2147483647`; предполагаемый диапазон — неотрицательные значения.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/ImagePositionX`

- **Type:** `Integer` (`UINT32`)
- **Description:** Горизонтальная позиция изображения брифинга на экране, в пикселях.
- **Numeric semantics:** увеличение = вправо, уменьшение = влево.
- **Values:** `0..4294967295`; загрузчик сохраняет это поле как `UINT32`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/ImagePositionY`

- **Type:** `Integer` (`UINT32`)
- **Description:** Вертикальная позиция изображения брифинга на экране, в пикселях.
- **Numeric semantics:** увеличение = вниз, уменьшение = вверх.
- **Values:** `0..4294967295`; загрузчик сохраняет это поле как `UINT32`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

### `BRIEFINGROOM/DATA/NextMission`

- **Type:** `Integer` (`INT32`)
- **Description:** Индекс следующей записи брифинга или `-1`, если следующей миссии нет. Это
  ссылка.
- **Numeric semantics:** `-1` = следующей миссии нет; неотрицательное = индекс следующей записи
  брифинга.
- **Values:** Индекс брифинга или `-1`; загрузчик сохраняет это поле как `INT32`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (briefing-room loader); table data.
- **Confidence:** `source`

## Relations

- `NextMission` ссылается на другой `uiIndex` брифинга или на `-1` для конца цепочки.

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
