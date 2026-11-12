---
schema: 1
lang: ru
family: map-facility-types
title: "FacilityTypes — объекты, назначения и производственные линии"
tables:
  - FacilityTypes
table_ids:
  - Map.FacilityTypes
files:
  - Map/FacilityTypes.xml
root: FACILITYTYPES
record: FACILITYTYPE
key: ubIndex
sources:
  - Map table data
  - Field names, XML comments and observed values
  - Game code (facility type loader and facility runtime)
status: reviewed
updated: 2026-11-12
---

# FacilityTypes — объекты, назначения и производственные линии

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`FACILITYTYPES/FACILITYTYPE/ubIndex`](#facilitytypesfacilitytypeubindex)
  - [`FACILITYTYPES/FACILITYTYPE/szFacilityName`](#facilitytypesfacilitytypeszfacilityname)
  - [`FACILITYTYPES/FACILITYTYPE/szFacilityShortName`](#facilitytypesfacilitytypeszfacilityshortname)
  - [`FACILITYTYPES/FACILITYTYPE/ubTotalStaffLimit`](#facilitytypesfacilitytypeubtotalstafflimit)
  - [`FACILITYTYPES/FACILITYTYPE/ubMilitiaTrainersAllowed`](#facilitytypesfacilitytypeubmilitiatrainersallowed)
  - [`FACILITYTYPES/FACILITYTYPE/pmcentrypoint`](#facilitytypesfacilitytypepmcentrypoint)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubAssignmentType`](#facilitytypesfacilitytypeassignmentubassignmenttype)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/szTooltipText`](#facilitytypesfacilitytypeassignmentsztooltiptext)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubStaffLimit`](#facilitytypesfacilitytypeassignmentubstafflimit)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPerformance`](#facilitytypesfacilitytypeassignmentusperformance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usKitDegrade`](#facilitytypesfacilitytypeassignmentuskitdegrade)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCostPerHour`](#facilitytypesfacilitytypeassignmentscostperhour)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usSleep`](#facilitytypesfacilitytypeassignmentussleep)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usFatigue`](#facilitytypesfacilitytypeassignmentusfatigue)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPrisonBaseLimit`](#facilitytypesfacilitytypeassignmentusprisonbaselimit)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCantinaFoodModifier`](#facilitytypesfacilitytypeassignmentscantinafoodmodifier)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fCountEnemiesInWild`](#facilitytypesfacilitytypeassignmentfcountenemiesinwild)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fDetectEnemiesDynamic`](#facilitytypesfacilitytypeassignmentfdetectenemiesdynamic)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubDetectEnemiesImmediate`](#facilitytypesfacilitytypeassignmentubdetectenemiesimmediate)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS`](#facilitytypesfacilitytypeassignmentconditions)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumAgility`](#facilitytypesfacilitytypeassignmentconditionsubminimumagility)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumDexterity`](#facilitytypesfacilitytypeassignmentconditionsubminimumdexterity)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumExplosives`](#facilitytypesfacilitytypeassignmentconditionsubminimumexplosives)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumHealth`](#facilitytypesfacilitytypeassignmentconditionsubminimumhealth)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLeadership`](#facilitytypesfacilitytypeassignmentconditionsubminimumleadership)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLevel`](#facilitytypesfacilitytypeassignmentconditionsubminimumlevel)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLoyaltyHere`](#facilitytypesfacilitytypeassignmentconditionsubminimumloyaltyhere)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMechanical`](#facilitytypesfacilitytypeassignmentconditionsubminimummechanical)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMedical`](#facilitytypesfacilitytypeassignmentconditionsubminimummedical)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumStrength`](#facilitytypesfacilitytypeassignmentconditionsubminimumstrength)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumWisdom`](#facilitytypesfacilitytypeassignmentconditionsubminimumwisdom)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/usPerformance`](#facilitytypesfacilitytypeassignmentconditionsusperformance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubChance`](#facilitytypesfacilitytypeassignmentmoraleubchance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/usChance`](#facilitytypesfacilitytypeassignmentmoraleuschance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/bBaseEffect`](#facilitytypesfacilitytypeassignmentmoralebbaseeffect)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubRange`](#facilitytypesfacilitytypeassignmentmoraleubrange)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubChance`](#facilitytypesfacilitytypeassignmentloyalty_localubchance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/bBaseEffect`](#facilitytypesfacilitytypeassignmentloyalty_localbbaseeffect)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubRange`](#facilitytypesfacilitytypeassignmentloyalty_localubrange)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubChance`](#facilitytypesfacilitytypeassignmentdrunkubchance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/bBaseEffect`](#facilitytypesfacilitytypeassignmentdrunkbbaseeffect)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubRange`](#facilitytypesfacilitytypeassignmentdrunkubrange)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/drugitems/drugitem`](#facilitytypesfacilitytypeassignmentdrunkdrugitemsdrugitem)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/bBaseEffect`](#facilitytypesfacilitytypeassignmentinjurybbaseeffect)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubChance`](#facilitytypesfacilitytypeassignmentinjuryubchance)
  - [`FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubRange`](#facilitytypesfacilitytypeassignmentinjuryubrange)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/requires_staff`](#facilitytypesfacilitytypeproductionlinerequires_staff)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sGridNo_Creation`](#facilitytypesfacilitytypeproductionlinesgridno_creation)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sHourlyCost`](#facilitytypesfacilitytypeproductionlineshourlycost)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sMinutesRequired`](#facilitytypesfacilitytypeproductionlinesminutesrequired)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szAdditionalRequirementTips`](#facilitytypesfacilitytypeproductionlineszadditionalrequirementtips)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szProductionName`](#facilitytypesfacilitytypeproductionlineszproductionname)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usItemToCreate`](#facilitytypesfacilitytypeproductionlineusitemtocreate)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_ItemUsedUp`](#facilitytypesfacilitytypeproductionlineusoptional_itemusedup)
  - [`FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_LoyaltyRequired`](#facilitytypesfacilitytypeproductionlineusoptional_loyaltyrequired)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`FacilityTypes.xml` описывает типы объектов и назначения, которые можно в них выполнять. У объекта есть название, лимиты персонала и необязательная точка входа PMC; у каждого назначения есть тип, подсказка, лимит персонала, производительность, стоимость, значения усталости и сна, а также необязательные эффекты морали, лояльности, опьянения и травм. Объекты также могут задавать производственные линии, которые превращают ресурсы в предметы за почасовую плату при требовании к лояльности.

Шансы рисков задаются относительно настройки INI `FACILITY_EVENT_RARITY` (по умолчанию `1000`, поэтому сохранённое `250` — это примерно 25% в час). Загрузчик сохраняет каждый `ubChance` через приведение к одному байту, поэтому записанные значения берутся по модулю 256: `500` превращается в `244`, а не в 50%. Поставляемый элемент `usChance` вообще отсутствует в списке элементов загрузчика, а `usPerformance` внутри `CONDITIONS` пропускается по той же причине.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `FacilityTypes` | `Map/FacilityTypes.xml` | — |

## Structure

```
FACILITYTYPES
└── FACILITYTYPE
    ├── ubIndex
    ├── szFacilityName
    ├── szFacilityShortName
    ├── ubTotalStaffLimit
    ├── ubMilitiaTrainersAllowed
    ├── pmcentrypoint
    ├── ASSIGNMENT
    │   ├── ubAssignmentType
    │   ├── szTooltipText
    │   ├── ubStaffLimit
    │   ├── usPerformance
    │   ├── usKitDegrade
    │   ├── sCostPerHour
    │   ├── usSleep
    │   ├── usFatigue
    │   ├── usPrisonBaseLimit
    │   ├── sCantinaFoodModifier
    │   ├── fCountEnemiesInWild
    │   ├── fDetectEnemiesDynamic
    │   ├── ubDetectEnemiesImmediate
    │   ├── CONDITIONS
    │   │   ├── ubMinimumAgility
    │   │   ├── ubMinimumDexterity
    │   │   ├── ubMinimumExplosives
    │   │   ├── ubMinimumHealth
    │   │   ├── ubMinimumLeadership
    │   │   ├── ubMinimumLevel
    │   │   ├── ubMinimumLoyaltyHere
    │   │   ├── ubMinimumMechanical
    │   │   ├── ubMinimumMedical
    │   │   ├── ubMinimumStrength
    │   │   ├── ubMinimumWisdom
    │   │   └── usPerformance
    │   ├── MORALE
    │   │   ├── ubChance
    │   │   ├── usChance
    │   │   ├── bBaseEffect
    │   │   └── ubRange
    │   ├── LOYALTY_LOCAL
    │   │   ├── ubChance
    │   │   ├── bBaseEffect
    │   │   └── ubRange
    │   ├── DRUNK
    │   │   ├── ubChance
    │   │   ├── bBaseEffect
    │   │   ├── ubRange
    │   │   └── drugitems
    │   │       └── drugitem
    │   └── INJURY
    │       ├── bBaseEffect
    │       ├── ubChance
    │       └── ubRange
    └── PRODUCTIONLINE
        ├── requires_staff
        ├── sGridNo_Creation
        ├── sHourlyCost
        ├── sMinutesRequired
        ├── szAdditionalRequirementTips
        ├── szProductionName
        ├── usItemToCreate
        ├── usOptional_ItemUsedUp
        └── usOptional_LoyaltyRequired
```

## Fields

### `FACILITYTYPES/FACILITYTYPE/ubIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс типа объекта. Это ключ массива, на который ссылаются размещённые объекты; загрузчик разбирает запись, но сохраняет её только если индекс не меньше `1`.
- **Values:** `1..254`; массив объектов содержит `MAX_NUM_FACILITY_TYPES` (`255`) записей, а индекс `0` не сохраняется.
- **Default:** Обязательно; идентифицирует запись или строку.
- **Source:** game code (facility type loader); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/szFacilityName`

- **Type:** `String` (`CHAR16`)
- **Description:** Полное отображаемое название объекта, например `Hospital` или `Munitions Factory`. Оно преобразуется из UTF-8 в буфер на 30 символов, поэтому более длинный текст усекается.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (facility type loader); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/szFacilityShortName`

- **Type:** `String` (`CHAR16`)
- **Description:** Краткое отображаемое название объекта. Оно преобразуется в буфер на 15 символов и используется в строке назначения панели наёмников на стратегической карте и в кнопках энциклопедии, где усекается до 7 символов.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (facility type loader; strategic screen and encyclopedia); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ubTotalStaffLimit`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число наёмников, которые могут одновременно работать на объекте по всем его назначениям. `0` означает, что там никто не может работать.
- **Numeric semantics:** Большие значения допускают больше персонала; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; free-slot counting); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ubMilitiaTrainersAllowed`

- **Type:** `Integer` (`UINT8`)
- **Description:** Число инструкторов ополчения, которых объект может одновременно принимать; оно добавляется к допустимому числу инструкторов ополчения в секторе.
- **Numeric semantics:** Большие значения допускают больше инструкторов; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; militia training allowance); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/pmcentrypoint`

- **Type:** `Boolean`
- **Description:** Элемент-флаг. Любое ненулевое значение устанавливает флаг точки входа PMC у объекта, и секторы с таким объектом предлагаются как площадки посадки PMC; само число не сохраняется.
- **Values:** `0` или `1`; любое ненулевое значение устанавливает флаг.
- **Default:** `0` (флаг снят), если элемент отсутствует.
- **Source:** game code (facility type loader; PMC landing-site list); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubAssignmentType`

- **Type:** `String`
- **Description:** Код типа назначения. Это закрытое перечисление с учётом регистра, а не свободный текст: загрузчик сопоставляет точную строку с индексом назначения и прерывает загрузку с ошибкой при неизвестном коде.
- **Values:** `AMBIENT`, `STAFF`, `FACILITY_EAT`, `REST`, `DOCTOR`, `PATIENT`, `REPAIR_ITEMS`, `REPAIR_VEHICLE`, `REPAIR_ROBOT`, `PRACTICE_STRENGTH`, `PRACTICE_DEXTERITY`, `PRACTICE_AGILITY`, `PRACTICE_HEALTH`, `PRACTICE_MARKSMANSHIP`, `PRACTICE_LEADERSHIP`, `PRACTICE_MEDICAL`, `PRACTICE_MECHANICAL`, `PRACTICE_EXPLOSIVES`, `TRAINER_STRENGTH`, `TRAINER_DEXTERITY`, `TRAINER_AGILITY`, `TRAINER_HEALTH`, `TRAINER_MARKSMANSHIP`, `TRAINER_LEADERSHIP`, `TRAINER_MECHANICAL`, `TRAINER_MEDICAL`, `TRAINER_EXPLOSIVES`, `STUDENT_STRENGTH`, `STUDENT_DEXTERITY`, `STUDENT_AGILITY`, `STUDENT_HEALTH`, `STUDENT_MARKSMANSHIP`, `STUDENT_LEADERSHIP`, `STUDENT_MECHANICAL`, `STUDENT_MEDICAL`, `STUDENT_EXPLOSIVES`, `INTERROGATE_PRISONERS`, `PRISON_SNITCH`, `SPREAD_PROPAGANDA`, `SPREAD_PROPAGANDA_GLOBAL`, `GATHER_RUMOURS`, `STRATEGIC_MILITIA_MOVEMENT`.
- **Default:** Обязательно; `ASSIGNMENT` без этого элемента прерывает загрузку.
- **Source:** game code (facility type loader assignment-type mapping); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/szTooltipText`

- **Type:** `String` (`CHAR16`)
- **Description:** Подсказка, показываемая для назначения в интерфейсе объекта. Это отображаемый текст, а не правило, и он преобразуется в буфер на 300 символов.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (facility type loader); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubStaffLimit`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число наёмников, назначенных на это конкретное назначение. Загрузчик требует ненулевое значение для каждого назначения, кроме `AMBIENT` (языковые оверлеи освобождены), и прерывает загрузку, если элемент отсутствует или равен `0`.
- **Numeric semantics:** Большие значения допускают больше работников на назначении; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; для назначений, отличных от `AMBIENT`, это недопустимо.
- **Source:** game code (facility type loader; assignment slot counting); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPerformance`

- **Type:** `Integer` (`UINT16`)
- **Description:** Эффективность назначения в процентах. Значение назначения и значение `AMBIENT` объекта (индекс `0`) складываются как смещения от `100`.
- **Numeric semantics:** `100` = обычная, `200` = вдвое выше обычной; большие значения эффективнее, отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `100`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility performance modifier); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usKitDegrade`

- **Type:** `Integer` (`UINT16`)
- **Description:** Скорость износа медицинского или ремонтного набора при выполнении назначения, в процентах.
- **Numeric semantics:** `100` = обычный износ, больше = быстрее износ; отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `100`, если элемент отсутствует.
- **Source:** game code (facility type loader; kit-degradation modifier); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCostPerHour`

- **Type:** `Integer` (`INT16`)
- **Description:** Деньги, списываемые за час, пока наёмник выполняет назначение. Отрицательные значения означают чистую прибыль.
- **Numeric semantics:** Большие значения дороже; отрицательные значения приносят игроку доход.
- **Values:** `-32768..32767`; загрузчик сохраняет это поле как `INT16`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility running costs); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usSleep`

- **Type:** `Integer` (`UINT16`)
- **Description:** Эффективность сна при выполнении этого назначения, в процентах.
- **Numeric semantics:** `100` = обычный отдых, больше = лучше отдых; отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `100`, если элемент отсутствует.
- **Source:** game code (facility type loader; sleep modifier); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usFatigue`

- **Type:** `Integer` (`UINT16`)
- **Description:** Скорость, с которой наёмник теряет максимальный запас дыхания при выполнении назначения, в процентах.
- **Numeric semantics:** `100` = обычная усталость, больше = быстрее теряется дыхание; отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `100`, если элемент отсутствует.
- **Source:** game code (facility type loader; fatigue modifier); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/usPrisonBaseLimit`

- **Type:** `Integer` (`UINT16`)
- **Description:** Базовое число мест для заключённых для тюремного назначения. Ненулевое значение также служит для игры признаком того, что объект является тюрьмой.
- **Numeric semantics:** Большие значения означают больше заключённых; отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; prison capacity and prison detection); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/sCantinaFoodModifier`

- **Type:** `Integer` (`INT16`)
- **Description:** Очки еды, добавляемые, когда наёмник ест на объекте с назначением `FACILITY_EAT`; количество напитка втрое больше этого значения. `0` или меньше отключает питание на объекте.
- **Numeric semantics:** Большие значения дают больше еды и напитков; неположительные значения отключают эффект, а не уменьшают его.
- **Values:** `-32768..32767`; загрузчик сохраняет это поле как `INT16`.
- **Default:** `100`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility food handling); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fCountEnemiesInWild`

- **Type:** `Boolean`
- **Description:** Если установлен, назначение включает подсчёт групп противника в диких секторах на стратегической карте.
- **Values:** `0` или `1`.
- **Default:** `0` (ложь), если элемент отсутствует.
- **Source:** game code (facility type loader; strategic detection flags); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/fDetectEnemiesDynamic`

- **Type:** `Boolean`
- **Description:** Если установлен, назначение включает обнаружение противника в районах мобильного ополчения.
- **Values:** `0` или `1`.
- **Default:** `0` (ложь), если элемент отсутствует.
- **Source:** game code (facility type loader; strategic detection flags); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/ubDetectEnemiesImmediate`

- **Type:** `Integer` (`UINT8`)
- **Description:** Радиус немедленного обнаружения в секторах. Каждый сектор в пределах этого расстояния от объекта помечается как обнаруженный, пока назначение укомплектовано персоналом. Загрузчик приводит значение через `BOOLEAN` (байтовый тип), поэтому значения больше `1` сохраняются.
- **Numeric semantics:** Большие значения означают более широкий радиус обнаружения; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; sector detection loop); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS`

- **Type:** `Container`
- **Description:** Необязательный контейнер минимальных требований к характеристикам для назначения. Пустой элемент `CONDITIONS` означает, что у назначения нет требований к навыкам; сам контейнер значения не имеет.
- **Values:** Пусто или один или несколько дочерних элементов минимальных характеристик.
- **Default:** Отсутствует; у назначения нет требований.
- **Source:** game code (facility type loader parse stage); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumAgility`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальная ловкость, необходимая наёмнику для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumDexterity`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальная проворность, необходимая для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumExplosives`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальный навык взрывчатки, необходимый для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumHealth`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальное здоровье, необходимое для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLeadership`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальное лидерство, необходимое для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLevel`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальный уровень опыта, необходимый для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumLoyaltyHere`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальная лояльность в городе, где размещён объект. Действует только в городских секторах.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMechanical`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальный навык механики, необходимый для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumMedical`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальный навык медицины, необходимый для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumStrength`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальная сила, необходимая для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/ubMinimumWisdom`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальная мудрость, необходимая для этого назначения.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; assignment condition check); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/CONDITIONS/usPerformance`

- **Type:** `Integer` (`UINT16`)
- **Description:** Значение производительности, вложенное в `CONDITIONS`. Список элементов `CONDITIONS` загрузчика не включает `usPerformance`, поэтому элемент пропускается и не даёт эффекта; размещайте `usPerformance` непосредственно в `ASSIGNMENT`.
- **Numeric semantics:** Большие значения означали бы более эффективное назначение, но в этой позиции значение не читается.
- **Values:** `0..65535`; загрузчик сохранил бы его как `UINT16`, если бы читал.
- **Default:** Эффекта нет, если элемент отсутствует.
- **Source:** game code (facility type loader `CONDITIONS` parse stage); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс в час, что сработает риск морали, заданный относительно настройки INI `FACILITY_EVENT_RARITY` (по умолчанию `1000`). Загрузчик сохраняет значение в одном байте, поэтому записанное значение берётся по модулю 256: `500` сохраняется как `244`, и только значения до `255` ведут себя так, как указаны.
- **Numeric semantics:** Большие значения вероятнее; `0` отключает эффект, а отрицательные значения заворачиваются в верхнюю часть байтового диапазона.
- **Values:** Задумано `0..1000`; фактически сохраняемый диапазон `0..255` из-за приведения в загрузчике.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader `(UINT8)` cast; facility risk roll); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/usChance`

- **Type:** `Integer`
- **Description:** Альтернативный элемент шанса, присутствующий в поставляемом `FacilityTypes.xml`, но в загрузчике нет ни ветки для элемента `usChance`, ни присваивания `usChance`. Он не читается и не влияет на игру.
- **Numeric semantics:** Задуманные единицы шанса, но ни один путь загрузчика не применяет этот элемент.
- **Values:** `0..1000` в поставляемых данных; текущий загрузчик игнорирует.
- **Default:** Игнорируется в любом случае.
- **Source:** game code (facility type loader reads only `ubChance`); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/bBaseEffect`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое изменение морали при срабатывании события морали. Оно прибавляется к морали наёмника (ограничивается `0..100`) и к стратегическому модификатору морали (ограничивается `-50..50`).
- **Numeric semantics:** Положительные значения повышают мораль, отрицательные понижают; `0` оставляет знак результата на волю случая.
- **Values:** `-128..127`; загрузчик сохраняет это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/MORALE/ubRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Случайное отклонение, добавляемое вокруг базового эффекта морали.
- **Numeric semantics:** Большие значения означают больший разброс; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс в час, что сработает риск местной лояльности, заданный относительно настройки INI `FACILITY_EVENT_RARITY` (по умолчанию `1000`). Как и для любого `ubChance`, загрузчик берёт записанное значение по модулю 256.
- **Numeric semantics:** Большие значения вероятнее; `0` отключает эффект, а отрицательные значения заворачиваются в верхнюю часть байтового диапазона.
- **Values:** Задумано `0..1000`; фактически сохраняемый диапазон `0..255` из-за приведения в загрузчике.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader `(UINT8)` cast; facility risk roll); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/bBaseEffect`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовое изменение местной лояльности при срабатывании события. Перед применением игра умножает значение на константу числа очков лояльности на процент.
- **Numeric semantics:** Положительные значения повышают лояльность, отрицательные понижают; `0` оставляет знак результата на волю случая.
- **Values:** `-128..127`; загрузчик сохраняет это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/LOYALTY_LOCAL/ubRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Случайное отклонение, добавляемое вокруг базового эффекта местной лояльности.
- **Numeric semantics:** Большие значения означают больший разброс; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс в час, что наёмник напьётся при выполнении этого назначения, заданный относительно настройки INI `FACILITY_EVENT_RARITY` (по умолчанию `1000`). Как и для любого `ubChance`, загрузчик берёт записанное значение по модулю 256.
- **Numeric semantics:** Большие значения вероятнее; `0` отключает эффект, а отрицательные значения заворачиваются в верхнюю часть байтового диапазона.
- **Values:** Задумано `0..1000`; фактически сохраняемый диапазон `0..255` из-за приведения в загрузчике.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader `(UINT8)` cast; facility risk roll); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/bBaseEffect`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовый результат события опьянения. Текущий код проверяет только, ненулевой ли итоговый результат, затем применяет одну из перечисленных записей `drugitem` или алкоголь, если список пуст.
- **Numeric semantics:** Ненулевой результат включает событие; знак и величина значения не меняют эффект.
- **Values:** `-128..127`; загрузчик сохраняет это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/ubRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Случайное отклонение, добавляемое вокруг базового результата опьянения.
- **Numeric semantics:** Большие значения означают больший разброс; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/DRUNK/drugitems/drugitem`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета — наркотика или напитка, — который может быть употреблён при срабатывании события опьянения. Элемент может повторяться, и игра выбирает одну запись случайно, поэтому повторение ID повышает вероятность, а не добавляет количество.
- **Values:** `-32768..32767`; загрузчик сохраняет каждую запись как `INT16`.
- **Default:** Пустой список; тогда игра применяет алкоголь.
- **Source:** game code (facility type loader drug-item vector; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/bBaseEffect`

- **Type:** `Integer` (`INT8`)
- **Description:** Базовый результат травмы. Применяется только отрицательный итоговый результат — как прямой урон плюс кровотечение; положительный итоговый результат ничего не делает.
- **Numeric semantics:** Более отрицательные базовые значения наносят больше урона; положительное базовое значение не лечит, а `0` всё ещё может дать урон через `ubRange`.
- **Values:** `-128..127`; загрузчик сохраняет это поле как `INT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubChance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс в час, что сработает риск травмы, заданный относительно настройки INI `FACILITY_EVENT_RARITY` (по умолчанию `1000`). Как и для любого `ubChance`, загрузчик берёт записанное значение по модулю 256.
- **Numeric semantics:** Большие значения вероятнее; `0` отключает эффект, а отрицательные значения заворачиваются в верхнюю часть байтового диапазона.
- **Values:** Задумано `0..1000`; фактически сохраняемый диапазон `0..255` из-за приведения в загрузчике.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader `(UINT8)` cast; facility risk roll); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/ASSIGNMENT/INJURY/ubRange`

- **Type:** `Integer` (`UINT8`)
- **Description:** Случайное отклонение, добавляемое вокруг базового эффекта травмы.
- **Numeric semantics:** Большие значения означают больший разброс; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; facility risk result); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/requires_staff`

- **Type:** `Boolean`
- **Description:** Если установлен, производственная линия работает только пока наёмник обслуживает объект; иначе она продолжает работать без присмотра.
- **Values:** `0` или `1`; любое ненулевое значение устанавливает флаг.
- **Default:** `0` (ложь), если элемент отсутствует.
- **Source:** game code (facility type loader `REQUIRES_STAFFING` flag; hourly factory update); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sGridNo_Creation`

- **Type:** `Integer` (`INT32`)
- **Description:** Номер клетки карты, где появляется произведённый предмет. `-1` — значение по умолчанию загрузчика, и это не настоящий номер клетки.
- **Values:** `-2147483648..2147483647`; `-1` — сигнальное значение по умолчанию.
- **Default:** `-1`, если элемент отсутствует.
- **Source:** game code (facility type loader; hourly factory update item placement); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sHourlyCost`

- **Type:** `Integer` (`INT32`)
- **Description:** Деньги, списываемые за час, пока работает производственная линия; они не списываются за каждый произведённый предмет. Отрицательные значения означают, что линия приносит деньги.
- **Numeric semantics:** Большие значения дороже; отрицательные значения дают доход.
- **Values:** `-2147483648..2147483647`; загрузчик сохраняет это поле как `INT32`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; hourly factory update cost); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/sMinutesRequired`

- **Type:** `Integer` (`INT32`)
- **Description:** Минуты, необходимые для производства одного предмета. Загрузчик ограничивает значение снизу единицей.
- **Numeric semantics:** Большие значения означают более медленное производство; `0` и отрицательные значения становятся `1`.
- **Values:** `1..2147483647`; загрузчик применяет `max(1, value)`.
- **Default:** `60`, если элемент отсутствует.
- **Source:** game code (facility type loader; hourly factory update); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szAdditionalRequirementTips`

- **Type:** `String` (`CHAR16`)
- **Description:** Дополнительный текст требований, показываемый для производственной линии, например `- Requires storage access`. Это отображаемый текст, а не правило, и он преобразуется в буфер на 80 символов.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (facility type loader); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/szProductionName`

- **Type:** `String` (`CHAR16`)
- **Description:** Необязательное отображаемое название произведённого предмета; если оно пустое, игра показывает собственное название предмета. Оно преобразуется в буфер на 30 символов.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (facility type loader; hourly factory update); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usItemToCreate`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, производимого этой линией. `0` означает, что предмет не задан.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16` и по умолчанию задаёт `NOTHING` (`0`).
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; item creation); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_ItemUsedUp`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, расходуемого на каждый произведённый предмет. Элемент повторяется по одному разу на каждую требуемую единицу, и повторение одного и того же ID увеличивает требуемое количество этого предмета.
- **Values:** `0..65535`; каждая запись сохраняется как `UINT16`.
- **Default:** Отсутствует; линия ничего не расходует.
- **Source:** game code (facility type loader pre-product list; hourly factory update); map table data.
- **Confidence:** `source`

### `FACILITYTYPES/FACILITYTYPE/PRODUCTIONLINE/usOptional_LoyaltyRequired`

- **Type:** `Integer` (`UINT8`)
- **Description:** Минимальная местная лояльность, необходимая для работы производственной линии. Проверяется только в городских секторах; `0` означает отсутствие требования.
- **Numeric semantics:** Большие значения строже; отрицательные значения не имеют смысла.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (facility type loader; hourly factory update loyalty check); map table data.
- **Confidence:** `source`

## Relations

- Размещённые объекты в [Facilities](Facilities.md) ссылаются на `FacilityTypes.ubIndex`; каждое `ASSIGNMENT` выбирает тип назначения, используемый интерфейсом объекта и стратегической системой назначений. Размещение объектов использует данные [Cities](Cities.md).

## Localization

Отображаемые названия, подсказки и описания локализуются через языковые оверлеи; числовые ID, коды секторов и значения матриц — нет.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться. В обеих версиях поставляются элементы `usChance` и `CONDITIONS/usPerformance`, которые текущий загрузчик не читает.

## Sources

- Данные таблиц карты.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Код игры (загрузчик типов объектов, бросок риска объекта, ежечасное обновление фабрики, список площадок посадки PMC, стратегическое обнаружение и обработка питания на объекте).
- Типы полей — это типы полей времени выполнения `FACILITYTYPE`, `FACILITYASSIGNMENTTYPE` и `PRODUCTION_LINE`, а также приведений в загрузчике.

## Open questions

- Загрузчик сохраняет `ubChance` в одном байте, поэтому задуманная шкала `0..1000` не может использоваться для значений выше `255`; поставляемое `500` задумано как 50% в этой шкале, но фактически сохраняется как `244`. Поставляемое значение `usChance` `700` не читается вообще. Были ли эти значения написаны для загрузчика, соблюдавшего полную шкалу, неизвестно.
- У `usChance` нет ветки в загрузчике и нет эффекта; является ли он остатком старой схемы или запланированной альтернативой, неизвестно.
- `usPerformance` внутри `CONDITIONS` пропускается загрузчиком; ожидалось ли, что поставляемые значения будут применяться, неизвестно.
