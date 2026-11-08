---
schema: 1
lang: en
family: army-patrol-groups
title: "PatrolGroups - army patrol routes"
tables:
  - PatrolGroups
table_ids:
  - Army.PatrolGroups
files:
  - Army/PatrolGroups.xml
root: PATROL_INFO
record: PATROL
key: none
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# PatrolGroups - army patrol routes

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`PATROL_INFO/PATROL/Priority`](#patrol_infopatrolpriority)
  - [`PATROL_INFO/PATROL/Size`](#patrol_infopatrolsize)
  - [`PATROL_INFO/PATROL/Waypoints/Sector1`](#patrol_infopatrolwaypointssector1)
  - [`PATROL_INFO/PATROL/Waypoints/Sector2`](#patrol_infopatrolwaypointssector2)
  - [`PATROL_INFO/PATROL/Waypoints/Sector3`](#patrol_infopatrolwaypointssector3)
  - [`PATROL_INFO/PATROL/Waypoints/Sector4`](#patrol_infopatrolwaypointssector4)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`PatrolGroups.xml` defines army patrol groups by priority, size and up to four waypoint sectors. A patrol follows its listed waypoints in order; unused waypoints are left empty or set to `0`.

## Tables

| Table | File | Notes |
|---|---|---|
| `PatrolGroups` | `Army/PatrolGroups.xml` | — |

## Structure

```
PATROL_INFO
└── PATROL
    ├── Priority
    ├── Size
    └── Waypoints
        ├── Sector1
        ├── Sector2
        ├── Sector3
        └── Sector4
```

## Fields

### `PATROL_INFO/PATROL/Priority`

- **Type:** `Integer`
- **Description:** Patrol priority.
- **Numeric semantics:** higher = more important; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `PATROL_INFO/PATROL/Size`

- **Type:** `Integer`
- **Description:** Number of soldiers in the patrol.
- **Numeric semantics:** higher = larger patrol; negative values are not meaningful.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `PATROL_INFO/PATROL/Waypoints/Sector1`

- **Type:** `String`
- **Description:** First patrol waypoint sector. It is a map coordinate; an empty value or `0` means the waypoint is unused.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `PATROL_INFO/PATROL/Waypoints/Sector2`

- **Type:** `String`
- **Description:** Second patrol waypoint sector. It is a map coordinate; an empty value or `0` means the waypoint is unused.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `PATROL_INFO/PATROL/Waypoints/Sector3`

- **Type:** `String`
- **Description:** Third patrol waypoint sector. It is a map coordinate; an empty value or `0` means the waypoint is unused.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `PATROL_INFO/PATROL/Waypoints/Sector4`

- **Type:** `String`
- **Description:** Fourth patrol waypoint sector. It is a map coordinate; an empty value or `0` means the waypoint is unused.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- This table is part of the strategic army and inventory data; references are described in the field notes.

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
