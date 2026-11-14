---
schema: 1
lang: en
family: backgrounds
title: "Backgrounds - character backgrounds"
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
updated: 2026-11-07
---

# Backgrounds - character backgrounds

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

`Backgrounds.xml` defines optional character backgrounds: skill and stat bonuses, assignment modifiers, environmental modifiers, resistances and background eligibility rules. The background system and IMP-creation options decide which rows are offered.

## Tables

| Table | File | Notes |
|---|---|---|
| `Backgrounds` | `Backgrounds.xml` | v4 and v5 have the same field paths; values differ. |

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
- **Description:** Background row identity.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/szName`

- **Type:** `String` (`String`)
- **Description:** Display name of the background.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/szShortName`

- **Type:** `String` (`String`)
- **Description:** Short display name for the background.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/szDescription`

- **Type:** `String` (`String`)
- **Description:** Player-facing background description.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Free text.
- **Default:** empty string when omitted.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_polar`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the polar environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_desert`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the desert environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_swamp`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the swamp environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_urban`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the urban environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_forest`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the forest environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_plains`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the plains environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_river`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the river environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_coastal`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the coastal environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_tropical`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the tropical environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_mountain`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the mountain environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/agility`

- **Type:** `Integer` (`INT16`)
- **Description:** Agility bonus or penalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/strength`

- **Type:** `Integer` (`INT16`)
- **Description:** Strength bonus or penalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/dexterity`

- **Type:** `Integer` (`INT16`)
- **Description:** Dexterity bonus or penalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/mechanical`

- **Type:** `Integer` (`INT16`)
- **Description:** Mechanical skill bonus or penalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/medical`

- **Type:** `Integer` (`INT16`)
- **Description:** Medical skill bonus or penalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/wisdom`

- **Type:** `Integer` (`INT16`)
- **Description:** Wisdom bonus or penalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/explosives`

- **Type:** `Integer` (`INT16`)
- **Description:** Explosives skill bonus or penalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/leadership`

- **Type:** `Integer` (`INT16`)
- **Description:** Leadership bonus or penalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/marksmanship`

- **Type:** `Integer` (`INT16`)
- **Description:** Marksmanship bonus or penalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_height`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the height environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_swimming`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the swimming environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_fortify`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the fortify environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_artillery`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the artillery environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_inventory`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the inventory environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_airdrop`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the airdrop environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ap_assault`

- **Type:** `Integer` (`INT16`)
- **Description:** Action-point modifier for the assault environment/action.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/travel_foot`

- **Type:** `Integer` (`INT16`)
- **Description:** Travel modifier for foot travel.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/travel_car`

- **Type:** `Integer` (`INT16`)
- **Description:** Travel modifier for car travel.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/travel_air`

- **Type:** `Integer` (`INT16`)
- **Description:** Travel modifier for air travel.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/travel_boat`

- **Type:** `Integer` (`INT16`)
- **Description:** Travel modifier for boat travel.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_fear`

- **Type:** `Integer` (`INT16`)
- **Description:** Resistance modifier against fear.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Higher values mean a stronger reduction/resistance; negative values reduce that protection and are not meaningful unless the field explicitly defines a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_suppression`

- **Type:** `Integer` (`INT16`)
- **Description:** Resistance modifier against suppression.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Higher values mean a stronger reduction/resistance; negative values reduce that protection and are not meaningful unless the field explicitly defines a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_physical`

- **Type:** `Integer` (`INT16`)
- **Description:** Resistance modifier against physical.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Higher values mean a stronger reduction/resistance; negative values reduce that protection and are not meaningful unless the field explicitly defines a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_alcohol`

- **Type:** `Integer` (`INT16`)
- **Description:** Resistance modifier against alcohol.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Higher values mean a stronger reduction/resistance; negative values reduce that protection and are not meaningful unless the field explicitly defines a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/resistance_disease`

- **Type:** `Integer` (`INT16`)
- **Description:** Resistance modifier against disease.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Higher values mean a stronger reduction/resistance; negative values reduce that protection and are not meaningful unless the field explicitly defines a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/interrogation`

- **Type:** `Integer` (`INT16`)
- **Description:** Interrogation modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/prisonguard`

- **Type:** `Integer` (`INT16`)
- **Description:** Prison-guard modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/betterprices_guns`

- **Type:** `Integer` (`INT16`)
- **Description:** Gun-shop price modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/betterprices`

- **Type:** `Integer` (`INT16`)
- **Description:** Shop price modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/capitulation`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to enemy capitulation chance.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/speed_run`

- **Type:** `Integer` (`INT16`)
- **Description:** Running-speed modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Higher values mean a faster speed; negative values are not meaningful.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/speed_bandaging`

- **Type:** `Integer` (`INT16`)
- **Description:** Bandaging-speed modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Higher values mean a faster speed; negative values are not meaningful.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/drink_energyregen`

- **Type:** `Integer` (`INT16`)
- **Description:** Drink energy-regeneration modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/carrystrength`

- **Type:** `Integer` (`INT16`)
- **Description:** Carry-strength modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/food`

- **Type:** `Integer` (`INT16`)
- **Description:** Food-consumption modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/water`

- **Type:** `Integer` (`INT16`)
- **Description:** Water-consumption modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/sleep`

- **Type:** `Integer` (`INT16`)
- **Description:** Sleep modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/meleedamage`

- **Type:** `Integer` (`INT16`)
- **Description:** Melee damage modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/cth_blades`

