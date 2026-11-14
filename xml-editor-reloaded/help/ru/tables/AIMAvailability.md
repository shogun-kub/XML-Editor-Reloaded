---
schema: 1
lang: ru
family: mercs-availability
title: "AIMAvailability — список доступности AIM"
tables:
  - AIMAvailability
table_ids:
  - AIMAvailability
files:
  - AIMAvailability.xml
root: AIM_AVAILABLES
record: AIM
key: uiIndex
sources:
  - AIM availability table data
status: reviewed
updated: 2026-11-12
---

# AIMAvailability — список доступности AIM

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`AIM_AVAILABLES/AIM/uiIndex`](#aim_availablesaimuiindex)
  - [`AIM_AVAILABLES/AIM/description`](#aim_availablesaimdescription)
  - [`AIM_AVAILABLES/AIM/ProfilId`](#aim_availablesaimprofilid)
  - [`AIM_AVAILABLES/AIM/AimBioID`](#aim_availablesaimaimbioid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AIMAvailability.xml` связывает профиль AIM с записью его биографии и описанием, которое
показывается в списке доступности AIM.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `AIMAvailability` | `AIMAvailability.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
AIM_AVAILABLES
└── AIM
    ├── uiIndex
    ├── description
    ├── ProfilId
    └── AimBioID
```

## Fields

### `AIM_AVAILABLES/AIM/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Идентификатор строки в списке доступности AIM.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** aim availability table data; item/profile metadata.
- **Confidence:** `source`

### `AIM_AVAILABLES/AIM/description`

- **Type:** `String`
- **Description:** Текст описания хранится в таблице, но загрузчик не читает этот элемент; он игнорируется.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (AIM availability loader); table data.
- **Confidence:** `source`

### `AIM_AVAILABLES/AIM/ProfilId`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID профиля наёмника AIM.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** aim availability table data; item/profile metadata.
- **Confidence:** `source`

### `AIM_AVAILABLES/AIM/AimBioID`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID биографии для сайта AIM.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** aim availability table data; item/profile metadata.
- **Confidence:** `source`

## Relations

- `ProfilId` указывает на [MercProfiles](MercProfiles.md).

## Localization

`description` — отображаемый текст для списка AIM.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы доступности AIM.
- Типы полей: имена XML-элементов и метаданные профилей/таблиц.
- Описания используют комментарии схемы таблицы, где они есть; формулы, проверенные по коду, не заявлены.

## Open questions

Биография и описание AIM — отображаемые данные; правила найма находятся вне этой таблицы.
