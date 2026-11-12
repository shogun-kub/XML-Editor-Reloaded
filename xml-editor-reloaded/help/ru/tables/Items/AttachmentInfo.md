---
schema: 1
lang: ru
family: items-attachments
title: "AttachmentInfo — проверки навыка для вложений"
tables:
  - AttachmentInfo
table_ids:
  - Items.AttachmentInfo
files:
  - Items/AttachmentInfo.xml
root: ATTACHMENTINFOLIST
record: ATTACHMENTINFO
key: uiIndex
sources:
  - Attachment info table data
  - Item and skill lookup tables
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# AttachmentInfo — проверки навыка для вложений

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/uiIndex`](#attachmentinfolistattachmentinfouiindex)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/usItem`](#attachmentinfolistattachmentinfousitem)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/uiItemClass`](#attachmentinfolistattachmentinfouiitemclass)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/bAttachmentSkillCheck`](#attachmentinfolistattachmentinfobattachmentskillcheck)
  - [`ATTACHMENTINFOLIST/ATTACHMENTINFO/bAttachmentSkillCheckMod`](#attachmentinfolistattachmentinfobattachmentskillcheckmod)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AttachmentInfo.xml` добавляет метаданные, специфичные для вложений: к какому классу предметов может прикрепляться вложение и какая проверка навыка (с модификатором) требуется.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `AttachmentInfo` | `Items/AttachmentInfo.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
ATTACHMENTINFOLIST
└── ATTACHMENTINFO
    ├── uiIndex
    ├── usItem
    ├── uiItemClass
    ├── bAttachmentSkillCheck
    └── bAttachmentSkillCheckMod
```

## Fields

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/uiIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс строки информации о вложении и идентификатор записи XML.
- **Values:** Неотрицательное целое; уникально в пределах файла.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/usItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета вложения, описываемого этой строкой.
- **Values:** ID предмета в [Items](Items.md); предмет должен быть вложением.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/uiItemClass`

- **Type:** `Integer` (`UINT32`)
- **Description:** Битовая маска классов предметов, к которым может прикрепляться это вложение. Класс предмета-носителя должен пересекаться с этой маской.
- **Numeric semantics:** Битовая маска или перечислимый код; смысл несут отдельные биты/варианты, поэтому большее число не означает просто «больше». Отрицательные значения не имеют смысла.
- **Values:** Битовая маска ID из `Lookup.ItemClass`. Вложение можно установить на предметы, класс которых пересекается с этой маской.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/bAttachmentSkillCheck`

- **Type:** `Integer` (`INT8`)
- **Description:** Проверка навыка, используемая при прикреплении предмета. `None` отключает проверку.
- **Values:** ID из `Lookup.SkillCheckType`: `0` None, `1` Lockpick, `2` Elec. Lockpick, `3` Attach Timed Detonator, `4` Attach Remote Detonator, `5` Plant Timed Bomb, `6` Plant Remote Bomb, `7` Open With Crowbar, `8` Smash Door, `9` Disarm Trap, `10` Unjam Gun, `11` Notice Dart, `12` Lie to Queen, `13` Attach Special Item, `14` Attach Special Elec. Item, `15` Disarm Elec. Trap.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** attachment info table data; field metadata.
- **Confidence:** `source`

### `ATTACHMENTINFOLIST/ATTACHMENTINFO/bAttachmentSkillCheckMod`

- **Type:** `Integer` (`INT8`)
- **Description:** Модификатор, добавляемый к выбранной проверке навыка вложения.
- **Numeric semantics:** Знаковый модификатор, добавляемый напрямую к значению навыка: положительные значения облегчают проверку, отрицательные — усложняют.
- **Values:** Целочисленный модификатор, добавляемый к выбранной проверке навыка; загрузчик сохраняет его как `INT8`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`SkillCheck`, `iChance = iSkill + bChanceMod`); attachment info table data.
- **Confidence:** `source`

## Relations

- `usItem` указывает на [Items](Items.md).
- `uiItemClass` — битовая маска [Lookup.ItemClass](../Lookup.md#itemclass).
- `bAttachmentSkillCheck` использует [Lookup.SkillCheckType](../Lookup.md#skillchecktype).

## Localization

В этой таблице нет отображаемых строк.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы информации о вложениях.
- Таблицы справочников предметов и навыков.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов.
- Описания используют имена XML-элементов, значения справочников и устаревшие подписи полей; записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Для `bAttachmentSkillCheckMod` открытых вопросов не осталось: код проверки навыка добавляет модификатор напрямую к значению навыка, поэтому положительные значения помогают, а отрицательные мешают.
