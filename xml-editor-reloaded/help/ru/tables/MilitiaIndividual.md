---
schema: 1
lang: ru
family: militia
title: "MilitiaIndividual — данные отдельного ополченца"
tables:
  - MilitiaIndividual
table_ids:
  - MilitiaIndividual
files:
  - MilitiaIndividual.xml
root: MILITIAORIGINDATA
record: ORIGINDATA
key: none
sources:
  - Militia individual table data
  - MilitiaIndividual schema comment
status: reviewed
updated: 2026-11-12
---

# MilitiaIndividual — данные отдельного ополченца

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MILITIAORIGINDATA/ORIGINDATA/male_forename`](#militiaorigindataorigindatamale_forename)
  - [`MILITIAORIGINDATA/ORIGINDATA/male_surname`](#militiaorigindataorigindatamale_surname)
  - [`MILITIAORIGINDATA/ORIGINDATA/female_forename`](#militiaorigindataorigindatafemale_forename)
  - [`MILITIAORIGINDATA/ORIGINDATA/female_surname`](#militiaorigindataorigindatafemale_surname)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_REGMALE`](#militiaorigindataorigindatachance_bodytype_regmale)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_BIGMALE`](#militiaorigindataorigindatachance_bodytype_bigmale)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_STOCKYMALE`](#militiaorigindataorigindatachance_bodytype_stockymale)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_REGFEMALE`](#militiaorigindataorigindatachance_bodytype_regfemale)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_skin_PINKSKIN`](#militiaorigindataorigindatachance_skin_pinkskin)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_skin_TANSKIN`](#militiaorigindataorigindatachance_skin_tanskin)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_skin_DARKSKIN`](#militiaorigindataorigindatachance_skin_darkskin)
  - [`MILITIAORIGINDATA/ORIGINDATA/chance_skin_BLACKSKIN`](#militiaorigindataorigindatachance_skin_blackskin)
  - [`MILITIAORIGINDATA/ORIGINDATA/dailycost_green`](#militiaorigindataorigindatadailycost_green)
  - [`MILITIAORIGINDATA/ORIGINDATA/dailycost_regular`](#militiaorigindataorigindatadailycost_regular)
  - [`MILITIAORIGINDATA/ORIGINDATA/dailycost_elite`](#militiaorigindataorigindatadailycost_elite)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MilitiaIndividual.xml` определяет визуальные данные и данные имён для отдельного ополченца: шансы типа телосложения и кожи, дневное жалованье и пулы мужских и женских имён. Боевые способности он не меняет.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `MilitiaIndividual` | `MilitiaIndividual.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
MILITIAORIGINDATA
└── ORIGINDATA
    ├── male_forename
    ├── male_surname
    ├── female_forename
    ├── female_surname
    ├── chance_bodytype_REGMALE
    ├── chance_bodytype_BIGMALE
    ├── chance_bodytype_STOCKYMALE
    ├── chance_bodytype_REGFEMALE
    ├── chance_skin_PINKSKIN
    ├── chance_skin_TANSKIN
    ├── chance_skin_DARKSKIN
    ├── chance_skin_BLACKSKIN
    ├── dailycost_green
    ├── dailycost_regular
    └── dailycost_elite
```

`MILITIAORIGINDATA` не имеет стабильного ключа на строку. Строки `ORIGINDATA` выбираются по
происхождению и порядку документа; `key: none` фиксирует этот факт.

## Fields

### `MILITIAORIGINDATA/ORIGINDATA/male_forename`

- **Type:** `String` (`CHAR16`)
- **Description:** Пул мужских имён, используемый генератором случайных ополченцев.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Values:** Имя свободным текстом, усечённое до 29 символов; повторяющиеся элементы образуют пул.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/male_surname`

- **Type:** `String` (`CHAR16`)
- **Description:** Пул мужских фамилий.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Values:** Фамилия свободным текстом, усечённая до 29 символов; повторяющиеся элементы образуют пул.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/female_forename`

- **Type:** `String` (`CHAR16`)
- **Description:** Пул женских имён.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Values:** Имя свободным текстом, усечённое до 29 символов; повторяющиеся элементы образуют пул.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/female_surname`

- **Type:** `String` (`CHAR16`)
- **Description:** Пул женских фамилий.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Values:** Фамилия свободным текстом, усечённая до 29 символов; повторяющиеся элементы образуют пул.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_REGMALE`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что новый ополченец получит обычное мужское телосложение.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Процент `0..100`; загрузчик ограничивает меньшие значения до `0`, а большие — до `100`.
- **Values:** `0..100` после ограничения; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_BIGMALE`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что новый ополченец получит крупное мужское телосложение; шансы типов телосложения должны в сумме давать 100.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Процент `0..100`; загрузчик ограничивает меньшие значения до `0`, а большие — до `100`.
- **Values:** `0..100` после ограничения; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_STOCKYMALE`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что новый ополченец получит коренастое мужское телосложение.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Процент `0..100`; загрузчик ограничивает меньшие значения до `0`, а большие — до `100`.
- **Values:** `0..100` после ограничения; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_bodytype_REGFEMALE`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что новый ополченец получит обычное женское телосложение.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Процент `0..100`; загрузчик ограничивает меньшие значения до `0`, а большие — до `100`.
- **Values:** `0..100` после ограничения; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_skin_PINKSKIN`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что новый ополченец получит розовую кожу.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Процент `0..100`; загрузчик ограничивает меньшие значения до `0`, а большие — до `100`.
- **Values:** `0..100` после ограничения; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_skin_TANSKIN`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что новый ополченец получит загорелую кожу.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Процент `0..100`; загрузчик ограничивает меньшие значения до `0`, а большие — до `100`.
- **Values:** `0..100` после ограничения; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_skin_DARKSKIN`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что новый ополченец получит тёмную кожу.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Процент `0..100`; загрузчик ограничивает меньшие значения до `0`, а большие — до `100`.
- **Values:** `0..100` после ограничения; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/chance_skin_BLACKSKIN`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что новый ополченец получит чёрную кожу; шансы кожи должны в сумме давать 100.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Процент `0..100`; загрузчик ограничивает меньшие значения до `0`, а большие — до `100`.
- **Values:** `0..100` после ограничения; загрузчик сохраняет это поле как `UINT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/dailycost_green`

- **Type:** `Integer` (`UINT16`)
- **Description:** Дневное жалованье зелёного ополченца.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Дневное жалованье в деньгах; большие значения означают большее жалованье, а отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/dailycost_regular`

- **Type:** `Integer` (`UINT16`)
- **Description:** Дневное жалованье обычного ополченца.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Дневное жалованье в деньгах; большие значения означают большее жалованье, а отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

### `MILITIAORIGINDATA/ORIGINDATA/dailycost_elite`

- **Type:** `Integer` (`UINT16`)
- **Description:** Дневное жалованье элитного ополченца.
- **Only used with:** INDIVIDUAL_MILITIA.
- **Numeric semantics:** Дневное жалованье в деньгах; большие значения означают большее жалованье, а отрицательные значения не имеют смысла.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_MilitiaIndividual.cpp`); table data.
- **Confidence:** `source`

## Relations

- В таблице нет ключей предметов или профилей; она выбирает происхождение ополчения через собственный порядок строк.
- Опция `INDIVIDUAL_MILITIA` управляет тем, используются ли эти данные.

## Localization

Пулы имён и фамилий, а также значения телосложения, кожи и жалованья не локализуются; это данные для генератора.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы отдельного ополченца.
- Комментарий схемы MilitiaIndividual.
- Типы полей: данные таблицы и имена полей; значения enum берутся из комментариев схемы.
- Описания осторожны там, где формула не указана; наблюдаемые диапазоны значений не используются.

## Open questions

Генерация имени и внешности управляется кодом; таблица поставляет пулы и веса.
