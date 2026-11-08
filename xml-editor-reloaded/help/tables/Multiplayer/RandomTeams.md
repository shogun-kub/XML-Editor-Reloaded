---
schema: 1
lang: en
family: multiplayer-random-teams
title: "RandomTeams - multiplayer random team IDs"
tables:
  - RandomTeams
table_ids:
  - Multiplayer.RandomTeams
files:
  - Multiplayer/RandomTeams.xml
root: mp_teams
record: team
key: none
sources:
  - Table data
  - Field names, XML comments and observed values
status: draft
updated: 2026-11-08
---

# RandomTeams - multiplayer random team IDs

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`mp_teams/team/id`](#mp_teamsteamid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`RandomTeams.xml` lists the team IDs available to the multiplayer random-team selector. Each `team` element contributes one ID.

## Tables

| Table | File | Notes |
|---|---|---|
| `RandomTeams` | `Multiplayer/RandomTeams.xml` | — |

## Structure

```
mp_teams
└── team
    └── id
```

## Fields

### `mp_teams/team/id`

- **Type:** `Integer`
- **Description:** Team ID in the random-team list. It is an identifier; the element may repeat for several teams.
- **Values:** Integer; see the description for its scale.
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
