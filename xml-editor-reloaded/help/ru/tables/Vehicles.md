---
schema: 1
lang: ru
family: vehicles
title: "Vehicles — определения транспортных средств и сидений"
tables:
  - Vehicles
table_ids:
  - Vehicles
files:
  - Vehicles.xml
root: VEHICLES
record: VEHICLE
key: uiIndex
sources:
  - Table data
  - Field names, XML comments and observed values
status: reviewed
updated: 2026-11-12
---

# Vehicles — определения транспортных средств и сидений

## Contents

- [Purpose](#purpose)
- [Tables](#tables)
- [Structure](#structure)
- [Fields](#fields)
  - [`VEHICLES/VEHICLE/uiIndex`](#vehiclesvehicleuiindex)
  - [`VEHICLES/VEHICLE/Name`](#vehiclesvehiclename)
  - [`VEHICLES/VEHICLE/LongName`](#vehiclesvehiclelongname)
  - [`VEHICLES/VEHICLE/ShortName`](#vehiclesvehicleshortname)
  - [`VEHICLES/VEHICLE/MvtTypes`](#vehiclesvehiclemvttypes)
  - [`VEHICLES/VEHICLE/StiFaceIcon`](#vehiclesvehiclestifaceicon)
  - [`VEHICLES/VEHICLE/SeatingCapacities`](#vehiclesvehicleseatingcapacities)
  - [`VEHICLES/VEHICLE/EnterVehicleSndID`](#vehiclesvehicleentervehiclesndid)
  - [`VEHICLES/VEHICLE/MoveVehicleSndID`](#vehiclesvehiclemovevehiclesndid)
  - [`VEHICLES/VEHICLE/VehicleArmourType`](#vehiclesvehiclevehiclearmourtype)
  - [`VEHICLES/VEHICLE/Neutral`](#vehiclesvehicleneutral)
  - [`VEHICLES/VEHICLE/VehicleEnabled`](#vehiclesvehiclevehicleenabled)
  - [`VEHICLES/VEHICLE/Pilot`](#vehiclesvehiclepilot)
  - [`VEHICLES/VEHICLE/SEAT/SeatIndex`](#vehiclesvehicleseatseatindex)
  - [`VEHICLES/VEHICLE/SEAT/SeatName`](#vehiclesvehicleseatseatname)
  - [`VEHICLES/VEHICLE/SEAT/Driver`](#vehiclesvehicleseatdriver)
  - [`VEHICLES/VEHICLE/SEAT/Rotation`](#vehiclesvehicleseatrotation)
  - [`VEHICLES/VEHICLE/SEAT/OffsetX`](#vehiclesvehicleseatoffsetx)
  - [`VEHICLES/VEHICLE/SEAT/OffsetY`](#vehiclesvehicleseatoffsety)
  - [`VEHICLES/VEHICLE/SEAT/Compartment`](#vehiclesvehicleseatcompartment)
  - [`VEHICLES/VEHICLE/SEAT/BlockedShots`](#vehiclesvehicleseatblockedshots)
  - [`VEHICLES/VEHICLE/SEAT/BlockedView`](#vehiclesvehicleseatblockedview)
  - [`VEHICLES/VEHICLE/SEAT/Hidden`](#vehiclesvehicleseathidden)
- [Relations](#relations)
- [Localization](#localization)
- [Versions](#versions)
- [Sources](#sources)
- [Open questions](#open-questions)

## Purpose

`Vehicles.xml` определяет названия транспортных средств, ссылки на движение, броню и звуки, а также раскладку сидений, используемую когда наёмники едут. У каждого транспортного средства могут быть повторяющиеся элементы `SEAT`; эта страница описывает общий набор полей сиденья.

## Tables

| Таблица | Файл | Примечания |
|---|---|---|
| `Vehicles` | `Vehicles.xml` | — |

## Structure

```
VEHICLES
└── VEHICLE
    ├── uiIndex
    ├── Name
    ├── LongName
    ├── ShortName
    ├── MvtTypes
    ├── StiFaceIcon
    ├── SeatingCapacities
    ├── EnterVehicleSndID
    ├── MoveVehicleSndID
    ├── VehicleArmourType
    ├── Neutral
    ├── VehicleEnabled
    ├── Pilot
    └── SEAT
        ├── SeatIndex
        ├── SeatName
        ├── Driver
        ├── Rotation
        ├── OffsetX
        ├── OffsetY
        ├── Compartment
        ├── BlockedShots
        ├── BlockedView
        └── Hidden
```

## Fields

### `VEHICLES/VEHICLE/uiIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс транспортного средства. Это ключ массива.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** Обязательно; идентифицирует запись.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/Name`

- **Type:** `String` (`CHAR16`)
- **Description:** Внутреннее и отображаемое название транспортного средства.
- **Values:** Свободный текст или символьный код.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/LongName`

- **Type:** `String` (`CHAR16`)
- **Description:** Длинное отображаемое название, показываемое в подробном интерфейсе; может быть пустым.
- **Values:** Свободный текст или символьный код.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/ShortName`

- **Type:** `String` (`CHAR16`)
- **Description:** Короткое отображаемое название транспортного средства; может быть пустым.
- **Values:** Свободный текст или символьный код.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/MvtTypes`

- **Type:** `Integer` (`INT32`)
- **Description:** Код типа движения транспортного средства. Это enum/битовое поле.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/StiFaceIcon`

- **Type:** `String` (`CHAR8`)
- **Description:** Путь к STI-значку лица для транспортного средства. Это ссылка на файл.
- **Values:** Свободный текст или символьный код.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SeatingCapacities`

- **Type:** `Integer` (`INT32`)
- **Description:** Количество сидений в транспортном средстве.
- **Numeric semantics:** больше = больше сидений; отрицательные значения не имеют смысла.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/EnterVehicleSndID`

- **Type:** `Integer` (`INT32`)
- **Description:** ID звука, воспроизводимого при входе в транспортное средство. Это ссылка на звук.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/MoveVehicleSndID`

- **Type:** `Integer` (`INT32`)
- **Description:** ID звука, воспроизводимого при движении транспортного средства. Это ссылка на звук.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/VehicleArmourType`

- **Type:** `Integer` (`INT16`)
- **Description:** Тип брони или ID предмета, используемый транспортным средством. Это ссылка.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/Neutral`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, транспортное средство нейтрально, а не принадлежит фракции. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/VehicleEnabled`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, транспортное средство включено в игре. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/Pilot`

- **Type:** `Integer` (`INT8`)
- **Description:** ID профиля пилота по умолчанию или `-1`, если пилота нет. Это ссылка.
- **Values:** `-128..127`; `-1` означает отсутствие пилота. Загрузчик сохраняет это поле как `INT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/SeatIndex`

- **Type:** `Integer` (`UINT8`)
- **Description:** Индекс сиденья внутри транспортного средства. Это ключ сиденья.
- **Values:** `0..255`; загрузчик сохраняет это поле как `UINT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/SeatName`

- **Type:** `String` (`CHAR16`)
- **Description:** Отображаемое название сиденья, например `Driver` или `Passenger`.
- **Values:** Свободный текст или символьный код.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/Driver`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, это сиденье водителя. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/Rotation`

- **Type:** `Integer` (`UINT8`)
- **Description:** Код направления или поворота для того, кто занимает сиденье. Это код ориентации.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/OffsetX`

- **Type:** `Integer` (`INT8`)
- **Description:** Горизонтальное смещение позиции сиденья в пикселях.
- **Numeric semantics:** увеличение = вправо, уменьшение = влево; отрицательные значения допустимы.
- **Values:** `-128..127`; загрузчик сохраняет это поле как `INT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/OffsetY`

- **Type:** `Integer` (`INT8`)
- **Description:** Вертикальное смещение позиции сиденья в пикселях.
- **Numeric semantics:** увеличение = вниз, уменьшение = вверх; отрицательные значения допустимы.
- **Values:** `-128..127`; загрузчик сохраняет это поле как `INT8`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/Compartment`

- **Type:** `Integer` (`UINT8`)
- **Description:** Код отсека или области для сиденья. Это enum или ссылка.
- **Values:** Целое число; шкалу см. в описании.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/BlockedShots`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, выстрелы с этого сиденья заблокированы. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/BlockedView`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, обзор занимающего сиденье заблокирован. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

### `VEHICLES/VEHICLE/SEAT/Hidden`

- **Type:** `Boolean` (`BOOLEAN`)
- **Description:** Если установлено, сиденье и занимающий его скрыты в интерфейсе транспортного средства. Это переключатель.
- **Values:** `0` или `1`.
- **Default:** Отсутствует; действует игровое значение по умолчанию.
- **Source:** game code (`XML_Vehicles.cpp`); table data.
- **Confidence:** `source`

## Relations

- ID звуков ссылаются на [Sounds](Sounds/Sounds.md); поля брони и значков ссылаются на [Items](Items/Items.md) и графические данные; `Pilot` ссылается на [MercProfiles](MercProfiles.md).

## Localization

Отображаемые названия и метки локализуются через языковые оверлеи; числовые ID и коды — нет.

## Versions

В v4 и v5 совпадают пути полей; значения могут различаться.

## Sources

- Данные таблицы.
- Имена полей, комментарии XML и наблюдаемые значения.
- Описания выведены из имён и наблюдаемых значений; формула времени выполнения не заявляется.

## Open questions

- Какая именно поверхность интерфейса использует `ShortName` вместо `Name`/`LongName`, не подтверждено; нужна проверка кода и интерфейса.
- Точная формула или сопоставление enum для выведенных полей в данных таблицы не указаны.
