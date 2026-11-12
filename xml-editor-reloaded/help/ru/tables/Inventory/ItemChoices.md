---
schema: 1
lang: ru
family: inventory-item-choices
title: "ItemChoices — выбор брони и снаряжения для врагов и ополчения"
tables:
  - EnemyItemChoices
  - ItemChoices_Enemy_Admin
  - ItemChoices_Enemy_Elite
  - ItemChoices_Enemy_Regular
  - ItemChoices_Militia_Elite
  - ItemChoices_Militia_Green
  - ItemChoices_Militia_Regular
table_ids:
  - Inventory.EnemyItemChoices
  - Inventory.ItemChoices_Enemy_Admin
  - Inventory.ItemChoices_Enemy_Elite
  - Inventory.ItemChoices_Enemy_Regular
  - Inventory.ItemChoices_Militia_Elite
  - Inventory.ItemChoices_Militia_Green
  - Inventory.ItemChoices_Militia_Regular
files:
  - Inventory/EnemyItemChoices.xml
  - Inventory/ItemChoices_Enemy_Admin.xml
  - Inventory/ItemChoices_Enemy_Elite.xml
  - Inventory/ItemChoices_Enemy_Regular.xml
  - Inventory/ItemChoices_Militia_Elite.xml
  - Inventory/ItemChoices_Militia_Green.xml
  - Inventory/ItemChoices_Militia_Regular.xml
