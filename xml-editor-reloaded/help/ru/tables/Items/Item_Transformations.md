---
schema: 1
lang: ru
family: items-merges
title: "Item_Transformations — преобразования предметов"
tables:
  - Item_Transformations
table_ids:
  - Items.Item_Transformations
files:
  - Items/Item_Transformations.xml
root: TRANSFORMATIONS_LIST
record: TRANSFORM
key: usItem
sources:
  - Item transformation table data
  - Game code (transformation loader, transformation record and item transformation logic)
status: reviewed
updated: 2026-11-12
---

# Item_Transformations — преобразования предметов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usItem`](#transformations_listtransformusitem)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult1`](#transformations_listtransformusresult1)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult2`](#transformations_listtransformusresult2)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult3`](#transformations_listtransformusresult3)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult4`](#transformations_listtransformusresult4)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult5`](#transformations_listtransformusresult5)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult6`](#transformations_listtransformusresult6)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult7`](#transformations_listtransformusresult7)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult8`](#transformations_listtransformusresult8)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult9`](#transformations_listtransformusresult9)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usResult10`](#transformations_listtransformusresult10)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/usAPCost`](#transformations_listtransformusapcost)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/iBPCost`](#transformations_listtransformibpcost)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/szMenuRowText`](#transformations_listtransformszmenurowtext)
  - [`TRANSFORMATIONS_LIST/TRANSFORM/szTooltipText`](#transformations_listtransformsztooltiptext)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Item_Transformations.xml` определяет меню преобразования для предмета: затраты AP и очков дыхания (BP), до десяти предметов-результатов, а также текст меню и подсказки, показываемый игроку.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Item_Transformations` | `Items/Item_Transformations.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
TRANSFORMATIONS_LIST
└── TRANSFORM
    ├── usItem
    ├── usResult1
    ├── usResult2
    ├── usResult3
    ├── usResult4
    ├── usResult5
    ├── usResult6
    ├── usResult7
    ├── usResult8
    ├── usResult9
    ├── usResult10
    ├── usAPCost
    ├── iBPCost
    ├── szMenuRowText
    └── szTooltipText
```

## Fields

### `TRANSFORMATIONS_LIST/TRANSFORM/usItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, который преобразуется. Сама запись хранится в `Transform[N]`, где `N` — позиция строки в документе, а не в `usItem`, поэтому это поле лишь связывает строку с её предметом. Несколько строк могут указывать один и тот же предмет, и каждая подходящая строка становится одним пунктом меню. Строки после `MAXITEMS` отбрасываются.
- **Numeric semantics:** Разбирается через `INT16`, поэтому литерал выше `32767` приходит отрицательным и затем сохраняется без изменений в `UINT16`. Отсутствующий элемент оставляет предустановленное значение `-1` (`65535`) — это сигнальное значение, которое останавливает каждый скан меню и поиска; поэтому строка без `usItem` скрывает остаток таблицы.
- **Values:** ID предмета; `0` и отрицательные значения также вызывают отладочное утверждение (`Entry #N missing proper <usItem> tag`).
- **Default:** `65535` (`0xFFFF`), если элемент отсутствует, — это не то же самое, что `0`.
- **Source:** game code (the transformation loader; the menu and lookup stop at `65535`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult1`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в первом слоте результата. Слоты заполняются в порядке документа: загрузчик принимает любое имя элемента, содержащее `usResult`, и записывает каждое значение в следующий слот, поэтому завершающая цифра игнорируется, и `usResult2`, стоящий перед `usResult1`, попал бы в слот 1. `0` оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Numeric semantics:** Только слоты выше `0` создают предметы, а положительное значение, указывающее на запись случайного предмета, сначала разрешается в конкретный предмет.
- **Values:** ID предмета; `0` оставляет слот пустым. Игра читает ровно 10 слотов (`MAX_NUM_TRANSFORMATION_RESULTS`), тогда как загрузчик не ограничивает свой счётчик, поэтому одиннадцатый элемент `usResult*` был бы записан за пределы массива.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`; 10 result slots).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult2`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в слоте результата 2. Как и каждый элемент `usResult*`, он размещается по порядку документа, а не по своей цифре. Ноль оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult3`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в слоте результата 3. Как и каждый элемент `usResult*`, он размещается по порядку документа, а не по своей цифре. Ноль оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult4`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в слоте результата 4. Как и каждый элемент `usResult*`, он размещается по порядку документа, а не по своей цифре. Ноль оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult5`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в слоте результата 5. Как и каждый элемент `usResult*`, он размещается по порядку документа, а не по своей цифре. Ноль оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult6`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в слоте результата 6. Как и каждый элемент `usResult*`, он размещается по порядку документа, а не по своей цифре. Ноль оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult7`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в слоте результата 7. Как и каждый элемент `usResult*`, он размещается по порядку документа, а не по своей цифре. Ноль оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult8`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в слоте результата 8. Как и каждый элемент `usResult*`, он размещается по порядку документа, а не по своей цифре. Ноль оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult9`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в слоте результата 9. Как и каждый элемент `usResult*`, он размещается по порядку документа, а не по своей цифре. Ноль оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult10`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, создаваемого в слоте результата 10, последнем из десяти слотов. Как и каждый элемент `usResult*`, он размещается по порядку документа, а не по своей цифре. Ноль оставляет слот пустым; один и тот же предмет может встречаться в нескольких слотах.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; `TransformObject`; 10 result slots).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usAPCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** Стоимость преобразования в очках действия. Загрузчик пересчитывает значение через `DynamicAdjustAPConstants`, то есть `value * AP_MAXIMUM / 100` с округлением, поэтому стоимость следует системе AP в файле констант AP/BP; `AP_MAXIMUM` по умолчанию равен `100`, что оставляет значение без изменений.
- **Numeric semantics:** AP. Списывается только в пошаговом бою — в реальном времени часть AP обнуляется — и меню добавляет `(N AP)` к строке, когда стоимость выше `0` и бой пошаговый.
- **Values:** AP; `0` делает преобразование бесплатным по очкам действия.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; the AP-constant adjustment; the point checks; the menu).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/iBPCost`

- **Type:** `Integer` (`INT32`) — it is not a `UINT16`.
- **Description:** Стоимость преобразования в очках дыхания, а не стоимость чертежа или очков создания. Положительное значение — потеря дыхания, отрицательное — прирост дыхания.
- **Numeric semantics:** BP (дыхание). `EnoughPoints` проверяет только часть AP, поэтому изменение дыхания применяется через `DeductPoints` без проверки достаточности; роботы и машины его пропускают.
- **Values:** Очки дыхания; положительное = стоимость, отрицательное = прирост.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the transformation loader; the breath-point deduction, whose comment reads “iBPCost > 0 - breath loss, iBPCost < 0 - breath gain”).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/szMenuRowText`

- **Type:** `String` — stored as a 50-character wide string.
- **Description:** Метка строки меню преобразования. Загрузчик преобразует текст UTF-8 в фиксированный буфер `CHAR16[50]` и принудительно ставит `'\0'` в последнюю позицию, поэтому помещается около 49 символов, а более длинный текст сохранить нельзя.
- **Values:** Свободный текст; короткая метка действия, а не полное предложение.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (the transformation loader; the transformation record).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/szTooltipText`

