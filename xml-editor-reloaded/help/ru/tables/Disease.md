---
schema: 1
lang: ru
family: disease
title: "Disease — определения болезней"
tables:
  - Disease
table_ids:
  - Disease
files:
  - Disease.xml
root: DISEASESLIST
record: DISEASE
key: uiIndex
sources:
  - Game code (disease loader and hourly update)
  - Disease table data
  - Disease schema comments
status: reviewed
updated: 2026-11-12
---

# Disease — определения болезней

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`DISEASESLIST/DISEASE/uiIndex`](#diseaseslistdiseaseuiindex)
  - [`DISEASESLIST/DISEASE/szName`](#diseaseslistdiseaseszname)
  - [`DISEASESLIST/DISEASE/szFatName`](#diseaseslistdiseaseszfatname)
  - [`DISEASESLIST/DISEASE/szDescription`](#diseaseslistdiseaseszdescription)
  - [`DISEASESLIST/DISEASE/sInfectionPtsInitial`](#diseaseslistdiseasesinfectionptsinitial)
  - [`DISEASESLIST/DISEASE/sInfectionPtsOutbreak`](#diseaseslistdiseasesinfectionptsoutbreak)
  - [`DISEASESLIST/DISEASE/sInfectionPtsFull`](#diseaseslistdiseasesinfectionptsfull)
  - [`DISEASESLIST/DISEASE/sInfectionPtsGainPerHour`](#diseaseslistdiseasesinfectionptsgainperhour)
  - [`DISEASESLIST/DISEASE/InfectionChance_SWAMP`](#diseaseslistdiseaseinfectionchance_swamp)
  - [`DISEASESLIST/DISEASE/InfectionChance_TROPICS`](#diseaseslistdiseaseinfectionchance_tropics)
  - [`DISEASESLIST/DISEASE/InfectionChance_SEX`](#diseaseslistdiseaseinfectionchance_sex)
  - [`DISEASESLIST/DISEASE/InfectionChance_CONTACT_HUMAN`](#diseaseslistdiseaseinfectionchance_contact_human)
  - [`DISEASESLIST/DISEASE/InfectionChance_CONTACT_CORPSE`](#diseaseslistdiseaseinfectionchance_contact_corpse)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_ANIMAL`](#diseaseslistdiseaseinfectionchance_wound_animal)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_OPEN`](#diseaseslistdiseaseinfectionchance_wound_open)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_GUNSHOT`](#diseaseslistdiseaseinfectionchance_wound_gunshot)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_FIRE`](#diseaseslistdiseaseinfectionchance_wound_fire)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_GAS`](#diseaseslistdiseaseinfectionchance_wound_gas)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_AGI`](#diseaseslistdiseaseinfectionchance_wound_agi)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_DEX`](#diseaseslistdiseaseinfectionchance_wound_dex)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_STR`](#diseaseslistdiseaseinfectionchance_wound_str)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_WIS`](#diseaseslistdiseaseinfectionchance_wound_wis)
  - [`DISEASESLIST/DISEASE/InfectionChance_WOUND_TRAUMATIC`](#diseaseslistdiseaseinfectionchance_wound_traumatic)
  - [`DISEASESLIST/DISEASE/InfectionChance_BADFOOD`](#diseaseslistdiseaseinfectionchance_badfood)
  - [`DISEASESLIST/DISEASE/InfectionChance_BADWATER`](#diseaseslistdiseaseinfectionchance_badwater)
  - [`DISEASESLIST/DISEASE/fCanBeCured`](#diseaseslistdiseasefcanbecured)
  - [`DISEASESLIST/DISEASE/fReverseOnFull`](#diseaseslistdiseasefreverseonfull)
  - [`DISEASESLIST/DISEASE/fCanReInfect`](#diseaseslistdiseasefcanreinfect)
  - [`DISEASESLIST/DISEASE/fHideSymbol`](#diseaseslistdiseasefhidesymbol)
  - [`DISEASESLIST/DISEASE/fDisgusting`](#diseaseslistdiseasefdisgusting)
  - [`DISEASESLIST/DISEASE/fSpecialFlagPTSDBuns`](#diseaseslistdiseasefspecialflagptsdbuns)
  - [`DISEASESLIST/DISEASE/fSpecialFlagContractDisability`](#diseaseslistdiseasefspecialflagcontractdisability)
  - [`DISEASESLIST/DISEASE/fSpecialFlagLimitedUseArms`](#diseaseslistdiseasefspecialflaglimitedusearms)
  - [`DISEASESLIST/DISEASE/fSpecialFlagLimitedUseLegs`](#diseaseslistdiseasefspecialflaglimiteduselegs)
  - [`DISEASESLIST/DISEASE/sEffStatAGI`](#diseaseslistdiseaseseffstatagi)
  - [`DISEASESLIST/DISEASE/sEffStatDEX`](#diseaseslistdiseaseseffstatdex)
  - [`DISEASESLIST/DISEASE/sEffStatSTR`](#diseaseslistdiseaseseffstatstr)
  - [`DISEASESLIST/DISEASE/sEffStatWIS`](#diseaseslistdiseaseseffstatwis)
  - [`DISEASESLIST/DISEASE/sEffStatEXP`](#diseaseslistdiseaseseffstatexp)
  - [`DISEASESLIST/DISEASE/sEffAP`](#diseaseslistdiseaseseffap)
  - [`DISEASESLIST/DISEASE/usMaxBreath`](#diseaseslistdiseaseusmaxbreath)
  - [`DISEASESLIST/DISEASE/sEffCarryStrength`](#diseaseslistdiseaseseffcarrystrength)
  - [`DISEASESLIST/DISEASE/sLifeRegenHundreds`](#diseaseslistdiseasesliferegenhundreds)
  - [`DISEASESLIST/DISEASE/sNeedToSleep`](#diseaseslistdiseasesneedtosleep)
  - [`DISEASESLIST/DISEASE/sDrinkModifier`](#diseaseslistdiseasesdrinkmodifier)
  - [`DISEASESLIST/DISEASE/sFoodModifier`](#diseaseslistdiseasesfoodmodifier)
  - [`DISEASESLIST/DISEASE/moralemodifier`](#diseaseslistdiseasemoralemodifier)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Disease.xml` определяет каждую болезнь как набор очков болезни плюс эффекты, которые эти очки вызывают. Заражение добавляет начальные очки; затем очки растут или падают каждый час, пока не пересекут порог вспышки, а порог полного заражения масштабирует симптомы. Таблица также определяет базовый шанс для каждого источника заражения и флаги, управляющие лечением, повторным заражением, видимостью и тяжёлыми ограничениями. Она действует, только когда включена система болезней.

Шансы заражения — это проценты, а не очки: каждое поле задаёт базовый шанс для одного подходящего события, например ежечасной проверки в болоте, ранения или испорченной еды. Сопротивляемость болезням и ситуативные модификаторы уменьшают итоговый шанс. Эффекты заданы при полном заражении и интерполируются от `1.0` к текущей величине.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Disease` | `Disease.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
DISEASESLIST
└── DISEASE
    ├── uiIndex
    ├── szName
    ├── szFatName
    ├── szDescription
    ├── sInfectionPtsInitial
    ├── sInfectionPtsOutbreak
    ├── sInfectionPtsFull
    ├── sInfectionPtsGainPerHour
    ├── InfectionChance_SWAMP
    ├── InfectionChance_TROPICS
    ├── InfectionChance_SEX
    ├── InfectionChance_CONTACT_HUMAN
    ├── InfectionChance_CONTACT_CORPSE
    ├── InfectionChance_WOUND_ANIMAL
    ├── InfectionChance_WOUND_OPEN
    ├── InfectionChance_WOUND_GUNSHOT
    ├── InfectionChance_WOUND_FIRE
    ├── InfectionChance_WOUND_GAS
    ├── InfectionChance_WOUND_AGI
    ├── InfectionChance_WOUND_DEX
    ├── InfectionChance_WOUND_STR
    ├── InfectionChance_WOUND_WIS
    ├── InfectionChance_WOUND_TRAUMATIC
    ├── InfectionChance_BADFOOD
    ├── InfectionChance_BADWATER
    ├── fCanBeCured
    ├── fReverseOnFull
    ├── fCanReInfect
    ├── fHideSymbol
    ├── fDisgusting
    ├── fSpecialFlagPTSDBuns
    ├── fSpecialFlagContractDisability
    ├── fSpecialFlagLimitedUseArms
    ├── fSpecialFlagLimitedUseLegs
    ├── sEffStatAGI
    ├── sEffStatDEX
    ├── sEffStatSTR
    ├── sEffStatWIS
    ├── sEffStatEXP
    ├── sEffAP
    ├── usMaxBreath
    ├── sEffCarryStrength
    ├── sLifeRegenHundreds
    ├── sNeedToSleep
    ├── sDrinkModifier
    ├── sFoodModifier
    └── moralemodifier
```

## Fields

### `DISEASESLIST/DISEASE/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс этой болезни в массиве `Disease`. Это ключ массива; индексы должны быть уникальными и неотрицательными.
- **Only used with:** DISEASE.
- **Values:** Целое значение болезни; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/szName`

- **Type:** `String` (`String`)
- **Description:** Отображаемое название в списке болезней и в сообщениях диагностики.
- **Only used with:** DISEASE.
- **Values:** Свободный текст.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/szFatName`

- **Type:** `String` (`String`)
- **Description:** Альтернативное название, используемое в отображении диагностики и состояния. Символы `|` управляют подсветкой по буквам; они не входят в отображаемое название.
- **Only used with:** DISEASE.
- **Values:** Свободный текст.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/szDescription`

- **Type:** `String` (`String`)
- **Description:** Описание болезни для игрока, показываемое на панели подробностей болезни.
- **Only used with:** DISEASE.
- **Values:** Свободный текст.
- **Default:** пустая строка, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sInfectionPtsInitial`

- **Type:** `Integer` (`INT32`)
- **Description:** Очки болезни, добавляемые при первичном заражении наёмника. Очки накапливаются по направлению к `sInfectionPtsOutbreak` и `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения начинают заражение ближе к вспышке; отрицательные значения не имеют смысла.
- **Values:** Целые очки болезни.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sInfectionPtsOutbreak`

- **Type:** `Integer` (`INT32`)
- **Description:** Очки болезни, при которых наступает вспышка: начинаются симптомы и эффекты характеристик, а болезнь становится видимой.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения откладывают вспышку; отрицательные значения не имеют смысла.
- **Values:** Целые очки болезни.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sInfectionPtsFull`

- **Type:** `Integer` (`INT32`)
- **Description:** Номинальные очки болезни при полном развитии заражения. Эффекты масштабируются текущими очками, делёнными на это значение.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения требуют больше очков для полного эффекта; отрицательные значения не имеют смысла.
- **Values:** Целые очки болезни.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sInfectionPtsGainPerHour`

- **Type:** `Integer` (`INT32`)
- **Description:** Очки болезни, добавляемые каждый час, пока наёмник заражён. `fReverseOnFull` меняет знак на стадии полного заражения.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения ускоряют развитие; отрицательные значения означают выздоровление.
- **Values:** Целые очки в час; отрицательные значения убирают очки.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_SWAMP`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах при каждой ежечасной проверке болезни, пока наёмник находится в болотном секторе или на болотной дороге. Сопротивляемость болезням и страх насекомых изменяют итоговый шанс.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; `0.5` означает 0.5%, а отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`; дробные значения допускаются (`0.5` = 0.5%).
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_TROPICS`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах при каждой ежечасной проверке болезни, пока наёмник находится в тропическом секторе или на тропической дороге. Сопротивляемость болезням и страх насекомых изменяют итоговый шанс.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; `0.25` означает 0.25%, а отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`; дробные значения допускаются (`0.25` = 0.25%).
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_SEX`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах от одной половой связи.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_CONTACT_HUMAN`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах за ежечасную проверку, когда в том же секторе находится другой заражённый наёмник. Совместная служба в отряде или машине либо пребывание обоих пациентами увеличивает итоговый шанс.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_CONTACT_CORPSE`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах за ежечасную проверку, когда в секторе есть очки болезни от трупов. Итоговый шанс также масштабируется уровнем заражения сектора.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_ANIMAL`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах, когда наёмник ранен животным или существом, например кровокотом или щупальцами.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_OPEN`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах, когда ранение клинком, в рукопашной или от взрыва оставляет свежую открытую рану. Итоговый шанс изменяется нанесённым уроном.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_GUNSHOT`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах, когда огнестрельное ранение наносит более 20 единиц урона.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_FIRE`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах, когда наёмник ранен огнём.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_GAS`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах, когда наёмник ранен газом, не являющимся огнём.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_AGI`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах, когда ранение вызывает критическую потерю «Ловкости», а не просто любое ранение. Это событие `WOUND_AGI` из системы болезней.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_DEX`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах, когда ранение вызывает критическую потерю «Проворства», а не просто любое ранение.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_STR`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах, когда ранение вызывает критическую потерю «Силы», а не просто любое ранение.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_WIS`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах, когда ранение вызывает критическую потерю «Мудрости», а не просто любое ранение.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_WOUND_TRAUMATIC`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах для травматического события, а не физического ранения: падение здоровья ниже `OKLIFE` после урона или сильный удар по морали при морали ниже 15.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_BADFOOD`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах при употреблении еды в плохом состоянии. Итоговый шанс растёт по мере ухудшения состояния еды.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/InfectionChance_BADWATER`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Базовый шанс заражения в процентах при питье воды в плохом состоянии. Итоговый шанс растёт по мере ухудшения состояния воды.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения означают более высокую заражаемость; отрицательные значения выходят за допустимый диапазон.
- **Values:** Процент, ограниченный диапазоном `0..100`.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fCanBeCured`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлен, врачи могут вылечить болезнь. Если снят, заражение может только идти своим ходом или повернуть вспять за счёт прироста очков.
- **Only used with:** DISEASE.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fReverseOnFull`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлен, то, как только очки болезни достигают `sInfectionPtsFull`, знак `sInfectionPtsGainPerHour` меняется на противоположный. При обычном положительном приросте это заставляет заражение угасать.
- **Only used with:** DISEASE.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fCanReInfect`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлен, болезнь можно применить повторно, пока наёмник уже заражён; если снят, имеющееся заражение блокирует новое.
- **Only used with:** DISEASE.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fHideSymbol`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлен, на портрете наёмника не показывается значок болезни даже после диагностики.
- **Only used with:** DISEASE.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fDisgusting`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлен, другие наёмники реагируют с отвращением, когда болезнь диагностирована и уже произошла вспышка.
- **Only used with:** DISEASE.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fSpecialFlagPTSDBuns`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлен и наёмник — Buns (или её хаотичная личность), эта болезнь может запустить её изменение личности из-за PTSD.
- **Only used with:** DISEASE.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fSpecialFlagContractDisability`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлен, заражение этой болезнью добавляет наёмнику новый недуг. Читается только при включённом `DISEASE_SEVERE_LIMITATIONS`.
- **Only used with:** DISEASE, DISEASE_SEVERE_LIMITATIONS.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fSpecialFlagLimitedUseArms`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлен, болезнь ограничивает использование рук. Шина может улучшить состояние. Читается только при включённом `DISEASE_SEVERE_LIMITATIONS`.
- **Only used with:** DISEASE, DISEASE_SEVERE_LIMITATIONS.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/fSpecialFlagLimitedUseLegs`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлен, болезнь ограничивает использование ног. Шина может улучшить состояние. Читается только при включённом `DISEASE_SEVERE_LIMITATIONS`.
- **Only used with:** DISEASE, DISEASE_SEVERE_LIMITATIONS.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatAGI`

- **Type:** `Integer` (`INT8`)
- **Description:** Процентное изменение эффективной «Ловкости» при полном заражении. Игра применяет его в расчёте эффективной характеристики, масштабируя текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** `+10` означает +10% эффективной «Ловкости», `-10` означает -10%, а `0` нейтрально.
- **Values:** Процент; `+10` = +10% эффективной «Ловкости» при полном заражении, `-10` = -10%.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatDEX`

- **Type:** `Integer` (`INT8`)
- **Description:** Процентное изменение эффективного «Проворства» при полном заражении. Игра применяет его в расчёте эффективной характеристики, масштабируя текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** `+10` означает +10% эффективного «Проворства», `-10` означает -10%, а `0` нейтрально.
- **Values:** Процент; `+10` = +10% эффективного «Проворства» при полном заражении, `-10` = -10%.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatSTR`

- **Type:** `Integer` (`INT8`)
- **Description:** Процентное изменение эффективной «Силы» при полном заражении. Игра применяет его в расчёте эффективной характеристики, масштабируя текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** `+10` означает +10% эффективной «Силы», `-10` означает -10%, а `0` нейтрально.
- **Values:** Процент; `+10` = +10% эффективной «Силы» при полном заражении, `-10` = -10%.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatWIS`

- **Type:** `Integer` (`INT8`)
- **Description:** Процентное изменение эффективной «Мудрости» при полном заражении. Игра применяет его в расчёте эффективной характеристики, масштабируя текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** `+10` означает +10% эффективной «Мудрости», `-10` означает -10%, а `0` нейтрально.
- **Values:** Процент; `+10` = +10% эффективной «Мудрости» при полном заражении, `-10` = -10%.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffStatEXP`

- **Type:** `Integer` (`INT8`)
- **Description:** Изменение эффективного уровня опыта при полном заражении, в уровнях. Игра добавляет его в расчёт эффективного уровня, масштабируя текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** положительные значения повышают эффективный уровень, отрицательные понижают, а `0` нейтрально.
- **Values:** Очки эффективного уровня; положительные повышают, отрицательные понижают.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffAP`

- **Type:** `Integer` (`INT8`)
- **Description:** Изменение очков действия при полном заражении. Фактическое изменение масштабируется текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** положительные значения добавляют ОД, отрицательные отнимают ОД, а `0` нейтрально.
- **Values:** Очки действия; положительные добавляют, отрицательные отнимают.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/usMaxBreath`

- **Type:** `Integer` (`UINT8`)
- **Description:** Процент, на который снижается максимум дыхания при полном заражении. Фактическое снижение масштабируется текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** Большие значения сильнее снижают дыхание; `20` означает -20% при полном заражении, а отрицательные значения не имеют смысла.
- **Values:** Процент `0..100`; `20` означает -20% при полном заражении.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sEffCarryStrength`

- **Type:** `Integer` (`INT8`)
- **Description:** Процентное изменение силы переноски при полном заражении. Игра применяет его в расчёте силы переноски, масштабируя текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** `+10` означает +10% силы переноски, `-10` означает -10%, а `0` нейтрально.
- **Values:** Процент; `+10` = +10% силы переноски при полном заражении, `-10` = -10%.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sLifeRegenHundreds`

- **Type:** `Integer` (`INT16`)
- **Description:** Изменение ежечасной регенерации здоровья при полном заражении, в сотых долях единицы здоровья. Фактическое изменение масштабируется текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** положительные значения лечат, отрицательные отнимают здоровье, а `0` нейтрально; `-600` означает -6 HP в час при полном заражении.
- **Values:** Сотые доли единицы здоровья в час; отрицательные значения отнимают здоровье.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sNeedToSleep`

- **Type:** `Integer` (`INT8`)
- **Description:** Изменение потребности во сне при полном заражении. Фактическое изменение масштабируется текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** положительные значения увеличивают потребность во сне, отрицательные уменьшают её, а `0` нейтрально.
- **Values:** Очки потребности во сне; положительные увеличивают, отрицательные уменьшают.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sDrinkModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентное изменение потребления воды при полном заражении. Фактическое изменение масштабируется текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** положительные значения увеличивают расход воды, отрицательные уменьшают его, а `0` нейтрально.
- **Values:** Процентное изменение потребления воды.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/sFoodModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентное изменение потребления еды при полном заражении. Фактическое изменение масштабируется текущими очками болезни, делёнными на `sInfectionPtsFull`.
- **Only used with:** DISEASE.
- **Numeric semantics:** положительные значения увеличивают расход еды, отрицательные уменьшают его, а `0` нейтрально.
- **Values:** Процентное изменение потребления еды.
- **Default:** `0`, если поле отсутствует; пустой элемент использует значение игры по умолчанию.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

### `DISEASESLIST/DISEASE/moralemodifier`

- **Type:** `Decimal` (`FLOAT`)
- **Description:** Множитель морали при полном заражении. Игра интерполирует от `1.0` к этому значению по мере приближения болезни к полному развитию.
- **Only used with:** DISEASE.
- **Numeric semantics:** `1.0` нейтрально, `0.8` означает мораль на 20% ниже, а `1.2` — на 20% выше; отрицательные значения не имеют смысла.
- **Values:** Дробный множитель; `1.0` нейтрально.
- **Default:** `1.0`, если поле отсутствует.
- **Source:** game code (disease loader, hourly disease update and effect application); Disease table data.
- **Confidence:** `source`

## Relations

- `DISEASE_EFFECT/disease` в [Drugs](Items/Drugs.md) указывает на `Disease.uiIndex`.
- Система болезней управляется `DISEASE`; три специальных флага также требуют `DISEASE_SEVERE_LIMITATIONS`.

## Localization

`szName`, `szFatName` и `szDescription` — отображаемый текст; числовые эффекты не локализуются.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Игровой код: `XML_Disease.cpp` (загрузчик), `Disease.cpp` (ежечасное обновление и проверки заражения), `Soldier Control.cpp` (величина и применение эффектов).
- Данные таблицы Disease и комментарии схемы.
- Типы полей и ограничения: код болезней и XML-загрузчик.
- Описания охватывают путь кода, который использует каждое поле; количество строк и наблюдаемые диапазоны данных не используются.

## Open questions

Таблица хранит балансные значения; она не определяет, как часто происходят половая связь или приём испорченной еды. Точное поведение шины и тяжёлых ограничений реализовано кодом болезней, а не дополнительными XML-полями.
