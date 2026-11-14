---
schema: 1
lang: ru
family: items-attachments
title: "CompatibleFaceItems — совместимость предметов на лице"
tables:
  - CompatibleFaceItems
table_ids:
  - Items.CompatibleFaceItems
files:
  - Items/CompatibleFaceItems.xml
root: COMPATIBLEFACEITEMLIST
record: COMPATIBLEFACEITEM
key: compatiblefaceitemIndex + itemIndex
sources:
  - Compatible face-item table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# CompatibleFaceItems — совместимость предметов на лице

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`COMPATIBLEFACEITEMLIST/COMPATIBLEFACEITEM/compatiblefaceitemIndex`](#compatiblefaceitemlistcompatiblefaceitemcompatiblefaceitemindex)
  - [`COMPATIBLEFACEITEMLIST/COMPATIBLEFACEITEM/itemIndex`](#compatiblefaceitemlistcompatiblefaceitemitemindex)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`CompatibleFaceItems.xml` объявляет пары предметов на лице, которые можно носить вместе. Каждая строка — одна совместимая пара; обычно отношение записывается симметрично.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `CompatibleFaceItems` | `Items/CompatibleFaceItems.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
COMPATIBLEFACEITEMLIST
└── COMPATIBLEFACEITEM
    ├── compatiblefaceitemIndex
    └── itemIndex
```

## Fields

### `COMPATIBLEFACEITEMLIST/COMPATIBLEFACEITEM/compatiblefaceitemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID первого предмета на лице. Строка объявляет, что два предмета на лице можно носить вместе.
- **Values:** ID первого предмета на лице.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `COMPATIBLEFACEITEMLIST/COMPATIBLEFACEITEM/itemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID второго предмета на лице.
- **Values:** ID второго предмета на лице.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

## Relations

- Оба столбца указывают на предметы на лице в [Items](Items.md).
- Пара — положительное правило совместимости; она не определяет сами слоты предметов на лице.

## Localization

В этой таблице нет отображаемых строк.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы совместимых предметов на лице.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов.
- Описания используют имена XML-элементов, значения справочников и устаревшие подписи полей; записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Таблица хранит одно направление; загрузчик может рассматривать пару как симметричную, но схема XML этого не утверждает.
