---
schema: 1
lang: ru
family: items-pockets
title: "LoadBearingEquipment — раскладки разгрузки в NIV"
tables:
  - LoadBearingEquipment
table_ids:
  - Items.LoadBearingEquipment
files:
  - Items/LoadBearingEquipment.xml
root: LOADBEARINGEQUIPMENTLIST
record: LOADBEARINGEQUIPMENT
key: lbeIndex
sources:
  - LBE table data
  - LBEClass lookup table
  - Game code (LBE loader, item-size calculation, pocket and volume checks)
status: reviewed
updated: 2026-11-14
---

# LoadBearingEquipment — раскладки разгрузки в NIV

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeIndex`](#loadbearingequipmentlistloadbearingequipmentlbeindex)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeClass`](#loadbearingequipmentlistloadbearingequipmentlbeclass)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeCombo`](#loadbearingequipmentlistloadbearingequipmentlbecombo)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeFilledSize`](#loadbearingequipmentlistloadbearingequipmentlbefilledsize)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeAvailableVolume`](#loadbearingequipmentlistloadbearingequipmentlbeavailablevolume)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketsAvailable`](#loadbearingequipmentlistloadbearingequipmentlbepocketsavailable)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex1`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex1)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex2`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex2)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex3`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex3)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex4`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex4)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex5`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex5)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex6`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex6)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex7`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex7)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex8`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex8)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex9`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex9)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex10`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex10)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex11`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex11)
  - [`LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex12`](#loadbearingequipmentlistloadbearingequipmentlbepocketindex12)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`LoadBearingEquipment.xml` определяет раскладку карманов предметов разгрузки в новой системе инвентаря: класс, объём, битовую маску доступных карманов и строку кармана, назначенную каждому слоту.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `LoadBearingEquipment` | `Items/LoadBearingEquipment.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
LOADBEARINGEQUIPMENTLIST
└── LOADBEARINGEQUIPMENT
    ├── lbeIndex
    ├── lbeClass
    ├── lbeCombo
    ├── lbeFilledSize
    ├── lbeAvailableVolume
    ├── lbePocketsAvailable
    ├── lbePocketIndex1
    ├── lbePocketIndex2
    ├── lbePocketIndex3
    ├── lbePocketIndex4
    ├── lbePocketIndex5
    ├── lbePocketIndex6
    ├── lbePocketIndex7
    ├── lbePocketIndex8
    ├── lbePocketIndex9
    ├── lbePocketIndex10
    ├── lbePocketIndex11
    └── lbePocketIndex12
```

## Fields

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Порядковый номер строки разгрузки. Загрузчик добавляет каждую запись в игровой список `LoadBearingEquipment` в порядке документа и сохраняет это значение, не используя его для размещения, поэтому оно должно совпадать с позицией строки, отсчитываемой от нуля; сама игра никогда не читает сохранённое значение.
- **Only used with:** NIV.
- **Values:** `0..65535`; записывайте строки в возрастающем порядке. `Items.ubClassIndex` выбирает строку по её позиции в этом списке, а отладочный код записи снова выводит счётчик цикла как `lbeIndex`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (the LBE loader; the item lookup uses the row position, not this value).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeClass`

- **Type:** `Integer` (`UINT32`) — the game stores this field in a `UINT32`.
- **Description:** Класс разгрузки. Вместе с классом предмета он определяет, какой слот тела занимает изделие (набедренный, жилет, боевой рюкзак, рюкзак).
- **Only used with:** NIV.
- **Values:** Игровой enum `eLBE_CLASS` — это `1` `THIGH_PACK`, `2` `VEST_PACK`, `3` `COMBAT_PACK`, `4` `BACKPACK`, `5` `LBE_POCKET`, `6` `OTHER_POCKET`. `Lookup.LBEClass` называет значения от `0` Nothing до `4` Backpack; поставляемая таблица использует только `1..4`, а `0` — не класс, который распознают проверки слотов.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (the LBE class enum; per-slot class checks); `Lookup.LBEClass`.
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeCombo`

- **Type:** `Integer` (`UINT32`) — parsed with a base-0 conversion, so decimal or `0x` hex.
- **Description:** Битовая маска совместимости для изделий разгрузки, носимых одновременно (например, боевой рюкзак поверх рюкзака). Два изделия можно носить вместе, только если их маски имеют хотя бы один общий бит, а уже надетое изделие должно само иметь ненулевую маску.
- **Only used with:** NIV.
- **Numeric semantics:** Битовая маска; биты объединяются через OR. Большее значение не означает «больше» чего-либо. `0` не имеет общих битов ни с чем, поэтому его нельзя совместить.
- **Values:** До 32 бит; поставляемая таблица использует `0..4`, и большинство строк — `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (the `lbeCombo` field comment “Increased the LBE combo to 32bit … bitwise system”; combat-pack/backpack checks).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeFilledSize`

- **Type:** `Integer` (`UINT8`) — the game stores this field in a `UINT8`, so values above `255` truncate.
- **Description:** Размер предмета, которым считается разгрузка, когда её карманы заполнены. `CalculateItemSize` начинает с `Item[usItem].ItemSize`, добавляет долю `lbeFilledSize - ItemSize`, соответствующую степени заполнения карманов, и никогда не превышает `max(ItemSize, lbeFilledSize)`.
- **Only used with:** NIV.
- **Numeric semantics:** Значение размера предмета, а не количество; поставляемые строки используют `0`, `10` и `22..34` — это размеры предметов.
- **Values:** `0..255`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (the LBE loader; the item-size calculation).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbeAvailableVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Вместимость разгрузки по объёму. Содержимое отклоняется, когда уже занятый объём плюс объём заполняемого кармана превышает это значение, а описание предмета показывает занятый и доступный объём.
- **Only used with:** NIV.
- **Numeric semantics:** Единицы объёма; `0` означает, что предмет не является носителем с ограничением объёма (в поставляемой таблице большинство строк оставлено `0`, а для носителей используются `12..100`).
- **Values:** `0..255`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (the volume check; the item-description line).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketsAvailable`

- **Type:** `Integer` (`UINT16`)
- **Description:** Битовая маска слотов карманов, карман которых предоставляется навеской разгрузки, надетой на предмет (MOLLE). Когда `lbePocketIndex` слота равен `0` и соответствующий бит установлен, игра ищет навеску в слоте навески, у которого `ubPocketMapping - 1` равен номеру этого кармана, и использует первый ненулевой карман навески; без бита слот остаётся неактивным.
- **Only used with:** NIV.
- **Numeric semantics:** Битовая маска, а не количество. Бит `n` соответствует слоту кармана `n` — тому же индексу, отсчитываемому от нуля, что и `lbePocketIndex1..12`; биты объединяются через OR.
- **Values:** `0..4095` (12 слотов); поставляемая таблица использует значения вроде `15`, `115`, `2099` и `4067`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (the pocket fallback and the attachment pocket lookup).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex1`

- **Type:** `Integer` (`UINT8`) — the game stores each pocket index in a `UINT8`, so values above `255` truncate.
- **Description:** Строка кармана, назначенная слоту 1. Загрузчик принимает `lbePocketIndex1` и точечное написание `lbePocketIndex.1` и записывает значение в первый слот кармана.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в [Pockets](Pockets.md); `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует; игра выделяет 12 слотов карманов и заполняет их нулями.
- **Source:** game code (the LBE loader; 12 pocket slots).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex2`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 2 (`lbePocketIndex2` или `lbePocketIndex.2`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex3`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 3 (`lbePocketIndex3` или `lbePocketIndex.3`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex4`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 4 (`lbePocketIndex4` или `lbePocketIndex.4`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex5`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 5 (`lbePocketIndex5` или `lbePocketIndex.5`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex6`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 6 (`lbePocketIndex6` или `lbePocketIndex.6`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex7`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 7 (`lbePocketIndex7` или `lbePocketIndex.7`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex8`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 8 (`lbePocketIndex8` или `lbePocketIndex.8`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex9`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 9 (`lbePocketIndex9` или `lbePocketIndex.9`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex10`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 10 (`lbePocketIndex10` или `lbePocketIndex.10`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex11`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 11 (`lbePocketIndex11` или `lbePocketIndex.11`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader).
- **Confidence:** `source`

### `LOADBEARINGEQUIPMENTLIST/LOADBEARINGEQUIPMENT/lbePocketIndex12`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка кармана, назначенная слоту 12, последнему из 12 слотов (`lbePocketIndex12` или `lbePocketIndex.12`). `0` оставляет слот неактивным.
- **Only used with:** NIV.
- **Values:** Индекс строки кармана в `Pockets`; `0` оставляет слот неактивным.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (the LBE loader; 12 pocket slots).
- **Confidence:** `source`

## Relations

- `ubClassIndex` в [Items](Items.md) — это позиция строки в этой таблице; `lbeIndex` записывает эту позицию, но игра его не читает.
- `lbeClass` использует [Lookup.LBEClass](../Lookup.md#lbeclass), тогда как enum кода `eLBE_CLASS` также определяет `5` и `6`.
- `lbePocketIndex1..12` ссылаются на [Pockets](Pockets.md); `0` оставляет слот неактивным.
- Бит `n` в `lbePocketsAvailable` работает вместе со слотом `n` в `lbePocketIndex`: бит поставляет карман из надетой навески разгрузки, когда у самого слота нет строки кармана.
- `lbeCombo` связывает эту строку с другими строками разгрузки, которые можно носить одновременно.

## Localization

В этой таблице нет отображаемых строк; названия разгрузки берутся из `Items`.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы разгрузки.
- Справочная таблица LBEClass.
- Игровой код: загрузчик разгрузки, структуры разгрузки и карманов, вычисления размера предмета и объёма, а также резервный выбор слота и кармана в интерфейсе инвентаря.
- Типы полей — это типы хранения игровой структуры, а не обобщённый вид `Integer` в редакторе.
- Остальные утверждения о поставляемых значениях взяты из XML-данных и помечены как таковые; они не заявляют проверенную по коду формулу.

## Open questions

- Какой именно бит `lbeCombo` закреплён за семейством изделий разгрузки — это соглашение автора данных; игра проверяет только наличие общего бита.
- Игра выбирает строку разгрузки по её позиции в списке, поэтому перенумерация `lbeIndex` без сохранения порядка строк нарушила бы связь с `Items.ubClassIndex`; сама таблица этого требования не оговаривает.
