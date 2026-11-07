---
schema: 1
lang: en
family: items-launchables
title: "Items.Launchables - launcher-to-ammo pairs"
tables:
  - Items.Launchables
files:
  - Items/Launchables.xml
root: LAUNCHABLELIST
record: LAUNCHABLE
key: launchableIndex + itemIndex
sources:
  - Tactical/XML_Launchable.cpp
  - Item Types.h
status: draft
updated: 2026-11-07
---

# Items.Launchables - launcher-to-ammo pairs

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`LAUNCHABLELIST/LAUNCHABLE/launchableIndex`](#launchablelistlaunchablelaunchableindex)
  - [`LAUNCHABLELIST/LAUNCHABLE/itemIndex`](#launchablelistlaunchableitemindex)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Launchables.xml` lists which items a launcher can fire. Each record is a pair: the launcher
item and the launchable ammo/projectile item. The game stores the pairs in
`Launchable[MAXITEMS+1][2]` and uses them when a grenade launcher, rocket launcher or similar
weapon selects ammunition.

The v5 file has 159 records and no duplicate pairs.

## Tables

| Table | File | Records | Notes |
|---|---|---:|---|
| `Items.Launchables` | `Items/Launchables.xml` | 159 | v4 and v5 have the same two fields |

## Structure

```
LAUNCHABLELIST
└── LAUNCHABLE        one launcher -> ammo pair
    ├── launchableIndex  item index of the launcher
    └── itemIndex        item index of the launchable ammo
```

The loader does not use a key; it appends records in file order to `Launchable[][]` and sets
`gMAXLAUNCHABLES_READ` to the count. The editor uses the pair (`launchableIndex`,
`itemIndex`) as a compound key. Reordering records changes the runtime array order, so treat
the file order as meaningful until a caller proves otherwise.

## Fields

### `LAUNCHABLELIST/LAUNCHABLE/launchableIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item index of the launcher in `Items.Items.uiIndex`. It identifies the
  weapon that can fire the item on the other side of the pair.
- **Values:** valid `Items.Items.uiIndex` values; shipped data uses 140, 147, and other
  launcher items.
- **Default:** `0`; the pair is still stored, so a zero value is a data error, not "none".
- **Source:** `Tactical/XML_Launchable.cpp:102`, `Item Types.h:1873`
- **Confidence:** `source`

### `LAUNCHABLELIST/LAUNCHABLE/itemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Item index of the launchable ammo/projectile in `Items.Items.uiIndex`.
  It is the item the launcher fires.
- **Values:** valid `Items.Items.uiIndex` values.
- **Default:** `0`; a zero value is a data error.
- **Source:** `Tactical/XML_Launchable.cpp:107`, `Item Types.h:1873`
- **Confidence:** `source`

## Relations

- `Items.Launchables.launchableIndex` -> `Items.Items.uiIndex` (launcher).
- `Items.Launchables.itemIndex` -> `Items.Items.uiIndex` (ammo/projectile).
- The launcher item itself is an `Items.Items` record with a launcher class; the ammo item is
  usually an `IC_AMMO` or explosive item.

## Localization

No strings; both fields are numeric item indexes.

## Versions

v4 and v5 use the same `LAUNCHABLELIST/LAUNCHABLE` structure and the same two fields. Values
may differ; compare the files when a specific pair matters.

## Sources

- `Tactical/XML_Launchable.cpp` - parser, append, `gMAXLAUNCHABLES_READ`.
- `Item Types.h:1873` - `extern UINT16 Launchable[MAXITEMS+1][2]`.
- `TableData/Items/Launchables.xml` - shipped pairs.

## Open questions

- The loader appends in file order and does not deduplicate. The shipped file has no duplicate
  pairs, but the runtime consequence of a duplicate is not documented.
- Which launcher classes use the table at runtime is decided in `Items.cpp`/`Weapons.cpp`;
  this page documents the data contract, not every call site.
