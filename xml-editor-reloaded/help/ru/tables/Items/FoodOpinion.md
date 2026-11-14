---
schema: 1
lang: ru
family: items-food
title: "FoodOpinion — пищевые предпочтения профилей"
tables:
  - FoodOpinion
table_ids:
  - Items.FoodOpinion
files:
  - Items/FoodOpinion.xml
root: FOODOPINIONSLIST
record: FOODOPINION
key: uProfile + FoodNumber
sources:
  - Food opinion table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# FoodOpinion — пищевые предпочтения профилей

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`FOODOPINIONSLIST/FOODOPINION/uProfile`](#foodopinionslistfoodopinionuprofile)
  - [`FOODOPINIONSLIST/FOODOPINION/PROFILE_OPINION/FoodNumber`](#foodopinionslistfoodopinionprofile_opinionfoodnumber)
  - [`FOODOPINIONSLIST/FOODOPINION/PROFILE_OPINION/MoraleMod`](#foodopinionslistfoodopinionprofile_opinionmoralemod)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`FoodOpinion.xml` хранит пищевые предпочтения по профилям: каждая строка связывает профиль со
строкой еды и задаёт модификатор боевого духа для этой комбинации.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `FoodOpinion` | `Items/FoodOpinion.xml` | в v4 и v5 одинаковые пути полей; значения различаются. |

## Structure

```
FOODOPINIONSLIST
└── FOODOPINION
    ├── uProfile
    └── PROFILE_OPINION
        ├── FoodNumber
        └── MoraleMod
```

## Fields

### `FOODOPINIONSLIST/FOODOPINION/uProfile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Профиль, пищевое предпочтение которого описывает эта строка.
- **Only used with:** Система еды.
- **Values:** ID профиля; загрузчик хранит одну запись на профиль, поэтому более поздняя строка для
  того же профиля перезаписывает более раннюю.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (food-opinion loader); food opinion table data.
- **Confidence:** `source`

### `FOODOPINIONSLIST/FOODOPINION/PROFILE_OPINION/FoodNumber`

- **Type:** `Integer` (`UINT16`)
- **Description:** Строка еды, которую профиль любит или не любит.
- **Only used with:** Система еды.
- **Values:** Индекс строки еды в [Food](Food.md).
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (food/food-opinion loader); table data.
- **Confidence:** `source`

### `FOODOPINIONSLIST/FOODOPINION/PROFILE_OPINION/MoraleMod`

- **Type:** `Integer` (`INT8`)
- **Description:** Модификатор боевого духа, применяемый к этому профилю при употреблении еды.
- **Only used with:** Система еды.
- **Numeric semantics:** Знаковый модификатор боевого духа: положительные значения повышают боевой
  дух, отрицательные понижают, а `0` нейтрально.
- **Values:** Модификатор боевого духа для этого профиля и еды.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (food/food-opinion loader); table data.
- **Confidence:** `source`

## Relations

- `uProfile` указывает на профиль; `FoodNumber` указывает на [Food](Food.md).
- Чтобы эти предпочтения влияли на игру, система еды должна быть включена.

## Localization

В этой таблице нет отображаемых строк; имена профилей и еды берутся из их собственных таблиц.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы пищевых предпочтений.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов.
- Описания используют имена XML-элементов, значения справочников и комментарии схемы таблицы;
  записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Открытых вопросов по объединению строк не осталось: загрузчик пишет в `FoodOpinions[profile]`,
поэтому более поздние строки для того же профиля заменяют более ранние, а не накапливают мнения.
