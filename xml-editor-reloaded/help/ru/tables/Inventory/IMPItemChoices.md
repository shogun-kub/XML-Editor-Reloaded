---
schema: 1
lang: ru
family: inventory-imp-item-choices
title: "IMPItemChoices — стартовый выбор предметов IMP"
tables:
  - IMPItemChoices
table_ids:
  - Inventory.IMPItemChoices
files:
  - Inventory/IMPItemChoices.xml
root: IMPITEMCHOICESLIST
record: IMPITEMCHOICES
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# IMPItemChoices — стартовый выбор предметов IMP

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/uiIndex`](#impitemchoiceslistimpitemchoicesuiindex)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/name`](#impitemchoiceslistimpitemchoicesname)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/ubChoices`](#impitemchoiceslistimpitemchoicesubchoices)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/ubNumItems`](#impitemchoiceslistimpitemchoicesubnumitems)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo1`](#impitemchoiceslistimpitemchoicesbitemno1)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo2`](#impitemchoiceslistimpitemchoicesbitemno2)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo3`](#impitemchoiceslistimpitemchoicesbitemno3)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo4`](#impitemchoiceslistimpitemchoicesbitemno4)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo5`](#impitemchoiceslistimpitemchoicesbitemno5)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo6`](#impitemchoiceslistimpitemchoicesbitemno6)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo7`](#impitemchoiceslistimpitemchoicesbitemno7)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo8`](#impitemchoiceslistimpitemchoicesbitemno8)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo9`](#impitemchoiceslistimpitemchoicesbitemno9)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo10`](#impitemchoiceslistimpitemchoicesbitemno10)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo11`](#impitemchoiceslistimpitemchoicesbitemno11)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo12`](#impitemchoiceslistimpitemchoicesbitemno12)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo13`](#impitemchoiceslistimpitemchoicesbitemno13)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo14`](#impitemchoiceslistimpitemchoicesbitemno14)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo15`](#impitemchoiceslistimpitemchoicesbitemno15)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo16`](#impitemchoiceslistimpitemchoicesbitemno16)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo17`](#impitemchoiceslistimpitemchoicesbitemno17)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo18`](#impitemchoiceslistimpitemchoicesbitemno18)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo19`](#impitemchoiceslistimpitemchoicesbitemno19)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo20`](#impitemchoiceslistimpitemchoicesbitemno20)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo21`](#impitemchoiceslistimpitemchoicesbitemno21)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo22`](#impitemchoiceslistimpitemchoicesbitemno22)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo23`](#impitemchoiceslistimpitemchoicesbitemno23)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo24`](#impitemchoiceslistimpitemchoicesbitemno24)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo25`](#impitemchoiceslistimpitemchoicesbitemno25)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo26`](#impitemchoiceslistimpitemchoicesbitemno26)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo27`](#impitemchoiceslistimpitemchoicesbitemno27)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo28`](#impitemchoiceslistimpitemchoicesbitemno28)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo29`](#impitemchoiceslistimpitemchoicesbitemno29)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo30`](#impitemchoiceslistimpitemchoicesbitemno30)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo31`](#impitemchoiceslistimpitemchoicesbitemno31)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo32`](#impitemchoiceslistimpitemchoicesbitemno32)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo33`](#impitemchoiceslistimpitemchoicesbitemno33)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo34`](#impitemchoiceslistimpitemchoicesbitemno34)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo35`](#impitemchoiceslistimpitemchoicesbitemno35)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo36`](#impitemchoiceslistimpitemchoicesbitemno36)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo37`](#impitemchoiceslistimpitemchoicesbitemno37)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo38`](#impitemchoiceslistimpitemchoicesbitemno38)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo39`](#impitemchoiceslistimpitemchoicesbitemno39)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo40`](#impitemchoiceslistimpitemchoicesbitemno40)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo41`](#impitemchoiceslistimpitemchoicesbitemno41)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo42`](#impitemchoiceslistimpitemchoicesbitemno42)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo43`](#impitemchoiceslistimpitemchoicesbitemno43)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo44`](#impitemchoiceslistimpitemchoicesbitemno44)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo45`](#impitemchoiceslistimpitemchoicesbitemno45)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo46`](#impitemchoiceslistimpitemchoicesbitemno46)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo47`](#impitemchoiceslistimpitemchoicesbitemno47)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo48`](#impitemchoiceslistimpitemchoicesbitemno48)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo49`](#impitemchoiceslistimpitemchoicesbitemno49)
  - [`IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo50`](#impitemchoiceslistimpitemchoicesbitemno50)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`IMPItemChoices.xml` определяет группы предметов и ID предметов, которые наёмник IMP может выбрать при создании. `ubNumItems` управляет тем, сколько предметов выдаётся из группы.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `IMPItemChoices` | `Inventory/IMPItemChoices.xml` | — |

## Structure

```
IMPITEMCHOICESLIST
└── IMPITEMCHOICES
    ├── uiIndex
    ├── name
    ├── ubChoices
    ├── ubNumItems
    ├── bItemNo1
    ├── bItemNo2
    ├── bItemNo3
    ├── bItemNo4
    ├── bItemNo5
    ├── bItemNo6
    ├── bItemNo7
    ├── bItemNo8
    ├── bItemNo9
    ├── bItemNo10
    ├── bItemNo11
    ├── bItemNo12
    ├── bItemNo13
    ├── bItemNo14
    ├── bItemNo15
    ├── bItemNo16
    ├── bItemNo17
    ├── bItemNo18
    ├── bItemNo19
    ├── bItemNo20
    ├── bItemNo21
    ├── bItemNo22
    ├── bItemNo23
    ├── bItemNo24
    ├── bItemNo25
    ├── bItemNo26
    ├── bItemNo27
    ├── bItemNo28
    ├── bItemNo29
    ├── bItemNo30
    ├── bItemNo31
    ├── bItemNo32
    ├── bItemNo33
    ├── bItemNo34
    ├── bItemNo35
    ├── bItemNo36
    ├── bItemNo37
    ├── bItemNo38
    ├── bItemNo39
    ├── bItemNo40
    ├── bItemNo41
    ├── bItemNo42
    ├── bItemNo43
    ├── bItemNo44
    ├── bItemNo45
    ├── bItemNo46
    ├── bItemNo47
    ├── bItemNo48
    ├── bItemNo49
    └── bItemNo50
```

## Fields

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс группы выбора предметов IMP. Это ключ массива.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/name`

- **Type:** `String`
- **Description:** Подпись группы хранится в таблице, но загрузчик не читает этот элемент; он информационный и игнорируется.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`
### `IMPITEMCHOICESLIST/IMPITEMCHOICES/ubChoices`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число допустимых слотов предметов в группе.
- **Numeric semantics:** больше = больше вариантов; отрицательные значения не имеют смысла.
- **Values:** Количество `0..50`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/ubNumItems`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число предметов, которые IMP может взять из этой группы.
- **Numeric semantics:** больше = больше предметов; отрицательные значения не имеют смысла.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo1`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 1. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo2`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 2. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo3`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 3. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo4`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 4. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo5`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 5. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo6`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 6. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo7`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 7. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo8`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 8. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo9`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 9. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo10`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 10. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo11`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 11. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo12`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 12. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo13`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 13. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo14`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 14. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo15`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 15. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo16`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 16. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo17`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 17. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo18`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 18. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo19`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 19. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo20`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 20. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo21`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 21. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo22`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 22. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo23`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 23. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo24`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 24. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo25`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 25. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo26`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 26. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo27`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 27. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo28`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 28. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo29`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 29. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo30`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 30. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo31`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 31. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo32`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 32. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo33`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 33. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo34`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 34. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo35`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 35. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo36`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 36. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo37`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 37. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo38`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 38. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo39`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 39. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo40`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 40. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo41`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 41. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo42`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 42. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo43`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 43. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo44`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 44. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo45`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 45. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo46`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 46. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo47`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 47. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo48`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 48. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo49`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 49. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo50`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте IMP 50. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

## Relations

- Значения `bItemNoN` ссылаются на индексы предметов в [Items](../Items/Items.md).

## Localization

Отображаемые названия и подписи локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не заявляется.

## Open questions

- Точная формула или отображение перечисления для выведенных полей в данных таблицы не указаны.
