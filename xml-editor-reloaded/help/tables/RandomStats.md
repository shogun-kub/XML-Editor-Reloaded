---
schema: 1
lang: en
family: random-stats
title: "RandomStats - per-profile random stats"
tables:
  - RandomStats
table_ids:
  - RandomStats
files:
  - RandomStats.xml
root: RANDOM_STATS
record: PROFILE
key: uiIndex
sources:
  - Random stats table data
  - RandomStats schema comments
status: reviewed
updated: 2026-11-07
---

# RandomStats - per-profile random stats

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`RANDOM_STATS/PROFILE/uiIndex`](#random_statsprofileuiindex)
  - [`RANDOM_STATS/PROFILE/Name`](#random_statsprofilename)
  - [`RANDOM_STATS/PROFILE/Enabled`](#random_statsprofileenabled)
  - [`RANDOM_STATS/PROFILE/BaseAttribute`](#random_statsprofilebaseattribute)
  - [`RANDOM_STATS/PROFILE/RandomExpLevel`](#random_statsprofilerandomexplevel)
  - [`RANDOM_STATS/PROFILE/RandomLife`](#random_statsprofilerandomlife)
  - [`RANDOM_STATS/PROFILE/RandomAgility`](#random_statsprofilerandomagility)
  - [`RANDOM_STATS/PROFILE/RandomDexterity`](#random_statsprofilerandomdexterity)
  - [`RANDOM_STATS/PROFILE/RandomStrength`](#random_statsprofilerandomstrength)
  - [`RANDOM_STATS/PROFILE/RandomLeadership`](#random_statsprofilerandomleadership)
  - [`RANDOM_STATS/PROFILE/RandomWisdom`](#random_statsprofilerandomwisdom)
  - [`RANDOM_STATS/PROFILE/RandomMarksmanship`](#random_statsprofilerandommarksmanship)
  - [`RANDOM_STATS/PROFILE/RandomMechanical`](#random_statsprofilerandommechanical)
  - [`RANDOM_STATS/PROFILE/RandomExplosive`](#random_statsprofilerandomexplosive)
  - [`RANDOM_STATS/PROFILE/RandomMedical`](#random_statsprofilerandommedical)
  - [`RANDOM_STATS/PROFILE/RandomScientific`](#random_statsprofilerandomscientific)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`RandomStats.xml` controls which stats are randomized for each profile when `MERCS_RANDOM_STATS` is set to 2 or 3. A profile can enable randomization per stat; `BaseAttribute` is the mode-3 base value.

## Tables

| Table | File | Notes |
|---|---|---|
| `RandomStats` | `RandomStats.xml` | v4 and v5 have the same field paths; values differ. |

## Structure

```
RANDOM_STATS
└── PROFILE
    ├── uiIndex
    ├── Name
    ├── Enabled
    ├── BaseAttribute
    ├── RandomExpLevel
    ├── RandomLife
    ├── RandomAgility
    ├── RandomDexterity
    ├── RandomStrength
    ├── RandomLeadership
    ├── RandomWisdom
    ├── RandomMarksmanship
    ├── RandomMechanical
    ├── RandomExplosive
    ├── RandomMedical
    └── RandomScientific
```

## Fields

### `RANDOM_STATS/PROFILE/uiIndex`

- **Type:** `Integer` (`INT32`)
- **Description:** Profile identity.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** Integer base attribute value.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/Name`

- **Type:** `String` (`String`)
- **Description:** Profile name.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** Profile name.
- **Default:** empty string when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/Enabled`

- **Type:** `Boolean` (`Boolean`)
- **Description:** If set, random stats are enabled for this profile.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/BaseAttribute`

- **Type:** `Integer` (`INT32`)
- **Description:** Base attribute used to build random stats; only used when `MERCS_RANDOM_STATS` is 3.
- **Only used with:** MERCS_RANDOM_STATS 3.
- **Numeric semantics:** Higher values mean a higher base attribute used to build the random stat; negative values are outside the normal range.
- **Values:** Integer base attribute value.
- **Default:** `0` when omitted; an empty element follows the game default.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomExpLevel`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize experience level.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomLife`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize life.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomAgility`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize agility.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomDexterity`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize dexterity.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomStrength`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize strength.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomLeadership`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize leadership.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomWisdom`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize wisdom.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomMarksmanship`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize marksmanship.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomMechanical`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize mechanical skill.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomExplosive`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize explosives skill.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomMedical`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize medical skill.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

### `RANDOM_STATS/PROFILE/RandomScientific`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Randomize scientific skill.
- **Only used with:** MERCS_RANDOM_STATS 2 or 3.
- **Values:** `0` or `1`.
- **Default:** `0` when omitted.
- **Source:** random stats table data; table metadata.
- **Confidence:** `inferred`

## Relations

- `uiIndex` points to [MercProfiles](MercProfiles.md).

## Localization

`Name` is a profile label; random-stat flags are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Random stats table data.
- RandomStats schema comments.
- Field types: table data and field names; enum values come from the schema comments.
- Descriptions are conservative where no formula is stated; no observed value ranges are used.

## Open questions

The random range for each stat is derived from the profile level and the selected `MERCS_RANDOM_STATS` mode, not stored in this table.
