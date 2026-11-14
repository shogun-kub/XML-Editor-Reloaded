---
schema: 1
lang: ru
family: inventory-gun-choices
title: "GunChoices — выбор оружия для врагов и ополчения"
tables:
  - EnemyGunChoices
  - GunChoices_Enemy_Admin
  - GunChoices_Enemy_Elite
  - GunChoices_Enemy_Regular
  - GunChoices_Militia_Elite
  - GunChoices_Militia_Green
  - GunChoices_Militia_Regular
  - Backup: GunChoices_Enemy_Admin
  - Backup: GunChoices_Enemy_Elite
  - Backup: GunChoices_Enemy_Regular
  - Backup: GunChoices_Militia_Elite
  - Backup: GunChoices_Militia_Green
  - Backup: GunChoices_Militia_Regular
table_ids:
  - Inventory.EnemyGunChoices
  - Inventory.GunChoices_Enemy_Admin
  - Inventory.GunChoices_Enemy_Elite
  - Inventory.GunChoices_Enemy_Regular
  - Inventory.GunChoices_Militia_Elite
  - Inventory.GunChoices_Militia_Green
  - Inventory.GunChoices_Militia_Regular
  - Inventory.Backup original Gun Choices.GunChoices_Enemy_Admin
  - Inventory.Backup original Gun Choices.GunChoices_Enemy_Elite
  - Inventory.Backup original Gun Choices.GunChoices_Enemy_Regular
  - Inventory.Backup original Gun Choices.GunChoices_Militia_Elite
  - Inventory.Backup original Gun Choices.GunChoices_Militia_Green
  - Inventory.Backup original Gun Choices.GunChoices_Militia_Regular
files:
  - Inventory/EnemyGunChoices.xml
  - Inventory/GunChoices_Enemy_Admin.xml
  - Inventory/GunChoices_Enemy_Elite.xml
  - Inventory/GunChoices_Enemy_Regular.xml
  - Inventory/GunChoices_Militia_Elite.xml
  - Inventory/GunChoices_Militia_Green.xml
  - Inventory/GunChoices_Militia_Regular.xml
  - Inventory/Backup original Gun Choices/GunChoices_Enemy_Admin.xml
  - Inventory/Backup original Gun Choices/GunChoices_Enemy_Elite.xml
  - Inventory/Backup original Gun Choices/GunChoices_Enemy_Regular.xml
  - Inventory/Backup original Gun Choices/GunChoices_Militia_Elite.xml
  - Inventory/Backup original Gun Choices/GunChoices_Militia_Green.xml
  - Inventory/Backup original Gun Choices/GunChoices_Militia_Regular.xml
