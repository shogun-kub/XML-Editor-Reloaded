---
schema: 1
lang: ru
family: items-launchables
title: "Launchables — пары «пусковая установка — боеприпас»"
tables:
  - Launchables
table_ids:
  - Items.Launchables
files:
  - Items/Launchables.xml
root: LAUNCHABLELIST
record: LAUNCHABLE
key: launchableIndex + itemIndex
sources:
  - Game code (launcher loader)
  - Item table data
status: reviewed
updated: 2026-11-12
---

# Launchables — пары «пусковая установка — боеприпас»

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`LAUNCHABLELIST/LAUNCHABLE/launchableIndex`](#launchablelistlaunchablelaunchableindex)
  - [`LAUNCHABLELIST/LAUNCHABLE/itemIndex`](#launchablelistlaunchableitemindex)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Launchables.xml` перечисляет, какие предметы может запускать пусковая установка. Каждая запись — пара: предмет-пусковая установка и предмет-боеприпас/снаряд. Игра хранит пары в
`Launchable[MAXITEMS+1][2]` и использует их, когда гранатомёт, ракетная установка или похожее
оружие выбирает боеприпас.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Launchables` | `Items/Launchables.xml` | В v4 и v5 одни и те же два поля |

## Structure

```
LAUNCHABLELIST
└── LAUNCHABLE
    ├── launchableIndex
    └── itemIndex
```

Загрузчик не использует ключ; он добавляет записи в порядке файла в таблицу пар времени выполнения и
записывает число пар. Редактор использует пару (`launchableIndex`, `itemIndex`) как составной ключ.
Переупорядочивание записей меняет порядок времени выполнения, поэтому считайте порядок файла
значимым, пока не доказано обратное.

## Fields

### `LAUNCHABLELIST/LAUNCHABLE/launchableIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс предмета-пусковой установки в `Items.uiIndex`. Он определяет
  оружие, которое может запускать предмет с другой стороны пары.
- **Values:** допустимые значения `Items.uiIndex`; хранится как `UINT16`.
- **Default:** `0`; пара всё равно сохраняется, поэтому нулевое значение — ошибка данных, а не «нет».
- **Source:** game code (launcher loader); item table data.
- **Confidence:** `source`

### `LAUNCHABLELIST/LAUNCHABLE/itemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс предмета-боеприпаса/снаряда в `Items.uiIndex`.
  Это предмет, который запускает пусковая установка.
- **Values:** допустимые значения `Items.uiIndex`; хранится как `UINT16`.
- **Default:** `0`; нулевое значение — ошибка данных.
- **Source:** game code (launcher loader); item table data.
- **Confidence:** `source`

## Relations

- `Launchables.launchableIndex` -> [Items](Items.md) `uiIndex` (пусковая установка).
- `Launchables.itemIndex` -> [Items](Items.md) `uiIndex` (боеприпас/снаряд).
- Сам предмет-пусковая установка — запись [Items](Items.md) с классом пусковой установки; предмет-боеприпас
  обычно имеет класс `IC_AMMO` или является взрывчатым предметом.

## Localization

Строк нет; оба поля — числовые индексы предметов.

## Versions

В v4 и v5 одна и та же структура `LAUNCHABLELIST/LAUNCHABLE` и одни и те же два поля. Значения
могут различаться; сравнивайте файлы, когда важна конкретная пара.

## Sources

- Игровой код: загрузчик пар пусковая установка/боеприпас.
- Данные таблицы: `Items/Launchables.xml`.

## Open questions

- Загрузчик добавляет в порядке файла и не удаляет дубликаты. В файле нет повторяющихся
  пар, но последствие дубликата во время выполнения не документировано.
- Какие классы пусковых установок используют таблицу во время выполнения, решается в коде предметов и оружия;
  эта страница документирует контракт данных, а не все места вызовов.
