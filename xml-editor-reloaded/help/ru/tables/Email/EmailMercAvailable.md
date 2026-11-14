---
schema: 1
lang: ru
family: email-merc-available
title: "EmailMercAvailable — почта о доступности наёмника"
tables:
  - EmailMercAvailable
table_ids:
  - Email.EmailMercAvailable
files:
  - Email/EmailMercAvailable.xml
root: EMAIL_MERC_AVAILABLE
record: EMAIL
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# EmailMercAvailable — почта о доступности наёмника

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`EMAIL_MERC_AVAILABLE/EMAIL/uiIndex`](#email_merc_availableemailuiindex)
  - [`EMAIL_MERC_AVAILABLE/EMAIL/Subject`](#email_merc_availableemailsubject)
  - [`EMAIL_MERC_AVAILABLE/EMAIL/Message`](#email_merc_availableemailmessage)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EmailMercAvailable.xml` содержит сгенерированное письмо, которое отправляется, когда наёмник AIM
снова становится доступен. В каждой строке есть тема и сообщение. Оба — буквальный отображаемый
текст; эта таблица не подставляет заполнители. См. [форматирование текста писем](../../EMAIL-TEXT.md).

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EmailMercAvailable` | `Email/EmailMercAvailable.xml` | — |

## Structure

```
EMAIL_MERC_AVAILABLE
└── EMAIL
    ├── uiIndex
    ├── Subject
    └── Message
```

## Fields


### `EMAIL_MERC_AVAILABLE/EMAIL/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс шаблона электронного письма. Это ключ массива.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (email/sender loader); table data.
- **Confidence:** `source`


### `EMAIL_MERC_AVAILABLE/EMAIL/Subject`

- **Type:** `String` (`CHAR16`)
- **Description:** Тема сгенерированного письма, используется как буквальный текст. См.
  [форматирование текста писем](../../EMAIL-TEXT.md).
- **Values:** Свободный текст или символьный код.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (email/sender loader); table data.
- **Confidence:** `source`


### `EMAIL_MERC_AVAILABLE/EMAIL/Message`

- **Type:** `String` (`CHAR16`)
- **Description:** Тело сгенерированного письма. `±` обозначает разрыв строки в сохранённом
  сообщении. См. [форматирование текста писем](../../EMAIL-TEXT.md).
- **Values:** Свободный текст или символьный код.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (email/sender loader); table data.
- **Confidence:** `source`

## Relations

- Шаблон выбирается по своему `uiIndex`, когда срабатывает соответствующее игровое событие.
- Отображаемое имя отправителя берётся из [MercProfiles](../MercProfiles.md) `zNickname` для того же
  индекса профиля; в этой таблице оно не хранится.

## Localization

Отображаемые имена и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, комментарии XML и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не утверждается.

## Open questions

- Нет. Тема и сообщение используются буквально; см.
  [форматирование текста писем](../../EMAIL-TEXT.md).
