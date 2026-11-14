---
schema: 1
lang: ru
family: items-armours
title: "Armours — броня для тела"
tables:
  - Armours
table_ids:
  - Items.Armours
files:
  - Items/Armours.xml
root: ARMOURLIST
record: ARMOUR
key: uiIndex
sources:
  - Game code (armour loader and protection)
  - Armour table data
status: reviewed
updated: 2026-11-12
---

# Armours — броня для тела

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ARMOURLIST/ARMOUR/uiIndex`](#armourlistarmouruiindex)
  - [`ARMOURLIST/ARMOUR/ubArmourClass`](#armourlistarmourubarmourclass)
  - [`ARMOURLIST/ARMOUR/ubProtection`](#armourlistarmourubprotection)
  - [`ARMOURLIST/ARMOUR/ubCoverage`](#armourlistarmourubcoverage)
  - [`ARMOURLIST/ARMOUR/ubDegradePercent`](#armourlistarmourubdegradepercent)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Armours.xml` определяет защитные значения брони для тела. Предмет-броня в `Items`
указывает на одну строку через свой `ubClassIndex`; строка хранится в таблице брони
времени выполнения. Значения определяют снижение урона, износ брони, фильтры Bobby Ray
и панель описания предмета.

`Armours` — плоская таблица: одна запись `ARMOUR` на каждый тип брони, с ключом `uiIndex`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Armours` | `Items/Armours.xml` | В v4 и v5 одинаковая схема; значения могут различаться |

## Structure

```
ARMOURLIST
└── ARMOUR
    ├── uiIndex
    ├── ubArmourClass
    ├── ubProtection
    ├── ubCoverage
    └── ubDegradePercent
```

Загрузчик записывает каждую запись в `Armour[uiIndex]`, когда элемент `ARMOUR` закрывается.
Повторяющийся `uiIndex` перезаписывает более раннюю строку. Записи с `uiIndex` за пределами
массива игнорируются.

## Fields

### `ARMOURLIST/ARMOUR/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс строки в таблице брони времени выполнения и идентификатор записи
  XML. Сюда указывает `Items.ubClassIndex` предмета-брони.
- **Values:** `0..N-1`; неотрицательный индекс массива. Загрузчик игнорирует значения за
  пределами массива.
- **Default:** нет; при отсутствии значения слот таблицы остаётся нулевым.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubArmourClass`

- **Type:** `Integer` (`UINT8`), enum `ARMOURCLASS_*`
- **Description:** Слот тела/класс брони. Это не значение защиты; он сообщает интерфейсу и
  проверкам снаряжения, является ли броня шлемом, жилетом, штанами или пластиной.
- **Values:** `0` шлем, `1` жилет, `2` штаны, `3` пластина, `4` монстр, `5` машина.
- **Default:** `0` (шлем), если элемент отсутствует, потому что запись сначала обнуляется.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubProtection`

- **Type:** `Integer` (`UINT16` in the struct, cast to `UINT8` by the loader)
- **Description:** Необработанное значение защиты брони. Загрузчик приводит разобранное значение к `UINT8`, поэтому значения выше 255 усекаются, хотя поле структуры имеет тип `UINT16`.
- **Numeric semantics:** Большие значения означают большую защиту/покрытие, отрицательные значения не имеют смысла.
- **Values:** Фактически `0..255`; загрузчик усекает до `UINT8`.
- **Default:** `0`.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubCoverage`

- **Type:** `Integer` (`UINT8`), percent
- **Description:** Процент слота тела, покрытого бронёй. Используется как множитель в расчётах защиты и в панели сравнения предметов.
- **Numeric semantics:** Большие значения означают большую защиту/покрытие, отрицательные значения не имеют смысла.
- **Values:** процент; хранится как `UINT8`. Отдельное ограничение валидации не документировано.
- **Default:** `100`, когда `ENABLE_ARMOR_COVERAGE` равно false. Опция по умолчанию `FALSE`,
  поэтому в игре с настройками по умолчанию каждая строка принудительно получает 100, а значение
  XML игнорируется. Когда покрытие включено и элемент отсутствует, обнулённая запись даёт `0`.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

### `ARMOURLIST/ARMOUR/ubDegradePercent`

- **Type:** `Integer` (`UINT8`), percent
- **Description:** Процент применённой защиты, вычитаемый из состояния брони, когда она поглощает попадание.
- **Numeric semantics:** Процент; большие значения быстрее изнашивают броню, `0` означает отсутствие износа, а отрицательные значения не имеют смысла для этого беззнакового процента.
- **Values:** процент; хранится как `UINT8`. Отдельное ограничение валидации не документировано.
- **Default:** `0`.
- **Source:** game code (armour loader); related: [Armour protection](../../FORMULAS.md#armour-protection).
- **Confidence:** `source`

## Relations

- `Items.ubClassIndex` -> `Armours.uiIndex` для предметов, у которых `usItemClass` равен
  `IC_ARMOUR`.
- `Armours.ubArmourClass` -> `ArmourClass` (`../Lookup.md#armourclass`).
- `usItemClass` и `ubClassIndex` предмета находятся в [Items](Items.md).

## Related formulas

У многоступенчатой модели урона есть отдельные главы в [Related formulas](../../FORMULAS.md):

- [Armour protection](../../FORMULAS.md#armour-protection) — покрытие, состояние, пробитие
  боеприпасом и износ.
- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt) — как результат
  по броне превращается в итоговый урон.

Приведённые выше описания полей определяют входные данные; формулы здесь не повторяются.

## Localization

В этой таблице нет строк. Названия и описания брони — поля `Items` и переводятся там.

## Versions

В v4 и v5 используются одни и те же пять полей и одинаковая структура `ARMOURLIST/ARMOUR`.
Значения могут различаться; сравнивайте два файла, когда важно конкретное значение. Ни один
оверлей не покрывает `Items/Armours.xml`.

## Sources

- Игровой код: загрузчик брони, проход защиты при попадании и настройка предмета, управляющая
  покрытием.
- Данные таблицы: значения `Items/Armours.xml`.
- Расчёт защиты и износа описан в [Related formulas](../../FORMULAS.md#armour-protection).

## Open questions

- Точная формула урона и то, как сочетаются `ubProtection`, `ubCoverage` и тип боеприпаса,
  описаны в [Related formulas](../../FORMULAS.md); полное обсуждение баланса выходит за рамки
  этой страницы таблицы.
- Обработка повторяющегося `uiIndex` в загрузчике — «побеждает последняя запись»; последствие
  дубликата такое же, как при перезаписи более ранней строки любой более поздней.
