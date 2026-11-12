---
schema: 1
lang: ru
family: inventory-weapon-drops
title: "EnemyWeaponDrops — шансы выпадения оружия"
tables:
  - EnemyWeaponDrops
table_ids:
  - Inventory.EnemyWeaponDrops
files:
  - Inventory/EnemyWeaponDrops.xml
root: WEAPONDROPLIST
record: DROPITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# EnemyWeaponDrops — шансы выпадения оружия

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`WEAPONDROPLIST/DROPITEM/uiIndex`](#weapondroplistdropitemuiindex)
  - [`WEAPONDROPLIST/DROPITEM/ubWeaponType`](#weapondroplistdropitemubweapontype)
  - [`WEAPONDROPLIST/DROPITEM/ubEnemyDropRate`](#weapondroplistdropitemubenemydroprate)
  - [`WEAPONDROPLIST/DROPITEM/ubMilitiaDropRate`](#weapondroplistdropitemubmilitiadroprate)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyWeaponDrops.xml` задаёт шанс, с которым вражеские солдаты и ополченцы выбрасывают предметы из категории оружия. Каждая запись выбирает тип оружия и задаёт отдельные шансы для врагов и ополчения.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EnemyWeaponDrops` | `Inventory/EnemyWeaponDrops.xml` | — |

## Structure

```
WEAPONDROPLIST
└── DROPITEM
    ├── uiIndex
    ├── ubWeaponType
    ├── ubEnemyDropRate
    └── ubMilitiaDropRate
```

## Fields

### `WEAPONDROPLIST/DROPITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс правила выпадения. Это ключ массива.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `WEAPONDROPLIST/DROPITEM/ubWeaponType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Тип оружия, выбираемый этим правилом выпадения. Это идентификатор категории/ссылки.
- **Values:** Идентификатор типа оружия.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `WEAPONDROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым вражеский солдат выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `WEAPONDROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым ополченец выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

## Relations

- `ubWeaponType` ссылается на [Lookup.WeaponType](../Lookup.md#weapontype).

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