- **Type:** `Integer` (`INT16`)
- **Description:** Chance-to-hit modifier with blade weapons.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/camo`

- **Type:** `Integer` (`INT16`)
- **Description:** Camouflage modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/stealth`

- **Type:** `Integer` (`INT16`)
- **Description:** Stealth modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/increased_maxcth`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to maximum chance to hit.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/hearing_night`

- **Type:** `Integer` (`INT16`)
- **Description:** Night-time hearing modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/hearing_day`

- **Type:** `Integer` (`INT16`)
- **Description:** Daytime hearing modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/disarm_trap`

- **Type:** `Integer` (`INT16`)
- **Description:** Trap-disarming modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/SAM_cth`

- **Type:** `Integer` (`INT16`)
- **Description:** SAM-site chance-to-hit modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/approach_friendly`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to friendly approach dialogue.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/approach_direct`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to direct approach dialogue.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/approach_threaten`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to threatening approach dialogue.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/approach_recruit`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to recruitment approach dialogue.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/breachingcharge`

- **Type:** `Integer` (`INT16`)
- **Description:** Breaching-charge effectiveness modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/cth_vs_creatures`

- **Type:** `Integer` (`INT16`)
- **Description:** Chance-to-hit modifier against creatures.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/insurance`

- **Type:** `Integer` (`INT16`)
- **Description:** Insurance cost or availability modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/spotter`

- **Type:** `Integer` (`INT16`)
- **Description:** Spotter modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/disease_diagnose`

- **Type:** `Integer` (`INT16`)
- **Description:** Disease-diagnosis modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/disease_treatment`

- **Type:** `Integer` (`INT16`)
- **Description:** Disease-treatment modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/tracker_ability`

- **Type:** `Integer` (`INT16`)
- **Description:** Tracker-ability modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/ambush_radius`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to ambush detection radius.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Higher values mean a longer range/radius, and negative values are not meaningful.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/smoker`

- **Type:** `Integer` (`INT16`)
- **Description:** Smoker flag or modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/croucheddefense`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to crouched defence.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/fortify_assignment`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to fortify-assignment effectiveness.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/snake_defense`

- **Type:** `Integer` (`INT16`)
- **Description:** Snake-defence modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/hackerskill`

- **Type:** `Integer` (`INT16`)
- **Description:** Hacking skill bonus.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/burial_assignment`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to burial-assignment effectiveness.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/administration_assignment`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to administration-assignment effectiveness.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/exploration_assignment`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to exploration-assignment effectiveness.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/dislikebackground`

- **Type:** `Integer` (`INT16`)
- **Description:** Background ID that this background dislikes.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/druguse`

- **Type:** `Integer` (`INT16`)
- **Description:** Drug-use modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/drugtypes/drugtype`

- **Type:** `Integer` (`INT16`)
- **Description:** Drug type ID associated with the background.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Drug row index from [Drugs](Items/Drugs.md).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/drugitems/drugitem`

- **Type:** `Integer` (`INT16`)
- **Description:** Drug item ID associated with the background.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** Item ID from [Items](Items/Items.md).
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/xenophobic`

- **Type:** `Integer` (`INT16`)
- **Description:** Xenophobia modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/level_underground`

- **Type:** `Integer` (`INT16`)
- **Description:** Underground-level modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Higher values mean a higher level; negative values are outside the normal level range.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/scrounging`

- **Type:** `Integer` (`INT16`)
- **Description:** Scrounging modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/traplevel`

- **Type:** `Integer` (`INT16`)
- **Description:** Trap-level modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/corruptionspread`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to corruption spread.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/no_male`

- **Type:** `Integer` (`INT16`)
- **Description:** Excludes male characters from this background.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/no_female`

- **Type:** `Integer` (`INT16`)
- **Description:** Excludes female characters from this background.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/loyalitylossondeath`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to loyalty loss on death.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/animal_friend`

- **Type:** `Integer` (`INT16`)
- **Description:** Animal-friend interaction modifier.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/civgroup_loyal`

- **Type:** `Integer` (`INT16`)
- **Description:** Modifier to civilian-group loyalty.
- **Only used with:** ENABLE_BACKGROUNDS.
- **Numeric semantics:** Signed background modifier: higher values mean a stronger positive bonus, and negative values mean a penalty.
- **Values:** Integer modifier; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

### `BACKGROUNDS/BACKGROUND/alt_impcreation`

- **Type:** `Integer` (`INT16`)
- **Description:** Alternative IMP-creation flag for the background.
- **Only used with:** ENABLE_BACKGROUNDS, ALTERNATIVE_IMP_CREATION.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** game code (`XML_Background.cpp`); table data.
- **Confidence:** `source`

## Relations

- `usBackground` in [MercProfiles](MercProfiles.md) points to `Backgrounds.uiIndex`.
- `drugitem` points to [Items](Items/Items.md); `drugtype` points to [Drugs](Items/Drugs.md).
- `dislikebackground` points to another `Backgrounds.uiIndex`.

## Localization

`szName`, `szShortName` and `szDescription` are display text; numeric modifiers are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Background table data.
- Background schema comments.
- Field types: table data and field names; enum values come from the schema comments.
- Descriptions are conservative where no formula is stated; no observed value ranges are used.

## Open questions

The exact stacking and rounding of the many background modifiers are defined by the background/assignment code, not by this table.

Which UI surface uses `szShortName` instead of the full background name is not confirmed; this needs a code/UI check, and the neutral wording above is intentional until then.
