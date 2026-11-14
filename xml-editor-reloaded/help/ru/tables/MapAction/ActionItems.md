---
schema: 1
lang: ru
family: map-action-items
title: "ActionItems — предметы действий на карте"
tables:
  - ActionItems
table_ids:
  - MapAction.ActionItems
files:
  - MapAction/ActionItems.xml
root: ACTION_ITEM_LIST
record: ACTION_ITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# ActionItems — предметы действий на карте

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/uiIndex`](#action_item_listaction_itemuiindex)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/Name`](#action_item_listaction_itemname)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/ActionID`](#action_item_listaction_itemactionid)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/Blow_up`](#action_item_listaction_itemblow_up)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/BombItem`](#action_item_listaction_itembombitem)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`ActionItems.xml` сопоставляет ID действий с отображаемыми именами, флагом взрыва и
предметом-бомбой, используемым действием. Это палитра действий, используемая редактором карт и
сценарными событиями карты.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `ActionItems` | `MapAction/ActionItems.xml` | — |

## Structure

```
ACTION_ITEM_LIST
└── ACTION_ITEM
    ├── uiIndex
    ├── Name
    ├── ActionID
    ├── Blow_up
    └── BombItem
```

## Fields

### `ACTION_ITEM_LIST/ACTION_ITEM/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс предмета действия. Это ключ массива.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (action-item loader); table data.
- **Confidence:** `source`

### `ACTION_ITEM_LIST/ACTION_ITEM/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое название действия, например `Klaxon Mine` или `Open Door`.
- **Values:** Свободный текст.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (action-item loader); table data.
- **Confidence:** `source`

### `ACTION_ITEM_LIST/ACTION_ITEM/ActionID`

- **Type:** `Integer` (`UINT8`)
- **Description:** Код типа действия, выполняемого этой записью. Это код перечисления из
  `Action Items.h` (`ACTION_ITEM_*`).
- **Values:** `1..255`; `1` = открыть дверь, `2` = закрыть дверь, `3` = взорвать
  (`ACTION_ITEM_BLOW_UP`), а остальные значения выбирают другие типы действий.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (`Action Items.h`, action-item loader); table data.
- **Confidence:** `source`

### `ACTION_ITEM_LIST/ACTION_ITEM/Blow_up`

- **Type:** `Boolean` (`UINT8`)
- **Only used with:** `ActionID = 3` (`ACTION_ITEM_BLOW_UP`); иначе загрузчик сохраняет `0`.
- **Description:** Если задано, действие является действием взрыва или сноса. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (action-item loader); table data.
- **Confidence:** `source`

### `ACTION_ITEM_LIST/ACTION_ITEM/BombItem`

- **Type:** `Integer` (`UINT16`)
- **Only used with:** `ActionID = 3` (`ACTION_ITEM_BLOW_UP`); иначе загрузчик сохраняет `0`.
- **Description:** ID предмета, используемого как бомба или взрывчатка для этого действия. Это
  ссылка на предмет.
- **Values:** ID предмета; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (action-item loader); table data.
- **Confidence:** `source`

## Relations

- `BombItem` ссылается на ID предмета в [Items](../Items/Items.md).

## Localization

Отображаемые имена и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не заявляется.

## Open questions

- Точная формула или сопоставление перечисления для выведенных полей в данных таблицы не указаны.
