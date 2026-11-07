---
schema: 1
lang: en
family: items-clothes
title: "Clothes - clothing colour variants"
tables:
  - Clothes
table_ids:
  - Items.Clothes
files:
  - Items/Clothes.xml
root: CLOTHESLIST
record: CLOTHES
key: uiIndex
sources:
  - Clothes table data
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# Clothes - clothing colour variants

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CLOTHESLIST/CLOTHES/uiIndex`](#clotheslistclothesuiindex)
  - [`CLOTHESLIST/CLOTHES/szName`](#clotheslistclothesszname)
  - [`CLOTHESLIST/CLOTHES/Vest`](#clotheslistclothesvest)
  - [`CLOTHESLIST/CLOTHES/Pants`](#clotheslistclothespants)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Clothes.xml` maps a clothing variant to a display name and to the vest and pants palette keys used by the clothing renderer.

## Tables

| Table | File | Notes |
|---|---|---|
| `Clothes` | `Items/Clothes.xml` | v4 and v5 have the same field paths; values differ. Empty `Vest`/`Pants` elements are meaningful. |

## Structure

```
CLOTHESLIST
└── CLOTHES
    ├── uiIndex
    ├── szName
    ├── Vest
    └── Pants
```

## Fields

### `CLOTHESLIST/CLOTHES/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the clothing row and identity of the XML record.
- **Values:** Non-negative integer; unique within the file.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** clothes table data; field metadata.
- **Confidence:** `inferred`

### `CLOTHESLIST/CLOTHES/szName`

- **Type:** `String` (`String`)
- **Description:** Display name of the clothing variant.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** clothes table data; field metadata.
- **Confidence:** `inferred`

### `CLOTHESLIST/CLOTHES/Vest`

- **Type:** `String` (`String`)
- **Description:** Vest palette key used by the clothing renderer. An empty element means no vest override.
- **Values:** Vest palette key, for example `YELLOWVEST`. An empty element means no vest override.
- **Default:** empty element means no override for that clothing layer.
- **Source:** clothes table data; field metadata.
- **Confidence:** `inferred`

### `CLOTHESLIST/CLOTHES/Pants`

- **Type:** `String` (`String`)
- **Description:** Pants palette key used by the clothing renderer. An empty element means no pants override.
- **Values:** Pants palette key, for example `GREENPANTS`. An empty element means no pants override.
- **Default:** empty element means no override for that clothing layer.
- **Source:** clothes table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `Vest` and `Pants` are palette keys, not item IDs.
- The clothing row is presentation data and does not define armour or protection.

## Localization

`szName` is a display label; `Vest` and `Pants` are palette keys and are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Clothes table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and legacy field labels; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The renderer that consumes the palette keys is outside this table; the string values are keys, not free-form colours.
