---
schema: 1
lang: ru
family: items-pockets
title: "Pockets — ёмкость карманов NIV"
tables:
  - Pockets
table_ids:
  - Items.Pockets
files:
  - Items/Pockets.xml
root: POCKETLIST
record: POCKET
key: pIndex
sources:
  - Game code (pocket loader and inventory/LBE use)
  - Pocket table data
  - Pocket lookup tables
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# Pockets — ёмкость карманов NIV

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`POCKETLIST/POCKET/pIndex`](#pocketlistpocketpindex)
  - [`POCKETLIST/POCKET/pName`](#pocketlistpocketpname)
  - [`POCKETLIST/POCKET/pSilhouette`](#pocketlistpocketpsilhouette)
  - [`POCKETLIST/POCKET/pType`](#pocketlistpocketptype)
  - [`POCKETLIST/POCKET/pRestriction`](#pocketlistpocketprestriction)
  - [`POCKETLIST/POCKET/pVolume`](#pocketlistpocketpvolume)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize0`](#pocketlistpocketitemcapacitypersize0)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize1`](#pocketlistpocketitemcapacitypersize1)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize2`](#pocketlistpocketitemcapacitypersize2)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize3`](#pocketlistpocketitemcapacitypersize3)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize4`](#pocketlistpocketitemcapacitypersize4)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize5`](#pocketlistpocketitemcapacitypersize5)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize6`](#pocketlistpocketitemcapacitypersize6)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize7`](#pocketlistpocketitemcapacitypersize7)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize8`](#pocketlistpocketitemcapacitypersize8)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize9`](#pocketlistpocketitemcapacitypersize9)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize10`](#pocketlistpocketitemcapacitypersize10)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize11`](#pocketlistpocketitemcapacitypersize11)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize12`](#pocketlistpocketitemcapacitypersize12)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize13`](#pocketlistpocketitemcapacitypersize13)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize14`](#pocketlistpocketitemcapacitypersize14)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize15`](#pocketlistpocketitemcapacitypersize15)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize16`](#pocketlistpocketitemcapacitypersize16)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize17`](#pocketlistpocketitemcapacitypersize17)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize18`](#pocketlistpocketitemcapacitypersize18)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize19`](#pocketlistpocketitemcapacitypersize19)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize20`](#pocketlistpocketitemcapacitypersize20)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize21`](#pocketlistpocketitemcapacitypersize21)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize22`](#pocketlistpocketitemcapacitypersize22)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize23`](#pocketlistpocketitemcapacitypersize23)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize24`](#pocketlistpocketitemcapacitypersize24)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize25`](#pocketlistpocketitemcapacitypersize25)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize26`](#pocketlistpocketitemcapacitypersize26)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize27`](#pocketlistpocketitemcapacitypersize27)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize28`](#pocketlistpocketitemcapacitypersize28)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize29`](#pocketlistpocketitemcapacitypersize29)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize30`](#pocketlistpocketitemcapacitypersize30)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize31`](#pocketlistpocketitemcapacitypersize31)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize32`](#pocketlistpocketitemcapacitypersize32)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize33`](#pocketlistpocketitemcapacitypersize33)
  - [`POCKETLIST/POCKET/ItemCapacityPerSize34`](#pocketlistpocketitemcapacitypersize34)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Pockets.xml` определяет типы карманов, используемые новой системой инвентаря (NIV): силуэт, который рисуется для пустого кармана, относительный весовой коэффициент размера, допустимые классы предметов, объём, который карман занимает на своём носителе, и максимальное число предметов каждого размера, которые помещаются. Предметы разгрузки ссылаются на эти строки через `lbePocketIndex*`.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Pockets` | `Items/Pockets.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
POCKETLIST
└── POCKET
    ├── pIndex
    ├── pName
    ├── pSilhouette
    ├── pType
    ├── pRestriction
    ├── pVolume
    ├── ItemCapacityPerSize0
    ├── ItemCapacityPerSize1
    ├── ItemCapacityPerSize2
    ├── ItemCapacityPerSize3
    ├── ItemCapacityPerSize4
    ├── ItemCapacityPerSize5
    ├── ItemCapacityPerSize6
    ├── ItemCapacityPerSize7
    ├── ItemCapacityPerSize8
    ├── ItemCapacityPerSize9
    ├── ItemCapacityPerSize10
    ├── ItemCapacityPerSize11
    ├── ItemCapacityPerSize12
    ├── ItemCapacityPerSize13
    ├── ItemCapacityPerSize14
    ├── ItemCapacityPerSize15
    ├── ItemCapacityPerSize16
    ├── ItemCapacityPerSize17
    ├── ItemCapacityPerSize18
    ├── ItemCapacityPerSize19
    ├── ItemCapacityPerSize20
    ├── ItemCapacityPerSize21
    ├── ItemCapacityPerSize22
    ├── ItemCapacityPerSize23
    ├── ItemCapacityPerSize24
    ├── ItemCapacityPerSize25
    ├── ItemCapacityPerSize26
    ├── ItemCapacityPerSize27
    ├── ItemCapacityPerSize28
    ├── ItemCapacityPerSize29
    ├── ItemCapacityPerSize30
    ├── ItemCapacityPerSize31
    ├── ItemCapacityPerSize32
    ├── ItemCapacityPerSize33
    └── ItemCapacityPerSize34
```

## Fields

### `POCKETLIST/POCKET/pIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** Индекс строки кармана и идентификатор записи XML.
- **Only used with:** Только для NIV.
- **Values:** Неотрицательный индекс, уникальный в пределах файла. На него ссылаются поля `LoadBearingEquipment.lbePocketIndex*`; эти ссылки хранятся как `UINT8`, поэтому адресуемы только первые 255 строк.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `XML_LoadBearingEquipment.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pName`

- **Type:** `String` (`String`)
- **Description:** Отображаемая подпись типа кармана; используется как заголовок кармана в описаниях предметов разгрузки.
- **Only used with:** Только для NIV.
- **Values:** Свободный текст; читается как UTF-8 в буфер шириной 80 символов, более длинный текст усекается.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (`XML_LBEPocket.cpp`, `BobbyRGuns.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pSilhouette`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс подызображения силуэта кармана в `INTERFACE\Silhouettes.sti`; он рисуется для пустого кармана.
- **Only used with:** Только для NIV.
- **Values:** `0..255`; `Lookup.Silhouette` называет те же ID.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`Interface Items.cpp`, `Interface.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pType`

- **Type:** `Integer` (`UINT16`)
- **Description:** Относительный весовой коэффициент размера кармана; используется, когда размер предмета разгрузки пересчитывается по его содержимому. Это не индекс ёмкости по размеру предмета; за него отвечают `ItemCapacityPerSize*`.
- **Only used with:** Только для NIV.
- **Values:** ID из `Lookup.PocketSize`: `0` None, `1` Small, `2` Medium, `3` Large. `1`, `2` и `3` взвешиваются как малый/средний/большой; `0` не имеет веса.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`Items.cpp`, `XML_LBEPocket.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pRestriction`

- **Type:** `Flags` (`UINT32`)
- **Description:** Битовая маска классов предметов, которые принимает карман. Ноль означает, что у кармана нет ограничения по классу.
- **Only used with:** Только для NIV.
- **Values:** Битовая маска значений `Lookup.ItemClass` (биты `IC_*`, хранящиеся в `Items.usItemClass`). `0` означает отсутствие ограничения; ненулевая маска должна пересекаться с классом предмета, иначе карман не принимает ничего.
- **Numeric semantics:** разбирается через `strtoul` с основанием 0, поэтому принимается и десятичный текст, и шестнадцатеричный `0x`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/pVolume`

- **Type:** `Integer` (`UINT8`)
- **Description:** Объём, который карман занимает на своём носителе-разгрузке. Карман можно присоединить, только пока свободный объём носителя покрывает сумму объёмов присоединённых карманов.
- **Only used with:** Только для NIV.
- **Numeric semantics:** относительные единицы объёма; больше означает больший карман, отрицательные значения невозможны.
- **Values:** `0..255`; сумма сравнивается с `LoadBearingEquipment.lbeAvailableVolume`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`Items.cpp`, `BobbyRGuns.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize0`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 0, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize1`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 1, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize2`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 2, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize3`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 3, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize4`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 4, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize5`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 5, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize6`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 6, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize7`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 7, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize8`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 8, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize9`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 9, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize10`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 10, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize11`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 11, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize12`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 12, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize13`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 13, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize14`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 14, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize15`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 15, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize16`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 16, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize17`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 17, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize18`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 18, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize19`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 19, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize20`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 20, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize21`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 21, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize22`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 22, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize23`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 23, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize24`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 24, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize25`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 25, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize26`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 26, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize27`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 27, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize28`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 28, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize29`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 29, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize30`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 30, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize31`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 31, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize32`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 32, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize33`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 33, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

### `POCKETLIST/POCKET/ItemCapacityPerSize34`

- **Type:** `Integer` (`UINT8`)
- **Description:** Максимальное число предметов размера 34, которые помещаются в карман. Ноль означает, что карман не вмещает этот размер.
- **Only used with:** Только для NIV.
- **Values:** `0..255`. Суффикс — это индекс размера предмета; устаревшая запись с точкой (`ItemCapacityPerSize.N`) принимается, а суффиксы выше настроенного максимального размера предмета (`MAX_ITEM_SIZE`, по умолчанию `34`) игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (`XML_LBEPocket.cpp`, `Items.cpp`); table data.
- **Confidence:** `source`

## Relations

- На `pIndex` ссылаются поля `LoadBearingEquipment.lbePocketIndex*`; эти поля имеют тип `UINT8`, поэтому на строку кармана выше 255 сослаться нельзя. Загруженные карманы добавляются в порядке документа, а языковые оверлеи используют `pIndex` как слот строки, поэтому `pIndex` должен совпадать с позицией записи, считая от нуля.
- `pType` использует [Lookup.PocketSize](../Lookup.md#pocketsize); `pSilhouette` — [Lookup.Silhouette](../Lookup.md#silhouette); `pRestriction` — [Lookup.ItemClass](../Lookup.md#itemclass).
- Класс размера предмета, который индексирует `ItemCapacityPerSize*`, — `Items.ItemSize`.
- `pVolume` проверяется против `LoadBearingEquipment.lbeAvailableVolume`, когда карман присоединяется к разгрузке.

## Localization

`pName` — отображаемая подпись, переводимая через языковые оверлеи; числовые поля и коды силуэта и размера не локализуются.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Игровой код: `XML_LBEPocket.cpp` (загрузчик), `Item Types.h` (`POCKETTYPE`), `Items.cpp` (использование ограничения, объёма и ёмкости), `Interface Items.cpp` (отрисовка силуэта).
- Данные таблицы карманов.
- Справочники карманов (`Lookup/PocketSize.xml`, `Lookup/Silhouette.xml`, `Lookup/ItemClass.xml`).
- Типы и пределы полей: члены `POCKETTYPE` и приведения в загрузчике.
- Описания охватывают путь кода, который использует каждое поле; количество строк и наблюдаемые диапазоны данных не используются.

## Open questions

Физическая единица, стоящая за `pVolume`, кодом не определена; важно только её относительное сравнение с `lbeAvailableVolume`. `pType` задаёт относительный вес, поэтому сама кривая ёмкости задаётся по размеру предмета в `ItemCapacityPerSize*`.