root: ENEMYGUNCHOICESLIST
record: ENEMYGUNCHOICES
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# GunChoices — выбор оружия для врагов и ополчения

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/uiIndex`](#enemygunchoiceslistenemygunchoicesuiindex)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/name`](#enemygunchoiceslistenemygunchoicesname)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/ubChoices`](#enemygunchoiceslistenemygunchoicesubchoices)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo1`](#enemygunchoiceslistenemygunchoicesbitemno1)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo2`](#enemygunchoiceslistenemygunchoicesbitemno2)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo3`](#enemygunchoiceslistenemygunchoicesbitemno3)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo4`](#enemygunchoiceslistenemygunchoicesbitemno4)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo5`](#enemygunchoiceslistenemygunchoicesbitemno5)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo6`](#enemygunchoiceslistenemygunchoicesbitemno6)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo7`](#enemygunchoiceslistenemygunchoicesbitemno7)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo8`](#enemygunchoiceslistenemygunchoicesbitemno8)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo9`](#enemygunchoiceslistenemygunchoicesbitemno9)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo10`](#enemygunchoiceslistenemygunchoicesbitemno10)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo11`](#enemygunchoiceslistenemygunchoicesbitemno11)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo12`](#enemygunchoiceslistenemygunchoicesbitemno12)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo13`](#enemygunchoiceslistenemygunchoicesbitemno13)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo14`](#enemygunchoiceslistenemygunchoicesbitemno14)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo15`](#enemygunchoiceslistenemygunchoicesbitemno15)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo16`](#enemygunchoiceslistenemygunchoicesbitemno16)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo17`](#enemygunchoiceslistenemygunchoicesbitemno17)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo18`](#enemygunchoiceslistenemygunchoicesbitemno18)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo19`](#enemygunchoiceslistenemygunchoicesbitemno19)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo20`](#enemygunchoiceslistenemygunchoicesbitemno20)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo21`](#enemygunchoiceslistenemygunchoicesbitemno21)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo22`](#enemygunchoiceslistenemygunchoicesbitemno22)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo23`](#enemygunchoiceslistenemygunchoicesbitemno23)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo24`](#enemygunchoiceslistenemygunchoicesbitemno24)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo25`](#enemygunchoiceslistenemygunchoicesbitemno25)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo26`](#enemygunchoiceslistenemygunchoicesbitemno26)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo27`](#enemygunchoiceslistenemygunchoicesbitemno27)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo28`](#enemygunchoiceslistenemygunchoicesbitemno28)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo29`](#enemygunchoiceslistenemygunchoicesbitemno29)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo30`](#enemygunchoiceslistenemygunchoicesbitemno30)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo31`](#enemygunchoiceslistenemygunchoicesbitemno31)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo32`](#enemygunchoiceslistenemygunchoicesbitemno32)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo33`](#enemygunchoiceslistenemygunchoicesbitemno33)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo34`](#enemygunchoiceslistenemygunchoicesbitemno34)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo35`](#enemygunchoiceslistenemygunchoicesbitemno35)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo36`](#enemygunchoiceslistenemygunchoicesbitemno36)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo37`](#enemygunchoiceslistenemygunchoicesbitemno37)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo38`](#enemygunchoiceslistenemygunchoicesbitemno38)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo39`](#enemygunchoiceslistenemygunchoicesbitemno39)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo40`](#enemygunchoiceslistenemygunchoicesbitemno40)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo41`](#enemygunchoiceslistenemygunchoicesbitemno41)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo42`](#enemygunchoiceslistenemygunchoicesbitemno42)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo43`](#enemygunchoiceslistenemygunchoicesbitemno43)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo44`](#enemygunchoiceslistenemygunchoicesbitemno44)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo45`](#enemygunchoiceslistenemygunchoicesbitemno45)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo46`](#enemygunchoiceslistenemygunchoicesbitemno46)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo47`](#enemygunchoiceslistenemygunchoicesbitemno47)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo48`](#enemygunchoiceslistenemygunchoicesbitemno48)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo49`](#enemygunchoiceslistenemygunchoicesbitemno49)
  - [`ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo50`](#enemygunchoiceslistenemygunchoicesbitemno50)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

Файлы выбора оружия задают до 50 ID предметов-оружия для группы ранга/уровня крутости. Шесть основных файлов рангов — это действующий выбор; `EnemyGunChoices.xml` — общая таблица. Шесть файлов в `Inventory/Backup original Gun Choices/` — исторические копии; они перечислены как псевдонимы, а не как отдельные действующие профили.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EnemyGunChoices` | `Inventory/EnemyGunChoices.xml` | — |
| `GunChoices_Enemy_Admin` | `Inventory/GunChoices_Enemy_Admin.xml` | — |
| `GunChoices_Enemy_Elite` | `Inventory/GunChoices_Enemy_Elite.xml` | — |
| `GunChoices_Enemy_Regular` | `Inventory/GunChoices_Enemy_Regular.xml` | — |
| `GunChoices_Militia_Elite` | `Inventory/GunChoices_Militia_Elite.xml` | — |
| `GunChoices_Militia_Green` | `Inventory/GunChoices_Militia_Green.xml` | — |
| `GunChoices_Militia_Regular` | `Inventory/GunChoices_Militia_Regular.xml` | — |
| `Backup: GunChoices_Enemy_Admin` | `Inventory/Backup original Gun Choices/GunChoices_Enemy_Admin.xml` | — |
| `Backup: GunChoices_Enemy_Elite` | `Inventory/Backup original Gun Choices/GunChoices_Enemy_Elite.xml` | — |
| `Backup: GunChoices_Enemy_Regular` | `Inventory/Backup original Gun Choices/GunChoices_Enemy_Regular.xml` | — |
| `Backup: GunChoices_Militia_Elite` | `Inventory/Backup original Gun Choices/GunChoices_Militia_Elite.xml` | — |
| `Backup: GunChoices_Militia_Green` | `Inventory/Backup original Gun Choices/GunChoices_Militia_Green.xml` | — |
| `Backup: GunChoices_Militia_Regular` | `Inventory/Backup original Gun Choices/GunChoices_Militia_Regular.xml` | — |

## Structure

```
ENEMYGUNCHOICESLIST
└── ENEMYGUNCHOICES
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

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс группы выбора оружия. Это ключ массива.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/name`

- **Type:** `String`
- **Description:** Подпись группы хранится в таблице, но загрузчик не читает этот элемент; он информационный и игнорируется.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`
### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/ubChoices`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число допустимых слотов оружия в этой группе.
- **Numeric semantics:** больше = больше вариантов; отрицательные значения не имеют смысла.
- **Values:** Количество `0..50`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo1`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 1. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo2`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 2. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo3`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 3. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo4`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 4. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo5`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 5. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo6`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 6. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo7`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 7. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo8`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 8. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo9`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 9. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo10`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 10. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo11`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 11. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo12`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 12. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo13`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 13. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo14`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 14. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo15`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 15. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo16`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 16. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo17`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 17. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo18`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 18. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo19`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 19. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo20`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 20. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo21`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 21. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo22`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 22. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo23`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 23. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo24`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 24. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo25`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 25. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo26`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 26. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo27`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 27. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo28`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 28. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo29`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 29. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo30`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 30. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo31`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 31. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo32`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 32. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo33`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 33. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo34`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 34. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo35`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 35. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo36`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 36. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo37`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 37. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo38`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 38. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo39`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 39. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo40`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 40. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo41`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 41. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo42`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 42. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo43`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 43. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo44`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 44. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo45`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 45. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo46`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 46. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo47`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 47. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo48`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 48. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo49`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 49. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo50`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета в слоте оружия 50. `0` означает, что слот пуст; другие значения ссылаются на ID предмета. Это идентификатор.
- **Values:** Целое число; масштаб см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

## Relations

- Значения `bItemNoN` ссылаются на индексы предметов в [Items](../Items/Items.md).
- Резервные таблицы — псевдонимы основных файлов выбора оружия; их правка не меняет действующие файлы выбора.

## Localization

Отображаемые названия и подписи локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не заявляется.

## Open questions

- Резервные файлы описаны как псевдонимы. Прежде чем считать их действующими данными, нужно подтвердить, загружает ли игра хоть один из них.
