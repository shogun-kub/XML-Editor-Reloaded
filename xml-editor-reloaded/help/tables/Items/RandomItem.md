---
schema: 1
lang: en
family: items-random
title: "RandomItem - random item categories"
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
  - Random item table data
  - Item metadata
status: draft
updated: 2026-11-07
---

# RandomItem - random item categories

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

`RandomItem.xml` defines named random-item categories. A category can contain direct item IDs and nested random categories, allowing random tables to be composed.

## Tables

| Table | File | Notes |
|---|---|---|
| `RandomItem` | `Items/RandomItem.xml` | v4 and v5 have the same field paths; values differ. |

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

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the random-item category and identity of the XML record.
- **Values:** Non-negative index; unique within the file. It is referenced by `randomitem*` slots in this table and by random-item selection.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/szName`

- **Type:** `String` (`String`)
- **Description:** Display name of the random category.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 1. Zero leaves the slot empty.
- **Values:** Nested random-item category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 2. Zero leaves the slot empty.
- **Values:** Nested random-category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 3. Zero leaves the slot empty.
- **Values:** Nested random-category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 4. Zero leaves the slot empty.
- **Values:** Nested random-category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 5. Zero leaves the slot empty.
- **Values:** Nested random-category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 6. Zero leaves the slot empty.
- **Values:** Nested random-category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 7. Zero leaves the slot empty.
- **Values:** Nested random-category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 8. Zero leaves the slot empty.
- **Values:** Nested random-category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 9. Zero leaves the slot empty.
- **Values:** Nested random-category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/randomitem10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 10. Zero leaves the slot empty.
- **Values:** Nested random-category index; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 1. Zero leaves the slot empty.
- **Values:** Direct item ID from [Items](Items.md); `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 2. Zero leaves the slot empty.
- **Values:** Direct item ID; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 3. Zero leaves the slot empty.
- **Values:** Direct item ID; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 4. Zero leaves the slot empty.
- **Values:** Direct item ID; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 5. Zero leaves the slot empty.
- **Values:** Direct item ID; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 6. Zero leaves the slot empty.
- **Values:** Direct item ID; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 7. Zero leaves the slot empty.
- **Values:** Direct item ID; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 8. Zero leaves the slot empty.
- **Values:** Direct item ID; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 9. Zero leaves the slot empty.
- **Values:** Direct item ID; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

### `RANDOMITEMLIST/RANDOMITEM/item10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 10. Zero leaves the slot empty.
- **Values:** Direct item ID; `0` means the slot is empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random item table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `randomitem1..10` refer to [RandomItem](RandomItem.md) rows; `item1..10` refer to [Items](Items.md).
- Zero in either slot means the slot is empty.

## Localization

`szName` is an internal category label; direct item names come from `Items`.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Random item table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and the table schema comments; entries marked `inferred` do not claim a code-verified formula.

## Open questions

Whether a slot may contain both a nested category and a direct item, or only one of them, is not stated by the table.
