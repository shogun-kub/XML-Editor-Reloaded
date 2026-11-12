---
schema: 1
lang: ru
family: mercs-dialogue
title: "MercQuote — индексы событий реплик"
tables:
  - MercQuote
table_ids:
  - MercQuote
files:
  - MercQuote.xml
root: QARRAY
record: PROFILE
key: uiIndex
sources:
  - Merc quote table data
status: reviewed
updated: 2026-11-12
---

# MercQuote — индексы событий реплик

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`QARRAY/PROFILE/uiIndex`](#qarrayprofileuiindex)
  - [`QARRAY/PROFILE/Name`](#qarrayprofilename)
  - [`QARRAY/PROFILE/QuoteExpHeadShotOnly`](#qarrayprofilequoteexpheadshotonly)
  - [`QARRAY/PROFILE/QuoteExpTeamSpecific`](#qarrayprofilequoteexpteamspecific)
  - [`QARRAY/PROFILE/QuoteExpGenderCode`](#qarrayprofilequoteexpgendercode)
  - [`QARRAY/PROFILE/QuoteExpGotGunOrUsedGun`](#qarrayprofilequoteexpgotgunorusedgun)
  - [`QARRAY/PROFILE/QuoteExpPassingDislike`](#qarrayprofilequoteexppassingdislike)
  - [`QARRAY/PROFILE/QuoteExpWitnessQueenBugDeath`](#qarrayprofilequoteexpwitnessqueenbugdeath)
  - [`QARRAY/PROFILE/QuoteExpWitnessDeidrannaDeath`](#qarrayprofilequoteexpwitnessdeidrannadeath)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercQuote.xml` сопоставляет профиль с индексами событий реплик, используемыми в диалогах: пол, использование оружия, выстрелы в голову, неприязнь при проходе мимо, реплики для конкретной команды и реакции свидетеля.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `MercQuote` | `MercQuote.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
QARRAY
└── PROFILE
    ├── uiIndex
    ├── Name
    ├── QuoteExpHeadShotOnly
    ├── QuoteExpTeamSpecific
    ├── QuoteExpGenderCode
    ├── QuoteExpGotGunOrUsedGun
    ├── QuoteExpPassingDislike
    ├── QuoteExpWitnessQueenBugDeath
    └── QuoteExpWitnessDeidrannaDeath
```

## Fields

### `QARRAY/PROFILE/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Идентификатор профиля для этой строки реплик.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/Name`

- **Type:** `String` (`String`)
- **Description:** Имя наёмника, хранящееся в таблице, но загрузчик не читает этот элемент; он игнорируется.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpHeadShotOnly`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Индекс события реплики для ситуации, когда возможен только выстрел в голову.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpTeamSpecific`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Индекс события реплики для диалога, зависящего от команды.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpGenderCode`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Код пола, используемый при выборе реплики.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpGotGunOrUsedGun`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Индекс события реплики для получения или применения оружия.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpPassingDislike`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Индекс события реплики при проходе мимо нелюбимого персонажа.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpWitnessQueenBugDeath`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Индекс события реплики при виде смерти королевы жуков.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

### `QARRAY/PROFILE/QuoteExpWitnessDeidrannaDeath`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Индекс события реплики при виде смерти Дейдранны.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_Qarray.cpp`, `QARRAY_VALUES`); table data.
- **Confidence:** `source`

## Relations

- `uiIndex` указывает на [MercProfiles](MercProfiles.md).
- Индексы выражений реплик разрешаются системой диалогов, а не другой XML-таблицей.

## Localization

`Name` — метка профиля; сам текст реплик хранится в файлах диалогов.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы реплик наёмников.
- Типы полей: имена XML-элементов и метаданные профилей/таблиц.
- Описания используют комментарии схемы таблицы, где они есть; формулы, проверенные по коду, не заявлены.

## Open questions

Индексы выражений реплик ссылаются на внешние файлы диалогов; таблица не содержит текст реплик.
