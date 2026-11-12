---
schema: 1
lang: ru
family: items-clothes
title: "Clothes — цветовые варианты одежды"
tables:
  - Clothes
table_ids:
  - Items.Clothes
files:
  - Items/Clothes.xml
root: CLOTHESLIST
record: CLOTHES
key: uiIndex
sources:
  - Clothes table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# Clothes — цветовые варианты одежды

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CLOTHESLIST/CLOTHES/uiIndex`](#clotheslistclothesuiindex)
  - [`CLOTHESLIST/CLOTHES/szName`](#clotheslistclothesszname)
  - [`CLOTHESLIST/CLOTHES/Vest`](#clotheslistclothesvest)
  - [`CLOTHESLIST/CLOTHES/Pants`](#clotheslistclothespants)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Clothes.xml` сопоставляет вариант одежды отображаемому названию и ключам палитры жилета и штанов, используемым отрисовщиком одежды.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Clothes` | `Items/Clothes.xml` | В v4 и v5 совпадают пути полей; значения различаются. Пустые элементы `Vest`/`Pants` имеют значение. |

## Structure

```
CLOTHESLIST
└── CLOTHES
    ├── uiIndex
    ├── szName
    ├── Vest
    └── Pants
```

## Fields

### `CLOTHESLIST/CLOTHES/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс строки одежды и идентификатор записи XML.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Clothes.cpp`); table data.
- **Confidence:** `source`

### `CLOTHESLIST/CLOTHES/szName`

- **Type:** `String` (`String`)
- **Description:** Название одежды, хранящееся в таблице, но загрузчик явно игнорирует этот элемент; он носит только информационный характер.
- **Values:** Свободный текст; игнорируется при загрузке.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_Clothes.cpp`); table data.
- **Confidence:** `source`

### `CLOTHESLIST/CLOTHES/Vest`

- **Type:** `String` (`CHAR8`)
- **Description:** Ключ палитры жилета, используемый отрисовщиком одежды. Пустой элемент означает отсутствие переопределения жилета.
- **Values:** Ключ палитры, например `YELLOWVEST` / `GREENPANTS`; пустое значение означает отсутствие переопределения для этого слоя.
- **Default:** пустой элемент означает отсутствие переопределения для этого слоя одежды.
- **Source:** game code (`XML_Clothes.cpp`); table data.
- **Confidence:** `source`

### `CLOTHESLIST/CLOTHES/Pants`

- **Type:** `String` (`CHAR8`)
- **Description:** Ключ палитры штанов, используемый отрисовщиком одежды. Пустой элемент означает отсутствие переопределения штанов.
- **Values:** Ключ палитры, например `YELLOWVEST` / `GREENPANTS`; пустое значение означает отсутствие переопределения для этого слоя.
- **Default:** пустой элемент означает отсутствие переопределения для этого слоя одежды.
- **Source:** game code (`XML_Clothes.cpp`); table data.
- **Confidence:** `source`

## Relations

- `Vest` и `Pants` — ключи палитры, а не ID предметов.
- Строка одежды — данные представления; она не определяет броню или защиту.

## Localization

`szName` — отображаемая подпись; `Vest` и `Pants` — ключи палитры, они не локализуются.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы одежды.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов.
- Описания используют имена XML-элементов, значения справочников и устаревшие подписи полей; записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Отрисовщик, использующий ключи палитры, находится вне этой таблицы; строковые значения — ключи, а не произвольные цвета.
