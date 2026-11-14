---
schema: 1
lang: ru
family: map-extra-items
title: "ExtraItems — дополнительные предметы сектора по сложности"
tables:
  - A2_0_ExtraItems_Experienced
  - A2_0_ExtraItems_Expert
  - A2_0_ExtraItems_Insane
  - A2_0_ExtraItems_Novice
  - A9_0_ExtraItems_Experienced
  - A9_0_ExtraItems_Expert
  - A9_0_ExtraItems_Insane
  - A9_0_ExtraItems_Novice
  - C5_0_ExtraItems_Experienced
  - C5_0_ExtraItems_Expert
  - C5_0_ExtraItems_Insane
  - C5_0_ExtraItems_Novice
  - H14_0_ExtraItems_Experienced
  - H14_0_ExtraItems_Expert
  - H14_0_ExtraItems_Insane
  - H14_0_ExtraItems_Novice
  - I13_0_ExtraItems_Experienced
  - I13_0_ExtraItems_Expert
  - I13_0_ExtraItems_Insane
  - I13_0_ExtraItems_Novice
table_ids:
  - Map.ExtraItems.A2_0_ExtraItems_Experienced
  - Map.ExtraItems.A2_0_ExtraItems_Expert
  - Map.ExtraItems.A2_0_ExtraItems_Insane
  - Map.ExtraItems.A2_0_ExtraItems_Novice
  - Map.ExtraItems.A9_0_ExtraItems_Experienced
  - Map.ExtraItems.A9_0_ExtraItems_Expert
  - Map.ExtraItems.A9_0_ExtraItems_Insane
  - Map.ExtraItems.A9_0_ExtraItems_Novice
  - Map.ExtraItems.C5_0_ExtraItems_Experienced
  - Map.ExtraItems.C5_0_ExtraItems_Expert
  - Map.ExtraItems.C5_0_ExtraItems_Insane
  - Map.ExtraItems.C5_0_ExtraItems_Novice
  - Map.ExtraItems.H14_0_ExtraItems_Experienced
  - Map.ExtraItems.H14_0_ExtraItems_Expert
  - Map.ExtraItems.H14_0_ExtraItems_Insane
  - Map.ExtraItems.H14_0_ExtraItems_Novice
  - Map.ExtraItems.I13_0_ExtraItems_Experienced
  - Map.ExtraItems.I13_0_ExtraItems_Expert
  - Map.ExtraItems.I13_0_ExtraItems_Insane
  - Map.ExtraItems.I13_0_ExtraItems_Novice
files:
  - Map/ExtraItems/A2_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/A2_0_ExtraItems_Expert.xml
  - Map/ExtraItems/A2_0_ExtraItems_Insane.xml
  - Map/ExtraItems/A2_0_ExtraItems_Novice.xml
  - Map/ExtraItems/A9_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/A9_0_ExtraItems_Expert.xml
  - Map/ExtraItems/A9_0_ExtraItems_Insane.xml
  - Map/ExtraItems/A9_0_ExtraItems_Novice.xml
  - Map/ExtraItems/C5_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/C5_0_ExtraItems_Expert.xml
  - Map/ExtraItems/C5_0_ExtraItems_Insane.xml
  - Map/ExtraItems/C5_0_ExtraItems_Novice.xml
  - Map/ExtraItems/H14_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/H14_0_ExtraItems_Expert.xml
  - Map/ExtraItems/H14_0_ExtraItems_Insane.xml
  - Map/ExtraItems/H14_0_ExtraItems_Novice.xml
  - Map/ExtraItems/I13_0_ExtraItems_Experienced.xml
  - Map/ExtraItems/I13_0_ExtraItems_Expert.xml
  - Map/ExtraItems/I13_0_ExtraItems_Insane.xml
  - Map/ExtraItems/I13_0_ExtraItems_Novice.xml
