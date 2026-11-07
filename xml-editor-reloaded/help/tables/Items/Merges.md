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
- **Description:** Item ID of the first input item in the merge. Numeric semantics: Identifier; larger numbers have no inherent meaning beyond row order, and negative values are invalid.
- **Values:** Item ID of the first input.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merge table data; field metadata.
- **Confidence:** `inferred`

### `MERGELIST/MERGE/secondItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the second input item in the merge. Numeric semantics: Identifier; larger numbers have no inherent meaning beyond row order, and negative values are invalid.
- **Values:** Item ID of the second input.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merge table data; field metadata.
- **Confidence:** `inferred`

### `MERGELIST/MERGE/firstResultingItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in the first result slot. Numeric semantics: Identifier; larger numbers have no inherent meaning beyond row order, and negative values are invalid.
- **Values:** Item ID of the first result.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merge table data; field metadata.
- **Confidence:** `inferred`

### `MERGELIST/MERGE/secondResultingItemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID produced in the second result slot; `0` means no second result. Numeric semantics: Identifier; larger numbers have no inherent meaning beyond row order, and negative values are invalid.
- **Values:** Item ID of the second result; `0` means the merge produces only one item.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merge table data; field metadata.
- **Confidence:** `inferred`

### `MERGELIST/MERGE/mergeType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Merge/use type that selects the action, skill check and result handling. Numeric semantics: Numeric code that selects a listed type/class; larger numbers have no inherent ordering, and negative values are outside the listed choices.
- **Values:** IDs from `Lookup.MergeType`: `0` Destruction, `1` Combine, `2` Treat Armour, `3` Explosive (Hard), `4` Easy, `5` Electronic, `6` Use Item, `7` Use Item (Hard), `8` Swap Barrel, `9` Explosive (Easy), `10` Mechanical (Easy), `11` Mechanical (Hard), `12` Tripwire Roll, `13` Use (Drain Status), `14` Cannibalize, `15` Lose 10 percent.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merge table data; field metadata.
- **Confidence:** `source`

### `MERGELIST/MERGE/APCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** AP cost of the merge. Numeric semantics: Amount of money or cost; higher values mean a larger amount, and negative values are not meaningful.
- **Values:** AP cost; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** merge table data; field metadata.
- **Confidence:** `inferred`

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
