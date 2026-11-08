---
schema: 1
lang: en
family: profiles-soldier-templates
title: "Soldier profiles - enemy and militia templates"
tables:
  - SoldierProfileEnemyAdmin
  - SoldierProfileEnemyElite
  - SoldierProfileEnemyRegular
  - SoldierProfileMilitiaGreen
  - SoldierProfileMilitiaRegular
  - SoldierProfileMilitiaVeteran
table_ids:
  - Profiles.SoldierProfileEnemyAdmin
  - Profiles.SoldierProfileEnemyElite
  - Profiles.SoldierProfileEnemyRegular
  - Profiles.SoldierProfileMilitiaGreen
  - Profiles.SoldierProfileMilitiaRegular
  - Profiles.SoldierProfileMilitiaVeteran
files:
  - Profiles/SoldierProfileEnemyAdmin.xml
  - Profiles/SoldierProfileEnemyElite.xml
  - Profiles/SoldierProfileEnemyRegular.xml
  - Profiles/SoldierProfileMilitiaGreen.xml
  - Profiles/SoldierProfileMilitiaRegular.xml
  - Profiles/SoldierProfileMilitiaVeteran.xml
root: PROFILES
record: PROFILE
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# Soldier profiles - enemy and militia templates

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`PROFILES/PROFILE/uiIndex`](#profilesprofileuiindex)
  - [`PROFILES/PROFILE/szName`](#profilesprofileszname)
  - [`PROFILES/PROFILE/uiBodyType`](#profilesprofileuibodytype)
  - [`PROFILES/PROFILE/uiHair`](#profilesprofileuihair)
  - [`PROFILES/PROFILE/uiSkin`](#profilesprofileuiskin)
  - [`PROFILES/PROFILE/uiTrait1`](#profilesprofileuitrait1)
  - [`PROFILES/PROFILE/uiTrait2`](#profilesprofileuitrait2)
  - [`PROFILES/PROFILE/uiTrait3`](#profilesprofileuitrait3)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

The six `SoldierProfile*` files define the visual and trait templates for enemy and militia ranks: body type, hair, skin and up to three skill traits. They use the same eight fields; only the rank-specific data differs.

## Tables

| Table | File | Notes |
|---|---|---|
| `SoldierProfileEnemyAdmin` | `Profiles/SoldierProfileEnemyAdmin.xml` | — |
| `SoldierProfileEnemyElite` | `Profiles/SoldierProfileEnemyElite.xml` | — |
| `SoldierProfileEnemyRegular` | `Profiles/SoldierProfileEnemyRegular.xml` | — |
| `SoldierProfileMilitiaGreen` | `Profiles/SoldierProfileMilitiaGreen.xml` | — |
| `SoldierProfileMilitiaRegular` | `Profiles/SoldierProfileMilitiaRegular.xml` | — |
| `SoldierProfileMilitiaVeteran` | `Profiles/SoldierProfileMilitiaVeteran.xml` | — |

## Structure

```
PROFILES
└── PROFILE
    ├── uiIndex
    ├── szName
    ├── uiBodyType
    ├── uiHair
    ├── uiSkin
    ├── uiTrait1
    ├── uiTrait2
    └── uiTrait3
```

## Fields

### `PROFILES/PROFILE/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Profile index within the file. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/szName`

- **Type:** `String`
- **Description:** Template/profile name used for identification and editor display.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiBodyType`

- **Type:** `Integer` (`UINT8`)
- **Description:** Body-type graphic ID used by this profile. It is a graphics reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiHair`

- **Type:** `Integer` (`UINT8`)
- **Description:** Hair graphic ID used by this profile. It is a graphics reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiSkin`

- **Type:** `Integer` (`UINT8`)
- **Description:** Skin graphic ID used by this profile. It is a graphics reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiTrait1`

- **Type:** `Integer` (`UINT8`)
- **Description:** First skill-trait ID for the template. It is an enum/reference; `0` means no trait.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiTrait2`

- **Type:** `Integer` (`UINT8`)
- **Description:** Second skill-trait ID for the template. It is an enum/reference; `0` means no trait.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiTrait3`

- **Type:** `Integer` (`UINT8`)
- **Description:** Third skill-trait ID for the template. It is an enum/reference; `0` means no trait.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

## Relations

- `uiIndex` is local to each template file. The files correspond to enemy admin/elite/regular and militia green/regular/veteran ranks. The visual and trait fields mirror the matching [MercProfiles](../MercProfiles.md) fields.

## Localization

Display names and labels are localized through language overlays; numeric IDs and codes are not.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Field names, XML comments and observed values.
- Descriptions are inferred from names and observed values; no runtime formula is claimed.

## Open questions

- The exact formula or enum mapping for inferred fields is not stated by the table data.
