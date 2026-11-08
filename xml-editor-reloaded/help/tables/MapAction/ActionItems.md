---
schema: 1
lang: en
family: map-action-items
title: "ActionItems - map action items"
tables:
  - ActionItems
table_ids:
  - MapAction.ActionItems
files:
  - MapAction/ActionItems.xml
root: ACTION_ITEM_LIST
record: ACTION_ITEM
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# ActionItems - map action items

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/uiIndex`](#action_item_listaction_itemuiindex)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/Name`](#action_item_listaction_itemname)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/ActionID`](#action_item_listaction_itemactionid)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/Blow_up`](#action_item_listaction_itemblow_up)
  - [`ACTION_ITEM_LIST/ACTION_ITEM/BombItem`](#action_item_listaction_itembombitem)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`ActionItems.xml` maps action IDs to display names, an explosion flag and the bomb item used by the action. It is the action palette used by the map/editor and scripted map events.

## Tables

| Table | File | Notes |
|---|---|---|
| `ActionItems` | `MapAction/ActionItems.xml` | — |

## Structure

```
ACTION_ITEM_LIST
└── ACTION_ITEM
    ├── uiIndex
    ├── Name
    ├── ActionID
    ├── Blow_up
    └── BombItem
```

## Fields

### `ACTION_ITEM_LIST/ACTION_ITEM/uiIndex`

- **Type:** `Integer`
- **Description:** Index of the action item. It is an array key.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ACTION_ITEM_LIST/ACTION_ITEM/Name`

- **Type:** `String`
- **Description:** Display name of the action, such as `Klaxon Mine` or `Open Door`.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ACTION_ITEM_LIST/ACTION_ITEM/ActionID`

- **Type:** `Integer`
- **Description:** Action-type code executed by this entry. It is an enum code.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ACTION_ITEM_LIST/ACTION_ITEM/Blow_up`

- **Type:** `Boolean`
- **Description:** If set, the action is an explosion/demolition action. It is a switch.
- **Values:** `0` or `1`.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `ACTION_ITEM_LIST/ACTION_ITEM/BombItem`

- **Type:** `Integer`
- **Description:** Item ID used as the bomb/explosive for this action. It is an item reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `BombItem` references an item ID in [Items](../Items/Items.md).

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
