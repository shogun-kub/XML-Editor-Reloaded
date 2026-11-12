---
schema: 1
lang: ru
family: army-uniform-colors
title: "UniformColors — цвета формы врагов и ополчения"
tables:
  - UniformColors
table_ids:
  - Army.UniformColors
files:
  - Army/UniformColors.XML
root: UNIFORMS
record: UNIFORMS
key: none
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# UniformColors — цвета формы врагов и ополчения

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`UNIFORMS/ENEMY_ADMIN/Vest`](#uniformsenemy_adminvest)
  - [`UNIFORMS/ENEMY_ADMIN/Pants`](#uniformsenemy_adminpants)
  - [`UNIFORMS/ENEMY_TROOP/Vest`](#uniformsenemy_troopvest)
  - [`UNIFORMS/ENEMY_TROOP/Pants`](#uniformsenemy_trooppants)
  - [`UNIFORMS/ENEMY_ELITE/Vest`](#uniformsenemy_elitevest)
  - [`UNIFORMS/ENEMY_ELITE/Pants`](#uniformsenemy_elitepants)
  - [`UNIFORMS/MILITIA_ROOKIE/Vest`](#uniformsmilitia_rookievest)
  - [`UNIFORMS/MILITIA_ROOKIE/Pants`](#uniformsmilitia_rookiepants)
  - [`UNIFORMS/MILITIA_REGULAR/Vest`](#uniformsmilitia_regularvest)
  - [`UNIFORMS/MILITIA_REGULAR/Pants`](#uniformsmilitia_regularpants)
  - [`UNIFORMS/MILITIA_ELITE/Vest`](#uniformsmilitia_elitevest)
  - [`UNIFORMS/MILITIA_ELITE/Pants`](#uniformsmilitia_elitepants)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`UniformColors.xml` назначает цвет жилета и штанов каждому званию врагов и ополчения. Все шесть
блоков формы обязательны; загрузчик выдаёт ошибку, если какой-либо отсутствует. Значения должны быть
одним из имён палитры с учётом регистра, перечисленных в комментарии XML, а не произвольным текстом.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `UniformColors` | `Army/UniformColors.XML` | — |

## Structure

```
UNIFORMS
├── ENEMY_ADMIN
│   ├── Vest
│   └── Pants
├── ENEMY_TROOP
│   ├── Vest
│   └── Pants
├── ENEMY_ELITE
│   ├── Vest
│   └── Pants
├── MILITIA_ROOKIE
│   ├── Vest
│   └── Pants
├── MILITIA_REGULAR
│   ├── Vest
│   └── Pants
└── MILITIA_ELITE
    ├── Vest
    └── Pants
```

## Fields

### `UNIFORMS/ENEMY_ADMIN/Vest`

- **Type:** `String`
- **Description:** Код цвета жилета для формы ENEMY_ADMIN. Это символьная константа цвета, используемая
  графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_ADMIN/Pants`

- **Type:** `String`
- **Description:** Код цвета штанов для формы ENEMY_ADMIN. Это символьная константа цвета,
  используемая графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_TROOP/Vest`

- **Type:** `String`
- **Description:** Код цвета жилета для формы ENEMY_TROOP. Это символьная константа цвета, используемая
  графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_TROOP/Pants`

- **Type:** `String`
- **Description:** Код цвета штанов для формы ENEMY_TROOP. Это символьная константа цвета,
  используемая графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_ELITE/Vest`

- **Type:** `String`
- **Description:** Код цвета жилета для формы ENEMY_ELITE. Это символьная константа цвета, используемая
  графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/ENEMY_ELITE/Pants`

- **Type:** `String`
- **Description:** Код цвета штанов для формы ENEMY_ELITE. Это символьная константа цвета,
  используемая графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_ROOKIE/Vest`

- **Type:** `String`
- **Description:** Код цвета жилета для формы MILITIA_ROOKIE. Это символьная константа цвета,
  используемая графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_ROOKIE/Pants`

- **Type:** `String`
- **Description:** Код цвета штанов для формы MILITIA_ROOKIE. Это символьная константа цвета,
  используемая графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_REGULAR/Vest`

- **Type:** `String`
- **Description:** Код цвета жилета для формы MILITIA_REGULAR. Это символьная константа цвета,
  используемая графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_REGULAR/Pants`

- **Type:** `String`
- **Description:** Код цвета штанов для формы MILITIA_REGULAR. Это символьная константа цвета,
  используемая графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_ELITE/Vest`

- **Type:** `String`
- **Description:** Код цвета жилета для формы MILITIA_ELITE. Это символьная константа цвета,
  используемая графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

### `UNIFORMS/MILITIA_ELITE/Pants`

- **Type:** `String`
- **Description:** Код цвета штанов для формы MILITIA_ELITE. Это символьная константа цвета,
  используемая графикой солдат.
- **Values:** Имя палитры с учётом регистра из комментария XML: жилет `YELLOWVEST`, `BLACKSHIRT`,
  `REDVEST`, `GREENVEST`, `JEANVEST`, `BLUEVEST`, `greyVEST`, `GYELLOWSHIRT`, `WHITEVEST`,
  `PURPLESHIRT` или `BROWNVEST`; штаны `GREENPANTS`, `BLACKPANTS`, `BEIGEPANTS`, `TANPANTS`,
  `JEANPANTS` или `BLUEPANTS`. Произвольный текст недопустим.
- **Default:** Обязательно; все шесть блоков формы должны задавать и `Vest`, и `Pants`.
- **Source:** game code (uniform-colour loader); table data and XML comment.
- **Confidence:** `source`

## Relations

- Имена цветов общие с графикой [SoldierProfiles](../Profiles/SoldierProfiles.md); это константы
  отображения, а не ссылки на предметы.

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
