---
schema: 1
lang: en
family: layout-main-menu
title: "LayoutMainMenu - main-menu images and buttons"
tables:
  - LayoutMainMenu
table_ids:
  - Layout.LayoutMainMenu
files:
  - Layout/LayoutMainMenu.xml
root: MAINMENU
record: IMAGE
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# LayoutMainMenu - main-menu images and buttons

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MAINMENU/IMAGE/uiIndex`](#mainmenuimageuiindex)
  - [`MAINMENU/IMAGE/Description`](#mainmenuimagedescription)
  - [`MAINMENU/IMAGE/FileName`](#mainmenuimagefilename)
  - [`MAINMENU/IMAGE/Visible`](#mainmenuimagevisible)
  - [`MAINMENU/IMAGE/FitToScreen`](#mainmenuimagefittoscreen)
  - [`MAINMENU/IMAGE/StretchImage`](#mainmenuimagestretchimage)
  - [`MAINMENU/IMAGE/CenterImage`](#mainmenuimagecenterimage)
  - [`MAINMENU/IMAGE/ImagePositionRelative`](#mainmenuimageimagepositionrelative)
  - [`MAINMENU/IMAGE/ImagePositionX`](#mainmenuimageimagepositionx)
  - [`MAINMENU/IMAGE/ImagePositionY`](#mainmenuimageimagepositiony)
  - [`MAINMENU/IMAGE/MainMenuX`](#mainmenuimagemainmenux)
  - [`MAINMENU/IMAGE/MainMenuY`](#mainmenuimagemainmenuy)
  - [`MAINMENU/IMAGE/MainMenuYSpace`](#mainmenuimagemainmenuyspace)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`LayoutMainMenu.xml` positions the main-menu buttons and optional background/logo images. `uiIndex = 0` is required for the menu-button layout; other entries are images.

## Tables

| Table | File | Notes |
|---|---|---|
| `LayoutMainMenu` | `Layout/LayoutMainMenu.xml` | — |

## Structure

```
MAINMENU
└── IMAGE
    ├── uiIndex
    ├── Description
    ├── FileName
    ├── Visible
    ├── FitToScreen
    ├── StretchImage
    ├── CenterImage
    ├── ImagePositionRelative
    ├── ImagePositionX
    ├── ImagePositionY
    ├── MainMenuX
    ├── MainMenuY
    └── MainMenuYSpace
```

## Fields

### `MAINMENU/IMAGE/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the menu image/entry. `0` is the special entry that defines the menu-button layout.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/Description`

- **Type:** `String`
- **Description:** Editor/UI description of the image entry.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/FileName`

- **Type:** `String`
- **Description:** Image file path, relative to the game data directory. Empty for the menu-button layout entry.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/Visible`

- **Type:** `Boolean`
- **Description:** If set, the image is shown on the main menu. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/FitToScreen`

- **Type:** `Boolean`
- **Description:** If set, the image is scaled to fit the screen. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/StretchImage`

- **Type:** `Boolean`
- **Description:** If set, the image is stretched rather than proportionally scaled. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/CenterImage`

- **Type:** `Boolean`
- **Description:** If set, the image is centered in its area. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/ImagePositionRelative`

- **Type:** `Boolean`
- **Description:** If set, `ImagePositionX` and `ImagePositionY` are relative to the image area rather than absolute screen coordinates. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/ImagePositionX`

- **Type:** `Integer`
- **Description:** Horizontal image position in pixels.
- **Numeric semantics:** increasing = right, decreasing = left.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/ImagePositionY`

- **Type:** `Integer`
- **Description:** Vertical image position in pixels.
- **Numeric semantics:** increasing = down, decreasing = up.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/MainMenuX`

- **Type:** `Integer`
- **Description:** Horizontal position of the menu buttons in pixels.
- **Numeric semantics:** increasing = right; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/MainMenuY`

- **Type:** `Integer`
- **Description:** Vertical position of the first menu button in pixels.
- **Numeric semantics:** increasing = down; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `MAINMENU/IMAGE/MainMenuYSpace`

- **Type:** `Integer`
- **Description:** Vertical spacing between menu buttons in pixels.
- **Numeric semantics:** higher = more spacing; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- This table is part of the strategic army and inventory data; references are described in the field notes.

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
