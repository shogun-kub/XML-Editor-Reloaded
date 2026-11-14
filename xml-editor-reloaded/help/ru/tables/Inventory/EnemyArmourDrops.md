---
schema: 1
lang: ru
family: inventory-armour-drops
title: "EnemyArmourDrops — шансы выпадения брони"
tables:
  - EnemyArmourDrops
table_ids:
  - Inventory.EnemyArmourDrops
files:
  - Inventory/EnemyArmourDrops.xml
root: ARMOURDROPLIST
record: DROPITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# EnemyArmourDrops — шансы выпадения брони

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ARMOURDROPLIST/DROPITEM/uiIndex`](#armourdroplistdropitemuiindex)
  - [`ARMOURDROPLIST/DROPITEM/ubArmourClass`](#armourdroplistdropitemubarmourclass)
  - [`ARMOURDROPLIST/DROPITEM/ubEnemyDropRate`](#armourdroplistdropitemubenemydroprate)
  - [`ARMOURDROPLIST/DROPITEM/ubMilitiaDropRate`](#armourdroplistdropitemubmilitiadroprate)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyArmourDrops.xml` задаёт шанс, с которым вражеские солдаты и ополченцы выбрасывают предметы из категории брони. Каждая запись выбирает класс брони и задаёт отдельные шансы для врагов и ополчения.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EnemyArmourDrops` | `Inventory/EnemyArmourDrops.xml` | — |

## Structure

```
ARMOURDROPLIST
└── DROPITEM
    ├── uiIndex
    ├── ubArmourClass
    ├── ubEnemyDropRate
    └── ubMilitiaDropRate
```

## Fields

### `ARMOURDROPLIST/DROPITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс правила выпадения. Это ключ массива.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `ARMOURDROPLIST/DROPITEM/ubArmourClass`

- **Type:** `Integer` (`UINT8`)
- **Description:** Класс брони, выбираемый этим правилом выпадения. Это идентификатор категории/ссылки.
- **Values:** Идентификатор класса брони.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `ARMOURDROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым вражеский солдат выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `ARMOURDROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым ополченец выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

## Relations

- `ubArmourClass` ссылается на [Lookup.ArmourClass](../Lookup.md#armourclass).

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
