---
schema: 1
lang: ru
family: layout-main-menu
title: "LayoutMainMenu — изображения и кнопки главного меню"
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
updated: 2026-11-12
---

# LayoutMainMenu — изображения и кнопки главного меню

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

`LayoutMainMenu.xml` задаёт положение кнопок главного меню и необязательных изображений фона и
логотипа. Для раскладки кнопок меню обязательна запись `uiIndex = 0`; остальные записи —
изображения.

## Tables

| Таблица | Файл | Примечания |
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
- **Description:** Индекс изображения или записи меню. `0` — особая запись, задающая раскладку кнопок меню.
- **Values:** `0..4294967295`; загрузчик сохраняет это поле как `UINT32`.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/Description`

- **Type:** `String`
- **Description:** Описание записи изображения для редактора и интерфейса. Текущий загрузчик его не читает; хранится как данные для совместимости.
- **Values:** Свободный текст; при загрузке игнорируется.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/FileName`

- **Type:** `String` (`CHAR8`)
- **Description:** Путь к файлу изображения относительно каталога данных игры. Читается только для записей с `uiIndex > 0`; в записи раскладки кнопок `0` игнорируется.
- **Values:** Относительный путь; более длинные значения обрезаются до `MAX_MAIN_MENU_CHARS`.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/Visible`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, изображение показывается в главном меню. Действует только при `uiIndex > 0`; в записи `0` загрузчик принудительно ставит `0`.
- **Values:** `0` или `1`; в записи `0` принудительно ставится `0`.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/FitToScreen`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, изображение масштабируется по размеру экрана. Действует только при `uiIndex > 0`; в записи `0` загрузчик принудительно ставит `0`.
- **Values:** `0` или `1`; в записи `0` принудительно ставится `0`.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/StretchImage`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, изображение растягивается, а не масштабируется пропорционально. Действует только при `uiIndex > 0`; в записи `0` загрузчик принудительно ставит `0`.
- **Values:** `0` или `1`; в записи `0` принудительно ставится `0`.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/CenterImage`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, изображение центрируется в своей области. Действует только при `uiIndex > 0`; в записи `0` загрузчик принудительно ставит `0`.
- **Values:** `0` или `1`; в записи `0` принудительно ставится `0`.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/ImagePositionRelative`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если включено, `ImagePositionX` и `ImagePositionY` задаются относительно области изображения, а не в абсолютных координатах экрана. Действует только при `uiIndex > 0`; в записи `0` загрузчик принудительно ставит `0`.
- **Values:** `0` или `1`; в записи `0` принудительно ставится `0`.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/ImagePositionX`

- **Type:** `Integer` (`INT32`)
- **Description:** Горизонтальное положение изображения в пикселях. Читается только для записей с `uiIndex > 0`; в записи `0` игнорируется.
- **Values:** Пиксели со знаком; рост = вправо, отрицательные = влево.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/ImagePositionY`

- **Type:** `Integer` (`INT32`)
- **Description:** Вертикальное положение изображения в пикселях. Читается только для записей с `uiIndex > 0`; в записи `0` игнорируется.
- **Values:** Пиксели со знаком; рост = вниз, отрицательные = вверх.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/MainMenuX`

- **Type:** `Integer` (`UINT32`)
- **Description:** Горизонтальное положение кнопок меню в пикселях. Читается только в записи раскладки `uiIndex = 0`.
- **Values:** `0..4294967295`; отрицательные значения не имеют смысла.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/MainMenuY`

- **Type:** `Integer` (`UINT32`)
- **Description:** Вертикальное положение первой кнопки меню в пикселях. Читается только в записи раскладки `uiIndex = 0`.
- **Values:** `0..4294967295`; отрицательные значения не имеют смысла.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`

### `MAINMENU/IMAGE/MainMenuYSpace`

- **Type:** `Integer` (`UINT32`)
- **Description:** Вертикальный интервал между кнопками меню в пикселях. Читается только в записи раскладки `uiIndex = 0`.
- **Values:** `0..4294967295`; больше = больше интервал.
- **Default:** Отсутствует; ноль или пусто, если таблица не задаёт значение.
- **Source:** game code (main-menu layout loader); table data.
- **Confidence:** `source`
