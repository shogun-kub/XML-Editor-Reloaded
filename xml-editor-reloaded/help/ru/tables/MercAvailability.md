---
schema: 1
lang: ru
family: mercs-availability
title: "MercAvailability — расписание доступности MERC"
tables:
  - MercAvailability
table_ids:
  - MercAvailability
files:
  - MercAvailability.xml
root: MERC_AVAILABLES
record: MERC
key: uiIndex
sources:
  - MERC availability table data
status: reviewed
updated: 2026-11-12
---

# MercAvailability — расписание доступности MERC

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERC_AVAILABLES/MERC/uiIndex`](#merc_availablesmercuiindex)
  - [`MERC_AVAILABLES/MERC/Name`](#merc_availablesmercname)
  - [`MERC_AVAILABLES/MERC/Drunk`](#merc_availablesmercdrunk)
  - [`MERC_AVAILABLES/MERC/uiAlternateIndex`](#merc_availablesmercuialternateindex)
  - [`MERC_AVAILABLES/MERC/StartMercsAvailable`](#merc_availablesmercstartmercsavailable)
  - [`MERC_AVAILABLES/MERC/NewMercsAvailable`](#merc_availablesmercnewmercsavailable)
  - [`MERC_AVAILABLES/MERC/MercBioID`](#merc_availablesmercmercbioid)
  - [`MERC_AVAILABLES/MERC/ProfilId`](#merc_availablesmercprofilid)
  - [`MERC_AVAILABLES/MERC/usMoneyPaid`](#merc_availablesmercusmoneypaid)
  - [`MERC_AVAILABLES/MERC/usDay`](#merc_availablesmercusday)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercAvailability.xml` определяет, когда наёмники MERC становятся доступны, сколько они стоят и их альтернативную строку доступности.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `MercAvailability` | `MercAvailability.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
MERC_AVAILABLES
└── MERC
    ├── uiIndex
    ├── Name
    ├── Drunk
    ├── uiAlternateIndex
    ├── StartMercsAvailable
    ├── NewMercsAvailable
    ├── MercBioID
    ├── ProfilId
    ├── usMoneyPaid
    └── usDay
```

## Fields

### `MERC_AVAILABLES/MERC/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Идентификатор строки в списке доступности.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

### `MERC_AVAILABLES/MERC/Name`

- **Type:** `String` (`String`)
- **Description:** Имя наёмника, хранящееся в таблице, но загрузчик не читает этот элемент; он игнорируется.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

### `MERC_AVAILABLES/MERC/Drunk`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Помечает MERC как пьяного для его состояния доступности и диалогов.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

### `MERC_AVAILABLES/MERC/uiAlternateIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс альтернативной доступности; `-1` означает отсутствие.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

### `MERC_AVAILABLES/MERC/StartMercsAvailable`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Доступен ли наёмник в начале игры.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

### `MERC_AVAILABLES/MERC/NewMercsAvailable`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Становится ли наёмник доступным позже.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

### `MERC_AVAILABLES/MERC/MercBioID`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID биографии для сайта MERC.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

### `MERC_AVAILABLES/MERC/ProfilId`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID профиля наёмника MERC.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

### `MERC_AVAILABLES/MERC/usMoneyPaid`

- **Type:** `Integer` (`UINT16`)
- **Description:** Значение выплаченных денег, связанное со строкой доступности.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

### `MERC_AVAILABLES/MERC/usDay`

- **Type:** `Integer` (`UINT16`)
- **Description:** День, в который меняется доступность.
- **Numeric semantics:** Номер дня; большие значения означают более поздний день, а отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_ConditionsForMercAvailability.cpp`); table data.
- **Confidence:** `source`

## Relations

- `ProfilId` указывает на [MercProfiles](MercProfiles.md).

## Localization

`Name` — отображаемая метка.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы доступности MERC.
- Типы полей: имена XML-элементов и метаданные профилей/таблиц.
- Описания используют комментарии схемы таблицы, где они есть; формулы, проверенные по коду, не заявлены.

## Open questions

Точный расчёт расписания с использованием `usDay` и `usMoneyPaid` определяется кодом доступности MERC.
