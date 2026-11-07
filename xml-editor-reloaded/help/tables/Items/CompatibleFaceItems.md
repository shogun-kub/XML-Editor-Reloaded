---
schema: 1
lang: en
family: items-attachments
title: "CompatibleFaceItems - face-item compatibility"
tables:
  - CompatibleFaceItems
table_ids:
  - Items.CompatibleFaceItems
files:
  - Items/CompatibleFaceItems.xml
root: COMPATIBLEFACEITEMLIST
record: COMPATIBLEFACEITEM
key: compatiblefaceitemIndex + itemIndex
sources:
  - Compatible face-item table data
  - Item metadata
status: draft
updated: 2026-11-07
---

# CompatibleFaceItems - face-item compatibility

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`COMPATIBLEFACEITEMLIST/COMPATIBLEFACEITEM/compatiblefaceitemIndex`](#compatiblefaceitemlistcompatiblefaceitemcompatiblefaceitemindex)
  - [`COMPATIBLEFACEITEMLIST/COMPATIBLEFACEITEM/itemIndex`](#compatiblefaceitemlistcompatiblefaceitemitemindex)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`CompatibleFaceItems.xml` declares pairs of face items that can be worn together. Each row is one compatible pair; the relation is normally written symmetrically.

## Tables

| Table | File | Notes |
|---|---|---|
| `CompatibleFaceItems` | `Items/CompatibleFaceItems.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
COMPATIBLEFACEITEMLIST
└── COMPATIBLEFACEITEM      one compatible face-item pair
    ├── compatiblefaceitemIndex  first face item ID
    └── itemIndex                second face item ID
```

## Fields

### `COMPATIBLEFACEITEMLIST/COMPATIBLEFACEITEM/compatiblefaceitemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the first face item. The row declares that the two face items can be worn together.
- **Values:** Item ID of the first face item.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** compatible face-item table data; field metadata.
- **Confidence:** `inferred`

### `COMPATIBLEFACEITEMLIST/COMPATIBLEFACEITEM/itemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item ID of the second face item.
- **Values:** Item ID of the second face item.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** compatible face-item table data; field metadata.
- **Confidence:** `inferred`

## Relations

- Both columns point to face items in [Items](Items.md).
- The pair is a positive compatibility rule; it does not define the face-item slots themselves.

## Localization

This table has no display strings.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Compatible face-item table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The table stores one direction; the loader may treat the pair as symmetric, but the XML schema does not say so.
