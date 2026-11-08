---
schema: 1
lang: en
family: items-merges
title: "Merges - item merge recipes"
tables:
  - Merges
table_ids:
  - Items.Merges
files:
  - Items/Merges.xml
root: MERGELIST
record: MERGE
key: firstItemIndex + secondItemIndex
sources:
  - Merge table data
  - MergeType lookup table
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# Merges - item merge recipes

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERGELIST/MERGE/firstItemIndex`](#mergelistmergefirstitemindex)
  - [`MERGELIST/MERGE/secondItemIndex`](#mergelistmergeseconditemindex)
  - [`MERGELIST/MERGE/firstResultingItemIndex`](#mergelistmergefirstresultingitemindex)
  - [`MERGELIST/MERGE/secondResultingItemIndex`](#mergelistmergesecondresultingitemindex)
  - [`MERGELIST/MERGE/mergeType`](#mergelistmergemergetype)
  - [`MERGELIST/MERGE/APCost`](#mergelistmergeapcost)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Merges.xml` defines item-to-item recipes: two input items, up to two results, a merge type and an AP cost. The merge type selects the action and skill-check family.

## Tables

| Table | File | Notes |
|---|---|---|
| `Merges` | `Items/Merges.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
MERGELIST
└── MERGE
    ├── firstItemIndex
    ├── secondItemIndex
    ├── firstResultingItemIndex
    ├── secondResultingItemIndex
    ├── mergeType
    └── APCost
```

## Fields

### `MERGELIST/MERGE/firstItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the first input item in the merge.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/secondItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the second input item in the merge.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/firstResultingItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in the first result slot.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/secondResultingItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in the second result slot; `0` means no second result.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/mergeType`

- **Type:** `Integer` (`UINT16`)
- **Description:** Merge/use type that selects the action, skill check and result handling.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

### `MERGELIST/MERGE/APCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** AP cost of the merge.
- **Numeric semantics:** Action-point cost; the loader rescales it with `DynamicAdjustAPConstants`, so the stored value differs when `AP_MAXIMUM` is not `100`.
- **Values:** `0..65535` after rescaling; the loader stores this field as `UINT16`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (merge loader); table data.
- **Confidence:** `source`

## Relations

- All item columns point to [Items](Items.md).
- `mergeType` uses [Lookup.MergeType](../Lookup.md#mergetype).
- Attachment combinations are handled by [AttachmentComboMerges](AttachmentComboMerges.md).

## Localization

This table has no display strings; item names come from `Items`.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Merge table data.
- MergeType lookup table.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The exact skill check for each `mergeType` is defined by the merge implementation, not by this table.
