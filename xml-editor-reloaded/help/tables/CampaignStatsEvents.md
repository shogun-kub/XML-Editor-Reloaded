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

`CampaignStatsEvents.xml` defines the newspaper-style campaign news. Each event has up to ten text paragraphs and an optional city reference.

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

- **Type:** `Integer`
- **Description:** Index of the campaign-stats event. It is an array key and identifies the event type.
- **Values:** Integer; see the description for its scale.
- **Default:** Required; identifies the record.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/usCityTaken`

- **Type:** `Integer`
- **Description:** City affected by this event, or `0` when the event is not tied to a city. It is a city reference.
- **Values:** Integer; see the description for its scale.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText0`

- **Type:** `String`
- **Description:** Paragraph 1 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText1`

- **Type:** `String`
- **Description:** Paragraph 2 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText2`

- **Type:** `String`
- **Description:** Paragraph 3 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText3`

- **Type:** `String`
- **Description:** Paragraph 4 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText4`

- **Type:** `String`
- **Description:** Paragraph 5 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText5`

- **Type:** `String`
- **Description:** Paragraph 6 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText6`

- **Type:** `String`
- **Description:** Paragraph 7 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText7`

- **Type:** `String`
- **Description:** Paragraph 8 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText8`

- **Type:** `String`
- **Description:** Paragraph 9 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

### `CAMPAIGNSTATSEVENTS/EVENT/szText9`

- **Type:** `String`
- **Description:** Paragraph 10 of the event text. Empty paragraphs are allowed; the game displays the non-empty paragraphs in order.
- **Values:** Free text or a symbolic code.
- **Default:** Absent; the game default applies.
- **Source:** table data; field names, XML comments and observed values.
- **Confidence:** `inferred`

## Relations

- `usCityTaken` references a city index in `Cities.xml`; `0` means the event is not city-specific.

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
