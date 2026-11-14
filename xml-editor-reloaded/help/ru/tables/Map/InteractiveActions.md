---
schema: 1
lang: ru
family: map-interactive-actions
title: "InteractiveActions — интерактивные действия тайлов"
tables:
  - InteractiveActions
table_ids:
  - Map.InteractiveActions
files:
  - Map/InteractiveActions.xml
root: INTERACTIVEACTIONS
record: ACTION
key: none
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# InteractiveActions — интерактивные действия тайлов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`INTERACTIVEACTIONS/ACTION/szTileSetName`](#interactiveactionsactionsztilesetname)
  - [`INTERACTIVEACTIONS/ACTION/usTileIndex`](#interactiveactionsactionustileindex)
  - [`INTERACTIVEACTIONS/ACTION/sActionType`](#interactiveactionsactionsactiontype)
  - [`INTERACTIVEACTIONS/ACTION/difficulty`](#interactiveactionsactiondifficulty)
  - [`INTERACTIVEACTIONS/ACTION/SectorGrid`](#interactiveactionsactionsectorgrid)
  - [`INTERACTIVEACTIONS/ACTION/sectorlevel`](#interactiveactionsactionsectorlevel)
  - [`INTERACTIVEACTIONS/ACTION/sStructureGridno`](#interactiveactionsactionsstructuregridno)
  - [`INTERACTIVEACTIONS/ACTION/sLevel`](#interactiveactionsactionslevel)
  - [`INTERACTIVEACTIONS/ACTION/luaactionid`](#interactiveactionsactionluaactionid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`InteractiveActions.xml` привязывает наборы тайлов и индексы структур к типу действия, сложности,
необязательным ограничениям по сектору и уровню и необязательному действию Lua. Повторяющиеся
элементы `usTileIndex` расширяют одно действие на несколько структур; для строки, привязанной к
сектору, повторяющиеся элементы `sStructureGridno` расширяют её на несколько точных местоположений.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `InteractiveActions` | `Map/InteractiveActions.xml` | — |

## Structure

```
INTERACTIVEACTIONS
└── ACTION
    ├── szTileSetName
    ├── usTileIndex
    ├── sActionType
    ├── difficulty
    ├── SectorGrid
    ├── sectorlevel
    ├── sStructureGridno
    ├── sLevel
    └── luaactionid
```

У `ACTION` нет ключевого элемента. Загрузчик сохраняет строки в порядке документа; когда совпадает
несколько строк, совпадение по сектору имеет приоритет над строкой «где угодно».

## Fields

### `INTERACTIVEACTIONS/ACTION/szTileSetName`

- **Type:** `String` (`CHAR8`)
- **Description:** Файл набора тайлов, содержащий интерактивную структуру, например `o_furn.sti`. Он
  сравнивается без учёта регистра с именами файлов тайловой поверхности набора, сравниваются первые
  11 символов.
- **Values:** Свободный текст; сохраняется в 20-байтовом буфере без добавления завершающего нуля,
  поэтому более длинный текст обрезается и может заполнить буфер.
- **Default:** Пусто, если поле отсутствует; пустое имя не совпадает ни с одним набором тайлов.
- **Source:** game code (interactive-actions loader; tileset matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/usTileIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс структуры внутри `szTileSetName`, который запускает это действие; игра
  сравнивает его с `usStructureNumber` структуры. Элемент может повторяться, и применяется любая
  совпавшая запись.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`. Элемент может повторяться.
- **Default:** Пустой список, если поле отсутствует (строка не совпадает ни с одной структурой);
  пустой элемент добавляет индекс `0`.
- **Source:** game code (interactive-actions loader; structure matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/sActionType`

- **Type:** `Integer` (`UINT16`)
- **Description:** Код типа действия, используемый системой взаимодействия: `0` нет действия,
  `1` взломать компьютер, `2` прочитать файл, `3` набрать воды, `4` купить газировку, `5` сыграть в
  мини-игру, `6` разное.
- **Values:** `0..6` имеют определённое поведение; загрузчик сохраняет значение как `UINT16` без
  проверки диапазона.
- **Default:** `0` (нет действия); пустой элемент разбирается как `0`.
- **Source:** game code (interactive-actions loader; `INTERACTIVE_STRUCTURE_TYPE` enum); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/difficulty`

- **Type:** `Integer` (`INT32`)
- **Description:** Порог сложности, сравниваемый с навыком интерактивного действия действующего
  солдата. Навык выводится из типа действия (навык взлома из предыстории, мудрость или фиксированное
  значение, например `100`) и обычно находится в шкале `0..100`. Действие по умолчанию успешно, когда
  навык не меньше этого значения; путь завершения многоходового взлома передаёт в результат по
  умолчанию `skill > difficulty`.
- **Numeric semantics:** Большие значения означают сложнее. Загрузчик сохраняет значение без
  ограничения и без отказа; поскольку навык беззнаковый, а сравнение расширяется до `INT32`,
  отрицательная сложность делает проверку тривиально истинной.
- **Values:** Целое число (диапазона в коде нет).
- **Default:** `0`; пустой элемент разбирается как `0`.
- **Source:** game code (interactive-actions loader; interactive-action skill check and default result); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/SectorGrid`

- **Type:** `String`
- **Description:** Ограничение по сектору, записанное буквой строки и номером столбца (`A1`..`P16`).
  Загрузчик преобразует его в ID сектора и сохраняет, только когда и строка, и столбец входят в
  `1..16`; для совпадения также требуется, чтобы `sectorlevel` равнялся текущему уровню сектора.
- **Numeric semantics:** Первый символ — строка (маскируется `0x1F`, поэтому строчные буквы тоже
  работают), остальные символы — десятичный столбец. Отсутствующее или недопустимое значение оставляет
  сектор по умолчанию `-1`, что означает отсутствие ограничения по сектору.
- **Values:** `A1`..`P16`.
- **Default:** Отсутствует или недопустимо = `-1` (любой сектор); пустой элемент также оставляет
  `-1`.
- **Source:** game code (interactive-actions loader; sector matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/sectorlevel`

- **Type:** `Integer` (`INT8`)
- **Description:** Уровень сектора для ограничения: `0` поверхность, `1..3` подвал, `-1` любой.
  Загрузчик ограничивает значение диапазоном `-1..3`. Строка, привязанная к сектору, совпадает,
  только когда это значение равно текущему уровню сектора, поэтому строка с `-1` ведёт себя как
  строка «где угодно», только если `SectorGrid` тоже отсутствует.
- **Values:** `-1..3` (с ограничением).
- **Default:** `-1` (любой); пустой элемент разбирается как `0` (поверхность).
- **Source:** game code (interactive-actions loader; sector matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/sStructureGridno`

- **Type:** `Integer` (`INT32`)
- **Description:** Точный номер сетки структуры (базовая сетка структуры) для действия, привязанного к
  сектору. Элемент может повторяться, и применяется любая совпавшая запись. Он учитывается, только
  когда заданы `SectorGrid` и `sectorlevel`; для строки «где угодно» игра его игнорирует.
- **Values:** Номер сетки; загрузчик сохраняет это поле как `INT32`. Элемент может повторяться.
- **Default:** Пустой список, если поле отсутствует; пустой элемент добавляет номер сетки `0`.
- **Source:** game code (interactive-actions loader; structure matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/sLevel`

- **Type:** `Integer` (`INT8`)
- **Description:** Уровень карты, используемый вместе с `sStructureGridno` для уточнения совпадения по
  точному местоположению для действия, привязанного к сектору: `0` пол, `1` крыша, `-1` не важно.
  Загрузчик ограничивает значение диапазоном `0..1`, когда элемент присутствует. Для строки «где
  угодно» игра его игнорирует.
- **Values:** `-1..1` (`-1` при отсутствии, `0..1` при наличии).
- **Default:** `-1` (не важно); пустой элемент разбирается как `0` (пол).
- **Source:** game code (interactive-actions loader; structure matching); table data and XML comment.
- **Confidence:** `source`

### `INTERACTIVEACTIONS/ACTION/luaactionid`

- **Type:** `Integer` (`INT32`)
- **Description:** ID действия Lua. Когда он `>= 0`, игра вызывает обработчик результата
  интерактивного действия Lua (`HandleInteractiveActionResult`) с этим ID; `-1` означает отсутствие
  вызова Lua и использование фиксированного результата по умолчанию.
- **Values:** `-1` отключает вызов Lua; `0..2147483647` — ID, передаваемый в Lua.
- **Default:** `-1`; пустой элемент разбирается как `0`, что запрашивает вызов Lua с ID `0`.
- **Source:** game code (interactive-actions loader; interactive-action result dispatch); table data and XML comment.
- **Confidence:** `source`

## Relations

- `usTileIndex` и `szTileSetName` определяют структуру карты; `luaactionid` передаётся в обработчик
  результата интерактивного действия Lua дополнительно к фиксированному результату.

## Localization

Эта таблица не содержит отображаемого текста; все поля — структурные идентификаторы или числовые
значения, они не локализуются.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы карты.
- Код игры: загрузчик интерактивных действий, сопоставление наборов тайлов и структур, сопоставление
  секторов, диспетчеризация результата интерактивного действия.
- Имена полей, комментарии XML и перечисление `INTERACTIVE_STRUCTURE_TYPE`.
- Описания охватывают значения, которые сохраняет загрузчик, и логику сопоставления, которая их
  использует; каждое поле в этом документе подтверждено загрузчиком.

## Open questions

Контракт Lua для `HandleInteractiveActionResult` определяется скриптами Lua, а не этой таблицей.
