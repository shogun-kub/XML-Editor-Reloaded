---
schema: 1
lang: en
family: inventory-imp-item-choices
title: "IMPItemChoices - IMP starting item choices"
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
updated: 2026-11-08
---

# IMPItemChoices - IMP starting item choices

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

`IMPItemChoices.xml` defines the item groups and item IDs an IMP mercenary can choose during creation. `ubNumItems` controls how many items are granted from the group.

## Tables

| Table | File | Notes |
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
- **Description:** Index of the IMP item-choice group. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/name`

- **Type:** `String`
- **Description:** Group label stored in the table, but the loader does not read this element; it is informational/ignored.
- **Values:** Free text; ignored at load time.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`
### `IMPITEMCHOICESLIST/IMPITEMCHOICES/ubChoices`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of valid item slots in the group.
- **Numeric semantics:** higher = more choices; negative values are not meaningful.
- **Values:** Count `0..50`.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/ubNumItems`

- **Type:** `Integer` (`UINT8`)
- **Description:** Number of items the IMP may take from this group.
- **Numeric semantics:** higher = more items; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo1`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 1. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo2`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 2. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo3`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 3. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo4`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 4. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo5`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 5. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo6`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 6. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo7`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 7. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo8`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 8. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo9`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 9. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo10`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 10. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo11`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 11. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo12`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 12. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo13`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 13. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo14`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 14. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo15`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 15. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo16`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 16. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo17`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 17. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo18`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 18. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo19`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 19. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo20`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 20. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo21`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 21. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo22`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 22. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo23`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 23. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo24`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 24. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo25`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 25. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo26`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 26. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo27`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 27. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo28`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 28. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo29`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 29. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo30`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 30. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo31`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 31. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo32`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 32. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo33`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 33. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo34`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 34. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo35`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 35. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo36`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 36. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo37`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 37. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo38`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 38. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo39`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 39. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo40`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 40. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo41`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 41. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo42`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 42. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo43`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 43. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo44`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 44. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo45`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 45. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo46`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 46. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo47`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 47. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo48`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 48. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo49`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 49. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

### `IMPITEMCHOICESLIST/IMPITEMCHOICES/bItemNo50`

- **Type:** `Integer` (`INT16`)
- **Description:** Item ID in IMP slot 50. `0` means the slot is empty; other values reference an item ID. It is an identifier.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (choice-table loader); table data.
- **Confidence:** `source`

## Relations

- `bItemNoN` values reference [Items](../Items/Items.md) item indexes.

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
