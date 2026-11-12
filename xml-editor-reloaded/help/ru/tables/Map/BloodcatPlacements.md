---
schema: 1
lang: ru
family: map-bloodcat-placements
title: "BloodcatPlacements — размещение кровяных котов по секторам"
tables:
  - BloodcatPlacements
table_ids:
  - Map.BloodcatPlacements
files:
  - Map/BloodcatPlacements.XML
root: BLOODCAT_PLACEMENTS
record: SECTOR
key: SectorGrid
sources:
  - Game code (bloodcat placement loader and strategic/tactical use)
  - Map table data
  - Field names and XML comments
status: reviewed
updated: 2026-11-12
---

# BloodcatPlacements — размещение кровяных котов по секторам

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`BLOODCAT_PLACEMENTS/SECTOR/SectorGrid`](#bloodcat_placementssectorsectorgrid)
  - [`BLOODCAT_PLACEMENTS/SECTOR/ubPlacementType`](#bloodcat_placementssectorubplacementtype)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/fRespawn`](#bloodcat_placementssectornovicefrespawn)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubAmbushChance`](#bloodcat_placementssectornoviceubambushchance)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubFactionAffiliation`](#bloodcat_placementssectornoviceubfactionaffiliation)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMinBloodcats`](#bloodcat_placementssectornoviceubminbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMaxBloodcats`](#bloodcat_placementssectornoviceubmaxbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/fRespawn`](#bloodcat_placementssectorexperiencedfrespawn)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubAmbushChance`](#bloodcat_placementssectorexperiencedubambushchance)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubFactionAffiliation`](#bloodcat_placementssectorexperiencedubfactionaffiliation)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMinBloodcats`](#bloodcat_placementssectorexperiencedubminbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMaxBloodcats`](#bloodcat_placementssectorexperiencedubmaxbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/fRespawn`](#bloodcat_placementssectorexpertfrespawn)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubAmbushChance`](#bloodcat_placementssectorexpertubambushchance)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubFactionAffiliation`](#bloodcat_placementssectorexpertubfactionaffiliation)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMinBloodcats`](#bloodcat_placementssectorexpertubminbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMaxBloodcats`](#bloodcat_placementssectorexpertubmaxbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/fRespawn`](#bloodcat_placementssectorinsanefrespawn)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubAmbushChance`](#bloodcat_placementssectorinsaneubambushchance)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubFactionAffiliation`](#bloodcat_placementssectorinsaneubfactionaffiliation)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMinBloodcats`](#bloodcat_placementssectorinsaneubminbloodcats)
  - [`BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMaxBloodcats`](#bloodcat_placementssectorinsaneubmaxbloodcats)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`BloodcatPlacements.xml` перечисляет секторы, где могут появляться кровяные коты. У каждого сектора есть один тип размещения и для каждой сложности — значения размера группы, шанса засады, принадлежности к фракции и возрождения; какие из этих значений важны, зависит от типа размещения.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `BloodcatPlacements` | `Map/BloodcatPlacements.XML` | — |

## Structure

```
BLOODCAT_PLACEMENTS
└── SECTOR
    ├── SectorGrid
    ├── ubPlacementType
    ├── NOVICE
    │   ├── fRespawn
    │   ├── ubAmbushChance
    │   ├── ubFactionAffiliation
    │   ├── ubMinBloodcats
    │   └── ubMaxBloodcats
    ├── EXPERIENCED
    │   ├── fRespawn
    │   ├── ubAmbushChance
    │   ├── ubFactionAffiliation
    │   ├── ubMinBloodcats
    │   └── ubMaxBloodcats
    ├── EXPERT
    │   ├── fRespawn
    │   ├── ubAmbushChance
    │   ├── ubFactionAffiliation
    │   ├── ubMinBloodcats
    │   └── ubMaxBloodcats
    └── INSANE
        ├── fRespawn
        ├── ubAmbushChance
        ├── ubFactionAffiliation
        ├── ubMinBloodcats
        └── ubMaxBloodcats
```

## Fields

### `BLOODCAT_PLACEMENTS/SECTOR/SectorGrid`

- **Type:** `String`
- **Description:** Код сектора в форме `A1`..`P16`: буква строки (`A`..`P`), за которой следует номер столбца (`1`..`16`). Он выбирает `gBloodcatPlacements[sectorId]`; сам текст сетки в записи не сохраняется.
- **Values:** `A1`..`P16`. Загрузчик берёт первый символ как строку (`& 0x1F`) и разбирает остаток как столбец.
- **Default:** Обязателен; отсутствующий или некорректный код игнорируется (сохраняются только координаты, разобранные в допустимом диапазоне), поэтому используется предыдущий сектор.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/ubPlacementType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Правило размещения для сектора: `0` — засада (кровяные коты появляются только при срабатывании засады), `1` — статическое (они присутствуют с начала и подчиняются правилам фракции), `2` — логово (они присутствуют с начала и со временем пополняются). Должен существовать хотя бы один сектор-логово, иначе запуск игры завершается ошибкой.
- **Values:** `0` — засада, `1` — статическое размещение, `2` — логово; другие значения не обрабатываются.
- **Default:** `0`, если элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/fRespawn`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Переключатель возрождения для сложности «Новичок». Загрузчик сохраняет его только для размещения-логова (`ubPlacementType` `2`); ни один игровой код в исходниках 1.13 не читает этот флаг, поэтому сейчас он не влияет на пополнение.
- **Numeric semantics:** `0` = выключено, любое ненулевое значение = включено; байт сохраняется как записан.
- **Values:** `0` или `1`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubAmbushChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс срабатывания засады в этом секторе на сложности «Новичок», в процентах. Применяется только к размещению-засаде (`ubPlacementType` `0`). Эффективный шанс ограничен числом часов с момента последней загрузки сектора, поэтому он растёт на один пункт в час до настроенного значения.
- **Numeric semantics:** `0` отключает засаду; `100` и больше всегда проходят бросок `PreChance`; отрицательные значения невозможны.
- **Values:** Процент `0..100`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubFactionAffiliation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Принадлежность к группе гражданских, назначаемая кровяным котам сектора на сложности «Новичок». Используется только статическим размещением (`ubPlacementType` `1`): `0` означает отсутствие группы, ненулевое значение назначает эту группу, а `14` (`QUEENS_CIV_GROUP`) заставляет кровяных котов и армию игнорировать друг друга.
- **Values:** Индекс группы гражданских из перечисления `EditorMercs.h` (`0` = `NON_CIV_GROUP`, `14` = `QUEENS_CIV_GROUP`).
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `LOS.cpp`, `Overhead.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMinBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Нижняя граница группы при срабатывании засады кровяных котов на сложности «Новичок». Отсчёт начинается здесь и растёт с прогрессом игрока к `ubMaxBloodcats`. Статическое размещение и логово игнорируют его (загрузчик принудительно ставит `0`).
- **Numeric semantics:** больше = больше кровяных котов; не должно превышать `ubMaxBloodcats`, и загрузчик не проверяет пару, поэтому перевёрнутая пара вызывает переполнение снизу вычисленного диапазона.
- **Values:** Количество `0..255`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/NOVICE/ubMaxBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Верхняя граница группы засады на сложности «Новичок»; для статического размещения и логова это начальное число кровяных котов, задаваемое при старте игры, и вместимость сектора по кровяным котам.
- **Numeric semantics:** больше = больше кровяных котов; должно быть не меньше `ubMinBloodcats`.
- **Values:** Количество `0..255`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/fRespawn`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Переключатель возрождения для сложности «Опытный». Загрузчик сохраняет его только для размещения-логова (`ubPlacementType` `2`); ни один игровой код в исходниках 1.13 не читает этот флаг, поэтому сейчас он не влияет на пополнение.
- **Numeric semantics:** `0` = выключено, любое ненулевое значение = включено; байт сохраняется как записан.
- **Values:** `0` или `1`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubAmbushChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс срабатывания засады в этом секторе на сложности «Опытный», в процентах. Применяется только к размещению-засаде (`ubPlacementType` `0`). Эффективный шанс ограничен числом часов с момента последней загрузки сектора, поэтому он растёт на один пункт в час до настроенного значения.
- **Numeric semantics:** `0` отключает засаду; `100` и больше всегда проходят бросок `PreChance`; отрицательные значения невозможны.
- **Values:** Процент `0..100`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubFactionAffiliation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Принадлежность к группе гражданских, назначаемая кровяным котам сектора на сложности «Опытный». Используется только статическим размещением (`ubPlacementType` `1`): `0` означает отсутствие группы, ненулевое значение назначает эту группу, а `14` (`QUEENS_CIV_GROUP`) заставляет кровяных котов и армию игнорировать друг друга.
- **Values:** Индекс группы гражданских из перечисления `EditorMercs.h` (`0` = `NON_CIV_GROUP`, `14` = `QUEENS_CIV_GROUP`).
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `LOS.cpp`, `Overhead.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMinBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Нижняя граница группы при срабатывании засады кровяных котов на сложности «Опытный». Отсчёт начинается здесь и растёт с прогрессом игрока к `ubMaxBloodcats`. Статическое размещение и логово игнорируют его (загрузчик принудительно ставит `0`).
- **Numeric semantics:** больше = больше кровяных котов; не должно превышать `ubMaxBloodcats`, и загрузчик не проверяет пару, поэтому перевёрнутая пара вызывает переполнение снизу вычисленного диапазона.
- **Values:** Количество `0..255`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERIENCED/ubMaxBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Верхняя граница группы засады на сложности «Опытный»; для статического размещения и логова это начальное число кровяных котов, задаваемое при старте игры, и вместимость сектора по кровяным котам.
- **Numeric semantics:** больше = больше кровяных котов; должно быть не меньше `ubMinBloodcats`.
- **Values:** Количество `0..255`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/fRespawn`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Переключатель возрождения для сложности «Эксперт». Загрузчик сохраняет его только для размещения-логова (`ubPlacementType` `2`); ни один игровой код в исходниках 1.13 не читает этот флаг, поэтому сейчас он не влияет на пополнение.
- **Numeric semantics:** `0` = выключено, любое ненулевое значение = включено; байт сохраняется как записан.
- **Values:** `0` или `1`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubAmbushChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс срабатывания засады в этом секторе на сложности «Эксперт», в процентах. Применяется только к размещению-засаде (`ubPlacementType` `0`). Эффективный шанс ограничен числом часов с момента последней загрузки сектора, поэтому он растёт на один пункт в час до настроенного значения.
- **Numeric semantics:** `0` отключает засаду; `100` и больше всегда проходят бросок `PreChance`; отрицательные значения невозможны.
- **Values:** Процент `0..100`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubFactionAffiliation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Принадлежность к группе гражданских, назначаемая кровяным котам сектора на сложности «Эксперт». Используется только статическим размещением (`ubPlacementType` `1`): `0` означает отсутствие группы, ненулевое значение назначает эту группу, а `14` (`QUEENS_CIV_GROUP`) заставляет кровяных котов и армию игнорировать друг друга.
- **Values:** Индекс группы гражданских из перечисления `EditorMercs.h` (`0` = `NON_CIV_GROUP`, `14` = `QUEENS_CIV_GROUP`).
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `LOS.cpp`, `Overhead.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMinBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Нижняя граница группы при срабатывании засады кровяных котов на сложности «Эксперт». Отсчёт начинается здесь и растёт с прогрессом игрока к `ubMaxBloodcats`. Статическое размещение и логово игнорируют его (загрузчик принудительно ставит `0`).
- **Numeric semantics:** больше = больше кровяных котов; не должно превышать `ubMaxBloodcats`, и загрузчик не проверяет пару, поэтому перевёрнутая пара вызывает переполнение снизу вычисленного диапазона.
- **Values:** Количество `0..255`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/EXPERT/ubMaxBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Верхняя граница группы засады на сложности «Эксперт»; для статического размещения и логова это начальное число кровяных котов, задаваемое при старте игры, и вместимость сектора по кровяным котам.
- **Numeric semantics:** больше = больше кровяных котов; должно быть не меньше `ubMinBloodcats`.
- **Values:** Количество `0..255`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/fRespawn`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Переключатель возрождения для сложности «Безумный». Загрузчик сохраняет его только для размещения-логова (`ubPlacementType` `2`); ни один игровой код в исходниках 1.13 не читает этот флаг, поэтому сейчас он не влияет на пополнение.
- **Numeric semantics:** `0` = выключено, любое ненулевое значение = включено; байт сохраняется как записан.
- **Values:** `0` или `1`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubAmbushChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс срабатывания засады в этом секторе на сложности «Безумный», в процентах. Применяется только к размещению-засаде (`ubPlacementType` `0`). Эффективный шанс ограничен числом часов с момента последней загрузки сектора, поэтому он растёт на один пункт в час до настроенного значения.
- **Numeric semantics:** `0` отключает засаду; `100` и больше всегда проходят бросок `PreChance`; отрицательные значения невозможны.
- **Values:** Процент `0..100`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubFactionAffiliation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Принадлежность к группе гражданских, назначаемая кровяным котам сектора на сложности «Безумный». Используется только статическим размещением (`ubPlacementType` `1`): `0` означает отсутствие группы, ненулевое значение назначает эту группу, а `14` (`QUEENS_CIV_GROUP`) заставляет кровяных котов и армию игнорировать друг друга.
- **Values:** Индекс группы гражданских из перечисления `EditorMercs.h` (`0` = `NON_CIV_GROUP`, `14` = `QUEENS_CIV_GROUP`).
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `LOS.cpp`, `Overhead.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMinBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Нижняя граница группы при срабатывании засады кровяных котов на сложности «Безумный». Отсчёт начинается здесь и растёт с прогрессом игрока к `ubMaxBloodcats`. Статическое размещение и логово игнорируют его (загрузчик принудительно ставит `0`).
- **Numeric semantics:** больше = больше кровяных котов; не должно превышать `ubMaxBloodcats`, и загрузчик не проверяет пару, поэтому перевёрнутая пара вызывает переполнение снизу вычисленного диапазона.
- **Values:** Количество `0..255`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`); table data.
- **Confidence:** `source`

### `BLOODCAT_PLACEMENTS/SECTOR/INSANE/ubMaxBloodcats`

- **Type:** `Integer` (`UINT8`)
- **Description:** Верхняя граница группы засады на сложности «Безумный»; для статического размещения и логова это начальное число кровяных котов, задаваемое при старте игры, и вместимость сектора по кровяным котам.
- **Numeric semantics:** больше = больше кровяных котов; должно быть не меньше `ubMinBloodcats`.
- **Values:** Количество `0..255`.
- **Default:** `0`, если раздел или элемент отсутствует; массивы размещения очищаются перед загрузкой.
- **Source:** game code (`XML_Bloodcats.cpp`, `Strategic Movement.cpp`, `Game Init.cpp`); table data.
- **Confidence:** `source`

## Relations

- `SectorGrid` определяет сектор, названия и местность которого заданы в [SectorNames](SectorNames.md); разделы сложности повторяют уровни сложности кампании (`NOVICE` = 0, `EXPERIENCED` = 1, `EXPERT` = 2, `INSANE` = 3).
- `ubPlacementType` не зависит от сложности: он сохраняется один раз на сектор.
- `ubMaxBloodcats` также является вместимостью сектора по кровяным котам (`SectorInfo.bBloodCatPlacements`), которая ограничивает число засадных котов.
- При старте игры хотя бы один сектор должен иметь тип размещения `2` (логово), иначе инициализация выбрасывает исключение.
- `ubFactionAffiliation` применяется только к статическому размещению; значения — индексы групп гражданских, общие с [CivGroupNames](../CivGroupNames.md).

## Localization

В таблице нет отображаемого текста; коды секторов и числовые значения не локализуются.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Игровой код: `XML_Bloodcats.cpp` (загрузчик), `Strategic Movement.cpp` (логика засад и логова), `Game Init.cpp` (начальная популяция и требование логова), `LOS.cpp` и `Overhead.cpp` (поведение статической фракции).
- Данные таблиц карты.
- Имена полей и XML-комментарии.
- Описания охватывают путь кода, который использует каждое поле; количество строк и наблюдаемые диапазоны данных не используются.

## Open questions

- `fRespawn` сохраняется, но не читается игровым кодом 1.13; то, должно ли пополнение логова быть необязательным, в этих исходниках не реализовано.
- Рост `ubAmbushChance` на один пункт в час с момента последней загрузки сектора задан кодом и не документирован таблицей.
