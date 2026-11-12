---
schema: 1
lang: ru
family: map-sector-names
title: "SectorNames — названия секторов, погода и население"
tables:
  - SectorNames
table_ids:
  - Map.SectorNames
files:
  - Map/SectorNames.xml
root: SECTOR_NAMES
record: SECTOR
key: SectorGrid
sources:
  - Game code (sector-names loader, weather, disease, workers, radio scan and prison code)
  - Map table data and XML comments
status: reviewed
updated: 2026-11-12
---

# SectorNames — названия секторов, погода и население

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SECTOR_NAMES/SECTOR/SectorGrid`](#sector_namessectorsectorgrid)
  - [`SECTOR_NAMES/SECTOR/szUnexploredName`](#sector_namessectorszunexploredname)
  - [`SECTOR_NAMES/SECTOR/szDetailedUnexploredName`](#sector_namessectorszdetailedunexploredname)
  - [`SECTOR_NAMES/SECTOR/szExploredName`](#sector_namessectorszexploredname)
  - [`SECTOR_NAMES/SECTOR/szDetailedExploredName`](#sector_namessectorszdetailedexploredname)
  - [`SECTOR_NAMES/SECTOR/sWaterType`](#sector_namessectorswatertype)
  - [`SECTOR_NAMES/SECTOR/usNaturalDirt`](#sector_namessectorusnaturaldirt)
  - [`SECTOR_NAMES/SECTOR/usCivilianPopulation`](#sector_namessectoruscivilianpopulation)
  - [`SECTOR_NAMES/SECTOR/rainchance`](#sector_namessectorrainchance)
  - [`SECTOR_NAMES/SECTOR/sandstormchance`](#sector_namessectorsandstormchance)
  - [`SECTOR_NAMES/SECTOR/snowchance`](#sector_namessectorsnowchance)
  - [`SECTOR_NAMES/SECTOR/snakechance`](#sector_namessectorsnakechance)
  - [`SECTOR_NAMES/SECTOR/numsnakes`](#sector_namessectornumsnakes)
  - [`SECTOR_NAMES/SECTOR/bloodcatraidpossible`](#sector_namessectorbloodcatraidpossible)
  - [`SECTOR_NAMES/SECTOR/zombieraidpossible`](#sector_namessectorzombieraidpossible)
  - [`SECTOR_NAMES/SECTOR/banditraidpossible`](#sector_namessectorbanditraidpossible)
  - [`SECTOR_NAMES/SECTOR/maxworkers`](#sector_namessectormaxworkers)
  - [`SECTOR_NAMES/SECTOR/wealth`](#sector_namessectorwealth)
  - [`SECTOR_NAMES/SECTOR/usCurfewValue`](#sector_namessectoruscurfewvalue)
  - [`SECTOR_NAMES/SECTOR/sRadioScanModifier`](#sector_namessectorsradioscanmodifier)
  - [`SECTOR_NAMES/SECTOR/usPrisonRoomNumber`](#sector_namessectorusprisonroomnumber)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`SectorNames.xml` задаёт каждому сектору карты исследованные и неисследованные названия, шансы погоды и змей, значения воды и загрязнения, гражданское население, богатство, комендантский час, тюремную комнату и переключатели рейдов. Ключ таблицы — `SectorGrid`.

Все четыре поля `sz*Name` должны быть непустыми, чтобы использовались пользовательские названия сектора; если хотя бы одно пусто, игра игнорирует весь пользовательский набор и генерирует названия по умолчанию для всех четырёх. `rainchance`, `sandstormchance` и `snowchance` — веса по секторам при локализации погоды, а не самостоятельные ежедневные вероятности.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `SectorNames` | `Map/SectorNames.xml` | — |

## Structure

```
SECTOR_NAMES
└── SECTOR
    ├── SectorGrid
    ├── szUnexploredName
    ├── szDetailedUnexploredName
    ├── szExploredName
    ├── szDetailedExploredName
    ├── sWaterType
    ├── usNaturalDirt
    ├── usCivilianPopulation
    ├── rainchance
    ├── sandstormchance
    ├── snowchance
    ├── snakechance
    ├── numsnakes
    ├── bloodcatraidpossible
    ├── zombieraidpossible
    ├── banditraidpossible
    ├── maxworkers
    ├── wealth
    ├── usCurfewValue
    ├── sRadioScanModifier
    └── usPrisonRoomNumber
```

## Fields

### `SECTOR_NAMES/SECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Код сектора в форме `A1`..`P16`: буква — строка карты (`y`), число — столбец (`x`). Это ключ таблицы.
- **Values:** `A1`..`P16`; обе координаты должны быть `1..16`, иначе парсер сохраняет свои предыдущие координаты.
- **Default:** Обязателен; идентифицирует запись.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/szUnexploredName`

- **Type:** `String`
- **Description:** Короткое название неисследованного сектора; сохраняется в слоте 0 и используется в недетализированном названии сектора на стратегической карте.
- **Values:** Свободный текст (UTF-8, преобразуется в `CHAR16`).
- **Default:** Отсутствует; пустая строка. Все четыре поля названия должны быть непустыми, иначе весь пользовательский набор игнорируется.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/szDetailedUnexploredName`

