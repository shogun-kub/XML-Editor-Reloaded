---
schema: 1
lang: ru
family: email-sender-names
title: "EmailSenderNameList — имена отправителей писем"
tables:
  - EmailSenderNameList
table_ids:
  - Email.EmailSenderNameList
files:
  - Email/EmailSenderNameList.xml
root: SENDER_LIST
record: NAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# EmailSenderNameList — имена отправителей писем

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`SENDER_LIST/NAME/uiIndex`](#sender_listnameuiindex)
  - [`SENDER_LIST/NAME/Name`](#sender_listnamename)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EmailSenderNameList.xml` — список отображаемых имён, используемых общими письмами `Emails.xml`.
Индекс выбирает имя отправителя; текст используется дословно. См.
[форматирование текста писем](../../EMAIL-TEXT.md).

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EmailSenderNameList` | `Email/EmailSenderNameList.xml` | — |

## Structure

```
SENDER_LIST
└── NAME
    ├── uiIndex
    └── Name
```

## Fields


### `SENDER_LIST/NAME/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс записи имени отправителя. Это ключ массива.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (email/sender loader); table data.
- **Confidence:** `source`


### `SENDER_LIST/NAME/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое имя, используемое как отправитель письма, применяется дословно. См.
  [форматирование текста писем](../../EMAIL-TEXT.md).
- **Values:** Свободный текст или символьный код.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (email/sender loader); table data.
- **Confidence:** `source`

## Relations

- Поле `Sender` в `Emails.xml` ссылается на `uiIndex` в этой таблице.

## Localization

Отображаемые имена и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, комментарии XML и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не утверждается.

## Open questions

- Нет. Имена — буквальный отображаемый текст; см.
  [форматирование текста писем](../../EMAIL-TEXT.md).
