---
schema: 1
lang: ru
family: items-random
title: "RandomItem — категории случайных предметов"
tables:
  - RandomItem
table_ids:
  - Items.RandomItem
files:
  - Items/RandomItem.xml
root: RANDOMITEMLIST
record: RANDOMITEM
key: uiIndex
sources:
  - Game code (random-item loader and selection)
  - Random item table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# RandomItem — категории случайных предметов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`RANDOMITEMLIST/RANDOMITEM/uiIndex`](#randomitemlistrandomitemuiindex)
  - [`RANDOMITEMLIST/RANDOMITEM/szName`](#randomitemlistrandomitemszname)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem1`](#randomitemlistrandomitemrandomitem1)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem2`](#randomitemlistrandomitemrandomitem2)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem3`](#randomitemlistrandomitemrandomitem3)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem4`](#randomitemlistrandomitemrandomitem4)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem5`](#randomitemlistrandomitemrandomitem5)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem6`](#randomitemlistrandomitemrandomitem6)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem7`](#randomitemlistrandomitemrandomitem7)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem8`](#randomitemlistrandomitemrandomitem8)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem9`](#randomitemlistrandomitemrandomitem9)
  - [`RANDOMITEMLIST/RANDOMITEM/randomitem10`](#randomitemlistrandomitemrandomitem10)
  - [`RANDOMITEMLIST/RANDOMITEM/item1`](#randomitemlistrandomitemitem1)
  - [`RANDOMITEMLIST/RANDOMITEM/item2`](#randomitemlistrandomitemitem2)
  - [`RANDOMITEMLIST/RANDOMITEM/item3`](#randomitemlistrandomitemitem3)
  - [`RANDOMITEMLIST/RANDOMITEM/item4`](#randomitemlistrandomitemitem4)
  - [`RANDOMITEMLIST/RANDOMITEM/item5`](#randomitemlistrandomitemitem5)
  - [`RANDOMITEMLIST/RANDOMITEM/item6`](#randomitemlistrandomitemitem6)
  - [`RANDOMITEMLIST/RANDOMITEM/item7`](#randomitemlistrandomitemitem7)
  - [`RANDOMITEMLIST/RANDOMITEM/item8`](#randomitemlistrandomitemitem8)
  - [`RANDOMITEMLIST/RANDOMITEM/item9`](#randomitemlistrandomitemitem9)
  - [`RANDOMITEMLIST/RANDOMITEM/item10`](#randomitemlistrandomitemitem10)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`RandomItem.xml` определяет именованные категории случайных предметов. Категория содержит до десяти прямых ID предметов и до десяти вложенных категорий; оба списка независимы, а поиск перед выбором предмета сводит их в единый список кандидатов. Категории упоминаются из `Items.randomitem`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `RandomItem` | `Items/RandomItem.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
RANDOMITEMLIST
└── RANDOMITEM
    ├── uiIndex
    ├── szName
    ├── randomitem1
    ├── randomitem2
    ├── randomitem3
    ├── randomitem4
    ├── randomitem5
    ├── randomitem6
    ├── randomitem7
    ├── randomitem8
    ├── randomitem9
    ├── randomitem10
    ├── item1
    ├── item2
    ├── item3
    ├── item4
    ├── item5
    ├── item6
    ├── item7
    ├── item8
    ├── item9
    └── item10
```

## Fields

### `RANDOMITEMLIST/RANDOMITEM/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс категории случайных предметов; загрузчик сохраняет строку в `gRandomItemClass[uiIndex]`. На него ссылаются `Items.randomitem` и слоты `randomitem*` других строк.
- **Values:** `0..999` (размер массива — `RANDOM_ITEM_MAX_CATEGORIES` = `1000`); строки с этим индексом и выше отбрасываются, дубликаты перезаписывают более раннюю строку.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/szName`

- **Type:** `String` (`String`)
- **Description:** Информационная метка. Загрузчик разбирает элемент, но намеренно не сохраняет его, поэтому игра никогда его не отображает и не использует.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_RandomItem.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 1. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 2. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 3. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 4. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 5. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 6. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 7. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 8. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 9. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вложенная категория случайных предметов в слоте 10. Ноль оставляет слот пустым.
- **Values:** Индекс вложенной категории (`0..999`); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 1. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 2. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 3. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 4. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 5. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 6. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 7. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 8. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 9. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Прямой ID предмета в слоте 10. Ноль оставляет слот пустым.
- **Values:** ID предмета из [Items](Items.md); `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

## Relations

- На `uiIndex` ссылаются `Items.randomitem` и слоты `randomitem1..10` других строк.
- `item1..10` ссылаются на [Items](Items.md); предмет, который сам является случайным предметом, раскрывается рекурсивно, а не добавляется.
- `randomitem1..10` ссылаются на другие строки `RANDOMITEM`; их предметы добавляются в тот же список кандидатов, а не выбирается сама категория как один элемент.
- Категория, уже посещённая в ходе того же поиска, повторно не раскрывается (список табу), что предотвращает циклы.
- Ноль в любом слоте означает, что слот пуст. Слоты предметов раскрываются раньше вложенных категорий, после чего из сведённого списка равномерно выбирается один элемент; дубликаты не удаляются.

## Localization

В таблице нет отображаемого текста: `szName` не сохраняется при загрузке и никогда не показывается; названия прямых предметов берутся из `Items`.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Игровой код: `XML_RandomItem.cpp` (загрузчик), `Items.cpp` (`GetItemFromRandomItem` и рекурсивные построители списков), `Item Types.h` (`RANDOM_ITEM_CHOICE_TYPE`, `RANDOM_ITEM_MAX_CATEGORIES`).
- Данные таблицы случайных предметов.
- Метаданные предметов (`Items.randomitem`, `Items.ubCoolness`).
- Типы и пределы полей: члены `RANDOM_ITEM_CHOICE_TYPE` и приведения в загрузчике.
- Описания охватывают путь кода, который использует каждое поле; количество строк и наблюдаемые диапазоны данных не используются.

## Open questions

- В таблице нет поля веса: выбор равномерен по сведённым элементам, поэтому предмет, указанный дважды (напрямую или через две категории), встречается вдвое чаще. Ожидают ли этого моддеры, не задокументировано.
- Категория, упомянутая более одного раза, раскрывается только один раз за поиск из-за списка табу посещённых классов; формат этого не документирует.
