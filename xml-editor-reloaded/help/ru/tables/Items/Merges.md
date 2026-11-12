---
schema: 1
lang: ru
family: items-merges
title: "Merges — рецепты объединения предметов"
tables:
  - Merges
table_ids:
  - Items.Merges
files:
  - Items/Merges.xml
root: MERGELIST
record: MERGE
key: firstItemIndex + secondItemIndex
sources:
  - Merge table data
  - MergeType lookup table
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# Merges — рецепты объединения предметов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERGELIST/MERGE/firstItemIndex`](#mergelistmergefirstitemindex)
  - [`MERGELIST/MERGE/secondItemIndex`](#mergelistmergeseconditemindex)
  - [`MERGELIST/MERGE/firstResultingItemIndex`](#mergelistmergefirstresultingitemindex)
  - [`MERGELIST/MERGE/secondResultingItemIndex`](#mergelistmergesecondresultingitemindex)
  - [`MERGELIST/MERGE/mergeType`](#mergelistmergemergetype)
  - [`MERGELIST/MERGE/APCost`](#mergelistmergeapcost)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Merges.xml` определяет рецепты «предмет — предмет»: два входных предмета, до двух результатов, тип
объединения и стоимость в ОД. Тип объединения выбирает действие и семейство проверок навыка.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Merges` | `Items/Merges.xml` | в v4 и v5 одинаковые пути полей; значения различаются. |

## Structure

```
MERGELIST
└── MERGE
    ├── firstItemIndex
    ├── secondItemIndex
    ├── firstResultingItemIndex
    ├── secondResultingItemIndex
    ├── mergeType
    └── APCost
```

## Fields

### `MERGELIST/MERGE/firstItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID первого входного предмета в объединении.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/secondItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID второго входного предмета в объединении.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/firstResultingItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в первом слоте результата.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/secondResultingItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого во втором слоте результата; `0` означает отсутствие
  второго результата.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/mergeType`

- **Type:** `Integer` (`UINT16`)
- **Description:** Тип объединения или использования, который выбирает действие, проверку навыка и
  обработку результата.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/APCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** Стоимость объединения в очках действия.
- **Numeric semantics:** Стоимость в очках действия; загрузчик пересчитывает её через
  `DynamicAdjustAPConstants`, поэтому сохранённое значение отличается, когда `AP_MAXIMUM` не равен
  `100`.
- **Values:** `0..65535` после пересчёта; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

## Relations

- Все колонки предметов указывают на [Items](Items.md).
- `mergeType` использует [Lookup.MergeType](../Lookup.md#mergetype).
- Комбинации вложений обрабатываются в [AttachmentComboMerges](AttachmentComboMerges.md).

## Localization

В этой таблице нет отображаемых строк; имена предметов берутся из `Items`.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы объединений.
- Таблица справочника MergeType.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов.
- Описания используют имена XML-элементов, значения справочников и устаревшие метки полей; записи с
  пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Точная проверка навыка для каждого `mergeType` определяется реализацией объединения, а не этой
таблицей.
