---
schema: 1
lang: ru
family: inventory-misc-drops
title: "EnemyMiscDrops — шансы выпадения прочих предметов"
tables:
  - EnemyMiscDrops
table_ids:
  - Inventory.EnemyMiscDrops
files:
  - Inventory/EnemyMiscDrops.xml
root: MISCDROPLIST
record: DROPITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# EnemyMiscDrops — шансы выпадения прочих предметов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MISCDROPLIST/DROPITEM/uiIndex`](#miscdroplistdropitemuiindex)
  - [`MISCDROPLIST/DROPITEM/usItemClass`](#miscdroplistdropitemusitemclass)
  - [`MISCDROPLIST/DROPITEM/ubEnemyDropRate`](#miscdroplistdropitemubenemydroprate)
  - [`MISCDROPLIST/DROPITEM/ubMilitiaDropRate`](#miscdroplistdropitemubmilitiadroprate)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyMiscDrops.xml` задаёт шанс, с которым вражеские солдаты и ополченцы выбрасывают предметы, относящиеся к категории «прочее». Каждая запись выбирает класс предмета и задаёт отдельные шансы для врагов и ополчения.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EnemyMiscDrops` | `Inventory/EnemyMiscDrops.xml` | — |

## Structure

```
MISCDROPLIST
└── DROPITEM
    ├── uiIndex
    ├── usItemClass
    ├── ubEnemyDropRate
    └── ubMilitiaDropRate
```

## Fields

### `MISCDROPLIST/DROPITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс правила выпадения. Это ключ массива.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `MISCDROPLIST/DROPITEM/usItemClass`

- **Type:** `Flags` (`UINT32`)
- **Description:** Битовая маска классов предметов, выбираемых этим правилом выпадения.
- **Numeric semantics:** Битовая маска; биты классов предметов объединяются через OR, поэтому большее число не означает просто «больше».
- **Values:** Биты из `Lookup.ItemClass`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `MISCDROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым вражеский солдат выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `MISCDROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым ополченец выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

## Relations

- `usItemClass` ссылается на [Lookup.ItemClass](../Lookup.md#itemclass).

## Localization

Отображаемые названия и подписи локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не заявляется.

## Open questions

- Точная формула или сопоставление enum для выведенных полей в данных таблицы не указаны.
