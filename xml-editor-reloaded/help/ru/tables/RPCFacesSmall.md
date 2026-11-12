---
schema: 1
lang: ru
family: mercs-faces
title: "RPCFacesSmall — малые портреты RPC"
tables:
  - RPCFacesSmall
table_ids:
  - RPCFacesSmall
files:
  - RPCFacesSmall.xml
root: SMALLFACE
record: FACE
key: uiIndex
sources:
  - RPC small-face table data
status: reviewed
updated: 2026-11-12
---

# RPCFacesSmall — малые портреты RPC

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SMALLFACE/FACE/uiIndex`](#smallfacefaceuiindex)
  - [`SMALLFACE/FACE/Name`](#smallfacefacename)
  - [`SMALLFACE/FACE/FaceIndex`](#smallfacefacefaceindex)
  - [`SMALLFACE/FACE/EyesX`](#smallfacefaceeyesx)
  - [`SMALLFACE/FACE/EyesY`](#smallfacefaceeyesy)
  - [`SMALLFACE/FACE/MouthX`](#smallfacefacemouthx)
  - [`SMALLFACE/FACE/MouthY`](#smallfacefacemouthy)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`RPCFacesSmall.xml` определяет индекс малого портрета RPC и координаты рта и глаз, используемые анимацией малого лица.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `RPCFacesSmall` | `RPCFacesSmall.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
SMALLFACE
└── FACE
    ├── uiIndex
    ├── Name
    ├── FaceIndex
    ├── EyesX
    ├── EyesY
    ├── MouthX
    └── MouthY
```

## Fields

### `SMALLFACE/FACE/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Идентификатор строки малого лица.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/Name`

- **Type:** `String` (`String`)
- **Description:** Имя RPC, хранящееся в таблице, но загрузчик не читает этот элемент; он игнорируется.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/FaceIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс лица в таблицах портретов.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/EyesX`

- **Type:** `Integer` (`INT8`)
- **Description:** Координата X глаз на малом лице.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты выходят за допустимый диапазон.
- **Values:** `-128..127`; знаковая координата лица.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/EyesY`

- **Type:** `Integer` (`INT8`)
- **Description:** Координата Y глаз на малом лице.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты выходят за допустимый диапазон.
- **Values:** `-128..127`; знаковая координата лица.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/MouthX`

- **Type:** `Integer` (`INT8`)
- **Description:** Координата X рта на малом лице.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты выходят за допустимый диапазон.
- **Values:** `-128..127`; знаковая координата лица.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

### `SMALLFACE/FACE/MouthY`

- **Type:** `Integer` (`INT8`)
- **Description:** Координата Y рта на малом лице.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты выходят за допустимый диапазон.
- **Values:** `-128..127`; знаковая координата лица.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (`XML_RPCFacesSmall.cpp`); table data.
- **Confidence:** `source`

## Relations

- `FaceIndex` указывает на таблицы портретов и лиц, используемые [MercProfiles](MercProfiles.md).

## Localization

`Name` — отображаемая метка.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы малых лиц RPC.
- Типы полей: имена XML-элементов и метаданные профилей/таблиц.
- Описания используют комментарии схемы таблицы, где они есть; формулы, проверенные по коду, не заявлены.

## Open questions

Отрисовщик малых лиц использует эти координаты; точное масштабирование находится вне этой таблицы.
