---
schema: 1
lang: ru
family: items-ammo
title: "AmmoTypes — поведение боеприпасов"
tables:
  - AmmoTypes
table_ids:
  - Items.AmmoTypes
files:
  - Items/AmmoTypes.xml
root: AMMOTYPELIST
record: AMMOTYPE
key: uiIndex
sources:
  - Ammo type table data
  - AmmoTypes schema comment
  - Item metadata and lookup tables
status: reviewed
updated: 2026-11-12
---

# AmmoTypes — поведение боеприпасов

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Game options](#game-options)
- [Fields](#fields)
  - [`AMMOTYPELIST/AMMOTYPE/uiIndex`](#ammotypelistammotypeuiindex)
  - [`AMMOTYPELIST/AMMOTYPE/name`](#ammotypelistammotypename)
  - [`AMMOTYPELIST/AMMOTYPE/red`](#ammotypelistammotypered)
  - [`AMMOTYPELIST/AMMOTYPE/green`](#ammotypelistammotypegreen)
  - [`AMMOTYPELIST/AMMOTYPE/blue`](#ammotypelistammotypeblue)
  - [`AMMOTYPELIST/AMMOTYPE/structureImpactReductionMultiplier`](#ammotypelistammotypestructureimpactreductionmultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/structureImpactReductionDivisor`](#ammotypelistammotypestructureimpactreductiondivisor)
  - [`AMMOTYPELIST/AMMOTYPE/armourImpactReductionMultiplier`](#ammotypelistammotypearmourimpactreductionmultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/armourImpactReductionDivisor`](#ammotypelistammotypearmourimpactreductiondivisor)
  - [`AMMOTYPELIST/AMMOTYPE/beforeArmourDamageMultiplier`](#ammotypelistammotypebeforearmourdamagemultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/beforeArmourDamageDivisor`](#ammotypelistammotypebeforearmourdamagedivisor)
  - [`AMMOTYPELIST/AMMOTYPE/afterArmourDamageMultiplier`](#ammotypelistammotypeafterarmourdamagemultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/afterArmourDamageDivisor`](#ammotypelistammotypeafterarmourdamagedivisor)
  - [`AMMOTYPELIST/AMMOTYPE/zeroMinimumDamage`](#ammotypelistammotypezerominimumdamage)
  - [`AMMOTYPELIST/AMMOTYPE/usPiercePersonChanceModifier`](#ammotypelistammotypeuspiercepersonchancemodifier)
  - [`AMMOTYPELIST/AMMOTYPE/standardIssue`](#ammotypelistammotypestandardissue)
  - [`AMMOTYPELIST/AMMOTYPE/numberOfBullets`](#ammotypelistammotypenumberofbullets)
  - [`AMMOTYPELIST/AMMOTYPE/multipleBulletDamageMultiplier`](#ammotypelistammotypemultiplebulletdamagemultiplier)
  - [`AMMOTYPELIST/AMMOTYPE/multipleBulletDamageDivisor`](#ammotypelistammotypemultiplebulletdamagedivisor)
  - [`AMMOTYPELIST/AMMOTYPE/highExplosive`](#ammotypelistammotypehighexplosive)
  - [`AMMOTYPELIST/AMMOTYPE/explosionSize`](#ammotypelistammotypeexplosionsize)
  - [`AMMOTYPELIST/AMMOTYPE/dart`](#ammotypelistammotypedart)
  - [`AMMOTYPELIST/AMMOTYPE/knife`](#ammotypelistammotypeknife)
  - [`AMMOTYPELIST/AMMOTYPE/monsterSpit`](#ammotypelistammotypemonsterspit)
  - [`AMMOTYPELIST/AMMOTYPE/acidic`](#ammotypelistammotypeacidic)
  - [`AMMOTYPELIST/AMMOTYPE/ignoreArmour`](#ammotypelistammotypeignorearmour)
  - [`AMMOTYPELIST/AMMOTYPE/lockBustingPower`](#ammotypelistammotypelockbustingpower)
  - [`AMMOTYPELIST/AMMOTYPE/tracerEffect`](#ammotypelistammotypetracereffect)
  - [`AMMOTYPELIST/AMMOTYPE/spreadPattern`](#ammotypelistammotypespreadpattern)
  - [`AMMOTYPELIST/AMMOTYPE/temperatureModificator`](#ammotypelistammotypetemperaturemodificator)
  - [`AMMOTYPELIST/AMMOTYPE/dirtModificator`](#ammotypelistammotypedirtmodificator)
  - [`AMMOTYPELIST/AMMOTYPE/ammoflag`](#ammotypelistammotypeammoflag)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierLife`](#ammotypelistammotypeddamagemodifierlife)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierBreath`](#ammotypelistammotypeddamagemodifierbreath)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierTank`](#ammotypelistammotypeddamagemodifiertank)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierArmouredVehicle`](#ammotypelistammotypeddamagemodifierarmouredvehicle)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierCivilianVehicle`](#ammotypelistammotypeddamagemodifiercivilianvehicle)
  - [`AMMOTYPELIST/AMMOTYPE/dDamageModifierZombie`](#ammotypelistammotypeddamagemodifierzombie)
- [Relations](#relations)
- [Related formulas](#related-formulas)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AmmoTypes.xml` определяет, что делает снаряд после выстрела: пробитие брони, урон по различным классам целей, поведение осколков, трассеры и особые эффекты. Магазины выбирают строку через `ubAmmoType`; взрывчатка использует строку как тип осколков.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `AmmoTypes` | `Items/AmmoTypes.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
AMMOTYPELIST
└── AMMOTYPE
    ├── uiIndex
    ├── name
    ├── red
    ├── green
    ├── blue
    ├── structureImpactReductionMultiplier
    ├── structureImpactReductionDivisor
    ├── armourImpactReductionMultiplier
    ├── armourImpactReductionDivisor
    ├── beforeArmourDamageMultiplier
    ├── beforeArmourDamageDivisor
    ├── afterArmourDamageMultiplier
    ├── afterArmourDamageDivisor
    ├── zeroMinimumDamage
    ├── usPiercePersonChanceModifier
    ├── standardIssue
    ├── numberOfBullets
    ├── multipleBulletDamageMultiplier
    ├── multipleBulletDamageDivisor
    ├── highExplosive
    ├── explosionSize
    ├── dart
    ├── knife
    ├── monsterSpit
    ├── acidic
    ├── ignoreArmour
    ├── lockBustingPower
    ├── tracerEffect
    ├── spreadPattern
    ├── temperatureModificator
    ├── dirtModificator
    ├── ammoflag
    ├── dDamageModifierLife
    ├── dDamageModifierBreath
    ├── dDamageModifierTank
    ├── dDamageModifierArmouredVehicle
    ├── dDamageModifierCivilianVehicle
    └── dDamageModifierZombie
```

## Game options

Для этой таблицы не документировано глобальной опции уровня таблицы. Её поля читаются в обеих моделях инвентаря и в многопользовательской игре; единственное ограничение по версии — набор полей, описанный в разделе [Versions](#versions).

## Fields

Комментарий схемы — основное описание этой таблицы; поля этапов урона являются множителями/делителями, а не фиксированными штрафами. См. [FORMULAS](../../FORMULAS.md).

### `AMMOTYPELIST/AMMOTYPE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс строки поведения боеприпаса. Магазины ссылаются на него через `ubAmmoType`; взрывчатка может ссылаться на него как на тип осколков.
- **Values:** Неотрицательное целое; уникально в пределах файла. На него ссылаются `Magazines.ubAmmoType` и `Explosives.ubFragType`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/name`

- **Type:** `String` (`String`)
- **Description:** Отображаемое название поведения боеприпаса, показываемое в описаниях предметов и списках редактора.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/red`

- **Type:** `Integer` (`UINT8`)
- **Description:** Красная компонента цвета количества боеприпасов, `0..255`.
- **Values:** `0..255`; байтовая компонента цвета.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/green`

- **Type:** `Integer` (`UINT8`)
- **Description:** Зелёная компонента цвета количества боеприпасов, `0..255`.
- **Values:** `0..255`; байтовая компонента цвета.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/blue`

- **Type:** `Integer` (`UINT8`)
- **Description:** Синяя компонента цвета количества боеприпасов, `0..255`.
- **Values:** `0..255`; байтовая компонента цвета.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/structureImpactReductionMultiplier`

- **Type:** `Integer` (`INT16`)
- **Description:** Множитель в дроби снижения урона по строениям. Вместе с делителем он определяет, какая часть урона пули теряется при попадании в строение.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/structureImpactReductionDivisor`

- **Type:** `Integer` (`INT16`)
- **Description:** Делитель в паре с `structureImpactReductionMultiplier` для снижения урона по строениям.
- **Numeric semantics:** Большие значения означают более сильное снижение/сопротивление; отрицательные значения уменьшают эту защиту и не имеют смысла, если поле явно не определяет штраф.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/armourImpactReductionMultiplier`

- **Type:** `Integer` (`UINT16`)
- **Description:** Множитель, применяемый к значению защиты брони, в которую попадает пуля.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/armourImpactReductionDivisor`

- **Type:** `Integer` (`UINT16`)
- **Description:** Делитель в паре с `armourImpactReductionMultiplier` для защиты брони.
- **Numeric semantics:** Большие значения означают более сильное снижение/сопротивление; отрицательные значения уменьшают эту защиту и не имеют смысла, если поле явно не определяет штраф.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/beforeArmourDamageMultiplier`

- **Type:** `Integer` (`INT16`)
- **Description:** Множитель для урона, применяемого до попадания в броню. Используется только когда `highExplosive` больше нуля.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/beforeArmourDamageDivisor`

- **Type:** `Integer` (`INT16`)
- **Description:** Делитель в паре с `beforeArmourDamageMultiplier` для урона до брони.
- **Numeric semantics:** Большие значения означают больший урон; отрицательные значения не имеют смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/afterArmourDamageMultiplier`

- **Type:** `Integer` (`UINT16`)
- **Description:** Множитель для урона, применяемого после попадания в броню. Не используется против вооружённых машин.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/afterArmourDamageDivisor`

- **Type:** `Integer` (`UINT16`)
- **Description:** Делитель в паре с `afterArmourDamageMultiplier` для урона после брони.
- **Numeric semantics:** Большие значения означают больший урон; отрицательные значения не имеют смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/zeroMinimumDamage`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлено, урон пули может быть нулевым; иначе минимальный урон не меньше единицы.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/usPiercePersonChanceModifier`

- **Type:** `Integer` (`UINT16`)
- **Description:** Дополнительный шанс, что пуля продолжит полёт после попадания в человека, при условии, что оружие не является ракетной установкой или пушкой и пуля всё ещё достаточно мощна.
- **Numeric semantics:** Большие значения означают больший шанс; отрицательные значения выходят за допустимый диапазон шанса.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/standardIssue`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлено, магазины этого боеприпаса учитываются при генерации стартового инвентаря ИИ.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/numberOfBullets`

- **Type:** `Integer` (`INT16`)
- **Description:** Количество снарядов, выпускаемых за выстрел. Значения больше единицы означают картечь или заряд с несколькими снарядами.
- **Numeric semantics:** Большие значения означают большее количество; отрицательные значения не имеют смысла.
- **Values:** Количество снарядов за выстрел; `1` — одна пуля, значения больше `1` — картечь или заряд с несколькими снарядами.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/multipleBulletDamageMultiplier`

- **Type:** `Integer` (`UINT16`)
- **Description:** Множитель, применяемый сразу после выстрела, когда выпущено более одного снаряда. Вместе с делителем он масштабирует урон нескольких снарядов.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/multipleBulletDamageDivisor`

- **Type:** `Integer` (`UINT16`)
- **Description:** Делитель в паре с `multipleBulletDamageMultiplier` для урона нескольких снарядов.
- **Numeric semantics:** Большие значения означают больший урон; отрицательные значения не имеют смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/highExplosive`

- **Type:** `Integer` (`UINT16`)
- **Description:** Количество взрывчатых предметов, используемых при взрыве этого боеприпаса. Ноль означает, что боеприпас не использует взрывной путь.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/explosionSize`

- **Type:** `Integer` (`UINT8`)
- **Description:** Класс размера взрыва, используемый взрывным эффектом боеприпаса.
- **Numeric semantics:** Большие значения означают большее количество; отрицательные значения не имеют смысла.
- **Values:** ID из `Lookup.ExplosionSize`: `0` нет, `1` стандартный, `2` фугасный, `3` Big Bada Boom, `4` пламя / огнезащитный.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dart`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлено, урон больше нуля вызывает у цели сонливость; снаряд очень точен и может усыпить цель. Не действует на вооружённые машины.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/knife`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлено, снаряд ведёт себя как скрытная атака ножом: цель должна быть видна, снаряд не является осколком, а попадание может мгновенно убить.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/monsterSpit`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлено, снаряд использует поведение газа существ: повышенный урон от предметов с газом существ, ослепление, повреждение головных уборов и создание газа существ.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/acidic`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлено, броня разрушается в четыре раза быстрее.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/ignoreArmour`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлено, броня торса и ног игнорируется.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/lockBustingPower`

- **Type:** `Integer` (`UINT16`)
- **Description:** Дополнительный урон, наносимый замкам.
- **Numeric semantics:** Большие значения означают более сильный эффект, отрицательные значения не имеют смысла.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/tracerEffect`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлено, пули могут быть трассирующими. Трассеры не выбираются для оружия ИИ с одиночными выстрелами, и пламегасители их не скрывают.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/spreadPattern`

- **Type:** `String` (`String`)
- **Description:** Ключ шаблона разброса из `SpreadPatterns.xml`. Он задаёт шаблон рассеивания выстрела; пустое значение использует шаблон по умолчанию.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/temperatureModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Аддитивный модификатор генерации температуры. `1.0` означает +100%.
- **Numeric semantics:** Знаковый модификатор: большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект.
- **Values:** Десятичное число.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dirtModificator`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Аддитивный модификатор генерации загрязнения. `1.0` означает +100%.
- **Numeric semantics:** Знаковый модификатор: большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект.
- **Values:** Десятичное число.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/ammoflag`

- **Type:** `Flags` (`UINT16`)
- **Description:** Битовая маска особых свойств боеприпаса: крио, ослепление, противотехнический, след белого дыма и огненный след.
- **Values:** Битовая маска: `1` — крио, `2` — ослепление, `4` — противотехнический, `8` — след белого дыма, `16` — огненный след. Биты объединяются побитовым ИЛИ. Справочник `AmmoFlag` даёт подписи позиций битов, а не необработанное значение маски.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierLife`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Множитель урона, наносимого здоровью.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Десятичное число.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierBreath`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Множитель урона, наносимого дыханию.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Десятичное число.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierTank`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Дополнительный множитель урона здоровью, когда цель — танк.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Десятичное число.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierArmouredVehicle`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Дополнительный множитель урона здоровью, когда цель — вооружённая машина или робот, но не танк.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Десятичное число.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierCivilianVehicle`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Дополнительный множитель урона здоровью, когда цель — невооружённая машина.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Десятичное число.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

### `AMMOTYPELIST/AMMOTYPE/dDamageModifierZombie`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Дополнительный множитель урона здоровью, когда цель — зомби.
- **Numeric semantics:** Знаковый модификатор; большие значения означают более сильный положительный эффект, отрицательные дают противоположный эффект (штраф или снижение).
- **Values:** Десятичное число.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** ammo type table data; field metadata.
- **Confidence:** `source`

## Relations

- `Magazines.ubAmmoType` указывает на `AmmoTypes.uiIndex` в [Magazines](Magazines.md).
- Предмет-боеприпас в [Items](Items.md) использует строку магазина, которая, в свою очередь, выбирает тип боеприпаса; тип боеприпаса — не сама строка предмета.
- `Explosives.ubFragType` может указывать на `AmmoTypes.uiIndex`, чтобы повторно использовать поведение осколков в [Explosives](Explosives.md).
- `spreadPattern` задаёт имя шаблона в [SpreadPatterns](../SpreadPatterns.md).

## Related formulas

- [Ammo and weapon damage dealt](../../FORMULAS.md#ammo-and-weapon-damage-dealt)
- [Armour protection](../../FORMULAS.md#armour-protection)

## Localization

`name` — отображаемая подпись. Числовые и булевы поля не локализуются.

## Versions

В v4 и v5 используются одни и те же 38 путей полей; значения могут различаться.

## Sources

- Данные таблицы типов боеприпасов.
- Комментарий схемы AmmoTypes.
- Метаданные предметов и таблицы справочников.
- Типы и ограничения полей: метаданные предметов; числовые ограничения используют диапазоны валидации метаданных.
- Семантические описания используют комментарии схемы таблицы и устаревшие подписи полей; записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Порядок этапов урона по броне и точное соответствие между битами `ammoflag` и подписями справочника `AmmoFlag` в комментарии таблицы не указаны. Значения битов выше взяты из комментария схемы; рассматривайте ID справочника как подписи, а не как значения маски.
