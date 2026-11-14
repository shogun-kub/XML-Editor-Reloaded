---
schema: 1
lang: en
family: mercs-availability
title: "AIMAvailability - AIM availability list"
tables:
  - AIMAvailability
table_ids:
  - AIMAvailability
files:
  - AIMAvailability.xml
root: AIM_AVAILABLES
record: AIM
key: uiIndex
sources:
  - AIM availability table data
status: reviewed
updated: 2026-11-07
---

# AIMAvailability - AIM availability list

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`AIM_AVAILABLES/AIM/uiIndex`](#aim_availablesaimuiindex)
  - [`AIM_AVAILABLES/AIM/description`](#aim_availablesaimdescription)
  - [`AIM_AVAILABLES/AIM/ProfilId`](#aim_availablesaimprofilid)
  - [`AIM_AVAILABLES/AIM/AimBioID`](#aim_availablesaimaimbioid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AIMAvailability.xml` links an AIM profile to its biography entry and the description shown in the AIM availability list.

## Tables

| Table | File | Notes |
|---|---|---|
| `AIMAvailability` | `AIMAvailability.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
AIM_AVAILABLES
└── AIM
    ├── uiIndex
    ├── description
    ├── ProfilId
    └── AimBioID
```

## Fields

### `AIM_AVAILABLES/AIM/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** AIM availability row identity.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** aim availability table data; item/profile metadata.
- **Confidence:** `source`

### `AIM_AVAILABLES/AIM/description`

- **Type:** `String`
- **Description:** Description text stored in the table, but the loader does not read this element; it is ignored.
- **Values:** Free text; ignored at load time.
- **Default:** Absent; the game default applies.
- **Source:** game code (AIM availability loader); table data.
- **Confidence:** `source`
### `AIM_AVAILABLES/AIM/ProfilId`

- **Type:** `Integer` (`UINT8`)
- **Description:** Profile ID of the AIM mercenary.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** aim availability table data; item/profile metadata.
- **Confidence:** `source`

### `AIM_AVAILABLES/AIM/AimBioID`

- **Type:** `Integer` (`UINT8`)
- **Description:** Biography ID for the AIM site.
- **Values:** Integer; no narrower range is declared by the field metadata.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** aim availability table data; item/profile metadata.
- **Confidence:** `source`

## Relations

- `ProfilId` points to [MercProfiles](MercProfiles.md).

## Localization

`description` is display text for the AIM list.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- AIM availability table data.
- Field types: XML element names and profile/table metadata.
- Descriptions use the table schema comments where present; no code-verified formulas are claimed.

## Open questions

The AIM biography and description are display data; the hiring rules are outside this table.
