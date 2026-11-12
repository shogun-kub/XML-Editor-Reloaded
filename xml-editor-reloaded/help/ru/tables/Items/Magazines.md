---
schema: 1
lang: ru
family: items-magazines
title: "Magazines — магазины и предметы боеприпасов"
tables:
  - Magazines
table_ids:
  - Items.Magazines
files:
  - Items/Magazines.xml
root: MAGAZINELIST
record: MAGAZINE
key: uiIndex
sources:
  - Game code (magazine loader and ammo lookups)
  - Ammo type and calibre data
status: reviewed
updated: 2026-11-12
---

# Magazines — магазины и предметы боеприпасов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MAGAZINELIST/MAGAZINE/uiIndex`](#magazinelistmagazineuiindex)
  - [`MAGAZINELIST/MAGAZINE/ubCalibre`](#magazinelistmagazineubcalibre)
  - [`MAGAZINELIST/MAGAZINE/ubMagSize`](#magazinelistmagazineubmagsize)
  - [`MAGAZINELIST/MAGAZINE/ubAmmoType`](#magazinelistmagazineubammotype)
  - [`MAGAZINELIST/MAGAZINE/ubMagType`](#magazinelistmagazineubmagtype)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Magazines.xml` определяет предметы боеприпасов: магазины, отдельные патроны, коробки и ящики
боеприпасов. Предмет, у которого `usItemClass` равен `IC_AMMO`, указывает на строку через
`ubClassIndex`; строка хранится во время выполнения в массиве `Magazine[]`. Строка решает, какое
оружие может заряжаться этим предметом (`ubCalibre`, `ubMagSize`, `ubAmmoType`), и как предмет
ведёт себя в оптовом виде (`ubMagType`).

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Magazines` | `Items/Magazines.xml` | в v4 и v5 одинаковая схема; значения могут различаться |

## Structure

```
MAGAZINELIST
└── MAGAZINE
    ├── uiIndex
    ├── ubCalibre
    ├── ubMagSize
    ├── ubAmmoType
    └── ubMagType
```

Загрузчик записывает каждую запись в `Magazine[uiIndex]`. Записи вне массива игнорируются. Ключ
редактора — `uiIndex`; после загрузки поле `MAGTYPE.uiIndex` времени выполнения используется как
обратная ссылка на ID владеющего предмета, поэтому не читайте поле времени выполнения как ключ XML.

## Fields

### `MAGAZINELIST/MAGAZINE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Идентичность в XML и индекс в массиве `Magazine[]` времени выполнения. `Items`
  использует `ubClassIndex`, чтобы указывать на эту строку для предметов боеприпасов.
- **Values:** `0..MAXITEMS-1`; неотрицательный индекс массива.
- **Default:** нет; при отсутствующем значении слот массива остаётся нулевым.
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubCalibre`

- **Type:** `Integer` (`UINT8`)
- **Description:** Группа калибра боеприпаса. Игра использует её вместе с `ubMagSize` и
  `ubAmmoType`, чтобы найти подходящий магазин для оружия и назвать калибр в интерфейсе. Она
  индексирует список `AmmoStrings`, который заполняется загрузчиком калибров в порядке файла
  (элемент `uiIndex` этим загрузчиком не читается).
- **Values:** Индекс `UINT8`; файл `AmmoStrings` предоставляет название для каждого используемого
  калибра.
- **Default:** `0` (нет калибра).
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubMagSize`

- **Type:** `Integer` (`UINT16`)
- **Description:** Ёмкость магазина в патронах. Отдельные патроны используют 1; коробки и ящики
  используют большую ёмкость. Загрузчик приводит значение к `UINT16`.
- **Numeric semantics:** Большие значения означают большее количество; отрицательные значения не
  имеют смысла.
- **Values:** `0..65535`; загрузчик приводит значение к `UINT16`.
- **Default:** `0`.
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubAmmoType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс типа боеприпаса в `AmmoTypes`. Он выбирает урон, взаимодействие с бронёй,
  трассер и другие свойства снаряда.
- **Values:** Индекс `UINT8`; допустимые ID — строки `AmmoTypes`.
- **Default:** `0` (обычный боеприпас).
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

### `MAGAZINELIST/MAGAZINE/ubMagType`

- **Type:** `Integer` (`UINT8`), enum `AMMO_*`
- **Description:** Вид предмета боеприпасов. Значения `AMMO_BOX` и выше считаются оптовыми
  боеприпасами и используют другие правила инвентаря, перезарядки и переноски.
- **Values:** `0` магазин (`AMMO_MAGAZINE`), `1` патрон или патроны (`AMMO_BULLET`), `2` коробка
  (`AMMO_BOX`), `3` ящик (`AMMO_CRATE`).
- **Default:** `0` (магазин).
- **Source:** game code (magazine loader); related: [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt).
- **Confidence:** `source`

## Relations

- [Items](Items.md) `ubClassIndex` -> `Magazines.uiIndex` для предметов `IC_AMMO`.
- `Magazines.ubCalibre` -> [AmmoStrings](AmmoStrings.md) (название калибра), заполняется в порядке
  файла.
- `Magazines.ubAmmoType` -> [AmmoTypes](AmmoTypes.md) `uiIndex`.
- `Magazines.ubMagType` -> [Lookup.MagazineType](../Lookup.md#magazinetype).
- [Weapons](Weapons.md) использует ту же тройку «калибр — ёмкость — тип боеприпаса», чтобы найти
  магазин.

## Related formulas

Значения боеприпасов питают многоступенчатую модель урона, описанную в
[Related formulas](../../FORMULAS.md):

- [Armour protection](../../FORMULAS.md#armour-protection) — как `ubAmmoType` меняет значение
  защиты и износ брони.
- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt) — где `ubAmmoType`
  и количество снарядов из типа боеприпаса входят в результат урона.

Записи полей выше определяют входные данные; они не повторяют формулы.

## Localization

В этой таблице нет строк. Названия калибров находятся в `AmmoStrings`, их предоставляют языковые
файлы игры; сама таблица не покрывается оверлеями `Russian.*`.

## Versions

В v4 и v5 используются одни и те же пять полей и одинаковая структура `MAGAZINELIST/MAGAZINE`.
Значения различаются; сравнивайте файлы, когда важен конкретный магазин.

## Sources

- Игровой код: загрузчик магазинов и код поиска боеприпасов и калибров.
- Данные таблиц: `Items/Magazines.xml`, `Items/AmmoStrings.xml`, `Items/AmmoTypes.xml`.
- Взаимодействие урона и брони: [Related formulas](../../FORMULAS.md).

## Open questions

- `ubCalibre` — это индекс в списке, заполняемом в порядке файла, а не явный ID. Переупорядочивание
  `AmmoStrings.xml` молча изменило бы каждое название калибра. Это риск целостности данных, о
  котором стоит сообщать, но загрузчик является авторитетным.
- Точное правило подбора боеприпаса (калибр, ёмкость, тип боеприпаса) находится в коде поиска
  предметов; эта страница документирует поля, а не алгоритм поиска.
