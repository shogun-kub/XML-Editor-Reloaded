---
schema: 1
lang: en
family: inventory-gun-choices
title: "GunChoices - enemy and militia weapon choices"
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
status: draft
updated: 2026-11-08
---

# GunChoices - enemy and militia weapon choices

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

Gun-choice files define up to 50 weapon item IDs for a rank/coolness group. The six main rank files are the live choices; `EnemyGunChoices.xml` is the general table. The six files under `Inventory/Backup original Gun Choices/` are historical copies and are listed as aliases, not as separate live profiles.

## Tables

| Table | File | Notes |
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

- **Type:** `Integer`
- **Description:** Index of the weapon-choice group. It is an array key, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/name`

- **Type:** `String`
- **Description:** Label of the choice group, for example a coolness percentage or rank.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/ubChoices`

- **Type:** `Integer`
- **Description:** Number of valid weapon slots in this group. Higher values mean more choices; `0` means none. Numeric semantics: higher = more choices; negative values are not meaningful.
- **Values:** Count `0..50`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo1`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 1. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo2`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 2. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo3`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 3. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo4`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 4. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo5`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 5. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo6`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 6. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo7`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 7. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo8`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 8. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo9`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 9. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo10`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 10. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo11`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 11. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo12`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 12. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo13`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 13. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo14`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 14. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo15`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 15. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo16`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 16. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo17`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 17. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo18`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 18. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo19`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 19. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo20`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 20. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo21`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 21. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo22`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 22. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo23`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 23. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo24`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 24. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo25`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 25. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo26`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 26. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo27`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 27. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo28`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 28. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo29`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 29. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo30`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 30. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo31`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 31. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo32`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 32. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo33`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 33. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo34`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 34. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo35`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 35. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo36`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 36. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo37`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 37. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo38`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 38. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo39`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 39. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo40`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 40. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo41`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 41. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo42`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 42. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo43`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 43. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo44`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 44. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo45`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 45. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo46`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 46. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo47`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 47. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo48`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 48. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo49`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 49. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ENEMYGUNCHOICESLIST/ENEMYGUNCHOICES/bItemNo50`

- **Type:** `Integer`
- **Description:** Item ID in weapon slot 50. `0` means the slot is empty; other values reference an item ID. It is an identifier, not a magnitude.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `bItemNoN` values reference `Items.xml` item indexes.
- The backup tables are aliases of the main gun-choice files; editing them does not change the live choice files.

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- The backup files are documented as aliases. Confirm whether the game still loads any of them before treating them as live data.
