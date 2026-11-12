---
schema: 1
lang: ru
family: map-facilities
title: "Facilities — размещение объектов"
tables:
  - Facilities
table_ids:
  - Map.Facilities
files:
  - Map/Facilities.xml
root: SECTORFACILITIES
record: FACILITY
key: SectorGrid + FacilityType
sources:
  - Game code (facility placement loader)
  - Facility type data
status: reviewed
updated: 2026-11-12
---

# Facilities — размещение объектов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SECTORFACILITIES/FACILITY/SectorGrid`](#sectorfacilitiesfacilitysectorgrid)
  - [`SECTORFACILITIES/FACILITY/FacilityType`](#sectorfacilitiesfacilityfacilitytype)
  - [`SECTORFACILITIES/FACILITY/ubHidden`](#sectorfacilitiesfacilityubhidden)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Facilities.xml` размещает экземпляры объектов на стратегической карте. Каждая запись указывает,
какой тип объекта присутствует в секторе и как он раскрывается во всплывающем окне сектора. Сам
тип объекта (название, назначение, эффекты) определён в `FacilityTypes`; эта таблица лишь
привязывает тип к сектору.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Facilities` | `Map/Facilities.xml` | нет ключа метаданных; логический ключ — сектор + тип |

У `Facilities` нет строки в метаданных редактора, поэтому редактор показывает её как таблицу,
управляемую документом.

## Structure

```
SECTORFACILITIES
└── FACILITY
    ├── SectorGrid
    ├── FacilityType
    └── ubHidden
```

Загрузчик сохраняет запись в `gFacilityLocations[sector][facilityType]`. Логический ключ — пара
`SectorGrid + FacilityType`; в самом файле элемента ключа нет. Дубликат пары перезаписал бы
предыдущие флаги (побеждает последняя запись).

## Fields

### `SECTORFACILITIES/FACILITY/SectorGrid`

- **Type:** `String` (sector code)
- **Description:** Сектор, в котором размещён объект, например `B13`. Загрузчик берёт первый
  символ как строку карты (`A`..`P`), а остальной текст — как столбец (`1`..`16`).
- **Values:** `A1`..`P16`; загрузчик принимает только строки и столбцы 1..16 и игнорирует
  недопустимый сектор.
- **Default:** нет; при недопустимом значении запись остаётся неразмещённой.
- **Source:** game code (facility placement loader); facility type data.
- **Confidence:** `source`

### `SECTORFACILITIES/FACILITY/FacilityType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс типа объекта в `FacilityTypes.ubIndex`. Он выбирает название объекта,
  назначение, лимиты персонала и эффекты.
- **Values:** `0` означает «нет объекта», и загрузчик пропускает запись; ненулевые значения —
  допустимые ID `FacilityTypes.ubIndex`.
- **Default:** `0` (пропускается).
- **Source:** game code (facility placement loader); facility type data.
- **Confidence:** `source`

### `SECTORFACILITIES/FACILITY/ubHidden`

- **Type:** `Integer` (`0..2`)
- **Description:** Когда объект появляется во всплывающем окне информации сектора: всегда, после
  исследования сектора или никогда. `2` — допустимое значение, поэтому не стоит считать это
  обычным флагом 0/1.
- **Values:** `0` показывать всегда, `1` после исследования, `2` никогда.
- **Default:** `0` (показывать всегда).
- **Source:** game code (facility placement loader); facility type data.
- **Confidence:** `source`

## Relations

- `Facilities.FacilityType` -> [FacilityTypes](FacilityTypes.md) `ubIndex`.
- Код сектора определяет, к какому городу или региону относится объект; данные городов — в
  [Cities](Cities.md).
- Названия объектов, подсказки и текст назначений находятся в [FacilityTypes](FacilityTypes.md)
  и её оверлее `Russian.*`, а не в этой таблице.

## Localization

В этой таблице нет строк. Видимое название объекта и текст назначения берутся из
`FacilityTypes`; правила оверлеев см. в `Localization.md`.

## Versions

В v4 и v5 используются одни и те же три поля и одинаковая структура
`SECTORFACILITIES/FACILITY`. Размещённые объекты могут различаться; сравнивайте файлы, когда
важен конкретный сектор.

## Sources

- Игровой код: загрузчик размещения объектов и загрузчик типов объектов.
- Данные таблицы: `Map/Facilities.xml`; комментарий в её заголовке документирует три поля.

## Open questions

- Для `Facilities` нет строки ключа в метаданных; составной ключ — логический контракт,
  используемый редактором, а не объявленная запись `TableKeys`. Добавление ключа было бы
  задачей метаданных, а не правкой данных.
- Парсер сохраняет `ubHidden` в байтовом флаге; значение 2 сохраняется, но внутреннее имя типа
  вводит в заблуждение.
