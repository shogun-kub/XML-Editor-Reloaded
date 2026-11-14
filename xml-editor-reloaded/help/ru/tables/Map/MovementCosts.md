---
schema: 1
lang: ru
family: map-movement-costs
title: "MovementCosts — стратегический рельеф и стоимость перемещения"
tables:
  - MovementCosts
table_ids:
  - Map.MovementCosts
files:
  - Map/MovementCosts.xml
root: StrategicMovementCosts
record: SectorTraversal/Sector
key: none
sources:
  - Map table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# MovementCosts — стратегический рельеф и стоимость перемещения

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`StrategicMovementCosts/SectorTraversal/Sector/TravelRating`](#strategicmovementcostssectortraversalsectortravelrating)
  - [`StrategicMovementCosts/SectorTraversal/Sector/North`](#strategicmovementcostssectortraversalsectornorth)
  - [`StrategicMovementCosts/SectorTraversal/Sector/East`](#strategicmovementcostssectortraversalsectoreast)
  - [`StrategicMovementCosts/SectorTraversal/Sector/South`](#strategicmovementcostssectortraversalsectorsouth)
  - [`StrategicMovementCosts/SectorTraversal/Sector/West`](#strategicmovementcostssectortraversalsectorwest)
  - [`StrategicMovementCosts/SectorTraversal/Sector/Here`](#strategicmovementcostssectortraversalsectorhere)
  - [`StrategicMovementCosts/SectorTraversal/Sector/BadSector`](#strategicmovementcostssectortraversalsectorbadsector)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MovementCosts.xml` задаёт каждому сектору его стратегический `TravelRating` и категорию рельефа на
каждом краю и в самом секторе. Запись идентифицируется атрибутами `x` и `y` элемента `Sector`, а не
дочерним ключом.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `MovementCosts` | `Map/MovementCosts.xml` | — |

## Structure

```
StrategicMovementCosts
└── SectorTraversal
    └── Sector
        ├── TravelRating
        ├── North
        ├── East
        ├── South
        ├── West
        ├── Here
        └── BadSector
```

## Fields

### `StrategicMovementCosts/SectorTraversal/Sector/TravelRating`

- **Type:** `Integer` (`UINT8`)
- **Description:** Стратегическая стоимость перемещения при входе в этот сектор.
- **Numeric semantics:** больше = медленнее; разборщик принимает отрицательный ввод, но сохраняет его как `UINT8`, а значения выше 255 обрезаются.
- **Values:** Целое число; о его шкале см. описание.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/North`

- **Type:** `String`
- **Description:** Категория рельефа на северном краю сектора, например `PLAINS`, `ROAD`, `TOWN` или `EDGEOFWORLD`. Это код категории.
- **Values:** Одно из значений: `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; неизвестный текст вызывает утверждение и оставляет разобранный тип неопределённым.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/East`

- **Type:** `String`
- **Description:** Категория рельефа на восточном краю сектора. Это код категории.
- **Values:** Одно из значений: `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; неизвестный текст вызывает утверждение и оставляет разобранный тип неопределённым.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/South`

- **Type:** `String`
- **Description:** Категория рельефа на южном краю сектора. Это код категории.
- **Values:** Одно из значений: `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; неизвестный текст вызывает утверждение и оставляет разобранный тип неопределённым.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/West`

- **Type:** `String`
- **Description:** Категория рельефа на западном краю сектора. Это код категории.
- **Values:** Одно из значений: `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; неизвестный текст вызывает утверждение и оставляет разобранный тип неопределённым.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/Here`

- **Type:** `String`
- **Description:** Категория рельефа самого сектора. Это код категории.
- **Values:** Одно из значений: `TOWN`, `ROAD`, `PLAINS`, `SAND`, `SPARSE`, `DENSE`, `SWAMP`, `WATER`, `HILLS`, `GROUNDBARRIER`, `NS_RIVER`, `EW_RIVER`, `EDGEOFWORLD`, `TROPICS`, `FARMLAND`, `PLAINS_ROAD`, `SPARSE_ROAD`, `FARMLAND_ROAD`, `TROPICS_ROAD`, `DENSE_ROAD`, `COASTAL`, `HILLS_ROAD`, `COASTAL_ROAD`, `SAND_ROAD`, `SWAMP_ROAD`; неизвестный текст вызывает утверждение и оставляет разобранный тип неопределённым.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

### `StrategicMovementCosts/SectorTraversal/Sector/BadSector`

- **Type:** `Boolean`
- **Description:** Если включено, сектор помечается как плохой или недостижимый для стратегического перемещения. Это переключатель, а не стоимость.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (strategic movement-cost loader); map table data.
- **Confidence:** `source`

## Relations

- Запись `Sector` имеет ключ из атрибутов `x` и `y`, которые задают те же координаты сектора, что используются в [SectorNames](SectorNames.md).

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
