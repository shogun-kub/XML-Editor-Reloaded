---
schema: 1
lang: ru
family: map-cities
title: "Cities — города, страна и карта секторов-городов"
tables:
  - Cities
table_ids:
  - Map.Cities
files:
  - Map/Cities.xml
root: CITY_INFO
record: CITY
key: uiIndex
sources:
  - Game code (strategic-map loader and map UI)
  - City table data
status: reviewed
updated: 2026-11-12
---

# Cities — города, страна и карта секторов-городов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CITY_INFO/CITYLIST/CITY/uiIndex`](#city_infocitylistcityuiindex)
  - [`CITY_INFO/CITYLIST/CITY/townName`](#city_infocitylistcitytownname)
  - [`CITY_INFO/CITYLIST/CITY/townUsesLoyalty`](#city_infocitylistcitytownusesloyalty)
  - [`CITY_INFO/CITYLIST/CITY/townRebelSentiment`](#city_infocitylistcitytownrebelsentiment)
  - [`CITY_INFO/CITYLIST/CITY/townMilitiaAllowed`](#city_infocitylistcitytownmilitiaallowed)
  - [`CITY_INFO/CITYLIST/CITY/hiddenTown`](#city_infocitylistcityhiddentown)
  - [`CITY_INFO/CITYLIST/CITY/townIcon`](#city_infocitylistcitytownicon)
  - [`CITY_INFO/CITYLIST/CITY/szIconFile`](#city_infocitylistcitysziconfile)
  - [`CITY_INFO/CITYLIST/CITY/iconPosition/x`](#city_infocitylistcityiconpositionx)
  - [`CITY_INFO/CITYLIST/CITY/iconPosition/y`](#city_infocitylistcityiconpositiony)
  - [`CITY_INFO/CITYLIST/CITY/baseSector/x`](#city_infocitylistcitybasesectorx)
  - [`CITY_INFO/CITYLIST/CITY/baseSector/y`](#city_infocitylistcitybasesectory)
  - [`CITY_INFO/CITYLIST/CITY/townPoint/x`](#city_infocitylistcitytownpointx)
  - [`CITY_INFO/CITYLIST/CITY/townPoint/y`](#city_infocitylistcitytownpointy)
  - [`CITY_INFO/COUNTRY/countryName`](#city_infocountrycountryname)
  - [`CITY_INFO/COUNTRY/countryNoun`](#city_infocountrycountrynoun)
  - [`CITY_INFO/CITY_TABLE/CITY_TABLE_ROW`](#city_infocity_tablecity_table_row)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Cities.xml` определяет города Арулко, название страны и матрицу «сектор — город». Игра
читает его в начале новой кампании; значения управляют лояльностью городов, склонностью к
мятежу, разрешением ополчения, значками стратегической карты и названием города,
показываемым для сектора.

Файл не является единым плоским списком: он содержит один синглтон `COUNTRY`, `CITYLIST`
городов и матрицу секторов-городов `CITY_TABLE`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Cities` | `Map/Cities.xml` | плюс `COUNTRY` и `CITY_TABLE` |

Редактор рассматривает `CITYLIST/CITY` как коллекцию записей (ключ `uiIndex`), а `COUNTRY` —
как корневой синглтон. `CITY_TABLE` — особая матрица, а не коллекция записей.

## Structure

```
CITY_INFO
├── CITYLIST
│   └── CITY
│       ├── uiIndex
│       ├── townName
│       ├── townUsesLoyalty
│       ├── townRebelSentiment
│       ├── townMilitiaAllowed
│       ├── hiddenTown
│       ├── townIcon
│       ├── szIconFile
│       ├── iconPosition
│       │   ├── x
│       │   └── y
│       ├── baseSector
│       │   ├── x
│       │   └── y
│       └── townPoint
│           ├── x
│           └── y
├── COUNTRY
│   ├── countryName
│   └── countryNoun
└── CITY_TABLE
    └── CITY_TABLE_ROW
```

`uiIndex` — это индекс в массиве городов. Загрузчик игнорирует `0` (зарезервирован для
«нет города») и значения вне массива; наибольший допустимый индекс + 1 становится `NUM_TOWNS`.
Дубликаты индексов перезаписали бы более ранние строки.

## Fields

### `CITY_INFO/CITYLIST/CITY/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Идентификатор города и индекс в массиве. `0` зарезервирован (нет города);
  загрузчик его пропускает. Наибольший индекс задаёт `NUM_TOWNS`.
- **Values:** `1..MAX_TOWNS-1`; загрузчик пропускает `0` и индексы вне массива.
- **Default:** нет; отсутствующее значение считается недопустимым, и город пропускается.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townName`

- **Type:** `String`
- **Description:** Отображаемое название города. Оно копируется в `pTownNames[uiIndex]` и
  переводится русским оверлеем в v4.
- **Values:** свободный текст, ограничен `MAX_TOWN_NAME_LENGHT`.
- **Default:** пусто.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townUsesLoyalty`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Поддерживает и отображает ли город лояльность. Когда выключено, события
  лояльности не меняют значение города.
- **Values:** `0` без лояльности, `1` использовать лояльность.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townRebelSentiment`

- **Type:** `Integer` (`UINT8`)
- **Description:** Насколько сильно город ненавидит правление Королевы. Это масштабирует скорость изменения лояльности: одно и то же событие меняет лояльность сильнее в городе с высоким значением.
- **Numeric semantics:** Большие значения означают более сильную склонность к мятежу и больший эффект изменения лояльности; отрицательные значения вне допустимого диапазона.
- **Values:** `1..100`. Загрузчик ограничивает значения выше 100 до 100, а значения ниже 1 — до 1.
- **Default:** `1` после ограничения.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townMilitiaAllowed`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Может ли игрок обучать ополчение в городе.
- **Values:** `0` без ополчения, `1` ополчение разрешено.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/hiddenTown`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Начинает ли город скрытым на стратегической карте. `0` означает видимый,
  `1` — скрытый. Загрузчик сохраняет обратное значение во внутреннем флаге видимости, а
  комментарий самого флага использует противоположное именование, поэтому внутреннее имя
  вводит в заблуждение; доверяйте значению XML и комментарию XML, а не имени внутреннего флага.
- **Values:** `0` видимый, `1` скрытый.
- **Default:** `0` (видимый).
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townIcon`

- **Type:** `Boolean` (`0`/`1`)
- **Description:** Показывает ли город свой особый значок на стратегической карте.
- **Values:** `0` выкл, `1` вкл.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/szIconFile`

- **Type:** `String`
- **Description:** Путь к STI-файлу, используемому как значок города, например
  `interface\\PRISON.sti`. Загрузчик копирует его в `gHiddenIcon[uiIndex].IconSti` до
  `MAX_ICON_CHARS`.
- **Values:** путь относительно игровых данных; пусто означает отсутствие особого значка.
- **Default:** пусто.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/iconPosition/x`

- **Type:** `Integer` (`UINT8`)
- **Description:** Позиция X особого значка города на стратегической карте.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** `0..255`; экранные координаты.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/iconPosition/y`

- **Type:** `Integer` (`UINT8`)
- **Description:** Позиция Y особого значка города на стратегической карте.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** `0..255`; экранные координаты.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/baseSector/x`

- **Type:** `Integer` (`UINT8`)
- **Description:** Координата X верхнего левого сектора вырезки 3x3 города в `Interface/MilitiaMaps.sti`. `1..16`; загрузчик игнорирует значения вне этого диапазона.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** `1..16`.
- **Default:** `0` (недопустимо; строка не размещается).
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/baseSector/y`

- **Type:** `Integer` (`UINT8`)
- **Description:** Координата Y верхнего левого сектора вырезки ополчения города.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** `1..16`.
- **Default:** `0` (недопустимо).
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townPoint/x`

- **Type:** `Integer` (32-bit map coordinate)
- **Description:** Позиция X метки названия города на стратегической карте, в десятых долях сектора (комментарий XML описывает её как `10 * (X, Y)`). Дроби возможны.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** координаты карты в десятых долях сектора.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITYLIST/CITY/townPoint/y`

- **Type:** `Integer` (32-bit map coordinate)
- **Description:** Позиция Y метки названия города, в десятых долях сектора.
- **Numeric semantics:** Координата; большие значения X/Y смещают вправо/вниз на карте или портрете, а отрицательные координаты вне допустимого диапазона.
- **Values:** координаты карты.
- **Default:** `0`.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/COUNTRY/countryName`

- **Type:** `String`
- **Description:** Название страны, показываемое в стратегическом интерфейсе и в некоторых диалогах.
- **Values:** свободный текст; русский оверлей предоставляет переведённое значение в v4.
- **Default:** пусто.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/COUNTRY/countryNoun`

- **Type:** `String`
- **Description:** Существительное для жителя страны (демоним), используемое в тексте диалогов.
- **Values:** свободный текст; локализуется в v4.
- **Default:** пусто.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

### `CITY_INFO/CITY_TABLE/CITY_TABLE_ROW`

- **Type:** `Matrix row` (text)
- **Description:** Одна строка матрицы «сектор — город». Атрибут `row` — это
  отсчитываемый от нуля Y карты; текст содержит разделённые пробелами значения `uiIndex` городов (0 = нет
  города). Загрузчик записывает каждое значение в `StrategicMap[x + 18*y].bNameId`.
- **Values:** целочисленные ID городов по строке; `row` — `00`..`17`.
- **Default:** нет; отсутствующая или короткая строка оставляет остальные ячейки без изменений.
- **Source:** game code (strategic-map loader and map UI); city table data.
- **Confidence:** `source`

## Relations

- Ячейки `CITY_INFO/CITY_TABLE/CITY_TABLE_ROW` -> `CITY_INFO/CITYLIST/CITY/uiIndex`.
- [Facilities](Facilities.md) размещает объекты в секторах; названия объектов берутся из
  [FacilityTypes](FacilityTypes.md).
- `townName`, `countryName` и `countryNoun` — единственные переводимые поля; см.
  `../../Localization.md`.

## Localization

`Map/Russian.Cities.xml` — оверлей для v4. Он содержит только `countryName`, `countryNoun` и
`townName`, с ключом по `uiIndex`. Числовые поля, матрица `CITY_TABLE` и поля значков
остаются в базовом файле.

## Versions

В v4 и v5 одинаковые поля и структура. В v4 поставляется русский оверлей; в v5 нет файла
`Russian.*`, что нормально для английской базы.

## Sources

- Игровой код: загрузчик стратегической карты, проверка и интерфейс карты.
- Данные таблицы: `Map/Cities.xml` и `Map/Russian.Cities.xml`.
- `hiddenTown` — известная ловушка именования: значение XML верное, но внутренний флаг
  видимости инвертирован. Это описано в записи поля выше.

## Open questions

- `townPoint` описан комментарием XML как десятые доли сектора; загрузчик сохраняет
  разобранное целое, а точное масштабирование в экранные пиксели находится в интерфейсе
  карты, а не в этой таблице.
- `hiddenTown` верен согласно комментарию XML, но внутренний флаг инвертирован. Не
  используйте имя внутреннего флага как значение XML-поля.
- Размеры матрицы фиксированы кодом. Модификация полной конверсии с другим размером мира
  потребовала бы изменения кода, а не только правок XML.
