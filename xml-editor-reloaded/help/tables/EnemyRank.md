---
schema: 1
lang: en
family: enemy-rank
title: "EnemyRank - enemy rank names"
tables:
  - EnemyRank
table_ids:
  - EnemyRank
files:
  - EnemyRank.xml
root: ENEMY_RANK
record: RANK
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# EnemyRank - enemy rank names

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ENEMY_RANK/RANK/uiIndex`](#enemy_rankrankuiindex)
  - [`ENEMY_RANK/RANK/Enabled`](#enemy_rankrankenabled)
  - [`ENEMY_RANK/RANK/ExpLevel`](#enemy_rankrankexplevel)
  - [`ENEMY_RANK/RANK/szRank`](#enemy_rankrankszrank)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyRank.xml` maps experience levels to enemy rank names for the UI and campaign display.

## Tables

| Table | File | Notes |
|---|---|---|
| `EnemyRank` | `EnemyRank.xml` | — |

## Structure

```
ENEMY_RANK
└── RANK
    ├── uiIndex
    ├── Enabled
    ├── ExpLevel
    └── szRank
```

## Fields

### `ENEMY_RANK/RANK/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index of the enemy rank. It is an array key.
- **Values:** `0..65535`; the loader stores this field as `UINT16`.
- **Default:** Required; identifies the record.
- **Source:** game code (enemy-rank loader); table data.
- **Confidence:** `source`

### `ENEMY_RANK/RANK/Enabled`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** If set, this rank name is enabled. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-rank loader); table data.
- **Confidence:** `source`

### `ENEMY_RANK/RANK/ExpLevel`

- **Type:** `Integer` (`UINT16`)
- **Description:** Experience level associated with this rank.
- **Numeric semantics:** higher = more experienced; the table comment gives the intended range `1..9`.
- **Values:** `1..9` intended; the loader stores this field as `UINT16`.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-rank loader); table XML comment.
- **Confidence:** `source`

### `ENEMY_RANK/RANK/szRank`

- **Type:** `String` (`CHAR16`)
- **Description:** Display name of the rank, such as `Recruit` or `Colonel`.
- **Values:** Free text.
- **Default:** Absent; the game default applies.
- **Source:** game code (enemy-rank loader); table data.
- **Confidence:** `source`

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
