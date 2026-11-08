---
schema: 1
lang: en
family: civ-group-names
title: "CivGroupNames - civilian group names and flags"
tables:
  - CivGroupNames
table_ids:
  - CivGroupNames
files:
  - CivGroupNames.xml
root: CIV_GROUP_NAMES
record: NAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# CivGroupNames - civilian group names and flags

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CIV_GROUP_NAMES/NAME/uiIndex`](#civ_group_namesnameuiindex)
  - [`CIV_GROUP_NAMES/NAME/Enabled`](#civ_group_namesnameenabled)
  - [`CIV_GROUP_NAMES/NAME/szGroup`](#civ_group_namesnameszgroup)
  - [`CIV_GROUP_NAMES/NAME/AddToBattle`](#civ_group_namesnameaddtobattle)
  - [`CIV_GROUP_NAMES/NAME/Loyalty`](#civ_group_namesnameloyalty)
  - [`CIV_GROUP_NAMES/NAME/fCanBeCaptured`](#civ_group_namesnamefcanbecaptured)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`CivGroupNames.xml` names civilian factions and controls whether they join battles, count as loyal, or can be captured.

## Tables

| Table | File | Notes |
|---|---|---|
| `CivGroupNames` | `CivGroupNames.xml` | — |

## Structure

```
CIV_GROUP_NAMES
└── NAME
    ├── uiIndex
    ├── Enabled
    ├── szGroup
    ├── AddToBattle
    ├── Loyalty
    └── fCanBeCaptured
```

## Fields

### `CIV_GROUP_NAMES/NAME/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index of the civilian group. It is an array key.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** Required; identifies the record.
- **Source:** game code (`XML_CivGroupNames.cpp`); table data.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/Enabled`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, civilians of this group display `szGroup` as their name in tactical view; `0` keeps the default civilian label. The effect is visible only when the `INDIVIDUAL_CIVILIAN_NAMES` game setting is enabled.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_CivGroupNames.cpp`, `Interface.cpp`); table XML comment.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/szGroup`

- **Type:** `String` (`CHAR16`)
- **Description:** Display name of the civilian group used when `Enabled` is set.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_CivGroupNames.cpp`); table data.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/AddToBattle`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, all members of this group are counted when a battle with that group occurs.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_CivGroupNames.cpp`, `Overhead.cpp`); table XML comment.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/Loyalty`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, killing a member of this group can cause a town-loyalty/morale penalty; `0` suppresses that penalty for the group.
- **Values:** `0` or `1`; `0` = no loyalty/morale penalty for killing group members.
- **Default:** Absent; the game default applies.
- **Source:** game code (`Strategic Town Loyalty.cpp`); table XML comment.
- **Confidence:** `source`

### `CIV_GROUP_NAMES/NAME/fCanBeCaptured`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, hostile members of this group can be captured and interrogated.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_CivGroupNames.cpp`, `Soldier Control.cpp`); table XML comment.
- **Confidence:** `source`

## Relations

- `uiIndex` is referenced by the civilian-group field in [MercProfiles](MercProfiles.md).

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
