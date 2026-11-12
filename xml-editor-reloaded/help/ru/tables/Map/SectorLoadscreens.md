---
schema: 1
lang: ru
family: map-sector-loadscreens
title: "SectorLoadscreens — экраны загрузки секторов"
tables:
  - SectorLoadscreens
table_ids:
  - Map.SectorLoadscreens
files:
  - Map/SectorLoadscreens.xml
root: LOADSCREENS
record: SECTOR
key: uiIndex
sources:
  - Game code (sector-loadscreens loader and loading-screen display)
  - Map table data and XML comments
status: reviewed
updated: 2026-11-12
---

# SectorLoadscreens — экраны загрузки секторов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`LOADSCREENS/SECTOR/uiIndex`](#loadscreenssectoruiindex)
  - [`LOADSCREENS/SECTOR/szLocation`](#loadscreenssectorszlocation)
  - [`LOADSCREENS/SECTOR/RandomAltSector`](#loadscreenssectorrandomaltsector)
  - [`LOADSCREENS/SECTOR/szImageFormat`](#loadscreenssectorszimageformat)
  - [`LOADSCREENS/SECTOR/szDay`](#loadscreenssectorszday)
  - [`LOADSCREENS/SECTOR/szNight`](#loadscreenssectorsznight)
  - [`LOADSCREENS/SECTOR/szImageFormatAlt`](#loadscreenssectorszimageformatalt)
  - [`LOADSCREENS/SECTOR/szDayAlt`](#loadscreenssectorszdayalt)
  - [`LOADSCREENS/SECTOR/szNightAlt`](#loadscreenssectorsznightalt)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SectorLoadscreens.xml` сопоставляет сектору изображение, показываемое во время его загрузки. Запись
`0` — экран зоны высадки/вертолёта; записи `1..256` сопоставляются по `szLocation` с текущим сектором
поверхности. Изображения — пути без расширения в каталоге игровых данных: загрузчик добавляет суффикс
разрешения и проверяет сначала `.png`, затем `.sti`. Отсутствующий или пустой путь не откатывается к
общему экрану; игра заливает экран чёрным и печатает сообщение «loadscreen data file not found».
Загрузчик также разбирает альтернативные поля дня/ночи, но текущий код отображения использует только
дневную альтернативу.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `SectorLoadscreens` | `Map/SectorLoadscreens.xml` | — |

## Structure

```
LOADSCREENS
└── SECTOR
    ├── uiIndex
    ├── szLocation
    ├── RandomAltSector
    ├── szImageFormat
    ├── szDay
    ├── szNight
    ├── szImageFormatAlt
    ├── szDayAlt
    └── szNightAlt
```

## Fields

### `LOADSCREENS/SECTOR/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс записи экрана загрузки и ячейки массива, в которой она хранится; он должен
  быть уникальным. Запись `0` используется напрямую для экрана вертолёта/зоны высадки.
- **Values:** `0..256` (`MAX_SECTOR_LOADSCREENS - 1`); загрузчик разбирает десятичное или `0x`
  шестнадцатеричное значение и отбрасывает записи вне диапазона.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szLocation`

- **Type:** `String` (`CHAR8[10]`)
- **Description:** Код сектора (`A1`..`P16`), для которого эта запись задаёт экран загрузки, или
  `START` для зоны высадки в записи `0`. Записи `1..256` сопоставляются с текущим кодом сектора;
  запись `0` читается напрямую для экрана вертолёта.
- **Values:** `A1`..`P16` или `START`; сохраняется в буфере `CHAR8[10]`.
- **Default:** Обязательно для записей `1..256`; запись `0` использует экран зоны высадки независимо
  от своего текста.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/RandomAltSector`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, игра выбирает с равной вероятностью основной или альтернативный
  экран для дня или ночи (`Random(2) > 0`). В текущем коде отображения действует только дневная
  альтернатива (см. `szNightAlt`).
- **Values:** `0` или `1`; любое ненулевое значение считается включённым.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szImageFormat`

- **Type:** `String` (`CHAR8[5]`)
- **Description:** Метка формата изображения из таблицы (`STI` или `PNG` в поставляемых данных).
  Текущий выбор для сектора поверхности не читает её; он сам проверяет имена файлов `.png` и `.sti`,
  поэтому это поле не выбирает расширение файла.
- **Values:** Свободный текст до 4 символов (`CHAR8[5]`).
- **Default:** Отсутствует; пустая строка.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szDay`

- **Type:** `String` (`CHAR8[80]`)
- **Description:** Путь к основному изображению дневного экрана загрузки, относительно каталога
  игровых данных и без расширения (например `Loadscreens\LS_DayTropical`). Загрузчик добавляет
  суффикс разрешения и проверяет сначала `.png`, затем `.sti`.
- **Values:** Путь в виде свободного текста до 79 символов (`CHAR8[80]`); пусто = нет дневного
  изображения (тогда отображение сообщает об отсутствующем файле).
- **Default:** Отсутствует; пустая строка.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szNight`

- **Type:** `String` (`CHAR8[80]`)
- **Description:** Путь к основному изображению ночного экрана загрузки, относительно каталога игровых
  данных и без расширения. Те же суффикс разрешения и проверка `.png`/`.sti`, что и у `szDay`.
- **Values:** Путь в виде свободного текста до 79 символов (`CHAR8[80]`); пусто = нет ночного
  изображения (тогда отображение сообщает об отсутствующем файле).
- **Default:** Отсутствует; пустая строка.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szImageFormatAlt`

- **Type:** `String` (`CHAR8[5]`)
- **Description:** Метка альтернативного формата изображения; когда `RandomAltSector` выбирает
  альтернативу и это значение не пусто, она заменяет `szImageFormat`. Как и `szImageFormat`, в
  текущем коде она не используется для выбора расширения файла поверхности.
- **Values:** Свободный текст до 4 символов (`CHAR8[5]`).
- **Default:** Отсутствует; пустая строка.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szDayAlt`

- **Type:** `String` (`CHAR8[80]`)
- **Description:** Путь к альтернативному изображению дневного экрана загрузки; используется, когда
  `RandomAltSector` выбирает альтернативу и экран — `DAY`. Те же правила пути без расширения, что и у
  `szDay`.
- **Values:** Путь в виде свободного текста до 79 символов; пусто = нет альтернативного дневного
  изображения (тогда отображение сообщает об отсутствующем файле, а не откатывается к `szDay`).
- **Default:** Отсутствует; пустая строка.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

### `LOADSCREENS/SECTOR/szNightAlt`

- **Type:** `String` (`CHAR8[80]`)
- **Description:** Путь к альтернативному изображению ночного экрана загрузки в таблице, но текущая
  `DisplayLoadScreenWithID` использует `szNight` для случая `NIGHT_ALT`, поэтому это значение
  разбирается и никогда не читается кодом отображения. Поставляемая запись `B2`, например, хранит
  `LS_NightChitzena` вместо `LS_MineNight`.
- **Values:** Путь в виде свободного текста до 79 символов; игнорируется текущим кодом отображения.
- **Default:** Отсутствует; игнорируется.
- **Source:** game code (sector-loadscreens loader and display); table data and XML comments.
- **Confidence:** `source`

## Relations

- `szLocation` определяет сектор поверхности, чей экран загрузки используется; сам сектор определён в
  [SectorNames](SectorNames.md). Запись `0` — запись зоны высадки/вертолёта, она не сопоставляется по
  коду сектора.

## Localization

Отображаемые имена, подсказки и описания локализуются через языковые оверлеи; числовые ID, коды
секторов и значения матриц — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Код игры: загрузчик экранов загрузки секторов и отображение экрана загрузки.
- Данные таблицы карты и комментарии XML.
- Описания следуют текущему пути экрана загрузки; откат к общему изображению не утверждается,
  поскольку вместо этого игра рисует чёрный экран с диагностикой.

## Open questions

- `DisplayLoadScreenWithID` выбирает `szNight` для случая `NIGHT_ALT` вместо `szNightAlt`, поэтому
  `szNightAlt` разбирается, но никогда не используется; является ли это ошибкой или задумано —
  неизвестно.
- `szImageFormat` и `szImageFormatAlt` разбираются, но путь сектора поверхности выбирает
  `.png`/`.sti` проверкой файлов; должны ли поля формата влиять на выбор — неизвестно.
- Если ни одна запись не совпадает или выбранный путь пуст либо отсутствует, игра показывает чёрный
  экран с диагностикой; предполагался ли общий откат — неизвестно.
