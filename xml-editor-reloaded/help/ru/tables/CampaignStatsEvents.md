---
schema: 1
lang: ru
family: campaign-stats-events
title: "CampaignStatsEvents — новостные события кампании"
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
updated: 2026-11-12
---

# CampaignStatsEvents — новостные события кампании

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

`CampaignStatsEvents.xml` определяет новости кампании в стиле газеты. У каждого события десять
фиксированных текстовых слотов и необязательная ссылка на город.

## Tables

| Таблица | Файл | Примечания |
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
- **Description:** Индекс события статистики кампании. Это ключ массива; для разового события он
  также выбирает кадр картинки события и десять текстовых слотов, показываемых в сводке истории
  кампании.
- **Values:** `0..19` (`NUM_CAMPAIGNSTATSEVENTS`); загрузчик сохраняет строку, только если этот
  индекс меньше числа событий. Само поле — `UINT16`.
- **Default:** Обязательно; отсутствующий элемент читается как `0`, поэтому строка сохраняется в
  слот 0.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/usCityTaken`

- **Type:** `Integer` (`UINT8`)
- **Description:** Город, к которому относится это событие, или `0`, если событие не привязано к
  городу. Для разового события сводка истории кампании сравнивает его с ID города, полученным из
  сектора происшествия; значения — индексы городов [Cities](Map/Cities.md).
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`. `0` означает отсутствие города;
  ненулевое значение — индекс города.
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (campaign-stats loader; campaign-history summary town matching); table data and XML comment.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText0`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 1 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText1`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 2 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText2`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 3 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText3`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 4 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText4`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 5 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText5`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 6 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText6`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 7 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText7`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 8 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText8`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 9 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

### `CAMPAIGNSTATSEVENTS/EVENT/szText9`

- **Type:** `String` (`CHAR16`)
- **Description:** Абзац 10 текста события. Для разового события сводка истории кампании перебирает
  все десять текстовых слотов по порядку и вызывает отрисовщик текста для каждого; пустые слоты не
  пропускаются.
- **Values:** Свободный отображаемый текст (UTF-8); сохраняется как UTF-16 в буфере на 300 символов,
  более длинный текст обрезается.
- **Default:** Пусто, если поле отсутствует; пустой элемент даёт пустую строку.
- **Source:** game code (campaign-stats loader; campaign-history summary); table data.
- **Confidence:** `source`

## Relations

- `usCityTaken` ссылается на индекс города в [Cities](Map/Cities.md); `0` означает, что событие не
  привязано к городу.

## Localization

Десять текстовых слотов переводятся оверлеем с языковым префиксом: после базового файла загрузчик
читает языковой файл с флагом, который копирует только слоты `szText*` в существующие записи событий.
`uiIndex` и `usCityTaken` не локализуются.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Код игры: загрузчик статистики кампании и отрисовщик сводки истории кампании.
- Имена полей, комментарии XML и наблюдаемые значения.
- Описания охватывают значения, которые сохраняет загрузчик, и которые использует отрисовщик; каждое
  поле в этом документе подтверждено загрузчиком.

## Open questions

Картинка события, на которую ссылается `uiIndex`, — внешний ресурс; эта таблица не определяет, сколько
кадров содержит этот ресурс.
