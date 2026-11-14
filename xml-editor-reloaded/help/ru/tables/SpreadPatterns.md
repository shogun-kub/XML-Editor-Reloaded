---
schema: 1
lang: ru
family: spread-patterns
title: "SpreadPatterns — шаблоны разлёта дроби"
tables:
  - SpreadPatterns
table_ids:
  - SpreadPatterns
files:
  - SpreadPatterns.xml
root: SPREADPATTERNLIST
record: SPREADPATTERN
key: NAME
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# SpreadPatterns — шаблоны разлёта дроби

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SPREADPATTERNLIST/SPREADPATTERN/NAME`](#spreadpatternlistspreadpatternname)
  - [`SPREADPATTERNLIST/SPREADPATTERN/DESC`](#spreadpatternlistspreadpatterndesc)
  - [`SPREADPATTERNLIST/SPREADPATTERN/xspread`](#spreadpatternlistspreadpatternxspread)
  - [`SPREADPATTERNLIST/SPREADPATTERN/yspread`](#spreadpatternlistspreadpatternyspread)
  - [`SPREADPATTERNLIST/SPREADPATTERN/method`](#spreadpatternlistspreadpatternmethod)
  - [`SPREADPATTERNLIST/SPREADPATTERN/p/x`](#spreadpatternlistspreadpatternpx)
  - [`SPREADPATTERNLIST/SPREADPATTERN/p/y`](#spreadpatternlistspreadpatternpy)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SpreadPatterns.xml` определяет шаблоны разлёта дроби: явные углы дробин плюс необязательные случайные диапазоны `xspread`/`yspread` и форму распределения. Первый шаблон используется по умолчанию, когда не выбран другой.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `SpreadPatterns` | `SpreadPatterns.xml` | — |

## Structure

```
SPREADPATTERNLIST
└── SPREADPATTERN
    ├── NAME
    ├── DESC
    ├── xspread
    ├── yspread
    ├── method
    └── p
        ├── x
        └── y
```

## Fields

### `SPREADPATTERNLIST/SPREADPATTERN/NAME`

- **Type:** `String` (`CHAR8`)
- **Description:** Имя шаблона разлёта, используемое другими XML-файлами. Имя сравнивается без учёта регистра; ведущие и замыкающие пробелы обрезаются при загрузке.
- **Values:** Свободный текст; загрузчик сохраняет не более `SPREADPATTERN_NAME_SIZE - 1` символов.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/DESC`

- **Type:** `String`
- **Description:** Справочное описание шаблона. Загрузчик его не читает, и во время выполнения оно ни на что не влияет.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/xspread`

- **Type:** `Decimal` (`DOUBLE`)
- **Description:** Максимальный горизонтальный случайный разброс в радианах. Загрузчик применяет `fabs()`, поэтому отрицательное значение сохраняется как его абсолютная величина.
- **Numeric semantics:** больше = шире разброс; `0` отключает горизонтальную рандомизацию.
- **Values:** Неотрицательные радианы после загрузки.
- **Default:** Отсутствует; шаблон начинается с `0.0`.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/yspread`

- **Type:** `Decimal` (`DOUBLE`)
- **Description:** Максимальный вертикальный случайный разброс в радианах. Загрузчик применяет `fabs()`, поэтому отрицательное значение сохраняется как его абсолютная величина.
- **Numeric semantics:** больше = шире разброс; `0` отключает вертикальную рандомизацию.
- **Values:** Неотрицательные радианы после загрузки.
- **Default:** Отсутствует; шаблон начинается с `0.0`.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/method`

- **Type:** `String`
- **Description:** Форма распределения случайных дробин. Загрузчик сравнивает это значение **с учётом регистра** с `RECTANGLE`, `DIAMOND` и `ELLIPSE`; если значение не совпало, остаётся значение по умолчанию `ELLIPSE`.
- **Numeric semantics:** Текст, подобный перечислению; регистр и написание должны точно совпадать с именами в загрузчике, записанными в верхнем регистре.
- **Values:** `RECTANGLE`, `DIAMOND` или `ELLIPSE`. Написания `Rectangle`/`Diamond`/`Ellipse` из XML-комментария не совпадают и приводят к значению по умолчанию.
- **Default:** Отсутствует; загрузчик оставляет `ELLIPSE` (`giSpreadPatternMethod_Default`).
- **Source:** game code (`XML_SpreadPatterns.cpp`, `LOS.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/p/x`

- **Type:** `Decimal` (`DOUBLE`)
- **Description:** Горизонтальный угол одной явной дробины в радианах. Положительные значения отклоняются в одну сторону, отрицательные — в другую; значение знаковое.
- **Values:** Знаковый дробный угол.
- **Default:** Отсутствует; запись дробины начинается с `0.0`.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

### `SPREADPATTERNLIST/SPREADPATTERN/p/y`

- **Type:** `Decimal` (`DOUBLE`)
- **Description:** Вертикальный угол одной явной дробины в радианах. Положительные значения отклоняются в одну сторону, отрицательные — в другую; значение знаковое.
- **Values:** Знаковый дробный угол.
- **Default:** Отсутствует; запись дробины начинается с `0.0`.
- **Source:** game code (`XML_SpreadPatterns.cpp`); table XML comment.
- **Confidence:** `source`

## Relations

- `NAME` используется в [AmmoTypes](Items/AmmoTypes.md), [Weapons](Items/Weapons.md) и [Items](Items/Items.md), когда они выбирают шаблон разлёта.

## Localization

Отображаемые названия и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не заявлена.

## Open questions

- Точная формула или сопоставление перечисления для выведенных полей не указаны в данных таблицы.
