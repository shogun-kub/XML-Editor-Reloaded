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

- **Type:** `Integer` (`UINT32`)
- **Description:** Index of the menu image/entry. `0` is the special entry that defines the menu-button layout.
- **Values:** `0..4294967295`; the loader stores this field as `UINT32`.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/Description`

- **Type:** `String`
- **Description:** Editor/UI description of the image entry. It is not read by the current loader; kept as data for compatibility.
- **Values:** Free text; ignored at load time.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/FileName`

- **Type:** `String` (`CHAR8`)
- **Description:** Image file path, relative to the game data directory. Only read for entries with `uiIndex > 0`; ignored on the menu-button layout entry `0`.
- **Values:** Relative path; longer values are truncated to `MAX_MAIN_MENU_CHARS`.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/Visible`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the image is shown on the main menu. Only effective for `uiIndex > 0`; on entry `0` the loader forces it to `0`.
- **Values:** `0` or `1`; forced to `0` on entry `0`.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/FitToScreen`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the image is scaled to fit the screen. Only effective for `uiIndex > 0`; on entry `0` the loader forces it to `0`.
- **Values:** `0` or `1`; forced to `0` on entry `0`.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/StretchImage`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the image is stretched rather than proportionally scaled. Only effective for `uiIndex > 0`; on entry `0` the loader forces it to `0`.
- **Values:** `0` or `1`; forced to `0` on entry `0`.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/CenterImage`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, the image is centered in its area. Only effective for `uiIndex > 0`; on entry `0` the loader forces it to `0`.
- **Values:** `0` or `1`; forced to `0` on entry `0`.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/ImagePositionRelative`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, `ImagePositionX` and `ImagePositionY` are relative to the image area rather than absolute screen coordinates. Only effective for `uiIndex > 0`; on entry `0` the loader forces it to `0`.
- **Values:** `0` or `1`; forced to `0` on entry `0`.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/ImagePositionX`

- **Type:** `Integer` (`INT32`)
- **Description:** Horizontal image position in pixels. Only read for entries with `uiIndex > 0`; ignored on entry `0`.
- **Values:** Signed pixels; increasing = right, negative = left.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/ImagePositionY`

- **Type:** `Integer` (`INT32`)
- **Description:** Vertical image position in pixels. Only read for entries with `uiIndex > 0`; ignored on entry `0`.
- **Values:** Signed pixels; increasing = down, negative = up.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/MainMenuX`

- **Type:** `Integer` (`UINT32`)
- **Description:** Horizontal position of the menu buttons in pixels. Only read on the layout entry `uiIndex = 0`.
- **Values:** `0..4294967295`; negative values are not meaningful.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/MainMenuY`

- **Type:** `Integer` (`UINT32`)
- **Description:** Vertical position of the first menu button in pixels. Only read on the layout entry `uiIndex = 0`.
- **Values:** `0..4294967295`; negative values are not meaningful.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/MainMenuYSpace`

- **Type:** `Integer` (`UINT32`)
- **Description:** Vertical spacing between menu buttons in pixels. Only read on the layout entry `uiIndex = 0`.
- **Values:** `0..4294967295`; higher = more spacing.
- **Default:** Absent; zero or empty unless the table provides a value.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`
