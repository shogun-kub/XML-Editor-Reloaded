---
schema: 1
lang: ru
family: civ-group-names
title: "CivGroupNames — названия и флаги гражданских групп"
tables:
  - CivGroupNames
table_ids:
  - CivGroupNames
files:
  - CivGroupNames.xml
root: CIV_GROUP_NAMES
record: NAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# CivGroupNames — названия и флаги гражданских групп

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CIV_GROUP_NAMES/NAME/uiIndex`](#civ_group_namesnameuiindex)
  - [`CIV_GROUP_NAMES/NAME/Enabled`](#civ_group_namesnameenabled)
  - [`CIV_GROUP_NAMES/NAME/szGroup`](#civ_group_namesnameszgroup)
  - [`CIV_GROUP_NAMES/NAME/AddToBattle`](#civ_group_namesnameaddtobattle)
  - [`CIV_GROUP_NAMES/NAME/Loyalty`](#civ_group_namesnameloyalty)
  - [`CIV_GROUP_NAMES/NAME/fCanBeCaptured`](#civ_group_namesnamefcanbecaptured)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`CivGroupNames.xml` задаёт названия гражданских фракций и управляет тем, вступают ли они в бой,
считаются ли лояльными и можно ли их захватывать.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `CivGroupNames` | `CivGroupNames.xml` | — |

## Structure

```
CIV_GROUP_NAMES
└── NAME
    ├── uiIndex
    ├── Enabled
    ├── szGroup
    ├── AddToBattle
    ├── Loyalty
    └── fCanBeCaptured
```

## Fields

### `CIV_GROUP_NAMES/NAME/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс гражданской группы. Это ключ массива.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (`XML_CivGroupNames.cpp`); table data.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/Enabled`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, гражданские этой группы отображают `szGroup` как своё имя в
  тактическом режиме; `0` сохраняет стандартную гражданскую метку. Эффект виден только при
  включённой игровой настройке `INDIVIDUAL_CIVILIAN_NAMES`.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_CivGroupNames.cpp`, `Interface.cpp`); table XML comment.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/szGroup`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое название гражданской группы, используемое, когда установлен
  `Enabled`.
- **Values:** Свободный текст.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_CivGroupNames.cpp`); table data.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/AddToBattle`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, все члены этой группы учитываются, когда происходит бой с этой
  группой.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_CivGroupNames.cpp`, `Overhead.cpp`); table XML comment.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/Loyalty`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, убийство члена этой группы может вызвать штраф к лояльности
  города/морали; `0` подавляет этот штраф для группы.
- **Values:** `0` или `1`; `0` = нет штрафа к лояльности/морали за убийство членов группы.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`Strategic Town Loyalty.cpp`); table XML comment.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/fCanBeCaptured`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, враждебных членов этой группы можно захватывать и
  допрашивать.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_CivGroupNames.cpp`, `Soldier Control.cpp`); table XML comment.
- **Confidence:** `source`

## Relations

- `uiIndex` упоминается в поле гражданской группы в [MercProfiles](MercProfiles.md).

## Localization

Отображаемые имена и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, комментарии XML и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не утверждается.

## Open questions

- Точная формула или сопоставление перечисления для выведенных полей не указаны в данных таблицы.
