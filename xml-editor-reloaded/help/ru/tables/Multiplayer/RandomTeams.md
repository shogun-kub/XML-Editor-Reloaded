---
schema: 1
lang: ru
family: multiplayer-random-teams
title: "RandomTeams — ID случайных команд в сетевой игре"
tables:
  - RandomTeams
table_ids:
  - Multiplayer.RandomTeams
files:
  - Multiplayer/RandomTeams.xml
root: mp_teams
record: team
key: none
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# RandomTeams — ID случайных команд в сетевой игре

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`mp_teams/team/id`](#mp_teamsteamid)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`RandomTeams.xml` перечисляет ID команд, доступные выбору случайной команды в сетевой игре. Каждый
элемент `team` даёт один ID.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `RandomTeams` | `Multiplayer/RandomTeams.xml` | — |

## Structure

```
mp_teams
└── team
    └── id
```

## Fields

### `mp_teams/team/id`

- **Type:** `Integer` (`INT8`)
- **Description:** Индекс профиля наёмника одного участника объемлющего `<team>`. Каждый `<team>`
  может содержать несколько элементов `<id>`, а необязательный атрибут `name` у `<team>` игрой не
  используется.
- **Values:** `-128..127`; комментарий XML указывает, что значение должно совпадать с индексом
  выбранного наёмника в ProEdit.
- **Default:** Отсутствует; команда без элементов `<id>` не имеет записи об участнике.
- **Source:** game code (`MPXmlTeams.cpp`); table XML comment.
- **Confidence:** `source`

## Relations

- Эта таблица является частью данных стратегической армии и инвентаря; ссылки описаны в
  примечаниях к полям.

## Localization

Отображаемые имена и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, XML-комментарии и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не заявляется.

## Open questions

- Точная формула или сопоставление перечисления для выведенных полей в данных таблицы не указаны.
