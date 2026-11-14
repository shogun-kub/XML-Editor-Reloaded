---
schema: 1
lang: ru
family: items-food
title: "Food — значения еды и питья"
tables:
  - Food
table_ids:
  - Items.Food
files:
  - Items/Food.xml
root: FOODSLIST
record: FOOD
key: uiIndex
sources:
  - Food table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# Food — значения еды и питья

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`FOODSLIST/FOOD/uiIndex`](#foodslistfooduiindex)
  - [`FOODSLIST/FOOD/szName`](#foodslistfoodszname)
  - [`FOODSLIST/FOOD/bFoodPoints`](#foodslistfoodbfoodpoints)
  - [`FOODSLIST/FOOD/bDrinkPoints`](#foodslistfoodbdrinkpoints)
  - [`FOODSLIST/FOOD/usDecayRate`](#foodslistfoodusdecayrate)
  - [`FOODSLIST/FOOD/bMoraleMod`](#foodslistfoodbmoralemod)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Food.xml` определяет значения еды и питья для расходуемых предметов: очки еды, очки питья,
модификатор боевого духа и скорость порчи.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Food` | `Items/Food.xml` | в v4 и v5 одинаковые пути полей; значения различаются. |

## Structure

```
FOODSLIST
└── FOOD
    ├── uiIndex
    ├── szName
    ├── bFoodPoints
    ├── bDrinkPoints
    ├── usDecayRate
    └── bMoraleMod
```

## Fields

### `FOODSLIST/FOOD/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс строки еды и идентификатор XML-записи.
- **Only used with:** Система еды.
- **Values:** Неотрицательный индекс; уникален внутри файла. На него ссылается строка предмета через
  тип еды.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (food loader); food table data.
- **Confidence:** `source`

### `FOODSLIST/FOOD/szName`

- **Type:** `String` (`String`)
- **Description:** Информационная метка для еды или питья. Текущий загрузчик еды читает элемент, но
  не использует его для игровой логики или поиска отображаемого текста.
- **Only used with:** Система еды.
- **Values:** Свободный текст.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (food loader ignores the value); food table data.
- **Confidence:** `source`

### `FOODSLIST/FOOD/bFoodPoints`

- **Type:** `Integer` (`INT32`)
- **Description:** Очки еды, даваемые при употреблении предмета.
- **Only used with:** Система еды.
- **Numeric semantics:** Большие значения означают большее количество; отрицательные значения не
  имеют смысла.
- **Values:** Очки еды, даваемые предметом; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (food loader); food table data.
- **Confidence:** `source`

### `FOODSLIST/FOOD/bDrinkPoints`

- **Type:** `Integer` (`INT32`)
- **Description:** Очки питья, даваемые при употреблении предмета.
- **Only used with:** Система еды.
- **Numeric semantics:** Большие значения означают большее количество; отрицательные значения не
  имеют смысла.
- **Values:** Очки питья, даваемые предметом; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (food loader); food table data.
- **Confidence:** `source`

### `FOODSLIST/FOOD/usDecayRate`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Скорость порчи пищевого предмета; единица измерения определяется системой еды.
- **Only used with:** Система еды.
- **Numeric semantics:** Большие значения означают более быструю порчу, а отрицательные значения не
  имеют смысла.
- **Values:** Десятичная скорость порчи; единица измерения определяется системой еды.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (food loader); table data.
- **Confidence:** `source`

### `FOODSLIST/FOOD/bMoraleMod`

- **Type:** `Integer` (`INT8`)
- **Description:** Устаревшее значение боевого духа, присутствующее в `Food.xml`. Текущий загрузчик
  еды не читает этот элемент; боевой дух от еды определяется порогами `FoodMoraleMods`.
- **Only used with:** Система еды.
- **Numeric semantics:** Устаревшее знаковое значение; через текущий загрузчик оно не действует.
- **Values:** Целое число; сохраняется для совместимости с существующим XML.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the food loader has no `bMoraleMod` case); food table data.
- **Confidence:** `source`

## Relations

- На `uiIndex` ссылается тип еды предмета и [FoodOpinion](FoodOpinion.md).
- Чтобы эти значения влияли на игру, система еды должна быть включена.

## Localization

`szName` — отображаемая метка.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы еды.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов.
- Описания используют имена XML-элементов, значения справочников и комментарии схемы таблицы;
  записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Масштаб очков еды и единица порчи определяются системой еды, а не этой таблицей.
