---
schema: 1
lang: ru
family: inventory-explosive-drops
title: "EnemyExplosiveDrops — шансы выпадения взрывчатки"
tables:
  - EnemyExplosiveDrops
table_ids:
  - Inventory.EnemyExplosiveDrops
files:
  - Inventory/EnemyExplosiveDrops.xml
root: EXPLOSIVEDROPLIST
record: DROPITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# EnemyExplosiveDrops — шансы выпадения взрывчатки

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`EXPLOSIVEDROPLIST/DROPITEM/uiIndex`](#explosivedroplistdropitemuiindex)
  - [`EXPLOSIVEDROPLIST/DROPITEM/ubType`](#explosivedroplistdropitemubtype)
  - [`EXPLOSIVEDROPLIST/DROPITEM/ubEnemyDropRate`](#explosivedroplistdropitemubenemydroprate)
  - [`EXPLOSIVEDROPLIST/DROPITEM/ubMilitiaDropRate`](#explosivedroplistdropitemubmilitiadroprate)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyExplosiveDrops.xml` задаёт шанс, с которым вражеские солдаты и ополченцы выбрасывают предметы из категории взрывчатки. Каждая запись выбирает тип взрывчатки и задаёт отдельные шансы для врагов и ополчения.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EnemyExplosiveDrops` | `Inventory/EnemyExplosiveDrops.xml` | — |

## Structure

```
EXPLOSIVEDROPLIST
└── DROPITEM
    ├── uiIndex
    ├── ubType
    ├── ubEnemyDropRate
    └── ubMilitiaDropRate
```

## Fields

### `EXPLOSIVEDROPLIST/DROPITEM/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс правила выпадения. Это ключ массива.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `EXPLOSIVEDROPLIST/DROPITEM/ubType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Тип взрывчатки, выбираемый этим правилом выпадения. Это идентификатор категории/ссылки.
- **Values:** Идентификатор типа взрывчатки.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `EXPLOSIVEDROPLIST/DROPITEM/ubEnemyDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым вражеский солдат выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

### `EXPLOSIVEDROPLIST/DROPITEM/ubMilitiaDropRate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, с которым ополченец выбрасывает эту категорию, в процентах.
- **Numeric semantics:** чем больше, тем выше вероятность; отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (enemy-drop loader); table data.
- **Confidence:** `source`

## Relations

- `ubType` ссылается на тип взрывчатки в [Explosives](../Items/Explosives.md).

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
