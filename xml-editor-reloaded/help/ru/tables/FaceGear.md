---
schema: 1
lang: ru
family: mercs-faces
title: "FaceGear — ресурсы наложения на лицо"
tables:
  - FaceGear
table_ids:
  - FaceGear
files:
  - FaceGear.xml
root: FACE_GEAR
record: ITEM
key: uiIndex
sources:
  - Face gear table data
status: reviewed
updated: 2026-11-12
---

# FaceGear — ресурсы наложения на лицо

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`FACE_GEAR/ITEM/uiIndex`](#face_gearitemuiindex)
  - [`FACE_GEAR/ITEM/Type`](#face_gearitemtype)
  - [`FACE_GEAR/ITEM/szFile`](#face_gearitemszfile)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`FaceGear.xml` сопоставляет типы снаряжения лица с STI-наложением, используемым отрисовщиком
портретов.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `FaceGear` | `FaceGear.xml` | v4 и v5 имеют одинаковые пути полей; значения различаются. |

## Structure

```
FACE_GEAR
└── ITEM
    ├── uiIndex
    ├── Type
    └── szFile
```

## Fields

### `FACE_GEAR/ITEM/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Идентичность строки снаряжения лица.
- **Values:** Целое число; более узкий диапазон не объявлен метаданными поля.
- **Default:** `0`, если опущено; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `FACE_GEAR/ITEM/Type`

- **Type:** `Integer` (`UINT16`)
- **Description:** Тип снаряжения лица, используемый отрисовщиком портретов.
- **Values:** Целое число; более узкий диапазон не объявлен метаданными поля.
- **Default:** `0`, если опущено; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `FACE_GEAR/ITEM/szFile`

- **Type:** `String` (`CHAR8`)
- **Description:** STI-файл для наложения снаряжения лица.
- **Values:** Свободный текст.
- **Default:** пустая строка, если опущено.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

## Relations

- `Type` выбирает наложение снаряжения лица; `szFile` — это STI-ресурс. Отрисовщик портретов и его
  значения по умолчанию описаны в [IMPPortraits](IMPPortraits.md).

## Localization

`szFile` — ключ ресурса, а не локализуемый текст.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы снаряжения лица.
- Типы полей: имена элементов XML и метаданные профиля/таблицы.
- Описания используют комментарии схемы таблицы, где они есть; формулы, проверенные кодом, не
  утверждаются.

## Open questions

Отрисовщик, который использует `Type` и `szFile`, находится вне этой таблицы.
