---
schema: 1
lang: ru
family: inventory-ammo-drops
title: "EnemyAmmoDrops — шансы выпадения боеприпасов"
tables:
  - EnemyAmmoDrops
table_ids:
  - Inventory.EnemyAmmoDrops
files:
  - Inventory/EnemyAmmoDrops.xml
root: AMMODROPLIST
record: DROPITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# EnemyAmmoDrops — шансы выпадения боеприпасов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`AMMODROPLIST/DROPITEM/uiIndex`](#ammodroplistdropitemuiindex)
  - [`AMMODROPLIST/DROPITEM/uiType`](#ammodroplistdropitemuitype)
  - [`AMMODROPLIST/DROPITEM/ubEnemyDropRate`](#ammodroplistdropitemubenemydroprate)
  - [`AMMODROPLIST/DROPITEM/ubMilitiaDropRate`](#ammodroplistdropitemubmilitiadroprate)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyAmmoDrops.xml` задаёт шанс, с которым вражеские солдаты и ополченцы выбрасывают предметы из категории боеприпасов. Каждая запись выбирает тип боеприпаса и задаёт отдельные шансы для врагов и ополчения.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EnemyAmmoDrops` | `Inventory/EnemyAmmoDrops.xml` | — |

## Structure

```
AMMODROPLIST
└── DROPITEM
    ├── uiIndex
    ├── uiType
    ├── ubEnemyDropRate
    └── ubMilitiaDropRate
```

## Fields

### `AMMODROPLIST/DROPITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс правила выпадения. Это ключ массива.
- **Values:** `0..4294967295`; загрузчик сохраняет это поле как `UINT32`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (ammo-drop loader); table data.
- **Confidence:** `source`

### `AMMODROPLIST/DROPITEM/uiType`

- **Type:** `Integer` (`UINT32`)
- **Description:** Тип боеприпаса, выбираемый этим правилом выпадения. Это идентификатор категории/ссылки.
- **Values:** ID типа боеприпаса; загрузчик сохраняет это поле как `UINT32`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (ammo-drop loader); table data.
- **Confidence:** `source`

### `AMMODROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым вражеский солдат выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (ammo-drop loader); table data.
- **Confidence:** `source`

### `AMMODROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым ополченец выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (ammo-drop loader); table data.
- **Confidence:** `source`

## Relations

- `uiType` ссылается на тип боеприпаса в [AmmoTypes](../Items/AmmoTypes.md).

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
