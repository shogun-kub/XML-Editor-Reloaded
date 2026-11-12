---
schema: 1
lang: ru
family: items-attachments
title: "AttachmentComboMerges — результаты комбинированных вложений"
tables:
  - AttachmentComboMerges
table_ids:
  - Items.AttachmentComboMerges
files:
  - Items/AttachmentComboMerges.xml
root: ATTACHMENTCOMBOMERGELIST
record: ATTACHMENTCOMBOMERGE
key: uiIndex
sources:
  - Game code (combo-merge loader and merge application)
  - Attachment combo-merge table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# AttachmentComboMerges — результаты комбинированных вложений

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/uiIndex`](#attachmentcombomergelistattachmentcombomergeuiindex)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usItem`](#attachmentcombomergelistattachmentcombomergeusitem)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment1`](#attachmentcombomergelistattachmentcombomergeusattachment1)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment2`](#attachmentcombomergelistattachmentcombomergeusattachment2)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment3`](#attachmentcombomergelistattachmentcombomergeusattachment3)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment4`](#attachmentcombomergelistattachmentcombomergeusattachment4)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment5`](#attachmentcombomergelistattachmentcombomergeusattachment5)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment6`](#attachmentcombomergelistattachmentcombomergeusattachment6)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment7`](#attachmentcombomergelistattachmentcombomergeusattachment7)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment8`](#attachmentcombomergelistattachmentcombomergeusattachment8)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment9`](#attachmentcombomergelistattachmentcombomergeusattachment9)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment10`](#attachmentcombomergelistattachmentcombomergeusattachment10)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment11`](#attachmentcombomergelistattachmentcombomergeusattachment11)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment12`](#attachmentcombomergelistattachmentcombomergeusattachment12)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment13`](#attachmentcombomergelistattachmentcombomergeusattachment13)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment14`](#attachmentcombomergelistattachmentcombomergeusattachment14)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment15`](#attachmentcombomergelistattachmentcombomergeusattachment15)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment16`](#attachmentcombomergelistattachmentcombomergeusattachment16)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment17`](#attachmentcombomergelistattachmentcombomergeusattachment17)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment18`](#attachmentcombomergelistattachmentcombomergeusattachment18)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment19`](#attachmentcombomergelistattachmentcombomergeusattachment19)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment20`](#attachmentcombomergelistattachmentcombomergeusattachment20)
  - [`ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usResult`](#attachmentcombomergelistattachmentcombomergeusresult)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AttachmentComboMerges.xml` определяет базовый предмет и до двадцати вложений, а также предмет, который получается при наличии полной комбинации. Это аналог `Merges` для комбинаций вложений.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `AttachmentComboMerges` | `Items/AttachmentComboMerges.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
ATTACHMENTCOMBOMERGELIST
└── ATTACHMENTCOMBOMERGE
    ├── uiIndex
    ├── usItem
    ├── usAttachment1
    ├── usAttachment2
    ├── usAttachment3
    ├── usAttachment4
    ├── usAttachment5
    ├── usAttachment6
    ├── usAttachment7
    ├── usAttachment8
    ├── usAttachment9
    ├── usAttachment10
    ├── usAttachment11
    ├── usAttachment12
    ├── usAttachment13
    ├── usAttachment14
    ├── usAttachment15
    ├── usAttachment16
    ├── usAttachment17
    ├── usAttachment18
    ├── usAttachment19
    ├── usAttachment20
    └── usResult
```

## Fields

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/uiIndex`

- **Type:** `Integer` (`UINT32`)
- **Description:** Индекс строки комбинированного слияния. Загрузчик сохраняет строку в `AttachmentComboMerge[uiIndex]`.
- **Values:** Неотрицательное целое меньше `MAXITEMS` (`16001`); строки с таким или большим индексом отбрасываются, дубликаты перезаписывают более раннюю строку, а поиск останавливается на первой пустой строке, поэтому индексы должны быть непрерывными начиная с `0`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usItem`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета-основы, на котором комбинируются вложения. Строка с ID предмета `0` считается концом таблицы.
- **Values:** ID предмета-основы; `0` завершает поиск.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment1`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 1. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment2`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 2. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment3`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 3. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment4`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 4. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment5`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 5. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment6`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 6. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment7`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 7. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment8`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 8. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment9`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 9. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment10`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 10. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment11`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 11. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment12`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 12. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment13`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 13. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment14`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 14. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment15`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 15. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment16`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 16. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment17`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 17. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment18`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 18. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment19`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 19. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usAttachment20`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета для слота вложения 20. Ноль означает, что слот не используется. Загрузчик заполняет 20 слотов в порядке документа, игнорируя цифру в имени элемента, и строка требует наличия всех ненулевых слотов.
- **Values:** ID предмета; `0` оставляет слот пустым.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `ATTACHMENTCOMBOMERGELIST/ATTACHMENTCOMBOMERGE/usResult`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID предмета, получаемого, когда все перечисленные вложения присутствуют на базовом предмете. Слияние применяется, только если этот предмет допустим (`ItemIsLegal`).
- **Values:** ID предмета-результата комбинации.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_ComboMergeInfo.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

## Relations

- `usItem`, `usAttachment1..20` и `usResult` указывают на [Items](Items.md).
- Порядок вложений не важен: каждый ненулевой слот ищется по ID предмета, и строка совпадает, когда прикреплены все они.
- Строки проверяются в порядке возрастания индекса, и побеждает первая совпавшая строка; сканирование останавливается на первой строке, где `usItem` равен `0`.
- 20 слотов вложений заполняются в порядке документа: загрузчик игнорирует цифру в имени элемента и не проверяет границы счётчика слотов.
- `usResult` должен быть допустимым предметом, иначе строка пропускается.

## Localization

В этой таблице нет отображаемых строк; названия предметов берутся из `Items`.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Игровой код: `XML_ComboMergeInfo.cpp` (загрузчик), `Items.cpp` (`GetAttachmentComboMerge` и `PerformAttachmentComboMerge`), `Item Types.h` (`ComboMergeInfoStruct`, `MAX_DEFAULT_ATTACHMENTS`).
- Данные таблицы комбинированных слияний вложений.
- Метаданные предметов.
- Типы и ограничения полей: члены `ComboMergeInfoStruct` и приведения в загрузчике.
- Описания охватывают путь кода, который использует каждое поле; количество строк и наблюдаемые диапазоны данных не используются.

## Open questions

Загрузчик принимает любой элемент, имя которого содержит `usAttachment`, и никогда их не считает, поэтому более 20 элементов вложений переполняют структуру строки; формат таблицы не выражает этот случай.
