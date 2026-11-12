---
schema: 1
lang: ru
family: profiles-soldier-templates
title: "Soldier profiles — шаблоны врагов и ополчения"
tables:
  - SoldierProfileEnemyAdmin
  - SoldierProfileEnemyElite
  - SoldierProfileEnemyRegular
  - SoldierProfileMilitiaGreen
  - SoldierProfileMilitiaRegular
  - SoldierProfileMilitiaVeteran
table_ids:
  - Profiles.SoldierProfileEnemyAdmin
  - Profiles.SoldierProfileEnemyElite
  - Profiles.SoldierProfileEnemyRegular
  - Profiles.SoldierProfileMilitiaGreen
  - Profiles.SoldierProfileMilitiaRegular
  - Profiles.SoldierProfileMilitiaVeteran
files:
  - Profiles/SoldierProfileEnemyAdmin.xml
  - Profiles/SoldierProfileEnemyElite.xml
  - Profiles/SoldierProfileEnemyRegular.xml
  - Profiles/SoldierProfileMilitiaGreen.xml
  - Profiles/SoldierProfileMilitiaRegular.xml
  - Profiles/SoldierProfileMilitiaVeteran.xml
root: PROFILES
record: PROFILE
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# Soldier profiles — шаблоны врагов и ополчения

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`PROFILES/PROFILE/uiIndex`](#profilesprofileuiindex)
  - [`PROFILES/PROFILE/szName`](#profilesprofileszname)
  - [`PROFILES/PROFILE/uiBodyType`](#profilesprofileuibodytype)
  - [`PROFILES/PROFILE/uiHair`](#profilesprofileuihair)
  - [`PROFILES/PROFILE/uiSkin`](#profilesprofileuiskin)
  - [`PROFILES/PROFILE/uiTrait1`](#profilesprofileuitrait1)
  - [`PROFILES/PROFILE/uiTrait2`](#profilesprofileuitrait2)
  - [`PROFILES/PROFILE/uiTrait3`](#profilesprofileuitrait3)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

Шесть файлов `SoldierProfile*` определяют визуальные шаблоны и шаблоны черт для вражеских званий
и званий ополчения: тип тела, волосы, кожу и до трёх черт навыков. Они используют одни и те же
восемь полей; различаются только данные, зависящие от звания.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `SoldierProfileEnemyAdmin` | `Profiles/SoldierProfileEnemyAdmin.xml` | — |
| `SoldierProfileEnemyElite` | `Profiles/SoldierProfileEnemyElite.xml` | — |
| `SoldierProfileEnemyRegular` | `Profiles/SoldierProfileEnemyRegular.xml` | — |
| `SoldierProfileMilitiaGreen` | `Profiles/SoldierProfileMilitiaGreen.xml` | — |
| `SoldierProfileMilitiaRegular` | `Profiles/SoldierProfileMilitiaRegular.xml` | — |
| `SoldierProfileMilitiaVeteran` | `Profiles/SoldierProfileMilitiaVeteran.xml` | — |

## Structure

```
PROFILES
└── PROFILE
    ├── uiIndex
    ├── szName
    ├── uiBodyType
    ├── uiHair
    ├── uiSkin
    ├── uiTrait1
    ├── uiTrait2
    └── uiTrait3
```

## Fields

### `PROFILES/PROFILE/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс профиля внутри файла. Это ключ массива.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/szName`

- **Type:** `String`
- **Description:** Имя шаблона или профиля, используемое для идентификации и отображения в
  редакторе.
- **Values:** Свободный текст или символьный код.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiBodyType`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID графики типа тела, используемый этим профилем. Это ссылка на графику.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiHair`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID графики волос, используемый этим профилем. Это ссылка на графику.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiSkin`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID графики кожи, используемый этим профилем. Это ссылка на графику.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiTrait1`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID первой черты навыка для шаблона. Это перечисление или ссылка; `0` означает
  отсутствие черты.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiTrait2`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID второй черты навыка для шаблона. Это перечисление или ссылка; `0` означает
  отсутствие черты.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

### `PROFILES/PROFILE/uiTrait3`

- **Type:** `Integer` (`UINT8`)
- **Description:** ID третьей черты навыка для шаблона. Это перечисление или ссылка; `0` означает
  отсутствие черты.
- **Values:** целое число; масштаб см. в описании.
- **Default:** Отсутствует; применяется игровое значение по умолчанию.
- **Source:** game code (soldier-profile loader); table data.
- **Confidence:** `source`

## Relations

- `uiIndex` локален для каждого файла шаблона. Файлы соответствуют вражеским званиям
  admin/elite/regular и званиям ополчения green/regular/veteran. Визуальные поля и поля черт
  повторяют соответствующие поля [MercProfiles](../MercProfiles.md).

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
