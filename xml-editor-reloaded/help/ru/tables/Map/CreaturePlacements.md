---
schema: 1
lang: ru
family: map-creature-placements
title: "CreaturePlacements — составы и размещение существ"
tables:
  - CreaturePlacements
table_ids:
  - Map.CreaturePlacements
files:
  - Map/CreaturePlacements.xml
root: CREATURE_INFO
record: PLACEMENT_LIST/PLACEMENT
key: Index
sources:
  - Game code (creature-placements loader and creature spreading)
  - Map table data and XML comments
status: reviewed
updated: 2026-11-12
---

# CreaturePlacements — составы и размещение существ

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/Index`](#creature_infoplacement_listplacementindex)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/SectorGrid`](#creature_infoplacement_listplacementattacksourcesectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/GridNo`](#creature_infoplacement_listplacementattacksourcegridno)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorGrid`](#creature_infoplacement_listplacementalternatemapsectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorZ`](#creature_infoplacement_listplacementalternatemapsectorz)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorGrid`](#creature_infoplacement_listplacementwarptosectorsectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorZ`](#creature_infoplacement_listplacementwarptosectorsectorz)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/GridNo`](#creature_infoplacement_listplacementwarptosectorgridno)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorGrid`](#creature_infoplacement_listplacementcreaturesectorqueensectorsectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorZ`](#creature_infoplacement_listplacementcreaturesectorqueensectorsectorz)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorGrid`](#creature_infoplacement_listplacementcreaturesectorhabitatsectorsectorgrid)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorZ`](#creature_infoplacement_listplacementcreaturesectorhabitatsectorsectorz)
  - [`CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/Composition`](#creature_infoplacement_listplacementcreaturesectorhabitatsectorcomposition)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/CompIndex`](#creature_infocomposition_listcompositioncompindex)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultFemalePercent`](#creature_infocomposition_listcompositionadultfemalepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultMalePercent`](#creature_infocomposition_listcompositionadultmalepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungFemalePercent`](#creature_infocomposition_listcompositionyoungfemalepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungMalePercent`](#creature_infocomposition_listcompositionyoungmalepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/InfantPercent`](#creature_infocomposition_listcompositioninfantpercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/LarvaePercent`](#creature_infocomposition_listcompositionlarvaepercent)
  - [`CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/MaxPopulation`](#creature_infocomposition_listcompositionmaxpopulation)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`CreaturePlacements.xml` состоит из двух частей. `COMPOSITION_LIST` определяет составы колоний и пределы их популяции; `PLACEMENT_LIST` определяет источник атаки, альтернативную карту, точку перемещения, сектор королевы и секторы обитания для каждого заражения существами. Секторы обитания ссылаются на состав через `CompIndex`.

В каждом составе шесть полей `*Percent` — последовательные полосы броска `Random(100)`, обрабатываемые в порядке: личинки, младенец, молодой самец, молодая самка, взрослый самец, взрослая самка; в сумме они должны давать ровно `100`. Значения `Index` размещения нумеруются с 1, значения `CompIndex` состава — с 0, а сектор обитания с `Composition` `0` игнорируется.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `CreaturePlacements` | `Map/CreaturePlacements.xml` | — |

## Structure

```
CREATURE_INFO
├── PLACEMENT_LIST
│   └── PLACEMENT
│       ├── Index
│       ├── ATTACKSOURCE
│       │   ├── SectorGrid
│       │   └── GridNo
│       ├── ALTERNATEMAP
│       │   ├── SectorGrid
│       │   └── SectorZ
│       ├── WARPTOSECTOR
│       │   ├── SectorGrid
│       │   ├── SectorZ
│       │   └── GridNo
│       └── CREATURESECTOR
│           ├── QUEENSECTOR
│           │   ├── SectorGrid
│           │   └── SectorZ
│           └── HABITATSECTOR
│               ├── SectorGrid
│               ├── SectorZ
│               └── Composition
└── COMPOSITION_LIST
    └── COMPOSITION
        ├── CompIndex
        ├── AdultFemalePercent
        ├── AdultMalePercent
        ├── YoungFemalePercent
        ├── YoungMalePercent
        ├── InfantPercent
        ├── LarvaePercent
        └── MaxPopulation
```

## Fields

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/Index`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс записи размещения существ. Это ключ массива с нумерацией от 1; наибольший допустимый индекс также задаёт число заражаемых участков.
- **Values:** `1..10` (`MAX_NUMBER_OF_INFECTIBLE_SITES`); `0` недопустим, и запись отбрасывается. Верхняя проверка загрузчика — `> 11`, поэтому `11` принимается, но выходит за пределы массива из 10 участков.
- **Default:** Обязателен; идентифицирует запись.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/SectorGrid`

- **Type:** `String`
- **Description:** Сектор, где существа выходят на поверхность для атаки на город, в форме `A1`..`P16`: буква — строка карты (`y`), число — столбец (`x`). Комментарий таблицы также требует, чтобы связанный подземный сектор был сектором обитания или сектором королевы.
- **Values:** `A1`..`P16`; обе координаты должны быть `1..16`, иначе сохранённый сектор не обновляется.
- **Default:** Фактически обязателен; недопустимые значения игнорируются.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ATTACKSOURCE/GridNo`

- **Type:** `Integer` (`INT32`)
- **Description:** Номер сетки внутри сектора атаки, используемый для вставки существ, когда атака начинается в самом секторе-источнике атаки.
- **Values:** Номер тактической сетки; `0` = не задан.
- **Default:** Отсутствует; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorGrid`

- **Type:** `String`
- **Description:** Подземный сектор, альтернативная карта которого загружается и помечается (`SF_PENDING_ALTERNATE_MAP`) при запуске квеста существ.
- **Values:** `A1`..`P16`; обе координаты должны быть `1..16`, иначе сохранённый сектор не обновляется.
- **Default:** Фактически обязателен; недопустимые значения игнорируются.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/ALTERNATEMAP/SectorZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Подземный уровень альтернативной карты: `0` — поверхность, `1`..`3` — подземные уровни.
- **Values:** `0..3` (`MAXIMUM_VALID_Z_COORDINATE`).
- **Default:** Отсутствует; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Сектор назначения, используемый когда игрок покидает сектор королевы или сектор обитания на `z >= 2` после завершения квеста существ; `InitSectorExitMenu` проверяет оба условия.
- **Values:** `A1`..`P16`; обе координаты должны быть `1..16`, иначе сохранённый сектор не обновляется.
- **Default:** Фактически обязателен; недопустимые значения игнорируются.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/SectorZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Подземный уровень пункта перемещения: `0` — поверхность, `1`..`3` — подземные уровни.
- **Values:** `0..3` (`MAXIMUM_VALID_Z_COORDINATE`).
- **Default:** Отсутствует; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/WARPTOSECTOR/GridNo`

- **Type:** `Integer` (`INT32`)
- **Description:** Номер сетки для вставки в пункте перемещения.
- **Values:** Номер тактической сетки; `0` = не задан.
- **Default:** Отсутствует; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Сектор, содержащий логово королевы — корень списка директив существ.
- **Values:** `A1`..`P16`; обе координаты должны быть `1..16`, иначе сохранённый сектор не обновляется.
- **Default:** Фактически обязателен; недопустимые значения игнорируются.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/QUEENSECTOR/SectorZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Подземный уровень логова королевы: `0` — поверхность, `1`..`3` — подземные уровни.
- **Values:** `0..3` (`MAXIMUM_VALID_Z_COORDINATE`).
- **Default:** Отсутствует; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Один связанный сектор обитания, куда могут распространяться существа. Сектор обитания используется, только если его `Composition` не равен нулю; на одно размещение читается до 255 секторов обитания.
- **Values:** `A1`..`P16`; обе координаты должны быть `1..16`, иначе сохранённый сектор не обновляется.
- **Default:** Фактически обязателен; недопустимые значения игнорируются.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/SectorZ`

- **Type:** `Integer` (`UINT8`)
- **Description:** Подземный уровень сектора обитания: `0` — поверхность, `1`..`3` — подземные уровни.
- **Values:** `0..3` (`MAXIMUM_VALID_Z_COORDINATE`).
- **Default:** Отсутствует; `0`.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/PLACEMENT_LIST/PLACEMENT/CREATURESECTOR/HABITATSECTOR/Composition`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс состава, используемый этим сектором обитания. `0` зарезервирован для логова королевы и оставляет сектор обитания недействительным; ненулевое значение помечает сектор обитания действительным и ссылается на `CompIndex`.
- **Values:** `1..49`; `0` = игнорируемый сектор обитания.
- **Default:** Отсутствует или `0`; сектор обитания игнорируется.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/CompIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс состава существ с нумерацией от 0; `0` — состав логова королевы, и `HABITATSECTOR/Composition` ссылается на это значение. Наибольший индекс плюс один задаёт число составов.
- **Values:** `0..49` (`MAX_NUMBER_OF_CREATURE_COMPOSITIONS`); проверка загрузчика также принимает `50` — на единицу за концом массива.
- **Default:** Обязателен; `0` = логово королевы.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultFemalePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Доля взрослых самок в этом составе, в процентах. Шесть долей образуют последовательные полосы броска `Random(100)` (см. Purpose).
- **Values:** `0..100`; сумма шести полей должна равняться `100`.
- **Default:** Отсутствует; `0`, если предыдущий состав не задал тот же тег.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/AdultMalePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Доля взрослых самцов в этом составе, в процентах. Шесть долей образуют последовательные полосы броска `Random(100)` (см. Purpose).
- **Values:** `0..100`; сумма шести полей должна равняться `100`.
- **Default:** Отсутствует; `0`, если предыдущий состав не задал тот же тег.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungFemalePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Доля молодых самок в этом составе, в процентах. Шесть долей образуют последовательные полосы броска `Random(100)` (см. Purpose).
- **Values:** `0..100`; сумма шести полей должна равняться `100`.
- **Default:** Отсутствует; `0`, если предыдущий состав не задал тот же тег.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/YoungMalePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Доля молодых самцов в этом составе, в процентах. Шесть долей образуют последовательные полосы броска `Random(100)` (см. Purpose).
- **Values:** `0..100`; сумма шести полей должна равняться `100`.
- **Default:** Отсутствует; `0`, если предыдущий состав не задал тот же тег.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/InfantPercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Доля младенцев в этом составе, в процентах. Шесть долей образуют последовательные полосы броска `Random(100)` (см. Purpose).
- **Values:** `0..100`; сумма шести полей должна равняться `100`.
- **Default:** Отсутствует; `0`, если предыдущий состав не задал тот же тег.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/LarvaePercent`

- **Type:** `Integer` (`UINT8`)
- **Description:** Доля личинок в этом составе, в процентах. Шесть долей образуют последовательные полосы броска `Random(100)` (см. Purpose).
- **Values:** `0..100`; сумма шести полей должна равняться `100`.
- **Default:** Отсутствует; `0`, если предыдущий состав не задал тот же тег.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

### `CREATURE_INFO/COMPOSITION_LIST/COMPOSITION/MaxPopulation`

- **Type:** `Integer` (`INT32`)
- **Description:** Базовый предел популяции для этого состава до масштабирования по сложности и расстоянию. Код распространения применяет 50% на лёгкой сложности, 80% на средней, 100% на сложной и 200% на безумной, масштабирует по расстоянию от логова (минимум 25%) и округляет результат вниз до `4`.
- **Numeric semantics:** больше = крупнее колония; вычисленный предел округляется вниз до `4`.
- **Values:** Неотрицательная популяция; поставляемые составы используют `10..32`.
- **Default:** Отсутствует; `0`, если предыдущий состав не задал его.
- **Source:** game code (creature-placements loader and spreading); table data and XML comments.
- **Confidence:** `source`

## Relations

- `HABITATSECTOR/Composition` ссылается на `COMPOSITION/CompIndex`, только когда оно не равно нулю; `Composition` `0` оставляет сектор обитания недействительным.
- Все поля секторов используют обычные координаты секторов карты (`A1`..`P16`) и названия секторов из [SectorNames](SectorNames.md).

## Localization

Отображаемые названия, всплывающие подсказки и описания локализуются через языковые оверлеи; числовые ID, коды секторов и значения матриц — нет.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Игровой код: загрузчик размещения существ, структуры распространения существ и их использование.
- Данные таблиц карты и XML-комментарии.
- Типы полей следуют структурам времени выполнения и приведениям загрузчика; описания следуют коду, который использует каждое значение.

## Open questions

- Сам `GetWarpOutOfMineCodes` отвергает только `z == 0`; проверки `z >= 2` и завершённости квеста находятся в его вызывающих сторонах, поэтому будущий вызывающий код мог бы перемещать на уровне 1.
- Проверка `Index` в загрузчике принимает `11`, а проверка `CompIndex` — `50`, на единицу за концом соответствующих массивов; неизвестно, сделано ли это намеренно.
- Отсутствующие скалярные поля размещения и проценты состава наследуют значение предыдущей записи, потому что промежуточные структуры разбора не очищаются для каждой записи; неизвестно, полагается ли игра на это.
- То, ограничен ли `MaxPopulation` также значением `MAX_NUMBER_CREATURES_IN_TACTICAL` во всех путях распространения, здесь прослежено не полностью.
