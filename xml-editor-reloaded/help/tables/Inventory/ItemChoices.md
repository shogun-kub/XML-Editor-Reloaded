---
schema: 1
lang: en
family: inventory-item-choices
title: "ItemChoices - enemy and militia armour and equipment choices"
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
status: draft
updated: 2026-11-08
---

# ItemChoices - enemy and militia armour and equipment choices

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

Item-choice files define up to 50 armour or equipment item IDs for an enemy/militia rank or category group. `EnemyItemChoices.xml` is the general table; the rank files override it for their own groups.

## Tables

| Table | File | Notes |
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

- **Type:** `Integer`
- **Description:** Index of the item-choice group. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/name`

- **Type:** `String`
- **Description:** Category label for the group, such as `Helmets`, `Vests` or `Leggings`.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/ubChoices`

- **Type:** `Integer`
- **Description:** Number of valid item slots in this group.
- **Numeric semantics:** higher = more choices; negative values are not meaningful.
- **Values:** Count `0..50`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo1`

- **Type:** `Integer`
- **Description:** Item ID in slot 1. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo2`

- **Type:** `Integer`
- **Description:** Item ID in slot 2. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo3`

- **Type:** `Integer`
- **Description:** Item ID in slot 3. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo4`

- **Type:** `Integer`
- **Description:** Item ID in slot 4. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo5`

- **Type:** `Integer`
- **Description:** Item ID in slot 5. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo6`

- **Type:** `Integer`
- **Description:** Item ID in slot 6. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo7`

- **Type:** `Integer`
- **Description:** Item ID in slot 7. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo8`

- **Type:** `Integer`
- **Description:** Item ID in slot 8. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo9`

- **Type:** `Integer`
- **Description:** Item ID in slot 9. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo10`

- **Type:** `Integer`
- **Description:** Item ID in slot 10. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo11`

- **Type:** `Integer`
- **Description:** Item ID in slot 11. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo12`

- **Type:** `Integer`
- **Description:** Item ID in slot 12. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo13`

- **Type:** `Integer`
- **Description:** Item ID in slot 13. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo14`

- **Type:** `Integer`
- **Description:** Item ID in slot 14. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo15`

- **Type:** `Integer`
- **Description:** Item ID in slot 15. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo16`

- **Type:** `Integer`
- **Description:** Item ID in slot 16. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo17`

- **Type:** `Integer`
- **Description:** Item ID in slot 17. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo18`

- **Type:** `Integer`
- **Description:** Item ID in slot 18. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo19`

- **Type:** `Integer`
- **Description:** Item ID in slot 19. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo20`

- **Type:** `Integer`
- **Description:** Item ID in slot 20. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo21`

- **Type:** `Integer`
- **Description:** Item ID in slot 21. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo22`

- **Type:** `Integer`
- **Description:** Item ID in slot 22. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo23`

- **Type:** `Integer`
- **Description:** Item ID in slot 23. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo24`

- **Type:** `Integer`
- **Description:** Item ID in slot 24. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo25`

- **Type:** `Integer`
- **Description:** Item ID in slot 25. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo26`

- **Type:** `Integer`
- **Description:** Item ID in slot 26. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo27`

- **Type:** `Integer`
- **Description:** Item ID in slot 27. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo28`

- **Type:** `Integer`
- **Description:** Item ID in slot 28. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo29`

- **Type:** `Integer`
- **Description:** Item ID in slot 29. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo30`

- **Type:** `Integer`
- **Description:** Item ID in slot 30. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo31`

- **Type:** `Integer`
- **Description:** Item ID in slot 31. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo32`

- **Type:** `Integer`
- **Description:** Item ID in slot 32. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo33`

- **Type:** `Integer`
- **Description:** Item ID in slot 33. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo34`

- **Type:** `Integer`
- **Description:** Item ID in slot 34. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo35`

- **Type:** `Integer`
- **Description:** Item ID in slot 35. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo36`

- **Type:** `Integer`
- **Description:** Item ID in slot 36. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo37`

- **Type:** `Integer`
- **Description:** Item ID in slot 37. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo38`

- **Type:** `Integer`
- **Description:** Item ID in slot 38. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo39`

- **Type:** `Integer`
- **Description:** Item ID in slot 39. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo40`

- **Type:** `Integer`
- **Description:** Item ID in slot 40. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo41`

- **Type:** `Integer`
- **Description:** Item ID in slot 41. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo42`

- **Type:** `Integer`
- **Description:** Item ID in slot 42. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo43`

- **Type:** `Integer`
- **Description:** Item ID in slot 43. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo44`

- **Type:** `Integer`
- **Description:** Item ID in slot 44. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo45`

- **Type:** `Integer`
- **Description:** Item ID in slot 45. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo46`

- **Type:** `Integer`
- **Description:** Item ID in slot 46. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo47`

- **Type:** `Integer`
- **Description:** Item ID in slot 47. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo48`

- **Type:** `Integer`
- **Description:** Item ID in slot 48. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo49`

- **Type:** `Integer`
- **Description:** Item ID in slot 49. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYITEMCHOICESLIST/ENEMYITEMCHOICES/bItemNo50`

- **Type:** `Integer`
- **Description:** Item ID in slot 50. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `bItemNoN` values reference `Items.xml` item indexes.

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for inferred fields is not stated by the table data.