- **Type:** `String`
- **Description:** Подробное название неисследованного сектора; сохраняется в слоте 1 и используется в подробном названии сектора на стратегической карте.
- **Values:** Свободный текст (UTF-8, преобразуется в `CHAR16`).
- **Default:** Отсутствует; пустая строка. Все четыре поля названия должны быть непустыми, иначе весь пользовательский набор игнорируется.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/szExploredName`

- **Type:** `String`
- **Description:** Короткое название исследованного сектора; сохраняется в слоте 2 и используется в недетализированном названии после посещения сектора или обнаружения там позиции ПВО (SAM).
- **Values:** Свободный текст (UTF-8, преобразуется в `CHAR16`).
- **Default:** Отсутствует; пустая строка. Все четыре поля названия должны быть непустыми, иначе весь пользовательский набор игнорируется.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/szDetailedExploredName`

- **Type:** `String`
- **Description:** Подробное название исследованного сектора; сохраняется в слоте 3 и используется в подробном названии после посещения сектора или обнаружения там позиции ПВО (SAM).
- **Values:** Свободный текст (UTF-8, преобразуется в `CHAR16`).
- **Default:** Отсутствует; пустая строка. Все четыре поля названия должны быть непустыми, иначе весь пользовательский набор игнорируется.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/sWaterType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Категория воды: `0` = нет воды, `1` = питьевая вода, `2` = солёная вода, `3` = вода, которая может представлять риск для здоровья (отравленная или болотная).
- **Values:** `0..3`.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/usNaturalDirt`

- **Type:** `Integer` (`UINT16`)
- **Description:** Процент накопления загрязнения для оружия, из которого стреляют в наземном секторе: `100` — чистый город, а большие значения накапливают загрязнение пропорционально быстрее (`1000` — пустыня, в десять раз быстрее). Код стрельбы умножает коэффициент загрязнения предмета на это значение и делит на `5000` при минимальном коэффициенте `1`; в помещении значение уменьшается вдвое, а подземные секторы используют фиксированное `100`.
- **Numeric semantics:** больше = грязнее.
- **Values:** Неотрицательный процент; поставляемые значения лежат в диапазоне `100..1000`.
- **Default:** Отсутствует; `100` (значение сброса загрузчика).
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/usCivilianPopulation`

- **Type:** `Integer` (`UINT16`)
- **Description:** Оценочное гражданское население сектора, включая гражданских, физически отсутствующих на тактической карте. Используется в расчётах болезней и добровольцев.
- **Values:** Неотрицательное количество.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/rainchance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Вес погоды по секторам, используемый при локализации дождя: он смещает стартовый сектор и задаёт шанс, что соседний сектор тоже получит эту погоду. Это не самостоятельная ежедневная вероятность дождя.
- **Values:** `0..100`; загрузчик сохраняет `UINT8` и после приведения применяет `min(100, value)`.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/sandstormchance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Вес погоды по секторам, используемый при локализации песчаных бурь: он смещает стартовый сектор и задаёт шанс, что соседний сектор тоже получит эту погоду. Это не самостоятельная ежедневная вероятность песчаной бури.
- **Values:** `0..100`; загрузчик сохраняет `UINT8` и после приведения применяет `min(100, value)`.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/snowchance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Вес погоды по секторам, используемый при локализации снега: он смещает стартовый сектор и задаёт шанс, что соседний сектор тоже получит эту погоду. Это не самостоятельная ежедневная вероятность снега.
- **Values:** `0..100`; загрузчик сохраняет `UINT8` и после приведения применяет `min(100, value)`.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/snakechance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс за ход, в процентах, что змеи могут напасть на людей в воде в этом секторе; проверяется только на поверхности (`z = 0`), когда змеи включены.
- **Values:** `0..100`; загрузчик сохраняет `UINT8` и после приведения ограничивает значением `100`.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/numsnakes`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число змей для змеиной атаки. Атака с положительным значением создаёт `1 + Random(numsnakes)` змей, поэтому фактическое количество — `1..numsnakes`.
- **Values:** `0..20`; загрузчик ограничивает значением `20`, хотя комментарий таблицы говорит `0..100`.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/bloodcatraidpossible`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если не ноль, кровяные коты могут совершать рейды на этот сектор; загрузчик устанавливает `SECTORFLAG_RAIDPOSSIBLE_BLOODCAT` в маске флагов сектора уровня 0.
- **Values:** `0` или `1`; любое ненулевое значение устанавливает флаг.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/zombieraidpossible`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если не ноль, зомби могут совершать рейды на этот сектор; загрузчик устанавливает `SECTORFLAG_RAIDPOSSIBLE_ZOMBIE` в маске флагов сектора уровня 0.
- **Values:** `0` или `1`; любое ненулевое значение устанавливает флаг.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/banditraidpossible`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если не ноль, бандиты могут совершать рейды на этот сектор; загрузчик устанавливает `SECTORFLAG_RAIDPOSSIBLE_BANDIT` в маске флагов сектора уровня 0.
- **Values:** `0` или `1`; любое ненулевое значение устанавливает флаг.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/maxworkers`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число рабочих, которых можно нанять в этом секторе. Используется как предел рабочих города и для начального заполнения рабочей силы (`maxworkers * dInitialWorkerRate`).
- **Values:** `0..255` (`UINT8`).
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/wealth`

