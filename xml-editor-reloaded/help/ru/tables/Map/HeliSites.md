---
schema: 1
lang: ru
family: map-heli-sites
title: "HeliSites — точки дозаправки вертолёта"
tables:
  - HeliSites
table_ids:
  - Map.HeliSites
files:
  - Map/HeliSites.xml
root: HELI_INFO
record: REFUELLIST/REFUEL
key: refuelIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# HeliSites — точки дозаправки вертолёта

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelIndex`](#heli_inforefuellistrefuelrefuelindex)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelSector/x`](#heli_inforefuellistrefuelrefuelsectorx)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelSector/y`](#heli_inforefuellistrefuelrefuelsectory)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelHidden`](#heli_inforefuellistrefuelrefuelhidden)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelHeliTileIndex`](#heli_inforefuellistrefuelrefuelhelitileindex)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelHeliGridNo`](#heli_inforefuellistrefuelrefuelheligridno)
  - [`HELI_INFO/REFUELLIST/REFUEL/refuelSkyriderGridNo`](#heli_inforefuellistrefuelrefuelskyridergridno)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`HeliSites.xml` перечисляет точки дозаправки вертолёта. Каждая запись размещает точку в секторе и
задаёт позиции сетки для вертолёта и Скайрайдера, а также необязательный флаг скрытости.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `HeliSites` | `Map/HeliSites.xml` | — |

## Structure

```
HELI_INFO
└── REFUELLIST
    └── REFUEL
        ├── refuelIndex
        ├── refuelSector
        │   ├── x
        │   └── y
        ├── refuelHidden
        ├── refuelHeliTileIndex
        ├── refuelHeliGridNo
        └── refuelSkyriderGridNo
```

## Fields

### `HELI_INFO/REFUELLIST/REFUEL/refuelIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс точки дозаправки вертолёта. Это ключ массива.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Обязательно; идентифицирует запись или строку.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelSector/x`

- **Type:** `Integer` (`INT16`)
- **Description:** Столбец карты сектора дозаправки. Меньшие значения означают дальше на запад; это
  координата.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelSector/y`

- **Type:** `Integer` (`INT16`)
- **Description:** Строка карты сектора дозаправки. Меньшие значения означают дальше на север; это
  координата.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelHidden`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если задано, точка дозаправки скрыта от игрока до обнаружения.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelHeliTileIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Индекс тайла графики посадки и дозаправки вертолёта. Это идентификатор тайла.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelHeliGridNo`

- **Type:** `Integer` (`INT32`)
- **Description:** Номер клетки карты, где вертолёт садится для дозаправки. Это идентификатор
  клетки.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

### `HELI_INFO/REFUELLIST/REFUEL/refuelSkyriderGridNo`

- **Type:** `Integer` (`INT32`)
- **Description:** Номер клетки карты, где Скайрайдер стоит в точке дозаправки. Это идентификатор
  клетки.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (helicopter refuel-site loader); map table data.
- **Confidence:** `source`

## Relations

- `refuelSector/x` и `refuelSector/y` задают сектор, названия и погода которого определены в
  [SectorNames](SectorNames.md).

## Localization

Отображаемые имена, подсказки и описания локализуются через языковые оверлеи; числовые ID, коды
секторов и значения матриц — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблиц карты.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания основаны на именах полей и наблюдаемых значениях таблиц; формула времени выполнения не
  заявляется.

## Open questions

- Точная формула или сопоставление перечисления для полей, помеченных как выведенные, в данных
  таблицы не указаны.
