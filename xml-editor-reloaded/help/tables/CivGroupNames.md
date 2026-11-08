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

- **Type:** `Integer`
- **Description:** Index of the civilian group. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CIV_GROUP_NAMES/NAME/Enabled`

- **Type:** `Boolean`
- **Description:** If set, the civilian group is enabled. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CIV_GROUP_NAMES/NAME/szGroup`

- **Type:** `String`
- **Description:** Display name of the civilian group.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CIV_GROUP_NAMES/NAME/AddToBattle`

- **Type:** `Boolean`
- **Description:** If set, members of this group can join a battle. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CIV_GROUP_NAMES/NAME/Loyalty`

- **Type:** `Boolean`
- **Description:** If set, the group is treated as loyal to the player. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CIV_GROUP_NAMES/NAME/fCanBeCaptured`

- **Type:** `Boolean`
- **Description:** If set, members of this group can be captured. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

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
