---
schema: 1
lang: ru
family: army-garrison-groups
title: "GarrisonGroups — состав гарнизона по сектору"
tables:
  - GarrisonGroups
table_ids:
  - Army.GarrisonGroups
files:
  - Army/GarrisonGroups.xml
root: GARRISON_INFO
record: GARRISON
key: Sector
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# GarrisonGroups — состав гарнизона по сектору

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`GARRISON_INFO/GARRISON/Sector`](#garrison_infogarrisonsector)
  - [`GARRISON_INFO/GARRISON/Composition`](#garrison_infogarrisoncomposition)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`GarrisonGroups.xml` назначает состав армии гарнизону сектора. Ключ — сектор; состав выбирает
смесь войск из `ArmyComposition.xml`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `GarrisonGroups` | `Army/GarrisonGroups.xml` | — |

## Structure

```
GARRISON_INFO
└── GARRISON
    ├── Sector
    └── Composition
```

## Fields

### `GARRISON_INFO/GARRISON/Sector`

- **Type:** `String`
- **Description:** Сектор, в котором размещён гарнизон, в форме `A1`..`P16`. Он идентифицирует
  точку на карте.
- **Values:** Код сектора, например `P3`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (garrison loader); table data.
- **Confidence:** `source`

### `GARRISON_INFO/GARRISON/Composition`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс состава армии, назначенного этому гарнизону. Он ссылается на
  `ArmyComposition.Index`; это числовой ID, а не свободный текст.
- **Values:** `ArmyComposition.Index`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (garrison loader); table data.
- **Confidence:** `source`

## Relations

- `Composition` ссылается на [ArmyComposition](ArmyComposition.md) `Index`; группа заполняется из
  [SoldierProfiles](../Profiles/SoldierProfiles.md).

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
