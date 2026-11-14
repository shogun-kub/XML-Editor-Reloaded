---
schema: 1
lang: ru
family: items-attachments
title: "Attachments — сопоставления вложений с предметами"
tables:
  - Attachments
table_ids:
  - Items.Attachments
files:
  - Items/Attachments.xml
root: ATTACHMENTLIST
record: ATTACHMENT
key: attachmentIndex + itemIndex
sources:
  - Attachment table data
  - Item metadata
  - Attachment-system field names
status: reviewed
updated: 2026-11-12
---

# Attachments — сопоставления вложений с предметами

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ATTACHMENTLIST/ATTACHMENT/attachmentIndex`](#attachmentlistattachmentattachmentindex)
  - [`ATTACHMENTLIST/ATTACHMENT/itemIndex`](#attachmentlistattachmentitemindex)
  - [`ATTACHMENTLIST/ATTACHMENT/APCost`](#attachmentlistattachmentapcost)
  - [`ATTACHMENTLIST/ATTACHMENT/NASOnly`](#attachmentlistattachmentnasonly)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Attachments.xml` сопоставляет предмет-вложение предмету-носителю и задаёт стоимость в ОД для этого сопоставления. Таблица используется совместно старой системой вложений (OAS) и новой системой вложений (NAS); `NASOnly` помечает сопоставления, существующие только в NAS.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Attachments` | `Items/Attachments.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
ATTACHMENTLIST
└── ATTACHMENT
    ├── attachmentIndex
    ├── itemIndex
    ├── APCost
    └── NASOnly
```

## Fields

### `ATTACHMENTLIST/ATTACHMENT/attachmentIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета вложения. Предмет должен быть вложением; этот столбец определяет, какое вложение сопоставляется.
- **Values:** ID предмета в [Items](Items.md); предмет должен быть вложением.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `ATTACHMENTLIST/ATTACHMENT/itemIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета-носителя (оружия, брони или другого предмета, к которому можно прикреплять), принимающего вложение.
- **Values:** ID предмета в [Items](Items.md); предмет-носитель должен принимать вложение.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `ATTACHMENTLIST/ATTACHMENT/APCost`

- **Type:** `Integer` (`UINT16`)
- **Description:** Стоимость в ОД, взимаемая при использовании сопоставления вложения. Это стоимость прикрепления или отсоединения этого сопоставления, а не стоимость выстрела из предмета-носителя.
- **Numeric semantics:** Стоимость в очках действия; загрузчик пересчитывает её с помощью `DynamicAdjustAPConstants`.
- **Values:** Стоимость в ОД; более узкий диапазон в метаданных поля не объявлен.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (attachment/face-gear loader); table data.
- **Confidence:** `source`

### `ATTACHMENTLIST/ATTACHMENT/NASOnly`

- **Type:** `Boolean` (`Boolean`)
- **Description:** Если установлено, сопоставление существует только в новой системе вложений (NAS); старая система вложений (OAS) его игнорирует.
- **Only used with:** NAS.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует; сопоставление доступно в обеих системах вложений.
- **Source:** attachment table data; field metadata.
- **Confidence:** `source`

## Relations

- `attachmentIndex` и `itemIndex` указывают на [Items](Items.md).
- `NASOnly` — переключатель только для NAS; OAS игнорирует сопоставление, когда он установлен.
- Комбинации вложений описаны в [AttachmentComboMerges](AttachmentComboMerges.md); исключения — в [IncompatibleAttachments](IncompatibleAttachments.md).

## Localization

В этой таблице нет отображаемых строк; названия предметов берутся из `Items`.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы вложений.
- Метаданные предметов.
- Имена полей системы вложений.
- Типы и ограничения полей: метаданные предметов.
- Описания используют имена XML-элементов, значения справочников и устаревшие подписи полей; записи с пометкой `inferred` не заявляют проверенную кодом формулу.

## Open questions

Точное событие, за которое взимается `APCost` (прикрепление, отсоединение или оба), в таблице не указано; описание сохраняет смысл на уровне сопоставления.
