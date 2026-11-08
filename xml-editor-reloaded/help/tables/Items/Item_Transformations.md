---
schema: 1
lang: en
family: items-merges
title: "Item_Transformations - item transformations"
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
updated: 2026-11-08
---

# Item_Transformations - item transformations

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

`Item_Transformations.xml` defines a transformation menu for an item: AP and breath-point (BP) costs, up to ten result items, and the menu/tooltip text shown to the player.

## Tables

| Table | File | Notes |
|---|---|---|
| `Item_Transformations` | `Items/Item_Transformations.xml` | v4 and v5 have the same field paths; values differ. |

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
- **Description:** Item ID of the item being transformed. The record itself is stored at `Transform[N]` where `N` is the row's document position, not at `usItem`, so this field only links the row to its item. Several rows may name the same item, and each matching row becomes one entry in the menu. Rows past `MAXITEMS` are dropped.
- **Numeric semantics:** Parsed through an `INT16`, so a literal above `32767` arrives negative and is then stored unchanged in a `UINT16`. A missing element leaves the pre-set `-1` (`65535`), which is the sentinel that stops every menu and lookup scan — a row without `usItem` therefore hides the rest of the table.
- **Values:** Item ID; `0` and negative values also trip a debug assertion (`Entry #N missing proper <usItem> tag`).
- **Default:** `65535` (`0xFFFF`) when omitted, which is not the same as `0`.
- **Source:** game code (the transformation loader; the menu and lookup stop at `65535`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult1`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in the first result slot. Slots are filled in document order: the loader accepts any element name containing `usResult` and writes each value into the next slot, so the trailing digit is ignored and a `usResult2` placed before `usResult1` would land in slot 1. `0` leaves the slot empty; the same item may appear in several slots.
- **Numeric semantics:** Only slots above `0` produce items, and a positive value that names a random-item entry is resolved to a concrete item first.
- **Values:** Item ID; `0` leaves the slot empty. The game reads exactly 10 slots (`MAX_NUM_TRANSFORMATION_RESULTS`), while the loader does not bound its counter, so an eleventh `usResult*` element would be written past the array.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`; 10 result slots).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult2`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 2. Like every `usResult*` element it is placed by document order, not by its digit. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult3`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 3. Like every `usResult*` element it is placed by document order, not by its digit. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult4`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 4. Like every `usResult*` element it is placed by document order, not by its digit. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult5`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 5. Like every `usResult*` element it is placed by document order, not by its digit. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult6`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 6. Like every `usResult*` element it is placed by document order, not by its digit. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult7`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 7. Like every `usResult*` element it is placed by document order, not by its digit. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult8`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 8. Like every `usResult*` element it is placed by document order, not by its digit. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult9`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 9. Like every `usResult*` element it is placed by document order, not by its digit. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usResult10`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in result slot 10, the last of the ten slots. Like every `usResult*` element it is placed by document order, not by its digit. Zero leaves the slot empty; the same item may appear in several slots.
- **Values:** Item ID; `0` leaves the slot empty.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; `TransformObject`; 10 result slots).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/usAPCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** Action-point cost of the transformation. The loader rescales the value with `DynamicAdjustAPConstants`, i.e. `value * AP_MAXIMUM / 100` rounded, so the cost follows the AP system in the AP/BP constants file; `AP_MAXIMUM` defaults to `100`, which leaves the value unchanged.
- **Numeric semantics:** AP. It is charged only in turn-based combat — in real time the AP part is zeroed — and the menu appends `(N AP)` to the row when the cost is above `0` and combat is turn-based.
- **Values:** AP; `0` makes the transformation free in action points.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; the AP-constant adjustment; the point checks; the menu).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/iBPCost`

- **Type:** `Integer` (`INT32`) — it is not a `UINT16`.
- **Description:** Breath-point cost of the transformation, not a blueprint or creation-point cost. A positive value is breath lost and a negative value is breath gained.
- **Numeric semantics:** BP (breath). `EnoughPoints` checks only the AP part, so the breath change is applied by `DeductPoints` without a sufficiency test; robots and vehicles skip it.
- **Values:** Breath points; positive = cost, negative = gain.
- **Default:** `0` when omitted.
- **Source:** game code (the transformation loader; the breath-point deduction, whose comment reads “iBPCost > 0 - breath loss, iBPCost < 0 - breath gain”).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/szMenuRowText`

- **Type:** `String` — stored as a 50-character wide string.
- **Description:** Label of the transformation menu row. The loader converts the UTF-8 text into a fixed `CHAR16[50]` buffer and forces the last position to `'\0'`, so about 49 characters fit and longer text cannot be stored.
- **Values:** Free text; a short action label rather than a full sentence.
- **Default:** empty string when omitted.
- **Source:** game code (the transformation loader; the transformation record).
- **Confidence:** `source`

### `TRANSFORMATIONS_LIST/TRANSFORM/szTooltipText`

- **Type:** `String` — stored as a 300-character wide string.
- **Description:** Tooltip shown for the transformation menu row, read into a fixed `CHAR16[300]` buffer.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (the transformation loader; the transformation record).
- **Confidence:** `source`

## Relations

- `usItem` and `usResult1..10` point to [Items](Items.md).
- A row is found by scanning the loaded `Transform` array for a matching `usItem`; the first row whose `usItem` is `65535` stops the scan, and every matching row becomes an entry in the menu.
- The menu and tooltip strings are presentation data; the action itself is defined by the costs and results.

## Localization

`szMenuRowText` and `szTooltipText` are player-facing text and may be localized by the item overlay.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Item transformation table data.
- Game code: the transformation loader, the transformation record structure, the item transformation routine, the transformation menu and lookup, and the AP/BP handling.
- Field types are the storage types of the game structure; the editor's generic `Integer` kind does not distinguish the unsigned widths used above.

## Open questions

- No open question remains for `iBPCost`: it is the breath-point cost, positive for breath lost and negative for breath gained.
- No open question remains for the menu order: entries follow the document order of the `<TRANSFORM>` rows, and each row matching the item ID becomes one entry.
- Not established here: whether any tool validates `usAPCost` against the `AP_MAXIMUM` rescale; the table always stores the pre-scale value.
