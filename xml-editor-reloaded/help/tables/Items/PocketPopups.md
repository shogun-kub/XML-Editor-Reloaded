---
schema: 1
lang: en
family: items-pockets
title: "PocketPopups - NIV pocket menus"
tables:
  - PocketPopups
table_ids:
  - Items.PocketPopups
files:
  - Items/PocketPopups.xml
root: POCKETPOPUPS
record: POCKET
key: pIndex
sources:
  - Pocket popup table data
  - Item metadata
status: reviewed
updated: 2026-11-07
---

# PocketPopups - NIV pocket menus

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`POCKETPOPUPS/POCKET/pIndex`](#pocketpopupspocketpindex)
  - [`POCKETPOPUPS/POCKET/pName`](#pocketpopupspocketpname)
  - [`POCKETPOPUPS/POCKET/popup/generator/id`](#pocketpopupspocketpopupgeneratorid)
  - [`POCKETPOPUPS/POCKET/popup/subMenu/name`](#pocketpopupspocketpopupsubmenuname)
  - [`POCKETPOPUPS/POCKET/popup/subMenu/generator/id`](#pocketpopupspocketpopupsubmenugeneratorid)
  - [`POCKETPOPUPS/POCKET/popup/subMenu/subMenu/name`](#pocketpopupspocketpopupsubmenusubmenuname)
  - [`POCKETPOPUPS/POCKET/popup/subMenu/subMenu/generator/id`](#pocketpopupspocketpopupsubmenusubmenugeneratorid)
  - [`POCKETPOPUPS/POCKET/generator/id`](#pocketpopupspocketgeneratorid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`PocketPopups.xml` defines the popup menu and nested submenus shown for a pocket in the New Inventory System. Each popup is built by a named generator rather than by a fixed item list.

## Tables

| Table | File | Notes |
|---|---|---|
| `PocketPopups` | `Items/PocketPopups.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
POCKETPOPUPS
└── POCKET
    ├── pIndex
    ├── pName
    ├── popup
    │   ├── generator
    │   │   └── id
    │   └── subMenu
    │       ├── name
    │       ├── generator
    │       │   └── id
    │       └── subMenu
    │           ├── name
    │           └── generator
    │               └── id
    └── generator
        └── id
```

## Fields

### `POCKETPOPUPS/POCKET/pIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Pocket index that owns this popup definition. Numeric semantics: Identifier; larger numbers have no inherent meaning beyond row order, and negative values are invalid.
- **Only used with:** NIV.
- **Values:** Pocket index; it should match a row in [Pockets](Pockets.md).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** pocket popup table data; field metadata.
- **Confidence:** `inferred`

### `POCKETPOPUPS/POCKET/pName`

- **Type:** `String` (`String`)
- **Description:** Display name of the pocket popup.
- **Only used with:** NIV.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** pocket popup table data; field metadata.
- **Confidence:** `inferred`

### `POCKETPOPUPS/POCKET/popup/generator/id`

- **Type:** `String` (`String`)
- **Description:** Generator that builds the top-level popup contents.
- **Only used with:** NIV.
- **Values:** Generator key registered by the inventory popup system; examples are `addWeapons`, `addAmmo`, `addGrenades`, `addKits`, `addMisc`, `addFaceGear`, `addWeaponGroups`, `addRifleGrenades` and `addRocketAmmo`.
- **Default:** empty string when omitted.
- **Source:** pocket popup table data; field metadata.
- **Confidence:** `inferred`

### `POCKETPOPUPS/POCKET/popup/subMenu/name`

- **Type:** `String` (`String`)
- **Description:** Label of a submenu in the popup.
- **Only used with:** NIV.
- **Values:** Display label of a submenu inside the pocket popup.
- **Default:** empty string when omitted.
- **Source:** pocket popup table data; field metadata.
- **Confidence:** `inferred`

### `POCKETPOPUPS/POCKET/popup/subMenu/generator/id`

- **Type:** `String` (`String`)
- **Description:** Generator that builds the submenu contents.
- **Only used with:** NIV.
- **Values:** Generator key for the submenu contents.
- **Default:** empty string when omitted.
- **Source:** pocket popup table data; field metadata.
- **Confidence:** `inferred`

### `POCKETPOPUPS/POCKET/popup/subMenu/subMenu/name`

- **Type:** `String` (`String`)
- **Description:** Label of a nested submenu.
- **Only used with:** NIV.
- **Values:** Display label of a nested submenu.
- **Default:** empty string when omitted.
- **Source:** pocket popup table data; field metadata.
- **Confidence:** `inferred`

### `POCKETPOPUPS/POCKET/popup/subMenu/subMenu/generator/id`

- **Type:** `String` (`String`)
- **Description:** Generator that builds the nested submenu contents.
- **Only used with:** NIV.
- **Values:** Generator key for the nested submenu contents.
- **Default:** empty string when omitted.
- **Source:** pocket popup table data; field metadata.
- **Confidence:** `inferred`

### `POCKETPOPUPS/POCKET/generator/id`

- **Type:** `String` (`String`)
- **Description:** Generator used when the pocket has a direct generator instead of a `popup` wrapper.
- **Only used with:** NIV.
- **Values:** Generator key used when the popup is attached directly to the pocket without a `popup` wrapper.
- **Default:** empty string when omitted.
- **Source:** pocket popup table data; field metadata.
- **Confidence:** `inferred`

## Relations

- `pIndex` refers to [Pockets](Pockets.md).
- Generator IDs are keys in the inventory popup system, not item IDs.

## Localization

`pName` and submenu `name` elements are display labels; generator IDs are code keys and are not translated.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Pocket popup table data.
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use XML element names, lookup values and the table schema comments; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The complete generator registry is not part of the table; the examples are the generator keys seen in the schema examples.
