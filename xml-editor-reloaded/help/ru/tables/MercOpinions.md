---
schema: 1
lang: ru
family: mercs-dialogue
title: "MercOpinions — мнения наёмников"
tables:
  - MercOpinions
table_ids:
  - MercOpinions
files:
  - MercOpinions.xml
root: MERCOPINIONS
record: OPINION
key: uiIndex
sources:
  - Merc opinion table data
status: reviewed
updated: 2026-11-12
---

# MercOpinions — мнения наёмников

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERCOPINIONS/OPINION/uiIndex`](#mercopinionsopinionuiindex)
  - [`MERCOPINIONS/OPINION/zNickname`](#mercopinionsopinionznickname)
  - [`MERCOPINIONS/OPINION/AnOpinion`](#mercopinionsopinionanopinion)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercOpinions.xml` хранит текст мнения, связанный с профилем наёмника.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `MercOpinions` | `MercOpinions.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
MERCOPINIONS
└── OPINION
    ├── uiIndex
    ├── zNickname
    └── AnOpinion   (повторяющийся; атрибуты id и modifier)
```

## Fields

### `MERCOPINIONS/OPINION/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Идентификатор профиля для этой строки мнений.
- **Values:** ID профиля; загрузчик сохраняет индекс строки как `UINT32`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (opinion loader); merc opinion table data.
- **Confidence:** `source`

### `MERCOPINIONS/OPINION/zNickname`

- **Type:** `String` (`String`)
- **Description:** Справочное прозвище профиля. Загрузчик читает элемент, но не использует его в игровой механике.
- **Values:** Свободный текст.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** game code (opinion loader ignores the value); merc opinion table data.
- **Confidence:** `source`

### `MERCOPINIONS/OPINION/AnOpinion`

- **Type:** `Element` (repeated; attributes `id` and `modifier`)
- **Description:** Один модификатор мнения для профиля. Элемент пустой; `id` выбирает слот мнения (индекс профиля), а `modifier` — знаковая дельта мнения.
- **Numeric semantics:** Атрибут `modifier` — знаковое значение: положительные значения улучшают мнение, отрицательные ухудшают его, а `0` нейтрально.
- **Values:** Повторяющиеся элементы `AnOpinion`; `id` `0..74`, `modifier` — знаковое целое.
- **Default:** при отсутствии модификатор мнения для этого слота не задан.
- **Source:** game code (`XML_Opinions.cpp` reads the `id` and `modifier` attributes); merc opinion table data.
- **Confidence:** `source`

## Relations

- `uiIndex` указывает на [MercProfiles](MercProfiles.md).

## Localization

`AnOpinion` не содержит отображаемого текста; `zNickname` носит справочный характер, а строки мнений игра формирует в другом месте.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы мнений наёмников.
- Типы полей: имена XML-элементов и метаданные профилей/таблиц.
- Описания используют комментарии схемы таблицы, где они есть; формулы, проверенные по коду, не заявлены.

## Open questions

Таблица хранит только числовые модификаторы мнений; правила, определяющие, какое мнение показывается, находятся в коде диалогов.
