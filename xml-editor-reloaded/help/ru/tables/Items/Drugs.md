---
schema: 1
lang: ru
family: items-drugs
title: "Drugs — эффекты наркотиков"
tables:
  - Drugs
table_ids:
  - Items.Drugs
files:
  - Items/Drugs.xml
root: DRUGSLIST
record: DRUG
key: uiIndex
sources:
  - Drug table data
  - Drug schema comment
  - Game code (drug loader, drug table struct and effect application)
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# Drugs — эффекты наркотиков

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`DRUGSLIST/DRUG/uiIndex`](#drugslistdruguiindex)
  - [`DRUGSLIST/DRUG/szName`](#drugslistdrugszname)
  - [`DRUGSLIST/DRUG/opinionevent`](#drugslistdrugopinionevent)
  - [`DRUGSLIST/DRUG/DRUG_EFFECT/effect`](#drugslistdrugdrug_effecteffect)
  - [`DRUGSLIST/DRUG/DRUG_EFFECT/duration`](#drugslistdrugdrug_effectduration)
  - [`DRUGSLIST/DRUG/DRUG_EFFECT/size`](#drugslistdrugdrug_effectsize)
  - [`DRUGSLIST/DRUG/DRUG_EFFECT/chance`](#drugslistdrugdrug_effectchance)
  - [`DRUGSLIST/DRUG/DISEASE_EFFECT/disease`](#drugslistdrugdisease_effectdisease)
  - [`DRUGSLIST/DRUG/DISEASE_EFFECT/size`](#drugslistdrugdisease_effectsize)
  - [`DRUGSLIST/DRUG/DISEASE_EFFECT/chance`](#drugslistdrugdisease_effectchance)
  - [`DRUGSLIST/DRUG/DISABILITY_EFFECT/disability`](#drugslistdrugdisability_effectdisability)
  - [`DRUGSLIST/DRUG/DISABILITY_EFFECT/duration`](#drugslistdrugdisability_effectduration)
  - [`DRUGSLIST/DRUG/DISABILITY_EFFECT/chance`](#drugslistdrugdisability_effectchance)
  - [`DRUGSLIST/DRUG/PERSONALITY_EFFECT/personality`](#drugslistdrugpersonality_effectpersonality)
  - [`DRUGSLIST/DRUG/PERSONALITY_EFFECT/duration`](#drugslistdrugpersonality_effectduration)
  - [`DRUGSLIST/DRUG/PERSONALITY_EFFECT/chance`](#drugslistdrugpersonality_effectchance)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Drugs.xml` определяет эффекты наркотиков: изменения характеристик, изменения болезней, временные недуги и изменения характера. Наркотик может содержать несколько блоков эффектов одного или разных типов.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Drugs` | `Items/Drugs.xml` | В v4 и v5 совпадают пути полей и, в справочных данных, значения. |

## Structure

```
DRUGSLIST
└── DRUG
    ├── uiIndex
    ├── szName
    ├── opinionevent
    ├── DRUG_EFFECT
    │   ├── effect
    │   ├── duration
    │   ├── size
    │   └── chance
    ├── DISEASE_EFFECT
    │   ├── disease
    │   ├── size
    │   └── chance
    ├── DISABILITY_EFFECT
    │   ├── disability
    │   ├── duration
    │   └── chance
    └── PERSONALITY_EFFECT
        ├── personality
        ├── duration
        └── chance
```

## Fields

### `DRUGSLIST/DRUG/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс строки наркотика и идентификатор XML-записи. На него ссылается поле типа наркотика предмета в [Items](Items.md).
- **Values:** `0..99`; массив времени выполнения `NewDrug` имеет `NEW_DRUGS_MAX` (`100`) слотов. Загрузчик сохраняет это поле как `UINT8` и не проверяет диапазон перед записью.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader and `NewDrug` array); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/szName`

- **Type:** `String` (`CHAR16`)
- **Description:** Имя, копируемое в `DRUG.szName`. Комментарий загрузчика называет его информационным, и ни один код отображения или игровой логики его не читает; видимое игроку имя несёт строка предмета.
- **Values:** Свободный текст, усечённый до буфера `CHAR16` в 80 символов.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (drug loader and `DRUG` struct); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/opinionevent`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено и динамические мнения включены, употребление наркотика запускает событие мнения о зависимости, которое другие наёмники оценивают отрицательно. Событие срабатывает только тогда, когда был применён хотя бы один блок `DRUG_EFFECT`.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; drug application and dynamic-opinion code); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DRUG_EFFECT/effect`

- **Type:** `Integer` (`UINT8`)
- **Description:** Характеристика или ресурс, изменяемые этим эффектом.
- **Values:** ID из enum эффектов наркотиков: `0` HP, `1` BP, `2` AP, `3` Morale, `4` Physical resistance, `5` Strength, `6` Agility, `7` Dexterity, `8` Wisdom.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; `DRUG_EFFECT_*` enum); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DRUG_EFFECT/duration`

- **Type:** `Integer` (`UINT16`)
- **Description:** Длительность эффекта в ходах. Игра уменьшает её во время обновления наркотиков в конце хода.
- **Numeric semantics:** Время; большие значения означают более длительный эффект.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; end-turn drug update); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DRUG_EFFECT/size`

- **Type:** `Integer` (`INT16`)
- **Description:** Сила эффекта. Положительные значения повышают затронутую характеристику или ресурс, отрицательные понижают.
- **Numeric semantics:** Знаковая сила; для только что применённого эффекта игра умножает её на процент использованного предмета перед сохранением. Поставляемые данные используют отрицательные значения для таких эффектов, как штраф к Wisdom у алкоголя.
- **Values:** `-32768..32767`; загрузчик сохраняет это поле как `INT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; effect application); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DRUG_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что этот эффект будет применён при употреблении наркотика. `0` и `100` оба означают, что эффект применяется всегда; значения `1..99` — это процент.
- **Numeric semantics:** Большие значения до `99` означают больший шанс; `0` — сокращение «применять всегда».
- **Values:** `0..100`; `0` и `100` применяются безусловно.
- **Default:** `100`, если элемент отсутствует; загрузчик инициализирует каждый блок эффекта значением `100`.
- **Source:** game code (drug loader and chance check); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISEASE_EFFECT/disease`

- **Type:** `Integer` (`UINT8`)
- **Description:** Строка болезни, на которую влияет этот наркотик. Значение индексирует таблицу болезней времени выполнения.
- **Only used with:** Система болезней.
- **Values:** Индекс строки болезни в [Disease](../Disease.md); `0` — арулканская чума, а не пустая строка.
- **Default:** `0` (арулканская чума), если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; disease-point application); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISEASE_EFFECT/size`

- **Type:** `Integer` (`INT32`)
- **Description:** Добавляемые очки болезни. Игра умножает значение на процент использованного предмета, затем прибавляет его; положительные значения усиливают болезнь, отрицательные излечивают её, а результат ограничивается порогами вспышки и полного заражения болезни.
- **Only used with:** Система болезней.
- **Numeric semantics:** Знаковые очки; большие положительные значения продвигают болезнь быстрее, а отрицательные уменьшают её.
- **Values:** `-2147483648..2147483647`; загрузчик сохраняет это поле как `INT32`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; `AddDiseasePoints`); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISEASE_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что этот эффект болезни будет применён. `0` и `100` оба означают, что эффект применяется всегда; значения `1..99` — это процент.
- **Only used with:** Система болезней.
- **Numeric semantics:** Большие значения до `99` означают больший шанс; `0` — сокращение «применять всегда».
- **Values:** `0..100`; `0` и `100` применяются безусловно.
- **Default:** `100`, если элемент отсутствует; загрузчик инициализирует каждый блок эффекта значением `100`.
- **Source:** game code (drug loader and chance check); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISABILITY_EFFECT/disability`

- **Type:** `Integer` (`UINT8`)
- **Description:** Недуг, добавляемый на время действия эффекта. Одновременно активен только один недуг: более поздний другой недуг заменяет предыдущий, а тот же самый недуг накапливает длительность.
- **Values:** ID недуга: `0` None, `1` Heat intolerant, `2` Nervous, `3` Claustrophobic, `4` Nonswimmer, `5` Fear of insects, `6` Forgetful, `7` Psycho, `8` Deaf, `9` Shortsighted, `10` Hemophiliac, `11` Afraid of heights, `12` Self-harm.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; disability enum and application); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISABILITY_EFFECT/duration`

- **Type:** `Integer` (`UINT16`)
- **Description:** Длительность недуга в ходах. Игра масштабирует её на процент использованного предмета.
- **Numeric semantics:** Время; большие значения означают более длительный недуг.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; disability application and end-turn update); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/DISABILITY_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что этот недуг будет применён. `0` и `100` оба означают, что эффект применяется всегда; значения `1..99` — это процент.
- **Numeric semantics:** Большие значения до `99` означают больший шанс; `0` — сокращение «применять всегда».
- **Values:** `0..100`; `0` и `100` применяются безусловно.
- **Default:** `100`, если элемент отсутствует; загрузчик инициализирует каждый блок эффекта значением `100`.
- **Source:** game code (drug loader and chance check); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/PERSONALITY_EFFECT/personality`

- **Type:** `Integer` (`UINT8`)
- **Description:** Черта характера, добавляемая на время действия эффекта. Одновременно активна только одна черта: более поздняя другая черта заменяет предыдущую, а та же самая черта накапливает длительность.
- **Values:** ID черты характера: `0` Normal, `1` Sociable, `2` Loner, `3` Optimist, `4` Assertive, `5` Intellectual, `6` Primitive, `7` Aggressive, `8` Phlegmatic, `9` Dauntless, `10` Pacifist, `11` Malicious, `12` Show-off. Enum движка также определяет `13` Coward, хотя комментарий таблицы заканчивается на `12`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; character-trait enum and application); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/PERSONALITY_EFFECT/duration`

- **Type:** `Integer` (`UINT16`)
- **Description:** Длительность эффекта характера в ходах. Игра масштабирует её на процент использованного предмета.
- **Numeric semantics:** Время; большие значения означают более длительное изменение характера.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (drug loader; personality application and end-turn update); drug table data.
- **Confidence:** `source`

### `DRUGSLIST/DRUG/PERSONALITY_EFFECT/chance`

- **Type:** `Integer` (`UINT8`)
- **Description:** Шанс, что этот эффект характера будет применён. `0` и `100` оба означают, что эффект применяется всегда; значения `1..99` — это процент.
- **Numeric semantics:** Большие значения до `99` означают больший шанс; `0` — сокращение «применять всегда».
- **Values:** `0..100`; `0` и `100` применяются безусловно.
- **Default:** `100`, если элемент отсутствует; загрузчик инициализирует каждый блок эффекта значением `100`.
- **Source:** game code (drug loader and chance check); drug table data.
- **Confidence:** `source`

## Relations

- `uiIndex` упоминается значением `DrugType` предмета в [Items](Items.md); игра использует это значение напрямую как индекс в `NewDrug`.
- `DISEASE_EFFECT/disease` ссылается на строку в [Disease](../Disease.md).
- Блоки эффектов могут повторяться. Повторяющиеся блоки `DRUG_EFFECT` одного типа объединяют свою длительность и силу; для недугов и характеров активен только последний успешно применённый эффект, а повторный идентичный эффект добавляет длительность.

## Localization

`szName` только сохраняется и не отображается игрой; ID эффектов и значения enum не локализуются.

## Versions

В v4 и v5 совпадают пути полей, и справочные файлы содержат одинаковые значения.

## Sources

- Данные таблицы наркотиков.
- Комментарий схемы наркотиков.
- Игровой код (загрузчик наркотиков, структура таблицы наркотиков, применение эффектов и обработка болезней).
- Метаданные предметов.
- Типы и пределы полей: метаданные предметов.
- Описания используют игровой код и имена XML-элементов; записи, помеченные `inferred`, не заявляют проверенную по коду формулу.

## Open questions

Повторяющиеся блоки `DRUG_EFFECT` одного типа объединяются в коде, а не заменяются: длительности усредняются, а результирующая сила пересчитывается из суммарного итога эффекта. Комментарий XML не описывает это объединение, и таблица не может напрямую выразить веса объединения.

Метаданные редактора сопоставляют `Items.DrugType` с `Lookup.DrugType`, чьи ID — битовые флаги, но игра использует значение предмета как прямой индекс в `NewDrug`; поставляемые значения предметов — последовательные индексы. Таблица фиксирует поведение кода и не разрешает это расхождение метаданных.
