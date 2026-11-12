---
schema: 1
lang: ru
family: mercs-gear
title: "MercStartingGear — стартовые наборы снаряжения"
tables:
  - MercStartingGear
table_ids:
  - Inventory.MercStartingGear
files:
  - Inventory/MercStartingGear.xml
root: MERCGEARLIST
record: MERCGEAR
key: mIndex + mGearKitName
sources:
  - Merc starting gear table data
  - Item metadata
status: reviewed
updated: 2026-11-12
---

# MercStartingGear — стартовые наборы снаряжения

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`MERCGEARLIST/MERCGEAR/mIndex`](#mercgearlistmercgearmindex)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mGearKitName`](#mercgearlistmercgeargearkitmgearkitname)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mPriceMod`](#mercgearlistmercgeargearkitmpricemod)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mAbsolutePrice`](#mercgearlistmercgeargearkitmabsoluteprice)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mHelmet`](#mercgearlistmercgeargearkitmhelmet)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetDrop`](#mercgearlistmercgeargearkitmhelmetdrop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetStatus`](#mercgearlistmercgeargearkitmhelmetstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mVest`](#mercgearlistmercgeargearkitmvest)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mVestDrop`](#mercgearlistmercgeargearkitmvestdrop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mVestStatus`](#mercgearlistmercgeargearkitmveststatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mLeg`](#mercgearlistmercgeargearkitmleg)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mLegDrop`](#mercgearlistmercgeargearkitmlegdrop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mLegStatus`](#mercgearlistmercgeargearkitmlegstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mWeapon`](#mercgearlistmercgeargearkitmweapon)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponDrop`](#mercgearlistmercgeargearkitmweapondrop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponStatus`](#mercgearlistmercgeargearkitmweaponstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig0`](#mercgearlistmercgeargearkitmbig0)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Status`](#mercgearlistmercgeargearkitmbig0status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Quantity`](#mercgearlistmercgeargearkitmbig0quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Drop`](#mercgearlistmercgeargearkitmbig0drop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig1`](#mercgearlistmercgeargearkitmbig1)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Status`](#mercgearlistmercgeargearkitmbig1status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Quantity`](#mercgearlistmercgeargearkitmbig1quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Drop`](#mercgearlistmercgeargearkitmbig1drop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig2`](#mercgearlistmercgeargearkitmbig2)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Status`](#mercgearlistmercgeargearkitmbig2status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Quantity`](#mercgearlistmercgeargearkitmbig2quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Drop`](#mercgearlistmercgeargearkitmbig2drop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig3`](#mercgearlistmercgeargearkitmbig3)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Status`](#mercgearlistmercgeargearkitmbig3status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Quantity`](#mercgearlistmercgeargearkitmbig3quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Drop`](#mercgearlistmercgeargearkitmbig3drop)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0`](#mercgearlistmercgeargearkitmsmall0)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Status`](#mercgearlistmercgeargearkitmsmall0status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Quantity`](#mercgearlistmercgeargearkitmsmall0quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1`](#mercgearlistmercgeargearkitmsmall1)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Status`](#mercgearlistmercgeargearkitmsmall1status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Quantity`](#mercgearlistmercgeargearkitmsmall1quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2`](#mercgearlistmercgeargearkitmsmall2)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Status`](#mercgearlistmercgeargearkitmsmall2status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Quantity`](#mercgearlistmercgeargearkitmsmall2quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3`](#mercgearlistmercgeargearkitmsmall3)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Status`](#mercgearlistmercgeargearkitmsmall3status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Quantity`](#mercgearlistmercgeargearkitmsmall3quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4`](#mercgearlistmercgeargearkitmsmall4)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Status`](#mercgearlistmercgeargearkitmsmall4status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Quantity`](#mercgearlistmercgeargearkitmsmall4quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5`](#mercgearlistmercgeargearkitmsmall5)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Status`](#mercgearlistmercgeargearkitmsmall5status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Quantity`](#mercgearlistmercgeargearkitmsmall5quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6`](#mercgearlistmercgeargearkitmsmall6)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Status`](#mercgearlistmercgeargearkitmsmall6status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Quantity`](#mercgearlistmercgeargearkitmsmall6quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7`](#mercgearlistmercgeargearkitmsmall7)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Status`](#mercgearlistmercgeargearkitmsmall7status)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Quantity`](#mercgearlistmercgeargearkitmsmall7quantity)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lVest`](#mercgearlistmercgeargearkitlvest)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lVestStatus`](#mercgearlistmercgeargearkitlveststatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThigh`](#mercgearlistmercgeargearkitlleftthigh)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThighStatus`](#mercgearlistmercgeargearkitlleftthighstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lRightThigh`](#mercgearlistmercgeargearkitlrightthigh)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lRightThighStatus`](#mercgearlistmercgeargearkitlrightthighstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lCPack`](#mercgearlistmercgeargearkitlcpack)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lCPackStatus`](#mercgearlistmercgeargearkitlcpackstatus)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lBPack`](#mercgearlistmercgeargearkitlbpack)
  - [`MERCGEARLIST/MERCGEAR/GEARKIT/lBPackStatus`](#mercgearlistmercgeargearkitlbpackstatus)
  - [`MERCGEARLIST/MERCGEAR/mName`](#mercgearlistmercgearmname)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`MercStartingGear.xml` задаёт стартовое снаряжение каждого наёмника. У наёмника может быть до пяти именованных блоков `GEARKIT` (`NUM_MERCSTARTINGGEAR_KITS`); каждый блок перечисляет предметы, состояние, количество и флаги выпадения для набора.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `MercStartingGear` | `Inventory/MercStartingGear.xml` | В v4 и v5 совпадают пути полей; значения различаются. |

## Structure

```
MERCGEARLIST
└── MERCGEAR
    ├── mIndex
    ├── GEARKIT
    │   ├── mGearKitName
    │   ├── mPriceMod
    │   ├── mAbsolutePrice
    │   ├── mHelmet
    │   ├── mHelmetDrop
    │   ├── mHelmetStatus
    │   ├── mVest
    │   ├── mVestDrop
    │   ├── mVestStatus
    │   ├── mLeg
    │   ├── mLegDrop
    │   ├── mLegStatus
    │   ├── mWeapon
    │   ├── mWeaponDrop
    │   ├── mWeaponStatus
    │   ├── mBig0
    │   ├── mBig0Status
    │   ├── mBig0Quantity
    │   ├── mBig0Drop
    │   ├── mBig1
    │   ├── mBig1Status
    │   ├── mBig1Quantity
    │   ├── mBig1Drop
    │   ├── mBig2
    │   ├── mBig2Status
    │   ├── mBig2Quantity
    │   ├── mBig2Drop
    │   ├── mBig3
    │   ├── mBig3Status
    │   ├── mBig3Quantity
    │   ├── mBig3Drop
    │   ├── mSmall0
    │   ├── mSmall0Status
    │   ├── mSmall0Quantity
    │   ├── mSmall1
    │   ├── mSmall1Status
    │   ├── mSmall1Quantity
    │   ├── mSmall2
    │   ├── mSmall2Status
    │   ├── mSmall2Quantity
    │   ├── mSmall3
    │   ├── mSmall3Status
    │   ├── mSmall3Quantity
    │   ├── mSmall4
    │   ├── mSmall4Status
    │   ├── mSmall4Quantity
    │   ├── mSmall5
    │   ├── mSmall5Status
    │   ├── mSmall5Quantity
    │   ├── mSmall6
    │   ├── mSmall6Status
    │   ├── mSmall6Quantity
    │   ├── mSmall7
    │   ├── mSmall7Status
    │   ├── mSmall7Quantity
    │   ├── lVest
    │   ├── lVestStatus
    │   ├── lLeftThigh
    │   ├── lLeftThighStatus
    │   ├── lRightThigh
    │   ├── lRightThighStatus
    │   ├── lCPack
    │   ├── lCPackStatus
    │   ├── lBPack
    │   └── lBPackStatus
    └── mName
```

## Fields

### `MERCGEARLIST/MERCGEAR/mIndex`

- **Type:** `Integer` (`UINT16`)
- **Description:** ID профиля наёмника; указывает на [MercProfiles](MercProfiles.md).
- **Values:** ID профиля `0..254`; загрузчик сохраняет его как `UINT16` и использует младшие 8 бит как индекс строки таблицы снаряжения, поэтому значения выше `254` вне диапазона.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mGearKitName`

- **Type:** `String` (`CHAR16`)
- **Description:** Название стартового набора снаряжения. У наёмника может быть до пяти наборов (`NUM_MERCSTARTINGGEAR_KITS`).
- **Values:** Свободный текст, до 79 символов.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mPriceMod`

- **Type:** `Integer` (`INT16`)
- **Description:** Процентная поправка, применяемая к вычисленной цене набора.
- **Numeric semantics:** Множитель равен `(mPriceMod + 100) / 100`: `0` = обычная цена, `50` = ×1.5, `-50` = ×0.5, `-100` = бесплатно, `100` = ×2. Значения вне `-100..200` игнорируются.
- **Values:** `-100..200`; значения вне этого диапазона игнорируются.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mAbsolutePrice`

- **Type:** `Integer` (`INT16`)
- **Description:** Абсолютная цена набора. `-1` оставляет цену вычисляемой по предметам набора; значение в `0..32000` переопределяет цену предметов и `mPriceMod`.
- **Numeric semantics:** `-1` = не задано; `0..32000` = фиксированная сумма денег; значения выше `32000` игнорируются.
- **Values:** `-1..32767`; игра принимает `-1` или `0..32000`.
- **Default:** `-1`, если элемент отсутствует; загрузчик сбрасывает его в `-1` в начале каждого `GEARKIT`.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mHelmet`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в слоте шлема (`HELMETPOS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetDrop`

- **Type:** `Integer` (`INT32`)
- **Description:** Флаг выпадения для слота шлема. `0` помечает предмет как невыпадающий для профилей RPC/NPC (индекс выше 56); ненулевое значение оставляет его выпадающим.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mHelmetStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние шлема (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mVest`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в слоте жилета (`VESTPOS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mVestDrop`

- **Type:** `Integer` (`INT32`)
- **Description:** Флаг выпадения для слота жилета. `0` помечает предмет как невыпадающий для профилей RPC/NPC (индекс выше 56); ненулевое значение оставляет его выпадающим.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mVestStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние жилета (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mLeg`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в слоте штанов (`LEGPOS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mLegDrop`

- **Type:** `Integer` (`INT32`)
- **Description:** Флаг выпадения для слота штанов. `0` помечает предмет как невыпадающий для профилей RPC/NPC (индекс выше 56); ненулевое значение оставляет его выпадающим.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mLegStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние штанов (в процентах; `100` = новые).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mWeapon`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в слоте руки (оружия) (`HANDPOS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponDrop`

- **Type:** `Integer` (`INT32`)
- **Description:** Флаг выпадения для слота руки (оружия). `0` помечает предмет как невыпадающий для профилей RPC/NPC (индекс выше 56); ненулевое значение оставляет его выпадающим.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mWeaponStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние оружия (в процентах; `100` = новое).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в большом кармане 1 (`BIGPOCK1POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в большом кармане 1 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в большом кармане 1.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig0Drop`

- **Type:** `Integer` (`INT32`)
- **Description:** Флаг выпадения для большого кармана 1. `0` помечает предмет как невыпадающий для профилей RPC/NPC (индекс выше 56); ненулевое значение оставляет его выпадающим.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в большом кармане 2 (`BIGPOCK2POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в большом кармане 2 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в большом кармане 2.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig1Drop`

- **Type:** `Integer` (`INT32`)
- **Description:** Флаг выпадения для большого кармана 2. `0` помечает предмет как невыпадающий для профилей RPC/NPC (индекс выше 56); ненулевое значение оставляет его выпадающим.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в большом кармане 3 (`BIGPOCK3POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в большом кармане 3 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в большом кармане 3.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig2Drop`

- **Type:** `Integer` (`INT32`)
- **Description:** Флаг выпадения для большого кармана 3. `0` помечает предмет как невыпадающий для профилей RPC/NPC (индекс выше 56); ненулевое значение оставляет его выпадающим.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в большом кармане 4 (`BIGPOCK4POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в большом кармане 4 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в большом кармане 4.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mBig3Drop`

- **Type:** `Integer` (`INT32`)
- **Description:** Флаг выпадения для большого кармана 4. `0` помечает предмет как невыпадающий для профилей RPC/NPC (индекс выше 56); ненулевое значение оставляет его выпадающим.
- **Values:** `0` или `1`.
- **Default:** `0`, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в малом кармане 1 (`SMALLPOCK1POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в малом кармане 1 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall0Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в малом кармане 1.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в малом кармане 2 (`SMALLPOCK2POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в малом кармане 2 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall1Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в малом кармане 2.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в малом кармане 3 (`SMALLPOCK3POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в малом кармане 3 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall2Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в малом кармане 3.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в малом кармане 4 (`SMALLPOCK4POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в малом кармане 4 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall3Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в малом кармане 4.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в малом кармане 5 (`SMALLPOCK5POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в малом кармане 5 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall4Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в малом кармане 5.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в малом кармане 6 (`SMALLPOCK6POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в малом кармане 6 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall5Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в малом кармане 6.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в малом кармане 7 (`SMALLPOCK7POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в малом кармане 7 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall6Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в малом кармане 7.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета, размещаемого в малом кармане 8 (`SMALLPOCK8POS`).
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Status`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета в малом кармане 8 (в процентах; `100` = новый).
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/mSmall7Quantity`

- **Type:** `Integer` (`INT32`)
- **Description:** Размер стека для предмета в малом кармане 8.
- **Numeric semantics:** Количество; большие значения означают больше предметов. Поля количества есть только у карманов; слоты шлема, жилета, штанов и руки всегда используют `1`.
- **Values:** Количество; `0` используется, когда слот пуст.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lVest`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета в слоте разгрузки жилета NIV (`VESTPOCKPOS`).
- **Only used with:** NIV.
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lVestStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета разгрузки в жилете NIV (в процентах; `100` = новый).
- **Only used with:** NIV.
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата; размер стека разгрузки всегда `1`.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThigh`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета в слоте разгрузки на левом бедре NIV (`LTHIGHPOCKPOS`).
- **Only used with:** NIV.
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lLeftThighStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета разгрузки на левом бедре NIV (в процентах; `100` = новый).
- **Only used with:** NIV.
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата; размер стека разгрузки всегда `1`.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lRightThigh`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета в слоте разгрузки на правом бедре NIV (`RTHIGHPOCKPOS`).
- **Only used with:** NIV.
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lRightThighStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета разгрузки на правом бедре NIV (в процентах; `100` = новый).
- **Only used with:** NIV.
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата; размер стека разгрузки всегда `1`.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lCPack`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета в слоте разгрузки боевого рюкзака NIV (`CPACKPOCKPOS`).
- **Only used with:** NIV.
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lCPackStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета разгрузки в боевом рюкзаке NIV (в процентах; `100` = новый).
- **Only used with:** NIV.
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата; размер стека разгрузки всегда `1`.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lBPack`

- **Type:** `Integer` (`INT32`)
- **Description:** ID предмета в слоте разгрузки рюкзака NIV (`BPACKPOCKPOS`).
- **Only used with:** NIV.
- **Values:** ID предмета; `0` означает пусто.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/GEARKIT/lBPackStatus`

- **Type:** `Integer` (`INT32`)
- **Description:** Состояние предмета разгрузки в рюкзаке NIV (в процентах; `100` = новый).
- **Only used with:** NIV.
- **Numeric semantics:** Проценты; большие значения означают лучшее состояние. Значение передаётся в `objectStatus` предмета при создании солдата; размер стека разгрузки всегда `1`.
- **Values:** `0..100`.
- **Default:** `0`, если элемент отсутствует; пустой элемент означает игровое значение по умолчанию.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

### `MERCGEARLIST/MERCGEAR/mName`

- **Type:** `String` (`CHAR8`)
- **Description:** Имя наёмника, используемое таблицей стартового снаряжения.
- **Values:** Свободный текст, до 79 символов.
- **Default:** пустая строка, если элемент отсутствует.
- **Source:** game code (starting-gear loader); merc starting-gear table data.
- **Confidence:** `source`

## Relations

- `mIndex` указывает на [MercProfiles](MercProfiles.md).
- Колонки предметов `m*` указывают на [Items](Items/Items.md); колонки предметов `l*` указывают на [Items](Items/Items.md) и используются слотами разгрузки NIV.

## Localization

`mName` и `mGearKitName` — отображаемые подписи; названия предметов берутся из `Items`.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы стартового снаряжения наёмников.
- Метаданные предметов.
- Код игры: XML-загрузчик стартового снаряжения, загрузчик профилей, создание солдата, создание предметов, экран членов AIM.

## Open questions

Нет: семантика выпадения, состояния, количества и цены определяется загрузчиком стартового снаряжения, загрузчиком профилей и созданием предметов.
