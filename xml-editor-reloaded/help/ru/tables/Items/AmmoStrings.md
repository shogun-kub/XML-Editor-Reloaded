---
schema: 1
lang: ru
family: items-ammo
title: "AmmoStrings — названия калибров"
tables:
  - AmmoStrings
table_ids:
  - Items.AmmoStrings
files:
  - Items/AmmoStrings.xml
root: AMMOLIST
record: AMMO
key: uiIndex
sources:
  - Ammo string table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# AmmoStrings — названия калибров

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`AMMOLIST/AMMO/uiIndex`](#ammolistammouiindex)
  - [`AMMOLIST/AMMO/AmmoCaliber`](#ammolistammoammocaliber)
  - [`AMMOLIST/AMMO/BRCaliber`](#ammolistammobrcaliber)
  - [`AMMOLIST/AMMO/NWSSCaliber`](#ammolistammonwsscaliber)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AmmoStrings.xml` сопоставляет числовой индекс калибра тексту, отображаемому для этого калибра. Оружие и магазины хранят индекс; строки в этой таблице — видимые пользователю подписи.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `AmmoStrings` | `Items/AmmoStrings.xml` | В v5 добавлен `NWSSCaliber`; записи v4 не содержат этого элемента. |

## Structure

```
AMMOLIST
└── AMMO
    ├── uiIndex
    ├── AmmoCaliber
    ├── BRCaliber
    └── NWSSCaliber
```

## Game options

Для этой таблицы не документировано глобальной опции уровня таблицы. Её поля читаются в обеих моделях инвентаря и в многопользовательской игре; единственное ограничение по версии — набор полей, описанный в разделе [Versions](#versions).

## Fields

### `AMMOLIST/AMMO/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс калибра, на который ссылаются `Weapons.ubCalibre` и строки магазинов.
- **Values:** Неотрицательное целое; уникально в пределах файла. Сюда указывает `Weapons.ubCalibre`.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** ammo string table data; field metadata.
- **Confidence:** `source`

### `AMMOLIST/AMMO/AmmoCaliber`

- **Type:** `String` (`String`)
- **Description:** Основная отображаемая подпись калибра. Это технический текст, а не переводимое предложение.
- **Values:** Свободный текст.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (AmmoStrings loader); ammo string table data.
- **Confidence:** `source`

### `AMMOLIST/AMMO/BRCaliber`

- **Type:** `String` (`String`)
- **Description:** Подпись калибра, используемая оружейным магазином Bobby Ray.
- **Values:** Свободный текст.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (AmmoStrings loader); ammo string table data.
- **Confidence:** `source`

### `AMMOLIST/AMMO/NWSSCaliber`

- **Type:** `String` (`String`)
- **Description:** Альтернативное написание калибра, используемое звуковым путём NWS. Записи v4 не содержат этого элемента.
- **Values:** Свободный текст.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (AmmoStrings loader); ammo string table data.
- **Confidence:** `source`

## Relations

- `Weapons.ubCalibre` указывает на `AmmoStrings.uiIndex`.
- Строки магазинов и оружия несут индекс калибра; три строки — альтернативные подписи одного и того же калибра.

## Related formulas

Ни одна формула в [FORMULAS](../../FORMULAS.md) не относится к этой таблице.

## Localization

Три строки калибра — отображаемые подписи. Это технические названия, и обычно они совпадают во всех языках.

## Versions

В v5 добавлен `NWSSCaliber`; строки v4 его не содержат. `AmmoCaliber` и `BRCaliber` есть в обеих версиях.

## Sources

- Данные таблицы названий калибров.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов; числовые ограничения используют диапазоны валидации метаданных.
- Семантические описания используют комментарии схемы таблицы и устаревшие подписи полей; записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

`BRCaliber` помечен как вариант для Bobby Ray; точный путь интерфейса, который его выбирает, не является частью схемы таблицы.
