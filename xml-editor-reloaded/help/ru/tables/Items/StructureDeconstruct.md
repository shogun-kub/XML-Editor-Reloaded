---
schema: 1
lang: ru
family: items-structures
title: "StructureDeconstruct — разбираемые строения"
tables:
  - StructureDeconstruct
table_ids:
  - Items.StructureDeconstruct
files:
  - Items/StructureDeconstruct.xml
root: STRUCTURESLIST
record: STRUCTURE
key: none
sources:
  - Structure deconstruction table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# StructureDeconstruct — разбираемые строения

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`STRUCTURESLIST/STRUCTURE/usDeconstructItem`](#structuresliststructureusdeconstructitem)
  - [`STRUCTURESLIST/STRUCTURE/usItemToCreate`](#structuresliststructureusitemtocreate)
  - [`STRUCTURESLIST/STRUCTURE/usCreatedItemStatus`](#structuresliststructureuscreateditemstatus)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`](#structuresliststructuresztilesetdisplayname)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetName`](#structuresliststructuresztilesetname)
  - [`STRUCTURESLIST/STRUCTURE/dCreationCost`](#structuresliststructuredcreationcost)
  - [`STRUCTURESLIST/STRUCTURE/allowedtile`](#structuresliststructureallowedtile)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`StructureDeconstruct.xml` определяет, как строение разбирается и какой предмет оно даёт: предмет
инструмента, создаваемый предмет и его состояние, набор тайлов, стоимость и разрешённые тайлы.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `StructureDeconstruct` | `Items/StructureDeconstruct.xml` | В v4 и v5 совпадают пути полей; значения различаются. `allowedtile` повторяется. |

## Structure

```
STRUCTURESLIST
└── STRUCTURE
    ├── usDeconstructItem
    ├── usItemToCreate
    ├── usCreatedItemStatus
    ├── szTileSetDisplayName
    ├── szTileSetName
    ├── dCreationCost
    └── allowedtile
```

У `STRUCTURESLIST` нет ключевого элемента. Загрузчик сохраняет строки в порядке документа; строка
выбирается по предмету разбора в руках (`usDeconstructItem`) и затем проверяется по имени набора
тайлов и разрешённому номеру строения, а при автоматическом сносе укреплений выбор идёт по имени
набора тайлов, и номер строения решает, разрешён ли снос.

## Fields

### `STRUCTURESLIST/STRUCTURE/usDeconstructItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, необходимого для разбора строения. Значение `0` не совпадает ни с одним настоящим предметом, поэтому отключает разбор по предмету для этой строки.
- **Values:** ID предмета; `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (structure-deconstruct loader; deconstruction item matching); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/usItemToCreate`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого при разборе строения. Предмет создаётся только тогда, когда и это поле, и `usCreatedItemStatus` не равны нулю.
- **Values:** ID предмета; `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует (предмет не создаётся); пустой элемент разбирается как `0`.
- **Source:** game code (structure-deconstruct loader; deconstruction result); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/usCreatedItemStatus`

- **Type:** `Integer` (`UINT8`)
- **Description:** Состояние (прочность), задаваемое создаваемому предмету, в процентах состояния предмета. Значение `0` означает, что предмет не создаётся; ненулевое значение передаётся в создание предмета как начальное состояние.
- **Numeric semantics:** Процентные пункты прочности предмета в шкале `objectStatus`. Вызов создания предмета защищается от нуля через `max(1, status)`, но сохранённое значение — беззнаковый байт, поэтому ненулевое значение уже не меньше `1`.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`, а код предметов трактует его как процент прочности (`0..100` — обычный диапазон).
- **Default:** `0`, если поле отсутствует (предмет не создаётся); пустой элемент разбирается как `0`.
- **Source:** game code (structure-deconstruct loader; item creation on deconstruction); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`

- **Type:** `String` (`CHAR8`)
- **Description:** Название строения, предназначенное для игрока (в комментарии к структуре оно названо именем строения, потому что имена наборов тайлов малопонятны). Загрузчик сохраняет его, но ни одного потребителя этого поля в игровой логике или интерфейсе в этом дереве исходников не найдено.
- **Values:** Свободный текст; сохраняется в 20-байтовом буфере без добавления завершающего нуля, поэтому более длинный текст обрезается и может заполнить буфер.
- **Default:** Пустая строка, если поле отсутствует.
- **Source:** game code (structure-deconstruct loader and `STRUCTURE_DECONSTRUCT` comment); table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/szTileSetName`

- **Type:** `String` (`CHAR8`)
- **Description:** Имя файла набора тайлов строения. Оно сравнивается без учёта регистра с именами файлов тайловой поверхности набора, сравниваются первые 11 символов.
- **Values:** Свободный текст, например `sandbag.sti`; сохраняется в 20-байтовом буфере без добавления завершающего нуля, поэтому более длинный текст обрезается.
- **Default:** Пустая строка, если поле отсутствует; пустое имя не совпадает ни с одним набором тайлов.
- **Source:** game code (structure-deconstruct loader; tileset matching); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/dCreationCost`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Стоимость сноса одного строения, выраженная в очках работы (прогресса) укрепления. Стоимость суммируется в максимальный прогресс укрепления сектора для плана сноса и вычитается по мере сноса каждого строения. Это не денежная величина.
- **Numeric semantics:** Дробные очки прогресса; большие значения означают больше работы. Загрузчик сохраняет разобранное значение без ограничения и без отказа.
- **Values:** Десятичное число (диапазона в коде нет).
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (structure-deconstruct loader; fortification progress accounting and consumption); structure deconstruction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/allowedtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Номер строения, которое можно снести по этой строке. Игра сравнивает его с `usStructureNumber` строения, приведённым к `UINT8`, после совпадения имени набора тайлов; элемент может повторяться, и совпадение любой записи разрешает снос.
- **Numeric semantics:** Номер строения (`usStructureNumber`) внутри набора тайлов; `0..255`.
- **Values:** `0..255`; элемент может повторяться.
- **Default:** Пустой список, если поле отсутствует (по этой строке нельзя снести ни одно строение); пустой элемент добавляет запись `0`.
- **Source:** game code (structure-deconstruct loader; deconstruction tile matching); structure deconstruction table data.
- **Confidence:** `source`

## Relations

- `usDeconstructItem` и `usItemToCreate` указывают на [Items](Items.md).
- Колонки набора тайлов ссылаются на строения в указанном наборе тайлов, а не на строки предметов.

## Localization

`szTileSetDisplayName` задуман как название строения для игрока, но в этом дереве исходников
потребитель не найден; `szTileSetName` — ключ ресурса.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы разбора строений.
- Код игры: загрузчик разбора строений, сопоставление предмета разбора, создание предмета, учёт
  прогресса укрепления.
- Значения по умолчанию из записи `STRUCTURE_DECONSTRUCT`.
- Описания охватывают значения, которые код читает, сохраняет и использует; каждое поле в этом
  документе подтверждено загрузчиком.

## Open questions

- Неизвестно, используется ли `szTileSetDisplayName` каким-либо интерфейсом вне этого дерева
  исходников; в поставляемом коде чтения этого поля не найдено.
- Количество очков строительства, создаваемых за один такт работы задания, и, следовательно,
  практический баланс значений `dCreationCost` определяются системой заданий вне этой таблицы.
