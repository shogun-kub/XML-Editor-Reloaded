---
schema: 1
lang: ru
family: mercs-dialogue
title: "OldAIMArchive — устаревшие биографии AIM"
tables:
  - OldAIMArchive
table_ids:
  - OldAIMArchive
files:
  - OldAIMArchive.xml
root: OLD_MERC
record: MERC
key: uiIndex
sources:
  - Old AIM archive table data
status: reviewed
updated: 2026-11-12
---

# OldAIMArchive — устаревшие биографии AIM

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`OLD_MERC/MERC/uiIndex`](#old_mercmercuiindex)
  - [`OLD_MERC/MERC/NickName`](#old_mercmercnickname)
  - [`OLD_MERC/MERC/Name`](#old_mercmercname)
  - [`OLD_MERC/MERC/Bio`](#old_mercmercbio)
  - [`OLD_MERC/MERC/FaceID`](#old_mercmercfaceid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`OldAIMArchive.xml` хранит устаревшие биографии наёмников AIM и ID лиц, используемые путём архива и импорта.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `OldAIMArchive` | `OldAIMArchive.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
OLD_MERC
└── MERC
    ├── uiIndex
    ├── NickName
    ├── Name
    ├── Bio
    └── FaceID
```

## Fields

### `OLD_MERC/MERC/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Идентификатор строки архива.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `source`

### `OLD_MERC/MERC/NickName`

- **Type:** `String` (`CHAR16`)
- **Description:** Прозвище архивного наёмника.
- **Values:** Свободный текст.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `source`

### `OLD_MERC/MERC/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Полное имя архивного наёмника.
- **Values:** Свободный текст.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `source`

### `OLD_MERC/MERC/Bio`

- **Type:** `String` (`CHAR16`)
- **Description:** Текст биографии архивного наёмника.
- **Values:** Свободный текст.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `source`

### `OLD_MERC/MERC/FaceID`

- **Type:** `Integer` (`INT16`)
- **Description:** Индекс лица архивного наёмника.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** old aim archive table data; item/profile metadata.
- **Confidence:** `source`

## Relations

- `uiIndex` — идентификатор записи архива; `FaceID` указывает на ресурсы лиц, используемые [MercProfiles](MercProfiles.md).

## Localization

`Name`, `NickName` и `Bio` — отображаемый текст.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы архива AIM.
- Типы полей: имена XML-элементов и метаданные профилей/таблиц.
- Описания используют комментарии схемы таблицы, где они есть; формулы, проверенные по коду, не заявлены.

## Open questions

Таблица архива — устаревший источник миграции; действующие данные AIM находятся в таблицах профилей.
