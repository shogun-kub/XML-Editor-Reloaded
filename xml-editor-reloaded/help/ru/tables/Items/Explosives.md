---
schema: 1
lang: ru
family: items-explosives
title: "Explosives — определения взрывов"
tables:
  - Explosives
table_ids:
  - Items.Explosives
files:
  - Items/Explosives.xml
root: EXPLOSIVELIST
record: EXPLOSIVE
key: uiIndex
sources:
  - Explosive table data
  - Game code (explosive loader and `EXPLOSIVETYPE`)
  - Item metadata and lookup tables
status: reviewed
updated: 2026-11-12
---

# Explosives — определения взрывов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`EXPLOSIVELIST/EXPLOSIVE/uiIndex`](#explosivelistexplosiveuiindex)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubType`](#explosivelistexplosiveubtype)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubDamage`](#explosivelistexplosiveubdamage)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubStunDamage`](#explosivelistexplosiveubstundamage)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubRadius`](#explosivelistexplosiveubradius)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubVolume`](#explosivelistexplosiveubvolume)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubVolatility`](#explosivelistexplosiveubvolatility)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubAnimationID`](#explosivelistexplosiveubanimationid)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubDuration`](#explosivelistexplosiveubduration)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubStartRadius`](#explosivelistexplosiveubstartradius)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubMagSize`](#explosivelistexplosiveubmagsize)
  - [`EXPLOSIVELIST/EXPLOSIVE/fExplodeOnImpact`](#explosivelistexplosivefexplodeonimpact)
  - [`EXPLOSIVELIST/EXPLOSIVE/usNumFragments`](#explosivelistexplosiveusnumfragments)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubFragType`](#explosivelistexplosiveubfragtype)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubFragDamage`](#explosivelistexplosiveubfragdamage)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubFragRange`](#explosivelistexplosiveubfragrange)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubHorizontalDegree`](#explosivelistexplosiveubhorizontaldegree)
  - [`EXPLOSIVELIST/EXPLOSIVE/ubVerticalDegree`](#explosivelistexplosiveubverticaldegree)
  - [`EXPLOSIVELIST/EXPLOSIVE/bIndoorModifier`](#explosivelistexplosivebindoormodifier)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Explosives.xml` определяет взрыв, создаваемый взрывчатыми предметами, гранатами, боеголовками пусковых установок и взрывами окружения. Взрывчатый предмет указывает сюда через `ubClassIndex`; строка управляет уроном, радиусом, оглушением, осколками, длительностью газа/дыма и используемой анимацией.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Explosives` | `Items/Explosives.xml` | В v4 и v5 одинаковые 19 путей полей, включая `bIndoorModifier`; эталонные файлы содержат одинаковые значения. |

## Structure

```
EXPLOSIVELIST
└── EXPLOSIVE
    ├── uiIndex
    ├── ubType
    ├── ubDamage
    ├── ubStunDamage
    ├── ubRadius
    ├── ubVolume
    ├── ubVolatility
    ├── ubAnimationID
    ├── ubDuration
    ├── ubStartRadius
    ├── ubMagSize
    ├── fExplodeOnImpact
    ├── usNumFragments
    ├── ubFragType
    ├── ubFragDamage
    ├── ubFragRange
    ├── ubHorizontalDegree
    ├── ubVerticalDegree
    └── bIndoorModifier
```

## Game options

Для этой таблицы не документировано глобальной опции уровня таблицы. Записи не зависят от модели инвентаря. Создание осколков отключено в многопользовательской игре (`FireFragments` сразу возвращает управление в сетевом режиме); остальные поля используются как обычно.

## Fields

### `EXPLOSIVELIST/EXPLOSIVE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс строки взрыва в таблице взрывов времени выполнения. `Items.ubClassIndex` взрывчатого предмета указывает сюда.
- **Values:** `0..16000`; загрузчик пишет в `Explosive[MAXITEMS]`, где `MAXITEMS` = `16001`, а строки с большим `uiIndex` молча игнорируются.
- **Default:** Отсутствие или пустой элемент означает `0`; парсер обнуляет запись, поэтому такая строка сохраняется с индексом `0`.
- **Source:** game code (explosive loader and runtime `Explosive` array); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Тип взрыва из `Lookup.ExplosionType`; он выбирает обработку урона, поведение газа/дыма и визуальное/звуковое оформление. Имена перечисления движка для последних записей: `EXPLOSV_BURNABLEGAS` (`8`), `EXPLOSV_SIGNAL_SMOKE` (`10`), `EXPLOSV_SMOKE_DEBRIS` (`11`) и `EXPLOSV_SMOKE_FIRERETARDANT` (`12`).
- **Values:** ID из `Lookup.ExplosionType`: `0` обычный, `1` оглушающий, `2` слезоточивый газ, `3` иприт, `4` фальшфейер, `5` шум, `6` дым, `7` газ существ, `8` огонь, `9` светошумовая граната, `10` сигнальный снаряд, `11` обрушение крыши, `12` белый дым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; `EXPLOSV_*` enum); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubDamage`

- **Type:** `Integer` (`UINT8`)
- **Description:** Базовый урон от ран, наносимый взрывом. Игра добавляет случайный бросок `0..99%` и уменьшает результат по расстоянию в пределах `ubRadius`; если взрыв происходит в помещении, сначала применяется `bIndoorModifier`.
- **Numeric semantics:** Большие значения означают больший урон; загрузчик хранит это поле как `UINT8`.
- **Values:** `0..250`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; explosion damage calculation); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubStunDamage`

- **Type:** `Integer` (`UINT8`)
- **Description:** Базовый урон оглушения (дыхания), наносимый взрывом. Применяются тот же случайный бросок (в половинную силу) и спад по радиусу, что и для `ubDamage`, а в помещении применяется `bIndoorModifier`.
- **Numeric semantics:** Большие значения означают больший урон оглушения/дыхания; загрузчик хранит это поле как `UINT8`.
- **Values:** `0..250`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; explosion breath-damage calculation); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubRadius`

- **Type:** `Integer` (`UINT8`)
- **Description:** Радиус эффекта в клетках. Урон линейно убывает с расстоянием до этого радиуса; на нём или дальше урон становится `sWoundAmt / ubRadius / 2`. `0` оставляет урон несниженным.
- **Numeric semantics:** Большие значения означают более широкий взрыв; загрузчик ограничивает разобранное значение диапазоном `0..255`.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; distance falloff); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Громкость шума взрыва, передаваемая в `MakeNoise` с `NOISE_EXPLOSION`. Она влияет на слух ИИ и не является громкостью воспроизведения звука.
- **Numeric semantics:** Большие значения означают более громкий взрыв; `0` делает его беззвучным для системы шума.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; explosion noise); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubVolatility`

- **Type:** `Integer` (`UINT8`)
- **Description:** Базовая нестабильность, используемая проверкой цепной реакции при получении взрывчатым предметом урона. Это не прямой процент: игра преобразует её в `50 + (value - 1) * 10` на земле или `3 + (value - 1) * 3` при переноске, затем масштабирует по состоянию предмета и полученному урону.
- **Numeric semantics:** Большие значения означают более нестабильную взрывчатку; `0` отключает случайную детонацию.
- **Values:** `0..255`; наблюдаемые значения — `0`, `1`, `2` и `10`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; `CheckForChainReaction` and `CalcTotalVolatility`); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubAnimationID`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс строки анимации и звука взрыва в `ExplosionData`. Он становится `ubTypeID` взрыва и выбирает STI взрыва, количество кадров и ID звуков.
- **Values:** Индекс строки в [ExplosionData](ExplosionData.md); поставляемые значения — `0..15`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; `GenerateExplosion` and `ExplosionData` lookup); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubDuration`

- **Type:** `Integer` (`UINT8`)
- **Description:** Длительность остаточного эффекта. Она используется как длительность дыма/газа (в ходах), длительность светового эффекта, счётчик ослепления от светошумовой гранаты и число волн сигнального снаряда; `0` означает отсутствие остаточного эффекта.
- **Numeric semantics:** Большие значения означают более длительный эффект, но единица измерения зависит от типа взрыва, который его использует.
- **Values:** `0..255`; наблюдаемые значения — `0..10`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; smoke, light and blinding code); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubStartRadius`

- **Type:** `Integer` (`UINT8`)
- **Description:** Начальный радиус эффекта в момент детонации. Дым и свет начинаются с этого радиуса, а эффект дыма расширяется к `ubRadius`.
- **Numeric semantics:** Большие значения означают большую начальную область; загрузчик хранит это поле как `UINT8`.
- **Values:** `0..255`; наблюдаемые значения — `0..7`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; smoke and light effect setup); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubMagSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Количество зарядов, которое даёт взрывчатый предмет при заряжании в многозарядный гранатомёт. Значения больше `1` позволяют гранатомёту стрелять предметом повторно, и каждый выстрел уменьшает состояние предмета на `ceil(100 / ubMagSize)`. Это не радиус взрыва.
- **Numeric semantics:** Большие значения означают больше выстрелов на заряженный предмет.
- **Values:** `0..255`; загрузчик хранит это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; `GetExpMagSize` and launcher ammo deduction); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/fExplodeOnImpact`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, предмет `IC_EXPLOSV` детонирует при ударе, а не ждёт запала или дистанционного подрыва. Предметы класса `IC_BOMB` всегда детонируют при ударе независимо от этого флага.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; `OBJECT_DETONATE_ON_IMPACT`); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/usNumFragments`

- **Type:** `Integer` (`UINT16`)
- **Description:** Количество осколочных снарядов, создаваемых взрывом. Осколки появляются, когда это значение положительно, кроме воды и многопользовательской игры; оценка урона ИИ добавляет урон осколков только для взрывов `EXPLOSV_NORMAL`.
- **Numeric semantics:** Большие значения означают больше осколков; каждый осколок использует `ubFragType`, `ubFragDamage` и `ubFragRange`.
- **Values:** `0..10000`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; `FireFragments` and fragment damage); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubFragType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Тип боеприпаса, используемый для осколочных снарядов; он задаёт урон осколка и поведение при попадании.
- **Values:** Индекс строки в [AmmoTypes](AmmoTypes.md); осколочный снаряд использует этот тип боеприпаса.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; fragment bullet setup); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubFragDamage`