- **Type:** `String` — stored as a 300-character wide string.
- **Description:** Подсказка, показываемая для строки меню преобразования; читается в фиксированный буфер `CHAR16[300]`.
- **Values:** Свободный текст.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (the transformation loader; the transformation record).
- **Confidence:** `source`

## Relations

- `usItem` и `usResult1..10` указывают на [Items](Items.md).
- Строка находится сканированием загруженного массива `Transform` в поисках совпадающего `usItem`; первая строка, у которой `usItem` равен `65535`, останавливает сканирование, и каждая подходящая строка становится пунктом меню.
- Строки меню и подсказки — это презентационные данные; само действие определяется стоимостями и результатами.

## Localization

`szMenuRowText` и `szTooltipText` — текст для игрока, и он может быть локализован оверлеем предмета.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы преобразований предметов.
- Игровой код: загрузчик преобразований, структура записи преобразования, процедура преобразования предмета, меню и поиск преобразования, а также обработка AP/BP.
- Типы полей — это типы хранения игровой структуры; обобщённый вид `Integer` в редакторе не различает беззнаковые ширины, использованные выше.

## Open questions

- Для `iBPCost` открытых вопросов не осталось: это стоимость в очках дыхания, положительная при потере дыхания и отрицательная при приросте.
- Для порядка меню открытых вопросов не осталось: пункты идут в порядке документа строк `<TRANSFORM>`, и каждая строка, совпадающая с ID предмета, становится одним пунктом.
- Здесь не установлено: проверяет ли какой-либо инструмент `usAPCost` на соответствие пересчёту `AP_MAXIMUM`; таблица всегда хранит значение до пересчёта.
