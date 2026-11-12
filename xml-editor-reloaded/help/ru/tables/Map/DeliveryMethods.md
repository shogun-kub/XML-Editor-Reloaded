---
schema: 1
lang: ru
family: map-delivery-methods
title: "DeliveryMethods — способы доставки, плата и задержки"
tables:
  - DeliveryMethods
table_ids:
  - Map.DeliveryMethods
files:
  - Map/DeliveryMethods.xml
root: DELIVERYMETHODTABLE
record: DELIVERYMETHOD
key: uiIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# DeliveryMethods — способы доставки, плата и задержки

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/uiIndex`](#deliverymethodtabledeliverymethoduiindex)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/description`](#deliverymethodtabledeliverymethoddescription)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/uiDestinationIndex`](#deliverymethodtabledeliverymethoddestinationdeliveryinfotabledestinationdeliveryinfouidestinationindex)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/usDestinationFee`](#deliverymethodtabledeliverymethoddestinationdeliveryinfotabledestinationdeliveryinfousdestinationfee)
  - [`DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/bDaysAhead`](#deliverymethodtabledeliverymethoddestinationdeliveryinfotabledestinationdeliveryinfobdaysahead)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`DeliveryMethods.xml` определяет каждый вариант доставки Bobby Ray и для каждого пункта назначения — его плату и задержку. Индекс пункта назначения ссылается на `ShippingDestinations.uiIndex`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `DeliveryMethods` | `Map/DeliveryMethods.xml` | — |

## Structure

```
DELIVERYMETHODTABLE
└── DELIVERYMETHOD
    ├── uiIndex
    ├── description
    └── DESTINATIONDELIVERYINFOTABLE
        └── DESTINATIONDELIVERYINFO
            ├── uiDestinationIndex
            ├── usDestinationFee
            └── bDaysAhead
```

## Fields

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс способа доставки, хранящийся в XML, но загрузчик его не читает: способы сохраняются в порядке документа, а элемент игнорируется.
- **Values:** Целое число; изменение значения не меняет порядок сохранённых способов.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/description`

- **Type:** `String` (`CHAR16`)
- **Description:** Название способа доставки, отображаемое игроку, например `Overnight express`.
- **Values:** Свободный текст, усекается до `MAX_DELIVERYMETHOD_DESC_LENGTH`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/uiDestinationIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс пункта назначения доставки, к которому относятся эта цена и задержка. Это ID.
- **Values:** `0..4294967295`; ID пункта назначения.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/usDestinationFee`

- **Type:** `Integer` (`UINT16`)
- **Description:** Плата за доставку в этот пункт назначения, в деньгах.
- **Numeric semantics:** больше = дороже; отрицательные значения не имеют смысла.
- **Values:** `0..65535` денег.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

### `DELIVERYMETHODTABLE/DELIVERYMETHOD/DESTINATIONDELIVERYINFOTABLE/DESTINATIONDELIVERYINFO/bDaysAhead`

- **Type:** `Integer` (`INT8`)
- **Description:** Количество дней, за которое приходит груз.
- **Numeric semantics:** больше = медленнее; загрузчик сохраняет это поле как `INT8`.
- **Values:** `-128..127` дней.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`
## Relations

- `uiDestinationIndex` ссылается на `uiIndex` в [ShippingDestinations](ShippingDestinations.md); название пункта назначения определено там.

## Localization

Отображаемые имена, подсказки и описания локализуются через языковые оверлеи; числовые ID, коды секторов и значения матриц — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблиц карты.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания основаны на именах полей и наблюдаемых значениях таблиц; формула времени выполнения не заявляется.

## Open questions

- Точная формула или сопоставление перечисления для полей, помеченных как выведенные, в данных таблицы не указаны.
`

- **Type:** `Integer` (`INT8`)
- **Description:** Количество дней, за которое приходит груз.
- **Numeric semantics:** больше = медленнее; отрицательные значения не имеют смысла.
- **Values:** `-128..127` дней; загрузчик сохраняет это поле как `INT8`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (delivery-methods loader); map table data.
- **Confidence:** `source`

## Relations

- `uiDestinationIndex` ссылается на `uiIndex` в [ShippingDestinations](ShippingDestinations.md); название пункта назначения определено там.

## Localization

Отображаемые имена, подсказки и описания локализуются через языковые оверлеи; числовые ID, коды секторов и значения матриц — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблиц карты.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания основаны на именах полей и наблюдаемых значениях таблиц; формула времени выполнения не заявляется.

## Open questions

- Точная формула или сопоставление перечисления для полей, помеченных как выведенные, в данных таблицы не указаны.