- **Type:** `Integer` (`UINT16`)
- **Description:** Значение урона/попадания, используемое для каждого осколочного снаряда.
- **Numeric semantics:** Большие значения означают больший урон на осколок; загрузчик хранит это поле как `UINT16`.
- **Values:** `0..250`; загрузчик хранит это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; fragment bullet impact); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubFragRange`

- **Type:** `Integer` (`UINT16`)
- **Description:** Максимальная дальность полёта осколков в десятых долях клетки (`10` единиц = одна клетка). Игра использует это как дальность осколочной пули.
- **Numeric semantics:** Большие значения означают большую дальность осколков; отображение делит значение на `CELL_X_SIZE` (`10`), чтобы показать клетки.
- **Values:** `0..250`; загрузчик хранит это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; `FireFragments` and fragment range display); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubHorizontalDegree`

- **Type:** `Integer` (`UINT16`)
- **Description:** Полная ширина горизонтальной дуги конуса осколков в градусах для направленных взрывчатых веществ. Ненаправленные взрывчатые вещества игнорируют её и разбрасывают осколки случайно. Код берёт значение по модулю `360`, поэтому `360` ведёт себя как `0`, а `359` даёт почти полный круг.
- **Only used with:** Направленные взрывчатые вещества.
- **Numeric semantics:** Большие значения означают более широкий горизонтальный разброс; загрузчик хранит это поле как `UINT16`.
- **Values:** `0..360`; загрузчик хранит это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; directional fragment spread); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/ubVerticalDegree`

- **Type:** `Integer` (`UINT16`)
- **Description:** Полная ширина вертикальной дуги конуса осколков в градусах для направленных взрывчатых веществ. Ненаправленные взрывчатые вещества игнорируют её и разбрасывают осколки случайно. Код берёт значение по модулю `180`, поэтому `180` ведёт себя как `0`.
- **Only used with:** Направленные взрывчатые вещества.
- **Numeric semantics:** Большие значения означают более широкий вертикальный разброс; загрузчик хранит это поле как `UINT16`.
- **Values:** `0..360` в метаданных; код использует значение по модулю `180`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; directional fragment spread); explosive table data.
- **Confidence:** `source`

### `EXPLOSIVELIST/EXPLOSIVE/bIndoorModifier`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Множитель, применяемый к урону и оглушению, когда взрыв происходит в помещении. `0` оставляет взрыв без изменений; `0.25` добавляет `25%`, а `-1` делает урон в помещении нулевым.
- **Numeric semantics:** Положительные значения усиливают взрыв в помещении, отрицательные ослабляют, `0` нейтрален. Модификатор применяется только когда `InARoom` истинно.
- **Values:** `-1..10`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (explosive loader; `InARoom` damage adjustment); explosive table data.
- **Confidence:** `source`

## Relations

- `Items.ubClassIndex` взрывчатого предмета указывает на `Explosives.uiIndex`.
- `ubAnimationID` указывает на [ExplosionData](ExplosionData.md).
- `ubFragType` указывает на [AmmoTypes](AmmoTypes.md) для поведения осколков.
- `ubType` использует [Lookup.ExplosionType](../Lookup.md#explosiontype).

## Related formulas

Ни одна формула в [FORMULAS](../../FORMULAS.md) не применяется к этой таблице.

## Localization

В этой таблице нет отображаемых строк; локализованные названия и описания находятся в строке предмета в `Items`.

## Versions

В v4 и v5 одинаковые 19 путей полей, и `bIndoorModifier` присутствует в обеих версиях. Эталонные файлы v4 и v5 содержат одинаковые значения.

## Sources

- Данные таблицы взрывчатки.
- Игровой код (загрузчик взрывчатки, структура таблицы взрывов, функции взрыва, осколков и цепной реакции).
- Метаданные предметов и таблицы справочников.
- Типы и ограничения полей: метаданные предметов; числовые ограничения используют диапазоны валидации метаданных.
- Семантические описания используют игровой код; записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

`ubDuration` — одно значение, используемое повторно для длительности дыма, длительности светового эффекта, счётчика ослепления от светошумовой гранаты и волн сигнального снаряда. Таблица не может выразить разные длительности для этих эффектов; единица измерения и смысл зависят от типа взрыва, который его использует.
