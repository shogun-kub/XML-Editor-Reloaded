---
schema: 1
lang: ru
family: backgrounds
title: "Backgrounds — предыстории персонажей"
tables:
  - Backgrounds
table_ids:
  - Backgrounds
files:
  - Backgrounds.xml
root: BACKGROUNDS
record: BACKGROUND
key: uiIndex
sources:
  - Background table data
  - Background schema comments
status: reviewed
updated: 2026-11-12
---

# Backgrounds — предыстории персонажей

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`BACKGROUNDS/BACKGROUND/uiIndex`](#backgroundsbackgrounduiindex)
  - [`BACKGROUNDS/BACKGROUND/szName`](#backgroundsbackgroundszname)
  - [`BACKGROUNDS/BACKGROUND/szShortName`](#backgroundsbackgroundszshortname)
  - [`BACKGROUNDS/BACKGROUND/szDescription`](#backgroundsbackgroundszdescription)
  - [`BACKGROUNDS/BACKGROUND/ap_polar`](#backgroundsbackgroundap_polar)
  - [`BACKGROUNDS/BACKGROUND/ap_desert`](#backgroundsbackgroundap_desert)
  - [`BACKGROUNDS/BACKGROUND/ap_swamp`](#backgroundsbackgroundap_swamp)
  - [`BACKGROUNDS/BACKGROUND/ap_urban`](#backgroundsbackgroundap_urban)
  - [`BACKGROUNDS/BACKGROUND/ap_forest`](#backgroundsbackgroundap_forest)
  - [`BACKGROUNDS/BACKGROUND/ap_plains`](#backgroundsbackgroundap_plains)
  - [`BACKGROUNDS/BACKGROUND/ap_river`](#backgroundsbackgroundap_river)
  - [`BACKGROUNDS/BACKGROUND/ap_coastal`](#backgroundsbackgroundap_coastal)
  - [`BACKGROUNDS/BACKGROUND/ap_tropical`](#backgroundsbackgroundap_tropical)
  - [`BACKGROUNDS/BACKGROUND/ap_mountain`](#backgroundsbackgroundap_mountain)
  - [`BACKGROUNDS/BACKGROUND/agility`](#backgroundsbackgroundagility)
  - [`BACKGROUNDS/BACKGROUND/strength`](#backgroundsbackgroundstrength)
  - [`BACKGROUNDS/BACKGROUND/dexterity`](#backgroundsbackgrounddexterity)
  - [`BACKGROUNDS/BACKGROUND/mechanical`](#backgroundsbackgroundmechanical)
  - [`BACKGROUNDS/BACKGROUND/medical`](#backgroundsbackgroundmedical)
  - [`BACKGROUNDS/BACKGROUND/wisdom`](#backgroundsbackgroundwisdom)
  - [`BACKGROUNDS/BACKGROUND/explosives`](#backgroundsbackgroundexplosives)
  - [`BACKGROUNDS/BACKGROUND/leadership`](#backgroundsbackgroundleadership)
  - [`BACKGROUNDS/BACKGROUND/marksmanship`](#backgroundsbackgroundmarksmanship)
  - [`BACKGROUNDS/BACKGROUND/ap_height`](#backgroundsbackgroundap_height)
  - [`BACKGROUNDS/BACKGROUND/ap_swimming`](#backgroundsbackgroundap_swimming)
  - [`BACKGROUNDS/BACKGROUND/ap_fortify`](#backgroundsbackgroundap_fortify)
  - [`BACKGROUNDS/BACKGROUND/ap_artillery`](#backgroundsbackgroundap_artillery)
  - [`BACKGROUNDS/BACKGROUND/ap_inventory`](#backgroundsbackgroundap_inventory)
  - [`BACKGROUNDS/BACKGROUND/ap_airdrop`](#backgroundsbackgroundap_airdrop)
  - [`BACKGROUNDS/BACKGROUND/ap_assault`](#backgroundsbackgroundap_assault)
  - [`BACKGROUNDS/BACKGROUND/travel_foot`](#backgroundsbackgroundtravel_foot)
  - [`BACKGROUNDS/BACKGROUND/travel_car`](#backgroundsbackgroundtravel_car)
  - [`BACKGROUNDS/BACKGROUND/travel_air`](#backgroundsbackgroundtravel_air)
  - [`BACKGROUNDS/BACKGROUND/travel_boat`](#backgroundsbackgroundtravel_boat)
  - [`BACKGROUNDS/BACKGROUND/resistance_fear`](#backgroundsbackgroundresistance_fear)
  - [`BACKGROUNDS/BACKGROUND/resistance_suppression`](#backgroundsbackgroundresistance_suppression)
  - [`BACKGROUNDS/BACKGROUND/resistance_physical`](#backgroundsbackgroundresistance_physical)
  - [`BACKGROUNDS/BACKGROUND/resistance_alcohol`](#backgroundsbackgroundresistance_alcohol)
  - [`BACKGROUNDS/BACKGROUND/resistance_disease`](#backgroundsbackgroundresistance_disease)
  - [`BACKGROUNDS/BACKGROUND/interrogation`](#backgroundsbackgroundinterrogation)
  - [`BACKGROUNDS/BACKGROUND/prisonguard`](#backgroundsbackgroundprisonguard)
  - [`BACKGROUNDS/BACKGROUND/betterprices_guns`](#backgroundsbackgroundbetterprices_guns)
  - [`BACKGROUNDS/BACKGROUND/betterprices`](#backgroundsbackgroundbetterprices)
  - [`BACKGROUNDS/BACKGROUND/capitulation`](#backgroundsbackgroundcapitulation)
  - [`BACKGROUNDS/BACKGROUND/speed_run`](#backgroundsbackgroundspeed_run)
  - [`BACKGROUNDS/BACKGROUND/speed_bandaging`](#backgroundsbackgroundspeed_bandaging)
  - [`BACKGROUNDS/BACKGROUND/drink_energyregen`](#backgroundsbackgrounddrink_energyregen)
  - [`BACKGROUNDS/BACKGROUND/carrystrength`](#backgroundsbackgroundcarrystrength)
  - [`BACKGROUNDS/BACKGROUND/food`](#backgroundsbackgroundfood)
  - [`BACKGROUNDS/BACKGROUND/water`](#backgroundsbackgroundwater)
  - [`BACKGROUNDS/BACKGROUND/sleep`](#backgroundsbackgroundsleep)
  - [`BACKGROUNDS/BACKGROUND/meleedamage`](#backgroundsbackgroundmeleedamage)
  - [`BACKGROUNDS/BACKGROUND/cth_blades`](#backgroundsbackgroundcth_blades)
  - [`BACKGROUNDS/BACKGROUND/camo`](#backgroundsbackgroundcamo)
  - [`BACKGROUNDS/BACKGROUND/stealth`](#backgroundsbackgroundstealth)
  - [`BACKGROUNDS/BACKGROUND/increased_maxcth`](#backgroundsbackgroundincreased_maxcth)
  - [`BACKGROUNDS/BACKGROUND/hearing_night`](#backgroundsbackgroundhearing_night)
  - [`BACKGROUNDS/BACKGROUND/hearing_day`](#backgroundsbackgroundhearing_day)
  - [`BACKGROUNDS/BACKGROUND/disarm_trap`](#backgroundsbackgrounddisarm_trap)
  - [`BACKGROUNDS/BACKGROUND/SAM_cth`](#backgroundsbackgroundsam_cth)
  - [`BACKGROUNDS/BACKGROUND/approach_friendly`](#backgroundsbackgroundapproach_friendly)
  - [`BACKGROUNDS/BACKGROUND/approach_direct`](#backgroundsbackgroundapproach_direct)
  - [`BACKGROUNDS/BACKGROUND/approach_threaten`](#backgroundsbackgroundapproach_threaten)
  - [`BACKGROUNDS/BACKGROUND/approach_recruit`](#backgroundsbackgroundapproach_recruit)
  - [`BACKGROUNDS/BACKGROUND/breachingcharge`](#backgroundsbackgroundbreachingcharge)
  - [`BACKGROUNDS/BACKGROUND/cth_vs_creatures`](#backgroundsbackgroundcth_vs_creatures)
  - [`BACKGROUNDS/BACKGROUND/insurance`](#backgroundsbackgroundinsurance)
  - [`BACKGROUNDS/BACKGROUND/spotter`](#backgroundsbackgroundspotter)
  - [`BACKGROUNDS/BACKGROUND/disease_diagnose`](#backgroundsbackgrounddisease_diagnose)
  - [`BACKGROUNDS/BACKGROUND/disease_treatment`](#backgroundsbackgrounddisease_treatment)
  - [`BACKGROUNDS/BACKGROUND/tracker_ability`](#backgroundsbackgroundtracker_ability)
  - [`BACKGROUNDS/BACKGROUND/ambush_radius`](#backgroundsbackgroundambush_radius)
  - [`BACKGROUNDS/BACKGROUND/smoker`](#backgroundsbackgroundsmoker)
  - [`BACKGROUNDS/BACKGROUND/croucheddefense`](#backgroundsbackgroundcroucheddefense)
  - [`BACKGROUNDS/BACKGROUND/fortify_assignment`](#backgroundsbackgroundfortify_assignment)
  - [`BACKGROUNDS/BACKGROUND/snake_defense`](#backgroundsbackgroundsnake_defense)
  - [`BACKGROUNDS/BACKGROUND/hackerskill`](#backgroundsbackgroundhackerskill)
  - [`BACKGROUNDS/BACKGROUND/burial_assignment`](#backgroundsbackgroundburial_assignment)
  - [`BACKGROUNDS/BACKGROUND/administration_assignment`](#backgroundsbackgroundadministration_assignment)
  - [`BACKGROUNDS/BACKGROUND/exploration_assignment`](#backgroundsbackgroundexploration_assignment)
  - [`BACKGROUNDS/BACKGROUND/dislikebackground`](#backgroundsbackgrounddislikebackground)
  - [`BACKGROUNDS/BACKGROUND/druguse`](#backgroundsbackgrounddruguse)
  - [`BACKGROUNDS/BACKGROUND/drugtypes/drugtype`](#backgroundsbackgrounddrugtypesdrugtype)
  - [`BACKGROUNDS/BACKGROUND/drugitems/drugitem`](#backgroundsbackgrounddrugitemsdrugitem)
  - [`BACKGROUNDS/BACKGROUND/xenophobic`](#backgroundsbackgroundxenophobic)
  - [`BACKGROUNDS/BACKGROUND/level_underground`](#backgroundsbackgroundlevel_underground)
  - [`BACKGROUNDS/BACKGROUND/scrounging`](#backgroundsbackgroundscrounging)
  - [`BACKGROUNDS/BACKGROUND/traplevel`](#backgroundsbackgroundtraplevel)
  - [`BACKGROUNDS/BACKGROUND/corruptionspread`](#backgroundsbackgroundcorruptionspread)
  - [`BACKGROUNDS/BACKGROUND/no_male`](#backgroundsbackgroundno_male)
  - [`BACKGROUNDS/BACKGROUND/no_female`](#backgroundsbackgroundno_female)
  - [`BACKGROUNDS/BACKGROUND/loyalitylossondeath`](#backgroundsbackgroundloyalitylossondeath)
  - [`BACKGROUNDS/BACKGROUND/animal_friend`](#backgroundsbackgroundanimal_friend)
  - [`BACKGROUNDS/BACKGROUND/civgroup_loyal`](#backgroundsbackgroundcivgroup_loyal)
  - [`BACKGROUNDS/BACKGROUND/alt_impcreation`](#backgroundsbackgroundalt_impcreation)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Backgrounds.xml` задаёт необязательные предыстории персонажей: бонусы к навыкам и характеристикам, модификаторы заданий, модификаторы среды, сопротивления и правила допустимости предысторий. Какие строки предлагаются, решают система предысторий и параметры создания IMP.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Backgrounds` | `Backgrounds.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
BACKGROUNDS
└── BACKGROUND
    ├── uiIndex
    ├── szName
    ├── szShortName
    ├── szDescription
    ├── ap_polar
    ├── ap_desert
    ├── ap_swamp
    ├── ap_urban
    ├── ap_forest
    ├── ap_plains
    ├── ap_river
    ├── ap_coastal
    ├── ap_tropical
    ├── ap_mountain
    ├── agility
    ├── strength
    ├── dexterity
    ├── mechanical
    ├── medical
    ├── wisdom
    ├── explosives
    ├── leadership
    ├── marksmanship
    ├── ap_height
    ├── ap_swimming
    ├── ap_fortify
    ├── ap_artillery
    ├── ap_inventory
    ├── ap_airdrop
    ├── ap_assault
    ├── travel_foot
    ├── travel_car
    ├── travel_air
    ├── travel_boat
    ├── resistance_fear
    ├── resistance_suppression
    ├── resistance_physical
    ├── resistance_alcohol
    ├── resistance_disease
    ├── interrogation
    ├── prisonguard
    ├── betterprices_guns
    ├── betterprices
    ├── capitulation
    ├── speed_run
    ├── speed_bandaging
    ├── drink_energyregen
    ├── carrystrength
    ├── food
    ├── water
    ├── sleep
    ├── meleedamage
    ├── cth_blades
    ├── camo
    ├── stealth
    ├── increased_maxcth
    ├── hearing_night
    ├── hearing_day
    ├── disarm_trap
    ├── SAM_cth
    ├── approach_friendly
    ├── approach_direct
    ├── approach_threaten
    ├── approach_recruit
    ├── breachingcharge
    ├── cth_vs_creatures
    ├── insurance
    ├── spotter
    ├── disease_diagnose
    ├── disease_treatment
    ├── tracker_ability
    ├── ambush_radius
    ├── smoker
    ├── croucheddefense
    ├── fortify_assignment
    ├── snake_defense
    ├── hackerskill
    ├── burial_assignment
    ├── administration_assignment
    ├── exploration_assignment
    ├── dislikebackground
    ├── druguse
    ├── drugtypes
    │   └── drugtype
    ├── drugitems
    │   └── drugitem
    ├── xenophobic
    ├── level_underground
    ├── scrounging
    ├── traplevel
    ├── corruptionspread
    ├── no_male
    ├── no_female
    ├── loyalitylossondeath
    ├── animal_friend
    ├── civgroup_loyal
    └── alt_impcreation
```

## Fields

### `BACKGROUNDS/BACKGROUND/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Идентификатор строки предыстории.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/szName`

- **Type:** `String` (`String`)
- **Description:** Отображаемое название предыстории.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/szShortName`

- **Type:** `String` (`String`)
- **Description:** Короткое отображаемое название предыстории.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/szDescription`

- **Type:** `String` (`String`)
- **Description:** Описание предыстории, показываемое игроку.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Свободный текст.
- **Default:** Пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_polar`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «полярная среда».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_desert`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «пустыня».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_swamp`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «болото».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_urban`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «город».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_forest`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «лес».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_plains`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «равнины».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_river`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «река».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_coastal`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «побережье».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_tropical`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «тропики».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_mountain`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «горы».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/agility`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус или штраф к ловкости.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/strength`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус или штраф к силе.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/dexterity`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус или штраф к проворству.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/mechanical`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус или штраф к навыку «Техника».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/medical`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус или штраф к навыку «Медицина».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/wisdom`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус или штраф к мудрости.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/explosives`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус или штраф к навыку «Взрывное дело».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/leadership`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус или штраф к лидерству.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/marksmanship`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус или штраф к меткости.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_height`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «высота».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_swimming`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «плавание».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_fortify`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «укрепление».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_artillery`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «артиллерия».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_inventory`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «инвентарь».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_airdrop`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «воздушный десант».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_assault`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор очков действия для среды/действия «штурм».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/travel_foot`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор путешествия пешком.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/travel_car`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор путешествия на автомобиле.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/travel_air`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор путешествия по воздуху.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/travel_boat`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор путешествия на лодке.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_fear`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор сопротивления страху.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Большие значения означают более сильное снижение/сопротивление; отрицательные значения ослабляют эту защиту и не имеют смысла, если поле явно не задаёт штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_suppression`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор сопротивления подавлению.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Большие значения означают более сильное снижение/сопротивление; отрицательные значения ослабляют эту защиту и не имеют смысла, если поле явно не задаёт штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_physical`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор сопротивления физическому воздействию.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Большие значения означают более сильное снижение/сопротивление; отрицательные значения ослабляют эту защиту и не имеют смысла, если поле явно не задаёт штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_alcohol`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор сопротивления алкоголю.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Большие значения означают более сильное снижение/сопротивление; отрицательные значения ослабляют эту защиту и не имеют смысла, если поле явно не задаёт штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_disease`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор сопротивления болезням.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Большие значения означают более сильное снижение/сопротивление; отрицательные значения ослабляют эту защиту и не имеют смысла, если поле явно не задаёт штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/interrogation`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор допроса.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/prisonguard`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор тюремного надзирателя.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/betterprices_guns`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор цен в оружейном магазине.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/betterprices`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор цен в магазине.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/capitulation`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор шанса капитуляции противника.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/speed_run`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор скорости бега.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Большие значения означают более высокую скорость; отрицательные значения не имеют смысла.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/speed_bandaging`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор скорости перевязки.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Большие значения означают более высокую скорость; отрицательные значения не имеют смысла.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/drink_energyregen`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор восстановления энергии от напитков.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/carrystrength`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор силы переноски.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/food`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор потребления еды.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/water`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор потребления воды.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/sleep`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор сна.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/meleedamage`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор урона в ближнем бою.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/cth_blades`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор шанса попадания при использовании клинкового оружия.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/camo`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор маскировки.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/stealth`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор скрытности.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/increased_maxcth`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор максимального шанса попадания.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/hearing_night`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор слуха ночью.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/hearing_day`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор слуха днём.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/disarm_trap`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор обезвреживания ловушек.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/SAM_cth`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор шанса попадания объекта SAM.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/approach_friendly`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор дружелюбного диалога при подходе.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/approach_direct`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор прямого диалога при подходе.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/approach_threaten`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор диалога с угрозами при подходе.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/approach_recruit`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор диалога найма при подходе.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/breachingcharge`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор эффективности пробивного заряда.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/cth_vs_creatures`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор шанса попадания по существам.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/insurance`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор стоимости или доступности страховки.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/spotter`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор корректировщика.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/disease_diagnose`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор диагностики болезней.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/disease_treatment`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор лечения болезней.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/tracker_ability`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор способности следопыта.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ambush_radius`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор радиуса обнаружения засады.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Большие значения означают большую дальность/радиус, отрицательные значения не имеют смысла.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/smoker`

- **Type:** `Integer` (`INT16`)
- **Description:** Флаг или модификатор курильщика.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/croucheddefense`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор обороны в приседе.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/fortify_assignment`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор эффективности задания «укрепление».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/snake_defense`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор защиты от змей.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/hackerskill`

- **Type:** `Integer` (`INT16`)
- **Description:** Бонус к навыку взлома.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/burial_assignment`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор эффективности задания «захоронение».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/administration_assignment`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор эффективности задания «администрирование».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/exploration_assignment`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор эффективности задания «разведка».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/dislikebackground`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предыстории, которую не любит эта предыстория.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/druguse`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор употребления наркотиков.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/drugtypes/drugtype`

- **Type:** `Integer` (`INT16`)
- **Description:** ID типа наркотика, связанного с предысторией.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Индекс строки наркотика из [Drugs](Items/Drugs.md).
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/drugitems/drugitem`

- **Type:** `Integer` (`INT16`)
- **Description:** ID предмета-наркотика, связанного с предысторией.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** ID предмета из [Items](Items/Items.md).
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/xenophobic`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор ксенофобии.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/level_underground`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор подземного уровня.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Большие значения означают более высокий уровень; отрицательные выходят за обычный диапазон уровней.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/scrounging`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор поиска припасов.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/traplevel`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор уровня ловушек.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/corruptionspread`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор распространения коррупции.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/no_male`

- **Type:** `Integer` (`INT16`)
- **Description:** Исключает мужских персонажей из этой предыстории.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/no_female`

- **Type:** `Integer` (`INT16`)
- **Description:** Исключает женских персонажей из этой предыстории.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/loyalitylossondeath`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор потери лояльности при смерти.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/animal_friend`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор взаимодействия «друг животных».
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/civgroup_loyal`

- **Type:** `Integer` (`INT16`)
- **Description:** Модификатор лояльности группы гражданских.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Знаковый модификатор предыстории: большие значения означают более сильный положительный бонус, отрицательные — штраф.
- **Values:** Целое число-модификатор; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/alt_impcreation`

- **Type:** `Integer` (`INT16`)
- **Description:** Флаг альтернативного создания IMP для предыстории.
- **Only used with:** ENABLE_BACKGROUNDS, ALTERNATIVE_IMP_CREATION.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

## Relations

- `usBackground` в [MercProfiles](MercProfiles.md) указывает на `Backgrounds.uiIndex`.
- `drugitem` указывает на [Items](Items/Items.md); `drugtype` — на [Drugs](Items/Drugs.md).
- `dislikebackground` указывает на другой `Backgrounds.uiIndex`.

## Localization

`szName`, `szShortName` и `szDescription` — отображаемый текст; числовые модификаторы не локализуются.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы предысторий.
- Комментарии схемы предысторий.
- Типы полей: данные таблицы и имена полей; значения перечислений взяты из комментариев схемы.
- Описания осторожны там, где формула не указана; наблюдаемые диапазоны значений не используются.

## Open questions

Точное суммирование и округление множества модификаторов предысторий определяются кодом предысторий и заданий, а не этой таблицей.

Какой элемент интерфейса использует `szShortName` вместо полного названия предыстории, не подтверждено; это требует проверки кода и интерфейса, и до тех пор нейтральные формулировки выше выбраны намеренно.
