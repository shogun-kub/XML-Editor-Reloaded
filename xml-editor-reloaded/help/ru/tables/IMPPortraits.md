---
schema: 1
lang: ru
family: mercs-faces
title: "IMPPortraits — портреты IMP по умолчанию"
tables:
  - IMPPortraits
table_ids:
  - IMPPortraits
files:
  - IMPPortraits.xml
root: IMPPORTRAITSLIST
record: IMP
key: uiIndex
sources:
  - IMP portrait table data
status: reviewed
updated: 2026-11-12
---

# IMPPortraits — портреты IMP по умолчанию

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`IMPPORTRAITSLIST/IMP/uiIndex`](#impportraitslistimpuiindex)
  - [`IMPPORTRAITSLIST/IMP/PortraitId`](#impportraitslistimpportraitid)
  - [`IMPPORTRAITSLIST/IMP/bSex`](#impportraitslistimpbsex)
  - [`IMPPORTRAITSLIST/IMP/usEyesX`](#impportraitslistimpuseyesx)
  - [`IMPPORTRAITSLIST/IMP/usEyesY`](#impportraitslistimpuseyesy)
  - [`IMPPORTRAITSLIST/IMP/usMouthX`](#impportraitslistimpusmouthx)
  - [`IMPPORTRAITSLIST/IMP/usMouthY`](#impportraitslistimpusmouthy)
  - [`IMPPORTRAITSLIST/IMP/DefaultSkin`](#impportraitslistimpdefaultskin)
  - [`IMPPORTRAITSLIST/IMP/DefaultHair`](#impportraitslistimpdefaulthair)
  - [`IMPPORTRAITSLIST/IMP/DefaultShirt`](#impportraitslistimpdefaultshirt)
  - [`IMPPORTRAITSLIST/IMP/DefaultPants`](#impportraitslistimpdefaultpants)
  - [`IMPPORTRAITSLIST/IMP/DefaultBigBody`](#impportraitslistimpdefaultbigbody)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`IMPPortraits.xml` определяет список портретов IMP и их палитры тела, волос, штанов, рубашки и кожи
по умолчанию, а также координаты глаз и рта.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `IMPPortraits` | `IMPPortraits.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
IMPPORTRAITSLIST
└── IMP
    ├── uiIndex
    ├── PortraitId
    ├── bSex
    ├── usEyesX
    ├── usEyesY
    ├── usMouthX
    ├── usMouthY
    ├── DefaultSkin
    ├── DefaultHair
    ├── DefaultShirt
    ├── DefaultPants
    └── DefaultBigBody
```

## Fields

### `IMPPORTRAITSLIST/IMP/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Идентификатор строки портрета IMP.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/PortraitId`

- **Type:** `Integer` (`INT32`)
- **Description:** ID портрета, используемый списком портретов IMP.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/bSex`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Пол портрета.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/usEyesX`

- **Type:** `Integer` (`INT8`)
- **Description:** Координата X глаз.
- **Numeric semantics:** Координата; большие значения X/Y сдвигают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** `-128..127`; координата портрета со знаком.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/usEyesY`

- **Type:** `Integer` (`INT8`)
- **Description:** Координата Y глаз.
- **Numeric semantics:** Координата; большие значения X/Y сдвигают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** `-128..127`; координата портрета со знаком.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/usMouthX`

- **Type:** `Integer` (`INT8`)
- **Description:** Координата X рта.
- **Numeric semantics:** Координата; большие значения X/Y сдвигают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** `-128..127`; координата портрета со знаком.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/usMouthY`

- **Type:** `Integer` (`INT8`)
- **Description:** Координата Y рта.
- **Numeric semantics:** Координата; большие значения X/Y сдвигают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** `-128..127`; координата портрета со знаком.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/DefaultSkin`

- **Type:** `Integer` (`INT32`)
- **Description:** Индекс палитры кожи по умолчанию.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/DefaultHair`

- **Type:** `Integer` (`INT32`)
- **Description:** Индекс палитры волос по умолчанию.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/DefaultShirt`

- **Type:** `Integer` (`INT32`)
- **Description:** Индекс палитры рубашки по умолчанию.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/DefaultPants`

- **Type:** `Integer` (`INT32`)
- **Description:** Индекс палитры штанов по умолчанию.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

### `IMPPORTRAITSLIST/IMP/DefaultBigBody`

- **Type:** `Integer` (`INT32`)
- **Description:** Тип крупного телосложения по умолчанию.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если поле отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_IMPPortraits.cpp`); table data.
- **Confidence:** `source`

## Relations

- `PortraitId` указывает на ресурсы портретов; поля `Default*` выбирают палитры. Выбранный портрет используется в [MercProfiles](MercProfiles.md).

## Localization

Отображаемых строк нет; поля палитр и координат — числовые.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы портретов IMP.
- Типы полей: имена XML-элементов и метаданные профиля и таблицы.
- Описания опираются на комментарии схемы таблицы, где они есть; проверенные кодом формулы не
  утверждаются.

## Open questions

Отрисовка портретов и поиск палитр находятся вне этой таблицы.
