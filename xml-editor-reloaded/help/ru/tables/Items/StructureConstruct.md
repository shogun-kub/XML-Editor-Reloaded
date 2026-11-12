---
schema: 1
lang: ru
family: items-structures
title: "StructureConstruct — возводимые строения"
tables:
  - StructureConstruct
table_ids:
  - Items.StructureConstruct
files:
  - Items/StructureConstruct.xml
root: STRUCTURESLIST
record: STRUCTURE
key: none
sources:
  - Structure construction table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# StructureConstruct — возводимые строения

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`STRUCTURESLIST/STRUCTURE/usCreationItem`](#structuresliststructureuscreationitem)
  - [`STRUCTURESLIST/STRUCTURE/usItemStatusLoss`](#structuresliststructureusitemstatusloss)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`](#structuresliststructuresztilesetdisplayname)
  - [`STRUCTURESLIST/STRUCTURE/szTileSetName`](#structuresliststructuresztilesetname)
  - [`STRUCTURESLIST/STRUCTURE/dCreationCost`](#structuresliststructuredcreationcost)
  - [`STRUCTURESLIST/STRUCTURE/fFortifyAdjacentAdjustment`](#structuresliststructureffortifyadjacentadjustment)
  - [`STRUCTURESLIST/STRUCTURE/northfacingtile`](#structuresliststructurenorthfacingtile)
  - [`STRUCTURESLIST/STRUCTURE/southfacingtile`](#structuresliststructuresouthfacingtile)
  - [`STRUCTURESLIST/STRUCTURE/eastfacingtile`](#structuresliststructureeastfacingtile)
  - [`STRUCTURESLIST/STRUCTURE/westfacingtile`](#structuresliststructurewestfacingtile)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`StructureConstruct.xml` определяет строения, возводимые из предмета: расходуемый предмет, потерю
его состояния, набор тайлов и тайлы ориентации, стоимость и поправку укрепления.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `StructureConstruct` | `Items/StructureConstruct.xml` | В v4 и v5 совпадают пути полей; значения различаются. Элементы `*facingtile` повторяются. |

## Structure

```
STRUCTURESLIST
└── STRUCTURE
    ├── usCreationItem
    ├── usItemStatusLoss
    ├── szTileSetDisplayName
    ├── szTileSetName
    ├── dCreationCost
    ├── fFortifyAdjacentAdjustment
    ├── northfacingtile
    ├── southfacingtile
    ├── eastfacingtile
    └── westfacingtile
```

У `STRUCTURESLIST` нет ключевого элемента. Загрузчик сохраняет строки в порядке документа; когда
солдат строит с помощью предмета, игра выбирает первую строку, у которой `usCreationItem` совпадает
с предметом в руках и список ориентации для направления солдата не пуст, а план укрепления сохраняет
индекс выбранной строки.

## Fields

### `STRUCTURESLIST/STRUCTURE/usCreationItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, расходуемого на возведение строения. Значение `0` означает, что
  автоматический строитель укреплений не расходует предмет, и строку с `0` нельзя выбрать, держа
  предмет в руках.
- **Values:** ID предмета; `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (structure-construct loader; construction item consumption); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/usItemStatusLoss`

- **Type:** `Integer` (`UINT8`)
- **Description:** Состояние (прочность) предмета, снимаемое с предмета создания при каждом
  возведении строения. Если оставшееся состояние предмета меньше или равно этому значению, предмет
  удаляется, а не уменьшается.
- **Numeric semantics:** Процентные пункты прочности предмета в шкале `objectStatus`. `0` означает,
  что предмет не повреждён.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`, а код предметов трактует его как
  процент прочности (`0..100` — обычный диапазон).
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (structure-construct loader; construction item consumption and fortification resource consumption); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/szTileSetDisplayName`

- **Type:** `String` (`CHAR8`)
- **Description:** Название строения, показываемое игроку; выводится в меню строительства укреплений
  для записей, чей набор тайлов доступен в текущем наборе (или в общем наборе).
- **Values:** Свободный текст; сохраняется в 20-байтовом буфере без добавления завершающего нуля,
  поэтому более длинный текст обрезается и может заполнить буфер.
- **Default:** Пустая строка, если поле отсутствует.
- **Source:** game code (structure-construct loader; fortification build-menu list); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/szTileSetName`

- **Type:** `String` (`CHAR8`)
- **Description:** Имя файла набора тайлов, тайлы которого изображают строение. Оно сравнивается без
  учёта регистра с префиксом имён файлов тайловой поверхности набора: 10 символов в пути ручного
  строительства и 11 в путях меню строительства и плана.
- **Values:** Свободный текст, например `sandbag.sti`; сохраняется в 20-байтовом буфере без
  добавления завершающего нуля, поэтому более длинный текст обрезается.
- **Default:** Пустая строка, если поле отсутствует; пустое имя не совпадает ни с одним набором
  тайлов.
- **Source:** game code (structure-construct loader; tileset matching); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/dCreationCost`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Стоимость возведения одного строения, выраженная в очках работы (прогресса)
  укрепления. Стоимость суммируется в максимальный прогресс укрепления сектора при оценке плана и
  вычитается по мере возведения каждого строения. Это не денежная величина.
- **Numeric semantics:** Дробные очки прогресса; большие значения означают больше работы. Загрузчик
  сохраняет разобранное значение без ограничения и без отказа.
- **Values:** Десятичное число (диапазона в коде нет).
- **Default:** `1.0` из конструктора `STRUCTURE_CONSTRUCT`, если поле отсутствует. Пустой элемент
  разбирается как `0`.
- **Source:** game code (structure-construct loader; fortification progress accounting and consumption); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/fFortifyAdjacentAdjustment`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, игра пытается согласовать только что построенный узел укрепления
  с соседними узлами той же строки, выбирая совместимые варианты направления из списков тайлов
  ориентации.
- **Values:** `0` или `1`.
- **Default:** `1` (TRUE) из конструктора `STRUCTURE_CONSTRUCT`, если поле отсутствует. Пустой
  элемент разбирается как `0`.
- **Source:** game code (structure-construct loader; adjacent-node adjustment); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/northfacingtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс тайла в наборе, используемый, когда строитель обращён на север. Элемент
  может повторяться; игра выбирает одну запись из этого списка случайно.
- **Numeric semantics:** Индекс в наборе тайлов; `0..255`. Объединение четырёх списков ориентации —
  это набор строений, предлагаемых в меню строительства укреплений. Для возведения в одном из
  основных направлений нужен список этого направления; при диагональной ориентации игра пробует
  север, затем юг, затем запад, затем восток. Когда `fFortifyAdjacentAdjustment` включено, списки
  также поставляют совместимые варианты для соседних узлов.
- **Values:** `0..255`; элемент может повторяться.
- **Default:** Пустой список, если поле отсутствует (не `0`); пустой элемент добавляет индекс `0`.
- **Source:** game code (structure-construct loader; fortification build selection); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/southfacingtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс тайла в наборе, используемый, когда строитель обращён на юг. Элемент может
  повторяться; игра выбирает одну запись из этого списка случайно.
- **Numeric semantics:** Индекс в наборе тайлов; `0..255`. Объединение четырёх списков ориентации —
  это набор строений, предлагаемых в меню строительства укреплений. Для возведения в одном из
  основных направлений нужен список этого направления; при диагональной ориентации игра пробует
  север, затем юг, затем запад, затем восток. Когда `fFortifyAdjacentAdjustment` включено, списки
  также поставляют совместимые варианты для соседних узлов.
- **Values:** `0..255`; элемент может повторяться.
- **Default:** Пустой список, если поле отсутствует (не `0`); пустой элемент добавляет индекс `0`.
- **Source:** game code (structure-construct loader; fortification build selection); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/eastfacingtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс тайла в наборе, используемый, когда строитель обращён на восток. Элемент
  может повторяться; игра выбирает одну запись из этого списка случайно.
- **Numeric semantics:** Индекс в наборе тайлов; `0..255`. Объединение четырёх списков ориентации —
  это набор строений, предлагаемых в меню строительства укреплений. Для возведения в одном из
  основных направлений нужен список этого направления; при диагональной ориентации игра пробует
  север, затем юг, затем запад, затем восток. Когда `fFortifyAdjacentAdjustment` включено, списки
  также поставляют совместимые варианты для соседних узлов.
- **Values:** `0..255`; элемент может повторяться.
- **Default:** Пустой список, если поле отсутствует (не `0`); пустой элемент добавляет индекс `0`.
- **Source:** game code (structure-construct loader; fortification build selection); structure construction table data.
- **Confidence:** `source`

### `STRUCTURESLIST/STRUCTURE/westfacingtile`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс тайла в наборе, используемый, когда строитель обращён на запад. Элемент
  может повторяться; игра выбирает одну запись из этого списка случайно.
- **Numeric semantics:** Индекс в наборе тайлов; `0..255`. Объединение четырёх списков ориентации —
  это набор строений, предлагаемых в меню строительства укреплений. Для возведения в одном из
  основных направлений нужен список этого направления; при диагональной ориентации игра пробует
  север, затем юг, затем запад, затем восток. Когда `fFortifyAdjacentAdjustment` включено, списки
  также поставляют совместимые варианты для соседних узлов.
- **Values:** `0..255`; элемент может повторяться.
- **Default:** Пустой список, если поле отсутствует (не `0`); пустой элемент добавляет индекс `0`.
- **Source:** game code (structure-construct loader; fortification build selection); structure construction table data.
- **Confidence:** `source`

## Relations

- `usCreationItem` указывает на [Items](Items.md).
- Колонки тайлов ориентации ссылаются на тайлы (индексы структур) в указанном наборе тайлов, а не
  на строки предметов.

## Localization

`szTileSetDisplayName` — название строения для игрока; оно преобразуется из UTF-8 в UTF-16 при
построении меню строительства; `szTileSetName` — ключ ресурса.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы строительства строений.
- Код игры: загрузчик строительства строений, расход предметов при строительстве, меню строительства
  укреплений, учёт прогресса укрепления.
- Значения по умолчанию из записи `STRUCTURE_CONSTRUCT`.
- Описания охватывают значения, которые код читает, сохраняет и использует; каждое поле в этом
  документе подтверждено загрузчиком и его потребителем.

## Open questions

- Сравнение префикса имени набора тайлов составляет 10 символов в пути ручного строительства и 11
  символов в путях меню строительства и плана, поэтому имя, отличающееся только в позиции 11, может
  вести себя по-разному в этих путях.
- Количество очков строительства, создаваемых за один такт работы задания, и, следовательно,
  практический баланс значений `dCreationCost` определяются системой заданий вне этой таблицы.
