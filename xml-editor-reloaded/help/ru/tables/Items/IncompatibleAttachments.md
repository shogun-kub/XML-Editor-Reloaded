---
schema: 1
lang: ru
family: items-attachments
title: "IncompatibleAttachments — исключения навески"
tables:
  - IncompatibleAttachments
table_ids:
  - Items.IncompatibleAttachments
files:
  - Items/IncompatibleAttachments.xml
root: INCOMPATIBLEATTACHMENTLIST
record: INCOMPATIBLEATTACHMENT
key: itemIndex + incompatibleattachmentIndex
sources:
  - Incompatible attachment table data
  - Item metadata
status: reviewed
updated: 2026-11-14
---

# IncompatibleAttachments — исключения навески

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`INCOMPATIBLEATTACHMENTLIST/INCOMPATIBLEATTACHMENT/itemIndex`](#incompatibleattachmentlistincompatibleattachmentitemindex)
  - [`INCOMPATIBLEATTACHMENTLIST/INCOMPATIBLEATTACHMENT/incompatibleattachmentIndex`](#incompatibleattachmentlistincompatibleattachmentincompatibleattachmentindex)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`IncompatibleAttachments.xml` объявляет пары навески, которые нельзя устанавливать вместе. Пара — правило исключения, а не замена маскам классов `AttachmentInfo`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `IncompatibleAttachments` | `Items/IncompatibleAttachments.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
INCOMPATIBLEATTACHMENTLIST
└── INCOMPATIBLEATTACHMENT
    ├── itemIndex
    └── incompatibleattachmentIndex
```

## Fields

### `INCOMPATIBLEATTACHMENTLIST/INCOMPATIBLEATTACHMENT/itemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета навески. Строка объявляет, что эту навеску нельзя устанавливать вместе с несовместимой навеской.
- **Values:** ID предмета навески.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `INCOMPATIBLEATTACHMENTLIST/INCOMPATIBLEATTACHMENT/incompatibleattachmentIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета навески, который нельзя устанавливать вместе с `itemIndex`.
- **Values:** ID предмета навески, который нельзя устанавливать вместе с `itemIndex`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

## Relations

- Оба столбца указывают на [Items](Items.md).
- Пара — правило исключения; таблица не добавляет положительного отношения совместимости.

## Localization

В этой таблице нет отображаемых строк.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы несовместимой навески.
- Метаданные предметов.
- Типы и ограничения полей: метаданные предметов.
- Описания используют имена XML-элементов, значения справочников и устаревшие подписи полей; записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Таблица записывается как неупорядоченные пары; она не указывает, является ли исключение автоматически симметричным во время выполнения.
