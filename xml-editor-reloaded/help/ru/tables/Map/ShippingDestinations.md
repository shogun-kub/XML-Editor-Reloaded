---
schema: 1
lang: ru
family: map-shipping-destinations
title: "ShippingDestinations — пункты назначения доставки"
tables:
  - ShippingDestinations
table_ids:
  - Map.ShippingDestinations
files:
  - Map/ShippingDestinations.xml
root: DESTINATIONLIST
record: DESTINATION
key: uiIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# ShippingDestinations — пункты назначения доставки

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`DESTINATIONLIST/DESTINATION/uiIndex`](#destinationlistdestinationuiindex)
  - [`DESTINATIONLIST/DESTINATION/name`](#destinationlistdestinationname)
  - [`DESTINATIONLIST/DESTINATION/ubMapX`](#destinationlistdestinationubmapx)
  - [`DESTINATIONLIST/DESTINATION/ubMapY`](#destinationlistdestinationubmapy)
  - [`DESTINATIONLIST/DESTINATION/ubMapZ`](#destinationlistdestinationubmapz)
  - [`DESTINATIONLIST/DESTINATION/sGridNo`](#destinationlistdestinationsgridno)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`ShippingDestinations.xml` перечисляет места, куда Bobby Ray может отправлять грузы. Зарубежным
пунктам назначения нужно только название; местные пункты дополнительно несут координаты карты и
номер клетки, куда прибывает груз.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `ShippingDestinations` | `Map/ShippingDestinations.xml` | — |

## Structure

```
DESTINATIONLIST
└── DESTINATION
    ├── uiIndex
    ├── name
    ├── ubMapX
    ├── ubMapY
    ├── ubMapZ
    └── sGridNo
```

## Fields

### `DESTINATIONLIST/DESTINATION/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс пункта назначения доставки. Это ключ массива, на него ссылается
  `DeliveryMethods`.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** Обязательно; идентифицирует запись или строку.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/name`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое название пункта назначения, например `Drassen` или `London`.
- **Values:** Свободный текст, усекается до `MAX_DEST_NAME_LENGTH`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/ubMapX`

- **Type:** `Integer` (`UINT8`)
- **Description:** Столбец карты местного пункта назначения. Меньшие значения означают дальше на
  запад; для зарубежных пунктов отсутствует.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/ubMapY`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка карты местного пункта назначения. Меньшие значения означают дальше на
  север; для зарубежных пунктов отсутствует.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/ubMapZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Уровень карты местного пункта назначения; `0` — поверхность. Это уровень.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

### `DESTINATIONLIST/DESTINATION/sGridNo`

- **Type:** `Integer` (`UINT32`)
- **Description:** Номер клетки карты, куда прибывает местный груз. Это идентификатор клетки.
- **Values:** `0..4294967295`; номер клетки карты.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (shipping-destinations loader); map table data.
- **Confidence:** `source`

## Relations

- [DeliveryMethods](DeliveryMethods.md) ссылается на эту таблицу по `uiIndex`, чтобы задать плату и
  задержку для каждого пункта назначения.

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
