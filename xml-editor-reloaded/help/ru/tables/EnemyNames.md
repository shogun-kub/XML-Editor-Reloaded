---
schema: 1
lang: ru
family: enemy-names
title: "EnemyNames — именованные вражеские группы по сектору"
tables:
  - EnemyNames
table_ids:
  - EnemyNames
files:
  - EnemyNames.xml
root: ENEMY_NAMES
record: NAME
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# EnemyNames — именованные вражеские группы по сектору

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ENEMY_NAMES/NAME/uiIndex`](#enemy_namesnameuiindex)
  - [`ENEMY_NAMES/NAME/Enabled`](#enemy_namesnameenabled)
  - [`ENEMY_NAMES/NAME/Sector`](#enemy_namesnamesector)
  - [`ENEMY_NAMES/NAME/szGroup`](#enemy_namesnameszgroup)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`EnemyNames.xml` даёт вражеским группам в определённых секторах собственное отображаемое имя.
Отключённая или отсутствующая запись оставляет имя по умолчанию.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `EnemyNames` | `EnemyNames.xml` | — |

## Structure

```
ENEMY_NAMES
└── NAME
    ├── uiIndex
    ├── Enabled
    ├── Sector
    └── szGroup
```

## Fields

### `ENEMY_NAMES/NAME/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс записи имени врага. Это ключ массива.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (enemy-names loader); table data.
- **Confidence:** `source`

### `ENEMY_NAMES/NAME/Enabled`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, собственное имя вражеской группы включено. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (enemy-names loader); table data.
- **Confidence:** `source`

### `ENEMY_NAMES/NAME/Sector`

- **Type:** `String`
- **Description:** Код сектора, где применяется собственное имя вражеской группы. Это координата
  карты в форме `A1`..`P16`: одна буква для строки и десятичный номер столбца.
- **Numeric semantics:** Загрузчик принимает первый символ как строку, а остальные цифры — как
  столбец; он сохраняет значение, только когда оба находятся в `1..16`.
- **Values:** Сектор, например `A1`, `B5` или `P16`; значение вне диапазона оставляет
  предыдущую/стандартную координату без изменений.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (enemy-names loader); table data.
- **Confidence:** `source`

### `ENEMY_NAMES/NAME/szGroup`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое имя вражеской группы в этом секторе.
- **Values:** Свободный текст.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (enemy-names loader); table data.
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