root: ENEMYITEMCHOICESLIST
record: ENEMYITEMCHOICES
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# ItemChoices — выбор брони и снаряжения для врагов и ополчения

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/uiIndex`](#enemyitemchoiceslistenemyitemchoicesuiindex)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/name`](#enemyitemchoiceslistenemyitemchoicesname)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/ubChoices`](#enemyitemchoiceslistenemyitemchoicesubchoices)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo1`](#enemyitemchoiceslistenemyitemchoicesbitemno1)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo2`](#enemyitemchoiceslistenemyitemchoicesbitemno2)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo3`](#enemyitemchoiceslistenemyitemchoicesbitemno3)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo4`](#enemyitemchoiceslistenemyitemchoicesbitemno4)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo5`](#enemyitemchoiceslistenemyitemchoicesbitemno5)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo6`](#enemyitemchoiceslistenemyitemchoicesbitemno6)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo7`](#enemyitemchoiceslistenemyitemchoicesbitemno7)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo8`](#enemyitemchoiceslistenemyitemchoicesbitemno8)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo9`](#enemyitemchoiceslistenemyitemchoicesbitemno9)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo10`](#enemyitemchoiceslistenemyitemchoicesbitemno10)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo11`](#enemyitemchoiceslistenemyitemchoicesbitemno11)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo12`](#enemyitemchoiceslistenemyitemchoicesbitemno12)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo13`](#enemyitemchoiceslistenemyitemchoicesbitemno13)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo14`](#enemyitemchoiceslistenemyitemchoicesbitemno14)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo15`](#enemyitemchoiceslistenemyitemchoicesbitemno15)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo16`](#enemyitemchoiceslistenemyitemchoicesbitemno16)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo17`](#enemyitemchoiceslistenemyitemchoicesbitemno17)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo18`](#enemyitemchoiceslistenemyitemchoicesbitemno18)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo19`](#enemyitemchoiceslistenemyitemchoicesbitemno19)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo20`](#enemyitemchoiceslistenemyitemchoicesbitemno20)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo21`](#enemyitemchoiceslistenemyitemchoicesbitemno21)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo22`](#enemyitemchoiceslistenemyitemchoicesbitemno22)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo23`](#enemyitemchoiceslistenemyitemchoicesbitemno23)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo24`](#enemyitemchoiceslistenemyitemchoicesbitemno24)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo25`](#enemyitemchoiceslistenemyitemchoicesbitemno25)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo26`](#enemyitemchoiceslistenemyitemchoicesbitemno26)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo27`](#enemyitemchoiceslistenemyitemchoicesbitemno27)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo28`](#enemyitemchoiceslistenemyitemchoicesbitemno28)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo29`](#enemyitemchoiceslistenemyitemchoicesbitemno29)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo30`](#enemyitemchoiceslistenemyitemchoicesbitemno30)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo31`](#enemyitemchoiceslistenemyitemchoicesbitemno31)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo32`](#enemyitemchoiceslistenemyitemchoicesbitemno32)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo33`](#enemyitemchoiceslistenemyitemchoicesbitemno33)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo34`](#enemyitemchoiceslistenemyitemchoicesbitemno34)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo35`](#enemyitemchoiceslistenemyitemchoicesbitemno35)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo36`](#enemyitemchoiceslistenemyitemchoicesbitemno36)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo37`](#enemyitemchoiceslistenemyitemchoicesbitemno37)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo38`](#enemyitemchoiceslistenemyitemchoicesbitemno38)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo39`](#enemyitemchoiceslistenemyitemchoicesbitemno39)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo40`](#enemyitemchoiceslistenemyitemchoicesbitemno40)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo41`](#enemyitemchoiceslistenemyitemchoicesbitemno41)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo42`](#enemyitemchoiceslistenemyitemchoicesbitemno42)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo43`](#enemyitemchoiceslistenemyitemchoicesbitemno43)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo44`](#enemyitemchoiceslistenemyitemchoicesbitemno44)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo45`](#enemyitemchoiceslistenemyitemchoicesbitemno45)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo46`](#enemyitemchoiceslistenemyitemchoicesbitemno46)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo47`](#enemyitemchoiceslistenemyitemchoicesbitemno47)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo48`](#enemyitemchoiceslistenemyitemchoicesbitemno48)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo49`](#enemyitemchoiceslistenemyitemchoicesbitemno49)
  - [`ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo50`](#enemyitemchoiceslistenemyitemchoicesbitemno50)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

Файлы выбора предметов задают до 50 ID предметов брони или снаряжения для группы ранга или категории врагов/ополчения. `EnemyItemChoices.xml` — общая таблица; файлы рангов переопределяют её для своих групп.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EnemyItemChoices` | `Inventory/EnemyItemChoices.xml` | — |
| `ItemChoices_Enemy_Admin` | `Inventory/ItemChoices_Enemy_Admin.xml` | — |
| `ItemChoices_Enemy_Elite` | `Inventory/ItemChoices_Enemy_Elite.xml` | — |
| `ItemChoices_Enemy_Regular` | `Inventory/ItemChoices_Enemy_Regular.xml` | — |
| `ItemChoices_Militia_Elite` | `Inventory/ItemChoices_Militia_Elite.xml` | — |
| `ItemChoices_Militia_Green` | `Inventory/ItemChoices_Militia_Green.xml` | — |
| `ItemChoices_Militia_Regular` | `Inventory/ItemChoices_Militia_Regular.xml` | — |

## Structure

```
ENEMYITEMCHOICESLIST
└── ENEMYITEMCHOICES
    ├── uiIndex
    ├── name
    ├── ubChoices
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

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс группы выбора предметов. Это ключ массива.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/name`

- **Type:** `String`
- **Description:** Подпись группы хранится в таблице, но загрузчик не читает этот элемент; он информационный и игнорируется.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`
### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/ubChoices`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число допустимых слотов предметов в этой группе.
- **Numeric semantics:** больше = больше вариантов; отрицательные значения не имеют смысла.
- **Values:** Количество `0..50`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo1`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 1. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo2`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 2. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo3`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 3. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo4`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 4. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo5`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 5. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo6`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 6. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo7`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 7. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo8`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 8. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo9`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 9. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo10`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 10. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo11`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 11. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo12`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 12. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo13`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 13. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo14`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 14. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo15`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 15. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo16`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 16. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo17`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 17. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo18`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 18. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo19`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 19. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo20`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 20. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo21`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 21. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo22`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 22. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo23`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 23. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo24`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 24. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo25`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 25. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo26`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 26. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo27`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 27. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo28`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 28. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo29`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 29. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo30`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 30. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo31`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 31. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo32`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 32. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo33`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 33. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo34`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 34. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo35`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 35. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo36`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 36. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo37`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 37. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo38`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 38. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo39`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 39. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo40`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 40. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo41`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 41. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo42`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 42. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo43`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 43. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo44`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 44. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo45`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 45. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo46`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 46. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo47`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 47. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo48`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 48. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo49`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 49. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo50`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте 50. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
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
