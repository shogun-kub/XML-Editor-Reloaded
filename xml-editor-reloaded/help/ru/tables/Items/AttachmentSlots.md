---
schema: 1
lang: ru
family: items-attachments
title: "AttachmentSlots — слоты навески NAS"
tables:
  - AttachmentSlots
table_ids:
  - Items.AttachmentSlots
files:
  - Items/AttachmentSlots.xml
root: ATTACHMENTSLOTLIST
record: ATTACHMENTSLOT
key: uiSlotIndex
sources:
  - Attachment slot table data
  - NAS lookup tables
  - Item metadata
status: reviewed
updated: 2026-11-14
---

# AttachmentSlots — слоты навески NAS

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/uiSlotIndex`](#attachmentslotlistattachmentslotuislotindex)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/szSlotName`](#attachmentslotlistattachmentslotszslotname)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasAttachmentClass`](#attachmentslotlistattachmentslotnasattachmentclass)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasLayoutClass`](#attachmentslotlistattachmentslotnaslayoutclass)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosX`](#attachmentslotlistattachmentslotusdescpanelposx)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosY`](#attachmentslotlistattachmentslotusdescpanelposy)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fMultiShot`](#attachmentslotlistattachmentslotfmultishot)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fBigSlot`](#attachmentslotlistattachmentslotfbigslot)
  - [`ATTACHMENTSLOTLIST/ATTACHMENTSLOT/ubPocketMapping`](#attachmentslotlistattachmentslotubpocketmapping)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`AttachmentSlots.xml` определяет слоты, используемые новой системой навески (NAS): их имена,
принимаемые классы навески, классы компоновки, позиции на панели и привязку к карманам.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `AttachmentSlots` | `Items/AttachmentSlots.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
ATTACHMENTSLOTLIST
└── ATTACHMENTSLOT
    ├── uiSlotIndex
    ├── szSlotName
    ├── nasAttachmentClass
    ├── nasLayoutClass
    ├── usDescPanelPosX
    ├── usDescPanelPosY
    ├── fMultiShot
    ├── fBigSlot
    └── ubPocketMapping
```

`uiSlotIndex` — ключ: загрузчик сохраняет каждую строку по этому индексу массива. Поиск слотов NAS
начинается с индекса `1` и останавливается на первом индексе, где `uiSlotIndex` равен `0`, поэтому
индексы слотов должны быть непрерывными начиная с `1`, иначе поздние слоты недостижимы.

## Fields

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/uiSlotIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Идентификатор слота навески. Строки сохраняются по этому индексу, и код
  отображения и компоновки слотов ссылается на слот по этому значению.
- **Only used with:** NAS.
- **Numeric semantics:** Индекс массива. Загрузчик разбирает текст через `strtoul` и сохраняет
  результат в члене `UINT16`; значения вне границы `MAXITEMS` отбрасываются, а повторяющиеся индексы
  перезаписывают более ранние строки. Поиск слотов начинается с `1` и останавливается на первом
  индексе, где `uiSlotIndex` равен `0`, поэтому индекс `0` не предлагается как слот.
- **Values:** `0..16000` (граница `MAXITEMS`); загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`, поэтому строка
  сохраняется по индексу `0` и не предлагается.
- **Source:** game code (attachment-slot loader; NAS slot lookup); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/szSlotName`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое имя слота; используется как текст быстрой справки (подсказки) для
  пустого слота навески в описании предмета.
- **Only used with:** NAS.
- **Values:** Свободный текст, преобразуемый из UTF-8; сохраняется в буфере на 200 символов, более
  длинный текст обрезается.
- **Default:** Пустая строка, если поле отсутствует.
- **Source:** game code (attachment-slot loader; item-description fast help); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasAttachmentClass`

- **Type:** `Flags` (`UINT64`)
- **Description:** Битовая маска классов навески, которые принимает слот. Поиск слотов NAS проверяет
  её побитовым И с каждым битом класса навески, который принимает предмет.
- **Only used with:** NAS.
- **Numeric semantics:** Битовая маска; смысл несут отдельные биты, поэтому большее число не означает
  просто «больше». `0` означает None.
- **Values:** Битовая маска ID из `Lookup.NasAttachmentClass`: `0` None, `1` Default, `2` Barrel,
  `4` Laser, `8` Sight, `16` Scope, `32` Stock, `64` Ammo, `128` Internal, `256` External,
  `512` Underbarrel, `1024` Grenade, `2048` Rocket, `4096` MOLLE Small, `8192` MOLLE Medium,
  `16384` Batteries. Объединяйте биты через OR.
- **Default:** `0` (None), если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (attachment-slot loader; NAS slot selection); `Lookup.NasAttachmentClass`; attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/nasLayoutClass`

- **Type:** `Flags` (`UINT64`)
- **Description:** Битовая маска классов компоновки, к которым принадлежит слот. Поиск слотов NAS
  проверяет её побитовым И с классом компоновки предмета. Если у предмета нет слота для конкретной
  компоновки, используется слот с классом компоновки `1` (Weapons) как откат по умолчанию.
- **Only used with:** NAS.
- **Numeric semantics:** Битовая маска; смысл несут отдельные биты, поэтому большее число не означает
  просто «больше». `0` означает None.
- **Values:** Битовая маска ID из `Lookup.NasLayoutClass`: `0` None, `1` Weapons,
  `2` Multi-Shot Launchers, `4` Molle Leg Rigs, `8` Molle Vests, `16` Molle Combat Packs,
  `32` Molle Backpacks. Объединяйте биты через OR.
- **Default:** `0` (None), если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (attachment-slot loader; NAS slot selection and default-layout fallback); `Lookup.NasLayoutClass`; attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosX`

- **Type:** `Integer` (`UINT16`)
- **Description:** Горизонтальное смещение значка слота в пикселях относительно начала панели
  описания предмета. Значок рисуется в позиции X панели плюс это значение минус `6`; значение больше
  `300` подавляет отрисовку слота.
- **Only used with:** NAS.
- **Numeric semantics:** Смещение в пикселях; большие значения сдвигают вправо. Значения `0..300`
  отрисовываются.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (attachment-slot loader; item-description slot drawing); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/usDescPanelPosY`

- **Type:** `Integer` (`UINT16`)
- **Description:** Вертикальное смещение значка слота в пикселях относительно начала панели описания
  предмета. Значок рисуется в позиции Y панели плюс это значение минус `1`; значение больше `300`
  подавляет отрисовку слота.
- **Only used with:** NAS.
- **Numeric semantics:** Смещение в пикселях; большие значения сдвигают вниз. Значения `0..300`
  отрисовываются.
- **Values:** `0..65535`; загрузчик сохраняет это поле как `UINT16`.
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (attachment-slot loader; item-description slot drawing); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fMultiShot`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Помечает слот как один магазин или одну трубу многозарядного гранатомёта. Когда
  список слотов строится для гранатомёта или ракетной установки, каждый такой слот
  расходует одну единицу ёмкости магазина оружия, а после исчерпания ёмкости слоты пропускаются.
- **Only used with:** NAS.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (attachment-slot loader; NAS slot generation for multi-shot launchers); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/fBigSlot`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Помечает слот как большой; панель описания предмета использует для него графику
  большого слота и широкую ширину слота.
- **Only used with:** NAS.
- **Values:** `0` или `1`.
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (attachment-slot loader; item-description slot layout); attachment slot table data.
- **Confidence:** `source`

### `ATTACHMENTSLOTLIST/ATTACHMENTSLOT/ubPocketMapping`

- **Type:** `Integer` (`UINT8`)
- **Description:** Для предметов разгрузки — индекс кармана с отсчётом от 1 в маске доступности
  карманов предмета; слот предлагается только когда этот карман доступен. `0` означает отсутствие
  привязки к карману, поэтому слот не фильтруется по доступности карманов.
- **Only used with:** NAS.
- **Numeric semantics:** `0` — нет; `1..N` выбирает бит `1 << (value - 1)` в маске карманов разгрузки.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8` (рабочий диапазон — число карманов
  разгрузки).
- **Default:** `0`, если поле отсутствует; пустой элемент разбирается как `0`.
- **Source:** game code (attachment-slot loader; NAS LBE slot filtering); attachment slot table data.
- **Confidence:** `source`

## Relations

- `nasAttachmentClass` использует [Lookup.NasAttachmentClass](../Lookup.md#nasattachmentclass).
- `nasLayoutClass` использует [Lookup.NasLayoutClass](../Lookup.md#naslayoutclass).
- На слоты ссылается компоновка NAS, а не таблица сопоставления [Attachments](Attachments.md).

## Localization

`szSlotName` показывается как текст быстрой справки для пустого слота навески. У загрузчика есть
локализованный проход, который копирует только `szSlotName` в существующие записи слотов, поэтому
языковой оверлей меняет имена слотов и ничего больше.

## Versions

В v4 и v5 используются одни и те же пути полей; значения могут различаться.

## Sources

- Данные таблицы слотов навески.
- Таблицы справочников NAS.
- Код игры: загрузчик слотов навески, выбор слотов NAS, отрисовка слотов в описании предмета.
- Описания охватывают значения, которые сохраняет загрузчик, и потребителей, которые их используют;
  каждое поле в этом документе подтверждено загрузчиком.

## Open questions

Начало панели и размеры слотов (`gsInvDescX`/`gsInvDescY`, ширины слотов) — константы интерфейса, а
не данные таблицы.
