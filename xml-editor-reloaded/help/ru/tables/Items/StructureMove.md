---
schema: 1
lang: ru
family: items-structures
title: "StructureMove — перемещаемые тайлы строений"
tables:
  - StructureMove
table_ids:
  - Items.StructureMove
files:
  - Items/StructureMove.xml
root: STRUCTURESLIST
record: STRUCTURE
key: none
sources:
  - Structure move table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# StructureMove — перемещаемые тайлы строений

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`](#structuresliststructuresztilesetdisplayname)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetName`](#structuresliststructuresztilesetname)
  - [`STRUCTURESLIST/STRUCTURE/allowedtile`](#structuresliststructureallowedtile)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`StructureMove.xml` перечисляет тайлсеты, строения которых можно перемещать, и индексы тайлов, на
которых перемещение разрешено.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `StructureMove` | `Items/StructureMove.xml` | в v4 и v5 одинаковые пути полей; значения различаются. `allowedtile` повторяется. |

## Structure

```
STRUCTURESLIST
└── STRUCTURE
    ├── szTileSetDisplayName
    ├── szTileSetName
    └── allowedtile
```

У `STRUCTURESLIST` нет стабильного ключа на строку; строка строения идентифицируется своими полями
тайлсета и порядком документа.

## Fields

### `STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`

- **Type:** `String` (`CHAR8`, 20-byte buffer)
- **Description:** Отображаемое название тайлсета перемещаемого строения.
- **Values:** Свободный текст, усекается до 20 байт; явный завершающий нуль не добавляется, если
  ввод заполняет буфер.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (structure-move loader); table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/szTileSetName`

- **Type:** `String` (`CHAR8`, 20-byte buffer)
- **Description:** Файл STI-тайлсета, используемый для отрисовки перемещаемого строения.
- **Values:** Свободный текст, усекается до 20 байт; явный завершающий нуль не добавляется, если
  ввод заполняет буфер.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (structure-move loader); table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/allowedtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс тайла, на котором перемещение разрешено. Элемент может повторяться.
- **Values:** Разрешённый индекс тайла; элемент может повторяться.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (structure-move loader); table data.
- **Confidence:** `source`

## Relations

- Колонки тайлсетов ссылаются на ресурсы STI, а не на строки предметов.
- Перемещение строения не расходует предмет; пути возведения и удаления документированы в
  [StructureConstruct](StructureConstruct.md) и [StructureDeconstruct](StructureDeconstruct.md).

## Localization

`szTileSetDisplayName` — название тайлсета для игрока; `szTileSetName` — ключ ресурса.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы перемещения строений.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов.
- Описания используют имена XML-элементов, значения справочников и устаревшие метки полей; записи с
  пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Таблица перечисляет разрешённые индексы тайлов, но не задаёт стоимость перемещения или правила
столкновений.
