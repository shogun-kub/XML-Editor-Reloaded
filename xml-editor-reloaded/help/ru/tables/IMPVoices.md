---
schema: 1
lang: ru
family: mercs-faces
title: "IMPVoices — наборы голосов IMP"
tables:
  - IMPVoices
table_ids:
  - IMPVoices
files:
  - IMPVoices.xml
root: IMPVOICELIST
record: VOICE
key: voiceset
sources:
  - IMP voice table data
status: reviewed
updated: 2026-11-12
---

# IMPVoices — наборы голосов IMP

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`IMPVOICELIST/VOICE/voiceset`](#impvoicelistvoicevoiceset)
  - [`IMPVOICELIST/VOICE/szVoiceSetName`](#impvoicelistvoiceszvoicesetname)
  - [`IMPVOICELIST/VOICE/bSex`](#impvoicelistvoicebsex)

## Purpose

`IMPVoices.xml` сопоставляет ID набора голосов IMP и пол с названием набора голосов, используемым
при создании персонажа.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `IMPVoices` | `IMPVoices.xml` | в v4 и v5 одинаковые пути полей; значения различаются. |

## Structure

```
IMPVOICELIST
└── VOICE
    ├── voiceset
    ├── szVoiceSetName
    └── bSex
```

## Fields

### `IMPVOICELIST/VOICE/voiceset`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID набора голосов, используемый выбором голоса IMP.
- **Values:** Целое число; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент следует игровому значению по умолчанию.
- **Source:** game code (IMP voice loader); table data.
- **Confidence:** `source`

### `IMPVOICELIST/VOICE/szVoiceSetName`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое название набора голосов.
- **Values:** Свободный текст.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (IMP voice loader); table data.
- **Confidence:** `source`

### `IMPVOICELIST/VOICE/bSex`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Пол, для которого предназначен набор голосов.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (IMP voice loader); table data.
- **Confidence:** `source`
