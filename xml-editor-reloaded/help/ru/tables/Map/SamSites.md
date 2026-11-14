---
schema: 1
lang: ru
family: map-sam-sites
title: "SamSites — позиции ПВО и прикрываемые секторы"
tables:
  - SamSites
table_ids:
  - Map.SamSites
files:
  - Map/SamSites.xml
root: SAM_INFO
record: SAMLIST/SAM
key: samIndex
sources:
  - Game code (SAM-site loader and SAM coverage code)
  - Map table data and XML comments
status: reviewed
updated: 2026-11-12
---

# SamSites — позиции ПВО и прикрываемые секторы

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SAM_INFO/SAMLIST/SAM/samIndex`](#sam_infosamlistsamsamindex)
  - [`SAM_INFO/SAMLIST/SAM/samSector/x`](#sam_infosamlistsamsamsectorx)
  - [`SAM_INFO/SAMLIST/SAM/samSector/y`](#sam_infosamlistsamsamsectory)
  - [`SAM_INFO/SAMLIST/SAM/samHidden`](#sam_infosamlistsamsamhidden)
  - [`SAM_INFO/SAMLIST/SAM/samOrientation`](#sam_infosamlistsamsamorientation)
  - [`SAM_INFO/SAMLIST/SAM/samGridNoA`](#sam_infosamlistsamsamgridnoa)
  - [`SAM_INFO/SAMLIST/SAM/samGridNoB`](#sam_infosamlistsamsamgridnob)
  - [`SAM_INFO/SAM_CONTROLLED_SECTORS/MAP_ROW`](#sam_infosam_controlled_sectorsmap_row)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SamSites.xml` определяет позиции ПВО и матрицу прикрываемых ими секторов. У каждой позиции есть
сектор, флаг скрытости, ориентация терминала и две позиции терминала на сетке. Матрица покрытия
содержит по одной строке на строку карты; каждая ячейка — битовая маска позиций, прикрывающих этот
сектор, а не один индекс позиции.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `SamSites` | `Map/SamSites.xml` | — |

## Structure

```
SAM_INFO
├── SAMLIST
│   └── SAM
│       ├── samIndex
│       ├── samSector
│       │   ├── x
│       │   └── y
│       ├── samHidden
│       ├── samOrientation
│       ├── samGridNoA
│       └── samGridNoB
└── SAM_CONTROLLED_SECTORS
    └── MAP_ROW
```

## Fields

### `SAM_INFO/SAMLIST/SAM/samIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Уникальный индекс позиции ПВО с нумерацией от 1, используемый как ключ массива (внутри хранится как `samIndex - 1`). `NUMBER_OF_SAMS` устанавливается в наибольший прочитанный индекс; индексы `1..4` к тому же имеют жёстко заданные роли в линиях Meanwhile/Skyrider.
- **Values:** `1..50` (`MAX_NUMBER_OF_SAMS`); `0` зарезервирован для «нет покрытия ПВО» и отвергается как индекс позиции.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samSector/x`

- **Type:** `Integer` (`INT16`)
- **Description:** Столбец карты (`x`) сектора ПВО: `1` — западный край, `16` — восточный край. Используется вместе с `y` для построения ID сектора.
- **Values:** `1..16`.
- **Default:** Отсутствует; `0` (недопустимый сектор).
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samSector/y`

- **Type:** `Integer` (`INT16`)
- **Description:** Строка карты (`y`) сектора ПВО: `1` — северный край (`A`), `16` — южный край (`P`).
- **Values:** `1..16`.
- **Default:** Отсутствует; `0` (недопустимый сектор).
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samHidden`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если значение ненулевое, позиция ПВО скрыта в начале новой игры (`fSamSiteFoundOrig` устанавливается в `!samHidden`). Изменение этого поля в существующем сохранении не действует; комментарий к таблице требует новой игры.
- **Values:** `0` или `1`; любое ненулевое значение скрывает позицию.
- **Default:** Отсутствует; `0` (видима).
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samOrientation`

- **Type:** `Integer` (`INT8`)
- **Description:** Ориентация терминала управления ПВО, если он отремонтирован: `3` для `/` (от середины к нижнему левому краю) и `4` для `\` (от середины к нижнему правому краю). Любое другое значение принудительно заменяется на `3`.
- **Values:** `3` или `4`; другие значения читаются как `3`.
- **Default:** Отсутствует; `3`.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samGridNoA`

- **Type:** `Integer` (`INT32`)
- **Description:** Номер клетки нижнего тайла терминала на тактической карте (тайл A на схеме A/B в комментарии к таблице).
- **Values:** Номер тактической клетки меньше `MAX_ALLOWED_WORLD_MAX`; значения на этом пороге и выше заменяются на `0`.
- **Default:** Отсутствует; `0`.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAMLIST/SAM/samGridNoB`

- **Type:** `Integer` (`INT32`)
- **Description:** Номер клетки верхнего тайла терминала (тайл B); `samGridNoA` всегда ниже на экране, чем `samGridNoB`.
- **Values:** Номер тактической клетки меньше `MAX_ALLOWED_WORLD_MAX`; значения на этом пороге и выше заменяются на `1`.
- **Default:** Отсутствует; `1`.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

### `SAM_INFO/SAM_CONTROLLED_SECTORS/MAP_ROW`

- **Type:** `String` (`CHAR8`)
- **Description:** Одна строка матрицы покрытия ПВО. Атрибут `row` выбирает строку карты (`00`..`17`); текст элемента — список из 18 ячеек столбцов, разделённых пробельными символами. Каждая ячейка — битовая маска: бит 0 = позиция 1, бит 1 = позиция 2, бит 2 = позиция 3 и так далее, поэтому `5` означает позиции 1 и 3; `0` = нет покрытия.
- **Values:** 18 ячеек, по одной на столбец `0..17`; строки и столбцы `0` и `17` — границы мира, и проверки покрытия их не читают.
- **Default:** Обязательно; строки с отсутствующим атрибутом `row` или с `row >= 18` пропускаются. Нечисловое значение разбирается как `0`.
- **Source:** game code (SAM-site loader and coverage code); table data and XML comments.
- **Confidence:** `source`

## Relations

- `samSector/x` и `samSector/y` задают сектор, названный в [SectorNames](SectorNames.md); матрица покрытия индексируется строкой и столбцом карты, а каждая ячейка — битовая маска индексов позиций.

## Localization

Отображаемые имена, подсказки и описания локализуются через языковые оверлеи; числовые ID, коды
секторов и значения матрицы — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Код игры: загрузчик позиций ПВО, матрица покрытия и вспомогательные функции секторов.
- Данные таблицы карты и XML-комментарии.
- Типы полей соответствуют загрузчику, локальной структуре и массивам времени выполнения; описания
  следуют коду, который использует каждое значение.

## Open questions

- Матрица покрытия — битовая маска (`1 << (samIndex - 1)`), а не список индексов позиций;
  поставляемые значения включают комбинации, например `5`, `6`, `9`, `12` и `13`.
- Индексы `samIndex` 1..4 имеют жёстко заданные роли в диалогах Meanwhile/Skyrider и в логике
  отступления; их изменение может сломать эти сцены.
- Загрузчик сохраняет строки и столбцы матрицы `0` и `17`, хотя проверки покрытия читают только
  `1..16`; неизвестно, влияет ли ненулевое значение на границе.
- Разборщик матрицы не проверяет число столбцов; строка более чем с 18 ячейками запишет данные за
  пределы массива строки.
