---
schema: 1
lang: en
family: sounds
title: "Sounds - sound and burst-sound file lists"
tables:
  - Sounds
  - BurstSounds
table_ids:
  - Sounds.Sounds
  - Sounds.BurstSounds
files:
  - Sounds/Sounds.xml
  - Sounds/BurstSounds.xml
root: SOUNDLIST
record: SOUND
key: none
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# Sounds - sound and burst-sound file lists

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SOUNDLIST/SOUND`](#soundlistsound)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Sounds.xml` and `BurstSounds.xml` map numeric sound IDs to WAV file paths. The position of a `SOUND` element is its ID; `BurstSounds` uses the same layout for burst-fire sounds. `NOSOUND` is a valid placeholder.

## Tables

| Table | File | Notes |
|---|---|---|
| `Sounds` | `Sounds/Sounds.xml` | — |
| `BurstSounds` | `Sounds/BurstSounds.xml` | — |

## Structure

```
SOUNDLIST
└── SOUND
```

## Fields

### `SOUNDLIST/SOUND`

- **Type:** `String` (`CHAR8`)
- **Description:** Path to a sound file, relative to the game data directory. The zero-based position in the list is the sound ID; the element may repeat, and the order is significant. It is a file reference.
- **Values:** A path such as `SOUNDS\RICOCHET 01.wav`; values are truncated to 255 characters.
- **Default:** Absent; the game default applies.
- **Source:** game code (`XML_Sounds.cpp`); table data.
- **Confidence:** `source`

## Relations

- Sound IDs are referenced by [Weapons](../Items/Weapons.md), [Vehicles](../Vehicles.md) and UI fields elsewhere in TableData.

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
