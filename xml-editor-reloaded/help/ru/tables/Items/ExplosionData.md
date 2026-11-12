---
schema: 1
lang: ru
family: items-explosives
title: "ExplosionData — анимация и звук взрыва"
tables:
  - ExplosionData
table_ids:
  - Items.ExplosionData
files:
  - Items/ExplosionData.xml
root: EXPDATALIST
record: EXPDATA
key: uiIndex
sources:
  - Explosion data table
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# ExplosionData — анимация и звук взрыва

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`EXPDATALIST/EXPDATA/uiIndex`](#expdatalistexpdatauiindex)
  - [`EXPDATALIST/EXPDATA/name`](#expdatalistexpdataname)
  - [`EXPDATALIST/EXPDATA/TransKeyFrame`](#expdatalistexpdatatranskeyframe)
  - [`EXPDATALIST/EXPDATA/DamageKeyFrame`](#expdatalistexpdatadamagekeyframe)
  - [`EXPDATALIST/EXPDATA/ExplosionSoundID`](#expdatalistexpdataexplosionsoundid)
  - [`EXPDATALIST/EXPDATA/AltExplosionSoundID`](#expdatalistexpdataaltexplosionsoundid)
  - [`EXPDATALIST/EXPDATA/BlastFilename`](#expdatalistexpdatablastfilename)
  - [`EXPDATALIST/EXPDATA/BlastSpeed`](#expdatalistexpdatablastspeed)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`ExplosionData.xml` описывает визуальный взрыв и его звук: какая STI-анимация проигрывается, какие
кадры отмечают моменты прозрачности и урона и какие ID звуков сопровождают взрыв. Взрывчатка
выбирает строку через `ubAnimationID`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `ExplosionData` | `Items/ExplosionData.xml` | в v4 и v5 одинаковые пути полей; значения различаются. |

## Structure

```
EXPDATALIST
└── EXPDATA
    ├── uiIndex
    ├── name
    ├── TransKeyFrame
    ├── DamageKeyFrame
    ├── ExplosionSoundID
    ├── AltExplosionSoundID
    ├── BlastFilename
    └── BlastSpeed
```

## Game options

Для этой таблицы не документировано глобальной опции уровня таблицы. Её поля читаются в обеих
моделях инвентаря и в многопользовательской игре; единственное ограничение по версии — набор полей,
описанный в разделе [Versions](#versions).

## Fields

### `EXPDATALIST/EXPDATA/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс строки визуального представления взрыва. Сюда указывает
  `Explosives.ubAnimationID`.
- **Values:** Неотрицательное целое число; уникально внутри файла. Сюда указывает
  `Explosives.ubAnimationID`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** explosion data table; field metadata.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/name`

- **Type:** `String` (`String`)
- **Description:** Внутренняя метка, хранящаяся в таблице, но загрузчик не читает этот элемент; он
  игнорируется.
- **Values:** Свободный текст; игнорируется при загрузке.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (explosion-data loader); table data.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/TransKeyFrame`

- **Type:** `Integer` (`UINT8`)
- **Description:** Кадр анимации, на котором применяется ключ прозрачности.
- **Values:** Индекс кадра; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/DamageKeyFrame`

- **Type:** `Integer` (`UINT8`)
- **Description:** Кадр анимации, на котором взрыв начинает наносить урон.
- **Values:** Индекс кадра; `0` — первый кадр. Номер кадра не подразумевает масштаб урона.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/ExplosionSoundID`

- **Type:** `Integer` (`INT32`)
- **Description:** Основной ID звука, проигрываемого для взрыва.
- **Values:** Целое число; загрузчик сохраняет это поле как `INT32`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/AltExplosionSoundID`

- **Type:** `Integer` (`INT32`)
- **Description:** Альтернативный ID звука, используемый, когда взрыву нужен вариант.
- **Values:** Целое число; загрузчик сохраняет это поле как `INT32`.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/BlastFilename`

- **Type:** `String` (`CHAR8`)
- **Description:** Путь к STI-анимации взрыва относительно табличных и графических данных игры.
- **Values:** Путь STI, усекается до `MAX_BLAST_FILENAME_LEN`.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (explosion-data loader); table data.
- **Confidence:** `source`

### `EXPDATALIST/EXPDATA/BlastSpeed`

- **Type:** `Integer` (`INT8`)
- **Description:** Скорость воспроизведения анимации взрыва.
- **Numeric semantics:** Большие значения означают более высокую скорость; отрицательные значения не
  имеют смысла.
- **Values:** Целое число; загрузчик сохраняет это поле в знаковом байте.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (explosion data loader); explosion data table.
- **Confidence:** `source`

## Relations

- `Explosives.ubAnimationID` указывает на `ExplosionData.uiIndex`.
- `ExplosionSoundID` и `AltExplosionSoundID` — ID звуковой таблицы; звуковая таблица находится за
  пределами этого набора документов.

## Related formulas

Ни одна формула в [FORMULAS](../../FORMULAS.md) не применяется к этой таблице.

## Localization

`name` — внутренняя метка, а не локализуемая строка интерфейса. ID звуков не переводятся.

## Versions

В v4 и v5 используются одни и те же 8 путей полей; значения могут различаться.

## Sources

- Данные таблицы взрывов.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов; числовые ограничения используют диапазоны
  валидации метаданных.
- Семантические описания используют комментарии схемы таблицы и устаревшие метки полей; записи с
  пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

`TransKeyFrame` и `DamageKeyFrame` — индексы кадров анимации; тайминг кадров зависит от STI и не
является частью таблицы.
