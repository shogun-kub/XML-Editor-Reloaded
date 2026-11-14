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
  - Game code (pocket-popup loader and popup definition)
  - Item metadata
status: reviewed
updated: 2026-11-08
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
| `PocketPopups` | `Items/PocketPopups.xml` | v4 and v5 have the same field paths and, in the reference data, the same values. |

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

- **Type:** `Integer` (`UINT8`)
- **Description:** Pocket type that owns this popup definition. It is the key of the runtime `LBEPocketPopup` map, which the interface looks up with the pocket type returned for the slot; that value is also the `pIndex` of the corresponding `Pockets` row.
- **Values:** `0..255`; the loader casts the parsed value to `UINT8`. Shipped values are pocket-type IDs `1..22`.
- **Default:** `0` in the first record; a later record keeps the previous pocket ID when the element is omitted, because the parser does not reset the current ID per `POCKET`.
- **Source:** game code (pocket-popup loader; pocket-type lookup); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/pName`

- **Type:** `String` (`String`)
- **Description:** Pocket name present in the data. The pocket-popup loader does not read it, so it has no effect on the popup; where a pocket name is shown elsewhere (for example in Bobby Ray's LBE descriptions), it comes from `Pockets`.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (pocket-popup loader); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/generator/id`

- **Type:** `String` (`String`)
- **Description:** Generator that builds the top-level popup contents. The element may repeat, and each occurrence adds a generator to the popup. Unknown names silently map to no generator.
- **Values:** A generator name accepted by the loader: `dummy`, `addArmor`, `addLBE`, `addWeapons`, `addWeaponGroups`, `addGrenades`, `addBombs`, `addFaceGear`, `addAmmo`, `addRifleGrenades`, `addRocketAmmo`, `addMisc` or `addKits`; any other value (including `none`) adds nothing.
- **Default:** empty string when omitted; no generator is added.
- **Source:** game code (pocket-popup loader and generator registry); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/subMenu/name`

- **Type:** `String` (`String`)
- **Description:** Label of a submenu in the popup. The loader stores it in a 128-wide-character buffer (longer text is truncated) and applies it to the submenu option.
- **Values:** Free text; the label is displayed in the popup.
- **Default:** empty string when omitted.
- **Source:** game code (pocket-popup loader; submenu renaming); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/subMenu/generator/id`

- **Type:** `String` (`String`)
- **Description:** Generator that builds the submenu contents. It uses the same name registry as the top-level generator, and unknown names add nothing.
- **Values:** Same generator names as `popup/generator/id`.
- **Default:** empty string when omitted; no generator is added.
- **Source:** game code (pocket-popup loader and generator registry); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/subMenu/subMenu/name`

- **Type:** `String` (`String`)
- **Description:** Label of a nested submenu. Submenus may nest; the loader tracks up to `POPUP_MAX_SUB_POPUPS` (`24`) levels.
- **Values:** Free text; the label is displayed in the nested popup.
- **Default:** empty string when omitted.
- **Source:** game code (pocket-popup loader; nested submenu renaming); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/popup/subMenu/subMenu/generator/id`

- **Type:** `String` (`String`)
- **Description:** Generator that builds the nested submenu contents. It uses the same name registry as the top-level generator, and unknown names add nothing.
- **Values:** Same generator names as `popup/generator/id`.
- **Default:** empty string when omitted; no generator is added.
- **Source:** game code (pocket-popup loader and generator registry); pocket popup table data.
- **Confidence:** `source`

### `POCKETPOPUPS/POCKET/generator/id`

- **Type:** `String` (`String`)
- **Description:** A generator placed directly under `POCKET` instead of inside a `popup` wrapper. The loader only reads `generator` under `popup` or `subMenu`, so this element is ignored and creates no popup; the shipped data nevertheless contains such elements (for example `pIndex` `13`, `14`, `20` and `21`).
- **Values:** Any generator name; the loader does not consume it in this position.
- **Default:** empty string when omitted; no effect.
- **Source:** game code (pocket-popup loader); pocket popup table data.
- **Confidence:** `source`

## Relations

- `pIndex` is the pocket type used as the `LBEPocketPopup` key and corresponds to a [Pockets](Pockets.md) row `pIndex`.
- Generator IDs are keys in the inventory popup system, not item IDs.

## Localization

`pName` appears in localization overlays but the loader does not read it, so it does not affect the displayed pocket; the pocket name comes from the localized `Pockets` table. Submenu `name` labels are displayed verbatim from the table, and generator IDs are code keys that are not translated.

## Versions

v4 and v5 use the same field paths, and the reference files contain the same values.

## Sources

- Pocket popup table data.
- Game code (pocket-popup loader, generator registry and popup definition).
- Item metadata.
- Field types and limits: item metadata.
- Descriptions use the game code and XML element names; entries marked `inferred` do not claim a code-verified formula.

## Open questions

The shipped data contains direct `POCKET/generator` elements that the loader ignores. The source does not document whether this is a data error or a feature that was never completed; the table cannot make those pockets use a generator without a `popup` wrapper.
