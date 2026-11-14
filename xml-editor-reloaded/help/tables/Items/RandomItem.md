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
  - Game code (random-item loader and selection)
  - Random item table data
  - Item metadata
status: reviewed
updated: 2026-11-08
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

`RandomItem.xml` defines named random-item categories. A category holds up to ten direct item IDs and up to ten nested categories; both lists are independent, and a lookup flattens them into one candidate list before choosing an item. Categories are referenced from `Items.randomitem`.

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

- **Type:** `Integer` (`UINT16`)
- **Description:** Index of the random-item category; the loader stores the row in `gRandomItemClass[uiIndex]`. It is referenced by `Items.randomitem` and by the `randomitem*` slots of other rows.
- **Values:** `0..999` (the array size is `RANDOM_ITEM_MAX_CATEGORIES` = `1000`); rows at or above it are discarded, duplicates overwrite the earlier row.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/szName`

- **Type:** `String` (`String`)
- **Description:** Informational label. The loader parses the element but deliberately does not store it, so it is never displayed or used by the game.
- **Values:** Free text; ignored at load time.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_RandomItem.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 1. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 2. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 3. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 4. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 5. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 6. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 7. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 8. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 9. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/randomitem10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Nested random-item category in slot 10. Zero leaves the slot empty.
- **Values:** Nested category index (`0..999`); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 1. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 2. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 3. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 4. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 5. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 6. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 7. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 8. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 9. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `RANDOMITEMLIST/RANDOMITEM/item10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Direct item ID in slot 10. Zero leaves the slot empty.
- **Values:** Item ID from [Items](Items.md); `0` leaves the slot empty.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_RandomItem.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

## Relations

- `uiIndex` is referenced by `Items.randomitem` and by the `randomitem1..10` slots of other rows.
- `item1..10` refer to [Items](Items.md); an item that is itself a random item is expanded recursively instead of being added.
- `randomitem1..10` refer to other `RANDOMITEM` rows; their items are appended to the same candidate list rather than the category being chosen as one entry.
- A category already visited during the same lookup is not expanded again (taboo list), which prevents loops.
- Zero in any slot means the slot is empty. The item slots are expanded before the nested categories, and one entry is then chosen uniformly from the flattened list; duplicate entries are not removed.

## Localization

The table has no display text: `szName` is not stored at load time and is never shown; direct item names come from `Items`.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Game code: `XML_RandomItem.cpp` (loader), `Items.cpp` (`GetItemFromRandomItem` and the recursive list builders), `Item Types.h` (`RANDOM_ITEM_CHOICE_TYPE`, `RANDOM_ITEM_MAX_CATEGORIES`).
- Random item table data.
- Item metadata (`Items.randomitem`, `Items.ubCoolness`).
- Field types and limits: `RANDOM_ITEM_CHOICE_TYPE` members and the loader casts.
- Descriptions cover the code path that consumes each field; no row counts or observed data ranges are used.

## Open questions

- The table has no weight field: selection is uniform over the flattened entries, so an item listed twice (directly or through two categories) is twice as likely. Whether modders expect that is not documented.
- A category referenced more than once is expanded only once per lookup because of the visited-class taboo list; the format does not document this.
