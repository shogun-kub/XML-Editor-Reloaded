---
schema: 1
lang: ru
family: enemy-rank
title: "EnemyRank — названия вражеских званий"
tables:
  - EnemyRank
table_ids:
  - EnemyRank
files:
  - EnemyRank.xml
root: ENEMY_RANK
record: RANK
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# EnemyRank — названия вражеских званий

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ENEMY_RANK/RANK/uiIndex`](#enemy_rankrankuiindex)
  - [`ENEMY_RANK/RANK/Enabled`](#enemy_rankrankenabled)
  - [`ENEMY_RANK/RANK/ExpLevel`](#enemy_rankrankexplevel)
  - [`ENEMY_RANK/RANK/szRank`](#enemy_rankrankszrank)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyRank.xml` сопоставляет уровни опыта с названиями вражеских званий для интерфейса и
отображения кампании.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EnemyRank` | `EnemyRank.xml` | — |

## Structure

```
ENEMY_RANK
└── RANK
    ├── uiIndex
    ├── Enabled
    ├── ExpLevel
    └── szRank
```

## Fields

### `ENEMY_RANK/RANK/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс вражеского звания. Это ключ массива.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (enemy-rank loader); table data.
- **Confidence:** `source`

### `ENEMY_RANK/RANK/Enabled`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, это название звания включено. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (enemy-rank loader); table data.
- **Confidence:** `source`

### `ENEMY_RANK/RANK/ExpLevel`

- **Type:** `Integer` (`UINT16`)
- **Description:** Уровень опыта, связанный с этим званием.
- **Numeric semantics:** больше = опытнее; комментарий таблицы задаёт предполагаемый диапазон
  `1..9`.
- **Values:** Предполагается `1..9`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (enemy-rank loader); table XML comment.
- **Confidence:** `source`

### `ENEMY_RANK/RANK/szRank`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое название звания, например `Recruit` или `Colonel`.
- **Values:** Свободный текст.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (enemy-rank loader); table data.
- **Confidence:** `source`

## Relations

- Эта таблица — часть стратегических данных армии и инвентаря; ссылки описаны в примечаниях к полям.

## Localization

Отображаемые имена и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, комментарии XML и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не утверждается.

## Open questions

- Точная формула или сопоставление перечисления для выведенных полей не указаны в данных таблицы.
