---
schema: 1
lang: en
family: campaign-stats-events
title: "CampaignStatsEvents - campaign news events"
tables:
  - CampaignStatsEvents
table_ids:
  - CampaignStatsEvents
files:
  - CampaignStatsEvents.xml
root: CAMPAIGNSTATSEVENTS
record: EVENT
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-08
---

# CampaignStatsEvents - campaign news events

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`CAMPAIGNSTATSEVENTS/EVENT/uiIndex`](#campaignstatseventseventuiindex)
  - [`CAMPAIGNSTATSEVENTS/EVENT/usCityTaken`](#campaignstatseventseventuscitytaken)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText0`](#campaignstatseventseventsztext0)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText1`](#campaignstatseventseventsztext1)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText2`](#campaignstatseventseventsztext2)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText3`](#campaignstatseventseventsztext3)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText4`](#campaignstatseventseventsztext4)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText5`](#campaignstatseventseventsztext5)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText6`](#campaignstatseventseventsztext6)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText7`](#campaignstatseventseventsztext7)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText8`](#campaignstatseventseventsztext8)
  - [`CAMPAIGNSTATSEVENTS/EVENT/szText9`](#campaignstatseventseventsztext9)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`CampaignStatsEvents.xml` defines the newspaper-style campaign news. Each event has ten fixed text slots and an optional town reference.

## Tables

| Table | File | Notes |
|---|---|---|
| `CampaignStatsEvents` | `CampaignStatsEvents.xml` | — |

## Structure

```
CAMPAIGNSTATSEVENTS
└── EVENT
    ├── uiIndex
    ├── usCityTaken
    ├── szText0
    ├── szText1
    ├── szText2
    ├── szText3
    ├── szText4
    ├── szText5
    ├── szText6
    ├── szText7
    ├── szText8
    └── szText9
```

## Fields

### `CAMPAIGNSTATSEVENTS/EVENT/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Index of the campaign-stats event. It is the array key; for a one-time event it also selects the event picture frame and the ten text slots shown in the campaign-history summary.
- **Values:** `0..19` (`NUM_CAMPAIGNSTATSEVENTS`); the loader stores a row only when this index is below the event count. The field itself is `UINT16`.
- **Default:** Required; an omitted element is read as `0`, so the row is stored in slot 0.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/usCityTaken`

- **Type:** `Integer` (`UINT8`)
- **Description:** Town this event belongs to, or `0` when the event is not tied to a town. For a one-time event, the campaign-history summary compares it with the town ID derived from the incident's sector; the values are [Cities](Map/Cities.md) town indexes.
- **Values:** `0..255`; the loader stores this field as `UINT8`. `0` means no town; a non-zero value is a town index.
- **Default:** `0` when omitted; an empty element is parsed as `0`.
- **Source:** game code (campaign-stats loader; campaign-history summary town matching); table data and XML comment.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText0`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 1 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText1`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 2 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText2`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 3 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText3`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 4 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText4`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 5 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText5`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 6 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText6`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 7 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText7`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 8 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText8`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 9 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText9`

- **Type:** `String` (`CHAR16`)
- **Description:** Paragraph 10 of the event text. For a one-time event, the campaign-history summary iterates all ten text slots in order and calls the text renderer for each; it does not skip empty slots.
- **Values:** Free display text (UTF-8); stored as UTF-16 in a 300-character buffer, and longer text is truncated.
- **Default:** Empty when omitted; an empty element yields an empty string.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

## Relations

- `usCityTaken` references a town index in [Cities](Map/Cities.md); `0` means the event is not town-specific.

## Localization

The ten text slots are translated by a language-prefixed overlay file: after the base file, the
loader reads the language file with a flag that copies only the `szText*` slots into the
existing event records. `uiIndex` and `usCityTaken` are not localized.

## Versions

v4 and v5 use the same field paths; values may differ.

## Sources

- Table data.
- Game code: campaign-stats loader and campaign-history summary renderer.
- Field names, XML comments and observed values.
- Descriptions cover the values the loader stores and the renderer uses; every field in this document is backed by the loader.

## Open questions

The event picture referenced by `uiIndex` is an external asset; this table does not define how many picture frames that asset contains.