- **Type:** `Integer` (`UINT16`)
- **Description:** Оценка богатства населения сектора; также используется как масштаб денег для поиска добычи (`wealth + Random(4 * wealth)`), поэтому `0` отключает это поведение.
- **Values:** `0..65535` (`UINT16`).
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/usCurfewValue`

- **Type:** `Integer` (`UINT8`)
- **Description:** Категория комендантского часа, используемая для обнаружения скрытных гражданских: `0` = комендантского часа нет, `1` = ночной комендантский час, `2` = гражданские не допускаются. Тактический ИИ повышает эффективное значение как минимум до `1` ночью и как минимум до `2` под землёй.
- **Values:** `0`, `1` или `2`.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/sRadioScanModifier`

- **Type:** `Integer` (`INT8`)
- **Description:** Аддитивный модификатор дальности задания радиоперехвата в этом секторе; базовая дальность — параметр `RADIO_OPERATOR_ASSIGNMENT_SCAN_BASE_RANGE` в `Ja2_Options.ini` (по умолчанию `5`), и результат должен быть не меньше `1`, чтобы сканирование выполнялось.
- **Numeric semantics:** положительные = дальше, отрицательные = короче. Загрузчик ограничивает значения выше `3` значением `3`; задуманное нижнее ограничение `-3` перезаписывается в текущем исходнике, поэтому меньшие отрицательные значения сохраняются.
- **Values:** `INT8`; эффективная верхняя граница `3`.
- **Default:** Отсутствует; `0`.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

### `SECTOR_NAMES/SECTOR/usPrisonRoomNumber`

- **Type:** `Integer` (`UINT16`)
- **Description:** Номер комнаты тюрьмы в этом секторе. Элемент может повторяться; каждое ненулевое значение добавляется в список тюремных комнат сектора, и заключённые распределяются по этим комнатам по кругу. `0` ничего не добавляет.
- **Values:** `1..65535`; `0` = нет комнаты.
- **Default:** Отсутствует; тюремных комнат нет.
- **Source:** game code (sector-names loader and related game systems); table data and XML comments.
- **Confidence:** `source`

## Relations

- `SectorNames.SectorGrid` — канонический ключ сектора, используемый в [SectorLoadscreens](SectorLoadscreens.md), [MovementCosts](MovementCosts.md), [BloodcatPlacements](BloodcatPlacements.md) и [CreaturePlacements](CreaturePlacements.md).
- `SectorGrid` строки разбирается как буква (строка `y`), за которой следует число (столбец `x`); полученный ID сектора индексирует массивы названий и внешних данных.

## Localization

Отображаемые названия, всплывающие подсказки и описания локализуются через языковые оверлеи; числовые ID, коды секторов и значения матриц — нет.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Игровой код: загрузчик названий секторов; системы погоды, загрязнения, болезней, рабочих, радиоперехвата, тюремных комнат и комендантского часа.
- Данные таблиц карты и XML-комментарии.
- Типы полей следуют структуре данных сектора времени выполнения и приведениям загрузчика; описания следуют коду, который использует каждое значение.

## Open questions

- Загрузчик вычисляет `max(-3, value)`, а затем сразу перезаписывает результат через `min(3, value)`, поэтому задуманное нижнее ограничение `-3` не применяется; неизвестно, намеренно ли это.
- Недопустимый или отсутствующий `SectorGrid` не отвергается: парсер повторно использует координаты предыдущей записи (или `0,0` для первой записи), и запись может перезаписать другой сектор. Неизвестно, должен ли загрузчик её отвергать.
- `rainchance`, `sandstormchance` и `snowchance` передаются в `DetermineLocalizedWeatherGivenHelperArray`, который использует их и для выбора стартового сектора, и для распространения погоды на соседние секторы; полная формула локализации здесь не описана.
- Тюремные комнаты и флаги рейдов записываются только в слот времени выполнения уровня 0 (`[0]`); слоты уровней 1..3 остаются нулевыми, и неизвестно, должны ли их определять файлы уровней.
- Загрузчик сбрасывает `usNaturalDirt` в `100` после каждой записи, поэтому отсутствующее значение в первой записи начинается с `0`, а в последующих записях — с `100`; неизвестно, намерена ли эта асимметрия.