root: ExtraItems
record: Item
key: uiIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# ExtraItems — дополнительные предметы сектора по сложности

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ExtraItems/Item/uiIndex`](#extraitemsitemuiindex)
  - [`ExtraItems/Item/quantity`](#extraitemsitemquantity)
  - [`ExtraItems/Item/condition`](#extraitemsitemcondition)
  - [`ExtraItems/Item/gridno`](#extraitemsitemgridno)
  - [`ExtraItems/Item/visible`](#extraitemsitemvisible)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

Файлы `ExtraItems` добавляют предметы в конкретные секторы. Имя каждого файла задаётся сектором и
сложностью: `A2_0` — это сектор A2, уровень 0, а `Novice`, `Experienced`, `Expert` или `Insane`
выбирает сложность. Все файлы используют одни и те же пять полей предмета.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `A2_0_ExtraItems_Experienced` | `Map/ExtraItems/A2_0_ExtraItems_Experienced.xml` | — |
| `A2_0_ExtraItems_Expert` | `Map/ExtraItems/A2_0_ExtraItems_Expert.xml` | — |
| `A2_0_ExtraItems_Insane` | `Map/ExtraItems/A2_0_ExtraItems_Insane.xml` | — |
| `A2_0_ExtraItems_Novice` | `Map/ExtraItems/A2_0_ExtraItems_Novice.xml` | — |
| `A9_0_ExtraItems_Experienced` | `Map/ExtraItems/A9_0_ExtraItems_Experienced.xml` | — |
| `A9_0_ExtraItems_Expert` | `Map/ExtraItems/A9_0_ExtraItems_Expert.xml` | — |
| `A9_0_ExtraItems_Insane` | `Map/ExtraItems/A9_0_ExtraItems_Insane.xml` | — |
| `A9_0_ExtraItems_Novice` | `Map/ExtraItems/A9_0_ExtraItems_Novice.xml` | — |
| `C5_0_ExtraItems_Experienced` | `Map/ExtraItems/C5_0_ExtraItems_Experienced.xml` | — |
| `C5_0_ExtraItems_Expert` | `Map/ExtraItems/C5_0_ExtraItems_Expert.xml` | — |
| `C5_0_ExtraItems_Insane` | `Map/ExtraItems/C5_0_ExtraItems_Insane.xml` | — |
| `C5_0_ExtraItems_Novice` | `Map/ExtraItems/C5_0_ExtraItems_Novice.xml` | — |
| `H14_0_ExtraItems_Experienced` | `Map/ExtraItems/H14_0_ExtraItems_Experienced.xml` | — |
| `H14_0_ExtraItems_Expert` | `Map/ExtraItems/H14_0_ExtraItems_Expert.xml` | — |
| `H14_0_ExtraItems_Insane` | `Map/ExtraItems/H14_0_ExtraItems_Insane.xml` | — |
| `H14_0_ExtraItems_Novice` | `Map/ExtraItems/H14_0_ExtraItems_Novice.xml` | — |
| `I13_0_ExtraItems_Experienced` | `Map/ExtraItems/I13_0_ExtraItems_Experienced.xml` | — |
| `I13_0_ExtraItems_Expert` | `Map/ExtraItems/I13_0_ExtraItems_Expert.xml` | — |
| `I13_0_ExtraItems_Insane` | `Map/ExtraItems/I13_0_ExtraItems_Insane.xml` | — |
| `I13_0_ExtraItems_Novice` | `Map/ExtraItems/I13_0_ExtraItems_Novice.xml` | — |

## Structure

```
ExtraItems
└── Item
    ├── uiIndex
    ├── quantity
    ├── condition
    ├── gridno
    └── visible
```

## Fields

### `ExtraItems/Item/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** ID предмета, размещаемого в секторе. Это ссылка на предмет.
- **Values:** ID предмета `> 0`; значения `0` пропускаются загрузчиком.
- **Default:** Обязательно; идентифицирует запись или строку.
- **Source:** game code (extra-items loader); map table data.
- **Confidence:** `source`

### `ExtraItems/Item/quantity`

- **Type:** `Integer` (`UINT32`)
- **Description:** Количество размещаемых предметов.
- **Numeric semantics:** больше = больше предметов; отрицательные значения не имеют смысла.
- **Values:** `1..4294967295`; нулевое количество пропускается.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (extra-items loader); map table data.
- **Confidence:** `source`

### `ExtraItems/Item/condition`

- **Type:** `Integer` (`UINT32`)
- **Description:** Состояние (прочность) размещаемого предмета в процентах.
- **Numeric semantics:** Проценты; загрузчик размещает предмет только при состоянии `1..100`, иначе запись пропускается.
- **Values:** `1..100`; при других значениях запись пропускается.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (extra-items loader); map table data.
- **Confidence:** `source`

### `ExtraItems/Item/gridno`

- **Type:** `Integer` (`UINT32`)
- **Description:** Номер клетки карты, куда размещается предмет. Это идентификатор клетки.
- **Values:** `0..4294967295`; номер клетки карты.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (extra-items loader); map table data.
- **Confidence:** `source`

### `ExtraItems/Item/visible`

- **Type:** `Boolean` (`UINT32`)
- **Description:** Если включено, предмет виден на карте с самого начала. Это переключатель.
- **Values:** `0` или `1`; передаётся как флаг видимости пула предметов.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (extra-items loader); map table data.
- **Confidence:** `source`

## Relations

- `Item/uiIndex` ссылается на предмет в [Items](../Items/Items.md); сектор и сложность закодированы в имени файла, а не в поле.

## Localization

Отображаемые имена, подсказки и описания локализуются через языковые оверлеи; числовые ID, коды
секторов и значения матрицы — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблиц карты.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания основаны на именах полей и наблюдаемых значениях таблиц; формулы времени выполнения не
  утверждаются.

## Open questions

- Точная формула или сопоставление перечисления для полей, помеченных как выведенные, в данных
  таблиц не указаны.
